using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;

        public UsersController(
            IUserService userService,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="authDTO">Authentication DTO</param>
        /// <returns>User</returns>
        [AllowAnonymous]
        [Route("authenticateuser")]
        [HttpPost]
        public IActionResult Authenticate([FromBody]AuthDTO authDTO)
        {
            var user = _userService.Authenticate(authDTO.id, authDTO.password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });


            // return basic user info (without password) and token to store client sides
            return Ok(new
            {
                Id = user.UserId,
                Username = user.UserName,
                PhoneNumber = user.UserPhoneNumber
            });
        }
        /// <summary>
        /// Registers a user in application
        /// </summary>
        /// <param name="userDto">User DTO</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("registeruser")]
        [HttpPost]
        public IActionResult Register([FromBody]UserDTO userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try
            {
                // save 
                _userService.Create(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of UserDTO</returns>
        [Route("getusers")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var userDtos = _mapper.Map<IList<UserDTO>>(users);
            return Ok(userDtos);
        }
        /// <summary>
        /// gets a user DTO with user id
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User ID</returns>
        [Route("getuser/{id}")]
        [HttpGet]
        public IActionResult GetById(string id)
        {
            var user = _userService.GetById(id);
            var userDto = _mapper.Map<UserDTO>(user);
            return Ok(userDto);
        }
        /// <summary>
        /// Updates a user data
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="userDto">UserDTO</param>
        /// <returns></returns>
        [Route("updateuser/{id}")]
        [HttpPut]
        public IActionResult Update(string id, [FromBody]UserDTO userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
            user.UserId = id;

            try
            {
                // save 
                _userService.Update(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Deletes a user entry  
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns></returns>
        [Route("deleteuser/{id}")]
        [HttpDelete]
        public IActionResult Delete(string id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }
}