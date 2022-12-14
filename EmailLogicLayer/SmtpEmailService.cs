using EmailLogicLayer;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SendEmailAndSMSConsole
{
    public class SmtpEmailService
    {
        private readonly EmailConfiguration _configuration;
        public SmtpEmailService()
        {
            _configuration = new EmailConfiguration()
            {
                From = "oleksandr.burda@ukr.net",
                SmtpServer = "smtp.ukr.net",
                Port = 2525,
                UserName = "oleksandr.burda@ukr.net",
                Password = "OIA39mPgQp8Xk0we"
            };
        }
        public void Send(Message message, Multipart attachments)
        {
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message.Body;
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_configuration.From));
            emailMessage.To.Add(new MailboxAddress(message.To));
            emailMessage.Subject = message.Subject;

            var multipart = new Multipart("mixed");
            multipart.Add(bodyBuilder.ToMessageBody());
            multipart.Add(attachments);

            emailMessage.Body = multipart;

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_configuration.SmtpServer, _configuration.Port, true);
                    client.Authenticate(_configuration.UserName, _configuration.Password);
                    client.Send(emailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Send message error {0}", ex.Message);
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
