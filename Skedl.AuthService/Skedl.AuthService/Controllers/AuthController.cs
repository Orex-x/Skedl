using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Skedl.AuthService.Models;
using Skedl.AuthService.Services;
using Skedl.AuthService.Services.CodeGeneration;
using Skedl.AuthService.Services.MailService;

namespace Skedl.AuthService.Controllers;

public class AuthController : Controller
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMailService _mailService;
    private readonly ICodeGenerator _codeGenerator;

    public AuthController(DatabaseContext context, IConfiguration configuration, IMailService mailService, ICodeGenerator codeGenerator)
    {
        _context = context;
        _configuration = configuration;
        _mailService = mailService;
        _codeGenerator = codeGenerator;
    }


    [HttpGet]
    public async Task<ActionResult<string>> SendCode(string to)
    {
        var userCodes = _context.UserCodes.Where(x => x.Email == to);
        _context.UserCodes.RemoveRange(userCodes);
        await _context.SaveChangesAsync();
        
        var currentUser = _context.Users.FirstOrDefault(x => x.Email == to);
        
        if(currentUser != null)
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
        
        
        var user = new User
        {
            Email = model.Email,
            Name = model.Name,
            Password = model.Password,
            Login = model.Login
        };
        
        string token = CreateToken(user);
        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken, user);

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
        var user = Authenticate(request);

        if (user == null) 
            return BadRequest("Wrong email or password.");
        
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
    
    private User? Authenticate(UserDto userDto)
    {
        var currentUser =
            _context.Users.FirstOrDefault(
                x => (x.Email == userDto.Email || x.Login == userDto.Email) 
                     && x.Password == userDto.Password);

        return currentUser!;
    }
}