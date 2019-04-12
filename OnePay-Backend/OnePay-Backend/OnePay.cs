using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnePay_Backend.Models;

namespace OnePay_Backend.Controllers
{
    public class OnePay
    {
        private readonly OnePayContext _context;
        public OnePay(OnePayContext context)
        {
            _context = context;
        }

        private List<BusinessProduct> GetAccountsEligibleForOnePay(string userId)
        {
            var accounts = new List<BusinessProduct>();
            var user = _context.User.Where(x => x.UserId == userId).First();

            if (user!=null)
            {
                if (user.IsRegisteredForOnePay)
                {
                    accounts = _context.BusinessProduct.Where(z => z.UserId == user.UserId && z.PaymentCycleTypeId == 1).ToList();
                }

            }
            if (accounts.Count != 0)
            {
                return accounts;
            }
            else
                return null;
        }
        private decimal CalculateTotal(string userId)
        {
            decimal sum = 0;
            var user = _context.User.Where(x => x.UserId == userId).First();
            if(user!=null)
            {
                var accounts = GetAccountsEligibleForOnePay(user.UserId);
                foreach (var account in accounts)
                {
                    sum += (decimal)account.MonthlyCyclePayment;
                }
            }
            
            return sum;
        }
        private bool ModifyBalance(string PAccountId, string userId, decimal amount)
        {
            var account = _context.BusinessProduct.Where(x => x.PaccountId == PAccountId && x.UserId == userId).First();
            if (account != null)
            {
                return (amount >= account.MinimumPayment) ? true : false;
            }
            else
                return false;
        }

        private decimal CalculateModifiedTotalAmount(Dictionary<string,decimal> accountIds,string userId)
        {
            decimal modifiedTotalAmount = 0;
            foreach(KeyValuePair<string,decimal> account in accountIds)
            {
                bool CanBeModified = ModifyBalance(account.Key, userId,account.Value);
                if(CanBeModified)
                {
                    modifiedTotalAmount = ReCalculateTotal(account.Key, userId, account.Value);
                }
            }
            return modifiedTotalAmount;

        }
        private decimal ReCalculateTotal(string accountId,string userId, decimal balance)
        {
            decimal totalAmount = CalculateTotal(userId);
            var modifiedTotalAmount = totalAmount - GetMonthlyCyclePaymentsOfAccount(accountId, userId)+balance;
            return modifiedTotalAmount;
        }
        private decimal GetMonthlyCyclePaymentsOfAccount(string accountId, string userId)
        {
            var account = _context.BusinessProduct.Where(x => x.PaccountId == accountId && x.UserId == userId).First();
            return account != null ? (decimal)account.MonthlyCyclePayment : 0;
        }

        
    }
}
