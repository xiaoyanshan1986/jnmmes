using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Threading.Tasks;
using ServiceCenter.Client.Mvc.Resources;
using ServiceCenter.MES.Model.ERP;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class ChestQueryController : Controller
    {
        #region 原柜号明细查询方法注释
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ChestDetailQuery(ChestDetailQueryViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (PackageInChestServiceClient client = new PackageInChestServiceClient())
        //        {
        //            await Task.Run(() =>
        //            {
        //                PagingConfig cfg = new PagingConfig()
        //                {
        //                    OrderBy = "Key.ChestNo Desc,ItemNo Asc",
        //                    Where = GetQueryCondition(model)
        //                };
        //                MethodReturnResult<IList<ChestDetail>> result = client.GetDetail(ref cfg);

        //                if (result.Code == 0)
        //                {
        //                    ViewBag.ChestDetailList = result.Data;
        //                    ViewBag.PagingConfig = cfg;
        //                }
        //            });
        //        }
        //    }
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_ChestDetailListPartial", new ChestDetailQueryViewModel());
        //    }
        //    else
        //    {
        //        return View("ChestIndex", model);
        //    }
        //}
        #endregion

        // GET: /ZPVM/ChestQuery/
        /// <summary>
        /// 显示入柜明细界面。
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(new ChestDetailQueryViewModel());
        }

        public ActionResult ChestIndex()
        {
            return View(new ChestDetailQueryViewModel());
        }

        //柜明细查询
        [HttpPost]
        public ActionResult ChestDetailQuery(ChestDetailQueryViewModel model)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                ChestParameter param = new ChestParameter();
                if (model.PackageNo != "" && model.PackageNo != null)
                {
                    param.PackageNo = model.PackageNo;
                }
                else
                {
                    param.PackageNo = "";
                }
                if (model.ChestNo != "" && model.ChestNo != null)
                {
                    param.ChestNo = model.ChestNo;
                }
                else
                {
                    param.ChestNo = "";
                }
                if (model.MaterialCode != "" && model.MaterialCode != null)
                {
                    param.MaterialCode = model.MaterialCode;
                }
                else
                {
                    param.MaterialCode = "";
                }
                if (model.ChestDate != "" && model.ChestDate != null)
                {
                    param.ChestDateStart = model.ChestDate;
                    param.ChestDateEnd = Convert.ToDateTime(model.ChestDate).AddDays(1).ToString("yyyy-MM-dd");
                }
                else
                {
                    param.ChestDateStart = "";
                    param.ChestDateEnd = "";

                }
                if (model.LotNumber != "" && model.LotNumber != null)
                {
                    param.LotNumber = model.LotNumber;
                }
                else
                {
                    param.LotNumber = "";
                }
                if (model.OrderNumber != "" && model.OrderNumber != null)
                {
                    param.OrderNumber = model.OrderNumber;
                }
                else
                {
                    param.OrderNumber = "";
                }
                param.PageSize = model.PageSize;
                param.PageNo = model.PageNo;
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    MethodReturnResult<DataSet> ds = client.GetChestDetailByDB(ref param);
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
                return PartialView("_ChestDetailListPartial", model);
            }
            else
            {
                return View("ChestIndex", model);
            }
        }     

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

                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
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
                        MethodReturnResult<IList<ChestDetail>> result = client.GetDetail(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.ChestDetailList = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ChestDetailListPartial", new ChestDetailQueryViewModel());
        }        

        public string GetQueryCondition(ChestDetailQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.ChestNo))
                {
                    where.AppendFormat(" {0} Key.ChestNo LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ChestNo);
                }

                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    where.AppendFormat(string.Format(@"{0} EXISTS(FROM Package as p
                                                           WHERE p.Key=self.Key.ObjectNumber
                                                           AND p.OrderNumber LIKE '{1}%')"
                                                            , where.Length > 0 ? "AND" : string.Empty
                                                            , model.OrderNumber));
                }

                if (!string.IsNullOrEmpty(model.MaterialCode))
                {
                    where.AppendFormat(" {0} MaterialCode LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialCode);
                }

                if (!string.IsNullOrEmpty(model.PackageNo))
                {
                    where.AppendFormat(" {0} Key.ObjectNumber LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PackageNo);
                }
            }
            return where.ToString();
        }

        //POST: /ZPVM/ChestQuery/DetailQuery
        [HttpPost]
        public ActionResult DetailQuery(ChestDetailQueryViewModel model)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                ChestParameter param = new ChestParameter();
                if (model.PackageNo != "" && model.PackageNo != null)
                {
                    param.PackageNo = model.PackageNo;
                }
                else
                {
                    param.PackageNo = "";
                }
                if (model.ChestNo != "" && model.ChestNo != null)
                {
                    param.ChestNo = model.ChestNo;
                }
                else
                {
                    param.ChestNo = "";
                }
                if (model.MaterialCode != "" && model.MaterialCode != null)
                {
                    param.MaterialCode = model.MaterialCode;
                }
                else
                {
                    param.MaterialCode = "";
                }
                if (model.ChestDate != "" && model.ChestDate != null)
                {
                    param.ChestDateStart = model.ChestDate;
                    param.ChestDateEnd = Convert.ToDateTime(model.ChestDate).AddDays(1).ToString("yyyy-MM-dd");
                }
                else
                {
                    param.ChestDateStart = "";
                    param.ChestDateEnd = "";

                }
                if (model.LotNumber != "" && model.LotNumber != null)
                {
                    param.LotNumber = model.LotNumber;
                }
                else
                {
                    param.LotNumber = "";
                }
                if (model.OrderNumber != "" && model.OrderNumber != null)
                {
                    param.OrderNumber = model.OrderNumber;
                }
                else
                {
                    param.OrderNumber = "";
                }
                param.PageSize = model.PageSize;
                param.PageNo = model.PageNo;
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    MethodReturnResult<DataSet> ds = client.GetChestDetail(ref param);
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
                return PartialView("_DetailListPartial", model);
            }
            else
            {
                return View("Index", model);
            }
        }

        public async Task<ActionResult> ExportToExcel(ChestDetailQueryViewModel model)
        {
            //JsonResult JsonResult = null;
            DataTable dt = new DataTable();
            ChestParameter param = new ChestParameter();
            if (model.PackageNo != "" && model.PackageNo != null)
            {
                param.PackageNo = model.PackageNo;
            }
            else
            {
                param.PackageNo = "";
            }
            if (model.ChestNo != "" && model.ChestNo != null)
            {
                param.ChestNo = model.ChestNo;
            }
            else
            {
                param.ChestNo = "";
            }
            if (model.MaterialCode != "" && model.MaterialCode != null)
            {
                param.MaterialCode = model.MaterialCode;
            }
            else
            {
                param.MaterialCode = "";
            }
            if (model.ChestDate != "" && model.ChestDate != null)
            {
                param.ChestDateStart = model.ChestDate;
                param.ChestDateEnd = Convert.ToDateTime(model.ChestDate).AddDays(1).ToString("yyyy-MM-dd");
            }
            else
            {
                param.ChestDateStart = "";
                param.ChestDateEnd = "";

            }
            if (model.LotNumber != "" && model.LotNumber != null)
            {
                param.LotNumber = model.LotNumber;
            }
            else
            {
                param.LotNumber = "";
            }
            if (model.OrderNumber != "" && model.OrderNumber != null)
            {
                param.OrderNumber = model.OrderNumber;
            }
            else
            {
                param.OrderNumber = "";
            }
            param.PageSize = model.PageSize;
            param.PageNo = -1;
            await Task.Run(() =>
            {
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    MethodReturnResult<DataSet> ds = client.GetChestDetail(ref param);

                    if (ds.Code == 0 && ds.Data != null && ds.Data.Tables.Count > 0)
                    {
                        dt = ds.Data.Tables[0];
                    }
                }
            });
            string template_path = Server.MapPath("~\\Labels\\");//模板路径
            string template_file = template_path + "报检数据.xls";
            FileInfo tempFileInfo = new FileInfo(template_file);
            FileStream file = new FileStream(template_file, FileMode.Open, FileAccess.Read);
            IWorkbook hssfworkbook = new HSSFWorkbook(file);

            //创建sheet
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

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                ICell cellData = null;
                IRow rowData = null;
                //string isLast;

                rowData = sheet1.CreateRow(j + 1);

                #region //数据
                //cellData = rowData.CreateCell(rowData.Cells.Count);
                //cellData.CellStyle = style;
                //cellData.SetCellValue(j + 1);  //序号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["组件号"].ToString());//组件号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["入库日期"].ToString());//入库日期

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["托盘号"].ToString());//托盘号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["柜号"].ToString());//柜号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["组件型号"].ToString());//组件型号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["VOC (V）"].ToString());//VOC (V）

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["ISC（A）"].ToString());//ISC（A）

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["Vmpp (V）"].ToString());//Vmpp (V）

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["Impp（A）"].ToString());//Impp（A）

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["Pmpp（W）"].ToString());//Pmpp（W）

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["FF（%)"].ToString());//FF（%)

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["SurfTemp"].ToString());//SurfTemp

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["电流分档"].ToString());//电流分档

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["标称功率"].ToString());//标称功率

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["电池片厂家"].ToString());//电池片厂家

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["栅线数量"].ToString());//栅线数量

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["电池片档位"].ToString());//电池片档位

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["背板厂家+规格"].ToString());//背板厂家+规格

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["EVA厂家+型号"].ToString());//EVA厂家+型号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["玻璃厂家型号"].ToString());//玻璃厂家型号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["镀膜与否"].ToString());//镀膜与否

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["接线盒厂家+型号+承载电压"].ToString());//接线盒厂家+型号+承载电压

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["线缆长度"].ToString());//线缆长度

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["接线端子型号"].ToString());//接线端子型号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["铝框厂家+型号+颜色+有无加强筋"].ToString());//铝框厂家+型号+颜色+有无加强筋

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["涂锡带厂家"].ToString());//涂锡带厂家

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["硅胶厂家+型号"].ToString());//硅胶厂家+型号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["灌封胶厂家+型号"].ToString());//灌封胶厂家+型号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["认证类型"].ToString());//认证类型

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["包装方式"].ToString());//包装方式

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["组件类型+耐压"].ToString());//组件类型+耐压

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["电池片数"].ToString());//电池片数

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["电池片类型"].ToString());//电池片类型

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["电池片工艺"].ToString());//电池片工艺

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["单位号"].ToString());//单位号

                #endregion
            }
            MemoryStream ms = new MemoryStream();
            hssfworkbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "报检数据.xls");

        }

        public async Task<ActionResult> ExportToExcelChest(ChestDetailQueryViewModel model)
        {
            //IList<ChestDetail> lstChestDetail = new List<ChestDetail>();
            //using (PackageInChestServiceClient client = new PackageInChestServiceClient())
            //{
            //    await Task.Run(() =>
            //    {
            //        PagingConfig cfg = new PagingConfig()
            //        {
            //            IsPaging = false,
            //            OrderBy = "Key.ChestNo Desc,ItemNo Asc",
            //            Where = GetQueryCondition(model)
            //        };
            //        MethodReturnResult<IList<ChestDetail>> result = client.GetDetail(ref cfg);

            //        if (result.Code == 0)
            //        {
            //            lstChestDetail = result.Data;
            //        }
            //    });
            //}

            DataTable dt = new DataTable();
            ChestParameter param = new ChestParameter();
            if (model.PackageNo != "" && model.PackageNo != null)
            {
                param.PackageNo = model.PackageNo;
            }
            else
            {
                param.PackageNo = "";
            }
            if (model.ChestNo != "" && model.ChestNo != null)
            {
                param.ChestNo = model.ChestNo;
            }
            else
            {
                param.ChestNo = "";
            }
            if (model.MaterialCode != "" && model.MaterialCode != null)
            {
                param.MaterialCode = model.MaterialCode;
            }
            else
            {
                param.MaterialCode = "";
            }
            if (model.ChestDate != "" && model.ChestDate != null)
            {
                param.ChestDateStart = model.ChestDate;
                param.ChestDateEnd = Convert.ToDateTime(model.ChestDate).AddDays(1).ToString("yyyy-MM-dd");
            }
            else
            {
                param.ChestDateStart = "";
                param.ChestDateEnd = "";

            }
            if (model.LotNumber != "" && model.LotNumber != null)
            {
                param.LotNumber = model.LotNumber;
            }
            else
            {
                param.LotNumber = "";
            }
            if (model.OrderNumber != "" && model.OrderNumber != null)
            {
                param.OrderNumber = model.OrderNumber;
            }
            else
            {
                param.OrderNumber = "";
            }
            param.PageSize = model.PageSize;
            param.PageNo = -1;
            await Task.Run(() =>
            {
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    MethodReturnResult<DataSet> ds = client.GetChestDetailByDB(ref param);

                    if (ds.Code == 0 && ds.Data != null && ds.Data.Tables.Count > 0)
                    {
                        dt = ds.Data.Tables[0];
                    }
                }
            });

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

            ChestDetailQueryViewModel m = new ChestDetailQueryViewModel();
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
                    cell.SetCellValue("柜号");  //柜号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("库位");  //库位

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("柜属性");  //柜属性

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("项目号");  //柜属性

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("包装号");  //包装号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("托内数量");  //托内数量

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("工单号");  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("产品编码");  //产品编码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("等级");  //等级

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("花色");  //花色

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("功率档位");  //功率档位

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电流档");  //电流档

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("入柜时间");  //入柜时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("托号状态");  //托号状态

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("入库单号");  //花色

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("入库单状态");  //功率档位

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("入库接收核对状态");  //电流档
                    #endregion
                    font.Boldweight = 5;
                }
                //Package package = m.GetPackage(lstChestDetail[j].Key.ObjectNumber);
                //Chest chest = m.GetChest(lstChestDetail[j].Key.ChestNo);
                //WOReportDetail woReportDetail = m.GetWOReportDetail(lstChestDetail[j].Key.ObjectNumber);
                //WOReport woReport = null;
                //if (woReportDetail != null)
                //{
                //    woReport = m.GetWOReport(woReportDetail.Key.BillCode);
                //}
                
                IRow rowData = ws.CreateRow(j + 1);

                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);  //项目号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["柜号"].ToString());  //柜号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["库位"].ToString());  //库位

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["柜属性"].ToString());  //柜属性

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["项目号"].ToString());  //项目号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["包装号"].ToString());  //包装号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["托内数量"].ToString());  //托内数量

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["工单号"].ToString());  //工单号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["产品编码"].ToString());  //产品编码

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["等级"].ToString());  //等级

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["花色"].ToString());  //花色

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["功率档"].ToString());  //功率档

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["电流档"].ToString());  //电流档


                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["入柜时间"].ToString());  //入柜时间

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(((EnumPackageState)dt.Rows[j]["托号状态"]).GetDisplayName());  //托号状态

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(dt.Rows[j]["入库单号"].ToString());  //入库单号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                if (dt.Rows[j]["入库单状态"].ToString() == "")
                {
                    cellData.SetCellValue("");  //入库单状态
                }
                else
                {
                    cellData.SetCellValue(((EnumBillState)dt.Rows[j]["入库单状态"]).GetDisplayName());  //入库单状态
                }
               
                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                if (dt.Rows[j]["入库接收核对状态"].ToString() == "")
                {
                    cellData.SetCellValue("");  //入库接收核对状态
                }
                else
                {
                    cellData.SetCellValue(((EnumPackageCheckState)dt.Rows[j]["入库接收核对状态"]).GetDisplayName());  //入库接收核对状态
                }                
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "柜明细数据.xls");

        }
    }
}