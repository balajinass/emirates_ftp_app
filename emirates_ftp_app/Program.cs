using emirates_ftp_app.Data;
using emirates_ftp_app.Middleware.Inbound;
using emirates_ftp_app.Model.Common;
using emirates_ftp_app.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;

namespace emirates_ftp_app
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string moduleName = string.Empty;
            args = new[] { "SO" }; // dev-only test input
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
                        var primaryCs = context.Configuration.GetConnectionString("PrimaryConnection");
                        var nassCs = context.Configuration.GetConnectionString("NassConnection");

                        Console.WriteLine("PrimaryConnection = " + (string.IsNullOrWhiteSpace(primaryCs) ? "NULL" : "FOUND"));
                        Console.WriteLine("NassConnection    = " + (string.IsNullOrWhiteSpace(nassCs) ? "NULL" : "FOUND"));

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

                        services.AddScoped<SalesOrder>();
                        
                        services.AddScoped<nassRepository>();
                        
                    })
                    .Build();

                switch (moduleName)
                {
                    case "SO":
                        var oSku = host.Services.GetRequiredService<SalesOrder>();
                        await oSku.SoCreation("SO");
                        break;
                }
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