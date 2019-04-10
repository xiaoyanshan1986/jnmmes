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
    public class BinRuleController : Controller
    {

        //
        // GET: /FMM/BinRule/
        public ActionResult Index()
        {
            return View(new BinRuleQueryViewModel());
        }
        //
        //POST: /FMM/BinRule/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(BinRuleQueryViewModel model)
        {
            if (ModelState.IsValid&&model.PackageLine!=null)
            {
                using (BinRuleServiceClient client = new BinRuleServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging =false,
                        Where = string.Format(" Key.PackageLine = '{0}'"
                                                    , model.PackageLine)
                    };
                    MethodReturnResult<IList<BinRule>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;

                    }
                }
            }
            else
            {
                using (BinRuleServiceClient client = new BinRuleServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy ="Key.PackageLine,Key.BinNo"

                    };
                    MethodReturnResult<IList<BinRule>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;

                    }
                }  
            }
                return PartialView("_ListPartial");

        }

        // POST: /FMM/BinRule/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(BinRuleViewModel model)
        {
            using (BinRuleServiceClient client = new BinRuleServiceClient())
            {
                BinRule obj = new BinRule()
                {
                    Key = new BinRuleKey() { 
                         BinNo = model.BinNo,
                         Color = model.Color,
                         Grade = model.Grade,
                         PackageLine = model.PackageLine,
                         PsCode = model.PsCode,
                         PsSubCode = model.PsSubCode,
                         PsItemNo = model.PsItemNo,
                         WorkOrderNumber = model.WorkOrderNumber
                    },
                    LocationName = model.LocationName,
                    Creator = User.Identity.Name,
                    CreateTime = DateTime.Now,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.BinRule_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/BinRule/Modify
        public async Task<ActionResult> Modify(string BinNo, string Color, string Grade, string PackageLine, string PsCode, string PsSubCode, int PsItemNo, string WorkOrderNumber)
        {
            BinRuleViewModel viewModel = new BinRuleViewModel();
            using (BinRuleServiceClient client = new BinRuleServiceClient())
            {
                MethodReturnResult<BinRule> result = await client.GetAsync(new BinRuleKey()
                {
                    BinNo = BinNo,
                    Color = Color,
                    Grade = Grade,
                    PackageLine = PackageLine,
                    PsCode = PsCode,
                    PsSubCode = PsSubCode,
                    PsItemNo = PsItemNo,
                    WorkOrderNumber = WorkOrderNumber
                });
                if (result.Code == 0)
                {
                    viewModel = new BinRuleViewModel()
                    {
                        BinNo = result.Data.Key.BinNo,
                        Color = result.Data.Key.Color,
                        Grade = result.Data.Key.Grade,
                        PackageLine = result.Data.Key.PackageLine,
                        PsCode = result.Data.Key.PsCode,
                        PsSubCode = result.Data.Key.PsSubCode,
                        PsItemNo = result.Data.Key.PsItemNo,
                        WorkOrderNumber = result.Data.Key.WorkOrderNumber,

                        LocationName = result.Data.LocationName,
                        Creator = result.Data.Creator,
                        CreateTime = result.Data.CreateTime,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(BinRuleViewModel model)
        {
            using (BinRuleServiceClient client = new BinRuleServiceClient())
            {
                MethodReturnResult<BinRule> result = await client.GetAsync(new BinRuleKey()
                {
                         BinNo = model.BinNo,
                         Color = model.Color,
                         Grade = model.Grade,
                         PackageLine = model.PackageLine,
                         PsCode = model.PsCode,
                         PsSubCode = model.PsSubCode,
                         PsItemNo = model.PsItemNo,
                         WorkOrderNumber = model.WorkOrderNumber
                });

                if (result.Code == 0)
                {
                    result.Data.LocationName = model.LocationName;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.BinRule_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // POST: /FMM/BinRule/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string BinNo, string Color, string Grade, string PackageLine, string PsCode, string PsSubCode, int PsItemNo, string WorkOrderNumber)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (BinRuleServiceClient client = new BinRuleServiceClient())
            {
               BinRuleKey key = new BinRuleKey()
                {
                    BinNo = BinNo,
                    Color = Color,
                    Grade = Grade,
                    PackageLine = PackageLine,
                    PsCode = PsCode,
                    PsSubCode = PsSubCode,
                    PsItemNo = PsItemNo,
                    WorkOrderNumber = WorkOrderNumber
                };
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.BinRule_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}