using ServiceCenter.Client.Mvc.Areas.QAM.Models;
using QAMResources = ServiceCenter.Client.Mvc.Resources.QAM;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.MES.Service.Client.QAM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.QAM.Controllers
{
    public class CheckSettingPointDetailController : Controller
    {

        //
        // GET: /QAM/CheckSettingPointDetail/
        public async Task<ActionResult> Index(string checksettingKey,int itemNo)
        {
            string groupName = string.Empty;
            using (CheckSettingServiceClient client = new CheckSettingServiceClient())
            {
                MethodReturnResult<CheckSetting> result = await client.GetAsync(checksettingKey ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "CheckSetting");
                }
                groupName = result.Data.GroupName;
            }

            using (CheckSettingPointServiceClient client = new CheckSettingPointServiceClient())
            {
                MethodReturnResult<CheckSettingPoint> result = await client.GetAsync(new CheckSettingPointKey()
                {
                        CheckSettingKey=checksettingKey,
                        ItemNo=itemNo
                });
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "CheckSettingPoint");
                }
            }

            using (CheckSettingPointDetailServiceClient client = new CheckSettingPointDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key.ItemNo",
                        Where = string.Format(" Key.CheckSettingKey = '{0}' AND Key.ItemNo='{1}'"
                                               ,checksettingKey
                                               ,itemNo)
                    };
                    MethodReturnResult<IList<CheckSettingPointDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new CheckSettingPointDetailQueryViewModel() { CheckSettingKey = checksettingKey, GroupName = groupName,ItemNo=itemNo });
        }

        //
        //POST: /QAM/CheckSettingPointDetail/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(CheckSettingPointDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (CheckSettingPointDetailServiceClient client = new CheckSettingPointDetailServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.CheckSettingKey = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.CheckSettingKey);
                            where.AppendFormat(" {0} Key.ItemNo = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ItemNo);

                            if (!string.IsNullOrEmpty(model.ParameterName))
                            {
                                where.AppendFormat(" {0} Key.ParameterName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ParameterName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.ItemNo",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<CheckSettingPointDetail>> result = client.Get(ref cfg);

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
        //POST: /QAM/CheckSettingPointDetail/PagingQuery
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

                using (CheckSettingPointDetailServiceClient client = new CheckSettingPointDetailServiceClient())
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
                        MethodReturnResult<IList<CheckSettingPointDetail>> result = client.Get(ref cfg);
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
        // GET: /QAM/CheckSettingPointDetail/Modify
        public async Task<ActionResult> Modify(string checksettingKey,int itemNo, string parameterName)
        {
            CheckSettingPointDetailViewModel viewModel = new CheckSettingPointDetailViewModel();
            using (CheckSettingPointDetailServiceClient client = new CheckSettingPointDetailServiceClient())
            {
                MethodReturnResult<CheckSettingPointDetail> result = await client.GetAsync(new CheckSettingPointDetailKey()
                {
                    CheckSettingKey = checksettingKey,
                    ItemNo=itemNo,
                    ParameterName=parameterName
                });
                if (result.Code == 0)
                {
                    viewModel = new CheckSettingPointDetailViewModel()
                    {
                        CheckSettingKey = result.Data.Key.CheckSettingKey,
                        ItemNo = result.Data.Key.ItemNo,
                        ParameterName = result.Data.Key.ParameterName,
                        ParameterItemNo=result.Data.ParameterItemNo,
                        DataType=result.Data.DataType,
                        DerivedFormula=result.Data.DerivedFormula,
                        DeviceType=result.Data.DeviceType,
                        IsDerived=result.Data.IsDerived,
                        Mandatory=result.Data.Mandatory,
                        ParameterCount=result.Data.ParameterCount,
                        ParameterType=result.Data.ParameterType,
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
        // POST: /QAM/CheckSettingPointDetail/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(CheckSettingPointDetailViewModel model)
        {
            using (CheckSettingPointDetailServiceClient client = new CheckSettingPointDetailServiceClient())
            {
                MethodReturnResult<CheckSettingPointDetail> result = await client.GetAsync(new CheckSettingPointDetailKey()
                {
                    CheckSettingKey = model.CheckSettingKey,
                    ItemNo=model.ItemNo,
                    ParameterName = model.ParameterName
                });

                if (result.Code == 0)
                {
                    result.Data.DataType=model.DataType;
                    result.Data.DerivedFormula=model.DerivedFormula;
                    result.Data.IsDerived = model.IsDerived;
                    result.Data.Mandatory = model.Mandatory;
                    result.Data.ParameterCount = model.ParameterCount;
                    result.Data.ParameterType = model.ParameterType;
                    result.Data.ParameterItemNo = model.ParameterItemNo;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(QAMResources.StringResource.CheckSettingPointDetail_SaveModify_Success
                                                    , result.Data.Key.ParameterName);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /QAM/CheckSettingPointDetail/Detail
        public async Task<ActionResult> Detail(string checksettingKey, int itemNo, string parameterName)
        {
            using (CheckSettingPointDetailServiceClient client = new CheckSettingPointDetailServiceClient())
            {
                MethodReturnResult<CheckSettingPointDetail> result = await client.GetAsync(new CheckSettingPointDetailKey()
                {
                    CheckSettingKey = checksettingKey,
                    ItemNo = itemNo,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    CheckSettingPointDetailViewModel viewModel = new CheckSettingPointDetailViewModel()
                    {
                        CheckSettingKey = result.Data.Key.CheckSettingKey,
                        ItemNo = result.Data.Key.ItemNo,
                        ParameterName = result.Data.Key.ParameterName,
                        ParameterItemNo = result.Data.ParameterItemNo,
                        DataType = result.Data.DataType,
                        DerivedFormula = result.Data.DerivedFormula,
                        DeviceType = result.Data.DeviceType,
                        IsDerived = result.Data.IsDerived,
                        Mandatory = result.Data.Mandatory,
                        ParameterCount = result.Data.ParameterCount,
                        ParameterType = result.Data.ParameterType,
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