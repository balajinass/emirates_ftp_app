using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace emirates_ftp_app.Model.Email
{
    internal class EmailTemplate
    {
        [Key]
        public int ID { get; set; }
        public string CREATE_USER { get; set; } = "";
        public DateTime CREATE_DATE { get; set; }
        public string RUN_USER { get; set; } = "";
        public DateTime RUN_DATE { get; set; }
        public string? TEMPLATE { get; set; }
        public string? STATUS { get; set; }
    }
}
