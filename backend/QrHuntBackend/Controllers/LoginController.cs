using Microsoft.AspNetCore.Mvc;
using QrHuntBackend.Models;

namespace QrHuntBackend.Controllers {
    [ApiController]
    [Route("Api")]
    public class LoginController : ControllerBase {
       

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
    }
}
