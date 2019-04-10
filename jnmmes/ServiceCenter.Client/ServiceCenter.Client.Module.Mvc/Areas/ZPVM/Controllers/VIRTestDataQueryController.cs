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
    public class VIRTestDataQueryController : Controller
    {
        //
        // GET: /WIP/IVTestDataQuery/
        public async Task<ActionResult> Index()
        {
            return await Query(new VIRTestDataQueryViewModel());
            //return  QueryDB(new VIRTestDataQueryViewModel());
        }

        //
        //POST: /WIP/IVTestDataQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(VIRTestDataQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (VIRTestDataServiceClient client = new VIRTestDataServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.TestTime DESC",
                            Where = GetQueryCondition(model)
                        };
                        MethodReturnResult<IList<VIRTestData>> result = client.Get(ref cfg);

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
                return PartialView("_ListPartial", new VIRTestDataViewModel());
            }
            else
            {
                return View("Index", model);
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult QueryDB(VIRTestDataQueryViewModel model)
        //{
        //    string strErrorMessage = string.Empty;
        //    MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
        //    try
        //    {
        //        LotVIRdataParameter param = new LotVIRdataParameter();
               
        //        using (VIRTestDataServiceClient client = new VIRTestDataServiceClient())
        //        {
        //            param.SelectSQL = "SELECT EQUIPMENT_CODE,TEST_TIME,LOT_NUMBER,TEST_TYPE,TEST_RESULT,TEST_STEP_SEQ,TEST_PARAM1,TEST_PARAM2,VOLTAGE,FREQUENCY,[CURRENT],HILIMIT,LOLIMIT,RAMPUP,DELAYTIME,DWELLTIME,CHARGELO,ARCSENSE FROM [BackUpDBName]ZWIP_VIR_TESTDATA";
        //            param.WhereSQL = "TEST_TIME>'" + model.StartTestTime + "' AND TEST_TIME<'" + model.EndTestTime + "' AND LOT_NUMBER='" + model.LotNumber + "'AND EQUIPMENT_CODE='" + model.EquipmentCode + "'";
        //            param.OrderSQL = "ORDER BY TEST_TIME ASC";

        //            MethodReturnResult<DataSet> ds = client.GetVIRdata(ref param);
        //            ViewBag.ListData = ds.Data.Tables[0];
        //            ViewBag.PagingConfig = new PagingConfig()
        //            {
        //                PageNo = model.PageNo,
        //                PageSize = model.PageSize,
        //                Records = param.TotalRecords

        //            };
        //            model.TotalRecords = param.TotalRecords;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = 1000;
        //        result.Message = ex.Message;
        //        result.Detail = ex.ToString();
        //    }
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_ListPartial", new VIRTestDataViewModel());
        //    }
        //    else
        //    {
        //        return View("Index", model);
        //    }
        //}


        //POST: /ZPVM/VIRTestDataQuery/PagingQuery
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

                using (VIRTestDataServiceClient client = new VIRTestDataServiceClient())
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
                        MethodReturnResult<IList<VIRTestData>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new VIRTestDataViewModel());
        }
        //
        //POST: /ZPVM/VIRTestDataQuery/ExportToExcel

        //
        //POST: /ZPVM/VIRTestDataQuery/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportToExcel(VIRTestDataQueryViewModel model)
        {
            IList<VIRTestData> lstVIRTestData = new List<VIRTestData>();

            VIRTestDataViewModel m = new VIRTestDataViewModel();

            using (VIRTestDataServiceClient client = new VIRTestDataServiceClient())
            {

                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "Key.TestTime DESC",
                    Where = GetQueryCondition(model)
                };
                MethodReturnResult<IList<VIRTestData>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    lstVIRTestData = result.Data;
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
            for (int j = 0; j < lstVIRTestData.Count; j++)
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
                    cell.SetCellValue("仪器型号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("测试时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("批次号");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("测试结果");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("步骤");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("测试类型");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("测试数据1");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("测试数据2");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("输出电压");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("频率");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("加载电流");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("范围上限");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("范围下限");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("缓升时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("延时时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("持续时间");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("最低电荷");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("灵敏度");
                    #endregion

                }


                VIRTestData obj = lstVIRTestData[j];
                IRow rowData = ws.CreateRow(j + 1);

                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);  //项目号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key.EquipmentCode);  //设备号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.Key.TestTime));

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key.LotNumber);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key.TestType);


                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.TestResult);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.TestStepSeq);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.TestResult);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.TestParam1);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.TestParam2);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Voltage);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Frequency);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Ecurren);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Hilimit);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Lolimit);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Rampup);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Delaytime);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Chargelo);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Arcsense);


                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "VIRTestDataData.xls");
        }

        public string GetQueryCondition(VIRTestDataQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();

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