using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Common;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Repository.CommonFunctions;
using emirates_ftp_app.Repository.Customer;
using emirates_ftp_app.Repository.FtpConnection;
using emirates_ftp_app.Repository.Inbound.CustomerCreation;
using emirates_ftp_app.Repository.Inbound.SoCancel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace emirates_ftp_app.Middleware.Inbound
{
    internal class CustomerCreation
    {
        private readonly ILogger<CustomerCreation> _logger;
        private readonly AppSettings _appSettings;
        private readonly IGetCustomer oCommonManager_;
        private readonly FtpConnection oFtp_;
        private readonly ICommonFunctions oCommon_;
        private readonly ICusCreationDao CusCreationDao_;
        public CustomerCreation(ILogger<CustomerCreation> logger,
                IOptions<AppSettings> appSettings,
               IGetCustomer getCustomer, FtpConnection oftp, ICommonFunctions oCommon, ICusCreationDao oCusCreationDao)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            oCommonManager_ = getCustomer;
            oFtp_ = oftp;
            oCommon_ = oCommon;
            CusCreationDao_ = oCusCreationDao;
        }

        #region CustomerCreation
        public async Task<(List<email_request_model> Success, List<string> Errors)> CustomerCreations(string module)
        {
            var allEmailRequests = new List<email_request_model>();
            var allErrors = new List<string>();

            try
            {
                var listOfCustomers = await oCommonManager_.GetListofCustomer();
                if (listOfCustomers == null || listOfCustomers.Count == 0)
                {
                    MyLogger.GetInstance().Info("Number of Customers: 0");
                    Console.WriteLine("Number of Customers: 0");
                    return (allEmailRequests, allErrors);
                }

                MyLogger.GetInstance().Info("Number of Customers: " + listOfCustomers.Count);
                Console.WriteLine("Number of Customers: " + listOfCustomers.Count);

                foreach (var customer in listOfCustomers)
                {
                    try
                    {
                        MyLogger.GetInstance().Info("PROJECT-NAME: " + customer.PROJECT_NAME);
                        Console.WriteLine("PROJECT-NAME: " + customer.PROJECT_NAME);

                        var customerModule = customer.MODULES?.FirstOrDefault(m => m.MODULE_NAME == module);
                        if (customerModule == null) continue;

                        var credentials = new NetworkCredential(customer.FTP_USERNAME, customer.FTP_PASSWORD);
                        var listOfFiles = await oFtp_.GetFilesfromFTP(customer, customerModule, credentials, module);
                        if (listOfFiles == null || listOfFiles.Count == 0) continue;

                        int fileLoop = 1;
                        int totalFiles = listOfFiles.Count;

                        foreach (var oFiles in listOfFiles)
                        {
                            var fileStartTime = DateTime.Now;

                            try
                            {
                                // ====================================================
                                // START LOG
                                // ====================================================
                                Console.ForegroundColor = ConsoleColor.Cyan;

                                string startLog = $@"
                                *************************************************************
                                FILE PROCESS START   [{fileLoop}/{totalFiles}]
                                *************************************************************
                                Customer     : {customer.PROJECT_NAME}
                                Module       : {module}
                                File Name    : {oFiles.fileName}
                                Start Time   : {fileStartTime:dd-MMM-yyyy HH:mm:ss}
                                *************************************************************";

                                Console.WriteLine(startLog);
                                MyLogger.GetInstance().Info(startLog);
                                Console.ResetColor();

                                // ====================================================
                                if (!await oFtp_.DownloadFile(customer, customerModule, oFiles, credentials))
                                {
                                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
                                    continue;
                                }

                                oFiles.slNo = await CusCreationDao_.GenerateCusCreationSlNo();

                                var csvData = await oCommon_.ReadCsvFile(customer, customerModule, oFiles);
                                if (csvData == null || csvData.Count == 0)
                                {
                                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
                                    continue;
                                }

                                // CUSTOMER Import
                                oFiles.moduleType = "CUSTOMER";
                                var ediLog = await oCommonManager_.InsertEdiLog(customer, customerModule, oFiles);
                                if (ediLog == null)
                                {
                                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
                                    continue;
                                }

                                if (!await CusCreationDao_.InsertClientImport(csvData, ediLog))
                                {
                                    Console.WriteLine($"InsertClientImport failed for file {oFiles.fileName}");
                                    throw new Exception($"InsertClientImport failed for file {oFiles.fileName}");
                                }

                                var proInput = new Model.Inbound.pro_client_import_model
                                {
                                    FA_COMPANY_CODE = customer.COMPANY_CODE,
                                    FA_BRANCH_CODE = customer.BRANCH_CODE,
                                    FA_LOCATION_CODE = customer.LOCATION_CODE,
                                    FA_SL_NO = oFiles.slNo.ToString()
                                };

                                var insertSuccess = await CusCreationDao_.ExecClientImportProcedure(proInput);                               

                                if (insertSuccess)
                                {
                                    await oFtp_.MoveFileToBackupFolder(customer, customerModule, oFiles, credentials);

                                    var fileData = await oCommonManager_.GetEdiFileAsEmailRequestAsync(oFiles.fileName!, module, "COMPLETED");
                                    if (fileData != null)
                                        allEmailRequests.Add(fileData);
                                }

                                else
                                {
                                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
                                    var fileDataErr = await oCommonManager_.GetEdiFileAsEmailRequestAsync(oFiles.fileName!, module, "PENDING");
                                    if (fileDataErr != null)
                                        allEmailRequests.Add(fileDataErr);
                                }

                               
                                // ====================================================
                                var endTime = DateTime.Now;
                                var totalSeconds = (endTime - fileStartTime).TotalSeconds;

                                Console.ForegroundColor = ConsoleColor.Cyan;
                                string endLog = $@"
                                *************************************************************
                                FILE PROCESS COMPLETED
                                *************************************************************
                                File Name    : {oFiles.fileName}
                                End Time     : {endTime:dd-MMM-yyyy HH:mm:ss}
                                Duration     : {totalSeconds} Seconds
                                Status       : SUCCESS
                                *************************************************************";

                                Console.WriteLine(endLog);
                                MyLogger.GetInstance().Info(endLog);
                                Console.ResetColor();
                            }
                            catch (Exception ex)
                            {
                                // ====================================================
                                // EXCEPTION HANDLING
                                // ====================================================
                                Console.ForegroundColor = ConsoleColor.Red;

                                string errorLog = $@"
                                *************************************************************
                                FILE PROCESS FAILED
                                *************************************************************
                                File Name    : {oFiles.fileName}
                                Error        : {ex.Message}
                                Time         : {DateTime.Now:dd-MMM-yyyy HH:mm:ss}
                                *************************************************************";

                                Console.WriteLine(errorLog);
                                MyLogger.GetInstance().Error(errorLog);
                                Console.ResetColor();

                                var errorHtml = await oCommon_.GenerateExceptionHtml($"Customer Creation File: {oFiles.fileName}", ex);
                                allErrors.Add(errorHtml);

                                await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
                            }

                            fileLoop++;
                        }
                    }
                    catch (Exception ex)
                    {
                        var previousColors = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Error.WriteLine($"Error in Customer Creation: {ex}");
                        Console.ForegroundColor = previousColors;

                        var errorHtml = await oCommon_.GenerateExceptionHtml(
                            $"SO Customer: {customer.PROJECT_NAME}",
                            ex
                        );

                        allErrors.Add(errorHtml);
                        MyLogger.GetInstance().Error("Error in Customer Creation: " + ex);
                    }
                }
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Customer Creation Completed");
                Console.ForegroundColor = previousColor;

            }
            catch (Exception ex)
            {
                var errorHtml = await oCommon_.GenerateExceptionHtml("SO Fatal Error", ex);
                allErrors.Add(errorHtml);
            }

            return (allEmailRequests, allErrors);
        }
        #endregion

    }
}
