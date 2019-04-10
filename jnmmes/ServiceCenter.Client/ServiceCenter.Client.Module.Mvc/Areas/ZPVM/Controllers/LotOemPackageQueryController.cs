using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.MES.Model.ZPVM;
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
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using System.IO;
using System.Data;
using ServiceCenter.Service.Client;
using ServiceCenter.MES.Model.WIP;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class LotOemPackageQueryController : Controller
    {

        //
        // GET: /ZPVM/LotOemPackageQuery/
        public ActionResult Index()
        {
            return View(new LotOemPackageQueryViewModel());
        }
        
        //POST: /ZPVM/LotOemPackageQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(LotOemPackageQueryViewModel model)
        {
            string keyList = null;
            if (model.Type != null || model.SN != null || model.PNOM != null || model.PackageNo != null)
            {
                using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        	
                        IsPaging = false,
                        Where = GetQueryCondition(model)
                    };
                    MethodReturnResult<IList<PackageOemDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0 && result.Data.Count > 0)
                    {
                      
                        StringBuilder strb = new StringBuilder();

                        foreach (var item in result.Data)
                        {   
                            strb.Append("'" + item.Key.SN + "',");
                 
                        }
                        keyList = strb.ToString().Substring(0, strb.Length - 1);
                       
                    }
                    else
                    {
                        return PartialView("_ListPartial", new LotOemPackageViewModel());
                    }
                }
            }

            if (keyList != null)
            {
                using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {   
                      
                        //IsPaging = false,
                        Where = string.Format(" Key in ({0})"
                                                   , keyList),
                        OrderBy = "PackageNo",
                       
                    };
                    MethodReturnResult<IList<PackageOemDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;

                    }
                }
            }
            else
            {
                using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "PackageNo"

                    };
                    MethodReturnResult<IList<PackageOemDetail>> result = client.Get(ref cfg);
                                               
                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;

                    };
                }
            }
            return PartialView("_ListPartial",new LotOemPackageViewModel());

        }
        
        //POST: / ZPVM/LotOemPackageQuery/PagingQuery
       
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

                using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
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
                        MethodReturnResult<IList<PackageOemDetail>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new LotOemPackageViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(LotOemPackageQueryViewModel model)
        {
            IList<PackageOemDetail> lstLotOemPackage = new List<PackageOemDetail>();

            LotOemPackageViewModel m = new LotOemPackageViewModel();

            using (PackageOemQueryServiceClient client = new PackageOemQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "PackageNo",
                        Where = GetQueryCondition(model)
                    };
                    MethodReturnResult<IList<PackageOemDetail>> result = client.GetDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        lstLotOemPackage = result.Data;
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
            for (int j = 0; j < lstLotOemPackage.Count; j++)
            {
                if (j % 65535 == 0)
                {
                    ws = wb.CreateSheet();
                    IRow row = ws.CreateRow(0);
                    #region //列名
                    ICell cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue(StringResource.ItemNo);  //项目号

                
                    cell.SetCellValue("序号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("包装号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("型号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("序列号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("实测功率");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("短路电流");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("开路电压");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("工作电流");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("工作电压");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("填充因子");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("功率档");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电流档");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("时间");

                    #endregion
                    font.Boldweight = 5;
                }
                PackageOemDetail obj = lstLotOemPackage[j];
                IRow rowData = ws.CreateRow(j + 1);
                //Lot lot = m.GetLotData(obj.Key.ObjectNumber);
                ////IVTestData ivtest = m.GetIVTestData(obj.Key.ObjectNumber);

                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);

              

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.No);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.PackageNo);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Type);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key.SN);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.PMP);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.ISC);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.VOC);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.IMP);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.VMP);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.FF);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.PNOM);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.DL);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd }", obj.Time));


                string powerName = string.Empty;
                //if (ivtest != null
                //    && !string.IsNullOrEmpty(ivtest.PowersetCode)
                //    && ivtest.PowersetItemNo != null)
                //{
                //    powerName = m.GetPowersetName(ivtest.Key.LotNumber, ivtest.PowersetCode, ivtest.PowersetItemNo.Value);
                //}
                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(powerName);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                //cellData.SetCellValue(ivtest != null ? ivtest.PowersetSubCode : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd}", obj.Time));
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "LotOemPackageData.xls");
        }

        public string GetQueryCondition(LotOemPackageQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {

                if (!string.IsNullOrEmpty(model.Type))
                {
                    where.AppendFormat(@" {0}  Type Like  '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.Type);
                }
                if (!string.IsNullOrEmpty(model.SN))
                {
                    where.AppendFormat(@" {0} SN  Like '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.SN);
                }

                if (!string.IsNullOrEmpty(model.PNOM))
                {
                    where.AppendFormat(@" {0} PNOM  Like '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PNOM);
                }
                if (!string.IsNullOrEmpty(model.PackageNo))
                {
                    where.AppendFormat(@" {0} PackageNo  Like '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PackageNo);
                }
            }
            return where.ToString();
        }
    }
}