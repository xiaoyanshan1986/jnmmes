using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinClient.VTestDataTransfer.Configuration;

namespace WinClient.VTestDataTransfer
{
    class CommonFun
    {
        public static string GetFullFile(string path, string format)
        {
            string strFileFullName = string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                return strFileFullName;
            }
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            FileSystemInfo[] fileInfo = TheFolder.GetFileSystemInfos("*.mdb");
            DateTime dtLastWrite = DateTime.MinValue;
            FileInfo fiLastWrite = null;
            DateTime dtFileName = DateTime.Now;
            string fileName = string.Format(format, dtFileName);

            ////根据测试机早上8点钟才会更换测试数据文件的属性，增加下面的判断
            //if (DateTime.Now >= Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00") 
            //    && DateTime.Now < Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 08:00:00"))
            //{
            //    dtFileName = dtFileName.AddDays(-1);
            //}
                        
            //获取自定义的文件路径。
            if (string.IsNullOrEmpty(strFileFullName))
            {
                foreach (FileSystemInfo i in fileInfo)
                {
                    if (i is FileInfo && i.Name == fileName)
                    {
                        strFileFullName = i.FullName;
                        break;
                    }
                }
            }

            //获取最新的文件。
            if (string.IsNullOrEmpty(strFileFullName))
            {
                foreach (FileSystemInfo i in fileInfo)
                {
                    if (i is FileInfo && i.LastWriteTime > dtLastWrite)
                    {
                        dtLastWrite = i.LastWriteTime;
                        fiLastWrite = i as FileInfo;
                    }
                }
                if (fiLastWrite != null)
                {
                    strFileFullName = fiLastWrite.FullName;
                }
            }
            return strFileFullName;
        }
        
        public static string GetFullPath(string path, string format)
        {
            string strFullPath = string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                return strFullPath;
            }
            DateTime dtFileName = DateTime.Now;
            ////根据测试机早上8点钟才会更换测试数据文件的属性，增加下面的判断
            //if (DateTime.Now >= Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00") 
            //    && DateTime.Now < Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 08:00:00"))
            //{
            //    dtFileName = dtFileName.AddDays(-1);
            //}
            string subFolderName = string.Format(format, dtFileName);
            strFullPath = path + subFolderName;
            
            DirectoryInfo TheFolder = new DirectoryInfo(strFullPath);
            if (TheFolder!=null &&TheFolder.Exists)
            {
                return strFullPath;
            }else
            {
                return path;
            }
        }

        public static bool CheckFileExists(string fullFileName)
        {
            return File.Exists(fullFileName);
        }

        public static string UploadFile(string strFileName,IVTestDeviceElement element)
        {
            string strMessage = "";
            try 
            { 
                if (File.Exists(strFileName))
                {
                    FileInfo fInfo = new FileInfo(strFileName);
                    FtpManager.UploadFile(fInfo, element.FtpTargetFolder, element.FtpServer, element.FtpUser, element.FtpPassword);
                    strMessage = string.Format("上传图片({0})成功", strFileName);
                }
                else
                {
                    strMessage = string.Format("图片({0})不存在",strFileName);
                }
            }
            catch(Exception err)
            {
                strMessage = err.Message;
            }
            return strMessage;
        }
    }
}
