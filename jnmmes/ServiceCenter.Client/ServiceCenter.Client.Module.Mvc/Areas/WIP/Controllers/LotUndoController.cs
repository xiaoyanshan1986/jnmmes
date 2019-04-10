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
using System.Data;
using ServiceCenter.MES.Service.Contract.ZPVM;

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
        public ActionResult Undo(string lotNumber, string transactionKey, string description)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (string.IsNullOrEmpty(lotNumber) || string.IsNullOrEmpty(transactionKey))
                {
                    result.Code = 1;
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
                MethodReturnResult<LotTransaction> rst = client.GetTransaction(key);
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
                MethodReturnResult<IList<Resource>> rst = client.GetResourceList(User.Identity.Name, ResourceType.RouteOperation);
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

            string strRouteStepName = obj.RouteStepName;

            if (strRouteStepName == "")
            {
                strRouteStepName = "创批";
            }

            var lnq = from item in lstResource
                      where item.Data == strRouteStepName
                      select item;

            if (lnq.Count() == 0)
            {
                result.Code = 1;
                result.Message = string.Format("用户({0})权限不足，对工序（{1}）的操作不能撤销。", User.Identity.Name, strRouteStepName);
                return result;
            }

            return result;
        }
        [HttpPost]
        public ActionResult Query(string lotNumber)
        {
            MethodReturnResult result = GetLot(lotNumber);
            if (lotNumber.Contains(",")) 
            {
                result.Message = string.Format("批次（{0}）已撤销完成。", lotNumber);
            }
            if (result.Code > 0)
            {
                return Json(result);
            }

            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {

                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("LotNumber='{0}' AND UndoFlag = 0 AND Activity <> '{1}'"
                                            , lotNumber
                                            , Convert.ToInt32(EnumLotActivity.Undo)),
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


        public ActionResult UndoIndex()
        {
            return View("UndoIndex", new LotUndoViewModel());
        }
        public ActionResult QueryUndo(string lotNumber)
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
                    IsPaging = false,
                    Where = string.Format("LotNumber='{0}' AND UndoFlag=0 AND Activity>'{1}'"
                                            , lotNumber
                                            , Convert.ToInt32(EnumLotActivity.Undo)),
                    OrderBy = "CreateTime DESC"
                };

                MethodReturnResult<IList<LotTransaction>> rst = client.GetTransaction(ref cfg);

                if (rst.Code > 0)
                {
                    return Json(rst);
                }

                ViewBag.TransactionList = rst.Data;
                return PartialView("_UndoListPartial", new LotViewModel());
            }
        }

        public ActionResult BatchRevocationIndex()
        {
            return View();
        }

        /// <summary>批次进行批量查询 </summary>
        /// <param name="lotNumber"></param>
        /// <returns></returns>
        public ActionResult QueryBatchRevocation(string lotNumber)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                LotProcessingParameter param = new LotProcessingParameter();

                param.Lotlist = lotNumber;
                param.PageSize = 20;
                param.PageNo = -1;
                using (LotUndoServiceClient client = new LotUndoServiceClient())
                {
                    MethodReturnResult<DataSet> ds = client.GetLotProcessing(ref param);

                    if (ds.Code > 0)
                    {
                        result.Code = ds.Code;
                        result.Message = ds.Message;
                        result.Detail = ds.Detail;

                        return Json(result);
                    }

                    ViewBag.ListData = ds.Data.Tables[0];
                    ViewBag.PagingConfig = new PagingConfig()
                    {
                        PageNo = -1,
                        PageSize = 20,
                    };
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
                return PartialView("_BatchRevocationListPartial", new LotViewModel());
            }
            else
            {
                return View("BatchRevocationIndex", new LotViewModel());
            }
        }

        /// <summary>批量进行批次撤销 </summary>
        /// <param name="LotNumber"></param>
        /// <returns></returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBatchRevocation(string LotNumber)
        {
            MethodReturnResult result = new MethodReturnResult();
            LotTransaction LotTransaction = null;
            LotTransaction LotTransaction1 = null;
            LotTransaction LotTransaction2 = null;
            try
            {
                UndoParameter p = new UndoParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    UndoTransactionKeys = new Dictionary<string, IList<string>>(),
                    LotNumbers = new List<string>()
                };

                char splitChar = ',';

                //获取批次号值

                string[] lotNumbers = Request["LotNumber"].ToUpper().Split(splitChar);
                p.LotNumbers = lotNumbers.ToList();

                for (int i = 0; i < p.LotNumbers.Count; i++)
                {
                    string lotNumber = p.LotNumbers[i];
                    if (i == 0) 
                    {
                        //获取第一个批次的最新一条加工历史数据
                        LotTransaction = GetLotLastTransaction(p.LotNumbers[0]);
                        LotTransaction2 = LotTransaction;
                       
                    }
                    else 
                    {
                        //获取除第一个批次的最新一条加工历史数据
                        LotTransaction1 = GetLotLastTransaction(lotNumber);
                        LotTransaction2 = LotTransaction1;
                    }
                    if (LotTransaction != null && LotTransaction1 != null)
                    {

                        //判定批次是否为同一个工艺流程组。
                        if (LotTransaction.RouteEnterpriseName != LotTransaction1.RouteEnterpriseName)
                        {
                            result.Code = 1001;
                            result.Message = string.Format("批次（{0}）与批次（{1}）工艺流程组不相同。", p.LotNumbers[0], lotNumber);
                            return Json(result);

                        }
                        //判定批次是否为同一个工艺流程。
                        if (LotTransaction.RouteName != LotTransaction1.RouteName)
                        {
                            result.Code = 1002;
                            result.Message = string.Format("批次（{0}）与批次（{1}）工艺流程不相同。", p.LotNumbers[0], lotNumber);
                            return Json(result);

                        }
                        //判定批次是否为同一个工序。
                        if (LotTransaction.RouteStepName != LotTransaction1.RouteStepName)
                        {
                            result.Code = 1003;
                            result.Message = string.Format("批次（{0}）与批次（{1}）工序不相同。", p.LotNumbers[0], lotNumber);
                            return Json(result);

                        }
                        //判定批次是否为同一个操作流程。
                        if (LotTransaction.Activity != LotTransaction1.Activity)
                        {
                            result.Code = 1004;
                            result.Message = string.Format("批次（{0}）与批次（{1}）操作名称不相同。", p.LotNumbers[0], lotNumber);
                            return Json(result);
                        }
                    }
                    //p.LotNumbers.Add(lotNumber);
                    p.UndoTransactionKeys.Add(lotNumber, new List<string>());
                    p.UndoTransactionKeys[lotNumber].Add(LotTransaction2.Key);
                }
               

                if (result.Code == 0) 
                {
                    using (LotUndoServiceClient client = new LotUndoServiceClient())
                    {
                       
                        result = client.Undo(p);
                    }
                }
              
                if (result.Code == 0)
                {
                    return Query(LotNumber);
                }
                if (result.Code > 0)
                {
                    return Json(result);
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

        /// <summary> 获取最新的一条加工历史数据/// </summary>
        /// <param name="LotNumber"></param>
        /// <returns></returns>
        public LotTransaction GetLotLastTransaction(string LotNumber)
        {
            LotTransaction obj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("LotNumber='{0}' AND UndoFlag=0 AND Activity!='{1}'"
                                            , LotNumber
                                            , Convert.ToInt32(EnumLotActivity.Undo)),
                    OrderBy = "EditTime DESC"
                };

                MethodReturnResult<IList<LotTransaction>> rst = client.GetTransaction(ref cfg);
                obj = rst.Data[0];
            }
            return obj;
        }
    }
}