using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnePay_Backend.DTO;
using OnePay_Backend.Helpers;
using OnePay_Backend.Models;
using OnePay_Backend.Services;


namespace OnePay_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnePayController : ControllerBase
    {
        private ITransactionService _transactionService;
        private IUserService _userService;
        private IOnePayService _onePayService;
        private ICoreBankingService _coreBankingService;
        private IMapper _mapper;

        public OnePayController(
            ITransactionService transactionService,
            IUserService userService,
            IOnePayService onePayService,
            ICoreBankingService coreBankingService,
            IMapper mapper)
        {
            _transactionService = transactionService;
            _userService = userService;
            _onePayService = onePayService;
            _coreBankingService = coreBankingService;
            _mapper = mapper;
        }
        /// <summary>
        /// Register For OnePay
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        [Route("registerforonepay")]
        [HttpPost]
        public IActionResult RegisterUserForOnePay(string userId)
        {
            var user = _userService.GetById(userId);
            if (user == null)
            {
                return NotFound("User Not found");
            }
            if(user.IsRegisteredForOnePay)
            {
                return Ok("User Already registered for OnePay");
            }
            try
            {
                // save 
                _userService.RegisterUserForOnePay(user);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// GetAccountsEligibleForOnePay
        /// </summary>
        /// <param name="Id">User ID</param>
        /// <returns>list of business products eligible for onepay</returns>
        [Route("getonepayaccounts")]
        [HttpGet]
        public IActionResult GetAccountsForOnePay(string Id)
        {
            var user = _userService.GetById(Id);
            var businessProducts = _onePayService.GetAccountsEligibleForOnePay(user);
            var businessProductDTOs = _mapper.Map<IList<BusinessProductDTO>>(businessProducts);
            return Ok(businessProductDTOs);
        }
        /// <summary>
        /// Get Total Due Amounts 
        /// </summary>
        /// <param name="Id">User ID</param>
        /// <returns>returns total due amount</returns>
        [Route("getdue/{Id}")]
        [HttpGet]
        public IActionResult GetDue(string Id)
        {
            var user = _userService.GetById(Id);
            var dueAmount = _onePayService.CalculateTotal(user.UserId);
            return Ok(dueAmount); 
        }

        /// <summary>
        /// Modify Amounts for certain account
        /// </summary>
        /// <param name="itemDTO">Transaction Item DTO</param>
        /// <returns></returns>
        [Route("getmodifieddue")]
        [HttpPost]
        public IActionResult GetModifiedDue([FromBody]TransactionItemDTO itemDTO)
        {
            decimal dueAmount = 0;
            if(itemDTO != null)
            {
                dueAmount = _onePayService.CalculateModifiedTotalAmount(itemDTO);
            }
            if (dueAmount < itemDTO.TransactionAmount)
                return BadRequest(new { message = $"Amount should be more than {itemDTO.TransactionAmount}" });
            else
                return Ok(dueAmount);
        }
        /// <summary>
        /// Initiates Payment through One Pay
        /// </summary>
        /// <param name="itemDTOs">Transaction Item DTO</param>
        /// <returns></returns>
        [Route("makepayment")]
        [HttpPost]
        public IActionResult PayDueAmount([FromBody]List<TransactionItemDTO> itemDTOs)
        {
            try
            {
                _coreBankingService.BeginTransaction(itemDTOs);
                return Ok();
            }
            catch (AppException ex)
            {
                if(ex.Message.Contains(OnePayEnums.ErrorMessage.balanceIsNotEnough))
                     return BadRequest(new { message = OnePayEnums.ErrorMessage.balanceIsNotEnough });
                else if(ex.Message.Contains(OnePayEnums.ErrorMessage.transactionFailed))
                    return BadRequest(new { message = OnePayEnums.ErrorMessage.transactionFailed });

                return BadRequest("An error occured in Transaction");
            }
        }
    }
}