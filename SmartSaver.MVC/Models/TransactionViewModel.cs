using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.MVC.Models
{
    public class TransactionViewModel
    {
        public List<Transaction> Transactions { get; set; }
        public List<Category> Categories { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }
    }
}
