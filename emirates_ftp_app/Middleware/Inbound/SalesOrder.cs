using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Common;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Ftp;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
using emirates_ftp_app.Model.Nass;
using emirates_ftp_app.Repository.CommonFunctions;
using emirates_ftp_app.Repository.Customer;
using emirates_ftp_app.Repository.FtpConnection;
using emirates_ftp_app.Repository.Inbound.SalesOrders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;

namespace emirates_ftp_app.Middleware.Inbound
{
    internal class SalesOrder
    {
        private readonly ILogger<SalesOrder> _logger;
        private readonly AppSettings _appSettings;
        private readonly IGetCustomer oCommonManager_;
        private readonly FtpConnection oFtp_;
        private readonly ICommonFunctions oCommon_;
        private readonly ISalesOrderDao oSOManager_;
        private readonly IConfiguration _config;

        public SalesOrder(ILogger<SalesOrder> logger,            
            IOptions<AppSettings> appSettings,
           IGetCustomer getCustomer,FtpConnection oftp, ICommonFunctions oCommon,ISalesOrderDao oSOManager, IConfiguration config)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            oCommonManager_ = getCustomer;
            oFtp_ = oftp;
            oCommon_ = oCommon;
            oSOManager_ = oSOManager;
            _config = config;
        }

        #region SoCreation_first
        //public async Task SoCreation(string module)
        //{
        //    _logger.LogInformation($"{module} creation started");
        //    try
        //    {
        //        List<web_wms_edi_config_model> listofConfig_ = await _nassRepository.GetListofConfig();
        //        if(listofConfig_ != null)
        //        {
        //            foreach(web_wms_edi_config_model config_ in listofConfig_)
        //            {

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error in SoCreation()");
        //    }
        //}
        #endregion

        #region SoCreation_old
        //public async Task SoCreationold(string module)
        //{
        //    try
        //    {
        //        var listOfCustomers = await oCommonManager_.GetListofCustomer();
        //        if (listOfCustomers == null || listOfCustomers.Count == 0)
        //        {
        //            MyLogger.GetInstance().Info("Number of Customers: 0");
        //            return;
        //        }

        //        MyLogger.GetInstance().Info("Number of Customers: " + listOfCustomers.Count);

        //        foreach (var customer in listOfCustomers)
        //        {
        //            MyLogger.GetInstance().Info("PROJECT-NAME: " + customer.PROJECT_NAME);

        //            var customerModule = customer.MODULES?.FirstOrDefault(m => m.MODULE_NAME == module);
        //            if (customerModule == null) continue;

        //            var credentials = new NetworkCredential(customer.FTP_USERNAME, customer.FTP_PASSWORD);
        //            var listOfFiles = await oFtp_.GetFilesfromFTP(customer, customerModule, credentials, module);
        //            if (listOfFiles == null || listOfFiles.Count == 0) continue;

        //            var emailRequests = new List<email_request_model>(); 

        //            foreach (var oFiles in listOfFiles)
        //            {
        //                // Download
        //                if (!await oFtp_.DownloadFile(customer, customerModule, oFiles, credentials))
        //                {
        //                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
        //                    continue;
        //                }
        //                MyLogger.GetInstance().Info($"Download Success: {oFiles.fileName}");

        //                oFiles.slNo = await oSOManager_.GenerateSOSlNo();
        //                var csvData = await oCommon_.ReadCsvFile(customer, customerModule, oFiles);
        //                if (csvData == null || csvData.Count == 0)
        //                {
        //                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
        //                    continue;
        //                }

        //                // CUSTOMER Import
        //                oFiles.moduleType = "CUSTOMER";
        //                var ediLog = await oCommonManager_.InsertEdiLog(customer, customerModule, oFiles);
        //                if (ediLog == null)
        //                {
        //                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
        //                    continue;
        //                }
        //                MyLogger.GetInstance().Info("SO CUS- EDI Log Insert Success");

        //                //InsertClientImport//
        //                try
        //                {
        //                    if (!await oSOManager_.InsertClientImport(csvData, ediLog))
        //                    {
        //                        await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
        //                        throw new Exception($"InsertClientImport failed for file {oFiles.fileName}");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MyLogger.GetInstance().Error($"Error processing file {oFiles.fileName}: {ex.Message}");
        //                    var exceptionHtml = await oCommon_.GenerateExceptionHtml("Sales Order Creation - Customer", ex);
        //                    //await oCommon_.SendFinalMail($"Error in Sales Order Creation Customer Insert - {oFiles.fileName}", exceptionHtml);

        //                    continue;
        //                }

        //                MyLogger.GetInstance().Info("SO CUS Insert Success");

        //                var proInput = new Model.Inbound.pro_client_import_model
        //                {
        //                    FA_COMPANY_CODE = customer.COMPANY_CODE,
        //                    FA_BRANCH_CODE = customer.BRANCH_CODE,
        //                    FA_LOCATION_CODE = customer.LOCATION_CODE,
        //                    FA_SL_NO = oFiles.slNo.ToString()
        //                };
        //                await oSOManager_.ExecClientImportProcedure(proInput);

