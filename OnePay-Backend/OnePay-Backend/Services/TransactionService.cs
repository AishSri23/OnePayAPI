using Microsoft.EntityFrameworkCore;
using OnePay_Backend.DTO;
using OnePay_Backend.Helpers;
using OnePay_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OnePay_Backend.Models.OnePayEnums;

namespace OnePay_Backend.Services
{
    public interface ITransactionService
    {
        IEnumerable<OnePayTransaction> GetAll(User user);
        OnePayTransaction GetById(string userId, string tid);
        OnePayTransaction SaveTransaction(TransactionItemDTO transaction, bool status);
    }
    public class TransactionService : ITransactionService
    {
        private OnePayContext _context;
        private OnePayTransaction _transaction;

        public TransactionService(OnePayContext context)
        {
            _context = context;
        }

        public IEnumerable<OnePayTransaction> GetAll(User user)
        {
            return _context.OnePayTransaction.Include("User").Include("Account").Include("Paccount").Include("TransactionProvideType")
                .Include("TransactionStatus").Include("TransactionType").Where(x => x.UserId == user.UserId).ToList();

        }

        public OnePayTransaction GetById(string userId, string id)
        {
            return _context.OnePayTransaction.Include("User").Include("Account").Include("Paccount").Include("TransactionProvideType")
                .Include("TransactionStatus").Include("TransactionType").Where(x => x.UserId == userId && x.TransactionId == id).First();
        }
        public OnePayTransaction SaveTransaction(TransactionItemDTO transaction,bool status)
        {
            try
            {
                _transaction = new OnePayTransaction();
                _transaction.TransactionId = "T" + RandomNumerGenerator.GetRandomNumber(10);
                _transaction.TransactionComment = "Transaction from OnePay for AccountId";
                _transaction.TransactionAmount = transaction.TransactionAmount;
                _transaction.UserId = transaction.UserId;
                _transaction.AccountId = transaction.AccountId;
                _transaction.PaccountId = transaction.PAccountId;
                _transaction.TransactionTypeId = transaction.TransactionType;
                _transaction.TransactionStatusId = (status) ? (int)TransactionStatusTypes.Successful : (int)TransactionStatusTypes.Declined;
                _transaction.TransactionProvideTypeId = transaction.TransactionProviderType;
                _transaction.Timestamp = DateTime.Now.ToString();
                if (_transaction != null)
                {
                    _context.OnePayTransaction.Add(_transaction);
                    _context.SaveChanges();
                }
            }
            catch (AppException ex)
            {
                throw new AppException("Transaction not successful", ex.Message);
            }
            return _transaction;
    }
    }
}
