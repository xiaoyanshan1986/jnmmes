using ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Client.EDC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Areas.EDC.Models;
using System.Text;
using ServiceCenter.MES.Service.Contract.EDC;
using System.Data;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Controllers
{
    public class DataAcquisitionDataController : Controller
    {
        /// <summary> 创建数据列表 </summary>
        /// <param name="dtSet">取得数据集（采集数据列表）</param>
        /// <param name="dt">返回整理后数据列表</param>
        /// <returns></returns>
        public MethodReturnResult CreateTableList(MethodReturnResult<DataSet> dtSet, ref DataTable dt)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                result.Code = 0;
                DataTable dtField = new DataTable();                //字段数据
                DataTable dtRowData = new DataTable();              //行数据列表
                DataTable dtAcquisitionData = new DataTable();      //数据列表
                
                string sColumnType = "";                            //列项目类型
                string sColumn = "";                                //列项目代码
                string sColumnTitle = "";                           //列项目标题
                
                #region 增加项目固定列
                //增加子报表链接字段
                DataColumn dcStatus = new DataColumn("ItemCode");
                dcStatus.Caption = "HIDE";                          //设置隐藏属性在标题（自定义）
                dt.Columns.Add(dcStatus);

                //序号
                dcStatus = new DataColumn("RowsNumber");
                dcStatus.Caption = "序号";
                dt.Columns.Add(dcStatus);

                //采集时间
                dcStatus = new DataColumn("EDCTime");
                dcStatus.Caption = "采集时间";
                dt.Columns.Add(dcStatus);

                //线别
                dcStatus = new DataColumn("LineCode");
                dcStatus.Caption = "线别";
                dt.Columns.Add(dcStatus);
                
                //设备
                dcStatus = new DataColumn("EquipmentCode");
                dcStatus.Caption = "设备";
                dt.Columns.Add(dcStatus);

                //车间
                dcStatus = new DataColumn("LocationName");
                dcStatus.Caption = "车间";
                dt.Columns.Add(dcStatus);                
                #endregion

                #region 创建动态列
                dtField = dtSet.Data.Tables[0];                     //采集字段数据
                dtRowData = dtSet.Data.Tables[1];                   //行数据明细
                dtAcquisitionData = dtSet.Data.Tables[2];           //采集数据明细

                var columnquery = from t in dtField.AsEnumerable()
                                  select new
                                  {
                                      ColumnIndex = t.Field<int>("SerialNumber"),          //字段序号
                                      ColumnType = t.Field<Int16>("DataType"),             //列字段类型
                                      ColumnCode = t.Field<string>("FieldCode"),           //列字段代码
                                      ColumnTitle = t.Field<string>("FieldName"),          //列字段标题
                                  } into r
                                  orderby r.ColumnIndex
                                  select r;

                foreach ( var columndata in columnquery )
                {
                    sColumnType = columndata.ColumnType.ToString();     //列项目类型
                    sColumn = columndata.ColumnCode;                    //列项目代码
                    sColumnTitle = columndata.ColumnTitle;              //列项目标题

                    dcStatus = new DataColumn(sColumn);
                    dcStatus.Caption = sColumnTitle;
                    dcStatus.DefaultValue = 0;

                    dt.Columns.Add(dcStatus);
                }                                
                #endregion

                #region 增加尾部字段
                //数据状态                
                dcStatus = new DataColumn("DataState");
                dcStatus.Caption = "状态";
                dt.Columns.Add(dcStatus);

                //创建时间                
                dcStatus = new DataColumn("CreateTime");
                dcStatus.Caption = "创建时间";                
                dt.Columns.Add(dcStatus);

                //创建人                
                dcStatus = new DataColumn("Creator");
                dcStatus.Caption = "创建人";
                dt.Columns.Add(dcStatus);

                //审核时间                
                dcStatus = new DataColumn("AuditTime");
                dcStatus.Caption = "审核时间";
                dt.Columns.Add(dcStatus);

                //审核人                
                dcStatus = new DataColumn("Auditor");
                dcStatus.Caption = "审核人";
                dt.Columns.Add(dcStatus);

                //编辑时间                
                dcStatus = new DataColumn("EditTime");
                dcStatus.Caption = "编辑时间";
                dt.Columns.Add(dcStatus);

                //编辑人                
                dcStatus = new DataColumn("Editor");
                dcStatus.Caption = "编辑人";
                dt.Columns.Add(dcStatus);
                #endregion

                #region 设置行信息
                //取得行信息字段
                var rowquery = from t in dtRowData.AsEnumerable()
                               select new
                               {
                                   RowsNumber = t.Field<Int64>("RowsNumber"),                   //总结果集行号
                                   EDCTime = t.Field<System.DateTime>("EDCTime"),               //采集时间
                                   ItemCode = t.Field<string>("ItemCode"),                      //项目号
                                   LineCode = t.Field<string>("LINE_CODE"),                     //线别
                                   EquipmentCode = t.Field<string>("EQUIPMENT_CODE"),           //设备代码
                                   DataState = t.Field<Int16>("DataState"),                     //状态
                                   CreateTime = t.Field<System.DateTime>("Create_Time"),        //创建时间
                                   Creator = t.Field<string>("Creator"),                        //创建人
                                   EditTime = t.Field<System.DateTime>("Edit_Time"),            //编辑时间
                                   Editor = t.Field<string>("Editor"),                          //编辑人
                                   AuditTime = t.Field<System.DateTime>("AuditTime"),           //审批时间
                                   Auditor = t.Field<string>("Auditor")                         //审批人
                               } into r
                               orderby r.RowsNumber
                               select r;

                DataRow dr = dt.NewRow();

                //设置行信息
                foreach (var data in rowquery)
                {                    
                    dr = dt.NewRow();

                    dr["RowsNumber"] = data.RowsNumber;             //总结果集行号
                    dr["EDCTime"] = data.EDCTime;                   //采集时间
                    dr["ItemCode"] = data.ItemCode;                 //项目号
                    dr["LineCode"] = data.LineCode;                 //线别
                    dr["EquipmentCode"] = data.EquipmentCode;       //设备代码

                    if (data.DataState == 1)
                    {
                        dr["DataState"] = "审核";                   //状态
                    }
                    else
                    {
                        dr["DataState"] = "";                       //状态
                    }
                    
                    dr["CreateTime"] = data.CreateTime;             //创建时间
                    dr["Creator"] = data.Creator;                   //创建人
                    dr["EditTime"] = data.EditTime;                 //编辑时间
                    dr["Editor"] = data.Editor;                     //编辑人
                    dr["AuditTime"] = data.AuditTime;               //审批时间
                    dr["Auditor"] = data.Auditor;                   //审批人
                    
                    dt.Rows.Add(dr);                    
                }
                #endregion

                #region 填充数据
                string strFieldCode = "";
                string strValues = "";
                int intRowNum = 0;
                
                for (int i = 0; i < dtAcquisitionData.Rows.Count; i++)
                {
                    intRowNum = Convert.ToInt32(dtAcquisitionData.Rows[i]["RowNum"]);               //行号                    
                    strFieldCode = dtAcquisitionData.Rows[i]["FieldCode"].ToString();               //字段代码
                    strValues = dtAcquisitionData.Rows[i]["DataValue"].ToString();                  //值
                    
                    dt.Rows[intRowNum][strFieldCode] = strValues;
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

        /// <summary>
        /// 初始化采集信息主界面
        /// </summary>
        /// <param name="itemCode">项目代码</param>        
        /// <returns></returns>
        public async Task<ActionResult> Index(string itemCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            DataAcquisitionDataQueryViewModel model = new DataAcquisitionDataQueryViewModel() 
            {
                StartDate = System.DateTime.Now.ToString("yyyy-MM-dd 00:00:00"),
                EndDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                EDCTime = System.DateTime.Now,
                PageSize = 20,
                PageNo = 1
            };

            try
            {
                //取得项目信息
                using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
                {
                    await Task.Run(() =>
                    {
                        MethodReturnResult<DataAcquisitionItem> resultItem = client.Get(itemCode);

                        if (result.Code == 0)
                        {
                            ViewBag.DataAcquisitionItem = resultItem.Data;

                            model.RouteOperationName = resultItem.Data.RouteStepName;
                            model.ItemCode = itemCode;
                        }
                    });
                }

                //取得字段列表
                using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
                {
                    await Task.Run(() =>
                    {
                        string where = string.Format("Key.ItemCode = '{0}'", itemCode);

                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            OrderBy = "Key.ItemCode,SerialNumber",
                            Where = where
                        };

                        MethodReturnResult<IList<DataAcquisitionField>> resultField = client.Get(ref cfg);

                        if (resultField.Code == 0)
                        {
                            ViewBag.DataAcquisitionFieldList = resultField.Data;
                        }
                    });
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ListPartial", model);
                }

                return View(model);
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
        /// 查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(DataAcquisitionDataQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            DataAcquisitionDataGetParameter p = new DataAcquisitionDataGetParameter();

            try
            {
                #region 取得数据
                DataTable dt = new DataTable();                     //采集结果集
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "",
                    Where = ""
                };

                using (DataAcquisitionDataServiceClient client = new DataAcquisitionDataServiceClient())
                {
                    //初始化参数                    
                    p.StartDate = model.StartDate;              //开始日期
                    p.EndDate = model.EndDate;                  //结束日期
                    p.ItemCode = model.ItemCode;                //项目代码
                    p.LocationName = model.LocationName;        //车间
                    p.LineCode = model.LineCode;                //线别
                    p.EquipmentCode = model.EquipmentCode;      //设备代码
                    p.PageSize = model.PageSize;
                    p.PageNo = model.PageNo;

                    MethodReturnResult<DataSet> rst = client.GetData(ref p);

                    if (rst.Code > 0)                   //产生错误
                    {
                        result.Code = rst.Code;
                        result.Message = rst.Message;
                        result.Detail = rst.Detail;

                        return Json(result);
                    }
                    else
                    {
                        ViewBag.FieldData = rst.Data.Tables[0];     //采集字段数据
                        cfg.Records = p.Records;                    //总记录数

                        //根据报表格式及数据清单组合数据列表
                        result = CreateTableList(rst, ref dt);

                        if (result.Code > 0)            //产生错误
                        {
                            return Json(result);
                        }
                    }
                }
                #endregion

                //回传参数
                //创建Where条件
                cfg.Where = "?ItemCode=" + model.ItemCode + ";" +
                            "StartDate=" + model.StartDate + ";" +
                            "EndDate=" + model.EndDate + ";" +
                            "LineCode=" + model.LineCode + ";" +
                            "EquipmentCode=" + model.EquipmentCode;                

                ViewBag.PagingConfig = cfg;
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

        /// <summary>
        /// 分页处理程序
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="currentPageNo"></param>
        /// <param name="currentPageSize"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
        {
            MethodReturnResult result = new MethodReturnResult();
            DataAcquisitionDataGetParameter p = new DataAcquisitionDataGetParameter();

            try
            {
                int pageNo = currentPageNo ?? 0;
                int pageSize = currentPageSize ?? 20;
                string s = Request["PageNo"];
                if (Request["PageNo"] != null)
                {
                    pageNo = Convert.ToInt32(Request["PageNo"]);
                }

                if (Request["PageSize"] != null)
                {
                    pageSize = Convert.ToInt32(Request["PageSize"]);
                }

                //初始化参数                    
                p.StartDate = GetValueDataByString(where, "StartDate", "=", ";");              //开始日期
                p.EndDate = GetValueDataByString(where, "EndDate", "=", ";");                  //结束日期
                p.ItemCode = GetValueDataByString(where, "ItemCode", "=", ";");                //项目代码
                p.LocationName = GetValueDataByString(where, "LocationName", "=", ";");        //车间
                p.LineCode = GetValueDataByString(where, "LineCode", "=", ";");                //线别
                p.EquipmentCode = GetValueDataByString(where, "EquipmentCode", "=", ";");      //设备代码
                p.PageSize = pageSize;
                p.PageNo = pageNo + 1;

                await Task.Run(() =>
                {
                    using (DataAcquisitionDataServiceClient client = new DataAcquisitionDataServiceClient())
                    {
                        DataTable dt = new DataTable();                     //采集结果集

                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = pageNo,
                            PageSize = pageSize,
                            Where = where ?? string.Empty,
                            OrderBy = orderBy ?? string.Empty
                        };

                        MethodReturnResult<DataSet> rst = client.GetData(ref p);

                        if (rst.Code > 0)               //产生错误
                        {
                            result.Code = rst.Code;
                            result.Message = rst.Message;
                            result.Detail = rst.Detail;

                            //return result;
                        }
                        else
                        {
                            cfg.Records = p.Records;    //总记录数

                            //根据报表格式及数据清单组合数据列表
                            result = CreateTableList(rst, ref dt);

                            if (result.Code > 0)       //产生错误
                            {
                                //return Json(result);
                            }
                        }

                        ViewBag.PagingConfig = cfg;
                        ViewBag.ListData = dt;
                    }
                });                
                                
                return PartialView("_ListPartial");
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
        /// 保存采集数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(DataAcquisitionDataViewModel model)
        {            
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                string itemCode = model.ItemCode;

                IList<DataAcquisitionData> lstVal = new List<DataAcquisitionData>();
            
                //取得字段列表
                using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
                {
                    await Task.Run(() =>
                    {
                        string where = string.Format("Key.ItemCode = '{0}'", itemCode);

                        //取得字段列表
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            OrderBy = "Key.ItemCode,SerialNumber",
                            Where = where
                        };

                        MethodReturnResult<IList<DataAcquisitionField>> resultField = client.Get(ref cfg);

                        if (resultField.Code > 0)
                        {
                            result.Code = resultField.Code;
                            result.Message = resultField.Message;
                            result.Detail = resultField.Detail;                           
                        }
                        else
                        {
                            DateTime now = DateTime.Now;

                            foreach (DataAcquisitionField field in resultField.Data)
                            {
                                string fieldValue = Request[field.Key.FieldCode] ?? string.Empty;

                                DataAcquisitionData val = new DataAcquisitionData()
                                {
                                    Key = new DataAcquisitionDataKey()
                                    {
                                        EDCTime = model.EDCTime,
                                        ItemCode = model.ItemCode,
                                        FieldCode = field.Key.FieldCode,
                                        LineCode = model.ViewLineCode,
                                        EquipmentCode = model.ViewEquipmentCode,
                                        LocationName = ""
                                    },
                                    DataValue = fieldValue.Split(',')[0],
                                    CreateTime = now,
                                    Creator = User.Identity.Name,
                                    AuditTime = now,
                                    Auditor = "",
                                    EditTime = now,
                                    Editor = User.Identity.Name                                    
                                };

                                lstVal.Add(val);
                            }                            
                        }
                    });

                    if (result.Code == 0)
                    {
                        using (DataAcquisitionDataServiceClient dataclient = new DataAcquisitionDataServiceClient())
                        {
                            result = await dataclient.AddAsync(lstVal);
                            
                            if (result.Code == 0)
                            {
                                result.Message = string.Format(StringResource.DataAcquisitionData_Save_Success);
                            }
                        }
                    }
                    
                    return Json(result);
                }
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
        /// 修改数据提取
        /// </summary>
        /// <param name="eDCTime">采集时间</param>
        /// <param name="itemCode">项目代码</param>
        /// <param name="lineCode">线别</param>
        /// <param name="equipmentCode">设备代码</param>
        /// <param name="locationName">车间</param>
        /// <returns></returns>
        public async Task<ActionResult> Modify(DateTime eDCTime, string itemCode, string lineCode, string equipmentCode, string locationName)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                //参数模型
                DataAcquisitionDataViewModel model = new DataAcquisitionDataViewModel()
                {
                    EDCTime = eDCTime,
                    ItemCode = itemCode,
                    ViewLineCode = lineCode,
                    ViewEquipmentCode = equipmentCode
                };

                await Task.Run(() =>
                {
                    result = GetDataAcquisitionData(eDCTime, itemCode, lineCode, equipmentCode, locationName, ref model);
                });

                if (result.Code > 0)
                {
                    return Json(result);
                }
                else
                {      
                    return PartialView("_ModifyPartial",model);
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
        /// 取得采集数据信息
        /// </summary>
        /// <param name="eDCTime">采集时间</param>
        /// <param name="itemCode">项目代码</param>
        /// <param name="lineCode">线别</param>
        /// <param name="equipmentCode">设备代码</param>
        /// <param name="locationName">车间</param>
        /// <param name="routeStepName">返回项目工序</param>
        /// <returns></returns>
        private MethodReturnResult GetDataAcquisitionData(DateTime eDCTime, string itemCode, string lineCode, string equipmentCode, string locationName, ref DataAcquisitionDataViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                string where = "";

                //取得项目信息
                using (DataAcquisitionItemServiceClient client = new DataAcquisitionItemServiceClient())
                {
                    MethodReturnResult<DataAcquisitionItem> resultItem = client.Get(itemCode);

                    if (result.Code == 0)
                    {
                        //采集项目
                        ViewBag.DataAcquisitionItem = resultItem.Data;

                        //返回采集项目工序
                        model.ViewRouteOperationName = resultItem.Data.RouteStepName;
                    }
                }

                //取得字段列表
                using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
                {
                    where = string.Format("Key.ItemCode = '{0}'", itemCode);

                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key.ItemCode,SerialNumber",
                        Where = where
                    };

                    MethodReturnResult<IList<DataAcquisitionField>> resultField = client.Get(ref cfg);

                    if (resultField.Code == 0)
                    {
                        //采集字段数据
                        ViewBag.DataAcquisitionFieldList = resultField.Data;
                    }
                }

                using (DataAcquisitionDataServiceClient client = new DataAcquisitionDataServiceClient())
                {                    
                    //取得采集数据
                    where = string.Format("Key.EDCTime = '{0}' AND Key.ItemCode = '{1}' AND Key.LineCode = '{2}' AND Key.EquipmentCode = '{3}' AND Key.LocationName = '{4}'"
                                                , eDCTime
                                                , itemCode
                                                , lineCode
                                                , equipmentCode
                                                , locationName);

                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = where
                    };

                    MethodReturnResult<IList<DataAcquisitionData>> resultdata = client.Get(ref cfg);

                    if (resultdata.Code > 0)
                    {
                        result.Code = resultdata.Code;
                        result.Message = resultdata.Message;
                        result.Detail = resultdata.Detail;

                        return result;
                    }

                    model.Creator = resultdata.Data[0].Creator;             //创建人
                    model.CreateTime = resultdata.Data[0].CreateTime;       //创建日期
                    model.Auditor = resultdata.Data[0].Auditor;             //审核人
                    model.AuditTime = resultdata.Data[0].AuditTime;         //审核日期

                    ViewBag.DataAcquisitionDataList = resultdata.Data;
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();                
            }

            return result;
        }

        /// <summary>
        /// 保存修改信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(DataAcquisitionDataViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                string itemCode = model.ItemCode;

                IList<DataAcquisitionData> lstVal = new List<DataAcquisitionData>();

                //取得字段列表
                using (DataAcquisitionFieldServiceClient client = new DataAcquisitionFieldServiceClient())
                {
                    await Task.Run(() =>
                    {
                        string where = string.Format("Key.ItemCode = '{0}'", itemCode);

                        //取得字段列表
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            OrderBy = "Key.ItemCode,SerialNumber",
                            Where = where
                        };

                        MethodReturnResult<IList<DataAcquisitionField>> resultField = client.Get(ref cfg);

                        if (resultField.Code > 0)
                        {
                            result.Code = resultField.Code;
                            result.Message = resultField.Message;
                            result.Detail = resultField.Detail;
                        }
                        else
                        {
                            DateTime now = DateTime.Now;

                            foreach (DataAcquisitionField field in resultField.Data)
                            {
                                string fieldValue = Request[field.Key.FieldCode] ?? string.Empty;

                                DataAcquisitionData val = new DataAcquisitionData()
                                {
                                    Key = new DataAcquisitionDataKey()
                                    {
                                        EDCTime = model.EDCTime,
                                        ItemCode = model.ItemCode,
                                        FieldCode = field.Key.FieldCode,
                                        LineCode = model.ViewLineCode,
                                        EquipmentCode = model.ViewEquipmentCode,
                                        LocationName = ""
                                    },
                                    DataValue = fieldValue.Split(',')[0],
                                    CreateTime = model.CreateTime,
                                    Creator = model.Creator,
                                    AuditTime = model.AuditTime,
                                    Auditor = model.Auditor ?? string.Empty,
                                    EditTime = now,
                                    Editor = User.Identity.Name
                                };

                                lstVal.Add(val);
                            }
                        }
                    });

                    if (result.Code == 0)
                    {
                        using (DataAcquisitionDataServiceClient dataclient = new DataAcquisitionDataServiceClient())
                        {
                            result = await dataclient.ModifyAsync(lstVal);

                            if (result.Code == 0)
                            {
                                result.Message = string.Format(StringResource.DataAcquisitionData_Save_Success);
                            }
                        }
                    }

                    return Json(result);
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
        /// 明细数据提取
        /// </summary>
        /// <param name="eDCTime">采集时间</param>
        /// <param name="itemCode">项目代码</param>
        /// <param name="lineCode">线别</param>
        /// <param name="equipmentCode">设备代码</param>
        /// <param name="locationName">车间</param>
        /// <returns></returns>
        public async Task<ActionResult> Detail(DateTime eDCTime, string itemCode, string lineCode, string equipmentCode, string locationName)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                //参数模型
                DataAcquisitionDataViewModel model = new DataAcquisitionDataViewModel()
                {
                    EDCTime = eDCTime,
                    ItemCode = itemCode,
                    ViewLineCode = lineCode,
                    ViewEquipmentCode = equipmentCode
                };

                await Task.Run(() =>
                {
                    result = GetDataAcquisitionData(eDCTime, itemCode, lineCode, equipmentCode, locationName, ref model);
                });

                if (result.Code > 0)
                {
                    return Json(result);
                }
                else
                {
                    return PartialView("_InfoPartial", model);
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
        /// 删除数据
        /// </summary>
        /// <param name="eDCTime">采集时间</param>
        /// <param name="itemCode">项目代码</param>
        /// <param name="lineCode">线别</param>
        /// <param name="equipmentCode">设备代码</param>
        /// <param name="locationName">车间</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(DateTime eDCTime, string itemCode, string lineCode, string equipmentCode, string locationName)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using (DataAcquisitionDataServiceClient client = new DataAcquisitionDataServiceClient())
                {
                    result = await client.DeleteAsync(eDCTime, itemCode, lineCode, equipmentCode, locationName);

                    if (result.Code == 0)
                    {
                        result.Message = StringResource.DataAcquisitionData_Delete_Success;
                    }
                    return Json(result);
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
        /// 根据属性名称查找字符串中设置的属性值
        /// </summary>
        /// <param name="valueString">属性字符串</param>
        /// <param name="valueName">属性名称</param>
        /// <param name="operators">赋值操作符号</param>
        /// <param name="terminator">终止符</param>
        /// <returns></returns>
        public string GetValueDataByString(string valueString, string valueName, string operators, string terminator)
        {
            string valueData = "";
            int ifind = 0;
            int iEnd = 0;

            ifind = valueString.IndexOf(valueName + operators);

            if (ifind > 0)
            {
                ifind = ifind + valueName.Length + operators.Length;

                iEnd = valueString.IndexOf(terminator, ifind);

                if (iEnd >= ifind)
                {
                    valueData = valueString.Substring(ifind, iEnd - ifind);
                }
                else
                {
                    valueData = valueString.Substring(ifind, valueString.Length - ifind);
                }
            }

            return valueData;
        }
	}
}