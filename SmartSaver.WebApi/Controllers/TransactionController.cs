using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using SmartSaver.Domain.CustomAttributes;
using SmartSaver.Domain.Repositories;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.WebApi.RequestModels;
using SmartSaver.WebApi.ResponseModels;
using AutoMapper;

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
        public TransactionResponseModel Index(int transactionId)
        {
            return _mapper.Map<TransactionResponseModel>(
                _transactions.GetById(transactionId));
        }

        [HttpGet("transactions/{accountId}")]
        public IEnumerable<TransactionResponseModel> Get(int accountId)
        {
            return _mapper.Map<IEnumerable<TransactionResponseModel>>(
                _transactions.SearchFor(t => t.AccountId == accountId));
        }

        [HttpGet("transactions/{accountId}/{start}/{end}")]
        public IEnumerable<TransactionResponseModel> Get(int accountId, DateTime start, DateTime end)
        {
            return _mapper.Map<IEnumerable<TransactionResponseModel>>(
                _transactions.SearchFor(t => t.AccountId == accountId && t.ActionTime >= start && t.ActionTime < end));
        }

        [HttpPost("transactions")]
        [CheckForInvalidModel]
        public ActionResult Create(TransactionRequestModel transaction)
        {
            _transactions.Insert(_mapper.Map<Transaction>(transaction));
            return Created($"transaction/{transaction}", transaction);
        }

        [HttpDelete("transaction/{transactionId}")]
        public ActionResult Delete(int transactionId)
        {
            _transactions.Delete(_transactions.GetById(transactionId));
            _transactions.Save();
            return Ok();
        }
    }
}
