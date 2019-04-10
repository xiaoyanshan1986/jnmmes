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
    public class CheckSettingPointController : Controller
    {

        //
        // GET: /QAM/CheckSettingPoint/
        public async Task<ActionResult> Index(string checkSettingKey,string groupName)
        {
            using (CheckSettingServiceClient client = new CheckSettingServiceClient())
            {
                MethodReturnResult<CheckSetting> result = await client.GetAsync(checkSettingKey ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "CheckSetting");
                }
                ViewBag.CheckSetting = result.Data;
            }

            using (CheckSettingPointServiceClient client = new CheckSettingPointServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key.ItemNo",
                        Where = string.Format(" Key.CheckSettingKey = '{0}'"
                                                    , checkSettingKey)
                    };
                    MethodReturnResult<IList<CheckSettingPoint>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new CheckSettingPointQueryViewModel() { CheckSettingKey=checkSettingKey,  GroupName = groupName });
        }

        //
        //POST: /QAM/CheckSettingPoint/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(CheckSettingPointQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (CheckSettingPointServiceClient client = new CheckSettingPointServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.CheckSettingKey = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.CheckSettingKey);

                            if (!string.IsNullOrEmpty(model.CategoryName))
                            {
                                where.AppendFormat(" {0} CategoryName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.CategoryName);
                            }
                            if (!string.IsNullOrEmpty(model.CheckPlanName))
                            {
                                where.AppendFormat(" {0} CheckPlanName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.CheckPlanName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.ItemNo",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<CheckSettingPoint>> result = client.Get(ref cfg);

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
        //POST: /QAM/CheckSettingPoint/PagingQuery
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

                using (CheckSettingPointServiceClient client = new CheckSettingPointServiceClient())
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
                        MethodReturnResult<IList<CheckSettingPoint>> result = client.Get(ref cfg);
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
        // POST: /QAM/CheckSettingPoint/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(CheckSettingPointViewModel model)
        {
            using (CheckSettingPointServiceClient client = new CheckSettingPointServiceClient())
            {
                CheckSettingPoint obj = new CheckSettingPoint()
                {
                    Key = new CheckSettingPointKey() {
                        CheckSettingKey = model.CheckSettingKey,
                        ItemNo = model.ItemNo
                    },
                    CategoryName=model.CategoryName,
                    CheckPlanName=model.CheckPlanName,
                    Status=model.Status,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(QAMResources.StringResource.CheckSettingPoint_Save_Success
                                                , model.ItemNo);
                }
                return Json(rst);
            }
        }
        //
        // GET: /QAM/CheckSettingPoint/Modify
        public async Task<ActionResult> Modify(string checksettingKey,int itemNo)
        {
            CheckSettingPointViewModel viewModel = new CheckSettingPointViewModel();
            using (CheckSettingPointServiceClient client = new CheckSettingPointServiceClient())
            {
                MethodReturnResult<CheckSettingPoint> result = await client.GetAsync(new CheckSettingPointKey()
                {
                    CheckSettingKey = checksettingKey,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    viewModel = new CheckSettingPointViewModel()
                    {
                        CheckSettingKey = result.Data.Key.CheckSettingKey,
                        ItemNo = result.Data.Key.ItemNo,
                        CheckPlanName=result.Data.CheckPlanName,
                        CategoryName=result.Data.CategoryName,
                        Status=result.Data.Status,
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
        // POST: /QAM/CheckSettingPoint/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(CheckSettingPointViewModel model)
        {
            using (CheckSettingPointServiceClient client = new CheckSettingPointServiceClient())
            {
                MethodReturnResult<CheckSettingPoint> result = await client.GetAsync(new CheckSettingPointKey()
                {
                    CheckSettingKey = model.CheckSettingKey,
                    ItemNo = model.ItemNo
                });

                if (result.Code == 0)
                {
                    result.Data.CategoryName = model.CategoryName;
                    result.Data.CheckPlanName = model.CheckPlanName;
                    result.Data.Status = model.Status;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(QAMResources.StringResource.CheckSettingPoint_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /QAM/CheckSettingPoint/Detail
        public async Task<ActionResult> Detail(string checksettingKey, int itemNo)
        {
            using (CheckSettingPointServiceClient client = new CheckSettingPointServiceClient())
            {
                MethodReturnResult<CheckSettingPoint> result = await client.GetAsync(new CheckSettingPointKey()
                {
                    CheckSettingKey = checksettingKey,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    CheckSettingPointViewModel viewModel = new CheckSettingPointViewModel()
                    {
                        CheckSettingKey = result.Data.Key.CheckSettingKey,
                        ItemNo = result.Data.Key.ItemNo,
                        CheckPlanName = result.Data.CheckPlanName,
                        CategoryName = result.Data.CategoryName,
                        Status = result.Data.Status,
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
        // POST: /QAM/CheckSettingPoint/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string checksettingKey, int itemNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (CheckSettingPointServiceClient client = new CheckSettingPointServiceClient())
            {
                result = await client.DeleteAsync(new CheckSettingPointKey()
                {
                    CheckSettingKey = checksettingKey,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(QAMResources.StringResource.CheckSettingPoint_Delete_Success
                                                    , itemNo);
                }
                return Json(result);
            }
        }

        public ActionResult GetMaxItemNo(string checksettingKey)
        {
            using (CheckSettingPointServiceClient client = new CheckSettingPointServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.CheckSettingKey='{0}'", checksettingKey),
                    OrderBy = "Key.ItemNo Desc"
                };
                MethodReturnResult<IList<CheckSettingPoint>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    return Json(result.Data[0].Key.ItemNo + 1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}