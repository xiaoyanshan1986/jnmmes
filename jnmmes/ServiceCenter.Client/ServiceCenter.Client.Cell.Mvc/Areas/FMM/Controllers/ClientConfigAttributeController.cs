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
    public class ClientConfigAttributeController : Controller
    {

        //
        // GET: /FMM/ClientConfigAttribute/
        public async Task<ActionResult> Index(string clientName)
        {
            using (ClientConfigServiceClient client = new ClientConfigServiceClient())
            {
                MethodReturnResult<ClientConfig> result = await client.GetAsync(clientName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "ClientConfig");
                }
                ViewBag.ClientConfig = result.Data;
            }

            using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.ClientName = '{0}'"
                                                    , clientName),
                        OrderBy = "Key.ClientName,Key.AttributeName"
                    };
                    MethodReturnResult<IList<ClientConfigAttribute>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new ClientConfigAttributeQueryViewModel() { ClientName=clientName });
        }

        //
        //POST: /FMM/ClientConfigAttribute/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(ClientConfigAttributeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.ClientName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ClientName);

                            if (!string.IsNullOrEmpty(model.AttributeName))
                            {
                                where.AppendFormat(" {0} Key.AttributeName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.AttributeName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<ClientConfigAttribute>> result = client.Get(ref cfg);

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
        //POST: /FMM/ClientConfigAttribute/PagingQuery
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

                using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
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
                        MethodReturnResult<IList<ClientConfigAttribute>> result = client.Get(ref cfg);
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
        // POST: /FMM/ClientConfigAttribute/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ClientConfigAttributeViewModel model)
        {
            using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
            {
                ClientConfigAttribute obj = new ClientConfigAttribute()
                {
                    Key = new ClientConfigAttributeKey() { 
                         ClientName=model.ClientName,
                         AttributeName=model.AttributeName
                    },
                    Value=model.Value,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.ClientConfigAttribute_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/ClientConfigAttribute/Modify
        public async Task<ActionResult> Modify(string clientName,string attributeName)
        {
            ClientConfigAttributeViewModel viewModel = new ClientConfigAttributeViewModel();
            using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
            {
                MethodReturnResult<ClientConfigAttribute> result = await client.GetAsync(new ClientConfigAttributeKey()
                {
                    ClientName=clientName,
                    AttributeName=attributeName
                });
                if (result.Code == 0)
                {
                    viewModel = new ClientConfigAttributeViewModel()
                    {
                        ClientName = result.Data.Key.ClientName,
                        AttributeName = result.Data.Key.AttributeName,
                        Value = result.Data.Value,
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
        // POST: /FMM/ClientConfigAttribute/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(ClientConfigAttributeViewModel model)
        {
            using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
            {
                MethodReturnResult<ClientConfigAttribute> result = await client.GetAsync(new ClientConfigAttributeKey()
                {
                    ClientName = model.ClientName,
                    AttributeName = model.AttributeName
                });

                if (result.Code == 0)
                {
                    result.Data.Value = model.Value;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.ClientConfigAttribute_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/ClientConfigAttribute/Detail
        public async Task<ActionResult> Detail(string clientName, string attributeName)
        {
            using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
            {
                MethodReturnResult<ClientConfigAttribute> result = await client.GetAsync(new ClientConfigAttributeKey()
                {
                    ClientName = clientName,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    ClientConfigAttributeViewModel viewModel = new ClientConfigAttributeViewModel()
                    {
                        ClientName = result.Data.Key.ClientName,
                        AttributeName = result.Data.Key.AttributeName,
                        Value = result.Data.Value,
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
        // POST: /FMM/ClientConfigAttribute/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string clientName, string attributeName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
            {
                result = await client.DeleteAsync(new ClientConfigAttributeKey()
                {
                    ClientName = clientName,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.ClientConfigAttribute_Delete_Success
                                                    , attributeName);
                }
                return Json(result);
            }
        }
    }
}