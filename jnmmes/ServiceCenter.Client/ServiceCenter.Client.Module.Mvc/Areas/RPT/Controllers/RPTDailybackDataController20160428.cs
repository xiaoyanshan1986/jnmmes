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

namespace ServiceCenter.Client.Mvc.Areas.RPT.Controllers
{
    public class RPTDailyBackDataController : Controller
    {
        SortedList<string, string> sl = new SortedList<string, string>();

        /// <summary>
        /// 组装图形控件格式及数据
        /// </summary>
        /// <param name="dtData"></param>
        /// <returns></returns>
        public SortedList<string, string> AssembleSplineForModuleDebris(DataTable dtData)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                string smAxis = string.Empty;
                string snAxis = string.Empty;
                int iCurrRow = 0;
                int iIndex = 0;
                bool drawChart = false;

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
                                snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',data: [";

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
                        if (iCurrRow == 0)                      //在第一次循环时取得标题信息
                        {
                            if (datacolum.ColumnName.ToString().StartsWith("data") || datacolum.ColumnName.ToString().Equals("SUM"))    //合计列和数据列
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
                        }

                        //设置数据列
                        if (drawChart == true && (datacolum.ColumnName.ToString().StartsWith("data") || datacolum.ColumnName.ToString().Equals("SUM")))
                        {
                            if (iIndex != 0)
                            {
                                //开始第一列
                                snAxis = snAxis + " ,";
                            }

                            snAxis = snAxis + "[" + iIndex.ToString() + "," + datarow[datacolum].ToString() + "]";

                            iIndex++;
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
                        snAxis = snAxis + "]}";
                    }
                }

                //设置行数据结束标志
                snAxis = snAxis + "]";

