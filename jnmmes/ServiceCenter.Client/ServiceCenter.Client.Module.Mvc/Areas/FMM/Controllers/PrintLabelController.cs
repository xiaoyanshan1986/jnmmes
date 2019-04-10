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
using ServiceCenter.Common.Print;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class PrintLabelController : Controller
    {
        //
        // GET: /FMM/PrintLabel/
        public async Task<ActionResult> Index()
        {
            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<PrintLabel>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new PrintLabelQueryViewModel());
        }

        //
        //POST: /FMM/PrintLabel/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(PrintLabelQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (PrintLabelServiceClient client = new PrintLabelServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Code))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Code);
                            }

                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Name LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<PrintLabel>> result = client.Get(ref cfg);

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
        //POST: /FMM/PrintLabel/PagingQuery
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

                using (PrintLabelServiceClient client = new PrintLabelServiceClient())
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
                        MethodReturnResult<IList<PrintLabel>> result = client.Get(ref cfg);
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
        // POST: /FMM/PrintLabel/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(PrintLabelViewModel model)
        {
            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
            {
                PrintLabel obj = new PrintLabel()
                {
                    Key = model.Code.ToUpper(),
                    Name = model.Name.ToUpper(),
                    Content=model.Content,
                    Type=model.Type,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.PrintLabel_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/PrintLabel/Modify
        public async Task<ActionResult> Modify(string key)
        {
            PrintLabelViewModel viewModel = new PrintLabelViewModel();
            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
            {
                MethodReturnResult<PrintLabel> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new PrintLabelViewModel()
                    {
                        Name = result.Data.Name,
                        Code=result.Data.Key,
                        Content=result.Data.Content,
                        Type=result.Data.Type,
                        IsUsed=result.Data.IsUsed,
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
            return PartialView("_ModifyPartial", new PrintLabelViewModel());
        }

        //
        // POST: /FMM/PrintLabel/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(PrintLabelViewModel model)
        {
            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
            {
                MethodReturnResult<PrintLabel> result = await client.GetAsync(model.Code);

                if (result.Code == 0)
                {
                    result.Data.Name = model.Name.ToUpper();
                    result.Data.Content = model.Content;
                    result.Data.Type = model.Type;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.PrintLabel_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/PrintLabel/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
            {
                MethodReturnResult<PrintLabel> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    PrintLabelViewModel viewModel = new PrintLabelViewModel()
                    {
                        Name = result.Data.Name,
                        IsUsed =result.Data.IsUsed,
                        Content=result.Data.Content,
                        Code=result.Data.Key,
                        Type=result.Data.Type,
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
            return PartialView("_InfoPartial", new PrintLabelViewModel());
        }
        //
        // POST: /FMM/PrintLabel/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.PrintLabel_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }

        public ActionResult PrintTest(string key)
        {
            PrintTestViewModel viewModel = new PrintTestViewModel();
            if(!string.IsNullOrEmpty(key))
            {
                using (PrintLabelServiceClient client = new PrintLabelServiceClient())
                {
                    MethodReturnResult<PrintLabel> result = client.Get(key);
                    if (result.Code == 0)
                    {
                        viewModel.PrintContent = result.Data.Content;
                    }
                }
            }
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Print(PrintTestViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                 bool bSuccess = false;
                 using (IPrintHelper helper = PrintHelperFactory.CreatePrintHelper(model.PrintContent))
                 {
                     if (model.PrinterType == EnumPrinterType.Network)
                     {
                         string[] vals = model.PrinterName.Split(':');
                         string port = "9100";
                         if (vals.Length > 1)
                         {
                             port = vals[1];
                         }
                         bSuccess = helper.NetworkPrint(vals[0], port, model.PrintContent, null);
                     }
                     else if (model.PrinterType == EnumPrinterType.RAW)
                     {
                         bSuccess = helper.RAWPrint(model.PrinterName, model.PrintContent, null);
                     }
                 }
                if (bSuccess == false)
                {
                    result.Code = 1001;
                    result.Message ="打印失败。";
                }
                else
                {
                    result.Message = "打印成功。";
                }
            }
            catch(Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return Json(result);
        }
    }
}