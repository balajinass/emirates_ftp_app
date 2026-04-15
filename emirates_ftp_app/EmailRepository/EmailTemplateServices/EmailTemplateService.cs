using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.EmailRepository.EmailTemplateServices
{
    internal class EmailTemplateService
    {
        private readonly IConfiguration _configuration;

        public EmailTemplateService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetFormattedEmailBody(string template)
        {
            // Get placeholders from appsettings
            var title = _configuration["EmailTemplate:Title"];
            var companyName = _configuration["EmailTemplate:CompanyName"];
            var waveMessage = _configuration["EmailTemplate:WaveMessage"];
            var website = _configuration["EmailTemplate:WebsiteName"];
            var bestRegards = _configuration["EmailTemplate:BestRegards"];
            var companyNameBottom = _configuration["EmailTemplate:CompanyNameBottom"];
            var copyrights = _configuration["EmailTemplate:Copyrights"];
            var emailContent = _configuration["EmailTemplate:EmailContent"];
            var emailContentImportant = _configuration["EmailTemplate:EmailContentImportant"];
            var username = _configuration["EmailTemplate:UserName"];
            var hostname = _configuration["EmailTemplate:HostName"];

            // Replace placeholders in the template
            var formattedTemplate = template
                .Replace("{{TITLE}}", title)
                .Replace("{{COMPANYNAME}}", companyName)
                .Replace("{{WAVEMESSAGE}}", waveMessage)
                .Replace("{{WEBSITENAME}}", website)
                .Replace("{{BESTREGARDS}}", bestRegards)
                .Replace("{{COMPANYNAMEBTM}}", companyNameBottom)
                .Replace("{{COPYRIGHTS}}", copyrights)
                .Replace("{{EMAILCONTENT}}", emailContent)
                .Replace("{{EMAILCONTENTIMP}}", emailContentImportant)
                .Replace("{{USERNAME}}", username)
                .Replace("{{WEBHOSTNAME}}", hostname);

            return formattedTemplate;
        }
    }
}
