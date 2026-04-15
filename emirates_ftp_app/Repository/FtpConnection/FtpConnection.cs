using emirates_ftp_app.Log;
using emirates_ftp_app.Model.Nass;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using emirates_ftp_app.Model.Ftp;

namespace emirates_ftp_app.Repository.FtpConnection
{
    //FTPCLient based
    internal class FtpConnection 
    {

        #region Old_FTP_based_File_extract
        //   public async Task<List<input_request_model>> GetFilesfromFTP(
        //web_wms_edi_config_model oCustomer_,
        //web_wms_edi_module_config_model oModule_,
        //NetworkCredential credentials)
        //   {
        //       List<input_request_model> listInput = new List<input_request_model>();
        //       List<string> lines = new List<string>();
        //       try
        //       {
        //           string actualURL = oCustomer_.FTP_URL + oModule_.FTP_FILE_PATH + "/";
        //           FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(actualURL);
        //           listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
        //           listRequest.Credentials = credentials;
        //           listRequest.KeepAlive = true;
        //           listRequest.UsePassive = true;

        //           List<string> filterlines = new List<string>();
        //           MyLogger.GetInstance().Info("Connecting FTP...");
        //           FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
        //           {
        //               MyLogger.GetInstance().Info("FTP Connected");
        //               ///
        //               using (Stream listStream = listResponse.GetResponseStream())
        //               {
        //                   using (StreamReader listReader = new StreamReader(listStream))
        //                   {
        //                       MyLogger.GetInstance().Info("Reading " + oModule_.FTP_FILE_PATH + " folder");
        //                       while (!listReader.EndOfStream)
        //                       {
        //                           filterlines.Add(listReader.ReadLine());
        //                       }
        //                   }
        //               }
        //           }

        //           if (filterlines != null)
        //           {
        //               MyLogger.GetInstance().Info("Total files availabe in FTP  : " + filterlines.Count);
        //               foreach (string line in filterlines)
        //               {
        //                   //MyLogger.GetInstance().Info(line);
        //                   input_request_model oInput = new input_request_model();
        //                   oInput.lineItem = line;
        //                   var month = string.Empty;
        //                   var date = string.Empty;
        //                   var time = string.Empty;
        //                   string[] tokens = line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
        //                   if (tokens[8] != null && tokens[8].ToLower().Contains(".csv"))
        //                   {
        //                       if (tokens[5] != "")
        //                       {
        //                           month = tokens[5];
        //                       }
        //                       if (tokens[6] != "")
        //                       {
        //                           date = tokens[6];
        //                       }
        //                       if (tokens[5] != "" && tokens[6] != "")
        //                       {
        //                           if (tokens[7] != "")
        //                           {
        //                               time = tokens[7];
        //                           }
        //                           if (DateTime.Now.Month == 12 && DateTime.Now.Date.ToString("dd") == "31")
        //                           {
        //                               FtpWebRequest timerequest = (FtpWebRequest)WebRequest.Create(actualURL + tokens[8]);
        //                               timerequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
        //                               timerequest.Credentials = credentials;
        //                               timerequest.KeepAlive = true;
        //                               timerequest.UsePassive = true;
        //                               FtpWebResponse timeresponse = (FtpWebResponse)timerequest.GetResponse();
        //                               oInput.dateTime = Convert.ToString(timeresponse.LastModified);
        //                               //MyLogger.GetInstance().Info("File Name : " + tokens[8] + " | Modified On : " + oInput.dateTime + "");
        //                           }
        //                           else
        //                           {
        //                               oInput.dateTime = date + "-" + month + "-" + DateTime.Now.Year.ToString() + " " + time;
        //                               //MyLogger.GetInstance().Info("File Name : " + tokens[8] + " | Modified On : " + oInput.dateTime + "");
        //                           }
        //                       }
        //                       if (tokens[8] != null)
        //                       {
        //                           oInput.fileName = tokens[8];
        //                           oInput.fileExtension = tokens[8].ToString().Split('.')[1];
        //                           Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
        //                           Match result = re.Match(oInput.fileName);
        //                           if (result != null && result.Groups != null && result.Groups.Count > 0)
        //                           {
        //                               if (result.Groups[1].Value != null && result.Groups[1].Value != "")
        //                               {
        //                                   oInput.typeName = result.Groups[1].Value;
        //                               }
        //                               if (result.Groups[2].Value != null && result.Groups[2].Value != "")
        //                               {
        //                                   oInput.typeDigit = result.Groups[2].Value;
        //                               }
        //                           }
        //                       }
        //                       listInput.Add(oInput);
        //                   }
        //                   else
        //                   {
        //                       MyLogger.GetInstance().Warning("This file is not executable :" + line);
        //                   }
        //               }
        //           }
        //       }
        //       catch (Exception ex)
        //       {
        //           MyLogger.GetInstance().Error("Exception in GetFilesfromFTP - " + ex.Message);
        //       }
        //       return listInput;
        //   }
        #endregion

