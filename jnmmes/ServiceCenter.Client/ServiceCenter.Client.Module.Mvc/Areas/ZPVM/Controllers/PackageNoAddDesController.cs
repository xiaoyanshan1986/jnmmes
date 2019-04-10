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
using ServiceCenter.MES.Service.Contract.WIP;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class PackageNoAddDesController : Controller
    {
        //
        // GET: /ZPVM/PackageNoAdd/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取包装号数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GetRPTPackageNoInfo(PackageNoAddDesViewModel model)
        {
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                RPTpackagelistParameter param = new RPTpackagelistParameter();
                param.PackageNo = model.PackageNo;
                MethodReturnResult<DataSet> result = client.GetRPTPackageNoInfo(param);
                if (result.Code == 0)
                {
                    ViewBag.List = result.Data.Tables[0];
                };
            }
            return PartialView("_ListPartial");
        }


        /// <summary>
        /// 修改对话框的内容
        /// </summary>
        /// <param name="packageno"></param>
        /// <param name="description"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ActionResult Modify(string packageno, string description, string stepname)
        {
            return PartialView("_ModifyPartial", new PackageNoAddDesViewModel()
            {
                PackageNo = packageno,
                Action = stepname,
                Description = description
            });
        }

        /// <summary>
        /// 保存新增描述
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveModify(PackageNoAddDesViewModel model)
        {
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {
                MethodReturnResult<Package> result = client.Get(model.PackageNo);
                if (result.Code == 0)
                {
                    result.Data.Description = model.Description;
                    MethodReturnResult rst = client.UpdateAdd(result.Data, model.Action);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format("包装号{0}添加描述成功"
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                else
                {
                    result.Message = string.Format("包装号{0}不存在于当前库，已归档", model.PackageNo);
                    return Json(result);
                }
            }
        }

    }
}