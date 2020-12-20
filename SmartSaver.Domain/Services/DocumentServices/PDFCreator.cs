using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Services.TransactionsCounterService;

namespace SmartSaver.Domain.Services.DocumentServices
{
    public class PDFCreator
    {
        public delegate void TextBlock(string text, string font, float fontSize, int style);

        public const int _totalColumn = 3;
        PdfPCell _pdfPCell;
        readonly PdfPTable _pdfPTable = new PdfPTable(_totalColumn)
        {
            WidthPercentage = 100,
            HorizontalAlignment = Element.ALIGN_CENTER,
            HeaderRows = 2
        };

        public byte[] GeneratePDF(IEnumerable<TransactionDto> transaction, DateTime from, DateTime to)
        {
            Document _document = new Document(PageSize.A4, 20, 20, 20, 20);
            MemoryStream _memoryStream = new MemoryStream();
            PdfWriter.GetInstance(_document, _memoryStream);
            _document.Open();

            ReportHeader(FileHeader(), from, to);
            ReportBody(TransactionsCounter.TotalExpenseByCategory(transaction, from, to));

            _pdfPTable.SetWidths(new float[] { 20, 150, 100 });
            _document.Add(_pdfPTable);

            _document.Close();

            return _memoryStream.ToArray();
        }

        private void ReportHeader(TextBlock fileHeader, DateTime from, DateTime to)
        {
            fileHeader("Statement", "Tahoma", 20f, 1);
            fileHeader("spendings sorted by category", "Tahoma", 9f, 1);
            fileHeader($"from: {from.ToShortDateString()} to: {to.ToShortDateString()}", "Tahoma", 10f, 1);
            _pdfPTable.CompleteRow();
        }

        private void ReportBody(IDictionary<int, decimal> totalExpenseByCategory)
        {
            FillTableHeader(TableHeader());
            FillTableBody(TableBody(), totalExpenseByCategory);
        }
        
        private void FillTableHeader(TextBlock tableHead)
        {
            tableHead("ID", "Tahoma", 8f, 1);
            tableHead("Category", "Tahoma", 8f, 1);
            tableHead("Amount", "Tahoma", 8f, 1);
            _pdfPTable.CompleteRow();
        }

        private void FillTableBody(TextBlock tableBody, IDictionary<int, decimal> totalExpenseByCategory)
        {
            int serialNumber = 1;
            foreach (var expense in totalExpenseByCategory)
            {
                tableBody(serialNumber.ToString(), "Tahoma", 8f, 0);
                tableBody(expense.Key.ToString(), "Tahoma", 8f, 0);
                tableBody(expense.Value.ToString(), "Tahoma", 8f, 0);
                _pdfPTable.CompleteRow();
                serialNumber++;
            }
        }

        private TextBlock FileHeader()
        {
            return delegate (string text, string font, float fontSize, int style)
            {
                _pdfPCell = new PdfPCell(new Phrase(text, FontFactory.GetFont(font, fontSize, style)))
                {
                    Colspan = _totalColumn,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 0,
                    BackgroundColor = BaseColor.WHITE,
                    ExtraParagraphSpace = 0
                };
                _pdfPTable.AddCell(_pdfPCell);
                _pdfPTable.CompleteRow();
            };
        }

        private TextBlock TableHeader()
        {
            return delegate (string text, string font, float fontSize, int style)
            {
                _pdfPCell = new PdfPCell(new Phrase(text, FontFactory.GetFont(font, fontSize, style)))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    ExtraParagraphSpace = 0
                };
                _pdfPTable.AddCell(_pdfPCell);
            };
        }

        private TextBlock TableBody()
        {
            return delegate (string text, string font, float fontSize, int style)
            {
                _pdfPCell = new PdfPCell(new Phrase(text, FontFactory.GetFont(font, fontSize, style)))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.WHITE,
                    ExtraParagraphSpace = 0
                };
                _pdfPTable.AddCell(_pdfPCell);
            };
        }
    }
}
