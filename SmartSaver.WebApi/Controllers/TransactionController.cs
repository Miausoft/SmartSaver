using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;
using SmartSaver.WebApi.RequestModels;
using SmartSaver.WebApi.ResponseModels;
using AutoMapper;
using SmartSaver.Domain.ActionFilters;

namespace SmartSaver.WebApi.Controllers
{
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly ITransactionRepoositry _transactions;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionRepoositry transactions, IMapper mapper)
        {
            _transactions = transactions;
            _mapper = mapper;
        }


        [HttpGet("transaction/{transactionId}")]
        public TransactionResponseModel Index(int transactionId)
        {
            return _mapper.Map<TransactionResponseModel>(
                _transactions.GetById(transactionId)
            );
        }


        [HttpGet("transactions/{accountId}")]
        public IEnumerable<TransactionResponseModel> Get(int accountId)
        {
            return _mapper.Map<IEnumerable<TransactionResponseModel>>(
                _transactions.GetByAccountId(accountId)
            );
        }


        [HttpGet("transactions/{accountId}/{start}/{end}")]
        public IEnumerable<TransactionResponseModel> Get(int accountId, DateTime start, DateTime end)
        {
            var response = _transactions.GetByAccountForDateRange(accountId, start, end);
            return _mapper.Map<IEnumerable<TransactionResponseModel>>(response);
        }


        [HttpPost("transactions")]
        [CheckForInvalidModel]
        public async Task<ActionResult> Create(TransactionRequestModel transaction)
        {
            var id = await _transactions.CreateTransaction(
                _mapper.Map<Transaction>(transaction)
            );

            return Created($"transaction/{id}", transaction);
        }


        [HttpDelete("transaction/{transactionId}")]
        public async Task<ActionResult> Delete(int transactionId)
        {
            int rowsAffected = await _transactions.DeleteById(transactionId);

            if (rowsAffected < 1)
            {
                return Ok("No rows affected.");
            }

            return Ok($"{rowsAffected} rows affected.");
        }
    }
}
