using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotReleaseController : Controller
    {
        //
        // GET: /WIP/LotRelease/
        public ActionResult Index()
        {
            return View(new LotReleaseViewModel());
        }
        //
        // POST: /WIP/LotRelease/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotReleaseViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                ReleaseParameter p = new ReleaseParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    ReleaseDescription = model.ReleaseDescription,
                    ReleasePassword = model.ReleasePassword,
                    Remark = model.Description,
                    LotNumbers = new List<string>()
                };

                char splitChar = ',';
                //获取批次号值。
                string[] lotNumbers = Request["LotNumber"].ToUpper().Split(splitChar);
                p.LotNumbers = lotNumbers.ToList();

                for (int i = 0; i < p.LotNumbers.Count; i++)
                {
                    string lotNumber = p.LotNumbers[i];
                    result = GetLot(lotNumber);
                    if (result.Code > 0)
                    {
                        return Json(result);
                    }
                }
                //暂停批次。
                using (LotReleaseServiceClient client = new LotReleaseServiceClient())
                {
                    result = client.Release(p);
                }
                if (result.Code == 0)
                {
                    result.Message = string.Format("释放 {0} 成功。",model.LotNumber);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return Json(result);
        }

        public MethodReturnResult GetLot(string lotNumber)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<Lot> rst = null;
            Lot obj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                rst = client.Get(lotNumber);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    obj = rst.Data;
                }
                else
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.Detail = rst.Detail;
                    return result;
                }
            }
            if (obj == null || obj.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 2001;
                result.Message = string.Format(WIPResources.StringResource.LotIsNotExists, lotNumber);
                return result;
            }
            else if (obj.StateFlag == EnumLotState.Finished)
            {
                result.Code = 2002;
                result.Message = string.Format("批次({0})已完成。", lotNumber);
                return result;
            }
            else if (obj.Status == EnumObjectStatus.Disabled || obj.DeletedFlag == true)
            {
                result.Code = 2003;
                result.Message = string.Format("批次({0})已结束。", lotNumber);
                return result;
            }
            else if (obj.HoldFlag == false)
            {
                result.Code = 2004;
                result.Message = string.Format("批次({0})未暂停。", lotNumber);
                return result;
            }
            return rst;
        }
	}
}