using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.RPT.Models;
using ServiceCenter.MES.Service.Client.RPT;
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.Client.Mvc.Resources;
using System.Threading.Tasks;
using System.Text;
using NPOI.XSSF.UserModel;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.FMM;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Controllers
{
    public class LotMaterialListController : Controller
    {
        //
        // GET: /RPT/LotMaterialList/

        /// <summary>
        /// 定义静态数据集和静态记录总数，临时存储从后台获取的不分页数据的信息
        /// </summary>        
        protected static MethodReturnResult<DataSet> Resulted = new MethodReturnResult<DataSet>();
        protected static int AllRecords;

        /// <summary>
        /// 批次物料出库数据界面初始化
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexMaterial()
        {
            return View("IndexMaterial", new LotMaterialListOutViewModel());
        }

        /// <summary>
        /// 首页查询方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Querys(LotMaterialListOutViewModel model)
        {
            //如果出货包装号不为空且为多个
            if (model.OutPackageNo != "" && model.OutPackageNo != null)
            {
                if (model.OutPackageNo.IndexOf(",") != -1)
                {
                    model.OutPackageNo = model.OutPackageNo.Replace(",", "','");
                }
            }
            //如果查询条件均为空
            if ((model.BomMaterialCode == "" || model.BomMaterialCode == null)
             && (model.BomMaterialName == "" || model.BomMaterialName == null)
             && (model.ProductMaterialCode == "" || model.ProductMaterialCode == null)
             && (model.OutPackageNo == "" || model.OutPackageNo == null)
             && model.OutStartTime == null && model.OutEndTime == null)
            {
                ViewBag.Message = "请输入查询条件!";
            }

            //如果查询条件不为空
            else
            {
                //如果产品编码不为空
                if (model.ProductMaterialCode != "" && model.ProductMaterialCode != null)
                {
                    //如果物料编码或物料名称或出库托号全部为空
                    if ((model.BomMaterialCode == "" || model.BomMaterialCode == null)
                     && (model.BomMaterialName == "" || model.BomMaterialName == null)
                     && (model.OutPackageNo == "" || model.OutPackageNo == null))
                    {
                        ViewBag.Message = "查询条件不足，请增加物料编码或物料名称筛选条件!";
                    }
                    //如果物料编码或物料名称或出库托号至少有一个不为空
                    else
                    {
                        Resulted = GetAllData(model);
                        if (Resulted.Code == 0 && Resulted.Data != null && Resulted.Data.Tables.Count > 0)
                        {
                            model.PageNo = 0;
                            DataTable result = GetPageData(model);
                            if (result != null && result.Rows.Count > 0)
                            {
                                ViewBag.ListData = result;
                                ViewBag.PagingConfig = new PagingConfig()
                                {
                                    PageNo = model.PageNo,
                                    PageSize = model.PageSize,
                                    Records = AllRecords
                                };
                            }
                            else
                            {
                                ViewBag.Message = "无数据";
                            }
                        }
                    }
                }
                //如果产品编码为空
                else
                {
                    //如果物料编码或物料名称或出库托号全部为空
                    if ((model.BomMaterialCode == "" || model.BomMaterialCode == null)
                     && (model.BomMaterialName == "" || model.BomMaterialName == null)
                     && (model.OutPackageNo == "" || model.OutPackageNo == null))
                    {
                        ViewBag.Message = "查询条件不足，请增加筛选条件!";
                    }
                    //如果物料编码或物料名称或出库托号至少有一个不为空
                    else
                    {
                        //如果物料名称不为空
                        if (model.BomMaterialName != "" || model.BomMaterialName != null)
                        {
                            //如果物料编码或出库托号全部为空
                            if ((model.BomMaterialCode == "" || model.BomMaterialCode == null)
                             && (model.OutPackageNo == "" || model.OutPackageNo == null))
                            {
                                ViewBag.Message = "查询条件不足，请增加物料编码或出库托号筛选条件!";
                            }
                            else
                            {
                                Resulted = GetAllData(model);
                                if (Resulted.Code == 0 && Resulted.Data != null && Resulted.Data.Tables.Count > 0)
                                {
                                    model.PageNo = 0;
                                    DataTable result = GetPageData(model);
                                    if (result != null && result.Rows.Count > 0)
                                    {
                                        ViewBag.ListData = result;
                                        ViewBag.PagingConfig = new PagingConfig()
                                        {
                                            PageNo = model.PageNo,
                                            PageSize = model.PageSize,
                                            Records = AllRecords
                                        };
                                    }
                                    else
                                    {
                                        ViewBag.Message = "无数据";
                                    }
                                }
                            }
                        }
                        //如果物料名称为空
                        else
                        {
                            Resulted = GetAllData(model);
                            if (Resulted.Code == 0 && Resulted.Data != null && Resulted.Data.Tables.Count > 0)
                            {
                                model.PageNo = 0;
                                DataTable result = GetPageData(model);
                                if (result != null && result.Rows.Count > 0)
                                {
                                    ViewBag.ListData = result;
                                    ViewBag.PagingConfig = new PagingConfig()
                                    {
                                        PageNo = model.PageNo,
                                        PageSize = model.PageSize,
                                        Records = AllRecords
                                    };
                                }
                                else
                                {
                                    ViewBag.Message = "无数据";
                                }
                            }
                        }
                    }
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartialMaterial", model);
            }
            else
            {
                return View("IndexMaterial", model);
            }
        }

        /// <summary>
        /// 新建类 重写Npoi流方法
        /// </summary>
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

        /// <summary>
        /// 导出批次物料出库数据到Excel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> ExportOutToExcel(LotMaterialListOutViewModel model)
        {
            DataTable dt = new DataTable();
            if (Resulted.Code == 0 && Resulted.Data != null && Resulted.Data.Tables.Count > 0)
            {
                dt = Resulted.Data.Tables[0];
            }
            else
            {
                using (LotMaterialListServiceClient client = new LotMaterialListServiceClient())
                {
                    MaterialDataParameter p = new MaterialDataParameter()
                    {
                        ProductMaterialCode = model.ProductMaterialCode,
                        BomMaterialCode = model.BomMaterialCode,
                        BomMaterialName = model.BomMaterialName,
                        OutPackageNo = model.OutPackageNo,
                        OutStartTime = model.OutStartTime,
                        OutEndTime = model.OutEndTime,
                        PageSize = model.PageSize,
                        PageNo = 0
                    };
                    await Task.Run(() =>
                    {
                        MethodReturnResult<DataSet> ds = client.GetRPTMaterialData(ref p);
                        dt = ds.Data.Tables[0];
                    });
                }
            }
            #region 导出到EXCEL
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/LotMaterialOutData.xlsx");            
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read);
            //创建工作薄。
            IWorkbook wb = new XSSFWorkbook();
            //IWorkbook wb = new HSSFWorkbook();
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
                if (j == 0)
                {
                    ws = wb.CreateSheet();
                    IRow row = ws.CreateRow(0);
                    ICell cell = null;
                    #region //列名
                    foreach (DataColumn dc in dt.Columns)
                    {
                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue(dc.Caption);
                    }
                    #endregion
                    font.Boldweight = 5;
                }
                IRow rowData = ws.CreateRow(j + 1);
                #region //数据
                ICell cellData = null;
                foreach (DataColumn dc in dt.Columns)
                {
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;

                    if (dc.DataType == typeof(double) || dc.DataType == typeof(float))
                    {
                        cellData.SetCellValue(Convert.ToDouble(dt.Rows[j][dc]));
                    }
                    else if (dc.DataType == typeof(int))
                    {
                        cellData.SetCellValue(Convert.ToInt32(dt.Rows[j][dc]));
                    }
                    else
                    {
                        cellData.SetCellValue(Convert.ToString(dt.Rows[j][dc]));
                    }
                }
                #endregion
            }
            var ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            wb.Write(fs);
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            ms.AllowClose = false;
            return File(ms, "application/vnd.ms-excel", "LotMaterialOutData.xlsx");
            #endregion
        }


        /// <summary>
        /// 获取查询条件下所有数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetAllData(LotMaterialListOutViewModel model)
        {
            using (LotMaterialListServiceClient client = new LotMaterialListServiceClient())
            {
                MaterialDataParameter p = new MaterialDataParameter()
                {
                    ProductMaterialCode = model.ProductMaterialCode,
                    BomMaterialCode = model.BomMaterialCode,
                    BomMaterialName = model.BomMaterialName,
                    OutPackageNo = model.OutPackageNo,
                    OutStartTime = model.OutStartTime,
                    OutEndTime = model.OutEndTime,
                    PageSize = model.PageSize,
                    PageNo = 0
                };
                MethodReturnResult<DataSet> result = client.GetRPTMaterialData(ref p);
                AllRecords = p.Records;
                if (result.Code == 0 && result.Data != null && result.Data.Tables.Count > 0)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetPageData(LotMaterialListOutViewModel model)
        {
            DataTable dataTable = Resulted.Data.Tables[0];
            DataTable dataTables = new DataTable();
            dataTables = dataTable.Clone();
            for (int i = 0; i < model.PageSize; i++)
            {
                if ((i + (model.PageSize * model.PageNo)) < AllRecords)
                {
                    dataTables.Rows.Add(dataTable.Rows[i + (model.PageSize * model.PageNo)].ItemArray);
                }
            }
            return dataTables;
        }

        /// <summary>
        /// 点击页码获取对应页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PagingQuery(LotMaterialListOutViewModel model)
        {
            if (ModelState.IsValid)
            {
                DataTable result = GetPageData(model);
                if (result != null && result.Rows.Count > 0)
                {
                    ViewBag.ListData = result;
                    ViewBag.PagingConfig = new PagingConfig()
                    {
                        PageNo = model.PageNo,
                        PageSize = model.PageSize,
                        Records = AllRecords
                    };
                }
                else
                {
                    ViewBag.Message = "无数据";
                }
            }
            return PartialView("_ListPartialMaterial", new LotMaterialListOutViewModel());
        }



        public ActionResult Index()
        {
            return Query(new LotMaterialListViewModel());
        }
        public ActionResult IndexWl()
        {
            return QueryWl(new LotMaterialList1ViewModel());
        }
        public ActionResult IndexLotProcessingHistory()
        {
            return QueryLotProcessingHistory(new LotMaterialList1ViewModel());
        }
        public ActionResult Query(LotMaterialListViewModel model)
        {
            using (LotMaterialListServiceClient client = new LotMaterialListServiceClient())
            {
                model.TotalRecords = 0;
                LotMaterialListQueryParameter p = new LotMaterialListQueryParameter()
                {
                    OrderNumber = model.OrderNumber,
                    LocationName = model.LocationName,
                    EndCreateTime = model.EndCreateTime,
                    StartCreateTime = model.StartCreateTime,
                    LotNumber = model.LotNumber,
                    LotNumber1 = model.LotNumber1,
                    PageSize = model.PageSize,
                    PageNo = model.PageNo
                };
                MethodReturnResult<DataSet> result = client.Get(ref p);

                if (result.Code == 0 && result.Data != null && result.Data.Tables.Count > 0)
                {
                    ViewBag.ListData = result.Data.Tables[0];
                    ViewBag.PagingConfig = new PagingConfig()
                    {
                        PageNo = p.PageNo,
                        PageSize = p.PageSize,
                        Records = p.TotalRecords
                    };
                    model.TotalRecords = p.TotalRecords;
                }
                else
                {
                    ViewBag.Message = result.Message;
                }
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
        public ActionResult QueryWl(LotMaterialList1ViewModel model)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                RPTLotMateriallistParameter param = GetQueryConditionP(model);

                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    //RPTLotMateriallistParameter param = new RPTLotMateriallistParameter();
                    //param.LotNumber = model.LotNumber;

                    MethodReturnResult<DataSet> ds = client.GetRPTLotMaterialList(ref param);
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
                return PartialView("_WlListPartial", model);
            }
            else
            {
                return View("IndexWl", model);
            }

            //string strErrorMessage = string.Empty;
            //MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            //try
            //{
            //    using (LotQueryServiceClient client = new LotQueryServiceClient())
            //    {
            //        RPTLotMateriallistParameter param = new RPTLotMateriallistParameter();
            //        param.LotNumber = model.LotNumber;

            //        MethodReturnResult<DataSet> ds = client.GetRPTLotMaterialList(param);
            //        ViewBag.HistoryList = ds;
                    
            //    }
            //}
            //catch (Exception ex)
            //{
            //    result.Code = 1000;
            //    result.Message = ex.Message;
            //    result.Detail = ex.ToString();
            //}

            //if (Request.IsAjaxRequest())
            //{
            //    return PartialView("_WlListPartial", model);
            //}
            //else
            //{
            //    return View("IndexWl", model);
            //}
        }
        public ActionResult QueryLotProcessingHistory(LotMaterialList1ViewModel model)
        {

            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                RPTLotMateriallistParameter param = GetQueryConditionP(model);
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    //RPTLotMateriallistParameter param = new RPTLotMateriallistParameter();
                    
                    //param.LotNumber = model.LotNumber;
                    MethodReturnResult<DataSet> ds = client.GetRPTLotProcessingHistory( ref param);
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
                return PartialView("_LotProcessingHistoryListPartial", model);
            }
            else
            {
                return View("IndexLotProcessingHistory", model);
            }
        }

        public ActionResult ExportToExcel(LotMaterialListViewModel model)
        {
            DataTable dt = new DataTable();
            using (LotMaterialListServiceClient client = new LotMaterialListServiceClient())
            {
                LotMaterialListQueryParameter p = new LotMaterialListQueryParameter()
                {
                    OrderNumber = model.OrderNumber,
                    LocationName = model.LocationName,
                    EndCreateTime = model.EndCreateTime,
                    StartCreateTime = model.StartCreateTime,
                    LotNumber = model.LotNumber,
                    LotNumber1 = model.LotNumber1,
                    PageSize = model.TotalRecords,
                    PageNo = 0
                };
                MethodReturnResult<DataSet> result = client.Get(ref p);

                if (result.Code == 0 && result.Data != null && result.Data.Tables.Count > 0)
                {
                    dt = result.Data.Tables[0];
                }
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
                    ICell cell = null;
                    #region //列名
                    foreach (DataColumn dc in dt.Columns)
                    {
                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue(dc.Caption);
                    }
                    #endregion
                    font.Boldweight = 5;
                }
                IRow rowData = ws.CreateRow(j + 1);
                #region //数据
                ICell cellData = null;
                foreach (DataColumn dc in dt.Columns)
                {
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;

                    if (dc.DataType == typeof(double) || dc.DataType == typeof(float))
                    {
                        cellData.SetCellValue(Convert.ToDouble(dt.Rows[j][dc]));
                    }
                    else if (dc.DataType == typeof(int))
                    {
                        cellData.SetCellValue(Convert.ToInt32(dt.Rows[j][dc]));
                    }
                    else
                    {
                        cellData.SetCellValue(Convert.ToString(dt.Rows[j][dc]));
                    }
                }
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "LotMaterialData.xls");
        }

        public async Task<ActionResult> ExportToExcelWl(LotMaterialListViewModel model)
        {
            DataTable dt = new DataTable();
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            using (LotQueryServiceClient client = new LotQueryServiceClient())
                
            {
                RPTLotMateriallistParameter param = new RPTLotMateriallistParameter();
                param.LotNumber = model.LotNumber;
                param.PageSize = model.PageSize;
                param.PageNo = -1;

                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key.LotNumber,ItemNo",
                        Where = GetQueryCondition(model)
                    };
                 
                    MethodReturnResult<DataSet> ds = client.GetRPTLotMaterialList(ref param);
                    dt = ds.Data.Tables[0];                   
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
                    ICell cell = null;
                    #region //列名
                    foreach (DataColumn dc in dt.Columns)
                    {
                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue(dc.Caption);
                    }
                    #endregion
                    font.Boldweight = 5;
                }
                IRow rowData = ws.CreateRow(j + 1);
                #region //数据
                ICell cellData = null;

                foreach (DataColumn dc in dt.Columns)
                {
                    System.Data.DataRow obj = dt.Rows[j];
                    Manufacturer mf = null;
                    using (SupplierToManufacturerServiceClient clients = new SupplierToManufacturerServiceClient())
                    {
                        PagingConfig cfg0 = new PagingConfig()
                        {
                            Where = string.Format(@"Key.MaterialCode='{0}' AND Key.SupplierCode='{1}'"
                                                    , obj["MATERIAL_CODE"]
                                                    , obj["SUPPLIER_CODE"])
                        };
                        MethodReturnResult<IList<SupplierToManufacturer>> results = clients.Gets(ref cfg0);
                        if (results.Code <= 0 && results.Data.Count > 0)
                        {
                            if (results.Data[0].Key.OrderNumber == "*")
                            {
                                using (ManufacturerServiceClient clientss = new ManufacturerServiceClient())
                                {
                                    MethodReturnResult<Manufacturer> rsts = clientss.Get(results.Data[0].ManufacturerCode);
                                    if (rsts.Data != null)
                                    {
                                        mf = rsts.Data;
                                    }
                                }
                            }
                            else
                            {
                                PagingConfig cfg1 = new PagingConfig()
                                {
                                    Where = string.Format(@"Key.MaterialCode='{0}' AND Key.OrderNumber='{1}' AND Key.SupplierCode='{2}'"
                                                            , obj["MATERIAL_CODE"]
                                                            , obj["ORDER_NUMBER"]
                                                            , obj["SUPPLIER_CODE"])
                                };
                                MethodReturnResult<IList<SupplierToManufacturer>> resultss = clients.Gets(ref cfg1);
                                if (resultss.Code <= 0 && resultss.Data.Count > 0)
                                {
                                    using (ManufacturerServiceClient clientss = new ManufacturerServiceClient())
                                    {
                                        MethodReturnResult<Manufacturer> rsts = clientss.Get(resultss.Data[0].ManufacturerCode);
                                        if (rsts.Data != null)
                                        {
                                            mf = rsts.Data;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;

                    if (dc.DataType == typeof(double) || dc.DataType == typeof(float))
                    {
                        if (dc.ColumnName == "SUPPLIER_CODE")
                        {
                            cellData.SetCellValue(mf == null ? Convert.ToString(dt.Rows[j][dc]) : mf.Key);
                        }
                        else if(dc.ColumnName == "SUPPLIER_NAME") 
                        {
                            cellData.SetCellValue(mf == null ? Convert.ToString(dt.Rows[j][dc]) : mf.Name);
                        }
                        else
                        {
                            cellData.SetCellValue(Convert.ToDouble(dt.Rows[j][dc]));
                        }                      
                    }
                    else if (dc.DataType == typeof(int))
                    {
                        if (dc.ColumnName == "SUPPLIER_CODE" )
                        {
                            cellData.SetCellValue(mf == null ? Convert.ToString(dt.Rows[j][dc]) : mf.Key);
                        }
                        else if(dc.ColumnName == "SUPPLIER_NAME")
                        {
                            cellData.SetCellValue(mf == null ? Convert.ToString(dt.Rows[j][dc]) : mf.Name);
                        }
                        else
                        {
                            cellData.SetCellValue(Convert.ToInt32(dt.Rows[j][dc]));
                        }                        
                    }
                    else
                    {
                        if (dc.ColumnName == "SUPPLIER_CODE" )
                        {
                            cellData.SetCellValue(mf == null ? Convert.ToString(dt.Rows[j][dc]) : mf.Key);
                        }
                        else if(dc.ColumnName == "SUPPLIER_NAME")
                        {
                            cellData.SetCellValue(mf == null ? Convert.ToString(dt.Rows[j][dc]) : mf.Name);
                        }
                        else
                        {
                            cellData.SetCellValue(Convert.ToString(dt.Rows[j][dc]));
                        }
                    }
                }
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "LotMaterialData.xls");
        }

        public async  Task<ActionResult> ExportToExcelLotProcessingHistory(LotMaterialList1ViewModel model)
        {
            DataTable dt = new DataTable();
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                //RPTLotMateriallistParameter param = GetQueryCondition(model);
                RPTLotMateriallistParameter param = new RPTLotMateriallistParameter();
                param.LotNumber = model.LotNumber;
                param.PageSize = model.PageSize;
                param.PageNo = -1;
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key.LotNumber,ItemNo",
                        Where = GetQueryConditionP(model).ToString()
                        //Where = GetQueryConditionP(model).ToString()
                    };

                    MethodReturnResult<DataSet> ds = client.GetRPTLotProcessingHistory(ref param);
                    dt = ds.Data.Tables[0];

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
                    ICell cell = null;
                    #region //列名
                    foreach (DataColumn dc in dt.Columns)
                    {
                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue(dc.Caption);
                    }
                    #endregion
                    font.Boldweight = 5;
                }
                IRow rowData = ws.CreateRow(j + 1);
                #region //数据
                ICell cellData = null;
                foreach (DataColumn dc in dt.Columns)
                {
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;

                    if (dc.DataType == typeof(double) || dc.DataType == typeof(float))
                    {
                        cellData.SetCellValue(Convert.ToDouble(dt.Rows[j][dc]));
                    }
                    else if (dc.DataType == typeof(int))
                    {
                        cellData.SetCellValue(Convert.ToInt32(dt.Rows[j][dc]));
                    }
                    else
                    {
                        cellData.SetCellValue(Convert.ToString(dt.Rows[j][dc]));
                    }
                }
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "LotProcessingHistoryData.xls");
        }

        public string GetQueryCondition(LotMaterialListViewModel model)
        {
            RPTLotMateriallistParameter p = new RPTLotMateriallistParameter()
            {
                LotNumber = model.LotNumber,
                PageNo = model.PageNo,
                PageSize = model.PageSize
            };
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
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

                        foreach (string package in LotNumbers)
                        {
                            where.AppendFormat("'{0}',", package);
                        }
                        where.Remove(where.Length - 1, 1);
                        where.Append(")");
                    }
                }
            }
            return where.ToString();
        }

        public RPTLotMateriallistParameter GetQueryConditionP(LotMaterialList1ViewModel model)
        {
            RPTLotMateriallistParameter p = new RPTLotMateriallistParameter()
            {
                LotNumber = model.LotNumber,
                PageNo = model.PageNo,
                //PageNo = 2,
                PageSize = model.PageSize
            };
            return p;
        }

        #region//颜色数据查询

        public ActionResult IndexColor()
        {
            LotMaterialList1ViewModel model = new LotMaterialList1ViewModel
            {
            };
            return View(model);

        }
        public ActionResult QueryForColor(LotMaterialListViewModel model)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    //RPTLotMateriallistParameter param = new RPTLotMateriallistParameter();
                    //param.LotNumber = model.LotNumber;
                    string lot = model.LotNumber;

                    MethodReturnResult<DataSet> ds = client.GetLotColor(lot);
                    ViewBag.HistoryList = ds;

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
                return PartialView("_ColorListPartial", model);
            }
            else
            {
                return View("IndexColor", model);
            }
        }
        #endregion

        #region 运营物料消耗报表报表

        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <returns>查询主页面</returns>
        public ActionResult IndexMaterialConsume()
        {
            LotMaterialListViewModel model = new LotMaterialListViewModel
            {
                ////初始化参数
                ////ReportCode = "DAY01",       //报表代码
                StartTime = System.DateTime.Now.AddDays(-7).Date,
                EndTime = System.DateTime.Now.AddDays(0).Date,
            };
            return View(model);


        }

        /// <summary>
        /// 根据查询条件查询报表数据
        /// </summary>
        /// <param name="model">参数</param>
        /// <returns>返回结果集</returns>
        public ActionResult QueryMaterialConsume(LotMaterialList1ViewModel model)
        {

            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (LotMaterialListServiceClient client = new LotMaterialListServiceClient())
                {
                    RPTDailyDataGetParameter param = new RPTDailyDataGetParameter();
                    param.StartDate = model.StartTime.ToString();
                    param.EndDate = model.EndTime.ToString();
                    param.LocationName = model.LocationName;
                    param.LineCode = model.LineCode;
                    param.OrderNumber = model.OrderNumber;

                    MethodReturnResult<DataSet> ds = client.GetMaterialConsume(param);
                    ViewBag.HistoryList = ds;

                    if (ds.Code > 0)       //产生错误
                    {
                        result.Code = ds.Code;
                        result.Message = ds.Message;
                        result.Detail = ds.Detail;

                        return Json(result);
                    }
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
                return PartialView("_MaterialConsumeListPartial", model);
            }
            else
            {
                return View("IndexMaterialConsume", model);
            }
        }
        /// <summary>
        /// 导出到excel
        /// </summary>
        /// <param name="model">参数</param>
        /// <returns>导出数据到excel</returns>
        public ActionResult ExportToExcelMaterialConsume(LotMaterialListViewModel model)
        {
            DataTable dt = new DataTable();
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            using (LotMaterialListServiceClient client = new LotMaterialListServiceClient())
            {
                RPTDailyDataGetParameter param = new RPTDailyDataGetParameter();
                param.StartDate = model.StartTime.ToString();
                param.EndDate = model.EndTime.ToString();
                param.LocationName = model.LocationName;
                param.LineCode = model.LineCode;
                param.OrderNumber = model.OrderNumber;

                MethodReturnResult<DataSet> ds = client.GetMaterialConsume(param);
                dt = ds.Data.Tables[0];

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
                    ICell cell = null;
                    #region //列名
                    foreach (DataColumn dc in dt.Columns)
                    {
                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue(dc.Caption);
                    }
                    #endregion
                    font.Boldweight = 5;
                }
                IRow rowData = ws.CreateRow(j + 1);
                #region //数据
                ICell cellData = null;
                foreach (DataColumn dc in dt.Columns)
                {
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;

                    if (dc.DataType == typeof(double) || dc.DataType == typeof(float))
                    {
                        cellData.SetCellValue(Convert.ToDouble(dt.Rows[j][dc]));
                    }
                    else if (dc.DataType == typeof(int))
                    {
                        cellData.SetCellValue(Convert.ToInt32(dt.Rows[j][dc]));
                    }
                    else
                    {
                        cellData.SetCellValue(Convert.ToString(dt.Rows[j][dc]));
                    }
                }
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "LotMaterialData.xls");
        }

        //public string GetQueryCondition(LotMaterialListViewModel model)
        //{
        //    RPTLotMateriallistParameter p = new RPTLotMateriallistParameter()
        //    {
        //        LotNumber = model.LotNumber,
        //        PageNo = model.PageNo,
        //        PageSize = model.PageSize
        //    };
        //    StringBuilder where = new StringBuilder();
        //    if (model != null)
        //    {
        //        if (!string.IsNullOrEmpty(model.LotNumber))
        //        {
        //            char[] splitChars = new char[] { ',', '$' };
        //            string[] LotNumbers = model.LotNumber.TrimEnd(splitChars).Split(splitChars);
        //            if (LotNumbers.Length <= 1)
        //            {
        //                where.AppendFormat(" {0} Key.LotNumber = '{1}'"
        //                                    , where.Length > 0 ? "AND" : string.Empty
        //                                    , LotNumbers[0]);
        //            }
        //            else
        //            {
        //                where.AppendFormat(" {0} Key.LotNumber IN ("
        //                                    , where.Length > 0 ? "AND" : string.Empty);

        //                foreach (string package in LotNumbers)
        //                {
        //                    where.AppendFormat("'{0}',", package);
        //                }
        //                where.Remove(where.Length - 1, 1);
        //                where.Append(")");
        //            }
        //        }
        //    }
        //    return where.ToString();
        //}
        //public RPTLotMateriallistParameter GetQueryConditionP(LotMaterialList1ViewModel model)
        //{
        //    RPTLotMateriallistParameter p = new RPTLotMateriallistParameter()
        //    {
        //        LotNumber = model.LotNumber,
        //        PageNo = model.PageNo,
        //        //PageNo = 2,
        //        PageSize = model.PageSize
        //    };
        //    return p;
        //}


        #endregion
    }
}