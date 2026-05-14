using emirates_ftp_app.Data;
using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound;
using emirates_ftp_app.Model.Inbound.ASNDao;
using emirates_ftp_app.Repository.CommonFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace emirates_ftp_app.Repository.Inbound.Asn
{
    internal class ASNDao : IASNDao
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommonFunctions _commonFunctions;
        public ASNDao(IServiceProvider serviceProvider,ICommonFunctions commonFunctions)
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

        #region GenerateASNSlNo
        public async Task<int> GenerateASNSlNo()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                await using var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT ASN_SEQ.NEXTVAL SL_NO FROM DUAL";

                var result = await cmd.ExecuteScalarAsync();               
                MyLogger.GetInstance().Info("ASNSl Generated  :" + result);

                return Convert.ToInt32((decimal)result!);
            }
            catch (Exception ex)
            {                
                MyLogger.GetInstance().Error("Error in GenerateASNSlNo  :" + ex.ToString());
                return 0;
            }
        }
        #endregion

        #region InsertASNImport
        public async Task<bool> InsertASNImport(List<asn_csv_model> listofCsv_,wms_edi_ftp_model ediFtp)
        {
            bool bInsert = false;

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PrimaryDbContext>();

                    int iRow = 1;
                    bool skip = true;

                    foreach (var oItem_ in listofCsv_)
                    {
                        if (skip)
                        {
                            skip = false;
                            continue;
                        }

                        MyLogger.GetInstance().Info("RowNo: " + iRow);

                        var oSupplierInvoiceDate = _commonFunctions.dateFormat(oItem_.SUPPLIERINVOICEDATE!);
                        var oExpiryDate = _commonFunctions.dateFormat(oItem_.EXPIRYDATE!);
                        var oManufacturingDate = _commonFunctions.dateFormat(oItem_.MANUFACTURINGDATE!);
                        var oEstimatedTimeOfArrival = _commonFunctions.dateFormat(oItem_.ESTIMATEDTIMEOFARRIVAL!);

                        var entity = new wms_el_po_import
                        {
                            CREATE_USER = "LIVE",
                            RUN_USER = "LIVE",
                            CREATE_DATE = DateTime.Now,
                            RUN_DATE = DateTime.Now,
                            SOURCE_SL_NO = ediFtp.SL_NO,
                            SL_NO = iRow,
                            PRIMARY_COMPANY = oItem_?.PRIMARYCOMPANY,
                            WAREHOUSE = oItem_?.WAREHOUSE,
                            COST_BUCKET = "",
                            SHIPPED_PART_CODE = oItem_?.SHIPPEDPARTCODE,
                            SUPPLIER_CODE = oItem_?.SUPPLIERCODE,
                            SHIPPING_MODE_CODE = oItem_?.SHIPPINGMODECODE,
                            PO_TYPE_CODE = oItem_?.PURCHASEORDERTYPECODE,
                            PART_TYPE_CODE = oItem_?.PARTTYPECODE,
                            PO_NUMBER = oItem_?.PURCHASEORDERNUMBER,
                            PO_LINE_NUMBER = oItem_?.PURCHASEORDERLINENUMBER,
                            BILLING_UOM_CODE = oItem_?.BILLINGUOMCODE,
                            SUPPLIER_INVOICE_NO = oItem_?.SUPPLIERINVOICENO,
                            SUPPLIER_INVOICE_DATE = oSupplierInvoiceDate?.modifiedDate,
                            SHIPPED_QUAN_BILL_UOM = oItem_?.SHIPPEDQUANTITYINBILLINGUOM,
                            EXPIRY_DATE = oExpiryDate?.modifiedDate,
                            MANUFACTURING_DATE = oManufacturingDate?.modifiedDate,
                            SERIAL_NO = oItem_?.SERIALNO,    
                            BATCH_NO = oItem_?.BATCHNO,
                            ESTIMATED_TIME_OF_ARRIVAL = oEstimatedTimeOfArrival?.modifiedDate,
                            BILL_OF_LADING = oItem_?.BILLOFLADINGORAIRWAYBILLNO,
                            FILE_STATUS = "NEW",
                            COLOR = oItem_?.COLOR,   
                            ITEM_SIZE = oItem_?.SIZE
                        };

                        await context.WMS_EL_PO_IMPORT.AddAsync(entity);
                        iRow++;
                    }                    
                    int result = await context.SaveChangesAsync();                   

                    MyLogger.GetInstance().Info($"Insert Completed in WMS_EL_PO_IMPORT");
                    bInsert = result > 0;
                }
            }
            catch (Exception ex)
            {  
                MyLogger.GetInstance().Error("Error in WMS_EL_PO_IMPORT  :" + ex.Message);
                bInsert = false;
            }

            return bInsert;
        }
        #endregion

        #region ExecASNImportProcedure
        public async Task<bool> ExecASNImportProcedure(pro_client_import_model oProInput_)
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
                    "BEGIN EL_PO_IMPORT(:fa_company_code, :fa_branch_code, :fa_location_code, :fa_sl_no); END;",
                    parameters
                );

                MyLogger.GetInstance().Info("Values in Procedure EL_PO_IMPORT - " + "  Company Code - " + oProInput_.FA_COMPANY_CODE + "  ,Branch Code -  +" + oProInput_.FA_BRANCH_CODE + "  ,Location Code - " + oProInput_.FA_LOCATION_CODE + "  ,SOURCE_SL_NO - " + oProInput_.FA_SL_NO);

                MyLogger.GetInstance().Info($"Procedure Executed Successfully - EL_PO_IMPORT");
                return true;
            }
            catch (Exception ex)
            {  
                MyLogger.GetInstance().Error("Error in Exec EL_PO_IMPORT: " + ex.ToString());
                return false;
            }
        }
        #endregion
    }
}
