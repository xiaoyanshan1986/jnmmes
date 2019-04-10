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
    public class MaterialBOMController : Controller
    {
        //
        // GET: /FMM/MaterialBOM/
        public async Task<ActionResult> Index(string materialCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = await client.GetAsync(materialCode ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "Material");
                }
                ViewBag.Material = result.Data;
            }

            using (MaterialBOMServiceClient client = new MaterialBOMServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.MaterialCode = '{0}'"
                                                    , materialCode),
                        OrderBy = "Key.ItemNo"
                    };
                    MethodReturnResult<IList<MaterialBOM>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new MaterialBOMQueryViewModel() { MaterialCode = materialCode });
        }

        //
        //POST: /FMM/MaterialBOM/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialBOMQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialBOMServiceClient client = new MaterialBOMServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.MaterialCode = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.MaterialCode);

                            if (!string.IsNullOrEmpty(model.RawMaterialCode))
                            {
                                where.AppendFormat(" {0} RawMaterialCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RawMaterialCode);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.ItemNo",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<MaterialBOM>> result = client.Get(ref cfg);

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
        //POST: /FMM/MaterialBOM/PagingQuery
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

                using (MaterialBOMServiceClient client = new MaterialBOMServiceClient())
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
                        MethodReturnResult<IList<MaterialBOM>> result = client.Get(ref cfg);
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
        // POST: /FMM/MaterialBOM/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialBOMViewModel model)
        {
            using (MaterialBOMServiceClient client = new MaterialBOMServiceClient())
            {
                MaterialBOM obj = new MaterialBOM()
                {
                    Key = new MaterialBOMKey()
                    {
                        MaterialCode = model.MaterialCode.ToUpper(),
                        ItemNo = model.ItemNo
                    },
                    RawMaterialCode = model.RawMaterialCode.ToUpper(),
                    MaterialUnit=model.MaterialUnit,
                    Qty=model.Qty,
                    StoreLocation=model.StoreLocation,
                    WorkCenter=model.WorkCenter,
                    Description=model.Description,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.MaterialBOM_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/MaterialBOM/Modify
        public async Task<ActionResult> Modify(string materialCode, int itemNo)
        {
            MaterialBOMViewModel viewModel = new MaterialBOMViewModel();
            using (MaterialBOMServiceClient client = new MaterialBOMServiceClient())
            {
                MethodReturnResult<MaterialBOM> result = await client.GetAsync(new MaterialBOMKey()
                {
                    MaterialCode = materialCode,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    viewModel = new MaterialBOMViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        ItemNo = result.Data.Key.ItemNo,
                        RawMaterialCode = result.Data.RawMaterialCode,
                        WorkCenter=result.Data.WorkCenter,
                        StoreLocation=result.Data.StoreLocation,
                        Qty=result.Data.Qty,
                        MaterialUnit=result.Data.MaterialUnit,
                        Description=result.Data.Description,
                        CreateTime=result.Data.CreateTime,
                        Creator=result.Data.Creator,
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
        // POST: /FMM/MaterialBOM/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(MaterialBOMViewModel model)
        {
            using (MaterialBOMServiceClient client = new MaterialBOMServiceClient())
            {
                MethodReturnResult<MaterialBOM> result = await client.GetAsync(new MaterialBOMKey()
                {
                    MaterialCode = model.MaterialCode.ToUpper(),
                    ItemNo = model.ItemNo
                });

                if (result.Code == 0)
                {
                    result.Data.RawMaterialCode = model.RawMaterialCode.ToUpper();
                    result.Data.MaterialUnit = model.MaterialUnit;
                    result.Data.Qty = model.Qty;
                    result.Data.StoreLocation = model.StoreLocation;
                    result.Data.WorkCenter = model.WorkCenter;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.MaterialBOM_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/MaterialBOM/Detail
        public async Task<ActionResult> Detail(string materialCode, int itemNo)
        {
            using (MaterialBOMServiceClient client = new MaterialBOMServiceClient())
            {
                MethodReturnResult<MaterialBOM> result = await client.GetAsync(new MaterialBOMKey()
                {
                    MaterialCode = materialCode,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    MaterialBOMViewModel viewModel = new MaterialBOMViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        ItemNo = result.Data.Key.ItemNo,
                        WorkCenter = result.Data.WorkCenter,
                        StoreLocation=result.Data.StoreLocation,
                        Qty=result.Data.Qty,
                        MaterialUnit=result.Data.MaterialUnit,
                        RawMaterialCode=result.Data.RawMaterialCode,
                        Description=result.Data.Description,
                        Creator=result.Data.Creator,
                        CreateTime=result.Data.CreateTime,
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
        // POST: /FMM/MaterialBOM/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string materialCode, int itemNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialBOMServiceClient client = new MaterialBOMServiceClient())
            {
                result = await client.DeleteAsync(new MaterialBOMKey()
                {
                    MaterialCode = materialCode,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.MaterialBOM_Delete_Success
                                                    , itemNo);
                }
                return Json(result);
            }
        }

        public ActionResult GetMaxItemNo(string materialCode)
        {
            using (MaterialBOMServiceClient client = new MaterialBOMServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.MaterialCode='{0}'", materialCode),
                    OrderBy = "Key.ItemNo Desc"
                };
                MethodReturnResult<IList<MaterialBOM>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    return Json(result.Data[0].Key.ItemNo + 1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetMaterialUnit(string materialCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    return Json(result.Data.Unit, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRawMaterialCode(string q)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key LIKE '{0}%' AND IsRaw='1' AND Status='1'", q)
                };


                MethodReturnResult<IList<Material>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return Json(from item in result.Data
                                select new
                                {
                                    @label = string.Format("{0}[{1}]", item.Key, item.Name),
                                    @value = item.Key
                                }, JsonRequestBehavior.AllowGet); ;
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet); ;
        }
    }
}