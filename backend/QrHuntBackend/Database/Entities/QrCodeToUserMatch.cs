using System.ComponentModel.DataAnnotations;

namespace QrHuntBackend.Database.Entities
{
    public class QrCodeToUserMatch
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public Game Game { get; set; }
    }
}
