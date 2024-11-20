using QrHuntBackend.Database;
using QrHuntBackend.Database.Entities;
using System.Security.Claims;

namespace QrHuntBackend.Utilities {
    public static class IHttpContextExtensions {
        
        public static User GetUser(this IHttpContextAccessor accessor, DatabaseContext dbcontext) {
            var userNameIdentifier = accessor.HttpContext!.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = dbcontext.Users.First(x => x.Username == userNameIdentifier);
            return user;
        }

        public static string GetUserID(this IHttpContextAccessor accessor) {
            var userNameIdentifier = accessor.HttpContext!.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return userNameIdentifier;
        }
    }
}
