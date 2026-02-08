using System;
using System.Collections.Generic;
using System.Text;

namespace Mf.Core.Interfaces
{
    public interface IEmailSender
    {
        string SendEmail(string subject, string address, string body);
        string SendDataBaseEmail(string subject, string address, string body, string filePath);
    }

    public class EmailConfig
    {
        public string FromAddress { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public string SmtAddress { get; set; }
    }
}
