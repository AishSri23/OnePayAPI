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
    public class UserAccountsController : ControllerBase
    {
        private IAccountsService _userAccountService;
        private IUserService _userService;
        private IMapper _mapper;
        public UserAccountsController(IAccountsService userAccountService,IUserService userService,IMapper mapper)
        {
            _userAccountService = userAccountService;
            _userService = userService;
            _mapper = mapper;
        }
        // GET: api/UserAccounts
        [Route("GetAllAccounts")]
        [HttpGet]
        public IActionResult GetAll(string Id)
        {

            List<UserAccount> UserAccounts = _userAccountService.GetAllAccountOfUser(Id);
            var accountDTOs = _mapper.Map<IList<UserAccountDTO>>(UserAccounts);
            return Ok(accountDTOs);
        }
        [Route("GetAccount/{id}/{accountId}")]
        // GET: api/UserAccounts/5
        [HttpGet]
        public IActionResult Get(string id,string accountId)
        {
            var account =  _userAccountService.GetById(id, accountId);
            var accountDto = _mapper.Map<UserAccountDTO>(account);
            return Ok(accountDto);
        }

        // POST: api/CreateAccount
        [Route("CreateAccount")]
        [HttpPost]
        public IActionResult Create([FromBody] UserAccountDTO accountDTO)
        {
            var account = _mapper.Map<UserAccount>(accountDTO);

            try
            {
                // save 
                _userAccountService.Create(account);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/UserAccounts/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] UserAccountDTO accountDTO)
        {
            var account = _mapper.Map<UserAccount>(accountDTO);
            try
            {
                _userAccountService.Update(account, id);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string userId,string accountId)
        {
            _userAccountService.Delete(userId, accountId);
            return Ok();
        }
    }
}
