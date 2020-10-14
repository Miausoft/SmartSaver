using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using System.Collections.Generic;
using System;
using SmartSaver.Domain.Services.TransactionsCounter;
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
                    new Transaction(){Amount = -5,      CategoryId = 1,  ActionTime = new DateTime(2020, 01, 01) },
                    new Transaction(){Amount = -13.74,  CategoryId = 1,  ActionTime = new DateTime(2020, 01, 01) },
                    new Transaction(){Amount = -9.07,   CategoryId = 2,  ActionTime = new DateTime(2020, 03, 03) },
                    new Transaction(){Amount = 14.42,   CategoryId = 4,  ActionTime = new DateTime(2020, 04, 04) },
                    new Transaction(){Amount = -14.46,  CategoryId = 1,  ActionTime = new DateTime(2020, 05, 05) },
                    new Transaction(){Amount = 5.35,    CategoryId = 4,  ActionTime = new DateTime(2020, 06, 06) },
                    new Transaction(){Amount = -10.92,  CategoryId = 3,  ActionTime = new DateTime(2020, 07, 07) },
                    new Transaction(){Amount = 9.47,    CategoryId = 5,  ActionTime = new DateTime(2020, 08, 08) },
                    new Transaction(){Amount = 14.94,   CategoryId = 4,  ActionTime = new DateTime(2020, 09, 09) },
                    new Transaction(){Amount = 53.44,   CategoryId = 4,  ActionTime = new DateTime(2020, 10, 10) }
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
                        + Math.Round(d.Value/TransactionsCounter.TotalExpense(transaction, from, to)*100, 2)
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
                Console.WriteLine("si menesi jau isleidote: " + MoneyCounter.AmountSpentCurrentMonth(acc));
                Console.WriteLine("si menesi kolkas sutaupete: " + MoneyCounter.AmountSavedCurrentMonth(acc));
                Console.WriteLine("Pingu likutis, kuri galite skirti savo islaidoms, kad pasiektumete savo tiksla laiku " + MoneyCounter.AmountLeftToSpend(acc));
                Console.WriteLine("KODEL butent tiek? nes praejo apie 10 men nuo sausio iki spalio, sutaupyti i menesi jis turi 3,91, todel 44-39 ir turim tuos 5eurus");
            }
        }
    }
}
