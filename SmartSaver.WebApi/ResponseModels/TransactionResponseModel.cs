using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSaver.WebApi.ResponseModels
{
    public class TransactionResponseModel
    {
        public int Id { get; set; }

        public DateTime ActionTime { get; set; }

        public decimal Amount { get; set; }

        public int AccountId { get; set; }

        public int CategoryId { get; set; }
    }
}
