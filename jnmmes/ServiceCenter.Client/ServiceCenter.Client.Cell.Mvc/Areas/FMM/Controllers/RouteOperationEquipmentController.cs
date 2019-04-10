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
    public class RouteOperationEquipmentController : Controller
    {
        //
        // GET: /FMM/RouteOperationEquipment/
        public async Task<ActionResult> Index(string routeOperationName)
        {
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<RouteOperation> result = await client.GetAsync(routeOperationName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "RouteOperation");
                }
                ViewBag.RouteOperation = result.Data;
            }

            using (RouteOperationEquipmentServiceClient client = new RouteOperationEquipmentServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.RouteOperationName = '{0}'"
                                                    , routeOperationName)
                    };
                    MethodReturnResult<IList<RouteOperationEquipment>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RouteOperationEquipmentQueryViewModel() { RouteOperationName=routeOperationName });
        }

        //
        //POST: /FMM/RouteOperationEquipment/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RouteOperationEquipmentQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RouteOperationEquipmentServiceClient client = new RouteOperationEquipmentServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.RouteOperationName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteOperationName);

                            if (!string.IsNullOrEmpty(model.EquipmentCode))
                            {
                                where.AppendFormat(" {0} Key.EquipmentCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EquipmentCode);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<RouteOperationEquipment>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new RouteOperationEquipmentViewModel() { 
                 RouteOperationName=model.RouteOperationName
            });
        }
        //
        //POST: /FMM/RouteOperationEquipment/PagingQuery
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

                using (RouteOperationEquipmentServiceClient client = new RouteOperationEquipmentServiceClient())
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
                        MethodReturnResult<IList<RouteOperationEquipment>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new RouteOperationEquipmentViewModel() { });
        }
        //
        // POST: /FMM/RouteOperationEquipment/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RouteOperationEquipmentViewModel model)
        {
            using (RouteOperationEquipmentServiceClient client = new RouteOperationEquipmentServiceClient())
            {
                RouteOperationEquipment obj = new RouteOperationEquipment()
                {
                    Key = new RouteOperationEquipmentKey() { 
                         RouteOperationName=model.RouteOperationName,
                         EquipmentCode = model.EquipmentCode
                    },
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.RouteOperationEquipment_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        // POST: /FMM/RouteOperationEquipment/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string routeOperationName, string equipmentCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (RouteOperationEquipmentServiceClient client = new RouteOperationEquipmentServiceClient())
            {
                result = await client.DeleteAsync(new RouteOperationEquipmentKey()
                {
                    RouteOperationName = routeOperationName,
                    EquipmentCode = equipmentCode
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.RouteOperationEquipment_Delete_Success
                                                    , equipmentCode);
                }
                return Json(result);
            }
        }
    }
}