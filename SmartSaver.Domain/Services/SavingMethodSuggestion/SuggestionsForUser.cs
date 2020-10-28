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
            decimal amountToSaveAMonth = MoneyCounter.AmountToSaveAMonth(acc.Goal, acc.GoalStartDate, acc.GoalEndDate);
            decimal freeMoneyToSpend = MoneyCounter.AmountLeftToSpend(acc);
            string suggestion = "Šį mėnesį sutaupėte: " + amountSavedCurrentMonth.ToString("C") + "\n";

            if (Math.Round(amountToSaveAMonth, 2) == Math.Round(amountSavedCurrentMonth, 2))
            {
                if(freeMoneyToSpend < 0)
                {
                    suggestion += "Tačiau " + HowToIncreaseSavings(acc);
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
                    suggestion += "Tačiau to nepakanka, kadangi " + HowToIncreaseSavings(acc);
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
                    suggestion += "Taip pat " + HowToIncreaseSavings(acc);
                }
            }

            return suggestion;
        }

        private static string HowToIncreaseSavings(Account acc)
        {
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1);
            string suggestion = "";

            if (TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Any())
            {
                suggestion = "vis dar atsiliekate nuo savo taupymo režimo."; //ka siuo atveju jam pasiulyti vel? reitku kazka vel compare
                suggestion += " Šį mėnesį daugiausiai buvo išleista " +
                (TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Aggregate((l, r) => l.Value > r.Value ? r : l).Key-1) +
                    " kategorijai, kurios suma " +
                    Math.Round(-TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Values.Min(), 2).ToString("C") +
                    "\n";
                suggestion += "Kad tai daugiau nepasikartotų, siūlome:\n";
                suggestion += "a) sumažinti išlaidas šioje kategorijoje.\n";
                suggestion += "b) visiškai atsisakome išlaidų mažiausiai išleistoje kategorijoje, tai būtų " +
                    (TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Aggregate((l, r) => l.Value > r.Value ? l : r).Key-1) +
                    " kategorija, kurios suma viso labo " +
                    Math.Round(-TransactionsCounter.TotalExpenseByCategory(acc.Transactions, firstDayOfMonth, lastDayOfMonth).Values.Max(), 2).ToString("C") +
                    "\n";
                //suggestion = suggestion + "c) atsisakysime išlaidų kategorijoje, kuriai pinigų anksčiau niekur neleidote (bus įgyvendinta vėliau)";
            }

            else
            {
                suggestion = "neturite jokių išlaidų, todėl pasiūlymų, kaip taypyti pinigus, jums pateikti negalime :(";
            }

            return suggestion;
        }

        public static string EstimatedTime(Account acc)
        {
            decimal savedSum = TransactionsCounter.SavedSum(acc.Transactions, acc.GoalStartDate, acc.GoalEndDate);

            if (savedSum < 0) return "Per taupymo laikotarpį kol kas nesutaupėte jokios pinigų sumos";
            else if (savedSum >= acc.Goal && DateTime.Today < acc.GoalEndDate) return "Sugebėjote sutaupyti anksčiau nei numatėte!";
            else if (savedSum >= acc.Goal && DateTime.Today > acc.GoalEndDate) return "Sugebėjote sutaupyti, tačiau vėliau nei numatėte!";
            else if (MoneyCounter.Average(DateCounter.DaysPassed(acc.GoalStartDate), savedSum) == 1) return acc.Goal.ToString("C") + " sutaupysite iki\n " + acc.GoalEndDate.ToShortDateString();
            else if (MoneyCounter.Average(DateCounter.DaysPassed(acc.GoalStartDate), savedSum) == 0) return "Numatytas laikas iki taupymo pabaigos: N/A";
            else return acc.Goal.ToString("C") + " sutaupysite iki\n" + DateTime.Now.AddDays((double)Math.Ceiling((acc.Goal - savedSum) / MoneyCounter.Average(DateCounter.DaysPassed(acc.GoalStartDate), savedSum))).ToShortDateString();
        }

       public static string FreeMoneyToSpend(Account acc)
        {
            string suggestion = "Pinigų suma, kurią galite skirti papildomoms išlaidoms: ";

            if (MoneyCounter.AmountLeftToSpend(acc) >= 0)
            {
                suggestion += Math.Round(MoneyCounter.AmountLeftToSpend(acc), 2).ToString("C");
            }
            else
            {
                suggestion += "N/A";
            }

            return suggestion;
        }
    }
}
