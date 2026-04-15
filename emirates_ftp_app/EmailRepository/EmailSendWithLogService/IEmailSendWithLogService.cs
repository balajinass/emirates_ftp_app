using emirates_ftp_app.Model.Email;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.EmailRepository.EmailSendWithLogService
{
    internal interface IEmailSendWithLogService
    {
        Task ProcessPendingEmails();
        Task ProcessErrorEmails(Email_Scheduler content);
    }
}
