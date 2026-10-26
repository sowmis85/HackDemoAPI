using HackDemoAPI.Application.DTO;
using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Common;
using HackDemoAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace HackDemoAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        //DI for configuration
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration config, IAuthService authService)
        {
            _config = config;
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login login)
        {
            // Retrieve the values using colon separation for nested keys
            string? username = _config["ApiCredentials:Username"];
            string? password = _config["ApiCredentials:Password"];

            if (login.Username == username && login.Password == password)
            {
                var token = GenerateJwtToken(login.Username);
                return Ok(new { token });
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s: _config["" + "Jwt:Key"]));
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(
     RegisterRequest request,
     CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(
                request,
                cancellationToken);

            return Ok(result);
        }
    }
}
