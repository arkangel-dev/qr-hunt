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
        [SwaggerResponse(statusCode: 200, type: typeof(ScanResult), description: "Returned always")]
        public IActionResult Scan(
            int GameId,

            [FromQuery]
            string Code
        ) {
            var authenticated = User.Identity?.IsAuthenticated ?? false;

            var game = context.Games
                .Include(x => x.Codes)
                .Include(x => x.Winner)
                .SingleOrDefault(x => x.ID == GameId);
            if (game is null) return Ok(new ScanResult() {
                IsAuthenticated = authenticated,
                StatusCode = "INVALID_CODE"
            });



            if (game.EndDate <= DateTime.Now)
                return Ok(new ScanResult() {
                    IsAuthenticated = true,
                    StatusCode = "TIMES_UP"
                });


            var code = context.QrCodes.SingleOrDefault(x => x.Content == Code && x.Game.ID == GameId);
            if (code is null) return Ok(new ScanResult() {
                IsAuthenticated = authenticated,
                StatusCode = "INVALID_CODE"
            });

            if (authenticated) {
                var currentUser = accessor.GetUser(context);
                if (context.QrCodesToUserMatch.Any(x => x.UserId == currentUser.Username && x.CodeId == code.ID))
                    return Ok(new ScanResult() {
                        IsAuthenticated = authenticated,
                        StatusCode = "CODE_ALREADY_SCANNED"
                    });

                context.QrCodesToUserMatch.Add(new Database.Entities.QrCodeToUserMatch() {
                    CodeId = code.ID,
                    UserId = currentUser.Username
                });

                context.SaveChanges();
                if (game.Winner is null && context.QrCodesToUserMatch
                    .Where(x => x.UserId == currentUser.Username && x.Code.Game.ID == GameId)
                    .Count() >= game.WinningScore) {

                    game.Winner = currentUser;
                    context.SaveChanges();
                    return Ok(new ScanResult() {
                        IsAuthenticated = true,
                        StatusCode = "YOU_WON"
                    });
                }
            }
            return Ok(new ScanResult() {
                IsAuthenticated = authenticated,
                StatusCode = "CODE_FOUND"
            });
        }

        /// <summary>
        /// Get the leaderboard
        /// </summary>
        /// <param name="GameId">GameId</param>
        [HttpGet("{GameId}/GetLeaderboard")]

        [SwaggerResponse(statusCode: 200, type: typeof(LeaderboardModel), description: "Returned when successful")]
        public IActionResult GetLeaderboard(int GameId) {
            var game = context.Games.Find(GameId);
            if (game is null)
                return NotFound(new StatusMessageModel("No such game was found"));

            var leaderboardData = context.QrCodesToUserMatch
                .Where(x => x.Code.Game.ID == GameId)
                .GroupBy(x => new { Name = x.User.Fullname, ID = x.User.Username })
                .Select(x => new LeaderboardEntryModel() {
                    Name = x.Key.Name,
                    Count = x.Count(),
                    LastEntry = x.OrderBy(x => x.EntryDate).Last().EntryDate
                })
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.LastEntry)
                .ToList();

            var winner = leaderboardData
                .Where(x => x.Count >= game.WinningScore)
                .FirstOrDefault();
            if (winner is not null)
                winner.HasWon = true; 

            
            return Ok(new LeaderboardModel() {
                Leaderboard = leaderboardData,
                TimeRemaining = (game.EndDate - DateTime.Now).TotalSeconds
            });
        }

    }
}
