using emirates_ftp_app.Model.Nass;
using System;
using System.Collections.Generic;
using System.Text;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Ftp;
using emirates_ftp_app.Model.Inbound.ASNDao;
using emirates_ftp_app.Model.Inbound.SupplierDao;
using emirates_ftp_app.Model.Inbound.SoCancelDao;
using emirates_ftp_app.Model.Inbound;

namespace emirates_ftp_app.Repository.CommonFunctions
{
    internal interface ICommonFunctions
    {
        Task<wms_edi_ftp_model> mappingEDILog(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles);
        Task<List<salesorder_csv_model>> ReadCsvFile(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles);
        Task<List<asn_csv_model>> ReadCsvFileforASN(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles);
        Task<List<supplier_csv_model>> ReadCsvFileforSUPP(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles);
        Task<List<so_cancel_csv_model>> ReadCsvFileforSoCancel(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles);
        void mappingASNEDILog(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles);
        date_format_model dateFormat(string sDate);
        //Task SendErrorEmail(string methodName, Exception ex);
        Task SendFinalMail(string subject, string body,string recipient);
        Task<string> GenerateExceptionHtml(string methodName, Exception ex);
        Task<string> GenerateSummaryTableRows(List<email_request_model> files);
        Task<string> GenerateSummaryEmailHtml(string tableRows);
        

    }
}
