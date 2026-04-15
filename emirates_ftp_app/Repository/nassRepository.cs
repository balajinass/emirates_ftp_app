using emirates_ftp_app.Data;
using emirates_ftp_app.Model.Nass;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Repository
{
    internal class nassRepository
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly NassDbContext _nassDbContext;

        public nassRepository(NassDbContext nassDbContext)
        {
            _nassDbContext = nassDbContext;
        }

        public async Task<List<web_wms_edi_config_model>> GetListofConfig()
        {
            List<web_wms_edi_config_model> listofConfig_ = new List<web_wms_edi_config_model>();

            try
            {
                //listofConfig_ = await _nassDbContext.WEB_WMS_EDI_MODULE_CONFIG
                //    .AsNoTracking()
                //    .Include(x => x.MODULES)
                //    .Where(x => x.LOV_STATUS == "DISPLAY")
                //    .ToListAsync();

                //if (listofResponse_ != null && listofResponse_.Count > 0)
                //{
                //    foreach (var item in listofResponse_)
                //    {
                //        if (item.MODULES != null)
                //        {
                //            item.MODULES = item.MODULES
                //                .Where(m => m.LOV_STATUS == "DISPLAY")
                //                .OrderBy(m => m.SL_NO)
                //                .ToList();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetListofConfig()");
            }

            return listofConfig_;
        }
    }
}
