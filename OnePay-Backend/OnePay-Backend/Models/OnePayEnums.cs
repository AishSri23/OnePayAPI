using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OnePay_Backend.Models
{
    public static class OnePayEnums
    {
        public enum UserStatusType
        {

            Active = 3,

            NotActive = 4,
 
            Defaulter =5
        }
        public enum TransactionTypes
        {
            Debit = 1 ,

            Credit = 2
        }
        public enum TransactionStatusTypes
        {
            Successful = 1,
            Declined = 2,
            WaititngForConfirmation = 3
        }
        public enum TransactionStatusProviderTypes
        {
            Cash = 1,
            NetBanking =2,
            OnePay =3
        }
        public enum MessageStatus
        {
            Sent = 1,
            Delivered =2,
            Failed = 3
        }
        public enum PaymentCycleTypes
        {
            Monthly = 1,
            Quarterly = 2,
            Annually = 3 
        }
        public static class ErrorMessage
        {
            public static string balanceIsNotEnough = "Balance is not enough";
            public static string transactionFailed = "Transaction Failed";
        }


    }
}
