using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Inbound.ASNDao
{
    internal class wms_el_po_import
    {
        public string? CREATE_USER { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string? RUN_USER { get; set; }
        public DateTime RUN_DATE { get; set; }
        public int SOURCE_SL_NO { get; set; }
        public int SL_NO { get; set; }
        public string? PRIMARY_COMPANY { get; set; }
        public string? WAREHOUSE { get; set; }
        public string? COST_BUCKET { get; set; }
        public string? SHIPPED_PART_CODE { get; set; }
        public string? SUPPLIER_CODE { get; set; }
        public string? SHIPPING_MODE_CODE { get; set; }
        public string? PO_TYPE_CODE { get; set; }
        public string? PART_TYPE_CODE { get; set; }
        public string? PO_NUMBER { get; set; }
        public string? PO_LINE_NUMBER { get; set; }
        public string? BILLING_UOM_CODE { get; set; }
        public string? SUPPLIER_INVOICE_NO { get; set; }
        public DateTime? SUPPLIER_INVOICE_DATE { get; set; }
        public string? SHIPPED_QUAN_BILL_UOM { get; set; }
        public DateTime? EXPIRY_DATE { get; set; }
        public DateTime? MANUFACTURING_DATE { get; set; }
        public string? SERIAL_NO { get; set; }
        public string? BATCH_NO { get; set; }
        public DateTime? ESTIMATED_TIME_OF_ARRIVAL { get; set; }
        public string? BILL_OF_LADING { get; set; }
        public string? FILE_STATUS { get; set; }
        public string? COLOR { get; set; }
        public string? ITEM_SIZE { get; set; }
    }
}
