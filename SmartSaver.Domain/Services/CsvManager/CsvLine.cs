using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSaver.Domain.Services.CsvManager
{
    public class CsvLine
    {
        /// <summary>
        /// Time when purchase / income happened.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Categories of a purchase, eg. "Food", "Transport", ...
        /// If transaction is negative (means it's not a purchase, but spending), Categories is not set.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Purchase / income amount.
        /// </summary>
        public double Amount { get; set; }

        public override string ToString()
        {
            return $"Transactions: Time: {Time}, Categories: {Category}, Amount: {Amount}";
        }

    }
}
