using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Skedl.AuthService.Models;
using Skedl.AuthService.Services;
using Skedl.AuthService.Services.CodeGeneration;
using Skedl.AuthService.Services.FileService;
using Skedl.AuthService.Services.MailService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Skedl.AuthService.Controllers;

public class AuthController : Controller
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMailService _mailService;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IFileService _fileService;

    public AuthController(
        DatabaseContext context, 
        IConfiguration configuration, 
        IMailService mailService, 
        ICodeGenerator codeGenerator,
        IFileService fileService)
    {
        _context = context;
        _configuration = configuration;
        _mailService = mailService;
        _codeGenerator = codeGenerator;
        _fileService = fileService;
    }


    [HttpGet]
    public async Task<ActionResult<string>> SendCode(string to)
    {
        var userCodes = _context.UserCodes.Where(x => x.Email == to);
        _context.UserCodes.RemoveRange(userCodes);
        await _context.SaveChangesAsync();

        var currentUser = _context.Users.FirstOrDefault(x => x.Email == to);

        if (currentUser != null)
            return BadRequest("Почта занята");

        var code = _codeGenerator.Generation(5);

        await _mailService.SendMessage(to, "Подтверждения пароля", $"Ваш код подтверждения {code}");

        await _context.UserCodes.AddAsync(new UserCode()
        {
            Code = code,
            Email = to
        });

        await _context.SaveChangesAsync();

        return Ok("Code sent successfully");
    }


    [HttpGet]
    public async Task<ActionResult<string>> SendCodeForRecoverPassword(string emailOrLogin)
    {
        var userCodes = _context.UserCodes.Where(x => x.Email == emailOrLogin);
        _context.UserCodes.RemoveRange(userCodes);
        await _context.SaveChangesAsync();

        var currentUser = _context.Users.FirstOrDefault(x => x.Email == emailOrLogin || x.Login == emailOrLogin);

        if (currentUser == null)
            return BadRequest("Пользователь не найден");

        var code = _codeGenerator.Generation(5);

        await _mailService.SendMessage(currentUser.Email, "Восстановление пароля", $"Ваш код подтверждения {code}");

        await _context.UserCodes.AddAsync(new UserCode()
        {
            Code = code,
            Email = emailOrLogin
        });

        await _context.SaveChangesAsync();

        return Ok(currentUser.Password);
    }

    [HttpPost]
    public async Task<ActionResult> RecoverPassword([FromBody] RecoverPasswordModel model) 
    {
        var currentUser = _context.Users.FirstOrDefault(x => 
        (x.Email == model.EmailOrLogin || x.Login == model.EmailOrLogin) && x.Password == model.OldPassword);

        if (currentUser == null)
            return BadRequest("Пользователь не найден");

        var hasher = new PasswordHasher<User>();
        currentUser.Password = hasher.HashPassword(currentUser, model.NewPassword);
        _context.Users.Update(currentUser);
        await _context.SaveChangesAsync();

        return Ok();
    }


    [HttpGet]
    public async Task<ActionResult> VerifyCode(string to, string code)
    {

        var userCode = await _context.UserCodes.FirstOrDefaultAsync(x => x.Email == to);
        
        if (userCode == null)
        {
            return BadRequest("Invalid user");
        }

        if (userCode.Code == code)
        {
            _context.UserCodes.Remove(userCode);
            await _context.SaveChangesAsync();
            
            return Ok("Code verified successfully");
        }
        
        return BadRequest("Invalid code");
    }

    [HttpPost]
    public async Task<ActionResult> Register([FromBody] RegisterModel model)
    {
        var currentUser = _context.Users.FirstOrDefault(x => x.Email == model.Email);
        
        if(currentUser != null)
            return BadRequest("the login is already in use");

        var hasher = new PasswordHasher<User>();

        var user = new User
        {
            Email = model.Email,
            Name = model.Name,
            Login = model.Login
        };

        user.Password = hasher.HashPassword(user, model.Password);

        string token = CreateToken(user);
        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken, user);

        if (model.Avatar != null)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + model.AvatarName;
            string filePath = Path.Combine("Files/Images", uniqueFileName);
            bool ok = _fileService.ByteArrayToFile(filePath, model.Avatar);

            if (ok)
            {
                user.AvatarName = $"https://skedl.ru/multimedia/photos/{uniqueFileName}";
            }
        }

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    
        return Ok(new Dictionary<string, object>()
        {
            {"user", user},
            {"token", token}
        });
    }
   
    [HttpPost]
    public async Task<ActionResult> Login([FromBody] UserDto request)
    {
        var user = await Authenticate(request);

        if (user == null) 
            return BadRequest("Wrong email or newPassword.");
        
        string token = CreateToken(user);
        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken, user);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok(new Dictionary<string, object>()
        {
            {"user", user},
            {"token", token}
        });
    }

    [HttpPost]
    public async Task<ActionResult> UpdateUser([FromBody] User user)
    {
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult<string>> RefreshToken([FromBody] RefreshTokenModel refreshToken)
    {
        var user = _context.Users.FirstOrDefault(x => x.RefreshToken == refreshToken.Token);
        
        if (user == null)
            return Unauthorized("Invalid Refresh Token.");

        if(user.TokenExpires < DateTime.Now)
            return Unauthorized("Token expired.");
        
        string token = CreateToken(user);

        return Ok(token);
    }
    
    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new("email", user.Email),
            new("name", user.Name),
            new("role", "User"),
            new("type", "access"),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(3),
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
    
    private void SetRefreshToken(RefreshToken newRefreshToken, User user)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires
        };
        Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

        user.RefreshToken = newRefreshToken.Token;
        user.TokenCreated = newRefreshToken.Created;
        user.TokenExpires = newRefreshToken.Expires;
    }
    
    
    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(10),
            Created = DateTime.Now
        };

        return refreshToken;
    }
    
    private async Task<User?> Authenticate(UserDto userDto)
    {
        var hasher = new PasswordHasher<User>();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => (x.Email == userDto.Email || x.Login == userDto.Email));

        if (user != null)
        {
            var s = hasher.VerifyHashedPassword(user, user.Password, userDto.Password);
            if (s == PasswordVerificationResult.Success)
            {
                return user;
            }
        }
        return null;
    }
}