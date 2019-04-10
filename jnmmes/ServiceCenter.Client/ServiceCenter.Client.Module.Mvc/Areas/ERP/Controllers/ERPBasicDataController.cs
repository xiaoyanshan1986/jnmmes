using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Areas.ERP.Models;
using ServiceCenter.Common.Model;
using ServiceCenter.MES.Service.Client.ERP;
using ServiceCenter.Model;

namespace ServiceCenter.Client.Mvc.Areas.ERP.Controllers
{
    public class ERPBasicDataController : Controller
    {
        // GET: ERP/ERPBasicData
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddMaterialTypeFromERP()
        {
            MethodReturnResult returnResult = new MethodReturnResult();
            using (ERPClient  ErpClient = new ERPClient())
            {
                BaseMethodParameter p = new BaseMethodParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                };
                returnResult = ErpClient.AddMaterialTypeFromERP(p);
            };
            return Json(returnResult);
        }
    }
}