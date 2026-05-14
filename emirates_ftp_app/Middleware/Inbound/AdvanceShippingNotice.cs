using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Common;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Ftp;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.ASNDao;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
using emirates_ftp_app.Model.Nass;
using emirates_ftp_app.Repository.CommonFunctions;
using emirates_ftp_app.Repository.Customer;
using emirates_ftp_app.Repository.FtpConnection;
using emirates_ftp_app.Repository.Inbound.Asn;
using emirates_ftp_app.Repository.Inbound.SalesOrders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static System.Net.WebRequestMethods;

namespace emirates_ftp_app.Middleware.Inbound
{
        internal class AdvanceShippingNotice
        {
            private readonly ILogger<AdvanceShippingNotice> _logger;
            private readonly AppSettings _appSettings;
            private readonly IGetCustomer oCommonManager_;
            private readonly FtpConnection oFtp_;
            private readonly ICommonFunctions oCommon_;
            private readonly IASNDao oASNDao_;
        public AdvanceShippingNotice(ILogger<AdvanceShippingNotice> logger,
                IOptions<AppSettings> appSettings,
               IGetCustomer getCustomer, FtpConnection oftp, ICommonFunctions oCommon, IASNDao oASNDao)
            {
                _logger = logger;
                _appSettings = appSettings.Value;
                oCommonManager_ = getCustomer;
                oFtp_ = oftp;
                oCommon_ = oCommon;
                oASNDao_= oASNDao;
            }

        #region AsnCreation_OLD
        //public async Task AsnCreationOLD(string module)
        //{
        //    List<input_request_model> listofFiles_ = new();

        //    try
        //    {
        //        var listofcustomer_ = await oCommonManager_.GetListofCustomer();

        //        if (listofcustomer_ == null || listofcustomer_.Count == 0)
        //        {
        //            MyLogger.GetInstance().Info("Number of Customers : 0");
        //            return;
        //        }

        //        MyLogger.GetInstance().Info("Number of Customers : " + listofcustomer_.Count);

        //        foreach (var oCustomer_ in listofcustomer_)
        //        {
        //            MyLogger.GetInstance().Info("PROJECT-NAME : " + oCustomer_.PROJECT_NAME);

        //            var oModule_ = oCustomer_.MODULES?
        //                .FirstOrDefault(w => w.MODULE_NAME == module);

        //            if (oModule_ == null) continue;

        //            var credentials = new NetworkCredential(oCustomer_.FTP_USERNAME,oCustomer_.FTP_PASSWORD);

        //            listofFiles_ = await oFtp_.GetFilesfromFTP(oCustomer_, oModule_, credentials,module);

        //            if (listofFiles_ == null) continue;

        //            var emailRequests = new List<email_request_model>();

        //            foreach (var oFiles in listofFiles_)
        //            {
        //                string sFileMovetoBackup = string.Empty;
        //                string sFileMovetoError = string.Empty;

        //                bool bDownload = await oFtp_.DownloadFile(oCustomer_, oModule_, oFiles, credentials);
        //                if (!bDownload)
        //                {
        //                    sFileMovetoError = await oFtp_.MoveFiletoErrorFolder(oCustomer_, oModule_, oFiles, credentials);
        //                    continue;
        //                }
        //                MyLogger.GetInstance().Info($"Download Success: {oFiles.fileName}");

        //                oFiles.slNo = await oASNDao_.GenerateASNSlNo();
        //                List<asn_csv_model> listofCsv_ = new();
        //                oFiles.moduleType = "ASN - CUS TO EFS";
        //                var ediFtp_ = await oCommonManager_.InsertEdiLog(oCustomer_, oModule_, oFiles);

        //                if (ediFtp_ == null)
        //                {
        //                    sFileMovetoError = await oFtp_.MoveFiletoErrorFolder(oCustomer_, oModule_, oFiles, credentials);
        //                    continue;
        //                }

        //                #region Old_EDI_Insert
        //                //if (!await oCommonManager_.InsertEdiLog(ediFtp_, ediList))
        //                //{
        //                //    sFileMovetoError = await oFtp_.MoveFiletoErrorFolder(oCustomer_, oModule_, oFiles, credentials);
        //                //    continue;
        //                //}
        //                #endregion
        //                MyLogger.GetInstance().Info("ASN - EDI Log Insert Success");
        //                listofCsv_ = await oCommon_.ReadCsvFileforASN(oCustomer_, oModule_, oFiles);

        //                MyLogger.GetInstance().Info(oFiles.moduleType + " Log Inserted");

