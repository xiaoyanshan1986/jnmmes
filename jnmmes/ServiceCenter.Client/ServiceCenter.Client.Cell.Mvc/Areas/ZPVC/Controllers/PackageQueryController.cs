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
    public class PackageQueryController : Controller
    {
        //
        // GET: /ZPVC/PackageQuery/
        public ActionResult Index()
        {
            return Query(new PackageQueryViewModel());
        }
        //
        //POST: /ZPVC/PackageQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(PackageQueryViewModel model)
        {
            using (PackageInfoServiceClient client = new PackageInfoServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "Key",
                    Where = GetQueryCondition(model)
                };
                MethodReturnResult<IList<PackageInfo>> result = client.Get(ref cfg);

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
        //POST: /ZPVC/PackageQuery/PagingQuery
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

            using (PackageInfoServiceClient client = new PackageInfoServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = pageNo,
                    PageSize = pageSize,
                    Where = where ?? string.Empty,
                    OrderBy = orderBy ?? string.Empty
                };
                MethodReturnResult<IList<PackageInfo>> result = client.Get(ref cfg);
                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }
            return PartialView("_ListPartial", new PackageViewModel());
        }
        //
        //POST: /ZPVC/PackageQuery/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportToExcel(PackageQueryViewModel model)
        {
            IList<PackageInfo> lstPackage = new List<PackageInfo>();

            PackageViewModel m = new PackageViewModel();

            using (PackageInfoServiceClient client = new PackageInfoServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "Key",
                    Where = GetQueryCondition(model)
                };
                MethodReturnResult<IList<PackageInfo>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    lstPackage = result.Data;
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
            for (int j = 0; j < lstPackage.Count; j++)
            {
                if (j % 65535 == 0)
                {
                    ws = wb.CreateSheet();
                    IRow row = ws.CreateRow(0);
                    #region //列名
                    ICell cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(StringResource.ItemNo);  //项目号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_PackageNo); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_Qty); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_Code); 

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
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_OrderNumber);  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(ZPVCResources.StringResource.PackageViewModel_MaterialCode);  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("产品编号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("包装时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("操作人");

                    #endregion
                    font.Boldweight = 5;
                }
                PackageInfo obj = lstPackage[j];
                IRow rowData = ws.CreateRow(j + 1);
                Package packageObj = m.GetPackage(obj.Key);
                ProductionLine plObj = m.GetProductionLine(obj.LineCode);

                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);


                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageObj != null ? packageObj.Quantity : 0);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.ConfigCode);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.EfficiencyName);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Grade);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Color);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.PNType);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(plObj != null ? plObj.Name : obj.LineCode);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageObj.OrderNumber);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(packageObj.MaterialCode);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.ProductId);

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
            return File(ms, "application/vnd.ms-excel", "LotPackageData.xls");
        }

        public string GetQueryCondition(PackageQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.PackageNo) && !string.IsNullOrEmpty(model.PackageNo1))
                {
                    where.AppendFormat(" {0} Key >= '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PackageNo.Trim().ToUpper());
                    where.AppendFormat(" {0} Key <= '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PackageNo1.Trim().ToUpper());
                }
                else
                {
                    where.AppendFormat(" {0} Key LIKE '{1}%'"
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