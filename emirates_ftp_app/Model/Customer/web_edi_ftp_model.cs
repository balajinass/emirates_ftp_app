using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Customer
{
    internal class web_edi_ftp_model
    {
        public int SL_NO { get; set; }
        public string? COMPANY_CODE { get; set; }
        public string? BRANCH_CODE { get; set; }
        public string? LOCATION_CODE { get; set; }
        public string? FILE_UPLOAD_TIME { get; set; }
        public string? FILE_WRITE_TIME { get; set; }
        public string? FILE_NAME { get; set; }
        public string? FILE_TYPE { get; set; }
        public string? FILE_CONTENT { get; set; }
        public string? REFERENCE_ID { get; set; }
        public string? FILE_STATUS { get; set; }
        public string? FILE_DOWNLOAD_TIME { get; set; }
        public string? TRANSACTION_TIME { get; set; }
        public string? FILE_CREATED_TIME { get; set; }
        public string? IN_OUT { get; set; }
        public string? PO_LPO { get; set; }
        public string? NO_OF_INTEGRATED_ITEM { get; set; }
        public string? NO_OF_REJECTED_ITEM { get; set; }
        public string? NO_OF_REJECTED_PO_LPO { get; set; }
        public string? ERROR_MESSAGE { get; set; }
        public string? PRIMARY_COMPANY { get; set; }
    }
}
