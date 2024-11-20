using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrHuntBackend.Database;
using QrHuntBackend.Database.Entities;
using QrHuntBackend.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;

namespace QrHuntBackend.Controllers {

    /// <summary>
    /// Controller to manage games and the QR codes for each games
    /// </summary>
    [ApiController]
    [Route("Api/Games")]
    [Authorize(Roles = "moderator")]
    public class GameManagementController : ControllerBase {

        DatabaseContext context;
        /// <summary>
        /// Constructor
        /// </summary>
        public GameManagementController(DatabaseContext context) {
            this.context = context;
        }

        /// <summary>
        /// Get the available games registerd in the system. Supports pagination
        /// </summary>
        /// <param name="Take">Amount to take</param>
        /// <param name="Skip">Amount of skip/offset</param>
        [HttpGet]
        [SwaggerResponse(statusCode: 200, type: typeof(GameListModel), description: "Returned when successful")]
        public IActionResult GetGames(
            [FromQuery]
            int Take = 0,

            [FromQuery]
            int Skip = 0
        ) {
            var games = context.Games
                .Take(Take)
                .Skip(Skip)
                .Select(x =>
                    new GameListEntryModel() {
                        EndDate = x.EndDate,
                        ID = x.ID,
                        Name = x.Name,
                        WinningScore = x.WinningScore
                    }
                )
                .ToList();
            return Ok(new GameListModel() { Games = games });
        }

        /// <summary>
        /// Get a single game info
        /// </summary>
        /// <param name="gameId">Game ID</param>
        [HttpGet("{gameId}")]
        [SwaggerResponse(statusCode: 200, type: typeof(GameListEntryModel), description: "Returned when successful")]
        [SwaggerResponse(statusCode: 404, type: typeof(StatusMessageModel), description: "Returned when no results found")]
        public IActionResult GetGame(int gameId) {
            var game = context.Games
                .Select(x => new GameListEntryModel() {
                    ID = x.ID,
                    EndDate = x.EndDate,
                    Name = x.Name,
                    WinningScore = x.WinningScore
                })
                .SingleOrDefault(x => x.ID == gameId);
            if (game is null) return NotFound(new StatusMessageModel("Game not found"));
            return Ok(game);
        }

        /// <summary>
        /// Create a new game. Will require a name that is unique
        /// </summary>
        /// <param name="model">Model describing the parameters of the game</param>
        [HttpPost]
        [SwaggerResponse(statusCode: 200, type: typeof(StatusMessageModel), description: "Returned when successful")]
        [SwaggerResponse(statusCode: 400, type: typeof(StatusMessageModel), description: "Returned when a game with the same name already exists")]
        public IActionResult Create(GameManagementModel model) {
            if (context.Games.Any(x => x.Name == model.Name)) return BadRequest(new StatusMessageModel("A game with that name already exists"));
            context.Games.Add(new Database.Entities.Game() {
                EndDate = DateTime.Now,
                Name = model.Name,
                WinningScore = model.WinningScore
            });
            context.SaveChanges();
            return Ok(new StatusMessageModel("Operation completed successfully"));
        }

        /// <summary>
        /// Modify an existing game
        /// </summary>
        /// <param name="GameId">ID of the existing game</param>
        /// <param name="model">Model describing the new parameters of the game</param>
        [HttpPatch("{GameId}")]
        [SwaggerResponse(statusCode: 200, type: typeof(StatusMessageModel), description: "Returned when successful")]
        [SwaggerResponse(statusCode: 404, type: typeof(StatusMessageModel), description: "Returned when a game with the name could not be found")]
        public IActionResult Modify(int GameId, GameManagementModel model) {
            var entity = context.Games.SingleOrDefault(x => x.ID == model.ID);
            if (entity is null) return NotFound(new StatusMessageModel("The game was not found"));
            entity.WinningScore = model.WinningScore;
            entity.EndDate = model.EndDate;
            entity.Name = model.Name;
            context.SaveChanges();
            return Ok(new StatusMessageModel("Operation completed successfully"));
        }

        /// <summary>
        /// Delete an existing game
        /// </summary>
        /// <param name="GameId">Game to delete</param>
        [HttpDelete("{GameId}")]
        [SwaggerResponse(statusCode: 200, type: typeof(StatusMessageModel), description: "Returned when successful")]
        public IActionResult Delete(int GameId) {
            var count = context.Games
                .Where(x => x.ID == GameId)
                .ExecuteDelete();

            return (count > 0) ?
                Ok(new StatusMessageModel("Operation completed successfully")) :
                NotFound(new StatusMessageModel("No game was found with that ID"));
        }

        /// <summary>
        /// Get a list of all the QR codes in a game
        /// </summary>
        /// <param name="Take">How many QR codes to get</param>
        /// <param name="Skip">How much to offset</param>
        /// <param name="GameId">Game ID</param>
        [HttpGet("{GameId}/QrCodes")]
        [SwaggerResponse(statusCode: 200, type: typeof(QrSearchResultModel), description: "Returned when successful")]
        [SwaggerResponse(statusCode: 404, type: typeof(StatusMessageModel), description: "Returned the specified game doesn't exist")]
        public IActionResult GetQrList(
            int GameId,

            [FromQuery]
            int Take = 0,

            [FromQuery]
            int Skip = 0
        ) {
            var game = context.Games
                .Include(x => x.Codes)
                .SingleOrDefault(x => x.ID == GameId);
            if (game is null) return NotFound(new StatusMessageModel("Game with the specified ID was not found"));

            var results = game.Codes
                .Skip(Skip)
                .Take(Take)
                .Select(x => new QrSearchResultEntryModel() {
                    Content = x.Content,
                    ID = x.ID,
                    Notes = x.Notes,
                });
            return Ok(new QrSearchResultModel() { Results = results.ToArray() });
        }

