using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.EntityFrameworkCore.Repositories;

namespace SmartSaver.WebApi.Controllers
{
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly ITransactionRepo _transactions;

        public TransactionController(ITransactionRepo transactions)
        {
            _transactions = transactions;
        }

        [HttpGet("transaction/{transactionId}")]
        public ActionResult<Transaction> Index(int transactionId)
        {
            return _transactions.GetById(transactionId);
        }

        [HttpGet("transactions/{accountId}")]
        public ActionResult<IEnumerable<Transaction>> Get(int accountId)
        {
            return _transactions.GetByAccountId(accountId);
        }

        [HttpGet("transactions/{accountId}/{start}/{end}")]
        public ActionResult<IEnumerable<Transaction>> Get(int accountId, DateTime start, DateTime end)
        {
            var response = _transactions.GetByAccountForDateRange(accountId, start, end);
            return Ok(response);
        }

        [HttpPost("transactions")]
        public async Task<ActionResult> Create(TransactionRestModel trm)
        {
            var result = await _transactions.CreateTransactionForAccount(new Transaction()
            {
                Amount = trm.Amount,
                AccountId = trm.AccountId,
                CategoryId = trm.CategoryId,
            }, trm.AccountId);

            return result switch
            {
                CreateTransactionResponse.BadAmountError => 
                    BadRequest("Bad request: Invalid transaction amount"),

                CreateTransactionResponse.GeneralError => 
                    BadRequest("Bad request: An error occured."),

                _ => Ok(trm)
            };
        }
    }

    public class TransactionRestModel
    {
        public decimal Amount { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; }
    }
}
