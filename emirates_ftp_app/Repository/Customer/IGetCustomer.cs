using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Ftp;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Nass;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Repository.Customer
{
    internal interface IGetCustomer
    {
        Task<List<web_wms_edi_config_model>> GetListofCustomer();
        Task<List<web_wms_edi_config_model>> GetListofCustomerOUTBOUND();
        Task<wms_edi_ftp_model> InsertEdiLog(web_wms_edi_config_model oCustomer_,web_wms_edi_module_config_model oModule_,input_request_model oFiles);
        Task<email_request_model?> GetEdiFileAsEmailRequestAsync(string fileName,string module,string FileStatus);
    }
}
