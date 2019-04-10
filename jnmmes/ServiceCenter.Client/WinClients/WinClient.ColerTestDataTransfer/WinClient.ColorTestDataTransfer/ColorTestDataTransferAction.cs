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
using WinClient.ColorTestDataTransfer;
using WinClient.ColorTestDataTransfer.Configuration;
using WinClient.ColorTestDataTransfer.libs;

namespace WinClient.ColorTestDataTransfer
{

    /// <summary>
    /// IV测试数据转到MES数据库中。
    /// </summary>
    public class ColorTestDataTransferAction
    {
        static string connHost = System.Configuration.ConfigurationManager.ConnectionStrings["connHost"].ConnectionString;
        static string connClient = System.Configuration.ConfigurationManager.ConnectionStrings["connClient"].ConnectionString;

        public EventHandler<DataTransferFinishedArgs> OnDataTransferFinished;//声明自定义的事件委托，用来执行事件的声明，
        private System.DateTime dMaxUploadTestTime;
        private System.DateTime dNewMaxTestTime;
        public const string EVENT_SOURCE_NAME = "MES.IVTestDataTransfer";
        private string strAccessFullPath ="";
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="accessConnString">Access数据库连接字符串。</param>
        public ColorTestDataTransferAction(string accessConnString)
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

        public System.DateTime MaxUploadTestTime
        {
            get
            {
                return dMaxUploadTestTime;
            }
            private set
            {
                dMaxUploadTestTime = value;
            }
        }
        
