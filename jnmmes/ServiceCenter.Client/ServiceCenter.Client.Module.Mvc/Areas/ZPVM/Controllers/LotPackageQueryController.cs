using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
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
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using System.IO;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Model.WIP;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class LotPackageQueryController : Controller
    {
        //
        // GET: /WIP/LotPackageQuery/
        public async Task<ActionResult> Index()
        {
            return await Query(new LotPackageQueryViewModel());
        }
        //
        //POST: /WIP/LotPackageQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(LotPackageQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    await Task.Run(() =>
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
                    });
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new ZPVMLotPackageViewModel());
            }
            else
            {
                return View("Index", model);
            }
        }
        //
        //POST: /WIP/LotPackageQuery/PagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
        {
            if (ModelState.IsValid)
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
                    await Task.Run(() =>
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
                    });
                }
            }
            return PartialView("_ListPartial", new ZPVMLotPackageViewModel());
        }
        //
        //POST: /WIP/LotPackageQuery/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(LotPackageQueryViewModel model)
        {
            IList<PackageDetail> lstLotPackage = new List<PackageDetail>();

            ZPVMLotPackageViewModel m = new ZPVMLotPackageViewModel();

            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        OrderBy = "Key.PackageNo,ItemNo",
                        Where = GetQueryCondition(model)
                    };
                    MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        lstLotPackage = result.Data;
                    }
                });
            }
            //创建工作薄。
            IWorkbook wb = new HSSFWorkbook();
            //设置EXCEL格式
            ICellStyle style = wb.CreateCellStyle();
            style.FillForegroundColor = 10;
            //有边框
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            IFont font = wb.CreateFont();
            font.Boldweight = 10;
            style.SetFont(font);

            ISheet ws = null;
            for (int j = 0; j < lstLotPackage.Count; j++)
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
                    cell.SetCellValue("包装号"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("项目号"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("批次号"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("工单号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("物料编码"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("等级"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("花色");  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("功率");  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电流");  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("最大电流");  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电压");  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("最大电压"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("填充因子");  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("分档名称"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("子分档代码");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("包装日期");  
                    #endregion
                    font.Boldweight = 5;
                }
                PackageDetail obj = lstLotPackage[j];
                IRow rowData = ws.CreateRow(j + 1);
                Lot lot = m.GetLotData(obj.Key.ObjectNumber);
                IVTestData ivtest = m.GetIVTestData(obj.Key.ObjectNumber);
                List<string> dic = null;
                string ff = "";
                OemData oemData = m.GetOemData(obj.Key.ObjectNumber);
                if (oemData != null)
                {
                    dic = m.GetCodeAndItemNo(oemData);
                    ff = (oemData.FF * 100).ToString("F4");
                }
                else
                {
                    lot = m.GetLotData(obj.Key.ObjectNumber);
                    ivtest = m.GetIVTestData(obj.Key.ObjectNumber);
                } 

                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1); 

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key.PackageNo); 

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.ItemNo);

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key.ObjectNumber); 

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.OrderNumber);

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.MaterialCode);

                if (oemData != null)
                {
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(oemData.Grade);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(oemData.Color);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(oemData.PMAX);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(oemData.ISC);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(oemData.IPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(oemData.VOC);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(oemData.VPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ff);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(oemData.PnName == null ? string.Empty : oemData.PnName);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(oemData.PsSubCode);
                }
                else
                {
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(lot != null ? lot.Grade : string.Empty);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(lot != null ? lot.Color : string.Empty);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtest != null ? ivtest.CoefPM : 0);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtest != null ? ivtest.CoefISC : 0);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtest != null ? ivtest.CoefIPM : 0);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtest != null ? ivtest.CoefVOC : 0);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtest != null ? ivtest.CoefVPM : 0);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtest != null ? ivtest.CoefFF : 0);

                    string powerName = string.Empty;
                    if (ivtest != null
                        && !string.IsNullOrEmpty(ivtest.PowersetCode)
                        && ivtest.PowersetItemNo != null)
                    {
                        powerName = m.GetPowersetName(ivtest.Key.LotNumber, ivtest.PowersetCode, ivtest.PowersetItemNo.Value);
                    }
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(powerName);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtest != null ? ivtest.PowersetSubCode : string.Empty);
                }
                
                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd}",obj.CreateTime));
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "LotPackageData.xls");
        }

        public string GetQueryCondition(LotPackageQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.PackageNo) && !string.IsNullOrEmpty(model.PackageNo1))
                {
                    where.AppendFormat(" {0} Key.PackageNo >= '{1}' AND Key.PackageNo<='{2}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PackageNo
                                        , model.PackageNo1);
                }
                else if (!string.IsNullOrEmpty(model.PackageNo))
                {
                    char [] splitChars=new char[] { ',', '$' };
                    string[] packageNos = model.PackageNo.TrimEnd(splitChars).Split(splitChars);
                    if (packageNos.Length <= 1)
                    {
                        where.AppendFormat(" {0} Key.PackageNo = '{1}'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , packageNos[0]);
                    }
                    else
                    {
                        where.AppendFormat(" {0} Key.PackageNo IN ("
                                            , where.Length > 0 ? "AND" : string.Empty);

                        foreach (string package in packageNos)
                        {
                            where.AppendFormat("'{0}',", package);
                        }
                        where.Remove(where.Length - 1, 1);
                        where.Append(")");
                    }
                }


                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    where.AppendFormat(" {0} OrderNumber LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber);
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