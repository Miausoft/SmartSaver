using Microsoft.EntityFrameworkCore.Internal;
using SmartSaver.EntityFrameworkCore.Models;
using System;
using System.Linq;

namespace SmartSaver.Domain.Services.SavingMethodSuggestion
{
    public static class SuggestionsForUser
    {
        public static string CompareExpenses(Account acc)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1);
            decimal amountSpentCurrentMonth = TransactionsCounter.TransactionsCounter.AmountSpentCurrentMonth(acc.Transactions);
            decimal amountSavedCurrentMonth = TransactionsCounter.TransactionsCounter.AmountSavedCurrentMonth(acc.Transactions);
            decimal amountToSaveAMonth = MoneyCounter.AmountToSaveAMonth(acc);
            decimal savedSum = TransactionsCounter.TransactionsCounter.SavedSum(acc.Transactions, firstDayOfMonth, lastDayOfMonth);
            decimal freeMoneyToSpend = MoneyCounter.AmountLeftToSpend(acc);

            if (Math.Round(amountToSaveAMonth, 2) == Math.Round(amountSavedCurrentMonth, 2))
            {
                string suggestion = "Šį mėnesį sutaupėte tiek, kiek ir reikia.";

                if(freeMoneyToSpend == -1)
                {
                    suggestion = suggestion + "Tačiau " + HowToIncreaseSavings(acc);
                }

                else if (freeMoneyToSpend > 0)
                {
                    suggestion = suggestion + " Toliau kaip ir viskas ok, net neatsiliekate nuo taupymo režimo";
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
                    suggestion = suggestion + " Toliau kaip ir viskas ok, net neatsiliekate nuo taupymo režimo";
                }

                return suggestion;
            }

            else if (amountToSaveAMonth > amountSavedCurrentMonth)
            {
                string suggestion = "Šį mėnesį nesutaupėte numatytos sumos. Jums pritrūko " + Math.Round(amountToSaveAMonth - amountSavedCurrentMonth, 2).ToString("C") + "\n";
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

            else return "Undefined";
        }

        private static string HowToIncreaseSavings(Account acc)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1);
            string newsuggestion = "";
            if (TransactionsCounter.TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Any())
            {
                newsuggestion = "vis dar atsiliekate nuo savo taupymo režimo."; //ka siuo atveju jam pasiulyti vel? reitku kazka vel compare
                newsuggestion = newsuggestion + " Šį mėnesį daugiausiai buvo išleista " +
                    TransactionsCounter.TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Aggregate((l, r) => l.Value > r.Value ? r : l).Key +
                    " kategorijai, kurios suma " +
                    Math.Round(TransactionsCounter.TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Values.Min() * (-1), 2).ToString("C") +
                    "\n";
                newsuggestion = newsuggestion + "Kad tai daugiau nepasikartotų, siūlome:\n";
                newsuggestion = newsuggestion + "a) sumažinti išlaidas šioje kategorijoje.\n";
                newsuggestion = newsuggestion + "b) visiškai atsisakome išlaidų mažiausiai išleistoje kategorijoje, tai būtų " +
                    TransactionsCounter.TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Aggregate((l, r) => l.Value > r.Value ? l : r).Key +
                    " kategorija, kurios suma viso labo " +
                    Math.Round(TransactionsCounter.TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Values.Max(), 2).ToString("C") +
                    "\n";
                newsuggestion = newsuggestion + "c) atsisakysime išlaidų nenumatytoje kategorijoje, kuriai pinigų anksčiau niekur neleidote: (bus įgyvendinta vėliau)";
            }
            return newsuggestion;
        }
    }
}
