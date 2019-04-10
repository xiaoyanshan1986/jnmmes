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
    public class ParameterController : Controller
    {
        //
        // GET: /FMM/Parameter/
        public async Task<ActionResult> Index()
        {
            using (ParameterServiceClient client = new ParameterServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<Parameter>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new ParameterQueryViewModel());
        }

        //
        //POST: /FMM/Parameter/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(ParameterQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ParameterServiceClient client = new ParameterServiceClient())
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
                            if (model.Type!=null)
                            {
                                where.AppendFormat(" {0} Type = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , Convert.ToInt32(model.Type));
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Parameter>> result = client.Get(ref cfg);

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
        //POST: /FMM/Parameter/PagingQuery
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

                using (ParameterServiceClient client = new ParameterServiceClient())
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
                        MethodReturnResult<IList<Parameter>> result = client.Get(ref cfg);
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
        // POST: /FMM/Parameter/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ParameterViewModel model)
        {
            using (ParameterServiceClient client = new ParameterServiceClient())
            {
                Parameter obj = new Parameter()
                {
                    Key = model.Name,
                    DataType = model.DataType,
                    DerivedFormula= model.IsDerived?model.DerivedFormula:string.Empty,
                    Type=model.Type,
                    DeviceType=model.DeviceType,
                    IsDerived=model.IsDerived,
                    Mandatory=model.Mandatory,
                    Status=EnumObjectStatus.Available,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.Parameter_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/Parameter/Modify
        public async Task<ActionResult> Modify(string key)
        {
            ParameterViewModel viewModel = new ParameterViewModel();
            using (ParameterServiceClient client = new ParameterServiceClient())
            {
                MethodReturnResult<Parameter> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new ParameterViewModel()
                    {
                        Name = result.Data.Key,
                        DeviceType = result.Data.DeviceType,
                        Mandatory = result.Data.Mandatory,
                        IsDerived = result.Data.IsDerived,
                        DerivedFormula = result.Data.DerivedFormula,
                        DataType = result.Data.DataType,
                        Type = result.Data.Type,
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
        // POST: /FMM/Parameter/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(ParameterViewModel model)
        {
            using (ParameterServiceClient client = new ParameterServiceClient())
            {
                MethodReturnResult<Parameter> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.Type = model.Type;
                    result.Data.DataType = model.DataType;
                    result.Data.DerivedFormula = model.IsDerived?model.DerivedFormula:string.Empty;
                    result.Data.DeviceType = model.DeviceType;
                    result.Data.IsDerived = model.IsDerived;
                    result.Data.Mandatory = model.Mandatory;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.Parameter_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/Parameter/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (ParameterServiceClient client = new ParameterServiceClient())
            {
                MethodReturnResult<Parameter> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    ParameterViewModel viewModel = new ParameterViewModel()
                    {
                        Name = result.Data.Key,
                        DeviceType = result.Data.DeviceType,
                        Mandatory = result.Data.Mandatory,
                        IsDerived = result.Data.IsDerived,
                        DerivedFormula = result.Data.DerivedFormula,
                        DataType = result.Data.DataType,
                        Type = result.Data.Type,
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
        // POST: /FMM/Parameter/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ParameterServiceClient client = new ParameterServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.Parameter_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}