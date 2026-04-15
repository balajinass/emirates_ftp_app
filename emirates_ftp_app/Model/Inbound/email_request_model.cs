using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace emirates_ftp_app.Model.Inbound
{
    internal class email_request_model
    {
        public string? Module { get; set; }
        public string? File_name { get; set; }
        public string? Processed_On { get; set; }
        public string? Status { get; set; }
        public string? Company_Name { get; set; }
        public string? Archive_Status { get; set; }
    }
}
