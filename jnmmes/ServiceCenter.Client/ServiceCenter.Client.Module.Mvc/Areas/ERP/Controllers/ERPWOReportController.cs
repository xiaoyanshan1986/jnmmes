using ServiceCenter.Client.Mvc.Areas.ERP.Models;
using ServiceCenter.MES.Model.ERP;
using ServiceCenter.MES.Service.Client.ERP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Resources.ERP;
using System.Text;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Service.Client.LSM;
using System.Threading.Tasks;

namespace ServiceCenter.Client.Mvc.Areas.ERP.Controllers
{
    public class ERPWOReportController : Controller
    {
        #region 产成品及在制品入库申请表头相关操作
        // GET: ERP/ERPWOReport
        public ActionResult Index()
        {
            return View(new WOReportQueryViewModel() 
                        { 
                            BillState = EnumBillState.Create.GetHashCode().ToString()
                        });
        }

        //入库申请界面查询-非报废单据
        public ActionResult Query(WOReportQueryViewModel model)
        {            
            using (WOReportClient client = new WOReportClient())
            {
                StringBuilder where = new StringBuilder();

                where.AppendFormat("BillType != 1 ");
               
                //入库单号
                if (!string.IsNullOrEmpty(model.BillCode))
                {
                    where.AppendFormat(" {0} Key LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BillCode);
                }

                //入库类型
                if (model.BillType != null && model.BillType != "")
                {
                    where.AppendFormat(" {0} BillType = {1}"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BillType);
                }

                //状态
                if ( model.BillState != null && model.BillState != "" )
                {
                    where.AppendFormat(" {0} BillState = {1}"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BillState);
                }
                
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "CreateTime desc",
                    Where = where.ToString()
                };

                MethodReturnResult<IList<WOReport>> result = client.GetWOReport(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }
            
            return PartialView("_ListPartial");
        }

        //新增产成品或在制品入库申请单
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(WOReportViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                if (string.IsNullOrWhiteSpace(model.BillCode))
                {
                    string BillCode = string.Format("INC{0:yyMMdd}", DateTime.Now);
                    int itemNo = 0;

                    using (WOReportClient client = new WOReportClient())
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key LIKE '{0}%'"
                                                    , BillCode),
                            OrderBy = "Key Desc"
                        };

                        MethodReturnResult<IList<WOReport>> rst = client.GetWOReport(ref cfg);

                        if (rst.Code <= 0 && rst.Data.Count > 0)
                        {
                            string maxBillNo = rst.Data[0].Key.Replace(BillCode, "");
                            int.TryParse(maxBillNo, out itemNo);
                        }

                        itemNo++;

                        model.BillCode = BillCode + itemNo.ToString("000");
                    }
                }

