using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QrHuntBackend.Database;
using QrHuntBackend.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace QrHuntBackend.Controllers {
    /// <summary>
    /// This controller had the endpoints to manage the user roles
    /// </summary>
    [ApiController]
    [Route("Api/Admin")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase {

        DatabaseContext context;

        /// <summary>
        /// Constructor
        /// </summary>
        public AdminController(DatabaseContext context) {
            this.context = context;
        }

        /// <summary>
        /// Set roles for a user
        /// </summary>
        /// <param name="model">Model describing the username and the role</param>
        [HttpPatch("SetRoles")]
        [SwaggerResponse(statusCode: 404, type: typeof(StatusMessageModel), description: "Returned of the user was not found")]
        [SwaggerResponse(statusCode: 200, type: typeof(StatusMessageModel), description: "Returned when successful")]
        public IActionResult Scan(SetRolesModel model) {
            var user = context.Users.Find(model.Username);
            if (user is null) return NotFound(new StatusMessageModel($"User '{model.Username}' was not found"));
            user.Roles = model.Roles;
            context.SaveChanges();
            return Ok(new StatusMessageModel("Changes Made Successfully"));
        }

        /// <summary>
        /// Search for a user
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("Search")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserSearchResultModel), description: "Returned results. By Default")]
        public IActionResult SearchUser(
            [FromQuery]
            string query
        ) {
            var users = context.Users
                .Where(x =>
                    x.Username.Contains(query) ||
                    x.Fullname.Contains(query)
                )
                .Select(x => new UserSearchResultEntryModel() {
                    Username = x.Username,
                    Fullname = x.Fullname
                })
                .Take(5)
                .ToArray();

            return Ok(new UserSearchResultModel() {  Results = users });
        } 
    }
}
