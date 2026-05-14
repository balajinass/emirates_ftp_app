using emirates_ftp_app.Data;
using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.ASNDao;
using emirates_ftp_app.Model.Inbound.SupplierDao;
using emirates_ftp_app.Repository.CommonFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace emirates_ftp_app.Repository.Inbound.Supplier
{
    internal class SupplierDao : ISupplierDao
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommonFunctions _commonFunctions;
        public SupplierDao(IServiceProvider serviceProvider, ICommonFunctions commonFunctions)
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

        #region GenerateSupplierSlNo
        public async Task<int> GenerateSupplierSlNo()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                await using var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT SUPP_SEQ.nextval FROM DUAL";

                var result = await cmd.ExecuteScalarAsync();                
                MyLogger.GetInstance().Info("Supplier Sno Generated  :" + result);
                return Convert.ToInt32((decimal)result!);
            }
            catch (Exception ex)
            {
               
                MyLogger.GetInstance().Error("Error in Supplier Sno Generated  :" + ex.ToString());
                return 0;
            }
        }
        #endregion

        #region InsertSupplierImport
        public async Task<bool> InsertSupplierImport(List<supplier_csv_model> listofCsv_,wms_edi_ftp_model ediFtp)
        {
            int iRow = 1;
            bool skip = true;

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();
                    // (important for large files)
                    context.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var oItem_ in listofCsv_)
                    {
                        if (skip)
                        {
                            skip = false;
                            continue;
                        }

                        var entity = new wms_el_supplier_import
                        {
                            CREATE_USER = "LIVE",
                            RUN_USER = "LIVE",
                            CREATE_DATE = DateTime.Now,
                            RUN_DATE = DateTime.Now,
                            SOURCE_SL_NO = ediFtp.SL_NO,
                            SL_NO = iRow.ToString(),
                            SUPPLIER_NAME = oItem_?.SUPPLIERNAME,
                            SUPPLIER_CODE = oItem_?.SUPPLIERCODE,
                            SHIP_TO_CODE = oItem_?.SHIPTOCODE,
                            DEFAULT_TRADE_TERMCODE = oItem_?.DEFAULTTRADETERMCODE,
                            DEFAULT_FREIGHT_FORWARDER_CODE = oItem_?.DEFAULTFREIGHTFORWARDERCODE,
                            SUPPLIER_CURRENCY_CODE = oItem_?.SUPPLIERCURRENCYCODE,
                            ADDRESS_LINE1 = oItem_?.ADDRESSLINE1,
                            ADDRESS_LINE2 = oItem_?.ADDRESSLINE2,
                            POST_BOX_NUMBER = oItem_?.POSTBOXNUMBER,
                            POSTAL_CODE = oItem_?.POSTALCODE,

                            CITY_CODE = oItem_?.CITYCODE,
                            OTHER_CITY_NAME = oItem_?.OTHERCITYNAME,
                            STATE_CODE = oItem_?.STATECODE,
                            OTHER_STATE_NAME = oItem_?.OTHERSTATENAME,
                            COUNTRY_CODE = oItem_?.COUNTRYCODE,

                            CONTACT_NAME = oItem_?.CONTACTNAME,
                            INTERNATIONAL_DIALING_CODE = oItem_?.INTERNATIONALDIALINGCODE,
                            AREA_DIALING_CODE = oItem_?.AREADIALINGCODE,
                            PHONE_NUMBER = oItem_?.PHONENUMBER,
                            PHONE_EXTENSION_NUMBER = oItem_?.PHONEEXTENSTIONNUMBER,
                            PHONE_NUMBER2 = oItem_?.PHONENUMBER2,
                            EMAIL = oItem_?.EMAIL,
                            PRIMARY_COMPANY = oItem_?.PRIMARYCOMPANY,
                            TRANSACTION_TIME = DateTime.Now,
                            FILE_STATUS = "NEW",
                            NOTE = ""
                        };

                        await context.WMS_EL_SUPPLIER_IMPORT.AddAsync(entity);

                        iRow++;
                    }
                    
                    await context.SaveChangesAsync();

                    MyLogger.GetInstance().Info("Insert completed in WMS_EL_SUPPLIER_IMPORT");

                    return true;
                    
                }
            }
            catch (Exception ex)
            {               
                MyLogger.GetInstance().Error("Error in InsertSupplierImport  :" + ex.ToString());
                return false;
            }
        }
        #endregion

        #region ExecSUPPImportProcedure
        public async Task<bool> ExecSUPPImportProcedure(pro_client_import_model oProInput_)
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
                    "BEGIN EL_SUPPLIER_IMPORT(:fa_company_code, :fa_branch_code, :fa_location_code, :fa_sl_no); END;",
                    parameters
                );

                MyLogger.GetInstance().Info("Values for Procedure EL_SUPPLIER_IMPORT - " + "  Company Code - " + oProInput_.FA_COMPANY_CODE + "  ,Branch Code -  +" + oProInput_.FA_BRANCH_CODE + "  ,Location Code - " + oProInput_.FA_LOCATION_CODE + "  ,SOURCE_SL_NO - " + oProInput_.FA_SL_NO);

                MyLogger.GetInstance().Info("Procedure Executed Successfully - EL_SUPPLIER_IMPORT");
                
                return true;
            }
            catch (Exception ex)
            {                
                MyLogger.GetInstance().Error("Error in Exec EL_SUPPLIER_IMPORT: " + ex.ToString());
                return false;
            }
        }
        #endregion
    }
}
