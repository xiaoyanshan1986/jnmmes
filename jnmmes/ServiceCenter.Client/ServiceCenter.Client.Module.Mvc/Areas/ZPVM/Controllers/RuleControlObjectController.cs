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
    public class RuleControlObjectController : Controller
    {
        //
        // GET: /ZPVM/RuleControlObject/
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

            using (RuleControlObjectServiceClient client = new RuleControlObjectServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.Code = '{0}'"
                                              , code),
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<RuleControlObject>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RuleControlObjectQueryViewModel() { Code = code });
        }

        //
        //POST: /ZPVM/RuleControlObject/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RuleControlObjectQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RuleControlObjectServiceClient client = new RuleControlObjectServiceClient())
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
                        MethodReturnResult<IList<RuleControlObject>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/RuleControlObject/PagingQuery
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

                using (RuleControlObjectServiceClient client = new RuleControlObjectServiceClient())
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
                        MethodReturnResult<IList<RuleControlObject>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/RuleControlObject/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RuleControlObjectViewModel model)
        {
            using (RuleControlObjectServiceClient client = new RuleControlObjectServiceClient())
            {
                RuleControlObject obj = new RuleControlObject()
                {
                    Key = new RuleControlObjectKey(){
                        Code = model.Code.ToUpper(),
                        Object=model.Object,
                        Type=model.Type
                    },
                    Value=model.Value,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.RuleControlObject_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/RuleControlObject/Modify
        public async Task<ActionResult> Modify(string code, EnumPVMTestDataType obj, string type)
        {
            RuleControlObjectViewModel viewModel = new RuleControlObjectViewModel();
            using (RuleControlObjectServiceClient client = new RuleControlObjectServiceClient())
            {
                MethodReturnResult<RuleControlObject> result = await client.GetAsync(new RuleControlObjectKey()
                {
                    Code=code,
                    Object=obj,
                    Type=type
                });
                if (result.Code == 0)
                {
                    viewModel = new RuleControlObjectViewModel()
                    {
                        Code=result.Data.Key.Code,
                        Value=result.Data.Value,
                        Type=result.Data.Key.Type,
                        Object=result.Data.Key.Object,
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
        // POST: /ZPVM/RuleControlObject/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RuleControlObjectViewModel model)
        {
            using (RuleControlObjectServiceClient client = new RuleControlObjectServiceClient())
            {
                RuleControlObjectKey key = new RuleControlObjectKey()
                {
                    Code = model.Code,
                    Type=model.Type,
                    Object=model.Object
                };
                MethodReturnResult<RuleControlObject> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.Value = model.Value;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.RuleControlObject_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/RuleControlObject/Detail
        public async Task<ActionResult> Detail(string code, EnumPVMTestDataType obj, string type)
        {
            using (RuleControlObjectServiceClient client = new RuleControlObjectServiceClient())
            {
                RuleControlObjectKey key = new RuleControlObjectKey()
                {
                    Code = code,
                    Object=obj,
                    Type=type
                };
                MethodReturnResult<RuleControlObject> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    RuleControlObjectViewModel viewModel = new RuleControlObjectViewModel()
                    {
                        Code = result.Data.Key.Code,
                        Value = result.Data.Value,
                        Type = result.Data.Key.Type,
                        Object = result.Data.Key.Object,
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
        // POST: /ZPVM/RuleControlObject/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string code, EnumPVMTestDataType obj, string type)
        {
            MethodReturnResult result = new MethodReturnResult();
            RuleControlObjectKey key = new RuleControlObjectKey()
            {
                Code = code,
                Object=obj,
                Type=type
            };
            using (RuleControlObjectServiceClient client = new RuleControlObjectServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.RuleControlObject_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}