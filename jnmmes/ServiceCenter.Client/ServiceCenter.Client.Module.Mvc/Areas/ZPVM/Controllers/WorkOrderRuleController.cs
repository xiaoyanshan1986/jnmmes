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
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.PPM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class WorkOrderRuleController : Controller
    {

        //
        // GET: /ZPVM/WorkOrderRule/
        public async Task<ActionResult> Index(string orderNumber)
        {
            return await Query(new WorkOrderRuleQueryViewModel() { OrderNumber = orderNumber });
        }

        //
        //POST: /ZPVM/WorkOrderRule/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderRuleQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.OrderNumber LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);

                            if (!string.IsNullOrEmpty(model.MaterialCode))
                            {
                                where.AppendFormat(" {0} Key.MaterialCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.MaterialCode);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "EditTime DESC",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderProduct>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            if(Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new WorkOrderRuleViewModel());
            }
            else
            {
                return View("Index", model);
            }
            
        }

        //
        //POST: /ZPVM/WorkOrderRule/PagingQuery
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

                using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
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
                        MethodReturnResult<IList<WorkOrderProduct>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new WorkOrderRuleViewModel());
        }
        //
        // POST: /ZPVM/WorkOrderRule/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderRuleViewModel model)
        {
            using (WorkOrderRuleServiceClient client = new WorkOrderRuleServiceClient())
            {
                WorkOrderRule obj = new WorkOrderRule()
                {
                    Key = new WorkOrderRuleKey()
                    {
                        OrderNumber = model.OrderNumber.ToUpper(),
                        MaterialCode = model.MaterialCode.ToUpper()
                    },
                    CalibrationCycle=model.CalibrationCycle,
                    CalibrationType=model.CalibrationType,
                    FullPackageQty=model.FullPackageQty.Value,
                    Description=model.Description,
                    FixCycle=model.FixCycle,
                    MaxPower=model.MaxPower,
                    MinPower=model.MinPower,
                    PowerDegree=model.PowerDegree,
                    RuleCode = model.RuleCode,
                    RuleName = model.RuleName,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderRule_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/WorkOrderRule/Modify
        public async Task<ActionResult> Modify(string orderNumber, string materialCode)
        {
            WorkOrderRuleViewModel viewModel = new WorkOrderRuleViewModel();
            using (WorkOrderRuleServiceClient client = new WorkOrderRuleServiceClient())
            {
                MethodReturnResult<WorkOrderRule> result = await client.GetAsync(new WorkOrderRuleKey()
                {
                    OrderNumber = orderNumber,
                    MaterialCode = materialCode
                });
                if (result.Code == 0 && result.Data!=null)
                {
                    viewModel = new WorkOrderRuleViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        MaterialCode = result.Data.Key.MaterialCode,
                        CalibrationType=result.Data.CalibrationType,
                        CalibrationCycle=result.Data.CalibrationCycle,
                        FullPackageQty=result.Data.FullPackageQty,
                        PowerDegree=result.Data.PowerDegree,
                        MinPower=result.Data.MinPower,
                        MaxPower=result.Data.MaxPower,
                        FixCycle=result.Data.FixCycle,
                        Description=result.Data.Description,
                        RuleCode=result.Data.RuleCode,
                        RuleName=result.Data.RuleName,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_ModifyPartial", viewModel);
                }
            }
            return PartialView("_ModifyPartial", new WorkOrderRuleViewModel()
            {
                OrderNumber = orderNumber,
                MaterialCode = materialCode
            });
        }

        //
        // POST: /ZPVM/WorkOrderRule/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderRuleViewModel model)
        {
            using (WorkOrderRuleServiceClient client = new WorkOrderRuleServiceClient())
            {
                MethodReturnResult<WorkOrderRule> result = await client.GetAsync(new WorkOrderRuleKey()
                {
                    OrderNumber = model.OrderNumber,
                    MaterialCode = model.MaterialCode
                });

                if (result.Code == 0)
                {
                    result.Data.RuleName = model.RuleName;
                    result.Data.RuleCode = model.RuleCode;
                    result.Data.PowerDegree = model.PowerDegree;
                    result.Data.MinPower = model.MinPower;
                    result.Data.MaxPower = model.MaxPower;
                    result.Data.FixCycle = model.FixCycle;
                    result.Data.FullPackageQty = model.FullPackageQty.Value;
                    result.Data.CalibrationCycle = model.CalibrationCycle;
                    result.Data.CalibrationType = model.CalibrationType;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderRule_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                else
                {
                    return await Save(model);
                }
            }
        }

        //
        // GET: /ZPVM/WorkOrderRule/Detail
        public async Task<ActionResult> Detail(string orderNumber, string materialCode)
        {
            using (WorkOrderRuleServiceClient client = new WorkOrderRuleServiceClient())
            {
                MethodReturnResult<WorkOrderRule> result = await client.GetAsync(new WorkOrderRuleKey()
                {
                    OrderNumber = orderNumber,
                    MaterialCode = materialCode
                });
                if (result.Code == 0)
                {
                    WorkOrderRuleViewModel viewModel = new WorkOrderRuleViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        MaterialCode = result.Data.Key.MaterialCode,
                        CalibrationType = result.Data.CalibrationType,
                        CalibrationCycle = result.Data.CalibrationCycle,
                        PowerDegree = result.Data.PowerDegree,
                        MinPower = result.Data.MinPower,
                        MaxPower = result.Data.MaxPower,
                        FixCycle = result.Data.FixCycle,
                        FullPackageQty=result.Data.FullPackageQty,
                        Description = result.Data.Description,
                        RuleCode = result.Data.RuleCode,
                        RuleName = result.Data.RuleName,
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
            return PartialView("_InfoPartial", new WorkOrderRuleViewModel()
            {
                OrderNumber = orderNumber,
                MaterialCode = materialCode
            });
        }

        //
        // POST: /ZPVM/WorkOrderRule/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string orderNumber, string materialCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WorkOrderRuleServiceClient client = new WorkOrderRuleServiceClient())
            {
                result = await client.DeleteAsync(new WorkOrderRuleKey()
                {
                    OrderNumber = orderNumber,
                    MaterialCode = materialCode
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.WorkOrderRule_Delete_Success
                                                    , materialCode);
                }
                return Json(result);
            }
        }

        public ActionResult GetRuleCode(string q)
        {
            IList<Rule> lstDetail = new List<Rule>();
            using (RuleServiceClient client = new RuleServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key LIKE '{0}%'
                                            AND IsUsed=1"
                                           , q),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<Rule>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            var lnq = from item in lstDetail
                      select item.Key;

            return Json(from item in lstDetail
                        select new
                        {
                            @label = item.Key+"-"+item.Name,
                            @value = item.Key,
                            @Name = item.Name,
                            @MaxPower = item.MaxPower,
                            @MinPower=item.MinPower,
                            @FixCycle=item.FixCycle,
                            @CalibrationCycle=item.CalibrationCycle,
                            @CalibrationType=item.CalibrationType,
                            @PowerDegree=item.PowerDegree,
                            @FullPackageQty = item.FullPackageQty
                        }, JsonRequestBehavior.AllowGet);
        }
    }
}