        /// <summary>
        /// 将数据从Access数据库转移到MES数据库。
        /// </summary>
        /// <param name="element">设备配置。</param>
        /// <returns>true:转置成功。false：转置失败。</returns>
        public void Execute(IVTestDeviceElement element,System.DateTime MaxTestDateTimeFromServer)
        {
           
            bool blTransferDataResult = true;
            string strTransferDataMsg = "";

            dNewMaxTestTime = MaxTestDateTimeFromServer;
            dMaxUploadTestTime = MaxTestDateTimeFromServer;
            List<string> lstSqlForInsert = new List<string>();
            List<string> lstSqlForUpdateIVDataDefault = new List<string>();
            try
            {
                String _day = SqlHelper.ExecuteDataTable("select max(InspectTime) from ZWIP_COROL_TEST", connHost).Rows[0][0].ToString();

                string _str = "select top 1 [LotSN] ,[InspectTime] ,[InspectResult] ,[InspectValue],[BlueValue],[Operator],[Shift],[RcpName],[LocalIp],[DeviceID] from t_ColorInspectResult  where InspectTime > '" + _day + "'order by [InspectTime] ";
                DataTable d1t = SqlHelper.ExecuteDataTable(_str, connClient);
                if (d1t.Rows.Count > 0)
                {
                }
                else {
                    strTransferDataMsg = string.Format("没有找到对应的COLOR数据文件{0} ", strAccessFullPath);
                    blTransferDataResult = false;
                }               
                DataTable  dt = null;
                
                if (blTransferDataResult)
                {
                    String _keysql ="select count(*) from ZWIP_COROL_TEST where LOT_NUMBER ='" + d1t.Rows[0]["LotSN"].ToString() + "'and InspectTime='" + d1t.Rows[0]["InspectTime"].ToString() + "'";
                    int _key = Convert.ToInt32(SqlHelper.ExecuteDataTable(_keysql, connHost).Rows[0][0]);
                    if (_key == 0)
                    {
                        #region Builder Insert Sql
                        StringBuilder sbsql = new StringBuilder();
                        sbsql.Append("INSERT INTO [ZWIP_COROL_TEST] ([LOT_NUMBER] ,[InspectTime] ,[InspctResult],[InspectValue] ,[BlueValue]  ,[Opetator] ,[Shift] ,[RcpName]  ,[LocalIp],[DeviceID] ,[DataTime])");
                        sbsql.Append("VALUES ( ");
                        sbsql.Append("'" + d1t.Rows[0]["LotSN"].ToString() + "',");
                        sbsql.Append("'" + d1t.Rows[0]["InspectTime"].ToString() + "',");
                        sbsql.Append("'" + d1t.Rows[0]["InspectResult"].ToString() + "',");
                        sbsql.Append("'" + d1t.Rows[0]["InspectValue"].ToString() + "',");
                        sbsql.Append("'" + d1t.Rows[0]["BlueValue"].ToString() + "',");
                        sbsql.Append("'" + d1t.Rows[0]["Operator"].ToString() + "',");
                        sbsql.Append("'" + d1t.Rows[0]["Shift"].ToString() + "',");
                        sbsql.Append("'" + d1t.Rows[0]["RcpName"].ToString() + "',");
                        sbsql.Append("'" + d1t.Rows[0]["LocalIp"].ToString() + "',");
                        sbsql.Append("'" + d1t.Rows[0]["DeviceID"].ToString() + "',");
                        sbsql.Append("'" + DateTime.Now + "'");
                        sbsql.Append(" )");

                        string _str1 = sbsql.ToString();

                        SqlHelper.ExecuteNonQuery(_str1, connHost);
                        sbsql.Clear();
                        //isDefault = false;
                        #endregion
                    }
                    else
                    {
                        strTransferDataMsg = string.Format("{1:yyyy-MM-dd HH:mm:ss}数据已存在>{0} ", strTransferDataMsg, System.DateTime.Now.ToString());
                    }
                    
                
                }
            }
            catch (Exception ex)
            {
                blTransferDataResult = false;
                strTransferDataMsg = ex.Message;                
            }
            finally
            {
                lstSqlForInsert.Clear();
                lstSqlForUpdateIVDataDefault.Clear();
            }


            DataTransferFinishedArgs arg = new DataTransferFinishedArgs();
            arg.TransferDataResult = blTransferDataResult;
            arg.TransferDbFile = strAccessFullPath;
            if (blTransferDataResult)
            {
                strTransferDataMsg = string.Format("{1:yyyy-MM-dd HH:mm:ss}上传Coler数据成功>{0} ", strTransferDataMsg, System.DateTime.Now.ToString());
                arg.MaxTestDateTime = dNewMaxTestTime;
            }
            else
            {
                strTransferDataMsg = string.Format("{1:yyyy-MM-dd HH:mm:ss}上传Coler数据失败>{0} ", strTransferDataMsg, System.DateTime.Now.ToString());
                arg.MaxTestDateTime = dMaxUploadTestTime;
            }
            arg.TransferMsg = strTransferDataMsg;

            if (OnDataTransferFinished != null)
            {
                OnDataTransferFinished(this, arg);
            }
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
                sql = @"SELECT [ID]
                      ,[LotSN]
                      ,[InspectTime]
                      ,[InspectResult]
                      ,[InspectValue]
                      ,[BlueValue]
                      ,[Operator]
                      ,[Shift]
                      ,[RcpName]
                      ,[LocalIp]
                      ,[DeviceID]
                  FROM [t_ColorInspectResult]";
            }
            else if (type == IVTestDeviceType.Results)
            {
                sql = @"SELECT [ID]
                      ,[LotSN]
                      ,[InspectTime]
                      ,[InspectResult]
                      ,[InspectValue]
                      ,[BlueValue]
                      ,[Operator]
                      ,[Shift]
                      ,[RcpName]
                      ,[LocalIp]
                      ,[DeviceID]
                  FROM [t_ColorInspectResult]";
            }
            return sql;
        }
    }

    public class DataTransferFinishedArgs:EventArgs
    {
        private string strTransferMsg = "";
        private string strTransferDbFile = "";
        private bool blTransferDataResult = false;
        private DateTime dMaxTestDateTime;


        public DataTransferFinishedArgs()
        {
           
        }
        public DataTransferFinishedArgs(bool TransferDataResult, string transferMsg , DateTime maxTestDateTime)
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