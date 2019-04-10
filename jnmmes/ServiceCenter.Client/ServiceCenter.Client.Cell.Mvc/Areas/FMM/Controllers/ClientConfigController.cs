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
    public class ClientConfigController : Controller
    {

        //
        // GET: /FMM/ClientConfig/
        public async Task<ActionResult> Index()
        {
            using (ClientConfigServiceClient client = new ClientConfigServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<ClientConfig>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new ClientConfigQueryViewModel());
        }

        //
        //POST: /FMM/ClientConfig/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(ClientConfigQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ClientConfigServiceClient client = new ClientConfigServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (model.ClientType!=null)
                            {
                                where.AppendFormat(" {0} ClientType = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , Convert.ToInt32(model.ClientType));
                            }

                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }

                            if (!string.IsNullOrEmpty(model.IPAddress))
                            {
                                where.AppendFormat(" {0} IPAddress LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.IPAddress);
                            }

                            if (!string.IsNullOrEmpty(model.LocationName))
                            {
                                where.AppendFormat(" {0} LocationName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LocationName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<ClientConfig>> result = client.Get(ref cfg);

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
        //POST: /FMM/ClientConfig/PagingQuery
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

                using (ClientConfigServiceClient client = new ClientConfigServiceClient())
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
                        MethodReturnResult<IList<ClientConfig>> result = client.Get(ref cfg);
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
        // POST: /FMM/ClientConfig/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ClientConfigViewModel model)
        {
            using (ClientConfigServiceClient client = new ClientConfigServiceClient())
            {
                ClientConfig obj = new ClientConfig()
                {
                    Key = model.Name,
                    ClientType=model.ClientType,
                    IPAddress=model.IPAddress,
                    LocationName=model.LocationName,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.ClientConfig_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/ClientConfig/Modify
        public async Task<ActionResult> Modify(string key)
        {
            ClientConfigViewModel viewModel = new ClientConfigViewModel();
            using (ClientConfigServiceClient client = new ClientConfigServiceClient())
            {
                MethodReturnResult<ClientConfig> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new ClientConfigViewModel()
                    {
                        Name = result.Data.Key,
                        ClientType = result.Data.ClientType,
                        IPAddress=result.Data.IPAddress,
                        LocationName=result.Data.LocationName,
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
        // POST: /FMM/ClientConfig/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(ClientConfigViewModel model)
        {
            using (ClientConfigServiceClient client = new ClientConfigServiceClient())
            {
                MethodReturnResult<ClientConfig> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.ClientType = model.ClientType;
                    result.Data.IPAddress = model.IPAddress;
                    result.Data.LocationName = model.LocationName;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.ClientConfig_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/ClientConfig/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (ClientConfigServiceClient client = new ClientConfigServiceClient())
            {
                MethodReturnResult<ClientConfig> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    ClientConfigViewModel viewModel = new ClientConfigViewModel()
                    {
                        ClientType=result.Data.ClientType,
                        Name = result.Data.Key,
                        IPAddress = result.Data.IPAddress,
                        LocationName = result.Data.LocationName,
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
        // POST: /FMM/ClientConfig/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ClientConfigServiceClient client = new ClientConfigServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.ClientConfig_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}