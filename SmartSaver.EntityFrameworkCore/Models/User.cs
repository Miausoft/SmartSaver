using System;
using System.ComponentModel.DataAnnotations;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime DateJoined { get; set; }
        public string Token { get; set; }
    }
}
