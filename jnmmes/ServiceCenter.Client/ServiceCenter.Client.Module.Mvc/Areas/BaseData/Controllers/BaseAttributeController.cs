using ServiceCenter.Client.Mvc.Areas.BaseData.Models;
using ServiceCenter.Client.Mvc.Resources.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.BaseData.Controllers
{
    public class BaseAttributeController : Controller
    {

        //
        // GET: /BaseData/BaseAttribute/
        public async Task<ActionResult> Index(string categoryName)
        {
            using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
            {
                MethodReturnResult<BaseAttributeCategory> result= await client.GetAsync(categoryName??string.Empty);
                if(result.Code>0 || result.Data==null)
                {
                    return RedirectToAction("Index", "BaseAttributeCategory");
                }
                ViewBag.BaseAttributeCategory = result.Data;
            }
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                await Task.Run(() =>
                {
                    string where = string.Format("Key.CategoryName='{0}'", categoryName);
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key.CategoryName,Order",
                        Where=where
                    };
                    MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);
                    
                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }

            return View(new BaseAttributeQueryViewModel() { CategoryName=categoryName });
        }
        //
        //POST: /BaseData/BaseAttribute/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(BaseAttributeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.CategoryName))
                            {
                                where.AppendFormat(" Key.CategoryName = '{0}'", model.CategoryName);
                            }
                            if (!string.IsNullOrEmpty(model.AttributeName))
                            {
                                where.AppendFormat("{0} Key.AttributeName LIKE '{1}%'"
                                                   , where.Length > 0 ? "AND" : string.Empty
                                                   , model.AttributeName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.CategoryName,Order",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);

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
        //POST: /BaseData/BaseAttribute/PagingQuery
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

                using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
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
                        MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);
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
        // POST: /BaseData/BaseAttribute/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(BaseAttributeViewModel model)
        {
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                BaseAttribute obj = new BaseAttribute()
                {
                    Key = new BaseAttributeKey()
                    { 
                        CategoryName=model.CategoryName,
                        AttributeName=model.AttributeName
                    },
                    IsPrimaryKey  =model.IsPrimaryKey,
                    DataType=model.DataType,
                    Order=model.Order,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(StringResource.BaseAttribute_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /BaseData/BaseAttribute/Modify
        public async Task<ActionResult> Modify(string categoryName,string attributeName)
        {
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                MethodReturnResult<BaseAttribute> result = await client.GetAsync(new BaseAttributeKey()
                {
                        CategoryName=categoryName,
                        AttributeName=attributeName
                });
                if (result.Code == 0)
                {
                    BaseAttributeViewModel viewModel = new BaseAttributeViewModel()
                    {
                        CategoryName=result.Data.Key.CategoryName,
                        AttributeName=result.Data.Key.AttributeName,
                        Order=result.Data.Order,
                        DataType=result.Data.DataType,
                        IsPrimaryKey=result.Data.IsPrimaryKey,
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
        // POST: /BaseData/BaseAttribute/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(BaseAttributeViewModel model)
        {
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                MethodReturnResult<BaseAttribute> result = await client.GetAsync(new BaseAttributeKey()
                    {
                         CategoryName=model.CategoryName,
                         AttributeName=model.AttributeName
                    });

                if (result.Code == 0)
                {
                    result.Data.DataType = model.DataType;
                    result.Data.IsPrimaryKey = model.IsPrimaryKey;
                    result.Data.Order = model.Order;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(StringResource.BaseAttribute_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /BaseData/BaseAttribute/Detail
        public async Task<ActionResult> Detail(string categoryName, string attributeName)
        {
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                MethodReturnResult<BaseAttribute> result = await client.GetAsync(new BaseAttributeKey()
                {
                    CategoryName = categoryName,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    BaseAttributeViewModel viewModel = new BaseAttributeViewModel()
                    {
                        CategoryName = result.Data.Key.CategoryName,
                        AttributeName = result.Data.Key.AttributeName,
                        Order = result.Data.Order,
                        DataType = result.Data.DataType,
                        IsPrimaryKey = result.Data.IsPrimaryKey,
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
        // POST: /BaseData/BaseAttribute/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string categoryName, string attributeName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                var key = new BaseAttributeKey()
                {
                    CategoryName = categoryName,
                    AttributeName = attributeName
                };
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.BaseAttribute_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
	}
}