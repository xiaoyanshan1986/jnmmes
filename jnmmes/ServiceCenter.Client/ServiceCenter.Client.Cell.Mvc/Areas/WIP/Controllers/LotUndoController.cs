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
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.RBAC;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotUndoController : Controller
    {
        //
        // GET: /WIP/LotUndo/
        public ActionResult Index()
        {
            return View(new LotUndoViewModel());
        }

        //
        // POST: /WIP/LotUndo/Undo
        [HttpPost]
        public ActionResult Undo(string lotNumber,string transactionKey,string description)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (string.IsNullOrEmpty(lotNumber) || string.IsNullOrEmpty(transactionKey))
                {
                    result.Code=1;
                    result.Message = "参数为空。";
                    return Json(result);
                }

                //获取批次值。
                lotNumber = lotNumber.ToUpper();
                result = GetLot(lotNumber);
                if (result.Code > 0)
                {
                    return Json(result);
                }

                //获取批次操作数据
                result = GetLotTransaction(transactionKey);
                if (result.Code > 0)
                {
                    return Json(result);
                }

                UndoParameter p = new UndoParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    UndoTransactionKeys = new Dictionary<string, IList<string>>(),
                    Remark = description,
                    LotNumbers = new List<string>()
                };
                p.LotNumbers.Add(lotNumber);
                p.UndoTransactionKeys.Add(lotNumber, new List<string>());
                p.UndoTransactionKeys[lotNumber].Add(transactionKey);

                //批次撤销操作。
                using (LotUndoServiceClient client = new LotUndoServiceClient())
                {
                    result = client.Undo(p);
                }
                if (result.Code == 0)
                {
                    return Query(lotNumber);
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

            //判断用户生产线权限
            if (!string.IsNullOrEmpty(obj.PreLineCode))
            {
                IList<Resource> lstResource = new List<Resource>();
                using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
                {
                    MethodReturnResult<IList<Resource>> rst1 = client.GetResourceList(User.Identity.Name, ResourceType.ProductionLine);
                    if (rst1.Code <= 0 && rst1.Data != null)
                    {
                        lstResource = rst1.Data;
                    }
                    else
                    {
                        result.Code = rst1.Code;
                        result.Message = rst1.Message;
                        result.Detail = rst1.Detail;
                        return result;
                    }
                }
                var lnq = from item in lstResource
                          where item.Data == obj.PreLineCode
                          select item;
                if (lnq.Count() == 0)
                {
                    result.Code = 1;
                    result.Message = string.Format("用户({0})权限不足，对生产线（{1}）的操作不能撤销。", User.Identity.Name, obj.PreLineCode);
                    return result;
                }
            }
            return rst;
        }

        public MethodReturnResult GetLotTransaction(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            LotTransaction obj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                MethodReturnResult<LotTransaction> rst =  client.GetTransaction(key);
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
            if (obj == null)
            {
                result.Code = 2001;
                result.Message = "被撤销操作不存在。";
                return result;
            }
            //判断用户拥有的工序权限
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> rst = client.GetResourceList(User.Identity.Name,ResourceType.RouteOperation);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    lstResource = rst.Data;
                }
                else
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.Detail = rst.Detail;
                    return result;
                }
            }
            var lnq = from item in lstResource
                    where item.Data==obj.RouteStepName
                    select item;
            if (lnq.Count()==0) {
                result.Code = 1;
                result.Message = string.Format("用户({0})权限不足，对工序（{1}）的操作不能撤销。",User.Identity.Name,obj.RouteStepName);
                return result;
            }
            return result;
        }
        [HttpPost]
        public ActionResult Query(string lotNumber)
        {
            MethodReturnResult result = GetLot(lotNumber);
            if (result.Code > 0)
            {
                return Json(result);
            }

            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging=false,
                    Where = string.Format("LotNumber='{0}' AND UndoFlag=0 AND Activity>'{1}'"
                                            ,lotNumber
                                            ,Convert.ToInt32(EnumLotActivity.Undo)),
                    OrderBy = "CreateTime DESC"
                };

                MethodReturnResult<IList<LotTransaction>> rst = client.GetTransaction(ref cfg);

                if (rst.Code > 0)
                {
                    return Json(rst);
                }

                ViewBag.TransactionList = rst.Data;
                return PartialView("_ListPartial");
            }
        }
	}
}