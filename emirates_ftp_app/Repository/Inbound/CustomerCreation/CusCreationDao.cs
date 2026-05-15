using emirates_ftp_app.Data;
using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
using emirates_ftp_app.Repository.CommonFunctions;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace emirates_ftp_app.Repository.Inbound.CustomerCreation
{
    internal class CusCreationDao : ICusCreationDao
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommonFunctions _commonFunctions;
        public CusCreationDao(IServiceProvider serviceProvider, ICommonFunctions commonFunctions)
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

        #region GenerateCusCreationSlNo
        public async Task<int> GenerateCusCreationSlNo()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                await using var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "select CUSTOMER_SEQ.nextval from DUAL";

                var result = await cmd.ExecuteScalarAsync();
                Console.WriteLine("CustomerCreationSlNo Generated  :" + result);
                return Convert.ToInt32((decimal)result!);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error in GenerateCusCreationSlNo  :" + ex);
                MyLogger.GetInstance().Error(ex.ToString());
                return 0;
            }
        }
        #endregion

        #region InsertClientImport
        public async Task<bool> InsertClientImport(List<salesorder_csv_model> listofCsv_, wms_edi_ftp_model ediFtp)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                    int iRow = 1;
                    bool skip = true;
                    var entities = new List<wms_el_client_import>();

                    foreach (var oItem_ in listofCsv_)
                    {
                        if (!skip)
                        {
                            var entity = new wms_el_client_import
                            {
                                CREATE_USER = "LIVE",
                                CREATE_DATE = DateTime.Now,
                                RUN_USER = "LIVE",
                                RUN_DATE = DateTime.Now,
                                SOURCE_SL_NO = ediFtp.SL_NO,
                                SL_NO = iRow,
                                CUSTOMER_CODE = oItem_?.CUSTOMERCODE,
                                CUSTOMER_NAME = oItem_?.CUSTOMERNAME,
                                PRIMARY_COMPANY = oItem_?.PRIMARYCOMPANY,
                                WAREHOUSE = oItem_?.WAREHOUSE,
                                CUSTOMER_TYPE_CODE = oItem_?.CUSTOMERTYPECODE,
                                CUST_CURRENCY_CODE = oItem_?.CUSTCURRENCYCODE,
                                COUNTRY_CODE = oItem_?.COUNTRYCODE,
                                STATE_CODE = oItem_?.STATECODE,
                                OTHER_STATE_NAME = oItem_?.OTHERSTATENAME,
                                CITY_CODE = oItem_?.CITYCODE,
                                OTHER_CITY_NAME = oItem_?.OTHERCITYNAME,
                                ADDRESS_LINE1 = oItem_?.ADDRESSLINE1,
                                ADDRESS_LINE2 = oItem_?.ADDRESSLINE2,
                                POST_BOX_NUMBER = oItem_?.POSTBOXNUMBER,
                                POSTAL_CODE = oItem_?.POSTALCODE,
                                CONTACT_NAME = oItem_?.CONTACTNAME,
                                INTERANTIONAL_DIALING_CODE = oItem_?.INTERNATIONALDIALINGCODE,
                                AREA_DIALING_CODE = oItem_?.AREADIALINGCODE,
                                PHONE_NUMBER = oItem_?.PHONENUMBER,
                                PHONE_EXTENSION_NUMBER = oItem_?.PHONEEXTENSIONNUMBER,
                                MOBILE_NUMBER = oItem_?.MOBILENUMBER,
                                FAX_NUMBER = oItem_?.FAXNUMBER,
                                EMAIL = oItem_?.EMAIL,
                                FILE_STATUS = "NEW",
                                NOTE = ""
                            };

                            entities.Add(entity);
                            iRow++;
                        }

                        skip = false;
                    }

                    if (entities.Any())
                    {
                        await context.WMS_EL_CLIENT_IMPORT.AddRangeAsync(entities);
                        await context.SaveChangesAsync();
                    }
                    Console.WriteLine("Insert completed in WMS_EL_CLIENT_IMPORT");
                    return (iRow > 0 && iRow == listofCsv_.Count);
                }
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("Error in InsertClientImport  ;+" + ex);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error(ex.ToString());
                return false;
            }
        }
        #endregion

        #region ExecClientImportProcedure
        public async Task<bool> ExecClientImportProcedure(pro_client_import_model oProInput_)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                var parameters = new[]
                {
            new OracleParameter("fa_company_code", oProInput_.FA_COMPANY_CODE ?? (object)DBNull.Value),
            new OracleParameter("fa_branch_code", oProInput_.FA_BRANCH_CODE ?? (object)DBNull.Value),
            new OracleParameter("fa_location_code", oProInput_.FA_LOCATION_CODE ?? (object)DBNull.Value),
            new OracleParameter("fa_sl_no", oProInput_.FA_SL_NO)
                };

                // Execute the stored procedure
                await context.Database.ExecuteSqlRawAsync(
                    "BEGIN el_client_import(:fa_company_code, :fa_branch_code, :fa_location_code, :fa_sl_no); END;",
                    parameters
                );
                Console.WriteLine("Procedure Excecuted Successfully - el_client_import");
                return true;
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("Error in ExecClientImportProcedure  ;+" + ex);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error(ex.ToString());
                return false;
            }
        }
        #endregion
    }
}