        #region GetFilesfromFTP
        public async Task<List<input_request_model>> GetFilesfromFTP(web_wms_edi_config_model oCustomer_,
            web_wms_edi_module_config_model oModule_,NetworkCredential credentials,string module)
        {
            List<input_request_model> listInput = new List<input_request_model>();

            try
            {
                string host = oCustomer_.FTP_URL!.Replace("ftp://", "").TrimEnd('/');
                string path = oModule_.FTP_FILE_PATH!;
                string prefix = module;

                Console.WriteLine("Fetching filtered file list...");
               
                var fileNames = await ListCsvFilesPrefixAsync(host, path, credentials, prefix);

                Console.WriteLine("Total "+ module +" files : " + fileNames.Count);

                using var client = new FtpClient(host, credentials.UserName, credentials.Password);
                client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
                client.Connect();

                foreach (var fileName in fileNames)
                {
                    DateTime modifiedTime = client.GetModifiedTime(Path.Combine(path, fileName));
                    input_request_model oInput = new input_request_model
                    {
                        fileName = fileName,                        
                        dateTime = modifiedTime.ToString("dd-MMM-yyyy HH:mm:ss"),

                        //fileExtension- not need////
                        //fileExtension = Path.GetExtension(fileName).Replace(".", ""),
                    };

                    #region regex
                    //Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                    //Match result = re.Match(oInput.fileName);

                    //if (result.Success)
                    //{
                    //    oInput.typeName = result.Groups[1].Value;
                    //    oInput.typeDigit = result.Groups[2].Value;//Non Mandatory
                    //}
                    #endregion

                    listInput.Add(oInput);
                }
                if(fileNames.Count == 0)
                {
                    Console.WriteLine("No files found in FTP Folder - " + path);
                    MyLogger.GetInstance().Info("No files found in FTP Folder - " + path);
                }
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Exception in GetFilesfromFTP - " + ex.Message);
                Console.ForegroundColor = previousColor;
                
                MyLogger.GetInstance().Error("Exception in GetFilesfromFTP - " + ex.Message);
            }

            return listInput;
        }
        #endregion

        #region ListCsvFilesPrefixAsync
        public async Task<List<string>> ListCsvFilesPrefixAsync(string host,string path,
            NetworkCredential credentials,string filePrefix)
        {
            return await Task.Run(() =>
            {
                using var client = new FtpClient(host, credentials.UserName, credentials.Password);

                client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
                client.Connect();

                var files = client.GetListing(path)
                    .Where(f => f.Type == FtpObjectType.File &&
                                f.Name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) &&
                                f.Name.StartsWith(filePrefix, StringComparison.OrdinalIgnoreCase))
                    .Select(f => f.Name)
                    .ToList();

                client.Disconnect();

                return files;
            });
        }
        #endregion

        #region DownloadFile
        public async Task<bool> DownloadFile(web_wms_edi_config_model oCustomer_,web_wms_edi_module_config_model oModule_,input_request_model oFile,NetworkCredential credentials)
        {
            try
            {
                string sFtpFullPath = oCustomer_.FTP_URL + oModule_.FTP_FILE_PATH + "/" + oFile.fileName;
                string sLocalFullPath = oModule_.LOCAL_FILE_PATH + oFile.fileName;

                var request = CreateFtpWebRequest(sFtpFullPath, credentials, true);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                byte[] buffer = new byte[2048];
                int bytesRead;

                using (var response = (FtpWebResponse)await request.GetResponseAsync())
                using (var stream = response.GetResponseStream())
                using (var fileStream = new FileStream(sLocalFullPath, FileMode.Create, FileAccess.Write))
                {
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }

                MyLogger.GetInstance().Info("File Download Completed");
                return true;
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Exception in DownloadFile - " + ex.Message);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error("Exception in DownloadFile - " + ex.Message);
                return false;
            }
        }
        #endregion

