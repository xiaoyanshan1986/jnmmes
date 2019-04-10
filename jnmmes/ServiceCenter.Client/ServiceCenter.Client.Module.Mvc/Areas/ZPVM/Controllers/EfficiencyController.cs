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
    public class EfficiencyController : Controller
    {
        //
        // GET: /ZPVM/Efficiency/
        public async Task<ActionResult> Index()
        {
            using (EfficiencyServiceClient client = new EfficiencyServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<Efficiency>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new EfficiencyQueryViewModel());
        }

        //
        //POST: /ZPVM/Efficiency/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(EfficiencyQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (EfficiencyServiceClient client = new EfficiencyServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Group))
                            {
                                where.AppendFormat(" {0} Key.Group LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Group);
                            }
                            if (!string.IsNullOrEmpty(model.Code))
                            {
                                where.AppendFormat(" {0} Key.Code LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Code);
                            }

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Efficiency>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/Efficiency/PagingQuery
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

                using (EfficiencyServiceClient client = new EfficiencyServiceClient())
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
                        MethodReturnResult<IList<Efficiency>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/Efficiency/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(EfficiencyViewModel model)
        {
            using (EfficiencyServiceClient client = new EfficiencyServiceClient())
            {
                Efficiency obj = new Efficiency()
                {
                    Key = new EfficiencyKey(){
                        Code = model.Code.ToUpper(),
                        Group = model.Group.ToUpper()
                    },
                    Name = model.Name,
                    Lower=model.Lower,
                    Upper=model.Upper,
                    Description = model.Description,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.Efficiency_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/Efficiency/Modify
        public async Task<ActionResult> Modify(string group, string code)
        {
            EfficiencyViewModel viewModel = new EfficiencyViewModel();
            using (EfficiencyServiceClient client = new EfficiencyServiceClient())
            {
                MethodReturnResult<Efficiency> result = await client.GetAsync(new EfficiencyKey()
                {
                    Code=code,
                    Group=group
                });
                if (result.Code == 0)
                {
                    viewModel = new EfficiencyViewModel()
                    {
                        Code=result.Data.Key.Code,
                        Group = result.Data.Key.Group,
                        Description=result.Data.Description,
                        Name = result.Data.Name,
                        Lower=result.Data.Lower,
                        Upper=result.Data.Upper,
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
        // POST: /ZPVM/Efficiency/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(EfficiencyViewModel model)
        {
            using (EfficiencyServiceClient client = new EfficiencyServiceClient())
            {
                EfficiencyKey key = new EfficiencyKey()
                {
                    Code = model.Code,
                    Group=model.Group
                };
                MethodReturnResult<Efficiency> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.Lower = model.Lower;
                    result.Data.Upper = model.Upper;
                    result.Data.Name = model.Name;
                    result.Data.Description = model.Description;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.Efficiency_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/Efficiency/Detail
        public async Task<ActionResult> Detail(string group, string code)
        {
            using (EfficiencyServiceClient client = new EfficiencyServiceClient())
            {
                EfficiencyKey key = new EfficiencyKey()
                {
                    Code = code,
                    Group=group
                };
                MethodReturnResult<Efficiency> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    EfficiencyViewModel viewModel = new EfficiencyViewModel()
                    {
                        Description = result.Data.Description,
                        IsUsed =result.Data.IsUsed,
                        Group=result.Data.Key.Group,
                        Name=result.Data.Name,
                        Lower=result.Data.Lower,
                        Upper=result.Data.Upper,
                        Code=result.Data.Key.Code,
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
        // POST: /ZPVM/Efficiency/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string group, string code)
        {
            MethodReturnResult result = new MethodReturnResult();
            EfficiencyKey key = new EfficiencyKey()
            {
                Code = code,
                Group = group
            };
            using (EfficiencyServiceClient client = new EfficiencyServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.Efficiency_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}