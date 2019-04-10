using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace ServiceCenter.Client.WinService.ImageDataTransfer.Transfer
{
    class FTPManager
    {
        //string cftpServerIP;
        string cftpRemotePath;
        FtpWebRequest reqFTPConnect;      //FTP连接

        // 缓冲大小设置为2kb    
        int buffLength = 2048;

        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <param name="FtpServerIP">FTP连接地址</param>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="FtpUserID">用户名</param>
        /// <param name="FtpPassword">密码</param>
        public FTPManager(string FtpRemotePath, string FtpUserID, string FtpPassword)
        {
            //cftpServerIP = FtpServerIP;
            cftpRemotePath = FtpRemotePath;            

            //创建FTP链接对象
            reqFTPConnect =  CreateFTPConnect( FtpRemotePath, "test", "123456");
        }

        /// <summary>
        /// 创建FTP链接
        /// </summary>
        /// <param name="FtpServerIP">FTP连接地址</param>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="FtpUserID">用户名</param>
        /// <param name="FtpPassword">密码</param>
        /// <returns>FTP对象</returns>
        private FtpWebRequest CreateFTPConnect(string FtpRemotePath, string FtpUserID, string FtpPassword)
        {
            string ftpURI;

            //建立URI
            ftpURI = FtpRemotePath + "/";

            //建立连接
            //根据URI创建FtpWebRequest对象
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create( ftpURI );

            //ftp用户名和密码
            request.Credentials = new NetworkCredential( FtpUserID, FtpPassword );

            //代理服务器设置
            //request.Proxy = ""; 

            //命令执行完毕之后关闭连接                          
            request.KeepAlive = true;

            //指定数据传输类型                     
            request.UseBinary = true;                                               
            //request.UsePassive = usePassive;
            //request.EnableSsl = enableSsl;
            
            return request;
        }

        private FtpWebRequest CreateFTPConnect(FtpWebRequest ftpRequest, string FtpRemotePath )
        {
            cftpRemotePath = FtpRemotePath;

            string ftpURI = "";

            //建立URI
            //ftpURI = "ftp://" + cftpServerIP + "/" + FtpRemotePath + "/";

            //建立连接
            //根据URI创建FtpWebRequest对象
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpURI);

            //ftp用户名和密码
            request.Credentials = ftpRequest.Credentials;

            //代理服务器设置
            //request.Proxy = ""; 

            //命令执行完毕之后关闭连接                          
            request.KeepAlive = ftpRequest.KeepAlive;

            //指定数据传输类型                     
            request.UseBinary = ftpRequest.UseBinary;
            //request.UsePassive = usePassive;
            //request.EnableSsl = enableSsl;
            
            return request;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">需上传文件路径（包含文件）</param>
        /// <param name="filePath">上传文件名</param>
        /// <param name="ftpConnect">FTP对象</param>
        /// <param name="errorMSG">返回错误信息</param>
        /// <returns> 0 - 成功 -1 - 失败</returns>
        public int FileUpload(string filePath, string fileName, FtpWebRequest ftpConnect, ref string errorMSG)
        {
            int request = 0;
            
            try
            {
                //取得文件信息
                FileInfo fileInfo = new FileInfo(filePath);

                //打开文件
                using (FileStream fileStream = fileInfo.OpenRead())
                {    
                    //判断文件路径是否修改
                    if ( filePath != cftpRemotePath )
                    {
                        reqFTPConnect = CreateFTPConnect(reqFTPConnect, filePath);
                    }

                    //指定执行命令上传
                    ftpConnect.Method = WebRequestMethods.Ftp.UploadFile;

                    //建立根据ftp链接的字符序列
                    using (Stream ftpStream = ftpConnect.GetRequestStream())
                    {
                        //设置缓冲大小
                        byte[] buff = new byte[buffLength];

                        int i;
                        while ((i = fileStream.Read(buff, 0, buffLength)) > 0)
                        {
                            ftpStream.Write(buff, 0, i);
                        }

                        ftpStream.Close();
                    }

                    fileStream.Close();
                }
                                
                return request;
            }
            catch (Exception ex)
            {
                errorMSG = "上传文件失败错误为" + ex.Message;

                return -1;
            }
        }

        public int FileUpload(FileStream fileStream, string filePath, string fileName, ref string errorMSG)
        {
            int request = 0;

            try
            {
                //判断文件路径是否修改
                if (filePath != cftpRemotePath)
                {
                    reqFTPConnect = CreateFTPConnect(reqFTPConnect, filePath);
                }

                //指定执行命令上传
                reqFTPConnect.Method = WebRequestMethods.Ftp.UploadFile;

                //建立根据ftp链接的字符序列
                using (Stream ftpStream = reqFTPConnect.GetRequestStream())
                {
                    //设置缓冲大小
                    byte[] buff = new byte[buffLength];

                    int i;
                    while ((i = fileStream.Read(buff, 0, buffLength)) > 0)
                    {
                        ftpStream.Write(buff, 0, i);
                    }

                    ftpStream.Close();
                }

                return request;
            }
            catch (Exception ex)
            {
                errorMSG = "上传文件失败错误为" + ex.Message;

                return -1;
            }
        }
    }
}
