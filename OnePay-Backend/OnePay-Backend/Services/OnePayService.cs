using OnePay_Backend.DTO;
using OnePay_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePay_Backend.Services
{
    public interface IOnePayService
    {
        List<BusinessProduct> GetAccountsEligibleForOnePay(User user);
        decimal CalculateTotal(string userId);
        bool CanModifyBalance(BusinessProduct businessProduct, decimal amount);
        decimal CalculateModifiedTotalAmount(TransactionItemDTO transactionDTOs);
        decimal ReCalculateTotal(TransactionItemDTO transaction);
        decimal GetMonthlyCyclePaymentsOfAccount(string accountId, string userId);

    }
    public class OnePayService:IOnePayService   
    {
        private OnePayContext _context;
        private IBusinessProductService _businessProductService;
        private IUserService _userService;
        public OnePayService(OnePayContext context,IBusinessProductService businessProductService,IUserService userService)
        {
            _context = context;
            _businessProductService = businessProductService;
            _userService = userService;

        }
        public List<BusinessProduct> GetAccountsEligibleForOnePay(User user)
        {
            List<BusinessProduct> bList = null;
            if (user.IsRegisteredForOnePay)
                bList =  _businessProductService.GetAccountsWithMonthlyPayment(user.UserId);
            return bList;
        }
        public decimal CalculateTotal(string userId)
        {
            decimal sum = 0;
            var user = _userService.GetById(userId);
            if (user != null)
            {
                var accounts = GetAccountsEligibleForOnePay(user);
                foreach (var account in accounts)
                {
                    sum += (decimal)account.MonthlyCyclePayment;
                }
            }

            return sum;
        }
        public bool CanModifyBalance(BusinessProduct businessProduct,decimal amount)
        {
            if (businessProduct != null)
            {
                return (amount >= businessProduct.MonthlyCyclePayment) ? true : false;
            }
            else
                return false;
        }
        public decimal CalculateModifiedTotalAmount(TransactionItemDTO transactionDTO)
        {
            decimal modifiedTotalAmount = 0;
            var businessProduct = _businessProductService.GetById(transactionDTO.UserId, transactionDTO.PAccountId);
            bool canBeModified = CanModifyBalance(businessProduct, transactionDTO.TransactionAmount);
            if (canBeModified)
            {
                modifiedTotalAmount += ReCalculateTotal(transactionDTO);
            }
            else
            {
                return 0;
            }
            
            return modifiedTotalAmount;

        }
        public decimal ReCalculateTotal(TransactionItemDTO transaction)
        {
            decimal totalAmount = CalculateTotal(transaction.UserId);
            var modifiedTotalAmount = totalAmount - GetMonthlyCyclePaymentsOfAccount(transaction.PAccountId,transaction.UserId) + transaction.TransactionAmount;
            return modifiedTotalAmount;
        }
        public decimal GetMonthlyCyclePaymentsOfAccount(string accountId, string userId)
        {
            var account = _businessProductService.GetById(userId, accountId);
            return account != null ? (decimal)account.MonthlyCyclePayment : 0;
        }

    }
}
