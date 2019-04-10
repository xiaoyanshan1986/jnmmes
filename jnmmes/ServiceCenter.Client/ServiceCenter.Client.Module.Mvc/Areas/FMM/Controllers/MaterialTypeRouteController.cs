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
    public class MaterialTypeRouteController : Controller
    {

        //
        // GET: /FMM/MaterialTypeRoute/
        public async Task<ActionResult> Index(string materialType)
        {
            using (MaterialTypeServiceClient client = new MaterialTypeServiceClient())
            {
                MethodReturnResult<MaterialType> result = await client.GetAsync(materialType ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "MaterialType");
                }
                ViewBag.MaterialType = result.Data;
            }

            using (MaterialTypeRouteServiceClient client = new MaterialTypeRouteServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.MaterialType = '{0}'"
                                                    , materialType),
                        OrderBy = "Key.LocationName"
                    };
                    MethodReturnResult<IList<MaterialTypeRoute>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            
            return View(new MaterialTypeRouteQueryViewModel() { MaterialType = materialType });

        }

        //
        //POST: /FMM/MaterialTypeRoute/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialTypeRouteQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialTypeRouteServiceClient client = new MaterialTypeRouteServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();

                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.MaterialType = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.MaterialType);

                            if (!string.IsNullOrEmpty(model.LocationName))
                            {
                                where.AppendFormat(" {0} Key.LocationName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LocationName);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.LocationName",
                            Where = where.ToString()
                        };

                        MethodReturnResult<IList<MaterialTypeRoute>> result = client.Get(ref cfg);

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
        //POST: /FMM/MaterialTypeRoute/PagingQuery
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

                using (MaterialTypeRouteServiceClient client = new MaterialTypeRouteServiceClient())
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
                        MethodReturnResult<IList<MaterialTypeRoute>> result = client.Get(ref cfg);
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
        // POST: /FMM/MaterialTypeRoute/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialTypeRouteViewModel model)
        {
            using (MaterialTypeRouteServiceClient client = new MaterialTypeRouteServiceClient())
            {
                MaterialTypeRoute obj = new MaterialTypeRoute()
                {
                    Key = new MaterialTypeRouteKey() {
                        MaterialType = model.MaterialType,
                        LocationName = model.LocationName,
                        RouteEnterpriseName=model.RouteEnterpriseName,
                        IsRework=model.IsRework
                    },
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.MaterialTypeRoute_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }

        //
        // POST: /FMM/MaterialTypeRoute/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string materialType, string locationName, string routeEnterpriseName,bool isRework)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialTypeRouteServiceClient client = new MaterialTypeRouteServiceClient())
            {
                result = await client.DeleteAsync(new MaterialTypeRouteKey()
                {
                    MaterialType = materialType,
                    LocationName = locationName,
                    RouteEnterpriseName = routeEnterpriseName,
                    IsRework=isRework
                });
                if (result.Code == 0)
                {
                    string name = string.Format("{0}-{1}", locationName, routeEnterpriseName);
                    result.Message = string.Format(FMMResources.StringResource.MaterialTypeRoute_Delete_Success
                                                    , name);
                }
                return Json(result);
            }
        }
    }
}