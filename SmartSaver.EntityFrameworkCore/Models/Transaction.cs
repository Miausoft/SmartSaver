using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime ActionTime { get; set; }
        public decimal Amount { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        /// <summary>
        /// Can be used in listview binding.
        /// </summary>
        /*[NotMapped]
        public string CategoryString
        {
            get
            {
                if (Amount > 0) 
                    return "-";

                return Category.Title ?? "";
            }
        }*/
    }
}
