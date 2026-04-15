using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace emirates_ftp_app.EmailRepository.EmailSendService
{
    internal interface IEmailSendService
    {
        Task SendEmailAsyncScheduler(string from, List<string> to, string subject, string body, List<Attachment> attachments);
    }
}
