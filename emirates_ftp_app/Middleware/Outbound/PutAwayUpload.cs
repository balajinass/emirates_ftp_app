using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Common;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Repository.CommonFunctions;
using emirates_ftp_app.Repository.Customer;
using emirates_ftp_app.Repository.FtpConnection;
using emirates_ftp_app.Repository.Oubound.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace emirates_ftp_app.Middleware.Outbound
{
    internal class PutAwayUpload
    {
        private readonly ILogger<PutAwayUpload> _logger;
        private readonly IGetCustomer oCommonManager_;
        private readonly FtpConnection oFtp_;
        private readonly ICommonDao oComm_;
        private readonly IConfiguration _config;
        private readonly ICommonFunctions oCommon_;

        public PutAwayUpload(ILogger<PutAwayUpload> logger, IOptions<AppSettings> appSettings, IGetCustomer getCustomer, FtpConnection oftp, ICommonDao oComm, IConfiguration config,ICommonFunctions oCommon)
        {
            _logger = logger;
            oCommonManager_ = getCustomer;
            oFtp_ = oftp;
            oComm_ = oComm;
            _config = config;
            oCommon_ = oCommon;
        }

        #region PutAwayUploadFileOld
        public async Task PutAwayUploadFileOld(string module)
        {
            try
            {
                var customers = await oCommonManager_.GetListofCustomerOUTBOUND();

                if (customers == null || customers.Count == 0)
                {
                    MyLogger.GetInstance().Info("Number of Customers: 0");
                    return;
                }

                MyLogger.GetInstance().Info("Number of Customers: " + customers.Count);

                foreach (var customer in customers)
                {
                    var filesToProcess = await oComm_.GetFileContent(customer, module);

                    MyLogger.GetInstance().Info("PROJECT-NAME: " + customer.PROJECT_NAME);

                    if (customer.OUTBOUND == null) continue;

                    var moduleConfig = customer.OUTBOUND.FirstOrDefault(m => m.MODULE_NAME == module);
                    if (moduleConfig == null) continue;

                    var credentials = new NetworkCredential(customer.FTP_USERNAME, customer.FTP_PASSWORD);

                    if (filesToProcess == null || filesToProcess.Count == 0)
                    {
                        MyLogger.GetInstance().Info($"{module} - No files to process for PROJECT-NAME: {customer.PROJECT_NAME}");                       
                        continue;
                    }

                    var emailRequests = new List<email_request_model>();

                    foreach (var file in filesToProcess)
                    {
                        bool success = await oFtp_.FileContentMoveFtp(false, file.FILE_NAME!, file.FILE_CONTENT!, customer, moduleConfig, credentials);

                        if (success)
                        {
                            await oComm_.UpdateFileStatus(file.FILE_NAME!,customer.PROJECT_NAME!);                           
                            MyLogger.GetInstance().Info($"File {file.FILE_NAME} marked as processed.");
                        }
                        else
                        {                            
                            MyLogger.GetInstance().Error($"Failed to move file {file.FILE_NAME} via FTP.");
                        }

                        //var fileData = await oCommonManager_.GetEdiFileAsEmailRequestAsync(file.FILE_NAME!, module);
                        //if (fileData != null)
                        //{
                        //    emailRequests.Add(fileData);
                        //}
                    }

                    if (emailRequests.Count > 0)
                    {
                        //var summaryHtml = await oCommon_.GenerateSummaryEmailHtml(emailRequests, module);
                       // await oCommon_.SendFinalMail($"Processing Files - {module}", summaryHtml);
                    }
                }
            }
            catch (Exception ex)
            {
                #region exception_email_log

                string exceptionHtml = await oCommon_.GenerateExceptionHtml("PutAwayUploadFile", ex);
                //await oCommon_.SendFinalMail("PutAwayUploadFile", exceptionHtml);

                #endregion

                MyLogger.GetInstance().Error("Error in PutAway Upload File: " + ex);
            }
        }
        #endregion

        #region PutAwayUploadFile
        public async Task<(List<email_request_model> Success, List<string> Errors)> PutAwayUploadFile(string module)
        {
            var emailRequests = new List<email_request_model>();
            var errors = new List<string>();

            try
            {
                var customers = await oCommonManager_.GetListofCustomerOUTBOUND();

                if (customers == null || customers.Count == 0)
                {
                    MyLogger.GetInstance().Info("Number of Customers: 0");
                    return (emailRequests, errors);
                }

                MyLogger.GetInstance().Info("Number of Customers: " + customers.Count);               

                foreach (var customer in customers)
                {
                    MyLogger.GetInstance().Info("PROJECT-NAME: " + customer.PROJECT_NAME);

                    if (customer.OUTBOUND == null) continue;

                    var moduleConfig = customer.OUTBOUND.FirstOrDefault(m => m.MODULE_NAME == module);
                    if (moduleConfig == null) continue;

                    var credentials = new NetworkCredential(customer.FTP_USERNAME, customer.FTP_PASSWORD);

                    var filesToProcess = await oComm_.GetFileContent(customer, module);

                    MyLogger.GetInstance().Info($"PROJECT-NAME: {customer.PROJECT_NAME} | MODULE: {module} | Total Files to Process from WMS_EDI_FTP: {filesToProcess?.Count ?? 0}");

                    if (filesToProcess == null || filesToProcess.Count == 0)
                    {
                        MyLogger.GetInstance().Info($"{module} - No files to process for PROJECT-NAME: {customer.PROJECT_NAME}");                       
                        continue;
                    }

                    int fileCount = 0;
                    foreach (var file in filesToProcess)
                    {
                        fileCount++;

                        MyLogger.GetInstance().Info($"*************** Processing File - {fileCount} of {filesToProcess.Count} ***************");
                        MyLogger.GetInstance().Info($"SLNO : {file.SL_NO} | " + $"FILE_NAME : {file.FILE_NAME} | " + $"FILE_UPLOAD_TIME : {file.FILE_UPLOAD_TIME} | " +
                           $"FILE_TYPE : {file.FILE_TYPE} | " + $"FILE_STATUS : {file.FILE_STATUS} | " +
                           $"TRANSACTION_TIME : {file.TRANSACTION_TIME} | " + $"PRIMARY_COMPANY : {file.PRIMARY_COMPANY}");
                        MyLogger.GetInstance().Info($"FILE_NAME : {file.FILE_NAME} : " + " Generated from FILE_CONTENT");

                        bool success = await oFtp_.FileContentMoveFtp(false, file.FILE_NAME!, file.FILE_CONTENT!, customer, moduleConfig, credentials);

                        if (success)
                        {
                            await oComm_.UpdateFileStatus(file.FILE_NAME!, customer.PROJECT_NAME!);

                            MyLogger.GetInstance().Info($"File {file.FILE_NAME} marked as processed.");
                           
                            var fileData = await oCommonManager_.GetEdiFileAsEmailRequestAsync(file.FILE_NAME!, module,"COMPLETED");
                            if (fileData != null)
                                emailRequests.Add(fileData);
                        }
                        else
                        {
                            errors.Add($"Failed to move file {file.FILE_NAME} via FTP for PROJECT-NAME: {customer.PROJECT_NAME}");

                            MyLogger.GetInstance().Error($"Failed to move file {file.FILE_NAME} via FTP.");
                           
                            var fileDataerr = await oCommonManager_.GetEdiFileAsEmailRequestAsync(file.FILE_NAME!, module, "PENDING");
                            if (fileDataerr != null)
                                emailRequests.Add(fileDataerr);                           

                        }

                        
                    }
                }

                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;               
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Info("PutAway OutBound Completed");

            }
            catch (Exception ex)
            {
                errors.Add($"Unhandled exception in PutAway Upload File: {ex.Message}");
                MyLogger.GetInstance().Error("Error in PutAway Upload File: " + ex.ToString());
            }

            return (emailRequests, errors);
        }
        #endregion
    }
}
