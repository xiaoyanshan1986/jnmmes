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
using WinClient.VTestDataTransfer;
using WinClient.VTestDataTransfer.Configuration;
using System.Windows.Forms; 

namespace WinClient.VTestDataTransfer
{

    /// <summary>
    /// IV测试数据转到MES数据库中。
    /// </summary>
    public class IVTestDataTransferAction
    {

        public EventHandler<DataTransferFinishedArgs> OnDataTransferFinished;//声明自定义的事件委托，用来执行事件的声明，
        private System.DateTime dMaxUploadTestTime;
        private System.DateTime dNewMaxTestTime;
        public const string EVENT_SOURCE_NAME = "MES.IVTestDataTransfer";
        private string strAccessFullPath ="";
        private string strImageFullPath = "";
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
            double ctm = 0.0;
            bool blTransferDataResult = true;
            string strTransferDataMsg = "";

            dNewMaxTestTime = MaxTestDateTimeFromServer;
            dMaxUploadTestTime = MaxTestDateTimeFromServer;
            List<string> lstImageFiles = new List<string>();
            List<string> lstSqlForInsert = new List<string>();
            List<string> lstSqlForUpdateIVDataDefault = new List<string>();
            try
            {
                
                strAccessFullPath = CommonFun.GetFullFile(element.Path, element.Format);
                strImageFullPath = CommonFun.GetFullPath(element.SourceImagePathRoot, element.SourceImagePathFormat);

                AccessConnectionString = string.Format(element.ConnectionString, strAccessFullPath);
                if(string.IsNullOrEmpty(strAccessFullPath))
                {
                    strTransferDataMsg = string.Format("没有找到对应的IV数据文件{0} ", strAccessFullPath);
                    blTransferDataResult = false;
                }

                DataSet dsIVTestData = new DataSet();
                StringBuilder sbSql = new StringBuilder();
                string sql = string.Empty;
                if(blTransferDataResult)
                {
                    #region //Access 数据库正确
                    //组织查询IV测试数据的SQL语句
                    if (element.Type == IVTestDeviceType.Customer)
                    {
                        sql = element.Sql;
                    }
                    else
                    {
                        sql = GetQueryTestDataSql(element.Type);
                    }
                    //SELECT [Test_Date], [ID], [ModEff], [Rsh], [Rs], [FF], [Isc], [Voc], [Ipm], 
                    //[Vpm], [Pmax], [Temp], [EnvTemp], [TMod], [Insol], [SunRef], [Test_Time] ,[StdIsc1],[Stdsun1],[StdIsc2],[Stdsun2] FROM SunData;
                    sbSql.AppendFormat("SELECT TOP 1 '{2}' AS DeviceNo,a.* FROM ({0}) a WHERE a.TTIME>'{1}' ORDER BY a.TTIME ASC",
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
                            oleAdapter.Fill(dsIVTestData);
                        }
                        oleCon.Close();
                    }
                    #endregion //End Access数据库正确
                }
                DataTable  dt = null;
                //新增IV测试数据

