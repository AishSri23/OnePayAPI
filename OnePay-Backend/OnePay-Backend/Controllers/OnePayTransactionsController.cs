using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePay_Backend.DTO;
using OnePay_Backend.Helpers;
using OnePay_Backend.Models;
using OnePay_Backend.Services;

namespace OnePay_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnePayTransactionsController : ControllerBase
    {
        private ITransactionService _transactionService;
        private IUserService _userService;
        private IMapper _mapper;

        public OnePayTransactionsController(
            ITransactionService transactionService,
            IUserService userService,
            IMapper mapper)
        {
            _transactionService = transactionService;
            _userService = userService;
            _mapper = mapper;
        }

        // GET: api/OnePayTransactions
        [HttpGet]
        public IActionResult GetOnePayTransactionForUser(string userId)
        {
            var user = _userService.GetById(userId);
            var transactions = _transactionService.GetAll(user);
            var transactionDtos = _mapper.Map<IList<TransactionDTO>>(transactions);
            return Ok(transactionDtos);
        }
    }
}