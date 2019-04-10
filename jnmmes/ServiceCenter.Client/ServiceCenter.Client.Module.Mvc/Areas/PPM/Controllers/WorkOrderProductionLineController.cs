using ServiceCenter.Client.Mvc.Areas.PPM.Models;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Controllers
{
    public class WorkOrderProductionLineController : Controller
    {

        //
        // GET: /PPM/WorkOrderProductionLine/
        public async Task<ActionResult> Index(string orderNumber)
        {
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = await client.GetAsync(orderNumber ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "WorkOrder");
                }
                ViewBag.WorkOrder = result.Data;
            }

            using (WorkOrderProductionLineServiceClient client = new WorkOrderProductionLineServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.OrderNumber = '{0}'"
                                                    , orderNumber)
                    };
                    MethodReturnResult<IList<WorkOrderProductionLine>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderProductionLineQueryViewModel() { OrderNumber=orderNumber });
        }

        //
        //POST: /PPM/WorkOrderProductionLine/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderProductionLineQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderProductionLineServiceClient client = new WorkOrderProductionLineServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.OrderNumber = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);

                            if (!string.IsNullOrEmpty(model.LineCode))
                            {
                                where.AppendFormat(" {0} Key.LineCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LineCode);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderProductionLine>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new WorkOrderProductionLineViewModel());
        }
        //
        //POST: /PPM/WorkOrderProductionLine/PagingQuery
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

                using (WorkOrderProductionLineServiceClient client = new WorkOrderProductionLineServiceClient())
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
                        MethodReturnResult<IList<WorkOrderProductionLine>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new WorkOrderProductionLineViewModel());
        }
        //
        // POST: /PPM/WorkOrderProductionLine/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderProductionLineViewModel model)
        {
            using (WorkOrderProductionLineServiceClient client = new WorkOrderProductionLineServiceClient())
            {
                WorkOrderProductionLine obj = new WorkOrderProductionLine()
                {
                    Key = new WorkOrderProductionLineKey() {
                        OrderNumber = model.OrderNumber.ToUpper(),
                        LineCode = model.LineCode.ToUpper()
                    },
                    CreateTime = DateTime.Now,
                    Creator=User.Identity.Name,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(PPMResources.StringResource.WorkOrderProductionLine_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // POST: /PPM/WorkOrderProductionLine/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string orderNumber, string lineCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WorkOrderProductionLineServiceClient client = new WorkOrderProductionLineServiceClient())
            {
                result = await client.DeleteAsync(new WorkOrderProductionLineKey()
                {
                    OrderNumber = orderNumber,
                    LineCode = lineCode
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(PPMResources.StringResource.WorkOrderProductionLine_Delete_Success
                                                    , lineCode);
                }
                return Json(result);
            }
        }
    }
}