using ServiceCenter.Client.Mvc.Areas.EDC.Models;
using EDCResources = ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Client.EDC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Controllers
{
    public class PointDetailController : Controller
    {

        //
        // GET: /EDC/PointDetail/
        public async Task<ActionResult> Index(string groupName,string pointKey)
        {
            using (PointServiceClient client = new PointServiceClient())
            {
                MethodReturnResult<Point> result = await client.GetAsync(pointKey ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "Point");
                }
                ViewBag.Point = result.Data;
            }

            using (PointDetailServiceClient client = new PointDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "ItemNo",
                        Where = string.Format(" Key.PointKey = '{0}'"
                                                    , pointKey)
                    };
                    MethodReturnResult<IList<PointDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new PointDetailQueryViewModel() { PointKey = pointKey, GroupName=groupName });
        }

        //
        //POST: /EDC/PointDetail/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(PointDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (PointDetailServiceClient client = new PointDetailServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.PointKey = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.PointKey);

                            if (!string.IsNullOrEmpty(model.ParameterName))
                            {
                                where.AppendFormat(" {0} Key.ParameterName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ParameterName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "ItemNo",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<PointDetail>> result = client.Get(ref cfg);

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
        //POST: /EDC/PointDetail/PagingQuery
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

                using (PointDetailServiceClient client = new PointDetailServiceClient())
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
                        MethodReturnResult<IList<PointDetail>> result = client.Get(ref cfg);
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
        // GET: /EDC/PointDetail/Modify
        public async Task<ActionResult> Modify(string pointKey,string parameterName)
        {
            PointDetailViewModel viewModel = new PointDetailViewModel();
            using (PointDetailServiceClient client = new PointDetailServiceClient())
            {
                MethodReturnResult<PointDetail> result = await client.GetAsync(new PointDetailKey()
                {
                    PointKey=pointKey,
                    ParameterName=parameterName
                });
                if (result.Code == 0)
                {
                    viewModel = new PointDetailViewModel()
                    {
                        PointKey = result.Data.Key.PointKey,
                        ParameterName = result.Data.Key.ParameterName,
                        ItemNo = result.Data.ItemNo,
                        DataType=result.Data.DataType,
                        DerivedFormula=result.Data.DerivedFormula,
                        DeviceType=result.Data.DeviceType,
                        IsDerived=result.Data.IsDerived,
                        LowerBoundary=result.Data.LowerBoundary,
                        LowerControl=result.Data.LowerControl,
                        LowerSpecification=result.Data.LowerSpecification,
                        Mandatory=result.Data.Mandatory,
                        ParameterCount=result.Data.ParameterCount,
                        ParameterType=result.Data.ParameterType,
                        Target=result.Data.Target,
                        UpperBoundary=result.Data.UpperBoundary,
                        UpperControl=result.Data.UpperControl,
                        UpperSpecification=result.Data.UpperSpecification,
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
        // POST: /EDC/PointDetail/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(PointDetailViewModel model)
        {
            using (PointDetailServiceClient client = new PointDetailServiceClient())
            {
                MethodReturnResult<PointDetail> result = await client.GetAsync(new PointDetailKey()
                {
                    PointKey = model.PointKey,
                    ParameterName = model.ParameterName
                });

                if (result.Code == 0)
                {
                    result.Data.DataType=model.DataType;
                    result.Data.DerivedFormula=model.DerivedFormula;
                    result.Data.IsDerived = model.IsDerived;
                    result.Data.LowerBoundary = model.LowerBoundary;
                    result.Data.LowerControl = model.LowerControl;
                    result.Data.LowerSpecification = model.LowerSpecification;
                    result.Data.Mandatory = model.Mandatory;
                    result.Data.ParameterCount = model.ParameterCount;
                    result.Data.ParameterType = model.ParameterType;
                    result.Data.Target = model.Target;
                    result.Data.UpperBoundary = model.UpperBoundary;
                    result.Data.UpperControl = model.UpperControl;
                    result.Data.UpperSpecification = model.UpperSpecification;
                    result.Data.ItemNo = model.ItemNo;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(EDCResources.StringResource.PointDetail_SaveModify_Success
                                                    , result.Data.Key.ParameterName);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /EDC/PointDetail/Detail
        public async Task<ActionResult> Detail(string pointKey, string parameterName)
        {
            using (PointDetailServiceClient client = new PointDetailServiceClient())
            {
                MethodReturnResult<PointDetail> result = await client.GetAsync(new PointDetailKey()
                {
                    PointKey = pointKey,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    PointDetailViewModel viewModel = new PointDetailViewModel()
                    {
                        PointKey = result.Data.Key.PointKey,
                        ParameterName = result.Data.Key.ParameterName,
                        ItemNo = result.Data.ItemNo,
                        DataType = result.Data.DataType,
                        DerivedFormula = result.Data.DerivedFormula,
                        DeviceType = result.Data.DeviceType,
                        IsDerived = result.Data.IsDerived,
                        LowerBoundary = result.Data.LowerBoundary,
                        LowerControl = result.Data.LowerControl,
                        LowerSpecification = result.Data.LowerSpecification,
                        Mandatory = result.Data.Mandatory,
                        ParameterCount = result.Data.ParameterCount,
                        ParameterType = result.Data.ParameterType,
                        Target = result.Data.Target,
                        UpperBoundary = result.Data.UpperBoundary,
                        UpperControl = result.Data.UpperControl,
                        UpperSpecification = result.Data.UpperSpecification,
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

    }
}