using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    //[HttpPost("login")]
    //public IActionResult Login([FromBody] LoginModel model)
    //{
    //    // Add rate limiting
    //    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
    //    if (string.IsNullOrEmpty(ipAddress))
    //    {
    //        return BadRequest("Invalid request");
    //    }

    //    var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
    //    if (user == null)
    //    {
    //        Thread.Sleep(Random.Shared.Next(300, 700)); 
    //        return Unauthorized(new { message = "Authentication failed" });
    //    }

    //    // TODO: Implement proper password hashing with salt
    //    if (user.Password != model.Password)
    //    {
    //        Thread.Sleep(Random.Shared.Next(300, 700));
    //        return Unauthorized(new { message = "Authentication failed" });
    //    }

    //    var token = GenerateJwtToken(user);
    //    return Ok(new { token });
    //}

    [HttpPost("login")]
    public IActionResult Login()
    {
        // Récupérer le premier utilisateur directement
        var user = _context.Users.FirstOrDefault();

        if (user == null)
        {
            return Unauthorized(new { message = "No users found" });
        }

        // Générer directement le JWT
        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }


    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = creds,
            NotBefore = DateTime.UtcNow
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }
}

public class LoginModel
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
