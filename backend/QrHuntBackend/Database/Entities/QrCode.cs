using System.ComponentModel.DataAnnotations;

namespace QrHuntBackend.Database.Entities
{
    public class QrCode
    {

        [Key]
        public int ID { get; set; }
        public virtual Game Game { get; set; }
        public string Content { get; set; }
        public string Notes { get; set; }
    }
}
