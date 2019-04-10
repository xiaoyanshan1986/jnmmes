


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Data.Common;
using System.Windows.Forms;
using WinClient.ELImageTransfer;
using WinClient.ELImageTransfer.Configuration;

namespace ServiceCenter.Client.WinService.ImageDataTransfer.Transfer
{

    /// <summary>
    /// 图片数据转到MES数据库中。
    /// </summary>
    public class ImageDataTransferAction
    {

        public EventHandler<DataTransferFinishedArgs> OnDataTransferFinished;//声明自定义的事件委托，用来执行事件的声明，
        public const string EVENT_SOURCE_NAME = "MES.ImageDataTransfer";
        /// <summary>
        /// 构造函数。
        /// </summary>
        public ImageDataTransferAction()
        {
        }
        /// <summary>
        /// 上一次执行转置的数据数量。
        /// </summary>
        public int TransferCount
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取数据文件路径。先按照式化字符串中设置的格式寻找，如果没有匹配则返回文件夹中最新的文件夹路径。
        /// </summary>
        /// <param name="element">图片设备配置。</param>
        /// <returns>
        /// 图片数据源文件夹路径。
        /// </returns>
        public string GetSourcePath(ELImageDeviceElement element)
        {
            if (string.IsNullOrEmpty(element.SourcePathRoot)
                || Directory.Exists(element.SourcePathRoot) == false)
            {
                return string.Empty;
            }
            string rootPath = element.SourcePathRoot.TrimEnd(Path.DirectorySeparatorChar);
            string format = element.SourcePathFormat.Trim(Path.DirectorySeparatorChar);
            DateTime dtFileName = DateTime.Now;
            //根据测试机早上8点钟才会更换测试数据文件路径的属性，增加下面的判断
            if (DateTime.Now >= Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00")
                && DateTime.Now < Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 08:00:00"))
            {
                dtFileName = dtFileName.AddDays(-1);
            }
            string sourcePath = rootPath + Path.DirectorySeparatorChar + string.Format(format, dtFileName);
            if (Directory.Exists(sourcePath))
            {
                return sourcePath;
            }

            DirectoryInfo TheFolder = new DirectoryInfo(element.SourcePathRoot);
            DirectoryInfo[] directoryInfos = TheFolder.GetDirectories();
            DateTime dtLastWrite = DateTime.MinValue;
            DirectoryInfo diLastWrite = null;
            //获取最新的文件夹。
            foreach (DirectoryInfo item in directoryInfos)
            {
                if (item.LastWriteTime > dtLastWrite)
                {
                    dtLastWrite = item.LastWriteTime;
                    diLastWrite = item;
                }
            }
            if (diLastWrite != null)
            {
                return diLastWrite.FullName;
            }
            else
            {
                return rootPath;
            }
        }


        public string GetTargetPath(ELImageDeviceElement element)
        {
            string strPath = "";
            if (string.IsNullOrEmpty(element.TargetPathRoot)
              || Directory.Exists(element.TargetPathRoot) == false)
            {
                return string.Empty;
            }
            string rootPath = element.TargetPathRoot.TrimEnd(Path.DirectorySeparatorChar);
            string format = element.SourcePathFormat.Trim(Path.DirectorySeparatorChar);
            DateTime dtFileName = DateTime.Now;
            //根据测试机早上8点钟才会更换测试数据文件路径的属性，增加下面的判断
            if (DateTime.Now >= Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00")
                && DateTime.Now < Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 08:00:00"))
            {
                dtFileName = dtFileName.AddDays(-1);
            }
            string sourcePath = rootPath + Path.DirectorySeparatorChar + string.Format(format, dtFileName);
            if (Directory.Exists(sourcePath) == false)
            {
                Directory.CreateDirectory(sourcePath);
            }
            return sourcePath;
        }


        /// <summary>
        /// 将图片数据上传到MES中。
        /// </summary>
        /// <param name="element">图片设备配置。</param>
        /// <returns>true:转置成功。false：转置失败。</returns>
        public void Execute(ELImageDeviceElement element)
        {
            bool blFindFiles = false;
            bool blTransferDataResult = true;
            string strTransferDataMsg = "";
            try
            {

                //获取源文件夹路径。
                string sourcePath = this.GetSourcePath(element);
                if (string.IsNullOrEmpty(sourcePath))
                {
                    blTransferDataResult = false;
                    strTransferDataMsg = string.Format("获取 {0} 源文件夹路径失败。", element.Name);
                }
                //获取目标文件夹路径。
                string sourceRootPath = element.SourcePathRoot.TrimEnd(Path.DirectorySeparatorChar);
                string targetRootPath = element.TargetPathRoot.TrimEnd(Path.DirectorySeparatorChar);
                //string targetPath = sourcePath.Replace(sourceRootPath, targetRootPath);
                string targetPath = this.GetTargetPath(element);

                if (Directory.Exists(targetPath) == false)
                {
                    Directory.CreateDirectory(targetPath);
                }
                //获取源文件夹下的未移转图片。
                DirectoryInfo diSourcePath = new DirectoryInfo(sourcePath);
                string searchPattern = string.Format("*.{0}", element.FileExtensionName);
                FileInfo[] fileInfos = diSourcePath.GetFiles(searchPattern, SearchOption.AllDirectories);
                if (fileInfos != null && fileInfos.Length > 0)
                {
                    blFindFiles = true;
                }
                DateTime dtMaxTime = DateTime.MinValue;
              
                //遍历文件夹文件。
                int maxNumberForLoop = 10;
                int nIndexOfFile = 0;
                foreach (FileInfo fiItem in fileInfos)
                {
                    if (nIndexOfFile > maxNumberForLoop)
                        break;
                    nIndexOfFile = nIndexOfFile + 1;


                    string targetFileName = fiItem.FullName.Replace(sourcePath, targetPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
                    //复制源文件到目标文件夹下。
                    File.Copy(fiItem.FullName, targetFileName, true);
                    dtMaxTime = fiItem.LastWriteTime.AddMilliseconds(1);
                

                    #region //进行原文件的后续处理。
                    int count = 0;
                    while (count < 5)
                    {
                        count++;
                        try
                        {
                            //如设置删除源文件，则删除。
                            if (element.IsDeleteSourceFile)
                            {
                                File.Delete(fiItem.FullName);
                            }
                            else
                            {
                                //否则，移动到LocalFiles下。
                                //string copyFilePath = sourcePath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + "LocalFiles";
                                //if (Directory.Exists(copyFilePath) == false)
                                //{
                                //    Directory.CreateDirectory(copyFilePath);
                                //}
                                //string copyFileName = fiItem.FullName.Replace(sourcePath, copyFilePath);
                                //File.Copy(fiItem.FullName, copyFileName,true);
                                //File.Delete(fiItem.FullName);
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            LogMessage(false, ex.Message);
                            System.Threading.Thread.Sleep(500);
                            continue;
                        }
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                blTransferDataResult = false;
                LogMessage(false, ex.Message);
            }

            if (blFindFiles == false)
            {
                strTransferDataMsg = "";
                LogMessage(true, strTransferDataMsg);
            }


        }

        private void LogMessage(bool blTransferDataResult, string strTransferDataMsg)
        {

            DataTransferFinishedArgs arg = new DataTransferFinishedArgs();
            arg.TransferDataResult = blTransferDataResult;
            if (blTransferDataResult)
            {
                strTransferDataMsg = string.Format("{1:yyyy-MM-dd HH:mm:ss}读取批次图片成功>{0}", strTransferDataMsg, System.DateTime.Now.ToString());
            }
            else
            {
                strTransferDataMsg = string.Format("{1:yyyy-MM-dd HH:mm:ss}读取批次图片失败>{0}", strTransferDataMsg, System.DateTime.Now.ToString());
            }
            arg.TransferMsg = strTransferDataMsg;
            if (OnDataTransferFinished != null)
            {
                CommonFun.eventInvoket(() => { OnDataTransferFinished(this, arg); });
            }
        }
    }




    //namespace WinClient.ELImageTransfer
    //{
    //    class ELImageTransferAction
    //    {
    //        private string strImageFullPath = "";
    //        public void Execute(ELImageDeviceElement element, String strLot)
    //        {
    //            strImageFullPath = CommonFun.GetFullPath(element.SourceImagePathRoot, element.SourceImagePathFormat);
    //            string strFileName = strImageFullPath + strLot + "." + element.ImageExtensionName;
    //            FileInfo fInfo = new FileInfo(strFileName);
    //            FtpManager.UploadFile(fInfo, element.FtpTargetFolder, element.FtpServer, element.FtpUser, element.FtpPassword);

    //        }
    //    }

    //}


    public class DataTransferFinishedArgs : EventArgs
    {
        private string strTransferMsg = "";
        private string strTransferDbFile = "";
        private bool blTransferDataResult = false;
        private DateTime dMaxTestDateTime;


        public DataTransferFinishedArgs()
        {

        }
        public DataTransferFinishedArgs(bool TransferDataResult, string transferMsg, DateTime maxTestDateTime)
        {
            blTransferDataResult = TransferDataResult;
            strTransferMsg = transferMsg;
            dMaxTestDateTime = maxTestDateTime;
        }
        public string TransferMsg
        {
            get
            {
                return strTransferMsg;
            }
            set
            {
                strTransferMsg = value;
            }
        }

        public string TransferDbFile
        {
            get
            {
                return strTransferDbFile;
            }
            set
            {
                strTransferDbFile = value;
            }
        }

        public bool TransferDataResult
        {
            get
            {
                return blTransferDataResult;
            }
            set
            {
                blTransferDataResult = value;
            }
        }

        public DateTime MaxTestDateTime
        {
            get
            {
                return dMaxTestDateTime;
            }
            set
            {
                dMaxTestDateTime = value;
            }
        }

    }
}
