using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace LiveScoreAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class securityController : ControllerBase
{
    private static readonly IConfiguration _configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .Build();

    [HttpGet("authenticate")]
    public async Task<IActionResult> AddPlayerAsync([FromQuery] string? name)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        if (string.IsNullOrEmpty(name))
        {
            return Unauthorized();
        }
        else
        {
            SymmetricSecurityKey securityKey = new(keyBytes);
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha512);

            var claims = new[]
            {
                new Claim("name", name)
            };

            var token = new JwtSecurityToken(
                audience,
                issuer,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(1),
                credentials
                );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(tokenString);
        }
    }
}
