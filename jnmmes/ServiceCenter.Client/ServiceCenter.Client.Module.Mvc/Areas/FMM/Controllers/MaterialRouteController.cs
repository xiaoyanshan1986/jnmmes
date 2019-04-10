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
    public class MaterialRouteController : Controller
    {
        /// <summary> 初始化列表 </summary>
        /// <param name="materialType"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string materialCode)
        {
            //取得产品物料对象
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = await client.GetAsync(materialCode ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "MaterialCode");
                }

                ViewBag.Material = result.Data;
            }

            //取得产品工艺流程设置
            using (MaterialRouteServiceClient client = new MaterialRouteServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.MaterialCode = '{0}'"
                                                    , materialCode),
                        OrderBy = "Key.LocationName"
                    };

                    MethodReturnResult<IList<MaterialRoute>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            
            return View(new MaterialRouteQueryViewModel() { MaterialCode = materialCode });
        }

        /// <summary> 查询 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialRouteQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialRouteServiceClient client = new MaterialRouteServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();

                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.MaterialCode = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.MaterialCode);

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

                        MethodReturnResult<IList<MaterialRoute>> result = client.Get(ref cfg);

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

        /// <summary> 分页查询 </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="currentPageNo"></param>
        /// <param name="currentPageSize"></param>
        /// <returns></returns>
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

                using (MaterialRouteServiceClient client = new MaterialRouteServiceClient())
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
                        MethodReturnResult<IList<MaterialRoute>> result = client.Get(ref cfg);
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

        /// <summary> 产品加工工艺组保存 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialRouteViewModel model)
        {
            using (MaterialRouteServiceClient client = new MaterialRouteServiceClient())
            {
                MaterialRoute obj = new MaterialRoute()
                {
                    Key = new MaterialRouteKey() {
                        MaterialCode = model.MaterialCode,
                        LocationName = model.LocationName,
                        RouteEnterpriseName = model.RouteEnterpriseName
                    },
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name
                };

                MethodReturnResult rst = await client.AddAsync(obj);

                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.MaterialRoute_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }

        /// <summary> 删除产品加工工艺流程组 </summary>
        /// <param name="materialCode">产品代码</param>
        /// <param name="locationName">车间名称</param>
        /// <param name="routeEnterpriseName">工艺流程组</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string materialCode, string locationName, string routeEnterpriseName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (MaterialRouteServiceClient client = new MaterialRouteServiceClient())
            {
                result = await client.DeleteAsync(new MaterialRouteKey()
                {
                    MaterialCode = materialCode,
                    LocationName = locationName,
                    RouteEnterpriseName = routeEnterpriseName
                });

                if (result.Code == 0)
                {
                    string name = string.Format("{0}-{1}", locationName, routeEnterpriseName);
                    result.Message = string.Format(FMMResources.StringResource.MaterialRoute_Delete_Success
                                                    , name);
                }

                return Json(result);
            }
        }
    }
}