using emirates_ftp_app.Model.Email;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.EmailRepository.EmaillogInterface
{
    internal interface IEmaillogInterface
    {
        Task<int> CreateEmailLogAsync(Email_Scheduler emailLog);
        Task UpdateEmailLogStatusAsync(int logId, string status, string statusCode, string? errorMessage);
        Task InsertAttachmentsAsync(List<Email_attachment_Scheduler> attachments);
        Task<List<Email_Scheduler>> GetPendingEmailsAsync();
        Task<List<Email_attachment_Scheduler>> GetEmailAttachmentsAsync(int emailId);
        Task UpdateEmailLogScheduler(int logId, string status, string statusCode, string? errorMessage);
        Task<EmailTemplate> GetEmailTemplateByIdAsync(int templateId);
        Task<List<Email_Scheduler>> GetPendingErrorEmails();
    }
}