        /// <summary>
        /// Upload a single QR Code
        /// </summary>
        /// <param name="GameId">ID of the game</param>
        /// <param name="Content">Content of the game</param>
        /// <param name="Notes">Notes for the game</param>
        [HttpPost("{GameId}/QrCodes/Upload")]
        [SwaggerResponse(statusCode: 200, type: typeof(StatusMessageModel), description: "Returned when successful")]
        [SwaggerResponse(statusCode: 400, type: typeof(StatusMessageModel), description: "Returned when a the code provided already exists")]
        [SwaggerResponse(statusCode: 404, type: typeof(StatusMessageModel), description: "Returned when a the game ID provided was not found")]
        public IActionResult Upload(
            int GameId,

            [FromQuery]
            string Content,

            [FromQuery]
            string Notes = ""
        ) {
            var game = context.Games
                .Include(x => x.Codes)
                .SingleOrDefault(x => x.ID == GameId);
            if (game is null) return NotFound(new StatusMessageModel("Game not found"));
            if (game.Codes.Any(x => x.Content == Content)) return BadRequest(new StatusMessageModel("This code already exists in this game"));
            game.Codes.Add(new Database.Entities.QrCode() {
                Content = Content,
                Notes = Notes,
            });
            context.SaveChanges();
            return Ok(new StatusMessageModel("Operation completed successfully"));
        }

        /// <summary>
        /// Upload multiple QR codes to a game
        /// </summary>
        /// <param name="GameId">ID of the game to upload to</param>
        /// <param name="model">List of QR codes</param>
        [HttpPost("{GameId}/QrCodes/UploadBulk")]
        [SwaggerResponse(statusCode: 200, type: typeof(StatusMessageModel), description: "Returned when successful")]
        [SwaggerResponse(statusCode: 400, type: typeof(StatusMessageModel), description: "Returned a code in the payload already exists in the game or if one of them is an empty string")]
        [SwaggerResponse(statusCode: 404, type: typeof(StatusMessageModel), description: "Returned the specified game doesn't exist")]
        public IActionResult UploadBulk(int GameId, UploadBulkQrCodesModel model) {
            var game = context.Games
                .Include(x => x.ID == GameId)
                .SingleOrDefault(x => x.ID == GameId);

            if (game is null) return NotFound(new StatusMessageModel("The specified game was not found"));

            var invalidCodes = new List<string>();
            model.Codes
                .Where(game.Codes.Select(x => x.Content).Contains)
                .ToList()
                .ForEach(invalidCodes.Add);

            if (invalidCodes.Any(String.IsNullOrWhiteSpace)) return BadRequest(new StatusMessageModel("One or more code is an empty string"));
            if (invalidCodes.Any()) return BadRequest(new StatusMessageModel($"The following codes already exist in the game and cannot be added to the game : {String.Join(", ", invalidCodes)}"));

            game.Codes.AddRange(model.Codes.Select(x => new QrCode() { Content = x }));
            context.SaveChanges();
            return Ok(new StatusMessageModel("Operation completed successfully"));
        }

        /// <summary>
        /// Delete a single QR Code
        /// </summary>
        /// <param name="GameId">Game ID</param>
        /// <param name="QrId">QR Code ID</param>
        [HttpDelete("{GameId}/QrCodes/{QrId}")]
        [SwaggerResponse(statusCode: 200, type: typeof(StatusMessageModel), description: "Returned when the operation is successful")]
        [SwaggerResponse(statusCode: 404, type: typeof(StatusMessageModel), description: "Returned the specified game doesn't exist")]
        public IActionResult UploadBulk(int GameId, int QrId) {
            var game = context.Games
                .Include(x => x.Codes)
                .SingleOrDefault(x => x.ID == GameId);
            if (game is null) return NotFound(new StatusMessageModel("Game with the specified ID was not found"));

            var code = game.Codes.SingleOrDefault(x => x.ID == QrId);
            if (code is null) return NotFound(new StatusMessageModel("Qr code was not found"));

            game.Codes.Remove(code);
            context.SaveChanges();
            return Ok(new StatusMessageModel("Operation completed successfully"));
        }

        /// <summary>
        /// Delete all QR Codes in a game
        /// </summary>
        /// <param name="GameId">Game ID</param>
        [HttpDelete("{GameId}/QrCodes")]
        [SwaggerResponse(statusCode: 200, type: typeof(StatusMessageModel), description: "Returned when the operation is successful")]
        [SwaggerResponse(statusCode: 404, type: typeof(StatusMessageModel), description: "Returned when no QR codes were found")]
        public IActionResult DeleteAll(int GameId) {
            var count = context.QrCodes.Where(x => x.Game.ID == GameId).ExecuteDelete();
            return (count > 0) ?
                Ok(new StatusMessageModel("Operation completed successfully")) :
                NotFound(new StatusMessageModel("No QR codes were deleted because none were found"));
        }
    }
}
