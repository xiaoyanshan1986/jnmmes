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
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using System.Text;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotIVDataController : Controller
    {
        // GET: WIP/LotIVData
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save(LotIVDataViewModels model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                LotIVDataParameter p = new LotIVDataParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    LotNumber = model.LotNumber,       
                    ShiftName = "",
                    Remark  = "",
                    LotNumbers = new List<string>()
                };

                char splitChar = ',';
                //获取批次号值。
                string[] lotNumbers = Request["LotNumber"].ToUpper().Split(splitChar);
                p.LotNumbers = lotNumbers.ToList();

                for (int i = 0; i < p.LotNumbers.Count; i++)
                {
                    p.LotNumber = p.LotNumbers[i];
                    using (LotTrackOutServiceClient client = new LotTrackOutServiceClient())
                    {
                        result = client.ModifyIVDataForLot(p);
                    }
                    if (result.Code == 0)
                    {
                        result.Message += string.Format("批次{0}修改IVData成功.{1}", model.LotNumber, result.Message);
                    }
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
    }
}