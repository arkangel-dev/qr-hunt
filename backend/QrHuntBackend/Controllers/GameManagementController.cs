using Microsoft.AspNetCore.Mvc;
using QrHuntBackend.Models;

namespace QrHuntBackend.Controllers {
    [ApiController]
    [Route("Api/Games")]
    public class GameManagementController : ControllerBase {
        
        /// <summary>
        /// Create a new game
        /// </summary>
        /// <param name="model">Model describing the parameters of the game</param>
        [HttpPut("Create")]
        public IActionResult Create(GameManagementModel model) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Modify an existing game
        /// </summary>
        /// <param name="GameId">ID of the existing game</param>
        /// <param name="model">Model describing the new parameters of the game</param>
        [HttpPatch("Modify/{GameId}")]
        public IActionResult Modify(int GameId, GameManagementModel model) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Delete an existing game
        /// </summary>
        /// <param name="GameId">Game to delete</param>
        [HttpDelete("Delete/{GameId}")]
        public IActionResult Delete(int GameId) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Upload a single QR Code
        /// </summary>
        /// <param name="GameId">ID of the game</param>
        /// <param name="Content">Content of the game</param>
        [HttpPut("{GameId}/QrCode/Upload")]
        public IActionResult Upload(
            int GameId,

            [FromQuery]
            string Content
        ) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Upload multiple QR codes to a game
        /// </summary>
        /// <param name="List">List of QR codes</param>
        [HttpPost("{GameId}/QrCode/UploadBulk")]
        public IActionResult UploadBulk(int GameId, string[] List) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Get a list of all the QR codes in a game
        /// </summary>
        /// <param name="Take">How many QR codes to get</param>
        /// <param name="Skip">How much to offset</param>
        [HttpGet("{GameId}/QrCode/GetQrList")]
        public IActionResult GetQrList(

            [FromQuery]
            int Take = 0,

            [FromQuery]
            int Skip = 0
        ) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Delete a single QR Code
        /// </summary>
        /// <param name="GameId">Game ID</param>
        /// <param name="QrId">QR Code ID</param>
        [HttpDelete("{GameId}/QrCode/Delete/{QrId}")]
        public IActionResult UploadBulk(int GameId, int QrId) {
            return NotFound("Not implemented yet");
        }

        /// <summary>
        /// Delete all QR Codes in a game
        /// </summary>
        /// <param name="GameId">Game ID</param>
        [HttpDelete("{GameId}/QrCode/DeleteAll")]
        public IActionResult DeleteAll(int GameId) {
            return NotFound("Not implemented yet");
        }
    }
}
