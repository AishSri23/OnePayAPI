using OnePay_Backend.Helpers;
using OnePay_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePay_Backend.Services
{
    public interface IAccountsService
    {
        List<UserAccount> GetAllAccountOfUser(string userId);
        UserAccount GetById(string userId, string accountId);
        UserAccount Create(UserAccount userAccount);
        void Update(UserAccount userAccount, string userId);
        void Delete(string userId, string accountId);
        bool UpdateBalance(decimal balance, string accountId, string userId, out decimal accBalance);
    }
    public class AccountsService : IAccountsService
    {
        private OnePayContext _context;

        public AccountsService(OnePayContext context)
        {
            _context = context;
        }

        public UserAccount Create(UserAccount userAccount)
        {
            // validation
            if (userAccount == null)
                throw new AppException("No Account");
            userAccount.AccountId = "ONE" + RandomNumerGenerator.GetRandomNumber(8);
            if (_context.UserAccount.Any(x => x.AccountId == userAccount.AccountId))
                throw new AppException("AccountId \"" + userAccount.AccountId + "\" is already taken");

            try
            {
                _context.UserAccount.Add(userAccount);
                _context.SaveChanges();
            }
            catch (AppException ex)
            {
                throw new AppException("User Account not Created", ex.Message);
            }


            return userAccount;
        }

        public void Delete(string userId, string accountId)
        {
            var userAccount = _context.UserAccount.Where(x => x.AccountId == accountId && x.UserId == userId).First();
            if (userAccount != null)
            {
                _context.UserAccount.Remove(userAccount);
                _context.SaveChanges();
            }
        }

        public List<UserAccount> GetAllAccountOfUser(string userId)
        {
            var userAccounts = _context.UserAccount.Where(x => x.UserId == userId).ToList();
            return userAccounts.Count > 0 ? userAccounts : null;
        }

        public UserAccount GetById(string userId, string accountId)
        {
            return _context.UserAccount.Where(x => x.AccountId == accountId && x.UserId == userId).First();
        }

        public void Update(UserAccount userAcc, string userId)
        {
            try
            {
                var userAccount = _context.UserAccount.Where(x => x.AccountId == userAcc.AccountId && x.UserId == userId).First();
                userAccount.AccountName = userAcc.AccountName;
                userAccount.AccountBalance = userAcc.AccountBalance;
                userAccount.InterestRate = userAcc.InterestRate;

                _context.UserAccount.Update(userAccount);
                _context.SaveChanges();
            }
            catch (AppException ex)
            {
                throw new AppException("User Account Not updated", ex.Message);
            }

        }
        public bool UpdateBalance(decimal balance, string accountId, string userId, out decimal accBalance)
        {
            var userAccount = _context.UserAccount.Where(x => x.AccountId == accountId && x.UserId == userId).First();
            accBalance = 0;
            if (userAccount != null && userAccount.AccountBalance > balance)
            {
                userAccount.AccountBalance -= balance;
                try
                {
                    _context.UserAccount.Update(userAccount);
                    _context.SaveChanges();
                }
                catch (AppException ex)
                {
                    throw new AppException("Balance not updated", ex.Message);
                }
                accBalance = (decimal)userAccount.AccountBalance;
                return true;
            }
            else
                return false;


        }
    }
}
