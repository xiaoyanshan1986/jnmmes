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
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Controllers
{
    public class WorkOrderBOMController : Controller
    {

        //
        // GET: /PPM/WorkOrderBOM/
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

            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.OrderNumber = '{0}'"
                                                    , orderNumber),
                        OrderBy = "Key.ItemNo"
                    };
                    MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }

            //return View(new WorkOrderBOMViewModel() { OrderNumber = orderNumber });
            return View(new WorkOrderBOMQueryViewModel() { OrderNumber = orderNumber });
        }

        //
        //POST: /PPM/WorkOrderBOM/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderBOMQueryViewModel model)
        {
            //if (ModelState.IsValid)
            //{
                using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.OrderNumber = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);

                            if (!string.IsNullOrEmpty(model.MaterialCode))
                            {
                                where.AppendFormat(" {0} MaterialCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.MaterialCode);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.ItemNo",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            //}
                return PartialView("_ListPartial", new WorkOrderBOMViewModel() { OrderNumber = model.OrderNumber });
        }

        //
        //POST: /PPM/WorkOrderBOM/PagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
        {
            //if (ModelState.IsValid)
            //{
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

                using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
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
                        MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            //}

            return PartialView("_ListPartial");
        }
        //
        // POST: /PPM/WorkOrderBOM/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderBOMViewModel model)
        {
            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                WorkOrderBOM obj = new WorkOrderBOM()
                {
                    Key = new WorkOrderBOMKey()
                    {
                        OrderNumber = model.OrderNumber.ToUpper(),
                        ItemNo = model.ItemNo
                    },
                    MaterialCode = model.MaterialCode.ToUpper(),
                    MaterialUnit=model.MaterialUnit,
                    Qty= Convert.ToDecimal(model.Qty),
                    MinUnit = Convert.ToDecimal(model.MinUnit),
                    ReplaceMaterial = model.ReplaceMaterial,
                    StoreLocation=model.StoreLocation,
                    WorkCenter=model.WorkCenter,
                    Description=model.Description,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(PPMResources.StringResource.WorkOrderBOM_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /PPM/WorkOrderBOM/Modify
        public async Task<ActionResult> Modify(string orderNumber, int itemNo)
        {
            WorkOrderBOMViewModel viewModel = new WorkOrderBOMViewModel();
            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                MethodReturnResult<WorkOrderBOM> result = await client.GetAsync(new WorkOrderBOMKey()
                {
                    OrderNumber = orderNumber,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderBOMViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        ItemNo = result.Data.Key.ItemNo,
                        MaterialCode = result.Data.MaterialCode,
                        MaterialName = viewModel.GetMaterialName(result.Data.MaterialCode).ToString(),
                        WorkCenter=result.Data.WorkCenter,
                        StoreLocation=result.Data.StoreLocation,
                        Qty=Convert.ToDouble(result.Data.Qty),
                        MaterialUnit=result.Data.MaterialUnit,
                        MinUnit = Convert.ToDouble(result.Data.MinUnit),
                        ReplaceMaterial = result.Data.ReplaceMaterial,
                        Description=result.Data.Description,
                        CreateTime=result.Data.CreateTime,
                        Creator=result.Data.Creator,
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
        // POST: /PPM/WorkOrderBOM/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderBOMViewModel model)
        {
            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                MethodReturnResult<WorkOrderBOM> result = await client.GetAsync(new WorkOrderBOMKey()
                {
                    OrderNumber = model.OrderNumber.ToUpper(),
                    ItemNo = model.ItemNo
                });

                if (result.Code == 0)
                {
                    result.Data.MaterialCode = model.MaterialCode.ToUpper();
                    result.Data.MaterialUnit = model.MaterialUnit;
                    result.Data.Qty = Convert.ToDecimal(model.Qty);
                    result.Data.MinUnit = Convert.ToDecimal(model.MinUnit);
                    result.Data.ReplaceMaterial = model.ReplaceMaterial;
                    result.Data.StoreLocation = model.StoreLocation;
                    result.Data.WorkCenter = model.WorkCenter;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;

                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(PPMResources.StringResource.WorkOrderBOM_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /PPM/WorkOrderBOM/Detail
        public async Task<ActionResult> Detail(string orderNumber, int itemNo)
        {
            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                WorkOrderBOMViewModel viewModel = new WorkOrderBOMViewModel();

                MethodReturnResult<WorkOrderBOM> result = await client.GetAsync(new WorkOrderBOMKey()
                {
                    OrderNumber = orderNumber,
                    ItemNo = itemNo
                });

                if (result.Code == 0)
                {
                    viewModel = new WorkOrderBOMViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        ItemNo = result.Data.Key.ItemNo,
                        WorkCenter = result.Data.WorkCenter,
                        StoreLocation=result.Data.StoreLocation,
                        Qty=Convert.ToDouble(result.Data.Qty),
                        MaterialUnit=result.Data.MaterialUnit,
                        MaterialCode=result.Data.MaterialCode,
                        MaterialName = viewModel.GetMaterialName(result.Data.MaterialCode).ToString(),
                        MinUnit = Convert.ToDouble(result.Data.MinUnit),
                        ReplaceMaterial = result.Data.ReplaceMaterial,
                        Description=result.Data.Description,
                        Creator=result.Data.Creator,
                        CreateTime=result.Data.CreateTime,
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
        // POST: /PPM/WorkOrderBOM/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string orderNumber, int itemNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                result = await client.DeleteAsync(new WorkOrderBOMKey()
                {
                    OrderNumber = orderNumber,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(PPMResources.StringResource.WorkOrderBOM_Delete_Success
                                                    , itemNo);
                }
                return Json(result);
            }
        }

        public ActionResult GetMaxItemNo(string orderNumber)
        {
            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.OrderNumber='{0}'", orderNumber),
                    OrderBy = "Key.ItemNo Desc"
                };
                MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    return Json(result.Data[0].Key.ItemNo + 1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 取得物料计量单位
        /// </summary>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        public ActionResult GetMaterialUnit(string materialCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    return Json(result.Data.Unit, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得物料名称
        /// </summary>
        /// <param name="materialCode">物料代码</param>
        /// <returns>物料名称</returns>
        public ActionResult GetMaterialName(string materialCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    return Json(result.Data.Name, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialAttribute(string materialCode)
        {
            MethodReturnResult<Material> result = null;

            var item = new
            {
                @Name = string.Empty,
                @Unit = string.Empty
            };

            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                result = client.Get(materialCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    item = new
                    {
                        @Name = result.Data.Name,
                        @Unit = result.Data.Unit
                    };
                }
            }

            return Json(item, JsonRequestBehavior.AllowGet);
        }
    }
}