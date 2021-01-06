using Microsoft.EntityFrameworkCore.Internal;
using SmartSaver.EntityFrameworkCore.Models;
using System;
using System.Linq;
using SmartSaver.Domain.Services.TransactionsCounterService;
using System.Collections.Generic;

namespace SmartSaver.Domain.Services.SavingMethodSuggestion
{
    public static class SuggestionsForUser
    {
        public static string CompareExpenses(Account acc, IEnumerable<Category> categories)
        {
            decimal amountSavedCurrentMonth = TransactionsCounter.AmountSavedCurrentMonth(acc.Transactions);
            decimal amountToSaveAMonth = MoneyCounter.AmountToSaveAMonth(acc);
            decimal freeMoneyToSpend = MoneyCounter.FreeMoneyToSpend(acc);
            string suggestion = "Šį mėnesį sutaupėte: " + amountSavedCurrentMonth.ToString("C") + "\n";

            if (Math.Round(amountToSaveAMonth, 2) == Math.Round(amountSavedCurrentMonth, 2))
            {
                if(freeMoneyToSpend < 0)
                {
                    suggestion += "Tačiau " + HowToIncreaseSavings(acc, categories);
                }

                else if (freeMoneyToSpend >= 0)
                {
                    suggestion += "Linkime ir toliau taip stropiai taupyti, kadangi nuo savo taupymo režimo neatsiliekate!";
                }
            }

            else if(amountToSaveAMonth < amountSavedCurrentMonth)
            {
                suggestion += "Šį mėnesį papildomai sutaupėte " + Math.Round(amountSavedCurrentMonth - amountToSaveAMonth, 2).ToString("C") + "\n";

                if (freeMoneyToSpend < 0)
                {
                    suggestion += "Tačiau to nepakanka, kadangi " + HowToIncreaseSavings(acc, categories);
                }

                else if (freeMoneyToSpend >= 0)
                {
                    suggestion += "Linkime ir toliau taip stropiai taupyti, kadangi nuo savo taupymo režimo neatsiliekate!";
                }
            }

            else if (amountToSaveAMonth > amountSavedCurrentMonth)
            {
                suggestion = suggestion + "Šį mėnesį jums liko sutaupyti: " + Math.Round(amountToSaveAMonth - amountSavedCurrentMonth, 2).ToString("C") + "\n";

                if (freeMoneyToSpend >= 0)
                {
                    suggestion += " Tačiau viskas gerai, nes turėjote daugiau pinigų, kuriuos galėjote skirti papildomoms išlaidoms šį mėnesį";
                }

                else if (freeMoneyToSpend < 0)
                {
                    suggestion += "Taip pat " + HowToIncreaseSavings(acc, categories);
                }
            }

            return suggestion;
        }

        private static string HowToIncreaseSavings(Account acc, IEnumerable<Category> categories)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1);
            string suggestion = "";

            if (TransactionsCounter.TotalExpenseByCategory(acc.Transactions, categories, firstDayOfMonth, lastDayOfMonth).Any())
            {
                suggestion = "vis dar atsiliekate nuo savo taupymo režimo."; //ka siuo atveju jam pasiulyti vel? reitku kazka vel compare
                suggestion += " Šį mėnesį daugiausiai buvo išleista " +
                (TransactionsCounter.TotalExpenseByCategory(acc.Transactions, categories, firstDayOfMonth, lastDayOfMonth).Aggregate((l, r) => l.Value > r.Value ? r : l).Key) +
                    " kategorijai, kurios suma " +
                    Math.Round(-TransactionsCounter.TotalExpenseByCategory(acc.Transactions, categories, firstDayOfMonth, lastDayOfMonth).Values.Min(), 2).ToString("C") +
                    "\n";
                suggestion += "Kad tai daugiau nepasikartotų, siūlome:\n";
                suggestion += "a) sumažinti išlaidas šioje kategorijoje.\n";
                suggestion += "b) visiškai atsisakome išlaidų mažiausiai išleistoje kategorijoje, tai būtų " +
                    (TransactionsCounter.TotalExpenseByCategory(acc.Transactions, categories, firstDayOfMonth, lastDayOfMonth).Aggregate((l, r) => l.Value > r.Value ? l : r).Key) +
                    " kategorija, kurios suma viso labo " +
                    Math.Round(-TransactionsCounter.TotalExpenseByCategory(acc.Transactions, categories, firstDayOfMonth, lastDayOfMonth).Values.Max(), 2).ToString("C") +
                    "\n";
                //suggestion = suggestion + "c) atsisakysime išlaidų kategorijoje, kuriai pinigų anksčiau niekur neleidote (bus įgyvendinta vėliau)";
            }

            else
            {
                suggestion = "neturite jokių išlaidų, todėl pasiūlymų, kaip taypyti pinigus, jums pateikti negalime :(";
            }

            return suggestion;
        }
    }
}