        //                // SO - CUS TO EFS
        //                oFiles.moduleType = "SO - CUS TO EFS";
        //                ediLog = await oCommonManager_.InsertEdiLog(customer, customerModule, oFiles);
        //                if (ediLog == null)
        //                {
        //                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
        //                    continue;
        //                }
        //                MyLogger.GetInstance().Info("SO CUS TO EFS- EDI Log Insert Success");

        //                //InsertSOImport//
        //                try
        //                {
        //                    if (!await oSOManager_.InsertSOImport(csvData, ediLog))
        //                    {
        //                        await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
        //                        throw new Exception($"InsertSOImport failed for file {oFiles.fileName}");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {  
        //                    MyLogger.GetInstance().Error($"Error processing file {oFiles.fileName}: {ex.Message}");
        //                    var exceptionHtml = await oCommon_.GenerateExceptionHtml("Sales Order Creation", ex);
        //                    //await oCommon_.SendFinalMail($"Error in Sales Order Creation Insert - {oFiles.fileName}", exceptionHtml);

        //                    continue;
        //                }

        //                MyLogger.GetInstance().Info("SO CUS TO EFS Insert Success");

        //                var insertSuccess = await oSOManager_.ExecSOImportProcedure(proInput);
        //                if (insertSuccess)
        //                {
        //                    await oFtp_.MoveFileToBackupFolder(customer, customerModule, oFiles, credentials);
        //                }
        //                else
        //                {
        //                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
        //                }

        //                // Prepare email request
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
        //        MyLogger.GetInstance().Error("Error in SO creation: " + ex);
        //        var exceptionHtml = await oCommon_.GenerateExceptionHtml("Sales Order Creation", ex);
        //        //await oCommon_.SendFinalMail("Error in Sales Order Creation", exceptionHtml);
        //    }
        //}
        #endregion

        #region SoCreation
        public async Task<(List<email_request_model> Success, List<string> Errors)> SoCreation(string module)
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

                        foreach (var oFiles in listOfFiles)
                        {
                            try
                            {                               
                                if (!await oFtp_.DownloadFile(customer, customerModule, oFiles, credentials))
                                {
                                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
                                    continue;
                                }

                                oFiles.slNo = await oSOManager_.GenerateSOSlNo();

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

                                if (!await oSOManager_.InsertClientImport(csvData, ediLog))
                                {
                                    Console.WriteLine("InsertClientImport failed for file {oFiles.fileName");
                                    throw new Exception($"InsertClientImport failed for file {oFiles.fileName}");
                                }

                                var proInput = new Model.Inbound.pro_client_import_model
                                {
                                    FA_COMPANY_CODE = customer.COMPANY_CODE,
                                    FA_BRANCH_CODE = customer.BRANCH_CODE,
                                    FA_LOCATION_CODE = customer.LOCATION_CODE,
                                    FA_SL_NO = oFiles.slNo.ToString()
                                };

                                await oSOManager_.ExecClientImportProcedure(proInput);

                                // SO Import
                                oFiles.moduleType = "SO - CUS TO EFS";
                                ediLog = await oCommonManager_.InsertEdiLog(customer, customerModule, oFiles);
                                if (ediLog == null)
                                {
                                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
                                    continue;
                                }

                                if (!await oSOManager_.InsertSOImport(csvData, ediLog))
                                {
                                    Console.WriteLine("InsertSOImport failed for file {oFiles.fileName");
                                    throw new Exception($"InsertSOImport failed for file {oFiles.fileName}");
                                }

                                var insertSuccess = await oSOManager_.ExecSOImportProcedure(proInput);

                                if (insertSuccess)
                                {
                                    await oFtp_.MoveFileToBackupFolder(customer, customerModule, oFiles, credentials);
                                }
                                else
                                {
                                    await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
                                }
                               
                                var fileData = await oCommonManager_.GetEdiFileAsEmailRequestAsync(oFiles.fileName!, module);
                                if (fileData != null)
                                {
                                    allEmailRequests.Add(fileData);
                                }
                            }
                            catch (Exception ex)
                            {
                                MyLogger.GetInstance().Error($"Error processing file {oFiles.fileName}: {ex.Message}");
                                Console.Error.WriteLine($"Error processing file {oFiles.fileName}: {ex.Message}");

                                var errorHtml = await oCommon_.GenerateExceptionHtml(
                                    $"SO File: {oFiles.fileName}",
                                    ex
                                );

                                allErrors.Add(errorHtml);

                                await oFtp_.MoveFiletoErrorFolder(customer, customerModule, oFiles, credentials);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var previousColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Error.WriteLine($"Error in SOCreation: {ex}");
                        Console.ForegroundColor = previousColor;

                        var errorHtml = await oCommon_.GenerateExceptionHtml(
                            $"SO Customer: {customer.PROJECT_NAME}",
                            ex
                        );

                        allErrors.Add(errorHtml);
                        MyLogger.GetInstance().Error("Error in SOCreation: " + ex);
                    }
                }
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("SO Creation Completed");
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
