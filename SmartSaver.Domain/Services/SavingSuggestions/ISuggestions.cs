using SmartSaver.EntityFrameworkCore.Models;
using System;

namespace SmartSaver.Domain.Services.SavingSuggestions
{
    public interface ISuggestions
    {
        /// <summary>
        /// a dynamic number to represent amount of money account have to save current month.
        /// The method can return a negative number meaning that in previous months it was saved more than it was necessary.
        /// </summary>
        public decimal AmountToSaveAMonth(Account acc);

        /// <summary>
        /// amount of money account saved everyday on average
        /// </summary>
        public decimal Average(decimal daysPassed, decimal savedSum);

        /// <summary>
        /// A dynamic number which shows account's balance he can spend without any hesitation because goal will be reached in time.
        /// Zero represents that account can't have any more spendings if he wants to reach his goal in time.
        /// </summary>
        public decimal FreeMoneyToSpend(Account acc);

        /// <summary>
        /// A dynamic number to represent amount of money account left to save until now.
        /// The method does not consider today's date. It only counts previous months.
        /// The method will return a negative number if the account saved more than he had to.
        /// The method will return a positive number if the account saved less than he had to.
        /// </summary>
        public decimal AmountLeftToSave(Account acc);

        public DateTime EstimatedTime(Account acc);
    }
}
