using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime ActionTime { get; set; }
        public double Amount { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string GetCategoryString()
        {
            return Category.Title;
        }

        /// <summary>
        /// Can be used in listview binding.
        /// </summary>
        [NotMapped]
        public string CategoryString => Category.Title ?? "";
    }
}
