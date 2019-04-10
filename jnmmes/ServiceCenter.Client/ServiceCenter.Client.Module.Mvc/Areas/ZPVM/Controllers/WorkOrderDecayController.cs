using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class WorkOrderDecayController : Controller
    {
        //
        // GET: /ZPVM/WorkOrderDecay/
        public async Task<ActionResult> Index(string OrderNumber, string MaterialCode)
        {
            if (string.IsNullOrEmpty(OrderNumber) || string.IsNullOrEmpty(MaterialCode))
            {
                return RedirectToAction("Index", "WorkOrderRule");
            }

            using (WorkOrderRuleServiceClient client = new WorkOrderRuleServiceClient())
            {
                MethodReturnResult<WorkOrderRule> result = await client.GetAsync(new WorkOrderRuleKey()
                {
                   OrderNumber=OrderNumber,
                   MaterialCode=MaterialCode
                });

                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "WorkOrderRule");
                }
                ViewBag.Rule = result.Data;
            }

            using (WorkOrderDecayServiceClient client = new WorkOrderDecayServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.OrderNumber = '{0}' AND Key.MaterialCode='{1}'"
                                              , OrderNumber
                                              , MaterialCode),
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<WorkOrderDecay>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderDecayQueryViewModel() { OrderNumber = OrderNumber, MaterialCode = MaterialCode });
        }

        //
        //POST: /ZPVM/WorkOrderDecay/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderDecayQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderDecayServiceClient client = new WorkOrderDecayServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" Key.OrderNumber = '{0}' AND Key.MaterialCode='{1}'"
                                              , model.OrderNumber
                                              , model.MaterialCode);

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderDecay>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/WorkOrderDecay/PagingQuery
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

                using (WorkOrderDecayServiceClient client = new WorkOrderDecayServiceClient())
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
                        MethodReturnResult<IList<WorkOrderDecay>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/WorkOrderDecay/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderDecayViewModel model)
        {
            using (WorkOrderDecayServiceClient client = new WorkOrderDecayServiceClient())
            {
                WorkOrderDecay obj = new WorkOrderDecay()
                {
                    Key = new WorkOrderDecayKey(){
                        MaterialCode=model.MaterialCode,
                        OrderNumber=model.OrderNumber,
                        MaxPower=model.MaxPower.Value,
                        MinPower = model.MinPower.Value
                    },
                    DecayCode = model.DecayCode,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderDecay_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/WorkOrderDecay/Modify
        public async Task<ActionResult> Modify(string OrderNumber, string MaterialCode, double minPower,double maxPower)
        {
            WorkOrderDecayViewModel viewModel = new WorkOrderDecayViewModel();
            using (WorkOrderDecayServiceClient client = new WorkOrderDecayServiceClient())
            {
                MethodReturnResult<WorkOrderDecay> result = await client.GetAsync(new WorkOrderDecayKey()
                {
                    MaterialCode = MaterialCode,
                    OrderNumber = OrderNumber,
                    MaxPower = maxPower,
                    MinPower = minPower
                });
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderDecayViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        DecayCode=result.Data.DecayCode,
                        MaxPower=result.Data.Key.MaxPower,
                        MinPower = result.Data.Key.MinPower,
                        IsUsed=result.Data.IsUsed,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
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
            return PartialView("_ModifyPartial", new WorkOrderDecayViewModel());
        }

        //
        // POST: /ZPVM/WorkOrderDecay/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderDecayViewModel model)
        {
            using (WorkOrderDecayServiceClient client = new WorkOrderDecayServiceClient())
            {
                WorkOrderDecayKey key = new WorkOrderDecayKey()
                {
                    MaterialCode = model.MaterialCode,
                    OrderNumber = model.OrderNumber,
                    MaxPower = model.MaxPower.Value,
                    MinPower = model.MinPower.Value
                };
                MethodReturnResult<WorkOrderDecay> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.DecayCode = model.DecayCode;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderDecay_SaveModify_Success
                                                    , key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/WorkOrderDecay/Detail
        public async Task<ActionResult> Detail(string OrderNumber, string MaterialCode, double minPower, double maxPower)
        {
            using (WorkOrderDecayServiceClient client = new WorkOrderDecayServiceClient())
            {
                WorkOrderDecayKey key = new WorkOrderDecayKey()
                {
                    MaterialCode = MaterialCode,
                    OrderNumber = OrderNumber,
                    MaxPower = maxPower,
                    MinPower = minPower
                };
                MethodReturnResult<WorkOrderDecay> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    WorkOrderDecayViewModel viewModel = new WorkOrderDecayViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        DecayCode = result.Data.DecayCode,
                        MaxPower = result.Data.Key.MaxPower,
                        MinPower = result.Data.Key.MinPower,
                        IsUsed = result.Data.IsUsed,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
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
        // POST: /ZPVM/WorkOrderDecay/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string OrderNumber, string MaterialCode, double minPower,double maxPower)
        {
            MethodReturnResult result = new MethodReturnResult();
            WorkOrderDecayKey key = new WorkOrderDecayKey()
            {
                MaterialCode = MaterialCode,
                OrderNumber = OrderNumber,
                MaxPower = maxPower,
                MinPower = minPower
            };
            using (WorkOrderDecayServiceClient client = new WorkOrderDecayServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.WorkOrderDecay_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }


        public ActionResult GetDecayCode(string q)
        {
            IList<Decay> lstDetail = new List<Decay>();
            using (DecayServiceClient client = new DecayServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.Code LIKE '{0}%'
                                            AND IsUsed=1"
                                           , q),
                    OrderBy = "Key.Code"
                };

                MethodReturnResult<IList<Decay>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            var lnq = from item in lstDetail
                      select item.Key.Code;

            return Json(from item in lnq.Distinct()
                        select new
                        {
                            @label = item,
                            @value = item,
                        }, JsonRequestBehavior.AllowGet);
        }
    }
}