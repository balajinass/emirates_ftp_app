using emirates_ftp_app.Data;
//using emirates_ftp_app.EmailRepository.EmaillogInterface;
//using emirates_ftp_app.EmailRepository.EmailSendService;
//using emirates_ftp_app.EmailRepository.EmailSendWithLogService;
//using emirates_ftp_app.EmailRepository.EmailTemplateServices;
using emirates_ftp_app.Middleware.Inbound;
using emirates_ftp_app.Middleware.Outbound;
using emirates_ftp_app.Model.Common;
using emirates_ftp_app.Repository;
using emirates_ftp_app.Repository.CommonFunctions;
using emirates_ftp_app.Repository.Customer;
using emirates_ftp_app.Repository.FtpConnection;
using emirates_ftp_app.Repository.Inbound.Asn;
using emirates_ftp_app.Repository.Inbound.SalesOrders;
using emirates_ftp_app.Repository.Inbound.SoCancel;
using emirates_ftp_app.Repository.Inbound.Supplier;
using emirates_ftp_app.Repository.Oubound.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;
using System.Dynamic;

namespace emirates_ftp_app
{
    internal class Program
    {
        static async Task Main(string[] args)
        {            
            string moduleName = string.Empty;
            //"SO", "ASN", "SUPPLIER", "SOCANCEL","DNUPLOAD", "PUTAWAY", "STOCKTRANSFER"
            //args = new[] { "PUTAWAY" }; // dev - only test input
            string allRows = "";
            string allErrors = "";
            if (args == null || args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: Module name argument is required.");
                Console.WriteLine("Usage: emirates_ftp_app <MODULE_NAME>");
                Console.WriteLine("Example: emirates_ftp_app SKU");
                Console.ResetColor();

                Environment.ExitCode = 1;

                return;
            }
            else
            {
                moduleName = args[0].ToUpperInvariant(); 
            }

            string runStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            GlobalDiagnosticsContext.Set("ModuleName", moduleName);
            GlobalDiagnosticsContext.Set("RunStamp", runStamp);
            var configPath = System.IO.Path.Combine(AppContext.BaseDirectory, "nlog.config");
            Console.WriteLine("NLog config path: " + configPath);
            var Logger = LogManager.Setup()
                .LoadConfigurationFromFile(System.IO.Path.Combine(AppContext.BaseDirectory, "nlog.config"), optional: false)
                .GetCurrentClassLogger();

            try
            {
                Logger.Info("Application started for module {Module}", moduleName);
                using var host = Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        config.SetBasePath(AppContext.BaseDirectory);
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                                            optional: true, reloadOnChange: true);
                        config.AddEnvironmentVariables();
                    })
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                        // 🔇 stop EF SQL command logs
                        // General rules first
                        logging.AddFilter("Microsoft", Microsoft.Extensions.Logging.LogLevel.Warning);
                        logging.AddFilter("Microsoft.EntityFrameworkCore", Microsoft.Extensions.Logging.LogLevel.Warning);

