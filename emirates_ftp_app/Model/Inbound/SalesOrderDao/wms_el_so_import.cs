using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Inbound.SalesOrderDao
{
    internal class wms_el_so_import
    {
        public int SL_NO { get; set; }
        public string? CREATE_USER { get; set; } 
        public DateTime? CREATE_DATE { get; set; }
        public string? RUN_USER { get; set; } 
        public DateTime? RUN_DATE { get; set; }
        public int SOURCE_SL_NO { get; set; }
        public string? CUSTOMER_CODE { get; set; } 
        public string? SO_TYPE { get; set; } 
        public string? CUSTOMER_BILL_TO { get; set; } 
        public string? CUSTOMER_SHIP_TO { get; set; } 
        public string? SHIPPING_MODE { get; set; } 
        public string? TRADE_TERM { get; set; } 
        public DateTime? SO_DATE { get; set; }
        public string? CUSTOMER_ORDER_NO { get; set; } 
        public string? CUSTOMER_ORDER_LINE_NO { get; set; } 
        public string? PART_CODE { get; set; } 
        public int ALLOCATED_QTY { get; set; }
        public string? PART_BATCH_NO { get; set; } 
        public string? SERIAL_NO { get; set; } 
        public DateTime? EXPIRY_DATE { get; set; }
        public string? ADDITIONAL_INFORMATION { get; set; } 
        public DateTime? EST_TIME_OF_DEL { get; set; }
        public string? PRIMARY_COMPANY { get; set; } 
        public string? WAREHOUSE { get; set; } 
        public string? COST_BUCKET { get; set; } 
        public string? PAR_TYPE_CODE { get; set; } 
        public string? SO_ID { get; set; } 
        public DateTime? TRANSACTION_TIME { get; set; }
        public string? FILE_STATUS { get; set; } 
        public string? NOTE { get; set; } 
        public string? DESTINATION_COMPANY { get; set; } 
        public string? COLOR { get; set; } 
        public string? ITEM_SIZE { get; set; } 
        public string? PAYMENT { get; set; } 
        public string? DIMENTION { get; set; } 
        public string? VALUE { get; set; }
        public string? COUNTRY_CODE { get; set; } 
        public string? COST_CURRENCY_CODE { get; set; } // for local
        //public string? CUST_CURRENCY_CODE { get; set; } // for Prod
    }
}
