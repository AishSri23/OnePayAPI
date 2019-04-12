using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class Message
    {
        public int MessageId { get; set; }
        public string UserId { get; set; }
        public string MessageBody { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string Status { get; set; }

        public User User { get; set; }
    }
}