                using (WOReportClient client = new WOReportClient())
                {
                    WOReport woReport = new WOReport()
                    {
                        Key = model.BillCode,                       //入库单号
                        BillType = model.BillType,                  //入库类型
                        BillDate = model.BillDate,
                        BillMaker = User.Identity.Name,
                        BillMakedDate = model.BillMakedDate,
                        MixType = model.MixType,
                        ScrapType = ServiceCenter.MES.Model.ERP.EnumScrapType.False,
                        OrderNumber = model.OrderNumber,
                        MaterialCode = model.MaterialCode,
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name,
                        Note = model.Note,
                    };

                    result = client.AddWOReport(woReport);

                    if (result.Code == 0)
                    {
                        result.Message = string.Format(StringResource.WOReport_Save_Success, model.BillCode);
                        result.Detail = woReport.Key;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return Json(result);
        }

        //修改产成品或在制品入库单数据
        public ActionResult Modify(string key)
        {
            WOReportViewModel model = new WOReportViewModel();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<WOReport> result = client.GetWOReport(key);
                if (result.Code == 0)
                {
                    model = new WOReportViewModel()
                    {
                        BillCode = result.Data.Key,
                        BillType = result.Data.BillType,                  //入库类型
                        BillDate = result.Data.BillDate,
                        BillMakedDate = result.Data.BillMakedDate,
                        MixType = result.Data.MixType,
                        OrderNumber = result.Data.OrderNumber,
                        MaterialCode = result.Data.MaterialCode,
                        WRCode = result.Data.WRCode,
                        ERPWRKey = result.Data.ERPWRKey,
                        Creator = result.Data.Creator,
                        CreateTime = result.Data.CreateTime,
                        Editor = User.Identity.Name,
                        Note = result.Data.Note
                    };

                    return PartialView("_ModifyPartial", model);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            return PartialView("_ModifyPartial");
        }

        //保存产成品或在制品入库单据修改
        [HttpPost]
        public ActionResult SaveModify(WOReportViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WOReportClient client = new WOReportClient())
            {
                WOReport woReport = new WOReport()
                {
                    Key = model.BillCode,
                    BillType = model.BillType,                  //入库类型
                    BillDate = model.BillDate,
                    MixType = model.MixType,
                    OrderNumber = model.OrderNumber,
                    MaterialCode = model.MaterialCode,
                    WRCode = model.WRCode,
                    ERPWRKey = model.ERPWRKey,
                    Creator = model.Creator,
                    CreateTime = model.CreateTime,
                    Editor = User.Identity.Name,
                    Note = model.Note,
                };

                result = client.EditWOReport(woReport);

                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.WOReport_Edit_Success, model.BillCode);
                }

            }
            return Json(result);
        }

        /// <summary>
        /// 删除未申请的产成品或在制品入库申请单
        /// </summary>
        /// <param name="key">产成品或在制品入库申请单号</param>
        /// <param name="ScrapType">无用参数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string key,string ScrapType)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WOReportClient client = new WOReportClient())
            {
                WOReport woReport = new WOReport()
                {
                    Key =  key,                         //入库单号
                    Creator = User.Identity.Name,       //编辑人
                    Editor = User.Identity.Name,        //修改人
                };

                result = client.DeleteWOReport(woReport, key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.WOReport_Delete_Success, key);
                }
            }

            return Json(result);
        }

        //获取未关闭的工单列表
        public ActionResult GetOrderNumbers()
        {
            IList<WorkOrder> lstWorkOrder = new List<WorkOrder>();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"CloseType='{0}' "
                                           , Convert.ToInt32(EnumCloseType.None))
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lstWorkOrder = result.Data;
                }
            }
            return Json(from item in lstWorkOrder
                        select new
                        {
                            Text = item.Key,
                            Value = item.Key
                        }, JsonRequestBehavior.AllowGet);
        }

        //根据工单号获取工单产品编码
        public ActionResult GetMaterialCodes(string orderNumber)
        {
            List<string> lstMaterial = new List<string>();

            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = client.Get(orderNumber);
                if (result.Code <= 0 && result.Data != null)
                {
                    string materialCode = result.Data.MaterialCode;
                    lstMaterial.Add(materialCode);
                }
            }
            var lnq = from item in lstMaterial
                      select item;

            return Json(lnq.Distinct(), JsonRequestBehavior.AllowGet);
        }

        //产成品或在制品入库申请单界面分页查询
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

                using (WOReportClient client = new WOReportClient())
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
                        MethodReturnResult<IList<WOReport>> result = client.GetWOReport(ref cfg);
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

        #endregion

        #region 报废入库申请表头相关操作

        //报废入库申请界面初始化
        public ActionResult sIndex()
        {
            return View(new WOReportQueryViewModel()
            {
                BillState = EnumBillState.Create.GetHashCode().ToString(),
                BillType = EnumStockInType.Scrap.GetHashCode().ToString()
            });
        }

        //报废入库申请单查询
        public ActionResult sQuery(WOReportQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WOReportClient client = new WOReportClient())
                {
                    StringBuilder where = new StringBuilder();
                    if (model != null)
                    {
                        where.AppendFormat("ScrapType ='1'");

                        //入库单号
                        if (!string.IsNullOrEmpty(model.BillCode))
                        {
                            where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.BillCode);
                        }

                        //入库类型
                        if (model.BillType != null && model.BillType != "")
                        {
                            where.AppendFormat(" {0} BillType = {1}"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.BillType);
                        }

                        //状态
                        if (model.BillState != null && model.BillState != "")
                        {
                            where.AppendFormat(" {0} BillState = {1}"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.BillState);
                        }
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "CreateTime desc",
                        Where = where.ToString()
                    };

                    MethodReturnResult<IList<WOReport>> result = client.GetWOReport(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                }
            }
            return PartialView("_sListPartial");
        }

        //报废入库申请单新增
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sSave(WOReportViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (string.IsNullOrWhiteSpace(model.BillCode))
                {
                    string BillCode = string.Format("INC{0:yyMMdd}", DateTime.Now);
                    int itemNo = 0;
                    using (WOReportClient client = new WOReportClient())
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format("Key LIKE '{0}%'"
                                                    , BillCode),
                            OrderBy = "Key Desc"
                        };
                        MethodReturnResult<IList<WOReport>> rst = client.GetWOReport(ref cfg);
                        if (rst.Code <= 0 && rst.Data.Count > 0)
                        {
                            string maxBillNo = rst.Data[0].Key.Replace(BillCode, "");
                            int.TryParse(maxBillNo, out itemNo);
                        }
                        itemNo++;

                        model.BillCode = BillCode + itemNo.ToString("000");
                    }
                    model.BillCode = model.BillCode.ToUpper();
                }

                using (WOReportClient client = new WOReportClient())
                {
                    WOReport woReport = new WOReport()
                    {
                        Key = model.BillCode,
                        BillDate = model.BillDate,
                        BillType = model.BillType,
                        BillMaker = User.Identity.Name,
                        BillMakedDate = model.BillMakedDate,
                        MixType = model.MixType,
                        ScrapType = ServiceCenter.MES.Model.ERP.EnumScrapType.True,
                        OrderNumber = model.OrderNumber,
                        MaterialCode = model.MaterialCode,
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name,
                        Note = model.Note,
                    };
                    result = client.AddWOReport(woReport);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format(StringResource.WOReport_Save_Success, model.BillCode);
                        result.Detail = woReport.Key;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return Json(result);
        }

        //报废入库申请单修改
        public ActionResult sModify(string key)
        {
            WOReportViewModel model = new WOReportViewModel();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<WOReport> result = client.GetWOReport(key);
                if (result.Code == 0)
                {
                    model = new WOReportViewModel()
                    {
                        BillCode = result.Data.Key,
                        BillDate = result.Data.BillDate,
                        BillMakedDate = result.Data.BillMakedDate,
                        MixType = result.Data.MixType,
                        OrderNumber = result.Data.OrderNumber,
                        MaterialCode = result.Data.MaterialCode,
                        Creator = result.Data.Creator,
                        Editor = User.Identity.Name,
                        Note = result.Data.Note
                    };
                    return PartialView("_sModifyPartial", model);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            return PartialView("_sModifyPartial");
        }

        //保存报废入库申请单修改
        [HttpPost]
        public ActionResult sSaveModify(WOReportViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WOReportClient client = new WOReportClient())
            {
                WOReport woReport = new WOReport()
                {
                    Key = model.BillCode,
                    BillDate = model.BillDate,
                    MixType = model.MixType,
                    OrderNumber = model.OrderNumber,
                    MaterialCode = model.MaterialCode,
                    Creator = model.Creator,
                    Editor = User.Identity.Name,
                    Note = model.Note,
                };

                result = client.EditWOReport(woReport);

                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.WOReport_Edit_Success, model.BillCode);
                }

            }
            return Json(result);
        }

        /// <summary>
        /// 删除未申请的报废入库申请单
        /// </summary>
        /// <param name="key">报废入库申请单号</param>
        /// <param name="ScrapType">无用参数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult sDelete(string key,string ScrapType)
        {
            MethodReturnResult result = new MethodReturnResult();

            using (WOReportClient client = new WOReportClient())
            {
                //创建入库单Model传递参数
                WOReport woReport = new WOReport()
                {
                    Key = key,                         //入库单号
                    Creator = User.Identity.Name,       //编辑人
                    Editor = User.Identity.Name,        //修改人
                };

                result = client.DeleteWOReport(woReport, key);

                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.WOReport_Delete_Success, key);
                }
            }

            return Json(result);
        }

        //获取未关闭的工单列表
        public ActionResult sGetOrderNumbers()
        {
            IList<WorkOrder> lstWorkOrder = new List<WorkOrder>();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"CloseType='{0}' "
                                           , Convert.ToInt32(EnumCloseType.None))
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lstWorkOrder = result.Data;
                }
            }
            return Json(from item in lstWorkOrder
                        select new
                        {
                            Text = item.Key,
                            Value = item.Key
                        }, JsonRequestBehavior.AllowGet);
        }

        //根据工单获取工单产品编码
        public ActionResult sGetMaterialCodes(string orderNumber)
        {
            //根据物料类型获取物料。
            //IList<WorkOrder> lst = new List<WorkOrder>();
            List<string> lstMaterial = new List<string>();

            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = client.Get(orderNumber);
                if (result.Code <= 0 && result.Data != null)
                {
                    string materialCode = result.Data.MaterialCode;
                    lstMaterial.Add(materialCode);
                }
            }
            var lnq = from item in lstMaterial
                      select item;

            return Json(lnq.Distinct(), JsonRequestBehavior.AllowGet);
        }

        //报废入库申请单分页查询
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> sPagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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

                using (WOReportClient client = new WOReportClient())
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
                        MethodReturnResult<IList<WOReport>> result = client.GetWOReport(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_sListPartial");
        }

        #endregion
    }
}