                if (dsIVTestData != null && dsIVTestData.Tables.Count > 0 && dsIVTestData.Tables[0].Rows.Count > 0)
                {
                    #region //Get Access IV测试数据 
                    sql = string.Format(@" select top 1 * from [dbo].[ZWIP_IV_TEST] 
                        where EQUIPMENT_CODE='{0}'and LOT_NUMBER like 'JZ%' and IS_DEFAULT =1 order by TEST_TIME desc ", element.EqpName);

                    string strCalibrateTime ="";
                    string strCalibrationNo ="";
                    dt = DatabaseEx.getDbInstance().getDataTable(sql);
                    if(dt.Rows.Count>0)
                    {
                        strCalibrateTime = dt.Rows[0]["TEST_TIME"].ToString();
                        strCalibrationNo = dt.Rows[0]["LOT_NUMBER"].ToString();
                    }

                    string strInsertSql="";
                    bool isDefault =false;
                    foreach (DataRow dr in dsIVTestData.Tables[0].Rows)
                    {
                        isDefault = false;
                        if (!Convert.ToString(dr["LOT_NUMBER"]).ToUpper().StartsWith("JZ"))
                        {
                            #region not JZ
                            sql = string.Format(@"SELECT a.[LOT_NUMBER],     
                            a.[ROUTE_STEP_NAME],
                            a.STATE_FLAG,
                            b.ATTRIBUTE_NAME,
                            b.ATTRIBUTE_VALUE  
                            FROM [dbo].[WIP_LOT] a  inner join [dbo].[FMM_ROUTE_STEP_ATTR] b
                            on a.ROUTE_STEP_NAME = b.ROUTE_STEP_NAME
                            and a.ROUTE_NAME = b.ROUTE_NAME 
                            where a.LOT_NUMBER ='{0}'
                            and b.ATTRIBUTE_NAME ='IsAllowIVTestData'", Convert.ToString(dr["LOT_NUMBER"]).ToUpper());

                            strTransferDataMsg = strTransferDataMsg + "\n\t" + Convert.ToString(dr["LOT_NUMBER"]).ToUpper();

                            dt = DatabaseEx.getDbInstance().getDataTable(sql);
                            if (dt.Rows.Count > 0)
                            {
                                if (dt.Rows[0]["STATE_FLAG"].ToString()=="8")
                                { 
                                    //批次一定要在等待出站状态下才能把 ISDefault=1
                                    if (bool.TryParse(dt.Rows[0]["ATTRIBUTE_VALUE"].ToString(), out isDefault))
                                    {
                                        if (isDefault == true)
                                        {
                                            lstSqlForUpdateIVDataDefault.Add(Convert.ToString(dr["LOT_NUMBER"]).ToUpper());
                                            sql = string.Format(@"SELECT * FROM dbo.FMM_EQUIPMENT_PRG_RULE WHERE CONTROL_OBJ=7 AND EQUIPMENT_CODE='{0}'", element.EqpName);
                                            dt = DatabaseEx.getDbInstance().getDataTable(sql);
                                            double max = 0.00;
                                            double min = 0.00;
                                            if (dt.Rows.Count > 0)
                                            {
                                                if (Convert.ToDouble(dt.Rows[0]["CONTROL_VALUE"]) > Convert.ToDouble(dt.Rows[1]["CONTROL_VALUE"]))
                                                {
                                                    max = (dt.Rows[0]["CONTROL_VALUE"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["CONTROL_VALUE"]) : 0);
                                                    min = (dt.Rows[1]["CONTROL_VALUE"] != DBNull.Value ? Convert.ToDouble(dt.Rows[1]["CONTROL_VALUE"]) : 0);
                                                    if (min <= Convert.ToDouble(dr["AMBIENTTEMP"]) && Convert.ToDouble(dr["AMBIENTTEMP"]) <= max)
                                                    {
                                                        isDefault = true;
                                                    }
                                                    else
                                                    {
                                                        isDefault = false;
                                                        MessageBox.Show("测试温度超过控制参数设置温度范围,上传数据为无效值，请联系动力！");
                                                    }
                                                }
                                                else
                                                {
                                                    min = (dt.Rows[0]["CONTROL_VALUE"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["CONTROL_VALUE"]) : 0);
                                                    max = (dt.Rows[1]["CONTROL_VALUE"] != DBNull.Value ? Convert.ToDouble(dt.Rows[1]["CONTROL_VALUE"]) : 0);
                                                    if (min <= Convert.ToDouble(dr["AMBIENTTEMP"]) && Convert.ToDouble(dr["AMBIENTTEMP"]) <= max)
                                                    {
                                                        isDefault = true;
                                                    }
                                                    else
                                                    {
                                                        isDefault = false;
                                                        MessageBox.Show("测试温度超过控制参数设置温度范围,上传数据为无效值，请联系动力！");
                                                    }

                                                }

                                            }
                                        }
                                    }
                                }
                            }


                            #region ctm的计算
                            //ctm的计算 

                            DataTable dtCTM = new DataTable();
                            string sqlCTM = string.Format(@"SELECT  t1.LOT_NUMBER ,
                                                                    t1.MATERIAL_CODE ,
                                                                    t2.MAIN_RAW_QTY ,
                                                                    t1.ATTR_1
                                                            FROM    dbo.WIP_LOT AS t1
                                                                    INNER JOIN dbo.FMM_MATERIAL AS t2 ON t2.MATERIAL_CODE = t1.MATERIAL_CODE
                                                            WHERE   t1.LOT_NUMBER = '{0}'", Convert.ToString(dr["LOT_NUMBER"]).ToUpper());
                            dtCTM = DatabaseEx.getDbInstance().getDataTable(sqlCTM);

                            if (dtCTM != null && dtCTM.Rows.Count > 0)
                            {

                                //CTM计算方式 PM*0.156*0.156*1000*mainQTY*18.1/100
                                //组件电池片数
                                double mainQTY = 1.0;
                                double.TryParse(dtCTM.Rows[0][2].ToString(), out mainQTY);
                                //组件的转换率
                                double conversion = 1.0;

                                string attr = dtCTM.Rows[0][3].ToString();

                                if (attr.Contains("%"))
                                {
                                    attr = attr.Substring(0, attr.Length - 1);
                                }
                                double.TryParse(attr, out conversion);

                                double pm = 1.0;
                                double.TryParse(dr["PM"].ToString(), out pm);

                                ctm = pm / (0.156 * 0.156 * 1000 * mainQTY * conversion / 100);
                                ctm = Math.Round(ctm, 4);
                            }
                            else
                            {
                                ctm = 0.0;
                            }
                            #endregion
                            #endregion
                        }
                        else
                        {
                            #region JZ
                            if (Convert.ToString(dr["LOT_NUMBER"]).ToUpper().Length<11)
                            {
                                isDefault = false;
                            }
                            else
                            {
                                string testLot = Convert.ToString(dr["LOT_NUMBER"]).ToUpper().Substring(0, 11);
                                double testISC = (dr["ISC"] != DBNull.Value ? Convert.ToDouble(dr["ISC"]) : 0);
                                double testVOC = (dr["VOC"] != DBNull.Value ? Convert.ToDouble(dr["VOC"]) : 0);
                                double testPM = (dr["PM"] != DBNull.Value ? Convert.ToDouble(dr["PM"]) : 0);
                                double testAmbientTemperature = (dr["AMBIENTTEMP"] != DBNull.Value ? Convert.ToDouble(dr["AMBIENTTEMP"]) : 0);
                               
                                sql = string.Format(@"SELECT 
	                                       [CALIBRATION_PLATE_ID]
                                          ,[MAX_PM]
                                          ,[MIN_PM]
                                          ,[MAX_ISC]
                                          ,[MIN_ISC]
                                          ,[MAX_VOC]
                                          ,[MIN_VOC]
                                      FROM [dbo].[FMM_CALIBRATION_PLATE]
                                      WHERE  CALIBRATION_PLATE_ID='{0}'",testLot);

                                strTransferDataMsg = strTransferDataMsg + "\n\t" + Convert.ToString(dr["LOT_NUMBER"]).ToUpper();
                                dt = DatabaseEx.getDbInstance().getDataTable(sql);
                                if (dt.Rows.Count > 0)
                                {
                                    double maxPM = (dt.Rows[0]["MAX_PM"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["MAX_PM"]) : 0);
                                    double minPM = (dt.Rows[0]["MIN_PM"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["MIN_PM"]) : 0);
                                    double maxISC = (dt.Rows[0]["MAX_ISC"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["MAX_ISC"]) : 0);
                                    double minISC = (dt.Rows[0]["MIN_ISC"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["MIN_ISC"]) : 0);
                                    double maxVOC = (dt.Rows[0]["MAX_VOC"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["MAX_VOC"]) : 0);
                                    double minVOC = (dt.Rows[0]["MIN_VOC"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["MIN_VOC"]) : 0);
                                  
                                    if (minPM <testPM && testPM <maxPM && minISC <testISC && testISC< maxISC && minVOC < testVOC &&testVOC< maxVOC)
                                    {
                                        
                                        //isDefault = true;
                                        sql = string.Format(@"SELECT * FROM dbo.FMM_EQUIPMENT_PRG_RULE WHERE CONTROL_OBJ=7 AND EQUIPMENT_CODE='{0}'", element.EqpName);
                                        dt = DatabaseEx.getDbInstance().getDataTable(sql);
                                        double max = 0.00;
                                        double min = 0.00;
                                        if (dt.Rows.Count > 0)
                                        {
                                            if (Convert.ToDouble(dt.Rows[0]["CONTROL_VALUE"]) > Convert.ToDouble(dt.Rows[1]["CONTROL_VALUE"]))
                                            {
                                                max = (dt.Rows[0]["CONTROL_VALUE"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["CONTROL_VALUE"]) : 0);
                                                min = (dt.Rows[1]["CONTROL_VALUE"] != DBNull.Value ? Convert.ToDouble(dt.Rows[1]["CONTROL_VALUE"]) : 0);
                                                if (min <= Convert.ToDouble(dr["AMBIENTTEMP"]) && Convert.ToDouble(dr["AMBIENTTEMP"]) <= max)
                                                {
                                                    isDefault = true;
                                                }
                                                else
                                                {
                                                    isDefault = false;
                                                    MessageBox.Show("测试温度超过控制参数设置温度范围,上传数据为无效值，请联系动力！");

                                                }
                                            }
                                            else
                                            {
                                                min = (dt.Rows[0]["CONTROL_VALUE"] != DBNull.Value ? Convert.ToDouble(dt.Rows[0]["CONTROL_VALUE"]) : 0);
                                                max = (dt.Rows[1]["CONTROL_VALUE"] != DBNull.Value ? Convert.ToDouble(dt.Rows[1]["CONTROL_VALUE"]) : 0);
                                                if (min <= Convert.ToDouble(dr["AMBIENTTEMP"]) && Convert.ToDouble(dr["AMBIENTTEMP"]) <= max)
                                                {
                                                    isDefault = true;
                                                }
                                                else
                                                {
                                                    isDefault = false;
                                                    MessageBox.Show("测试温度超过控制参数设置温度范围,上传数据为无效值，请联系动力！");
                                                }

                                            }

                                        }
                                    }
                                    else
                                    {
                                        isDefault = false;
                                    }
                                    
                                }
                            }
                            #endregion
                        }

                        #region Builder Insert Sql

                        lstImageFiles.Add(Convert.ToString(dr["LOT_NUMBER"]).ToUpper());

                        strInsertSql =@" INSERT INTO [dbo].[ZWIP_IV_TEST](
                            [LOT_NUMBER],[TEST_TIME],[EQUIPMENT_CODE],[PM]
                            ,[ISC],[IPM],[VOC],[VPM]
                            ,[FF],[EFF],[RS],[RSH]
                            ,[AMBIENTTEMP],[SENSORTEMP],[INTENSITY]
                            ,[COEF_PMAX]
                            ,[COEF_ISC],[COEF_VOC],[COEF_IMAX],[COEF_VMAX]
                            ,[COEF_FF],[DEC_CTM],[PS_CODE],[PS_ITEM_NO]
                            ,[PS_SUBCODE],[IS_DEFAULT],[IS_PRINT],[PRINT_TIME]
                            ,[PRINT_COUNT],[CALIBRATE_TIME],[CALIBRATION_NO]
                            ,[STDISC1],[STDSUN1],[STDISC2],[STDSUN2],[CREATOR],[CREATE_TIME],[EDITOR],[EDIT_TIME]) values (";

                            strInsertSql = strInsertSql    + "'" + Convert.ToString(dr["LOT_NUMBER"]).ToUpper() + "'," ;
                            strInsertSql = strInsertSql    + "'" + Convert.ToString(dr["TTIME"]).ToUpper() + "',";

                            if (DateTime.TryParse(dr["TTIME"].ToString(), out dNewMaxTestTime) == false)
                            {
                                blTransferDataResult =false;
                                strTransferDataMsg ="IV数据生成的日期格式不正确，请联系设备厂商。";
                                break;
                            }

                            strInsertSql = strInsertSql    + "'" + element.EqpName + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["PM"] != DBNull.Value ? Convert.ToDouble(dr["PM"]) : 0) + "',";

                            // ,[ISC],[IPM],[VOC],[VPM]
                            strInsertSql = strInsertSql    + "'" + ( dr["ISC"] != DBNull.Value ? Convert.ToDouble(dr["ISC"]) : 0) + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["IPM"] != DBNull.Value ? Convert.ToDouble(dr["IPM"]) : 0) + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["VOC"] != DBNull.Value ? Convert.ToDouble(dr["VOC"]) : 0) + "',";                            
                            strInsertSql = strInsertSql    + "'" + ( dr["VPM"] != DBNull.Value ? Convert.ToDouble(dr["VPM"]) : 0) + "',"; 

                            //[FF],[EFF],[RS],[RSH]
                            strInsertSql = strInsertSql    + "'" + ( dr["FF"]!=DBNull.Value ? Convert.ToDouble(dr["FF"]) : 0) + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["EFF"] != DBNull.Value ? Convert.ToDouble(dr["EFF"]) : 0) + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["RS"] != DBNull.Value ? Convert.ToDouble(dr["RS"]) : 0)  + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["RSH"] != DBNull.Value ? Convert.ToDouble(dr["RSH"]) : 0) + "',";

