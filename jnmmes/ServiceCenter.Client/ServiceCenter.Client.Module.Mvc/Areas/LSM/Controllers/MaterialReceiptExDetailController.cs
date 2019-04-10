using ServiceCenter.Client.Mvc.Areas.LSM.Models;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Contract.LSM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LSMResources = ServiceCenter.Client.Mvc.Resources.LSM;

namespace ServiceCenter.Client.Mvc.Areas.LSM.Controllers
{
    public class MaterialReceiptExDetailController : Controller
    {
        //
        // GET: /LSM/MaterialReceiptExDetail/
        public ActionResult Index(string ReceiptNo)
        {
            MaterialReceiptExDetailQueryViewModel model = new MaterialReceiptExDetailQueryViewModel ();


            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                OrderBy = "Key.ItemNo",
                Where = string.Format(" Key.ReceiptNo = '{0}'"
                                            , ReceiptNo)
            };
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                MethodReturnResult<IList<MaterialReceiptDetail>> result = client.GetDetail(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
               MethodReturnResult<MaterialReceipt> res =  client.Get(ReceiptNo);
                if (res.Code==0)
	            {
		            model.ReceiptNo =res.Data.Key;
                    model.OrderNumber = res.Data.OrderNumber;
                    model.LineStoreName = res.Data.LineStore;
                    model.ReceiptDate = res.Data.ReceiptDate.ToString("yyyy-MM-dd");
                    ViewBag.State = res.Data.State;
	            }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new MaterialReceiptExDetailViewModel() { ReceiptNo = ReceiptNo });
            }
            return View("Index", model);

        }

        public ActionResult Query(MaterialReceiptExDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                {
                    StringBuilder where = new StringBuilder();
                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.ReceiptNo))
                        {
                            where.AppendFormat(" {0} Key.ReceiptNo LIKE '{1}%'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.ReceiptNo);
                        }
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key",
                        Where = where.ToString()
                    };

                    MethodReturnResult<IList<MaterialReceiptDetail>> result = client.GetDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                }
            }
            return PartialView("_ListPartial");
        }

        public ActionResult Save(MaterialReceiptExDetailQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            MaterialReceiptDetailParamter paramter = new MaterialReceiptDetailParamter()
            {
                ReceiptNo = model.ReceiptNo,
                MaterialLotNumber = model.MaterialLot,
                Creator = User.Identity.Name,
                IsReceiptOfCell = true
            };

            try
            {
                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                {
                    result = client.AddMaterialReceiptDetail(paramter);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format("添加{0}成功"
                                                    , model.MaterialLot);
                        
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

        public ActionResult Approve(MaterialReceiptExDetailQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            MaterialReceiptParamter paramter = new MaterialReceiptParamter()
            {
                ReceiptNo = model.ReceiptNo,
                Creator = User.Identity.Name,
            };

            try
            {
                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                {
                    result = client.ApproveMaterialReceipt(paramter);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format("审批{0}成功"
                                                    , model.ReceiptNo);
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
        public ActionResult Delete(string ReceiptNo, string MaterialLotNumber)
        {
            MethodReturnResult result = new MethodReturnResult();
            MaterialReceiptDetailParamter paramter = new MaterialReceiptDetailParamter()
            {
                ReceiptNo = ReceiptNo,
                MaterialLotNumber = MaterialLotNumber,
                Creator = User.Identity.Name,
                IsReceiptOfCell = true
            };
            try
            {
                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                {
                    result = client.DeleteMaterialReceiptDetail(paramter);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format("删除{0}成功"
                                                    , MaterialLotNumber);
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


	}
}