using emirates_ftp_app.Model.Nass;
using System;
using System.Collections.Generic;
using System.Text;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound;
namespace emirates_ftp_app.Repository.Inbound.SalesOrders
{
    internal interface ISalesOrderDao
    {
        Task<int> GenerateSOSlNo();
        Task<bool> InsertClientImport(List<salesorder_csv_model> listofCsv_, wms_edi_ftp_model ediFtp);
        Task<bool> ExecClientImportProcedure(pro_client_import_model oProInput_);
        Task<bool> InsertSOImport(List<salesorder_csv_model> listOfCsv, wms_edi_ftp_model ediFtp);
        Task<bool> ExecSOImportProcedure(pro_client_import_model oProInput_);

    }
}
