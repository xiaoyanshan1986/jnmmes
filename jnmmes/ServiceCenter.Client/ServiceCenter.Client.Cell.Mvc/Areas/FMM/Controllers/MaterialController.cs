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
    public class MaterialController : Controller
    {
        //
        // GET: /FMM/Material/
        public async Task<ActionResult> Index()
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<Material>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new MaterialQueryViewModel());
        }

        //
        //POST: /FMM/Material/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialServiceClient client = new MaterialServiceClient())
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
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Type LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Type);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Material>> result = client.Get(ref cfg);

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
        //POST: /FMM/Material/PagingQuery
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

                using (MaterialServiceClient client = new MaterialServiceClient())
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
                        MethodReturnResult<IList<Material>> result = client.Get(ref cfg);
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
        // POST: /FMM/Material/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialViewModel model)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                Material obj = new Material()
                {
                    Key = model.Code.ToUpper(),
                    BarCode=model.BarCode,
                    Name=model.Name,
                    Class=model.Class,
                    Spec=model.Spec,
                    Status=model.Status,
                    Type=model.Type,
                    Unit=model.Unit,
                    IsProduct=model.IsProduct,
                    IsRaw=model.IsRaw,
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
                    rst.Message = string.Format(FMMResources.StringResource.Material_Save_Success
                                                , model.Code);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/Material/Modify
        public async Task<ActionResult> Modify(string key)
        {
            MaterialViewModel viewModel = new MaterialViewModel();
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new MaterialViewModel()
                    {
                        Code=result.Data.Key,
                        BarCode=result.Data.BarCode,
                        Status=result.Data.Status,
                        Class=result.Data.Class,
                        Spec=result.Data.Spec,
                        Unit=result.Data.Unit,
                        MainRawQtyPerLot=result.Data.MainRawQtyPerLot,
                        MainProductQtyPerLot=result.Data.MainProductQtyPerLot,
                        Type=result.Data.Type,
                        Name = result.Data.Name,
                        IsProduct = result.Data.IsProduct,
                        IsRaw = result.Data.IsRaw,
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
        // POST: /FMM/Material/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(MaterialViewModel model)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = await client.GetAsync(model.Code);

                if (result.Code == 0)
                {
                    result.Data.MainProductQtyPerLot = model.MainProductQtyPerLot;
                    result.Data.MainRawQtyPerLot = model.MainRawQtyPerLot;
                    result.Data.Name = model.Name;
                    result.Data.Spec=model.Spec;
                    result.Data.Class = model.Class;
                    result.Data.Status = model.Status;
                    result.Data.Type = model.Type;
                    result.Data.Unit = model.Unit;
                    result.Data.IsRaw = model.IsRaw;
                    result.Data.IsProduct = model.IsProduct;
                    result.Data.BarCode = model.BarCode;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.Material_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/Material/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    MaterialViewModel viewModel = new MaterialViewModel()
                    {
                        MainRawQtyPerLot=result.Data.MainRawQtyPerLot,
                        MainProductQtyPerLot=result.Data.MainProductQtyPerLot,
                        Code = result.Data.Key,
                        Class=result.Data.Class,
                        BarCode = result.Data.BarCode,
                        Status = result.Data.Status,
                        Spec = result.Data.Spec,
                        Unit = result.Data.Unit,
                        Type = result.Data.Type,
                        IsProduct=result.Data.IsProduct,
                        IsRaw=result.Data.IsRaw,
                        Name = result.Data.Name,
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
        // POST: /FMM/Material/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.Material_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}