                sl.Add("mAxis", smAxis);
                sl.Add("nAxis", snAxis);
                return sl;
            }
            catch (Exception e)
            {
                result.Code = 1000;
                result.Message = e.Message;
                result.Detail = e.ToString();
            }
            return sl;
        }

        public MethodReturnResult AssembleSplineForModuleDebris(DataTable dtData, ref SortedList<string, string> sAxis)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                string smAxis = string.Empty;
                string snAxis = string.Empty;
                int iCurrRow = 0;
                int iIndex = 0;
                bool drawChart = false;

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
                                snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',data: [";

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
                        if (iCurrRow == 0)                      //在第一次循环时取得标题信息
                        {
                            if (datacolum.ColumnName.ToString().StartsWith("data") || datacolum.ColumnName.ToString().Equals("SUM"))    //合计列和数据列
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
                        }

                        //设置数据列
                        if (drawChart == true && (datacolum.ColumnName.ToString().StartsWith("data") || datacolum.ColumnName.ToString().Equals("SUM")))
                        {
                            if (iIndex != 0)
                            {
                                //开始第一列
                                snAxis = snAxis + " ,";
                            }

                            snAxis = snAxis + "[" + iIndex.ToString() + "," + datarow[datacolum].ToString() + "]";

                            iIndex++;
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
                        snAxis = snAxis + "]}";
                    }
                }

                //设置行数据结束标志
                snAxis = snAxis + "]";

                sAxis.Add("mAxis", smAxis);
                sAxis.Add("nAxis", snAxis);

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

        ///// <summary>
        ///// 创建数据列表
        ///// </summary>
        ///// <param name="dtSet">取得数据集（报表数据、报表格式）</param>
        ///// <param name="dt">返回报表数据列表</param>
        ///// <returns></returns>
        //public MethodReturnResult CreateTableList(MethodReturnResult<DataSet> dtSet, ref DataTable dt)
        //{
        //    MethodReturnResult result = new MethodReturnResult();
            
        //    try
        //    {
        //        result.Code = 0;
        //        DataTable dtModuleDailyData = new DataTable();      //报表数据
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

        //        #endregion

        //        #region 创建动态时间列
        //        dtModuleDailyData = dtSet.Data.Tables[0];             //报表数据
        //        dtModuleReportFormat = dtSet.Data.Tables[1];          //报表格式
                               
        //        var query = from t in dtModuleDailyData.AsEnumerable()
        //                    where (t.Field<string>("DataType") == "D")
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
        //            sColumnCode = "data" + Convert.ToDateTime(data.RPDateTime).ToString("MMdd");
        //            sColumnName = Convert.ToDateTime(data.RPDateTime).ToString("M月dd日");

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
        //                           GroupNo = t.Field<string>("groupno"),                        //分组代码
        //                           DataLabelsFormat = t.Field<string>("dataLabelsformat")       //数据标签显示格式
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
        //                var detialquery = from t in dtModuleDailyData.AsEnumerable()
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

        //                    dr["ItemCode"] = data.Item_code;
        //                    dr["ItemName"] = data.Item_name;
        //                    dr["LINK"] = data.RPTURL;
        //                    dr["DataType"] = data.DataType;
        //                    dr["ChildRPCode"] = data.ChildRPCode;
        //                    dr["ChildRPName"] = data.ChildRPName;
        //                    dr["MEMO"] = data.Memo;
        //                    dr["ChartType"] = data.ChartType;
        //                    dr["GroupNo"] = data.GroupNo;
        //                    dr["DataLabelsFormat"] = data.DataLabelsFormat;

        //                    //dr["ItemCode"] = "Detail" + detaildata.DetailItem;
        //                    //dr["ItemName"] = detaildata.DetailTitle;
        //                    //dr["LINK"] = data.RPTURL;
        //                    //dr["DataType"] = data.DataType;
        //                    //dr["MEMO"] = "";
        //                    //dr["ChartType"] = data.ChartType;
        //                    //dr["GroupNo"] = data.GroupNo;
        //                    //dr["DataLabelsFormat"] = data.DataLabelsFormat;

        //                    dt.Rows.Add(dr);
        //                }
        //            }
        //            else
        //            {
        //                dr = dt.NewRow();

        //                dr["ItemCode"] = data.Item_code;
        //                dr["ItemName"] = data.Item_name;
        //                dr["LINK"] = data.RPTURL;
        //                dr["DataType"] = data.DataType;
        //                dr["ChildRPCode"] = data.ChildRPCode;
        //                dr["ChildRPName"] = data.ChildRPName;
        //                dr["MEMO"] = data.Memo;
        //                dr["ChartType"] = data.ChartType;
        //                dr["GroupNo"] = data.GroupNo;
        //                dr["DataLabelsFormat"] = data.DataLabelsFormat;

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

        //        for (int i = 0; i < dtModuleDailyData.Rows.Count; i++)
        //        {
        //            strItemCode = dtModuleDailyData.Rows[i]["ItemCode"].ToString();                 //项目代码
        //            strValues = dtModuleDailyData.Rows[i]["sumQty"].ToString();                     //项目值
        //            strDataType = dtModuleDailyData.Rows[i]["DataType"].ToString();                 //项目数据类型
        //            dtRPDateTime = Convert.ToDateTime(dtModuleDailyData.Rows[i]["rp_datetime"]);    //项目数据日期

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

        //            //设置数据类型格式
        //            switch (strFieldDataType)
        //            {
        //                case "int":
        //                    strValues = Convert.ToDouble(strValues).ToString(); ;
        //                    break;
        //                case "dec":
        //                    break;
        //            }

        //            //设置数据格式
        //            if (strDataLabelsFormat.Length > 1)
        //            {
        //                if (strDataLabelsFormat.Substring(strDataLabelsFormat.Length - 1, 1) == "%")
        //                {
        //                    strValues = strValues + "%";
        //                }
        //            }

        //            //数据填充
        //            switch (strDataType)
        //            {
        //                case "D":       //日数据
        //                    //计算数据列名称
        //                    sColumnCode = "data" + dtRPDateTime.ToString("MMdd");

        //                    dt.Rows[selOfRow][sColumnCode] = strValues;
        //                    break;
        //                case "SUM":     //累计数据
        //                    dt.Rows[selOfRow]["SUM"] = strValues;
        //                    break;
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

        //#region 日运营报表主界面
        //// GET: RPT/RPTDailyBackData
        //public ActionResult Index()
        //{
        //    RPTDailyBackDataViewModel model = new RPTDailyBackDataViewModel
        //    {
        //        //初始化参数
        //        ReportCode = "DAY01",       //报表代码
        //        StartDate = System.DateTime.Now.AddDays(1 - System.DateTime.Now.Day).ToString("yyyy-MM-dd"),
        //        EndDate = System.DateTime.Now.ToString("yyyy-MM-dd")
        //    };
        //    return View(model);
        //}
        
        //[HttpPost]
        //public ActionResult Query(RPTDailyBackDataViewModel model)
        //{
        //    MethodReturnResult result = new MethodReturnResult();            

        //    try
        //    {
        //        #region 取得数据
        //        DataTable dt = new DataTable();                     //报表结果集
        //        //DataTable dtModuleDailyData = new DataTable();
        //        //DataTable dtModuleReportFormat = new DataTable();

        //        using (RPTDailyBackDataServiceClient client = new RPTDailyBackDataServiceClient())
        //        {
        //            MethodReturnResult<DataSet> rst = client.Get(new RPTDailyBackDataGetParameter()
        //            {
        //                ReportCode = "DAY01",                   //报表代码
        //                StartDate = model.StartDate,            //开始日期
        //                EndDate = model.EndDate,                //结束日期
        //                LocationName = model.LocationName,      //车间
        //                LineCode = model.LineCode,              //线别
        //                OrderNumber = model.OrderNumber,        //工单代码
        //                ProductID = model.ProductID             //产品类型
        //            });

        //            if (rst.Code > 0)       //产生错误
        //            {
        //                result.Code = rst.Code;
        //                result.Message = rst.Message;
        //                result.Detail = rst.Detail;
                        
        //                return Json(result);
        //            }
        //            else
        //            {
        //                //dtModuleDailyData = rst.Data.Tables[0];         //报表数据
        //                //dtModuleReportFormat = rst.Data.Tables[1];      //报表格式

        //                //根据报表格式及数据清单组合数据列表
        //                result = CreateTableList(rst,ref dt);

        //                if (result.Code > 0)       //产生错误
        //                {
        //                    return Json(result);
        //                }
        //            }                                       
        //        } 
        //        #endregion

        //        #region
        //        //#region 增加项目固定列
        //        ////增加子报表链接字段
        //        //DataColumn dcStatus = new DataColumn("ItemCode");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加项目名称字段
        //        //dcStatus = new DataColumn("ItemName");
        //        //dcStatus.Caption = "项目";
        //        //dt.Columns.Add(dcStatus);
                
        //        ////增加子报表链接字段
        //        //dcStatus = new DataColumn("LINK");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加字段类型
        //        //dcStatus = new DataColumn("DataType");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加子报表代码
        //        //dcStatus = new DataColumn("ChildRPCode");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加子报表名称
        //        //dcStatus = new DataColumn("ChildRPName");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加累计资源字段
        //        //dcStatus = new DataColumn("SUM");
        //        //dcStatus.Caption = "合计";
        //        //dcStatus.DefaultValue = 0;
        //        //dt.Columns.Add(dcStatus);

        //        //#endregion

        //        //#region 创建动态时间列
        //        //var query = from t in dtModuleDailyData.AsEnumerable()
        //        //            where (t.Field<string>("DataType") == "D")
        //        //            group t by new { t1 = t.Field<string>("rp_datetime") }
        //        //                into m
        //        //                select new
        //        //                {
        //        //                    RPDateTime = m.First().Field<string>("rp_datetime")
        //        //                } into r
        //        //                orderby r.RPDateTime
        //        //                select r;

        //        ////取得时间列行数
        //        //int iDateColumns = query.Count();
        //        //string sColumnCode = "";
        //        //string sColumnName = "";

        //        ////设置日期列
        //        //foreach (var data in query)
        //        //{
        //        //    sColumnCode = "data" + Convert.ToDateTime(data.RPDateTime).ToString("MMdd");
        //        //    sColumnName = Convert.ToDateTime(data.RPDateTime).ToString("M月dd日");

        //        //    dcStatus = new DataColumn(sColumnCode);
        //        //    dcStatus.Caption = sColumnName;
        //        //    dcStatus.DefaultValue = 0;

        //        //    dt.Columns.Add(dcStatus);
        //        //}

        //        ////增加计算说明字段
        //        //dcStatus = new DataColumn("MEMO");
        //        //dcStatus.Caption = "说明";
        //        //dt.Columns.Add(dcStatus);
        //        //#endregion

        //        //#region 定义行
        //        ////取得行信息字段
        //        //var rowquery = from t in dtModuleReportFormat.AsEnumerable()
        //        //               select new
        //        //               {
        //        //                   rowsnum = t.Field<System.Int16>("rowsnum"),
        //        //                   Item_code = t.Field<string>("item_code"),
        //        //                   Item_name = t.Field<string>("item_name"),
        //        //                   Memo = t.Field<string>("Memo"),
        //        //                   RPTURL = t.Field<string>("reportURL"),
        //        //                   ChildRPCode = t.Field<string>("childrpcode"),
        //        //                   ChildRPName = t.Field<string>("childrpname"),
        //        //                   DataType = t.Field<string>("datatype")
        //        //               } into r
        //        //               orderby r.rowsnum
        //        //               select r;

        //        //DataRow dr = dt.NewRow();

        //        ////设置行信息
        //        //foreach (var data in rowquery)
        //        //{
        //        //    dr = dt.NewRow();
        //        //    dr["ItemCode"] = data.Item_code;
        //        //    dr["ItemName"] = data.Item_name;
        //        //    dr["LINK"] = data.RPTURL;
        //        //    dr["DataType"] = data.DataType;
        //        //    dr["ChildRPCode"] = data.ChildRPCode;
        //        //    dr["ChildRPName"] = data.ChildRPName;
        //        //    dr["MEMO"] = data.Memo;

        //        //    dt.Rows.Add(dr);
        //        //}
        //        //#endregion

        //        //#region 填充数据
        //        //string strItemCode = "";
        //        //string strValues = "";
        //        //string strDataType = "";
        //        //DateTime dtRPDateTime;
        //        //int selOfRow = 0;
        //        //string strFieldDataType = "";
        //        //string strDataLabelsFormat = "";

        //        //for (int i = 0; i < dtModuleDailyData.Rows.Count; i++)
        //        //{
        //        //    strItemCode = dtModuleDailyData.Rows[i]["ItemCode"].ToString();                 //项目代码
        //        //    strValues = dtModuleDailyData.Rows[i]["sumQty"].ToString();                     //项目值
        //        //    strDataType = dtModuleDailyData.Rows[i]["DataType"].ToString();                 //项目数据类型
        //        //    dtRPDateTime = Convert.ToDateTime(dtModuleDailyData.Rows[i]["rp_datetime"]);    //项目数据日期

        //        //    //取得项目所在行数及相关信息
        //        //    var ItemRowquery = from t in dtModuleReportFormat.AsEnumerable()
        //        //                       where (t.Field<string>("item_code") == strItemCode)
        //        //                       select new
        //        //                       {
        //        //                           rowsnum = t.Field<System.Int16>("rowsnum"),
        //        //                           datatype = t.Field<string>("datatype"),
        //        //                           dataLabelsformat = t.Field<string>("dataLabelsformat")
        //        //                       };

        //        //    selOfRow = ItemRowquery.FirstOrDefault().rowsnum;
        //        //    strFieldDataType = ItemRowquery.FirstOrDefault().datatype;
        //        //    strDataLabelsFormat = ItemRowquery.FirstOrDefault().dataLabelsformat;       //数据格式

        //        //    //设置数据类型格式
        //        //    switch (strFieldDataType)
        //        //    {
        //        //        case "int":
        //        //            strValues = Convert.ToDouble(strValues).ToString(); ;
        //        //            break;
        //        //        case "dec":
        //        //            break;
        //        //    }

        //        //    //设置数据格式
        //        //    if (strDataLabelsFormat.Length > 1)
        //        //    {
        //        //        if (strDataLabelsFormat.Substring(strDataLabelsFormat.Length - 1, 1) == "%")
        //        //        {
        //        //            strValues = strValues + "%";
        //        //        }
        //        //    }

        //        //    //数据填充
        //        //    switch (strDataType)
        //        //    {
        //        //        case "D":       //日数据
        //        //            //计算数据列名称
        //        //            sColumnCode = "data" + dtRPDateTime.ToString("MMdd");

        //        //            dt.Rows[selOfRow][sColumnCode] = strValues;
        //        //            break;
        //        //        case "SUM":     //累计数据
        //        //            dt.Rows[selOfRow]["SUM"] = strValues;
        //        //            break;
        //        //    }
        //        //}
        //        ////}
        //        //#endregion
        //        #endregion

        //        ViewBag.ListData = dt;

        //        ViewBag.StartDate = model.StartDate;            //开始日期
        //        ViewBag.EndDate = model.EndDate;                //结束日期
        //        ViewBag.LocationName = model.LocationName;      //车间
        //        ViewBag.LineCode = model.LineCode;              //线别
        //        ViewBag.OrderNumber = model.OrderNumber;        //工单代码
        //        ViewBag.ProductID = model.ProductID;            //产品类型

        //        if (Request.IsAjaxRequest())
        //        {
        //            return PartialView("_ListPartial", model);
        //        }
        //        else
        //        {
        //            return View(model);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        result.Code = 1002;
        //        result.Message = e.Message;
        //        result.Detail = e.ToString();
        //    }

        //    return Json(result);
        //}

        //#endregion

        //#region 明细报表
        ///// <summary>
        ///// 初始化子报表界面
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public ActionResult DetailIndex(RPTDailyBackDataViewModel model)
        //{
        //    RPTDailyBackDataViewModel ModuleDetailModel = new RPTDailyBackDataViewModel();

        //    ModuleDetailModel.ReportCode = model.ReportCode;          //报表代码
        //    ModuleDetailModel.ReportName = model.ReportName;          //报表名称
        //    ModuleDetailModel.StartDate = model.StartDate;            //开始日期
        //    ModuleDetailModel.EndDate = model.EndDate;                //结束日期
        //    ModuleDetailModel.LocationName = model.LocationName;      //车间
        //    ModuleDetailModel.LineCode = model.LineCode;              //线别
        //    ModuleDetailModel.OrderNumber = model.OrderNumber;        //工单代码
        //    ModuleDetailModel.ProductID = model.ProductID;            //产品类型

        //    ViewBag.Title = model.ReportName;

        //    return View(ModuleDetailModel);
        //}

        ///// <summary>
        ///// 子报表图形设置查询
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public ActionResult QueryModuleDetailForCharts(RPTDailyBackDataViewModel model)
        //{
        //    MethodReturnResult result = new MethodReturnResult();

        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        DataTable dtModuleDailyData = new DataTable();
        //        DataTable dtModuleReportFormat = new DataTable();

        //        string scolumntext = string.Empty;
        //        string ssplinetext = string.Empty;
        //        string scolumnformat = string.Empty;
        //        string ssplineformat = string.Empty;
        //        string scolumnLabelsformat = string.Empty;
        //        string ssplineLabelsformat = string.Empty;
        //        string sshowcolumnLabels = string.Empty;
        //        string sshowsplineLabels = string.Empty;
        //        string scolumnStacking = string.Empty;
        //        string strSUMCaption = string.Empty;
        //        string strMemoCaption = string.Empty;

        //        using (RPTDailyBackDataServiceClient client = new RPTDailyBackDataServiceClient())
        //        {
        //            MethodReturnResult<DataSet> rst = client.Get(new RPTDailyBackDataGetParameter()
        //            {
        //                ReportCode = model.ReportCode,          //报表代码
        //                StartDate = model.StartDate,            //开始日期
        //                EndDate = model.EndDate,                //结束日期
        //                LocationName = model.LocationName,      //车间
        //                LineCode = model.LineCode,              //线别
        //                OrderNumber = model.OrderNumber,        //工单代码
        //                ProductID = model.ProductID             //产品类型
        //            });

        //            if (rst.Code > 0)       //产生错误
        //            {
        //                result.Code = rst.Code;
        //                result.Message = rst.Message;
        //                result.Detail = rst.Detail;

        //                return Json(result);
        //            }
        //            else
        //            {
        //                dtModuleDailyData = rst.Data.Tables[0];         //报表数据
        //                dtModuleReportFormat = rst.Data.Tables[1];      //报表格式

        //                //根据报表格式及数据清单组合数据列表
        //                result = CreateTableList(rst, ref dt);

        //                if (result.Code > 0)       //产生错误
        //                {
        //                    return Json(result);
        //                }
        //            }
        //        }

        //        //DataTable dt = new DataTable();

        //        //#region 增加项目固定列
        //        ////取得报表信息（合计、说明标题）
        //        //var reportinforquery = from t in dtModuleReportFormat.AsEnumerable()
        //        //                       group t by new
        //        //                       {
        //        //                           SUMCaption = t.Field<string>("sumcaption"),         //合计标题
        //        //                           MemoCaption = t.Field<string>("memocaption")        //说明标题
        //        //                       } into g
        //        //                       select new
        //        //                       {
        //        //                           SUMCaption = g.Key.SUMCaption,
        //        //                           MemoCaption = g.Key.MemoCaption
        //        //                       };

        //        ////取得信息
        //        //foreach (var data in reportinforquery)
        //        //{
        //        //    strSUMCaption = data.SUMCaption;
        //        //    strMemoCaption = data.MemoCaption;
        //        //}


        //        ////增加项目名称字段
        //        //DataColumn dcStatus = new DataColumn("ItemCode");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加项目名称字段
        //        //dcStatus = new DataColumn("ItemName");
        //        //dcStatus.Caption = "项目";
        //        //dt.Columns.Add(dcStatus);

        //        ////增加子报表链接字段
        //        //dcStatus = new DataColumn("LINK");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加累计资源字段
        //        //dcStatus = new DataColumn("SUM");
        //        //dcStatus.Caption = strSUMCaption;
        //        //dcStatus.DefaultValue = 0;
        //        //dt.Columns.Add(dcStatus);

        //        ////增加字段类型
        //        //dcStatus = new DataColumn("DataType");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加图形类型
        //        //dcStatus = new DataColumn("ChartType");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加图形分组
        //        //dcStatus = new DataColumn("GroupNo");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加数据显示格式
        //        //dcStatus = new DataColumn("DataLabelsFormat");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        //#endregion

        //        //#region 创建动态时间列
        //        ////取得日期字段
        //        //var query = from t in dtModuleDailyData.AsEnumerable()
        //        //            where (t.Field<string>("DataType") == "D")
        //        //            group t by new { t1 = t.Field<string>("rp_datetime") }
        //        //                into m
        //        //                select new
        //        //                {
        //        //                    RPDateTime = m.First().Field<string>("rp_datetime")
        //        //                } into r
        //        //                orderby r.RPDateTime
        //        //                select r;

        //        ////取得时间列行数
        //        //int iDateColumns = query.Count();
        //        //string sColumnCode = "";
        //        //string sColumnName = "";

        //        ////设置日期列
        //        //foreach (var data in query)
        //        //{
        //        //    sColumnCode = "data" + Convert.ToDateTime(data.RPDateTime).ToString("MMdd");
        //        //    sColumnName = Convert.ToDateTime(data.RPDateTime).ToString("M月dd日");

        //        //    dcStatus = new DataColumn(sColumnCode);
        //        //    dcStatus.Caption = sColumnName;
        //        //    dcStatus.DefaultValue = 0;

        //        //    dt.Columns.Add(dcStatus);
        //        //}

        //        ////增加计算说明字段
        //        //dcStatus = new DataColumn("MEMO");
        //        //dcStatus.Caption = strMemoCaption;
        //        //dt.Columns.Add(dcStatus);
        //        //#endregion

        //        //#region 定义行
        //        ////取得行信息字段
        //        //var rowquery = from t in dtModuleReportFormat.AsEnumerable()
        //        //               select new
        //        //               {
        //        //                   rowsnum = t.Field<System.Int16>("rowsnum"),                  //行号
        //        //                   Item_code = t.Field<string>("item_code"),                    //项目号
        //        //                   Item_name = t.Field<string>("item_name"),                    //项目名称
        //        //                   Memo = t.Field<string>("Memo"),                              //备注
        //        //                   RPTURL = t.Field<string>("reportURL"),                       //子报表链接
        //        //                   DataType = t.Field<string>("datatype"),                      //数据类型（合计、子数据）
        //        //                   ChartType = t.Field<string>("charttype"),                    //图形类型
        //        //                   IsDetial = t.Field<System.Int16>("isdetial"),                //明细项目数据标识（1 - 明细数据）
        //        //                   GroupNo = t.Field<string>("groupno"),                        //分组代码
        //        //                   DataLabelsFormat = t.Field<string>("dataLabelsformat")       //数据标签显示格式
        //        //               } into r
        //        //               orderby r.rowsnum
        //        //               select r;

        //        //DataRow dr = dt.NewRow();

        //        ////设置行信息
        //        //foreach (var data in rowquery)
        //        //{
        //        //    if (data.IsDetial == 1)
        //        //    {
        //        //        //处理明细数据
        //        //        var detialquery = from t in dtModuleDailyData.AsEnumerable()
        //        //                          where t.Field<string>("ItemCode") == data.Item_code
        //        //                          group t by new
        //        //                          {
        //        //                              DetailItem = t.Field<string>("DetailItem"),
        //        //                              DetailTitle = t.Field<string>("DetailTitle")
        //        //                          } into g
        //        //                          select new
        //        //                          {
        //        //                              DetailItem = g.Key.DetailItem,
        //        //                              DetailTitle = g.Key.DetailTitle
        //        //                          } into r
        //        //                          orderby r.DetailItem
        //        //                          select r;

        //        //        foreach (var detaildata in detialquery)
        //        //        {
        //        //            dr = dt.NewRow();

        //        //            dr["ItemCode"] = "Detail" + detaildata.DetailItem;
        //        //            dr["ItemName"] = detaildata.DetailTitle;
        //        //            dr["LINK"] = data.RPTURL;
        //        //            dr["DataType"] = data.DataType;
        //        //            dr["MEMO"] = "";
        //        //            dr["ChartType"] = data.ChartType;
        //        //            dr["GroupNo"] = data.GroupNo;
        //        //            dr["DataLabelsFormat"] = data.DataLabelsFormat;

        //        //            dt.Rows.Add(dr);
        //        //        }
        //        //    }
        //        //    else
        //        //    {
        //        //        dr = dt.NewRow();

        //        //        dr["ItemCode"] = data.Item_code;
        //        //        dr["ItemName"] = data.Item_name;
        //        //        dr["LINK"] = data.RPTURL;
        //        //        dr["DataType"] = data.DataType;
        //        //        dr["MEMO"] = data.Memo;
        //        //        dr["ChartType"] = data.ChartType;
        //        //        dr["GroupNo"] = data.GroupNo;
        //        //        dr["DataLabelsFormat"] = data.DataLabelsFormat;

        //        //        dt.Rows.Add(dr);
        //        //    }
        //        //}

        //        //取得图形信息字段
        //        var reportformatquery = from t in dtModuleReportFormat.AsEnumerable()
        //                                group t by new
        //                                {
        //                                    ChartType = t.Field<string>("charttype"),
        //                                    ChartText = t.Field<string>("charttext"),
        //                                    ChartFormat = t.Field<string>("chartformat"),
        //                                    ShowDataLabels = t.Field<string>("showdataLabels"),
        //                                    DataLabelsFormat = t.Field<string>("dataLabelsformat"),
        //                                    Stacking = t.Field<string>("stacking")
        //                                } into g
        //                                select new
        //                                {
        //                                    ChartType = g.Key.ChartType,
        //                                    ChartText = g.Key.ChartText,
        //                                    ChartFormat = g.Key.ChartFormat,
        //                                    ShowDataLabels = g.Key.ShowDataLabels,
        //                                    DataLabelsFormat = g.Key.DataLabelsFormat,
        //                                    Stacking = g.Key.Stacking
        //                                };

        //        //设置行信息
        //        foreach (var data in reportformatquery)
        //        {
        //            switch (data.ChartType.ToString())
        //            {
        //                case "column":
        //                    scolumntext = data.ChartText;                   //图形说明
        //                    scolumnformat = data.ChartFormat;               //图形格式
        //                    sshowcolumnLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
        //                    scolumnLabelsformat = data.DataLabelsFormat;    //图形标签格式
        //                    scolumnStacking = data.Stacking;                //图形堆叠属性

        //                    break;
        //                case "line":
        //                    ssplinetext = data.ChartText;                   //图形说明
        //                    ssplineformat = data.ChartFormat;               //图形格式
        //                    sshowsplineLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
        //                    ssplineLabelsformat = data.DataLabelsFormat;    //图形标签格式

        //                    break;
        //            }
        //        }
                

        //        #region 填充数据
        //        string sColumnCode = "";
        //        string strItemCode = "";
        //        string strValues = "";
        //        string strDataType = "";
        //        DateTime dtRPDateTime;
        //        string strFieldDataType = "";
        //        string strDetail = "";

        //        for (int i = 0; i < dtModuleDailyData.Rows.Count; i++)
        //        {
        //            strItemCode = dtModuleDailyData.Rows[i]["ItemCode"].ToString();                 //项目代码
        //            strValues = dtModuleDailyData.Rows[i]["sumQty"].ToString();                     //项目值
        //            strDataType = dtModuleDailyData.Rows[i]["DataType"].ToString();                 //项目数据类型
        //            dtRPDateTime = Convert.ToDateTime(dtModuleDailyData.Rows[i]["rp_datetime"]);    //项目数据日期
        //            strDetail = dtModuleDailyData.Rows[i]["DetailItem"].ToString();                 //明细项目标识

        //            //取得项目所在行数及相关信息
        //            var ItemRowquery = from t in dtModuleReportFormat.AsEnumerable()
        //                               where (t.Field<string>("item_code") == strItemCode)
        //                               select new
        //                               {
        //                                   rowsnum = t.Field<System.Int16>("rowsnum"),
        //                                   datatype = t.Field<string>("datatype")
        //                               };

        //            strFieldDataType = ItemRowquery.FirstOrDefault().datatype;

        //            if (strDetail != "")    //明细数据
        //            {
        //                strItemCode = "Detail" + strDetail;
        //            }

        //            DataRow[] ItemRow = dt.Select("ItemCode = '" + strItemCode + "'");

        //            if (ItemRow.GetLength(0) > 0)
        //            {
        //                //设置数据格式
        //                switch (strFieldDataType)
        //                {
        //                    case "int":
        //                        strValues = Convert.ToDouble(strValues).ToString(); ;
        //                        break;
        //                    case "dec":
        //                        break;
        //                }

        //                //数据填充
        //                switch (strDataType)
        //                {
        //                    case "D":       //日数据
        //                        //计算数据列名称
        //                        sColumnCode = "data" + dtRPDateTime.ToString("MMdd");

        //                        ItemRow[0][sColumnCode] = strValues;
        //                        break;
        //                    case "SUM":     //累计数据
        //                        ItemRow[0]["SUM"] = strValues;
        //                        break;
        //                }
        //            }
        //        }

        //        #endregion

        //        //组装图形XY轴数据
        //        AssembleSplineForModuleDebris(dt);

        //        //设置报表名称
        //        sl.Add("ReportName", model.ReportName);

        //        //设置图形说明
        //        sl.Add("ColumnText", scolumntext);
        //        sl.Add("SplineText", ssplinetext);

        //        //设置图形格式
        //        sl.Add("ColumnFormat", scolumnformat);
        //        sl.Add("SplineFormat", ssplineformat);

        //        //设置数据标签说明
        //        sl.Add("ShowColumnLabels", sshowcolumnLabels);
        //        sl.Add("ShowSplineLabels", sshowsplineLabels);

        //        //设置数据标签格式
        //        sl.Add("ColumnLabelsFormat", scolumnLabelsformat);
        //        sl.Add("SplineLabelsFormat", ssplineLabelsformat);

        //        //设置数据堆叠属性
        //        sl.Add("ColumnStacking", scolumnStacking);

        //        return Json(sl, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        result.Code = 1002;
        //        result.Message = e.Message;
        //        result.Detail = e.ToString();

        //        return Json(result);
        //    }
        //}

        //public ActionResult QueryModuleDetail(RPTDailyBackDataViewModel model)
        //{
        //    MethodReturnResult result = new MethodReturnResult();

        //    try
        //    {
        //        DataTable dt = new DataTable();                     //报表数据列表
        //        string sColumnCode = "";
        //        //DataTable dtModuleDailyData = new DataTable();
        //        //DataTable dtModuleReportFormat = new DataTable();
        //        string strSUMCaption = string.Empty;
        //        string strMemoCaption = string.Empty;

        //        using (RPTDailyBackDataServiceClient client = new RPTDailyBackDataServiceClient())
        //        {
        //            MethodReturnResult<DataSet> rst = client.Get(new RPTDailyBackDataGetParameter()
        //            {
        //                ReportCode = model.ReportCode,          //报表代码
        //                StartDate = model.StartDate,            //开始日期
        //                EndDate = model.EndDate,                //结束日期
        //                LocationName = model.LocationName,      //车间
        //                LineCode = model.LineCode,              //线别
        //                OrderNumber = model.OrderNumber,        //工单代码
        //                ProductID = model.ProductID             //产品类型
        //            });

        //            if (rst.Code > 0)       //产生错误
        //            {
        //                result.Code = rst.Code;
        //                result.Message = rst.Message;
        //                result.Detail = rst.Detail;

        //                return Json(result);
        //            }
        //            else
        //            {
        //                //dtModuleDailyData = rst.Data.Tables[0];         //报表数据
        //                //dtModuleReportFormat = rst.Data.Tables[1];      //报表格式

        //                //根据报表格式及数据清单组合数据列表
        //                result = CreateTableList(rst, ref dt);

        //                if (result.Code > 0)       //产生错误
        //                {
        //                    return Json(result);
        //                }
        //            }
        //        }

        //        #region
        //        //DataTable dt = new DataTable();

        //        //#region 增加项目固定列
        //        ////取得报表信息（合计、说明标题）
        //        //var reportinforquery = from t in dtModuleReportFormat.AsEnumerable()
        //        //                       group t by new
        //        //                       {
        //        //                           SUMCaption = t.Field<string>("sumcaption"),         //合计标题
        //        //                           MemoCaption = t.Field<string>("memocaption")        //说明标题
        //        //                       } into g
        //        //                       select new
        //        //                       {
        //        //                           SUMCaption = g.Key.SUMCaption,
        //        //                           MemoCaption = g.Key.MemoCaption
        //        //                       };

        //        ////取得信息
        //        //foreach (var data in reportinforquery)
        //        //{
        //        //    strSUMCaption = data.SUMCaption;
        //        //    strMemoCaption = data.MemoCaption;
        //        //}

        //        ////增加项目名称字段
        //        //DataColumn dcStatus = new DataColumn("ItemCode");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加项目名称字段
        //        //dcStatus = new DataColumn("ItemName");
        //        //dcStatus.Caption = "项目";
        //        //dt.Columns.Add(dcStatus);

        //        ////增加子报表链接字段
        //        //dcStatus = new DataColumn("LINK");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加累计资源字段
        //        //dcStatus = new DataColumn("SUM");
        //        //dcStatus.Caption = strSUMCaption;
        //        //dcStatus.DefaultValue = 0;
        //        //dt.Columns.Add(dcStatus);

        //        ////增加字段类型
        //        //dcStatus = new DataColumn("DataType");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加图形类型
        //        //dcStatus = new DataColumn("ChartType");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        ////增加数据显示格式
        //        //dcStatus = new DataColumn("DataLabelsFormat");
        //        //dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        //dt.Columns.Add(dcStatus);

        //        //#endregion

        //        //#region 创建动态时间列
        //        ////System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();

        //        ////取得日期字段
        //        //var query = from t in dtModuleDailyData.AsEnumerable()
        //        //            where (t.Field<string>("DataType") == "D")
        //        //            group t by new { t1 = t.Field<string>("rp_datetime") }
        //        //                into m
        //        //                select new
        //        //                {
        //        //                    RPDateTime = m.First().Field<string>("rp_datetime")
        //        //                } into r
        //        //                orderby r.RPDateTime
        //        //                select r;

        //        ////取得时间列行数
        //        //int iDateColumns = query.Count();
        //        //string sColumnCode = "";
        //        //string sColumnName = "";

        //        ////设置日期列
        //        //foreach (var data in query)
        //        //{
        //        //    sColumnCode = "data" + Convert.ToDateTime(data.RPDateTime).ToString("MMdd");
        //        //    sColumnName = Convert.ToDateTime(data.RPDateTime).ToString("M月dd日");

        //        //    dcStatus = new DataColumn(sColumnCode);
        //        //    dcStatus.Caption = sColumnName;
        //        //    dcStatus.DefaultValue = 0;

        //        //    dt.Columns.Add(dcStatus);
        //        //}

        //        ////增加计算说明字段
        //        //dcStatus = new DataColumn("MEMO");
        //        //dcStatus.Caption = strMemoCaption;
        //        //dt.Columns.Add(dcStatus);
        //        //#endregion

        //        //#region 定义行
        //        ////取得行信息字段
        //        //var rowquery = from t in dtModuleReportFormat.AsEnumerable()
        //        //               select new
        //        //               {
        //        //                   rowsnum = t.Field<System.Int16>("rowsnum"),                  //行号
        //        //                   Item_code = t.Field<string>("item_code"),                    //项目号
        //        //                   Item_name = t.Field<string>("item_name"),                    //项目名称
        //        //                   Memo = t.Field<string>("Memo"),                              //备注
        //        //                   RPTURL = t.Field<string>("reportURL"),                       //子报表链接
        //        //                   DataType = t.Field<string>("datatype"),                      //数据类型（合计、子数据）
        //        //                   ChartType = t.Field<string>("charttype"),                    //图形类型
        //        //                   IsDetial = t.Field<System.Int16>("isdetial"),                //明细项目数据标识（1 - 明细数据）
        //        //                   DataLabelsFormat = t.Field<string>("dataLabelsformat")       //数据标签显示格式
        //        //               } into r
        //        //               orderby r.rowsnum
        //        //               select r;

        //        //DataRow dr = dt.NewRow();

        //        ////设置行信息
        //        //foreach (var data in rowquery)
        //        //{
        //        //    if (data.IsDetial == 1)
        //        //    {
        //        //        //处理明细数据
        //        //        var detialquery = from t in dtModuleDailyData.AsEnumerable()
        //        //                          where t.Field<string>("ItemCode") == data.Item_code
        //        //                          group t by new
        //        //                          {
        //        //                              DetailItem = t.Field<string>("DetailItem"),
        //        //                              DetailTitle = t.Field<string>("DetailTitle")
        //        //                          } into g
        //        //                          select new
        //        //                          {
        //        //                              DetailItem = g.Key.DetailItem,
        //        //                              DetailTitle = g.Key.DetailTitle
        //        //                          } into r
        //        //                          orderby r.DetailItem
        //        //                          select r;

        //        //        foreach (var detaildata in detialquery)
        //        //        {
        //        //            dr = dt.NewRow();

        //        //            dr["ItemCode"] = "Detail" + detaildata.DetailItem;
        //        //            dr["ItemName"] = detaildata.DetailTitle;
        //        //            dr["LINK"] = data.RPTURL;
        //        //            dr["DataType"] = data.DataType;
        //        //            dr["MEMO"] = "";
        //        //            dr["ChartType"] = data.ChartType;
        //        //            dr["DataLabelsFormat"] = data.DataLabelsFormat;

        //        //            dt.Rows.Add(dr);
        //        //        }
        //        //    }
        //        //    else
        //        //    {
        //        //        dr = dt.NewRow();

        //        //        dr["ItemCode"] = data.Item_code;
        //        //        dr["ItemName"] = data.Item_name;
        //        //        dr["LINK"] = data.RPTURL;
        //        //        dr["DataType"] = data.DataType;
        //        //        dr["MEMO"] = data.Memo;
        //        //        dr["ChartType"] = data.ChartType;
        //        //        dr["DataLabelsFormat"] = data.DataLabelsFormat;

        //        //        dt.Rows.Add(dr);
        //        //    }
        //        //}
        //        //#endregion

        //        //#region 填充数据
        //        //string strItemCode = "";
        //        //string strValues = "";
        //        //string strDataType = "";
        //        //DateTime dtRPDateTime;
        //        //string strFieldDataType = "";
        //        //string strDetail = "";
        //        //string strDataLabelsFormat = "";

        //        //for (int i = 0; i < dtModuleDailyData.Rows.Count; i++)
        //        //{
        //        //    strItemCode = dtModuleDailyData.Rows[i]["ItemCode"].ToString();                 //项目代码
        //        //    strValues = dtModuleDailyData.Rows[i]["sumQty"].ToString();                     //项目值
        //        //    strDataType = dtModuleDailyData.Rows[i]["DataType"].ToString();                 //项目数据类型
        //        //    dtRPDateTime = Convert.ToDateTime(dtModuleDailyData.Rows[i]["rp_datetime"]);    //项目数据日期
        //        //    strDetail = dtModuleDailyData.Rows[i]["DetailItem"].ToString();                 //明细项目标识

        //        //    //取得项目所在行数及相关信息
        //        //    var ItemRowquery = from t in dtModuleReportFormat.AsEnumerable()
        //        //                       where (t.Field<string>("item_code") == strItemCode)
        //        //                       select new
        //        //                       {
        //        //                           rowsnum = t.Field<System.Int16>("rowsnum"),
        //        //                           datatype = t.Field<string>("datatype"),
        //        //                           dataLabelsformat = t.Field<string>("dataLabelsformat")
        //        //                       };

        //        //    strFieldDataType = ItemRowquery.FirstOrDefault().datatype;                  //数据类型
        //        //    strDataLabelsFormat = ItemRowquery.FirstOrDefault().dataLabelsformat;       //数据格式

        //        //    if (strDetail != "")    //明细数据
        //        //    {
        //        //        strItemCode = "Detail" + strDetail;
        //        //    }

        //        //    DataRow[] ItemRow = dt.Select("ItemCode = '" + strItemCode + "'");

        //        //    if (ItemRow.GetLength(0) > 0)
        //        //    {
        //        //        //设置数据类型格式
        //        //        switch (strFieldDataType)
        //        //        {
        //        //            case "int":
        //        //                strValues = Convert.ToDouble(strValues).ToString();


        //        //                break;
        //        //            case "dec":
        //        //                break;
        //        //        }

        //        //        //设置数据格式
        //        //        if (strDataLabelsFormat.Length > 1)
        //        //        {
        //        //            if (strDataLabelsFormat.Substring(strDataLabelsFormat.Length - 1, 1) == "%")
        //        //            {
        //        //                strValues = strValues + "%";
        //        //            }
        //        //        }

        //        //        //数据填充
        //        //        switch (strDataType)
        //        //        {
        //        //            case "D":       //日数据
        //        //                //计算数据列名称
        //        //                sColumnCode = "data" + dtRPDateTime.ToString("MMdd");

        //        //                ItemRow[0][sColumnCode] = strValues;
        //        //                break;
        //        //            case "SUM":     //累计数据
        //        //                ItemRow[0]["SUM"] = strValues;
        //        //                break;
        //        //        }
        //        //    }
        //        //}

        //        #endregion

        //        ViewBag.ListData = dt;

        //        if (Request.IsAjaxRequest())
        //        {
        //            return PartialView("_DetailListPartial", model);
        //        }
        //        else
        //        {
        //            return View(model);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        result.Code = 1002;
        //        result.Message = e.Message;
        //        result.Detail = e.ToString();
        //    }

        //    return Json(result);
        //}

        //#endregion

        //#region 明细子报表
        ///// <summary>
        ///// 初始化子报表界面
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public ActionResult SubDetailIndex(RPTDailyBackDataViewModel model)
        //{
        //    RPTDailyBackDataViewModel ModuleSubDetailModel = new RPTDailyBackDataViewModel();

        //    ModuleSubDetailModel.ReportCode = model.ReportCode;          //报表代码
        //    ModuleSubDetailModel.ReportName = model.ReportName;          //报表名称
        //    ModuleSubDetailModel.StartDate = model.StartDate;            //开始日期
        //    ModuleSubDetailModel.EndDate = model.EndDate;                //结束日期
        //    ModuleSubDetailModel.LocationName = model.LocationName;      //车间
        //    ModuleSubDetailModel.LineCode = model.LineCode;              //线别
        //    ModuleSubDetailModel.OrderNumber = model.OrderNumber;        //工单代码
        //    ModuleSubDetailModel.ProductID = model.ProductID;            //产品类型

        //    ViewBag.Title = model.ReportName;

        //    return View(ModuleSubDetailModel);
        //}

        ///// <summary>
        ///// 子报表图形设置查询
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public ActionResult QueryModuleSubDetailForCharts(RPTDailyBackDataViewModel model)
        //{
        //    MethodReturnResult result = new MethodReturnResult();
            
        //    try
        //    {
        //        DataTable dtModuleDailyData = new DataTable();
        //        DataTable dtModuleReportFormat = new DataTable();
        //        string scolumntext = string.Empty;
        //        string ssplinetext = string.Empty;
        //        string scolumnformat = string.Empty;
        //        string ssplineformat = string.Empty;
        //        string scolumnLabelsformat = string.Empty;
        //        string ssplineLabelsformat = string.Empty;
        //        string sshowcolumnLabels = string.Empty;
        //        string sshowsplineLabels = string.Empty;
        //        string scolumnStacking = string.Empty;
        //        string strSUMCaption = string.Empty;
        //        string strMemoCaption = string.Empty;

        //        using (RPTDailyBackDataServiceClient client = new RPTDailyBackDataServiceClient())
        //        {
        //            MethodReturnResult<DataSet> rst = client.Get(new RPTDailyBackDataGetParameter()
        //            {
        //                ReportCode = model.ReportCode,          //报表代码
        //                StartDate = model.StartDate,            //开始日期
        //                EndDate = model.EndDate,                //结束日期
        //                LocationName = model.LocationName,      //车间
        //                LineCode = model.LineCode,              //线别
        //                OrderNumber = model.OrderNumber,        //工单代码
        //                ProductID = model.ProductID             //产品类型
        //            });

        //            if (rst.Code > 0)       //产生错误
        //            {
        //                result.Code = rst.Code;
        //                result.Message = rst.Message;
        //                result.Detail = rst.Detail;

        //                return Json(result);
        //            }
        //            else
        //            {
        //                dtModuleDailyData = rst.Data.Tables[0];         //报表数据
        //                dtModuleReportFormat = rst.Data.Tables[1];      //报表格式
        //            }
        //        }

        //        DataTable dt = new DataTable();
                
        //        #region 增加项目固定列
        //        //取得报表信息（合计、说明标题）
        //        var reportinforquery = from t in dtModuleReportFormat.AsEnumerable()   
        //                group t by new 
        //                {
        //                    SUMCaption = t.Field<string>("sumcaption"),         //合计标题
        //                    MemoCaption = t.Field<string>("memocaption")        //说明标题
        //                } into g   
        //                select new
        //                {
        //                    SUMCaption = g.Key.SUMCaption,
        //                    MemoCaption = g.Key.MemoCaption
        //                };

        //        //取得信息
        //        foreach (var data in reportinforquery)
        //        {
        //            strSUMCaption = data.SUMCaption;
        //            strMemoCaption = data.MemoCaption;
        //        }


        //        //增加项目名称字段
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

        //        //增加累计资源字段
        //        dcStatus = new DataColumn("SUM");
        //        dcStatus.Caption = strSUMCaption;
        //        dcStatus.DefaultValue = 0;
        //        dt.Columns.Add(dcStatus);

        //        //增加字段类型
        //        dcStatus = new DataColumn("DataType");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
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

        //        #endregion

        //        #region 创建动态时间列                
        //        //取得日期字段
        //        var query = from t in dtModuleDailyData.AsEnumerable()
        //                    where (t.Field<string>("DataType") == "D")
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
        //            sColumnCode = "data" + Convert.ToDateTime(data.RPDateTime).ToString("MMdd");
        //            sColumnName = Convert.ToDateTime(data.RPDateTime).ToString("M月dd日");

        //            dcStatus = new DataColumn(sColumnCode);
        //            dcStatus.Caption = sColumnName;
        //            dcStatus.DefaultValue = 0;

        //            dt.Columns.Add(dcStatus);
        //        }

        //        //增加计算说明字段
        //        dcStatus = new DataColumn("MEMO");
        //        dcStatus.Caption = strMemoCaption;
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
        //                           DataType = t.Field<string>("datatype"),                      //数据类型（合计、子数据）
        //                           ChartType = t.Field<string>("charttype"),                    //图形类型
        //                           IsDetial = t.Field<System.Int16>("isdetial"),                //明细项目数据标识（1 - 明细数据）
        //                           GroupNo = t.Field<string>("groupno"),                        //分组代码
        //                           DataLabelsFormat = t.Field<string>("dataLabelsformat")       //数据标签显示格式
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
        //                var detialquery = from t in dtModuleDailyData.AsEnumerable()
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

        //                    dr["ItemCode"] = "Detail" + detaildata.DetailItem;
        //                    dr["ItemName"] = detaildata.DetailTitle;
        //                    dr["LINK"] = data.RPTURL;
        //                    dr["DataType"] = data.DataType;
        //                    dr["MEMO"] = "";
        //                    dr["ChartType"] = data.ChartType;
        //                    dr["GroupNo"] = data.GroupNo;
        //                    dr["DataLabelsFormat"] = data.DataLabelsFormat;

        //                    dt.Rows.Add(dr);
        //                }
        //            }
        //            else
        //            {
        //                dr = dt.NewRow();

        //                dr["ItemCode"] = data.Item_code;
        //                dr["ItemName"] = data.Item_name;
        //                dr["LINK"] = data.RPTURL;
        //                dr["DataType"] = data.DataType;
        //                dr["MEMO"] = data.Memo;
        //                dr["ChartType"] = data.ChartType;
        //                dr["GroupNo"] = data.GroupNo;
        //                dr["DataLabelsFormat"] = data.DataLabelsFormat;

        //                dt.Rows.Add(dr);
        //            }                    
        //        }

        //        //取得图形信息字段
        //        var reportformatquery = from t in dtModuleReportFormat.AsEnumerable()   
        //                group t by new 
        //                {
        //                    ChartType = t.Field<string>("charttype"),
        //                    ChartText = t.Field<string>("charttext"),
        //                    ChartFormat = t.Field<string>("chartformat"),
        //                    ShowDataLabels = t.Field<string>("showdataLabels"),
        //                    DataLabelsFormat = t.Field<string>("dataLabelsformat"),
        //                    Stacking = t.Field<string>("stacking")
        //                } into g   
        //                select new
        //                {
        //                    ChartType = g.Key.ChartType,
        //                    ChartText = g.Key.ChartText,
        //                    ChartFormat = g.Key.ChartFormat,
        //                    ShowDataLabels = g.Key.ShowDataLabels,
        //                    DataLabelsFormat = g.Key.DataLabelsFormat,
        //                    Stacking = g.Key.Stacking
        //                };

        //        //设置行信息
        //        foreach (var data in reportformatquery)
        //        {
        //            switch (data.ChartType.ToString())
        //            {
        //                case "column":
        //                    scolumntext = data.ChartText;                   //图形说明
        //                    scolumnformat = data.ChartFormat;               //图形格式
        //                    sshowcolumnLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
        //                    scolumnLabelsformat = data.DataLabelsFormat;    //图形标签格式
        //                    scolumnStacking = data.Stacking;                //图形堆叠属性

        //                    break;
        //                case "line":
        //                    ssplinetext = data.ChartText;                   //图形说明
        //                    ssplineformat = data.ChartFormat;               //图形格式
        //                    sshowsplineLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
        //                    ssplineLabelsformat = data.DataLabelsFormat;    //图形标签格式

        //                    break;
        //            }

        //            //if (data.ChartType.ToString().Equals("column"))
        //            //{
        //            //    scolumntext = data.ChartText;                   //图形说明
        //            //    scolumnformat = data.ChartFormat;               //图形格式
        //            //    sshowcolumnLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
        //            //    scolumnLabelsformat = data.DataLabelsFormat;    //图形标签格式
        //            //    scolumnStacking = data.Stacking;                //图形堆叠属性
        //            //}
        //            //else
        //            //{
        //            //    ssplinetext = data.ChartText;                   //图形说明
        //            //    ssplineformat = data.ChartFormat;               //图形格式
        //            //    sshowsplineLabels = data.ShowDataLabels;        //图形标签显示状态（true、false）
        //            //    ssplineLabelsformat = data.DataLabelsFormat;    //图形标签格式
        //            //}
        //        }
        //        #endregion

        //        #region 填充数据
        //        string strItemCode = "";
        //        string strValues = "";
        //        string strDataType = "";
        //        DateTime dtRPDateTime;
        //        string strFieldDataType = "";
        //        string strDetail = "";

        //        for (int i = 0; i < dtModuleDailyData.Rows.Count; i++)
        //        {
        //            strItemCode = dtModuleDailyData.Rows[i]["ItemCode"].ToString();                 //项目代码
        //            strValues = dtModuleDailyData.Rows[i]["sumQty"].ToString();                     //项目值
        //            strDataType = dtModuleDailyData.Rows[i]["DataType"].ToString();                 //项目数据类型
        //            dtRPDateTime = Convert.ToDateTime(dtModuleDailyData.Rows[i]["rp_datetime"]);    //项目数据日期
        //            strDetail = dtModuleDailyData.Rows[i]["DetailItem"].ToString();                 //明细项目标识
                    
        //            //取得项目所在行数及相关信息
        //            var ItemRowquery = from t in dtModuleReportFormat.AsEnumerable()
        //                               where (t.Field<string>("item_code") == strItemCode)
        //                               select new
        //                               {
        //                                   rowsnum = t.Field<System.Int16>("rowsnum"),
        //                                   datatype = t.Field<string>("datatype")
        //                               };

        //            strFieldDataType = ItemRowquery.FirstOrDefault().datatype;

        //            if (strDetail != "")    //明细数据
        //            {
        //                strItemCode = "Detail" + strDetail;
        //            }

        //            DataRow[] ItemRow = dt.Select("ItemCode = '" + strItemCode + "'");

        //            if (ItemRow.GetLength(0) > 0)
        //            {
        //                //设置数据格式
        //                switch (strFieldDataType)
        //                {
        //                    case "int":
        //                        strValues = Convert.ToDouble(strValues).ToString(); ;
        //                        break;
        //                    case "dec":
        //                        break;
        //                }

        //                //数据填充
        //                switch (strDataType)
        //                {
        //                    case "D":       //日数据
        //                        //计算数据列名称
        //                        sColumnCode = "data" + dtRPDateTime.ToString("MMdd");

        //                        ItemRow[0][sColumnCode] = strValues;
        //                        break;
        //                    case "SUM":     //累计数据
        //                        ItemRow[0]["SUM"] = strValues;
        //                        break;
        //                }                       
        //            }                   
        //        }
                
        //        #endregion

        //        //组装图形XY轴数据
        //        AssembleSplineForModuleDebris(dt);

        //        //设置报表名称
        //        sl.Add("ReportName", model.ReportName);

        //        //设置图形说明
        //        sl.Add("ColumnText", scolumntext);
        //        sl.Add("SplineText", ssplinetext);

        //        //设置图形格式
        //        sl.Add("ColumnFormat", scolumnformat);
        //        sl.Add("SplineFormat", ssplineformat);

        //        //设置数据标签说明
        //        sl.Add("ShowColumnLabels", sshowcolumnLabels);
        //        sl.Add("ShowSplineLabels", sshowsplineLabels);

        //        //设置数据标签格式
        //        sl.Add("ColumnLabelsFormat", scolumnLabelsformat);
        //        sl.Add("SplineLabelsFormat", ssplineLabelsformat);

        //        //设置数据堆叠属性
        //        sl.Add("ColumnStacking", scolumnStacking);

        //        return Json(sl, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        result.Code = 1002;
        //        result.Message = e.Message;
        //        result.Detail = e.ToString();

        //        return Json(result);
        //    }            
        //}

        ///// <summary>
        ///// 组装图形控件格式及数据
        ///// </summary>
        ///// <param name="dtData"></param>
        ///// <returns></returns>
        ////public SortedList<string, string> AssembleSplineForModuleDebris( DataTable dtData)
        ////{
        ////    MethodReturnResult result = new MethodReturnResult();

        ////    try
        ////    {
        ////        string smAxis = string.Empty;
        ////        string snAxis = string.Empty;
        ////        int iCurrRow = 0;
        ////        int iIndex = 0;
        ////        bool drawChart = false;

        ////        //逐行组装数据矩阵
        ////        foreach (DataRow datarow in dtData.Rows)
        ////        {
        ////            drawChart = false;  //图形显示标识

        ////            if (datarow["ChartType"].ToString().Equals("line") || datarow["ChartType"].ToString().Equals("spline") || datarow["ChartType"].ToString().Equals("column"))
        ////            {
        ////                if (snAxis == "")
        ////                {
        ////                    snAxis = "[{";
        ////                }
        ////                else
        ////                {
        ////                    snAxis = snAxis + ",{";
        ////                }

        ////                //取得项目名称
        ////                snAxis = snAxis + " name: '" + datarow["ItemName"].ToString() + "',";

        ////                //设置项目图形类型
        ////                switch (datarow["ChartType"].ToString())
        ////                {
        ////                    case "line":
        ////                        if (datarow["GroupNo"].ToString().Equals("0"))  //图形组
        ////                        {
        ////                            snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',data: [";
        ////                        }
        ////                        else
        ////                        {
        ////                            snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',yAxis: " + datarow["GroupNo"].ToString() + ",data: [";
        ////                        }

        ////                        drawChart = true;   //图形显示标识

        ////                        break;
        ////                    case "spline":
        ////                        if (datarow["GroupNo"].ToString().Equals("0"))  //图形组
        ////                        {
        ////                            snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',data: [";
        ////                        }
        ////                        else
        ////                        {
        ////                            snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',yAxis: " + datarow["GroupNo"].ToString() + ",data: [";
        ////                        }

        ////                        drawChart = true;   //图形显示标识

        ////                        break;
        ////                    case "column":
        ////                        snAxis = snAxis + " type:'" + datarow["ChartType"].ToString() + "',data: [";

        ////                        drawChart = true;   //图形显示标识

        ////                        break;
        ////                    default:
        ////                        drawChart = false;  //图形显示标识

        ////                        break;
        ////                }
        ////            }
                     
        ////            //取得数据及标题列信息
        ////            iIndex = 0;

        ////            foreach (DataColumn datacolum in dtData.Columns)
        ////            {
        ////                if (iCurrRow == 0)                      //在第一次循环时取得标题信息
        ////                {
        ////                    if (datacolum.ColumnName.ToString().StartsWith("data") || datacolum.ColumnName.ToString().Equals("SUM"))    //合计列和数据列
        ////                    {
        ////                        if (smAxis == "")
        ////                        {
        ////                            smAxis = "[";
        ////                        }
        ////                        else
        ////                        {
        ////                            smAxis = smAxis + ",";
        ////                        }

        ////                        smAxis = smAxis + "'" + datacolum.Caption + "'";
        ////                    }
        ////                }

        ////                //设置数据列
        ////                if (drawChart == true && (datacolum.ColumnName.ToString().StartsWith("data") || datacolum.ColumnName.ToString().Equals("SUM")))                        
        ////                {
        ////                    if (iIndex != 0)
        ////                    {
        ////                        //开始第一列
        ////                        snAxis = snAxis + " ,";
        ////                    }

        ////                    snAxis = snAxis + "[" + iIndex.ToString() + "," + datarow[datacolum].ToString() + "]";

        ////                    iIndex++;
        ////                } 
        ////            }

        ////            //设置标题信息的结束标志
        ////            if (iCurrRow == 0)              
        ////            {
        ////                smAxis = smAxis + "]";

        ////                iCurrRow++;
        ////            }

        ////            if (drawChart == true)
        ////            {
        ////                //设置单行数据结束标志
        ////                snAxis = snAxis + "]}";
        ////            }                    
        ////        }

        ////        //设置行数据结束标志
        ////        snAxis = snAxis + "]";

        ////        sl.Add("mAxis", smAxis);
        ////        sl.Add("nAxis", snAxis);
        ////        return sl;
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        result.Code = 1000;
        ////        result.Message = e.Message;
        ////        result.Detail = e.ToString();
        ////    }
        ////    return sl;
        ////}

        //public ActionResult QueryModuleSubDetail(RPTDailyBackDataViewModel model)
        //{
        //    MethodReturnResult result = new MethodReturnResult();

        //    try
        //    {
        //        DataTable dtModuleDailyData = new DataTable();
        //        DataTable dtModuleReportFormat = new DataTable();
        //        string strSUMCaption = string.Empty;
        //        string strMemoCaption = string.Empty;

        //        using (RPTDailyBackDataServiceClient client = new RPTDailyBackDataServiceClient())
        //        {
        //            MethodReturnResult<DataSet> rst = client.Get(new RPTDailyBackDataGetParameter()
        //            {
        //                ReportCode = model.ReportCode,          //报表代码
        //                StartDate = model.StartDate,            //开始日期
        //                EndDate = model.EndDate,                //结束日期
        //                LocationName = model.LocationName,      //车间
        //                LineCode = model.LineCode,              //线别
        //                OrderNumber = model.OrderNumber,        //工单代码
        //                ProductID = model.ProductID             //产品类型
        //            });

        //            if (rst.Code > 0)       //产生错误
        //            {
        //                result.Code = rst.Code;
        //                result.Message = rst.Message;
        //                result.Detail = rst.Detail;

        //                return Json(result);
        //            }
        //            else
        //            {
        //                dtModuleDailyData = rst.Data.Tables[0];         //报表数据
        //                dtModuleReportFormat = rst.Data.Tables[1];      //报表格式
        //            }
        //        }

        //        DataTable dt = new DataTable();

        //        #region 增加项目固定列
        //        //取得报表信息（合计、说明标题）
        //        var reportinforquery = from t in dtModuleReportFormat.AsEnumerable()
        //                               group t by new
        //                               {
        //                                   SUMCaption = t.Field<string>("sumcaption"),         //合计标题
        //                                   MemoCaption = t.Field<string>("memocaption")        //说明标题
        //                               } into g
        //                               select new
        //                               {
        //                                   SUMCaption = g.Key.SUMCaption,
        //                                   MemoCaption = g.Key.MemoCaption
        //                               };

        //        //取得信息
        //        foreach (var data in reportinforquery)
        //        {
        //            strSUMCaption = data.SUMCaption;
        //            strMemoCaption = data.MemoCaption;
        //        }

        //        //增加项目名称字段
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

        //        //增加累计资源字段
        //        dcStatus = new DataColumn("SUM");
        //        dcStatus.Caption = strSUMCaption;
        //        dcStatus.DefaultValue = 0;
        //        dt.Columns.Add(dcStatus);

        //        //增加字段类型
        //        dcStatus = new DataColumn("DataType");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加图形类型
        //        dcStatus = new DataColumn("ChartType");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        //增加数据显示格式
        //        dcStatus = new DataColumn("DataLabelsFormat");
        //        dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
        //        dt.Columns.Add(dcStatus);

        //        #endregion

        //        #region 创建动态时间列
        //        //System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();

        //        //取得日期字段
        //        var query = from t in dtModuleDailyData.AsEnumerable()
        //                    where (t.Field<string>("DataType") == "D")
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
        //            sColumnCode = "data" + Convert.ToDateTime(data.RPDateTime).ToString("MMdd");
        //            sColumnName = Convert.ToDateTime(data.RPDateTime).ToString("M月dd日");

        //            dcStatus = new DataColumn(sColumnCode);
        //            dcStatus.Caption = sColumnName;
        //            dcStatus.DefaultValue = 0;

        //            dt.Columns.Add(dcStatus);
        //        }

        //        //增加计算说明字段
        //        dcStatus = new DataColumn("MEMO");
        //        dcStatus.Caption = strMemoCaption;
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
        //                           DataType = t.Field<string>("datatype"),                      //数据类型（合计、子数据）
        //                           ChartType = t.Field<string>("charttype"),                    //图形类型
        //                           IsDetial = t.Field<System.Int16>("isdetial"),                //明细项目数据标识（1 - 明细数据）
        //                           DataLabelsFormat = t.Field<string>("dataLabelsformat")       //数据标签显示格式
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
        //                var detialquery = from t in dtModuleDailyData.AsEnumerable()
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

        //                    dr["ItemCode"] = "Detail" + detaildata.DetailItem;
        //                    dr["ItemName"] = detaildata.DetailTitle;
        //                    dr["LINK"] = data.RPTURL;
        //                    dr["DataType"] = data.DataType;
        //                    dr["MEMO"] = "";
        //                    dr["ChartType"] = data.ChartType;
        //                    dr["DataLabelsFormat"] = data.DataLabelsFormat;

        //                    dt.Rows.Add(dr);
        //                }
        //            }
        //            else
        //            {
        //                dr = dt.NewRow();

        //                dr["ItemCode"] = data.Item_code;
        //                dr["ItemName"] = data.Item_name;
        //                dr["LINK"] = data.RPTURL;
        //                dr["DataType"] = data.DataType;
        //                dr["MEMO"] = data.Memo;
        //                dr["ChartType"] = data.ChartType;
        //                dr["DataLabelsFormat"] = data.DataLabelsFormat;

        //                dt.Rows.Add(dr);
        //            }
        //        }                
        //        #endregion

        //        #region 填充数据
        //        string strItemCode = "";
        //        string strValues = "";
        //        string strDataType = "";
        //        DateTime dtRPDateTime;
        //        string strFieldDataType = "";
        //        string strDetail = "";
        //        string strDataLabelsFormat = "";

        //        for (int i = 0; i < dtModuleDailyData.Rows.Count; i++)
        //        {
        //            strItemCode = dtModuleDailyData.Rows[i]["ItemCode"].ToString();                 //项目代码
        //            strValues = dtModuleDailyData.Rows[i]["sumQty"].ToString();                     //项目值
        //            strDataType = dtModuleDailyData.Rows[i]["DataType"].ToString();                 //项目数据类型
        //            dtRPDateTime = Convert.ToDateTime(dtModuleDailyData.Rows[i]["rp_datetime"]);    //项目数据日期
        //            strDetail = dtModuleDailyData.Rows[i]["DetailItem"].ToString();                 //明细项目标识
                    
        //            //取得项目所在行数及相关信息
        //            var ItemRowquery = from t in dtModuleReportFormat.AsEnumerable()
        //                               where (t.Field<string>("item_code") == strItemCode)
        //                               select new
        //                               {
        //                                   rowsnum = t.Field<System.Int16>("rowsnum"),
        //                                   datatype = t.Field<string>("datatype"),
        //                                   dataLabelsformat = t.Field<string>("dataLabelsformat")
        //                               };
                    
        //            strFieldDataType = ItemRowquery.FirstOrDefault().datatype;                  //数据类型
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
        //                        strValues = Convert.ToDouble(strValues).ToString(); 


        //                        break;
        //                    case "dec":
        //                        break;
        //                }

        //                //设置数据格式
        //                if (strDataLabelsFormat.Length > 1)
        //                {
        //                    if (strDataLabelsFormat.Substring(strDataLabelsFormat.Length - 1,1) == "%")
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
        //                    case "SUM":     //累计数据
        //                        ItemRow[0]["SUM"] = strValues;
        //                        break;
        //                }
        //            }
        //        }

        //        #endregion

        //        ViewBag.ListData = dt;

        //        if (Request.IsAjaxRequest())
        //        {
        //            return PartialView("_SubDetailListPartial", model);
        //        }
        //        else
        //        {
        //            return View(model);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        result.Code = 1002;
        //        result.Message = e.Message;
        //        result.Detail = e.ToString();
        //    }

        //    return Json(result);
        //}
        //#endregion
    }
}