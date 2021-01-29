using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class EmailVerification
    {
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Key]
        public int UserId { get; set; }

        public string Token { get; set; }
    }
}