                            //,[AMBIENTTEMP],[SENSORTEMP],[INTENSITY]
                            strInsertSql = strInsertSql    + "'" + ( dr["AMBIENTTEMP"] != DBNull.Value ? Convert.ToDouble(dr["AMBIENTTEMP"]) : 0) + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["SENSORTEMP"] != DBNull.Value ? Convert.ToDouble(dr["SENSORTEMP"]) : 0) + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["INTENSITY"] != DBNull.Value ? Convert.ToDouble(dr["INTENSITY"]) : 0)  + "',";
                            
                            // ,[COEF_PMAX]
                            strInsertSql = strInsertSql    + "'" + ( dr["PM"] != DBNull.Value ? Convert.ToDouble(dr["PM"]) : 0) + "',";

                            //,[COEF_ISC],[COEF_VOC],[COEF_IMAX],[COEF_VMAX]
                            strInsertSql = strInsertSql    + "'" + ( dr["ISC"] != DBNull.Value ? Convert.ToDouble(dr["ISC"]) : 0) + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["VOC"] != DBNull.Value ? Convert.ToDouble(dr["VOC"]) : 0) + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["IPM"] != DBNull.Value ? Convert.ToDouble(dr["IPM"]) : 0) + "',";
                            strInsertSql = strInsertSql    + "'" + ( dr["VPM"] != DBNull.Value ? Convert.ToDouble(dr["VPM"]) : 0) + "',";
                            // ,[COEF_FF],
                            strInsertSql = strInsertSql     + "'" + ( dr["FF"]!=DBNull.Value ? Convert.ToDouble(dr["FF"]) : 0) + "',";

                            //[DEC_CTM],[PS_CODE],[PS_ITEM_NO]
                            string _ctm = ctm.ToString();
                            //select ATTR_1 from WIP_LOT where LOT_NUMBER='JN2230937214299'
                            strInsertSql = strInsertSql + "'" + _ctm + "',";//修改这里
                            strInsertSql = strInsertSql     + "NULL,";
                            strInsertSql = strInsertSql     + "NULL,";

                            //,[PS_SUBCODE],[IS_DEFAULT],[IS_PRINT],[PRINT_TIME]
                            strInsertSql = strInsertSql     + "NULL,";
                            if(isDefault)
                            { 
                                strInsertSql = strInsertSql + "'1',";
                            }
                            else
                            {
                                strInsertSql = strInsertSql + "'0',";
                            }
                            strInsertSql = strInsertSql     + "'0',";
                            strInsertSql = strInsertSql     + "NULL,";

                            // ,[PRINT_COUNT],[CALIBRATE_TIME],[CALIBRATION_NO],
                            strInsertSql = strInsertSql      + "'0',";
                            strInsertSql = strInsertSql      + "'" + strCalibrateTime + "',";
                            strInsertSql = strInsertSql      + "'" + strCalibrationNo + "',";
                            //,[STDISC1],[STDSUN1],[STDISC2],[STDSUN2]
                            strInsertSql = strInsertSql + "'" + (dr["STDISC1"] != DBNull.Value ? Convert.ToDouble(dr["STDISC1"]) : 0) + "',";
                            strInsertSql = strInsertSql + "'" + (dr["STDSUN1"] != DBNull.Value ? Convert.ToDouble(dr["STDSUN1"]) : 0) + "',";
                            strInsertSql = strInsertSql + "'" + (dr["STDISC2"] != DBNull.Value ? Convert.ToDouble(dr["STDISC2"]) : 0) + "',";
                            strInsertSql = strInsertSql + "'" + (dr["STDSUN2"] != DBNull.Value ? Convert.ToDouble(dr["STDSUN2"]) : 0) + "',";
                            
                            //,[CREATOR],[CREATE_TIME],[EDITOR],[EDIT_TIME],
                            string strDateTime = System.DateTime.Now.ToString();
                            strInsertSql = strInsertSql + "'system',";
                            strInsertSql = strInsertSql      + "'" + strDateTime + "',";
                            strInsertSql = strInsertSql + "'system',";
                            strInsertSql = strInsertSql      + "'" + strDateTime + "'";
                            

                            strInsertSql = strInsertSql      + ")";
                            lstSqlForInsert.Add(strInsertSql);

                            strInsertSql="";
                            isDefault = false;
                            #endregion

                    }

                    sql = "";
                    if(blTransferDataResult)
                    {
                        if(lstSqlForUpdateIVDataDefault!=null && lstSqlForUpdateIVDataDefault.Count>0)
                        {
                            string strWhere ="";
                            foreach(string lotNumber in lstSqlForUpdateIVDataDefault)
                            {
                                strWhere =  "'" + lotNumber + "',";
                            }
                            if(strWhere.Length>0)
                            {
                                strWhere= strWhere.Substring(0,strWhere.Length-1).Trim();
                            }
                            sql = " update ZWIP_IV_TEST set IS_DEFAULT=0 where IS_DEFAULT=1 and LOT_NUMBER in(" + strWhere + ")";
                            lstSqlForInsert.Insert(0, sql);
                        }
                        string strExeResult = DatabaseEx.getDbInstance().ExecuteNonQuery(lstSqlForInsert);
                        if(strExeResult!="" && strExeResult.Length>0)
                        {
                            blTransferDataResult = false;
                            strTransferDataMsg = strExeResult;
                        }
                        if(blTransferDataResult)
                        {
                            #region //上传Image图片
                            try
                            {
                                foreach (string strLot in lstImageFiles)
                                {
                                    //Upload Image Files
                                    string strFileName = strImageFullPath + strLot + "." + element.ImageExtensionName;
                                    if (File.Exists(strFileName))
                                    {
                                        FileInfo fInfo = new FileInfo(strFileName);
                                        FtpManager.UploadFile(fInfo, element.FtpTargetFolder, element.FtpServer, element.FtpUser, element.FtpPassword);
                                    }
                                }
                            }
                            catch(Exception exe)
                            {
                                string s = exe.Message;
                            }
                            #endregion
                        }
                    }
                    #endregion //End Get Access IV测试数据
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
            arg.MaxTestDateTime = dNewMaxTestTime;

            if (blTransferDataResult)
            {
                strTransferDataMsg = string.Format("{1:yyyy-MM-dd HH:mm:ss}上传IV数据成功>{0} ", strTransferDataMsg, System.DateTime.Now.ToString());
            }
            else
            {
                strTransferDataMsg = string.Format("{1:yyyy-MM-dd HH:mm:ss}上传IV数据失败>{0} ", strTransferDataMsg, System.DateTime.Now.ToString());
                //arg.MaxTestDateTime = dMaxUploadTestTime;
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
            //--STDISC1 标准电流1
            //--STDSUN1 标准光强1
            //--STDSUN2 标准电流2
            //--STDSUN2 标准光强2
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
	                        SunData.[Sun] AS INTENSITY,
                            SunData.[StdIsc1] AS STDISC1,
                            SunData.[Stdsun1] AS STDSUN1,
                            SunData.[StdIsc2] AS STDISC2,
                            SunData.[Stdsun2] AS STDSUN2
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
	                        Results.CIrr AS INTENSITY,
                            SunData.[StdIsc1] AS STDISC1,
                            SunData.[Stdsun1] AS STDSUN1,
                            SunData.[StdIsc2] AS STDISC2,
                            SunData.[Stdsun2] AS STDSUN2
                        FROM Results";
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