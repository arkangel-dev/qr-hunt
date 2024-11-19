using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using QrHuntBackend.Database;
using QrHuntBackend.Database.Entities;
using QrHuntBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QrHuntBackend.Controllers {
    [ApiController]
    [Route("Api")]
    [AllowAnonymous]
    public class LoginController : ControllerBase {

        public IConfiguration configuration;
        public DatabaseContext context;
        public LoginController(IConfiguration _configuration, DatabaseContext _context) {
            configuration = _configuration;
            context = _context;
        }

        /// <summary>
        /// Login
        /// </summary>
        [HttpPost("Login")]
        public IActionResult Login(LoginModel model) {
            var user = context
                .Users
                .Find(model.Username);

            if (user is null) return BadRequest(new StatusMessageModel("The username or password is invalid"));
            if (!user.Password.SequenceEqual(Encoding.UTF8.GetBytes(model.Password))) return BadRequest(new StatusMessageModel("The username or password is invalid"));

            return GenerateToken(user);
        }

        /// <summary>
        /// Register a user
        /// </summary>
        [HttpPost("Register")]
        public IActionResult Register(RegisterModel model) {
            var user = new User() {
                Fullname = model.Username,
                Password = Encoding.UTF8.GetBytes(model.Password),
                Username = model.Username,
                Salt = []
            };

            if (context.Users.Any(x => x.Username == model.Username))
                return BadRequest(new StatusMessageModel("A user with this name already exists!"));

            context.Users.Add(user);
            user.Roles = context.Users.Any() ? "player" : "admin,moderator,player";
            context.SaveChanges();
            return GenerateToken(user);
        }

        /// <summary>
        /// Generate a token
        /// </summary>
        /// <param name="fetchedUser">The fetched user</param>
        private IActionResult GenerateToken(User fetchedUser) {
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var jwtKey = configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey)) return StatusCode(500, new AuthenticationResponseModel() { Message = "JWTSetup Failed" });
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, fetchedUser.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                });
            foreach (var s in fetchedUser.Roles.Split(",").Select(x => x.Trim()))
                subject.AddClaim(new Claim(ClaimTypes.Role, s));

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = subject,
                Expires = DateTime.UtcNow.AddDays(6),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    key: new SymmetricSecurityKey(key),
                    algorithm: SecurityAlgorithms.HmacSha512Signature
                )
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            var stringToken = tokenHandler.WriteToken(token);

            return Ok(new AuthenticationResponseModel(stringToken, DateTime.UtcNow, DateTime.UtcNow.AddDays(6)));
        }
    }
}
