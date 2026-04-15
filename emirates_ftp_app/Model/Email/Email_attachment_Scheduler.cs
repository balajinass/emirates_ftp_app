using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace emirates_ftp_app.Model.Email
{
    internal class Email_attachment_Scheduler
    {
        [Key]
        public int ID { get; set; }
        public int HEADER_ID { get; set; }
        public string? CREATE_USER { get; set; } = "";
        public DateTime CREATE_DATE { get; set; }
        public string? RUN_USER { get; set; } = "";
        public DateTime RUN_DATE { get; set; }
        public string? FILE_NAME { get; set; }
        public string? FILE_TYPE { get; set; }
        public long? FILE_SIZE { get; set; }
        public byte[]? FILE_CONTENT { get; set; }
        public Email_Scheduler? EMAIL_LOG { get; set; }
    }
}
