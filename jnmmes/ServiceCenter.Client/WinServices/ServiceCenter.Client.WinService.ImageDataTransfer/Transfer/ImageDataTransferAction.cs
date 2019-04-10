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

namespace ServiceCenter.Client.WinService.ImageDataTransfer.Transfer
{

    /// <summary>
    /// 图片数据转到MES数据库中。
    /// </summary>
    public class ImageDataTransferAction
    {

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
            DirectoryInfo[] directoryInfos= TheFolder.GetDirectories();
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
            if (Directory.Exists(sourcePath)==false)
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
        public bool Execute(ImageDeviceElement element)
        {
            try
            {
                //获取源文件夹路径。
                string sourcePath = this.GetSourcePath(element);
                if (string.IsNullOrEmpty(sourcePath))
                {
                    EventLog.WriteEntry(EVENT_SOURCE_NAME
                                    ,  string.Format("获取 {0} 源文件夹路径失败。", element.Name)
                                    , EventLogEntryType.Error);
                    return false;
                }
                //获取目标文件夹路径。
                string sourceRootPath = element.SourcePathRoot.TrimEnd(Path.DirectorySeparatorChar);
                string targetRootPath = element.TargetPathRoot.TrimEnd(Path.DirectorySeparatorChar);
                //string targetPath = sourcePath.Replace(sourceRootPath, targetRootPath);
                string targetPath = this.GetTargetPath(element);

                if (Directory.Exists(targetPath)==false)
                {
                    EventLog.WriteEntry(EVENT_SOURCE_NAME
                                    , string.Format("{0} 目标文件夹路径（{1}）不存在。", element.Name, targetPath)
                                    , EventLogEntryType.Warning);
                    Directory.CreateDirectory(targetPath);
                }
                //获取源文件夹下的未移转图片。
                DirectoryInfo diSourcePath = new DirectoryInfo(sourcePath);
                string searchPattern = string.Format("*.{0}", element.FileExtensionName);
                FileInfo[] fileInfos = diSourcePath.GetFiles(searchPattern, SearchOption.AllDirectories);
                DateTime dtMaxTime = DateTime.MinValue;
                //移除最新的更新时间问题
                /*               
                using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
                {
                    MethodReturnResult<ClientConfigAttribute> rst = client.Get(new ClientConfigAttributeKey()
                    {
                        ClientName = element.Name,
                        AttributeName = string.Format("{0}ImageDateTime", element.Type)
                    });
                    if (rst.Code <= 0 && rst.Data != null)
                    {
                        dtMaxTime = DateTime.Parse(rst.Data.Value);
                    }
                    client.Close();
                }
                var lnq = from item in fileInfos
                          where item.LastWriteTime > dtMaxTime
                          orderby item.LastWriteTime
                          select item;
                */
                //遍历文件夹文件。
                IList<LotAttribute> lstLotAttribute = new List<LotAttribute>();
                foreach (FileInfo fiItem in fileInfos)
                {
                    string targetFileName = fiItem.FullName.Replace(sourcePath, targetPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
                    //复制源文件到目标文件夹下。
                    File.Copy(fiItem.FullName, targetFileName, true);
                    dtMaxTime = fiItem.LastWriteTime.AddMilliseconds(1);
                    #region  //更新批次属性数据。
                    try
                    {
                        string lotNumber = fiItem.Name.Replace(fiItem.Extension, string.Empty).ToUpper().Split('_','-')[0];
                        lotNumber = lotNumber.Trim();
                        string attributeName = string.Format("{0}ImagePath",element.Type);
                        string attributeValue = targetFileName.Replace(targetRootPath, element.HttpPathRoot);
                        attributeValue = attributeValue.Replace('\\', '/');
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
                        using (LotAttributeServiceClient client = new LotAttributeServiceClient())
                        {
                            MethodReturnResult result = client.Modify(obj);
                            if (result.Code > 0)
                            {
                                EventLog.WriteEntry(EVENT_SOURCE_NAME
                                           , string.Format("{0}:{1} {2}", element.Name, fiItem.FullName, result.Message)
                                           , EventLogEntryType.Warning);
                                client.Close();
                                continue;
                            }
                            client.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(EVENT_SOURCE_NAME
                                       , string.Format("{0}:{1} {2}", element.Name, fiItem.FullName, ex.Message)
                                       , EventLogEntryType.Error);
                        System.Threading.Thread.Sleep(500);
                        return false;
                    }
                    #endregion

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
                            {//否则，移动到LocalFiles下。
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
                            EventLog.WriteEntry(EVENT_SOURCE_NAME
                                        , string.Format("{0}:{1} {2}", element.Name, fiItem.FullName, ex.Message)
                                        , EventLogEntryType.Error);
                            System.Threading.Thread.Sleep(500);
                            continue;
                        }
                    }
                    #endregion

                    #region 更新ClientConfig的最新更新时间
                    /*
                    using (ClientConfigServiceClient client = new ClientConfigServiceClient())
                    {
                        MethodReturnResult<ClientConfig> rst = client.Get(element.Name);
                        if (rst.Data == null)
                        {
                            ClientConfig obj = new ClientConfig()
                            {
                                ClientType = EnumClientType.Other,
                                CreateTime = DateTime.Now,
                                Creator = "system",
                                Description = string.Empty,
                                Editor = "system",
                                EditTime = DateTime.Now,
                                IPAddress = element.Name,
                                Key = element.Name,
                                LocationName = null
                            };
                            client.Add(obj);
                        }
                        client.Close();
                    }
                    using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
                    {
                        ClientConfigAttribute obj = null;
                        ClientConfigAttributeKey ccaKey = new ClientConfigAttributeKey()
                        {
                            ClientName = element.Name,
                            AttributeName = string.Format("{0}ImageDateTime",element.Type)
                        };
                        MethodReturnResult<ClientConfigAttribute> rst = client.Get(ccaKey);
                        if (rst.Code <= 0 && rst.Data != null)
                        {
                            obj = rst.Data;
                        }
                        if (obj != null)
                        {
                            obj.Value = dtMaxTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            obj.Editor = "system";
                            obj.EditTime = DateTime.Now;
                            client.Modify(obj);
                        }
                        else
                        {
                            obj = new ClientConfigAttribute()
                            {
                                Key = ccaKey,
                                Value = dtMaxTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                Editor = "system",
                                EditTime = DateTime.Now
                            };
                            client.Add(obj);
                        }
                        client.Close();
                    }
                    */
                    #endregion
                }
                    
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(EVENT_SOURCE_NAME
                                    , string.Format("{0}:{1}", element.Name, ex.Message)
                                    , EventLogEntryType.Error);
                return false;
            }
            return true;
        }

    }
}
