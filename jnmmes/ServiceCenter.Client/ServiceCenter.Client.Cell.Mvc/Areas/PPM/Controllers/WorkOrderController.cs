using ServiceCenter.Client.Mvc.Areas.PPM.Models;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Controllers
{
    public class WorkOrderController : Controller
    {
        //
        // GET: /PPM/WorkOrder/
        public async Task<ActionResult> Index()
        {
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderQueryViewModel());
        }
        //
        //POST: /PPM/WorkOrder/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderServiceClient client = new WorkOrderServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.OrderNumber))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);

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
        //POST: /PPM/WorkOrder/PagingQuery
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

                using (WorkOrderServiceClient client = new WorkOrderServiceClient())
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
                        MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
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
        // POST: /PPM/WorkOrder/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try
            {
                using (WorkOrderServiceClient client = new WorkOrderServiceClient())
                {
                    WorkOrder obj  = new WorkOrder()
                    {
                        CloseType = model.CloseType,
                        CreateTime = DateTime.Now,
                        Creator = User.Identity.Name,
                        Description = model.Description,
                        Editor = User.Identity.Name,
                        MaterialCode = model.MaterialCode.ToUpper(),
                        EditTime = DateTime.Now,
                        Key = model.OrderNumber.ToUpper(),
                        FinishQuantity = model.FinishQuantity,
                        FinishTime = model.FinishTime,
                        LocationName = model.LocationName.ToUpper(),
                        OrderQuantity = model.OrderQuantity,
                        OrderState = model.OrderState,
                        OrderType = model.OrderType,
                        Priority = model.Priority,
                        RevenueType = model.RevenueType,
                        StartTime = model.StartTime
                    };
                    rst = await client.AddAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(PPMResources.StringResource.WorkOrder_Save_Success
                                                    , model.OrderNumber);
                    }
                }
            }
            catch(Exception ex)
            {
                rst.Code = 1000;
                rst.Message = ex.Message;
                rst.Detail = ex.ToString();
            }
            return Json(rst);
        }
        //
        // GET: /PPM/WorkOrder/Modify
        public async Task<ActionResult> Modify(string key)
        {
            WorkOrderViewModel viewModel = new WorkOrderViewModel();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderViewModel()
                    {
                        CloseType = result.Data.CloseType,
                        FinishQuantity = result.Data.FinishQuantity,
                        FinishTime = result.Data.FinishTime,
                        LeftQuantity = result.Data.LeftQuantity,
                        MaterialCode = result.Data.MaterialCode,
                        OrderNumber = result.Data.Key,
                        OrderQuantity = result.Data.OrderQuantity,
                        OrderState = result.Data.OrderState,
                        OrderType = result.Data.OrderType,
                        Priority = result.Data.Priority,
                        RepairQuantity = result.Data.RepairQuantity,
                        RevenueType = result.Data.RevenueType,
                        ReworkQuantity = result.Data.ReworkQuantity,
                        ScrapQuantity = result.Data.ScrapQuantity,
                        StartTime = result.Data.StartTime,
                        WIPQuantity = result.Data.WIPQuantity,
                        LocationName = result.Data.LocationName,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_ModifyPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_ModifyPartial");
        }

        //
        // POST: /PPM/WorkOrder/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try
            {
                using (WorkOrderServiceClient client = new WorkOrderServiceClient())
                {
                    MethodReturnResult<WorkOrder> result = await client.GetAsync(model.OrderNumber);
                    if (result.Code == 0 && result.Data != null)
                    {
                        result.Data.CloseType = model.CloseType;
                        result.Data.Description = model.Description;
                        result.Data.FinishTime = model.FinishTime;
                        result.Data.LocationName = model.LocationName.ToUpper();
                        result.Data.MaterialCode = model.MaterialCode.ToUpper();
                        result.Data.LeftQuantity = result.Data.LeftQuantity + (model.OrderQuantity - result.Data.OrderQuantity);
                        result.Data.OrderQuantity = model.OrderQuantity;
                        result.Data.OrderState = model.OrderState;
                        result.Data.OrderType = model.OrderType;
                        result.Data.Priority = model.Priority;
                        result.Data.RevenueType = model.RevenueType;
                        result.Data.StartTime = model.StartTime;
                        result.Data.Description = model.Description;
                        result.Data.Editor = User.Identity.Name;
                        result.Data.EditTime = DateTime.Now;

                        rst = await client.ModifyAsync(result.Data);
                        if (rst.Code == 0)
                        {
                            rst.Message = string.Format(PPMResources.StringResource.WorkOrder_Save_Success
                                                        , model.OrderNumber);
                        }
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
        //
        // GET: /PPM/WorkOrder/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    WorkOrderViewModel viewModel = new WorkOrderViewModel()
                    {
                        CloseType = result.Data.CloseType,
                        FinishQuantity = result.Data.FinishQuantity,
                        FinishTime = result.Data.FinishTime,
                        LeftQuantity = result.Data.LeftQuantity,
                        MaterialCode = result.Data.MaterialCode,
                        OrderNumber = result.Data.Key,
                        OrderQuantity = result.Data.OrderQuantity,
                        OrderState = result.Data.OrderState,
                        OrderType = result.Data.OrderType,
                        Priority = result.Data.Priority,
                        RepairQuantity = result.Data.RepairQuantity,
                        RevenueType = result.Data.RevenueType,
                        ReworkQuantity = result.Data.ReworkQuantity,
                        ScrapQuantity = result.Data.ScrapQuantity,
                        StartTime = result.Data.StartTime,
                        WIPQuantity = result.Data.WIPQuantity,
                        LocationName = result.Data.LocationName,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_InfoPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_InfoPartial");
        }
        //
        // POST: /PPM/WorkOrder/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(PPMResources.StringResource.WorkOrder_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }

        public ActionResult GetRawMaterialCode(string q)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key LIKE '{0}%' AND IsRaw='1' AND Status='1'", q)
                };


                MethodReturnResult<IList<Material>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return Json(from item in result.Data
                                select new
                                {
                                    @label = string.Format("{0}[{1}]", item.Key, item.Name),
                                    @value = item.Key
                                }, JsonRequestBehavior.AllowGet); ;
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet); ;
        }

        public ActionResult GetProductMaterialCode(string q)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key LIKE '{0}%' AND IsProduct='1' AND Status='1'", q)
                };


                MethodReturnResult<IList<Material>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return Json(from item in result.Data
                                select new
                                {
                                    @label = string.Format("{0}[{1}]",item.Key,item.Name),
                                    @value = item.Key
                                }, JsonRequestBehavior.AllowGet); ;
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet); ;
        }

        public ActionResult GetWorkOrderNo()
        {
            string prefix = string.Format("1CO-{0:yyMM}", DateTime.Now);
            int itemNo = 0;
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key LIKE '{0}%'", prefix),
                    OrderBy = "Key Desc"
                };
                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    string sItemNo = result.Data[0].Key.Replace(prefix, "");
                    int.TryParse(sItemNo, out itemNo);
                }
            }
            return Json(prefix + (itemNo+1).ToString("0000"), JsonRequestBehavior.AllowGet);
        }
    }
}