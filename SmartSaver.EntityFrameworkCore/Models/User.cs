using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }
        public int AccountId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateJoined { get; set; }
    }
}
