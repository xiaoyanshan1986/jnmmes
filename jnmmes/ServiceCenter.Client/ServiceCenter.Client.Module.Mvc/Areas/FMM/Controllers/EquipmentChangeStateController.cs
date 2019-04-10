using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class EquipmentChangeStateController : Controller
    {
        //
        // GET: /FMM/EquipmentChangeState/
        public async Task<ActionResult> Index()
        {
            using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        OrderBy = "Type,Key"
                    };
                    MethodReturnResult<IList<EquipmentState>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.StateList = result.Data;
                    }
                    else
                    {
                        ModelState.AddModelError("", result.Message);
                    }
                });
            }

            using (EquipmentChangeStateServiceClient client = new EquipmentChangeStateServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false
                    };
                    MethodReturnResult<IList<EquipmentChangeState>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.ChangeStateList = result.Data;
                    }
                    else
                    {
                        ModelState.AddModelError("", result.Message);
                    }
                });
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial");
            }
            else
            {
                return View();
            }
        }

        //
        // POST: /FMM/EquipmentGroup/SaveModify
        [HttpPost]
        public async Task<ActionResult> Change(string startState, string endState, bool isChecked)
        {
            MethodReturnResult result=new MethodReturnResult();                
            string changeStateName = string.Format("{0}->{1}", startState, endState);
            try
            {
                using (EquipmentChangeStateServiceClient client = new EquipmentChangeStateServiceClient())
                {
                    if (isChecked == false)
                    {
                        result = client.Delete(changeStateName);
                    }
                    else
                    {
                        EquipmentChangeState obj = new EquipmentChangeState()
                        {
                            Key = changeStateName,
                            FromStateName = startState,
                            ToStateName = endState,
                            CreateTime = DateTime.Now,
                            Creator = User.Identity.Name,
                            Editor = User.Identity.Name,
                            EditTime = DateTime.Now
                        };
                        result = await client.ModifyAsync(obj);
                    }
                }
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.EquipmentChangeState_Change_Success
                                                , changeStateName);
                }
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return Json(result);
        }
	}
}