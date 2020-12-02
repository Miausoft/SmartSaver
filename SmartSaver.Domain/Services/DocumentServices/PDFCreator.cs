using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Services.TransactionsCounterService;

namespace SmartSaver.Domain.Services.DocumentServices
{
    public class PDFCreator
    {
        public delegate void TextBlock(string text, float fontSize);

        #region Declaration
        int _totalColumn = 3;
        Document _document;
        Font _fontStyle;
        PdfPTable _pdfTable = new PdfPTable(3);
        PdfPCell _pdfPCell;
        MemoryStream _memoryStream = new MemoryStream();
        IEnumerable<Transaction> transactions;
        IDictionary<int, decimal> totalExpenseByCategory;
        String pickedDates;
        #endregion

        public byte[] GeneratePDF(IEnumerable<Transaction> transaction, DateTime from, DateTime to)
        {
            #region
            _document = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
            _document.SetPageSize(PageSize.A4);
            _document.SetMargins(20f, 20f, 20f, 20f);
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_CENTER;
            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfWriter.GetInstance(_document, _memoryStream);
            _document.Open();
            _pdfTable.SetWidths(new float[] { 20f, 150f, 100f });
            transactions = transaction;
            pickedDates = "from: " + from.Date.ToString() + "  to: " + to.Date.ToString();
            totalExpenseByCategory = TransactionsCounter.TotalExpenseByCategory(transaction, from, to);
            #endregion

            this.ReportHeader();
            this.ReportBody();
            _pdfTable.HeaderRows = 2;
            _document.Add(_pdfTable);
            _document.Close();
            return _memoryStream.ToArray();
        }

        private void ReportHeader()
        {
            TextBlock fileHeader = delegate (string text, float fontSize) // Formatting based on TABLE HEADER
            {
                _fontStyle = FontFactory.GetFont("Tahoma", fontSize, 1);
                _pdfPCell = new PdfPCell(new Phrase(text));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();
            };

            fileHeader("Statement", 20f);
            fileHeader("spendings sorted by category", 9f);
            fileHeader(pickedDates, 10f);
            _pdfTable.CompleteRow();
        }

        private void ReportBody()
        {
            #region Block creation methods
            TextBlock tableHead = delegate(string text, float fontSize) // Formatting based on TABLE HEADER
            {
                _fontStyle = FontFactory.GetFont("Tahoma", fontSize, 1);
                _pdfPCell = new PdfPCell(new Phrase(text));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
            };

            TextBlock tableBody = delegate (string text, float fontSize) // Formatting based on TABLE BODY
            {
                _fontStyle = FontFactory.GetFont("Tahoma", fontSize, 0);
                _pdfPCell = new PdfPCell(new Phrase(text, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);
            };
            #endregion

            #region Table header
            tableHead("ID", 8f);
            tableHead("Category", 8f);
            tableHead("Amount", 8f);
            _pdfTable.CompleteRow();
            #endregion

            #region Table body
            int serialNumber = 1;
            foreach(var expense in totalExpenseByCategory) // changed with: foreach(var item in totalIncomeByCategory)
            {
                tableBody(serialNumber.ToString(), 8f);
                tableBody(expense.Key.ToString(), 8f);
                tableBody(expense.Value.ToString(), 8f);
                _pdfTable.CompleteRow();
                serialNumber++;
            }
            if(totalExpenseByCategory.Count == 0)
            {
                tableBody("no data to display", 8f);
                tableBody("no data to display", 8f);
                tableBody("no data to display", 8f);
                _pdfTable.CompleteRow();
            }
            #endregion
        }
    }
}
