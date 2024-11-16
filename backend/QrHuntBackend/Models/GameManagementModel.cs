namespace QrHuntBackend.Models {
    /// <summary>
    /// Represents the model for managing a game with its details such as name, end date, and winning score.
    /// </summary>
    public class GameManagementModel {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime EndDate { get; set; }
        public int WinningScore { get; set; }
    }

    /// <summary>
    /// Represents a model for assigning roles to a user.
    /// </summary>
    public class SetRolesModel {
        public string Username { get; set; }
        public string Roles { get; set; }
    }

    /// <summary>
    /// Represents the login credentials model, containing the username and password for authentication.
    /// </summary>
    public class LoginModel {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// Represents a model for user registration, including full name, username, and password.
    /// </summary>
    public class RegisterModel {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// Represents a model for the leaderboard, containing a list of leaderboard entries.
    /// </summary>
    public class LeaderboardModel {
        public List<LeaderboardEntryModel> Leaderboard { get; set; }
    }

    /// <summary>
    /// Represents an entry in the leaderboard with a player's name and their count (e.g., score or position).
    /// </summary>
    public class LeaderboardEntryModel {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// Represents a model for search results, containing a list of individual search result entries.
    /// </summary>
    public class UserSearchResultModel {
        public UserSearchResultModel() =>
            Results = Array.Empty<UserSearchResultEntryModel>();

        public UserSearchResultModel(UserSearchResultEntryModel[] results) {
            Results = results;
        }

        public UserSearchResultEntryModel[] Results { get; set; }
    }

    /// <summary>
    /// Represents an entry in the search results with user ID, full name, and username.
    /// </summary>
    public class UserSearchResultEntryModel {
        public string Fullname { get; set; }
        public string Username { get; set; }
    }

    /// <summary>
    /// Message model
    /// </summary>
    public class StatusMessageModel {
        public StatusMessageModel() =>
            Message = string.Empty;
        
        public StatusMessageModel(string message) =>
            Message = message;
        
        public string Message { get; set; }
    }
    
    /// <summary>
    /// Model to upload Qr Codes in bulk
    /// </summary>
    public class UploadBulkQrCodesModel {
        public string[] Codes { get; set; }
    }

    /// <summary>
    /// Model used to fetch QR codes
    /// </summary>
    public class QrSearchResultModel {
        public QrSearchResultEntryModel[] Results;
    }

    /// <summary>
    /// Model used to represent a single line of result for QR search
    /// </summary>
    public class QrSearchResultEntryModel {
        public string Content { get; set; }
        public string Notes { get; set; }
        public int ID { get; set; } 
    }

}
