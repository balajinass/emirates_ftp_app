using emirates_ftp_app.Data;
using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Nass;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace emirates_ftp_app.Repository.Oubound.Common
{
    internal class CommonDao : ICommonDao
    {
        private readonly IServiceProvider _serviceProvider;

        public CommonDao(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region GetFileContent
        public async Task<List<wms_edi_ftp_content>> GetFileContent(web_wms_edi_config_model customerConfig,string module)
        {
            if (customerConfig.OUTBOUND == null || !customerConfig.OUTBOUND.Any())
                return new List<wms_edi_ftp_content>();

            var fileTypesForCustomer = customerConfig.OUTBOUND
                .Where(o => !string.IsNullOrEmpty(o.FILE_TYPE)
                            && !string.IsNullOrEmpty(o.MODULE_NAME)
                            && o.MODULE_NAME.Equals(module, StringComparison.OrdinalIgnoreCase))
                .Select(o => o.FILE_TYPE)
                .ToList();

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                    var files = await dbContext.WMS_EDI_FTP
                        .Where(f => f.FILE_STATUS == "NEW" &&
                                    f.PRIMARY_COMPANY == customerConfig.PROJECT_NAME &&
                                    fileTypesForCustomer.Contains(f.FILE_TYPE))
                        .Select(f => new wms_edi_ftp_content
                        {
                            FILE_NAME = f.FILE_NAME,
                            FILE_CONTENT = f.FILE_CONTENT,
                            SL_NO = f.SL_NO,
                        })
                        .ToListAsync();

                    return files;
                }
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine($"Error in  GetFileContent: {ex}");
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error($"Error in  GetFileContent: {ex}");               
                return new List<wms_edi_ftp_content>();
            }
        }
        #endregion

        #region UpdateFileStatus
        public async Task UpdateFileStatus(string fileName,string PrimaryCompany)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                //int updatedCount = await dbContext.WMS_EDI_FTP
                //    .Where(f => f.FILE_NAME == fileName && f.FILE_STATUS == "NEW" && f.PRIMARY_COMPANY == PrimaryCompany)
                       int updatedCount = await dbContext.WMS_EDI_FTP
                    .Where(f => f.FILE_NAME == fileName && f.FILE_STATUS == "NEW" && f.PRIMARY_COMPANY == PrimaryCompany)
                    .ExecuteUpdateAsync(f => f.SetProperty(p => p.FILE_STATUS, "PROCESSED"));

                Console.WriteLine($"{updatedCount} file(s) with name {fileName} marked as PROCESSED.");
                MyLogger.GetInstance().Info($"{updatedCount} file(s) with name {fileName} marked as PROCESSED.");
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine($"Error updating status for file {fileName}: {ex}");
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error($"Error updating status for file {fileName}: {ex}");                
            }
        }
        #endregion
    }
}
