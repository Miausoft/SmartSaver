using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TEST
{
    class TransactionCounter
    {
        public void LoadTest()
        {
            var db = new ApplicationDbContext();

            List<Transaction> transaction = new List<Transaction>()
        {
            new Transaction(){Id = 1,  Amount = 8.08,  CategoryId = 5,  AccountId = 29 },
            new Transaction(){Id = 2,  Amount = 13.74, CategoryId = 5,  AccountId = 29 },
            new Transaction(){Id = 3,  Amount = 9.07,  CategoryId = 4,  AccountId = 29 },
            new Transaction(){Id = 4,  Amount = 14.42, CategoryId = 1,  AccountId = 10 },
            new Transaction(){Id = 5,  Amount = 14.46, CategoryId = 1,  AccountId = 38 },
            new Transaction(){Id = 6,  Amount = 5.35,  CategoryId = 5,  AccountId = 1  },
            new Transaction(){Id = 7,  Amount = 10.92, CategoryId = 1,  AccountId = 10 },
            new Transaction(){Id = 8,  Amount = 9.64,  CategoryId = 5,  AccountId = 50 },
            new Transaction(){Id = 9,  Amount = 14.94, CategoryId = 3,  AccountId = 27 },
            new Transaction(){Id = 10, Amount = 13.44, CategoryId = 4,  AccountId = 1 }
        };

            Console.WriteLine("Amount    " + "CategoryId" + "     AccountId");
            foreach (var t in AmountSpentByCategory(transaction))
            {
                var a = t.Amount;
                var cid = t.CategoryId;
                var aid = t.AccountId;
                Console.WriteLine(a + "            " + cid + "      " + aid);
            }

            Console.WriteLine();

            foreach (var t in TotalAmountSpentByAccount(transaction))
            {
                var a = t.Amount;
                var cid = t.CategoryId;
                var aid = t.AccountId;
                Console.WriteLine(a + "            " + cid + "      " + aid);
            }
        }

        public IEnumerable<Transaction> FilterAccount(List<Transaction> transactions, int accountId)
        {
            return transactions.Where(t => t.AccountId == accountId);
        }

        public IEnumerable<Transaction> TotalAmountSpentByAccount(List<Transaction> transaction) //kiek is viso isleido vartotojas
        {
            return transaction.GroupBy(t => t.AccountId).Select(x => new Transaction
            {
                AccountId = x.First().AccountId,
                Amount = x.Sum(c => c.Amount),
            });
        }

        public IEnumerable<Transaction> AmountSpentByCategory(List<Transaction> transaction) // kiek vartotojas isleido atskiroms grupems
        {
            return transaction.GroupBy(t => new { t.CategoryId, t.AccountId }).Select(b => new Transaction
            {
                Amount = b.Sum(bn => bn.Amount),
                AccountId = b.Key.AccountId,
                CategoryId = b.Key.CategoryId
            }).OrderByDescending(b => b.AccountId);
        }
    }
}
