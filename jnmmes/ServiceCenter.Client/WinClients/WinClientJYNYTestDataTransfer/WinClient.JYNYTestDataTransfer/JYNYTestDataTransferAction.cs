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
using WinClient.JYNYTestDataTransfer;
using WinClient.JYNYTestDataTransfer.Configuration;

namespace WinClient.JYNYTestDataTransfer
{

    /// <summary>
    /// IV测试数据转到MES数据库中。
    /// </summary>
    public class JYNYTestDataTransferAction
    {

        public EventHandler<DataTransferFinishedArgs> OnDataTransferFinished;//声明自定义的事件委托，用来执行事件的声明，
        private System.DateTime dMaxUploadTestTime;
        private System.DateTime dNewMaxTestTime;
        public const string EVENT_SOURCE_NAME = "MES.JYNYTestDataTransfer";
        private string strAccessFullPath = "";
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="accessConnString">Access数据库连接字符串。</param>
        public JYNYTestDataTransferAction(string accessConnString)
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
        public void Execute(JYNYTestDeviceElement element, System.DateTime MaxTestDateTimeFromServer)
        {

            bool blTransferDataResult = true;
            string strTransferDataMsg = "";

            dNewMaxTestTime = MaxTestDateTimeFromServer;
            dMaxUploadTestTime = MaxTestDateTimeFromServer;
            List<string> lstSqlForInsert = new List<string>();
            List<string> lstSqlForUpdateEFCDataDefault = new List<string>();
            try
            {

                strAccessFullPath = CommonFun.GetFullFile(element.Path, element.Format);
                AccessConnectionString = string.Format(element.ConnectionString, strAccessFullPath);
                if (string.IsNullOrEmpty(strAccessFullPath))
                {
                    strTransferDataMsg = string.Format("没有找到对应的安规数据文件{0} ", strAccessFullPath);
                    blTransferDataResult = false;
                }

                DataSet dsJYNYTestData = new DataSet();
                StringBuilder sbSql = new StringBuilder();
                string sql = string.Empty;
                if (blTransferDataResult)
                {
                    #region //Access 数据库正确
                    //组织查询JYNY测试数据的SQL语句
                    if (element.Type == JYNYTestDeviceType.Customer)
                    {
                        sql = element.Sql;
                    }
                    else
                    {
                        sql = GetQueryTestDataSql(element.Type);
                    }

                    //SELECT Format(data.[日期],'yyyy-MM-dd ')+Format(data.[时间],'HH:mm:ss') AS TEST_TIME,data.[ID] AS TEST_ID,data.[产品条码] AS LOT_NUMBER,data.[测试结果] AS TEST_RESULT,
                    //  data.[是否测试] AS TEST_FLAG,
                    //data.[步骤] AS TEST_STEP_SEQ,data.[测试类型] AS TEST_TYPE,data.[步骤判断] AS TEST_STEP_RESULT,data.[测试数据1] AS TEST_PARAM1,data.[测试数据2] AS TEST_PARAM2,data.[PROMPT],
                    //  data.[VOLTAGE],data.[FREQUENCY],data.[FREQUENCY],data.[HILIMIT],data.[LOLIMIT],data.[RAMPUP],data.[DWELLTIME],data.[DELAYTIME],data.[RAMPHI],data.[CHARGELO],data.[OFFSET], 
                    //data.[ARCSENSE],data.[ARCFAIL],data.[SCANNER]  FROM data
                    sbSql.AppendFormat("SELECT TOP 1 '{2}' AS EQUIPMENT_CODE,a.* FROM ({0}) a WHERE a.TEST_TIME>'{1}' ORDER BY a.TEST_TIME ASC",
                                        sql,
                                        MaxTestDateTimeFromServer.ToString("yyyy-MM-dd HH:mm:ss"),
                                        element.EqpName);

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
                            oleAdapter.Fill(dsJYNYTestData);
                        }
                        oleCon.Close();
                    }
                    #endregion //End Access数据库正确
                }
                DataTable dt = null;
                //新增IV测试数据
                if (dsJYNYTestData != null && dsJYNYTestData.Tables.Count > 0 && dsJYNYTestData.Tables[0].Rows.Count > 0)
                {
                    #region //Get Access JYNY测试数据
                    string strInsertSql = "";

                    bool isDefault = false;


                    foreach (DataRow dr in dsJYNYTestData.Tables[0].Rows)
                    {

                        #region Builder Insert Sql
                        strInsertSql = @"INSERT INTO [dbo].[ZWIP_VIR_TEST]
                                                       ([EQUIPMENT_CODE]
                                                       ,[TEST_ID]
                                                       ,[TEST_TIME]
                                                       ,[LOT_NUMBER]
                                                       ,[TEST_RESULT]
                                                       ,[TEST_FLAG]
                                                       ,[TEST_STEP_SEQ]
                                                       ,[TEST_TYPE]
                                                       ,[TEST_STEP_RESULT]
                                                       ,[TEST_PARAM1]
                                                       ,[TEST_PARAM2]
                                                       ,[PROMPT]
                                                       ,[VOLTAGE]
                                                       ,[FREQUENCY]
                                                       ,[ECURREN]
                                                       ,[HILIMIT]
                                                       ,[LOLIMIT]
                                                       ,[RAMPUP]
                                                       ,[DWELLTIME]
                                                       ,[DELAYTIME]
                                                       ,[RAMPHI]
                                                       ,[CHARGELO]
                                                       ,[OFFSET]
                                                       ,[ARCSENSE]
                                                       ,[ARCFAIL]
                                                       ,[SCANNER])
                                                 VALUES
                                                       (";

                        strInsertSql = strInsertSql + "'" + element.EqpName + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["TEST_ID"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["TEST_TIME"]).ToUpper() + "',";

                        if (DateTime.TryParse(dr["TEST_TIME"].ToString(), out dNewMaxTestTime) == false)
                        {
                            blTransferDataResult = false;
                            strTransferDataMsg = "安规数据生成的日期格式不正确，请联系设备厂商。";
                            return;
                        }
                        String LOT_NUMBER = Convert.ToString(dr["LOT_NUMBER"]).ToUpper();
                        if (LOT_NUMBER.Contains("'"))
                        {
                            LOT_NUMBER = LOT_NUMBER.Replace("'", "''");
                        }

                        strInsertSql = strInsertSql + "'" + LOT_NUMBER + "',";


                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["TEST_RESULT"]).ToUpper() + "',";
                        if (Convert.ToString(dr["TEST_FLAG"]).ToUpper() == "Y")
                        {
                            strInsertSql = strInsertSql + " " + 1 + ", ";
                        }
                        else
                        {
                            strInsertSql = strInsertSql + " " + 0 + ", ";
                        }
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["TEST_STEP_SEQ"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["TEST_TYPE"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["TEST_STEP_RESULT"]).ToUpper() + "',";

                        //,[TEST_PARAM1],[TEST_PARAM2],[PROMPT],[VOLTAGE],[FREQUENCY],[ECURREN],[HILIMIT],[LOLIMIT]
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["TEST_PARAM1"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["TEST_PARAM2"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["PROMPT"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["VOLTAGE"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["FREQUENCY"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["ECURREN"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["HILIMIT"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["LOLIMIT"]).ToUpper() + "',";

                        //[RAMPUP] ,[DWELLTIME],[DELAYTIME],[RAMPHI],[CHARGELO],[OFFSET],[ARCSENSE],[ARCFAIL],[SCANNER]
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["RAMPUP"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["DWELLTIME"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["DELAYTIME"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["RAMPHI"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["CHARGELO"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["OFFSET"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["ARCSENSE"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["ARCFAIL"]).ToUpper() + "',";
                        strInsertSql = strInsertSql + "'" + Convert.ToString(dr["SCANNER"]).ToUpper() + "'";
                        strInsertSql = strInsertSql + ")";

                        //数据在数据库中是否已有（LOT_NUMBER\TEST_TIME\EQUIPMENT_CODE）
                        string TEST_ID = "";
                        TEST_ID = "select TEST_ID from ZWIP_VIR_TEST where TEST_ID='" + dr["TEST_ID"] + "'" + "and EQUIPMENT_CODE ='" + element.EqpName + "'";

                        if (TEST_ID != dr["TEST_ID"])
                        {
                            lstSqlForInsert.Add(strInsertSql);
                        }

                        strInsertSql = "";
                        isDefault = false;
                        #endregion

                    }

                    sql = "";
                    if (blTransferDataResult)
                    {
                        string strExeResult = DatabaseEx.getDbInstance().ExecuteNonQuery(lstSqlForInsert);
                        strInsertSql = "";
                        if (strExeResult != "" && strExeResult.Length > 0)
                        {
                            blTransferDataResult = false;
                            strTransferDataMsg = strExeResult;
                        }
                    }
                    #endregion //End Get Access JYNY测试数据
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
                lstSqlForUpdateEFCDataDefault.Clear();
            }


            DataTransferFinishedArgs arg = new DataTransferFinishedArgs();
            arg.TransferDataResult = blTransferDataResult;
            arg.TransferDbFile = strAccessFullPath;
            if (blTransferDataResult)
            {
                strTransferDataMsg = string.Format("{1:yyyy-MM-dd HH:mm:ss}上传安规数据数据成功>{0} ", strTransferDataMsg, System.DateTime.Now.ToString());
                arg.MaxTestDateTime = dNewMaxTestTime;
            }
            else
            {
                strTransferDataMsg = string.Format("{1:yyyy-MM-dd HH:mm:ss}上传安规数据数据失败>{0} ", strTransferDataMsg, System.DateTime.Now.ToString());
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
        private string GetQueryTestDataSql(JYNYTestDeviceType type)
        {
            string sql = string.Empty;
            if (type == JYNYTestDeviceType.baccini_table)
            {
                sql = @"SELECT Format(data.[日期],'yyyy-MM-dd ')+Format(data.[时间],'HH:mm:ss') AS TEST_TIME,
            	      data.[ID] AS TEST_ID,
	                  data.[产品条码] AS LOT_NUMBER,
	                  data.[测试结果] AS TEST_RESULT,
	                  data.[是否测试] AS TEST_FLAG,
	                  data.[步骤] AS TEST_STEP_SEQ,
	                  data.[测试类型] AS TEST_TYPE,
	                  data.[步骤判断] AS TEST_STEP_RESULT,
	                  data.[测试数据1] AS TEST_PARAM1,
	                  data.[测试数据2] AS TEST_PARAM2,
	                  data.[PROMPT],
	                  data.[VOLTAGE], 
                      data.[FREQUENCY],
	                  data.[FREQUENCY],
	                  data.[HILIMIT],
	                  data.[LOLIMIT],
	                  data.[RAMPUP], 
                      data.[DWELLTIME],
                      data.[DELAYTIME],
                      data.[RAMPHI], 
                      data.[CHARGELO],
                      data.[OFFSET], 
                      data.[ARCSENSE],
                      data.[ARCFAIL],
                      data.[SCANNER]
                  FROM data";
            }

            return sql;
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