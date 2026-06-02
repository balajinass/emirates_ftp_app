using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Model.Common
{
    public class TokenRequest
    {
        public string? clientID { get; set; }
        public string? saasID { get; set; }
    }
    public class TokenResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public TokenDetails? Details { get; set; }
    }

    public class TokenDetails
    {
        public string? Token { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
    }
}
