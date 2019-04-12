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
    public interface ICoreBankingService
    {
        void BeginTransaction(List<TransactionItemDTO> transactionsItemList);
        void ExecuteTransaction(TransactionItemDTO transaction, UserAccount userAccount, BusinessProduct bProduct, User user);
        void EndTransaction(OnePayTransaction transaction, User user);
        bool CheckBalanceIsEnough(UserAccount userAccount, decimal transactionAmount);
        bool DebitAmount(string userId, string accountId, decimal transactionAmount);
        bool CreditAmount(string userId, string accountId, decimal transactionAmount);
    }
    public class CoreBankingService:ICoreBankingService
    {
        private OnePayContext _context;
        private IUserService _userService;
        private IAccountsService _accountsService;
        private IBusinessProductService _businessProductService;
        private IMessagingService _messagingService;
        private ITransactionService _transactionService;

        public CoreBankingService(
            OnePayContext context,IUserService userService
            ,IAccountsService accountsService
            ,IBusinessProductService businessProductService
            ,IMessagingService messagingService
            ,ITransactionService transactionService
            )
        {
            _context = context;
            _userService = userService;
            _accountsService = accountsService;
            _businessProductService = businessProductService;
            _transactionService = transactionService;
            _messagingService = messagingService;
        }
        public void BeginTransaction(List<TransactionItemDTO> transactionsItemList)
        {
            if(transactionsItemList != null)
            {  
                try
                {
                    foreach (TransactionItemDTO transaction in transactionsItemList)
                    {
                        if (transaction.AccountId != null)
                        {
                            var user = _userService.GetById(transaction.UserId);
                            if (user != null && user.UserStatusId == (int)UserStatusType.Active)
                            {
                                var userAccount = (transaction.AccountId != null) ? _accountsService.GetById(transaction.UserId, transaction.AccountId) : null;
                                var bProduct = _businessProductService.GetById(transaction.UserId, transaction.PAccountId);
                                if (userAccount != null && bProduct != null && CheckBalanceIsEnough(userAccount, transaction.TransactionAmount))
                                {
                                    ExecuteTransaction(transaction, userAccount, bProduct, user);
                                }
                                else
                                {
                                    throw new AppException("Balance is not enough");
                                }

                            }
                        }
                    }
                }
                catch (AppException ex)
                {
                    throw new AppException("Transaction Failed" + ex);
                }
            }
        }
        public void ExecuteTransaction(TransactionItemDTO transaction,UserAccount userAccount,BusinessProduct bProduct,User user)
        {
            bool debitStatus,creditStatus = false;
            debitStatus = DebitAmount(userAccount.UserId,transaction.AccountId, transaction.TransactionAmount);
            if (debitStatus)
            {
                creditStatus = CreditAmount(userAccount.UserId, transaction.PAccountId, transaction.TransactionAmount);
                OnePayTransaction transactionObj = null;
                //call transaction service
                try
                {
                    transactionObj = _transactionService.SaveTransaction(transaction, creditStatus);
                }
                catch (AppException ex)
                {
                    throw new AppException("Transaction Not Successful", ex.Message);
                }
                if(transactionObj.TransactionId!=null)
                {
                    EndTransaction(transactionObj,user);
                }
            }
            
        }
        public void EndTransaction(OnePayTransaction transaction,User user)
        {
            //call messaging Service
            try
            {
                _messagingService.SendMessage(user, transaction);
            }
            catch ( AppException ex)
            {
                throw new AppException("Message could not be sent" + ex);
            }
        }
        public bool CheckBalanceIsEnough(UserAccount userAccount,decimal transactionAmount)
        {
            if (userAccount.AccountBalance > transactionAmount)
            {
                return true;
            }
            else
                return false;
        }
        public bool DebitAmount(string userId,string accountId, decimal transactionAmount)
        {
            bool debitStatus = false;
            decimal balance = 0;
            debitStatus = _accountsService.UpdateBalance(transactionAmount, accountId, userId,out balance);
            //call messaging service
            return (debitStatus) ? true : false;

        }
        public bool CreditAmount(string userId, string accountId, decimal transactionAmount)
        {
            bool creditStatus = false;
            creditStatus = _businessProductService.UpdateBalance(transactionAmount, accountId, userId);
            return creditStatus ? true : false;
        }

    }
}
