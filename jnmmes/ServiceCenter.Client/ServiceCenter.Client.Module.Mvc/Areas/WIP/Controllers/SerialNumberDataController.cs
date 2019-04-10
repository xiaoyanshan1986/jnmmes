using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Resources;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using System.IO;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.RPT;
using System.Data;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using NPOI.XSSF.UserModel;
using ServiceCenter.MES.Service.Client.FMM;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class SerialNumberDataController : Controller
    {
        public class NpoiMemoryStream : MemoryStream
        {
            public NpoiMemoryStream()
            {
                AllowClose = true;
            }

            public bool AllowClose { get; set; }

            public override void Close()
            {
                if (AllowClose)
                    base.Close();
            }
        }

        public ActionResult Index()
        {
            return View(new SerialNumberDataViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(SerialNumberDataViewModel model)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                RPTLotQueryDetailParameter param = new RPTLotQueryDetailParameter();
                param = GetQueryCondition(model,true);
                if ((model.PackageNo == "" || model.PackageNo == null)
                 && (model.LotNumber == "" || model.LotNumber == null)
                 && (model.OrderNumber == "" || model.OrderNumber == null)
                 && (model.MaterialCode == "" || model.LotNumber == null))
                {
                    result.Code = 2000;
                    result.Message = "包装号-批次号-工单号-产品编码四个筛选条件不可都为空！";

                    return Json(result);
                }
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    MethodReturnResult<DataSet> ds = client.GetMapDataQueryDb(ref param);

                    if (ds.Code > 0)
                    {
                        result.Code = ds.Code;
                        result.Message = ds.Message;
                        result.Detail = ds.Detail;

                        return Json(result);
                    }
                    else
                    {
                        result.Message = "查询完毕";
                    }

                    ViewBag.ListData = ds.Data.Tables[0];
                    ViewBag.PagingConfig = new PagingConfig()
                    {
                        PageNo = model.PageNo,
                        PageSize = model.PageSize,
                        Records = param.TotalRecords

                    };
                    model.TotalRecords = param.TotalRecords;

                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", model);
            }
            else
            {
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportToExcel(SerialNumberDataViewModel model)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                DataTable dt = new DataTable();
                RPTLotQueryDetailParameter param = new RPTLotQueryDetailParameter();
                param = GetQueryCondition(model,false);
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    MethodReturnResult<DataSet> ds = client.GetMapDataQueryDb(ref param);

                    if (ds.Code == 0 && ds.Data != null && ds.Data.Tables.Count > 0)
                    {
                        dt = ds.Data.Tables[0];
                    }
                }
                string template_path = Server.MapPath("~\\Labels\\");//模板路径
                string template_file = template_path + "匹配数据模板.xlsx";
                FileInfo tempFileInfo = new FileInfo(template_file);
                FileStream file = new FileStream(template_file, FileMode.Open, FileAccess.Read);
                IWorkbook xssfworkbook = new XSSFWorkbook(file);

                //创建sheet
                ISheet sheet1 = xssfworkbook.GetSheet("Sheet1");

                //创建单元格和单元格样式

                ICellStyle styles = xssfworkbook.CreateCellStyle();
                styles.FillForegroundColor = 10;
                styles.BorderBottom = BorderStyle.Thin;
                styles.BorderLeft = BorderStyle.Thin;
                styles.BorderRight = BorderStyle.Thin;
                styles.BorderTop = BorderStyle.Thin;
                styles.VerticalAlignment = VerticalAlignment.Center;
                styles.Alignment = HorizontalAlignment.Center;

                IFont font = xssfworkbook.CreateFont();
                font.Boldweight = 10;
                styles.SetFont(font);
                ICellStyle style = xssfworkbook.CreateCellStyle();
                style.FillForegroundColor = 10;
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                style.VerticalAlignment = VerticalAlignment.Center;
                IFont fonts = xssfworkbook.CreateFont();
                font.Boldweight = 10;

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    ICell cellData = null;
                    IRow rowData = null;

                    rowData = sheet1.CreateRow(j + 2);

                    #region //数据
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(j + 1);  //序号

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(dt.Rows[j]["SN码"].ToString());//SN码


                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(dt.Rows[j]["标签条码"].ToString());//标签条码


                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(dt.Rows[j]["标签条码位数"].ToString());//标签条码位数

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(dt.Rows[j]["标签条码格式"].ToString());//标签条码格式

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(dt.Rows[j]["工单号"].ToString());//工单号

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(dt.Rows[j]["包装号"].ToString());//包装号

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(dt.Rows[j]["产品编码"].ToString());//产品编码

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(dt.Rows[j]["工步"].ToString());//工步

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(dt.Rows[j]["订单编号"].ToString());//订单编号

                    #endregion
                }
                var ms = new NpoiMemoryStream();
                ms.AllowClose = false;
                xssfworkbook.Write(file);
                xssfworkbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                ms.AllowClose = false;
                return File(ms, "application/vnd.ms-excel", "匹配数据模板.xlsx");
            }
            catch(Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
                return Content(result.Message) ;
            }                           
        }

        public RPTLotQueryDetailParameter GetQueryCondition(SerialNumberDataViewModel model,bool flag)
        {
            RPTLotQueryDetailParameter param = new RPTLotQueryDetailParameter();
            //包装号
            if (model.PackageNo != "" && model.PackageNo != null)
            {
                param.PackageNo = model.PackageNo;
            }
            else
            {
                param.PackageNo = "";
            }
            //批次号
            if (model.LotNumber != "" && model.LotNumber != null)
            {
                param.LotNumber = model.LotNumber;
            }
            else
            {
                param.LotNumber = "";
            }
            //工艺工步
            if (model.RouteStepName != "" && model.RouteStepName != null)
            {
                param.RouteStepName = model.RouteStepName;
            }
            else
            {
                param.RouteStepName = "";
            }
            //工单号
            if (model.OrderNumber != "" && model.OrderNumber != null)
            {
                param.OrderNumber = model.OrderNumber;

            }
            else
            {
                param.OrderNumber = "";
            }
            //产品编码
            if (model.MaterialCode != "" && model.MaterialCode != null)
            {
                param.MaterialCode = model.MaterialCode;

            }
            else
            {
                param.MaterialCode = "";
            }
            //标签条码格式
            if (model.MapType != "" && model.MapType != null)
            {
                param.MapType = model.MapType;

            }
            else
            {
                param.MapType = "";
            }
            
            param.PageSize = model.PageSize;
            if (flag)
            {
                param.PageNo = model.PageNo;
            }
            else
            {
                param.PageNo = -1;
            }
            return param;
        }

        //获取工单号
        public ActionResult GetOrderNumber(string q)
        {
            IList<WorkOrder> lstDetail = new List<WorkOrder>();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key LIKE '{0}%' AND OrderState='0' AND CloseType=0"
                                           , q),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            var lnq = from item in lstDetail
                      select item.Key;

            return Json(from item in lstDetail
                        select new
                        {
                            @label = item.Key + "-" + item.MaterialCode,
                            @value = item.Key,
                            @ProductCode = item.MaterialCode
                        }, JsonRequestBehavior.AllowGet);
        }        

    }
}