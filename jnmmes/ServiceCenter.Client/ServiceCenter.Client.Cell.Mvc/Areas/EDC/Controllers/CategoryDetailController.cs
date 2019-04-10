using ServiceCenter.Client.Mvc.Areas.EDC.Models;
using EDCResources = ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Client.EDC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Controllers
{
    public class CategoryDetailController : Controller
    {

        //
        // GET: /EDC/CategoryDetail/
        public async Task<ActionResult> Index(string categoryName)
        {
            using (CategoryServiceClient client = new CategoryServiceClient())
            {
                MethodReturnResult<Category> result = await client.GetAsync(categoryName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "Category");
                }
                ViewBag.Category = result.Data;
            }

            using (CategoryDetailServiceClient client = new CategoryDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "ItemNo",
                        Where = string.Format(" Key.CategoryName = '{0}'"
                                                    , categoryName)
                    };
                    MethodReturnResult<IList<CategoryDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new CategoryDetailQueryViewModel() { CategoryName = categoryName });
        }

        //
        //POST: /EDC/CategoryDetail/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(CategoryDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (CategoryDetailServiceClient client = new CategoryDetailServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.CategoryName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.CategoryName);

                            if (!string.IsNullOrEmpty(model.ParameterName))
                            {
                                where.AppendFormat(" {0} Key.ParameterName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ParameterName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "ItemNo",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<CategoryDetail>> result = client.Get(ref cfg);

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
        //POST: /EDC/CategoryDetail/PagingQuery
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

                using (CategoryDetailServiceClient client = new CategoryDetailServiceClient())
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
                        MethodReturnResult<IList<CategoryDetail>> result = client.Get(ref cfg);
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
        // POST: /EDC/CategoryDetail/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(CategoryDetailViewModel model)
        {
            using (CategoryDetailServiceClient client = new CategoryDetailServiceClient())
            {
                CategoryDetail obj = new CategoryDetail()
                {
                    Key = new CategoryDetailKey() {
                        CategoryName = model.CategoryName.ToUpper(),
                        ParameterName = model.ParameterName.ToUpper()
                    },
                    ItemNo = model.ItemNo,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(EDCResources.StringResource.CategoryDetail_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /EDC/CategoryDetail/Modify
        public async Task<ActionResult> Modify(string categoryName,string parameterName)
        {
            CategoryDetailViewModel viewModel = new CategoryDetailViewModel();
            using (CategoryDetailServiceClient client = new CategoryDetailServiceClient())
            {
                MethodReturnResult<CategoryDetail> result = await client.GetAsync(new CategoryDetailKey()
                {
                    CategoryName=categoryName,
                    ParameterName=parameterName
                });
                if (result.Code == 0)
                {
                    viewModel = new CategoryDetailViewModel()
                    {
                        CategoryName = result.Data.Key.CategoryName,
                        ParameterName = result.Data.Key.ParameterName,
                        ItemNo = result.Data.ItemNo,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        CreateTime=result.Data.CreateTime,
                        Creator=result.Data.Creator
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
        // POST: /EDC/CategoryDetail/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(CategoryDetailViewModel model)
        {
            using (CategoryDetailServiceClient client = new CategoryDetailServiceClient())
            {
                MethodReturnResult<CategoryDetail> result = await client.GetAsync(new CategoryDetailKey()
                {
                    CategoryName = model.CategoryName,
                    ParameterName = model.ParameterName
                });

                if (result.Code == 0)
                {
                    result.Data.ItemNo = model.ItemNo;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(EDCResources.StringResource.CategoryDetail_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /EDC/CategoryDetail/Detail
        public async Task<ActionResult> Detail(string categoryName, string parameterName)
        {
            using (CategoryDetailServiceClient client = new CategoryDetailServiceClient())
            {
                MethodReturnResult<CategoryDetail> result = await client.GetAsync(new CategoryDetailKey()
                {
                    CategoryName = categoryName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    CategoryDetailViewModel viewModel = new CategoryDetailViewModel()
                    {
                        CategoryName = result.Data.Key.CategoryName,
                        ParameterName = result.Data.Key.ParameterName,
                        ItemNo = result.Data.ItemNo,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator
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
        // POST: /EDC/CategoryDetail/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string categoryName, string parameterName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (CategoryDetailServiceClient client = new CategoryDetailServiceClient())
            {
                result = await client.DeleteAsync(new CategoryDetailKey()
                {
                    CategoryName = categoryName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(EDCResources.StringResource.CategoryDetail_Delete_Success
                                                    , parameterName);
                }
                return Json(result);
            }
        }

        public ActionResult GetMaxItemNo(string categoryName)
        {
            using (CategoryDetailServiceClient client = new CategoryDetailServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.CategoryName='{0}'", categoryName),
                    OrderBy = "ItemNo Desc"
                };
                MethodReturnResult<IList<CategoryDetail>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    return Json(result.Data[0].ItemNo + 1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}