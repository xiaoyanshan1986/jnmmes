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
    public class DecayController : Controller
    {
        //
        // GET: /ZPVM/Decay/
        public async Task<ActionResult> Index()
        {
            using (DecayServiceClient client = new DecayServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<Decay>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new DecayQueryViewModel());
        }

        //
        //POST: /ZPVM/Decay/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(DecayQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (DecayServiceClient client = new DecayServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
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
                        MethodReturnResult<IList<Decay>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/Decay/PagingQuery
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

                using (DecayServiceClient client = new DecayServiceClient())
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
                        MethodReturnResult<IList<Decay>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/Decay/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(DecayViewModel model)
        {
            using (DecayServiceClient client = new DecayServiceClient())
            {
                Decay obj = new Decay()
                {
                    Key = new DecayKey(){
                        Code = model.Code.ToUpper(),
                        Object=model.Object
                    },
                    Type = model.Type,
                    Value=model.Value??0,
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
                    rst.Message = string.Format(ZPVMResources.StringResource.Decay_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/Decay/Modify
        public async Task<ActionResult> Modify(string code,EnumPVMTestDataType obj)
        {
            DecayViewModel viewModel = new DecayViewModel();
            using (DecayServiceClient client = new DecayServiceClient())
            {
                MethodReturnResult<Decay> result = await client.GetAsync(new DecayKey()
                {
                    Code=code,
                    Object=obj
                });
                if (result.Code == 0)
                {
                    viewModel = new DecayViewModel()
                    {
                        Code=result.Data.Key.Code,
                        Object=result.Data.Key.Object,
                        Description=result.Data.Description,
                        Value=result.Data.Value,
                        Type=result.Data.Type,
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
        // POST: /ZPVM/Decay/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(DecayViewModel model)
        {
            using (DecayServiceClient client = new DecayServiceClient())
            {
                DecayKey key = new DecayKey()
                {
                    Code = model.Code,
                    Object = model.Object
                };
                MethodReturnResult<Decay> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.Value = model.Value??0;
                    result.Data.Type = model.Type;
                    result.Data.Description = model.Description;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.Decay_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/Decay/Detail
        public async Task<ActionResult> Detail(string code, EnumPVMTestDataType obj)
        {
            using (DecayServiceClient client = new DecayServiceClient())
            {
                DecayKey key = new DecayKey()
                {
                    Code = code,
                    Object = obj
                };
                MethodReturnResult<Decay> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    DecayViewModel viewModel = new DecayViewModel()
                    {
                        Description = result.Data.Description,
                        IsUsed =result.Data.IsUsed,
                        Object=result.Data.Key.Object,
                        Type=result.Data.Type,
                        Value=result.Data.Value,
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
        // POST: /ZPVM/Decay/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string code, EnumPVMTestDataType obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            DecayKey key = new DecayKey()
            {
                Code = code,
                Object = obj
            };
            using (DecayServiceClient client = new DecayServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.Decay_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}