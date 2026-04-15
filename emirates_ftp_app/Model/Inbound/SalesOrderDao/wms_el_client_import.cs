using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Inbound.SalesOrderDao
{
    internal class wms_el_client_import
    {
        public string? CREATE_USER { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public string? RUN_USER { get; set; }
        public DateTime? RUN_DATE { get; set; }
        public int SOURCE_SL_NO { get; set; }
        public int SL_NO { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public string? CUSTOMER_NAME { get; set; }
        public string? PRIMARY_COMPANY { get; set; }
        public string? WAREHOUSE { get; set; }
        public string? CUSTOMER_TYPE_CODE { get; set; }
        public string? CUST_CURRENCY_CODE { get; set; }
        public string? COUNTRY_CODE { get; set; }
        public string? STATE_CODE { get; set; }
        public string? OTHER_STATE_NAME { get; set; }
        public string? CITY_CODE { get; set; }
        public string? OTHER_CITY_NAME { get; set; }
        public string? ADDRESS_LINE1 { get; set; }
        public string? ADDRESS_LINE2 { get; set; }
        public string? POST_BOX_NUMBER { get; set; }
        public string? POSTAL_CODE { get; set; }
        public string? CONTACT_NAME { get; set; }
        public string? INTERANTIONAL_DIALING_CODE { get; set; }
        public string? AREA_DIALING_CODE { get; set; }
        public string? PHONE_NUMBER { get; set; }
        public string? PHONE_EXTENSION_NUMBER { get; set; }
        public string? MOBILE_NUMBER { get; set; }
        public string? FAX_NUMBER { get; set; }
        public string? EMAIL { get; set; }
        public string? FILE_STATUS { get; set; }
        public string? NOTE { get; set; }
    }
}
