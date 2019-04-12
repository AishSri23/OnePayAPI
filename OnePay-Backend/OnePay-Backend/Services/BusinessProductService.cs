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
    public interface IBusinessProductService
    {
        IEnumerable<BusinessProduct> GetAllBusinessProductsOfUser(string userId);
        BusinessProduct GetById(string userId, string accountId);
        BusinessProduct Create(BusinessProduct bProduct);
        void Update(BusinessProduct bProduct, string userId);
        void Delete(string userId, string paccountId);
        bool UpdateBalance(decimal balance, string accountId, string userId);
        List<BusinessProduct> GetAccountsWithMonthlyPayment(string userId);
    }
    public class BusinessProductService : IBusinessProductService
    {
        private OnePayContext _context;

        public BusinessProductService(OnePayContext context)
        {
            _context = context;
        }

        public BusinessProduct Create(BusinessProduct bProduct)
        {
            // validation
            if (bProduct == null)
                throw new AppException("No Product");

            if (_context.UserAccount.Any(x => x.AccountId == bProduct.PaccountId))
                throw new AppException("AccountId \"" + bProduct.PaccountId + "\" is already taken");

            _context.BusinessProduct.Add(bProduct);
            _context.SaveChanges();

            return bProduct;
        }

        public void Delete(string userId, string paccountId)
        {
            var bProduct = _context.BusinessProduct.Where(x => x.PaccountId == paccountId && x.UserId == userId).First();
            if (bProduct != null)
            {
                _context.BusinessProduct.Remove(bProduct);
                _context.SaveChanges();
            }
        }

        public IEnumerable<BusinessProduct> GetAllBusinessProductsOfUser(string userId)
        {
            var businessProducts = _context.BusinessProduct.Include("PaymentCycleType").Include("BproductType")
                .Include("User").Where(x => x.UserId == userId).ToList();
            return businessProducts.Count > 0 ? businessProducts : null;
        }

        public BusinessProduct GetById(string userId, string accountId)
        {
            return _context.BusinessProduct.Include(e => e.BproductType).Include("User").Include("PaymentCycleType").Where(x => x.PaccountId == accountId && x.UserId == userId).First();
        }

        public void Update(BusinessProduct businessProduct, string userId)
        {
            try
            {
                var bProduct = _context.BusinessProduct.Include("PaymentCycleType").Include(e => e.BproductType).Where(x => x.PaccountId == businessProduct.PaccountId && x.UserId == userId).First();
                bProduct.Balance = businessProduct.Balance;
                bProduct.MonthlyCyclePayment = businessProduct.MonthlyCyclePayment;
                bProduct.MinimumPayment = businessProduct.MinimumPayment;
                bProduct.CanBeModified = businessProduct.CanBeModified;

                _context.BusinessProduct.Update(bProduct);
            }
            catch (AppException ex)
            {
                throw new AppException("Product updation not successful" , ex.Message);
            }

        }
        public bool UpdateBalance(decimal balance, string accountId, string userId)
        {
            var bProduct = _context.BusinessProduct.Where(x => x.PaccountId == accountId && x.UserId == userId).First();
            if (bProduct != null)
            {
                bProduct.TotalPayment+= balance;
                _context.BusinessProduct.Update(bProduct);
                return true;
            }
            return false;

        }
        public List<BusinessProduct> GetAccountsWithMonthlyPayment(string userId)
        {
            var accounts = _context.BusinessProduct.Where(x => x.PaymentCycleTypeId == (int)PaymentCycleTypes.Monthly && x.UserId == userId).ToList();
            return accounts != null ? accounts : null;
        }
    }
}
