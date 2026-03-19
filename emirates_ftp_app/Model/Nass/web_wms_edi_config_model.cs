using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Nass
{
    public class web_wms_edi_config_model
    {
        public string? CREATE_USER { get; set; }
        public string? CREATE_DATE { get; set; }
        public string? RUN_USER { get; set; }
        public string? RUN_DATE { get; set; }
        public string? PROJECT_ID { get; set; }
        public string? PROJECT_NAME { get; set; }
        public string? SAAS_ID { get; set; }
        public string? COMPANY_CODE { get; set; }
        public string? BRANCH_CODE { get; set; }
        public string? LOCATION_CODE { get; set; }
        public string? WAREHOUSE_CODE { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public string? API_KEY { get; set; }
        public string? FTP_URL { get; set; }
        public string? FTP_PORT { get; set; }
        public string? FTP_USERNAME { get; set; }
        public string? FTP_PASSWORD { get; set; }
        public string? FROM_EMAIL { get; set; }
        public string? TO_EMAIL { get; set; }
        public string? CC_EMAIL { get; set; }
        public string? ERROR_EMAIL { get; set; }
        public string? LOV_STATUS { get; set; }
        public List<web_wms_edi_module_config_model>? MODULES { get; set; }
    }

    public class web_wms_edi_module_config_model
    {
        public string? CREATE_USER { get; set; }
        public string? CREATE_DATE { get; set; }
        public string? RUN_USER { get; set; }
        public string? RUN_DATE { get; set; }
        public string? PROJECT_ID { get; set; }
        public string? SL_NO { get; set; }
        public string? MODULE_NAME { get; set; }
        public string? FTP_FILE_PATH { get; set; }
        public string? FTP_FILE_BACKUP_PATH { get; set; }
        public string? FTP_FILE_ERROR_PATH { get; set; }
        public string? LOCAL_FILE_PATH { get; set; }
        public string? LOCAL_FILE_BACKUP_PATH { get; set; }
        public string? LOCAL_FILE_ERROR_PATH { get; set; }
        public string? LOV_STATUS { get; set; }
    }
}
