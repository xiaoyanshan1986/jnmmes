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
    public class RuleGradeController : Controller
    {
        //
        // GET: /ZPVM/RuleGrade/
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

            using (RuleGradeServiceClient client = new RuleGradeServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.Code = '{0}'"
                                              , code),
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<RuleGrade>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RuleGradeQueryViewModel() { Code = code });
        }

        //
        //POST: /ZPVM/RuleGrade/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RuleGradeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RuleGradeServiceClient client = new RuleGradeServiceClient())
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
                        MethodReturnResult<IList<RuleGrade>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/RuleGrade/PagingQuery
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

                using (RuleGradeServiceClient client = new RuleGradeServiceClient())
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
                        MethodReturnResult<IList<RuleGrade>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/RuleGrade/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RuleGradeViewModel model)
        {
            using (RuleGradeServiceClient client = new RuleGradeServiceClient())
            {
                RuleGrade obj = new RuleGrade()
                {
                    Key = new RuleGradeKey(){
                        Code = model.Code.ToUpper(),
                        Grade=model.Grade
                    },
                    ItemNo=model.ItemNo??0,
                    MixColor=model.MixColor,
                    MixPowerset=model.MixPowerset,
                    MixSubPowerset=model.MixSubPowerset,
                    PackageGroup=model.PackageGroup,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.RuleGrade_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/RuleGrade/Modify
        public async Task<ActionResult> Modify(string code, string grade)
        {
            RuleGradeViewModel viewModel = new RuleGradeViewModel();
            using (RuleGradeServiceClient client = new RuleGradeServiceClient())
            {
                MethodReturnResult<RuleGrade> result = await client.GetAsync(new RuleGradeKey()
                {
                    Code=code,
                    Grade=grade
                });
                if (result.Code == 0)
                {
                    viewModel = new RuleGradeViewModel()
                    {
                        Code=result.Data.Key.Code,
                        Grade=result.Data.Key.Grade,
                        PackageGroup=result.Data.PackageGroup,
                        MixSubPowerset=result.Data.MixSubPowerset,
                        MixPowerset=result.Data.MixPowerset,
                        MixColor=result.Data.MixColor,
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
        // POST: /ZPVM/RuleGrade/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RuleGradeViewModel model)
        {
            using (RuleGradeServiceClient client = new RuleGradeServiceClient())
            {
                RuleGradeKey key = new RuleGradeKey()
                {
                    Code = model.Code,
                    Grade=model.Grade
                };
                MethodReturnResult<RuleGrade> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.ItemNo = model.ItemNo ?? 0;
                    result.Data.MixColor = model.MixColor;
                    result.Data.MixPowerset = model.MixPowerset;
                    result.Data.MixSubPowerset = model.MixSubPowerset;
                    result.Data.PackageGroup = model.PackageGroup;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.RuleGrade_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/RuleGrade/Detail
        public async Task<ActionResult> Detail(string code, string grade)
        {
            using (RuleGradeServiceClient client = new RuleGradeServiceClient())
            {
                RuleGradeKey key = new RuleGradeKey()
                {
                    Code = code,
                    Grade=grade
                };
                MethodReturnResult<RuleGrade> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    RuleGradeViewModel viewModel = new RuleGradeViewModel()
                    {
                        Code = result.Data.Key.Code,
                        Grade = result.Data.Key.Grade,
                        PackageGroup = result.Data.PackageGroup,
                        MixSubPowerset = result.Data.MixSubPowerset,
                        MixPowerset = result.Data.MixPowerset,
                        MixColor = result.Data.MixColor,
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
        // POST: /ZPVM/RuleGrade/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string code, string grade)
        {
            MethodReturnResult result = new MethodReturnResult();
            RuleGradeKey key = new RuleGradeKey()
            {
                Code = code,
                Grade = grade
            };
            using (RuleGradeServiceClient client = new RuleGradeServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.RuleGrade_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}