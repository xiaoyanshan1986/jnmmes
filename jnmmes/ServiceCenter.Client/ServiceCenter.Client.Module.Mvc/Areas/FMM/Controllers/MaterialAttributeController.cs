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
    public class MaterialAttributeController : Controller
    {

        //
        // GET: /FMM/MaterialAttribute/
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

            using (MaterialAttributeServiceClient client = new MaterialAttributeServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key.MaterialCode,Key.AttributeName",
                        Where = string.Format(" Key.MaterialCode = '{0}'"
                                                  , materialCode)
                    };
                    MethodReturnResult<IList<MaterialAttribute>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new MaterialAttributeQueryViewModel() { MaterialCode = materialCode });
        }

        //
        //POST: /FMM/MaterialAttribute/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialAttributeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialAttributeServiceClient client = new MaterialAttributeServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.MaterialCode = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.MaterialCode);

                            if (!string.IsNullOrEmpty(model.AttributeName))
                            {
                                where.AppendFormat(" {0} Key.AttributeName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.AttributeName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<MaterialAttribute>> result = client.Get(ref cfg);

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
        //POST: /FMM/MaterialAttribute/PagingQuery
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

                using (MaterialAttributeServiceClient client = new MaterialAttributeServiceClient())
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
                        MethodReturnResult<IList<MaterialAttribute>> result = client.Get(ref cfg);
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
        // POST: /FMM/MaterialAttribute/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialAttributeViewModel model)
        {
            using (MaterialAttributeServiceClient client = new MaterialAttributeServiceClient())
            {
                MaterialAttribute obj = new MaterialAttribute()
                {
                    Key = new MaterialAttributeKey() { 
                         MaterialCode=model.MaterialCode,
                         AttributeName=model.AttributeName
                    },
                    Value=model.Value,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.MaterialAttribute_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/MaterialAttribute/Modify
        public async Task<ActionResult> Modify(string materialCode,string attributeName)
        {
            MaterialAttributeViewModel viewModel = new MaterialAttributeViewModel();
            using (MaterialAttributeServiceClient client = new MaterialAttributeServiceClient())
            {
                MethodReturnResult<MaterialAttribute> result = await client.GetAsync(new MaterialAttributeKey()
                {
                    MaterialCode = materialCode,
                    AttributeName=attributeName
                });
                if (result.Code == 0)
                {
                    viewModel = new MaterialAttributeViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        AttributeName = result.Data.Key.AttributeName,
                        Value = result.Data.Value,
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
        // POST: /FMM/MaterialAttribute/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(MaterialAttributeViewModel model)
        {
            using (MaterialAttributeServiceClient client = new MaterialAttributeServiceClient())
            {
                MethodReturnResult<MaterialAttribute> result = await client.GetAsync(new MaterialAttributeKey()
                {
                    MaterialCode = model.MaterialCode,
                    AttributeName = model.AttributeName
                });

                if (result.Code == 0)
                {
                    result.Data.Value = model.Value;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.MaterialAttribute_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/MaterialAttribute/Detail
        public async Task<ActionResult> Detail(string materialCode, string attributeName)
        {
            using (MaterialAttributeServiceClient client = new MaterialAttributeServiceClient())
            {
                MethodReturnResult<MaterialAttribute> result = await client.GetAsync(new MaterialAttributeKey()
                {
                    MaterialCode = materialCode,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    MaterialAttributeViewModel viewModel = new MaterialAttributeViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        AttributeName = result.Data.Key.AttributeName,
                        Value = result.Data.Value,
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
        // POST: /FMM/MaterialAttribute/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string materialCode, string attributeName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialAttributeServiceClient client = new MaterialAttributeServiceClient())
            {
                result = await client.DeleteAsync(new MaterialAttributeKey()
                {
                    MaterialCode = materialCode,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.MaterialAttribute_Delete_Success
                                                    , attributeName);
                }
                return Json(result);
            }
        }
    }
}