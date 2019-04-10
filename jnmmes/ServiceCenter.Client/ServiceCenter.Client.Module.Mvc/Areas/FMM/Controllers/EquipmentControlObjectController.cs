using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.MES.Model.ZPVM;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class  EquipmentControlObjectController : Controller
    {
        //
        // GET: /ZPVM/RuleControlObject/
        public async Task<ActionResult> Index(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Index", "Rule");
            }

            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                MethodReturnResult<Equipment> result = await client.GetAsync(code);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "Rule");
                }
                ViewBag.Rule = result.Data;
            }

            using (EquipmentControlObjectServiceClient client = new EquipmentControlObjectServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.Code = '{0}'"
                                              , code),
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<EquipmentControlObject>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new EquipmentControlObjectQueryViewModel() { Code = code });
        }

        //
        //POST: /ZPVM/RuleControlObject/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(EquipmentControlObjectQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (EquipmentControlObjectServiceClient client = new EquipmentControlObjectServiceClient())
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
                        MethodReturnResult<IList<EquipmentControlObject>> result = client.Get(ref cfg);

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

                using (EquipmentControlObjectServiceClient client = new EquipmentControlObjectServiceClient())
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
                        MethodReturnResult<IList<EquipmentControlObject>> result = client.Get(ref cfg);
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
        public async Task<ActionResult> Save(EquipmentControlObjectViewModel model)
        {
            using (EquipmentControlObjectServiceClient client = new EquipmentControlObjectServiceClient())
            {
                EquipmentControlObject obj = new EquipmentControlObject()
                {
                    Key = new EquipmentControlObjectKey()
                    {
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
                    rst.Message = string.Format(FMMResources.StringResource.EquipmentControlObject_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/RuleControlObject/Modify
        public async Task<ActionResult> Modify(string code, EnumPVMTestDataType obj, string type)
        {
            EquipmentControlObjectViewModel viewModel = new EquipmentControlObjectViewModel();
            using (EquipmentControlObjectServiceClient client = new EquipmentControlObjectServiceClient())
            {
                MethodReturnResult<EquipmentControlObject> result = await client.GetAsync(new EquipmentControlObjectKey()
                {
                    Code=code,
                    Object=obj,
                    Type=type
                });
                if (result.Code == 0)
                {
                    viewModel = new EquipmentControlObjectViewModel()
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
        public async Task<ActionResult> SaveModify(EquipmentControlObjectViewModel model)
        {
            using (EquipmentControlObjectServiceClient client = new EquipmentControlObjectServiceClient())
            {
                EquipmentControlObjectKey key = new EquipmentControlObjectKey()
                {
                    Code = model.Code,
                    Type=model.Type,
                    Object=model.Object
                };
                MethodReturnResult<EquipmentControlObject> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.Value = model.Value;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.EquipmentControlObject_SaveModify_Success
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
            using (EquipmentControlObjectServiceClient client = new EquipmentControlObjectServiceClient())
            {
                EquipmentControlObjectKey key = new EquipmentControlObjectKey()
                {
                    Code = code,
                    Object=obj,
                    Type=type
                };
                MethodReturnResult<EquipmentControlObject> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    EquipmentControlObjectViewModel viewModel = new EquipmentControlObjectViewModel()
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
            EquipmentControlObjectKey key = new EquipmentControlObjectKey()
            {
                Code = code,
                Object=obj,
                Type=type
            };
            using (EquipmentControlObjectServiceClient client = new EquipmentControlObjectServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.EquipmentControlObject_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}