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
    public class CalibrationPlateController : Controller
    {

        //
        // GET: /FMM/CalibrationPlate/
        public ActionResult Index()
        {
            return View(new CalibrationPlateQueryViewModel());
        }
        //
        //POST: /FMM/CalibrationPlate/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(CalibrationPlateQueryViewModel model)
        {
            string keyList = null;
            if (model.LineCode!=null||model.LocationName!=null)
            {
                using (CalibrationPlateLineServiceClient client = new CalibrationPlateLineServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = GetQueryCondition(model)
                    };
                    MethodReturnResult<IList<CalibrationPlateLine>> result = client.Get(ref cfg);
                    if (result.Code == 0 && result.Data.Count > 0)
                    {
                        StringBuilder strb = new StringBuilder();
                        foreach (var item in result.Data)
                        {
                            strb.Append("'" + item.Key.CalibrationPlateID + "',");
                        }
                        keyList = strb.ToString().Substring(0, strb.Length - 1);
                    }
                    else
                    {
                        return PartialView("_ListPartial");
                    }
                }
            }
            
            


            if (keyList!= null)
            {
                using (CalibrationPlateServiceClient client = new CalibrationPlateServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging =false,
                        Where = string.Format(" Key in ({0})"
                                                    , keyList)
                    };
                    MethodReturnResult<IList<CalibrationPlate>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;

                    }
                }
            }
            else
            {
                using (CalibrationPlateServiceClient client = new CalibrationPlateServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key"

                    };
                    MethodReturnResult<IList<CalibrationPlate>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.List = result.Data;

                    }
                }  
            }
                return PartialView("_ListPartial");

        }

        // POST: /FMM/CalibrationPlate/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(CalibrationPlateViewModel model)
        {
            using (CalibrationPlateServiceClient client = new CalibrationPlateServiceClient())
            {
                CalibrationPlate obj = new CalibrationPlate()
                {
                    Key = model.CalibrationPlateID,
                    CalibrationPlateType = model.CalibrationPlateType,
                    CalibrationPlateName = model.CalibrationPlateName,
                    PM = model.PM,
                    ISC = model.ISC,
                    VOC = model.VOC,
                    MaxPM = model.MaxPM,
                    MinPM = model.MinPM,
                    MaxISC = model.MaxISC,
                    MinISC = model.MinISC,
                    MaxVOC = model.MaxVOC,
                    MinVOC = model.MinVOC,
                    StdIsc1 = model.StdIsc1,
                    StdIsc2 = model.StdIsc2,
                    Stdsun1 = model.Stdsun1,
                    Stdsun2 = model.Stdsun2,
                    Explain = model.Explain,
                    Creator = User.Identity.Name,
                    CreateTime = DateTime.Now,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.CalibrationPlate_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }

        //
        // GET: /FMM/CalibrationPlate/Modify
        public async Task<ActionResult> Modify(string CalibrationPlateID)
        {
            CalibrationPlateViewModel viewModel = new CalibrationPlateViewModel();
            using (CalibrationPlateServiceClient client = new CalibrationPlateServiceClient())
            {
                MethodReturnResult<CalibrationPlate> result = await client.GetAsync(CalibrationPlateID);
                if (result.Code == 0)
                {
                    viewModel = new CalibrationPlateViewModel()
                    {
                        CalibrationPlateID = result.Data.Key,
                        CalibrationPlateType = result.Data.CalibrationPlateType,
                        CalibrationPlateName = result.Data.CalibrationPlateName,
                        PM = result.Data.PM,
                        ISC = result.Data.ISC,
                        VOC = result.Data.VOC,
                        MaxPM = result.Data.MaxPM,
                        MinPM = result.Data.MinPM,
                        MaxISC = result.Data.MaxISC,
                        MinISC = result.Data.MinISC,
                        MaxVOC = result.Data.MaxVOC,
                        MinVOC = result.Data.MinVOC,
                        StdIsc1 = result.Data.StdIsc1,  //标准电流1
                        StdIsc2 = result.Data.StdIsc2,//标准电流2
                        Stdsun1 = result.Data.Stdsun1,//标准光强1
                        Stdsun2 = result.Data.Stdsun2,//标准光强2
                        Explain = result.Data.Explain,
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
        public async Task<ActionResult> SaveModify(CalibrationPlateViewModel model)
        {
            using (CalibrationPlateServiceClient client = new CalibrationPlateServiceClient())
            {
                MethodReturnResult<CalibrationPlate> result = await client.GetAsync(model.CalibrationPlateID);

                if (result.Code == 0)
                {
                    result.Data.CalibrationPlateType = model.CalibrationPlateType;
                    result.Data.CalibrationPlateName = model.CalibrationPlateName;
                    result.Data.PM = model.PM;
                    result.Data.ISC = model.ISC;
                    result.Data.VOC = model.VOC;
                    result.Data.MaxPM = model.MaxPM;
                    result.Data.MinPM = model.MinPM;
                    result.Data.MaxISC = model.MaxISC;
                    result.Data.MinISC = model.MinISC;
                    result.Data.MaxVOC = model.MaxVOC;
                    result.Data.MinVOC = model.MinVOC;
                    result.Data.StdIsc1 = model.StdIsc1;
                    result.Data.StdIsc2 = model.StdIsc2;
                    result.Data.Stdsun1 = model.Stdsun1;
                    result.Data.Stdsun2 = model.Stdsun2;
                    result.Data.Explain = model.Explain;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.CalibrationPlate_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // POST: /FMM/CalibrationPlate/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (CalibrationPlateServiceClient client = new CalibrationPlateServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.CalibrationPlate_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }

        public string GetQueryCondition(CalibrationPlateQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {

                if (!string.IsNullOrEmpty(model.LocationName))
                {
                    where.AppendFormat(@" {0}  Key.LocationName='{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LocationName);
                }
                if (!string.IsNullOrEmpty(model.LineCode))
                {
                    where.AppendFormat(@" {0} Key.LineCode='{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LineCode);
                }
            }
            return where.ToString();
        }
    }
}