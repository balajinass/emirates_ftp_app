using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Inbound.SalesOrderDao
{
    internal class salesorder_csv_model
    {
        public string? CUSTOMERCODE { get; set; }
        public string? CUSTOMERNAME { get; set; }
        public string? PRIMARYCOMPANY { get; set; }
        public string? CUSTOMERTYPECODE { get; set; }
        public string? CUSTCURRENCYCODE { get; set; }
        public string? COUNTRYCODE { get; set; }
        public string? STATECODE { get; set; }
        public string? OTHERSTATENAME { get; set; }
        public string? CITYCODE { get; set; }
        public string? OTHERCITYNAME { get; set; }
        public string? ADDRESSLINE1 { get; set; }
        public string? ADDRESSLINE2 { get; set; }
        public string? POSTBOXNUMBER { get; set; }
        public string? POSTALCODE { get; set; }
        public string? CONTACTNAME { get; set; }
        public string? INTERNATIONALDIALINGCODE { get; set; }
        public string? AREADIALINGCODE { get; set; }
        public string? PHONENUMBER { get; set; }
        public string? PHONEEXTENSIONNUMBER { get; set; }
        public string? MOBILENUMBER { get; set; }
        public string? FAXNUMBER { get; set; }
        public string? EMAIL { get; set; }
        public string? SOCUSTOMERCODE { get; set; }
        public string? SALESORDERTYPE { get; set; }
        public string? CUSTOMERBILLTO { get; set; }
        public string? CUSTOMERSHIPTO { get; set; }
        public string? SHIPPINGMODE { get; set; }
        public string? TRADETERM { get; set; }
        public string? SALESORDERDATE { get; set; }
        public string? CUSTOMERORDERNO { get; set; }
        public string? CUSTOMERORDERLINENO { get; set; }
        public string? PARTCODE { get; set; }
        public string? ALLOCATEDQTY { get; set; }
        public string? PARTBATCHNO { get; set; }
        public string? SERIALNO { get; set; }
        public string? EXPIRYDATE { get; set; }
        public string? ADDITIONALINFORMATION { get; set; }
        public string? ESTTIMEOFDELIVERY { get; set; }
        public string? ORIGINCOMPANY { get; set; }
        public string? DESTINATIONCOMPANY { get; set; }
        public string? WAREHOUSE { get; set; }
        public string? SIZE { get; set; }
        public string? COLOR { get; set; }
        public string? PAYMENT { get; set; }
        public string VALUE { get; set; } = string.Empty;
        public string DIMENTION { get; set; } = string.Empty;
    }
}
