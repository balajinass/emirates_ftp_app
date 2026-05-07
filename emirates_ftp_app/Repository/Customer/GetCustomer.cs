using emirates_ftp_app.Data;
//using emirates_ftp_app.EmailRepository.EmailSendWithLogService;
using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Ftp;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
//using emirates_ftp_app.Model.Email;
using emirates_ftp_app.Model.Nass;
using emirates_ftp_app.Repository.CommonFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace emirates_ftp_app.Repository.Customer
{
    internal class GetCustomer : IGetCustomer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommonFunctions oCommon_;

        public GetCustomer(IServiceProvider serviceProvider,ICommonFunctions oCommon)
        {
            _serviceProvider = serviceProvider;
            oCommon_ = oCommon;
        }

        #region GetCustomerList
        public async Task<List<web_wms_edi_config_model>> GetListofCustomer()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<NassDbContext>();
                    return await context.WEB_WMS_EDI_CONFIG
                                        .Where(c => c.LOV_STATUS == "DISPLAY")
                                        .Include(c => c.MODULES!.Where(m => m.LOV_STATUS == "DISPLAY"))
                                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("Error in GetListofCustomer: " + ex);
                Console.ForegroundColor = previousColor;               
                MyLogger.GetInstance().Error(ex.ToString());                
                return new List<web_wms_edi_config_model>();  
            }

        }
        #endregion

        #region OutBound_CustomerGet
        public async Task<List<web_wms_edi_config_model>> GetListofCustomerOUTBOUND()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<NassDbContext>();

                var result = await context.WEB_WMS_EDI_CONFIG
                    .Where(c => c.LOV_STATUS == "DISPLAY").Select(c => new web_wms_edi_config_model
                    {
                        PROJECT_ID = c.PROJECT_ID,
                        PROJECT_NAME = c.PROJECT_NAME,
                        CREATE_USER = c.CREATE_USER,
                        CREATE_DATE = c.CREATE_DATE,
                        RUN_USER = c.RUN_USER,
                        RUN_DATE = c.RUN_DATE,
                        SAAS_ID = c.SAAS_ID,
                        COMPANY_CODE = c.COMPANY_CODE,
                        BRANCH_CODE = c.BRANCH_CODE,
                        LOCATION_CODE = c.LOCATION_CODE,
                        WAREHOUSE_CODE = c.WAREHOUSE_CODE,
                        CUSTOMER_CODE = c.CUSTOMER_CODE,
                        API_KEY = c.API_KEY,
                        FTP_URL = c.FTP_URL,
                        FTP_PORT = c.FTP_PORT,
                        FTP_USERNAME = c.FTP_USERNAME,
                        FTP_PASSWORD = c.FTP_PASSWORD,
                        FROM_EMAIL = c.FROM_EMAIL,
                        TO_EMAIL = c.TO_EMAIL,
                        CC_EMAIL = c.CC_EMAIL,
                        ERROR_EMAIL = c.ERROR_EMAIL,
                        LOV_STATUS = c.LOV_STATUS,

                        // Filter only DISPLAY rows in OUTBOUND
                        OUTBOUND = c.OUTBOUND!
                                     .Where(o => o.LOV_STATUS == "DISPLAY")
                                     .ToList(),

                        // If you also want MODULES filtered by DISPLAY:
                        MODULES = c.MODULES!
                                     .Where(m => m.LOV_STATUS == "DISPLAY")
                                     .ToList()
                    })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("Error in GetListofCustomerOUTBOUND: " + ex);
                Console.ForegroundColor = previousColor;
                MyLogger.GetInstance().Error(ex.ToString());
                return new List<web_wms_edi_config_model>();
            }
        }
        #endregion

        #region Old_InsertEdiLog
        //public async Task<bool> InsertEdiLog(wms_edi_ftp_model ediFtp)
        //{
        //    try
        //    {
        //        using (var scope = _serviceProvider.CreateScope())
        //        {
        //            var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

        //            var now = DateTime.Now;
        //            ediFtp.FILE_UPLOAD_TIME = now;
        //            ediFtp.FILE_WRITE_TIME = now;
        //            ediFtp.FILE_DOWNLOAD_TIME = now;
        //            ediFtp.FILE_CREATED_TIME = now;

        //            await context.WMS_EDI_FTP.AddAsync(ediFtp);
        //            int result = await context.SaveChangesAsync();

        //            return result > 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Error(ex.ToString());
        //        return false;
        //    }
        //}
        #endregion

        #region InsertEdiLog
        public async Task<wms_edi_ftp_model> InsertEdiLog(web_wms_edi_config_model oCustomer_,web_wms_edi_module_config_model oModule_,input_request_model oFiles)
        {
            wms_edi_ftp_model ediFtp_ = new wms_edi_ftp_model();

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                var filePath = Path.Combine(oModule_.LOCAL_FILE_PATH!, oFiles.fileName!);

                ediFtp_ = new wms_edi_ftp_model()
                {
                    SL_NO = oFiles.slNo,
                    FILE_UPLOAD_TIME = DateTime.Now,
                    FILE_WRITE_TIME = DateTime.Now,
                    FILE_DOWNLOAD_TIME = DateTime.Now,
                    FILE_CREATED_TIME = DateTime.Now,                   
                    COMPANY_CODE = oCustomer_?.COMPANY_CODE,
                    BRANCH_CODE = oCustomer_?.BRANCH_CODE,
                    LOCATION_CODE = oCustomer_?.LOCATION_CODE,
                    FILE_NAME = oFiles?.fileName,
                    FILE_TYPE = oFiles?.moduleType,
                    FILE_CONTENT = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty,
                    REFERENCE_ID = oFiles?.fileName,
                    FILE_STATUS = "NEW",
                    IN_OUT = "I",
                    PRIMARY_COMPANY = oCustomer_?.PROJECT_NAME
                };

                await context.WMS_EDI_FTP.AddAsync(ediFtp_);
                await context.SaveChangesAsync();
                Console.WriteLine("Insert completed in WMS_EDI_FTP");

                return ediFtp_;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error($"Error processing file {oFiles.fileName}: {ex.Message}");
                var exceptionHtml = await oCommon_.GenerateExceptionHtml("Insert error in WMS_EDI_FTP", ex);
                await oCommon_.SendFinalMail($"Insert in WMS_EDI_FTP - {oFiles.fileName}", exceptionHtml, "ExceptionEmail");

                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine($"Error in InsertEdiLog: {ex}");
                Console.ForegroundColor = previousColor;               
                MyLogger.GetInstance().Error(ex.ToString());
                return null!;
            }
        }
        #endregion

        #region CheckExecutedStatus
        public bool CheckExecutedStatus(web_edi_ftp_model ediFtp)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();
                return !context.WMS_EDI_FTP.Any(f => f.FILE_TYPE == ediFtp.FILE_TYPE &&
                                                     f.REFERENCE_ID == ediFtp.REFERENCE_ID);
            }
        }
        #endregion

        public async Task<email_request_model?> GetEdiFileAsEmailRequestAsync(string fileName,string module)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                // Query the table for the record with the given file name
                var ediFtp = await context.WMS_EDI_FTP
                                           .Where(x => x.FILE_NAME == fileName)
                                           .FirstOrDefaultAsync();

                if (ediFtp == null)
                {
                    Console.WriteLine("No record found with the given filename");
                    return null;
                }

                // Map WMS_EDI_FTP record to email_request_model
                var emailRequest = new email_request_model
                {
                    Module= module,
                    File_name = ediFtp.FILE_NAME,
                    Processed_On = ediFtp.FILE_UPLOAD_TIME?.ToString("yyyy-MM-dd HH:mm:ss"),
                    Status = ediFtp.FILE_STATUS,
                    Archive_Status = "True" , 
                    Company_Name=ediFtp.PRIMARY_COMPANY
                };

                return emailRequest;
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine($"Error in GetEdiFileAsEmailRequestAsync: {ex}");
                Console.ForegroundColor = previousColor;                
                MyLogger.GetInstance().Error(ex.ToString());
                return null;
            }
        }
    }
}
