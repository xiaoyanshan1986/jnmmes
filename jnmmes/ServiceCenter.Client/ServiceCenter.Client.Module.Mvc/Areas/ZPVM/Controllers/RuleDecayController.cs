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
    public class RuleDecayController : Controller
    {
        //
        // GET: /ZPVM/RuleDecay/
        public async Task<ActionResult> Index(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Index", "Rule");
            }

            using (RuleServiceClient client = new RuleServiceClient())
            {
                MethodReturnResult<Rule> result = await client.GetAsync(code);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "Rule");
                }
                ViewBag.Rule = result.Data;
            }

            using (RuleDecayServiceClient client = new RuleDecayServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.Code = '{0}'"
                                              , code),
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<RuleDecay>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RuleDecayQueryViewModel() { Code = code });
        }

        //
        //POST: /ZPVM/RuleDecay/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RuleDecayQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RuleDecayServiceClient client = new RuleDecayServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.Code = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Code);

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<RuleDecay>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/RuleDecay/PagingQuery
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

                using (RuleDecayServiceClient client = new RuleDecayServiceClient())
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
                        MethodReturnResult<IList<RuleDecay>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/RuleDecay/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RuleDecayViewModel model)
        {
            using (RuleDecayServiceClient client = new RuleDecayServiceClient())
            {
                RuleDecay obj = new RuleDecay()
                {
                    Key = new RuleDecayKey(){
                        Code = model.Code.ToUpper(),
                        MaxPower=model.MaxPower.Value,
                        MinPower = model.MinPower.Value
                    },
                    DecayCode=model.DecayCode,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.RuleDecay_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/RuleDecay/Modify
        public async Task<ActionResult> Modify(string code, double minpower, double maxpower)
        {
            RuleDecayViewModel viewModel = new RuleDecayViewModel();
            using (RuleDecayServiceClient client = new RuleDecayServiceClient())
            {
                MethodReturnResult<RuleDecay> result = await client.GetAsync(new RuleDecayKey()
                {
                    Code=code,
                    MinPower=minpower,
                    MaxPower=maxpower
                });
                if (result.Code == 0)
                {
                    viewModel = new RuleDecayViewModel()
                    {
                        Code=result.Data.Key.Code,
                        DecayCode=result.Data.DecayCode,
                        MinPower=minpower,
                        MaxPower=maxpower,
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
            return PartialView("_ModifyPartial");
        }

        //
        // POST: /ZPVM/RuleDecay/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RuleDecayViewModel model)
        {
            using (RuleDecayServiceClient client = new RuleDecayServiceClient())
            {
                RuleDecayKey key = new RuleDecayKey()
                {
                    Code = model.Code,
                    MaxPower = model.MaxPower.Value,
                    MinPower = model.MinPower.Value
                };
                MethodReturnResult<RuleDecay> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.DecayCode = model.DecayCode;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.RuleDecay_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/RuleDecay/Detail
        public async Task<ActionResult> Detail(string code, double minpower, double maxpower)
        {
            using (RuleDecayServiceClient client = new RuleDecayServiceClient())
            {
                RuleDecayKey key = new RuleDecayKey()
                {
                    Code = code,
                    MaxPower = maxpower,
                    MinPower = minpower
                };
                MethodReturnResult<RuleDecay> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    RuleDecayViewModel viewModel = new RuleDecayViewModel()
                    {
                        Code = result.Data.Key.Code,
                        MinPower = result.Data.Key.MinPower,
                        MaxPower = result.Data.Key.MaxPower,
                        DecayCode=result.Data.DecayCode,
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
        // POST: /ZPVM/RuleDecay/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string code, double minpower, double maxpower)
        {
            MethodReturnResult result = new MethodReturnResult();
            RuleDecayKey key = new RuleDecayKey()
            {
                Code = code,
                MaxPower = maxpower,
                MinPower = minpower
            };
            using (RuleDecayServiceClient client = new RuleDecayServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.RuleDecay_Delete_Success
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