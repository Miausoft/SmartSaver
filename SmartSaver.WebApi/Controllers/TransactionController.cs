using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using SmartSaver.Domain.CustomAttributes;
using SmartSaver.Domain.Repositories;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.WebApi.RequestModels;
using SmartSaver.WebApi.ResponseModels;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartSaver.WebApi.Controllers
{
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly IRepository<Transaction> _transactions;
        private readonly IMapper _mapper;

        public TransactionController(IRepository<Transaction> transactions, IMapper mapper)
        {
            _transactions = transactions;
            _mapper = mapper;
        }

        [HttpGet("transaction/{transactionId}")]
        public async Task<TransactionResponseModel> Index(int transactionId)
        {
            return _mapper.Map<TransactionResponseModel>(
                await _transactions.GetByIdAsync(transactionId));
        }

        [HttpGet("transactions/{accountId}")]
        public async Task<IEnumerable<TransactionResponseModel>> Get(int accountId)
        {
            return _mapper.Map<IEnumerable<TransactionResponseModel>>(
               await _transactions.SearchFor(t => t.AccountId == accountId).ToListAsync());
        }

        [HttpGet("transactions/{accountId}/{start}/{end}")]
        public async Task<IEnumerable<TransactionResponseModel>> Get(int accountId, DateTime start, DateTime end)
        {
            return _mapper.Map<IEnumerable<TransactionResponseModel>>(
                await _transactions.SearchFor(t => t.AccountId == accountId && t.ActionTime >= start && t.ActionTime < end).ToListAsync());
        }

        [HttpPost("transactions")]
        [CheckForInvalidModel]
        public async Task<IActionResult> Create(TransactionRequestModel transaction)
        {
            await _transactions.InsertAsync(_mapper.Map<Transaction>(transaction));
            await _transactions.SaveAsync();
            return Created($"transaction/{transaction}", transaction);
        }

        [HttpDelete("transaction/{transactionId}")]
        public async Task<IActionResult> Delete(int transactionId)
        {
            _transactions.Delete(await _transactions.GetByIdAsync(transactionId));
            await _transactions.SaveAsync();
            return Ok();
        }
    }
}
