using Mf.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Mf.Core.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailConfig _config;
        public EmailSender(EmailConfig config)
        {
            _config = config;
        }

        public string SendEmail(string subject, string address, string body)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_config.FromAddress, _config.Password);
                MailAddress from = new MailAddress(_config.FromAddress);
                MailAddress to = new MailAddress(address);
                MailMessage message = new MailMessage(from, to);
                message.Body = body;
                message.BodyEncoding = Encoding.UTF8;
                message.Subject = subject;
                message.SubjectEncoding = Encoding.UTF8;
                client.Send(message);
                message.Dispose();
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string SendDataBaseEmail(string subject, string address, string body, string filePath)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_config.FromAddress, _config.Password);
                MailAddress from = new MailAddress(_config.FromAddress);
                MailAddress to = new MailAddress(address);
                MailMessage message = new MailMessage(from, to);
                message.Body = body;
                message.BodyEncoding = Encoding.UTF8;
                message.Subject = subject;
                message.SubjectEncoding = Encoding.UTF8;
                if (File.Exists(filePath))
                {
                    Attachment attachment = new Attachment(filePath);
                    message.Attachments.Add(attachment);
                }
                client.Send(message);
                message.Dispose();
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

    }
}
