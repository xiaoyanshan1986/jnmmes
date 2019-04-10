using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.Service.Client;
using ServiceCenter.Model;
using System.Text;
using NPOI.SS.UserModel;
using System.IO;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.Client.Mvc.Resources;
using NPOI.HSSF.UserModel;
using ServiceCenter.MES.Service.Client.ERP;
using System.Data;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class IVTestDataQueryForPackageController : Controller
    {
        //
        // GET: /ZPVM/IVTestDataQueryForPackage/
        public ActionResult Index()
        {
            return View("Index", new IVTestDataForPackageQueryViewModel());
        }
        public ActionResult IndexPackage()
        {
            return View("IndexPackage", new IVTestDataForPackageQueryViewModel());
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Query(IVTestDataForPackageQueryViewModel model)
        //{
        //    MethodReturnResult<ActionResult> result = new MethodReturnResult<ActionResult>();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
        //            {
        //                RPTpackagelistParameter param = new RPTpackagelistParameter();
        //                if (model.PackageNo != "" && model.PackageNo != null)
        //                {
        //                    param.PackageNo = model.PackageNo;

        //                }
        //                else
        //                {
        //                    param.PackageNo = "";
        //                }
        //                if (model.LotNumber != "" && model.LotNumber != null)
        //                {
        //                    param.LotNumber = model.LotNumber;
        //                }
        //                else
        //                {
        //                    param.LotNumber = "";

        //                }

        //                await Task.Run(() =>
        //                {
        //                    PagingConfig cfg = new PagingConfig()
        //                    {
        //                        OrderBy = "Key.PackageNo,ItemNo",
        //                        Where = GetQueryCondition(model)
        //                    };
        //                    // MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);
        //                    MethodReturnResult<DataSet> ds = client.GetRPTpackagelistQueryDb(ref param);
        //                    ViewBag.HistoryList = ds;


        //                    if (ds.Code > 0)
        //                    {
        //                        result.Message = ds.Message;
        //                        result.Detail = ds.ToString();
        //                        //return Json(result);
        //                    }
        //                });
        //            }
        //        }
        //        if (Request.IsAjaxRequest())
        //        {
        //            return PartialView("_ListPartial", new IVTestDataForPackageViewModel());
        //        }
        //        else
        //        {
        //            return View("Index", model);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = 1000;
        //        result.Message = ex.Message;
        //        result.Detail = ex.ToString();

        //    }
        //    return Json(result);

        //}


        public ActionResult Query(IVTestDataForPackageQueryViewModel model)
        {

            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                RPTpackagelistParameter param = new RPTpackagelistParameter();
                if (model.PackageNo != "" && model.PackageNo != null)
                {
                    param.PackageNo = model.PackageNo;

                }
                else
                {
                    param.PackageNo = "";
                }
                if (model.LotNumber != "" && model.LotNumber != null)
                {
                    param.LotNumber = model.LotNumber;
                }
                else
                {
                    param.LotNumber = "";

                }
                param.PageSize = model.PageSize;
                param.PageNo = model.PageNo;
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    //RPTLotMateriallistParameter param = new RPTLotMateriallistParameter();
                    //param.LotNumber = model.LotNumber;
                    
                    MethodReturnResult<DataSet> ds = client.GetRPTpackagelistQueryDb(ref param);

                    if (ds.Code > 0)
                    {
                        result.Code = ds.Code;
                        result.Message = ds.Message;
                        result.Detail = ds.Detail;

                        return Json(result);
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

        public async Task<ActionResult> QueryPackage(IVTestDataForPackageQueryViewModel model)
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
                return PartialView("_ListPartialPackage", new IVTestDataForPackageViewModel());
            }
            else
            {
                return View("IndexPackage", model);
            }
        }
        //
        //POST: /WIP/IVTestDataQuery/PagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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
                    };
                }
            }
            return PartialView("_ListPartial", new IVTestDataForPackageViewModel());
        }
        public ActionResult PagingQueryPackage(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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
                    };
                }
            }
            return PartialView("_ListPartialPackage", new IVTestDataForPackageViewModel());
        }
        //
        //POST: /WIP/IVTestDataQuery/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcelEx(IVTestDataForPackageQueryViewModel model)
        {
            //            StringBuilder where = new StringBuilder();
            //            char[] splitChars = new char[] { ',', '$' };
            //            string[] packageNos = model.PackageNo.TrimEnd(splitChars).Split(splitChars);


            //            if (packageNos.Length <= 1)
            //            {
            //                where.AppendFormat("'" + packageNos[0] + "'");
            //            }
            //            else
            //            {
            //                foreach (string package in packageNos)
            //                {
            //                    where.AppendFormat("'{0}',", package);
            //                }
            //                where.Remove(where.Length - 1, 1);
            //            }
            //            String sql = string.Format(@"select t1.PACKAGE_NO,t1.ITEM_NO,t1.OBJECT_NUMBER,t1.ORDER_NUMBER,t1.MATERIAL_CODE,
            //                                            t3.GRADE,t3.COLOR,
            //                                            t2.PM,t2.ISC,t2.IPM,t2.VOC,t2.VPM,t2.FF,t2.EFF,t2.RS,t2.RSH,t2.AMBIENTTEMP,t2.SENSORTEMP,t2.INTENSITY, 
            //                                            t2.COEF_PMAX,t2.COEF_ISC,t2.COEF_IMAX,t2.COEF_VOC,t2.COEF_VMAX,t2.COEF_FF,t2.DEC_CTM,t3.ATTR_1, 
            //                                            t2.PS_CODE,t2.PS_ITEM_NO,t4.PM_NAME,
            //                                            t2.PS_SUBCODE
            //                                            from WIP_PACKAGE_DETAIL t1
            //                                            inner join WIP_LOT t3 on t1.OBJECT_NUMBER =t3.LOT_NUMBER
            //                                            left join ZWIP_IV_TEST t2  on t1.OBJECT_NUMBER =t2.LOT_NUMBER
            //                                            inner join ZFMM_POWERSET t4 on t2.PS_CODE = t4.PS_CODE and t2.PS_ITEM_NO = t4.ITEM_NO 
            //                                            where t2.IS_DEFAULT =1
            //                                            and t1.PACKAGE_NO in (
            //                                            {0}
            //                                            ) order by PACKAGE_NO,ITEM_NO", where);
            //            DataTable dt = new DataTable();
            //            using (DBServiceClient client = new DBServiceClient())
            //            {
            //                MethodReturnResult<DataTable> result = client.ExecuteQuery(sql);
            //                if (result.Code == 0)
            //                {
            //                    dt = result.Data;
            //                }
            //            }
            DataTable dt = new DataTable();
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                RPTpackagelistParameter param = new RPTpackagelistParameter();
                param.PackageNo = model.PackageNo;
                param.LotNumber = model.LotNumber;
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key.PackageNo,ItemNo",
                        Where = GetQueryCondition(model)
                    };
                    // MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);
                    MethodReturnResult<DataSet> ds = client.GetRPTpackagelist(param);
                    dt = ds.Data.Tables[0];
                    //if (result.Code == 0)
                    //{
                    //    ViewBag.PagingConfig = cfg;
                    //    ViewBag.List = result.Data;
                    //    ViewBag.HistoryList = ds;
                    //}
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

            for (int j = 0; j < dt.Rows.Count; j++)
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

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("功率");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("短路电流");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("最大电流");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("开路电压");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("最大电压");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("填充因子");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("转换效率");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("串联电阻");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("并联电阻");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("背板温度");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("环境温度");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("光强");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("功率");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("最大电流");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电流");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("最大电压");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电压");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("填充因子");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("CTM");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电池片效率");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("分档代码");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("分档项目号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("分档名称");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("子分档代码");

                    #endregion
                    font.Boldweight = 5;
                }
                IRow rowData = ws.CreateRow(j + 1);

                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);  //项目号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PACKAGE_NO"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["ITEM_NO"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["OBJECT_NUMBER"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["ORDER_NUMBER"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["MATERIAL_CODE"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["GRADE"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COLOR"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["PM"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["ISC"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["IPM"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["VOC"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["VPM"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["FF"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["EFF"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["RS"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["RSH"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["AMBIENTTEMP"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["SENSORTEMP"].ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(dt.Rows[j]["INTENSITY"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_PMAX"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_ISC"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_IMAX"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_VOC"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_VMAX"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_FF"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["DEC_CTM"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["CellEfficiency"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PS_CODE"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PS_ITEM_NO"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PM_NAME"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PS_SUBCODE"].ToString());

                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "IVTestDataData.xls");
        }

        public async Task<ActionResult> ExportToExcel(IVTestDataForPackageQueryViewModel model)
        {


            JsonResult JsonResult = null;

            StringBuilder where = new StringBuilder();
            char[] splitChars = new char[] { ',', '$' };


            DataTable dt = new DataTable();
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                RPTpackagelistParameter p = new RPTpackagelistParameter();
                if (model.PackageNo != "" && model.PackageNo != null)
                {
                    p.PackageNo = model.PackageNo;
                    string[] packageNos = model.PackageNo.TrimEnd(splitChars).Split(splitChars);


                    if (packageNos.Length <= 1)
                    {
                        where.AppendFormat("'" + packageNos[0] + "'");
                    }
                    else
                    {
                        foreach (string package in packageNos)
                        {
                            where.AppendFormat("'{0}',", package);
                        }
                        where.Remove(where.Length - 1, 1);
                    }
                }
                else
                {
                    p.PackageNo = "";
                }
                if (model.LotNumber != "" && model.LotNumber != null)
                {
                    p.LotNumber = model.LotNumber;
                    string[] LotNumbers = model.LotNumber.TrimEnd(splitChars).Split(splitChars);


                    if (LotNumbers.Length <= 1)
                    {
                        where.AppendFormat("'" + LotNumbers[0] + "'");
                    }
                    else
                    {
                        foreach (string LotNumber in LotNumbers)
                        {
                            where.AppendFormat("'{0}',", LotNumber);
                        }
                        where.Remove(where.Length - 1, 1);
                    }
                }
                else
                {
                    p.LotNumber = "";

                }
                p.PageSize = model.PageSize;
                p.PageNo = -1;
                MethodReturnResult<DataSet> result = client.GetRPTpackagelistQueryDb(ref p);

                if (result.Code == 0 && result.Data != null && result.Data.Tables.Count > 0)
                {
                    dt = result.Data.Tables[0];
                }

            }

            string template_path = Server.MapPath("~\\Labels\\");//模板路径
            string template_file = template_path + "Flash report 模板.xls";
            FileInfo tempFileInfo = new FileInfo(template_file);
            FileStream file = new FileStream(template_file, FileMode.Open, FileAccess.Read);
            IWorkbook hssfworkbook = new HSSFWorkbook(file);

            //创建sheet
            //Number Pallet No.  Type S/N  Pmp [W]  Isc [A]  Voc [V]  Imp [A]  Vmp [V]  FF  Pnom (W)  Current(A)
            NPOI.SS.UserModel.ISheet sheet1 = hssfworkbook.GetSheet("Sheet0");

            //创建单元格和单元格样式

            ICellStyle styles = hssfworkbook.CreateCellStyle();
            styles.FillForegroundColor = 10;
            styles.BorderBottom = BorderStyle.Thin;
            styles.BorderLeft = BorderStyle.Thin;
            styles.BorderRight = BorderStyle.Thin;
            styles.BorderTop = BorderStyle.Thin;
            styles.VerticalAlignment = VerticalAlignment.Center;
            styles.Alignment = HorizontalAlignment.Center;

            IFont font = hssfworkbook.CreateFont();
            font.Boldweight = 10;
            styles.SetFont(font);
            ICellStyle style = hssfworkbook.CreateCellStyle();
            style.FillForegroundColor = 10;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.VerticalAlignment = VerticalAlignment.Center;
            IFont fonts = hssfworkbook.CreateFont();
            font.Boldweight = 10;
            //style.DataFormat = format.GetFormat("yyyy年m月d日");  

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                ICell cellData = null;
                IRow rowData = null;

                rowData = sheet1.CreateRow(j + 3);

                #region //数据
                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);  //序号

                //String a = dt.Rows[j]["MATERIAL_CODE"].ToString();
                //int b = Convert.ToInt32(dt.Rows[j]["MAIN_RAW_QTY"]);
                //String Type = string.Format("JNM{0}{1}"
                //             , a.StartsWith("1201") ? "M" : "P"
                //             , b.ToString());

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(Type);//型号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PACKAGE_NO"].ToString());//包装号


                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["ITEM_NO"].ToString());//项目号


                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["OBJECT_NUMBER"].ToString());//批次号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["ORDER_NUMBER"].ToString());//工单号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["MATERIAL_CODE"].ToString());//物料编码

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["GRADE"].ToString());//等级

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COLOR"].ToString());//花色

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_PMAX"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_ISC"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_IMAX"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_VOC"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_VMAX"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_FF"].ToString());//填充因子

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["DEC_CTM"].ToString());//CTM

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["CellEfficiency"].ToString());//电池片效率

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PS_CODE"].ToString());//分档代码

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PS_ITEM_NO"].ToString());//分档项目号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PM_NAME"].ToString());//功率档

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PS_SUBCODE"].ToString());//电流档

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["AMBIENTTEMP"].ToString());//测试温度

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["TEST_TIME"].ToString());//测试时间

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["RS"].ToString());//串联电阻

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["RSH"].ToString());//并联电阻

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["CHEST_NO"].ToString());//柜号

                #endregion
            }
            MemoryStream ms = new MemoryStream();
            hssfworkbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "IVTestDataData.xls");

        }


        public async Task<ActionResult> ExportToExcelPackage(IVTestDataForPackageQueryViewModel model)
        {


            JsonResult JsonResult = null;

            StringBuilder where = new StringBuilder();
            char[] splitChars = new char[] { ',', '$' };
            string[] packageNos = model.PackageNo.TrimEnd(splitChars).Split(splitChars);


            if (packageNos.Length <= 1)
            {
                where.AppendFormat("'" + packageNos[0] + "'");
            }
            else
            {
                foreach (string package in packageNos)
                {
                    where.AppendFormat("'{0}',", package);
                }
                where.Remove(where.Length - 1, 1);
            }
            String sql = string.Format(@"select 
											    t1.PACKAGE_NO,
											    t1.ITEM_NO,
											    t1.OBJECT_NUMBER,
											    t1.MATERIAL_CODE,
											    t5.MAIN_RAW_QTY,
                                                t3.GRADE,
                                                t3.COLOR,
                                                convert(decimal(18,2), t2.COEF_PMAX) COEF_PMAX ,
											    convert(decimal(18,2), t2.COEF_ISC)COEF_ISC,
											    convert(decimal(18,2), t2.COEF_VOC)COEF_VOC,
											    convert(decimal(18,2), t2.COEF_IMAX)COEF_IMAX,
											    convert(decimal(18,2), t2.COEF_VMAX)COEF_VMAX,
											    convert(decimal(18,2), t2.COEF_FF)COEF_FF,
											    t4.PM_NAME,
											    t2.PS_SUBCODE
                                                from WIP_PACKAGE_DETAIL t1
                                                inner join WIP_LOT t3 on t1.OBJECT_NUMBER =t3.LOT_NUMBER
                                                left join ZWIP_IV_TEST t2  on t1.OBJECT_NUMBER =t2.LOT_NUMBER
                                                inner join ZFMM_POWERSET t4 on t2.PS_CODE = t4.PS_CODE and t2.PS_ITEM_NO = t4.ITEM_NO
											    inner join FMM_MATERIAL t5 on t1.MATERIAL_CODE =t5.MATERIAL_CODE
                                            where 
                                                t2.IS_DEFAULT =1
                                                and t1.PACKAGE_NO 
											    in ({0}) 
											order by PACKAGE_NO,ITEM_NO", where);
            DataTable dt = new DataTable();
            using (DBServiceClient client = new DBServiceClient())
            {
                MethodReturnResult<DataTable> result = client.ExecuteQuery(sql);
                if (result.Code == 0)
                {
                    dt = result.Data;
                }
            }

            string template_path = Server.MapPath("~\\Labels\\");//模板路径
            string template_file = template_path + "Flash report 模板.xls";
            FileInfo tempFileInfo = new FileInfo(template_file);
            FileStream file = new FileStream(template_file, FileMode.Open, FileAccess.Read);
            IWorkbook hssfworkbook = new HSSFWorkbook(file);

            //创建sheet
            //Number Pallet No.  Type S/N  Pmp [W]  Isc [A]  Voc [V]  Imp [A]  Vmp [V]  FF  Pnom (W)  Current(A)
            NPOI.SS.UserModel.ISheet sheet1 = hssfworkbook.GetSheet("Sheet0");

            //创建单元格和单元格样式

            ICellStyle styles = hssfworkbook.CreateCellStyle();
            styles.FillForegroundColor = 10;
            styles.BorderBottom = BorderStyle.Thin;
            styles.BorderLeft = BorderStyle.Thin;
            styles.BorderRight = BorderStyle.Thin;
            styles.BorderTop = BorderStyle.Thin;
            styles.VerticalAlignment = VerticalAlignment.Center;
            styles.Alignment = HorizontalAlignment.Center;

            IFont font = hssfworkbook.CreateFont();
            font.Boldweight = 10;
            styles.SetFont(font);
            ICellStyle style = hssfworkbook.CreateCellStyle();
            style.FillForegroundColor = 10;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.VerticalAlignment = VerticalAlignment.Center;
            IFont fonts = hssfworkbook.CreateFont();
            font.Boldweight = 10;
            //style.DataFormat = format.GetFormat("yyyy年m月d日");  

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                ICell cellData = null;
                IRow rowData = null;

                rowData = sheet1.CreateRow(j + 3);

                #region //数据
                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);  //序号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PACKAGE_NO"].ToString());


                String a = dt.Rows[j]["MATERIAL_CODE"].ToString();
                int b = Convert.ToInt32(dt.Rows[j]["MAIN_RAW_QTY"]);
                String Type = string.Format("JNM{0}{1}"
                             , a.StartsWith("1201") ? "M" : "P"
                             , b.ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Type);


                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["OBJECT_NUMBER"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_PMAX"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_ISC"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_IMAX"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_VOC"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_VMAX"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COEF_FF"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PM_NAME"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["PS_SUBCODE"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["GRADE"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COLOR"].ToString());



                #endregion
            }
            MemoryStream ms = new MemoryStream();
            hssfworkbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "IVTestDataData.xls");

        }
        public string GetQueryCondition(IVTestDataForPackageQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.PackageNo))
                {
                    char[] splitChars = new char[] { ',', '$' };
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

                if (!string.IsNullOrEmpty(model.LotNumber))
                {
                    char[] splitChars = new char[] { ',', '$' };
                    string[] LotNumbers = model.LotNumber.TrimEnd(splitChars).Split(splitChars);
                    if (LotNumbers.Length <= 1)
                    {
                        where.AppendFormat(" {0} Key.LotNumber = '{1}'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , LotNumbers[0]);
                    }
                    else
                    {
                        where.AppendFormat(" {0} Key.LotNumber IN ("
                                            , where.Length > 0 ? "AND" : string.Empty);

                        foreach (string LotNumber in LotNumbers)
                        {
                            where.AppendFormat("'{0}',", LotNumber);
                        }
                        where.Remove(where.Length - 1, 1);
                        where.Append(")");
                    }
                }
            }
            return where.ToString();
        }
    }
}
