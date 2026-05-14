using CsvHelper;
using CsvHelper.Configuration;
using emirates_ftp_app.Data;
using emirates_ftp_app.Log;
using emirates_ftp_app.Middleware.Outbound;
using emirates_ftp_app.Model.Common;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Ftp;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.ASNDao;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
using emirates_ftp_app.Model.Inbound.SoCancelDao;
using emirates_ftp_app.Model.Inbound.SupplierDao;
using emirates_ftp_app.Model.Nass;
using emirates_ftp_app.Repository.Customer;
using emirates_ftp_app.Repository.Oubound.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
//using static System.Net.WebRequestMethods;

namespace emirates_ftp_app.Repository.CommonFunctions
{
    internal class CommonFunctions : ICommonFunctions
    {
        private readonly IConfiguration _config;

        public CommonFunctions(IConfiguration config)
        {
            _config = config;
        }
        #region EDILog
        public async Task<wms_edi_ftp_model> mappingEDILog(web_wms_edi_config_model oCustomer_,web_wms_edi_module_config_model oModule_,input_request_model oFiles)
        {
            wms_edi_ftp_model ediFtp_ = new wms_edi_ftp_model();
            try
            {
                ediFtp_ = new wms_edi_ftp_model()
                {
                    SL_NO = oFiles.slNo,
                    COMPANY_CODE = oCustomer_.COMPANY_CODE,
                    BRANCH_CODE = oCustomer_.BRANCH_CODE,
                    LOCATION_CODE = oCustomer_.LOCATION_CODE,
                    FILE_NAME = oFiles.fileName,
                    FILE_TYPE = oFiles.moduleType,
                    FILE_CONTENT = File.ReadAllText(oModule_.LOCAL_FILE_PATH + oFiles.fileName),
                    REFERENCE_ID = oFiles.fileName,
                    FILE_STATUS = "NEW",
                    IN_OUT = "I",
                    PRIMARY_COMPANY = oCustomer_.PROJECT_NAME
                };
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(ex.Message);
            }

            return ediFtp_;
        }
        #endregion


        #region ReadcsvFile
        public async Task<List<salesorder_csv_model>> ReadCsvFile(web_wms_edi_config_model oCustomer_,web_wms_edi_module_config_model oModule_,input_request_model oFiles)
        {
            List<salesorder_csv_model> listofCsv_ = new List<salesorder_csv_model>();
            try
            {
                var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    HasHeaderRecord = false,
                    MissingFieldFound = null
                };
                string sLocalFullPath = string.Concat(oModule_.LOCAL_FILE_PATH + oFiles.fileName);
                StreamReader streamReader = File.OpenText(sLocalFullPath);
                CsvReader csvReader = new CsvReader(streamReader, csvConfig);

                listofCsv_ = csvReader.GetRecords<salesorder_csv_model>().ToList();
                MyLogger.GetInstance().Info("CSV READ " + listofCsv_);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(ex.Message);
            }
            return listofCsv_;
        }
        #endregion

        #region old_readcsvforASN
        //public async Task<List<asn_csv_model>> ReadCsvFileforASN(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles)
        //{
        //    List<asn_csv_model> listofCsv_ = new List<asn_csv_model>();
        //    try
        //    {
        //        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        //        {
        //            HasHeaderRecord = false,
        //            MissingFieldFound = null
        //        };
        //        string sLocalFullPath = string.Concat(oModule_.LOCAL_FILE_PATH + oFiles.fileName);
        //        StreamReader streamReader = File.OpenText(sLocalFullPath);
        //        CsvReader csvReader = new CsvReader(streamReader, csvConfig);

        //        listofCsv_ = csvReader.GetRecords<asn_csv_model>().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Error(ex.Message);
        //    }
        //    return listofCsv_;
        //}
        #endregion


