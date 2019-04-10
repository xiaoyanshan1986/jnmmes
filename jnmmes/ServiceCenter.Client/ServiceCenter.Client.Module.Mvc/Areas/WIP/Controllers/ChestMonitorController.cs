using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using System.Text;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class ChestMonitorController : Controller
    {
        public ActionResult Index()
        {
            return View(new ChestMonitorQueryViewModel());
        }

        public ActionResult Query()
        {
            ChestMonitorQueryViewModel model = new ChestMonitorQueryViewModel();
            return Query(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(ChestMonitorQueryViewModel model)
        {          
            if (ModelState.IsValid)
            {
                StringBuilder where = new StringBuilder();
                where.AppendFormat(string.Format(@" (ChestState = 0 OR ChestState = 6)"));
                if (model.OrderNumber != null && model.OrderNumber != "")
                {
                    where.AppendFormat(" {0} OrderNumber = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber.Trim().ToUpper());
                }

                if (model.MaterialCode != null && model.MaterialCode != "")
                {
                    where.AppendFormat(" {0} MaterialCode = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialCode.Trim().ToUpper());
                }

                if (model.PowerName != null && model.PowerName != "")
                {
                    where.AppendFormat(" {0} PowerName LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PowerName.Trim().ToUpper());
                }

                if (model.Grade != null && model.Grade != "")
                {
                    where.AppendFormat(" {0} Grade LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.Grade.Trim().ToUpper());
                }

                if (model.Color != null && model.Color != "")
                {
                    where.AppendFormat(" {0} Color LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.Color.Trim().ToUpper());
                }

                if (model.PowerSubCode != null && model.PowerSubCode != "")
                {
                    where.AppendFormat(" {0} PowerSubCode = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PowerSubCode.Trim().ToUpper());
                }
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "EditTime",
                        Where = where.ToString()
                    };
                    MethodReturnResult<IList<Chest>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;
                    }
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial",model);
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult QueryChestDetail(String chestNo)
        {

            if (!string.IsNullOrEmpty(chestNo))
            {
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    MethodReturnResult<Chest> result2 = client.Get(chestNo.Trim().ToUpper());
                    if (result2.Code == 0)
                    {
                        ViewBag.Chest = result2.Data;
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.ChestNo='{0}'", chestNo.Trim().ToUpper()),
                        OrderBy = "ItemNo"
                    };
                    MethodReturnResult<IList<ChestDetail>> result = client.GetDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.ChestDetailList = result.Data;
                    }
                }
            }
            return PartialView("_ListChestDetailQuery", new ChestMonitorViewModel());
        }

        public ActionResult RefreshChestList(string orderNumber, string materialCode, string grade, string powerName, string color, string powerSubCode )
        {           
            if (ModelState.IsValid)
            {
                StringBuilder where = new StringBuilder();
                where.AppendFormat(string.Format(@" (ChestState = 0 OR ChestState = 6)"));
                if (orderNumber != null && orderNumber != "")
                {
                    where.AppendFormat(" {0} OrderNumber = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , orderNumber.Trim().ToUpper());
                }

                if (materialCode != null && materialCode != "")
                {
                    where.AppendFormat(" {0} MaterialCode = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , materialCode.Trim().ToUpper());
                }

                if (powerName != null && powerName != "")
                {
                    where.AppendFormat(" {0} PowerName LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , powerName.Trim().ToUpper());
                }

                if (grade != null && grade != "")
                {
                    where.AppendFormat(" {0} Grade LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , grade.Trim().ToUpper());
                }

                if (color != null && color != "")
                {
                    where.AppendFormat(" {0} Color LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , color.Trim().ToUpper());
                }

                if (powerSubCode != null && powerSubCode != "")
                {
                    where.AppendFormat(" {0} PowerSubCode = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , powerSubCode.Trim().ToUpper());
                }
                using (PackageInChestServiceClient client = new PackageInChestServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "EditTime",
                        Where = where.ToString()
                    };
                    MethodReturnResult<IList<Chest>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;

                    }
                }
            }
            return PartialView("_ListPartial", new ChestMonitorQueryViewModel());
        }

        //获取工单号
        public ActionResult GetOrderNumber(string q)
        {
            IList<WorkOrder> lstDetail = new List<WorkOrder>();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key LIKE '{0}%' AND OrderState='0' AND CloseType=0"
                                           , q),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            var lnq = from item in lstDetail
                      select item.Key;

            return Json(from item in lstDetail
                        select new
                        {
                            @label = item.Key,
                            @value = item.Key,
                        }, JsonRequestBehavior.AllowGet);
        }

    }
}