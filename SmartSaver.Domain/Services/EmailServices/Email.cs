using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSaver.Domain.Services.EmailServices
{
    public class Email
    {
        public string ReceiveAddress { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
