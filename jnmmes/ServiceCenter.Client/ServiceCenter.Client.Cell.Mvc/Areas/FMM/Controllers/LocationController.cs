using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using FMMResources=ServiceCenter.Client.Mvc.Resources.FMM;
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
    public class LocationController : Controller
    {

        //
        // GET: /FMM/Location/
        public async Task<ActionResult> Index()
        {
            using (LocationServiceClient client = new LocationServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key,Level"
                    };
                    MethodReturnResult<IList<Location>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            SetViewData();
            return View(new LocationQueryViewModel());
        }

        private void SetViewData()
        {
            ViewData["ParentLocationName"] = new List<SelectListItem>();
        }
        //
        //POST: /FMM/Location/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(LocationQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (LocationServiceClient client = new LocationServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (model.Level!=null)
                            {
                                where.AppendFormat("  Level = '{0}'"
                                                    , Convert.ToInt32(model.Level));
                            }
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }

                            if (!string.IsNullOrEmpty(model.ParentLocationName))
                            {
                                where.AppendFormat(" {0} ParentLocationName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ParentLocationName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key,Level",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Location>> result = client.Get(ref cfg);

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
        //POST: /FMM/Location/PagingQuery
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

                using (LocationServiceClient client = new LocationServiceClient())
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
                        MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
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
        // POST: /FMM/Location/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(LocationViewModel model)
        {
            using (LocationServiceClient client = new LocationServiceClient())
            {
                Location obj = new Location()
                {
                    Key = model.Name,
                    Description = model.Description,
                    ParentLocationName=model.ParentLocationName,
                    Level=model.Level,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.Location_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/Location/Modify
        public async Task<ActionResult> Modify(string name)
        {
            LocationViewModel viewModel = new LocationViewModel();
            using (LocationServiceClient client = new LocationServiceClient())
            {
                MethodReturnResult<Location> result = await client.GetAsync(name);
                if (result.Code == 0)
                {
                    viewModel = new LocationViewModel()
                    {
                        Name = result.Data.Key,
                        ParentLocationName =result.Data.ParentLocationName,
                        Level=result.Data.Level,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    SetParentLocationNameViewData("ParentLocationName", viewModel.Level);

                    return PartialView("_ModifyPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_ModifyPartial");
        }

        private void SetParentLocationNameViewData(string viewDataName, LocationLevel level)
        {
            SetViewData();
            if (level != LocationLevel.Factory)
            {
                using (LocationServiceClient client = new LocationServiceClient())
                {
                    LocationLevel parentLevel = LocationLevel.Factory;
                    if (level == LocationLevel.Area)
                    {
                        parentLevel = LocationLevel.Room;
                    }

                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Level='{0}'", Convert.ToInt32(parentLevel))
                    };

                    MethodReturnResult<IList<Location>> resultParentLocation = client.Get(ref cfg);
                    if (resultParentLocation.Code <= 0)
                    {
                        IEnumerable<SelectListItem> lst = from item in resultParentLocation.Data
                                                          select new SelectListItem()
                                                          {
                                                              Text = item.Key,
                                                              Value = item.Key
                                                          };
                        ViewData[viewDataName] = lst;
                    }
                }
            }
        }
        //
        // POST: /FMM/Location/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(LocationViewModel model)
        {
            using (LocationServiceClient client = new LocationServiceClient())
            {
                MethodReturnResult<Location> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.Level = model.Level;
                    result.Data.ParentLocationName = model.ParentLocationName;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.Location_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/Location/Detail
        public async Task<ActionResult> Detail(string name)
        {
            using (LocationServiceClient client = new LocationServiceClient())
            {
                MethodReturnResult<Location> result = await client.GetAsync(name);
                if (result.Code == 0)
                {
                    LocationViewModel viewModel = new LocationViewModel()
                    {
                        Name = result.Data.Key,
                        ParentLocationName = result.Data.ParentLocationName,
                        Level = result.Data.Level,
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
        // POST: /FMM/Location/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string name)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (LocationServiceClient client = new LocationServiceClient())
            {
                result = await client.DeleteAsync(name);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.Location_Delete_Success
                                                    , name);
                }
                return Json(result);
            }
        }


        public ActionResult GetParentLocations(LocationLevel? level)
        {
            if (level != null && level != LocationLevel.Factory)
            {
                using (LocationServiceClient client = new LocationServiceClient())
                {
                    LocationLevel parentLevel = LocationLevel.Factory;
                    if (level == LocationLevel.Area)
                    {
                        parentLevel = LocationLevel.Room;
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Level='{0}'", Convert.ToInt32(parentLevel))
                    };
                    MethodReturnResult<IList<Location>> result = client.Get(ref cfg);
                    if (result.Code <= 0)
                    {
                        if (Request.IsAjaxRequest())
                        {
                            return Json(result.Data, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return View("");
                        }
                    }
                }
            }
            return Json(new List<Location>(), JsonRequestBehavior.AllowGet);
        }
	}
}