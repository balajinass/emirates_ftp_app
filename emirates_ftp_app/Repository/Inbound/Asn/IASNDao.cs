using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.ASNDao;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Repository.Inbound.Asn
{
    internal interface IASNDao
    {
        Task<int> GenerateASNSlNo();
        Task<bool> InsertASNImport(List<asn_csv_model> listofCsv_, wms_edi_ftp_model ediFtp);
        Task<bool> ExecASNImportProcedure(pro_client_import_model oProInput_);
    }
}
