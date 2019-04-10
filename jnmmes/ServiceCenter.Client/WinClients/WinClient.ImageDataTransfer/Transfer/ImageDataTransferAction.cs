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
using ServiceCenter.Model;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.Client.WinService.ImageDataTransfer.Configuration;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Threading;


namespace ServiceCenter.Client.WinService.ImageDataTransfer.Transfer
{

    /// <summary>
    /// 图片数据转到MES数据库中。
    /// </summary>
    public class ImageDataTransferAction
    {
        public delegate void ThreadStart();
        public EventHandler<DataTransferFinishedArgs> OnDataTransferFinished;//声明自定义的事件委托，用来执行事件的声明，
        public const string EVENT_SOURCE_NAME = "MES.ImageDataTransfer";
        private FTPManager ftpManager;
               
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
        public string GetSourcePath(ImageDeviceElement element)
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
        
        public string GetTargetPath(ImageDeviceElement element)
        {
            //string strPath = "";
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

        public void CreateFTPManager( string FtpRemotePath, string FtpUserID, string FtpPassword)
        {
            ftpManager = new FTPManager(FtpRemotePath, FtpUserID, FtpPassword);
        }

        /// <summary>
        /// 将图片数据上传到MES中。
        /// </summary>
        /// <param name="element">图片设备配置。</param>
        /// <returns>true:转置成功。false：转置失败。</returns>
        public void Execute(ImageDeviceElement element)
        {
            bool blFindFiles = false;
            bool blTransferDataResult = true;
            string strTransferDataMsg = "";

            string sourceRootPath = "";             //数据来源文件路径
            string targetRootPath = "";             //数据迁移根目录

            try
            {
                //取得数据来源文件根目录。
                sourceRootPath = element.SourcePathRoot.TrimEnd(Path.DirectorySeparatorChar);

                //判断数据来源文件根目录是否存在
                if (Directory.Exists(sourceRootPath) == false)
                {
                    LogMessage(false, "数据来源文件路径:" + sourceRootPath + " 不存在！");

                    return;
                }

                //数据迁移根目录
                targetRootPath = element.TargetPathRoot.TrimEnd(Path.DirectorySeparatorChar);

                #region 获取源文件夹下的未移转图片。
                //取得数据来源文件目录对象
                DirectoryInfo diSourcePath = new DirectoryInfo(sourceRootPath);

                //数据文件格式
                string searchPattern = string.Format("*.{0}", element.FileExtensionName);

                //取得目录下所有文件
                FileInfo[] fileInfos = diSourcePath.GetFiles(searchPattern, SearchOption.AllDirectories);

                if (fileInfos != null && fileInfos.Length > 0)
                {
                    blFindFiles = true;
                }
                                
                //遍历文件夹文件。
                int maxNumberForLoop = 5;
                int nIndexOfFile = 0;
                if (fileInfos.Length > maxNumberForLoop)
                {
                    SortAsFileCreationTime(ref fileInfos);
                }

                #endregion










                //获取源文件夹路径。
                string sourcePath = this.GetSourcePath(element);

                //if (string.IsNullOrEmpty(sourcePath))
                //{
                //    blTransferDataResult = false;
                //    strTransferDataMsg = string.Format("获取 {0} 源文件夹路径失败。", element.Name);
                //}

                

                //目标目录数据目标文件夹根路径
                //string targetRootPath = element.TargetPathRoot.TrimEnd(Path.DirectorySeparatorChar);

                //设备名称;
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
                int maxNumberForLoop = 5;
                int nIndexOfFile = 0;
                if (fileInfos.Length > maxNumberForLoop)
                {
                    SortAsFileCreationTime(ref fileInfos);
                }
                IList<LotAttribute> lstLotAttribute = new List<LotAttribute>();
                foreach (FileInfo fiItem in fileInfos)
                {
                    if (nIndexOfFile > maxNumberForLoop)
                        break;
                    nIndexOfFile = nIndexOfFile + 1;

                    string targetFileName = fiItem.FullName.Replace(sourcePath, targetPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
                    #region  更新批次属性数据。
                    try
                    {
                        bool blFlag = true;
                        //复制源文件到目标文件夹下。
                        //blFlag = this.MoveFile(element, fiItem.FullName, targetFileName);
                        string lotNumber = "";

                        //if (blFlag)
                        //{
                        //    #region 更新 Lot属性
                        //    lotNumber = fiItem.Name.Replace(fiItem.Extension, string.Empty).ToUpper().Split('_', '-')[0];
                        //    lotNumber = lotNumber.Trim();
                        //    string attributeName = string.Format("{0}ImagePath", element.Type);
                        //    string attributeValue = targetFileName.Replace(targetRootPath, element.HttpPathRoot);
                        //    attributeValue = attributeValue.Replace('\\', '/');
                        //    LotAttribute obj = new LotAttribute()
                        //    {
                        //        Key = new LotAttributeKey()
                        //        {
                        //            LotNumber = lotNumber,
                        //            AttributeName = attributeName
                        //        },
                        //        AttributeValue = attributeValue,
                        //        Editor = "system",
                        //        EditTime = DateTime.Now
                        //    };
                        //    using (LotAttributeServiceClient client = new LotAttributeServiceClient())
                        //    {
                        //        MethodReturnResult result = client.Modify(obj);
                        //        if (result.Code > 0)
                        //        {
                        //            strTransferDataMsg = string.Format("更新批次号{0}错误:{1}", lotNumber, result.Message);
                        //            LogMessage(false, element.Type + ":" + strTransferDataMsg);
                        //            client.Close();
                        //        }
                        //        else
                        //        {
                        //            LogMessage(true, element.Type + ":" + lotNumber);
                        //        }
                        //        client.Close();
                        //    }
                        //    #endregion
                        //}
                    }
                    catch (Exception ex)
                    {
                        LogMessage(false, element.Type + ":" + ex.Message);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                blTransferDataResult = false;
                LogMessage(false, element.Type + ":" + ex.Message);
            }

            if (blFindFiles == false)
            {
                strTransferDataMsg = "";
                LogMessage(true, element.Type + ":" + strTransferDataMsg);
            }
        }

        private void SortAsFileCreationTime(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate(FileInfo x, FileInfo y) { return x.LastWriteTime.CompareTo(y.LastWriteTime); });
        }


        private bool MoveFile(ImageDeviceElement element, string sourceFile, string targetFile)
        {
            bool blReturn = false;
            int count = 0;
            FileInfo fi = new FileInfo(sourceFile);
            long ss = fi.Length;
            FileStream fileStream = null;
            int iReturn = 0;
            string strError = "";
            string strFilePath, strFileName;

            strFilePath = "ftp://10.0.3.238/IV/2016/06/20/";
            strFileName = "123.jpg";
                        
            while (count < 5)
            {
                count++;

                try
                {
                    //判断目标文件是否存在，若存在删除（后期是否改名称作为历史备份）
                    if (File.Exists(targetFile))
                    {
                        File.Delete(targetFile);
                    }

                    //判断文件是否打开（正在创建中）
                    if (fi.Length > 0)
                    {
                        //压缩图形
                        CompressPIC(sourceFile, ref fileStream);

                        //上传图形
                        //iReturn = FTPManager.FileUpload(fileStream, strFilePath, strFileName, targetFile, ref strError);
                        iReturn = ftpManager.FileUpload(fileStream, targetFile, strFileName, ref strError);
                    }

                    //if (fi.Length > 0)
                    //{
                    //    yasuo(sourceFile, targetFile);
                    //}
                    //else
                    //{
                    //    File.Move(sourceFile, targetFile);
                    //}
                    count = 5;
                    blReturn = true;
                }

                catch (IOException ex)
                {
                    LogMessage(false, string.Format(element.Type + ":" + "等待30秒(copy {0} to {1}).错误原因{2}", sourceFile, targetFile, ex.Message));
                    System.Threading.Thread.Sleep(3000);
                    continue;
                }
                catch (Exception ex)
                {
                    LogMessage(false, element.Type + ":" + ex.Message);
                    blReturn = false;
                }
            }
            return blReturn;
        }

        /// <summary>
        /// 图片转换（降低分辨率）
        /// </summary>
        /// <param name="sourceFile">源文件名</param>
        /// <param name="targetFile">转换后文件目录</param>
        public void CompressPIC(string sourceFile, ref FileStream fileStream)
        {
            Image img;
            Bitmap bmp;
            Graphics grap;
            int iWidth;                         //图形宽度
            int iHeight;                        //图形高度
            double decCompressRate = 0.5;       //压缩比率
            //MemoryStream stream = null;
            FileStream stream = null;

            //打开图形文件
            img = Image.FromFile(sourceFile);

            //取得文件信息
            FileInfo fi = new FileInfo(sourceFile);
                        
            //设置压缩后的宽和高
            iWidth = Convert.ToInt32(img.Width * decCompressRate);          //图形高度
            iHeight = Convert.ToInt32(img.Height * decCompressRate);        //图形宽度

            //创建新的画布，按照
            bmp = new Bitmap(iWidth, iHeight);

            //进行转换
            grap = Graphics.FromImage(bmp);

            grap.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            grap.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            grap.DrawImage(img, new Rectangle(0, 0, iWidth, iHeight));

            //图形上传
            //bmp.Save(targetFile, System.Drawing.Imaging.ImageFormat.Jpeg);

            //将图片传输到文件流            
            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

            fileStream = stream;

            bmp.Dispose();
            img.Dispose();
            grap.Dispose();

            //删除原文件
            //File.Delete(sourceFile);
        }

        /// <summary>
        /// 图片转换（降低分辨率）
        /// </summary>
        /// <param name="sourceFile">源文件名</param>
        /// <param name="targetFile">转换后文件目录</param>
        public void yasuo(string sourceFile, string targetFile)
        {
            Image img;
            Bitmap bmp;
            Graphics grap;
            int width;
            int height;
            img = Image.FromFile(sourceFile);
            FileInfo fi = new FileInfo(sourceFile);
            long ss = fi.Length;
            width = Convert.ToInt32(img.Width * 0.5);
            height = Convert.ToInt32(img.Height * 0.5);

            bmp = new Bitmap(width, height);
            grap = Graphics.FromImage(bmp);
            grap.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            grap.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            grap.DrawImage(img, new Rectangle(0, 0, width, height));

            bmp.Save(targetFile, System.Drawing.Imaging.ImageFormat.Jpeg);

            bmp.Dispose();
            img.Dispose();
            grap.Dispose();
            File.Delete(sourceFile);
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
