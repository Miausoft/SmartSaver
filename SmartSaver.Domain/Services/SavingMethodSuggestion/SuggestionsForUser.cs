using Microsoft.EntityFrameworkCore.Internal;
using SmartSaver.EntityFrameworkCore.Models;
using System;
using System.Linq;
using SmartSaver.Domain.Services.TransactionsCounterService;

namespace SmartSaver.Domain.Services.SavingMethodSuggestion
{
    public static class SuggestionsForUser
    {

        public static string CompareExpenses(Account acc)
        {
            decimal amountSavedCurrentMonth = TransactionsCounter.AmountSavedCurrentMonth(acc.Transactions);
            decimal amountToSaveAMonth = MoneyCounter.AmountToSaveAMonth(acc);
            decimal freeMoneyToSpend = MoneyCounter.AmountLeftToSpend(acc);

            if (Math.Round(amountToSaveAMonth, 2) == Math.Round(amountSavedCurrentMonth, 2))
            {
                string suggestion = "Šį mėnesį sutaupėte tiek, kiek ir reikia.\n";

                if(freeMoneyToSpend == -1)
                {
                    suggestion = suggestion + "Tačiau " + HowToIncreaseSavings(acc);
                }

                else if (freeMoneyToSpend > 0)
                {
                    suggestion = suggestion + "Linkime ir toliau taip stropiai taupyti, kadangi nuo savo taupymo režimo neatsiliekate!";
                }

                return suggestion;
            }

            else if(amountToSaveAMonth < amountSavedCurrentMonth)
            {
                string suggestion = "Šį mėnesį papildomai sutaupėte " + Math.Round(amountSavedCurrentMonth - amountToSaveAMonth, 2).ToString("C") + "\n";

                if (freeMoneyToSpend == -1)
                {
                    suggestion = suggestion + "Tačiau to nepakanka, kadangi " + HowToIncreaseSavings(acc);
                }

                else if (freeMoneyToSpend > 0)
                {
                    suggestion = suggestion + "Linkime ir toliau taip stropiai taupyti, kadangi nuo savo taupymo režimo neatsiliekate!";
                }

                return suggestion;
            }

            else if (amountToSaveAMonth > amountSavedCurrentMonth)
            {
                string suggestion = "Šį mėnesį jums liko sutaupyti: " + Math.Round(amountToSaveAMonth - amountSavedCurrentMonth, 2).ToString("C") + "\n";
                if (freeMoneyToSpend > 0)
                {
                    suggestion = suggestion + " Tačiau viskas gerai, nes turėjote daugiau pinigų, kuriuos galėjote skirti papildomoms išlaidoms šį mėnesį";
                }

                else if (freeMoneyToSpend == -1)
                {
                    suggestion = suggestion + "Taip pat " + HowToIncreaseSavings(acc);
                }

                return suggestion;
            }

            else return "Nenumatytas atvejis";
        }

        private static string HowToIncreaseSavings(Account acc)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1);
            string suggestion = "";

            if (TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Any())
            {
                suggestion = "vis dar atsiliekate nuo savo taupymo režimo."; //ka siuo atveju jam pasiulyti vel? reitku kazka vel compare
                suggestion = suggestion + " Šį mėnesį daugiausiai buvo išleista " +
                (TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Aggregate((l, r) => l.Value > r.Value ? r : l).Key-1) +
                    " kategorijai, kurios suma " +
                    Math.Round(TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Values.Min() * (-1), 2).ToString("C") +
                    "\n";
                suggestion = suggestion + "Kad tai daugiau nepasikartotų, siūlome:\n";
                suggestion = suggestion + "a) sumažinti išlaidas šioje kategorijoje.\n";
                suggestion = suggestion + "b) visiškai atsisakome išlaidų mažiausiai išleistoje kategorijoje, tai būtų " +
                    TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Aggregate((l, r) => l.Value > r.Value ? l : r).Key +
                    " kategorija, kurios suma viso labo " +
                    Math.Round(TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Values.Max(), 2).ToString("C") +
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
