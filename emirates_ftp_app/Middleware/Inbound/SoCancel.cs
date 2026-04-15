using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Common;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Ftp;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
using emirates_ftp_app.Model.Inbound.SoCancelDao;
using emirates_ftp_app.Model.Inbound.SupplierDao;
using emirates_ftp_app.Repository.CommonFunctions;
using emirates_ftp_app.Repository.Customer;
using emirates_ftp_app.Repository.FtpConnection;
using emirates_ftp_app.Repository.Inbound.SoCancel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace emirates_ftp_app.Middleware.Inbound
{
    internal class SoCancel
    {
        private readonly ILogger<SoCancel> _logger;
        private readonly AppSettings _appSettings;
        private readonly IGetCustomer oCommonManager_;
        private readonly FtpConnection oFtp_;
        private readonly ICommonFunctions oCommon_;
        private readonly ISoCancelDao oSoCancelDao_;
        public SoCancel(ILogger<SoCancel> logger,
                IOptions<AppSettings> appSettings,
               IGetCustomer getCustomer, FtpConnection oftp, ICommonFunctions oCommon, ISoCancelDao oSoCancelDao)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            oCommonManager_ = getCustomer;
            oFtp_ = oftp;
            oCommon_ = oCommon;
            oSoCancelDao_= oSoCancelDao;
        }

        #region SoCancelCreation_Old
        //public async Task SoCancelCreationOld(string module)
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

        //            var credentials = new NetworkCredential(
        //                oCustomer_.FTP_USERNAME,
        //                oCustomer_.FTP_PASSWORD);

        //            listofFiles_ = await oFtp_.GetFilesfromFTP(oCustomer_, oModule_, credentials, module);

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
        //                Console.WriteLine($"Download Success: {oFiles.fileName}");
        //                MyLogger.GetInstance().Info($"Download Success: {oFiles.fileName}");

        //                oFiles.slNo = await oSoCancelDao_.GenerateSoCancelSlNo();
        //                List<so_cancel_csv_model> listofCsv_ = new();

        //                var ediFtp_ = await oCommon_.mappingEDILog(oCustomer_, oModule_, oFiles);
        //                var ediList = new List<wms_edi_ftp_model> { ediFtp_ };
        //                if (ediFtp_ == null)
        //                {
        //                    sFileMovetoError = await oFtp_.MoveFiletoErrorFolder(oCustomer_, oModule_, oFiles, credentials);
        //                    continue;
        //                }

        //                #region SalesOrderCancel_Insert
        //                // -------- SalesOrderCancel Insert -------- //
        //                oFiles.moduleType = "SOC - CUS TO EFS";

        //                ediFtp_ = await oCommonManager_.InsertEdiLog(oCustomer_, oModule_, oFiles);
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
        //                Console.WriteLine("SalesOrderCancel - EDI Log Insert Success");
        //                MyLogger.GetInstance().Info("SalesOrderCancel - EDI Log Insert Success");

        //                listofCsv_ = await oCommon_.ReadCsvFileforSoCancel(oCustomer_, oModule_, oFiles);

        //                Console.WriteLine(oFiles.moduleType + " Log Inserted");
        //                MyLogger.GetInstance().Info(oFiles.moduleType + " Log Inserted");

        //                //InsertSoCancelImport//
        //                try
        //                {
        //                    if (!await oSoCancelDao_.InsertSoCancelImport(listofCsv_, ediFtp_))
        //                    {
        //                        sFileMovetoError = await oFtp_.MoveFiletoErrorFolder(oCustomer_, oModule_, oFiles, credentials);
        //                        throw new Exception($"InsertSoCancelImport failed for file {oFiles.fileName}");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MyLogger.GetInstance().Error($"Error processing file {oFiles.fileName}: {ex.Message}");
        //                    var exceptionHtml = await oCommon_.GenerateExceptionHtml("Error in SalesOrderCancel Creation", ex);
        //                    //await oCommon_.SendFinalMail($"Error in SalesOrderCancel Creation Insert - {oFiles.fileName}", exceptionHtml);
        //                    continue;
        //                }

        //                Console.WriteLine("SalesOrderCancel Insert Success");
        //                MyLogger.GetInstance().Info("SalesOrderCancel Insert Success");


        //                var oProInput_ = new Model.Inbound.pro_client_import_model
        //                {
        //                    FA_COMPANY_CODE = oCustomer_.COMPANY_CODE,
        //                    FA_BRANCH_CODE = oCustomer_.BRANCH_CODE,
        //                    FA_LOCATION_CODE = oCustomer_.LOCATION_CODE,
        //                    FA_SL_NO = Convert.ToString(oFiles.slNo)
        //                };
        //                bool bInsert_ = await oSoCancelDao_.ExecSoCancelImportProcedure(oProInput_);

        //                if (bInsert_)
        //                {
        //                    sFileMovetoBackup = await oFtp_.MoveFileToBackupFolder(oCustomer_, oModule_, oFiles, credentials);
        //                }
        //                else
        //                {
        //                    sFileMovetoError = await oFtp_.MoveFiletoErrorFolder(oCustomer_, oModule_, oFiles, credentials);
        //                }
        //                #endregion

        //                var fileData = await oCommonManager_.GetEdiFileAsEmailRequestAsync(oFiles.fileName!, module);
        //                if (fileData != null)
        //                {
        //                    emailRequests.Add(fileData);
        //                }
        //            }

        //            if (emailRequests.Count > 0)
        //            {
        //                //var summaryHtml = await oCommon_.GenerateSummaryEmailHtml(emailRequests, module);
        //                //await oCommon_.SendFinalMail($"Processing Files - {module}", summaryHtml);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        #region exception_email_log

        //        string exceptionHtml = await oCommon_.GenerateExceptionHtml("Sales Order Cancel Creation", ex);
        //        //await oCommon_.SendFinalMail("Sales Order Cancel Creation", exceptionHtml);

        //        #endregion
        //        Console.Error.WriteLine("Error in SOCancel Creation : " + ex);
        //        MyLogger.GetInstance().Error("Error in SOCancel creation : " + ex);
        //    }
        //}
        #endregion

        #region SoCancelCreation
        public async Task<(List<email_request_model> Success, List<string> Errors)> SoCancelCreation(string module)
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

                    foreach (var file in files)
                    {
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

                        file.slNo = await oSoCancelDao_.GenerateSoCancelSlNo();
                        file.moduleType = "SOC - CUS TO EFS";

                        var ediFtp = await oCommonManager_.InsertEdiLog(customer, moduleConfig, file);
                        if (ediFtp == null)
                        {
                            errorPath = await oFtp_.MoveFiletoErrorFolder(customer, moduleConfig, file, credentials);
                            errors.Add($"EDI Log insertion failed for file {file.fileName}");
                            continue;
                        }

                        MyLogger.GetInstance().Info("SalesOrderCancel - EDI Log Insert Success");

                        var csvData = await oCommon_.ReadCsvFileforSoCancel(customer, moduleConfig, file);

                        try
                        {
                            if (!await oSoCancelDao_.InsertSoCancelImport(csvData, ediFtp))
                            {
                                errorPath = await oFtp_.MoveFiletoErrorFolder(customer, moduleConfig, file, credentials);
                                throw new Exception($"InsertSoCancelImport failed for file {file.fileName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Error processing file {file.fileName}: {ex.Message}");
                            continue;
                        }

                        MyLogger.GetInstance().Info("SalesOrderCancel Insert Success");

                        var procedureInput = new Model.Inbound.pro_client_import_model
                        {
                            FA_COMPANY_CODE = customer.COMPANY_CODE,
                            FA_BRANCH_CODE = customer.BRANCH_CODE,
                            FA_LOCATION_CODE = customer.LOCATION_CODE,
                            FA_SL_NO = Convert.ToString(file.slNo)
                        };

                        bool procedureSuccess = await oSoCancelDao_.ExecSoCancelImportProcedure(procedureInput);

                        if (procedureSuccess)
                            backupPath = await oFtp_.MoveFileToBackupFolder(customer, moduleConfig, file, credentials);
                        else
                            errorPath = await oFtp_.MoveFiletoErrorFolder(customer, moduleConfig, file, credentials);

                        var fileData = await oCommonManager_.GetEdiFileAsEmailRequestAsync(file.fileName!, module);
                        if (fileData != null)
                            emailRequests.Add(fileData);
                    }
                }
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine($"Error in SoCancel Creation: {ex}");
                Console.ForegroundColor = previousColor;

                errors.Add($"Unhandled exception in SoCancel Creation: {ex.Message}");
                MyLogger.GetInstance().Error("Error in SoCancel creation: " + ex);
            }

            return (emailRequests, errors);
        }
        #endregion
    }
}
