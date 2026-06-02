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
                            FILE_UPLOAD_TIME=f.FILE_UPLOAD_TIME,
                            FILE_TYPE = f.FILE_TYPE,
                            FILE_STATUS = f.FILE_STATUS,
                            TRANSACTION_TIME = f.TRANSACTION_TIME,
                            PRIMARY_COMPANY = f.PRIMARY_COMPANY,
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
        public async Task UpdateFileStatus(string fileName, string primaryCompany)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                MyLogger.GetInstance().Info($"WMS_EDI_FTP Updating Status | FILE_NAME : {fileName} | PRIMARY_COMPANY : {primaryCompany} | FILE_STATUS : PROCESSED | FILE_WRITE_TIME : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                int updatedCount = await dbContext.WMS_EDI_FTP.Where(f => f.FILE_NAME == fileName
                  && f.FILE_STATUS == "NEW" 
                  && f.PRIMARY_COMPANY == primaryCompany).ExecuteUpdateAsync(f => f.SetProperty(p => p.FILE_STATUS, "PROCESSED").SetProperty(p => p.FILE_WRITE_TIME, DateTime.Now));

                if (updatedCount > 0)
                {
                    MyLogger.GetInstance().Info(
                        $"WMS_EDI_FTP Status Updated Successfully | FILE_NAME : {fileName} | PRIMARY_COMPANY : {primaryCompany} | RECORDS_UPDATED : {updatedCount}"
                    );                  
                    MyLogger.GetInstance().Info(
                        $"************************************************************");                

                }
                else
                {
                    MyLogger.GetInstance().Warning(
                        $"WMS_EDI_FTP No Records Found To Update | FILE_NAME : {fileName} | PRIMARY_COMPANY : {primaryCompany}"
                    );                  
                    MyLogger.GetInstance().Info(
                        $"************************************************************");                 
                }
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error(
                    $"WMS_EDI_FTP Error Updating Status | FILE_NAME : {fileName} | PRIMARY_COMPANY : {primaryCompany} | ERROR : {ex.Message}"
                );
                MyLogger.GetInstance().Info(
                        $"************************************************************");              
            }
        }
        #endregion
    }
}