        #region ReadCsvFileforASN
        public async Task<List<asn_csv_model>> ReadCsvFileforASN(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles)
        {
            var listofCsv_ = new List<asn_csv_model>();

            try
            {
                var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    HasHeaderRecord = false,
                    MissingFieldFound = null
                };

                string sLocalFullPath = Path.Combine(oModule_.LOCAL_FILE_PATH!, oFiles.fileName!);

                using var stream = new FileStream(
                    sLocalFullPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    useAsync: true);

                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, csvConfig);

                await foreach (var record in csv.GetRecordsAsync<asn_csv_model>())
                {
                    listofCsv_.Add(record);
                }               
                MyLogger.GetInstance().Info("CSV READ " + listofCsv_);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(ex.Message);
                Console.Error.WriteLine($"Error -" + ex.Message);
            }

            return listofCsv_;
        }
        #endregion

        #region ReadCsvFileforSUPPLIER
        public async Task<List<supplier_csv_model>> ReadCsvFileforSUPP(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles)
        {
            var listofCsv_ = new List<supplier_csv_model>();

            try
            {
                var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    HasHeaderRecord = false,
                    MissingFieldFound = null
                };

                string sLocalFullPath = Path.Combine(oModule_.LOCAL_FILE_PATH!, oFiles.fileName!);

                using var stream = new FileStream(
                    sLocalFullPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    useAsync: true);

                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, csvConfig);

                await foreach (var record in csv.GetRecordsAsync<supplier_csv_model>())
                {
                    listofCsv_.Add(record);
                }
                MyLogger.GetInstance().Info("CSV READ " + listofCsv_);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(ex.Message);
            }

            return listofCsv_;
        }
        #endregion

        #region ReadCsvFileforSoCancel
        public async Task<List<so_cancel_csv_model>> ReadCsvFileforSoCancel(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles)
        {
            var listofCsv_ = new List<so_cancel_csv_model>();

            try
            {
                var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    HasHeaderRecord = false,
                    MissingFieldFound = null
                };

                string sLocalFullPath = Path.Combine(oModule_.LOCAL_FILE_PATH!, oFiles.fileName!);

                using var stream = new FileStream(
                    sLocalFullPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    useAsync: true);

                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, csvConfig);

                await foreach (var record in csv.GetRecordsAsync<so_cancel_csv_model>())
                {
                    listofCsv_.Add(record);
                }
                MyLogger.GetInstance().Info("CSV READ " + listofCsv_);
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(ex.Message);
            }

            return listofCsv_;
        }
        #endregion


        #region ASN_EDILog
        public void mappingASNEDILog(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles)
        {

        }
        #endregion

        #region dateFormat
        public date_format_model dateFormat(string sDate)
        {
            if (string.IsNullOrWhiteSpace(sDate))
                return null!;

            string[] formats = { "dd-MM-yyyy","yyyy-MM-dd","dd/MM/yyyy","MM/dd/yyyy","yyyy/MM/dd" };
            if (DateTime.TryParseExact(sDate, formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDate))
            {
                return new date_format_model()
                {
                    actualDate = sDate,
                    year = parsedDate.Year,
                    month = parsedDate.Month,
                    date = parsedDate.Day,
                    modifiedDate = parsedDate
                };
            }

            return null!;
        }
        #endregion

        #region SummaryEmailOld
        //public async Task SendErrorEmail(string methodName, Exception ex)
        //{
        //    try
        //    {
        //        using var client = new HttpClient();

        //        var from = _config["EmailSettings:From"];//
        //        var to = _config["EmailSettings:To"];//
        //        var subject = _config["EmailSettings:Subject"];
        //        var token = _config["ApiSettings:Token"];//
        //        var url = _config["ApiSettings:BaseUrl"];//

        //        client.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Bearer", token);//

        //        var content = new MultipartFormDataContent();

        //        // Include the method name in the email body for better context
        //        var body = $"Error occurred in method: {methodName} \n\nException Details:\n{ex.ToString()}";
        //        content.Add(new StringContent(from!), "From");
        //        content.Add(new StringContent(to!), "To");
        //        content.Add(new StringContent(subject!), "Subject");
        //        content.Add(new StringContent(body), "Body");

        //        var response = await client.PostAsync(url, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            Console.WriteLine("Error notification sent successfully.");
        //        }
        //        else
        //        {
        //            Console.Error.WriteLine($"Failed to send error notification. Status Code: {response.StatusCode}");
        //            MyLogger.GetInstance().Error($"Failed to send error notification. Status Code: {response.StatusCode}");
        //        }
        //    }
        //    catch (Exception innerEx)
        //    {
        //        Console.Error.WriteLine($"Failed to send error notification: {innerEx.Message}");
        //        MyLogger.GetInstance().Error($"Failed to send error notification: {innerEx}");
        //    }
        
        //}
        #endregion
        //public async Task SendErrorEmailV2(string methodName, Exception ex)
        //{
        //    var subject = 
        //        body
        //        SendFinalMail --assign this method
        //}
        public async Task SendFinalMail(string subject, string body,string recipient)

        {
            try
            {
                using var client = new HttpClient();

                var from = _config["EmailSettings:From"];
                string to = string.Empty;
                if (recipient == "SummaryEmail")
                {
                    to = _config["EmailSettings:To"]!;
                }
                else
                {
                    to = _config["EmailSettings:ExceptionTo"]!;
                }

                var token = _config["ApiSettings:Token"];
                var url = _config["ApiSettings:BaseUrl"];

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var content = new MultipartFormDataContent();               
               
                content.Add(new StringContent(from!), "From");
                content.Add(new StringContent(to!), "To");
                content.Add(new StringContent(subject!), "Subject");
                content.Add(new StringContent(body), "Body");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error notification sent successfully.");
                }
                else
                {
                    Console.Error.WriteLine($"Failed to send error notification. Status Code: {response.StatusCode}");
                    MyLogger.GetInstance().Error($"Failed to send error notification. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception innerEx)
            {
                Console.Error.WriteLine($"Failed to send error notification: {innerEx.Message}");
                MyLogger.GetInstance().Error($"Failed to send error notification: {innerEx}");
            }

        }
        public async Task<string> GenerateExceptionHtml(string methodName, Exception ex)
        {
            if (ex == null)
                return "<p>No exception information available.</p>";

            var exceptionDetails = new StringBuilder();
            exceptionDetails.AppendLine($"<strong>Message:</strong> {ex.Message}<br><br>");
            exceptionDetails.AppendLine($"<strong>Stack Trace:</strong><br>{ex.StackTrace}<br>");

            var inner = ex.InnerException;
            while (inner != null)
            {
                exceptionDetails.AppendLine("<hr>");
                exceptionDetails.AppendLine("<strong>Inner Exception:</strong><br>");
                exceptionDetails.AppendLine($"<strong>Message:</strong> {inner.Message}<br><br>");
                exceptionDetails.AppendLine($"<strong>Stack Trace:</strong><br>{inner.StackTrace}<br>");
                inner = inner.InnerException;
            }

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            return $@"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>{methodName} - Error Notification</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin:0; padding:0; }}
                        .container {{ max-width: 600px; margin: 20px auto; background-color: #fff; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1); padding: 20px; }}
                        h1 {{ color: #d9534f; font-size: 22px; margin-bottom: 10px; }}
                        p {{ font-size: 16px; }}
                        .details {{ background-color: #f8f8f8; border-left: 4px solid #d9534f; padding: 10px; margin: 15px 0; font-family: monospace; white-space: pre-wrap; }}
                        .footer {{ font-size: 12px; color: #777; text-align: center; margin-top: 20px; }}
                    </style>
                    </head>
                    <body>
                    <div class='container'>
                        <h1>Emirates_EDI_FTP_APP_Exception</h1>
                        <h2>Error at {methodName}</h2>
                        <p><strong>Timestamp:</strong> {timestamp}</p>
                        <div class='details'>
                            {exceptionDetails}
                        </div>
                        <div class='footer'>Automated message from Emirates EDI FTP. Do not reply.</div>
                    </div>
                    </body>
                    </html>";
        }


        // Returns only <tr> rows
        public async Task<string> GenerateSummaryTableRows(List<email_request_model> files)
        {
            if (files == null || files.Count == 0)
                return "";

            StringBuilder tableRows = new StringBuilder();

            foreach (var file in files)
            {
                tableRows.Append($@"
        <tr>
            <td style='border:1px solid #ddd;padding:8px;text-align:center;white-space:nowrap;width:130px;'>
                {file.Module}
            </td>

            <td style='border:1px solid #ddd;padding:8px;width:300px;word-break:break-all;'>
                {file.File_name}
            </td>

            <td style='border:1px solid #ddd;padding:8px;text-align:center;white-space:nowrap;width:170px;'>
                {file.Processed_On:yyyy-MM-dd HH:mm:ss}
            </td>

            <td style='border:1px solid #ddd;padding:8px;text-align:center;width:110px;'>
                {file.Status}
            </td>

            <td style='border:1px solid #ddd;padding:8px;text-align:center;width:120px;'>
                {file.Archive_Status}
            </td>

            <td style='border:1px solid #ddd;padding:8px;text-align:center;white-space:nowrap;width:150px;'>
                {file.Company_Name}
            </td>
        </tr>");
            }

            return tableRows.ToString();
        }

        // Wraps rows into full HTML page
        public async Task<string> GenerateSummaryEmailHtml(string tableRows)
        {
            return $@"
                        <!DOCTYPE html>
                        <html>
                        <body style='margin:0;padding:0;background-color:#f4f4f4;font-family:Arial,sans-serif;'>

                        <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f4f4;padding:20px;'>
                        <tr>
                        <td align='center'>

                        <table width='850' cellpadding='0' cellspacing='0'
                        style='background-color:#ffffff;border-radius:6px;padding:20px;'>

                        <tr>
                        <td style='color:#0275d8;font-size:22px;font-weight:bold;padding-bottom:10px;'>
                        File Processing Summary
                        </td>
                        </tr>

                        <tr>
                        <td style='font-size:15px;padding-bottom:15px;'>
                        The following files were processed:
                        </td>
                        </tr>

                        <tr>
                        <td>

                        <table width='100%' cellpadding='0' cellspacing='0'
                        style='border-collapse:collapse;font-size:14px;table-layout:fixed;'>

                        <tr style='background-color:#f2f2f2;font-weight:bold;text-align:center;'>

                        <th style='border:1px solid #ddd;padding:8px;width:130px;white-space:nowrap;'>
                        Module Name
                        </th>

                        <th style='border:1px solid #ddd;padding:8px;width:300px;'>
                        File Name
                        </th>

                        <th style='border:1px solid #ddd;padding:8px;width:170px;white-space:nowrap;'>
                        Date / Time
                        </th>

                        <th style='border:1px solid #ddd;padding:8px;width:110px;'>
                        Status
                        </th>

                        <th style='border:1px solid #ddd;padding:8px;width:120px;'>
                        Archive Status
                        </th>

                        <th style='border:1px solid #ddd;padding:8px;width:150px;white-space:nowrap;'>
                        Company Name
                        </th>

                        </tr>

                        {tableRows}

                        </table>

                        </td>
                        </tr>

                        <tr>
                        <td style='font-size:12px;color:#777;text-align:center;padding-top:20px;'>
                        Automated message from New Age Software & Solutions Pvt Ltd. Do not reply.
                        </td>
                        </tr>

                        </table>

                        </td>
                        </tr>
                        </table>

                        </body>
                        </html>";
        }

    }
}
