using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.SoCancelDao;
using emirates_ftp_app.Model.Inbound.SupplierDao;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Repository.Inbound.SoCancel
{
    internal interface ISoCancelDao
    {
        Task<int> GenerateSoCancelSlNo();
        Task<bool> InsertSoCancelImport(List<so_cancel_csv_model> listofCsv_, wms_edi_ftp_model ediFtp);
        Task<bool> ExecSoCancelImportProcedure(pro_client_import_model oProInput_);
    }
}