        #region CreateFtpWebRequest
        public FtpWebRequest CreateFtpWebRequest(string ftpDirectoryPath, NetworkCredential credentials, bool keepAlive = false)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftpDirectoryPath));
            try
            {
                //Set proxy to null. Under current configuration if this option is not set then the proxy that is used will get an html response from the web content gateway (firewall monitoring system)
                request.Proxy = null;

                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = keepAlive;

                request.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Exception while MoveFiletoBackupFolder - " + ex.Message);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error("Exception while MoveFiletoBackupFolder - " + ex.Message);
                return request;
            }
            return request;
        }
        #endregion

        #region MoveFileToBackupFolder
        public async Task<string> MoveFileToBackupFolder(
            web_wms_edi_config_model oCustomer_,
            web_wms_edi_module_config_model oModule_,
            input_request_model oFile,
            NetworkCredential credentials)
        {
            string retval = string.Empty;
            try
            {
                string sLocalFullPath = string.Concat(oModule_.LOCAL_FILE_PATH + oFile.fileName);

                byte[] filecontents = File.ReadAllBytes(sLocalFullPath);
                retval += await SendFileToBackupFolder(oCustomer_, oModule_, oFile, credentials, filecontents);
                retval += DeleteSourceFile(oCustomer_, oModule_, oFile, credentials);
                return retval;
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Exception while MoveFiletoBackupFolder - " + ex.Message);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error("Exception while MoveFiletoBackupFolder - " + ex.Message);
                return "ERROR~" + ex.Message;
            }
        }
        #endregion

        #region SendFileToBackupFolder
        public static async Task<string> SendFileToBackupFolder(
            web_wms_edi_config_model oCustomer_,
            web_wms_edi_module_config_model oModule_,
            input_request_model oFile,
            NetworkCredential credentials,
            byte[] dataToSend)
        {
            FtpWebResponse ftpresponse;
            try
            {
                if (oModule_.FTP_FILE_BACKUP_PATH!.Substring(oModule_.FTP_FILE_BACKUP_PATH.Length - 1) != "/")
                {
                    oModule_.FTP_FILE_BACKUP_PATH += "/";
                }
                string sFtpBackupFullPath = string.Concat(oCustomer_.FTP_URL + oModule_.FTP_FILE_BACKUP_PATH + oFile.fileName);
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(sFtpBackupFullPath);
                ftp.Method = WebRequestMethods.Ftp.UploadFile;
                ftp.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
                ftp.UsePassive = true;
                ftp.KeepAlive = true;

                ftp.ContentLength = dataToSend.Length;
                Stream requestStream = ftp.GetRequestStream();
                requestStream.Write(dataToSend, 0, dataToSend.Length);
                requestStream.Close();

                ftpresponse = (FtpWebResponse)ftp.GetResponse();

            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Exception while SendFiletoBackupFolder - " + ex.Message);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error("Exception while SendFiletoBackupFolder - " + ex.Message);
                return "ERROR~" + ex.Message;
            }

            return ftpresponse.StatusDescription!;
        }
        #endregion

        #region DeleteSourceFile
        public async Task<string> DeleteSourceFile(
            web_wms_edi_config_model oCustomer_,
            web_wms_edi_module_config_model oModule_,
            input_request_model oFiles,
            NetworkCredential credentials)
        {

            try
            {
                if (oModule_.FTP_FILE_PATH.Substring(oModule_.FTP_FILE_PATH.Length - 1) != "/")
                {
                    oModule_.FTP_FILE_PATH += "/";
                }
                string sFtpDeleteFullPath = string.Concat(oCustomer_.FTP_URL + oModule_.FTP_FILE_PATH + oFiles.fileName);
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(sFtpDeleteFullPath);
                ftp.Method = WebRequestMethods.Ftp.DeleteFile;
                ftp.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
                ftp.UsePassive = true;
                ftp.KeepAlive = true;

                FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();

                Stream responseStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(responseStream);

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Exception while DeleteSourceFile - " + ex.Message);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error("Exception while DeleteSourceFile - " + ex.Message);
                return "ERROR~" + ex.Message;
            }

        }
        #endregion

        #region MoveFiletoErrorFolder
        public async Task<string> MoveFiletoErrorFolder(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles, NetworkCredential credentials)
        {
            string retval = string.Empty;
            try
            {
                string sLocalFullPath = string.Concat(oModule_.LOCAL_FILE_PATH + oFiles.fileName);

                byte[] filecontents = File.ReadAllBytes(sLocalFullPath);
                retval += await SendFileToBackupFolder(oCustomer_, oModule_, oFiles, credentials, filecontents);
                retval += DeleteSourceFile(oCustomer_, oModule_, oFiles, credentials);
                return retval;
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Exception while MoveFiletoErrorFolder - " + ex.Message);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error("Exception while MoveFiletoErrorFolder - " + ex.Message);
                return "ERROR~" + ex.Message;
            }
        }
        #endregion

        #region SendFiletoErrorFolder
        public static async Task<string> SendFiletoErrorFolder(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles, NetworkCredential credentials, byte[] datatosend)
        {
            FtpWebResponse ftpresponse;
            try
            {
                if (oModule_.FTP_FILE_BACKUP_PATH!.Substring(oModule_.FTP_FILE_BACKUP_PATH.Length - 1) != "/")
                {
                    oModule_.FTP_FILE_BACKUP_PATH += "/";
                }
                string sFtpBackupFullPath = string.Concat(oCustomer_.FTP_URL + oModule_.FTP_FILE_BACKUP_PATH + oFiles.fileName);
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(sFtpBackupFullPath);
                ftp.Method = WebRequestMethods.Ftp.UploadFile;
                ftp.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
                ftp.UsePassive = true;
                ftp.KeepAlive = true;

                ftp.ContentLength = datatosend.Length;
                Stream requestStream = ftp.GetRequestStream();
                requestStream.Write(datatosend, 0, datatosend.Length);
                requestStream.Close();

                ftpresponse = (FtpWebResponse)ftp.GetResponse();

            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Exception while SendFiletoErrorFolder - " + ex.Message);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error("Exception while SendFiletoErrorFolder - " + ex.Message);
                return "ERROR~" + ex.Message;
            }

            return ftpresponse.StatusDescription!;
        }
        #endregion

        #region DeleteErrorFile
        public async Task<string> DeleteErrorFile(web_wms_edi_config_model oCustomer_, web_wms_edi_module_config_model oModule_, input_request_model oFiles, NetworkCredential credentials)
        {
            try
            {
                string sFtpErrorFullPath = string.Concat(oCustomer_.FTP_URL + oModule_.FTP_FILE_ERROR_PATH + oFiles.fileName);
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(sFtpErrorFullPath);
                ftp.Method = WebRequestMethods.Ftp.DeleteFile;
                ftp.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
                ftp.UsePassive = true;
                ftp.KeepAlive = true;

                FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();

                Stream responseStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(responseStream);

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Exception while DeleteErrorFile - " + ex.Message);
                Console.ForegroundColor = previousColor;

                MyLogger.GetInstance().Error("Exception while DeleteErrorFile - " + ex.Message);
                return "ERROR~" + ex.Message;
            }

        }
        #endregion

        #region FileContentMoveFtp
        public async Task<bool> FileContentMoveFtp(bool success,string fileName, string fileContent,web_wms_edi_config_model oCustomer_,web_wms_edi_outbound_config oModule_,NetworkCredential credentials)
    {
        string localFullPath = Path.Combine(oModule_.LOCAL_FILE_PATH!, fileName);
       
        string host = oCustomer_.FTP_URL!.Replace("ftp://", "").TrimEnd('/');
        string remotePath = Path.Combine(oModule_.FTP_FILE_PATH!, fileName).Replace("\\", "/");

        try
        {
            await File.WriteAllTextAsync(localFullPath, fileContent);
           
            using var ftpClient = new FtpClient(host, credentials.UserName, credentials.Password);
            ftpClient.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
            ftpClient.Connect();
            
            FtpStatus status = ftpClient.UploadFile(localFullPath, remotePath, FtpRemoteExists.Overwrite, true);

            if (status != FtpStatus.Success)
            {
                Console.WriteLine($"Failed to upload file: {fileName}");
                return false;
            }

            Console.WriteLine($"File uploaded successfully: {fileName}");

            if (File.Exists(localFullPath))
            {
                File.Delete(localFullPath);
            }

               success = true;
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Error in FileContentMoveFtp:- " + ex.Message);
                Console.ForegroundColor = previousColor;

                Console.WriteLine("Error in FileContentMoveFtp: " + ex.Message);
                MyLogger.GetInstance().Error("Error in FileContentMoveFtp: " + ex.Message);
                success = false;
            }

           return success;
        }
        #endregion
    }
}
