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
        /// <summary> 初始化查询界面 </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" OrderState = '{0}'"
                                              , EnumWorkOrderState.Open.GetHashCode()),
                        OrderBy = "EditTime DESC"
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

        /// <summary> 查询 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderQueryViewModel model)
        {
            //if (ModelState.IsValid)
            //{
                using (WorkOrderServiceClient client = new WorkOrderServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            //工单号
                            if (!string.IsNullOrEmpty(model.OrderNumber))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);
                            }

                            //状态
                            if (model.OrderState != "" && model.OrderState != null)
                            {
                                where.AppendFormat(" {0} OrderState = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderState);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "EditTime DESC",
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
            //}
                return PartialView("_ListPartial", new WorkOrderViewModel());
        }

        /// <summary> 分页查询 </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="currentPageNo"></param>
        /// <param name="currentPageSize"></param>
        /// <returns></returns>
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

            WorkOrderViewModel model = new WorkOrderViewModel();

            return PartialView("_ListPartial", model);
        }

        /// <summary> 新增工单信息 </summary>
        /// <param name="model">工单模型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try
            {
                using (WorkOrderServiceClient client = new WorkOrderServiceClient())
                {
                    //创建工单对象
                    WorkOrder obj  = new WorkOrder()
                    {
                        Key = model.OrderNumber.ToUpper(),              //工单号
                        MaterialCode = model.MaterialCode.ToUpper(),    //产品代码
                        OrderQuantity = model.OrderQuantity,            //生产数量
                        OrderType = model.OrderType,                    //工单类型                        
                        LocationName = model.LocationName.ToUpper(),    //生产车间
                        PlanStartTime = model.PlanStartTime,            //计划开工日期
                        PlanFinishTime = model.PlanFinishTime,          //计划完工日期
                        StartTime = model.PlanStartTime,                //开始时间
                        FinishTime = model.PlanFinishTime,              //完工时间

                        LeftQuantity = model.OrderQuantity,             //剩余数量
                        FinishQuantity = 0,                             //完工数量
                        ReworkQuantity = 0,                             //返工数量
                        RepairQuantity = 0,                             //返修数量
                        CloseType = model.CloseType,                    //关闭状态
                        OrderState = model.OrderState,                  //工单状态      
                        Priority = model.Priority,                      //优先级
                        
                        Description = model.Description,                //说明
                        CreateTime = DateTime.Now,                      //创建时间
                        Creator = User.Identity.Name,                   //创建人
                        EditTime = DateTime.Now,                        //编辑时间
                        Editor = User.Identity.Name                     //编辑人
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

        /// <summary> 修改信息查询 </summary>
        /// <param name="key">工单号</param>
        /// <returns></returns>
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
                        PlanStartTime = result.Data.PlanStartTime,
                        PlanFinishTime = result.Data.PlanFinishTime,
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

        /// <summary>
        /// 修改信息保存
        /// </summary>
        /// <param name="model">工单模型</param>
        /// <returns></returns>
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
                        //判断工单数量是否满足已投批数量
                        if (result.Data.LeftQuantity + (model.OrderQuantity - result.Data.OrderQuantity) < 0)
                        {
                            rst.Code = 1000;
                            rst.Message = string.Format("工单数量（{0}）小于已创批数量（{1}）！",
                                                        model.OrderQuantity,
                                                        result.Data.OrderQuantity - result.Data.LeftQuantity);
                            rst.Detail = rst.Message;

                            return Json(rst);
                        }

                        result.Data.PlanStartTime = model.PlanStartTime;        //计划开工日期
                        result.Data.PlanFinishTime = model.PlanFinishTime;      //计划完工日期
                        result.Data.LeftQuantity = result.Data.LeftQuantity + (model.OrderQuantity - result.Data.OrderQuantity);    //剩余数量
                        result.Data.OrderQuantity = model.OrderQuantity;        //工单数量
                        result.Data.OrderType = model.OrderType;                //工单类型

                        result.Data.StartTime = model.PlanStartTime;            //开工时间
                        result.Data.FinishTime = model.PlanFinishTime;          //完工时间
                        result.Data.Description = model.Description;            //描述

                        result.Data.Editor = User.Identity.Name;                //编辑人
                        result.Data.EditTime = DateTime.Now;                    //编辑日期
                        result.Data.CloseType = model.CloseType;                //工单关闭类型
                        result.Data.OrderState = model.OrderState;              //工单状态          

                        //result.Data.Description = model.Description;
                        //result.Data.FinishTime = model.FinishTime;
                        //result.Data.LocationName = model.LocationName.ToUpper();
                        //result.Data.MaterialCode = model.MaterialCode.ToUpper();
                        //result.Data.LeftQuantity = result.Data.LeftQuantity + (model.OrderQuantity - result.Data.OrderQuantity);
                        //result.Data.OrderQuantity = model.OrderQuantity;
                        //result.Data.OrderState = model.OrderState;
                        //result.Data.OrderType = model.OrderType;
                        //result.Data.Priority = model.Priority;
                        //result.Data.RevenueType = model.RevenueType;
                        //result.Data.StartTime = model.StartTime;
                        //result.Data.Description = model.Description;
                        //result.Data.Editor = User.Identity.Name;
                        //result.Data.EditTime = DateTime.Now;

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

        /// <summary>
        /// 工单删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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
            string prefix = string.Format("1MO-{0:yyMM}", DateTime.Now);
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