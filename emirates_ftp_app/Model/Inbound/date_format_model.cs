using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Inbound
{
    internal class date_format_model
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public DateTime? modifiedDate { get; set; }
        public string? actualDate { get; set; }
    }
}
