using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace emirates_ftp_app.EmailRepository.EmailSendService
{
    internal class EmailSendService : IEmailSendService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _configuration;

        public EmailSendService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsyncScheduler(string from, List<string> to, string subject, string body, List<Attachment> attachments)
        {
            string smtpServer = _configuration["EmailSettings:Server"]!;
            int smtpPort = int.Parse(_configuration["EmailSettings:Port"]!);
            string senderName = _configuration["EmailSettings:SenderName"] ?? "";
            string fromAddress = from ?? "noreply@company.com";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(from!, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            foreach (var recipient in to)
            {
                mailMessage.To.Add(recipient);
            }

            // Add file attachments
            foreach (var attachment in attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }

            using System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = false, 
                UseDefaultCredentials = false, 
                Credentials = CredentialCache.DefaultNetworkCredentials  
            };

            try
            {
                await smtp.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"SMTP Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }
        }
    }
}
