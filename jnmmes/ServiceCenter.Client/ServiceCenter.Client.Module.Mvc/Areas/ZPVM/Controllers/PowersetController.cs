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
    public class PowersetController : Controller
    {
        //
        // GET: /ZPVM/Powerset/
        public async Task<ActionResult> Index()
        {
            using (PowersetServiceClient client = new PowersetServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<Powerset>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new PowersetQueryViewModel());
        }

        //
        //POST: /ZPVM/Powerset/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(PowersetQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (PowersetServiceClient client = new PowersetServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Code))
                            {
                                where.AppendFormat(" {0} Key.Code LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Code);
                            }

                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Name LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }

                            if (!string.IsNullOrEmpty(model.PowerDifference))
                            {
                                where.AppendFormat(" {0} PowerDifference LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.PowerDifference);
                            }

                            if (!string.IsNullOrEmpty(model.PowerName))
                            {
                                where.AppendFormat(" {0} PowerName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.PowerName);
                            }

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Powerset>> result = client.Get(ref cfg);

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
        //POST: /ZPVM/Powerset/PagingQuery
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

                using (PowersetServiceClient client = new PowersetServiceClient())
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
                        MethodReturnResult<IList<Powerset>> result = client.Get(ref cfg);
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
        // POST: /ZPVM/Powerset/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(PowersetViewModel model)
        {
            using (PowersetServiceClient client = new PowersetServiceClient())
            {
                Powerset obj = new Powerset()
                {
                    Key = new PowersetKey(){
                        Code = model.Code.ToUpper(),
                        ItemNo=model.ItemNo??0
                    },
                    ArticleNo = model.ArticleNo,
                    MaxValue = model.MaxValue,
                    MinValue = model.MinValue,
                    Name=model.Name.ToUpper(),
                    PowerDifference=model.PowerDifference,
                    PowerName=model.PowerName.ToUpper(),
                    StandardFuse=model.StandardFuse,
                    StandardIPM=model.StandardIPM,
                    StandardIsc=model.StandardIsc,
                    StandardPower=model.StandardPower,
                    StandardVoc=model.StandardVoc,
                    StandardVPM=model.StandardVPM,
                    SubWay=model.SubWay,
                    Description = model.Description,
                    MixColor =model.MixColor,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.Powerset_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/Powerset/Modify
        public async Task<ActionResult> Modify(string code,int itemNo)
        {
            PowersetViewModel viewModel = new PowersetViewModel();
            using (PowersetServiceClient client = new PowersetServiceClient())
            {
                MethodReturnResult<Powerset> result = await client.GetAsync(new PowersetKey()
                {
                    Code=code,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    viewModel = new PowersetViewModel()
                    {
                        Code=result.Data.Key.Code,
                        ArticleNo=result.Data.ArticleNo,
                        ItemNo=result.Data.Key.ItemNo,
                        PowerName=result.Data.PowerName,
                        SubWay=result.Data.SubWay,
                        StandardVPM=result.Data.StandardVPM,
                        StandardVoc=result.Data.StandardVoc,
                        StandardPower=result.Data.StandardPower,
                        StandardIsc=result.Data.StandardIsc,
                        StandardIPM=result.Data.StandardIPM,
                        StandardFuse=result.Data.StandardFuse,
                        PowerDifference=result.Data.PowerDifference,
                        MaxValue=result.Data.MaxValue,
                        MinValue=result.Data.MinValue,
                        Name=result.Data.Name,
                        Description=result.Data.Description,
                        MixColor = result.Data.MixColor,
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
            return PartialView("_ModifyPartial");
        }

        //
        // POST: /ZPVM/Powerset/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(PowersetViewModel model)
        {
            using (PowersetServiceClient client = new PowersetServiceClient())
            {
                PowersetKey key = new PowersetKey()
                {
                    Code = model.Code,
                    ItemNo=model.ItemNo??0
                };
                MethodReturnResult<Powerset> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.ArticleNo = model.ArticleNo;
                    result.Data.MaxValue = model.MaxValue;
                    result.Data.MinValue = model.MinValue;
                    result.Data.Name = model.Name;
                    result.Data.PowerDifference = model.PowerDifference;
                    result.Data.PowerName = model.PowerName;
                    result.Data.StandardFuse = model.StandardFuse;
                    result.Data.StandardIPM = model.StandardIPM;
                    result.Data.StandardIsc = model.StandardIsc;
                    result.Data.StandardPower = model.StandardPower;
                    result.Data.StandardVoc = model.StandardVoc;
                    result.Data.StandardVPM = model.StandardVPM;
                    result.Data.SubWay = model.SubWay;
                    result.Data.Description = model.Description;
                    result.Data.MixColor = model.MixColor;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.Powerset_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/Powerset/Detail
        public async Task<ActionResult> Detail(string code, int itemNo)
        {
            using (PowersetServiceClient client = new PowersetServiceClient())
            {
                PowersetKey key = new PowersetKey()
                {
                    Code = code,
                    ItemNo = itemNo
                };
                MethodReturnResult<Powerset> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    PowersetViewModel viewModel = new PowersetViewModel()
                    {
                        Code = result.Data.Key.Code,
                        ArticleNo = result.Data.ArticleNo,
                        ItemNo = result.Data.Key.ItemNo,
                        PowerName = result.Data.PowerName,
                        SubWay = result.Data.SubWay,
                        StandardVPM = result.Data.StandardVPM,
                        StandardVoc = result.Data.StandardVoc,
                        StandardPower = result.Data.StandardPower,
                        StandardIsc = result.Data.StandardIsc,
                        StandardIPM = result.Data.StandardIPM,
                        StandardFuse = result.Data.StandardFuse,
                        PowerDifference = result.Data.PowerDifference,
                        MaxValue = result.Data.MaxValue,
                        MinValue = result.Data.MinValue,
                        Name = result.Data.Name,
                        Description = result.Data.Description,
                        IsUsed = result.Data.IsUsed,
                        MixColor = result.Data.MixColor,
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
        // POST: /ZPVM/Powerset/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string code, int itemNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            PowersetKey key = new PowersetKey()
            {
                Code = code,
                ItemNo = itemNo
            };
            using (PowersetServiceClient client = new PowersetServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.Powerset_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}