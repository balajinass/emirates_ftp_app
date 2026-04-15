using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Inbound.SoCancelDao
{
    internal class so_cancel_csv_model
    {
        public string? SNO { get; set; }
        public string? PRIMARYCOMPANY { get; set; }
        public string? COSTBUCKET { get; set; }
        public string? SALESORDERNO { get; set; }
        public string? SALESORDERLINENO { get; set; }
        public string? EXTERNALSONO { get; set; }
        public string? ORDERLINENO { get; set; }
        public string? ITEMREF { get; set; }
        public string? CANCELLATIONQUANTITY { get; set; }
        public string? TRANSACTIONTIME { get; set; }
        public string? FILESTATUS { get; set; }
        public string? NOTE { get; set; }
        public string? SHIPTOCODE { get; set; }
    }
}
