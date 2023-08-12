using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Skedl.AuthService.Models;
using Skedl.AuthService.Services;
using Skedl.AuthService.Services.UserService;

namespace Skedl.AuthService.Controllers;

public class AuthController : Controller
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    
    public AuthController(DatabaseContext context, IConfiguration configuration, IUserService userService)
    {
        _context = context;
        _configuration = configuration;
        _userService = userService;
    }
    
    public IActionResult Authorization() => View();
    
    
    [HttpGet, Authorize]
    public ActionResult<string> GetMe()
    {
        var userName = _userService.GetMyName();
        return Ok(userName);
    }
    
    [HttpPost]
    public async Task<ActionResult<User>> Register([FromBody] UserRegister request)
    {
        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            Surname = request.Surname,
            LastName = request.LastName,
            Password = request.Password,
        };
        

        string token = CreateToken(user);
        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken, user);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    
        return Ok(new Dictionary<string, string>()
        {
            {"token", token},
            {"refreshToken", refreshToken.Token},
        });
    }
   
    [HttpPost]
    public async Task<ActionResult<string>> Login([FromBody] UserDto request)
    {
        var user = Authenticate(request);

        if (user == null) 
            return BadRequest("Wrong email or password.");
        
        string token = CreateToken(user);
        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken, user);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    
        return Ok(new Dictionary<string, string>()
        {
            {"token", token},
            {"refreshToken", refreshToken.Token},
        });
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
                x => x.Email == userDto.Email 
                && x.Password == userDto.Password);

        return currentUser!;
    }
}