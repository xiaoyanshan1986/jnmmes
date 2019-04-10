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
using ServiceCenter.Client.WinService.IVTestDataTransfer.Configuration;

namespace ServiceCenter.Client.WinService.IVTestDataTransfer.Transfer
{

    /// <summary>
    /// IV测试数据转到MES数据库中。
    /// </summary>
    public class IVTestDataTransferAction
    {
        public const string EVENT_SOURCE_NAME = "MES.IVTestDataTransfer";
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="accessConnString">Access数据库连接字符串。</param>
        public IVTestDataTransferAction(string accessConnString)
        {
            this.AccessConnectionString = accessConnString;
        }
        /// <summary>
        /// Access数据库连接字符串。
        /// </summary>
        private string AccessConnectionString
        {
            get;
            set;
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
        /// 获取数据文件路径。先按照式化字符串中设置的格式寻找，如果没有匹配则返回文件夹中最新的文件。
        /// </summary>
        /// <param name="path">数据文件所在文件夹路径。</param>
        /// <param name="format">数据文件名称的格式化字符串。</param>
        /// <returns>
        /// 数据文件路径。
        /// </returns>
        public static string GetFullFile(string path,string format)
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
            ////根据测试机早上8点钟才会更换测试数据文件的属性，增加下面的判断
            //if (DateTime.Now >= Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00") 
            //    && DateTime.Now < Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 08:00:00"))
            //{
            //    dtFileName = dtFileName.AddDays(-1);
            //}
            string fileName = string.Format(format, dtFileName);
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
        /// <summary>
        /// 将数据从Access数据库转移到MES数据库。
        /// </summary>
        /// <param name="element">设备配置。</param>
        /// <returns>true:转置成功。false：转置失败。</returns>
        public bool Execute(IVTestDeviceElement element)
        {
            try
            {
                //根据设备代码获取SQL Server数据库中最大的测试时间值。
                DateTime dtMaxTestTime = new DateTime(2000, 1, 1);
                PagingConfig cfg = new PagingConfig()
                {
                   PageNo=0,
                   PageSize=1,
                   Where = string.Format("Key.EquipmentCode='{0}'",element.Name),
                   OrderBy="Key.TestTime Desc"
                };
                using (IVTestDataServiceClient client = new IVTestDataServiceClient())
                {
                    MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);
                    if (result.Code == 0 
                        && result.Data != null
                        && result.Data.Count>0)
                    {
                        dtMaxTestTime = result.Data[0].Key.TestTime;
                    }
                    client.Close();
                }
                //组织查询IV测试数据的SQL语句
                StringBuilder sbSql = new StringBuilder();
                string sql = string.Empty;

                if (element.Type == IVTestDeviceType.Customer)
                {
                    sql = element.Sql;
                }
                else
                {
                    sql = GetQueryTestDataSql(element.Type);
                }
                sbSql.AppendFormat("SELECT TOP 100 '{2}' AS DeviceNo,a.* FROM ({0}) a WHERE a.TTIME>'{1}' ORDER BY a.TTIME ASC",
                                    sql,
                                    dtMaxTestTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                    element.Name);
                DataSet dsIVTestData = new DataSet();
                //创建 Access的连接对象。
                using (OleDbConnection oleCon = new OleDbConnection(this.AccessConnectionString))
                {
                    oleCon.Open();
                    using (OleDbCommand oleCmd = oleCon.CreateCommand())
                    {
                        //从Access数据库获取>开始日期和开始时间的数据。
                        oleCmd.CommandType = CommandType.Text;
                        oleCmd.CommandText = sbSql.ToString();
                        OleDbDataAdapter oleAdapter = new OleDbDataAdapter(oleCmd);
                        oleAdapter.Fill(dsIVTestData);
                    }
                    oleCon.Close();
                }
                //新增IV测试数据
                if (dsIVTestData != null && dsIVTestData.Tables.Count > 0 && dsIVTestData.Tables[0].Rows.Count > 0)
                {
                    IVTestDataTransferParameter p = new IVTestDataTransferParameter();
                    p.List = new List<IVTestData>();
                    foreach (DataRow dr in dsIVTestData.Tables[0].Rows)
                    {
                        IVTestData iv = new IVTestData();
                        iv.Key = new IVTestDataKey()
                        {
                            EquipmentCode = element.Name,
                            LotNumber = Convert.ToString(dr["LOT_NUMBER"]).ToUpper(),
                            TestTime = Convert.ToDateTime(dr["TTIME"])
                        };
                        //--TTIME 测试时间
                        //--LOT_NUMBER 批次号
                        //--FF 填充因子
                        //--ISC 测试短路电流
                        //--EFF 组件转换效率
                        //--RSH 串联电阻
                        //--RS 并联电阻
                        //--VOC 测试开路电压
                        //--IPM 测试最大电流
                        //--PM 测试最大功率
                        //--VPM 测试最大电压
                        //--AMBIENTTEMP 测度温度
                        //--SENSORTEMP 环境温度
                        //--INTENSITY 光强
                        iv.FF = dr["FF"]!=DBNull.Value ? Convert.ToDouble(dr["FF"]) : 0;
                        iv.ISC = dr["ISC"] != DBNull.Value ? Convert.ToDouble(dr["ISC"]) : 0;
                        iv.EFF = dr["EFF"] != DBNull.Value ? Convert.ToDouble(dr["EFF"]) : 0;
                        iv.RSH = dr["RSH"] != DBNull.Value ? Convert.ToDouble(dr["RSH"]) : 0;
                        iv.RS = dr["RS"] != DBNull.Value ? Convert.ToDouble(dr["RS"]) : 0;
                        iv.VOC = dr["VOC"] != DBNull.Value ? Convert.ToDouble(dr["VOC"]) : 0;
                        iv.IPM = dr["IPM"] != DBNull.Value ? Convert.ToDouble(dr["IPM"]) : 0;
                        iv.PM = dr["PM"] != DBNull.Value ? Convert.ToDouble(dr["PM"]) : 0;
                        iv.VPM = dr["VPM"] != DBNull.Value ? Convert.ToDouble(dr["VPM"]) : 0;
                        iv.AmbientTemperature = dr["AMBIENTTEMP"] != DBNull.Value ? Convert.ToDouble(dr["AMBIENTTEMP"]) : 0;
                        iv.SensorTemperature = dr["SENSORTEMP"] != DBNull.Value ? Convert.ToDouble(dr["SENSORTEMP"]) : 0;
                        iv.Intensity = dr["INTENSITY"] != DBNull.Value ? Convert.ToDouble(dr["INTENSITY"]) : 0;
                        iv.CoefFF = iv.FF;
                        iv.CoefIPM = iv.IPM;
                        iv.CoefISC = iv.ISC;
                        iv.CoefPM = iv.PM;
                        iv.CoefVOC = iv.VOC;
                        iv.CoefVPM = iv.VPM;
                        iv.IsDefault = false;
                        iv.IsPrint = false;
                        //新增IV测试数据。
                        p.List.Add(iv);
                    }
                    //开始移转IV测试数据。
                    if (p.List.Count != 0)
                    {
                        using (IVTestDataTransferServiceClient client = new IVTestDataTransferServiceClient())
                        {
                            MethodReturnResult result = client.Transfer(p);
                            if (result.Code > 0)
                            {
                                EventLog.WriteEntry(EVENT_SOURCE_NAME
                                                    , string.Format("{0}:{1}", element.Name, result.Message)
                                                    , EventLogEntryType.Error);
                                client.Close();
                                return false;
                            }
                            else
                            {
                                this.TransferCount = p.List.Count;
                                client.Close();
                            }
                        }
                    }
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
        /// <summary>
        /// 获取查询测试数据的SQL字符串。
        /// </summary>
        /// <param name="type">设备类型。</param>
        /// <returns>查询测试数据的SQL字符串。</returns>
        private string GetQueryTestDataSql(IVTestDeviceType type)
        {
            //--TTIME 测试时间
            //--LOT_NUMBER 批次号
            //--FF 填充因子
            //--ISC 测试短路电流
            //--EFF 组件转换效率
            //--RSH 串联电阻
            //--RS 并联电阻
            //--VOC 测试开路电压
            //--IPM 测试最大电流
            //--PM 测试最大功率
            //--AMBIENTTEMP 测度温度
            //--SENSORTEMP 环境温度
            //--INTENSITY 光强
            string sql = string.Empty;
            if (type == IVTestDeviceType.SunData)
            {
                sql = @"SELECT Format(SunData.[DateTime],'yyyy-MM-dd HH:mm:ss') AS TTIME,
	                        SunData.[Serial] AS LOT_NUMBER,
	                        SunData.[FF] AS FF,
	                        SunData.[Isc] AS ISC,
	                        SunData.[Eff] AS EFF,
	                        SunData.[Rsh] AS RSH,
	                        SunData.[Rs] AS RS,
	                        SunData.[Voc] AS VOC,
	                        SunData.[Imax] AS IPM,
	                        SunData.[Vmax] AS VPM,
	                        SunData.[Pmax] AS PM,
	                        SunData.[Temp] AS AMBIENTTEMP,
	                        SunData.[EnvTemp] AS SENSORTEMP, 
	                        SunData.[Sun] AS INTENSITY
                        FROM SunData";
            }
            else if (type == IVTestDeviceType.Results)
            {
                sql = @"SELECT  Format(Results.Test_Date,'yyyy-MM-dd ')+Format(Results.Test_Time,'HH:mm:ss') AS TTIME,
	                        Results.ID AS LOT_NUMBER,
	                        Results.FF AS FF,
	                        Results.Isc AS ISC,
	                        Results.ModEff AS EFF,
	                        Results.Rsh AS RSH,
	                        Results.Rs AS RS,
	                        Results.Voc AS VOC,
	                        Results.Ipm AS IPM,
	                        Results.Vpm AS VPM,
	                        Results.Pmax AS PM,
	                        Results.TMod AS AMBIENTTEMP,
	                        Results.TRef1 AS SENSORTEMP,
	                        Results.CIrr AS INTENSITY
                        FROM Results";
            }
            return sql;
        }
    }
}
