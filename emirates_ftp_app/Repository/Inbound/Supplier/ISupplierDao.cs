using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.ASNDao;
using emirates_ftp_app.Model.Inbound.SupplierDao;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Repository.Inbound.Supplier
{
    internal interface ISupplierDao
    {
        Task<int> GenerateSupplierSlNo();
        Task<bool> InsertSupplierImport(List<supplier_csv_model> listofCsv_, wms_edi_ftp_model ediFtp);
        Task<bool> ExecSUPPImportProcedure(pro_client_import_model oProInput_);
    }
}
