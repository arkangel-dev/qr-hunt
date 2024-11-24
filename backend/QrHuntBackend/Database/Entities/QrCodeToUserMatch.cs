using System.ComponentModel.DataAnnotations;

namespace QrHuntBackend.Database.Entities
{
    public class QrCodeToUserMatch
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CodeId { get; set; }
        public virtual User User { get; set; }
        public virtual QrCode Code { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
