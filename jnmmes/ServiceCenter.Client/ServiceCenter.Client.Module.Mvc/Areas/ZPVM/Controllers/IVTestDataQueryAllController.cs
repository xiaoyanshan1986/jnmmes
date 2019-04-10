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
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.MES.Model.WIP;
using System.IO;
using System.Data;
using ServiceCenter.Service.Client;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Contract.ZPVM;


namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class IVTestDataQueryAllController : Controller
    {
        //
        // GET: /WIP/IVTestDataQuery/
        //public async Task<ActionResult> Index()
        //{
        //    return await Query(new IVTestDataQueryViewModel());
        //}

        public ActionResult Index()
        {
            IVTestDataQueryViewModel model = new IVTestDataQueryViewModel
            {
                //初始化参数
                //ReportCode = "DAY01",       //报表代码
                //StartDate = System.DateTime.Now.AddDays(1 - System.DateTime.Now.Day).ToString("yyyy-MM-dd"),
                //EndDate = System.DateTime.Now.ToString("yyyy-MM-dd")
            };
            return View(model);
        }

        //
        // GET: /WIP/IVTestDataQuery/
        public async Task<ActionResult> JzIndex()
        {
            return await JzQuery(new IVTestDataQueryViewModel());
        }
        //
        //POST: /WIP/IVTestDataQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(IVTestDataQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (IVTestDataServiceClient client = new IVTestDataServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.TestTime DESC",

                            Where = GetQueryCondition(model)
                        };
                        MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

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
                return PartialView("_ListPartial", new IVTestDataViewModel());
            }
            else
            {
                return View("Index", model);
            }
        }

        /// <summary>调用存储过程查询数据 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> QueryIVdata(IVTestDataQueryViewModel model)
        {

            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (IVTestDataServiceClient client = new IVTestDataServiceClient())
                {
                    LotIVdataParameter param = new LotIVdataParameter();
                    param.Lotlist = model.LotNumber;
                    param.StratTime = model.StartTestTime;
                    param.EndTime = model.EndTestTime;
                    param.IsPrint = model.IsPrint;
                    param.IsDefault = model.IsDefault;
                    param.LineCode = model.lineCode;
                    param.PageNo = model.PageNo;
                    param.PageSize = model.PageSize;

                    MethodReturnResult<DataSet> ds = client.GetIVdata(ref param);
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

        /// <summary>调用存储过程导出TXT</summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public async Task<ActionResult> ExportToExcelIVdata(IVTestDataQueryViewModel model)
        {
            var path = Server.MapPath("~/IVTestDataData.txt");
            DataTable dt = new DataTable();
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            using (IVTestDataServiceClient client = new IVTestDataServiceClient())
            {
                LotIVdataParameter param = new LotIVdataParameter();
                param.Lotlist = model.LotNumber;
                param.StratTime = model.StartTestTime;
                param.EndTime = model.EndTestTime;
                param.IsPrint = model.IsPrint;
                param.IsDefault = model.IsDefault;
                param.LineCode = model.lineCode;
                param.PageNo = model.PageNo;
                param.PageSize = model.PageSize;
                param.PageNo = -1;

                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key.TestTime DESC",
                    };

                    MethodReturnResult<DataSet> ds = client.GetIVdata(ref param);
                    dt = ds.Data.Tables[0];

                });
            }
            System.IO.StreamWriter sw = new System.IO.StreamWriter(path);


            if (dt != null && dt.Rows.Count > 0)
            {
                string strHead = "ITEM_NO,";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    strHead = strHead + dt.Columns[i].ColumnName + ",";

                }
                sw.Write(strHead + "\r\n");
                string strLine = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strLine = i.ToString() + ",";
                    for (int indexOfCol = 0; indexOfCol < dt.Columns.Count; indexOfCol++)
                    {
                        string value = dt.Rows[i][indexOfCol].ToString();

                        //if (value.StartsWith("EMCY"))
                        //{
                        //    LotAttribute LotAttribute = model.GetLaminator(dt.Rows[i][0].ToString());
                        //    value = LotAttribute.AttributeValue;
                        //}

                        if (value == "" || value == null)
                        {
                            value = "null";
                        }

                        strLine = strLine + value + ",";
                    }
                    sw.Write(strLine + "\r\n");
                }
                sw.Close();
            }
            var name = Path.GetFileName(path);
            return File(path, "application/zip-x-compressed", name);
        }



        //public ActionResult Query(IVTestDataQueryViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (IVTestDataServiceClient client = new IVTestDataServiceClient())
        //        {

        //                PagingConfig cfg = new PagingConfig()
        //                {
        //                    OrderBy = "Key.TestTime DESC",

        //                    Where = GetQueryCondition(model)
        //                };
        //                MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

        //                if (result.Code == 0)
        //                {
        //                    ViewBag.PagingConfig = cfg;
        //                    ViewBag.List = result.Data;
        //                };
        //        }
        //    }
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_ListPartial", new IVTestDataViewModel());
        //    }
        //    else
        //    {
        //        return View("Index", model);
        //    }
        //}

        //
        //POST: /WIP/IVTestDataQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> JzQuery(IVTestDataQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (IVTestDataServiceClient client = new IVTestDataServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.TestTime DESC",
                            Where = GetJzQueryCondition(model)
                        };
                        MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

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
                return PartialView("_ListPartial", new IVTestDataViewModel());
            }
            else
            {
                return View("JzIndex", model);
            }
        }
        //
        //POST: /WIP/IVTestDataQuery/PagingQuery
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

                using (IVTestDataServiceClient client = new IVTestDataServiceClient())
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
                        MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new IVTestDataViewModel());
        }
        //
        //POST: /WIP/IVTestDataQuery/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportToExcel(IVTestDataQueryViewModel model)
        {
                IList<IVTestData> lstIVTestData = new List<IVTestData>();

                IVTestDataViewModel m = new IVTestDataViewModel();

                using (IVTestDataServiceClient client = new IVTestDataServiceClient())
                {

                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key.TestTime DESC",
                        Where = GetQueryCondition(model)
                    };
                    MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        lstIVTestData = result.Data;
                    }
                    ;
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
                if (model.IsJZ.ToString() == "False")
                {
                    for (int j = 0; j < lstIVTestData.Count; j++)
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
                            cell.SetCellValue("批次号");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("工单号");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("测试时间");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("线别");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("层压机编号");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("实际功率");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("实际电流");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("实际最大电流");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("实际电压");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("实际最大电压");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("实际填充因子");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("转换效率");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("串联电阻");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("并联电阻");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("测试温度");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("环境温度");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("光强");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("衰减功率");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("电流");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("最大电流");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("衰减电压");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("衰减最大电压");

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

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("有效值？");

                            //cell = row.CreateCell(row.Cells.Count);
                            //cell.CellStyle = style;
                            //cell.SetCellValue("打印？");  

                            //cell = row.CreateCell(row.Cells.Count);
                            //cell.CellStyle = style;
                            //cell.SetCellValue("打印时间");  

                            //cell = row.CreateCell(row.Cells.Count);
                            //cell.CellStyle = style;
                            //cell.SetCellValue("打印次数");  

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("校准时间");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("校准板编号");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("电池片厂商");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("玻璃厂商");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("EVA厂商");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("互联条厂商");

                            cell = row.CreateCell(row.Cells.Count);
                            cell.CellStyle = style;
                            cell.SetCellValue("背板厂商");
                            #endregion
                            font.Boldweight = 5;
                        }
                        IVTestData obj = lstIVTestData[j];
                        IRow rowData = ws.CreateRow(j + 1);

                        LotBOM lotBOMObj = m.GetLotCellMaterial(obj.Key.LotNumber);

                        LotBOM lotBOMGlass = m.GetLotGlassMaterial(obj.Key.LotNumber);

                        LotBOM lotBOMEva = m.GetLotEvaMaterial(obj.Key.LotNumber);

                        LotBOM lotBOMHlt = m.GetLotHltMaterial(obj.Key.LotNumber);

                        LotBOM lotBOMBB = m.GetLotBBMaterial(obj.Key.LotNumber);

                        Lot lot = m.GetLot(obj.Key.LotNumber);

                        Equipment Equipment  = m.GetEquipment(obj.Key.EquipmentCode);

                        LotAttribute LotAttribute = m.GetLaminator(obj.Key.LotNumber);

                        Supplier SupplierCell = null;
                        Supplier SupplierGlass = null;
                        Supplier SupplierEva = null;
                        Supplier SupplierHlt = null;
                        Supplier SupplierBB = null;

                        if (obj.Key.LotNumber.StartsWith("JZ") || obj.Key.LotNumber.StartsWith("HC"))
                        {

                            SupplierCell = null;//电池片供应商

                            SupplierGlass = null;//玻璃供应商

                            SupplierEva = null;//EVA供应商

                            SupplierHlt = null;//互联条供应商

                            SupplierBB = null;//背板供应商

                        }

                        else
                        {

                            SupplierCell = m.GetLotCellMaterialSupplier(obj.Key.LotNumber);//电池片供应商

                            SupplierGlass = m.GetLotGlassMaterialSupplier(obj.Key.LotNumber);//玻璃供应商

                            SupplierEva = m.GetLotEvaMaterialSupplier(obj.Key.LotNumber);//EVA供应商

                            SupplierHlt = m.GetLotHltMaterialSupplier(obj.Key.LotNumber);//互联条供应商

                            SupplierBB = m.GetLotBBMaterialSupplier(obj.Key.LotNumber);//背板供应商

                        }

                        #region //数据
                        ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(j + 1);  //项目号

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.Key.LotNumber);  //批次号

                        if (obj.Key.LotNumber.StartsWith("JZ") || obj.Key.LotNumber.StartsWith("HC"))
                        {
                            cellData = rowData.CreateCell(rowData.Cells.Count);
                            cellData.CellStyle = style;
                            cellData.SetCellValue("");  //工单号

                        }
                        else
                        {
                            cellData = rowData.CreateCell(rowData.Cells.Count);
                            cellData.CellStyle = style;
                            cellData.SetCellValue(lot.OrderNumber);  //工单号

                        }


                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.Key.TestTime));

                        if (obj.Key.LotNumber.StartsWith("JZ") || obj.Key.LotNumber.StartsWith("HC"))
                        {
                            cellData = rowData.CreateCell(rowData.Cells.Count);
                            cellData.CellStyle = style;
                            cellData.SetCellValue(Equipment.LineCode);
                            cellData = rowData.CreateCell(rowData.Cells.Count);
                            cellData.CellStyle = style;
                            cellData.SetCellValue("");
                        }
                        else
                        {
                            cellData = rowData.CreateCell(rowData.Cells.Count);
                            cellData.CellStyle = style;
                            cellData.SetCellValue(lot.LineCode);  //线别
                            cellData = rowData.CreateCell(rowData.Cells.Count);
                            cellData.CellStyle = style;
                            cellData.SetCellValue(LotAttribute != null ? LotAttribute.AttributeValue : string.Empty);  //层压机编号

                        }

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.PM);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.ISC);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.IPM);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.VOC);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.VPM);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.FF);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.EFF);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.RS);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.RSH);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.AmbientTemperature);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.SensorTemperature);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.Intensity);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.CoefPM);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.CoefISC);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.CoefIPM);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.CoefVOC);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.CoefVPM);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.CoefFF);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.CTM * 100 + "%");

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(model.GetEfficiency(obj.Key.LotNumber));

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.PowersetCode);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.PowersetItemNo == null ? string.Empty : Convert.ToString(obj.PowersetItemNo.Value));

                        string powerName = string.Empty;
                        if (!string.IsNullOrEmpty(obj.PowersetCode) && obj.PowersetItemNo != null)
                        {
                            powerName = m.GetPowersetName(obj.Key.LotNumber, obj.PowersetCode, obj.PowersetItemNo.Value);
                        }
                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(powerName);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.PowersetSubCode);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.IsDefault ? StringResource.Yes : StringResource.No);

                        //cellData = rowData.CreateCell(rowData.Cells.Count);
                        //cellData.CellStyle = style;
                        //cellData.SetCellValue(obj.IsPrint ? StringResource.Yes : StringResource.No);

                        //cellData = rowData.CreateCell(rowData.Cells.Count);
                        //cellData.CellStyle = style;
                        //cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.PrintTime));

                        //cellData = rowData.CreateCell(rowData.Cells.Count);
                        //cellData.CellStyle = style;
                        //cellData.SetCellValue(obj.PrintCount);

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.CalibrateTime));

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(obj.CalibrationNo);

                        
                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(lotBOMObj != null ? SupplierCell.Name : string.Empty);  //电池片供应商名称

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(lotBOMGlass != null ? SupplierGlass.Name : string.Empty);  //玻璃供应商名称

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(lotBOMEva != null ? SupplierEva.Name : string.Empty);  //EVA供应商名称

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(lotBOMHlt != null ? SupplierHlt.Name : string.Empty);  //互联条供应商名称

                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(lotBOMBB != null ? SupplierBB.Name : string.Empty);  //背板供应商名称
                        
                        #endregion
                    }
                }

            if (model.IsJZ.ToString() == "True")
            {
                for (int j = 0; j < lstIVTestData.Count; j++)
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
                        cell.SetCellValue("批次号");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("测试时间");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("线别");

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
                        cell.SetCellValue("转换效率");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("串联电阻");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("并联电阻");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("环境温度");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("测试温度");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("光强");

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("实际功率");  

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("实际电流");  

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("实际最大电流");  

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("实际电压");  

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("实际最大电压"); 

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("实际填充因子");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("CTM");

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("电池片效率");

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("分档代码");

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("分档项目号");

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("分档名称");

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("子分档代码");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("有效值？");

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("打印？");

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("打印时间");

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("打印次数");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("校准时间");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("校准板编号");
                        #endregion
                        font.Boldweight = 5;
                    }
                    IVTestData obj = lstIVTestData[j];
                    IRow rowData = ws.CreateRow(j + 1);
                    Equipment Equipment = m.GetEquipment(obj.Key.EquipmentCode);
                    #region //数据
                    ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(j + 1);  //项目号

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.Key.LotNumber);  //批次号

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.Key.TestTime));

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(Equipment.LineCode);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.PM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.ISC);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.IPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.VOC);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.VPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.FF);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.EFF);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.RS);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.RSH);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.AmbientTemperature);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.SensorTemperature);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.Intensity);

                    //cellData =rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.CoefPM);

                    //cellData =rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.CoefISC); 

                    //cellData =rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.CoefIPM);

                    //cellData =rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.CoefVOC); 

                    //cellData =rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.CoefVPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CoefFF);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CTM);

                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(model.GetEfficiency(obj.Key.LotNumber));

                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.PowersetCode);

                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.PowersetItemNo == null ? string.Empty : Convert.ToString(obj.PowersetItemNo.Value));

                    //string powerName = string.Empty;
                    //if (!string.IsNullOrEmpty(obj.PowersetCode) && obj.PowersetItemNo != null)
                    //{
                    //    powerName = m.GetPowersetName(obj.Key.LotNumber, obj.PowersetCode, obj.PowersetItemNo.Value);
                    //}
                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(powerName);

                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.PowersetSubCode);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.IsDefault ? StringResource.Yes : StringResource.No);

                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.IsPrint ? StringResource.Yes : StringResource.No);

                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.PrintTime));

                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.PrintCount);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.CalibrateTime));

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CalibrationNo);

                    #endregion


                }
            }
            if (model.IsJZ.ToString() == "null" || model.IsJZ == null)
            {
                for (int j = 0; j < lstIVTestData.Count; j++)
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
                        cell.SetCellValue("批次号");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("工单号");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("测试时间");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("线别");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("层压机编号");

                         cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("实际功率");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("实际电流");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("实际最大电流");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("实际电压");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("实际最大电压");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("实际填充因子");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("转换效率");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("串联电阻");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("并联电阻");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("测试温度");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("环境温度");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("光强");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("衰减功率");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("电流");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("最大电流");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("衰减电压");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("衰减最大电压");

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

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("有效值？");

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("打印？");  

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("打印时间");  

                        //cell = row.CreateCell(row.Cells.Count);
                        //cell.CellStyle = style;
                        //cell.SetCellValue("打印次数");  

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("校准时间");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("校准板编号");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("电池片厂商");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("玻璃厂商");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("EVA厂商");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("互联条厂商");

                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue("背板厂商");
                        #endregion
                        font.Boldweight = 5;
                    }
                    IVTestData obj = lstIVTestData[j];
                    IRow rowData = ws.CreateRow(j + 1);

                    LotBOM lotBOMObj = m.GetLotCellMaterial(obj.Key.LotNumber);

                    LotBOM lotBOMGlass = m.GetLotGlassMaterial(obj.Key.LotNumber);

                    LotBOM lotBOMEva = m.GetLotEvaMaterial(obj.Key.LotNumber);

                    LotBOM lotBOMHlt = m.GetLotHltMaterial(obj.Key.LotNumber);

                    LotBOM lotBOMBB = m.GetLotBBMaterial(obj.Key.LotNumber);

                    Equipment Equipment = m.GetEquipment(obj.Key.EquipmentCode);

                    LotAttribute LotAttribute = m.GetLaminator(obj.Key.LotNumber);

                    Lot lot = m.GetLot(obj.Key.LotNumber);

                    Supplier SupplierCell =null;
                    Supplier SupplierGlass = null;
                    Supplier SupplierEva = null;
                    Supplier SupplierHlt = null;
                    Supplier SupplierBB = null;

                    if (obj.Key.LotNumber.StartsWith("JZ") || obj.Key.LotNumber.StartsWith("HC"))
                    {

                        SupplierCell = null;//电池片供应商

                        SupplierGlass = null;//玻璃供应商

                        SupplierEva = null;//EVA供应商

                        SupplierHlt = null;//互联条供应商

                        SupplierBB = null;//背板供应商
                       
                    }

                    else 
                    {

                        SupplierCell = m.GetLotCellMaterialSupplier(obj.Key.LotNumber);//电池片供应商

                        SupplierGlass = m.GetLotGlassMaterialSupplier(obj.Key.LotNumber);//玻璃供应商

                        SupplierEva = m.GetLotEvaMaterialSupplier(obj.Key.LotNumber);//EVA供应商

                        SupplierHlt = m.GetLotHltMaterialSupplier(obj.Key.LotNumber);//互联条供应商

                        SupplierBB = m.GetLotBBMaterialSupplier(obj.Key.LotNumber);//背板供应商
                    
                    }
                   

                    #region //数据
                    ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(j + 1);  //项目号

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.Key.LotNumber);  //批次号
                    if (obj.Key.LotNumber.StartsWith("JZ") || obj.Key.LotNumber.StartsWith("HC"))
                    {
                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue("");  //工单号

                    }
                    else 
                    {
                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(lot.OrderNumber);  //工单号
                       
                    }
                    

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.Key.TestTime));

                    if (obj.Key.LotNumber.StartsWith("JZ") || obj.Key.LotNumber.StartsWith("HC"))
                    {
                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(Equipment.LineCode );
                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue("");
                    }
                    else
                    {
                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(lot.LineCode);  //线别
                        cellData = rowData.CreateCell(rowData.Cells.Count);
                        cellData.CellStyle = style;
                        cellData.SetCellValue(LotAttribute != null ? LotAttribute.AttributeValue : string.Empty);  //层压机

                    }

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.PM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.ISC);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.IPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.VOC);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.VPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.FF);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.EFF);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.RS);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.RSH);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.AmbientTemperature);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.SensorTemperature);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.Intensity);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CoefPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CoefISC);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CoefIPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CoefVOC);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CoefVPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CoefFF);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CTM * 100 + "%");

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(model.GetEfficiency(obj.Key.LotNumber));

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.PowersetCode);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.PowersetItemNo == null ? string.Empty : Convert.ToString(obj.PowersetItemNo.Value));

                    string powerName = string.Empty;
                    if (!string.IsNullOrEmpty(obj.PowersetCode) && obj.PowersetItemNo != null)
                    {
                        powerName = m.GetPowersetName(obj.Key.LotNumber, obj.PowersetCode, obj.PowersetItemNo.Value);
                    }
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(powerName);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.PowersetSubCode);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.IsDefault ? StringResource.Yes : StringResource.No);

                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.IsPrint ? StringResource.Yes : StringResource.No);

                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.PrintTime));

                    //cellData = rowData.CreateCell(rowData.Cells.Count);
                    //cellData.CellStyle = style;
                    //cellData.SetCellValue(obj.PrintCount);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.CalibrateTime));

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(obj.CalibrationNo);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(lotBOMObj != null ? SupplierCell.Name : string.Empty);  //电池片供应商名称

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(lotBOMGlass != null ? SupplierGlass.Name : string.Empty);  //玻璃供应商名称

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(lotBOMEva != null ? SupplierEva.Name : string.Empty);  //EVA供应商名称

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(lotBOMHlt != null ? SupplierHlt.Name : string.Empty);  //互联条供应商名称

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(lotBOMBB != null ? SupplierBB.Name : string.Empty);  //背板供应商名称

                    #endregion
                }
            }
                
                MemoryStream ms = new MemoryStream();
                wb.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return File(ms, "application/vnd.ms-excel", "IVTestDataData.xls");    
        }

        //
        //POST: /WIP/IVTestDataQuery/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JzExportToExcel(IVTestDataQueryViewModel model)
        {
            IList<IVTestData> lstIVTestData = new List<IVTestData>();

            IVTestDataViewModel m = new IVTestDataViewModel();

            using (IVTestDataServiceClient client = new IVTestDataServiceClient())
            {

                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "Key.TestTime DESC",
                    Where = GetJzQueryCondition(model)
                };
                MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    lstIVTestData = result.Data;
                }
                ;
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
            for (int j = 0; j < lstIVTestData.Count; j++)
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
                    cell.SetCellValue("批次号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("测试时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("设备代码");

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
                    cell.SetCellValue("转换效率");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("串联电阻");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("并联电阻");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("环境温度");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("测试温度");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("光强");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("实际功率");  

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("实际电流");  

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("实际最大电流");  

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("实际电压");  

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("实际最大电压"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("实际填充因子");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("CTM");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("电池片效率");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("分档代码");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("分档项目号");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("分档名称");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("子分档代码");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("有效值？");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("打印？");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("打印时间");

                    //cell = row.CreateCell(row.Cells.Count);
                    //cell.CellStyle = style;
                    //cell.SetCellValue("打印次数");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("校准时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("校准板编号");
                    #endregion
                    font.Boldweight = 5;
                }
                IVTestData obj = lstIVTestData[j];
                IRow rowData = ws.CreateRow(j + 1);

                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);  //项目号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key.LotNumber);  //批次号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.Key.TestTime));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key.EquipmentCode);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.PM);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.ISC);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.IPM);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.VOC);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.VPM);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.FF);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.EFF);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.RS);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.RSH);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.AmbientTemperature);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.SensorTemperature);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Intensity);

                //cellData =rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.CoefPM);

                //cellData =rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.CoefISC); 

                //cellData =rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.CoefIPM);

                //cellData =rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.CoefVOC); 

                //cellData =rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.CoefVPM);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.CoefFF);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.CTM);

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(model.GetEfficiency(obj.Key.LotNumber));

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.PowersetCode);

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.PowersetItemNo == null ? string.Empty : Convert.ToString(obj.PowersetItemNo.Value));

                //string powerName = string.Empty;
                //if (!string.IsNullOrEmpty(obj.PowersetCode) && obj.PowersetItemNo != null)
                //{
                //    powerName = m.GetPowersetName(obj.Key.LotNumber, obj.PowersetCode, obj.PowersetItemNo.Value);
                //}
                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(powerName);

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.PowersetSubCode);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.IsDefault ? StringResource.Yes : StringResource.No);

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.IsPrint ? StringResource.Yes : StringResource.No);

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.PrintTime));

                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(obj.PrintCount);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.CalibrateTime));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.CalibrationNo);

                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "IVTestDataData.xls");
        }

        public ActionResult ExportToExcel111(IVTestDataQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model.StartTestTime != null && model.StartTestTime.Length > 0)
            {
                where.Append("  and  t1.TEST_TIME> '" + model.StartTestTime + "'  ");
            }
            if (model.EndTestTime != null && model.EndTestTime.Length > 0)
            {
                where.Append("  and t1.TEST_TIME < '" + model.EndTestTime + "'  ");
            }
            if (model.IsDefault == true)
            {
                where.Append("  and  t1.[IS_DEFAULT]=1");
            }
            if (model.IsDefault == false)
            {

              //return ExportToExcelOrld(model);
                where.Append("  and  t1.[IS_DEFAULT]=0");
            }
            if (model.EquipmentCode!=null)
            {
                where.Append("  and t1.[EQUIPMENT_CODE] = '"+model.EquipmentCode+"'  " );
            }
            if (model.LotNumber!=null)
            {
                 where.Append(" and t1.[LOT_NUMBER] = '"+model.LotNumber+"'  " );
            }
            String sql = string.Format(@"SELECT 
	                                           t1.[LOT_NUMBER]
	                                          ,t3.ORDER_NUMBER
                                              ,t1.[TEST_TIME]
                                              ,t1.[EQUIPMENT_CODE]
                                              ,t1.[PM]
                                              ,t1.[ISC]
                                              ,t1.[IPM]
                                              ,t1.[VOC]
                                              ,t1.[VPM]
                                              ,t1.[FF]
                                              ,t1.[EFF]
                                              ,t1.[RS]
                                              ,t1.[RSH]
                                              ,t1.[AMBIENTTEMP]
                                              ,t1.[SENSORTEMP]
                                              ,t1.[INTENSITY]
                                              ,t1.[COEF_PMAX]
                                              ,t1.[COEF_ISC]
                                              ,t1.[COEF_VOC]
                                              ,t1.[COEF_IMAX]
                                              ,t1.[COEF_VMAX]
                                              ,t1.[COEF_FF]
                                              ,t1.[DEC_CTM]
	                                          ,t3.GRADE
	                                          ,t3.COLOR
	                                          ,t3.ATTR_1
	                                          ,t4.PM_NAME
                                              ,t1.[PS_CODE]
                                              ,t1.[PS_ITEM_NO]
                                              ,t1.[PS_SUBCODE]
                                              ,t1.[IS_DEFAULT]
                                              ,t1.[PRINT_COUNT]
                                              ,t1.[CALIBRATE_TIME]
                                              ,t1.[CALIBRATION_NO]
                                              ,t1.[CREATOR]
                                              ,t1.[CREATE_TIME]
                                              ,t1.[EDITOR]
                                              ,t1.[EDIT_TIME]
	                                          ,t2.MATERIAL_CODE
	                                          ,t2.MATERIAL_NAME
	                                          ,t2.SUPPLIER_NAME
                                          FROM [dbo].[ZWIP_IV_TEST] as t1
                                          left join WIP_LOT_BOM as t2 on t1.LOT_NUMBER= t2.LOT_NUMBER
                                          left  join WIP_LOT  as t3  on  t1.LOT_NUMBER = t3.LOT_NUMBER
                                          inner join [ZFMM_POWERSET] as t4 on t1.PS_CODE = t4.PS_CODE and t1.PS_ITEM_NO = t4.ITEM_NO
                                          where  (t2.MATERIAL_CODE  like '110%' or t2.MATERIAL_CODE like'130303%' or t2.MATERIAL_CODE like '180103%' or t2.MATERIAL_CODE like '2511%' ) 
		                                        {0}
                                          order by t1.LOT_NUMBER,t1.TEST_TIME", where.ToString());

            DataTable dt = new DataTable();
            using (DBServiceClient client = new DBServiceClient())
            {
                MethodReturnResult<DataTable> result = client.ExecuteQuery(sql);
                if (result.Code == 0)
                {
                    dt = result.Data;
                }
            }
            where = null;
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
                    cell.SetCellValue("批次号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("工单号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("测试时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("设备代码");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("功率");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("短路电流");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("最大电流");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("开路电压");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("最大电压");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("填充因子");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("转换效率");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("串联电阻");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("并联电阻");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("背板温度");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("环境温度");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("光强");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("实际功率");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("实际最大电流");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("实际电流");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("实际最大电压");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("实际电压");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("实际填充因子");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("CTM");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("等级");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("颜色");

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

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("有效值？");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("校准时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("校准板编号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("创建人");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("创建时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑人");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("物料编码");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("物料名称");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("供应商");


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
                cellData.SetCellValue(dt.Rows[j]["LOT_NUMBER"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["ORDER_NUMBER"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["TEST_TIME"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["EQUIPMENT_CODE"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["PM"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["ISC"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["IPM"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["VOC"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["VPM"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["FF"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["EFF"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["RS"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["RSH"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["AMBIENTTEMP"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["SENSORTEMP"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["INTENSITY"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["COEF_PMAX"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["COEF_ISC"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["COEF_VOC"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["COEF_IMAX"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["COEF_VMAX"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["COEF_FF"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(Convert.ToDouble(dt.Rows[j]["DEC_CTM"].ToString()));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["GRADE"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["COLOR"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["ATTR_1"].ToString());

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

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["IS_DEFAULT"].ToString() == "True" ? StringResource.Yes : StringResource.No);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["CALIBRATE_TIME"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["CALIBRATION_NO"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["CREATOR"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["CREATE_TIME"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["EDITOR"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["EDIT_TIME"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["MATERIAL_CODE"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["MATERIAL_NAME"].ToString());

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["SUPPLIER_NAME"].ToString());



                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "IVTestDataData.xls");
        }


        public string GetQueryCondition(IVTestDataQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();

            //where.Append("  Key.LotNumber Not Like 'JZ%'  ");

            if (model != null)
            {

                if (model.IsJZ != null)
                {
                    if (model.IsJZ.ToString() == "True")
                    {
                        where.Append("  Key.LotNumber Like 'JZ%'  ");
                    }
                    else
                    {
                        where.Append("  Key.LotNumber Not Like 'JZ%'  ");
                    }

                }
                //else 
                //{
                //    where.Append("  Key.LotNumber Not Like 'JZ%' or  Key.LotNumber  Like 'JZ%' ");
                
                //}


                if (!string.IsNullOrEmpty(model.LotNumber))
                {
                    where.AppendFormat(" {0} Key.LotNumber = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LotNumber);
                }

                if (!string.IsNullOrEmpty(model.EquipmentCode))
                {
                    where.AppendFormat(" {0} Key.EquipmentCode LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EquipmentCode);
                }

                if (model.IsDefault!=null)
                {
                    where.AppendFormat(" {0} IsDefault = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.IsDefault);
                }

                if (model.IsPrint != null)
                {
                    where.AppendFormat(" {0} IsPrint = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.IsPrint);
                }

                if (model.StartTestTime != null)
                {
                    where.AppendFormat(" {0} Key.TestTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.StartTestTime);
                }

                if (model.EndTestTime != null)
                {
                    where.AppendFormat(" {0} Key.TestTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EndTestTime);
                }
            }
            return where.ToString();
        }

        public string GetJzQueryCondition(IVTestDataQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            where.Append("  Key.LotNumber Like 'JZ%'  ");

            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.LotNumber))
                {
                    where.AppendFormat(" {0} Key.LotNumber = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LotNumber);
                }

                if (!string.IsNullOrEmpty(model.EquipmentCode))
                {
                    where.AppendFormat(" {0} Key.EquipmentCode LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EquipmentCode);
                }

                if (model.IsDefault != null)
                {
                    where.AppendFormat(" {0} IsDefault = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.IsDefault);
                }

                if (model.IsPrint != null)
                {
                    where.AppendFormat(" {0} IsPrint = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.IsPrint);
                }

                if (model.StartTestTime != null)
                {
                    where.AppendFormat(" {0} Key.TestTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.StartTestTime);
                }

                if (model.EndTestTime != null)
                {
                    where.AppendFormat(" {0} Key.TestTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EndTestTime);
                }
            }
            return where.ToString();
        }
	}
}