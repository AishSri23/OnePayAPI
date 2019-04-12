using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OnePay_Backend.Helpers;
using OnePay_Backend.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;

namespace OnePay_Backend.Services
{
    public interface IMessagingService
    {
        string SendMessage(User user, OnePayTransaction onePayTransaction);
        void SaveMessage(string messageBody, string recipient, string sender, string status, string userId);
        Message GetMessageById(int id,string userId);
        List<Message> GetMessages(string userId);
    }
    public class MessagingService:IMessagingService
    {
        private OnePayContext _context;
        private Message _message;
        public MessagingService(OnePayContext context)
        {
            _context = context;
        }
        public Message GetMessageById(int id, string userId)
        {
            Message message = _context.Message.Include("User").Where(x => x.MessageId == id && x.UserId == userId).First();
            return message != null ? message : null;
        }

        public List<Message> GetMessages(string userId)
        {
            return _context.Message.Include("User").Where(x => x.UserId == userId).ToList();
        }

        public void SaveMessage(string messageBody, string recipient,string sender,string status,string userId)
        {
            try
            {
                _message = new Message();
                _message.MessageBody = messageBody;
                _message.Recipient = recipient;
                _message.Sender = sender;
                _message.UserId = userId;
                _message.Status = status;
                _context.Message.Add(_message);
                _context.SaveChanges();
            }
            catch (AppException ex)
            {
                throw new AppException("Message cannot be saved",ex.Message);
            }
        }

        public string SendMessage(User user, OnePayTransaction onePayTransaction)
        {
            string APIKey = Startup.APIKEY;
            try
            {
                string recipient = user.UserPhoneNumber;
                string sender = "TXTLCL";
                string message = $"{onePayTransaction.TransactionAmount} has been deducted from {onePayTransaction.AccountId}.Message sent by OnePayBank";
                string encodedMessage = HttpUtility.UrlEncode(message);
                var status = "";
                using (var webClient = new WebClient())
                {
                    //    byte[] response = webClient.UploadValues(Startup.APIURL, new NameValueCollection()
                    //{
                    //    {"apikey",APIKey},
                    //    {"numbers",recipient},
                    //    {"message",encodedMessage},
                    //    {"sender",sender}});
                    //    string result = System.Text.Encoding.UTF8.GetString(response);
                    //    var jsonObject = JObject.Parse(result);
                    status = "Success";
                }
                if(status == "Success")
                {
                    SaveMessage(message, recipient, sender, status, user.UserId);
                }
                else
                    SaveMessage(message, recipient, sender, status, user.UserId);
                return status;
            }
            catch (AppException)
            {
                throw new AppException("Message not Delivered");
            }
        }
        
    }
}