                        // Most specific rules LAST (so they win)
                        logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", Microsoft.Extensions.Logging.LogLevel.None);
                        logging.AddFilter("Microsoft.EntityFrameworkCore.Model.Validation", Microsoft.Extensions.Logging.LogLevel.None);
                        logging.AddNLog();
                    })
                    .ConfigureServices((context, services) =>
                    {
                        var tnsAdminPath = context.Configuration.GetValue<string>("OracleSettings:TnsAdmin");

                        if (!string.IsNullOrEmpty(tnsAdminPath))
                        {
                            Environment.SetEnvironmentVariable("TNS_ADMIN", tnsAdminPath);
                        }
                        var primaryCs = context.Configuration.GetConnectionString("PrimaryConnection");
                        var nassCs = context.Configuration.GetConnectionString("NassConnection");
                        //var emailcs = context.Configuration.GetConnectionString("EmailConnection");

                        Console.WriteLine("PrimaryConnection = " + (string.IsNullOrWhiteSpace(primaryCs) ? "NULL" : "FOUND"));
                        Console.WriteLine("NassConnection    = " + (string.IsNullOrWhiteSpace(nassCs) ? "NULL" : "FOUND"));
                       // Console.WriteLine("EmailConnection    = " + (string.IsNullOrWhiteSpace(emailcs) ? "NULL" : "FOUND"));

                        if (string.IsNullOrWhiteSpace(primaryCs))
                            throw new InvalidOperationException("Connection string 'PrimaryConnection' not found.");

                        if (string.IsNullOrWhiteSpace(nassCs))
                            throw new InvalidOperationException("Connection string 'NassConnection' not found.");

                        services.Configure<AppSettings>(context.Configuration.GetSection("Settings"));

                        services.AddDbContext<PrimaryDbContext>(options =>
                        {
                            options.UseOracle(primaryCs, o => o.CommandTimeout(120));
                            options.EnableSensitiveDataLogging(false);
                            options.EnableDetailedErrors(false);
                        });

                        services.AddDbContext<NassDbContext>(options =>
                        {
                            options.UseOracle(nassCs, o => o.CommandTimeout(120));
                            options.EnableSensitiveDataLogging(false);
                            options.EnableDetailedErrors(false);
                        });

                        services.AddScoped<FtpConnection>();
                        services.AddScoped<ICommonFunctions, CommonFunctions>();                       
                        services.AddScoped<IGetCustomer, GetCustomer>();
                        services.AddScoped<SalesOrder>();
                        services.AddScoped<ISalesOrderDao, SalesOrderDao>();
                        services.AddScoped<AdvanceShippingNotice>();
                        services.AddScoped<IASNDao, ASNDao>();
                        services.AddScoped<Supplier>();
                        services.AddScoped<ISupplierDao, SupplierDao>();
                        services.AddScoped<SoCancel>();
                        services.AddScoped<ISoCancelDao, SoCancelDao>();

                        //OutBound//                        
                        services.AddScoped<ICommonDao, CommonDao>();
                        services.AddScoped<DNUpload>();
                        services.AddScoped<PutAwayUpload>();
                        services.AddScoped<StockTransferUpload>();
                        #region oldservices
                        //services.AddScoped<IEmaillogInterface, EmaillogInterface>();
                        //services.AddScoped<IEmailSendService,EmailSendService>();
                        //services.AddScoped<IEmailSendWithLogService, EmailSendWithLogService>();
                        //services.AddScoped<EmailTemplateService>();
                        // services.AddScoped<NassDbContext>();
                        //services.AddScoped<SalesOrder>();
                        //services.AddScoped<nassRepository>();
                        #endregion
                    })
                    .Build();

                
                var oCommon_ = host.Services.GetRequiredService<ICommonFunctions>();                
                    

                    switch (moduleName)
                    {
                        case "SO":
                            {
                                var oSku = host.Services.GetRequiredService<SalesOrder>();                               
                                var (soData, soErrors) = await oSku.SoCreation("SALESORDER");

                                allRows += await oCommon_.GenerateSummaryTableRows(soData);
                                allErrors += string.Join("<br/><hr/><br/>", soErrors);
                                break;
                            }

                        case "ASN":
                            {
                                var oAsn = host.Services.GetRequiredService<AdvanceShippingNotice>();  
                                var (soData, soErrors) = await oAsn.AsnCreation("ASN");
                                allRows += await oCommon_.GenerateSummaryTableRows(soData);
                                allErrors += string.Join("<br/><hr/><br/>", soErrors);
                                break;
                            }

                        case "SUPPLIER":
                            {
                                var oSup = host.Services.GetRequiredService<Supplier>();                                
                                var (soData, soErrors) = await oSup.SupplierCreation("SUPPLIER");
                                allRows += await oCommon_.GenerateSummaryTableRows(soData);
                                allErrors += string.Join("<br/><hr/><br/>", soErrors);

                                break;
                            }
                        case "SOCANCEL":
                            {
                                var oSoc = host.Services.GetRequiredService<SoCancel>();
                                var (soData, soErrors) = await oSoc.SoCancelCreation("SALESORDERCANCEL");
                                allRows += await oCommon_.GenerateSummaryTableRows(soData);
                                allErrors += string.Join("<br/><hr/><br/>", soErrors);
                                break;
                            }

                        case "DNUPLOAD":
                            {
                                var oDnu = host.Services.GetRequiredService<DNUpload>();
                                var (soData, soErrors) = await oDnu.DNUploadFile("DN");
                                allRows += await oCommon_.GenerateSummaryTableRows(soData);
                                allErrors += string.Join("<br/><hr/><br/>", soErrors);
                                break;
                            }

                        case "PUTAWAY":
                            {
                                var oPut = host.Services.GetRequiredService<PutAwayUpload>();
                                var (soData, soErrors) = await oPut.PutAwayUploadFile("PUTAWAY");
                                allRows += await oCommon_.GenerateSummaryTableRows(soData);
                                allErrors += string.Join("<br/><hr/><br/>", soErrors);
                                break;
                            }

                        case "STOCKTRANSFER":
                            {
                                var oStu = host.Services.GetRequiredService<StockTransferUpload>();
                                var (soData, soErrors) = await oStu.StockTransferUploadFile("STOCKTRANSFER");
                                allRows += await oCommon_.GenerateSummaryTableRows(soData);
                                allErrors += string.Join("<br/><hr/><br/>", soErrors);

                                break;
                            }
                        default:
                            Logger.Warn("Unknown module: {Module}", moduleName);
                            break;
                    }
               

                // Send emails after all modules
                if (!string.IsNullOrEmpty(allRows))
                {
                    string finalEmailHtml = await oCommon_.GenerateSummaryEmailHtml(allRows);
                    string subject = $"EMIRATES-EDI-FTP-at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    await oCommon_.SendFinalMail(subject, finalEmailHtml,"SummaryEmail");
                }
                else
                {                    
                    string noFilesHtml = "<p>No files were found to process for any module.</p>";
                    string subject = $"EMIRATES-EDI-FTP-No-Files-at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    await oCommon_.SendFinalMail(subject, noFilesHtml, "SummaryEmail");
                }

                if (!string.IsNullOrEmpty(allErrors))
                {
                    var errorHtml = await oCommon_.GenerateExceptionHtml("Modules Errors", new Exception(allErrors));
                    await oCommon_.SendFinalMail("Processing Files - Errors", errorHtml, "ExceptionEmail");
                }

                LogManager.Shutdown();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unhandled exception for module {Module}", moduleName);
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}