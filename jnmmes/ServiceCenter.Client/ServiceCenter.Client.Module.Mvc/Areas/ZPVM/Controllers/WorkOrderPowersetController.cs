using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class WorkOrderPowersetController : Controller
    {
        //
        // GET: /ZPVM/WorkOrderPowerset/
        public async Task<ActionResult> Index(string OrderNumber, string MaterialCode)
        {
            if (string.IsNullOrEmpty(OrderNumber) || string.IsNullOrEmpty(MaterialCode))
            {
                return RedirectToAction("Index", "WorkOrderRule");
            }

            using (WorkOrderRuleServiceClient client = new WorkOrderRuleServiceClient())
            {
                MethodReturnResult<WorkOrderRule> result = await client.GetAsync(new WorkOrderRuleKey()
                {
                    OrderNumber = OrderNumber,
                    MaterialCode = MaterialCode
                });

                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "WorkOrderRule");
                }
                ViewBag.Rule = result.Data;
            }

            using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.OrderNumber = '{0}' AND Key.MaterialCode='{1}'"
                                              , OrderNumber
                                              , MaterialCode),
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<WorkOrderPowerset>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderPowersetQueryViewModel() { OrderNumber = OrderNumber, MaterialCode = MaterialCode });
        }

        //
        //POST: /ZPVM/WorkOrderPowerset/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderPowersetQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" Key.OrderNumber = '{0}' AND Key.MaterialCode='{1}'"
                                              , model.OrderNumber
                                              , model.MaterialCode);

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderPowerset>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/WorkOrderPowerset/PagingQuery
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

                using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
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
                        MethodReturnResult<IList<WorkOrderPowerset>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/WorkOrderPowerset/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderPowersetViewModel model)
        {
            using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            {
                WorkOrderPowerset obj = new WorkOrderPowerset()
                {
                    Key = new WorkOrderPowersetKey(){
                        OrderNumber=model.OrderNumber,
                        MaterialCode=model.MaterialCode,
                        Code = model.Code.ToUpper(),
                        ItemNo=model.ItemNo??0
                    },
                    ArticleNo = model.ArticleNo,
                    MaxValue = model.MaxValue,
                    MinValue = model.MinValue,
                    Name=model.Name.ToUpper(),
                    PowerDifference=model.PowerDifference,
                    PowerName=model.PowerName.ToUpper(),
                    StandardFuse=model.StandardFuse,
                    StandardIPM=model.StandardIPM,
                    StandardIsc=model.StandardIsc,
                    StandardPower=model.StandardPower,
                    StandardVoc=model.StandardVoc,
                    StandardVPM=model.StandardVPM,
                    SubWay=model.SubWay,
                    Description = model.Description,
                    IsUsed=model.IsUsed,
                    MixColor = model.MixColor,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderPowerset_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/WorkOrderPowerset/Modify
        public async Task<ActionResult> Modify(string orderNumber,string materialCode,string code,int itemNo)
        {
            WorkOrderPowersetViewModel viewModel = new WorkOrderPowersetViewModel();
            using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            {
                MethodReturnResult<WorkOrderPowerset> result = await client.GetAsync(new WorkOrderPowersetKey()
                {
                    OrderNumber = orderNumber,
                    MaterialCode = materialCode,
                    Code=code,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderPowersetViewModel()
                    {
                        OrderNumber=result.Data.Key.OrderNumber,
                        MaterialCode=result.Data.Key.MaterialCode,
                        Code=result.Data.Key.Code,
                        ArticleNo=result.Data.ArticleNo,
                        ItemNo=result.Data.Key.ItemNo,
                        PowerName=result.Data.PowerName,
                        SubWay=result.Data.SubWay,
                        StandardVPM=result.Data.StandardVPM,
                        StandardVoc=result.Data.StandardVoc,
                        StandardPower=result.Data.StandardPower,
                        StandardIsc=result.Data.StandardIsc,
                        StandardIPM=result.Data.StandardIPM,
                        StandardFuse=result.Data.StandardFuse,
                        PowerDifference=result.Data.PowerDifference,
                        MaxValue=result.Data.MaxValue,
                        MinValue=result.Data.MinValue,
                        Name=result.Data.Name,
                        Description=result.Data.Description,
                        MixColor = result.Data.MixColor,
                        IsUsed=result.Data.IsUsed,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
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
            return PartialView("_ModifyPartial", new WorkOrderPowersetViewModel());
        }

        //
        // POST: /ZPVM/WorkOrderPowerset/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderPowersetViewModel model)
        {
            using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            {
                WorkOrderPowersetKey key = new WorkOrderPowersetKey()
                {
                    OrderNumber = model.OrderNumber,
                    MaterialCode = model.MaterialCode,
                    Code = model.Code,
                    ItemNo = model.ItemNo ?? 0
                };
                MethodReturnResult<WorkOrderPowerset> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.ArticleNo = model.ArticleNo;
                    result.Data.MaxValue = model.MaxValue;
                    result.Data.MinValue = model.MinValue;
                    result.Data.Name = model.Name;
                    result.Data.PowerDifference = model.PowerDifference;
                    result.Data.PowerName = model.PowerName;
                    result.Data.StandardFuse = model.StandardFuse;
                    result.Data.StandardIPM = model.StandardIPM;
                    result.Data.StandardIsc = model.StandardIsc;
                    result.Data.StandardPower = model.StandardPower;
                    result.Data.StandardVoc = model.StandardVoc;
                    result.Data.StandardVPM = model.StandardVPM;
                    result.Data.SubWay = model.SubWay;
                    result.Data.Description = model.Description;
                    result.Data.MixColor = model.MixColor;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderPowerset_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }


            //using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            //{                
            //    WorkOrderPowerset orderPowerset = new WorkOrderPowerset();

            //    orderPowerset.ArticleNo = model.ArticleNo;
            //    orderPowerset.MaxValue = model.MaxValue;
            //    orderPowerset.MinValue = model.MinValue;
            //    orderPowerset.Name = model.Name;
            //    orderPowerset.PowerDifference = model.PowerDifference;
            //    orderPowerset.PowerName = model.PowerName;
            //    orderPowerset.StandardFuse = model.StandardFuse;
            //    orderPowerset.StandardIPM = model.StandardIPM;
            //    orderPowerset.StandardIsc = model.StandardIsc;
            //    orderPowerset.StandardPower = model.StandardPower;
            //    orderPowerset.StandardVoc = model.StandardVoc;
            //    orderPowerset.StandardVPM = model.StandardVPM;
            //    orderPowerset.SubWay = model.SubWay;
            //    orderPowerset.Description = model.Description;
            //    orderPowerset.MixColor = model.MixColor;
            //    orderPowerset.IsUsed = model.IsUsed;
            //    orderPowerset.Editor = User.Identity.Name;
            //    orderPowerset.EditTime = DateTime.Now;

            //    MethodReturnResult rst = await client.ModifyAsync(orderPowerset);

            //    if (rst.Code == 0)
            //    {
            //        rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderPowerset_SaveModify_Success
            //                                    , model.Code);
            //    }

            //    return Json(rst);                
            //}
        }
        //
        // GET: /ZPVM/WorkOrderPowerset/Detail
        public async Task<ActionResult> Detail(string orderNumber, string materialCode, string code, int itemNo)
        {
            using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            {
                WorkOrderPowersetKey key = new WorkOrderPowersetKey()
                {
                    OrderNumber = orderNumber,
                    MaterialCode = materialCode,
                    Code = code,
                    ItemNo = itemNo
                };
                MethodReturnResult<WorkOrderPowerset> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    WorkOrderPowersetViewModel viewModel = new WorkOrderPowersetViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        MaterialCode = result.Data.Key.MaterialCode,
                        Code = result.Data.Key.Code,
                        ArticleNo = result.Data.ArticleNo,
                        ItemNo = result.Data.Key.ItemNo,
                        PowerName = result.Data.PowerName,
                        SubWay = result.Data.SubWay,
                        StandardVPM = result.Data.StandardVPM,
                        StandardVoc = result.Data.StandardVoc,
                        StandardPower = result.Data.StandardPower,
                        StandardIsc = result.Data.StandardIsc,
                        StandardIPM = result.Data.StandardIPM,
                        StandardFuse = result.Data.StandardFuse,
                        PowerDifference = result.Data.PowerDifference,
                        MaxValue = result.Data.MaxValue,
                        MinValue = result.Data.MinValue,
                        Name = result.Data.Name,
                        Description = result.Data.Description,
                        MixColor = result.Data.MixColor,
                        IsUsed = result.Data.IsUsed,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
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
            return PartialView("_InfoPartial", new WorkOrderPowersetViewModel());
        }
        //
        // POST: /ZPVM/WorkOrderPowerset/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string orderNumber, string materialCode, string code, int itemNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            WorkOrderPowersetKey key = new WorkOrderPowersetKey()
            {
                OrderNumber = orderNumber,
                MaterialCode = materialCode,
                Code = code,
                ItemNo = itemNo
            };
            using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.WorkOrderPowerset_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }

        public ActionResult GetPowersetCode(string q)
        {
            IList<Powerset> lstDetail = new List<Powerset>();
            using (PowersetServiceClient client = new PowersetServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"CONCAT(Key.Code ,cast(Key.ItemNo as string))  LIKE '{0}%'
                                            AND IsUsed=1"
                                           , q),
                     
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<Powerset>> result = client.Get(ref cfg);
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
                            @label = item.Key + "-" + item.Name+"-"+item.PowerName,
                            @value = item.Key.Code+ item.Key.ItemNo.ToString(),
                            //@value = item.Key.Code,
                            @Data=item,
                            @SubWay=item.SubWay.ToString()
                        }, JsonRequestBehavior.AllowGet);
        }
    }
}