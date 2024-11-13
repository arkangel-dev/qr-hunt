using Microsoft.AspNetCore.Mvc;
using QrHuntBackend.Models;

namespace QrHuntBackend.Controllers {
    [ApiController]
    [Route("Api/Admin")]
    public class AdminController : ControllerBase {
        /// <summary>
        /// Set roles for a user
        /// </summary>
        /// <param name="model">Model describing the username and the role</param>
        [HttpPost("SetRoles")]
        public IActionResult Scan(SetRolesModel model) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Search for a user
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SearchUser(string query) {
            return NotFound("Not implemented yet");
        }
    }
}
