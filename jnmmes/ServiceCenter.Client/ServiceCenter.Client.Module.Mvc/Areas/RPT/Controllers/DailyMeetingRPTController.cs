using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.RPT.Models;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.RPT;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Resources;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Collections;
using ServiceCenter.Client.Mvc.Areas.WIP.Models;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Controllers
{
    public class DailyMeetingRPTController : Controller
    {
        /// <summary> 取得格式化后的字符串 </summary>
        /// <param name="columnFormat">需要格式化字符串</param>
        /// <param name="dataTime">日期</param>
        /// <returns></returns>
        public string GetFormatString(string columnFormat, DateTime dataTime)
        {
            string result = "";
            int iStartPos = 0;
            int iEndPos = 0;
            string sFormat = "";

            //取得格式中的替换字段
            //取得开始位置
            iStartPos = columnFormat.IndexOf("[");

            //取得结束位置
            iEndPos = columnFormat.IndexOf("]");

            //存在相应的需替换格式
            if ( iStartPos >= 0 && iEndPos > 0 && iEndPos > iStartPos )
            {
                //截取格式
                sFormat = columnFormat.Substring(iStartPos + 1, iEndPos - iStartPos - 1);

                //格式化
                sFormat = dataTime.ToString(sFormat);

                //覆盖相应的位置
                //result = columnFormat.Substring(0, iStartPos) + sFormat + columnFormat.Substring(iEndPos + 1, columnFormat.Length - iEndPos + 1);
                result = columnFormat.Substring(0, iStartPos) + sFormat;
                
                if ( iEndPos + 1 > columnFormat.Length )
                {
                    result = result + columnFormat.Substring(iEndPos + 1, columnFormat.Length - iEndPos + 1);
                }
                
                //递归调用处理其它替换内容
                result = GetFormatString(result, dataTime);
            }
            else
            {
                result = columnFormat;
            }

            return result;            
        }

        /// <summary> 创建数据列表 </summary>
        /// <param name="dtSet">取得数据集（报表数据、报表格式）</param>
        /// <param name="dt">返回报表数据列表</param>
        /// <returns></returns>        
        public MethodReturnResult CreateTableList(MethodReturnResult<DataSet> dtSet, ref DataTable dt)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                result.Code = 0;
                DataTable dtModuleDailyData = new DataTable();      //报表数据
                DataTable dtModuleReportFormat = new DataTable();   //报表行项目及格式
                DataTable dtModuleReportColumn = new DataTable();   //报表列项目及属性
                string sColumnType = "";                            //列项目类型
                string sColumnTitleFormat = "";                     //列项目类型名称格式
                string sColumnFormat = "";                          //列项目类型字段格式
                string sColumn = "";                                //列项目代码
                string sColumnTitle = "";                           //列项目标题
                bool bShowMemo = false;                             //是否显示说明列

                #region 增加项目固定列
                //增加子报表链接字段
                DataColumn dcStatus = new DataColumn("ItemCode");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加项目名称字段
                dcStatus = new DataColumn("ItemName");
                dcStatus.Caption = "项目";
                dt.Columns.Add(dcStatus);

                //增加子报表链接字段
                dcStatus = new DataColumn("LINK");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加字段类型
                dcStatus = new DataColumn("DataType");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加子报表代码
                dcStatus = new DataColumn("ChildRPCode");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加子报表名称
                dcStatus = new DataColumn("ChildRPName");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加图形类型
                dcStatus = new DataColumn("ChartType");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加图形初始显示类型
                dcStatus = new DataColumn("ChartShow");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加图形分组
                dcStatus = new DataColumn("GroupNo");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加数据显示格式
                dcStatus = new DataColumn("DataLabelsFormat");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加日期子报表链接字段
                dcStatus = new DataColumn("DetailLINK");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加日期子报表代码
                dcStatus = new DataColumn("DetailChildRPCode");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加日期子报表名称
                dcStatus = new DataColumn("DetailChildRPName");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加产品类型
                dcStatus = new DataColumn("ProductID");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                #endregion

                #region 创建动态列
                dtModuleDailyData = dtSet.Data.Tables[0];             //报表数据
                dtModuleReportFormat = dtSet.Data.Tables[1];          //报表行项目格式
                dtModuleReportColumn = dtSet.Data.Tables[2];          //报表列项目及属性

                var columnquery = from t in dtModuleReportColumn.AsEnumerable()
                                  select new
                                  {
                                      ColumnIndex = t.Field<System.Int16>("columnIndex"),          //列项目序号
                                      ColumnType = t.Field<string>("columntype"),                  //列项目类型
                                      ColumnTitleFormat = t.Field<string>("columnTitleFormat"),    //列项目类型标题格式
                                      ColumnFormat = t.Field<string>("columnFormat")               //列项目类型字段格式
                                  } into r
                                  orderby r.ColumnIndex
                                  select r;

                foreach (var columndata in columnquery)
                {
                    sColumnType = columndata.ColumnType;                    //列项目类型
                    sColumnTitleFormat = columndata.ColumnTitleFormat;      //列项目类型标题格式
                    sColumnFormat = columndata.ColumnFormat;                //列项目类型字段格式

                    if (sColumnType == "MEMO")
                    {
                        sColumn = sColumnFormat;                //列名称
                        sColumnTitle = sColumnTitleFormat;      //列标题

                        dcStatus = new DataColumn(sColumn);
                        dcStatus.Caption = sColumnTitle;
                        dcStatus.DefaultValue = 0;

                        dt.Columns.Add(dcStatus);

                        bShowMemo = true;
                    }
                    else
                    {
                        //取得列类型对应数据数并设置对应标题
                        var query = from t in dtModuleDailyData.AsEnumerable()
                                    where (t.Field<string>("DataType") == sColumnType)
                                    group t by new { t1 = t.Field<string>("rp_datetime") }
                                        into m
                                        select new
                                        {
                                            RPDateTime = m.First().Field<string>("rp_datetime"),
                                            FieldTitle = m.First().Field<string>("FieldTitle")
                                        } into r
                                        orderby r.RPDateTime
                                        select r;

                        //取得时间列行数
                        int iDateColumns = query.Count();

                        //设置列信息
                        foreach (var data in query)
                        {
                            if (sColumnType == "DL")
                            {
                                sColumn = "{DDL}-" + data.RPDateTime;            //列名称
                                sColumnTitle = data.FieldTitle;             //列标题
                            }
                            else
                            {
                                sColumn = GetFormatString(sColumnFormat, Convert.ToDateTime(data.RPDateTime));                //列名称
                                sColumnTitle = GetFormatString(sColumnTitleFormat, Convert.ToDateTime(data.RPDateTime));      //列标题
                            }                            

                            dcStatus = new DataColumn(sColumn);
                            dcStatus.Caption = sColumnTitle;
                            dcStatus.DefaultValue = 0;

                            dt.Columns.Add(dcStatus);
                        }
                    }
                }

                //增加计算说明字段
                if (bShowMemo == false)
                {
                    dcStatus = new DataColumn("MEMO");
                    dcStatus.Caption = "说明";
                    dcStatus.Caption = "HIDE";             //设置隐藏属性在标题（自定义）
                    dt.Columns.Add(dcStatus);
                }
                #endregion

                #region 定义行
                //取得行信息字段
                var rowquery = from t in dtModuleReportFormat.AsEnumerable()
                               select new
                               {
                                   rowsnum = t.Field<System.Int16>("rowsnum"),                  //行号
                                   Item_code = t.Field<string>("item_code"),                    //项目号
                                   Item_name = t.Field<string>("item_name"),                    //项目名称
                                   Memo = t.Field<string>("Memo"),                              //备注
                                   RPTURL = t.Field<string>("reportURL"),                       //子报表链接
                                   ChildRPCode = t.Field<string>("childrpcode"),                //子报表代码
                                   ChildRPName = t.Field<string>("childrpname"),                //子报表名称
                                   DataType = t.Field<string>("datatype"),                      //数据类型（合计、子数据）
                                   ChartType = t.Field<string>("charttype"),                    //图形类型
                                   ChartShow = t.Field<string>("chartShow"),                    //图形初始显示类型
                                   IsDetial = t.Field<System.Int16>("isdetial"),                //明细项目数据标识（1 - 明细数据）
                                   GroupNo = t.Field<string>("groupno"),                        //数据分组代码（针对图形控件显示）
                                   DataLabelsFormat = t.Field<string>("dataLabelsformat"),      //数据标签显示格式
                                   DetailRPTURL = t.Field<string>("DetailreportURL"),           //日期子报表链接
                                   DetailChildRPCode = t.Field<string>("Detailchildrpcode"),    //日期子报表代码
                                   DetailChildRPName = t.Field<string>("Detailchildrpname"),    //日期子报表名称
                                   ProductID = t.Field<string>("ProductID")                     //产品类型
                               } into r
                               orderby r.rowsnum
                               select r;

                DataRow dr = dt.NewRow();

                //设置行信息
                foreach (var data in rowquery)
                {
                    if (data.IsDetial == 1)
                    {
                        //处理明细数据
                        var detialquery = from t in dtModuleDailyData.AsEnumerable()
                                          where t.Field<string>("ItemCode") == data.Item_code
                                          group t by new
                                          {
                                              DetailItem = t.Field<string>("DetailItem"),
                                              DetailTitle = t.Field<string>("DetailTitle")
                                          } into g
                                          select new
                                          {
                                              DetailItem = g.Key.DetailItem,
                                              DetailTitle = g.Key.DetailTitle
                                          } into r
                                          orderby r.DetailItem
                                          select r;

                        foreach (var detaildata in detialquery)
                        {
                            dr = dt.NewRow();

                            dr["ItemCode"] = "Detail" + detaildata.DetailItem;      //项目号
                            dr["ItemName"] = detaildata.DetailTitle;                //项目名称
                            dr["LINK"] = data.RPTURL;                               //合计栏链接
                            dr["ChildRPCode"] = data.ChildRPCode;                   //合计栏子报表代码
                            dr["ChildRPName"] = data.ChildRPName;                   //合计栏子报表名称
                            dr["DataType"] = data.DataType;                         //数据类型（合计、子数据）                            
                            dr["MEMO"] = data.Memo;                                 //备注
                            dr["ChartType"] = data.ChartType;                       //图形控件类型
                            dr["ChartShow"] = data.ChartShow;                       //图形控件初始显示类型
                            dr["GroupNo"] = data.GroupNo;                           //数据分组代码（针对图形控件显示）
                            dr["DataLabelsFormat"] = data.DataLabelsFormat;         //图形控件标签显示格式
                            dr["DetailLINK"] = data.DetailRPTURL;                   //日期栏链接
                            dr["DetailChildRPCode"] = data.DetailChildRPCode;       //日期栏子报表代码
                            dr["DetailChildRPName"] = data.DetailChildRPName;       //日期栏子报表名称
                            dr["ProductID"] = data.ProductID;                       //产品类型

                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr = dt.NewRow();

                        dr["ItemCode"] = data.Item_code;                        //项目代码
                        dr["ItemName"] = data.Item_name;                        //项目名称
                        dr["DataType"] = data.DataType;                         //数据类型（合计、子数据）
                        dr["LINK"] = data.RPTURL;                               //合计栏链接 
                        dr["ChildRPCode"] = data.ChildRPCode;                   //合计栏子报表代码
                        dr["ChildRPName"] = data.ChildRPName;                   //合计栏子报表名称
                        dr["MEMO"] = data.Memo;                                 //备注
                        dr["ChartType"] = data.ChartType;                       //图形类型
                        dr["ChartShow"] = data.ChartShow;                       //图形控件初始显示类型
                        dr["GroupNo"] = data.GroupNo;                           //数据分组代码（针对图形控件显示）
                        dr["DataLabelsFormat"] = data.DataLabelsFormat;         //数据标签显示格式
                        dr["DetailLINK"] = data.DetailRPTURL;                   //日期栏链接
                        dr["DetailChildRPCode"] = data.DetailChildRPCode;       //日期栏子报表代码
                        dr["DetailChildRPName"] = data.DetailChildRPName;       //日期栏子报表名称
                        dr["ProductID"] = data.ProductID;                       //产品类型

                        dt.Rows.Add(dr);
                    }
                }
                #endregion

                #region 填充数据
                string strItemCode = "";
                string strValues = "";
                string strDataType = "";
                DateTime dtRPDateTime = DateTime.Now;
                string strRPDateTime = "";
                int selOfRow = 0;
                string strFieldDataType = "";
                string strDataLabelsFormat = "";
                string strDetail = "";
                string strProductID = "";

                for (int i = 0; i < dtModuleDailyData.Rows.Count; i++)
                {
                    strItemCode = dtModuleDailyData.Rows[i]["ItemCode"].ToString();                 //项目代码
                    strValues = dtModuleDailyData.Rows[i]["sumQty"].ToString();                     //项目值
                    strDataType = dtModuleDailyData.Rows[i]["DataType"].ToString();                 //项目数据类型

                    if (strDataType == "DL")
                    {
                        strRPDateTime = dtModuleDailyData.Rows[i]["rp_datetime"].ToString();        //项目数据
                    }
                    else
                    {
                        dtRPDateTime = Convert.ToDateTime(dtModuleDailyData.Rows[i]["rp_datetime"]);    //项目数据日期
                    }
                    
                    strDetail = dtModuleDailyData.Rows[i]["DetailItem"].ToString();                 //明细项目标识
                    strProductID = dtModuleDailyData.Rows[i]["ProductID"].ToString();               //产品

                    //取得项目所在行数及相关信息
                    var ItemRowquery = from t in dtModuleReportFormat.AsEnumerable()
                                       where (t.Field<string>("item_code") == strItemCode)
                                       select new
                                       {
                                           rowsnum = t.Field<System.Int16>("rowsnum"),
                                           datatype = t.Field<string>("datatype"),
                                           dataLabelsformat = t.Field<string>("dataLabelsformat")
                                       };

                    selOfRow = ItemRowquery.FirstOrDefault().rowsnum;
                    strFieldDataType = ItemRowquery.FirstOrDefault().datatype;
                    strDataLabelsFormat = ItemRowquery.FirstOrDefault().dataLabelsformat;       //数据格式

                    if (strDetail != "")    //明细数据
                    {
                        strItemCode = "Detail" + strDetail;
                    }

                    DataRow[] ItemRow = dt.Select("ItemCode = '" + strItemCode + "'");

                    if (ItemRow.GetLength(0) > 0)
                    {
                        //设置数据类型格式
                        switch (strFieldDataType)
                        {
                            case "int":
                                //处理空值为0
                                if (strValues == "")
                                {
                                    strValues = "0";
                                }
                                else
                                {
                                    strValues = Convert.ToDouble(strValues).ToString();
                                }

                                break;
                            case "dec":
                                break;
                        }

                        //设置数据格式
                        if (strDataLabelsFormat.Length > 1)
                        {
                            if (strDataLabelsFormat.Substring(strDataLabelsFormat.Length - 1, 1) == "%")
                            {
                                strValues = strValues + "%";
                            }
                        }

                        //数据填充
                        //判断当前列类型与记录的列类型是否相同，若不同则取得对应的字段格式
                        if (sColumnType != strDataType)
                        {
                            var columnformat = from t in dtModuleReportColumn.AsEnumerable()
                                               where (t.Field<string>("columntype") == strDataType)
                                               select new
                                               {
                                                   ColumnFormat = t.Field<string>("columnFormat")                //列项目类型字段格式
                                               };

                            //记录相应列类型的字段格式，若下一个列类型相同则不进行取数
                            sColumnType = strDataType;
                            sColumnFormat = columnformat.FirstOrDefault().ColumnFormat;
                        }

                        if (strDataType == "DL")
                        {
                            sColumn = "{DDL}-" + strRPDateTime;                             //列名称
                        }
                        else
                        {
                            sColumn = GetFormatString(sColumnFormat, dtRPDateTime);                             //列名称
                        }
                        
                        ItemRow[0][sColumn] = strValues;

                        ItemRow[0]["ProductID"] = strProductID;
                    }
                }

                #endregion

                return result;
            }
            catch (Exception e)
            {
                result.Code = 1000;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return result;
            }
        }

        /// <summary> 根据数据列表创建图形控件数据字符串 </summary>
        /// <param name="dtData"></param>
        /// <param name="sAxis"></param>
        /// <returns></returns>
        public MethodReturnResult AssembleSplineForModuleDebris(DataTable dtData, ref SortedList<string, string> sAxis)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                string smAxis = string.Empty;
                string snAxis = string.Empty;
                string strPlotLinesText = string.Empty;
                string strPlotLinesValue = string.Empty;
                int iCurrRow = 0;
                int iIndex = 0;
                bool drawChart = false;
                string sValue = "";

                result.Code = 0;
               
                //逐行组装数据矩阵
                foreach (DataRow datarow in dtData.Rows)
                {
                    drawChart = false;  //图形显示标识

                    if (datarow["ChartType"].ToString().Equals("line") || datarow["ChartType"].ToString().Equals("spline") || datarow["ChartType"].ToString().Equals("column"))
                    {
                        if (snAxis == "")
                        {
                            snAxis = "[{";
                        }
                        else
                        {
                            snAxis = snAxis + ",{";
                        }

                        //取得项目名称
                        snAxis = snAxis + " name: '" + datarow["ItemName"].ToString() + "',";

                        //设置项目图形类型
                        switch (datarow["ChartType"].ToString())
                        {
                            case "line":
                                if (datarow["GroupNo"].ToString().Equals("0"))  //图形组
                                {
                                    snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',data: [";
                                }
                                else
                                {
                                    snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',yAxis: " + datarow["GroupNo"].ToString() + ",data: [";
                                }

                                drawChart = true;   //图形显示标识

                                break;
                            case "spline":
                                if (datarow["GroupNo"].ToString().Equals("0"))  //图形组
                                {
                                    snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',data: [";
                                }
                                else
                                {
                                    snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',yAxis: " + datarow["GroupNo"].ToString() + ",data: [";
                                }

                                drawChart = true;   //图形显示标识

                                break;
                            case "column":
                                //snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',data: [";
                                if (datarow["GroupNo"].ToString().Equals("0"))  //图形组
                                {
                                    snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',data: [";
                                }
                                else
                                {
                                    snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',yAxis: " + datarow["GroupNo"].ToString() + ",data: [";
                                }

                                drawChart = true;   //图形显示标识

                                break;
                            default:
                                drawChart = false;  //图形显示标识

                                break;
                        }
                    }

                    //取得数据及标题列信息
                    iIndex = 0;
                    
                    foreach (DataColumn datacolum in dtData.Columns)
                    {
                        //设置数据列
                        if (drawChart == true && (datacolum.ColumnName.ToString().StartsWith("{D") || datacolum.ColumnName.ToString().Equals("SUM")))   //合计列和数据列
                        {
                            if (datacolum.ColumnName.ToString().StartsWith("{DB") && (strPlotLinesValue == null || strPlotLinesValue == ""))   //BaseLine
                            {
                                if (datarow[datacolum].ToString().Substring(datarow[datacolum].ToString().Length - 1).Equals("%"))
                                {
                                    strPlotLinesValue = datarow[datacolum].ToString().Substring(0, datarow[datacolum].ToString().Length - 1);
                                }
                                else
                                {
                                    strPlotLinesValue = datarow[datacolum].ToString();
                                }

                                if (strPlotLinesValue == "0")
                                {
                                    strPlotLinesValue = "";
                                }
                                else
                                {
                                    strPlotLinesText = datacolum.Caption + ":" + datarow[datacolum].ToString();
                                }                                
                            }
                            else
                            {
                                #region 标题信息
                                if (iCurrRow == 0)                      //在第一次循环时取得标题信息
                                {
                                    if (smAxis == "")
                                    {
                                        smAxis = "[";
                                    }
                                    else
                                    {
                                        smAxis = smAxis + ",";
                                    }

                                    smAxis = smAxis + "'" + datacolum.Caption + "'";
                                }
                                #endregion

                                if (iIndex != 0)
                                {
                                    //开始第一列
                                    snAxis = snAxis + " ,";
                                }

                                //取消掉%号
                                if (datarow[datacolum].ToString().Substring(datarow[datacolum].ToString().Length - 1).Equals("%"))
                                {
                                    sValue = datarow[datacolum].ToString().Substring(0, datarow[datacolum].ToString().Length - 1);
                                }
                                else
                                {
                                    sValue = datarow[datacolum].ToString();
                                }

                                //对零值处理为NULL,在图形中不显示该节点
                                if (double.Parse(sValue) == 0)
                                {
                                    sValue = "null";
                                }

                                snAxis = snAxis + "[" + iIndex.ToString() + "," + sValue + "]";

                                iIndex++;
                            }
                        }                        
                    }

                    //设置标题信息的结束标志
                    if (iCurrRow == 0)
                    {
                        smAxis = smAxis + "]";

                        iCurrRow++;
                    }

                    if (drawChart == true)
                    {
                        //设置单行数据结束标志
                        if (datarow["ChartShow"].ToString() == "false")
                        {
                            snAxis = snAxis + "],visible: false}";
                        }
                        else
                        {                            
                            snAxis = snAxis + "]}";
                        }
                    } 
                }

                //设置行数据结束标志
                snAxis = snAxis + "]";

                //当结果集为空时清空控件
                if ( dtData.Rows.Count == 0 )
                {
                    smAxis = "[]";
                    snAxis = "[]";
                }
                
                sAxis.Add("mAxis", smAxis);
                sAxis.Add("nAxis", snAxis);
                sAxis.Add("PlotLinesValue", strPlotLinesValue);
                sAxis.Add("PlotLinesText", strPlotLinesText); 

                return result;
            }
            catch (Exception e)
            {
                result.Code = 1000;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return result;
            }            
        }
        
        #region 日运营报表主界面
        /// <summary>
        /// 日运营报表主界面初始化
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            DailyMeetingRPTViewModel model = new DailyMeetingRPTViewModel
            {
                //初始化参数
                ReportCode = "DAY01",         //报表代码

                StartDate = System.DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"),     //初始化开始日期
                EndDate = System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),       //初始化结束日期
                MonthDataNumber = 3,                                                    //显示历史月份数
                YearDataNumber = 3                                                      //显示历史年度数
            };

            return View(model);
        }
        
        /// <summary>
        /// 主界面查询窗体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Query(DailyMeetingRPTViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();

            try
            {
                #region 取得数据
                DataTable dt = new DataTable();                     //报表结果集
                
                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    //初始化参数
                    p.ReportCode = model.ReportCode;          //报表代码
                    p.StartDate = model.StartDate;            //开始日期
                    p.EndDate = model.EndDate;                //结束日期
                    p.LocationName = model.LocationName;      //车间
                    p.LineCode = model.LineCode;              //线别
                    p.OrderNumber = model.OrderNumber;        //工单代码
                    p.ProductID = model.ProductID;            //产品类型

                    switch (model.ReportCode)
                    {
                        case "DAY0202":
                            p.StartDate = p.EndDate;            //开始日期

                            break;
                        case "DAY0203":
                            p.StartDate = p.EndDate;            //开始日期

                            break;
                        case "":

                            break;
                    }


                    MethodReturnResult<DataSet> rst = client.Get(ref p);

                    if (rst.Code > 0)               //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;
                        
                        return Json(result);
                    }
                    else
                    {         
                        //根据报表格式及数据清单组合数据列表
                        result = CreateTableList(rst,ref dt);

                        if (result.Code > 0)       //产生错误
                        {
                            return Json(result);
                        }
                    }                                       
                } 
                #endregion
                
                ViewBag.ListData = dt;

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ListPartial", model);
                }
                else
                {
                    return View(model);
                }

            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();
            }

            return Json(result);
        }

        #endregion

        #region 明细报表
        /// <summary>
        /// 初始化子报表界面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult DetailIndex(DailyMeetingRPTViewModel model)
        {            
            //初始化调用数据查询
            return QueryModuleDetail(model);
        }

        /// <summary>
        /// 日期序列子报表图形设置查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult QueryModuleDetailForCharts(DailyMeetingRPTViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            SortedList<string, string> svalues = new SortedList<string, string>();

            try
            {
                DataTable dt = new DataTable();                
                DataTable dtModuleReportFormat = new DataTable();

                string scolumntext = string.Empty;
                string ssplinetext = string.Empty;
                string scolumnformat = string.Empty;
                string ssplineformat = string.Empty;
                string scolumnLabelsformat = string.Empty;
                string ssplineLabelsformat = string.Empty;
                string sshowcolumnLabels = string.Empty;
                string sshowsplineLabels = string.Empty;
                string scolumnStacking = string.Empty;
                string strSUMCaption = string.Empty;
                string strMemoCaption = string.Empty;
                string sChartShow = string.Empty;
                RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();

                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    //初始化参数
                    p.ReportCode = model.ReportCode;          //报表代码
                    p.StartDate = model.StartDate;            //开始日期
                    p.EndDate = model.EndDate;                //结束日期
                    p.LocationName = model.LocationName;      //车间
                    p.LineCode = model.LineCode;              //线别
                    p.OrderNumber = model.OrderNumber;        //工单代码
                    p.ProductID = model.ProductID;            //产品类型
                    p.MonthDataNumber = model.MonthDataNumber;  //月数据显示量
                    p.YearDataNumber = model.YearDataNumber;    //年数据显示量

                    #region 设置不同报表查询参数
                    switch (p.ReportCode)
                    {
                        case "DAY0201":
                            //p.MonthDataNumber = 1;          //月数据显示量
                            //p.YearDataNumber = 0;           //年数据显示量

                            break;
                        case "DAY0202":
                            p.StartDate = p.EndDate;

                            break;
                    }
                    #endregion

                    MethodReturnResult<DataSet> rst = client.Get(ref p);

                    if (rst.Code > 0)       //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;

                        return Json(result);
                    }
                    else
                    {
                        dtModuleReportFormat = rst.Data.Tables[1];      //报表格式

                        //根据报表格式及数据清单组合数据列表
                        result = CreateTableList(rst, ref dt);

                        if (result.Code > 0)       //产生错误
                        {
                            return Json(result);
                        }
                    }
                }
                                
                //取得图形信息字段
                var reportformatquery = from t in dtModuleReportFormat.AsEnumerable()
                                        group t by new
                                        {
                                            ChartType = t.Field<string>("charttype"),
                                            ChartText = t.Field<string>("charttext"),
                                            //ChartShow = t.Field<string>("chartShow"),
                                            ChartFormat = t.Field<string>("chartformat"),
                                            ShowDataLabels = t.Field<string>("showdataLabels"),
                                            DataLabelsFormat = t.Field<string>("dataLabelsformat"),
                                            Stacking = t.Field<string>("stacking")
                                        } into g
                                        select new
                                        {
                                            ChartType = g.Key.ChartType,
                                            ChartText = g.Key.ChartText,
                                            //ChartShow = g.Key.ChartShow,
                                            ChartFormat = g.Key.ChartFormat,
                                            ShowDataLabels = g.Key.ShowDataLabels,
                                            DataLabelsFormat = g.Key.DataLabelsFormat,
                                            Stacking = g.Key.Stacking
                                        };

                //设置行信息
                foreach (var data in reportformatquery)
                {
                    switch (data.ChartType.ToString())
                    {
                        case "column":
                            scolumntext = data.ChartText;                   //图形说明
                            scolumnformat = data.ChartFormat;               //图形格式
                            sshowcolumnLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
                            scolumnLabelsformat = data.DataLabelsFormat;    //图形标签格式
                            scolumnStacking = data.Stacking;                //图形堆叠属性
                            //sChartShow = data.ChartShow;                    //图形是否显示

                            break;
                        case "line":
                            ssplinetext = data.ChartText;                   //图形说明
                            ssplineformat = data.ChartFormat;               //图形格式
                            sshowsplineLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
                            ssplineLabelsformat = data.DataLabelsFormat;    //图形标签格式
                            //sChartShow = data.ChartShow;                    //图形是否显示

                            break;
                    }
                }
                
                //组装图形XY轴数据
                result = AssembleSplineForModuleDebris(dt, ref svalues);

                if (result.Code > 0)       //产生错误
                {
                    return Json(result);
                }

                //设置报表名称
                svalues.Add("ReportName", model.ReportName);

                //设置图形说明
                svalues.Add("ColumnText", scolumntext);
                svalues.Add("SplineText", ssplinetext);

                //设置图形格式
                svalues.Add("ColumnFormat", scolumnformat);
                svalues.Add("SplineFormat", ssplineformat);

                //设置数据标签说明
                svalues.Add("ShowColumnLabels", sshowcolumnLabels);
                svalues.Add("ShowSplineLabels", sshowsplineLabels);

                //设置数据标签格式
                svalues.Add("ColumnLabelsFormat", scolumnLabelsformat);
                svalues.Add("SplineLabelsFormat", ssplineLabelsformat);

                //设置数据堆叠属性
                svalues.Add("ColumnStacking", scolumnStacking);

                ViewBag.ListData = dt;

                return Json(svalues, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }
        }

        public ActionResult QueryTimeDetailForCharts(string ReportCode, string StartDate, string EndDate, string LocationName, string LineCode, string OrderNumber, string ProductID)
        {
            MethodReturnResult result = new MethodReturnResult();
            SortedList<string, string> svalues = new SortedList<string, string>();

            try
            {
                DataTable dt = new DataTable();
                DataTable dtModuleReportFormat = new DataTable();

                string scolumntext = string.Empty;
                string ssplinetext = string.Empty;
                string scolumnformat = string.Empty;
                string ssplineformat = string.Empty;
                string scolumnLabelsformat = string.Empty;
                string ssplineLabelsformat = string.Empty;
                string sshowcolumnLabels = string.Empty;
                string sshowsplineLabels = string.Empty;
                string scolumnStacking = string.Empty;
                string strSUMCaption = string.Empty;
                string strMemoCaption = string.Empty;
                RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();

                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    //初始化参数
                    p.ReportCode = ReportCode;          //报表代码
                    p.StartDate = StartDate;            //开始日期
                    p.EndDate = EndDate;                //结束日期
                    p.LocationName = LocationName;      //车间
                    p.LineCode = LineCode;              //线别
                    p.OrderNumber = OrderNumber;        //工单代码
                    p.ProductID = ProductID;            //产品类型

                    MethodReturnResult<DataSet> rst = client.Get(ref p);

                    if (rst.Code > 0)       //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;

                        return Json(result);
                    }
                    else
                    {
                        dtModuleReportFormat = rst.Data.Tables[1];      //报表格式

                        //根据报表格式及数据清单组合数据列表
                        result = CreateTableList(rst, ref dt);

                        if (result.Code > 0)       //产生错误
                        {
                            return Json(result);
                        }
                    }
                }

                //取得图形信息字段
                var reportformatquery = from t in dtModuleReportFormat.AsEnumerable()
                                        group t by new
                                        {
                                            ChartType = t.Field<string>("charttype"),
                                            ChartText = t.Field<string>("charttext"),
                                            ChartFormat = t.Field<string>("chartformat"),
                                            ShowDataLabels = t.Field<string>("showdataLabels"),
                                            DataLabelsFormat = t.Field<string>("dataLabelsformat"),
                                            Stacking = t.Field<string>("stacking")
                                        } into g
                                        select new
                                        {
                                            ChartType = g.Key.ChartType,
                                            ChartText = g.Key.ChartText,
                                            ChartFormat = g.Key.ChartFormat,
                                            ShowDataLabels = g.Key.ShowDataLabels,
                                            DataLabelsFormat = g.Key.DataLabelsFormat,
                                            Stacking = g.Key.Stacking
                                        };

                //设置行信息
                foreach (var data in reportformatquery)
                {
                    switch (data.ChartType.ToString())
                    {
                        case "column":
                            scolumntext = data.ChartText;                   //图形说明
                            scolumnformat = data.ChartFormat;               //图形格式
                            sshowcolumnLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
                            scolumnLabelsformat = data.DataLabelsFormat;    //图形标签格式
                            scolumnStacking = data.Stacking;                //图形堆叠属性

                            break;
                        case "line":
                            ssplinetext = data.ChartText;                   //图形说明
                            ssplineformat = data.ChartFormat;               //图形格式
                            sshowsplineLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
                            ssplineLabelsformat = data.DataLabelsFormat;    //图形标签格式

                            break;
                    }
                }

                //组装图形XY轴数据
                result = AssembleSplineForModuleDebris(dt, ref svalues);

                if (result.Code > 0)       //产生错误
                {
                    return Json(result);
                }

                //设置报表名称
                ViewBag.ReportName = "";

                //设置图形说明
                ViewBag.ColumnText = scolumntext;
                ViewBag.SplineText = ssplinetext;

                //设置图形格式
                ViewBag.ColumnFormat = scolumnformat;
                ViewBag.SplineFormat = ssplineformat;

                //设置数据标签说明
                ViewBag.ShowColumnLabels = sshowcolumnLabels;
                ViewBag.ShowSplineLabels = sshowsplineLabels;

                //设置数据标签格式
                ViewBag.ColumnLabelsFormat = scolumnLabelsformat;
                ViewBag.SplineLabelsFormat = ssplineLabelsformat;

                //设置数据堆叠属性
                ViewBag.ColumnStacking = scolumnStacking;

                //数据
                ViewBag.mAxis = svalues["mAxis"];
                ViewBag.nAxis = svalues["nAxis"];

                ViewBag.ListData = dt;

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_DetailListPartial");
                    //return View("DetailIndex");
                }
                else
                {
                    return Json(result);
                }

                //return Json(svalues, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }
        }

        /// <summary>
        /// 日期序列子报表数据列表查询及设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult QueryModuleDetail(DailyMeetingRPTViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                DataTable dt = new DataTable();                     //报表数据列表                
                //string strSUMCaption = string.Empty;
                //string strMemoCaption = string.Empty;
                RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();

                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    //初始化参数
                    p.ReportCode = model.ReportCode;          //报表代码
                    p.StartDate = model.StartDate;            //开始日期
                    p.EndDate = model.EndDate;                //结束日期
                    p.LocationName = model.LocationName;      //车间
                    p.LineCode = model.LineCode;              //线别
                    p.OrderNumber = model.OrderNumber;        //工单代码
                    p.ProductID = model.ProductID;            //产品类型

                    MethodReturnResult<DataSet> rst = client.Get(ref p);

                    if (rst.Code > 0)       //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;

                        return Json(result);
                    }
                    else
                    {
                        //根据报表格式及数据清单组合数据列表
                        result = CreateTableList(rst, ref dt);

                        if (result.Code > 0)       //产生错误
                        {
                            return Json(result);
                        }
                    }
                }
                
                ViewBag.ListData = dt;

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_DetailListPartial", model);
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();
            }

            return Json(result);
        }
                
        public ActionResult QueryTimeDetail(string ReportCode, string StartDate, string EndDate, string LocationName, string LineCode, string OrderNumber, string ProductID, int MonthDataNumber, int YearDataNumber)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                DataTable dt = new DataTable();                     //报表数据列表                
                RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();

                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    //初始化参数
                    p.ReportCode = ReportCode;          //报表代码
                    p.StartDate = StartDate;            //开始日期
                    p.EndDate = EndDate;                //结束日期
                    p.LocationName = LocationName;      //车间
                    p.LineCode = LineCode;              //线别
                    p.OrderNumber = OrderNumber;        //工单代码
                    p.ProductID = ProductID;            //产品类型
                    p.MonthDataNumber = MonthDataNumber;    //月数据显示量
                    p.YearDataNumber = YearDataNumber;      //年数据显示量

                    #region 设置不同报表查询参数
                    switch (p.ReportCode)
                    {
                        case "DAY0201":
                            //p.MonthDataNumber = 1;          //月数据显示量
                            //p.YearDataNumber = 0;           //年数据显示量

                            break;
                        case "DAY0202":
                            p.StartDate = p.EndDate;

                            break;
                    }
                    #endregion

                    MethodReturnResult<DataSet> rst = client.Get(ref p);

                    if (rst.Code > 0)       //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;

                        return Json(result);
                    }
                    else
                    {
                        //根据报表格式及数据清单组合数据列表
                        result = CreateTableList(rst, ref dt);

                        if (result.Code > 0)       //产生错误
                        {
                            return Json(result);
                        }
                    }
                }

                ViewBag.ListData = dt;

                if (Request.IsAjaxRequest())
                {                    
                    return PartialView("_DetailListPartial");
                }
                else
                {
                    //return View(model);
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();
            }

            return Json(result);
        }
        #endregion

        








        #region 数据列表类查询报表
        /// <summary>
        /// 初始化列表查询界面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult ListDetailIndex(DailyMeetingRPTViewModel model)
        {
            return View(model);
        }

        /// <summary>
        /// 列表子报表数据列表查询及设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult QueryModuleList(DailyMeetingRPTViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();            

            try
            {
                DataTable dt = new DataTable();                                 //报表数据列表 
                
                string strSUMCaption = string.Empty;                
                string strMemoCaption = string.Empty;
                RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();    //报表传递参数 

                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    //参数设定
                    p.ReportCode = model.ReportCode;          //报表代码
                    p.StartDate = model.StartDate;            //开始日期
                    p.EndDate = model.EndDate;                //结束日期
                    p.LocationName = model.LocationName;      //车间
                    p.LineCode = model.LineCode;              //线别
                    p.OrderNumber = model.OrderNumber;        //工单代码
                    p.ProductID = model.ProductID;            //产品类型
                    p.PageNo = model.PageNo + 1;              //页数
                    p.PageSize = model.PageSize;              //单页面大小

                    //取得报表数据
                    MethodReturnResult<DataSet> rst = client.Get(ref p );

                    if (rst.Code > 0)       //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;

                        return Json(result);
                    }
                    else
                    {
                        ////根据报表格式及数据清单组合数据列表
                        //result = CreateTableList(rst, ref dt);

                        //if (result.Code > 0)       //产生错误
                        //{
                        //    return Json(result);
                        //}

                        dt = rst.Data.Tables[0];                    //报表数据
                    }
                }

                ViewBag.ListData = dt;
                               
                ViewBag.StartDate = model.StartDate;            //开始日期
                ViewBag.EndDate = model.EndDate;                //结束日期
                ViewBag.LocationName = model.LocationName;      //车间
                ViewBag.LineCode = model.LineCode;              //线别
                ViewBag.OrderNumber = model.OrderNumber;        //工单代码
                ViewBag.ProductID = model.ProductID;            //产品类型

                ViewBag.PagingConfig = new PagingConfig()
                {
                    PageNo = model.PageNo,                      //页码
                    PageSize = model.PageSize,                  //单页面大小
                    Records = p.Records                         //总数据记录

                };

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ListDetailListPartial", model);
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();
            }

            return Json(result);
        }
        #endregion
        
        #region 小时序列明细报表
        /// <summary>
        /// 计算小时报表界面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult HourDetailIndex(DailyMeetingRPTViewModel model)
        {
            DailyMeetingRPTViewModel HourDetailModel = new DailyMeetingRPTViewModel();
            string strDateTime = "";

            //复制原Model中数据
            UpdateModel(HourDetailModel);

            //处理日期开始时间（默认8：00开始）
            strDateTime = model.StartDate.Substring(0, 10) + " 08:00:00";
            strDateTime = Convert.ToDateTime(strDateTime).ToString("yyyy-MM-dd HH:mm:ss");
            HourDetailModel.StartDate = strDateTime;              //开始日期

            ////处理结束日期
            strDateTime = model.EndDate.Substring(0, 10) + " 08:00:00";
            strDateTime = Convert.ToDateTime(strDateTime).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");            
            HourDetailModel.EndDate = strDateTime;    //结束日期            

            //清除原有Model数据便于装载新数据
            ModelState.Clear();

            return View(HourDetailModel);
        }

        /// <summary>
        /// 小时序列子报表图形设置查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult QueryModuleHourForCharts(DailyMeetingRPTViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            SortedList<string, string> svalues = new SortedList<string, string>();

            try
            {
                DataTable dt = new DataTable();
                DataTable dtModuleReportFormat = new DataTable();

                string scolumntext = string.Empty;
                string ssplinetext = string.Empty;
                string scolumnformat = string.Empty;
                string ssplineformat = string.Empty;
                string scolumnLabelsformat = string.Empty;
                string ssplineLabelsformat = string.Empty;
                string sshowcolumnLabels = string.Empty;
                string sshowsplineLabels = string.Empty;
                string scolumnStacking = string.Empty;
                string strSUMCaption = string.Empty;
                string strMemoCaption = string.Empty;
                RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();

                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    //初始化参数
                    p.ReportCode = model.ReportCode;          //报表代码
                    p.StartDate = model.StartDate;            //开始日期
                    p.EndDate = model.EndDate;                //结束日期
                    p.LocationName = model.LocationName;      //车间
                    p.LineCode = model.LineCode;              //线别
                    p.OrderNumber = model.OrderNumber;        //工单代码
                    p.ProductID = model.ProductID;            //产品类型

                    MethodReturnResult<DataSet> rst = client.Get(ref p);

                    if (rst.Code > 0)       //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;

                        return Json(result);
                    }
                    else
                    {
                        dtModuleReportFormat = rst.Data.Tables[1];      //报表格式

                        //根据报表格式及数据清单组合数据列表
                        //result = CreateHourTableList(rst, ref dt);
                        result = CreateTableList(rst, ref dt);

                        if (result.Code > 0)       //产生错误
                        {
                            return Json(result);
                        }
                    }
                }

                //取得图形信息字段
                var reportformatquery = from t in dtModuleReportFormat.AsEnumerable()
                                        group t by new
                                        {
                                            ChartType = t.Field<string>("charttype"),
                                            ChartText = t.Field<string>("charttext"),
                                            ChartFormat = t.Field<string>("chartformat"),
                                            ShowDataLabels = t.Field<string>("showdataLabels"),
                                            DataLabelsFormat = t.Field<string>("dataLabelsformat"),
                                            Stacking = t.Field<string>("stacking")
                                        } into g
                                        select new
                                        {
                                            ChartType = g.Key.ChartType,
                                            ChartText = g.Key.ChartText,
                                            ChartFormat = g.Key.ChartFormat,
                                            ShowDataLabels = g.Key.ShowDataLabels,
                                            DataLabelsFormat = g.Key.DataLabelsFormat,
                                            Stacking = g.Key.Stacking
                                        };

                //设置行信息
                foreach (var data in reportformatquery)
                {
                    switch (data.ChartType.ToString())
                    {
                        case "column":
                            scolumntext = data.ChartText;                   //图形说明
                            scolumnformat = data.ChartFormat;               //图形格式
                            sshowcolumnLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
                            scolumnLabelsformat = data.DataLabelsFormat;    //图形标签格式
                            scolumnStacking = data.Stacking;                //图形堆叠属性

                            break;
                        case "line":
                            ssplinetext = data.ChartText;                   //图形说明
                            ssplineformat = data.ChartFormat;               //图形格式
                            sshowsplineLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
                            ssplineLabelsformat = data.DataLabelsFormat;    //图形标签格式

                            break;
                    }
                }

                //组装图形XY轴数据
                result = AssembleSplineForModuleDebris(dt, ref svalues);

                if (result.Code > 0)       //产生错误
                {
                    return Json(result);
                }

                //设置报表名称
                svalues.Add("ReportName", model.ReportName);

                //设置图形说明
                svalues.Add("ColumnText", scolumntext);
                svalues.Add("SplineText", ssplinetext);

                //设置图形格式
                svalues.Add("ColumnFormat", scolumnformat);
                svalues.Add("SplineFormat", ssplineformat);

                //设置数据标签说明
                svalues.Add("ShowColumnLabels", sshowcolumnLabels);
                svalues.Add("ShowSplineLabels", sshowsplineLabels);

                //设置数据标签格式
                svalues.Add("ColumnLabelsFormat", scolumnLabelsformat);
                svalues.Add("SplineLabelsFormat", ssplineLabelsformat);

                //设置数据堆叠属性
                svalues.Add("ColumnStacking", scolumnStacking);

                ViewBag.ListData = dt;

                return Json(svalues, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }
        }

        /// <summary>
        /// 小时序列子报表数据列表查询及设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult QueryModuleHour(DailyMeetingRPTViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                DataTable dt = new DataTable();                                 //报表数据列表                
                RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();    //参数对象

                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    //初始化参数
                    p.ReportCode = model.ReportCode;          //报表代码
                    p.StartDate = model.StartDate;            //开始日期
                    p.EndDate = model.EndDate;                //结束日期
                    p.LocationName = model.LocationName;      //车间
                    p.LineCode = model.LineCode;              //线别
                    p.OrderNumber = model.OrderNumber;        //工单代码
                    p.ProductID = model.ProductID;            //产品类型

                    MethodReturnResult<DataSet> rst = client.Get(ref p);

                    if (rst.Code > 0)       //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;

                        return Json(result);
                    }
                    else
                    {
                        //根据报表格式及数据清单组合数据列表
                        result = CreateTableList(rst, ref dt);

                        if (result.Code > 0)       //产生错误
                        {
                            return Json(result);
                        }
                    }
                }

                ViewBag.ListData = dt;

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_HourDetailListPartial", model);
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();
            }

            return Json(result);
        }

        /// <summary>
        /// 创建小时数据列表
        /// </summary>
        /// <param name="dtSet">取得数据集（报表数据、报表格式）</param>
        /// <param name="dt">返回报表数据列表</param>
        /// <returns></returns>
        //public MethodReturnResult CreateHourTableList(MethodReturnResult<DataSet> dtSet, ref DataTable dt)
        //{
        //    MethodReturnResult result = new MethodReturnResult();

        //    try
        //    {
        //        result.Code = 0;
        //        DataTable dtModuleHourData = new DataTable();      //报表数据
        //        DataTable dtModuleReportFormat = new DataTable();   //报表格式

        //        #region 增加项目固定列
        //        //增加子报表链接字段
        //        DataColumn dcStatus = new DataColumn("ItemCode");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加项目名称字段
        //        dcStatus = new DataColumn("ItemName");
        //        dcStatus.Caption = "项目";
        //        dt.Columns.Add(dcStatus);

        //        //增加子报表链接字段
        //        dcStatus = new DataColumn("LINK");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加字段类型
        //        dcStatus = new DataColumn("DataType");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加子报表代码
        //        dcStatus = new DataColumn("ChildRPCode");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加子报表名称
        //        dcStatus = new DataColumn("ChildRPName");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加累计资源字段
        //        dcStatus = new DataColumn("SUM");
        //        dcStatus.Caption = "合计";
        //        dcStatus.DefaultValue = 0;
        //        dt.Columns.Add(dcStatus);

        //        //增加图形类型
        //        dcStatus = new DataColumn("ChartType");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加图形分组
        //        dcStatus = new DataColumn("GroupNo");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加数据显示格式
        //        dcStatus = new DataColumn("DataLabelsFormat");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加日期子报表链接字段
        //        dcStatus = new DataColumn("DetailLINK");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加日期子报表代码
        //        dcStatus = new DataColumn("DetailChildRPCode");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加日期子报表名称
        //        dcStatus = new DataColumn("DetailChildRPName");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        #endregion

        //        #region 创建动态小时时间列
        //        dtModuleHourData = dtSet.Data.Tables[0];             //报表数据
        //        dtModuleReportFormat = dtSet.Data.Tables[1];          //报表格式

        //        var query = from t in dtModuleHourData.AsEnumerable()
        //                    where (t.Field<string>("DataType") == "H")
        //                    group t by new { t1 = t.Field<string>("rp_datetime") }
        //                        into m
        //                        select new
        //                        {
        //                            RPDateTime = m.First().Field<string>("rp_datetime")
        //                        } into r
        //                        orderby r.RPDateTime
        //                        select r;

        //        //取得时间列行数
        //        int iDateColumns = query.Count();
        //        string sColumnCode = "";
        //        string sColumnName = "";

        //        //设置日期列
        //        foreach (var data in query)
        //        {
        //            sColumnCode = "data" + Convert.ToDateTime(data.RPDateTime).ToString("MMddHH");
        //            sColumnName = Convert.ToDateTime(data.RPDateTime).ToString("M.dd HH");

        //            dcStatus = new DataColumn(sColumnCode);
        //            dcStatus.Caption = sColumnName;
        //            dcStatus.DefaultValue = 0;

        //            dt.Columns.Add(dcStatus);
        //        }

        //        //增加计算说明字段
        //        dcStatus = new DataColumn("MEMO");
        //        dcStatus.Caption = "说明";
        //        dt.Columns.Add(dcStatus);
        //        #endregion

        //        #region 定义行
        //        //取得行信息字段
        //        var rowquery = from t in dtModuleReportFormat.AsEnumerable()
        //                       select new
        //                       {
        //                           rowsnum = t.Field<System.Int16>("rowsnum"),                  //行号
        //                           Item_code = t.Field<string>("item_code"),                    //项目号
        //                           Item_name = t.Field<string>("item_name"),                    //项目名称
        //                           Memo = t.Field<string>("Memo"),                              //备注
        //                           RPTURL = t.Field<string>("reportURL"),                       //子报表链接
        //                           ChildRPCode = t.Field<string>("childrpcode"),                //子报表代码
        //                           ChildRPName = t.Field<string>("childrpname"),                //子报表名称
        //                           DataType = t.Field<string>("datatype"),                      //数据类型（合计、子数据）
        //                           ChartType = t.Field<string>("charttype"),                    //图形类型
        //                           IsDetial = t.Field<System.Int16>("isdetial"),                //明细项目数据标识（1 - 明细数据）
        //                           GroupNo = t.Field<string>("groupno"),                        //数据分组代码（针对图形控件显示）
        //                           DataLabelsFormat = t.Field<string>("dataLabelsformat"),      //数据标签显示格式
        //                           DetailRPTURL = t.Field<string>("DetailreportURL"),           //日期子报表链接
        //                           DetailChildRPCode = t.Field<string>("Detailchildrpcode"),    //日期子报表代码
        //                           DetailChildRPName = t.Field<string>("Detailchildrpname")     //日期子报表名称
        //                       } into r
        //                       orderby r.rowsnum
        //                       select r;

        //        DataRow dr = dt.NewRow();

        //        //设置行信息
        //        foreach (var data in rowquery)
        //        {
        //            if (data.IsDetial == 1)
        //            {
        //                //处理明细数据
        //                var detialquery = from t in dtModuleHourData.AsEnumerable()
        //                                  where t.Field<string>("ItemCode") == data.Item_code
        //                                  group t by new
        //                                  {
        //                                      DetailItem = t.Field<string>("DetailItem"),
        //                                      DetailTitle = t.Field<string>("DetailTitle")
        //                                  } into g
        //                                  select new
        //                                  {
        //                                      DetailItem = g.Key.DetailItem,
        //                                      DetailTitle = g.Key.DetailTitle
        //                                  } into r
        //                                  orderby r.DetailItem
        //                                  select r;

        //                foreach (var detaildata in detialquery)
        //                {
        //                    dr = dt.NewRow();

        //                    dr["ItemCode"] = "Detail" + detaildata.DetailItem;      //项目号
        //                    dr["ItemName"] = detaildata.DetailTitle;                //项目名称
        //                    dr["LINK"] = data.RPTURL;                               //合计栏链接
        //                    dr["ChildRPCode"] = data.ChildRPCode;                   //合计栏子报表代码
        //                    dr["ChildRPName"] = data.ChildRPName;                   //合计栏子报表名称
        //                    dr["DataType"] = data.DataType;                         //数据类型（合计、子数据）                            
        //                    dr["MEMO"] = data.Memo;                                 //备注
        //                    dr["ChartType"] = data.ChartType;                       //图形控件类型
        //                    dr["GroupNo"] = data.GroupNo;                           //数据分组代码（针对图形控件显示）
        //                    dr["DataLabelsFormat"] = data.DataLabelsFormat;         //图形控件标签显示格式
        //                    dr["DetailLINK"] = data.DetailRPTURL;                   //日期栏链接
        //                    dr["DetailChildRPCode"] = data.DetailChildRPCode;       //日期栏子报表代码
        //                    dr["DetailChildRPName"] = data.DetailChildRPName;       //日期栏子报表名称

        //                    dt.Rows.Add(dr);
        //                }
        //            }
        //            else
        //            {
        //                dr = dt.NewRow();

        //                dr["ItemCode"] = data.Item_code;                        //项目代码
        //                dr["ItemName"] = data.Item_name;                        //项目名称
        //                dr["DataType"] = data.DataType;                         //数据类型（合计、子数据）
        //                dr["LINK"] = data.RPTURL;                               //合计栏链接 
        //                dr["ChildRPCode"] = data.ChildRPCode;                   //合计栏子报表代码
        //                dr["ChildRPName"] = data.ChildRPName;                   //合计栏子报表名称
        //                dr["MEMO"] = data.Memo;                                 //备注
        //                dr["ChartType"] = data.ChartType;                       //图形类型
        //                dr["GroupNo"] = data.GroupNo;                           //数据分组代码（针对图形控件显示）
        //                dr["DataLabelsFormat"] = data.DataLabelsFormat;         //数据标签显示格式
        //                dr["DetailLINK"] = data.DetailRPTURL;                   //日期栏链接
        //                dr["DetailChildRPCode"] = data.DetailChildRPCode;       //日期栏子报表代码
        //                dr["DetailChildRPName"] = data.DetailChildRPName;       //日期栏子报表名称

        //                dt.Rows.Add(dr);
        //            }
        //        }
        //        #endregion

        //        #region 填充数据
        //        string strItemCode = "";
        //        string strValues = "";
        //        string strDataType = "";
        //        DateTime dtRPDateTime;
        //        int selOfRow = 0;
        //        string strFieldDataType = "";
        //        string strDataLabelsFormat = "";
        //        string strDetail = "";

        //        for (int i = 0; i < dtModuleHourData.Rows.Count; i++)
        //        {
        //            strItemCode = dtModuleHourData.Rows[i]["ItemCode"].ToString();                 //项目代码
        //            strValues = dtModuleHourData.Rows[i]["sumQty"].ToString();                     //项目值
        //            strDataType = dtModuleHourData.Rows[i]["DataType"].ToString();                 //项目数据类型
        //            dtRPDateTime = Convert.ToDateTime(dtModuleHourData.Rows[i]["rp_datetime"]);    //项目数据日期
        //            strDetail = dtModuleHourData.Rows[i]["DetailItem"].ToString();                 //明细项目标识

        //            //取得项目所在行数及相关信息
        //            var ItemRowquery = from t in dtModuleReportFormat.AsEnumerable()
        //                               where (t.Field<string>("item_code") == strItemCode)
        //                               select new
        //                               {
        //                                   rowsnum = t.Field<System.Int16>("rowsnum"),
        //                                   datatype = t.Field<string>("datatype"),
        //                                   dataLabelsformat = t.Field<string>("dataLabelsformat")
        //                               };

        //            selOfRow = ItemRowquery.FirstOrDefault().rowsnum;
        //            strFieldDataType = ItemRowquery.FirstOrDefault().datatype;
        //            strDataLabelsFormat = ItemRowquery.FirstOrDefault().dataLabelsformat;       //数据格式

        //            if (strDetail != "")    //明细数据
        //            {
        //                strItemCode = "Detail" + strDetail;
        //            }

        //            DataRow[] ItemRow = dt.Select("ItemCode = '" + strItemCode + "'");

        //            if (ItemRow.GetLength(0) > 0)
        //            {
        //                //设置数据类型格式
        //                switch (strFieldDataType)
        //                {
        //                    case "int":
        //                        //处理空值为0
        //                        if (strValues == "")
        //                        {
        //                            strValues = "0";
        //                        }
        //                        else
        //                        {
        //                            strValues = Convert.ToDouble(strValues).ToString();
        //                        }
                                
        //                        break;
        //                    case "dec":
        //                        break;
        //                }

        //                //设置数据格式
        //                if (strDataLabelsFormat.Length > 1)
        //                {
        //                    if (strDataLabelsFormat.Substring(strDataLabelsFormat.Length - 1, 1) == "%")
        //                    {
        //                        strValues = strValues + "%";
        //                    }
        //                }

        //                //数据填充
        //                switch (strDataType)
        //                {
        //                    case "D":       //日数据
        //                        //计算数据列名称
        //                        sColumnCode = "data" + dtRPDateTime.ToString("MMdd");

        //                        ItemRow[0][sColumnCode] = strValues;
        //                        break;
        //                    case "H":       //小时数据
        //                        //计算数据列名称
        //                        sColumnCode = "data" + dtRPDateTime.ToString("MMddHH");

        //                        ItemRow[0][sColumnCode] = strValues;
        //                        break;
        //                    case "SUM":     //累计数据
        //                        ItemRow[0]["SUM"] = strValues;
        //                        break;
        //                }
        //            }
        //        }

        //        #endregion

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        result.Code = 1000;
        //        result.Message = e.Message;
        //        result.Detail = e.ToString();

        //        return result;
        //    }
        //}        
        
        #endregion

        #region 项目类明细报表
        /// <summary>
        /// 初始化子报表界面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //public ActionResult ItemIndex(DailyMeetingRPTViewModel model)
        //{
        //    ViewBag.Title = model.ReportName;

        //    return View(model);
        //}

        /// <summary>
        /// 日期序列子报表图形设置查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult QueryModuleItemForCharts(DailyMeetingRPTViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            SortedList<string, string> svalues = new SortedList<string, string>();

            try
            {
                DataTable dt = new DataTable();
                DataTable dtModuleReportFormat = new DataTable();

                string scolumntext = string.Empty;
                string ssplinetext = string.Empty;
                string scolumnformat = string.Empty;
                string ssplineformat = string.Empty;
                string scolumnLabelsformat = string.Empty;
                string ssplineLabelsformat = string.Empty;
                string sshowcolumnLabels = string.Empty;
                string sshowsplineLabels = string.Empty;
                string scolumnStacking = string.Empty;
                string strSUMCaption = string.Empty;
                string strMemoCaption = string.Empty;
                RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();

                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    //初始化参数
                    p.ReportCode = model.ReportCode;          //报表代码
                    p.StartDate = model.StartDate;            //开始日期
                    p.EndDate = model.EndDate;                //结束日期
                    p.LocationName = model.LocationName;      //车间
                    p.LineCode = model.LineCode;              //线别
                    p.OrderNumber = model.OrderNumber;        //工单代码
                    p.ProductID = model.ProductID;            //产品类型

                    MethodReturnResult<DataSet> rst = client.Get(ref p);

                    if (rst.Code > 0)       //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;

                        return Json(result);
                    }
                    else
                    {
                        dtModuleReportFormat = rst.Data.Tables[1];      //报表格式

                        //根据报表格式及数据清单组合数据列表
                        result = CreateItemTableList(rst, ref dt);

                        if (result.Code > 0)       //产生错误
                        {
                            return Json(result);
                        }
                    }
                }

                //取得图形信息字段
                var reportformatquery = from t in dtModuleReportFormat.AsEnumerable()
                                        group t by new
                                        {
                                            ChartType = t.Field<string>("charttype"),
                                            ChartText = t.Field<string>("charttext"),
                                            ChartFormat = t.Field<string>("chartformat"),
                                            ShowDataLabels = t.Field<string>("showdataLabels"),
                                            DataLabelsFormat = t.Field<string>("dataLabelsformat"),
                                            Stacking = t.Field<string>("stacking")
                                        } into g
                                        select new
                                        {
                                            ChartType = g.Key.ChartType,
                                            ChartText = g.Key.ChartText,
                                            ChartFormat = g.Key.ChartFormat,
                                            ShowDataLabels = g.Key.ShowDataLabels,
                                            DataLabelsFormat = g.Key.DataLabelsFormat,
                                            Stacking = g.Key.Stacking
                                        };

                //设置行信息
                foreach (var data in reportformatquery)
                {
                    switch (data.ChartType.ToString())
                    {
                        case "column":
                            scolumntext = data.ChartText;                   //图形说明
                            scolumnformat = data.ChartFormat;               //图形格式
                            sshowcolumnLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
                            scolumnLabelsformat = data.DataLabelsFormat;    //图形标签格式
                            scolumnStacking = data.Stacking;                //图形堆叠属性

                            break;
                        case "line":
                            ssplinetext = data.ChartText;                   //图形说明
                            ssplineformat = data.ChartFormat;               //图形格式
                            sshowsplineLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
                            ssplineLabelsformat = data.DataLabelsFormat;    //图形标签格式

                            break;
                    }
                }

                //组装图形XY轴数据
                result = AssembleSplineForModuleDebris(dt, ref svalues);

                if (result.Code > 0)       //产生错误
                {
                    return Json(result);
                }

                //设置报表名称
                svalues.Add("ReportName", model.ReportName);

                //设置图形说明
                svalues.Add("ColumnText", scolumntext);
                svalues.Add("SplineText", ssplinetext);

                //设置图形格式
                svalues.Add("ColumnFormat", scolumnformat);
                svalues.Add("SplineFormat", ssplineformat);

                //设置数据标签说明
                svalues.Add("ShowColumnLabels", sshowcolumnLabels);
                svalues.Add("ShowSplineLabels", sshowsplineLabels);

                //设置数据标签格式
                svalues.Add("ColumnLabelsFormat", scolumnLabelsformat);
                svalues.Add("SplineLabelsFormat", ssplineLabelsformat);

                //设置数据堆叠属性
                svalues.Add("ColumnStacking", scolumnStacking);

                ViewBag.ListData = dt;

                return Json(svalues, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }
        }

        /// <summary>
        /// 日期序列子报表数据列表查询及设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult QueryModuleItem(DailyMeetingRPTViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                DataTable dt = new DataTable();                     //报表数据列表                
                string strSUMCaption = string.Empty;
                string strMemoCaption = string.Empty;
                RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();

                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    //初始化参数
                    p.ReportCode = model.ReportCode;          //报表代码
                    p.StartDate = model.StartDate;            //开始日期
                    p.EndDate = model.EndDate;                //结束日期
                    p.LocationName = model.LocationName;      //车间
                    p.LineCode = model.LineCode;              //线别
                    p.OrderNumber = model.OrderNumber;        //工单代码
                    p.ProductID = model.ProductID;            //产品类型

                    MethodReturnResult<DataSet> rst = client.Get(ref p);

                    if (rst.Code > 0)       //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;

                        return Json(result);
                    }
                    else
                    {
                        //根据报表格式及数据清单组合数据列表
                        result = CreateTableList(rst, ref dt);

                        if (result.Code > 0)       //产生错误
                        {
                            return Json(result);
                        }
                    }
                }

                ViewBag.ListData = dt;

                ViewBag.StartDate = model.StartDate;            //开始日期
                ViewBag.EndDate = model.EndDate;                //结束日期
                ViewBag.LocationName = model.LocationName;      //车间
                ViewBag.LineCode = model.LineCode;              //线别
                ViewBag.OrderNumber = model.OrderNumber;        //工单代码
                ViewBag.ProductID = model.ProductID;            //产品类型

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_DetailListPartial", model);
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();
            }

            return Json(result);
        }

        /// <summary>
        /// 项目类数据整理
        /// </summary>
        /// <param name="dtSet"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public MethodReturnResult CreateItemTableList(MethodReturnResult<DataSet> dtSet, ref DataTable dt)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                result.Code = 0;
                DataTable dtModuleDailyData = new DataTable();      //报表数据
                DataTable dtModuleReportFormat = new DataTable();   //报表格式

                #region 增加项目固定列
                //增加子报表链接字段
                DataColumn dcStatus = new DataColumn("ItemCode");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加项目名称字段
                dcStatus = new DataColumn("ItemName");
                dcStatus.Caption = "项目";
                dt.Columns.Add(dcStatus);

                //增加子报表链接字段
                dcStatus = new DataColumn("LINK");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加字段类型
                dcStatus = new DataColumn("DataType");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加子报表代码
                dcStatus = new DataColumn("ChildRPCode");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加子报表名称
                dcStatus = new DataColumn("ChildRPName");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加累计资源字段
                dcStatus = new DataColumn("SUM");
                dcStatus.Caption = "合计";
                dcStatus.DefaultValue = 0;
                dt.Columns.Add(dcStatus);

                //增加图形类型
                dcStatus = new DataColumn("ChartType");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加图形分组
                dcStatus = new DataColumn("GroupNo");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加数据显示格式
                dcStatus = new DataColumn("DataLabelsFormat");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加日期子报表链接字段
                dcStatus = new DataColumn("DetailLINK");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加日期子报表代码
                dcStatus = new DataColumn("DetailChildRPCode");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //增加日期子报表名称
                dcStatus = new DataColumn("DetailChildRPName");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                #endregion

                #region 创建动态时间列
                dtModuleDailyData = dtSet.Data.Tables[0];             //报表数据
                dtModuleReportFormat = dtSet.Data.Tables[1];          //报表格式

                var query = from t in dtModuleDailyData.AsEnumerable()
                            where (t.Field<string>("DataType") == "D")
                            group t by new { t1 = t.Field<string>("rp_datetime") }
                                into m
                                select new
                                {
                                    RPDateTime = m.First().Field<string>("rp_datetime")
                                } into r
                                orderby r.RPDateTime
                                select r;

                //取得时间列行数
                int iDateColumns = query.Count();
                string sColumnCode = "";
                string sColumnName = "";

                //设置日期列
                foreach (var data in query)
                {
                    sColumnCode = "data" + Convert.ToDateTime(data.RPDateTime).ToString("MMdd");
                    sColumnName = Convert.ToDateTime(data.RPDateTime).ToString("M月dd日");

                    dcStatus = new DataColumn(sColumnCode);
                    dcStatus.Caption = sColumnName;
                    dcStatus.DefaultValue = 0;

                    dt.Columns.Add(dcStatus);
                }

                //增加计算说明字段
                dcStatus = new DataColumn("MEMO");
                dcStatus.Caption = "说明";
                dt.Columns.Add(dcStatus);
                #endregion

                #region 定义行
                //取得行信息字段
                var rowquery = from t in dtModuleReportFormat.AsEnumerable()
                               select new
                               {
                                   rowsnum = t.Field<System.Int16>("rowsnum"),                  //行号
                                   Item_code = t.Field<string>("item_code"),                    //项目号
                                   Item_name = t.Field<string>("item_name"),                    //项目名称
                                   Memo = t.Field<string>("Memo"),                              //备注
                                   RPTURL = t.Field<string>("reportURL"),                       //子报表链接
                                   ChildRPCode = t.Field<string>("childrpcode"),                //子报表代码
                                   ChildRPName = t.Field<string>("childrpname"),                //子报表名称
                                   DataType = t.Field<string>("datatype"),                      //数据类型（合计、子数据）
                                   ChartType = t.Field<string>("charttype"),                    //图形类型
                                   IsDetial = t.Field<System.Int16>("isdetial"),                //明细项目数据标识（1 - 明细数据）
                                   GroupNo = t.Field<string>("groupno"),                        //数据分组代码（针对图形控件显示）
                                   DataLabelsFormat = t.Field<string>("dataLabelsformat"),      //数据标签显示格式
                                   DetailRPTURL = t.Field<string>("DetailreportURL"),           //日期子报表链接
                                   DetailChildRPCode = t.Field<string>("Detailchildrpcode"),    //日期子报表代码
                                   DetailChildRPName = t.Field<string>("Detailchildrpname")     //日期子报表名称
                               } into r
                               orderby r.rowsnum
                               select r;

                DataRow dr = dt.NewRow();

                //设置行信息
                foreach (var data in rowquery)
                {
                    if (data.IsDetial == 1)
                    {
                        //处理明细数据
                        var detialquery = from t in dtModuleDailyData.AsEnumerable()
                                          where t.Field<string>("ItemCode") == data.Item_code
                                          group t by new
                                          {
                                              DetailItem = t.Field<string>("DetailItem"),
                                              DetailTitle = t.Field<string>("DetailTitle")
                                          } into g
                                          select new
                                          {
                                              DetailItem = g.Key.DetailItem,
                                              DetailTitle = g.Key.DetailTitle
                                          } into r
                                          orderby r.DetailItem
                                          select r;

                        foreach (var detaildata in detialquery)
                        {
                            dr = dt.NewRow();

                            dr["ItemCode"] = "Detail" + detaildata.DetailItem;      //项目号
                            dr["ItemName"] = detaildata.DetailTitle;                //项目名称
                            dr["LINK"] = data.RPTURL;                               //合计栏链接
                            dr["ChildRPCode"] = data.ChildRPCode;                   //合计栏子报表代码
                            dr["ChildRPName"] = data.ChildRPName;                   //合计栏子报表名称
                            dr["DataType"] = data.DataType;                         //数据类型（合计、子数据）                            
                            dr["MEMO"] = data.Memo;                                 //备注
                            dr["ChartType"] = data.ChartType;                       //图形控件类型
                            dr["GroupNo"] = data.GroupNo;                           //数据分组代码（针对图形控件显示）
                            dr["DataLabelsFormat"] = data.DataLabelsFormat;         //图形控件标签显示格式
                            dr["DetailLINK"] = data.DetailRPTURL;                   //日期栏链接
                            dr["DetailChildRPCode"] = data.DetailChildRPCode;       //日期栏子报表代码
                            dr["DetailChildRPName"] = data.DetailChildRPName;       //日期栏子报表名称

                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr = dt.NewRow();

                        dr["ItemCode"] = data.Item_code;                        //项目代码
                        dr["ItemName"] = data.Item_name;                        //项目名称
                        dr["DataType"] = data.DataType;                         //数据类型（合计、子数据）
                        dr["LINK"] = data.RPTURL;                               //合计栏链接 
                        dr["ChildRPCode"] = data.ChildRPCode;                   //合计栏子报表代码
                        dr["ChildRPName"] = data.ChildRPName;                   //合计栏子报表名称
                        dr["MEMO"] = data.Memo;                                 //备注
                        dr["ChartType"] = data.ChartType;                       //图形类型
                        dr["GroupNo"] = data.GroupNo;                           //数据分组代码（针对图形控件显示）
                        dr["DataLabelsFormat"] = data.DataLabelsFormat;         //数据标签显示格式
                        dr["DetailLINK"] = data.DetailRPTURL;                   //日期栏链接
                        dr["DetailChildRPCode"] = data.DetailChildRPCode;       //日期栏子报表代码
                        dr["DetailChildRPName"] = data.DetailChildRPName;       //日期栏子报表名称

                        dt.Rows.Add(dr);
                    }
                }
                #endregion

                #region 填充数据
                string strItemCode = "";
                string strValues = "";
                string strDataType = "";
                DateTime dtRPDateTime;
                int selOfRow = 0;
                string strFieldDataType = "";
                string strDataLabelsFormat = "";
                string strDetail = "";

                for (int i = 0; i < dtModuleDailyData.Rows.Count; i++)
                {
                    strItemCode = dtModuleDailyData.Rows[i]["ItemCode"].ToString();                 //项目代码
                    strValues = dtModuleDailyData.Rows[i]["sumQty"].ToString();                     //项目值
                    strDataType = dtModuleDailyData.Rows[i]["DataType"].ToString();                 //项目数据类型
                    dtRPDateTime = Convert.ToDateTime(dtModuleDailyData.Rows[i]["rp_datetime"]);    //项目数据日期
                    strDetail = dtModuleDailyData.Rows[i]["DetailItem"].ToString();                 //明细项目标识

                    //取得项目所在行数及相关信息
                    var ItemRowquery = from t in dtModuleReportFormat.AsEnumerable()
                                       where (t.Field<string>("item_code") == strItemCode)
                                       select new
                                       {
                                           rowsnum = t.Field<System.Int16>("rowsnum"),
                                           datatype = t.Field<string>("datatype"),
                                           dataLabelsformat = t.Field<string>("dataLabelsformat")
                                       };

                    selOfRow = ItemRowquery.FirstOrDefault().rowsnum;
                    strFieldDataType = ItemRowquery.FirstOrDefault().datatype;
                    strDataLabelsFormat = ItemRowquery.FirstOrDefault().dataLabelsformat;       //数据格式

                    if (strDetail != "")    //明细数据
                    {
                        strItemCode = "Detail" + strDetail;
                    }

                    DataRow[] ItemRow = dt.Select("ItemCode = '" + strItemCode + "'");

                    if (ItemRow.GetLength(0) > 0)
                    {
                        //设置数据类型格式
                        switch (strFieldDataType)
                        {
                            case "int":
                                //处理空值为0
                                if (strValues == "")
                                {
                                    strValues = "0";
                                }
                                else
                                {
                                    strValues = Convert.ToDouble(strValues).ToString();
                                }
                                
                                break;
                            case "dec":
                                break;
                        }

                        //设置数据格式
                        if (strDataLabelsFormat.Length > 1)
                        {
                            if (strDataLabelsFormat.Substring(strDataLabelsFormat.Length - 1, 1) == "%")
                            {
                                strValues = strValues + "%";
                            }
                        }

                        //数据填充
                        switch (strDataType)
                        {
                            case "D":       //日数据
                                //计算数据列名称
                                sColumnCode = "data" + dtRPDateTime.ToString("MMdd");

                                ItemRow[0][sColumnCode] = strValues;
                                break;
                            case "SUM":     //累计数据
                                ItemRow[0]["SUM"] = strValues;
                                break;
                        }
                    }
                }

                #endregion

                return result;
            }
            catch (Exception e)
            {
                result.Code = 1000;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return result;
            }
        }
        #endregion

        #region 日运营报表平台主界面
        // GET: RPT/RPTDailyData
        public ActionResult DailyReportSYSIndex()
        {
            //DailyMeetingRPTViewModel model = new DailyMeetingRPTViewModel
            //{
            //    //初始化参数
            //    ReportCode = "DAY01",         //报表代码

            //    StartDate = System.DateTime.Now.AddDays(-8).ToString("yyyy-MM-dd"),   //初始化开始日期
            //    EndDate = System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")      //初始化结束日期
            //};

            DailyMeetingRPTViewModel model = new DailyMeetingRPTViewModel
            {
                //初始化参数
                ReportCode = "DAY01",         //报表代码

                StartDate = System.DateTime.Now.AddDays(-8).ToString("yyyy-MM-dd"),   //初始化开始日期
                EndDate = System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")      //初始化结束日期
                //StartDate = System.DateTime.Now.AddDays(1 - System.DateTime.Now.Day).ToString("yyyy-MM-dd"),    //初始化开始日期
                //EndDate = System.DateTime.Now.ToString("yyyy-MM-dd")                                            //初始化结束日期
            };

            return View("DailyReportSYSIndex",model);

            //using (LotQueryServiceClient client = new LotQueryServiceClient())
            //{
            //    await Task.Run(() =>
            //    {
            //        MethodReturnResult<Lot> result = client.Get("1241517210001");

            //        if (result.Code == 0)
            //        {
            //            ViewBag.Lot = result.Data;
            //        }
            //    });
            //}

            //if (ViewBag.Lot == null)
            //{
            //    ViewBag.Lot = new Lot();
            //}

            //return View("DailyReportSYSIndex", new LotViewModel());
        }

        /// <summary>
        /// 主界面查询窗体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DailyReportSYSQuery(string reportCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            RPTDailyDataGetParameter p = new RPTDailyDataGetParameter();

            DailyMeetingRPTViewModel model = new DailyMeetingRPTViewModel
            {
                //初始化参数
                ReportCode = "DAY01",         //报表代码

                StartDate = System.DateTime.Now.AddDays(-8).ToString("yyyy-MM-dd"),   //初始化开始日期
                EndDate = System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")      //初始化结束日期
                //StartDate = System.DateTime.Now.AddDays(1 - System.DateTime.Now.Day).ToString("yyyy-MM-dd"),    //初始化开始日期
                //EndDate = System.DateTime.Now.ToString("yyyy-MM-dd")                                            //初始化结束日期
            };

            try
            {
                #region 取得数据
                DataTable dt = new DataTable();                     //报表结果集

                using (RPTDailyDataServiceClient client = new RPTDailyDataServiceClient())
                {
                    await Task.Run(() =>
                    {
                        
                        //初始化参数
                        p.ReportCode = model.ReportCode;          //报表代码
                        p.StartDate = model.StartDate;            //开始日期
                        p.EndDate = model.EndDate;                //结束日期
                        p.LocationName = model.LocationName;      //车间
                        p.LineCode = model.LineCode;              //线别
                        p.OrderNumber = model.OrderNumber;        //工单代码
                        p.ProductID = model.ProductID;            //产品类型
                                                
                        MethodReturnResult<DataSet> rst = client.Get(ref p);

                        if (rst.Code > 0)               //产生错误
                        {
                            result.Code = rst.Code;
                            result.Message = rst.Message;
                            result.Detail = rst.Detail;
                        }
                        else
                        {
                            //根据报表格式及数据清单组合数据列表
                            result = CreateTableList(rst, ref dt);                            
                        }
                    });
                }
                #endregion

                ViewBag.ListData = dt;

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ListPartial", model);
                }
                else
                {
                    return View(model);
                }

            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();
            }

            return Json(result);
        }

        #endregion
    }
        
}