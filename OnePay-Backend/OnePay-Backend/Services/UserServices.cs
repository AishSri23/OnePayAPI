using Microsoft.EntityFrameworkCore;
using OnePay_Backend.Helpers;
using OnePay_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OnePay_Backend.Models.OnePayEnums;

namespace OnePay_Backend.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(string id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(string id);
        void RegisterUserForOnePay(User userparam);
    }

    public class UserService : IUserService
    {
        private OnePayContext _context;

        public UserService(OnePayContext context)
        {
            _context = context;
        }

        public User Authenticate(string userId, string password)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.User.Include("UserStatus").SingleOrDefault(x => x.UserId == userId && x.Password == password);

            // check if username exists
            if (user == null)
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User;
        }

        public User GetById(string id)
        {
            var user = _context.User.Find(id);
            return user != null ? user : null;
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.User.Any(x => x.UserId == user.UserId))
                throw new AppException("UserID \"" + user.UserId + "\" is already taken");
            user.UserStatusId = (int)UserStatusType.Active;
            user.IsRegisteredForOnePay = false;
            _context.User.Add(user);
            _context.SaveChanges();

            return user;
        }
        public void RegisterUserForOnePay(User userparam)
        {
            var user = _context.User.Find(userparam.UserId);
            if(!user.IsRegisteredForOnePay)
            {
                if(user.UserStatusId == (int)UserStatusType.Active)
                {
                    user.IsRegisteredForOnePay = true;
                    _context.User.Update(user);
                    _context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Update User details
        /// </summary>
        /// <param name="userParam">User object</param>
        /// <param name="password">password</param>
        public void Update(User userParam, string password = null)
        {
            var user = _context.User.Find(userParam.UserId);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.UserName != user.UserName)
            {
                // username has changed so check if the new username is already taken
                if (_context.User.Any(x => x.UserName == userParam.UserName))
                    throw new AppException("Username " + userParam.UserName + " is already taken");
            }

            // update user properties
            user.UserName = userParam.UserName;
            user.UserPhoneNumber = userParam.UserPhoneNumber;
            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                user.Password = password;
            }

            _context.User.Update(user);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete user by user id
        /// </summary>
        /// <param name="id">User Id</param>
        public void Delete(string id)
        {
            var user = _context.User.Find(id);
            if (user != null)
            {
                _context.User.Remove(user);
                _context.SaveChanges();
            }
        }

    }
}
