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
        /// Category of a purchase, eg. "Food", "Transport", ...
        /// If transaction is negative (means it's not a purchase, but spending), Category is not set.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Purchase / income amount.
        /// </summary>
        public double Amount { get; set; }

        public override string ToString()
        {
            return $"Transaction: Time: {Time}, Category: {Category}, Amount: {Amount}";
        }

    }
}
