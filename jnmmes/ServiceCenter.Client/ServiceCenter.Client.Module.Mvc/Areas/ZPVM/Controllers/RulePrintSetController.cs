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
    public class RulePrintSetController : Controller
    {
        //
        // GET: /ZPVM/RulePrintSet/
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

            using (RulePrintSetServiceClient client = new RulePrintSetServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.Code = '{0}'"
                                              , code),
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<RulePrintSet>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RulePrintSetQueryViewModel() { Code = code });
        }

        //
        //POST: /ZPVM/RulePrintSet/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RulePrintSetQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RulePrintSetServiceClient client = new RulePrintSetServiceClient())
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
                        MethodReturnResult<IList<RulePrintSet>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/RulePrintSet/PagingQuery
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

                using (RulePrintSetServiceClient client = new RulePrintSetServiceClient())
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
                        MethodReturnResult<IList<RulePrintSet>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/RulePrintSet/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RulePrintSetViewModel model)
        {
            using (RulePrintSetServiceClient client = new RulePrintSetServiceClient())
            {
                RulePrintSet obj = new RulePrintSet()
                {
                    Key = new RulePrintSetKey(){
                        Code = model.Code.ToUpper(),
                        LabelCode=model.LabelCode
                    },
                    ItemNo=model.ItemNo??0,
                    Qty=model.Qty??1,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.RulePrintSet_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/RulePrintSet/Modify
        public async Task<ActionResult> Modify(string code, string labelCode)
        {
            RulePrintSetViewModel viewModel = new RulePrintSetViewModel();
            using (RulePrintSetServiceClient client = new RulePrintSetServiceClient())
            {
                MethodReturnResult<RulePrintSet> result = await client.GetAsync(new RulePrintSetKey()
                {
                    Code=code,
                    LabelCode=labelCode
                });
                if (result.Code == 0)
                {
                    viewModel = new RulePrintSetViewModel()
                    {
                        Code=result.Data.Key.Code,
                        LabelCode=result.Data.Key.LabelCode,
                        Qty=result.Data.Qty,
                        ItemNo=result.Data.ItemNo,
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
        // POST: /ZPVM/RulePrintSet/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RulePrintSetViewModel model)
        {
            using (RulePrintSetServiceClient client = new RulePrintSetServiceClient())
            {
                RulePrintSetKey key = new RulePrintSetKey()
                {
                    Code = model.Code,
                    LabelCode=model.LabelCode
                };
                MethodReturnResult<RulePrintSet> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.ItemNo = model.ItemNo ?? 0;
                    result.Data.Qty = model.Qty??1;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.RulePrintSet_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/RulePrintSet/Detail
        public async Task<ActionResult> Detail(string code, string labelCode)
        {
            using (RulePrintSetServiceClient client = new RulePrintSetServiceClient())
            {
                RulePrintSetKey key = new RulePrintSetKey()
                {
                    Code = code,
                    LabelCode = labelCode
                };
                MethodReturnResult<RulePrintSet> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    RulePrintSetViewModel viewModel = new RulePrintSetViewModel()
                    {
                        Code = result.Data.Key.Code,
                        LabelCode = result.Data.Key.LabelCode,
                        Qty = result.Data.Qty,
                        ItemNo = result.Data.ItemNo,
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
        // POST: /ZPVM/RulePrintSet/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string code, string labelCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            RulePrintSetKey key = new RulePrintSetKey()
            {
                Code = code,
                LabelCode = labelCode
            };
            using (RulePrintSetServiceClient client = new RulePrintSetServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.RulePrintSet_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}