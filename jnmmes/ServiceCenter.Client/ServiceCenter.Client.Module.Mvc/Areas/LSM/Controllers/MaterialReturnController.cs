using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.LSM.Models;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.Model;
using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LSMResources = ServiceCenter.Client.Mvc.Resources.LSM;
using ServiceCenter.MES.Service.Contract.LSM;
using ServiceCenter.Client.Mvc.Areas.ERP.Models;
using ServiceCenter.MES.Model.ERP;
using ServiceCenter.MES.Service.Client.ERP;
using ServiceCenter.Client.Mvc.Resources.ERP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using System.Xml;
using System.Net;
using ServiceCenter.MES.Service.Contract.ERP;
using ServiceCenter.Client.Mvc.RDLC;
using Microsoft.Reporting.WebForms;
using ServiceCenter.Common;
using System.Collections;
using System.Configuration;



namespace ServiceCenter.Client.Mvc.Areas.LSM.Controllers
{
    public class MaterialReturnController : Controller
    {
        //
        // GET: /LSM/MaterialReturn/
        public async Task<ActionResult> Index()
        {
            return await Query(new MaterialReturnQueryViewModel());
        }
        //
        //POST: /LSM/MaterialReturn/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialReturnQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.ReturnNo))
                            {
                                where.AppendFormat(" {0} Key = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ReturnNo);
                            }
                            if (!string.IsNullOrEmpty(model.OrderNumber))
                            {
                                where.AppendFormat(" {0} OrderNumber = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);
                            }

                            if (model.StartReturnDate != null)
                            {
                                where.AppendFormat(" {0} ReturnDate >= '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.StartReturnDate);
                            }

                            if (model.EndReturnDate != null)
                            {
                                where.AppendFormat(" {0} ReturnDate <= '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EndReturnDate);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc",
                            Where = where.ToString()
                        };

                        MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);

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
                return PartialView("_ListPartial");
            }
            else
            {
                return View("Index");
            }
        }
        //
        //POST: /LSM/MaterialReturn/PagingQuery
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

                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
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
                        MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial");
        }
        //
        // GET: /LSM/MaterialReturn/
        public async Task<ActionResult> Detail(MaterialReturnDetailQueryViewModel model)
        {
            return await DetailQuery(model);
        }
        //
        //POST: /LSM/MaterialReturn/DetailQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailQuery(MaterialReturnDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc,Key.ReturnNo,Key.ItemNo",
                            Where = GetWhereCondition(model)
                        };
                        MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);

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
                return PartialView("_DetailListPartial", new MaterialReturnDetailViewModel());
            }
            else
            {
                return View("Detail", model);
            }
        }

        public string GetWhereCondition(MaterialReturnDetailQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.ReturnNo))
                {
                    where.AppendFormat(" {0} Key.ReturnNo = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ReturnNo);
                }

                if (!string.IsNullOrEmpty(model.LineStoreName))
                {
                    where.AppendFormat(" {0} LineStoreName = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LineStoreName);
                }

                if (!string.IsNullOrEmpty(model.MaterialCode))
                {
                    where.AppendFormat(" {0} MaterialCode LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialCode);
                }

                if (!string.IsNullOrEmpty(model.MaterialLot))
                {
                    where.AppendFormat(" {0} MaterialLot LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialLot);
                }

                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    where.AppendFormat(@" {0} EXISTS(FROM MaterialReturn as p
                                                    WHERE p.Key=self.Key.ReturnNo
                                                    AND p.OrderNumber = '{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber);
                }

//                if (!IsNullOrEmpty(model.ReturnDate))
//                {
//                    where.AppendFormat(@" {0} EXISTS(FROM MaterialReturn as p
//                                                    WHERE p.Key=self.Key.ReturnNo
//                                                    AND p.ReturnDate = '{1}')"
//                                        , where.Length > 0 ? "AND" : string.Empty
//                                        , model.ReturnDate);
//                }
            }
            return where.ToString();
        }

        //
        //POST: /LSM/MaterialReturn/DetailPagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailPagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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

                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
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
                        MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_DetailListPartial",new MaterialReturnDetailQueryViewModel());
        }

        //
        //POST: /WIP/MaterialReturn/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(MaterialReturnDetailQueryViewModel model)
        {
            IList<MaterialReturnDetail> lst = new List<MaterialReturnDetail>();
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "CreateTime Desc,Key.ReturnNo,Key.ItemNo",
                        Where = GetWhereCondition(model)
                    };
                    MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        lst = result.Data;
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
            ICell cell = null;
            IRow row = null;
            ISheet ws = null;
            for (int j = 0; j < lst.Count; j++)
            {
                if (j % 65535 == 0)
                {
                    ws = wb.CreateSheet();
                    row = ws.CreateRow(0);
                    #region //列名
                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_ReturnNo);  //退料号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_OrderNumber);  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_ReturnDate);  //领料日期

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_ItemNo);  //项目号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_LineStoreName);  //线别仓

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_MaterialCode);  //物料编码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("物料名称");  //物料名称

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_MaterialLot);  //物料批号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_Qty);  //数量


                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("描述");  //描述

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑人");  //编辑人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑时间");  //编辑时间
                    #endregion
                    font.Boldweight = 5;
                }

                MaterialReturnDetail obj = lst[j];
                MaterialReturn mrObj = model.GetMaterialReturn(obj.Key.ReturnNo);
                Material m = model.GetMaterial(obj.MaterialCode);
                row = ws.CreateRow(j + 1);

                #region //数据
                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ReturnNo);  //领料号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(mrObj == null ? string.Empty : mrObj.OrderNumber);  //工单号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(mrObj == null ? string.Empty : string.Format("{0:yyyy-MM-dd}", mrObj.ReturnDate));  //领料日期

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ItemNo);  //项目号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.LineStoreName);  //线别仓

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.MaterialCode);  //物料编码

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(m == null ? string.Empty : m.Name);  //物料名称

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.MaterialLot);  //物料批号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Qty);  //数量


                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Description);  //描述

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Editor);  //编辑人

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.EditTime));  //编辑时间
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "MaterialReturnData.xls");
        }

        //
        // POST: /PPM/MaterialReturn/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialReturnViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try
            {
                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
                {
                    MaterialReturn obj = new MaterialReturn()
                    {
                        Key = model.ReturnNo.ToUpper(),
                        OrderNumber = model.OrderNumber.ToUpper(),
                        ReturnDate = model.ReturnDate,
                        Description = model.Description,
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name
                    };

                    char splitChar = ',';
                    var ItemNos = Request["ItemNo"].Split(splitChar);
                    var LineStoreNames = Request["LineStoreName"].Split(splitChar);
                    var MaterialCodes = Request["MaterialCode"].Split(splitChar);
                    var MaterialLots = Request["MaterialLot"].Split(splitChar);
                    var Qtys = Request["Qty"].Split(splitChar);
                    var Descriptions = Request["DetailDescription"].Split(splitChar);

                    List<MaterialReturnDetail> lst = new List<MaterialReturnDetail>();
                    for (int i = 0; i < ItemNos.Length; i++)
                    {
                        lst.Add(new MaterialReturnDetail()
                        {
                            Key = new MaterialReturnDetailKey()
                            {
                                ReturnNo = model.ReturnNo,
                                ItemNo = i + 1
                            },
                            LineStoreName = LineStoreNames[i].ToUpper(),
                            MaterialCode = MaterialCodes[i].ToUpper(),
                            MaterialLot = MaterialLots[i].ToUpper(),
                            Qty = Convert.ToDouble(Qtys[i]),
                            Description = Descriptions[i],
                            Editor = User.Identity.Name,
                            Creator = User.Identity.Name
                        });
                        if (Convert.ToDouble(Qtys[i]) == 0)
                        {
                            rst.Code = 1008;
                            rst.Message = string.Format("物料批号({0})退料数量不能为0。"
                                                            , MaterialLots[i].ToUpper());
                            return Json(rst);
                        } 
                    }

                    rst = await client.AddAsync(obj, lst);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(LSMResources.StringResource.MaterialReturn_Save_Success
                                                    , obj.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                rst.Code = 1000;
                rst.Message = ex.Message;
                rst.Detail = ex.ToString();
            }
            return Json(rst);
        }

        public ActionResult GetOrderNumber(string q)
        {
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key LIKE '{0}%' AND CloseType='{1}'"
                                            , q
                                            , Convert.ToInt32(EnumWorkOrderCloseType.None))
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return Json(from item in result.Data
                                select new
                                {
                                    @label = item.Key,
                                    @value = item.Key
                                }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLineStoreNames(string orderNumber)
        {
            string locationName = string.Empty;
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = client.Get(orderNumber);
                if (result.Code <= 0 && result.Data != null)
                {
                    locationName = result.Data.LocationName;
                }
            }

            IList<LineStore> lstLineStore = new List<LineStore>();
            using (LineStoreServiceClient client = new LineStoreServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("LocationName='{0}' AND Type='{1}'", locationName, Convert.ToInt32(EnumLineStoreType.Material))
                };

                MethodReturnResult<IList<LineStore>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstLineStore = result.Data;
                }
            }

            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(User.Identity.Name, ResourceType.LineStore);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            var lnq = from item in lstLineStore
                      where lstResource.Any(m => m.Data == item.Key)
                      select new
                      {
                          Key = item.Key
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialCode(string q, string orderNumber, string lineStoreName)
        {
            string routeOperationName = string.Empty;
            using (LineStoreServiceClient client = new LineStoreServiceClient())
            {
                MethodReturnResult<LineStore> result = client.Get(lineStoreName);
                if (result.Code <= 0 && result.Data != null)
                {
                    routeOperationName = result.Data.RouteOperationName;
                }
            }
            //根据工单获取物料编码。
            IList<WorkOrderBOM> lstBOM = new List<WorkOrderBOM>();
            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"MaterialCode LIKE '{0}%' AND Key.OrderNumber='{1}'"
                                            , q
                                            , orderNumber),
                    OrderBy = "Key.ItemNo"
                };
                //工作中心为空的可以领到任何线边仓。
                //线边仓对应工序为空的可以领任何料。
                if (!string.IsNullOrEmpty(routeOperationName))
                {
                    cfg.Where += string.Format(" AND (WorkCenter='' OR WorkCenter IS NULL Or WorkCenter='{0}')", routeOperationName);
                }

                MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstBOM = result.Data;
                }
            }
            //获取线边仓中已有物料明细数据。
            IList<LineStoreMaterialDetail> lstDetail=new List<LineStoreMaterialDetail>();
            using(LineStoreMaterialServiceClient client=new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.MaterialCode LIKE '{0}%' AND Key.LineStoreName='{1}' AND CurrentQty>0"
                                            , q
                                            , lineStoreName)
                };
                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            return Json(from item in lstBOM
                        where lstDetail.Any(m=>m.Key.MaterialCode==item.MaterialCode)
                        select new
                        {
                            @label = string.Format("{0}[{1}]", item.MaterialCode, item.Description),
                            @value = item.MaterialCode,
                            @desc = item.Description
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialLot(string materialLot, string materialCode, string orderNumber, string lineStoreName)
        {
            IList<LineStoreMaterialDetail> lstDetail = new List<LineStoreMaterialDetail>();
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging=false,
                    Where = string.Format(@"Key.LineStoreName='{0}'
                                            AND Key.MaterialCode='{1}'
                                            AND Key.OrderNumber='{3}'
                                            AND Key.MaterialLot LIKE '{2}%'
                                            AND CurrentQty>0"
                                            , lineStoreName
                                            , materialCode
                                            , materialLot
                                            , orderNumber),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            return Json(from item in lstDetail
                        select new
                        {
                            @label = string.Format("{0}[{1}]",item.Key.MaterialLot,item.Key.MaterialCode),
                            @value = item.Key.MaterialLot,
                            @qty=item.CurrentQty
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReturnNo()
        {
            string prefix = string.Format("TMK{0:yyMM}", DateTime.Now);
            int itemNo = 0;
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key LIKE '{0}%'", prefix),
                    OrderBy = "Key Desc"
                };
                MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    string sItemNo = result.Data[0].Key.Replace(prefix, "");
                    int.TryParse(sItemNo, out itemNo);
                }
            }
            return Json(prefix + (itemNo + 1).ToString("0000"), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据属性名称查找字符串中设置的属性值
        /// </summary>
        /// <param name="valueString">属性字符串</param>
        /// <param name="valueName">属性名称</param>
        /// <param name="operators">赋值操作符号</param>
        /// <param name="terminator">终止符</param>
        /// <returns></returns>
        public string GetValueDataByString(string valueString, string valueName, string operators, string terminator)
        {
            string valueData = "";
            int ifind = 0;
            int iEnd = 0;

            ifind = valueString.IndexOf(valueName + operators);

            if (ifind > 0)
            {
                ifind = ifind + valueName.Length + operators.Length;

                iEnd = valueString.IndexOf(terminator, ifind);

                if (iEnd >= ifind)
                {
                    valueData = valueString.Substring(ifind, iEnd - ifind);
                }
                else
                {
                    valueData = valueString.Substring(ifind, valueString.Length - ifind);
                }
            }

            return valueData;
        }


//        #region 退料到ERP

//        public async Task<ActionResult> IndexForERP()
//        {
//            return await QueryForERP(new MaterialReturnQueryViewModel());
//        }
//        //
//        //POST: /LSM/MaterialReturn/Query
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> QueryForERP(MaterialReturnQueryViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//                {
//                    await Task.Run(() =>
//                    {
//                        StringBuilder where = new StringBuilder();
//                        if (model != null)
//                        {
//                            if (!string.IsNullOrEmpty(model.ReturnNo))
//                            {
//                                where.AppendFormat(" {0} Key = '{1}'"
//                                                    , where.Length > 0 ? "AND" : string.Empty
//                                                    , model.ReturnNo);
//                            }
//                            if (!string.IsNullOrEmpty(model.OrderNumber))
//                            {
//                                where.AppendFormat(" {0} OrderNumber = '{1}'"
//                                                    , where.Length > 0 ? "AND" : string.Empty
//                                                    , model.OrderNumber);
//                            }

//                            if (model.StartReturnDate != null)
//                            {
//                                where.AppendFormat(" {0} ReturnDate >= '{1}'"
//                                                    , where.Length > 0 ? "AND" : string.Empty
//                                                    , model.StartReturnDate);
//                            }

//                            if (model.EndReturnDate != null)
//                            {
//                                where.AppendFormat(" {0} ReturnDate <= '{1}'"
//                                                    , where.Length > 0 ? "AND" : string.Empty
//                                                    , model.EndReturnDate);
//                            }
//                        }

//                        PagingConfig cfg = new PagingConfig()
//                        {
//                            OrderBy = "CreateTime Desc",
//                            Where = where.ToString()
//                        };

//                        MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);

//                        if (result.Code == 0)
//                        {
//                            ViewBag.PagingConfig = cfg;
//                            ViewBag.List = result.Data;
//                        }
//                    });
//                }
//            }
//            if (Request.IsAjaxRequest())
//            {
//                return PartialView("_ListPartialForERP");
//            }
//            else
//            {
//                return View("IndexForERP");
//            }
//        }
//        //
//        //POST: /LSM/MaterialReturn/PagingQuery
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> PagingQueryForERP(string where, string orderBy, int? currentPageNo, int? currentPageSize)
//        {
//            if (ModelState.IsValid)
//            {
//                int pageNo = currentPageNo ?? 0;
//                int pageSize = currentPageSize ?? 20;
//                if (Request["PageNo"] != null)
//                {
//                    pageNo = Convert.ToInt32(Request["PageNo"]);
//                }
//                if (Request["PageSize"] != null)
//                {
//                    pageSize = Convert.ToInt32(Request["PageSize"]);
//                }

//                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//                {
//                    await Task.Run(() =>
//                    {
//                        PagingConfig cfg = new PagingConfig()
//                        {
//                            PageNo = pageNo,
//                            PageSize = pageSize,
//                            Where = where ?? string.Empty,
//                            OrderBy = orderBy ?? string.Empty
//                        };
//                        MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);
//                        if (result.Code == 0)
//                        {
//                            ViewBag.PagingConfig = cfg;
//                            ViewBag.List = result.Data;
//                        }
//                    });
//                }
//            }
//            return PartialView("_ListPartialForERP");
//        }
//        //
//        // GET: /LSM/MaterialReturn/
//        public async Task<ActionResult> DetailForERP(MaterialReturnDetailQueryViewModel model)
//        {
//            return await DetailQueryForERP(model);
//        }
//        //
//        //POST: /LSM/MaterialReturn/DetailQuery
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> DetailQueryForERP(MaterialReturnDetailQueryViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//                {
//                    await Task.Run(() =>
//                    {
//                        PagingConfig cfg = new PagingConfig()
//                        {
//                            OrderBy = "CreateTime Desc,Key.ReturnNo,Key.ItemNo",
//                            Where = GetWhereCondition(model)
//                        };
//                        MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);

//                        if (result.Code == 0)
//                        {
//                            ViewBag.PagingConfig = cfg;
//                            ViewBag.List = result.Data;
//                        }
//                    });
//                }

//                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//                {
//                    List<SelectListItem> StoreList = new List<SelectListItem>();
//                    MethodReturnResult<DataSet> ds = client.GetStore();
//                    if (ds.Data.Tables[0].Rows.Count > 0)
//                    {
//                        for (int i = 0; i < ds.Data.Tables[0].Rows.Count; i++)
//                        {
//                            StoreList.Add(new SelectListItem() { Text = ds.Data.Tables[0].Rows[i]["STORNAME"].ToString(), Value = ds.Data.Tables[0].Rows[i]["STORCODE"].ToString() });
//                        }
//                    }
//                //StoreList.Add(new SelectListItem() { Text = "废料仓", Value = "FP001" });
//                    ViewBag.Store = StoreList;
//                }

//            }
//            if (Request.IsAjaxRequest())
//            {
//                return PartialView("_DetailListPartialForERP", new MaterialReturnDetailViewModel());
//            }
//            else
//            {
//                return View("DetailForERP", model);
//            }
//        }

//        public string GetWhereConditionForERP(MaterialReturnDetailQueryViewModel model)
//        {
//            StringBuilder where = new StringBuilder();
//            if (model != null)
//            {
//                if (!string.IsNullOrEmpty(model.ReturnNo))
//                {
//                    where.AppendFormat(" {0} Key.ReturnNo = '{1}'"
//                                        , where.Length > 0 ? "AND" : string.Empty
//                                        , model.ReturnNo);
//                }

//                if (!string.IsNullOrEmpty(model.LineStoreName))
//                {
//                    where.AppendFormat(" {0} LineStoreName = '{1}'"
//                                        , where.Length > 0 ? "AND" : string.Empty
//                                        , model.LineStoreName);
//                }

//                if (!string.IsNullOrEmpty(model.MaterialCode))
//                {
//                    where.AppendFormat(" {0} MaterialCode LIKE '{1}%'"
//                                        , where.Length > 0 ? "AND" : string.Empty
//                                        , model.MaterialCode);
//                }

//                if (!string.IsNullOrEmpty(model.MaterialLot))
//                {
//                    where.AppendFormat(" {0} MaterialLot LIKE '{1}%'"
//                                        , where.Length > 0 ? "AND" : string.Empty
//                                        , model.MaterialLot);
//                }

//                if (!string.IsNullOrEmpty(model.OrderNumber))
//                {
//                    where.AppendFormat(@" {0} EXISTS(FROM MaterialReturn as p
//                                                    WHERE p.Key=self.Key.ReturnNo
//                                                    AND p.OrderNumber = '{1}')"
//                                        , where.Length > 0 ? "AND" : string.Empty
//                                        , model.OrderNumber);
//                }

//                if (!string.IsNullOrEmpty(model.ReturnDate))
//                {
//                    where.AppendFormat(@" {0} EXISTS(FROM MaterialReturn as p
//                                                    WHERE p.Key=self.Key.ReturnNo
//                                                    AND p.ReturnDate = '{1}')"
//                                        , where.Length > 0 ? "AND" : string.Empty
//                                        , model.ReturnDate);
//                }
//            }
//            return where.ToString();
//        }

//        //
//        //POST: /LSM/MaterialReturn/DetailPagingQuery
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> DetailPagingQueryForERP(string where, string orderBy, int? currentPageNo, int? currentPageSize)
//        {
//            if (ModelState.IsValid)
//            {
//                int pageNo = currentPageNo ?? 0;
//                int pageSize = currentPageSize ?? 20;
//                if (Request["PageNo"] != null)
//                {
//                    pageNo = Convert.ToInt32(Request["PageNo"]);
//                }
//                if (Request["PageSize"] != null)
//                {
//                    pageSize = Convert.ToInt32(Request["PageSize"]);
//                }

//                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//                {
//                    await Task.Run(() =>
//                    {
//                        PagingConfig cfg = new PagingConfig()
//                        {
//                            PageNo = pageNo,
//                            PageSize = pageSize,
//                            Where = where ?? string.Empty,
//                            OrderBy = orderBy ?? string.Empty
//                        };
//                        MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);
//                        if (result.Code == 0)
//                        {
//                            ViewBag.PagingConfig = cfg;
//                            ViewBag.List = result.Data;
//                        }
//                    });
//                }
//            }
//            return PartialView("_DetailListPartialForERP", new MaterialReturnDetailQueryViewModel());
//        }


//        //
//        //POST: /WIP/MaterialReturn/ExportToExcel
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ExportToExcelForERP(MaterialReturnDetailQueryViewModel model)
//        {
//            IList<MaterialReturnDetail> lst = new List<MaterialReturnDetail>();
//            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//            {
//                await Task.Run(() =>
//                {
//                    PagingConfig cfg = new PagingConfig()
//                    {
//                        IsPaging = false,
//                        OrderBy = "CreateTime Desc,Key.ReturnNo,Key.ItemNo",
//                        Where = GetWhereCondition(model)
//                    };
//                    MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);

//                    if (result.Code == 0)
//                    {
//                        lst = result.Data;
//                    }
//                });
//            }
//            //创建工作薄。
//            IWorkbook wb = new HSSFWorkbook();
//            //设置EXCEL格式
//            ICellStyle style = wb.CreateCellStyle();
//            style.FillForegroundColor = 10;
//            //有边框
//            style.BorderBottom = BorderStyle.Thin;
//            style.BorderLeft = BorderStyle.Thin;
//            style.BorderRight = BorderStyle.Thin;
//            style.BorderTop = BorderStyle.Thin;
//            IFont font = wb.CreateFont();
//            font.Boldweight = 10;
//            style.SetFont(font);
//            ICell cell = null;
//            IRow row = null;
//            ISheet ws = null;
//            for (int j = 0; j < lst.Count; j++)
//            {
//                if (j % 65535 == 0)
//                {
//                    ws = wb.CreateSheet();
//                    row = ws.CreateRow(0);
//                    #region //列名
//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_ReturnNo);  //退料号

//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_OrderNumber);  //工单号

//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_ReturnDate);  //领料日期

//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_ItemNo);  //项目号

//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_LineStoreName);  //线别仓

//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_MaterialCode);  //物料编码

//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue("物料名称");  //物料名称

//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_MaterialLot);  //物料批号

//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_Qty);  //数量


//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue("描述");  //描述

//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue("编辑人");  //编辑人

//                    cell = row.CreateCell(row.Cells.Count);
//                    cell.CellStyle = style;
//                    cell.SetCellValue("编辑时间");  //编辑时间
//                    #endregion
//                    font.Boldweight = 5;
//                }

//                MaterialReturnDetail obj = lst[j];
//                MaterialReturn mrObj = model.GetMaterialReturn(obj.Key.ReturnNo);
//                Material m = model.GetMaterial(obj.MaterialCode);
//                row = ws.CreateRow(j + 1);

//                #region //数据
//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(obj.Key.ReturnNo);  //领料号

//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(mrObj == null ? string.Empty : mrObj.OrderNumber);  //工单号

//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(mrObj == null ? string.Empty : string.Format("{0:yyyy-MM-dd}", mrObj.ReturnDate));  //领料日期

//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(obj.Key.ItemNo);  //项目号

//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(obj.LineStoreName);  //线别仓

//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(obj.MaterialCode);  //物料编码

//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(m == null ? string.Empty : m.Name);  //物料名称

//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(obj.MaterialLot);  //物料批号

//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(obj.Qty);  //数量


//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(obj.Description);  //描述

//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(obj.Editor);  //编辑人

//                cell = row.CreateCell(row.Cells.Count);
//                cell.CellStyle = style;
//                cell.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.EditTime));  //编辑时间
//                #endregion
//            }

//            MemoryStream ms = new MemoryStream();
//            wb.Write(ms);
//            ms.Flush();
//            ms.Position = 0;
//            return File(ms, "application/vnd.ms-excel", "MaterialReturnData.xls");
//        }

//        //
//        // POST: /PPM/MaterialReturn/Save
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> SaveForERP(MaterialReturnViewModel model)
//        {
//            MethodReturnResult rst = new MethodReturnResult();
//            try
//            {
//                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//                {
//                    MaterialReturn obj = new MaterialReturn()
//                    {
//                        Key = model.ReturnNo.ToUpper(),
//                        OrderNumber = model.OrderNumber.ToUpper(),
//                        ReturnDate = model.ReturnDate,
//                        Description = model.Description,
//                        Editor = User.Identity.Name,
//                        Creator = User.Identity.Name
//                    };

//                    char splitChar = ',';
//                    var ItemNos = Request["ItemNo"].Split(splitChar);
//                    var LineStoreNames = Request["LineStoreName"].Split(splitChar);
//                    var MaterialCodes = Request["MaterialCode"].Split(splitChar);
//                    var MaterialLots = Request["MaterialLot"].Split(splitChar);
//                    var Qtys = Request["Qty"].Split(splitChar);
//                    var Descriptions = Request["DetailDescription"].Split(splitChar);

//                    List<MaterialReturnDetail> lst = new List<MaterialReturnDetail>();
//                    for (int i = 0; i < ItemNos.Length; i++)
//                    {
//                        lst.Add(new MaterialReturnDetail()
//                        {
//                            Key = new MaterialReturnDetailKey()
//                            {
//                                ReturnNo = model.ReturnNo,
//                                ItemNo = i + 1
//                            },
//                            LineStoreName = LineStoreNames[i].ToUpper(),
//                            MaterialCode = MaterialCodes[i].ToUpper(),
//                            MaterialLot = MaterialLots[i].ToUpper(),
//                            Qty = Convert.ToDouble(Qtys[i]),
//                            //SupplierCode = SupplierCodes[i].ToUpper(),
//                            Description = Descriptions[i],
//                            Editor = User.Identity.Name,
//                            Creator = User.Identity.Name
//                        });
//                    }

//                    rst = await client.AddAsync(obj, lst);
//                    if (rst.Code == 0)
//                    {
//                        rst.Message = string.Format(LSMResources.StringResource.MaterialReturn_Save_Success
//                                                    , obj.Key);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                rst.Code = 1000;
//                rst.Message = ex.Message;
//                rst.Detail = ex.ToString();
//            }
//            return Json(rst);
//        }

//        //public ActionResult GetOrderNumberForERP(string q)
//        //{
//        //    using (WorkOrderServiceClient client = new WorkOrderServiceClient())
//        //    {
//        //        PagingConfig cfg = new PagingConfig()
//        //        {
//        //            IsPaging = false,
//        //            Where = string.Format("Key LIKE '{0}%' AND CloseType='{1}'"
//        //                                    , q
//        //                                    , Convert.ToInt32(EnumWorkOrderCloseType.None))
//        //        };

//        //        MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
//        //        if (result.Code <= 0)
//        //        {
//        //            return Json(from item in result.Data
//        //                        select new
//        //                        {
//        //                            @label = item.Key,
//        //                            @value = item.Key
//        //                        }, JsonRequestBehavior.AllowGet);
//        //        }
//        //    }
//        //    return Json(null, JsonRequestBehavior.AllowGet);
//        //}

//        //public ActionResult GetLineStoreNamesForERP(string orderNumber)
//        //{
//        //    string locationName = string.Empty;
//        //    using (WorkOrderServiceClient client = new WorkOrderServiceClient())
//        //    {
//        //        MethodReturnResult<WorkOrder> result = client.Get(orderNumber);
//        //        if (result.Code <= 0 && result.Data != null)
//        //        {
//        //            locationName = result.Data.LocationName;
//        //        }
//        //    }

//        //    IList<LineStore> lstLineStore = new List<LineStore>();
//        //    using (LineStoreServiceClient client = new LineStoreServiceClient())
//        //    {
//        //        PagingConfig cfg = new PagingConfig()
//        //        {
//        //            IsPaging = false,
//        //            Where = string.Format("LocationName='{0}' AND Type='{1}'", locationName, Convert.ToInt32(EnumLineStoreType.Material))
//        //        };

//        //        MethodReturnResult<IList<LineStore>> result = client.Get(ref cfg);
//        //        if (result.Code <= 0 && result.Data != null)
//        //        {
//        //            lstLineStore = result.Data;
//        //        }
//        //    }

//        //    IList<Resource> lstResource = new List<Resource>();
//        //    using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
//        //    {
//        //        MethodReturnResult<IList<Resource>> result = client.GetResourceList(User.Identity.Name, ResourceType.LineStore);
//        //        if (result.Code <= 0 && result.Data != null)
//        //        {
//        //            lstResource = result.Data;
//        //        }
//        //    }

//        //    var lnq = from item in lstLineStore
//        //              where lstResource.Any(m => m.Data == item.Key)
//        //              select new
//        //              {
//        //                  Key = item.Key
//        //              };
//        //    return Json(lnq, JsonRequestBehavior.AllowGet);
//        //}

//        public ActionResult GetMaterialCodeForERP(string q, string orderNumber, string lineStoreName)
//        {
//            string routeOperationName = string.Empty;
//            using (LineStoreServiceClient client = new LineStoreServiceClient())
//            {
//                MethodReturnResult<LineStore> result = client.Get(lineStoreName);
//                if (result.Code <= 0 && result.Data != null)
//                {
//                    routeOperationName = result.Data.RouteOperationName;
//                }
//            }
//            //根据工单获取物料编码。
//            IList<WorkOrderBOM> lstBOM = new List<WorkOrderBOM>();
//            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"MaterialCode LIKE '{0}%' AND Key.OrderNumber='{1}'"
//                                            , q
//                                            , orderNumber),
//                    OrderBy = "Key.ItemNo"
//                };
//                //工作中心为空的可以领到任何线边仓。
//                //线边仓对应工序为空的可以领任何料。
//                if (!string.IsNullOrEmpty(routeOperationName))
//                {
//                    cfg.Where += string.Format(" AND (WorkCenter='' OR WorkCenter IS NULL Or WorkCenter='{0}')", routeOperationName);
//                }

//                MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);
//                if (result.Code <= 0 && result.Data != null)
//                {
//                    lstBOM = result.Data;
//                }
//            }
//            //获取线边仓中已有物料明细数据。
//            IList<LineStoreMaterialDetail> lstDetail = new List<LineStoreMaterialDetail>();
//            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"Key.MaterialCode LIKE '{0}%' AND Key.LineStoreName='{1}' AND CurrentQty>0"
//                                            , q
//                                            , lineStoreName)
//                };
//                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
//                if (result.Code <= 0 && result.Data != null)
//                {
//                    lstDetail = result.Data;
//                }
//            }

//            return Json(from item in lstBOM
//                        where lstDetail.Any(m => m.Key.MaterialCode == item.MaterialCode)
//                        select new
//                        {
//                            @label = string.Format("{0}[{1}]", item.MaterialCode, item.Description),
//                            @value = item.MaterialCode,
//                            @desc = item.Description
//                        }, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetMaterialLotForERP(string materialLot, string materialCode, string orderNumber, string lineStoreName)
//        {
//            IList<LineStoreMaterialDetail> lstDetail = new List<LineStoreMaterialDetail>();
//            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"Key.LineStoreName='{0}'
//                                            AND Key.MaterialCode='{1}'
//                                            AND Key.OrderNumber='{3}'
//                                            AND Key.MaterialLot LIKE '{2}%'
//                                            AND CurrentQty>0"
//                                            , lineStoreName
//                                            , materialCode
//                                            , materialLot
//                                            , orderNumber),
//                    OrderBy = "Key"
//                };

//                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
//                if (result.Code <= 0 && result.Data != null)
//                {
//                    lstDetail = result.Data;
//                }
//            }

//            return Json(from item in lstDetail
//                        select new
//                        {
//                            @label = string.Format("{0}[{1}]", item.Key.MaterialLot, item.Key.MaterialCode),
//                            @value = item.Key.MaterialLot,
//                            @qty = item.CurrentQty
//                            //@SupplierMaterialLot = item.SupplierMaterialLot

//                        }, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetReturnNoForERP(string orderNumber)
//        {
//            string prefix = string.Format("TMK{0:yyMM}", DateTime.Now);
//            int itemNo = 0;
//            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    PageNo = 0,
//                    PageSize = 1,
//                    Where = string.Format("Key LIKE '{0}%'", prefix),
//                    OrderBy = "Key Desc"
//                };
//                MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);
//                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
//                {
//                    string sItemNo = result.Data[0].Key.Replace(prefix, "");
//                    int.TryParse(sItemNo, out itemNo);
//                }
//            }
//            return Json(prefix + (itemNo + 1).ToString("0000"), JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult CreateXML(MaterialReturnDetailQueryViewModel model)
//        {
//            MethodReturnResult result = new MethodReturnResult();
//            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//            {

//                //MethodReturnResult<bool> check = client.CheckReportDetail(BillCode);

//                //if (!check.Data)
//                //{
//                //    result.Code = 1003;
//                //    result.Message = string.Format("【{0}】中数据有误，请核对单据与实物！", BillCode);
//                //    return Json(result);
//                //}
//                MethodReturnResult<MaterialReturn> rst = client.Get(model.ReturnNo);
//                if (!string.IsNullOrEmpty(rst.Data.ErpCode))
//                {
//                    result.Code = 1004;
//                    result.Message = string.Format(StringResource.WOReport_CreateXML_Error_Again, model.ReturnNo);
//                    return Json(result);
//                }

//                MethodReturnResult<DataSet> rst1 = client.GetDetailByReturnNo(model.ReturnNo);
//                try
//                {
//                    string msg = "";
//                    string code = CreateXmlFile(rst.Data, rst1.Data, out msg, model);
//                    if (!string.IsNullOrEmpty(code))
//                    {
//                        MaterialReturnParameter pram = new MaterialReturnParameter()
//                        {
//                            //State = EnumReturnState.Approved,
//                            ReturnNo = model.ReturnNo,
//                            Editor = User.Identity.Name,
//                            ErpCode = code,
//                            Store = model.Store
//                        };

//                        result = client.WO(pram);
//                        if (result.Code == 0)
//                        {
//                            result.Message = string.Format(StringResource.WOReport_CreateXML_Success, model.ReturnNo);
//                        }
//                    }
//                    else
//                    {
//                        result.Code = 1001;
//                        result.Message = string.Format(StringResource.WOReport_CreateXML_Error, model.ReturnNo) + msg;
//                    }
//                }
//                catch (Exception e)
//                {
//                    LogHelper.WriteLogError("End Send Xml File:Error" + e.Message);
//                    result.Code = 1002;
//                    result.Message = string.Format(StringResource.WOReport_CreateXML_Error, model.ReturnNo) + e.Message;
//                }
//            }
//            return Json(result);
//        }

//        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
//        {
//            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
//            node.InnerText = value;
//            parentNode.AppendChild(node);
//        }

//        public string CreateXmlFile(MaterialReturn materialReturn, DataSet lstDetail, out string msg, MaterialReturnDetailQueryViewModel model)
//        {
//            MethodReturnResult result = new MethodReturnResult();
//            DataSet dsData = new DataSet();
//            using (ERPClient client = new ERPClient())
//            {
//                MethodReturnResult<DataSet> dsResult = client.GetERPWorkOrder(materialReturn.OrderNumber);
//                if (dsResult.Code > 0)
//                {
//                    result.Code = dsResult.Code;
//                    result.Message = dsResult.Message;
//                    //return result;
//                }
//                dsData = dsResult.Data;

//            }

//            MethodReturnResult result1 = new MethodReturnResult();
//            DataSet dsData1 = new DataSet();
//            using (ERPClient client = new ERPClient())
//            {
//                MethodReturnResult<DataSet> dsResult = client.GetERPOrderType(dsData.Tables[0].Rows[0]["vtrantypecode"].ToString());
//                if (dsResult.Code > 0)
//                {
//                    result1.Code = dsResult.Code;
//                    result1.Message = dsResult.Message;
//                    //return result;
//                }
//                dsData1 = dsResult.Data;

//            }

//            msg = "";
//            string returnCode = "";
//            DataTable dt = GetWorkOrder(materialReturn.OrderNumber);
//            string strPreorderWord = materialReturn.OrderNumber;

//            Hashtable htTables = new Hashtable();
//            htTables.Add(materialReturn.OrderNumber, dt);

//            if (dsData1.Tables[0].Rows.Count > 0)
//            {
//                string startTime = Convert.ToDateTime(materialReturn.EditTime).ToShortDateString() + " 00:00:00";
//                string endTime = Convert.ToDateTime(materialReturn.EditTime).ToShortDateString() + " 23:59:59";

//                XmlDocument xmlDoc = new XmlDocument();
//                //创建类型声明节点  
//                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "GB2312", "");
//                xmlDoc.AppendChild(node);
//                //创建根节点  
//                XmlElement root = xmlDoc.CreateElement("ufinterface");
//                root.SetAttribute("receiver", ERPOrg);
//                root.SetAttribute("sender", "mes");
//                root.SetAttribute("roottag", "");
//                root.SetAttribute("replace", "Y");
//                root.SetAttribute("isexchange", "Y");
//                root.SetAttribute("groupcode", ERPGroupCode);
//                root.SetAttribute("filename", "");
//                root.SetAttribute("billtype", "4D");
//                root.SetAttribute("account", ERPAccount);
//                xmlDoc.AppendChild(root);

//                XmlElement Node = xmlDoc.CreateElement("bill");//创建节点ufinterface子节点bill   
//                Node.SetAttribute("id", "");
//                xmlDoc.DocumentElement.AppendChild(Node);

//                XmlNode billheadNode = xmlDoc.CreateNode(XmlNodeType.Element, "billhead", null);
//                Node.AppendChild(billheadNode);

//                CreateNode(xmlDoc, billheadNode, "cgeneralhid", "");

//                XmlElement cgeneralbidNode = xmlDoc.CreateElement("cgeneralbid");
//                billheadNode.AppendChild(cgeneralbidNode);

//                if (lstDetail.Tables[0].Rows.Count > 0)
//                {

//                    //t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.PACKAGE_NO ,t2.COLOR,t2.GRADE ,
//                    //t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER

//                    #region 创建xml
//                    int i = 1;
//                    foreach (var item in lstDetail.Tables[0].AsEnumerable())
//                    {
//                        //dt = GetWorkOrder(item["ORDER_NUMBER"].ToString().ToUpper());
//                        //if(dt==null || dt.Rows.Count==0)
//                        //{

//                        //}
//                        // string Effi = GetEffi(item["PACKAGE_NO"].ToString());
//                        if (htTables.ContainsKey(item["ORDER_NUMBER"].ToString().ToUpper()))
//                        {
//                            dt = (DataTable)htTables[item["ORDER_NUMBER"].ToString().ToUpper()];
//                        }
//                        else
//                        {
//                            dt = GetWorkOrder(item["ORDER_NUMBER"].ToString().ToUpper());
//                            if (dt == null || dt.Rows.Count == 0)
//                            {
//                                msg = string.Format("工单{0}在ERP中不存在。", item["ORDER_NUMBER"].ToString().ToUpper());
//                                return "";
//                            }
//                            htTables.Add(item["ORDER_NUMBER"].ToString().ToUpper(), dt);
//                        }

//                        DataTable dt_WRDetail = GetDetailByName(item["MATERIAL_LOT"].ToString());

//                        DataRow[] wrDetailRows = dt_WRDetail.Select(
//                               string.Format(" vbatchcode = '{0}'"
//                                              , item["MATERIAL_LOT"].ToString()));

//                        if (wrDetailRows == null || wrDetailRows.Length == 0)
//                        {
//                            result.Code = 1001;
//                            result.Message = string.Format("报工单{0}中工单{1}明细批号{2}在ERP中不存在", materialReturn.Key, item["ORDER_NUMBER"].ToString(), item["MATERIAL_LOT"].ToString());
//                            //return result2;
//                        }

//                        DataTable dt_Unit = GetUnit(item["MATERIAL_CODE"].ToString());//得到主辅单位
//                        DataTable dt_WorkStock = GetERPWorkStock(item["ORDER_NUMBER"].ToString());//得到备料号
//                        DataTable dt_WorkStockInfo = GetERPWorkStockInfo(dt_WorkStock.Rows[0]["VBILLCODE"].ToString());//得到备料计划信息
//                        DataTable dt_WorkSourceId = GetWorkOrder(item["ORDER_NUMBER"].ToString());//得到工单相关信息
//                        string EffiCode = GetEffiByName(item["MATERIAL_CODE"].ToString());//得到转换率

//                        XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
//                        cgeneralbidNode.AppendChild(itemNode);

//                        CreateNode(xmlDoc, itemNode, "cgeneralbid", "");
//                        CreateNode(xmlDoc, itemNode, "crowno", (i++).ToString());
//                        CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);
//                        CreateNode(xmlDoc, itemNode, "corpoid", ERPOrg);
//                        CreateNode(xmlDoc, itemNode, "corpvid", ERPOrg);
//                        CreateNode(xmlDoc, itemNode, "cstateid", "");
//                        CreateNode(xmlDoc, itemNode, "cmaterialoid", item["MATERIAL_CODE"].ToString());//物料编码
//                        CreateNode(xmlDoc, itemNode, "cmaterialvid", item["MATERIAL_CODE"].ToString());//物料编码
//                        CreateNode(xmlDoc, itemNode, "vfree1", "");
//                        CreateNode(xmlDoc, itemNode, "vfree2", "");
//                        CreateNode(xmlDoc, itemNode, "vfree3", "");
//                        CreateNode(xmlDoc, itemNode, "vfree4", wrDetailRows[0]["BATTRANSRATE"].ToString());//功率
//                        CreateNode(xmlDoc, itemNode, "vfree5", wrDetailRows[0]["BATCOLOR"].ToString());//颜色
//                        CreateNode(xmlDoc, itemNode, "vfree6", wrDetailRows[0]["BATLVL"].ToString());//等级
//                        CreateNode(xmlDoc, itemNode, "vfree7", "");
//                        CreateNode(xmlDoc, itemNode, "cunitid", dt_Unit.Rows[0]["MEASCODE"].ToString());//主单位
//                        CreateNode(xmlDoc, itemNode, "castunitid", dt_Unit.Rows[0]["ASTMEASCODE"].ToString());//单位
//                        CreateNode(xmlDoc, itemNode, "vchangerate", EffiCode.ToString());//换算率
//                        CreateNode(xmlDoc, itemNode, "vbatchcode", item["MATERIAL_LOT"].ToString());
//                        CreateNode(xmlDoc, itemNode, "nshouldnum", item["QTY"].ToString());//应收主数量
//                        decimal mideffiCode = 0;
//                        if (!string.IsNullOrEmpty(EffiCode) && EffiCode.Contains('/'))
//                        {
//                            mideffiCode =Convert.ToDecimal(EffiCode.Split('/')[0]) / Convert.ToDecimal(EffiCode.Split('/')[1]);
//                        }
//                        CreateNode(xmlDoc, itemNode, "nshouldassistnum", (Convert.ToDecimal(item["Qty"].ToString()) * mideffiCode).ToString("f3"));//应发辅数量
//                        CreateNode(xmlDoc, itemNode, "nnum", "");//主数量
//                        CreateNode(xmlDoc, itemNode, "nassistnum", "");//实发数量
//                        CreateNode(xmlDoc, itemNode, "dbizdate","");//出库日期
//                        CreateNode(xmlDoc, itemNode, "vproductbatch", item["ORDER_NUMBER"].ToString());
//                        CreateNode(xmlDoc, itemNode, "cpickmcode", dt_WorkStock.Rows[0]["VBILLCODE"].ToString());//备料计划单据号
//                        CreateNode(xmlDoc, itemNode, "csourcebillhid", dt_WorkStockInfo.Rows[0]["CPICKMID"].ToString());//dt.Rows[0]["PK_WR"].ToString());//来源单据表头主键
//                        CreateNode(xmlDoc, itemNode, "csourcebillbid", dt_WorkStockInfo.Rows[0]["CPICKM_BID"].ToString());// wrDetailRows[0]["PK_WR_QUALITY"] == null ? "" : wrDetailRows[0]["PK_WR_QUALITY"].ToString());
//                        CreateNode(xmlDoc, itemNode, "csourcetype", "55A3");
//                        CreateNode(xmlDoc, itemNode, "csourcebillbid", dt_WorkStockInfo.Rows[0]["CPICKM_BID"].ToString());// wrDetailRows[0]["PK_WR_QUALITY"] == null ? "" : wrDetailRows[0]["PK_WR_QUALITY"].ToString());
//                        CreateNode(xmlDoc, itemNode, "vsourcerowno", dt_WorkStockInfo.Rows[0]["VROWNO"].ToString());//wrDetailRows[0]["PK_WR_PRODUCT"] == null ? "" : wrDetailRows[0]["PK_WR_PRODUCT"].ToString());
//                        CreateNode(xmlDoc, itemNode, "cprojectid", "");
//                        CreateNode(xmlDoc, itemNode, "casscustid", "");
//                        CreateNode(xmlDoc, itemNode, "cfirsttype", "55C2");// wrDetailRows[0]["CBSRCTYPE"].ToString());
//                        CreateNode(xmlDoc, itemNode, "cfirstbillhid", dt_WorkSourceId.Rows[0]["PK_DMO"].ToString());
//                        CreateNode(xmlDoc, itemNode, "vfirstbillcode", item["ORDER_NUMBER"].ToString());
//                        CreateNode(xmlDoc, itemNode, "vfirstrowno", "");
//                        CreateNode(xmlDoc, itemNode, "cfirstbillbid", dt_WorkSourceId.Rows[0]["VFIRSTMOID"].ToString());// 源头单据行标识
//                        CreateNode(xmlDoc, itemNode, "cvendorid", wrDetailRows[0]["SUPPLIERCODE"].ToString());//供应商
//                        CreateNode(xmlDoc, itemNode, "cproductorid", wrDetailRows[0]["SUPPLIERCODE"].ToString());//生产厂商
//                        CreateNode(xmlDoc, itemNode, "vnotebody", "");
//                        CreateNode(xmlDoc, itemNode, "bbarcodeclose", "N");
//                        CreateNode(xmlDoc, itemNode, "bonroadflag", "N");
//                        CreateNode(xmlDoc, itemNode, "dproducedate", "");//生产日期
//                        CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);
//                        CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);
//                        CreateNode(xmlDoc, itemNode, "cbodywarehouseid", model.Store.ToString());//仓库
//                        CreateNode(xmlDoc, itemNode, "pk_batchcode", "");
//                        CreateNode(xmlDoc, itemNode, "csrcmaterialoid", "");
//                        CreateNode(xmlDoc, itemNode, "csrcmaterialvid", "");
//                        CreateNode(xmlDoc, itemNode, "ccostobject", item["MATERIAL_CODE"].ToString());

//                    }
//                    #endregion
//                }
//                CreateNode(xmlDoc, billheadNode, "pk_group", ERPGroupCode);
//                CreateNode(xmlDoc, billheadNode, "pk_org", ERPOrg);
//                CreateNode(xmlDoc, billheadNode, "pk_org_v", ERPOrg);
//                CreateNode(xmlDoc, billheadNode, "cdrawcalbodyvid", ERPOrg);
//                CreateNode(xmlDoc, billheadNode, "cdrawcalbodyoid", ERPOrg);
//                CreateNode(xmlDoc, billheadNode, "cwarehouseid", model.Store.ToString());//仓库
//                CreateNode(xmlDoc, billheadNode, "cdptid", dt.Rows[0]["DEPTCODE"].ToString());
//                CreateNode(xmlDoc, billheadNode, "cdptvid", dt.Rows[0]["DEPTCODE"].ToString());
//                CreateNode(xmlDoc, billheadNode, "ctrantypeid", "4D-01");
//                CreateNode(xmlDoc, billheadNode, "vtrantypecode","4D-01");
//                CreateNode(xmlDoc, billheadNode, "vnote", "");
//                CreateNode(xmlDoc, billheadNode, "cwhsmanagerid", "");
//                CreateNode(xmlDoc, billheadNode, "cbizid", "");
//                CreateNode(xmlDoc, billheadNode, "vnote", "");
//                CreateNode(xmlDoc, billheadNode, "fbillflag", "2");
//                CreateNode(xmlDoc, billheadNode, "creator", materialReturn.Creator.ToString());
//                CreateNode(xmlDoc, billheadNode, "billmaker", materialReturn.Editor.ToString());
//                CreateNode(xmlDoc, billheadNode, "creationtime",materialReturn.CreateTime.ToString());

//                string path = Server.MapPath("~/XMLFile/");
//                if (Directory.Exists(path) == false)
//                {
//                    Directory.CreateDirectory(path);
//                }
//                path = path + materialReturn.Key + ".xml";
//                xmlDoc.Save(path);

//                FileStream ms = new FileStream(path, FileMode.Open, FileAccess.Read);

//                string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];
//                HttpWebRequest loHttp = (HttpWebRequest)WebRequest.Create(url);  //URL为XChangeServlet地址

//                loHttp.Method = "POST";
//                // *** Set any header related and operational properties
//                loHttp.Timeout = 10000;  // 10 secs
//                loHttp.UserAgent = "Code Sample Web Client";

//                // *** reuse cookies if available
//                loHttp.CookieContainer = new CookieContainer();

//                if (this.oCookies != null && this.oCookies.Count > 0)
//                {
//                    loHttp.CookieContainer.Add(this.oCookies);
//                }

//                #region send Xml To NCS
//                LogHelper.WriteLogError("Begin Send Xml File");
//                //loHttp.ContentLength = ms.Length;
//                Stream requestStream = loHttp.GetRequestStream();

//                byte[] buffer = new Byte[(int)ms.Length];
//                int bytesRead = 0;
//                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) != 0)
//                    requestStream.Write(buffer, 0, bytesRead);
//                //requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);  
//                requestStream.Close();

//                // *** Return the Response data
//                HttpWebResponse loWebResponse = (HttpWebResponse)loHttp.GetResponse();

//                if (loWebResponse.Cookies.Count > 0)
//                    if (this.oCookies == null)
//                    {
//                        this.oCookies = loWebResponse.Cookies;
//                    }
//                    else
//                    {
//                        // ** If we already have cookies update the list
//                        foreach (Cookie oRespCookie in loWebResponse.Cookies)
//                        {
//                            bool bMatch = false;
//                            foreach (Cookie oReqCookie in this.oCookies)
//                            {
//                                if (oReqCookie.Name == oRespCookie.Name)
//                                {
//                                    oReqCookie.Value = oRespCookie.Name;
//                                    bMatch = true;
//                                    break; // 
//                                }
//                            }
//                            if (!bMatch)
//                                this.oCookies.Add(oRespCookie);
//                        }
//                    }

//                Encoding enc = Encoding.GetEncoding("gb2312");  // Windows-1252 or iso-
//                if (loWebResponse.ContentEncoding.Length > 0)
//                {
//                    enc = Encoding.GetEncoding(loWebResponse.ContentEncoding);
//                }

//                StreamReader loResponseStream =
//                    new StreamReader(loWebResponse.GetResponseStream());

//                string ResponseText = loResponseStream.ReadToEnd();

//                LogHelper.WriteLogError("End Send Xml File");
//                XmlDocument Doc = new XmlDocument();
//                Doc.LoadXml(ResponseText);

//                //获取ERP回执
//                XmlNode xnode = Doc.SelectSingleNode("ufinterface/sendresult/content");
//                returnCode = xnode.InnerText;

//                //获取ERP错误信息提示
//                if (returnCode == "")
//                {
//                    XmlNode errornode = Doc.SelectSingleNode("ufinterface/sendresult/resultdescription");
//                    msg = errornode.InnerText;
//                }

//                loResponseStream.Close();
//                loWebResponse.Close();
//                ms.Close();
//                requestStream.Close();
//                #endregion
//            }
//            else
//            {
//                msg = string.Format(StringResource.ERPWorkOrderQuery_Error_Query, materialReturn.OrderNumber);
//            }
//            return returnCode;
//        }


//        public DataTable GetUnit(string MaterialCode)
//        {
//            DataTable dt = new DataTable();
//            using (WOReportClient client = new WOReportClient())
//            {
//                MethodReturnResult<DataSet> ds = client.GetUnitByMaterialCode(MaterialCode);

//                dt = ds.Data.Tables[0];
//            }
//            return dt;
//        }//得到单位


//        public DataTable GetWorkOrder(string OrderNumber)
//        {
//            DataTable dt = new DataTable();
//            using (ERPClient client = new ERPClient())
//            {
//                MethodReturnResult<DataSet> ds = client.GetERPWorkOrder(OrderNumber);
//                if (ds != null && ds.Data != null && ds.Data.Tables.Count > 0)
//                {
//                    dt = ds.Data.Tables[0];
//                }
//            }
//            return dt;
//        }//得到工单号

//        public string GetCodeByName(string Name)//通过名字（效率、等级、花色等）得到在ERP中可以识别的名字
//        {
//            string Code = "";
//            using (WOReportClient client = new WOReportClient())
//            {
//                MethodReturnResult<DataSet> ds = client.GetCodeByName(Name);
//                if (ds.Code == 0 && ds.Data.Tables[0].Rows.Count > 0)
//                {
//                    Code = ds.Data.Tables[0].Rows[0]["CODE"].ToString();
//                }
//            }
//            return Code;
//        }

//        public string GetEffiByName(string Code)//根据物料批次得到转换率
//        {
//            //string Code = "";
//            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//            {
//                MethodReturnResult<DataSet> ds = client.GetEffiByName(Code);
//                if (ds.Code == 0 && ds.Data.Tables[0].Rows.Count > 0)
//                {
//                    if (ds.Code.ToString().StartsWith("1101"))
//                    {
//                        Code = "1/" + ds.Data.Tables[0].Rows[0]["ATTR_1"].ToString();
//                    }
//                    else
//                    {
//                        Code = ds.Data.Tables[0].Rows[0]["ATTR_1"].ToString();
//                    }
                    
//                }
//            }
//            return Code;
//        }


//        public DataTable GetDetailByName(string LotNo)//根据物料批次得到颜色、效率、等级等信息
//        {
//            //string Code = "";
//            DataTable dt = new DataTable();
//            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//            {
//                MethodReturnResult<DataSet> ds = client.GetERPMaterialReceiptDetail(LotNo);

//                dt = ds.Data.Tables[0];
//            }
//            return dt;
//        }

//        public DataTable GetERPWorkStock(string OrderNumber)
//        {
//            DataTable dt = new DataTable();
//            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//            {
//                MethodReturnResult<DataSet> ds = client.GetERPWorkStock(OrderNumber);

//                dt = ds.Data.Tables[0];
//            }
//            return dt;
//        }//根据工单号查询备料单号


//        public DataTable GetERPWorkStockInfo(string BLNumber)//根据备料号查询来源单据相关信息
//        {
//            DataTable dt = new DataTable();
//            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//            {
//                MethodReturnResult<DataSet> ds = client.GetERPWorkStockInfo(BLNumber);

//                dt = ds.Data.Tables[0];
//            }
//            return dt;
//        }


//        public DataTable GetMaterialInfo(string MaterialCode)//根据物料编码得到物料名称，规格等信息
//        {
//            DataTable dt = new DataTable();
//            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//            {
//                MethodReturnResult<DataSet> ds = client.GetMaterialInfo(MaterialCode);

//                dt = ds.Data.Tables[0];
//            }
//            return dt;
//        }


//        public DataTable GetStoreName(string Store)//根据物料编码得到物料名称，规格等信息
//        {
//            DataTable dt = new DataTable();
//            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//            {
//                MethodReturnResult<DataSet> ds = client.GetStoreName(Store);

//                dt = ds.Data.Tables[0];
//            }
//            return dt;
//        }

//        private CookieCollection _oCookies = null;
//        protected CookieCollection oCookies
//        {
//            get
//            {
//                return _oCookies;
//            }
//            set
//            {
//                _oCookies = value;
//            }

//        }



//        public ActionResult Print(MaterialReturnDetailQueryViewModel model)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(model.ReturnNo))
//                {
//                    return Content(string.Empty);
//                }
//                return ShowStgInReport(model.ReturnNo);
//            }
//            catch (Exception ex)
//            {
//                return Content(ex.ToString());
//            }
//        }

//        [AllowAnonymous]
//        public ActionResult ShowStgInReport(string ReturnNo)//得到打印单据上显示的数据
//        {
//            MethodReturnResult result = new MethodReturnResult();
//            DataSet dsData = new DataSet();
//            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
//            {
//                MethodReturnResult<DataSet> dsResult = client.GetReturnReportFromDB(ReturnNo);
//                if (dsResult.Code > 0)
//                {
//                    result.Code = dsResult.Code;
//                    result.Message = dsResult.Message;
//                    //return result;
//                }
//                dsData = dsResult.Data;
//            }
//            DataTable dtStgIn = dsData.Tables["StgIn"];

//            PackageListDataSet ds = new PackageListDataSet();
//            PackageListDataSet.StgInRow row = ds.StgIn.NewStgInRow();
//            if (dtStgIn.Rows.Count > 0)
//            {
//                DataTable dtERP = new DataTable();
//                using (MaterialReturnServiceClient clientERP = new MaterialReturnServiceClient())
//                {
//                    MethodReturnResult<DataSet> dsERP = clientERP.GetERPReportCodeById(dtStgIn.Rows[0]["ERP_RETURN_CODE"] == null ? "" : dtStgIn.Rows[0]["ERP_RETURN_CODE"].ToString());
//                    if (dsERP.Code > 0)
//                    {
//                        result.Code = dsERP.Code;
//                        result.Message = dsERP.Message;
//                        //return result;
//                    }
//                    else
//                    {
//                        dtERP = dsERP.Data.Tables[0];
//                    }

//                }
//                if (dtERP.Rows.Count > 0)
//                {
//                    row.ERPBillCode = dtERP.Rows[0]["VBILLCODE"].ToString();//出库单号
//                    //row.ERPBillCode = "1CC-16040001";//出库单号
//                    DataTable dt = GetWorkOrder(dtStgIn.Rows[0]["ORDER_NUMBER"].ToString());//得到备料号
//                    row.Cdptid = dt.Rows[0]["DEPTCODE"].ToString();//领料部门

//                    //row.Cdptid = "k01377";//领料部门
//                    DataTable dt_WorkStock = GetERPWorkStock(dtStgIn.Rows[0]["ORDER_NUMBER"].ToString());
//                    row.BillCode = dt_WorkStock.Rows[0]["VBILLCODE"].ToString();//备料单号
//                    //row.BillCode = "1BL-16040001";
//                    row.Type = dtERP.Rows[0]["TRANTYPENAME"].ToString();
//                }

//                row.BillDate = dtStgIn.Rows[0]["RETURN_DATE"].ToString();
//                row.BillMaker = dtStgIn.Rows[0]["CREATOR"].ToString();
//                row.OrderNumber = dtStgIn.Rows[0]["ORDER_NUMBER"].ToString();
//                DataTable dt_StoreName = GetStoreName(dtStgIn.Rows[0]["STORE"].ToString());
//                row.Store = dt_StoreName.Rows[0]["STORNAME"].ToString();
//                row.Note = dtStgIn.Rows[0]["DESCRIPTION"].ToString();
//                ds.StgIn.AddStgInRow(row);
//            }


//            if (dsData.Tables.Contains("StgInDetail"))
//            {
//                double dQty = 0;
//                DataTable dtStgInDetail = dsData.Tables["StgInDetail"];
//                PackageListDataSet.StgInDetailRow rowDetail = ds.StgInDetail.NewStgInDetailRow();
//                for (int i = 0; i < dtStgInDetail.Rows.Count; i++)
//                {


//                    rowDetail = ds.StgInDetail.NewStgInDetailRow();
//                    rowDetail.ItemNo = (i + 1).ToString();
//                    rowDetail.MaterialCode = dtStgInDetail.Rows[i]["MATERIAL_CODE"].ToString();
//                    DataTable dt_MaterialInfo = GetMaterialInfo(dtStgInDetail.Rows[i]["MATERIAL_CODE"].ToString());//得到物料名称、规格信息
//                    rowDetail.MaterialName = dt_MaterialInfo.Rows[i]["INVNAME"].ToString();
//                    //rowDetail.MaterialName = "玻璃";
//                    //rowDetail.MaterialStandard = dt_MaterialInfo.Rows[i]["MATERIAL_NAME"].ToString();
//                    rowDetail.MaterialStandard = dt_MaterialInfo.Rows[i]["MATERIALSPEC"].ToString();
//                    DataTable dt_Unit = GetUnit(dtStgInDetail.Rows[i]["MATERIAL_CODE"].ToString());//根据物料编码得到主辅单位
//                    rowDetail.PrimaryUnit = dt_Unit.Rows[0]["MEASNAME"].ToString();//主单位
//                    rowDetail.AuxiliaryUnit = dt_Unit.Rows[0]["ASTMEASNAME"].ToString();//辅单位
//                    string EffiCode = GetEffiByName(dtStgInDetail.Rows[i]["MATERIAL_CODE"].ToString());//得到转换率
//                    rowDetail.ChangeRate = EffiCode.ToString();
//                    rowDetail.ObjectNumber = dtStgInDetail.Rows[i]["MATERIAL_LOT"].ToString();//物料批号
//                    if (Double.TryParse(dtStgInDetail.Rows[i]["QTY"].ToString(), out dQty) == false)
//                    {
//                        dQty = 0;
//                    }
//                    rowDetail.Qty = dQty;
//                    //rowDetail.Qty = 0;
//                    ds.StgInDetail.AddStgInDetailRow(rowDetail);
//                }

//            }


//            //
//            using (LocalReport localReport = new LocalReport())
//            {
//                localReport.ReportPath = Server.MapPath("~/RDLC/ReturnInList.rdlc");

//                ReportDataSource reportDataSourcePackage = new ReportDataSource("StgIn", ds.Tables[ds.StgIn.TableName]);
//                localReport.DataSources.Add(reportDataSourcePackage);
//                ReportDataSource reportDataSourcePackageDetail = new ReportDataSource("StgInDetail", ds.Tables[ds.StgInDetail.TableName]);
//                localReport.DataSources.Add(reportDataSourcePackageDetail);
//                string reportType = "PDF";
//                string mimeType;
//                string encoding;
//                string fileNameExtension;
//                //The DeviceInfo settings should be changed based on the reportType
//                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
//                string deviceInfo =
//                                "<DeviceInfo>" +
//                                "  <OutputFormat>PDF</OutputFormat>" +
//                                "  <PageWidth>24cm</PageWidth>" +
//                                "  <PageHeight>14cm</PageHeight>" +
//                                "  <MarginTop>0.5cm</MarginTop>" +
//                                "  <MarginLeft>0.5cm</MarginLeft>" +
//                                "  <MarginRight>0cm</MarginRight>" +
//                                "  <MarginBottom>0.5cm</MarginBottom>" +
//                                "</DeviceInfo>";
//                Warning[] warnings;
//                string[] streams;
//                byte[] renderedBytes;
//                //Render the report
//                renderedBytes = localReport.Render(
//                    reportType,
//                    deviceInfo,
//                    out mimeType,
//                    out encoding,
//                    out fileNameExtension,
//                    out streams,
//                    out warnings);
//                //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
//                return File(renderedBytes, mimeType);
//            }

//        }


//        #endregion


        #region 退料到ERP

        public async Task<ActionResult> IndexForERP()
        {
            return await QueryForERP(new MaterialReturnQueryViewModel());
        }
        //
        //POST: /LSM/MaterialReturn/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QueryForERP(MaterialReturnQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.ReturnNo))
                            {
                                where.AppendFormat(" {0} Key = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ReturnNo);
                            }
                            if (!string.IsNullOrEmpty(model.OrderNumber))
                            {
                                where.AppendFormat(" {0} OrderNumber = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);
                            }

                            if (model.StartReturnDate != null)
                            {
                                where.AppendFormat(" {0} ReturnDate >= '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.StartReturnDate);
                            }

                            if (model.EndReturnDate != null)
                            {
                                where.AppendFormat(" {0} ReturnDate <= '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EndReturnDate);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc",
                            Where = where.ToString()
                        };

                        MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);

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
                return PartialView("_ListPartialForERP");
            }
            else
            {
                return View("IndexForERP");
            }
        }
        //
        //POST: /LSM/MaterialReturn/PagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PagingQueryForERP(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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

                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
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
                        MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartialForERP");
        }
        //
        // GET: /LSM/MaterialReturn/
        public async Task<ActionResult> DetailForERP(MaterialReturnDetailQueryViewModel model)
        {
            //取得退料单表头对象
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "CreateTime Desc,Key.ReturnNo,Key.ItemNo",
                        Where = GetWhereCondition(model)
                    };

                    MethodReturnResult<MaterialReturn> materialReturnData = client.Get(model.ReturnNo);

                    if (materialReturnData.Code == 0)
                    {
                        model.LineStoreName = materialReturnData.Data.Store;
                        model.OrderNumber = materialReturnData.Data.OrderNumber;
                        model.ReturnDate = materialReturnData.Data.ReturnDate;
                    }
                });
            }
            
            return await DetailQueryForERP(model);
        }
        //
        //POST: /LSM/MaterialReturn/DetailQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailQueryForERP(MaterialReturnDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
                {
                    await Task.Run(() =>
                    {
                        //PagingConfig cfg = new PagingConfig()
                        //{
                        //    OrderBy = "CreateTime Desc,Key.ReturnNo,Key.ItemNo",
                        //    Where = GetWhereCondition(model)
                        //};
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc,Key.ReturnNo,Key.ItemNo",
                            Where = string.Format("Key.ReturnNo = '{0}'", model.ReturnNo)
                        };

                        MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }

                //取得仓库列表
                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
                {
                    List<SelectListItem> StoreList = new List<SelectListItem>();
                    MethodReturnResult<DataSet> ds = client.GetStore();
                    if (ds.Data.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Data.Tables[0].Rows.Count; i++)
                        {
                            StoreList.Add(new SelectListItem() { Text = ds.Data.Tables[0].Rows[i]["STORNAME"].ToString(), Value = ds.Data.Tables[0].Rows[i]["STORCODE"].ToString() });
                        }
                    }
                    //StoreList.Add(new SelectListItem() { Text = "废料仓", Value = "FP001" });
                    ViewBag.Store = StoreList;
                }

            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_DetailListPartialForERP", new MaterialReturnDetailViewModel());
            }
            else
            {
                return View("DetailForERP", model);
            }
        }

        public string GetWhereConditionForERP(MaterialReturnDetailQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.ReturnNo))
                {
                    where.AppendFormat(" {0} Key.ReturnNo = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ReturnNo);
                }

                if (!string.IsNullOrEmpty(model.LineStoreName))
                {
                    where.AppendFormat(" {0} LineStoreName = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LineStoreName);
                }

                if (!string.IsNullOrEmpty(model.MaterialCode))
                {
                    where.AppendFormat(" {0} MaterialCode LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialCode);
                }

                if (!string.IsNullOrEmpty(model.MaterialLot))
                {
                    where.AppendFormat(" {0} MaterialLot LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialLot);
                }

                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    where.AppendFormat(@" {0} EXISTS(FROM MaterialReturn as p
                                                    WHERE p.Key=self.Key.ReturnNo
                                                    AND p.OrderNumber = '{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber);
                }

//                if (!string.IsNullOrEmpty(model.ReturnDate))
//                {
//                    where.AppendFormat(@" {0} EXISTS(FROM MaterialReturn as p
//                                                    WHERE p.Key=self.Key.ReturnNo
//                                                    AND p.ReturnDate = '{1}')"
//                                        , where.Length > 0 ? "AND" : string.Empty
//                                        , model.ReturnDate);
//                }
            }
            return where.ToString();
        }

        //
        //POST: /LSM/MaterialReturn/DetailPagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailPagingQueryForERP(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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

                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
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
                        MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_DetailListPartialForERP", new MaterialReturnDetailQueryViewModel());
        }


        //
        //POST: /WIP/MaterialReturn/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcelForERP(MaterialReturnDetailQueryViewModel model)
        {
            IList<MaterialReturnDetail> lst = new List<MaterialReturnDetail>();
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "CreateTime Desc,Key.ReturnNo,Key.ItemNo",
                        Where = GetWhereCondition(model)
                    };
                    MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        lst = result.Data;
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
            ICell cell = null;
            IRow row = null;
            ISheet ws = null;
            for (int j = 0; j < lst.Count; j++)
            {
                if (j % 65535 == 0)
                {
                    ws = wb.CreateSheet();
                    row = ws.CreateRow(0);
                    #region //列名
                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_ReturnNo);  //退料号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_OrderNumber);  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_ReturnDate);  //领料日期

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_ItemNo);  //项目号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_LineStoreName);  //线别仓

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_MaterialCode);  //物料编码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("物料名称");  //物料名称

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_MaterialLot);  //物料批号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_Qty);  //数量


                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("描述");  //描述

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑人");  //编辑人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑时间");  //编辑时间
                    #endregion
                    font.Boldweight = 5;
                }

                MaterialReturnDetail obj = lst[j];
                MaterialReturn mrObj = model.GetMaterialReturn(obj.Key.ReturnNo);
                Material m = model.GetMaterial(obj.MaterialCode);
                row = ws.CreateRow(j + 1);

                #region //数据
                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ReturnNo);  //领料号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(mrObj == null ? string.Empty : mrObj.OrderNumber);  //工单号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(mrObj == null ? string.Empty : string.Format("{0:yyyy-MM-dd}", mrObj.ReturnDate));  //领料日期

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ItemNo);  //项目号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.LineStoreName);  //线别仓

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.MaterialCode);  //物料编码

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(m == null ? string.Empty : m.Name);  //物料名称

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.MaterialLot);  //物料批号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Qty);  //数量


                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Description);  //描述

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Editor);  //编辑人

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.EditTime));  //编辑时间
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "MaterialReturnData.xls");
        }

        //
        // POST: /PPM/MaterialReturn/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveForERP(MaterialReturnViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try
            {
                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
                {
                    MaterialReturn obj = new MaterialReturn()
                    {
                        Key = model.ReturnNo.ToUpper(),
                        OrderNumber = model.OrderNumber.ToUpper(),
                        ReturnDate = model.ReturnDate,
                        Description = model.Description,
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name
                    };

                    char splitChar = ',';
                    var ItemNos = Request["ItemNo"].Split(splitChar);
                    var LineStoreNames = Request["LineStoreName"].Split(splitChar);
                    var MaterialCodes = Request["MaterialCode"].Split(splitChar);
                    var MaterialLots = Request["MaterialLot"].Split(splitChar);
                    var Qtys = Request["Qty"].Split(splitChar);
                    var Descriptions = Request["DetailDescription"].Split(splitChar);

                    List<MaterialReturnDetail> lst = new List<MaterialReturnDetail>();
                    for (int i = 0; i < ItemNos.Length; i++)
                    {
                        lst.Add(new MaterialReturnDetail()
                        {
                            Key = new MaterialReturnDetailKey()
                            {
                                ReturnNo = model.ReturnNo,
                                ItemNo = i + 1
                            },
                            LineStoreName = LineStoreNames[i].ToUpper(),
                            MaterialCode = MaterialCodes[i].ToUpper(),
                            MaterialLot = MaterialLots[i].ToUpper(),
                            Qty = Convert.ToDouble(Qtys[i]),
                            //SupplierCode = SupplierCodes[i].ToUpper(),
                            Description = Descriptions[i],
                            Editor = User.Identity.Name,
                            Creator = User.Identity.Name
                        });
                    }

                    rst = await client.AddAsync(obj, lst);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(LSMResources.StringResource.MaterialReturn_Save_Success
                                                    , obj.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                rst.Code = 1000;
                rst.Message = ex.Message;
                rst.Detail = ex.ToString();
            }
            return Json(rst);
        }

        //public ActionResult GetOrderNumberForERP(string q)
        //{
        //    using (WorkOrderServiceClient client = new WorkOrderServiceClient())
        //    {
        //        PagingConfig cfg = new PagingConfig()
        //        {
        //            IsPaging = false,
        //            Where = string.Format("Key LIKE '{0}%' AND CloseType='{1}'"
        //                                    , q
        //                                    , Convert.ToInt32(EnumWorkOrderCloseType.None))
        //        };

        //        MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
        //        if (result.Code <= 0)
        //        {
        //            return Json(from item in result.Data
        //                        select new
        //                        {
        //                            @label = item.Key,
        //                            @value = item.Key
        //                        }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetLineStoreNamesForERP(string orderNumber)
        //{
        //    string locationName = string.Empty;
        //    using (WorkOrderServiceClient client = new WorkOrderServiceClient())
        //    {
        //        MethodReturnResult<WorkOrder> result = client.Get(orderNumber);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            locationName = result.Data.LocationName;
        //        }
        //    }

        //    IList<LineStore> lstLineStore = new List<LineStore>();
        //    using (LineStoreServiceClient client = new LineStoreServiceClient())
        //    {
        //        PagingConfig cfg = new PagingConfig()
        //        {
        //            IsPaging = false,
        //            Where = string.Format("LocationName='{0}' AND Type='{1}'", locationName, Convert.ToInt32(EnumLineStoreType.Material))
        //        };

        //        MethodReturnResult<IList<LineStore>> result = client.Get(ref cfg);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lstLineStore = result.Data;
        //        }
        //    }

        //    IList<Resource> lstResource = new List<Resource>();
        //    using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
        //    {
        //        MethodReturnResult<IList<Resource>> result = client.GetResourceList(User.Identity.Name, ResourceType.LineStore);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lstResource = result.Data;
        //        }
        //    }

        //    var lnq = from item in lstLineStore
        //              where lstResource.Any(m => m.Data == item.Key)
        //              select new
        //              {
        //                  Key = item.Key
        //              };
        //    return Json(lnq, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetMaterialCodeForERP(string q, string orderNumber, string lineStoreName)
        {
            string routeOperationName = string.Empty;
            using (LineStoreServiceClient client = new LineStoreServiceClient())
            {
                MethodReturnResult<LineStore> result = client.Get(lineStoreName);
                if (result.Code <= 0 && result.Data != null)
                {
                    routeOperationName = result.Data.RouteOperationName;
                }
            }
            //根据工单获取物料编码。
            IList<WorkOrderBOM> lstBOM = new List<WorkOrderBOM>();
            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"MaterialCode LIKE '{0}%' AND Key.OrderNumber='{1}'"
                                            , q
                                            , orderNumber),
                    OrderBy = "Key.ItemNo"
                };
                //工作中心为空的可以领到任何线边仓。
                //线边仓对应工序为空的可以领任何料。
                if (!string.IsNullOrEmpty(routeOperationName))
                {
                    cfg.Where += string.Format(" AND (WorkCenter='' OR WorkCenter IS NULL Or WorkCenter='{0}')", routeOperationName);
                }

                MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstBOM = result.Data;
                }
            }
            //获取线边仓中已有物料明细数据。
            IList<LineStoreMaterialDetail> lstDetail = new List<LineStoreMaterialDetail>();
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.MaterialCode LIKE '{0}%' AND Key.LineStoreName='{1}' AND CurrentQty>0"
                                            , q
                                            , lineStoreName)
                };
                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            return Json(from item in lstBOM
                        where lstDetail.Any(m => m.Key.MaterialCode == item.MaterialCode)
                        select new
                        {
                            @label = string.Format("{0}[{1}]", item.MaterialCode, item.Description),
                            @value = item.MaterialCode,
                            @desc = item.Description
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialLotForERP(string materialLot, string materialCode, string orderNumber, string lineStoreName)
        {
            IList<LineStoreMaterialDetail> lstDetail = new List<LineStoreMaterialDetail>();
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.LineStoreName='{0}'
                                            AND Key.MaterialCode='{1}'
                                            AND Key.OrderNumber='{3}'
                                            AND Key.MaterialLot LIKE '{2}%'
                                            AND CurrentQty>0"
                                            , lineStoreName
                                            , materialCode
                                            , materialLot
                                            , orderNumber),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            return Json(from item in lstDetail
                        select new
                        {
                            @label = string.Format("{0}[{1}]", item.Key.MaterialLot, item.Key.MaterialCode),
                            @value = item.Key.MaterialLot,
                            @qty = item.CurrentQty
                            //@SupplierMaterialLot = item.SupplierMaterialLot

                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReturnNoForERP(string orderNumber)
        {
            string prefix = string.Format("TMK{0:yyMM}", DateTime.Now);
            int itemNo = 0;
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key LIKE '{0}%'", prefix),
                    OrderBy = "Key Desc"
                };
                MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    string sItemNo = result.Data[0].Key.Replace(prefix, "");
                    int.TryParse(sItemNo, out itemNo);
                }
            }
            return Json(prefix + (itemNo + 1).ToString("0000"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateXML(MaterialReturnDetailQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {

                //MethodReturnResult<bool> check = client.CheckReportDetail(BillCode);

                //if (!check.Data)
                //{
                //    result.Code = 1003;
                //    result.Message = string.Format("【{0}】中数据有误，请核对单据与实物！", BillCode);
                //    return Json(result);
                //}
                MethodReturnResult<MaterialReturn> rst = client.Get(model.ReturnNo);
                if (!string.IsNullOrEmpty(rst.Data.ErpCode))
                {
                    result.Code = 1004;
                    result.Message = string.Format(StringResource.WOReport_StockInApply_Error_Again, model.ReturnNo);
                    return Json(result);
                }

                MethodReturnResult<DataSet> rst1 = client.GetDetailByReturnNo(model.ReturnNo);
                try
                {
                    string msg = "";
                    string code = CreateXmlFile(rst.Data, rst1.Data, out msg, model);
                    if (!string.IsNullOrEmpty(code))
                    {
                        MaterialReturnParameter pram = new MaterialReturnParameter()
                        {
                            //State = EnumReturnState.Approved,
                            ReturnNo = model.ReturnNo,
                            Editor = User.Identity.Name,
                            ErpCode = code,
                            Store = model.Store
                        };

                        result = client.WO(pram);
                        if (result.Code == 0)
                        {
                            result.Message = string.Format(StringResource.WOReport_StockInApply_Success, model.ReturnNo);
                        }
                    }
                    else
                    {
                        result.Code = 1001;
                        result.Message = string.Format(StringResource.WOReport_StockInApply_Error, model.ReturnNo) + msg;
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteLogError("End Send Xml File:Error" + e.Message);
                    result.Code = 1002;
                    result.Message = string.Format(StringResource.WOReport_StockInApply_Error, model.ReturnNo) + e.Message;
                }
            }
            return Json(result);
        }

        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        public string CreateXmlFile(MaterialReturn materialReturn, DataSet lstDetail, out string msg, MaterialReturnDetailQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            DataSet dsData = new DataSet();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> dsResult = client.GetERPWorkOrder(materialReturn.OrderNumber);
                if (dsResult.Code > 0)
                {
                    result.Code = dsResult.Code;
                    result.Message = dsResult.Message;
                    //return result;
                }
                dsData = dsResult.Data;

            }

            MethodReturnResult result1 = new MethodReturnResult();
            DataSet dsData1 = new DataSet();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> dsResult = client.GetERPOrderType(dsData.Tables[0].Rows[0]["vtrantypecode"].ToString());
                if (dsResult.Code > 0)
                {
                    result1.Code = dsResult.Code;
                    result1.Message = dsResult.Message;
                    //return result;
                }
                dsData1 = dsResult.Data;

            }

            msg = "";
            string returnCode = "";

            string ERPAccount = "";                     //ERP账套代码
            string ERPGroupCode = "";                   //ERP集团代码
            string ERPOrg = "";                         //ERP组织代码

            #region 根据ERP接口字符串取得ERP账套相关信息
            //取得ERP连接字符串
            string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];

            //创建WEB访问对象
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            //取得查询语句
            string ERPQuery = httpWebRequest.RequestUri.Query;

            //取得ERP账套代码
            ERPAccount = GetValueDataByString(ERPQuery, "account", "=", "&");
            ERPGroupCode = GetValueDataByString(ERPQuery, "groupcode", "=", "&");
            ERPOrg = GetValueDataByString(ERPQuery, "orgcode", "=", "&");

            #endregion

            DataTable dt = GetWorkOrder(materialReturn.OrderNumber);
            string strPreorderWord = materialReturn.OrderNumber;

            Hashtable htTables = new Hashtable();
            htTables.Add(materialReturn.OrderNumber, dt);

            if (dsData1.Tables[0].Rows.Count > 0)
            {
                string startTime = Convert.ToDateTime(materialReturn.EditTime).ToShortDateString() + " 00:00:00";
                string endTime = Convert.ToDateTime(materialReturn.EditTime).ToShortDateString() + " 23:59:59";

                XmlDocument xmlDoc = new XmlDocument();
                //创建类型声明节点  
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "GB2312", "");
                xmlDoc.AppendChild(node);
                //创建根节点  
                XmlElement root = xmlDoc.CreateElement("ufinterface");
                root.SetAttribute("receiver", ERPOrg);
                root.SetAttribute("sender", "mes");
                root.SetAttribute("roottag", "");
                root.SetAttribute("replace", "Y");
                root.SetAttribute("isexchange", "Y");
                root.SetAttribute("groupcode", ERPGroupCode);
                root.SetAttribute("filename", "");
                root.SetAttribute("billtype", "4D");
                root.SetAttribute("account", ERPAccount);
                xmlDoc.AppendChild(root);

                XmlElement Node = xmlDoc.CreateElement("bill");//创建节点ufinterface子节点bill   
                Node.SetAttribute("id", "");
                xmlDoc.DocumentElement.AppendChild(Node);

                XmlNode billheadNode = xmlDoc.CreateNode(XmlNodeType.Element, "billhead", null);
                Node.AppendChild(billheadNode);

                CreateNode(xmlDoc, billheadNode, "cgeneralhid", "");

                XmlElement cgeneralbidNode = xmlDoc.CreateElement("cgeneralbid");
                billheadNode.AppendChild(cgeneralbidNode);

                if (lstDetail.Tables[0].Rows.Count > 0)
                {

                    //t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.PACKAGE_NO ,t2.COLOR,t2.GRADE ,
                    //t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER
                    decimal numerator=0;
                    decimal denominator=0;

                    #region 创建xml
                    int i = 1;
                    foreach (var item in lstDetail.Tables[0].AsEnumerable())
                    {
                        //dt = GetWorkOrder(item["ORDER_NUMBER"].ToString().ToUpper());
                        //if(dt==null || dt.Rows.Count==0)
                        //{

                        //}
                        // string Effi = GetEffi(item["PACKAGE_NO"].ToString());
                        if (htTables.ContainsKey(item["ORDER_NUMBER"].ToString().ToUpper()))
                        {
                            dt = (DataTable)htTables[item["ORDER_NUMBER"].ToString().ToUpper()];
                        }
                        else
                        {
                            dt = GetWorkOrder(item["ORDER_NUMBER"].ToString().ToUpper());
                            if (dt == null || dt.Rows.Count == 0)
                            {
                                msg = string.Format("工单{0}在ERP中不存在。", item["ORDER_NUMBER"].ToString().ToUpper());
                                return "";
                            }
                            htTables.Add(item["ORDER_NUMBER"].ToString().ToUpper(), dt);
                        }

                        DataTable dt_WRDetail = GetDetailByName(item["MATERIAL_LOT"].ToString());

                        DataRow[] wrDetailRows = dt_WRDetail.Select(
                               string.Format(" vbatchcode = '{0}'"
                                              , item["MATERIAL_LOT"].ToString()));

                        if (wrDetailRows == null || wrDetailRows.Length == 0)
                        {
                            result.Code = 1001;
                            result.Message = string.Format("报工单{0}中工单{1}明细批号{2}在ERP中不存在", materialReturn.Key, item["ORDER_NUMBER"].ToString(), item["MATERIAL_LOT"].ToString());
                            //return result2;
                        }

                        DataTable dt_Unit = GetUnit(item["MATERIAL_CODE"].ToString());//得到主辅单位
                        DataTable dt_WorkStock = GetERPWorkStock(item["ORDER_NUMBER"].ToString());//得到备料号
                        DataTable dt_WorkStockInfo = GetERPWorkStockInfo(dt_WorkStock.Rows[0]["VBILLCODE"].ToString(), item["MATERIAL_CODE"].ToString());//得到备料计划信息
                        DataTable dt_WorkSourceId = GetWorkOrder(item["ORDER_NUMBER"].ToString());//得到工单相关信息

                        //得到转换率
                        string EffiCode = GetEffiByMaterialLot(item["MATERIAL_LOT"].ToString(), ref numerator, ref denominator);                        

                        XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        cgeneralbidNode.AppendChild(itemNode);

                        CreateNode(xmlDoc, itemNode, "cgeneralbid", "");
                        CreateNode(xmlDoc, itemNode, "crowno", (i++).ToString());
                        CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);
                        CreateNode(xmlDoc, itemNode, "corpoid", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "corpvid", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "cstateid", "");
                        CreateNode(xmlDoc, itemNode, "cmaterialoid", item["MATERIAL_CODE"].ToString());//物料编码
                        CreateNode(xmlDoc, itemNode, "cmaterialvid", item["MATERIAL_CODE"].ToString());//物料编码
                        CreateNode(xmlDoc, itemNode, "vfree1", GetCodeByName(wrDetailRows[0]["ZJGL"].ToString(), "JN0001"));//组件功率
                        CreateNode(xmlDoc, itemNode, "vfree2", GetCodeByName(wrDetailRows[0]["ZJDL"].ToString(), "JN0002"));//组件电流档
                        CreateNode(xmlDoc, itemNode, "vfree3", GetCodeByName(wrDetailRows[0]["ZJWGDJ"].ToString(), "JN0003"));//组件等级
                        CreateNode(xmlDoc, itemNode, "vfree4", GetCodeByName(wrDetailRows[0]["BATTRANSRATE"].ToString(),"JN0004"));//电池片效率
                        CreateNode(xmlDoc, itemNode, "vfree5", GetCodeByName(wrDetailRows[0]["BATCOLOR"].ToString(),"JN0005"));//电池片颜色
                        CreateNode(xmlDoc, itemNode, "vfree6", GetCodeByName(wrDetailRows[0]["BATLVL"].ToString(),"JN0006"));//电池片等级
                        CreateNode(xmlDoc, itemNode, "vfree7", "");
                        CreateNode(xmlDoc, itemNode, "cunitid", dt_Unit.Rows[0]["MEASCODE"].ToString());//主单位
                        CreateNode(xmlDoc, itemNode, "castunitid", dt_Unit.Rows[0]["ASTMEASCODE"].ToString());//单位
                        CreateNode(xmlDoc, itemNode, "vchangerate", EffiCode.ToString());//换算率
                        CreateNode(xmlDoc, itemNode, "vbatchcode", item["MATERIAL_LOT"].ToString());
                        CreateNode(xmlDoc, itemNode, "nshouldnum", "-" + item["QTY"].ToString());//应收主数量
                        //decimal mideffiCode = 0;
                        //if (!string.IsNullOrEmpty(EffiCode) && EffiCode.Contains('/'))
                        //{
                        //    mideffiCode = Convert.ToDecimal(EffiCode.Split('/')[0]) / Convert.ToDecimal(EffiCode.Split('/')[1]);
                        //}
                        CreateNode(xmlDoc, itemNode, "nshouldassistnum", "-" + (Convert.ToDecimal(item["Qty"].ToString()) * denominator / numerator).ToString());//应发辅数量
                        CreateNode(xmlDoc, itemNode, "nnum", "-" + item["QTY"].ToString());//主数量
                        CreateNode(xmlDoc, itemNode, "nassistnum", "-" + (Convert.ToDecimal(item["Qty"].ToString()) * denominator / numerator).ToString());//实发数量                        
                        CreateNode(xmlDoc, itemNode, "dbizdate", "");//出库日期
                        CreateNode(xmlDoc, itemNode, "vproductbatch", item["ORDER_NUMBER"].ToString());
                        CreateNode(xmlDoc, itemNode, "cpickmcode", dt_WorkStock.Rows[0]["VBILLCODE"].ToString());//备料计划单据号
                        CreateNode(xmlDoc, itemNode, "csourcebillhid", dt_WorkStockInfo.Rows[0]["CPICKMID"].ToString());//dt.Rows[0]["PK_WR"].ToString());//来源单据表头主键
                        CreateNode(xmlDoc, itemNode, "csourcebillbid", dt_WorkStockInfo.Rows[0]["CPICKM_BID"].ToString());// wrDetailRows[0]["PK_WR_QUALITY"] == null ? "" : wrDetailRows[0]["PK_WR_QUALITY"].ToString());
                        CreateNode(xmlDoc, itemNode, "csourcetype", "55A3");
                        CreateNode(xmlDoc, itemNode, "csourcebillbid", dt_WorkStockInfo.Rows[0]["CPICKM_BID"].ToString());// wrDetailRows[0]["PK_WR_QUALITY"] == null ? "" : wrDetailRows[0]["PK_WR_QUALITY"].ToString());
                        CreateNode(xmlDoc, itemNode, "vsourcerowno", dt_WorkStockInfo.Rows[0]["VROWNO"].ToString());//wrDetailRows[0]["PK_WR_PRODUCT"] == null ? "" : wrDetailRows[0]["PK_WR_PRODUCT"].ToString());
                        CreateNode(xmlDoc, itemNode, "cprojectid", "");
                        CreateNode(xmlDoc, itemNode, "casscustid", "");
                        CreateNode(xmlDoc, itemNode, "cfirsttype", "55C2");// wrDetailRows[0]["CBSRCTYPE"].ToString());
                        CreateNode(xmlDoc, itemNode, "cfirstbillhid", dt_WorkSourceId.Rows[0]["PK_DMO"].ToString());
                        CreateNode(xmlDoc, itemNode, "vfirstbillcode", item["ORDER_NUMBER"].ToString());
                        CreateNode(xmlDoc, itemNode, "vfirstrowno", "");
                        CreateNode(xmlDoc, itemNode, "cfirstbillbid", dt_WorkSourceId.Rows[0]["VFIRSTMOID"].ToString());// 源头单据行标识
                        CreateNode(xmlDoc, itemNode, "cvendorid", wrDetailRows[0]["SUPPLIERCODE"].ToString());//供应商
                        CreateNode(xmlDoc, itemNode, "cproductorid", wrDetailRows[0]["SUPPLIERCODE"].ToString());//生产厂商
                        CreateNode(xmlDoc, itemNode, "vnotebody", "");
                        CreateNode(xmlDoc, itemNode, "bbarcodeclose", "N");
                        CreateNode(xmlDoc, itemNode, "bonroadflag", "N");
                        CreateNode(xmlDoc, itemNode, "dproducedate", "");//生产日期
                        CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "cbodywarehouseid", model.Store.ToString());//仓库
                        CreateNode(xmlDoc, itemNode, "pk_batchcode", "");
                        CreateNode(xmlDoc, itemNode, "csrcmaterialoid", "");
                        CreateNode(xmlDoc, itemNode, "csrcmaterialvid", "");
                        CreateNode(xmlDoc, itemNode, "ccostobject", dt_WorkSourceId.Rows[0]["MATERIALCODE"].ToString());

                    }
                    #endregion
                }
                CreateNode(xmlDoc, billheadNode, "pk_group", ERPGroupCode);
                CreateNode(xmlDoc, billheadNode, "pk_org", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "pk_org_v", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "cdrawcalbodyvid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "cdrawcalbodyoid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "cwarehouseid", model.Store.ToString());//仓库
                CreateNode(xmlDoc, billheadNode, "cdptid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cdptvid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "ctrantypeid", "4D-01");
                CreateNode(xmlDoc, billheadNode, "vtrantypecode", "4D-01");
                CreateNode(xmlDoc, billheadNode, "vnote", "");
                CreateNode(xmlDoc, billheadNode, "cwhsmanagerid", "");
                CreateNode(xmlDoc, billheadNode, "cbizid", "");
                CreateNode(xmlDoc, billheadNode, "vnote", "");
                CreateNode(xmlDoc, billheadNode, "fbillflag", "2");
                CreateNode(xmlDoc, billheadNode, "creator", materialReturn.Creator.ToString());
                CreateNode(xmlDoc, billheadNode, "billmaker", materialReturn.Editor.ToString());
                CreateNode(xmlDoc, billheadNode, "creationtime", materialReturn.CreateTime.ToString());

                string path = Server.MapPath("~/XMLFile/");
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                path = path + materialReturn.Key + ".xml";
                xmlDoc.Save(path);

                FileStream ms = new FileStream(path, FileMode.Open, FileAccess.Read);

                //string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];
                HttpWebRequest loHttp = (HttpWebRequest)WebRequest.Create(url);  //URL为XChangeServlet地址

                loHttp.Method = "POST";
                // *** Set any header related and operational properties
                loHttp.Timeout = 10000;  // 10 secs
                loHttp.UserAgent = "Code Sample Web Client";

                // *** reuse cookies if available
                loHttp.CookieContainer = new CookieContainer();

                if (this.oCookies != null && this.oCookies.Count > 0)
                {
                    loHttp.CookieContainer.Add(this.oCookies);
                }

                #region send Xml To NCS
                LogHelper.WriteLogError("Begin Send Xml File");
                //loHttp.ContentLength = ms.Length;
                Stream requestStream = loHttp.GetRequestStream();

                byte[] buffer = new Byte[(int)ms.Length];
                int bytesRead = 0;
                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) != 0)
                    requestStream.Write(buffer, 0, bytesRead);
                //requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);  
                requestStream.Close();

                // *** Return the Response data
                HttpWebResponse loWebResponse = (HttpWebResponse)loHttp.GetResponse();

                if (loWebResponse.Cookies.Count > 0)
                    if (this.oCookies == null)
                    {
                        this.oCookies = loWebResponse.Cookies;
                    }
                    else
                    {
                        // ** If we already have cookies update the list
                        foreach (Cookie oRespCookie in loWebResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.oCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Name;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.oCookies.Add(oRespCookie);
                        }
                    }

                Encoding enc = Encoding.GetEncoding("gb2312");  // Windows-1252 or iso-
                if (loWebResponse.ContentEncoding.Length > 0)
                {
                    enc = Encoding.GetEncoding(loWebResponse.ContentEncoding);
                }

                StreamReader loResponseStream =
                    new StreamReader(loWebResponse.GetResponseStream());

                string ResponseText = loResponseStream.ReadToEnd();

                LogHelper.WriteLogError("End Send Xml File");
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(ResponseText);

                //获取ERP回执
                XmlNode xnode = Doc.SelectSingleNode("ufinterface/sendresult/content");
                returnCode = xnode.InnerText;

                //获取ERP错误信息提示
                if (returnCode == "")
                {
                    XmlNode errornode = Doc.SelectSingleNode("ufinterface/sendresult/resultdescription");
                    msg = errornode.InnerText;
                }

                loResponseStream.Close();
                loWebResponse.Close();
                ms.Close();
                requestStream.Close();
                #endregion
            }
            else
            {
                msg = string.Format(StringResource.ERPWorkOrderQuery_Error_Query, materialReturn.OrderNumber);
            }
            return returnCode;
        }


        public DataTable GetUnit(string MaterialCode)
        {
            DataTable dt = new DataTable();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetUnitByMaterialCode(MaterialCode);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }//得到单位


        public DataTable GetWorkOrder(string OrderNumber)
        {
            DataTable dt = new DataTable();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWorkOrder(OrderNumber);
                if (ds != null && ds.Data != null && ds.Data.Tables.Count > 0)
                {
                    dt = ds.Data.Tables[0];
                }
            }
            return dt;
        }//得到工单号

        public string GetCodeByName(string Name,string ListCode)//通过名字（效率、等级、花色等）得到在ERP中可以识别的名字
        {
            string Code = "";
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetCodeByName(Name, ListCode);
                if (ds.Code == 0 && ds.Data.Tables[0].Rows.Count > 0)
                {
                    Code = ds.Data.Tables[0].Rows[0]["CODE"].ToString();
                }
            }
            return Code;
        }

        public string GetEffiByMaterialLot(string materialLot, ref decimal numerator, ref decimal denominator)//根据物料批次得到转换率
        {
            string strEffi = "1";

            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                MethodReturnResult<DataSet> ds = client.GetEffiByMaterialLot(materialLot);
                if (ds.Code == 0 && ds.Data.Tables[0].Rows.Count > 0)
                {
                    strEffi = ds.Data.Tables[0].Rows[0]["vchangerate"].ToString();
                    numerator = decimal.Parse(strEffi.Substring(0,strEffi.IndexOf("/")));

                    denominator = decimal.Parse(strEffi.Substring(strEffi.IndexOf("/") + 1));
                }
            }

            return strEffi;
        }


        public DataTable GetDetailByName(string LotNo)//根据物料批次得到颜色、效率、等级等信息
        {
            //string Code = "";
            DataTable dt = new DataTable();
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPMaterialReceiptDetail(LotNo);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        //public DataTable GetERPWorkStock(string OrderNumber,string MATERIALCODE)
        //{
        //    DataTable dt = new DataTable();
        //    using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
        //    {
        //        MethodReturnResult<DataSet> ds = client.GetERPWorkStock(OrderNumber, MATERIALCODE);

        //        dt = ds.Data.Tables[0];
        //    }
        //    return dt;
        //}//根据工单号查询备料单号


        public DataTable GetERPWorkStock(string OrderNumber)
        {
            DataTable dt = new DataTable();
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWorkStock(OrderNumber);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }//根据工单号查询备料单号


        public DataTable GetERPWorkStockInfo(string BLNumber, string MATERIALCODE)//根据备料号查询来源单据相关信息
        {
            DataTable dt = new DataTable();
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWorkStockInfo(BLNumber, MATERIALCODE);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }


        public DataTable GetMaterialInfo(string MaterialCode)//根据物料编码得到物料名称，规格等信息
        {
            DataTable dt = new DataTable();
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                MethodReturnResult<DataSet> ds = client.GetMaterialInfo(MaterialCode);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }


        public DataTable GetStoreName(string Store)//根据物料编码得到物料名称，规格等信息
        {
            DataTable dt = new DataTable();
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                MethodReturnResult<DataSet> ds = client.GetStoreName(Store);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }




        private CookieCollection _oCookies = null;
        protected CookieCollection oCookies
        {
            get
            {
                return _oCookies;
            }
            set
            {
                _oCookies = value;
            }

        }



        public ActionResult Print(MaterialReturnDetailQueryViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.ReturnNo))
                {
                    return Content(string.Empty);
                }
                return ShowStgInReport(model.ReturnNo);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [AllowAnonymous]
        public ActionResult ShowStgInReport(string ReturnNo)//得到打印单据上显示的数据
        {
            MethodReturnResult result = new MethodReturnResult();
            DataSet dsData = new DataSet();
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                MethodReturnResult<DataSet> dsResult = client.GetReturnReportFromDB(ReturnNo);
                if (dsResult.Code > 0)
                {
                    result.Code = dsResult.Code;
                    result.Message = dsResult.Message;
                    //return result;
                }
                dsData = dsResult.Data;
            }
            DataTable dtStgIn = dsData.Tables["StgIn"];

            PackageListDataSet ds = new PackageListDataSet();
            PackageListDataSet.StgInRow row = ds.StgIn.NewStgInRow();
            if (dtStgIn.Rows.Count > 0)
            {
                DataTable dtERP = new DataTable();
                using (MaterialReturnServiceClient clientERP = new MaterialReturnServiceClient())
                {
                    MethodReturnResult<DataSet> dsERP = clientERP.GetERPReportCodeById(dtStgIn.Rows[0]["ERP_RETURN_CODE"] == null ? "" : dtStgIn.Rows[0]["ERP_RETURN_CODE"].ToString());
                    if (dsERP.Code > 0)
                    {
                        result.Code = dsERP.Code;
                        result.Message = dsERP.Message;
                        //return result;
                    }
                    else
                    {
                        dtERP = dsERP.Data.Tables[0];
                    }

                }
                if (dtERP.Rows.Count > 0)
                {
                    row.ERPBillCode = dtERP.Rows[0]["VBILLCODE"].ToString();//出库单号
                    //row.ERPBillCode = "1CC-16040001";//出库单号
                    DataTable dt = GetWorkOrder(dtStgIn.Rows[0]["ORDER_NUMBER"].ToString());//得到备料号
                    row.Cdptid = dt.Rows[0]["DEPTCODE"].ToString();//领料部门

                    //row.Cdptid = "k01377";//领料部门
                    DataTable dt_WorkStock = GetERPWorkStock(dtStgIn.Rows[0]["ORDER_NUMBER"].ToString());
                    row.BillCode = dt_WorkStock.Rows[0]["VBILLCODE"].ToString();//备料单号
                    //row.BillCode = "1BL-16040001";
                    row.Type = dtERP.Rows[0]["TRANTYPENAME"].ToString();
                }

                row.BillDate = dtStgIn.Rows[0]["RETURN_DATE"].ToString();
                row.BillMaker = dtStgIn.Rows[0]["CREATOR"].ToString();
                row.OrderNumber = dtStgIn.Rows[0]["ORDER_NUMBER"].ToString();
                DataTable dt_StoreName = GetStoreName(dtStgIn.Rows[0]["STORE"].ToString());
                row.Store = dt_StoreName.Rows[0]["STORNAME"].ToString();
                row.Note = dtStgIn.Rows[0]["DESCRIPTION"].ToString();
                ds.StgIn.AddStgInRow(row);
            }


            if (dsData.Tables.Contains("StgInDetail"))
            {
                decimal numerator = 0;
                decimal denominator = 0;
                double dQty = 0;
                DataTable dtStgInDetail = dsData.Tables["StgInDetail"];
                PackageListDataSet.StgInDetailRow rowDetail = ds.StgInDetail.NewStgInDetailRow();
                for (int i = 0; i < dtStgInDetail.Rows.Count; i++)
                {


                    rowDetail = ds.StgInDetail.NewStgInDetailRow();
                    rowDetail.ItemNo = (i + 1).ToString();
                    rowDetail.MaterialCode = dtStgInDetail.Rows[i]["MATERIAL_CODE"].ToString();
                    DataTable dt_MaterialInfo = GetMaterialInfo(dtStgInDetail.Rows[i]["MATERIAL_CODE"].ToString());//得到物料名称、规格信息
                    rowDetail.MaterialName = null;
                    rowDetail.MaterialName = dt_MaterialInfo.Rows[0]["INVNAME"].ToString().Length > 0 ? dt_MaterialInfo.Rows[0]["INVNAME"].ToString() : string.Empty; //存放物料名称
                    //rowDetail.MaterialName = "玻璃";
                    //rowDetail.MaterialStandard = dt_MaterialInfo.Rows[i]["MATERIALSPEC"].ToString();
                    rowDetail.MaterialStandard = null;
                    rowDetail.MaterialStandard = dt_MaterialInfo.Rows[0]["MATERIALSPEC"].ToString().Length > 0 ? dt_MaterialInfo.Rows[0]["MATERIALSPEC"].ToString() : string.Empty; //存放物料规格
                    DataTable dt_Unit = GetUnit(dtStgInDetail.Rows[0]["MATERIAL_CODE"].ToString());//根据物料编码得到主辅单位
                    rowDetail.PrimaryUnit = dt_Unit.Rows[0]["MEASNAME"].ToString();//主单位
                    rowDetail.AuxiliaryUnit = dt_Unit.Rows[0]["ASTMEASNAME"].ToString();//辅单位
                    string EffiCode = GetEffiByMaterialLot(dtStgInDetail.Rows[i]["MATERIAL_LOT"].ToString(), ref numerator, ref denominator);//得到转换率
                    rowDetail.ChangeRate = EffiCode.ToString();
                    rowDetail.ObjectNumber = dtStgInDetail.Rows[i]["MATERIAL_LOT"].ToString();//物料批号
                    if (Double.TryParse(dtStgInDetail.Rows[i]["QTY"].ToString(), out dQty) == false)
                    {
                        dQty = 0;
                    }
                    rowDetail.Qty = dQty;
                    //rowDetail.Qty = 0;
                    ds.StgInDetail.AddStgInDetailRow(rowDetail);
                }

            }


            //
            using (LocalReport localReport = new LocalReport())
            {
                localReport.ReportPath = Server.MapPath("~/RDLC/ReturnInList.rdlc");

                ReportDataSource reportDataSourcePackage = new ReportDataSource("StgIn", ds.Tables[ds.StgIn.TableName]);
                localReport.DataSources.Add(reportDataSourcePackage);
                ReportDataSource reportDataSourcePackageDetail = new ReportDataSource("StgInDetail", ds.Tables[ds.StgInDetail.TableName]);
                localReport.DataSources.Add(reportDataSourcePackageDetail);
                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;
                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                string deviceInfo =
                                "<DeviceInfo>" +
                                "  <OutputFormat>PDF</OutputFormat>" +
                                "  <PageWidth>24cm</PageWidth>" +
                                "  <PageHeight>14cm</PageHeight>" +
                                "  <MarginTop>0.5cm</MarginTop>" +
                                "  <MarginLeft>0.5cm</MarginLeft>" +
                                "  <MarginRight>0cm</MarginRight>" +
                                "  <MarginBottom>0.5cm</MarginBottom>" +
                                "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                //Render the report
                renderedBytes = localReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);
                //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
                return File(renderedBytes, mimeType);
            }

        }




        #endregion
    }
}