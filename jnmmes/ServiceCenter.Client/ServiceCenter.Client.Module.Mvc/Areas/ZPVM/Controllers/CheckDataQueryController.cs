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
using System.Data;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class CheckDataQueryController : Controller
    {
        //
        // GET: /WIP/CheckDataQuery/
        public async Task<ActionResult> Index()
        {
            return await Query(new CheckDataQueryViewModel());
        }
        //
        //POST: /WIP/CheckDataQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(CheckDataQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime DESC",
                            Where = GetQueryCondition(model)
                        };
                        MethodReturnResult<IList<LotTransaction>> result = client.GetTransaction(ref cfg);

                        string where = GetQueryCountCondition(model);
                        MethodReturnResult<DataSet> count = client.GetLotCount(where);

                        if (count.Code == 0 && count.Data.Tables[0].Rows.Count>0)
                        {
                            string qty = count.Data.Tables[0].Rows[0][0].ToString();
                            ViewBag.count = qty;
                            ViewBag.stime =string.Format("{0:yyyy-MM-dd HH:mm:ss}",model.StartCheckTime);
                            ViewBag.etime =string.Format("{0:yyyy-MM-dd HH:mm:ss}",model.EndCheckTime);
                        }

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
                return PartialView("_ListPartial", new CheckDataViewModel());
            }
            else
            {
                return View("Index", model);
            }
        }
        //
        //POST: /WIP/CheckDataQuery/PagingQuery
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

                using (LotQueryServiceClient client = new LotQueryServiceClient())
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
                        MethodReturnResult<IList<LotTransaction>> result = client.GetTransaction(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new CheckDataViewModel());
        }
        //
        //POST: /WIP/CheckDataQuery/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(CheckDataQueryViewModel model)
        {
            IList<LotTransaction> lstCheckData = new List<LotTransaction>();

            CheckDataViewModel m = new CheckDataViewModel();

            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        OrderBy = "CreateTime DESC",
                        Where = GetQueryCondition(model)
                    };
                    MethodReturnResult<IList<LotTransaction>> result = client.GetTransaction(ref cfg);

                    if (result.Code == 0)
                    {
                        lstCheckData = result.Data;
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
            for (int j = 0; j < lstCheckData.Count; j++)
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
                    cell.SetCellValue("产品料号"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("线别");

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("等级"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("花色");

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
                    cell.SetCellValue("分档名称");  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("子分档代码"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("备注");  

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("检验时间"); 

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("检验操作人"); 
                    #endregion
                    font.Boldweight = 5;
                }
                LotTransaction obj = lstCheckData[j];
                IRow rowData = ws.CreateRow(j + 1);
                Lot lotObj = m.GetLot(obj.LotNumber);
                IVTestData ivtestData = m.GetIVTestData(obj.LotNumber);
                LotTransactionHistory lotHisObj = m.GetLotTransactionHistory(obj.Key);
                    
                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);  //项目号

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.LotNumber);  //批次号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.OrderNumber);  //工单号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(lotHisObj != null ? lotHisObj.MaterialCode : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(lotHisObj != null ? lotHisObj.LineCode : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(lotObj != null ? lotObj.Grade : string.Empty);

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(lotObj != null ? lotObj.Color : string.Empty);

                if(ivtestData!=null)
                {

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtestData.CoefPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtestData.CoefISC);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtestData.CoefIPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtestData.CoefVOC);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtestData.CoefVPM);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtestData.CoefFF);
                }
                else
                {

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Empty);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Empty);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Empty);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Empty);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Empty);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Empty);
                }

                if (ivtestData==null || string.IsNullOrEmpty(ivtestData.PowersetCode) || ivtestData.PowersetItemNo == null)
                {
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Empty);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(string.Empty);
                }
                else
                {
                    string powersetName = m.GetPowersetName(ivtestData.Key.LotNumber, ivtestData.PowersetCode, ivtestData.PowersetItemNo.Value);
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(powersetName);

                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    cellData.SetCellValue(ivtestData.PowersetSubCode);
                }

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Description);


                cellData =rowData.CreateCell(rowData.Cells.Count);
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
            return File(ms, "application/vnd.ms-excel", "CheckDataData.xls");
        }

        public string GetQueryCondition(CheckDataQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                where.AppendFormat(" {0} RouteStepName= '终检' AND Activity='{1}' AND UndoFlag=0"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , Convert.ToInt32(EnumLotActivity.TrackOut));

                if (!string.IsNullOrEmpty(model.LotNumber))
                {
                    where.AppendFormat(" {0} LotNumber= '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LotNumber);
                }

                if (!string.IsNullOrEmpty(model.LineCode))
                {
                    where.AppendFormat(@" {0} EXISTS(FROM LotTransactionHistory as p
                                                     WHERE p.Key=self.Key
                                                     AND p.LineCode='{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LineCode);
                }

                if (model.StartCheckTime != null)
                {
                    where.AppendFormat(" {0} CreateTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.StartCheckTime);
                }

                if (model.EndCheckTime != null)
                {
                    where.AppendFormat(" {0} CreateTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EndCheckTime);
                }
            }
            return where.ToString();
        }

        public string GetQueryCountCondition(CheckDataQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                where.AppendFormat(" {0} ROUTE_STEP_NAME ='终检' AND ACTIVITY=2 AND UNDO_FLAG=0"
                                        , where.Length > 0 ? "AND" : string.Empty);

                if (!string.IsNullOrEmpty(model.LotNumber))
                {
                    where.AppendFormat(" {0} LOT_NUMBER= '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LotNumber);
                }

                if (!string.IsNullOrEmpty(model.LineCode))
                {
                    where.AppendFormat(@" {0} EXISTS(SELECT LINE_CODE FROM dbo.WIP_TRANSACTION_LOT as p
                                                     WHERE p.TRANSACTION_KEY=w.TRANSACTION_KEY
                                                     AND p.LINE_CODE='{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LineCode);
                }

                if (model.StartCheckTime != null)
                {
                    where.AppendFormat(" {0} CREATE_TIME >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.StartCheckTime);
                }

                if (model.EndCheckTime != null)
                {
                    where.AppendFormat(" {0} CREATE_TIME <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EndCheckTime);
                }
            }
            return where.ToString();
        }
	}
}