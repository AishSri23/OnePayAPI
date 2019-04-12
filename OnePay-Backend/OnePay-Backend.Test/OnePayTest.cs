using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OnePay_Backend;
using OnePay_Backend.Controllers;
using OnePay_Backend.Models;
using OnePay_Backend.Services;
using AutoMapper;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using AutoMapper.Configuration;
using WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using OnePay_Backend.DTO;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Linq;
using NBench;
using NUnitPerformanceTesting;

namespace OnePay_Backend.Test.OnePayTests
{
    public class OnePayTest:PerformanceTestSuite<OnePayTest>
    {
        OnePayContext onePayContext;
        Microsoft.Extensions.Configuration.IConfiguration configuration;
        UsersController usersController;
        UserAccountsController userAccountsController;
        BusinessProductsController businessProductsController;
        OnePayController onePayController;
        OnePayTransactionsController onePayTransactionsController;

        public OnePayTest()
        {
            //Context preperation
            configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appSettings.json")
               .Build();
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            var dbOptions = new DbContextOptionsBuilder<OnePayContext>().UseSqlServer(connectionString).Options;
            onePayContext = new OnePayContext(dbOptions);

            //Initialize mapper
            var mappings = new MapperConfigurationExpression();
            mappings.AddProfile<AutoMapperProfile>();
            Mapper.Initialize(mappings);

            IUserService userService = new UserService(onePayContext);
            usersController = new UsersController(userService, Mapper.Instance);

            IAccountsService accountsService = new AccountsService(onePayContext);
            userAccountsController = new UserAccountsController(accountsService, userService, Mapper.Instance);

            IBusinessProductService businessProductService = new BusinessProductService(onePayContext);
            businessProductsController = new BusinessProductsController(businessProductService, userService, Mapper.Instance);

            ITransactionService transactionService = new TransactionService(onePayContext);
            onePayTransactionsController = new OnePayTransactionsController(transactionService, userService, Mapper.Instance);

            IOnePayService onePayService = new OnePayService(onePayContext, businessProductService, userService);
            IMessagingService messagingService = new MessagingService(onePayContext);
            ICoreBankingService coreBankingService = new CoreBankingService(onePayContext, userService, accountsService, businessProductService, messagingService, transactionService);
            onePayController = new OnePayController(transactionService, userService, onePayService,coreBankingService,Mapper.Instance);

            Startup.APIKEY = configuration["APIKEYs:TextLocal"];
            Startup.APIURL = configuration["APIURLs:TextLocalUrl"];
        }
        private Counter _counter;

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _counter = context.GetCounter("TestCounter");
        }
        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Test)]
        [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.LessThan, 300)]
        [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.LessThan, 150)]
        [GcThroughputAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.LessThan, 20)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.LessThan, 50)]
        [Test,Order(1)]
        public void RegisterUser()
        {
            UserDTO testUser = GetUserDTO();
            var result = usersController.Register(testUser) as OkResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);
        }

        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        [Test, Order(2)]
        public void GetUserById()
        {
            IActionResult result = usersController.GetById("9999");
            Assert.AreNotEqual(result, null);
            var user = ((OkObjectResult)result).Value as UserDTO;
            Assert.AreSame(user.UserPhoneNumber, GetUserDTO().UserPhoneNumber);
            Assert.AreSame(user.UserName, GetUserDTO().UserName);
        }

        [Test, Order(3)]
        public void GetUserById_UserNotExists()
        {
            var user = usersController.GetById("0") as OkObjectResult;
            Assert.IsNotNull(user);
            Assert.IsNull(user.Value);
            Assert.AreEqual(user.StatusCode, (int)HttpStatusCode.OK);
        }
        [PerfBenchmark(RunMode = RunMode.Iterations, TestMode = TestMode.Measurement)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        [Test, Order(4)]
        public void Authenticate_ValidCredentials()
        {
            var user = GetAuthDTO();
            var result = usersController.Authenticate(user) as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Test, Order(5)]
        public void Authenticate_InValidCredentials()
        {
            var user = GetIncorrectAuthDTO();
            IActionResult result = usersController.Authenticate(user);
            Assert.AreEqual(((BadRequestObjectResult)result).StatusCode, (int)HttpStatusCode.BadRequest);
        }
        [PerfBenchmark(Description = "You can write your description here.",
            NumberOfIterations = 5, RunMode = RunMode.Throughput, RunTimeMilliseconds = 2500, TestMode = TestMode.Test)]
        [MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.LessThanOrEqualTo, ByteConstants.SixtyFourKb)]
        [Test, Order(6)]
        public void GetAllUsers()
        {
            var result = usersController.GetAll() as OkObjectResult;
            var userList = (List<UserDTO>)result.Value;
            Assert.Greater(userList.Count, 0);
        }

        [Test, Order(7)]
        public void UpdateUser()
        {
            var userPhoneNumber = "9444444444";
            var testUser = GetUserDTO();
            testUser.UserPhoneNumber = userPhoneNumber;
            var result = usersController.Update(testUser.UserId,testUser) as OkResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }
        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.LessThanOrEqualTo, ByteConstants.ThirtyTwoKb)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 0.0d)]
        [Test, Order(8)]
        public void CreateUserAccount()
        {
            var result = userAccountsController.Create(GetUserAccountDTO()) as OkResult;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Test, Order(9)]
        public void GetUserAccountById()
        {
            var userAccountTest = GetUserAccountDTO();
            var accountId = GetAccountIdByUser(userAccountTest.UserId);
            var result = userAccountsController.Get(userAccountTest.UserId, accountId) as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(((UserAccountDTO)result.Value).AccountName, userAccountTest.AccountName);
            Assert.AreEqual(((UserAccountDTO)result.Value).UserId, userAccountTest.UserId);
        }

        [Test, Order(10)]
        public void GetAllUserAccounts()
        {
            var user = GetUserDTO();
            var result = userAccountsController.GetAll(user.UserId) as OkObjectResult;
            var userAccountList = (List<UserAccountDTO>)result.Value;
            Assert.Greater(userAccountList.Count, 0);
        }

        
        [Test, Order(11)]
        public void UpdateUserAccount()
        {
            var user = GetUserDTO();
            var userAccounts = (userAccountsController.GetAll(user.UserId) as OkObjectResult).Value as List<UserAccountDTO>;
            var selectedUserAccount = userAccounts.Find(u => u.UserId == user.UserId);
            var newIntrestRate = 3;
            selectedUserAccount.InterestRate = newIntrestRate;
            var result = userAccountsController.Put(user.UserId, selectedUserAccount) as OkResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode,(int)HttpStatusCode.OK);
        }

        [Test, Order(12)]
        public void CreateBusinessProduct()
        {
            var result = businessProductsController.Create(GetBusinessProductDTO()) as OkResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Test, Order(13)]
        public void GetBusinessProdcutById()
        {
            var businessProduct = GetBusinessProductDTO();
            var result = businessProductsController.Get(businessProduct.UserId, businessProduct.PaccountId) as OkObjectResult;
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(((BusinessProductDTO)result.Value).BproductName, businessProduct.BproductName);
            Assert.AreEqual(((BusinessProductDTO)result.Value).MonthlyCyclePayment, businessProduct.MonthlyCyclePayment);
            Assert.AreEqual(((BusinessProductDTO)result.Value).MinimumPayment, businessProduct.MinimumPayment);

        }

        [Test, Order(14)]
        public void GetAllBusinessProducts()
        {
            var user = GetUserDTO();
            var result = businessProductsController.GetAll(user.UserId) as OkObjectResult;
            var businessProductList = (List<BusinessProductDTO>)result.Value;
            Assert.Greater(businessProductList.Count, 0);
        }

        [Test, Order(15)]
        public void UpdateBusinessProduct()
        {
            var user = GetUserDTO();
            var businessProduct = GetBusinessProductDTO();
            var minPayment = 32000;
            businessProduct.MinimumPayment = minPayment;
            var result = businessProductsController.Put(user.UserId, businessProduct) as OkResult;
            Assert.IsNotNull(result, null);
            Assert.AreEqual(result.StatusCode,(int)HttpStatusCode.OK);
        }

        [Test, Order(16)]
        public void RegisterUserForOnePay()
        {
            var user = GetUserDTO();
            var result = onePayController.RegisterUserForOnePay(user.UserId) as OkResult;
            Assert.IsNotNull(result, null);
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Test, Order(17)]
        public void RegisterUserForOnePay_AlreadyRegisteredUserError()
        {
            var user = GetUserDTO();
            var result = onePayController.RegisterUserForOnePay(user.UserId) as OkObjectResult;
            Assert.IsNotNull(result, null);
            Assert.AreEqual(result.Value, "User Already registered for OnePay");
        }

        [Test, Order(18)]
        public void GetAccountsForOnePay()
        {
            var user = GetUserDTO();
            var result = onePayController.GetAccountsForOnePay(user.UserId) as OkObjectResult;
            Assert.IsNotNull(result, null);
            var businessProductList = (List<BusinessProductDTO>)result.Value;
            Assert.Greater(businessProductList.Count, 0);
        }

        [Test, Order(19)]
        public void GetDue()
        {
            var user = GetUserDTO();
            var result = onePayController.GetDue(user.UserId) as OkObjectResult;
            Assert.IsNotNull(result, null);
            Assert.AreEqual(result.Value, 10000);
        }

        [Test, Order(20)]
        public void GetModifiedDue()
        {
            var user = GetUserDTO();
            var transactionDTO = GetTransactionItemDTOs().First();
            var result = onePayController.GetModifiedDue(transactionDTO) as OkObjectResult;
            Assert.AreEqual(result.Value, 10000);
        }

        [Test, Order(21)]
        public void PayDueAmount()
        {
            var transactionDTOs = GetTransactionItemDTOs();
            var result = onePayController.PayDueAmount(transactionDTOs) as OkResult;
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Test, Order(22)]
        public void GetOnePayTransactionForUser()
        {
            var user = GetUserDTO();
            var result = onePayTransactionsController.GetOnePayTransactionForUser(user.UserId) as OkObjectResult;
            Assert.IsNotNull(result, null);
            var transactionList = (List<TransactionDTO>)result.Value;
            Assert.Greater(transactionList.Count, 0);
        }

        [Test, Order(23)]
        public void DeleteBusinessProduct()
        {
            var userAccount = GetUserAccountDTO();
            var businessProduct = GetBusinessProductDTO();
            DeleteOnePayTransaction(userAccount.UserId);
            DeleteTransactionMessages(userAccount.UserId);
            var result = businessProductsController.Delete(businessProduct.UserId, businessProduct.PaccountId) as OkResult;
            Assert.IsNotNull(result, null);
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Test, Order(24)]
        public void DeleteUserAccount()
        {
            var userAccount = GetUserAccountDTO();
            var result = userAccountsController.Delete(userAccount.UserId, GetAccountIdByUser(userAccount.UserId)) as OkResult;
            Assert.IsNotNull(result, null);
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Test, Order(25)]
        public void DeleteUser()
        {
            var testUser = GetUserDTO();
            var result = usersController.Delete(testUser.UserId) as OkResult;
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.OK);
        }

        #region TestData
        private UserDTO GetUserDTO()
        {
            UserDTO userObj = new UserDTO
            {
                UserId = "9999",
                Password = "12345",
                UserPhoneNumber = "9645856865",
                UserName = "Test User",
                UserEmailAddress = "test@gmail.com",
                IsRegisteredForOnePay = false
            };
            return userObj;
        }
        private AuthDTO GetAuthDTO()
        {
            AuthDTO authObj = new AuthDTO
            {
                id="9999",
                password = "12345"
            };
            return authObj;
        }
        private AuthDTO GetIncorrectAuthDTO()
        {
            AuthDTO authObj = new AuthDTO
            {
                id = "9999",
                password = "1234"
            };
            return authObj;
        }

        private UserAccountDTO GetUserAccountDTO()
        {
            UserAccountDTO userAccountObj = new UserAccountDTO
            {
                AccountName = "Calvin",
                AccountBalance = 234500,
                AccountTypeId = 12,
                InterestRate = 2,
                Timestamp = BitConverter.GetBytes(1552474127),
                UserId = "9999"
            };
            return userAccountObj;
        }

        private BusinessProductDTO GetBusinessProductDTO()
        {
            BusinessProductDTO businessProductObj = new BusinessProductDTO
            {
                UserId = "9999",
                PaccountId = "53126231237",
                BproductName = "Gold Loan",
                BproductTypeId = 6,
                Balance = 36000,
                CanBeModified = true,
                MinimumPayment = 30000,
                MonthlyCyclePayment = 10000,
                PaymentCycleTypeId = 1,
                TotalPayment = 50000
            };
            return businessProductObj;
        }

        private List<TransactionItemDTO> GetTransactionItemDTOs()
        {
            List<TransactionItemDTO> transactionDTOs = new List<TransactionItemDTO>();
            TransactionItemDTO transactionDTO = new TransactionItemDTO()
            {
                AccountId = GetAccountIdByUser("9999"),
                TransactionAmount = 10000,
                UserId = "9999",
                PAccountId = "53126231237",
                TransactionProviderType = 1,
                TransactionType = 1
            };

            TransactionItemDTO transactionDTOSecond = new TransactionItemDTO()
            {
                AccountId = GetAccountIdByUser("9999"),
                TransactionAmount = 5000,
                UserId = "9999",
                PAccountId = "53126231237",
                TransactionProviderType = 1,
                TransactionType = 1
            };

            transactionDTOs.Add(transactionDTO);
            transactionDTOs.Add(transactionDTOSecond);
            return transactionDTOs;
        }

      

        //private List<TransactionDTO> GetTransactionDTOs()
        //{
        //    List<TransactionDTO> transactionDTOs = new List<TransactionDTO>();
        //    TransactionDTO transactionDTO = new TransactionDTO()
        //    {
        //        AccountId = "12345678911",
        //        PaccountId = "531262312368",
        //        Timestamp = BitConverter.GetBytes(1552474127),
        //        TransactionAmount = 10000,
        //        TransactionComment = "Paid Loan amount",
        //        TransactionId = "99999",
        //        TransactionProvideTypeId = 1,
        //        TransactionStatusId = 1,
        //        TransactionTypeId = 1,
        //        UserId = "9999"
        //    };

        //    TransactionDTO transactionDTOSecond = new TransactionDTO()
        //    {
        //        AccountId = "12345678911",
        //        PaccountId = "531262312368",
        //        Timestamp = BitConverter.GetBytes(1552474127),
        //        TransactionAmount = 5000,
        //        TransactionComment = "Paid Loan amount",
        //        TransactionId = "99999",
        //        TransactionProvideTypeId = 1,
        //        TransactionStatusId = 1,
        //        TransactionTypeId = 1,
        //        UserId = "9999"
        //    };

        //    transactionDTOs.Add(transactionDTO);
        //    transactionDTOs.Add(transactionDTOSecond);
        //    return transactionDTOs;
        //}

        #endregion TestData

        private string GetAccountIdByUser(string userId)
        {
            string accountId;
            var userAccounts = (userAccountsController.GetAll(userId) as OkObjectResult).Value as List<UserAccountDTO>;
            accountId = userAccounts.Find(x => x.UserId == userId).AccountId;
            return accountId;
        }

        private void DeleteOnePayTransaction(string userid)
        {
            var testTransactionList = onePayContext.OnePayTransaction.Where(x => x.UserId == userid);
            if(testTransactionList != null)
            {
                onePayContext.OnePayTransaction.RemoveRange(testTransactionList);
                onePayContext.SaveChanges();
            }
        }

        private void DeleteTransactionMessages(string userid)
        {
            var testTransactionMessageList = onePayContext.Message.Where(x => x.UserId == userid);
            if (testTransactionMessageList != null)
            {
                onePayContext.Message.RemoveRange(testTransactionMessageList);
                onePayContext.SaveChanges();
            }
        }
    }
}

