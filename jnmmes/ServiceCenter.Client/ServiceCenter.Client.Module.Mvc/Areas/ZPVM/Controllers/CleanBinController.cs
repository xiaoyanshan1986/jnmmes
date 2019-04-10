using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Resources;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using System.IO;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Model.WIP;
using System.Data;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class CleanBinController : Controller
    {
        //
        // GET: /ZPVM/CleanBin/
        public ActionResult Index()
        {
            return View(new CleanBinViewModel());
        }
        public ActionResult CleanBin(CleanBinViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {

                result = client.CleanBin(model.LineCode, model.BinNo);
                if (result.Code == 0)
                {
                    if (model.BinNo == null)
                    {
                        result.Message = string.Format("清{0}线Bin成功！", model.LineCode);
                    }
                    else
                    {
                        result.Message = string.Format("清{0}线{1}号Bin成功！", model.LineCode, model.BinNo);
                    }

                };
            }
            return Json(result);

        }

	}
}