using emirates_ftp_app.Data;
using emirates_ftp_app.Model.Email;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace emirates_ftp_app.EmailRepository.EmaillogInterface
{
    internal class EmaillogInterface : IEmaillogInterface
    {
        private readonly EmailDbContext _context;

        public EmaillogInterface(EmailDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateEmailLogAsync(Email_Scheduler emailLog)
        {
            try
            {
                _context.WEB_EMAIL_SCHEDULER!.Add(emailLog);
                await _context.SaveChangesAsync();
                return emailLog.ID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task InsertAttachmentsAsync(List<Email_attachment_Scheduler> attachments)
        {
            try
            {
                if (attachments.Any())
                {
                    _context.WEB_EMAIL_ATTACHMENT_SCHEDULER!.AddRange(attachments);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public async Task UpdateEmailLogStatusAsync(int logId, string status, string statusCode, string? errorMessage)
        {
            try
            {
                var log = await _context.WEB_EMAIL_SCHEDULER!.FindAsync(logId);
                if (log != null)
                {
                    log.STATUS = status;
                    log.STATUS_CODE = statusCode;
                    log.ERROR_MESSAGE = errorMessage;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public async Task<List<Email_Scheduler>> GetPendingEmailsAsync()
        {
            return await _context.WEB_EMAIL_SCHEDULER!
                .Where(e => e.STATUS == "PENDING")
                .ToListAsync();
        }

        public async Task<List<Email_Scheduler>> GetPendingErrorEmails()
        {
            return await _context.WEB_EMAIL_SCHEDULER!
                .Where(e => e.TYPE == "ERRORMSG")
                .ToListAsync();
        }

        public async Task<List<Email_attachment_Scheduler>> GetEmailAttachmentsAsync(int emailId)
        {
            return await _context.WEB_EMAIL_ATTACHMENT_SCHEDULER!
                .Where(a => a.HEADER_ID == emailId)
                .ToListAsync();
        }

        public async Task UpdateEmailLogScheduler(int logId, string status, string statusCode, string? errorMessage)
        {
            var log = await _context.WEB_EMAIL_SCHEDULER!.FindAsync(logId);
            if (log != null)
            {
                log.STATUS = status;
                log.STATUS_CODE = statusCode;
                log.ERROR_MESSAGE = errorMessage;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<EmailTemplate?> GetEmailTemplateByIdAsync(int templateId)
        {
            return await _context.WEB_EMAIL_TEMPLATE_MASTER!
                .Where(t => t.ID == templateId)
                .FirstOrDefaultAsync();
        }
    }
}
