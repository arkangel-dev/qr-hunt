using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QrHuntBackend.Controllers {
    [ApiController]
    [Route("Api")]
    [Authorize(Roles = "player")]
    public class GameplayController : ControllerBase {
       
        /// <summary>
        /// Scan a QR Code and add it to the users score
        /// </summary>
        /// <param name="Code">Content of the code</param>
        /// <param name="GameId">ID of the game</param>
        [HttpGet("{GameId}/Scan")]
        public IActionResult Scan(
            int GameId,

            [FromQuery]
            string Code
        ) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Verify a QR code and see if its valid
        /// </summary>
        /// <param name="Code">Content of the code</param>
        /// <param name="GameId">ID of the game</param>
        [HttpGet("{GameId}/Verify")]
        public IActionResult Verify(
            int GameId,
            [FromQuery]
            string Code
        ) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Get the leaderboard
        /// </summary>
        /// <param name="GameId">GameId</param>
        [HttpGet("{GameId}/GetLeaderboard")]
        public IActionResult GetLeaderboard(int GameId) {
            return NotFound("Not implemented yet");
        }

    }
}
