﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using CsvHelper;

namespace SmartSaver.Services
{
    public class CsvManager
    {
        /// <summary>
        /// Gets all lines from .csv file.
        /// </summary>
        /// <returns>List of csv lines</returns>
        public List<CsvLine> Import(string filePath)
        {
            using var reader = new StreamReader(filePath, Encoding.Default);
            using var csv = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture);

            var lines = csv.GetRecords<CsvLine>().ToList();

            return lines;
        }

        public string Pvz(string message)
        {
            return message;
        }

        /// <summary>
        /// Exports List of csv lines to .csv file. 
        /// </summary>
        /// <param name="filePath">Path to .csv file</param>
        /// <param name="lines">List of csv lines</param>
        /// <param name="append">Should new lines be appended or written on top.</param>
        public void Export(string filePath, List<CsvLine> lines, bool append = true)
        {
            using var sw = new StreamWriter(filePath, append, new UTF8Encoding(true));
            using var cw = new CsvWriter(sw, System.Globalization.CultureInfo.CurrentCulture);

            cw.WriteHeader<CsvLine>();
            cw.NextRecord();

            foreach (CsvLine line in lines)
            {
                cw.WriteRecord(line);
                cw.NextRecord();
            }
        }
    }
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