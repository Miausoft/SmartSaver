using System;

namespace SmartSaver.WebApi.RequestModels
{
    public class TransactionRequestModel
    {
        public DateTime ActionTime { get; set; }

        public decimal Amount { get; set; }

        public int AccountId { get; set; }

        public int CategoryId { get; set; }
    }
}
