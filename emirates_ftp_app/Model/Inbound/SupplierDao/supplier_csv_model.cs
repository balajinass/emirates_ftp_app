using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Inbound.SupplierDao
{
    internal class supplier_csv_model
    {
        public string? SUPPLIERNAME { get; set; }
        public string? SUPPLIERCODE { get; set; }
        public string? SHIPTOCODE { get; set; }
        public string? DEFAULTTRADETERMCODE { get; set; }
        public string? DEFAULTFREIGHTFORWARDERCODE { get; set; }
        public string? SUPPLIERCURRENCYCODE { get; set; }
        public string? ADDRESSLINE1 { get; set; }
        public string? ADDRESSLINE2 { get; set; }
        public string? POSTBOXNUMBER { get; set; }
        public string? POSTALCODE { get; set; }
        public string? CITYCODE { get; set; }
        public string? OTHERCITYNAME { get; set; }
        public string? STATECODE { get; set; }
        public string? OTHERSTATENAME { get; set; }
        public string? COUNTRYCODE { get; set; }
        public string? CONTACTNAME { get; set; }
        public string? INTERNATIONALDIALINGCODE { get; set; }
        public string? AREADIALINGCODE { get; set; }
        public string? PHONENUMBER { get; set; }
        public string? PHONEEXTENSTIONNUMBER { get; set; }
        public string? PHONENUMBER2 { get; set; }
        public string? EMAIL { get; set; }
        public string? PRIMARYCOMPANY { get; set; }
    }
}
