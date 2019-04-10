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
    public class RuleController : Controller
    {
        //
        // GET: /ZPVM/Rule/
        public async Task<ActionResult> Index()
        {
            using (RuleServiceClient client = new RuleServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<Rule>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RuleQueryViewModel());
        }

        //
        //POST: /ZPVM/Rule/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RuleQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RuleServiceClient client = new RuleServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Code))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Code);
                            }

                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Name LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Rule>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/Rule/PagingQuery
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

                using (RuleServiceClient client = new RuleServiceClient())
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
                        MethodReturnResult<IList<Rule>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/Rule/Save
        public async Task<ActionResult> Save(RuleViewModel model)
        {
            using (RuleServiceClient client = new RuleServiceClient())
            {
                Rule obj = new Rule()
                {
                    Key = model.Code,
                    Name=model.Name.ToUpper(),
                    CalibrationCycle=model.CalibrationCycle,
                    CalibrationType=model.CalibrationType,
                    FixCycle=model.FixCycle,
                    MaxPower=model.MaxPower,
                    MinPower=model.MinPower,
                    PowerDegree=model.PowerDegree,
                    PowersetCode=model.PowersetCode.ToUpper(),
                    Description = model.Description,
                    IsUsed=model.IsUsed,
                    FullPackageQty = model.FullPackageQty.Value,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.Rule_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/Rule/Modify
        public async Task<ActionResult> Modify(string code)
        {
            RuleViewModel viewModel = new RuleViewModel();
            using (RuleServiceClient client = new RuleServiceClient())
            {
                MethodReturnResult<Rule> result = await client.GetAsync(code);
                if (result.Code == 0)
                {
                    viewModel = new RuleViewModel()
                    {
                        Code=result.Data.Key,
                        Name=result.Data.Name,
                        CalibrationType=result.Data.CalibrationType,
                        PowersetCode=result.Data.PowersetCode,
                        PowerDegree=result.Data.PowerDegree,
                        MinPower=result.Data.MinPower,
                        MaxPower=result.Data.MaxPower,
                        FixCycle=result.Data.FixCycle,
                        CalibrationCycle=result.Data.CalibrationCycle,
                        Description=result.Data.Description,
                        IsUsed=result.Data.IsUsed,
                        FullPackageQty=result.Data.FullPackageQty,
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
            return PartialView("_ModifyPartial");
        }

        //
        // POST: /ZPVM/Rule/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RuleViewModel model)
        {
            using (RuleServiceClient client = new RuleServiceClient())
            {
                MethodReturnResult<Rule> result = await client.GetAsync(model.Code);

                if (result.Code == 0)
                {
                    result.Data.FullPackageQty = model.FullPackageQty.Value;
                    result.Data.Name = model.Name;
                    result.Data.CalibrationCycle = model.CalibrationCycle;
                    result.Data.CalibrationType = model.CalibrationType;
                    result.Data.FixCycle = model.FixCycle;
                    result.Data.MaxPower = model.MaxPower;
                    result.Data.MinPower = model.MinPower;
                    result.Data.PowerDegree = model.PowerDegree;
                    result.Data.PowersetCode = model.PowersetCode.ToUpper();
                    result.Data.Description = model.Description;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.Rule_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/Rule/Detail
        public async Task<ActionResult> Detail(string code)
        {
            using (RuleServiceClient client = new RuleServiceClient())
            {
                MethodReturnResult<Rule> result = await client.GetAsync(code);
                if (result.Code == 0)
                {
                    RuleViewModel viewModel = new RuleViewModel()
                    {
                        FullPackageQty=result.Data.FullPackageQty,
                        Code = result.Data.Key,
                        Name = result.Data.Name,
                        CalibrationType = result.Data.CalibrationType,
                        PowersetCode = result.Data.PowersetCode,
                        PowerDegree = result.Data.PowerDegree,
                        MinPower = result.Data.MinPower,
                        MaxPower = result.Data.MaxPower,
                        FixCycle = result.Data.FixCycle,
                        CalibrationCycle = result.Data.CalibrationCycle,
                        Description = result.Data.Description,
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
        // POST: /ZPVM/Rule/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string code)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (RuleServiceClient client = new RuleServiceClient())
            {
                result = await client.DeleteAsync(code);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.Rule_Delete_Success
                                                    , code);
                }
                return Json(result);
            }
        }

        public ActionResult GetPowersetCode(string q)
        {
            IList<Powerset> lstDetail = new List<Powerset>();
            using (PowersetServiceClient client = new PowersetServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.Code LIKE '{0}%'
                                            AND IsUsed=1"
                                           ,q),
                    OrderBy = "Key.Code"
                };

                MethodReturnResult<IList<Powerset>> result = client.Get(ref cfg);
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