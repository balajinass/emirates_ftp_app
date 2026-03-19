using emirates_ftp_app.Model.Common;
using emirates_ftp_app.Model.Nass;
using emirates_ftp_app.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;

namespace emirates_ftp_app.Middleware.Inbound
{
    internal class SalesOrder
    {
        private readonly ILogger<SalesOrder> _logger;
        private readonly AppSettings _appSettings;
        private readonly nassRepository _nassRepository;

        public SalesOrder(ILogger<SalesOrder> logger,            
            IOptions<AppSettings> appSettings,
            nassRepository nassRepository)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _nassRepository = nassRepository;
        }

        public async Task SoCreation(string module)
        {
            _logger.LogInformation($"{module} creation started");
            try
            {
                List<web_wms_edi_config_model> listofConfig_ = await _nassRepository.GetListofConfig();
                if(listofConfig_ != null)
                {
                    foreach(web_wms_edi_config_model config_ in listofConfig_)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SoCreation()");
            }
        }
    }
}
