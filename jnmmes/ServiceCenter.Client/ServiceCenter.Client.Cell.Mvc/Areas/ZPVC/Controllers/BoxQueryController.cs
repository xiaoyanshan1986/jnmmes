using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.ZPVC.Models;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.MES.Service.Client.ZPVC;
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
using ZPVCResources = ServiceCenter.Client.Mvc.Resources.ZPVC;
using System.IO;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVC.Controllers
{
    public class BoxQueryController : Controller
    {
        //
        // GET: /ZPVC/BoxQuery/
        public ActionResult Index()
        {
            return Query(new BoxQueryViewModel());
        }
        //
        //POST: /ZPVC/BoxQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(BoxQueryViewModel model)
        {
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "Key.PackageNo,ItemNo",
                    Where = GetQueryCondition(model)
                };
                MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new PackageViewModel());
            }
            else
            {
                return View("Index", model);
            }
        }
        //
        //POST: /ZPVC/BoxQuery/PagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
        {
            int pageNo = currentPageNo ?? 0;
            int pageSize = currentPageSize ?? 20;
            if (Request["PageNo"] != null)
            {
                pageNo = Convert.ToInt32(Request["PageNo"]);
            }
            if (Request["PageSize"] != null)
            {
                pageSize = Convert.ToInt32(Request["PageSize"]);
            }

            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = pageNo,
                    PageSize = pageSize,
                    Where = where ?? string.Empty,
                    OrderBy = orderBy ?? string.Empty
                };
                MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);
                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }
            return PartialView("_ListPartial", new PackageViewModel());
        }
        //
        //POST: /ZPVC/BoxQuery/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportToExcel(BoxQueryViewModel model)
        {
            IList<PackageDetail> lstPackageDetail = new List<PackageDetail>();

            PackageViewModel m = new PackageViewModel();

            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "Key.PackageNo,ItemNo",
                    Where = GetQueryCondition(model)
                };
                MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);

                if (result.Code == 0)
                {
                    lstPackageDetail = result.Data;
                }
            }

            //创建工作薄。
            IWorkbook wb = new HSSFWorkbook();
            //设置EXCEL格式
            ICellStyle style = wb.CreateCellStyle();
            style.FillForegroundColor = 10;
            //有边框
            style.BorderBottom = BorderStyle.THIN;
            style.BorderLeft = BorderStyle.THIN;
            style.BorderRight = BorderStyle.THIN;
            style.BorderTop = BorderStyle.THIN;
            IFont font = wb.CreateFont();
            font.Boldweight = 10;
            style.SetFont(font);

            ISheet ws = null;
            for (int j = 0; j < lstPackageDetail.Count; j++)
            {
                if (j % 65535 == 0)
                {
                    ws = wb.CreateSheet();
                    IRow row = ws.CreateRow(0);
                    #region //列名
                    ICell cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(StringResource.ItemNo);  //序号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.BoxViewModel_BoxNo);

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("项目号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_PackageNo);

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_OrderNumber);

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_MaterialCode);

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("产品编号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_Code); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_Qty); 


                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_Name);

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_Grade); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_Color); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_PNType);  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_LineCode);

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("装箱时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("操作人");

                    #endregion
                    font.Boldweight = 5;
                }

                PackageDetail obj = lstPackageDetail[j];
                IRow rowData = ws.CreateRow(j + 1);
                Package packageObj = m.GetPackage(obj.Key.ObjectNumber);
                PackageInfo packageInfoObj = m.GetPackageInfo(obj.Key.ObjectNumber);
                //ProductionLine plObj = m.GetProductionLine(packageInfoObj.LineCode);

                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key.PackageNo);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.ItemNo);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key.ObjectNumber);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.OrderNumber);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.MaterialCode);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageInfoObj != null ? packageInfoObj.ProductId : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageInfoObj != null ? packageInfoObj.ConfigCode : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageObj != null ? packageObj.Quantity : 0);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageInfoObj != null ? packageInfoObj.EfficiencyName : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageInfoObj != null ? packageInfoObj.Grade : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageInfoObj != null ? packageInfoObj.Color : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageInfoObj != null ? packageInfoObj.PNType : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageInfoObj != null ? packageInfoObj.LineCode : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.CreateTime));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Creator);  
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "BoxData.xls");
        }

        public string GetQueryCondition(BoxQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                where.AppendFormat(" {0} Key.ObjectType = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , Convert.ToInt32(EnumPackageObjectType.Packet));

                if (!string.IsNullOrEmpty(model.BoxNo) && !string.IsNullOrEmpty(model.BoxNo1))
                {
                    where.AppendFormat(" {0} Key.PackageNo >= '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BoxNo);
                    where.AppendFormat(" {0} Key.PackageNo <= '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BoxNo1);
                }
                else
                {
                    where.AppendFormat(" {0} Key.PackageNo LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BoxNo);
                }

                if (!string.IsNullOrEmpty(model.PackageNo) && !string.IsNullOrEmpty(model.PackageNo1))
                {
                    where.AppendFormat(" {0} Key.ObjectNumber >= '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PackageNo);
                    where.AppendFormat(" {0} Key.ObjectNumber <= '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PackageNo1);
                }
                else
                {
                    where.AppendFormat(" {0} Key.ObjectNumber LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PackageNo);
                }

                if (model.StartCreateTime != null)
                {
                    where.AppendFormat(" {0} CreateTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.StartCreateTime);
                }

                if (model.EndCreateTime != null)
                {
                    where.AppendFormat(" {0} CreateTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EndCreateTime);
                }
            }
            return where.ToString();
        }
	}
}