        //                //InsertASNImport//
        //                try
        //                {
        //                    if (!await oASNDao_.InsertASNImport(listofCsv_, ediFtp_))
        //                    {
        //                        sFileMovetoError = await oFtp_.MoveFiletoErrorFolder(oCustomer_, oModule_, oFiles, credentials);
        //                        throw new Exception($"InsertASNImport failed for file {oFiles.fileName}");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MyLogger.GetInstance().Error($"Error processing file {oFiles.fileName}: {ex.Message}");
        //                    var exceptionHtml = await oCommon_.GenerateExceptionHtml("Error in ASN Creation", ex);
        //                    //await oCommon_.SendFinalMail($"Error in ASN Creation Insert - {oFiles.fileName}", exceptionHtml);

        //                    continue;
        //                }

        //                MyLogger.GetInstance().Info("ASN Insert Success");


        //                var oProInput_ = new Model.Inbound.pro_client_import_model
        //                {
        //                    FA_COMPANY_CODE = oCustomer_.COMPANY_CODE,
        //                    FA_BRANCH_CODE = oCustomer_.BRANCH_CODE,
        //                    FA_LOCATION_CODE = oCustomer_.LOCATION_CODE,
        //                    FA_SL_NO = Convert.ToString(oFiles.slNo)
        //                };
        //                bool bInsert_ = await oASNDao_.ExecASNImportProcedure(oProInput_);

        //                if (bInsert_)
        //                {
        //                    sFileMovetoBackup = await oFtp_.MoveFileToBackupFolder(oCustomer_, oModule_, oFiles, credentials);
        //                }
        //                else
        //                {
        //                    sFileMovetoError = await oFtp_.MoveFiletoErrorFolder(oCustomer_, oModule_, oFiles, credentials);
        //                }

        //                var fileData = await oCommonManager_.GetEdiFileAsEmailRequestAsync(oFiles.fileName!, module);
        //                if (fileData != null)
        //                {
        //                    emailRequests.Add(fileData);
        //                }
        //            }

        //            if (emailRequests.Count > 0)
        //            {
        //                //var summaryHtml = await oCommon_.GenerateSummaryEmailHtml(emailRequests, module);
        //               // await oCommon_.SendFinalMail($"Processing Files - {module}", summaryHtml);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        #region exception_email_log

        //        string exceptionHtml = await oCommon_.GenerateExceptionHtml("ASN Creation", ex);
        //        //await oCommon_.SendFinalMail("ASN Creation", exceptionHtml);

        //        #endregion
        //        Console.Error.WriteLine("Error in ASN Creation : " + ex);
        //        MyLogger.GetInstance().Error("Error in ASN creation : " + ex);
        //    }
        //}
        #endregion

