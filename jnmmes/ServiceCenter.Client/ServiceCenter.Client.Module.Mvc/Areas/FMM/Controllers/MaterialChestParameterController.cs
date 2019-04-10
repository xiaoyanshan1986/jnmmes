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
    public class MaterialChestParameterController : Controller
    {
        //GET: /FMM/MaterialChestParameter/
        public async Task<ActionResult> Index(string productCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = await client.GetAsync(productCode ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "Material");
                }
                ViewBag.Material = result.Data;
            }

            using (MaterialChestParameterServiceClient client = new MaterialChestParameterServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = " Key ",
                        Where = string.Format(" Key = '{0}'", productCode)
                    };
                    MethodReturnResult<IList<MaterialChestParameter>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new MaterialChestParameterQueryViewModel() { ProductCode = productCode });
        }
       
        //POST: /FMM/MaterialChestParameter/PagingQuery
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

                using (MaterialChestParameterServiceClient client = new MaterialChestParameterServiceClient())
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
                        MethodReturnResult<IList<MaterialChestParameter>> result = client.Get(ref cfg);
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

        // POST: /FMM/MaterialChestParameter/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialChestParameterViewModel model)
        {
            using (MaterialChestParameterServiceClient client = new MaterialChestParameterServiceClient())
            {
                MethodReturnResult rst = new MethodReturnResult();
                if (model.FullChestQty < 0)
                {
                    rst.Code = 1001;
                    rst.Message = String.Format("满柜数量不可小于0");
                    return Json(rst);
                }
                if (model.InChestFullPackageQty < 0)
                {
                    rst.Code = 1001;
                    rst.Message = String.Format("柜最大满包数量不可小于0");
                    return Json(rst);
                }
                MaterialChestParameter obj = new MaterialChestParameter()
                {
                    Key = model.ProductCode,
                    ColorLimit = model.ColorLimit,
                    GradeLimit = model.GradeLimit,
                    PowerLimit = model.PowerLimit,
                    IscLimit = model.IscLimit,
                    FullChestQty = model.FullChestQty,
                    InChestFullPackageQty = model.InChestFullPackageQty,
                    IsPackagedChest = model.IsPackagedChest,
                    OrderNumberLimit = model.OrderNumberLimit,
                    LastChestMaterialLimit = model.LastChestMaterialLimit,
                    Creator = User.Identity.Name,
                    CreateTime = DateTime.Now,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.MaterialChestParameter_Save_Success, obj.Key);
                }
                return Json(rst);
            }
        }
        
        // GET: /FMM/MaterialChestParameter/Modify
        public async Task<ActionResult> Modify(string productCode)
        {
            MaterialChestParameterViewModel viewModel = new MaterialChestParameterViewModel();
            using (MaterialChestParameterServiceClient client = new MaterialChestParameterServiceClient())
            {
                MethodReturnResult<MaterialChestParameter> result = await client.GetAsync(productCode);
                if (result.Code == 0)
                {
                    viewModel = new MaterialChestParameterViewModel()
                    {
                        ProductCode = result.Data.Key,
                        ColorLimit = result.Data.ColorLimit,
                        GradeLimit = result.Data.GradeLimit,
                        PowerLimit = result.Data.PowerLimit,
                        IscLimit = result.Data.IscLimit,
                        FullChestQty = result.Data.FullChestQty,
                        InChestFullPackageQty = result.Data.InChestFullPackageQty,
                        IsPackagedChest = result.Data.IsPackagedChest,
                        OrderNumberLimit = result.Data.OrderNumberLimit,
                        LastChestMaterialLimit = result.Data.LastChestMaterialLimit,
                        Creator = result.Data.Creator,
                        CreateTime = result.Data.CreateTime,
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

        // POST: /FMM/MaterialChestParameter/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(MaterialChestParameterViewModel model)
        {
            using (MaterialChestParameterServiceClient client = new MaterialChestParameterServiceClient())
            {
                MethodReturnResult<MaterialChestParameter> result = await client.GetAsync(model.ProductCode);

                if (result.Code == 0)
                {
                    MethodReturnResult rst = new MethodReturnResult();
                    if (model.FullChestQty < 0)
                    {
                        rst.Code = 1001;
                        rst.Message = String.Format("满柜数量不可小于0");
                        return Json(rst);
                    }
                    if (model.InChestFullPackageQty < 0)
                    {
                        rst.Code = 1001;
                        rst.Message = String.Format("柜最大满包数量不可小于0");
                        return Json(rst);
                    }

                    result.Data.Key = model.ProductCode;
                    result.Data.ColorLimit = model.ColorLimit;
                    result.Data.GradeLimit = model.GradeLimit;
                    result.Data.PowerLimit = model.PowerLimit;
                    result.Data.IscLimit = model.IscLimit;
                    result.Data.FullChestQty = model.FullChestQty;
                    result.Data.InChestFullPackageQty = model.InChestFullPackageQty;
                    result.Data.IsPackagedChest = model.IsPackagedChest;
                    result.Data.OrderNumberLimit = model.OrderNumberLimit;
                    result.Data.LastChestMaterialLimit = model.LastChestMaterialLimit;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.MaterialChestParameter_SaveModify_Success, result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        
        // GET: /FMM/MaterialChestParameter/Detail
        public async Task<ActionResult> Detail(string productCode)
        {
            using (MaterialChestParameterServiceClient client = new MaterialChestParameterServiceClient())
            {
                MethodReturnResult<MaterialChestParameter> result = await client.GetAsync(productCode);
                if (result.Code == 0)
                {
                    MaterialChestParameterViewModel viewModel = new MaterialChestParameterViewModel()
                    {
                        ProductCode = result.Data.Key,
                        ColorLimit = result.Data.ColorLimit,
                        GradeLimit = result.Data.GradeLimit,
                        PowerLimit = result.Data.PowerLimit,
                        IscLimit = result.Data.IscLimit,
                        FullChestQty = result.Data.FullChestQty,
                        InChestFullPackageQty = result.Data.InChestFullPackageQty,
                        IsPackagedChest = result.Data.IsPackagedChest,
                        OrderNumberLimit = result.Data.OrderNumberLimit,
                        LastChestMaterialLimit = result.Data.LastChestMaterialLimit,
                        Creator = result.Data.Creator,
                        CreateTime = result.Data.CreateTime,
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
        
        // POST: /FMM/MaterialChestParameter/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string productCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialChestParameterServiceClient client = new MaterialChestParameterServiceClient())
            {
                result = await client.DeleteAsync(productCode);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.MaterialChestParameter_Delete_Success, productCode);
                }
                return Json(result);
            }
        }
    }
}