using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QrHuntBackend.Database.Entities;
using QrHuntBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QrHuntBackend.Controllers {
    [ApiController]
    [Route("Api")]
    public class LoginController : ControllerBase {
       
        public IConfiguration _configuration { get; set; }
        public LoginController(IConfiguration configuration) { 
            _configuration = configuration;
        }

        /// <summary>
        /// Login
        /// </summary>
        [HttpPost("Login")]
        public IActionResult Login(LoginModel model) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Register a user
        /// </summary>
        [HttpPost("Register")]
        public IActionResult Register(RegisterModel model) {
            return NotFound("Not implemented yet");
        }

        private IActionResult _authenticateNoCheck(User fetchedUser, DateTime? _issueDate = null, DateTime? _expiryDate = null) {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var jwtKey = _configuration["Jwt:Key"];
            if (String.IsNullOrWhiteSpace(jwtKey)) return StatusCode(500, new AuthenticationResponseModel() { Message = "JWTSetup Failed" });
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim("username", fetchedUser.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                });
            foreach (var s in fetchedUser.Roles.Split(",").Select(x => x.Trim()))
                subject.AddClaim(new Claim(ClaimTypes.Role, s));


            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = subject,
                Expires = _expiryDate ?? DateTime.UtcNow.AddDays(6),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            var stringToken = tokenHandler.WriteToken(token);

            return Ok(new AuthenticationResponseModel(stringToken, _issueDate ?? DateTime.UtcNow, _expiryDate ?? DateTime.UtcNow.AddDays(6)));
        }
    }
}
