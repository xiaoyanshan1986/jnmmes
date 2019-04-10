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
    public class EquipmentReasonCodeCategoryDetailController : Controller
    {

        //
        // GET: /FMM/ReasonCodeCategoryDetail/
        public async Task<ActionResult> Index(string categoryName)
        {
            using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
            {
                MethodReturnResult<EquipmentReasonCodeCategory> result = await client.GetAsync(categoryName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "ReasonCodeCategory");
                }
                ViewBag.ReasonCodeCategory = result.Data;
            }

            using (EquipmentReasonCodeCategoryDetailServiceClient client = new EquipmentReasonCodeCategoryDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "ItemNo",
                        Where = string.Format(" Key.ReasonCodeCategoryName = '{0}'"
                                                    , categoryName)
                    };
                    MethodReturnResult<IList<EquipmentReasonCodeCategoryDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new EquipmentReasonCodeCategoryDetailQueryViewModel() { ReasonCodeCategoryName = categoryName });
        }

        //
        //POST: /FMM/ReasonCodeCategoryDetail/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(EquipmentReasonCodeCategoryDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (EquipmentReasonCodeCategoryDetailServiceClient client = new EquipmentReasonCodeCategoryDetailServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.ReasonCodeCategoryName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ReasonCodeCategoryName);

                            if (!string.IsNullOrEmpty(model.ReasonCodeName))
                            {
                                where.AppendFormat(" {0} Key.ReasonCodeName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ReasonCodeName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "ItemNo",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<EquipmentReasonCodeCategoryDetail>> result = client.Get(ref cfg);

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
        //POST: /FMM/ReasonCodeCategoryDetail/PagingQuery
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

                using (EquipmentReasonCodeCategoryDetailServiceClient client = new EquipmentReasonCodeCategoryDetailServiceClient())
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
                        MethodReturnResult<IList<EquipmentReasonCodeCategoryDetail>> result = client.Get(ref cfg);
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
        // POST: /FMM/ReasonCodeCategoryDetail/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(EquipmentReasonCodeCategoryDetailViewModel model)
        {
            using (EquipmentReasonCodeCategoryDetailServiceClient client = new EquipmentReasonCodeCategoryDetailServiceClient())
            {
                EquipmentReasonCodeCategoryDetail obj = new EquipmentReasonCodeCategoryDetail()
                {
                    Key = new EquipmentReasonCodeCategoryDetailKey()
                    {
                        ReasonCodeCategoryName = model.ReasonCodeCategoryName,
                        ReasonCodeName = model.ReasonCodeName
                    },
                    ItemNo = model.ItemNo,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.ReasonCodeCategoryDetail_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/ReasonCodeCategoryDetail/Modify
        public async Task<ActionResult> Modify(string categoryName,string reasonCodeName)
        {
            EquipmentReasonCodeCategoryDetailViewModel viewModel = new EquipmentReasonCodeCategoryDetailViewModel();
            using (EquipmentReasonCodeCategoryDetailServiceClient client = new EquipmentReasonCodeCategoryDetailServiceClient())
            {
                MethodReturnResult<EquipmentReasonCodeCategoryDetail> result = await client.GetAsync(new EquipmentReasonCodeCategoryDetailKey()
                {
                    ReasonCodeCategoryName=categoryName,
                    ReasonCodeName=reasonCodeName
                });
                if (result.Code == 0)
                {
                    viewModel = new EquipmentReasonCodeCategoryDetailViewModel()
                    {
                        ReasonCodeCategoryName = result.Data.Key.ReasonCodeCategoryName,
                        ReasonCodeName = result.Data.Key.ReasonCodeName,
                        ItemNo = result.Data.ItemNo,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        CreateTime=result.Data.CreateTime,
                        Creator=result.Data.Creator
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
        // POST: /FMM/ReasonCodeCategoryDetail/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(EquipmentReasonCodeCategoryDetailViewModel model)
        {
            using (EquipmentReasonCodeCategoryDetailServiceClient client = new EquipmentReasonCodeCategoryDetailServiceClient())
            {
                MethodReturnResult<EquipmentReasonCodeCategoryDetail> result = await client.GetAsync(new EquipmentReasonCodeCategoryDetailKey()
                {
                    ReasonCodeCategoryName = model.ReasonCodeCategoryName,
                    ReasonCodeName = model.ReasonCodeName
                });

                if (result.Code == 0)
                {
                    result.Data.ItemNo = model.ItemNo;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.ReasonCodeCategoryDetail_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/ReasonCodeCategoryDetail/Detail
        public async Task<ActionResult> Detail(string categoryName, string reasonCodeName)
        {
            using (EquipmentReasonCodeCategoryDetailServiceClient client = new EquipmentReasonCodeCategoryDetailServiceClient())
            {
                MethodReturnResult<EquipmentReasonCodeCategoryDetail> result = await client.GetAsync(new EquipmentReasonCodeCategoryDetailKey()
                {
                    ReasonCodeCategoryName = categoryName,
                    ReasonCodeName = reasonCodeName
                });
                if (result.Code == 0)
                {
                    EquipmentReasonCodeCategoryDetailViewModel viewModel = new EquipmentReasonCodeCategoryDetailViewModel()
                    {
                        ReasonCodeCategoryName = result.Data.Key.ReasonCodeCategoryName,
                        ReasonCodeName = result.Data.Key.ReasonCodeName,
                        ItemNo = result.Data.ItemNo,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator
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
        // POST: /FMM/ReasonCodeCategoryDetail/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string categoryName, string reasonCodeName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (EquipmentReasonCodeCategoryDetailServiceClient client = new EquipmentReasonCodeCategoryDetailServiceClient())
            {
                result = await client.DeleteAsync(new EquipmentReasonCodeCategoryDetailKey()
                {
                    ReasonCodeCategoryName = categoryName,
                    ReasonCodeName = reasonCodeName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.ReasonCodeCategoryDetail_Delete_Success
                                                    , reasonCodeName);
                }
                return Json(result);
            }
        }
    }
}