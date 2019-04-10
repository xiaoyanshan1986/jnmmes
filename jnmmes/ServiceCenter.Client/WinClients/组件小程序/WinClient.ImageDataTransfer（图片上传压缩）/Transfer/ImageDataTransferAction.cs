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
        public void Execute(ImageDeviceElement element)
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

                    #region  //更新批次属性数据。
                    try
                    {
                        bool blFlag = true;
                        //复制源文件到目标文件夹下。
                       // blFlag = this.MoveFile(element, fiItem.FullName, targetFileName);
                        string lotNumber = "";

                        if (blFlag)
                        {

                    #endregion

                            #region //更新 Lot属性
                            lotNumber = fiItem.Name.Replace(fiItem.Extension, string.Empty).ToUpper().Split('_', '-')[0];
                            lotNumber = lotNumber.Trim();

                            #region//获取批次所在工序及状态
                            using (LotQueryServiceClient client = new LotQueryServiceClient())
                            {
                                PagingConfig cfg = new PagingConfig()
                                {
                                    Where = string.Format("LOT_NUMBER = '{0} '", lotNumber),
                                    OrderBy = "EditTime Desc"
                                };
                                MethodReturnResult<IList<LotTransaction>> result = client.GetTransaction(ref cfg);
                                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0 && result.Data[0].RouteStepName == "功率测试" && result.Data[0].Activity == EnumLotActivity.TrackOut)
                                {
                                   
                                    string attributeName = string.Format("{0}ImagePath", element.Type);
                                    string attributeValue = targetFileName.Replace(targetRootPath, element.HttpPathRoot);
                                    attributeValue = attributeValue.Replace('\\', '/');

                                    //获取批次属性是否已存在EL3的属性，如果不存在，则更新EL3图片属性
                                    using (LotAttributeServiceClient client2 = new LotAttributeServiceClient())
                                    {
                                        PagingConfig cfg1 = new PagingConfig()
                                        {
                                            Where = string.Format("Key.LotNumber = '{0}'and Key.AttributeName='ELImagePath'", lotNumber),
                                            OrderBy = "EditTime Desc"
                                        };
                                        MethodReturnResult<IList<LotAttribute>> result2 = client.GetAttribute(ref cfg1);
                                        if (result2.Code <= 0 && result2.Data != null && result2.Data.Count > 0)
                                        { 
                                            blFlag = this.MoveFile(element, fiItem.FullName, targetFileName);
                                            FileInfo[] fileInfos1 = diSourcePath.GetFiles(searchPattern, SearchOption.AllDirectories);
                                            foreach (FileInfo fiItem1 in fileInfos1)
                                            {
                                                if (nIndexOfFile > maxNumberForLoop)
                                                    break;
                                                nIndexOfFile = nIndexOfFile + 1;

                                               targetFileName = fiItem1.FullName.Replace(sourcePath, targetPath);
                                                Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
                                                File.Delete(fiItem1.FullName);
                                            }
                                            attributeValue = targetFileName.Replace(targetRootPath, element.HttpPathRoot);
                                            LotAttribute obj = new LotAttribute()
                                            {
                                                Key = new LotAttributeKey()
                                                {
                                                    LotNumber = lotNumber,
                                                    AttributeName = attributeName
                                                },
                                                AttributeValue = attributeValue,
                                                Editor = "system",
                                                EditTime = DateTime.Now
                                            };
                                            using (LotAttributeServiceClient client1 = new LotAttributeServiceClient())
                                            {
                                                MethodReturnResult result1 = client1.Modify(obj);
                                                if (result.Code > 0)
                                                {
                                                    strTransferDataMsg = string.Format("更新批次号{0}错误:{1}", lotNumber, result.Message);
                                                    LogMessage(false, element.Type + ":" + strTransferDataMsg);
                                                    client.Close();
                                                }
                                                else
                                                {
                                                    LogMessage(true, element.Type + ":" + lotNumber);
                                                }
                                                client.Close();
                                            }  

                                        }
                                        else
                                        {

                                            LotAttribute obj = new LotAttribute()
                                            {
                                                Key = new LotAttributeKey()
                                                {
                                                    LotNumber = lotNumber,
                                                    AttributeName = attributeName
                                                },
                                                AttributeValue = attributeValue,
                                                Editor = "system",
                                                EditTime = DateTime.Now
                                            };
                                            using (LotAttributeServiceClient client1 = new LotAttributeServiceClient())
                                            {
                                                MethodReturnResult result1 = client1.Modify(obj);
                                                if (result.Code > 0)
                                                {
                                                    strTransferDataMsg = string.Format("更新批次号{0}错误:{1}", lotNumber, result.Message);
                                                    LogMessage(false, element.Type + ":" + strTransferDataMsg);
                                                    client.Close();
                                                }
                                                else
                                                {
                                                    LogMessage(true, element.Type + ":" + lotNumber);
                                                }
                                                client.Close();
                                            }
                                            blFlag = this.MoveFile(element, fiItem.FullName, targetFileName);
                                            File.Delete(fiItem.FullName);


                                            
                                        }
                                    }
                                }
                                else
                                {
                                    result.Message = "组件批次不在功率测试出站";
                                    strTransferDataMsg = string.Format("批次号{0}错误:{1}", lotNumber, result.Message);
                                    blFlag = this.MoveFile(element, fiItem.FullName, targetFileName);
                                    File.Delete(fiItem.FullName);
                                }

                            #endregion
                            }
                        }
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
            while (count < 5)
            {
                count++;

                try
                {
                    if (File.Exists(targetFile))
                    {
                        //File.Delete(targetFile);
                        // 列表中的原始文件全路径名
                        string oldStr = fi.Name;
                        if (fi.Name.Substring(0, 2) != "JN")
                        {
                            string i = "";
                            string newStr = "";

                            i = oldStr.Substring(13, 2);
                            if (i == "" || i == ".j")
                            {

                                int j = 1;
                                string m = Convert.ToString(j);
                                // 新文件名
                                newStr = fi.Name.Substring(0, 13) + "_" + m + fi.Name.Substring(13, 4);
                            }
                            else
                            {
                                i = oldStr.Substring(14, 1);
                                int j = Convert.ToInt16(i);
                                j = j + 1;
                                string m = Convert.ToString(j);
                                // 新文件名
                                newStr = fi.Name.Substring(0, 13) + "_" + j + fi.Name.Substring(15, 4);

                            }
                            //// 列表中的原始文件全路径名
                            string old = fi.DirectoryName + @"\" + oldStr;

                            //// 新文件名
                            string new1 = fi.DirectoryName + @"\" + newStr;

                            // 改名方法
                            FileInfo fi1 = new FileInfo(old);
                            fi1.MoveTo(Path.Combine(new1));
                            sourceFile = fi1.FullName;
                            targetFile = element.TargetPathRoot + @"\" + newStr;
                            if (fi1.Length > 0)
                            {
                                yasuo(sourceFile, targetFile);

                            }
                            else
                            {
                                File.Move(sourceFile, targetFile);
                            }

                        }
                        else
                        {
                            string i = "";
                            string newStr = "";

                            i = oldStr.Substring(15, 2);
                            if (i == "" || i == ".j")
                            {

                                int j = 1;
                                string m = Convert.ToString(j);
                                // 新文件名
                                newStr = fi.Name.Substring(0, 15) + "_" + m + fi.Name.Substring(15, 4);
                            }
                            else
                            {
                                i = oldStr.Substring(16, 1);
                                int j = Convert.ToInt16(i);
                                j = j + 1;
                                string m = Convert.ToString(j);
                                // 新文件名
                                newStr = fi.Name.Substring(0, 15) + "_" + j + fi.Name.Substring(17, 4);

                            }
                            //// 列表中的原始文件全路径名
                            string old = fi.DirectoryName + @"\" + oldStr;

                            //// 新文件名
                            string new1 = fi.DirectoryName + @"\" + newStr;

                            // 改名方法
                            FileInfo fi1 = new FileInfo(old);
                            fi1.MoveTo(Path.Combine(new1));
                            sourceFile = fi1.FullName;
                            targetFile = element.TargetPathRoot + @"\" + newStr;
                            if (fi1.Length > 0)
                            {
                                yasuo(sourceFile, targetFile);

                            }
                            else
                            {
                                File.Move(sourceFile, targetFile);
                            }                      
                        
                        }
                        
                    }
                    else 
                    {
                        if (fi.Length > 0)
                        {
                            yasuo(sourceFile, targetFile);
                        }
                        else
                        {
                            File.Move(sourceFile, targetFile);
                        }
                    }
                   
                    count = 5;
                    blReturn = true;
                }

                catch (IOException ex)
                {
                    LogMessage(false, string.Format(element.Type + ":" + "等待30秒(copy {0} to {1}).错误原因{2}", sourceFile, targetFile, ex.Message));
                    System.Threading.Thread.Sleep(30000);
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
            //File.Delete(sourceFile);
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
