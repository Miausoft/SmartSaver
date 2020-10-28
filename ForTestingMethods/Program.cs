using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using System.Collections.Generic;
using System;
using SmartSaver.Domain.Services.TransactionsCounterService;
using SmartSaver.Domain.Services.SavingMethodSuggestion;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var db = new ApplicationDbContext())
            {
                List<Transaction> transaction = new List<Transaction>()
                {
                    new Transaction(){Amount = -5m,      CategoryId = 1,  ActionTime = new DateTime(2020, 01, 01), Category =  new Category() { Id = 1, Title = "VIENAS"} },
                    new Transaction(){Amount = -13.74m,  CategoryId = 1,  ActionTime = new DateTime(2020, 01, 01), Category =  new Category() { Id = 1, Title = "VIENAS"} },
                    new Transaction(){Amount = -9.07m,   CategoryId = 2,  ActionTime = new DateTime(2020, 03, 03), Category =  new Category() { Id = 2, Title = "DU"} },
                    new Transaction(){Amount = 14.42m,   CategoryId = 4,  ActionTime = new DateTime(2020, 04, 04), Category =  new Category() { Id = 4, Title = "KETURI"} },
                    new Transaction(){Amount = -14.46m,  CategoryId = 1,  ActionTime = new DateTime(2020, 05, 05), Category =  new Category() { Id = 1, Title = "VIENAS"} },
                    new Transaction(){Amount = 5.35m,    CategoryId = 4,  ActionTime = new DateTime(2020, 06, 06), Category =  new Category() { Id = 4, Title = "KETURI"} },
                    new Transaction(){Amount = -10.92m,  CategoryId = 3,  ActionTime = new DateTime(2020, 07, 07), Category =  new Category() { Id = 4, Title = "TRYS"} },
                    new Transaction(){Amount = 9.47m,    CategoryId = 5,  ActionTime = new DateTime(2020, 08, 08), Category =  new Category() { Id = 4, Title = "PENKI"} },
                    new Transaction(){Amount = -14.94m,   CategoryId = 3,  ActionTime = new DateTime(2020, 10, 09), Category =  new Category() { Id = 3, Title = "KETURI"} },
                    new Transaction(){Amount = -53.44m,   CategoryId = 4,  ActionTime = new DateTime(2020, 10, 10), Category =  new Category() { Id = 4, Title = "KETURI"} }
                };

                Account acc = new Account()
                {
                    GoalStartDate = new DateTime(2020, 01, 01),
                    GoalEndDate = new DateTime(2021, 05, 21),
                    Goal = 65,
                    Transactions = transaction
                };

                DateTime from = new DateTime(2020, 01, 01);
                DateTime to = new DateTime(2020, 12, 21);


                Console.WriteLine("Kiek zmogus sutaupe pinigu kiekvienoj kategorijoj bendrai: ");
                var dic = TransactionsCounter.TotalIncomeByCategory(transaction, from, to);
                foreach (var d in dic)
                {
                    Console.WriteLine("Kategorija: " + d.Key + "     " + "Amount: " + d.Value);
                }

                Console.WriteLine();


                Console.WriteLine("Kiek zmogus isleido pinigu kiekvienoj kategorijoj procentaliai: ");
                var gg = TransactionsCounter.TotalExpenseByCategory(transaction, from, to);
                foreach (var d in gg)
                {
                    Console.WriteLine("Kategorija: "
                        + d.Key
                        + "     "
                        + "Amount: "
                        + d.Value
                        );
                }

                Console.WriteLine();
                Console.WriteLine("Per taupymo laikotarpi nuo " + from + " iki " + to + " sutaupete: " + TransactionsCounter.SavedSum(transaction, from, to));

                Console.WriteLine();
                Console.WriteLine("Kiek is viso iplauku: " + TransactionsCounter.TotalIncome(transaction, from, to));

                Console.WriteLine();
                Console.WriteLine("Kiek is viso islaidu: " + TransactionsCounter.TotalExpense(transaction, from, to));

                Console.WriteLine();
                Console.WriteLine(MoneyCounter.EstimatedTime(acc));

                Console.WriteLine();
                Console.WriteLine("Noredami pasiekti savo goal laiku: ");
                //Console.WriteLine("Kiekviena menesi galite isleisti: " + MoneyCounter.AmountToSpendAMonth(acc)); need better implementation of this method
                Console.WriteLine("Kiekviena menesi turite sutaupyti: " + MoneyCounter.AmountToSaveAMonth(acc));
                Console.WriteLine("si menesi jau isleidote: " + TransactionsCounter.AmountSpentCurrentMonth(acc.Transactions));
                Console.WriteLine("si menesi kolkas sutaupete: " + TransactionsCounter.AmountSavedCurrentMonth(acc.Transactions));
                Console.WriteLine("Pingu likutis, kuri galite skirti savo islaidoms, kad pasiektumete savo tiksla laiku " + MoneyCounter.AmountLeftToSpend(acc));
                Console.WriteLine("KODEL butent tiek? nes praejo apie 10 men nuo sausio iki spalio, sutaupyti i menesi jis turi 3,91, todel 44-39 ir turim tuos 5eurus");
            }
        }
    }
}
