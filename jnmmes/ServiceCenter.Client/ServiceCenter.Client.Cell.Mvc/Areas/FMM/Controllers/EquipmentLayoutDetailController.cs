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
    public class EquipmentLayoutDetailController : Controller
    {

        //
        // GET: /FMM/EquipmentLayoutDetail/
        public async Task<ActionResult> Index(string layoutName)
        {
            using (EquipmentLayoutServiceClient client = new EquipmentLayoutServiceClient())
            {
                MethodReturnResult<EquipmentLayout> result = await client.GetAsync(layoutName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "EquipmentLayout");
                }
                ViewBag.EquipmentLayout = result.Data;
            }

            using (EquipmentLayoutDetailServiceClient client = new EquipmentLayoutDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        Where = string.Format(" Key.LayoutName = '{0}'"
                                              , layoutName)
                    };
                    MethodReturnResult<IList<EquipmentLayoutDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new EquipmentLayoutDetailViewModel() { LayoutName = layoutName });
            }
            else
            {
                return View(new EquipmentLayoutDetailQueryViewModel() { LayoutName = layoutName });
            }
        }

        //
        // POST: /FMM/EquipmentLayoutDetail/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(EquipmentLayoutDetailViewModel model)
        {
             MethodReturnResult result = new MethodReturnResult();
            try
            {
                using (EquipmentLayoutDetailServiceClient client = new EquipmentLayoutDetailServiceClient())
                {
                    EquipmentLayoutDetail obj = new EquipmentLayoutDetail()
                    {
                        Key = new EquipmentLayoutDetailKey()
                        {
                            LayoutName = model.LayoutName,
                            EquipmentCode = model.EquipmentCode
                        },
                        Height=model.Height,
                        Left=model.Left,
                        Top=model.Top,
                        Width=model.Width,
                        Description=model.Description,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now
                    };
                    result = await client.AddAsync(obj);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format(FMMResources.StringResource.EquipmentLayoutDetail_Save_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> SaveModify(EquipmentLayoutDetailViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                using (EquipmentLayoutDetailServiceClient client = new EquipmentLayoutDetailServiceClient())
                {
                    EquipmentLayoutDetail obj = new EquipmentLayoutDetail()
                    {
                        Key = new EquipmentLayoutDetailKey()
                        {
                            LayoutName = model.LayoutName,
                            EquipmentCode = model.EquipmentCode
                        },
                        Height = model.Height,
                        Left = model.Left,
                        Top = model.Top,
                        Width = model.Width,
                        Description = model.Description,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now
                    };
                    result = await client.ModifyAsync(obj);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format(FMMResources.StringResource.EquipmentLayoutDetail_Save_Success);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return Json(result);
        }

        //
        // POST: /FMM/EquipmentLayoutDetail/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string layoutName, string equipmentCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (EquipmentLayoutDetailServiceClient client = new EquipmentLayoutDetailServiceClient())
            {
                result = await client.DeleteAsync(new EquipmentLayoutDetailKey()
                {
                    LayoutName = layoutName,
                    EquipmentCode = equipmentCode
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.EquipmentLayoutDetail_Delete_Success
                                                    , equipmentCode);
                }
                return Json(result);
            }
        }

        public ActionResult GetEquipmentName(string equipmentCode)
        {
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                MethodReturnResult<Equipment> result = client.Get(equipmentCode);
                if (result.Code <= 0)
                {
                    return Json(result.Data.Name,JsonRequestBehavior.AllowGet);
                }
            }
            return Json(equipmentCode,JsonRequestBehavior.AllowGet);
        }
    }
}