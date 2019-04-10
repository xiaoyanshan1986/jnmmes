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

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class MaterialTypeController : Controller
    {
        //
        // GET: /FMM/MaterialType/
        public async Task<ActionResult> Index()
        {
            using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<MaterialType>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new MaterialTypeQueryViewModel());
        }

        //
        //POST: /FMM/MaterialType/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialTypeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<MaterialType>> result = client.Get(ref cfg);

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
        //POST: /FMM/MaterialType/PagingQuery
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

                using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
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
                        MethodReturnResult<IList<MaterialType>> result = client.Get(ref cfg);
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
        // POST: /FMM/MaterialType/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialTypeViewModel model)
        {
            using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
            {
                MaterialType obj = new MaterialType()
                {
                    Key = model.Name,
                    MainProductQtyPerLot=model.MainProductQtyPerLot,
                    MainRawQtyPerLot=model.MainRawQtyPerLot,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.MaterialType_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/MaterialType/Modify
        public async Task<ActionResult> Modify(string key)
        {
            MaterialTypeViewModel viewModel = new MaterialTypeViewModel();
            using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
            {
                MethodReturnResult<MaterialType> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new MaterialTypeViewModel()
                    {
                        Name = result.Data.Key,
                     MainRawQtyPerLot=result.Data.MainRawQtyPerLot,
                      MainProductQtyPerLot=result.Data.MainProductQtyPerLot,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
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
        // POST: /FMM/MaterialType/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(MaterialTypeViewModel model)
        {
            using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
            {
                MethodReturnResult<MaterialType> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.MainProductQtyPerLot = model.MainProductQtyPerLot;
                    result.Data.MainRawQtyPerLot = model.MainRawQtyPerLot;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.MaterialType_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/MaterialType/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
            {
                MethodReturnResult<MaterialType> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    MaterialTypeViewModel viewModel = new MaterialTypeViewModel()
                    {
                        Name = result.Data.Key,
                        MainRawQtyPerLot=result.Data.MainRawQtyPerLot,
                        MainProductQtyPerLot=result.Data.MainProductQtyPerLot,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
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
        // POST: /FMM/MaterialType/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.MaterialType_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}