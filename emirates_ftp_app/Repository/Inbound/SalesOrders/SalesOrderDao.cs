using emirates_ftp_app.Data;
using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
using emirates_ftp_app.Model.Nass;
using emirates_ftp_app.Repository.CommonFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
namespace emirates_ftp_app.Repository.Inbound.SalesOrders
{
    internal class SalesOrderDao: ISalesOrderDao
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommonFunctions _commonFunctions;
        public SalesOrderDao(IServiceProvider serviceProvider,ICommonFunctions commonFunctions)
        {
            _serviceProvider = serviceProvider;
            _commonFunctions = commonFunctions;
        }

        #region DB_context_disposed
        //private readonly NassDbContext _nassdbcontext; //Cannot access a disposed object.Object name: 'OracleConnectionInternal'
        //private readonly PrimaryDbContext _primaryDbContext;  //Cannot access a disposed object.Object name: 'OracleConnectionInternal'
        #endregion

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

        #region GenerateSOSlNo
        public async Task<int> GenerateSOSlNo()
        {
            try
            {               
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();
               
                await using var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();
               
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT SALES_SEQ.NEXTVAL FROM DUAL";
                
                var result = await cmd.ExecuteScalarAsync();               
                MyLogger.GetInstance().Info("SOSlNo Generated  :" + result);

                return Convert.ToInt32((decimal)result!);
            }
            catch (Exception ex)
            {               
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

                    MyLogger.GetInstance().Info("Values in Insert WMS_EL_CLIENT_IMPORT - " + entities);

                    MyLogger.GetInstance().Info("Insert completed in WMS_EL_CLIENT_IMPORT");

                    return (iRow > 0 && iRow == listofCsv_.Count);
                }
            }
            catch (Exception ex)
            {               
                MyLogger.GetInstance().Error("Error in WMS_EL_CLIENT_IMPORT;+" + ex.ToString());
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
                var context = scope.ServiceProvider.GetRequiredService<NassDbContext>();

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

                MyLogger.GetInstance().Info("Values in Procedure el_client_import - " + "  Company Code - " + oProInput_.FA_COMPANY_CODE + "  ,Branch Code -  +" + oProInput_.FA_BRANCH_CODE + "  ,Location Code - " + oProInput_.FA_LOCATION_CODE + "  ,SOURCE_SL_NO - " + oProInput_.FA_SL_NO );

                MyLogger.GetInstance().Info("Procedure Excecuted Successfully - el_client_import");
                return true;
            }
            catch (Exception ex)
            {              
                MyLogger.GetInstance().Error("Error in ExecClientImportProcedure  ;" +ex.ToString());
                return false;
            }
        }
        #endregion

        #region InsertSOImport
        public async Task<bool> InsertSOImport(List<salesorder_csv_model> listOfCsv, wms_edi_ftp_model ediFtp)
        {
            try
            {
                // Create a fresh DbContext scope
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                int slNo = 1;
                bool skip = true;
                var entities = new List<wms_el_so_import>();

                foreach (var item in listOfCsv)
                {
                    if (!skip)
                    {
                        var soDate = _commonFunctions.dateFormat(item.SALESORDERDATE!)?.modifiedDate;
                        var expiryDate = _commonFunctions.dateFormat(item.EXPIRYDATE!)?.modifiedDate;
                        var estDate = _commonFunctions.dateFormat(item.ESTTIMEOFDELIVERY!)?.modifiedDate;

                        var entity = new wms_el_so_import
                        {
                            CREATE_USER = "LIVE",
                            CREATE_DATE = DateTime.Now,
                            RUN_USER = "LIVE",
                            RUN_DATE = DateTime.Now,
                            SOURCE_SL_NO = ediFtp.SL_NO,
                            SL_NO = slNo,
                            CUSTOMER_CODE = item?.CUSTOMERCODE,
                            SO_TYPE = item?.SALESORDERTYPE,
                            CUSTOMER_BILL_TO = item?.CUSTOMERBILLTO,
                            CUSTOMER_SHIP_TO = item?.CUSTOMERSHIPTO,
                            SHIPPING_MODE = item?.SHIPPINGMODE,
                            TRADE_TERM = item?.TRADETERM,
                            SO_DATE = soDate,
                            CUSTOMER_ORDER_NO = item?.CUSTOMERORDERNO,
                            CUSTOMER_ORDER_LINE_NO = item?.CUSTOMERORDERLINENO?.Substring(0, Math.Min(10, item.CUSTOMERORDERLINENO.Length)),
                            PART_CODE = item?.PARTCODE,
                            ALLOCATED_QTY = !string.IsNullOrWhiteSpace(item?.ALLOCATEDQTY?.Trim())
                                && decimal.TryParse(item.ALLOCATEDQTY.Trim(), out var decQty)
                                ? (int?)Decimal.ToInt32(decQty)
                                : null,
                            PART_BATCH_NO = item?.PARTBATCHNO,
                            SERIAL_NO = item?.SERIALNO,
                            EXPIRY_DATE = expiryDate,
                            ADDITIONAL_INFORMATION = item?.ADDITIONALINFORMATION,
                            EST_TIME_OF_DEL = estDate,
                            PRIMARY_COMPANY = item?.PRIMARYCOMPANY,
                            WAREHOUSE = item?.WAREHOUSE,
                            COST_BUCKET = "",
                            PAR_TYPE_CODE = "",
                            SO_ID = "",
                            TRANSACTION_TIME = DateTime.Now,
                            FILE_STATUS = "NEW",
                            NOTE = "",
                            DESTINATION_COMPANY = item?.DESTINATIONCOMPANY,
                            COLOR = item?.COLOR,
                            ITEM_SIZE = item?.SIZE,
                            PAYMENT = item?.PAYMENT,
                            DIMENTION = item?.DIMENTION,
                            VALUE = item?.VALUE,
                            COUNTRY_CODE = item?.COUNTRYCODE,
                            CUST_CURRENCY_CODE = item?.CUSTCURRENCYCODE
                        };

                        entities.Add(entity);
                        slNo++;
                    }
                    skip = false;
                }

                if (entities.Any())
                {
                    await context.WMS_EL_SO_IMPORT.AddRangeAsync(entities);
                    await context.SaveChangesAsync();
                }

                MyLogger.GetInstance().Info("Values in Insert WMS_EL_SO_IMPORT - " + entities);

                MyLogger.GetInstance().Info("Insert completed in WMS_EL_SO_IMPORT");

                return true;
            }
            catch (Exception ex)
            {  
                MyLogger.GetInstance().Error("Error in WMS_EL_SO_IMPORT ;" + ex.ToString());
                return false;
            }
        }
        #endregion

        #region ExecSOImportProcedure
        public async Task<bool> ExecSOImportProcedure(pro_client_import_model oProInput_)
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

                // Execute the stored procedure
                await context.Database.ExecuteSqlRawAsync(
                    "BEGIN EL_SO_IMPORT(:fa_company_code, :fa_branch_code, :fa_location_code, :fa_sl_no); END;",
                    parameters
                );

                MyLogger.GetInstance().Info("Values in Procedure EL_SO_IMPORT - " + "  Company Code - " + oProInput_.FA_COMPANY_CODE + "  ,Branch Code -  +" + oProInput_.FA_BRANCH_CODE + "  ,Location Code - " + oProInput_.FA_LOCATION_CODE + "  ,SOURCE_SL_NO - " + oProInput_.FA_SL_NO
                 );

                MyLogger.GetInstance().Info("Procedure Excecuted Successfully - EL_SO_IMPORT");

                return true;
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error in ExecSOImportProcedure  ;" + ex.ToString());
                return false;
            }
        }
        #endregion
    }
}
