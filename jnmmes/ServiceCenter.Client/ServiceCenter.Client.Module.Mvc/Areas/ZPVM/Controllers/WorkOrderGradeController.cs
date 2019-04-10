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
    public class WorkOrderGradeController : Controller
    {
        //
        // GET: /ZPVM/WorkOrderGrade/
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
                   OrderNumber=OrderNumber,
                   MaterialCode=MaterialCode
                });

                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "WorkOrderRule");
                }
                ViewBag.Rule = result.Data;
            }

            using (WorkOrderGradeServiceClient client = new WorkOrderGradeServiceClient())
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
                    MethodReturnResult<IList<WorkOrderGrade>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderGradeQueryViewModel() { OrderNumber = OrderNumber, MaterialCode = MaterialCode });
        }

        //
        //POST: /ZPVM/WorkOrderGrade/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderGradeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderGradeServiceClient client = new WorkOrderGradeServiceClient())
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
                        MethodReturnResult<IList<WorkOrderGrade>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/WorkOrderGrade/PagingQuery
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

                using (WorkOrderGradeServiceClient client = new WorkOrderGradeServiceClient())
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
                        MethodReturnResult<IList<WorkOrderGrade>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/WorkOrderGrade/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderGradeViewModel model)
        {
            using (WorkOrderGradeServiceClient client = new WorkOrderGradeServiceClient())
            {
                WorkOrderGrade obj = new WorkOrderGrade()
                {
                    Key = new WorkOrderGradeKey(){
                        MaterialCode=model.MaterialCode,
                        OrderNumber=model.OrderNumber,
                        Grade=model.Grade
                    },
                    ItemNo = model.ItemNo ?? 0,
                    MixColor = model.MixColor,
                    MixPowerset = model.MixPowerset,
                    MixSubPowerset = model.MixSubPowerset,
                    PackageGroup = model.PackageGroup,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderGrade_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/WorkOrderGrade/Modify
        public async Task<ActionResult> Modify(string OrderNumber, string MaterialCode, string grade)
        {
            WorkOrderGradeViewModel viewModel = new WorkOrderGradeViewModel();
            using (WorkOrderGradeServiceClient client = new WorkOrderGradeServiceClient())
            {
                MethodReturnResult<WorkOrderGrade> result = await client.GetAsync(new WorkOrderGradeKey()
                {
                    MaterialCode = MaterialCode,
                    OrderNumber = OrderNumber,
                    Grade = grade
                });
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderGradeViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        Grade = result.Data.Key.Grade,
                        PackageGroup = result.Data.PackageGroup,
                        MixSubPowerset = result.Data.MixSubPowerset,
                        MixPowerset = result.Data.MixPowerset,
                        MixColor = result.Data.MixColor,
                        ItemNo = result.Data.ItemNo,
                        IsUsed = result.Data.IsUsed,
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
            return PartialView("_ModifyPartial", new WorkOrderGradeViewModel());
        }

        //
        // POST: /ZPVM/WorkOrderGrade/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderGradeViewModel model)
        {
            using (WorkOrderGradeServiceClient client = new WorkOrderGradeServiceClient())
            {
                WorkOrderGradeKey key = new WorkOrderGradeKey()
                {
                    MaterialCode = model.MaterialCode,
                    OrderNumber = model.OrderNumber,
                    Grade = model.Grade
                };
                MethodReturnResult<WorkOrderGrade> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.ItemNo = model.ItemNo ?? 0;
                    result.Data.MixColor = model.MixColor;
                    result.Data.MixPowerset = model.MixPowerset;
                    result.Data.MixSubPowerset = model.MixSubPowerset;
                    result.Data.PackageGroup = model.PackageGroup;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.WorkOrderGrade_SaveModify_Success
                                                    , key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/WorkOrderGrade/Detail
        public async Task<ActionResult> Detail(string OrderNumber, string MaterialCode, string grade)
        {
            using (WorkOrderGradeServiceClient client = new WorkOrderGradeServiceClient())
            {
                WorkOrderGradeKey key = new WorkOrderGradeKey()
                {
                    MaterialCode = MaterialCode,
                    OrderNumber = OrderNumber,
                    Grade=grade
                };
                MethodReturnResult<WorkOrderGrade> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    WorkOrderGradeViewModel viewModel = new WorkOrderGradeViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        Grade = result.Data.Key.Grade,
                        PackageGroup = result.Data.PackageGroup,
                        MixSubPowerset = result.Data.MixSubPowerset,
                        MixPowerset = result.Data.MixPowerset,
                        MixColor = result.Data.MixColor,
                        ItemNo = result.Data.ItemNo,
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
            return PartialView("_InfoPartial");
        }
        //
        // POST: /ZPVM/WorkOrderGrade/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string OrderNumber, string MaterialCode,string grade)
        {
            MethodReturnResult result = new MethodReturnResult();
            WorkOrderGradeKey key = new WorkOrderGradeKey()
            {
                MaterialCode = MaterialCode,
                OrderNumber = OrderNumber,
                Grade=grade
            };
            using (WorkOrderGradeServiceClient client = new WorkOrderGradeServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.WorkOrderGrade_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }

    }
}