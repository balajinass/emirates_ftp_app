using emirates_ftp_app.Model.Nass;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using emirates_ftp_app.Model.Inbound.Ftp;

namespace emirates_ftp_app.Repository.FtpConnection
{
    internal interface IFtpConnection
    {
        Task<List<Input_Request_Model>> GetFilesfromFTP(web_wms_edi_config_model oCustomer_,web_wms_edi_module_config_model oModule_,NetworkCredential credentials);
        Task<bool> DownloadFile(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, Input_Request_Model oFile, NetworkCredential credentials);
        FtpWebRequest CreateFtpWebRequest(string ftpDirectoryPath, NetworkCredential credentials, bool keepAlive = false);
        Task<string> MoveFileToBackupFolder(web_wms_edi_config_model oCustomer_,web_wms_edi_module_config_model oModule_,Input_Request_Model oFile,NetworkCredential credentials);
        //Task<string> SendFileToBackupFolder(web_wms_edi_config_model oCustomer_,web_wms_edi_module_config_model oModule_,Input_Request_Model oFile,NetworkCredential credentials,byte[] dataToSend);
        Task<string> DeleteSourceFile(web_wms_edi_config_model oCustomer_,web_wms_edi_module_config_model oModule_,Input_Request_Model oFiles,NetworkCredential credentials);
        Task<string> MoveFiletoErrorFolder(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, Input_Request_Model oFiles, NetworkCredential credentials);
        Task<string> SendFiletoErrorFolder(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, Input_Request_Model oFiles, NetworkCredential credentials, byte[] datatosend);
        Task<string> DeleteErrorFile(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, Input_Request_Model oFiles, NetworkCredential credentials);

    }
}
