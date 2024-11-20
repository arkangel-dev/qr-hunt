using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrHuntBackend.Database;
using QrHuntBackend.Models;
using QrHuntBackend.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace QrHuntBackend.Controllers {
    [ApiController]
    [Route("Api")]
    [Authorize(Roles = "player")]
    public class GameplayController : ControllerBase {

        DatabaseContext context;
        IHttpContextAccessor accessor;

        /// <summary>
        /// Constructor
        /// </summary>
        public GameplayController(DatabaseContext context, IHttpContextAccessor accessor) {
            this.context = context;
            this.accessor = accessor;
        }

        /// <summary>
        /// Scan a QR Code and add it to the users score
        /// </summary>
        /// <param name="Code">Content of the code</param>
        /// <param name="GameId">ID of the game</param>
        [HttpPost("{GameId}/Scan")]
        [SwaggerResponse(statusCode: 200, type: typeof(StatusMessageModel), description: "Returned when successful")]
        [SwaggerResponse(statusCode: 404, type: typeof(StatusMessageModel), description: "Returned when the game or code is invalid")]
        public IActionResult Scan(
            int GameId,

            [FromQuery]
            string Code
        ) {
            var game = context.Games
                .Include(x => x.Codes)
                .SingleOrDefault(x => x.ID == GameId);
            if (game is null) return NotFound(new StatusMessageModel("The specified game was not found"));

            var code = context.QrCodes.SingleOrDefault(x => x.Content == Code);
            if (code is null) return NotFound(new StatusMessageModel("Invalid Code"));

            var currentUser = accessor.GetUser(context);
            if (context.QrCodesToUserMatch.Any(x =>x.UserId == currentUser.Username && x.CodeId == code.ID)) 
                return BadRequest(new StatusMessageModel("You already scanned this code!"));

            context.QrCodesToUserMatch.Add(new Database.Entities.QrCodeToUserMatch() {
                CodeId = code.ID,
                UserId = currentUser.Username
            });

            context.SaveChanges();
            return Ok(new StatusMessageModel("Code scanned successfully!"));
        }

        /// <summary>
        /// Get the leaderboard
        /// </summary>
        /// <param name="GameId">GameId</param>
        [HttpGet("{GameId}/GetLeaderboard")]

        [SwaggerResponse(statusCode: 200, type: typeof(LeaderboardModel), description: "Returned when successful")]
        public IActionResult GetLeaderboard(int GameId) {
            var leaderboardData = context.QrCodesToUserMatch
                .Where(x => x.Code.Game.ID == GameId)
                .GroupBy(x => x.User.Fullname)
                .Select(x => new LeaderboardEntryModel() {
                      Name = x.Key,
                       Count = x.Count()
                })
                .ToList();
            return Ok(new LeaderboardModel() {
                Leaderboard = leaderboardData
            });
        }

    }
}
