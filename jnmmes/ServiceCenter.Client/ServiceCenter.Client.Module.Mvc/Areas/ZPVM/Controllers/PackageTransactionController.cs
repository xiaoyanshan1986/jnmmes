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
    public class PackageTransactionController : Controller
    {
        //
        // GET: /ZPVM/PackageTransaction/
        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult GetPackageTransaction(PackageTransactionQueryViewModel model)
        //{
        //    using (PackageQueryServiceClient client = new PackageQueryServiceClient())
        //    {
                
        //            MethodReturnResult<DataSet> result = client.GetPackageTransaction(model.PackageNo);

        //            if (result.Code == 0)
        //            {
        //                ViewBag.List = result.Data.Tables[0];
        //            };
        //    }
        //    return PartialView("_ListPartial");

        //}
        /// <summary>包装历史记录数据查询（存储过程获取数据） </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GetPackageTransaction(PackageTransactionViewModel model)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                RPTpackagelistParameter param = GetQueryConditionP(model);
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    MethodReturnResult<DataSet> ds = client.GetPackageTransactionQueryDb(ref param);
                    ViewBag.ListData = ds.Data.Tables[0];
                    ViewBag.PagingConfig = new PagingConfig()
                    {
                        PageNo = model.PageNo,
                        PageSize = model.PageSize,
                        Records = param.TotalRecords

                    };
                    model.TotalRecords = param.TotalRecords;
                }

            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", model);
            }
            else
            {
                return View("Index", model);
            }

        }
        public ActionResult Detail(string packageNo)
        {
            using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            {

                MethodReturnResult<DataSet> result = client.GetPackageTransaction(packageNo);

                if (result.Code == 0)
                {
                    ViewBag.List = result.Data.Tables[0];
                };
            }
            return View("DetailPartial");

        }
        public RPTpackagelistParameter GetQueryConditionP(PackageTransactionViewModel model)
        {
            RPTpackagelistParameter p = new RPTpackagelistParameter()
            {
                PackageNo = model.PackageNo,//包装号
                PageNo = model.PageNo,     //页号
                PageSize = model.PageSize  //页码尺寸
            };
            return p;
        }

	}
}