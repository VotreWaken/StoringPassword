using StoringPassword.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoringPassword.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Mesage { get; set; }

        [DisplayName("User")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
