using emirates_ftp_app.EmailRepository.EmaillogInterface;
using emirates_ftp_app.EmailRepository.EmailSendService;
using emirates_ftp_app.EmailRepository.EmailTemplateServices;
using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Email;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace emirates_ftp_app.EmailRepository.EmailSendWithLogService
{
    internal class EmailSendWithLogService : IEmailSendWithLogService
    {
        private readonly IEmailSendService _emailService;
        private readonly IEmaillogInterface _emailLogRepository;
        private readonly EmailTemplateService _emailTemplateService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public EmailSendWithLogService(IEmailSendService emailService, IEmaillogInterface emailLogRepository, EmailTemplateService emailTemplateService)
        {
            _emailService = emailService;
            _emailLogRepository = emailLogRepository;
            _emailTemplateService = emailTemplateService;
        }


        public async Task ProcessPendingEmails()
        {
            int logId = 0;
            var pendingEmails = await _emailLogRepository.GetPendingEmailsAsync();

            foreach (var email in pendingEmails)
            {
                try
                {
                    _logger.Info("From - {From}, To - {To}, Subject - {Subject}, Body - {Body}, Attachments - {Attachments}", email.FROM_EMAIL, email.TO_EMAIL, email.SUBJECT, email.BODY, email.ATTACHMENTS);
                    #region Insert_Concepts_For_Log
                    //var emailLog = new Email_Scheduler
                    //{
                    //    CREATE_USER = email.CREATE_USER,
                    //    CREATE_DATE = email.CREATE_DATE,
                    //    RUN_USER = email.RUN_USER,
                    //    RUN_DATE = email.RUN_DATE,
                    //    TYPE = email.TYPE,
                    //    FROM_EMAIL = email.FROM_EMAIL,
                    //    TO_EMAIL = email.TO_EMAIL,
                    //    SUBJECT = email.SUBJECT,
                    //    BODY = email.BODY,
                    //    STATUS = email.STATUS
                    //};

                    //logId = await _emailLogRepository.CreateEmailLogAsync(emailLog);

                    // _logger.Info($"Insert in WEB_EMAIL_LOG - ", logId, nameof(ProcessPendingEmails));
                    #endregion

                    var emailTemplate = await _emailLogRepository.GetEmailTemplateByIdAsync(email.TEMPLATE_ID);

                    if (emailTemplate == null)
                    {

                        email.BODY = email.BODY ?? "Default email content";
                    }
                    else
                    {
                        email.BODY = _emailTemplateService.GetFormattedEmailBody(emailTemplate.TEMPLATE!);
                    }
                    var attachments = await _emailLogRepository.GetEmailAttachmentsAsync(email.ID);
                    #region Inser_Concept_Attachment_Log
                    //var emailAttachmentlst = new List<Email_attachment_Scheduler>();

                    //foreach (var attachment in attachments)
                    //{
                    //    var newAttachment = new Email_attachment_Scheduler
                    //    {
                    //        HEADER_ID = logId,
                    //        CREATE_USER = attachment.CREATE_USER,
                    //        CREATE_DATE = attachment.CREATE_DATE,
                    //        RUN_USER = attachment.RUN_USER,
                    //        RUN_DATE = attachment.RUN_DATE,
                    //        FILE_NAME = attachment.FILE_NAME,
                    //        FILE_TYPE = attachment.FILE_TYPE,
                    //        FILE_SIZE = attachment.FILE_SIZE,
                    //        FILE_CONTENT = attachment.FILE_CONTENT
                    //    };
                    //    emailAttachmentlst.Add(newAttachment);
                    //}

                    //if (attachments.Count > 0)
                    //{
                    //    await _emailLogRepository.InsertAttachmentsAsync(emailAttachmentlst);
                    //    logger.Info($"Insert in WEB_EMAIL_ATTACHMENT_LOG - ", logId, " - Header ID", nameof(ProcessPendingEmails));
                    //}
                    #endregion
                    var toList = email.TO_EMAIL?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

                    if (!toList.Any())
                    {
                        await _emailLogRepository.UpdateEmailLogStatusAsync(
                            email.ID,
                            "FAILURE",
                            "400",
                            "No valid recipients found."
                        );
                        _logger.Info($"Update in WEB_EMAIL_LOG - ", logId, nameof(ProcessPendingEmails));
                        continue;
                    }

                    var emailAttachments = new List<Attachment>();

                    foreach (var attachment in attachments)
                    {
                        if (attachment.FILE_CONTENT == null || attachment.FILE_CONTENT.Length == 0)
                        {
                            _logger.Warn($"Attachment {attachment.FILE_NAME} is empty. Skipping.");
                            continue;
                        }

                        var stream = new MemoryStream(attachment.FILE_CONTENT);
                        var contentType = string.IsNullOrEmpty(attachment.FILE_TYPE)
                            ? "application/octet-stream"
                            : attachment.FILE_TYPE;

                        var emailAttachment = new Attachment(stream, attachment.FILE_NAME, contentType);
                        emailAttachments.Add(emailAttachment);

                        _logger.Info($"Attachment added: {attachment.FILE_NAME}, Size: {attachment.FILE_CONTENT.Length} bytes");
                    }

                    await _emailService.SendEmailAsyncScheduler(
                        email.FROM_EMAIL!,
                        toList,
                        email.SUBJECT!,
                        email.BODY!,
                        emailAttachments
                    );

                    //await _emailLogRepository.UpdateEmailLogStatusAsync(
                    //    email.ID,
                    //    "SUCCESS",
                    //    "200",
                    //    null
                    //);
                    //logger.Info($"Update in WEB_EMAIL_LOG - ", logId, nameof(ProcessPendingEmails));

                    await _emailLogRepository.UpdateEmailLogScheduler(
                        email.ID,
                        "SUCCESS",
                        "200",
                        null
                    );
                    _logger.Info($"Update in WEB_EMAIL_SCHEDULER - ", email.ID, nameof(ProcessPendingEmails));
                }
                catch (SmtpException smtpEx)
                {
                    _logger.Error(smtpEx, $"Error in Send Mail - ID", logId, nameof(ProcessPendingEmails));
                    await _emailLogRepository.UpdateEmailLogStatusAsync(
                        email.ID,
                        "FAILURE",
                        "500",
                        smtpEx.Message
                    );
                    // Optionally log the detailed error here (for debugging purposes)
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error in Send Mail - ID", logId, nameof(ProcessPendingEmails));
                    await _emailLogRepository.UpdateEmailLogStatusAsync(
                        email.ID,
                        "FAILURE",
                        "500",
                        ex.Message
                    );
                }
            }
        }

        public async Task ProcessErrorEmails(Email_Scheduler request)
        {
            var pendingEmails = await _emailLogRepository.GetPendingErrorEmails();

            try
            {
                foreach (var email in pendingEmails)
                {
                    var toList = email.TO_EMAIL?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                                       
                    await _emailService.SendEmailAsyncScheduler(
                        email.FROM_EMAIL!, 
                        toList,            
                        request.SUBJECT!,    
                        request.BODY!,       
                        null               
                    );

                    // Update the email log status to SUCCESS
                    await _emailLogRepository.UpdateEmailLogStatusAsync(email.ID, "SUCCESS", "200", null);
                }

            }
            catch (Exception ex)
            {                
                MyLogger.GetInstance().Error($"Error sending email: {ex.Message}");                
                if (pendingEmails.Any())
                {
                    var lastEmail = pendingEmails.Last(); 
                    await _emailLogRepository.UpdateEmailLogStatusAsync(lastEmail.ID, "FAILURE", "500", ex.Message);
                }                
                throw;
            }
        }
    }
}
