using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HackDemoAPI.MiddleWare;
using HackDemoAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HackDemoAPI.Common;


namespace HackDemoAPI.MiddleWare
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        //DI for configuration
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Retrieve the values using colon separation for nested keys
            string? username = _config["ApiCredentials:Username"];
            string? password = _config["ApiCredentials:Password"];

            if (login.Username == username && login.Password == password)
            {
                var token = GenerateJwtToken(login.Username);
                return Ok(new { token = token });
            }

            return Unauthorized(ValidationConstant.InvalidCrendentials);
        }


        private string GenerateJwtToken(string username)
        {
            // 1. Define the User Claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin") // Optional role claim
            };

            // 2. Fetch keys from configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Create Token Definition
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // Token lifetime
                signingCredentials: creds
            );

            // 4. Serialize into a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
