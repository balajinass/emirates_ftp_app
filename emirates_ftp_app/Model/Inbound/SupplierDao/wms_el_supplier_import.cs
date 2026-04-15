using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Inbound.SupplierDao
{
    internal class wms_el_supplier_import
    {
        public string? CREATE_USER { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string? RUN_USER { get; set; }
        public DateTime RUN_DATE { get; set; }
        public int SOURCE_SL_NO { get; set; }
        public string? SL_NO { get; set; }
        public string? SUPPLIER_NAME { get; set; }
        public string? SUPPLIER_CODE { get; set; }
        public string? SHIP_TO_CODE { get; set; }
        public string? DEFAULT_TRADE_TERMCODE { get; set; }
        public string? DEFAULT_FREIGHT_FORWARDER_CODE { get; set; }
        public string? SUPPLIER_CURRENCY_CODE { get; set; }
        public string? ADDRESS_LINE1 { get; set; }
        public string? ADDRESS_LINE2 { get; set; }
        public string? POST_BOX_NUMBER { get; set; }
        public string? POSTAL_CODE { get; set; }
        public string? CITY_CODE { get; set; }
        public string? OTHER_CITY_NAME { get; set; }
        public string? STATE_CODE { get; set; }
        public string? OTHER_STATE_NAME { get; set; }
        public string? COUNTRY_CODE { get; set; }
        public string? CONTACT_NAME { get; set; }
        public string? INTERNATIONAL_DIALING_CODE { get; set; }
        public string? AREA_DIALING_CODE { get; set; }
        public string? PHONE_NUMBER { get; set; }
        public string? PHONE_EXTENSION_NUMBER { get; set; }
        public string? PHONE_NUMBER2 { get; set; }
        public string? EMAIL { get; set; }
        public string? PRIMARY_COMPANY { get; set; }
        public DateTime TRANSACTION_TIME { get; set; }
        public string? FILE_STATUS { get; set; }
        public string? NOTE { get; set; }
    }
}
