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
    public class EquipmentStateController : Controller
    {
        //
        // GET: /FMM/EquipmentState/
        public async Task<ActionResult> Index()
        {
            using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Type,Key"
                    };
                    MethodReturnResult<IList<EquipmentState>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new EquipmentStateQueryViewModel());
        }

        //
        //POST: /FMM/EquipmentState/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(EquipmentStateQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }

                            if(model.Type!=null)
                            {
                                where.AppendFormat(" {0} Type = '{1}'"
                                                   , where.Length > 0 ? "AND" : string.Empty
                                                   , Convert.ToInt32(model.Type));
                            }

                            if (model.Category != null)
                            {
                                where.AppendFormat(" {0} Category = '{1}'"
                                                   , where.Length > 0 ? "AND" : string.Empty
                                                   ,  Convert.ToInt32(model.Category));
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Type,Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<EquipmentState>> result = client.Get(ref cfg);

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
        //POST: /FMM/EquipmentState/PagingQuery
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

                using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
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
                        MethodReturnResult<IList<EquipmentState>> result = client.Get(ref cfg);
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
        // POST: /FMM/EquipmentState/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(EquipmentStateViewModel model)
        {
            using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
            {
                EquipmentState obj = new EquipmentState()
                {
                    Key = model.Name,
                    Category=model.Category,
                    Type=model.Type,
                    StateColor=model.StateColor,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.EquipmentState_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/EquipmentState/Modify
        public async Task<ActionResult> Modify(string key)
        {
            EquipmentStateViewModel viewModel = new EquipmentStateViewModel();
            using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
            {
                MethodReturnResult<EquipmentState> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new EquipmentStateViewModel()
                    {
                        Name = result.Data.Key,
                        Category=result.Data.Category,
                        Type=result.Data.Type,
                        StateColor = result.Data.StateColor,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
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
        // POST: /FMM/EquipmentState/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(EquipmentStateViewModel model)
        {
            using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
            {
                MethodReturnResult<EquipmentState> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.StateColor = model.StateColor;
                    result.Data.Type = model.Type;
                    result.Data.Category = model.Category;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.EquipmentState_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/EquipmentState/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
            {
                MethodReturnResult<EquipmentState> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    EquipmentStateViewModel viewModel = new EquipmentStateViewModel()
                    {
                        Name = result.Data.Key,
                        Category=result.Data.Category,
                        Type=result.Data.Type,
                        StateColor=result.Data.StateColor,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
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
        // POST: /FMM/EquipmentState/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.EquipmentState_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}