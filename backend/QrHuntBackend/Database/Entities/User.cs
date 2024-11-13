using System.ComponentModel.DataAnnotations;

namespace QrHuntBackend.Database.Entities
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Roles { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
    }
}
