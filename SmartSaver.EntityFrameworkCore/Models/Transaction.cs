using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class Transaction
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public DateTime ActionTime { get; set; }
        public decimal Amount { get; set; }

        [Key, ForeignKey(nameof(AccountId) + ", " + nameof(UserId)), Column(Order = 1)]
        public Account Account { get; set; }
        public int AccountId { get; set; }

        [Key, ForeignKey(nameof(UserId)), Column(Order = 2)]
        public User User { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
