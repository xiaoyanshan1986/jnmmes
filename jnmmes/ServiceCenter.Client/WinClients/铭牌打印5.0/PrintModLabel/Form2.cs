using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IV.Common;

using System.Data.SqlClient;
using IV.SQLServerDAL;
using System.Diagnostics;
using LabelManager2;
using System.Collections;


namespace PrintModLabel
{
    public partial class Form2 : Form
    {
        private Hashtable hsLabels = new Hashtable();
        public Form2()
        {
            InitializeComponent();
        }
        DataSet ds_LOT;
        DataSet ds_iv1;
        /// <summary>
        /// 判断子档位
        /// </summary>
        /// <param name="_lot"></param>
        /// <returns></returns>
        private System.Drawing.Image getImageDataFromOracle(string _lot)
        {
            //通过lotnumber得到bom和组件编号
            string LOTSQL1 = "select   COLOR,GRADE,ORG_ORDER_NUMBER,ORDER_NUMBER,MATERIAL_CODE  from WIP_LOT where LOT_NUMBER='" + _lot + "' ";
            ds_LOT = SQLServerDALServer.Query(LOTSQL1);
            if (ds_LOT.Tables[0].Rows.Count > 0)
            {
                DataRow lotRow = ds_LOT.Tables[0].Rows[0];
                //通过组件批次号 查询到bom 通过bom查询 iv_test查询功率 和电流 CoefPM
                //string _ivDataSQL1 = "select  COEF_PMAX,Coef_ISC,Coef_VOC,COEF_IMAX,COEF_VMAX,PS_CODE,PS_ITEM_NO,PS_SUBCODE from ZWIP_IV_TEST where  IS_DEFAULT=1 and PS_SUBCODE is not null  and LOT_NUMBER='" + _lot + "' order by Test_Time Desc ";
                //string _ivDataSQL1 = "select * from ZWIP_IV_TEST where  IS_DEFAULT=1 and PS_SUBCODE is not null  and LOT_NUMBER='" + _lot + "' order by Test_Time Desc ";

                string _ivDataSQL1 = "select * from ZWIP_IV_TEST where  IS_DEFAULT=1 and LOT_NUMBER='" + _lot + "' order by Test_Time Desc ";

                ds_iv1 = SQLServerDALServer.Query(_ivDataSQL1);
                //string _IVPRINTLOG = "select LOT_NUMBER,TEST_TIME,EQUIPMENT_CODE from ZWIP_IV_TEST_PRINTLOG where LOT_NUMBER='" + txtContent.Text.Trim().ToUpper() + "' and TEST_TIME='" + ds_iv1.Tables[0].Rows[0]["TEST_TIME"].ToString() + "' and EQUIPMENT_CODE='" + ds_iv1.Tables[0].Rows[0]["EQUIPMENT_CODE"].ToString() + "' and ITEM_NO=1";

                if (ds_iv1.Tables[0].Rows.Count > 0)
                {
                    string sql = "";
                    int _pm = getPMFromWorkOrderRule(lotRow["ORDER_NUMBER"].ToString(), lotRow["MATERIAL_CODE"].ToString(), float.Parse(ds_iv1.Tables[0].Rows[0]["COEF_PMAX"].ToString()));
                    if (_pm == null || _pm == 0)
                    {
                        _pm = PMShowMsg(float.Parse(ds_iv1.Tables[0].Rows[0]["COEF_PMAX"].ToString()));
                    }
                    sql = @" select Picture from ZPPM_WORK_ORDER_PRD_POWERSET_DETAIL 
                            where PS_CODE='" + ds_iv1.Tables[0].Rows[0]["PS_CODE"].ToString() + "'"
                        //+ " and ITEM_NO='" + ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString() + "'"
                            + " and PS_SUBCODE='" + ds_iv1.Tables[0].Rows[0]["PS_SUBCODE"].ToString() + "'"
                            + " and ORDER_NUMBER='" + lotRow["ORDER_NUMBER"].ToString() + "'"
                            + " and MATERIAL_CODE='" + lotRow["MATERIAL_CODE"].ToString() + "'";

                    if (!string.IsNullOrEmpty(ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString()))
                    {
                        sql += " and ITEM_NO=" + ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString();
                    }

                    byte[] fileData = (byte[])SQLServerDALServer.GetSingle(sql);
                    if (fileData != null)
                    {
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(fileData);
                        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                        return img;
                    }
                    sql = "select Picture from ZFMM_POWERSET_DETAIL  where PS_CODE='" + ds_iv1.Tables[0].Rows[0]["PS_CODE"].ToString()
                        //+ "' and ITEM_NO='" + ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString()
                        + "' and PS_SUBCODE='" + ds_iv1.Tables[0].Rows[0]["PS_SUBCODE"].ToString() + "'";
                    if (!string.IsNullOrEmpty(ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString()))
                    {
                        sql += "  and ITEM_NO = " + ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString();
                    }

                    fileData = (byte[])SQLServerDALServer.GetSingle(sql);
                    if (fileData != null)
                    {
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(fileData);
                        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                        return img;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {

                    MessageBox.Show("IV数据不存在");
                }
            }
            return null;

        }


        /// <summary>
        /// 判断功率值
        /// </summary>
        /// <param name="strMsg"></param>
        public int PMShowMsg(float strPM)
        {

            if (strPM >= 240 && strPM < 245)
            {//240
                return 240;
            }
            else if (strPM >= 245 && strPM < 250)
            {//245
                return 245;
            }
            else if (strPM >= 250 && strPM < 255)
            {//250
                return 250;
            }
            else if (strPM >= 255 && strPM < 260)
            {//255
                return 255;
            }
            else if (strPM >= 260 && strPM < 265)
            {//260
                return 260;
            }
            else if (strPM >= 265 && strPM < 270)
            {//265
                return 265;
            }
            else if (strPM >= 270 && strPM < 275)
            {//265
                return 270;
            }
            return 0;

        }

        public int getPMFromWorkOrderRule(string workOrderNumber, string materialNo, float fPM)
        {
            int nResult = 0;
            float objPM = 0;
            string strSql = string.Format(@" select top 1 min_value from  ZPPM_WORK_ORDER_PRD_POWERSET t1
                where t1.ORDER_NUMBER ='{0}' and t1.MATERIAL_CODE='{1}' and t1.MIN_VALUE <='{2}' and t1.MAX_VALE >'{2}'
                order by  t1.ORDER_NUMBER ,t1.MATERIAL_CODE,t1.MAX_VALE ", workOrderNumber, materialNo, fPM);
            DataSet dsPM = SQLServerDALServer.Query(strSql);
            if (dsPM != null && dsPM.Tables[0].Rows.Count > 0)
            {
                float.TryParse(dsPM.Tables[0].Rows[0][0].ToString(), out objPM);
                if (objPM == null)
                {
                    objPM = 0;
                }
                try
                {
                    nResult = Convert.ToInt32(objPM);
                }
                catch
                {
                    nResult = 0;
                }
            }
            return nResult;
        }
        /// <summary>
        /// 判断电流
        /// </summary>
        /// <param name="strMsg"></param>

        private void button1_Click(object sender, EventArgs e)
        {

            //判断批次号位数
            if (txtContent.Text.Trim().Length == 15)
            {
                string _ProductType = "";
                pictureBox1.Image = getImageDataFromOracle(txtContent.Text.Trim().ToUpper());// System.Drawing.Image.FromFile(@"图片路径");//图片显示           
                string _PS_ITEM_NO = ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString();
                //获取工单分档规则
                string _bomstep = @"select PS_NAME,PM_NAME,STAND_PMAX,STAND_ISC,STAND_VOC,STAND_IPM,STAND_VPM,STAND_FUSE,POWER_DIFFERENCE 
                    from ZPPM_WORK_ORDER_PRD_POWERSET where ORDER_NUMBER='" + ds_LOT.Tables[0].Rows[0]["ORDER_NUMBER"].ToString()
                    + "' and MATERIAL_CODE='" + ds_LOT.Tables[0].Rows[0]["MATERIAL_CODE"].ToString() + "'";
                //+ "'  and ITEM_NO='" + ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString() 
                //+ "'   and ps_code='" + ds_iv1.Tables[0].Rows[0]["PS_CODE"].ToString() + "'";
                if (!string.IsNullOrEmpty(ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString()))
                {
                    _bomstep += " and ITEM_NO=" + ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString();
                }
                if (!string.IsNullOrEmpty(ds_iv1.Tables[0].Rows[0]["PS_CODE"].ToString()))
                {
                    _bomstep += "  and ps_code='" + ds_iv1.Tables[0].Rows[0]["PS_CODE"].ToString() + "'";
                }
                DataSet ds_bomstep = SQLServerDALServer.Query(_bomstep);
                //根据物料编码获取物料数据，进一步获取产品类型。
                string _MATERIAL = "select MATERIAL_CODE,MAIN_RAW_QTY,MATERIAL_SPEC from FMM_MATERIAL where MATERIAL_CODE='" + ds_LOT.Tables[0].Rows[0]["MATERIAL_CODE"].ToString() + "'";
                DataSet ds_MATERIAL = SQLServerDALServer.Query(_MATERIAL);
                if (ds_MATERIAL.Tables[0].Rows.Count > 0 && ds_bomstep.Tables[0].Rows.Count > 0)
                {
                    int _qty = Convert.ToInt32(ds_MATERIAL.Tables[0].Rows[0]["MAIN_RAW_QTY"]);
                    string head = ds_MATERIAL.Tables[0].Rows[0]["MATERIAL_NAME"].ToString().Substring(0, 3);
                    int _PMAX = Convert.ToInt32(ds_bomstep.Tables[0].Rows[0]["STAND_PMAX"]);
                    _ProductType = string.Format("{3}{1}{2}-{0}"
                                                 , ds_bomstep == null ? string.Empty : _PMAX.ToString()
                                                 , ds_MATERIAL.Tables[0].Rows[0]["MATERIAL_CODE"].ToString().StartsWith("1201") ? "M" : "P"
                                                 , _qty.ToString()
                                                 , head);
                }
                if (ds_bomstep.Tables[0].Rows.Count > 0)
                {
                    lblPowerName.Text = ds_bomstep.Tables[0].Rows[0]["PM_NAME"].ToString();//档位
                }
                if (ds_LOT.Tables[0].Rows.Count > 0)
                {
                    lblColor.Text = ds_LOT.Tables[0].Rows[0]["COLOR"].ToString(); //花色
                    lblGrade.Text = ds_LOT.Tables[0].Rows[0]["GRADE"].ToString();//等级
                }
                if (ds_iv1.Tables[0].Rows.Count > 0)
                {
                    lblCoefPM.Text = ds_iv1.Tables[0].Rows[0]["COEF_PMAX"].ToString();//功率
                    lblCoefISC.Text = ds_iv1.Tables[0].Rows[0]["Coef_ISC"].ToString();//ISC
                    lblCoefIPM.Text = ds_iv1.Tables[0].Rows[0]["COEF_IMAX"].ToString(); //IPM
                    lblCoefVOC.Text = ds_iv1.Tables[0].Rows[0]["Coef_VOC"].ToString(); //VOC
                    lblCoefVPM.Text = ds_iv1.Tables[0].Rows[0]["COEF_VMAX"].ToString(); //VPM
                    lblPowersetSubCode.Text = ds_iv1.Tables[0].Rows[0]["PS_SUBCODE"].ToString();//子档位
                }

                //lab文件名称
                string _lab = "TUV.Lab";
                //获取lable路径
                string str = System.Environment.CurrentDirectory + @"\\" + _lab;

                string strLabelFileName = System.Environment.CurrentDirectory + @"\\" + cmbLabels.SelectedValue + ".lab";

                if (System.IO.File.Exists(strLabelFileName) == false)
                {
                    MessageBox.Show(string.Format("工单设置需要的模块{0}未找到,系统采用默认标签模板.", strLabelFileName));
                }
                else
                {
                    str = strLabelFileName;
                }

                ApplicationClass lbl = null;
                try
                {
                    lbl = new ApplicationClass();
                    lbl.Documents.Open(str, false);// 调用设计好的label文件

                    Document doc = lbl.ActiveDocument;
                    doc.Variables.FormVariables.Item("LotNumber").Value = txtContent.Text.Trim().ToUpper(); //组件序列号
                    doc.Variables.FormVariables.Item("ProductType").Value = _ProductType; //类型
                    doc.Variables.FormVariables.Item("StandardIPM").Value = ds_bomstep.Tables[0].Rows[0]["STAND_IPM"].ToString(); //IPM
                    doc.Variables.FormVariables.Item("StandardIsc").Value = ds_bomstep.Tables[0].Rows[0]["STAND_ISC"].ToString(); //ISC
                    doc.Variables.FormVariables.Item("StandardVoc").Value = ds_bomstep.Tables[0].Rows[0]["STAND_VOC"].ToString(); //VOC
                    doc.Variables.FormVariables.Item("StandardVPM").Value = ds_bomstep.Tables[0].Rows[0]["STAND_VPM"].ToString(); //VPM
                    doc.Variables.FormVariables.Item("ProductSpec").Value = ds_MATERIAL.Tables[0].Rows[0]["MATERIAL_SPEC"].ToString(); //尺寸
                    doc.Variables.FormVariables.Item("PowerDifference").Value = ds_bomstep.Tables[0].Rows[0]["POWER_DIFFERENCE"].ToString(); //给参数传值
                    doc.Variables.FormVariables.Item("StandardFuse").Value = "15.0";// ds_bomstep.Tables[0].Rows[0]["STAND_FUSE"].ToString(); //最大串联保险丝额定电流
                    doc.Variables.FormVariables.Item("StandardPower").Value = string.Format("{0:000.000}", ds_bomstep.Tables[0].Rows[0]["STAND_PMAX"].ToString()) + ".0";  //最大功率
                    //doc.Variables.FormVariables.Item("StandardPower").Value = "270.0";  //最大功率

                    int Num = 1;// Convert.ToInt32(txtQuentity.Text);        //打印数量
                    doc.PrintDocument(Num);                             //打印

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    lblPowerName.Text = "";//档位
                    lblColor.Text = ""; //花色
                    lblGrade.Text = "";//等级
                    lblCoefPM.Text = "";//功率
                    lblCoefISC.Text = "";//ISC
                    lblCoefIPM.Text = ""; //IPM
                    lblCoefVOC.Text = ""; //VOC
                    lblCoefVPM.Text = ""; //VPM
                    lblPowersetSubCode.Text = "";//子档位
                    // pictureBox1.Image = null;//
                    ds_LOT = null;
                    ds_iv1 = null;
                    lbl.Quit();                                         //退出
                }
            }
            else
            {

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processName"></param>
        private void KillProcess(string processName) //调用方法，传参
        {
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(processName);
            foreach (System.Diagnostics.Process p in process)
            {
                p.Kill();
            }
            /*
            try
            {

                Process[] thisproc = Process.GetProcessesByName(processName);
                //thisproc.lendth:名字为进程总数
   
                 if (thisproc.Length > 0 )
                 {
                     for (int i=0; i< thisproc.Length;i++)
                 {
                     if (!thisproc[i].CloseMainWindow()) //尝试关闭进程 释放资源
                 {
                     thisproc[i].Kill(); //强制关闭

                 }
                 Console.WriteLine("进程 {0}关闭成功", processName);
            }
                     }
            else
            {
                 Console.WriteLine("进程 {0} 关闭失败!", processName);
            }
            }
            catch //出现异常，表明 kill 进程失败
            {
                 Console.WriteLine("结束进程{0}出错！", processName);
            }
             * */
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            hsLabels.Clear();
            string strSql = @" select Label_Code,Label_Name  from [dbo].[FMM_PRINTLABEL] Where IS_USED='1' and Label_Type='10'  
                            union select 'TUV.Lab' as Label_Code , '默认模板' as  Label_Name ";

            DataSet dsLabel = SQLServerDALServer.Query(strSql);
            if (dsLabel != null && dsLabel.Tables.Count > 0)
            {
                DataTable dtLabel = dsLabel.Tables[0];
                cmbLabels.DataSource = dtLabel;
                cmbLabels.DisplayMember = "Label_Name";
                cmbLabels.ValueMember = "Label_Code";
                cmbLabels.SelectedValue = "TUV.Lab";
            }
            this.txtContent.Focus();

        }


        private void txtContent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                //判断批次号位数
                if (txtContent.Text.Trim().Length == 15 || txtContent.Text.Trim().Length == 13)
                {
                    string _ProductType = "";
                    pictureBox1.Image = getImageDataFromOracle(txtContent.Text.Trim().ToUpper());// System.Drawing.Image.FromFile(@"图片路径");//图片显示           
                    string _PS_ITEM_NO = "";
                    if (ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"] != null)
                    {
                        _PS_ITEM_NO = ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString();
                    }
                    //获取工单分档规则
                    string _bomstep = "select PS_NAME,PM_NAME,STAND_PMAX,STAND_ISC,STAND_VOC,STAND_IPM,STAND_VPM,STAND_FUSE,POWER_DIFFERENCE  from ZPPM_WORK_ORDER_PRD_POWERSET where ORDER_NUMBER='" + ds_LOT.Tables[0].Rows[0]["ORDER_NUMBER"].ToString()
                        + "' and MATERIAL_CODE='" + ds_LOT.Tables[0].Rows[0]["MATERIAL_CODE"].ToString() + "'";
                        //+ "  and ITEM_NO=" + ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString()
                        //+ " and ps_code='" + ds_iv1.Tables[0].Rows[0]["PS_CODE"].ToString() + "'";
                    if (!string.IsNullOrEmpty(ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString()))
                    {
                        _bomstep += "  and ITEM_NO=" + ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString();
                    }
                    if (!string.IsNullOrEmpty(ds_iv1.Tables[0].Rows[0]["PS_CODE"].ToString()))
                    {
                        _bomstep += "  and ps_code='" + ds_iv1.Tables[0].Rows[0]["PS_CODE"].ToString() + "'";
                    }

                    DataSet ds_bomstep = SQLServerDALServer.Query(_bomstep);
                    //根据物料编码获取物料数据，进一步获取产品类型。
                    string _MATERIAL = "select MATERIAL_NAME,MATERIAL_CODE,MAIN_RAW_QTY,MATERIAL_SPEC from FMM_MATERIAL where MATERIAL_CODE='" + ds_LOT.Tables[0].Rows[0]["MATERIAL_CODE"].ToString() + "'";
                    DataSet ds_MATERIAL = SQLServerDALServer.Query(_MATERIAL);
                    if (ds_MATERIAL.Tables[0].Rows.Count > 0 && ds_bomstep.Tables[0].Rows.Count > 0)
                    {
                        int _qty = Convert.ToInt32(ds_MATERIAL.Tables[0].Rows[0]["MAIN_RAW_QTY"]);
                        string head = ds_MATERIAL.Tables[0].Rows[0]["MATERIAL_NAME"].ToString().Substring(0,3);
                        int _PMAX = Convert.ToInt32(ds_bomstep.Tables[0].Rows[0]["STAND_PMAX"]);
                        _ProductType = string.Format("{3}{1}{2}-{0}"
                                                     , ds_bomstep == null ? string.Empty : _PMAX.ToString()
                                                     , ds_MATERIAL.Tables[0].Rows[0]["MATERIAL_CODE"].ToString().StartsWith("1201") ? "M" : "P"
                                                     , _qty.ToString()
                                                     ,head);
                    }
                    #region 显示内容
                    if (ds_bomstep.Tables[0].Rows.Count > 0)
                    {
                        lblPowerName.Text = ds_bomstep.Tables[0].Rows[0]["PM_NAME"].ToString();//档位
                    }
                    if (ds_LOT.Tables[0].Rows.Count > 0)
                    {
                        lblColor.Text = ds_LOT.Tables[0].Rows[0]["COLOR"].ToString(); //花色
                        lblGrade.Text = ds_LOT.Tables[0].Rows[0]["GRADE"].ToString();//等级
                    }
                    if (ds_iv1.Tables[0].Rows.Count > 0)
                    {
                        lblCoefPM.Text = ds_iv1.Tables[0].Rows[0]["COEF_PMAX"].ToString();//功率
                        lblCoefISC.Text = ds_iv1.Tables[0].Rows[0]["Coef_ISC"].ToString();//ISC
                        lblCoefIPM.Text = ds_iv1.Tables[0].Rows[0]["COEF_IMAX"].ToString(); //IPM
                        lblCoefVOC.Text = ds_iv1.Tables[0].Rows[0]["Coef_VOC"].ToString(); //VOC
                        lblCoefVPM.Text = ds_iv1.Tables[0].Rows[0]["COEF_VMAX"].ToString(); //VPM
                        lblPowersetSubCode.Text = ds_iv1.Tables[0].Rows[0]["PS_SUBCODE"].ToString();//子档位
                    }
                    #endregion
                    #region 打印内容

                    //lab文件名称
                    string _lab = "TUV.Lab";
                    //获取lable路径
                    string str = System.Environment.CurrentDirectory + @"\\" + _lab;
                    string strLot = System.Environment.CurrentDirectory + @"\\JCL-0001.lab";
                    //获取工单的打印模块
                    string strSql = @"select LABEL_CODE from [dbo].[ZPPM_WORK_ORDER_PRD_PRINTSET] t1
                        inner join WIP_LOT t2
                        on t1.MATERIAL_CODE =t2.MATERIAL_CODE
                        and t1.ORDER_NUMBER =t2.ORDER_NUMBER
                        and t2.LOT_NUMBER='" + txtContent.Text.Trim().ToUpper() + "'";

                    DataSet dsLabel = SQLServerDALServer.Query(strSql);
                    if (dsLabel != null && dsLabel.Tables[0].Rows.Count > 0)
                    {
                        string strLabel = dsLabel.Tables[0].Rows[0][0].ToString();
                        try
                        {
                            cmbLabels.SelectedValue = strLabel;
                        }
                        catch
                        {

                        }
                        string strLabelFileName = System.Environment.CurrentDirectory + @"\\" + strLabel + ".lab";
                        string strLotFileName = System.Environment.CurrentDirectory + @"\\JCL-0001.lab";
                        if (System.IO.File.Exists(strLabelFileName) == false)
                        {
                            MessageBox.Show(string.Format("工单设置需要的模块{0}未找到,系统采用默认标签模板.", strLabelFileName));
                        }
                        else
                        {
                            str = strLabelFileName;
                        }

                        if (System.IO.File.Exists(strLotFileName) == false)
                        {
                            MessageBox.Show(string.Format("工单设置需要的模块{0}未找到,系统采用默认标签模板.", strLotFileName));
                        }
                        else
                        {
                            strLot = strLotFileName;
                        }
                    }

                    ApplicationClass lbl = new ApplicationClass();
                    
                    try
                    {

                        lbl.Documents.Open(str, false);// 调用设计好的label文件
                        
                        //lbl.Documents.Open(@"D:\TUV.Lab", false);
                        Document doc = lbl.ActiveDocument;
                        doc.Variables.FormVariables.Item("LotNumber").Value = txtContent.Text.Trim().ToUpper(); //组件序列号
                        doc.Variables.FormVariables.Item("ProductType").Value = _ProductType; //类型
                        doc.Variables.FormVariables.Item("StandardIPM").Value = ds_bomstep.Tables[0].Rows[0]["STAND_IPM"].ToString(); //IPM
                        doc.Variables.FormVariables.Item("StandardIsc").Value = ds_bomstep.Tables[0].Rows[0]["STAND_ISC"].ToString(); //ISC
                        doc.Variables.FormVariables.Item("StandardVoc").Value = ds_bomstep.Tables[0].Rows[0]["STAND_VOC"].ToString(); //VOC
                        doc.Variables.FormVariables.Item("StandardVPM").Value = ds_bomstep.Tables[0].Rows[0]["STAND_VPM"].ToString(); //VPM
                        doc.Variables.FormVariables.Item("ProductSpec").Value = ds_MATERIAL.Tables[0].Rows[0]["MATERIAL_SPEC"].ToString(); //尺寸
                        doc.Variables.FormVariables.Item("PowerDifference").Value = ds_bomstep.Tables[0].Rows[0]["POWER_DIFFERENCE"].ToString(); //给参数传值
                        doc.Variables.FormVariables.Item("StandardFuse").Value = "15.0"; //ds_bomstep.Tables[0].Rows[0]["STAND_FUSE"].ToString(); //最大串联保险丝额定电流
                        doc.Variables.FormVariables.Item("StandardPower").Value = string.Format("{0:000.000}", ds_bomstep.Tables[0].Rows[0]["STAND_PMAX"].ToString()) + ".0";  //最大功率
                        //doc.Variables.FormVariables.Item("StandardPower").Value = "270.0";  //最大功率                    
                        int Num = 1;// Convert.ToInt32(txtQuentity.Text);        //打印数量
                        //打印
                        //按条件判断此组件是否存在,如不存在则把组件数据保存
                        string _IVPRINTLOG = "select ITEM_NO from ZWIP_IV_TEST_PRINTLOG where LOT_NUMBER='" + txtContent.Text.Trim().ToUpper() + "' and TEST_TIME='" + ds_iv1.Tables[0].Rows[0]["TEST_TIME"].ToString() + "' and EQUIPMENT_CODE='" + ds_iv1.Tables[0].Rows[0]["EQUIPMENT_CODE"].ToString() + "' and ITEM_NO=1";
                        DataSet ds_ivprintlog = SQLServerDALServer.Query(_IVPRINTLOG);


                        
                        
                        if (ds_ivprintlog.Tables[0].Rows.Count > 0)
                        {
                            //是否有权限可以重复打印
                            //string _ROLE = "select * from RBAC_ROLE_OWN_RESOURCE where RESOURCE_CODE='1003/1102/09/01' and editor =''";//传入用户名
                            //DataSet ds_role = SQLServerDALServer.Query(_ROLE);
                            //if(ds_role.Tables[0].Rows.Count>0){
                           // doc.PrintDocument(Num);
                            doc.Printer.SwitchTo("铭牌打印");
                            doc.PrintLabel(1, 1, 1, 1, 1, "");
                            doc.FormFeed();
                            //}else{
                            //    MessageBox.Show("铭牌重复打印无权限");
                            //}


                            if (checkLot.Checked)
                            {
                            ApplicationClass lb2 = new ApplicationClass();
                            lb2.Documents.Open(strLot, false);
                            Document doc2 = lb2.ActiveDocument;
                            doc2.Variables.FormVariables.Item("LotNumber").Value = txtContent.Text.Trim().ToUpper(); //组件序列号
                            //doc2.PrintDocument(Num);
                            doc2.Printer.SwitchTo("批次打印");
                            doc2.PrintLabel(1, 1, 1, 1, 1, "");
                            doc2.FormFeed();
                            }





                        }
                        else
                        {
                            //doc.PrintDocument(Num);
                            doc.Printer.SwitchTo("铭牌打印");
                            doc.PrintLabel(1, 1, 1, 1, 1, "");
                            doc.FormFeed();
                            if (checkLot.Checked)
                            {
                                ApplicationClass lb2 = new ApplicationClass();
                                lb2.Documents.Open(strLot, false);
                                Document doc2 = lb2.ActiveDocument;
                                doc2.Variables.FormVariables.Item("LotNumber").Value = txtContent.Text.Trim().ToUpper(); //组件序列号
                                //doc2.PrintDocument(Num);
                                doc2.Printer.SwitchTo("批次打印");
                                doc2.PrintLabel(1, 1, 1, 1, 1, "");
                                doc2.FormFeed();
                            }


                            #region 打印完后向数据库中插入数据
                            //打印完后向数据库中插入数据
                            StringBuilder logsb = new StringBuilder();
                            logsb.Append("INSERT INTO [ZWIP_IV_TEST_PRINTLOG]");
                            logsb.Append("([LOT_NUMBER],[TEST_TIME],[EQUIPMENT_CODE]");
                            logsb.Append(",[ITEM_NO],[COEF_PMAX] ,[COEF_ISC] ,[COEF_VOC]");
                            logsb.Append(",[COEF_IMAX],[COEF_VMAX] ,[COEF_FF],[DEC_CTM],[PS_CODE]");
                            logsb.Append(",[PS_ITEM_NO],[PS_SUBCODE],[PRINT_TIME] ,[LABEL_CODE],[CREATOR],[CREATE_TIME])  VALUES (");
                            logsb.Append("'" + txtContent.Text.Trim().ToUpper() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["TEST_TIME"].ToString() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["EQUIPMENT_CODE"].ToString() + "',");
                            logsb.Append("1,");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["COEF_PMAX"].ToString() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["COEF_ISC"].ToString() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["COEF_VOC"].ToString() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["COEF_IMAX"].ToString() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["COEF_VMAX"].ToString() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["COEF_FF"].ToString() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["DEC_CTM"].ToString() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["PS_CODE"].ToString() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["PS_ITEM_NO"].ToString() + "',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["PS_SUBCODE"].ToString() + "',");
                            logsb.Append("'" + System.DateTime.Now.ToString() + "',");
                            logsb.Append("'JNP-0001',");
                            logsb.Append("'" + ds_iv1.Tables[0].Rows[0]["CREATOR"].ToString() + "',");
                            logsb.Append("'" + System.DateTime.Now.ToString() + "')");
                            //string _str = logsb.ToString();
                            SQLServerDALServer.ExecuteSql(logsb.ToString());
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        // lblPowerName.Text = "";//档位
                        //lblColor.Text = ""; //花色
                        // lblGrade.Text = "";//等级
                        //lblCoefPM.Text = "";//功率
                        //lblCoefISC.Text = "";//ISC
                        //lblCoefIPM.Text = ""; //IPM
                        // lblCoefVOC.Text = ""; //VOC
                        //lblCoefVPM.Text = ""; //VPM
                        // lblPowersetSubCode.Text = "";//子档位
                        // pictureBox1.Image = null;//
                        ds_LOT = null;
                        ds_iv1 = null;
                        lbl.Quit();                                         //退出
                        txtContent.SelectAll();
                    }
                    #endregion
                }
                else
                {

                }
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

            //lbl.Quit();   
            //System.Windows.Forms.Application.Exit();qI

            // KillProcess("lsass");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void setSelectedComboxItem(ComboBox combobox, string itemValue)
        {

        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {

        }



        private void BtnLot_Click(object sender, EventArgs e)
        {
            string strLot = System.Environment.CurrentDirectory + @"\\JCL-0001.lab";
            string strLotFileName = System.Environment.CurrentDirectory + @"\\JCL-0001.lab";
            if (System.IO.File.Exists(strLotFileName) == false)
            {
                MessageBox.Show(string.Format("工单设置需要的模块{0}未找到,系统采用默认标签模板.", strLotFileName));
            }
            else
            {
                strLot = strLotFileName;
            }
            if (txtContent.Text.Trim().ToUpper()==null||txtContent.Text.Trim().ToUpper().Length==0)
            {
                MessageBox.Show("请输入批次号！");
            }
            int Num = 1;
            ApplicationClass lb2 = new ApplicationClass();
            lb2.Documents.Open(@"D:\JCL-0001.Lab", false);
            Document doc2 = lb2.ActiveDocument;
            doc2.Variables.FormVariables.Item("LotNumber").Value = txtContent.Text.Trim().ToUpper(); //组件序列号
            //doc2.PrintDocument(Num);
            doc2.Printer.SwitchTo("批次打印");
            doc2.PrintLabel(1, 1, 1, 1, 1, "");
            doc2.FormFeed();
        }
    }
}
