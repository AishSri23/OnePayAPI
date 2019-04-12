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
    public class BusinessProductsController : ControllerBase
    {
        private IBusinessProductService _bProductService;
        private IUserService _userService;
        private IMapper _mapper;
        public BusinessProductsController(IBusinessProductService bProductService, IUserService userService, IMapper mapper)
        {
            _bProductService = bProductService;
            _userService = userService;
            _mapper = mapper;
        }
        [Route("GetAllAccounts")]
        [HttpGet]
        public IActionResult GetAll(string Id)
        {
            var businessProducts = _bProductService.GetAllBusinessProductsOfUser(Id);
            var accountDTOs = _mapper.Map<IList<BusinessProductDTO>>(businessProducts);
            return Ok(accountDTOs);
        }
        [Route("GetAccount/{id}/{accountId}")]
        [HttpGet]
        public IActionResult Get(string id, string accountId)
        {
            var account = _bProductService.GetById(id, accountId);
            var accountDto = _mapper.Map<BusinessProductDTO>(account);
            return Ok(accountDto);
        }

        [Route("CreateAccount")]
        [HttpPost]
        public IActionResult Create([FromBody] BusinessProductDTO accountDTO)
        {
            var account = _mapper.Map<BusinessProduct>(accountDTO);

            try
            {
                // save 
                _bProductService.Create(account);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] BusinessProductDTO accountDTO)
        {
            var account = _mapper.Map<BusinessProduct>(accountDTO);
            try
            {
                _bProductService.Update(account, id);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string userId, string accountId)
        {
            if(!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(userId))
            {
                _bProductService.Delete(userId, accountId);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            
        }
    }
}