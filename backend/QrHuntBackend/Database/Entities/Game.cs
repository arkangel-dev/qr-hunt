using System.ComponentModel.DataAnnotations;

namespace QrHuntBackend.Database.Entities
{
    public class Game
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime EndDate { get; set; }
        public int WinningScore { get; set; }
        public User? Winner { get; set; }
        public virtual List<QrCode> Codes { get; set; }
    }
}
