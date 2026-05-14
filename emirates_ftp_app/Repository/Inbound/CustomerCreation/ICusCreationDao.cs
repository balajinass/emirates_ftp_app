using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Repository.Inbound.CustomerCreation
{
    internal interface ICusCreationDao
    {
        Task<int> GenerateCusCreationSlNo();
        Task<bool> InsertClientImport(List<salesorder_csv_model> listofCsv_, wms_edi_ftp_model ediFtp);
        Task<bool> ExecClientImportProcedure(pro_client_import_model oProInput_); 
    }
}
