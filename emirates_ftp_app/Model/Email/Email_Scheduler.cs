using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace emirates_ftp_app.Model.Email
{
    internal class Email_Scheduler
    {
        [Key]
        public int ID { get; set; }
        public string CREATE_USER { get; set; } = "";
        public DateTime CREATE_DATE { get; set; }
        public string RUN_USER { get; set; } = "";
        public DateTime RUN_DATE { get; set; }
        public string? TYPE { get; set; }
        public string? FROM_EMAIL { get; set; }
        public string? TO_EMAIL { get; set; }
        public string? SUBJECT { get; set; }
        public string? BODY { get; set; }
        public string? STATUS { get; set; }
        public string? STATUS_CODE { get; set; }
        public string? ERROR_MESSAGE { get; set; }
        public int TEMPLATE_ID { get; set; }
        public List<Email_attachment_Scheduler>? ATTACHMENTS { get; set; }
    }
}
