using ServiceCenter.Client.Mvc.Areas.QAM.Models;
using QAMResources = ServiceCenter.Client.Mvc.Resources.QAM;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.MES.Service.Client.QAM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.QAM.Controllers
{
    public class CheckCategoryDetailController : Controller
    {

        //
        // GET: /QAM/CheckCategoryDetail/
        public async Task<ActionResult> Index(string categoryName)
        {
            using (CheckCategoryServiceClient client = new CheckCategoryServiceClient())
            {
                MethodReturnResult<CheckCategory> result = await client.GetAsync(categoryName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "CheckCategory");
                }
                ViewBag.CheckCategory = result.Data;
            }

            using (CheckCategoryDetailServiceClient client = new CheckCategoryDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "ItemNo",
                        Where = string.Format(" Key.CategoryName = '{0}'"
                                                    , categoryName)
                    };
                    MethodReturnResult<IList<CheckCategoryDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new CheckCategoryDetailQueryViewModel() { CategoryName = categoryName });
        }

        //
        //POST: /QAM/CheckCategoryDetail/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(CheckCategoryDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (CheckCategoryDetailServiceClient client = new CheckCategoryDetailServiceClient())
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
                        MethodReturnResult<IList<CheckCategoryDetail>> result = client.Get(ref cfg);

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
        //POST: /QAM/CheckCategoryDetail/PagingQuery
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

                using (CheckCategoryDetailServiceClient client = new CheckCategoryDetailServiceClient())
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
                        MethodReturnResult<IList<CheckCategoryDetail>> result = client.Get(ref cfg);
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
        // POST: /QAM/CheckCategoryDetail/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(CheckCategoryDetailViewModel model)
        {
            using (CheckCategoryDetailServiceClient client = new CheckCategoryDetailServiceClient())
            {
                CheckCategoryDetail obj = new CheckCategoryDetail()
                {
                    Key = new CheckCategoryDetailKey() {
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
                    rst.Message = string.Format(QAMResources.StringResource.CheckCategoryDetail_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /QAM/CheckCategoryDetail/Modify
        public async Task<ActionResult> Modify(string categoryName,string parameterName)
        {
            CheckCategoryDetailViewModel viewModel = new CheckCategoryDetailViewModel();
            using (CheckCategoryDetailServiceClient client = new CheckCategoryDetailServiceClient())
            {
                MethodReturnResult<CheckCategoryDetail> result = await client.GetAsync(new CheckCategoryDetailKey()
                {
                    CategoryName=categoryName,
                    ParameterName=parameterName
                });
                if (result.Code == 0)
                {
                    viewModel = new CheckCategoryDetailViewModel()
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
        // POST: /QAM/CheckCategoryDetail/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(CheckCategoryDetailViewModel model)
        {
            using (CheckCategoryDetailServiceClient client = new CheckCategoryDetailServiceClient())
            {
                MethodReturnResult<CheckCategoryDetail> result = await client.GetAsync(new CheckCategoryDetailKey()
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
                        rst.Message = string.Format(QAMResources.StringResource.CheckCategoryDetail_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /QAM/CheckCategoryDetail/Detail
        public async Task<ActionResult> Detail(string categoryName, string parameterName)
        {
            using (CheckCategoryDetailServiceClient client = new CheckCategoryDetailServiceClient())
            {
                MethodReturnResult<CheckCategoryDetail> result = await client.GetAsync(new CheckCategoryDetailKey()
                {
                    CategoryName = categoryName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    CheckCategoryDetailViewModel viewModel = new CheckCategoryDetailViewModel()
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
        // POST: /QAM/CheckCategoryDetail/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string categoryName, string parameterName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (CheckCategoryDetailServiceClient client = new CheckCategoryDetailServiceClient())
            {
                result = await client.DeleteAsync(new CheckCategoryDetailKey()
                {
                    CategoryName = categoryName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(QAMResources.StringResource.CheckCategoryDetail_Delete_Success
                                                    , parameterName);
                }
                return Json(result);
            }
        }

        public ActionResult GetMaxItemNo(string categoryName)
        {
            using (CheckCategoryDetailServiceClient client = new CheckCategoryDetailServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.CategoryName='{0}'", categoryName),
                    OrderBy = "ItemNo Desc"
                };
                MethodReturnResult<IList<CheckCategoryDetail>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    return Json(result.Data[0].ItemNo + 1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}