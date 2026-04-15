using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Inbound.SoCancelDao
{
    internal class wms_el_so_cancel_import
    {
        public string? CREATE_USER { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string? RUN_USER { get; set; }
        public DateTime RUN_DATE { get; set; }
        public int SOURCE_SL_NO { get; set; }
        public string? SL_NO { get; set; }
        public string? PRIMARY_COMPANY { get; set; }
        public string? COST_BUCKET { get; set; }
        public string? SALES_ORDER_NO { get; set; }
        public string? SALES_ORDER_LINE_NO { get; set; }
        public string? EXTERNAL_SO_NO { get; set; }
        public string? ORDER_LINE_NO { get; set; }
        public string? ITEM_REF { get; set; }
        public string? CANCELLATION_QUANTITY { get; set; }
        public string? SHIP_TO_CODE { get; set; }
        public DateTime? TRANSACTION_TIME { get; set; }
        public string? FILE_STATUS { get; set; }
        public string? NOTE { get; set; }
    }
}
