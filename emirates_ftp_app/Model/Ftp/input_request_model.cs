using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Ftp
{
    internal class input_request_model
    {
        public string? lineItem { get; set; }
        public string? fileName { get; set; }
        public string? dateTime { get; set; }
        //public string? typeName { get; set; }
        //public string? typeDigit { get; set; }
        public string? fileExtension { get; set; }
        public int slNo { get; set; }
        public string? moduleType { get; set; }
    }
}
