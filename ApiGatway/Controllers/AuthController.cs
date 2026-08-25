using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiGatway.Controllers
{
    [Route("MainGateway/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private string CreateToken(string name, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role)
            };

            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YtIkTOf7FwmebsKWbVtHvEftfJlUTqyfRGxuzf6Ryz1"));

            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MyIssure",
                audience: "MyAudience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("admin")]
        public IActionResult Admin(string username, string password)
        {
            // Simulate user authentication
            if (username == "admin" && password == "password")
            {
                var token = CreateToken(username, "admin");
                return Ok(token);
            }

            return Unauthorized();
        }

        [HttpPost("user")]
        public IActionResult User(string username, string password)
        {
            // Simulate user authentication
            if (username == "user" && password == "password")
            {
                var token = CreateToken(username, "user");
                return Ok(token);
            }

            return Unauthorized();
        }
    }
}
