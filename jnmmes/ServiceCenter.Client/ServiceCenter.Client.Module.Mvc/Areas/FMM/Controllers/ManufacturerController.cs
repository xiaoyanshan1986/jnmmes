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
    public class ManufacturerController : Controller
    {
        //
        // GET: /FMM/Manufacturer/
        public async Task<ActionResult> Index()
        {
            using (ManufacturerServiceClient client = new ManufacturerServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<Manufacturer>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new ManufacturerQueryViewModel());
        }

        //
        //POST: /FMM/Manufacturer/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(ManufacturerQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ManufacturerServiceClient client = new ManufacturerServiceClient())
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
                            if (!string.IsNullOrEmpty(model.NickName))
                            {
                                where.AppendFormat(" {0} NickName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.NickName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Manufacturer>> result = client.Get(ref cfg);

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
        //POST: /FMM/Manufacturer/PagingQuery
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

                using (ManufacturerServiceClient client = new ManufacturerServiceClient())
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
                        MethodReturnResult<IList<Manufacturer>> result = client.Get(ref cfg);
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
        // POST: /FMM/Manufacturer/Save
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ManufacturerViewModel model)
        {
            using (ManufacturerServiceClient client = new ManufacturerServiceClient())
            {
                Manufacturer obj = new Manufacturer()
                {
                    Key = model.Code.ToString().Trim(), //新增供应商代码不可包含空格
                    Name = model.Name,                  //由于现有错误，供应商名称可以有空格
                    NickName = model.NickName,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.Manufacturer_Save_Success
                                                , model.Code);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/Manufacturer/Modify
        public async Task<ActionResult> Modify(string key)
        {
            ManufacturerViewModel viewModel = new ManufacturerViewModel();
            using (ManufacturerServiceClient client = new ManufacturerServiceClient())
            {
                MethodReturnResult<Manufacturer> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new ManufacturerViewModel()
                    {
                        Name = result.Data.Name,
                        Code =result.Data.Key,
                        NickName=result.Data.NickName,
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
        // POST: /FMM/Manufacturer/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(ManufacturerViewModel model)
        {
            using (ManufacturerServiceClient client = new ManufacturerServiceClient())
            {
                MethodReturnResult<Manufacturer> result = await client.GetAsync(model.Code);

                if (result.Code == 0)
                {
                    result.Data.Name = model.Name;
                    result.Data.NickName = model.NickName;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.Manufacturer_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/Manufacturer/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (ManufacturerServiceClient client = new ManufacturerServiceClient())
            {
                MethodReturnResult<Manufacturer> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    ManufacturerViewModel viewModel = new ManufacturerViewModel()
                    {
                        Name = result.Data.Name,
                        Code = result.Data.Key,
                        NickName = result.Data.NickName,
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
        // POST: /FMM/Manufacturer/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ManufacturerServiceClient client = new ManufacturerServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.Manufacturer_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}