using emirates_ftp_app.Data;
using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound.SupplierDao;
using emirates_ftp_app.Repository.CommonFunctions;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using emirates_ftp_app.Model.Inbound.SoCancelDao;
using emirates_ftp_app.Model.Inbound;

namespace emirates_ftp_app.Repository.Inbound.SoCancel
{
    internal class SoCancelDao : ISoCancelDao
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommonFunctions _commonFunctions;
        public SoCancelDao(IServiceProvider serviceProvider, ICommonFunctions commonFunctions)
        {
            _serviceProvider = serviceProvider;
            _commonFunctions = commonFunctions;
        }

        #region CRUD Methods - Pending Implementation
        public bool Delete(wms_edi_ftp_model t)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAsLOVSTATUS(wms_edi_ftp_model t)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<wms_edi_ftp_model> GetAll()
        {
            throw new NotImplementedException();
        }

        public wms_edi_ftp_model GetbyID(string USER_ID)
        {
            throw new NotImplementedException();
        }

        public bool Update(wms_edi_ftp_model t)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GenerateSoCancelSlNo
        public async Task<int> GenerateSoCancelSlNo()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<NassDbContext>();

                await using var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT SO_CANCEL_SEQ.nextval FROM DUAL";

                var result = await cmd.ExecuteScalarAsync();
                Console.WriteLine("SoCancel Sno Generated");
                return Convert.ToInt32((decimal)result!);
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Error in GenerateSoCancelSlNo- " + ex.Message);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error(ex.ToString());
                return 0;
            }
        }
        #endregion

        #region InsertSoCancelImport
        public async Task<bool> InsertSoCancelImport(List<so_cancel_csv_model> listofCsv_,wms_edi_ftp_model ediFtp)
        {
            int iRow = 1;
            bool skip = true;

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                    //for large data
                    context.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var oItem_ in listofCsv_)
                    { 
                        if (skip)
                        {
                            skip = false;
                            continue;
                        }

                        MyLogger.GetInstance().Info("RowNo: " + iRow);

                        var entity = new wms_el_so_cancel_import
                        {
                            CREATE_USER = "LIVE",
                            RUN_USER = "LIVE",
                            CREATE_DATE = DateTime.Now,
                            RUN_DATE = DateTime.Now,
                            SOURCE_SL_NO = ediFtp.SL_NO,
                            SL_NO = iRow.ToString(),
                            PRIMARY_COMPANY = oItem_?.PRIMARYCOMPANY,
                            COST_BUCKET = oItem_?.COSTBUCKET,
                            SALES_ORDER_NO = oItem_?.SALESORDERNO,
                            SALES_ORDER_LINE_NO = oItem_?.SALESORDERLINENO,
                            EXTERNAL_SO_NO = oItem_?.EXTERNALSONO,
                            ORDER_LINE_NO = oItem_?.ORDERLINENO,
                            ITEM_REF = oItem_?.ITEMREF,
                            CANCELLATION_QUANTITY = oItem_?.CANCELLATIONQUANTITY,
                            SHIP_TO_CODE = oItem_?.SHIPTOCODE,                            
                            TRANSACTION_TIME = _commonFunctions.dateFormat(oItem_?.TRANSACTIONTIME!)?.modifiedDate,
                            FILE_STATUS = "NEW",
                            NOTE = oItem_?.NOTE
                        };

                        await context.WMS_EL_SO_CANCEL_IMPORT.AddAsync(entity);

                        iRow++;
                    }
                  
                    await context.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("Error in InsertSoCancelImport: " + ex);
                Console.ForegroundColor = previousColor;
                
                MyLogger.GetInstance().Error(ex.ToString());
                return false;
            }
        }
        #endregion

        #region ExecSoCancelImportProcedure
        public async Task<bool> ExecSoCancelImportProcedure(pro_client_import_model oProInput_)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<NassDbContext>();

                var parameters = new[]
                        {
                    new OracleParameter("fa_company_code", oProInput_.FA_COMPANY_CODE ?? (object)DBNull.Value),
                    new OracleParameter("fa_branch_code", oProInput_.FA_BRANCH_CODE ?? (object)DBNull.Value),
                    new OracleParameter("fa_location_code", oProInput_.FA_LOCATION_CODE ?? (object)DBNull.Value),
                    new OracleParameter("fa_sl_no", oProInput_.FA_SL_NO)
                };

                await context.Database.ExecuteSqlRawAsync(
                    "BEGIN EL_SO_CANCEL_IMPORT(:fa_company_code, :fa_branch_code, :fa_location_code, :fa_sl_no); END;",
                    parameters
                );

                MyLogger.GetInstance().Info("Procedure Executed Successfully - EL_SO_CANCEL_IMPORT");
                return true;
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("Error in ExecSoCancelImportProcedure: " + ex);
                Console.ForegroundColor = previousColor;
               
                MyLogger.GetInstance().Error(ex.ToString());
                return false;
            }
        }
        #endregion
    }
}
