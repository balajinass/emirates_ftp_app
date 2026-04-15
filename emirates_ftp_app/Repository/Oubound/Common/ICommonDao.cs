using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Nass;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Repository.Oubound.Common
{
    internal interface ICommonDao
    {
        Task<List<wms_edi_ftp_content>> GetFileContent(web_wms_edi_config_model customerConfig,string module);
        Task UpdateFileStatus(string fileName,string PrimaryCompany);
    }
}
