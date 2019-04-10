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
    public class CalibrationPlateLineController : Controller
    {

        //
        // GET: /FMM/CalibrationPlateLine/
        public ActionResult Index()
        {
            return View(new CalibrationPlateLineQueryViewModel());
        }
        //
        //GET: /FMM/CalibrationPlateLine/Query
        public ActionResult Query(string CalibrationPlateID)
        {
            using (CalibrationPlateLineServiceClient client = new CalibrationPlateLineServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CalibrationPlateID = '{0}'", CalibrationPlateID)
                };
                MethodReturnResult<IList<CalibrationPlateLine>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.List = result.Data;
                    ViewBag.CalibrationPlateID = CalibrationPlateID;
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new CalibrationPlateLineViewModel() { CalibrationPlateID = CalibrationPlateID });
            }
            else
            {
                return View("Index");
            }
        }

        // POST: /FMM/CalibrationPlateLine/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(CalibrationPlateLineViewModel model)
        {
            using (CalibrationPlateLineServiceClient client = new CalibrationPlateLineServiceClient())
            {
                CalibrationPlateLineKey key = new CalibrationPlateLineKey()
                {
                    CalibrationPlateID = model.CalibrationPlateID,
                    LocationName = model.LocationName,
                    LineCode = model.LineCode
                };
                CalibrationPlateLine obj = new CalibrationPlateLine()
                {
                    Key = key,
                    Explain = model.Explain,
                    Creator = User.Identity.Name,
                    CreateTime = DateTime.Now,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.CalibrationPlateLine_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }

        //
        // GET: /FMM/CalibrationPlateLine/Modify
        public async Task<ActionResult> Modify(string CalibrationPlateID, string LocationName, string LineCode)
        {
            CalibrationPlateLineViewModel viewModel = new CalibrationPlateLineViewModel();
            using (CalibrationPlateLineServiceClient client = new CalibrationPlateLineServiceClient())
            {
                MethodReturnResult<CalibrationPlateLine> result = await client.GetAsync(new CalibrationPlateLineKey()
                {
                    CalibrationPlateID = CalibrationPlateID,
                    LocationName = LocationName,
                    LineCode = LineCode
                });
                if (result.Code == 0)
                {
                    viewModel = new CalibrationPlateLineViewModel()
                    {
                        CalibrationPlateID = result.Data.Key.CalibrationPlateID,
                        LocationName = result.Data.Key.LocationName,
                        LineCode = result.Data.Key.LineCode,
                        Explain = result.Data.Explain,
                        Creator = result.Data.Creator,
                        CreateTime = result.Data.CreateTime,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now,
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
        public async Task<ActionResult> SaveModify(CalibrationPlateLineViewModel model)
        {
            CalibrationPlateLineKey key = new CalibrationPlateLineKey()
            {
                CalibrationPlateID = model.CalibrationPlateID,
                LocationName = model.LocationName,
                LineCode = model.LineCode
            };
            using (CalibrationPlateLineServiceClient client = new CalibrationPlateLineServiceClient())
            {
                MethodReturnResult<CalibrationPlateLine> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.Explain = model.Explain;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.CalibrationPlateLine_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // POST: /FMM/CalibrationPlateLine/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string CalibrationPlateID, string LocationName, string LineCode)
        {
            CalibrationPlateLineKey key = new CalibrationPlateLineKey()
            {
                CalibrationPlateID = CalibrationPlateID,
                LocationName = LocationName,
                LineCode = LineCode
            };
            MethodReturnResult result = new MethodReturnResult();
            using (CalibrationPlateLineServiceClient client = new CalibrationPlateLineServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.CalibrationPlateLine_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}