        #region AsnCreation
        public async Task<(List<email_request_model> Success, List<string> Errors)> AsnCreation(string module)
        {
            var emailRequests = new List<email_request_model>();
            var errors = new List<string>();

            try
            {
                var listOfCustomers = await oCommonManager_.GetListofCustomer();

                if (listOfCustomers == null || listOfCustomers.Count == 0)
                {
                    MyLogger.GetInstance().Info("Number of Customers : 0");
                    return (emailRequests, errors);
                }

                MyLogger.GetInstance().Info("Number of Customers : " + listOfCustomers.Count);                

                foreach (var customer in listOfCustomers)
                {
                    MyLogger.GetInstance().Info("PROJECT-NAME : " + customer.PROJECT_NAME);                   

                    var moduleConfig = customer.MODULES?.FirstOrDefault(m => m.MODULE_NAME == module);
                    if (moduleConfig == null) continue;

                    var credentials = new NetworkCredential(customer.FTP_USERNAME, customer.FTP_PASSWORD);
                    var files = await oFtp_.GetFilesfromFTP(customer, moduleConfig, credentials, module);

                    if (files == null || files.Count == 0) continue;

                    int fileLoop = 1;
                    int totalFiles = files.Count;

                    foreach (var file in files)
                    {
                        var fileStartTime = DateTime.Now;

                        try
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;

                            string startLog = $@"
                                    *************************************************************
                                    FILE PROCESS START   [{fileLoop}/{totalFiles}]
                                    *************************************************************
                                    Customer     : {customer.PROJECT_NAME}
                                    Module       : {module}
                                    File Name    : {file.fileName}
                                    Start Time   : {fileStartTime:dd-MMM-yyyy HH:mm:ss}
                                    *************************************************************";
                           
                            MyLogger.GetInstance().Info(startLog);
                            Console.ResetColor();

                            // ====================================================
                            // EXECUTE INTERNAL LOGIC (DOWNLOAD / CSV / INSERT / PROCEDURE)
                            // ====================================================                            
                            string backupPath = string.Empty;
                            string errorPath = string.Empty;

                            bool downloaded = await oFtp_.DownloadFile(customer, moduleConfig, file, credentials);
                            if (!downloaded)
                            {
                                errorPath = await oFtp_.MoveFiletoErrorFolder(customer, moduleConfig, file, credentials);
                                errors.Add($"Download failed for file {file.fileName}");
                                continue;
                            }

                            MyLogger.GetInstance().Info($"Download Success: {file.fileName}");                           

                            file.slNo = await oASNDao_.GenerateASNSlNo();
                            file.moduleType = "ASN - CUS TO EFS";

                            var ediFtp = await oCommonManager_.InsertEdiLog(customer, moduleConfig, file);
                            if (ediFtp == null)
                            {
                                errorPath = await oFtp_.MoveFiletoErrorFolder(customer, moduleConfig, file, credentials);
                                errors.Add($"EDI Log insertion failed for file {file.fileName}");
                                continue;
                            }

                            MyLogger.GetInstance().Info("ASN - EDI Log Insert Success");

                            var csvData = await oCommon_.ReadCsvFileforASN(customer, moduleConfig, file);

                            try
                            {
                                if (!await oASNDao_.InsertASNImport(csvData, ediFtp))
                                {
                                    errorPath = await oFtp_.MoveFiletoErrorFolder(customer, moduleConfig, file, credentials);
                                    throw new Exception($"InsertASNImport failed for file {file.fileName}");
                                }
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Error processing file {file.fileName}: {ex.Message}");
                                continue;
                            }

                            MyLogger.GetInstance().Info("ASN Insert Success");                          

                            var procedureInput = new Model.Inbound.pro_client_import_model
                            {
                                FA_COMPANY_CODE = customer.COMPANY_CODE,
                                FA_BRANCH_CODE = customer.BRANCH_CODE,
                                FA_LOCATION_CODE = customer.LOCATION_CODE,
                                FA_SL_NO = Convert.ToString(file.slNo)
                            };

                            bool procedureSuccess = await oASNDao_.ExecASNImportProcedure(procedureInput);

                            if (procedureSuccess)
                            {
                                backupPath = await oFtp_.MoveFileToBackupFolder(customer, moduleConfig, file, credentials);
                                var fileData = await oCommonManager_.GetEdiFileAsEmailRequestAsync(file.fileName!, module, "COMPLETED");
                                if (fileData != null)
                                    emailRequests.Add(fileData);
                            }

                            else
                            {
                                errorPath = await oFtp_.MoveFiletoErrorFolder(customer, moduleConfig, file, credentials);
                                var fileDataerr = await oCommonManager_.GetEdiFileAsEmailRequestAsync(file.fileName!, module, "PENDING");
                                if (fileDataerr != null)
                                    emailRequests.Add(fileDataerr);

                            }

                            // ====================================================
                            // END LOG
                            // ====================================================
                            var endTime = DateTime.Now;
                            var totalSeconds = (endTime - fileStartTime).TotalSeconds;

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            string endLog = $@"
                                    *************************************************************
                                    FILE PROCESS COMPLETED
                                    *************************************************************
                                    File Name    : {file.fileName}
                                    End Time     : {endTime:dd-MMM-yyyy HH:mm:ss}
                                    Duration     : {totalSeconds} Seconds
                                    Status       : SUCCESS
                                    *************************************************************";
                           
                            MyLogger.GetInstance().Info(endLog);
                            Console.ResetColor();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            string errorLog = $@"
                                *************************************************************
                                FILE PROCESS FAILED
                                *************************************************************
                                File Name    : {file.fileName}
                                Error        : {ex.Message}
                                Time         : {DateTime.Now:dd-MMM-yyyy HH:mm:ss}
                                *************************************************************";
                           
                            MyLogger.GetInstance().Error(errorLog);
                            Console.ResetColor();

                            var errorHtml = await oCommon_.GenerateExceptionHtml($"ASN File: {file.fileName}", ex);
                            errors.Add(errorHtml);

                            await oFtp_.MoveFiletoErrorFolder(customer, moduleConfig, file, credentials);
                        }

                        fileLoop++;
                    }
                }
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;                
                MyLogger.GetInstance().Info("ASN Creation Completed");
                Console.ForegroundColor = previousColor;
            }
            catch (Exception ex)
            {
                errors.Add($"Unhandled exception in ASN Creation: {ex.Message}");
                MyLogger.GetInstance().Error("Error in ASN creation: " + ex);
            }

            return (emailRequests, errors);
        }
        #endregion
    }

}
