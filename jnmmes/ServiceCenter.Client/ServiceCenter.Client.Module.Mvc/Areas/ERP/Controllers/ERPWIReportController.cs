using ServiceCenter.Client.Mvc.Areas.ERP.Models;
using ServiceCenter.MES.Model.ERP;
using ServiceCenter.MES.Service.Client.ERP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Resources.ERP;
using System.Text;
using System.Data;
using System.Xml;
using System.Net;
using System.IO;
using ServiceCenter.MES.Service.Contract.ERP;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.Common;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;

namespace ServiceCenter.Client.Mvc.Areas.ERP.Controllers
{
    public class ERPWIReportController : Controller
    {
        // GET: ERP/ERPWIReport
        public ActionResult Index(string BillCode)
        {
            return View(new WOReportQueryViewModel()
            {
                BillState = EnumBillState.Apply.GetHashCode().ToString()
            });
        }

        public ActionResult Check()
        {
            return View(new WOReportDetailQueryViewModel());
        }

        //托号核对
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Check(WOReportDetailQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                using (WOReportClient clientOfWoReport = new WOReportClient())
                {
                    result = clientOfWoReport.CheckPackageInWIReportDetail(model.ObjectNumber, model.BillCode, User.Identity.Name);
                    //返回包装结果。
                    if (result.Code <= 0)
                    {
                        MethodReturnResult<WOReportDetailQueryViewModel> rstFinal = new MethodReturnResult<WOReportDetailQueryViewModel>()
                        {
                            Code = result.Code,
                            Data = model,
                            Detail = result.Detail,
                            HelpLink = result.HelpLink,
                            Message = result.Message,
                            ObjectNo = ((EnumBillCheckState)Convert.ToInt32(result.ObjectNo)).GetDisplayName()
                        };
                        return Json(rstFinal);
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

        //托号取消核对
        [HttpPost]
        public ActionResult UnCheck(string billCode, string packageNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                using (WOReportClient clientOfChest = new WOReportClient())
                {
                    result = clientOfChest.UnCheckPackageInWIReportDetail(packageNo, billCode, User.Identity.Name);
                    if (result.Code > 0)
                    {
                        return Json(result);
                    }
                }
                WOReportDetailQueryViewModel model = new WOReportDetailQueryViewModel()
                {
                    ObjectNumber = packageNo,
                    BillCode = billCode
                };

                //返回包装结果。
                if (result.Code <= 0)
                {
                    MethodReturnResult<WOReportDetailQueryViewModel> rstFinal = new MethodReturnResult<WOReportDetailQueryViewModel>()
                    {
                        Code = result.Code,
                        Data = model,
                        Detail = result.Detail,
                        HelpLink = result.HelpLink,
                        Message = result.Message,
                        ObjectNo = ((EnumBillCheckState)Convert.ToInt32(result.ObjectNo)).GetDisplayName()
                    };
                    return Json(rstFinal);
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

        //核对托号显示
        [HttpPost]
        public ActionResult QueryChecked(WOReportDetailViewModel model)
        {
            if (!string.IsNullOrEmpty(model.BillCode))
            {
                using (WOReportClient client = new WOReportClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.BillCode = '{0}' AND PackageCheckState={1}", model.BillCode, Convert.ToInt32(EnumPackageCheckState.Checked))
                    };
                    MethodReturnResult<IList<WOReportDetail>> result = client.GetWOReportDetail(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.WOReportDetailList = result.Data;
                    }

                    MethodReturnResult<WOReport> result1 = client.GetWOReport(model.BillCode);
                    if (result1.Code == 0)
                    {
                        ViewBag.WOReport = result1.Data;
                    }
                }
            }
            return PartialView("_ListPartialCheck", model);
        }

        // 获取入库单信息--当前数量
        public ActionResult GetMmStockInfo(string billCode)
        {
            decimal currentQuantity = 0;
            int code = 0;

            if (!string.IsNullOrEmpty(billCode))
            {
                //获取当前数量
                using (WOReportClient client = new WOReportClient())
                {
                    MethodReturnResult<WOReport> rst2 = client.GetWOReport(billCode);
                    if (rst2.Code == 1000)
                    {
                        return Json(rst2, JsonRequestBehavior.AllowGet);
                    }
                    if (rst2.Code <= 0 && rst2.Data != null && rst2.Data.BillCheckState == EnumBillCheckState.Checked)
                    {
                        currentQuantity = rst2.Data.TotalQty;
                    }
                }
            }

            return Json(new
            {
                Code = code,
                CurrentQuantity = currentQuantity
            }, JsonRequestBehavior.AllowGet);
        }       

        public ActionResult Query(WOReportQueryViewModel model)
        {
            using (WOReportClient client = new WOReportClient())
            {
                StringBuilder where = new StringBuilder();

                //仅查询产品入库单并且入库状态为申报完成及入库完成单据
                where.AppendFormat(" BillType = 0 AND (BillState = 1 or BillState = 2 ) ");
                                        
                if (!string.IsNullOrEmpty(model.BillCode))
                {
                    where.AppendFormat(" {0} Key LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BillCode);
                }

                //入库类型
                if (model.BillType != null && model.BillType != "")
                {
                    where.AppendFormat(" {0} BillType = {1}"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BillType);
                }

                //状态
                if ( model.BillState != null && model.BillState != "" )
                {
                    where.AppendFormat(" {0} BillState = {1}"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BillState);
                }

                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "EditTime desc",
                    Where = where.ToString()
                };

                MethodReturnResult<IList<WOReport>> result = client.GetWOReport(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }
            

            return PartialView("_ListPartial");
        }

        public ActionResult Detail(string BillCode)
        {
            WOReportDetailQueryViewModel model = new WOReportDetailQueryViewModel();

            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<WOReport> rst = client.GetWOReport(BillCode);

                if (rst.Code > 0 || rst.Data == null)
                {
                    return RedirectToAction("Index", "ERPWIReport");
                }
                else
                {
                    model.BillCode = rst.Data.Key;                  //入库单号

                    model.BillDate = rst.Data.BillDate;             //入库日期
                    model.BillMakedDate = rst.Data.BillMakedDate;   //制单日期
                    model.ScrapType = ServiceCenter.MES.Model.ERP.EnumScrapType.False;  //报废单标识
                    model.MaterialCode = rst.Data.MaterialCode;     //产品代码
                    model.OrderNumber = rst.Data.OrderNumber;       //工单号
                    model.Note = rst.Data.Note;                     //备注
                    model.BillState = rst.Data.BillState;           //单据状态
                    model.BillType = rst.Data.BillType;             //入库类型
                    //model.Store = "CP002";                          //设置默认仓库
                }

                List<SelectListItem> StoreList = new List<SelectListItem>();
                MethodReturnResult<DataSet> ds = client.GetStore();

                //取得ERP仓库列表
                if (ds.Code == 0)
                {
                    for (int i = 0; i < ds.Data.Tables[0].Rows.Count; i++)
                    {
                        if (ds.Data.Tables[0].Rows[i]["STORCODE"].ToString() == "CP002")
                        {
                            StoreList.Insert(0, new SelectListItem() { Text = ds.Data.Tables[0].Rows[i]["STORNAME"].ToString(), Value = ds.Data.Tables[0].Rows[i]["STORCODE"].ToString() });
                        }
                        else
                        {
                            StoreList.Add(new SelectListItem() { Text = ds.Data.Tables[0].Rows[i]["STORNAME"].ToString(), Value = ds.Data.Tables[0].Rows[i]["STORCODE"].ToString() });
                        }
                    }
                }
                                
                ViewBag.Store = StoreList;

                //取得入库单明细
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "CreateTime",
                    Where = string.Format(" Key.BillCode = '{0}'"
                                                , BillCode)
                };

                MethodReturnResult<IList<WOReportDetail>> result = client.GetWOReportDetail(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_DetailListPartial", new WOReportDetailViewModel() { BillCode = BillCode });
            }
            else
            {
                return View(model);
            }
        }
        
        /// <summary>
        /// 入库接收操作
        /// </summary>
        /// <param name="BillCode">入库单号</param>
        /// <param name="store">仓库代码</param>
        /// <param name="ScrapType">是否报废</param>
        /// <returns></returns>
        public ActionResult StockInReceive(string BillCode, string store, string ScrapType)
        {
            MethodReturnResult result = new MethodReturnResult();
            DateTime dtStartTime = DateTime.Now;
            DateTime dtOperationTime = DateTime.Now;
            try
            {
                //判断仓库代码是否为空
                if (store == "")
                {
                    result.Code = 1006;
                    result.Message = string.Format("仓库代码为空！");
                    return Json(result);
                }

                using (WOReportClient client = new WOReportClient())
                {
                    #region 1.取得入库单表头对象
                    MethodReturnResult<WOReport> rstInWOrder = client.GetWOReport(BillCode);

                    if (rstInWOrder.Data.BillState != EnumBillState.Apply)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("入库单[{0}]入库状态[{1}]", 
                                                        BillCode,
                                                        rstInWOrder.Data.BillState.GetDisplayName());
                        return Json(result);
                    }
                    if (rstInWOrder.Data.BillCheckState != EnumBillCheckState.Checked)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("入库单[{0}]接收核对状态为[{1}]",
                                                        BillCode,
                                                        rstInWOrder.Data.BillCheckState.GetDisplayName());
                        return Json(result);
                    }
                    #endregion

                    #region 2.判断ERP入库单已经生成
                    DataTable dt_ErpInCode = GetERPCodeByBillCode(BillCode);

                    if (dt_ErpInCode.Rows.Count > 0)
                    {
                        result.Code = 1002;
                        result.Message = string.Format("入库单[{0}]已经完成入库，ERP入库单号[{1}]。",
                                                        BillCode,
                                                        dt_ErpInCode.Rows[0]["vbillcode"].ToString());
                        return Json(result);
                    }
                    #endregion

                    #region 3.托号合规性检查
                    
                    #region 3.1判断托号是否已在其他待入库单据
                    //取得入库单明细数据
                    PagingConfig cfg00 = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(" Key.BillCode = '{0}'"
                                                    , BillCode)
                    };

                    MethodReturnResult<IList<WOReportDetail>> resultIWDetail00 = client.GetWOReportDetail(ref cfg00);

                    if (resultIWDetail00.Code > 0)
                    {
                        return Json(resultIWDetail00);
                    }
                    else
                    {
                        foreach (WOReportDetail item in resultIWDetail00.Data)
                        {
                            //判断托号是否已经存在入库单中
                            PagingConfig cfg1 = new PagingConfig()
                            {
                                IsPaging = false,
                                OrderBy = "ItemNo DESC",
                                Where = string.Format(" ObjectNumber = '{0}' and Key.BillCode <> '{1}'"
                                                        , item.ObjectNumber, BillCode)
                            };

                            MethodReturnResult<IList<WOReportDetail>> lstWOReportDetail1 = client.GetWOReportDetail(ref cfg1);

                            if (lstWOReportDetail1.Data != null && lstWOReportDetail1.Data.Count > 0)
                            {
                                for (int h = 0; h < lstWOReportDetail1.Data.Count; h++)
                                {
                                    PagingConfig cfg3 = new PagingConfig()
                                    {
                                        IsPaging = false,
                                        OrderBy = "EditTime DESC",
                                        Where = string.Format(" Key = '{0}' and (BillState = 0 or BillState = 1)"
                                                                , lstWOReportDetail1.Data[h].Key.BillCode)
                                    };

                                    MethodReturnResult<IList<WOReport>> lstWOReport = client.GetWOReport(ref cfg3);
                                    if (lstWOReport.Data != null && lstWOReport.Data.Count > 0)
                                    {
                                        result.Code = 1003;
                                        result.Message = String.Format("项目[{0}]已经在入库单{1}中！",
                                                                       item.ObjectNumber,
                                                                       lstWOReportDetail1.Data[h].Key.BillCode);

                                        return Json(result);
                                    }
                                }
                            }
                        }
                    }

                    #endregion                    

                    #endregion

                    #region 4.取得MES入库单明细数据
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(" Key.BillCode = '{0}'"
                                                    , BillCode)
                    };

                    List<WOReportDetail> lstStockInDetail = new List<WOReportDetail>();       //入库单明细列表
                    int i = 0;

                    MethodReturnResult<IList<WOReportDetail>> resultIWDetail = client.GetWOReportDetail(ref cfg);

                    if (resultIWDetail.Code > 0)
                    {
                        return Json(resultIWDetail);
                    }

                    //将入库单明细结果集放入可删除修改条数结果集类型
                    lstStockInDetail.AddRange(resultIWDetail.Data);
                    #endregion
                                       
                    #region 5.创建ERP入库单
                    DateTime dtERPStartTime = DateTime.Now;
                    DateTime dtERPEndTime = DateTime.Now;

                    string workOrderType = "";
                    string ERPStockInType = "";                         //ERP入库单类型代码
                    string ERPStockInTypeKey = "";                      //ERP入库单类型代码主键
                    string ERPStockInBillCode = "";                     //ERP入库单号
                    string ERPStockInBillKey = "";                      //ERP入库单主键                    
                    List<WOReportDetail> lstIWDetail = null;            //入库单明细列表
                    List<int> lstDelItemNo = null;                      //删除项目临时列表
                    string strERPOperationMSG = "";                     //ERP入库单生成信息
      
                    //创建入库单参数对象
                    WOReportParameter p = new WOReportParameter()
                    {
                        Store = store,                                      //仓库代码
                        BillCode = BillCode,                                //入库单号
                        Editor = User.Identity.Name,                        //编辑人
                        OperateComputer = Request.UserHostAddress,          //客户端
                        BillState = EnumBillState.Receive,                  //单据状态入库接收
                        ERPStockInCodes = new Dictionary<string, string>(), //ERP入库单号
                        ERPStockInKeys = new Dictionary<string, string>(),  //ERP入库单主键
                        OperationType = 0                                   //操作状态 0 - 新增申请                        
                    };

                CreateStockIn:      //循环处理不同工单类型入库明细

                    //初始化删除列表
                    lstDelItemNo = new List<int>();

                    //循环对于每一类工单生成单独入库单（生产入库、返工入库、试验入库）
                    for (i = 0; i < lstStockInDetail.Count; i++)
                    {
                        //取得工单对象
                        WorkOrder workOrder = new WorkOrder();

                        using (WorkOrderServiceClient clientwo = new WorkOrderServiceClient())
                        {
                            MethodReturnResult<WorkOrder> resultWorkOrder = clientwo.Get(lstStockInDetail[i].OrderNumber);

                            if (resultWorkOrder.Code > 0)
                            {
                                return Json(resultWorkOrder);
                            }

                            workOrder = resultWorkOrder.Data;
                        }

                        //第一行初始化工单类型
                        if (i == 0)
                        {
                            //初始化工单类型
                            workOrderType = workOrder.OrderType;

                            using (ERPClient ERPclient = new ERPClient())
                            {
                                //取得ERP入库单类型
                                ERPStockInType = ERPclient.GetERPStockInType(workOrderType);

                                //取得ERP入库单类型主键
                                ERPStockInTypeKey = ERPclient.GetERPStockInTypeKey(workOrderType);
                            }

                            //初始化入库单明细对象列表
                            lstIWDetail = new List<WOReportDetail>();
                        }
                        else
                        {
                            //判断入库单类型是否一致
                            if (workOrderType != workOrder.OrderType)
                            {
                                continue;
                            }
                        }

                        //将入库明细对象加入列表
                        lstIWDetail.Add(lstStockInDetail[i]);

                        //添加删除列表
                        lstDelItemNo.Add(i);
                    }

                    if (lstIWDetail.Count > 0)
                    {
                        dtERPStartTime = DateTime.Now;      //记录ERP入库单开始操作时间

                        //创建ERP入库单，成功后返回ERP入库单主键
                        result = CreateERPStockIn(ERPStockInType, ERPStockInTypeKey, rstInWOrder.Data, lstIWDetail, store, p.Editor, dtOperationTime);

                        if (result.Code > 0)
                        {
                            return Json(result);
                        }

                        //取得生成的ERP入库单主键
                        ERPStockInBillKey = result.ObjectNo;

                        //根据ERP入库主键取得ERP入库单号
                        result = client.GetERPStockInBillCodeByKey(ERPStockInBillKey);

                        if (result.Code > 0)
                        {
                            return Json(result);
                        }

                        ERPStockInBillCode = result.ObjectNo;

                        //记录ERP入库单结束操作时间
                        dtERPEndTime = DateTime.Now;

                        strERPOperationMSG += string.Format(" ERP入库单[{0}]创建时间：[{1}] 秒,[{2} - {3}] ",
                                                            ERPStockInBillKey,
                                                            (dtERPEndTime - dtERPStartTime).TotalSeconds.ToString("F1"),
                                                            dtERPStartTime.ToString(),
                                                            dtERPEndTime.ToString());
                                                
                        //设置所有入库明细对应的ERP入库单主键
                        foreach (WOReportDetail woReportDetail in lstIWDetail)
                        {
                            //记录每行对应的入库单号
                            p.ERPStockInCodes.Add(woReportDetail.Key.ItemNo.ToString(), ERPStockInBillCode);

                            //记录每行对应的入库单主键
                            p.ERPStockInKeys.Add(woReportDetail.Key.ItemNo.ToString(), ERPStockInBillKey);
                        }

                        //删除已经处理的入库单明细列
                        for (i = lstDelItemNo.Count - 1; i >= 0; i--)
                        {
                            //从数据清单中删除当前入库明细对象
                            lstStockInDetail.RemoveAt(lstDelItemNo[i]);
                        }

                        //当入库明细数据还有数据时继续生成ERP入库单
                        if (lstStockInDetail.Count > 0)
                        {
                            goto CreateStockIn;
                        }
                    }                    
                    #endregion

                    #region 6.入库操作
                    result = client.StockIn(p);

                    if (result.Code == 0)
                    {
                        DateTime dtEndTime = DateTime.Now;

                        result.Message = string.Format(StringResource.WOReport_StockInApply_Success, BillCode) +
                                         "执行时间：" + (dtEndTime - dtStartTime).TotalSeconds.ToString("F1") + " 秒，[ "
                                         + dtStartTime.ToString() + " - "
                                         + dtEndTime.ToString() + " ]"
                                         + strERPOperationMSG;
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.WOReport_StockInApply_Error, BillCode) + e.Message;
            }

            return Json(result);
        }

        /// <summary>
        /// 入库接收模拟修复
        /// </summary>
        /// <param name="BillCode"></param>
        /// <param name="store"></param>
        /// <param name="ScrapType"></param>
        /// <returns></returns>
        public ActionResult StockInReceiveTest(string BillCode, string store, string ScrapType)
        {
            MethodReturnResult result = new MethodReturnResult();
            UserInRoleServiceClient userInRoleClient = new UserInRoleServiceClient();

            UserInRoleKey userInRoleKey = new UserInRoleKey()
            {
                LoginName = User.Identity.Name,
                RoleName = "系统管理员"
            };

            MethodReturnResult<UserInRole> resultOfUserInRole = new MethodReturnResult<UserInRole>();
            resultOfUserInRole = userInRoleClient.Get(userInRoleKey);
            if (resultOfUserInRole == null || resultOfUserInRole.Data == null)
            {
                result.Code = 1002;
                result.Message = string.Format("用户{0}非系统管理员，不可执行入库修复。",
                                               User.Identity.Name);
                return Json(result);
            }
          
            DateTime dtStartTime = DateTime.Now;
            DateTime dtOperationTime = DateTime.Now;
            try
            {
                //判断仓库代码是否为空
                if (store == "")
                {
                    result.Code = 1006;
                    result.Message = string.Format("仓库代码为空！");
                    return Json(result);
                }

                using (WOReportClient client = new WOReportClient())
                {
                    #region 1.取得入库单表头对象
                    MethodReturnResult<WOReport> rstInWOrder = client.GetWOReport(BillCode);

                    if (rstInWOrder.Data.BillState != EnumBillState.Apply)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("入库单[{0}]入库状态[{1}]",
                                                        BillCode,
                                                        rstInWOrder.Data.BillState.GetDisplayName());
                        return Json(result);
                    }
                    #endregion

                    #region 2.判断ERP入库单已经生成
                    DataTable dt_ErpInCode = GetERPCodeByBillCode(BillCode);

                    if (dt_ErpInCode.Rows.Count > 0)
                    {
                        //result.Code = 1002;
                        //result.Message = string.Format("入库单[{0}]已经完成入库，ERP入库单号[{1}]。",
                        //                                BillCode,
                        //                                dt_ErpInCode.Rows[0]["vbillcode"].ToString());
                        //return Json(result);
                    }
                    #endregion

                    #region 3.判断托号是否已在其他待入库单据
                    //取得入库单明细数据
                    PagingConfig cfg00 = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(" Key.BillCode = '{0}'"
                                                    , BillCode)
                    };

                    MethodReturnResult<IList<WOReportDetail>> resultIWDetail00 = client.GetWOReportDetail(ref cfg00);

                    if (resultIWDetail00.Code > 0)
                    {
                        return Json(resultIWDetail00);
                    }
                    else
                    {
                        foreach (WOReportDetail item in resultIWDetail00.Data)
                        {
                            //判断托号是否已经存在入库单中
                            PagingConfig cfg1 = new PagingConfig()
                            {
                                IsPaging = false,
                                OrderBy = "ItemNo DESC",
                                Where = string.Format(" ObjectNumber = '{0}' and Key.BillCode <> '{1}'"
                                                        , item.ObjectNumber, BillCode)
                            };

                            MethodReturnResult<IList<WOReportDetail>> lstWOReportDetail1 = client.GetWOReportDetail(ref cfg1);

                            if (lstWOReportDetail1.Data != null && lstWOReportDetail1.Data.Count > 0)
                            {
                                for (int h = 0; h < lstWOReportDetail1.Data.Count; h++)
                                {
                                    PagingConfig cfg3 = new PagingConfig()
                                    {
                                        IsPaging = false,
                                        OrderBy = "EditTime DESC",
                                        Where = string.Format(" Key = '{0}' and (BillState = 0 or BillState = 1)"
                                                                , lstWOReportDetail1.Data[h].Key.BillCode)
                                    };

                                    MethodReturnResult<IList<WOReport>> lstWOReport = client.GetWOReport(ref cfg3);
                                    if (lstWOReport.Data != null && lstWOReport.Data.Count > 0)
                                    {
                                        result.Code = 1003;
                                        result.Message = String.Format("项目[{0}]已经在入库单{1}中！",
                                                                       item.ObjectNumber,
                                                                       lstWOReportDetail1.Data[h].Key.BillCode);

                                        return Json(result);
                                    }
                                }
                            }
                        }
                    }


                    #endregion

                    #region 4.取得MES入库单明细数据
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(" Key.BillCode = '{0}'"
                                                    , BillCode)
                    };

                    List<WOReportDetail> lstStockInDetail = new List<WOReportDetail>();       //入库单明细列表
                    int i = 0;

                    MethodReturnResult<IList<WOReportDetail>> resultIWDetail = client.GetWOReportDetail(ref cfg);

                    if (resultIWDetail.Code > 0)
                    {
                        return Json(resultIWDetail);
                    }

                    //将入库单明细结果集放入可删除修改条数结果集类型
                    lstStockInDetail.AddRange(resultIWDetail.Data);
                    #endregion

                    #region 5.创建ERP入库单
                    DateTime dtERPStartTime = DateTime.Now;
                    DateTime dtERPEndTime = DateTime.Now;

                    string workOrderType = "";
                    string ERPStockInType = "";                         //ERP入库单类型代码
                    string ERPStockInTypeKey = "";                      //ERP入库单类型代码主键
                    string ERPStockInBillCode = "";                     //ERP入库单号
                    string ERPStockInBillKey = "";                      //ERP入库单主键                    
                    List<WOReportDetail> lstIWDetail = null;            //入库单明细列表
                    List<int> lstDelItemNo = null;                      //删除项目临时列表
                    string strERPOperationMSG = "";                     //ERP入库单生成信息

                    //创建入库单参数对象
                    WOReportParameter p = new WOReportParameter()
                    {
                        Store = store,                                      //仓库代码
                        BillCode = BillCode,                                //入库单号
                        Editor = User.Identity.Name,                        //编辑人
                        OperateComputer = Request.UserHostAddress,          //客户端
                        BillState = EnumBillState.Receive,                  //单据状态入库接收
                        ERPStockInCodes = new Dictionary<string, string>(), //ERP入库单号
                        ERPStockInKeys = new Dictionary<string, string>(),  //ERP入库单主键
                        OperationType = 0                                   //操作状态 0 - 新增申请                        
                    };

                CreateStockIn:      //循环处理不同工单类型入库明细

                    //初始化删除列表
                    lstDelItemNo = new List<int>();

                    //循环对于每一类工单生成单独入库单（生产入库、返工入库、试验入库）
                    for (i = 0; i < lstStockInDetail.Count; i++)
                    {
                        //取得工单对象
                        WorkOrder workOrder = new WorkOrder();

                        using (WorkOrderServiceClient clientwo = new WorkOrderServiceClient())
                        {
                            MethodReturnResult<WorkOrder> resultWorkOrder = clientwo.Get(lstStockInDetail[i].OrderNumber);

                            if (resultWorkOrder.Code > 0)
                            {
                                return Json(resultWorkOrder);
                            }

                            workOrder = resultWorkOrder.Data;
                        }

                        //第一行初始化工单类型
                        if (i == 0)
                        {
                            //初始化工单类型
                            workOrderType = workOrder.OrderType;

                            using (ERPClient ERPclient = new ERPClient())
                            {
                                //取得ERP入库单类型
                                ERPStockInType = ERPclient.GetERPStockInType(workOrderType);

                                //取得ERP入库单类型主键
                                ERPStockInTypeKey = ERPclient.GetERPStockInTypeKey(workOrderType);
                            }

                            //初始化入库单明细对象列表
                            lstIWDetail = new List<WOReportDetail>();
                        }
                        else
                        {
                            //判断入库单类型是否一致
                            if (workOrderType != workOrder.OrderType)
                            {
                                continue;
                            }
                        }

                        //将入库明细对象加入列表
                        lstIWDetail.Add(lstStockInDetail[i]);

                        //添加删除列表
                        lstDelItemNo.Add(i);
                    }

                    if (lstIWDetail.Count > 0)
                    {
                        dtERPStartTime = DateTime.Now;      //记录ERP入库单开始操作时间

                        //创建ERP入库单，成功后返回ERP入库单主键
                        //result = CreateERPStockIn(ERPStockInType, ERPStockInTypeKey, rstInWOrder.Data, lstIWDetail, store, p.Editor, dtOperationTime);

                        if (result.Code > 0)
                        {
                            return Json(result);
                        }

                        //取得生成的ERP入库单主键
                        //ERPStockInBillKey = result.ObjectNo;
                        ERPStockInBillKey = dt_ErpInCode.Rows[0]["CGENERALHID"].ToString();

                        //根据ERP入库主键取得ERP入库单号
                        result = client.GetERPStockInBillCodeByKey(ERPStockInBillKey);

                        if (result.Code > 0)
                        {
                            return Json(result);
                        }

                        ERPStockInBillCode = result.ObjectNo;

                        //记录ERP入库单结束操作时间
                        dtERPEndTime = DateTime.Now;

                        strERPOperationMSG += string.Format(" ERP入库单[{0}]创建时间：[{1}] 秒,[{2} - {3}] ",
                                                            ERPStockInBillKey,
                                                            (dtERPEndTime - dtERPStartTime).TotalSeconds.ToString("F1"),
                                                            dtERPStartTime.ToString(),
                                                            dtERPEndTime.ToString());

                        //设置所有入库明细对应的ERP入库单主键
                        foreach (WOReportDetail woReportDetail in lstIWDetail)
                        {
                            //记录每行对应的入库单号
                            p.ERPStockInCodes.Add(woReportDetail.Key.ItemNo.ToString(), ERPStockInBillCode);

                            //记录每行对应的入库单主键
                            p.ERPStockInKeys.Add(woReportDetail.Key.ItemNo.ToString(), ERPStockInBillKey);
                        }

                        //删除已经处理的入库单明细列
                        for (i = lstDelItemNo.Count - 1; i >= 0; i--)
                        {
                            //从数据清单中删除当前入库明细对象
                            lstStockInDetail.RemoveAt(lstDelItemNo[i]);
                        }

                        //当入库明细数据还有数据时继续生成ERP入库单
                        if (lstStockInDetail.Count > 0)
                        {
                            goto CreateStockIn;
                        }
                    }
                    #endregion

                    #region 6.入库操作
                    result = client.StockIn(p);

                    if (result.Code == 0)
                    {
                        DateTime dtEndTime = DateTime.Now;

                        result.Message = string.Format(StringResource.WOReport_StockInApply_Success, BillCode) +
                                         "执行时间：" + (dtEndTime - dtStartTime).TotalSeconds.ToString("F1") + " 秒，[ "
                                         + dtStartTime.ToString() + " - "
                                         + dtEndTime.ToString() + " ]"
                                         + strERPOperationMSG;
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.WOReport_StockInApply_Error, BillCode) + e.Message;
            }

            return Json(result);
        }

        /// <summary>
        /// 生成ERP入库单（每个种工单类型一张,产成品、返工、试验）
        /// </summary>
        /// <param name="ERPStockInType">ERP入库类型代码</param>
        /// <param name="ERPStockInTypeKey">ERP入库类型主键</param>
        /// <param name="woReport">入库单表头对象</param>
        /// <param name="lstWORDetail">入库单表体对象列表</param>
        /// <param name="Store">ERP仓库代码</param>
        /// <param name="creator">制单人</param>
        /// <param name="dtOperationTime">操作时间</param>
        /// <returns></returns>
        public MethodReturnResult CreateERPStockIn(string ERPStockInType, string ERPStockInTypeKey, WOReport woReport, IList<WOReportDetail> lstWORDetail, string Store, string creator, DateTime dtOperationTime)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };           

            XmlDocument xmlDoc = new XmlDocument();     //XML对象
            string ERPAccount = "";                     //ERP账套代码
            string ERPGroupCode = "";                   //ERP集团代码
            string ERPOrg = "";                         //ERP组织代码            
            string ERPDEPTCode = "";                    //ERP生产部门编码（取自ERP报工单信息）
            DataTable dtWorkOrder;                      //ERP工单属性
            DateTime now = DateTime.Now;

            try
            {
                #region 根据ERP接口字符串取得ERP账套相关信息
                //取得ERP连接字符串
                string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];

                //创建WEB访问对象
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                //取得查询语句
                string ERPQuery = httpWebRequest.RequestUri.Query;

                //取得ERP账套代码
                ERPAccount = GetValueDataByString(ERPQuery, "account", "=", "&");
                ERPGroupCode = GetValueDataByString(ERPQuery, "groupcode", "=", "&");
                ERPOrg = GetValueDataByString(ERPQuery, "orgcode", "=", "&");

                #endregion

                //创建XML文件
                #region 1.创建类型声明节点
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");

                xmlDoc.AppendChild(node);
                #endregion
                
                #region 2.创建根节点
                XmlElement root = xmlDoc.CreateElement("ufinterface");

                root.SetAttribute("receiver", ERPOrg);          //ERP接收组织
                root.SetAttribute("sender", "mes");             //发送者
                root.SetAttribute("roottag", "");               //？？？
                root.SetAttribute("replace", "Y");              //
                root.SetAttribute("isexchange", "Y");           //
                root.SetAttribute("groupcode", ERPGroupCode);   //ERP组织编码
                root.SetAttribute("filename", "");              //文件名
                root.SetAttribute("billtype", "46");            //单据类型
                root.SetAttribute("account", ERPAccount);       //账套

                xmlDoc.AppendChild(root);
                #endregion

                #region 3.单据子节点
                //3.1单据节点
                XmlElement Node = xmlDoc.CreateElement("bill");

                Node.SetAttribute("id", "");
                xmlDoc.DocumentElement.AppendChild(Node);

                //3.2表头节点
                XmlNode billheadNode = xmlDoc.CreateNode(XmlNodeType.Element, "billhead", null);

                Node.AppendChild(billheadNode);

                //3.2.1创建表头属性节点
                CreateNode(xmlDoc, billheadNode, "cgeneralhid", "");              //入库单主键？？？

                //3.2.2创建产品节点
                XmlElement productNode = xmlDoc.CreateElement("cgeneralbid");
                billheadNode.AppendChild(productNode);

                #endregion
                
                #region 4.产品明细节点
                int rowNo = 0;              //行号

                //取得ERP报工单主键明细数据woReport.WRCode
                DataTable dtERPWRDetail = GetERPWRDetail(woReport.ERPWRKey);
                if (dtERPWRDetail == null || dtERPWRDetail.Rows.Count <= 0)
                {
                    result.Code = 1001;
                    result.Message = string.Format("ERP系统中报工单主键[{0}]对应的报工单号[{1}]不存在，请检查ERP系统中MES传入的单据号[{2}]对应的报工单是否被删除！"
                                                    , woReport.ERPWRKey    //报工单主键
                                                    , woReport.WRCode      //报工单号
                                                    , woReport.Key);       //MES入库单号

                    return result;
                }

                //取得报工单对应的生产部门代码
                ERPDEPTCode = dtERPWRDetail.Rows[0]["cdeptvid"].ToString();

                foreach (WOReportDetail worDetail in lstWORDetail)
                {
                    //取得ERP工单属性
                    dtWorkOrder = GetWorkOrder(worDetail.OrderNumber.ToString());

                    //取得报工单对应的生产部门代码
                    if (dtWorkOrder.Rows[0]["VBILLCODE"].ToString().ToUpper().Substring(0, 3) == "2MO" || dtWorkOrder.Rows[0]["VBILLCODE"].ToString().ToUpper().Substring(0, 3) == "8MO")
                    {
                        ERPDEPTCode = dtWorkOrder.Rows[0]["CJCODE"].ToString();
                    }                   

                    //根据工单、产品代码、批次（托号）、功率、电流、等级取得对应ERP报工单明细
                    DataRow[] wrERPDetailRow = dtERPWRDetail.Select(string.Format(" vbmobillcode = '{0}' " +        //工单号
                                                                                " and materialcode = '{1}' " +      //产品代码
                                                                                " and vbbatchcode = '{2}' " +       //批次（托号）
                                                                                " and vbfree1 = '{3}' " +           //功率
                                                                                " and vbfree2 = '{4}' " +           //电流
                                                                                " and vbfree3 = '{5}' "             //等级
                                                                    , worDetail.OrderNumber.ToString()              //工单号
                                                                    , worDetail.MaterialCode.ToString()             //产品代码
                                                                    , worDetail.ObjectNumber                        //批次（托号）
                                                                    , worDetail.EffiName.ToString()                 //功率
                                                                    , worDetail.PsSubcode.ToString()                //电流
                                                                    , worDetail.Grade.ToString())                   //等级
                                                                    );

                    if (wrERPDetailRow == null || wrERPDetailRow.Length == 0)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("ERP报工单[{0}]中工单[{1}]、产品[{2}]、批次[{3}]、功率[{4}]、电流[{5}、等级[{6}]]分类明细不存在！"
                                                        , woReport.WRCode                                   //报工单号
                                                        , worDetail.OrderNumber.ToString()                  //工单号
                                                        , worDetail.MaterialCode.ToString()                 //产品代码
                                                        , worDetail.ObjectNumber                            //批次（托号）
                                                        , worDetail.EffiName.ToString()                     //功率
                                                        , worDetail.PsSubcode.ToString()                    //电流
                                                        , worDetail.Grade.ToString());                      //等级

                        return result;
                    }

                    //4.1创建项目明细主节点
                    XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);

                    productNode.AppendChild(itemNode);

                    #region 4.2创建项目明细属性节点
                    CreateNode(xmlDoc, itemNode, "cgeneralbid", "");                                    //产成品入库单表体
                    CreateNode(xmlDoc, itemNode, "crowno", (rowNo++).ToString());                       //行号
                    CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);                             //集团
                    CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);                                     //所属组织
                    CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);                                   //工厂
                    CreateNode(xmlDoc, itemNode, "corpoid", ERPOrg);                                    //公司最新版本
                    CreateNode(xmlDoc, itemNode, "corpvid", ERPOrg);                                    //公司
                    CreateNode(xmlDoc, itemNode, "cstateid", "2");                                      //库存状态
                                        
                    CreateNode(xmlDoc, itemNode, "cbodywarehouseid", Store);                            //仓库

                    CreateNode(xmlDoc, itemNode, "dbizdate", dtOperationTime.ToString());               //入库日期
                    CreateNode(xmlDoc, itemNode, "dproducedate", woReport.BillDate.ToString());         //生产日期
                    CreateNode(xmlDoc, itemNode, "cmaterialoid", worDetail.MaterialCode);               //物料最新编码
                    CreateNode(xmlDoc, itemNode, "cmaterialvid", worDetail.MaterialCode);               //物料编码
                    CreateNode(xmlDoc, itemNode, "cproductid", worDetail.MaterialCode);                 //产品
                    CreateNode(xmlDoc, itemNode, "fproductclass", "1");                                 //产品类型???
                    //CreateNode(xmlDoc, itemNode, "csrcmaterialoid", "");//来源物料
                    //CreateNode(xmlDoc, itemNode, "csrcmaterialvid", "");//来源物料编码

                    CreateNode(xmlDoc, itemNode, "cunitid", "Pcs");                                     //主单位
                    CreateNode(xmlDoc, itemNode, "castunitid", "WA");                                   //单位
                    CreateNode(xmlDoc, itemNode, "vbatchcode", worDetail.ObjectNumber);                 //生产批次号(托号)
                    //CreateNode(xmlDoc, itemNode, "pk_batchcode", "");                                   //批次号编码

                    CreateNode(xmlDoc, itemNode, "nshouldnum", worDetail.Qty.ToString());               //应收主数量（块）                    
                    CreateNode(xmlDoc, itemNode, "nnum", worDetail.Qty.ToString());                     //实收主数量（块）

                    //（D级组件标称功率换算率为1，应收数量、实收数量（WA）为0，功率为000：0瓦）
                    if (worDetail.Grade == "D")
                    {
                        CreateNode(xmlDoc, itemNode, "nshouldassistnum", worDetail.Qty.ToString());          //应收数量（WA）
                        CreateNode(xmlDoc, itemNode, "nassistnum", worDetail.Qty.ToString());                //实收数量（WA）

                        //CreateNode(xmlDoc, itemNode, "vfree1", "000");                      //组件功率
                        CreateNode(xmlDoc, itemNode, "vchangerate", "1/1");                 //标称功率换算率
                    }
                    else
                    {
                        CreateNode(xmlDoc, itemNode, "nshouldassistnum", Convert.ToDecimal(Convert.ToDecimal(worDetail.EffiCode) * worDetail.Qty).ToString("f3"));  //应收数量（WA）
                        CreateNode(xmlDoc, itemNode, "nassistnum", Convert.ToDecimal(Convert.ToDecimal(worDetail.EffiCode) * worDetail.Qty).ToString("f3"));        //实收数量（WA）

                        //CreateNode(xmlDoc, itemNode, "vfree1", GetCodeByName(worDetail.EffiName));      //组件功率名称
                        CreateNode(xmlDoc, itemNode, "vchangerate", "1/" + worDetail.EffiCode);         //标称功率换算率
                    }

                    CreateNode(xmlDoc, itemNode, "vfree1", GetCodeByName(worDetail.EffiName,"JN0001"));          //组件功率名称
                    CreateNode(xmlDoc, itemNode, "vbdef1", worDetail.SumCoefPMax.ToString());           //实际总功率

                    CreateNode(xmlDoc, itemNode, "vfree2", GetCodeByName(worDetail.PsSubcode,"JN0002"));         //电流档
                    CreateNode(xmlDoc, itemNode, "vfree3", GetCodeByName(worDetail.Grade,"JN0003"));             //产品等级


                    CreateNode(xmlDoc, itemNode, "vproductbatch", worDetail.OrderNumber.ToString());                    //生产订单号

                    CreateNode(xmlDoc, itemNode, "csourcebillhid", woReport.ERPWRKey.ToString());                       //来源单据表头主键（报工单主键）
                    CreateNode(xmlDoc, itemNode, "csourcebillbid", wrERPDetailRow[0]["PK_WR_QUALITY"] == null ? "" : wrERPDetailRow[0]["PK_WR_QUALITY"].ToString());    //来源单据表体主键（报工单行主键）
                    CreateNode(xmlDoc, itemNode, "csourcetype", "55A4");                                                //来源单据类型
                    CreateNode(xmlDoc, itemNode, "csourcetranstype", dtERPWRDetail.Rows[0]["VTRANTYPEID"].ToString());  //来源交易类型
                    CreateNode(xmlDoc, itemNode, "vsourcebillcode", dtERPWRDetail.Rows[0]["VBILLCODE"].ToString());     //来源单据号
                    CreateNode(xmlDoc, itemNode, "vsourcerowno", wrERPDetailRow[0]["PK_WR_PRODUCT"] == null ? "" : wrERPDetailRow[0]["PK_WR_PRODUCT"].ToString());  //来源单据行号

                    //CreateNode(xmlDoc, itemNode, "cprojectid", "");           //项目
                    //CreateNode(xmlDoc, itemNode, "casscustid", "");           //客户

                    CreateNode(xmlDoc, itemNode, "cfirsttype", wrERPDetailRow[0]["CBSRCTYPE"].ToString());              //源头单据类型
                    CreateNode(xmlDoc, itemNode, "cfirsttranstype", wrERPDetailRow[0]["CBSRCTRANSTYPE"].ToString());    //源头交易类型
                    CreateNode(xmlDoc, itemNode, "cfirstbillhid", wrERPDetailRow[0]["VBSRCID"].ToString());             //源头单据表头主键
                    CreateNode(xmlDoc, itemNode, "vfirstbillcode", wrERPDetailRow[0]["VBSRCCODE"].ToString());          //源头单据号
                    //CreateNode(xmlDoc, itemNode, "vfirstrowno", "");//, dt_wr.Rows[0]["VBSRCROWNO"].ToString()//源头单据行号
                    CreateNode(xmlDoc, itemNode, "cfirstbillbid", wrERPDetailRow[0]["VBSRCID"].ToString());             //源头单据表体主键
                    //CreateNode(xmlDoc, itemNode, "vnotebody", "");                                                      //行备注
                    CreateNode(xmlDoc, itemNode, "bbarcodeclose", "N");                                                 //单据行是否条码关闭
                    CreateNode(xmlDoc, itemNode, "bonroadflag", "N");                                                   //是否在途
                    CreateNode(xmlDoc, itemNode, "flargess", "N");                                                      //赠品
                    
                    #endregion
                }
                #endregion

                #region 5.入库单表头属性节点
                CreateNode(xmlDoc, billheadNode, "dbilldate", woReport.BillDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));         //单据日期
                CreateNode(xmlDoc, billheadNode, "creationtime", now.ToString("yyyy-MM-dd HH:mm:ss"));                          //单据创建日期
                CreateNode(xmlDoc, billheadNode, "cwarehouseid", Store);                        //仓库
                CreateNode(xmlDoc, billheadNode, "cprowarehouseid", Store);                     //仓库？？

                CreateNode(xmlDoc, billheadNode, "cprocalbodyoid", ERPOrg);                     //ERP组织代码
                CreateNode(xmlDoc, billheadNode, "cprocalbodyvid", ERPOrg);                     //ERP组织代码
                CreateNode(xmlDoc, billheadNode, "cdptid", ERPDEPTCode);                        //生产部门最新
                CreateNode(xmlDoc, billheadNode, "cdptvid", ERPDEPTCode);                       //生产部门

                CreateNode(xmlDoc, billheadNode, "ctrantypeid", ERPStockInTypeKey);             //入库单PK值
                CreateNode(xmlDoc, billheadNode, "vtrantypecode", ERPStockInType);              //入库单类型

                CreateNode(xmlDoc, billheadNode, "vnote", woReport.Note);                       //备注

                CreateNode(xmlDoc, billheadNode, "fbillflag", "2");

                CreateNode(xmlDoc, billheadNode, "vdef17", woReport.Key.ToString());            //MES入库单号
                CreateNode(xmlDoc, billheadNode, "vdef20", "-1");                               //OA-ERP单据状态

                CreateNode(xmlDoc, billheadNode, "creator", creator);                           //创建人
                CreateNode(xmlDoc, billheadNode, "billmaker", creator);                         //制单人

                #endregion

                //XML路径
                string path = Server.MapPath("~/XMLFile/");
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                path = path + woReport.Key + ".xml";
                xmlDoc.Save(path);

                FileStream ms = new FileStream(path, FileMode.Open, FileAccess.Read);

                //string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];
                //HttpWebRequest loHttp = (HttpWebRequest)WebRequest.Create(url);                             //URL为XChangeServlet地址
                httpWebRequest.Method = "POST";

                // *** Set any header related and operational properties
                httpWebRequest.Timeout = 600000;                         //超时控制
                httpWebRequest.UserAgent = "Code Sample Web Client";

                // *** reuse cookies if available
                httpWebRequest.CookieContainer = new CookieContainer();

                if (this.oCookies != null && this.oCookies.Count > 0)
                {
                    httpWebRequest.CookieContainer.Add(this.oCookies);
                }

                #region send Xml To NCS
                //LogHelper.WriteLogError("Begin Send Xml File");
                //loHttp.ContentLength = ms.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();

                byte[] buffer = new Byte[(int)ms.Length];
                int bytesRead = 0;

                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) != 0)
                    requestStream.Write(buffer, 0, bytesRead);

                requestStream.Close();

                // *** Return the Response data
                HttpWebResponse loWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (loWebResponse.Cookies.Count > 0)
                    if (this.oCookies == null)
                    {
                        this.oCookies = loWebResponse.Cookies;
                    }
                    else
                    {
                        // ** If we already have cookies update the list
                        foreach (Cookie oRespCookie in loWebResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.oCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Name;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.oCookies.Add(oRespCookie);
                        }
                    }

                Encoding enc = Encoding.GetEncoding("gb2312");  // Windows-1252 or iso-
                if (loWebResponse.ContentEncoding.Length > 0)
                {
                    enc = Encoding.GetEncoding(loWebResponse.ContentEncoding);
                }

                StreamReader loResponseStream = new StreamReader(loWebResponse.GetResponseStream());

                string ResponseText = loResponseStream.ReadToEnd();

                //LogHelper.WriteLogError("End Send Xml File");
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(ResponseText);

                //获取ERP回执
                XmlNode xnode = Doc.SelectSingleNode("ufinterface/sendresult/content");
                result.ObjectNo = xnode.InnerText;

                //获取ERP错误信息提示
                if (result.ObjectNo == "")
                {
                    XmlNode errornode = Doc.SelectSingleNode("ufinterface/sendresult/resultdescription");
                    result.Message = errornode.InnerText;

                    loResponseStream.Close();
                    loWebResponse.Close();
                    ms.Close();
                    requestStream.Close();

                    result.Code = 1000;

                    return result;
                }

                loResponseStream.Close();
                loWebResponse.Close();
                ms.Close();
                requestStream.Close();
                #endregion

                return result;

            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message + e.Source;

                return result;
            }
        }

        /// <summary>
        /// 撤销入库接收
        /// </summary>
        /// <param name="billCode">入库单号</param>
        /// <returns></returns>
        public ActionResult RevokeStockIn(string billCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            DateTime dtStartTime = DateTime.Now;

            try
            {
                using (WOReportClient client = new WOReportClient())
                {
                    //取得入库单表头对象
                    MethodReturnResult<WOReport> rstInWOrder = client.GetWOReport(billCode);

                    if (rstInWOrder.Data.BillState != EnumBillState.Receive)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("入库单状态为[{0}],不能撤销", rstInWOrder.Data.BillState.GetDisplayName());

                        return Json(result);
                    }

                    //判断ERP入库单是否删除
                    DataTable dt_ErpInCode = GetERPCodeByBillCode(billCode);

                    if (dt_ErpInCode.Rows.Count > 0)
                    {
                        result.Code = 1002;
                        result.Message = string.Format("ERP入库单[{1}]未删除，请先删除ERP入单后在进行取消操作。",
                                                        billCode,
                                                        dt_ErpInCode.Rows[0]["vbillcode"].ToString());
                        return Json(result);
                    }

                    //创建参数对象
                    WOReportParameter pram = new WOReportParameter()
                    {
                        BillCode = billCode,
                        BillState = EnumBillState.Receive,              //单据状态入库接收完成状态
                        OperateComputer = Request.UserHostAddress,      //客户端
                        Editor = User.Identity.Name,                    //编辑人
                        Creator = User.Identity.Name,                   //编辑日期
                        OperationType = -1                              //操作状态 -1 - 撤销入库接收
                    };

                    result = client.StockIn(pram);

                    if (result.Code == 0)
                    {
                        DateTime dtEndTime = DateTime.Now;

                        result.Message = string.Format(StringResource.WOReport_RevokeStockInApply_Success, billCode) +
                                         "执行时间：" + (dtEndTime - dtStartTime).TotalSeconds.ToString("F1") + "秒，[ "
                                         + dtStartTime.ToString() + " - "
                                         + dtEndTime.ToString() + " ]";
                    }
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = string.Format(StringResource.WOReport_RevokeStockInApply_Error, billCode) + e.Message;
            }

            return Json(result);
        }

        /// <summary>
        /// 根据属性名称查找字符串中设置的属性值
        /// </summary>
        /// <param name="valueString">属性字符串</param>
        /// <param name="valueName">属性名称</param>
        /// <param name="operators">赋值操作符号</param>
        /// <param name="terminator">终止符</param>
        /// <returns></returns>
        public string GetValueDataByString(string valueString, string valueName, string operators, string terminator)
        {
            string valueData = "";
            int ifind = 0;
            int iEnd = 0;

            ifind = valueString.IndexOf(valueName + operators);

            if (ifind > 0)
            {
                ifind = ifind + valueName.Length + operators.Length;

                iEnd = valueString.IndexOf(terminator, ifind);

                if (iEnd >= ifind)
                {
                    valueData = valueString.Substring(ifind, iEnd - ifind);
                }
                else
                {
                    valueData = valueString.Substring(ifind, valueString.Length - ifind);
                }
            }

            return valueData;
        }

        //public ActionResult CreateXML(WOReportDetailQueryViewModel model)
        //{
        //    MethodReturnResult result = new MethodReturnResult();
        //    MethodReturnResult<WOReport> rst = null;
        //    MethodReturnResult<DataSet> rst1 = null;
        //    using (WOReportClient client = new WOReportClient())
        //    {
        //        rst = client.GetWOReport(model.BillCode);

        //        if (string.IsNullOrEmpty(rst.Data.WRCode))
        //        {
        //            result.Code = 1006;
        //            result.Message = string.Format(StringResource.WIReport_Error_Approve, model.BillCode);
        //            return Json(result);
        //        }
        //        if (!string.IsNullOrEmpty(rst.Data.INCode))
        //        {
        //            result.Code = 1005;
        //            result.Message = string.Format(StringResource.WOReport_CreateXML_Error_Again, model.BillCode);
        //            return Json(result);
        //        }
        //        rst1 = client.GetReportDetailByObjectNumber(model.BillCode,model.ScrapType.ToString());
        //    }

        //    try
        //    {
        //        MethodReturnResult re = new MethodReturnResult();
        //        DataSet dsData = new DataSet();
        //        MethodReturnResult re1 = new MethodReturnResult();
        //        DataSet dsData1 = new DataSet();
        //        using (ERPClient cl = new ERPClient())
        //        {
        //            MethodReturnResult<DataSet> dsResult = cl.GetERPWorkOrder(rst.Data.OrderNumber);
        //            if (dsResult.Data.Tables[0].Rows.Count <= 0)
        //            {
        //                re.Code = dsResult.Code;
        //                re.Message = dsResult.Message;
        //            }
        //            dsData = dsResult.Data;
        //        }

        //        if (dsData.Tables[0].Rows.Count > 0)
        //        {
        //            MethodReturnResult<DataSet> dsResult = null;
        //            using (ERPClient cl1 = new ERPClient())
        //            {
        //                dsResult = cl1.GetERPOrderType(dsData.Tables[0].Rows[0]["vtrantypecode"].ToString());
        //            }
        //            if (dsResult.Data.Tables[0].Rows.Count <= 0)
        //            {
        //                re1.Code = dsResult.Code;
        //                re1.Message = dsResult.Message;
        //            }
        //            else
        //            {
        //                dsData1 = dsResult.Data;
        //                if (dsData1.Tables[0].Rows.Count > 0)
        //                {
        //                    #region Call ERP Interface

        //                    MethodReturnResult<string> resultOfCreateXml = CreateXmlFile(
        //                            rst.Data, rst1.Data, model.Store,
        //                            dsData1.Tables[0].Rows[0]["ctrantypeid3"].ToString(),
        //                            dsData1.Tables[0].Rows[0]["transtype3"].ToString()
        //                        );

        //                    if (resultOfCreateXml.Code == 0)
        //                    {
        //                        WOReportParameter pram = new WOReportParameter()
        //                        {
        //                            BillCode = model.BillCode,
        //                            Editor = User.Identity.Name,
        //                            INCode = resultOfCreateXml.Data,
        //                            Store = model.Store
        //                        };

        //                        using (WOReportClient client = new WOReportClient())
        //                        {
        //                            result = client.WI(pram);
        //                        }

        //                        if (result.Code == 0)
        //                        {
        //                            result.Message = string.Format(StringResource.WOReport_CreateXML_Success, model.BillCode);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        result.Code = resultOfCreateXml.Code;
        //                        result.Message = resultOfCreateXml.Message;
        //                    }
        //                    #endregion
        //                }
        //            }
        //        }
        //        else
        //        {
        //            result.Message = "请检查ERP中工单号[" + rst.Data.OrderNumber + "]是否已经完工！";
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        result.Code = 1002;
        //        result.Message = string.Format(StringResource.WOReport_CreateXML_Error, model.BillCode) + e.Message;
        //    }

        //    return Json(result);
        //}

        public ActionResult CreateXML(WOReportDetailQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<WOReport> rst = null;
            MethodReturnResult<DataSet> rst1 = null;
            using (WOReportClient client = new WOReportClient())
            {
                rst = client.GetWOReport(model.BillCode);

                if (string.IsNullOrEmpty(rst.Data.WRCode))
                {
                    result.Code = 1006;
                    result.Message = string.Format(StringResource.WIReport_Error_Approve, model.BillCode);
                    return Json(result);
                }
                if (!string.IsNullOrEmpty(rst.Data.INCode))
                {
                    result.Code = 1005;
                    result.Message = string.Format(StringResource.WOReport_StockInApply_Error_Again, model.BillCode);
                    return Json(result);
                }
                rst1 = client.GetReportDetailByObjectNumber(model.BillCode, model.ScrapType.ToString());
            }

            try
            {
                MethodReturnResult re = new MethodReturnResult();
                DataSet dsData = new DataSet();
                MethodReturnResult re1 = new MethodReturnResult();
                DataSet dsData1 = new DataSet();
                using (ERPClient cl = new ERPClient())
                {
                    MethodReturnResult<DataSet> dsResult = cl.GetERPWorkOrder(rst.Data.OrderNumber);
                    if (dsResult.Data.Tables[0].Rows.Count <= 0)
                    {
                        re.Code = dsResult.Code;
                        re.Message = dsResult.Message;
                    }
                    dsData = dsResult.Data;
                }

                if (dsData.Tables[0].Rows.Count > 0)
                {
                    MethodReturnResult<DataSet> dsResult = null;
                    using (ERPClient cl1 = new ERPClient())
                    {
                        dsResult = cl1.GetERPOrderType(dsData.Tables[0].Rows[0]["vtrantypecode"].ToString());
                    }
                    if (dsResult.Data.Tables[0].Rows.Count <= 0)
                    {
                        re1.Code = dsResult.Code;
                        re1.Message = dsResult.Message;
                    }
                    else
                    {
                        dsData1 = dsResult.Data;
                        
                        #region //查询是否在ERP中已经存在回执单号，如存在，则取到回执单号，并且将状态置为相应状态 
                        DataTable dt_ErpInCode = GetERPCodeByBillCode(model.BillCode);
                        if (dsData1.Tables[0].Rows.Count > 0 && dt_ErpInCode.Rows.Count == 0)//当入库单还未进入ERP系统，即，没有得到回执单号时
                        {
                            #region Call ERP Interface

                            MethodReturnResult<string> resultOfCreateXml = CreateXmlFile(
                                    rst.Data, rst1.Data, model.Store,
                                    dsData1.Tables[0].Rows[0]["ctrantypeid3"].ToString(),
                                    dsData1.Tables[0].Rows[0]["transtype3"].ToString()
                                );

                            if (resultOfCreateXml.Code == 0)
                            {
                                WOReportParameter pram = new WOReportParameter()
                                {
                                    BillCode = model.BillCode,
                                    Editor = User.Identity.Name,
                                     //= resultOfCreateXml.Data,
                                    Store = model.Store
                                };

                                using (WOReportClient client = new WOReportClient())
                                {
                                    result = client.WI(pram);
                                }

                                if (result.Code == 0)
                                {
                                    result.Message = string.Format(StringResource.WOReport_StockInApply_Success, model.BillCode);
                                }
                            }
                            else
                            {
                                result.Code = resultOfCreateXml.Code;
                                result.Message = resultOfCreateXml.Message;
                            }
                            #endregion
                        }

                        else if (dsData1.Tables[0].Rows.Count > 0 && dt_ErpInCode != null)//当入库单已进入ERP系统，但是没有得到回执单号时
                        {
                            #region Call ERP Interface

                                WOReportParameter pram = new WOReportParameter()
                                {
                                    BillCode = model.BillCode,
                                    Editor = User.Identity.Name,
                                    //INCode = dt_ErpInCode.Rows[0]["CGENERALHID"].ToString(),
                                    Store = model.Store
                                };

                                using (WOReportClient client = new WOReportClient())
                                {
                                    result = client.WI(pram);
                                }

                                if (result.Code == 0)
                                {
                                    result.Message = string.Format(StringResource.WOReport_StockInApply_Success, model.BillCode);
                                }
                            #endregion
                        }
                        #endregion

                    }
                }
                else
                {
                    result.Message = "请检查ERP中工单号[" + rst.Data.OrderNumber + "]是否已经完工！";
                }

            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = string.Format(StringResource.WOReport_StockInApply_Error, model.BillCode) + e.Message;
            }

            return Json(result);
        }

        public DataTable GetERPCodeByBillCode(string BillCode)//根据入库单号查询入库单回执
        {
            DataTable dt = new DataTable();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPCodeByBillCode(BillCode);
                                
                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        public ActionResult AntiState(string BillCode, string INCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPINCodeById(INCode);

                if (ds.Data.Tables[0].Rows.Count > 0)
                {
                    result.Code = 1004;
                    result.Message = string.Format("撤销");
                }
                else
                {
                    MethodReturnResult resultwo = new MethodReturnResult();
                    using (WOReportClient clientwo = new WOReportClient())
                    {
                        WOReportParameter pram = new WOReportParameter()
                        {
                            BillCode = BillCode,
                            Editor = User.Identity.Name,
                            //WRCode = null,
                            Store = null
                        };

                        result = clientwo.WI(pram);
                        if (result.Code == 0)
                        {
                            result.Message = string.Format(StringResource.WOReport_StockInApply_Success, BillCode);
                        }
                    }
                }
            }
            return Json(result);
        }

        public MethodReturnResult<string> CreateXmlFile(WOReport woReport, DataSet lstDetail, string Store, string ctrantypeid3, string transtype3)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>
            {
                Code = 0
            };

            DataTable dt = GetWorkOrder(woReport.OrderNumber);

            DataTable dt_wr = GetERPWR(woReport.WRCode);
            if (dt_wr.Rows.Count <= 0)
            {
                result.Code = 1001;
                result.Message = string.Format("报工单{0}在ERP中不存在", woReport.WRCode);
                return result;
            }

            DataTable dt_WRDetail = GetERPWRDetail(woReport.WRCode);

            string startTime = Convert.ToDateTime(woReport.BillDate).ToShortDateString() + " 00:00:00";
            string endTime = Convert.ToDateTime(woReport.BillDate).ToShortDateString() + " 23:59:59";

            XmlDocument xmlDoc = new XmlDocument();

            string ERPAccount = "";                     //ERP账套代码
            string ERPGroupCode = "";                   //ERP集团代码
            string ERPOrg = "";                         //ERP组织代码

            #region 根据ERP接口字符串取得ERP账套相关信息
            //取得ERP连接字符串
            string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];

            //创建WEB访问对象
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            //取得查询语句
            string ERPQuery = httpWebRequest.RequestUri.Query;

            //取得ERP账套代码
            ERPAccount = GetValueDataByString(ERPQuery, "account", "=", "&");
            ERPGroupCode = GetValueDataByString(ERPQuery, "groupcode", "=", "&");
            ERPOrg = GetValueDataByString(ERPQuery, "orgcode", "=", "&");

            #endregion

            if (woReport.ScrapType.ToString() == "True")
            {
                 #region 报废
                 //创建类型声明节点  
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "GB2312", "");
                xmlDoc.AppendChild(node);
                //创建根节点  
                XmlElement root = xmlDoc.CreateElement("ufinterface");
                root.SetAttribute("sender", "mes");
                root.SetAttribute("roottag", "");
                root.SetAttribute("replace", "Y");
                root.SetAttribute("receiver", ERPOrg);
                root.SetAttribute("isexchange", "Y");
                root.SetAttribute("groupcode", ERPGroupCode);
                root.SetAttribute("filename", "");
                root.SetAttribute("billtype", "4X");
                root.SetAttribute("account", ERPAccount);
                xmlDoc.AppendChild(root);

                XmlElement Node = xmlDoc.CreateElement("bill");//创建节点ufinterface子节点bill   
                Node.SetAttribute("id", "");
                xmlDoc.DocumentElement.AppendChild(Node);

                XmlNode billheadNode = xmlDoc.CreateNode(XmlNodeType.Element, "billhead", null);
                Node.AppendChild(billheadNode);

                CreateNode(xmlDoc, billheadNode, "cgeneralhid", "");

                XmlElement productNode = xmlDoc.CreateElement("cgeneralbid");
                billheadNode.AppendChild(productNode);

                if (lstDetail.Tables[0].Rows.Count > 0)
                {
                    #region
                    //int i = 1;
                    foreach (var item in lstDetail.Tables[0].AsEnumerable())
                    {
                        //string Effi = GetEffi(item.Key.ObjectNumber);

                        DataTable dt_Unit = GetUnit(item["MATERIAL_CODE"].ToString());

                        //dt_WRDetail


                        //根据批次号、工单获取ERP报工明细记录
                        //DataRow[] wrDetailRows = dt_WRDetail.Select(string.Format(" vbbatchcode = '{0}' and pk_wr = '{1}' and VBMOBILLCODE='{2}'"
                        //        , item["PACKAGE_NO"].ToString(), woReport.WRCode, item["ORDER_NUMBER"].ToString())
                        //        );

                        DataRow[] wrDetailRows = dt_WRDetail.Select(
                                    string.Format(" vbbatchcode = '{0}' and pk_wr = '{1}' and VBMOBILLCODE='{2}'"//and vbfree1='{3}' and vbfree2='{4}' and vbfree3='{5}' "
                                                                                                , item["PACKAGE_NO"].ToString()
                                                                                                , woReport.WRCode
                                                                                                , item["ORDER_NUMBER"].ToString()
                                                                                                , item["PM_NAME"].ToString()
                                                                                                , item["PS_SUBCODE_Name"].ToString()
                                                                                                , item["GRADE"].ToString())
                                                                                                );
                        if (wrDetailRows == null || wrDetailRows.Length == 0)
                        {
                            result.Code = 1001;
                            result.Message = string.Format("报工单{0}中工单{1}明细批号{2}在ERP中不存在", woReport.WRCode, item["ORDER_NUMBER"].ToString(), item["PACKAGE_NO"].ToString());
                            return result;
                        }

                        XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        productNode.AppendChild(itemNode);

                        CreateNode(xmlDoc, itemNode, "cgeneralbid", "");
                        CreateNode(xmlDoc, itemNode, "cgenerallid", ""); //行号,最大长度为20,类型为:String
                        CreateNode(xmlDoc, itemNode, "crowno", "");   //行号,最大长度为20,类型为:String
                        CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);
                        CreateNode(xmlDoc, itemNode, "corpoid", ERPOrg);//公司最新版本
                        CreateNode(xmlDoc, itemNode, "corpvid", ERPOrg);//公司
                        CreateNode(xmlDoc, itemNode, "cmaterialoid", woReport.MaterialCode);//物料OID
                        CreateNode(xmlDoc, itemNode, "cmaterialvid", woReport.MaterialCode);//物料版本号
                        CreateNode(xmlDoc, itemNode, "cunitid", dt_Unit.Rows[0]["MEASCODE"].ToString());//主单位
                        CreateNode(xmlDoc, itemNode, "castunitid", dt_Unit.Rows[0]["ASTMEASCODE"].ToString());//单位
                        CreateNode(xmlDoc, itemNode, "vchangerate", "1/1");//+ item["SPM_VALUE"].ToString());
                        //<!--库存状态,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "cstateid", "");
                        //<!--生产厂商,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "cproductorid", "");
                        CreateNode(xmlDoc, itemNode, "vbatchcode", item["PACKAGE_NO"].ToString());
                        //<!--批次主键,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "pk_batchcode", "");
                        //<!--质量等级,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "cqualitylevelid", "");
                        //<!--生产日期,最大长度为19,类型为:UFDate-->
                        CreateNode(xmlDoc, itemNode, "dproducedate", woReport.BillDate.ToString());
                        //<!--失效日期,最大长度为19,类型为:UFDate-->
                        CreateNode(xmlDoc, itemNode, "dvalidate", woReport.BillDate.ToString());
                        //<!--供应商批次,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "vvendbatchcode", "");
                        CreateNode(xmlDoc, itemNode, "nshouldnum", item["Qty"].ToString());//应收主数量
                        CreateNode(xmlDoc, itemNode, "nshouldassistnum", "4.478");//(Convert.ToDecimal(item["Qty"].ToString()) * Convert.ToDecimal(item["SPM_VALUE"].ToString())).ToString("f3"));//应收数量
                        CreateNode(xmlDoc, itemNode, "nnum", item["Qty"].ToString());//组装数量
                        CreateNode(xmlDoc, itemNode, "nassistnum", (Convert.ToDecimal(item["Qty"].ToString()) * Convert.ToDecimal(item["SPM_VALUE"].ToString())).ToString("f3"));//数量
                        //<!--单价,最大长度为28,类型为:UFDouble-->
                        CreateNode(xmlDoc, itemNode, "ncostprice", "");
                        //<!--金额,最大长度为28,类型为:UFDouble-->
                        CreateNode(xmlDoc, itemNode, "ncostmny", "");
                        CreateNode(xmlDoc, itemNode, "dbizdate", DateTime.Now.ToString());//业务日期
                        CreateNode(xmlDoc, itemNode, "vproductbatch", "");//wrDetailRows[0]["VBFIRSTMOCODE"].ToString());//生产订单号
                        CreateNode(xmlDoc, itemNode, "csourcebillhid", dt_wr.Rows[0]["PK_WR"].ToString());//来源单据表头主键
                        CreateNode(xmlDoc, itemNode, "csourcebillbid", wrDetailRows[0]["PK_WR_QUALITY"] == null ? "" : wrDetailRows[0]["PK_WR_QUALITY"].ToString());
                        //<!--来源单据类型,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "csourcetype", "55A4");
                        CreateNode(xmlDoc, itemNode, "csourcetranstype", "55A4-01");
                        CreateNode(xmlDoc, itemNode, "vsourcebillcode", dt_wr.Rows[0]["VBILLCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "vsourcerowno","10");// wrDetailRows[0]["PK_WR_PRODUCT"] == null ? "" : wrDetailRows[0]["PK_WR_PRODUCT"].ToString());
                        CreateNode(xmlDoc, itemNode, "cfirstbillhid", wrDetailRows[0]["VBSRCID"].ToString());
                        CreateNode(xmlDoc, itemNode, "cfirstbillbid", wrDetailRows[0]["VBSRCID"].ToString());
                        //<!--源头单据类型,最大长度为20,类型为:String-->
                        //<cfirsttype>55C2</cfirsttype>
                        CreateNode(xmlDoc, itemNode, "cfirsttype", wrDetailRows[0]["CBSRCTYPE"].ToString());
                        //<!--源头单据交易类型,最大长度为20,类型为:String-->
                        //<cfirsttranstype>55C2-Cxx-01</cfirsttranstype>
                        CreateNode(xmlDoc, itemNode, "cfirsttranstype", "55C2-Cxx-01");//wrDetailRows[0]["CBSRCTRANSTYPE"].ToString());
                        //<!--源头单据号,最大长度为40,类型为:String-->
                        //<vfirstbillcode>1MO-15100001</vfirstbillcode>
                        CreateNode(xmlDoc, itemNode, "vfirstbillcode", wrDetailRows[0]["VBSRCCODE"].ToString());
                        //<!--源头单据行号,最大长度为20,类型为:String-->
                        //<cfirstrowno></cfirstrowno>
                        CreateNode(xmlDoc, itemNode, "cfirstrowno", "");
                        CreateNode(xmlDoc, itemNode, "vfree1", "");
                        CreateNode(xmlDoc, itemNode, "vfree2", "");
                        CreateNode(xmlDoc, itemNode, "vfree3", "");
                        CreateNode(xmlDoc, itemNode, "vfree4", "");
                        CreateNode(xmlDoc, itemNode, "vfree5", "");
                        CreateNode(xmlDoc, itemNode, "vfree6", "");
                        CreateNode(xmlDoc, itemNode, "vbdef1", "");
                        CreateNode(xmlDoc, itemNode, "ccheckstateid", "");
                        CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);
                        //<!--行备注,最大长度为181,类型为:String-->
                        //<vnotebody></vnotebody>
                        CreateNode(xmlDoc, itemNode, "vnotebody", "");
                        //<!--批次档案备注,最大长度为50,类型为:String-->
                        //<vbatchcodenote></vbatchcodenote>
                        CreateNode(xmlDoc, itemNode, "vbatchcodenote", "");
                        //<!--检验时间,最大长度为19,类型为:UFDateTime-->
                        //<tchecktime>2015-10-28 12:05:46</tchecktime>
                        CreateNode(xmlDoc, itemNode, "tchecktime", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        //<!--批次形成时间,最大长度为19,类型为:UFDateTime-->
                        //<tbcts>2015-10-28 12:05:46</tbcts>
                        CreateNode(xmlDoc, itemNode, "tbcts", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        //<!--首次入库日期,最大长度为19,类型为:UFDate-->
                        //<dinbounddate>2015-10-28 12:05:46</dinbounddate>
                        CreateNode(xmlDoc, itemNode, "dinbounddate", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        CreateNode(xmlDoc, itemNode, "cproductid", item["MATERIAL_CODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "fproductclass", "1");
                        CreateNode(xmlDoc, itemNode, "vprodfree1", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree2", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree3", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree4", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree5", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree6", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree7", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree8", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree9", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree10", "");


                    }
                    #endregion
                }
                CreateNode(xmlDoc, billheadNode, "pk_group", ERPGroupCode);
                CreateNode(xmlDoc, billheadNode, "corpoid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "corpvid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "vbillcode", "");
                CreateNode(xmlDoc, billheadNode, "dbilldate", woReport.BillDate.ToString());
                CreateNode(xmlDoc, billheadNode, "cbiztype", "");
                CreateNode(xmlDoc, billheadNode, "cwarehouseid", Store);
                CreateNode(xmlDoc, billheadNode, "ctrantypeid", ctrantypeid3);//入库单PK值
                CreateNode(xmlDoc, billheadNode, "vtrantypecode", transtype3);//入库单交易类型
                //CreateNode(xmlDoc, billheadNode, "cprocalbodyoid", ERPOrg);
                //CreateNode(xmlDoc, billheadNode, "cprocalbodyvid", ERPOrg);
                //CreateNode(xmlDoc, billheadNode, "cprowarehouseid", Store);
                CreateNode(xmlDoc, billheadNode, "pk_org", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "pk_org_v", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "cwhsmanagerid", "");
                CreateNode(xmlDoc, billheadNode, "cdptid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cdptvid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cbizid", "");
                CreateNode(xmlDoc, billheadNode, "fbillflag", "2");
                CreateNode(xmlDoc, billheadNode, "vnote", woReport.Note);
                CreateNode(xmlDoc, billheadNode, "vdef1", "");
                CreateNode(xmlDoc, billheadNode, "vdef2", "");
                CreateNode(xmlDoc, billheadNode, "vdef3", "");
                CreateNode(xmlDoc, billheadNode, "creator", woReport.Creator);
                CreateNode(xmlDoc, billheadNode, "billmaker", woReport.BillMaker);
                CreateNode(xmlDoc, billheadNode, "creationtime", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                //<!--签字人,最大长度为20,类型为:String-->
                //<approver></approver>
                 CreateNode(xmlDoc, billheadNode, "approver", "");
                //<!--签字日期,最大长度为19,类型为:UFDate-->
                //<taudittime></taudittime>
                 CreateNode(xmlDoc, billheadNode, "taudittime", "");
                //<!--最后修改人,最大长度为20,类型为:String-->
                //<modifier></modifier>
                 CreateNode(xmlDoc, billheadNode, "modifier", "");
                //<!--最后修改时间,最大长度为19,类型为:UFDateTime-->
                //<modifiedtime></modifiedtime>
                 CreateNode(xmlDoc, billheadNode, "modifiedtime", "");
                //<!--制单日期,最大长度为19,类型为:UFDate-->
                //<dmakedate>2015-10-28 12:05:46</dmakedate>
                 CreateNode(xmlDoc, billheadNode, "dmakedate", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                 #endregion
            }
            else
            {
                #region 产成品入库
                //创建类型声明节点  
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                xmlDoc.AppendChild(node);
                //创建根节点  
                XmlElement root = xmlDoc.CreateElement("ufinterface");
                root.SetAttribute("receiver", ERPOrg);
                root.SetAttribute("sender", "mes");
                root.SetAttribute("roottag", "");
                root.SetAttribute("replace", "Y");
                root.SetAttribute("isexchange", "Y");
                root.SetAttribute("groupcode", "");
                root.SetAttribute("filename", "");
                root.SetAttribute("billtype", "46");
                root.SetAttribute("account", ERPGroupCode);
                xmlDoc.AppendChild(root);

                XmlElement Node = xmlDoc.CreateElement("bill");//创建节点ufinterface子节点bill   
                Node.SetAttribute("id", "");
                xmlDoc.DocumentElement.AppendChild(Node);

                XmlNode billheadNode = xmlDoc.CreateNode(XmlNodeType.Element, "billhead", null);
                Node.AppendChild(billheadNode);

                CreateNode(xmlDoc, billheadNode, "cgeneralhid", "");
                CreateNode(xmlDoc, billheadNode, "pk_group", ERPGroupCode);
                CreateNode(xmlDoc, billheadNode, "corpoid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "corpvid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "vbillcode", "");
                CreateNode(xmlDoc, billheadNode, "dbilldate", woReport.BillDate.ToString());
                CreateNode(xmlDoc, billheadNode, "pk_org", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "pk_org_v", ERPOrg);

                XmlElement productNode = xmlDoc.CreateElement("cgeneralbid");
                billheadNode.AppendChild(productNode);

                if (lstDetail.Tables[0].Rows.Count > 0)
                {
                    #region
                    int i = 1;
                    foreach (var item in lstDetail.Tables[0].AsEnumerable())
                    {
                        //string Effi = GetEffi(item.Key.ObjectNumber);

                        DataTable dt_Unit = GetUnit(item["MATERIAL_CODE"].ToString());

                        //dt_WRDetail


                        //根据批次号、工单获取ERP报工明细记录
                        //DataRow[] wrDetailRows = dt_WRDetail.Select(string.Format(" vbbatchcode = '{0}' and pk_wr = '{1}' and VBMOBILLCODE='{2}'"
                        //        , item["PACKAGE_NO"].ToString(), woReport.WRCode, item["ORDER_NUMBER"].ToString())
                        //        );

                        DataRow[] wrDetailRows = dt_WRDetail.Select(
                                    string.Format(" vbbatchcode = '{0}' and pk_wr = '{1}' and VBMOBILLCODE='{2}' and vbfree1='{3}' and vbfree2='{4}' and vbfree3='{5}' "
                                                                                                , item["PACKAGE_NO"].ToString()
                                                                                                , woReport.WRCode
                                                                                                , item["ORDER_NUMBER"].ToString()
                                                                                                , item["PM_NAME"].ToString()
                                                                                                , item["PS_SUBCODE_Name"].ToString()
                                                                                                , item["GRADE"].ToString())
                                                                                                );
                        if (wrDetailRows == null || wrDetailRows.Length == 0)
                        {
                            result.Code = 1001;
                            result.Message = string.Format("报工单{0}中工单{1}明细批号{2}在ERP中不存在", woReport.WRCode, item["ORDER_NUMBER"].ToString(), item["PACKAGE_NO"].ToString());
                            return result;
                        }

                        XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        productNode.AppendChild(itemNode);

                        CreateNode(xmlDoc, itemNode, "cgeneralbid", "");
                        CreateNode(xmlDoc, itemNode, "crowno", (i++).ToString());
                        CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);
                        CreateNode(xmlDoc, itemNode, "corpoid", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "corpvid", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "cstateid", "2");
                        CreateNode(xmlDoc, itemNode, "cmaterialoid", woReport.MaterialCode);
                        CreateNode(xmlDoc, itemNode, "cmaterialvid", woReport.MaterialCode);
                        //D级组件功率
                        if (item["GRADE"].ToString() == "D")
                        {
                            CreateNode(xmlDoc, itemNode, "vfree1", "000");//功率PM_NAME
                        }
                        else
                        {
                            CreateNode(xmlDoc, itemNode, "vfree1", GetCodeByName(item["PM_NAME"].ToString(),"JN0001"));//功率PM_NAME
                        }

                        //CreateNode(xmlDoc, itemNode, "vfree1", GetCodeByName(item["PM_NAME"].ToString()));
                        CreateNode(xmlDoc, itemNode, "vfree2", GetCodeByName(item["PS_SUBCODE_Name"].ToString(),"JN0002"));
                        CreateNode(xmlDoc, itemNode, "vfree3", GetCodeByName(item["GRADE"].ToString(),"JN0003"));
                        CreateNode(xmlDoc, itemNode, "vfree4", "");
                        CreateNode(xmlDoc, itemNode, "vfree5", "");
                        CreateNode(xmlDoc, itemNode, "vfree6", "");
                        CreateNode(xmlDoc, itemNode, "cunitid", dt_Unit.Rows[0]["MEASCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "castunitid", dt_Unit.Rows[0]["ASTMEASCODE"].ToString());
                        //D级组件换算率
                        if (item["GRADE"].ToString() == "D")
                        {
                            CreateNode(xmlDoc, itemNode, "vchangerate", "1/1");
                        }
                        else
                        {
                            CreateNode(xmlDoc, itemNode, "vchangerate", "1/" + item["SPM_VALUE"].ToString());
                        }

                        //CreateNode(xmlDoc, itemNode, "vchangerate", "1/" + item["SPM_VALUE"].ToString());
                        CreateNode(xmlDoc, itemNode, "vbatchcode", item["PACKAGE_NO"].ToString());
                        CreateNode(xmlDoc, itemNode, "nshouldnum", item["Qty"].ToString());
                        CreateNode(xmlDoc, itemNode, "nshouldassistnum", (Convert.ToDecimal(item["Qty"].ToString()) * Convert.ToDecimal(item["SPM_VALUE"].ToString())).ToString("f3"));
                        CreateNode(xmlDoc, itemNode, "nnum", item["Qty"].ToString());
                        CreateNode(xmlDoc, itemNode, "nassistnum", (Convert.ToDecimal(item["Qty"].ToString()) * Convert.ToDecimal(item["SPM_VALUE"].ToString())).ToString("f3"));
                        CreateNode(xmlDoc, itemNode, "dbizdate", DateTime.Now.ToString());//入库日期
                        //CreateNode(xmlDoc, itemNode, "dbizdate", "2016-11-28 07:50:00");//入库日期
                        CreateNode(xmlDoc, itemNode, "vproductbatch", wrDetailRows[0]["VBFIRSTMOCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "csourcebillhid", dt_wr.Rows[0]["PK_WR"].ToString());
                        CreateNode(xmlDoc, itemNode, "csourcebillbid", wrDetailRows[0]["PK_WR_QUALITY"] == null ? "" : wrDetailRows[0]["PK_WR_QUALITY"].ToString());
                        CreateNode(xmlDoc, itemNode, "csourcetype", "55A4");
                        CreateNode(xmlDoc, itemNode, "csourcetranstype", dt_wr.Rows[0]["VTRANTYPEID"].ToString());
                        CreateNode(xmlDoc, itemNode, "csourcebillbid", wrDetailRows[0]["PK_WR_QUALITY"] == null ? "" : wrDetailRows[0]["PK_WR_QUALITY"].ToString());
                        CreateNode(xmlDoc, itemNode, "vsourcebillcode", dt_wr.Rows[0]["VBILLCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "vsourcerowno", wrDetailRows[0]["PK_WR_PRODUCT"] == null ? "" : wrDetailRows[0]["PK_WR_PRODUCT"].ToString());
                        CreateNode(xmlDoc, itemNode, "cprojectid", "");
                        CreateNode(xmlDoc, itemNode, "casscustid", "");
                        CreateNode(xmlDoc, itemNode, "cfirsttype", wrDetailRows[0]["CBSRCTYPE"].ToString());
                        CreateNode(xmlDoc, itemNode, "cfirsttranstype", wrDetailRows[0]["CBSRCTRANSTYPE"].ToString());
                        CreateNode(xmlDoc, itemNode, "cfirstbillhid", wrDetailRows[0]["VBSRCID"].ToString());
                        CreateNode(xmlDoc, itemNode, "vfirstbillcode", wrDetailRows[0]["VBSRCCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "vfirstrowno", "");//, dt_wr.Rows[0]["VBSRCROWNO"].ToString()
                        CreateNode(xmlDoc, itemNode, "cfirstbillbid", wrDetailRows[0]["VBSRCID"].ToString());
                        CreateNode(xmlDoc, itemNode, "vnotebody", "");
                        CreateNode(xmlDoc, itemNode, "bbarcodeclose", "N");
                        CreateNode(xmlDoc, itemNode, "bonroadflag", "N");
                        CreateNode(xmlDoc, itemNode, "dproducedate", woReport.BillDate.ToString());
                        CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "cbodywarehouseid", Store);
                        CreateNode(xmlDoc, itemNode, "flargess", "N");
                        CreateNode(xmlDoc, itemNode, "pk_batchcode", "");
                        CreateNode(xmlDoc, itemNode, "csrcmaterialoid", "");//, item.MaterialCode
                        CreateNode(xmlDoc, itemNode, "csrcmaterialvid", "");//, item.MaterialCode
                        CreateNode(xmlDoc, itemNode, "cproductid", item["MATERIAL_CODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "fproductclass", "1");
                        if (item["GRADE"].ToString() == "D")
                        {
                            CreateNode(xmlDoc, itemNode, "vbdef1", "0.000");
                        }
                        else
                        {
                            CreateNode(xmlDoc, itemNode, "vbdef1", item["sumCOEF_PMAX"].ToString());
                        }
                        

                    }
                    #endregion
                }

                CreateNode(xmlDoc, billheadNode, "cwarehouseid", Store);
                CreateNode(xmlDoc, billheadNode, "ctrantypeid", ctrantypeid3);//入库单PK值
                CreateNode(xmlDoc, billheadNode, "vtrantypecode", transtype3);//入库单交易类型
                CreateNode(xmlDoc, billheadNode, "cprocalbodyoid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "cprocalbodyvid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "cprowarehouseid", Store);
                CreateNode(xmlDoc, billheadNode, "cwhsmanagerid", "");
                CreateNode(xmlDoc, billheadNode, "cdptid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cdptvid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cbizid", "");
                CreateNode(xmlDoc, billheadNode, "vnote", woReport.Note);
                CreateNode(xmlDoc, billheadNode, "fbillflag", "2");
                CreateNode(xmlDoc, billheadNode, "creator", woReport.Creator);
                CreateNode(xmlDoc, billheadNode, "billmaker", woReport.BillMaker);
                CreateNode(xmlDoc, billheadNode, "creationtime", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));//单据日期
                 #endregion
             }

            string path = Server.MapPath("~/XMLFile/");
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            path = path + woReport.Key + "IN.xml";
            xmlDoc.Save(path);

            try
            {
                #region Call Servelet ，Get Result
                FileStream ms = new FileStream(path, FileMode.Open, FileAccess.Read);
                //string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];
                HttpWebRequest loHttp = (HttpWebRequest)WebRequest.Create(url);  //URL为XChangeServlet地址

                loHttp.Method = "POST";
                // *** Set any header related and operational properties
                loHttp.Timeout = 30000;  // 30 secs
                loHttp.UserAgent = "Code Sample Web Client";
                    
                // *** reuse cookies if available
                loHttp.CookieContainer = new CookieContainer();

                if (this.oCookies != null && this.oCookies.Count > 0)
                {
                    loHttp.CookieContainer.Add(this.oCookies);
                }

                //loHttp.ContentLength = ms.Length;
                Stream requestStream = loHttp.GetRequestStream();

                byte[] buffer = new Byte[(int)ms.Length];
                int bytesRead = 0;
                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) != 0)
                    requestStream.Write(buffer, 0, bytesRead);
                //requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);  
                requestStream.Close();

                // *** Return the Response data
                HttpWebResponse loWebResponse = (HttpWebResponse)loHttp.GetResponse();

                if (loWebResponse.Cookies.Count > 0)
                    if (this.oCookies == null)
                    {
                        this.oCookies = loWebResponse.Cookies;
                    }
                    else
                    {
                        // ** If we already have cookies update the list
                        foreach (Cookie oRespCookie in loWebResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.oCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Name;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.oCookies.Add(oRespCookie);
                        }
                    }

                Encoding enc = Encoding.GetEncoding("gb2312");  // Windows-1252 or iso-
                if (loWebResponse.ContentEncoding.Length > 0)
                {
                    enc = Encoding.GetEncoding(loWebResponse.ContentEncoding);
                }

                StreamReader loResponseStream =
                    new StreamReader(loWebResponse.GetResponseStream());

                string ResponseText = loResponseStream.ReadToEnd();

                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(ResponseText);

                //获取ERP回执
                XmlNode xnode = Doc.SelectSingleNode("ufinterface/sendresult/content");
                string returnCode = xnode.InnerText;
                result.Data = xnode.InnerText;

                //获取ERP错误信息提示
                if (returnCode == "")
                {
                    XmlNode errornode = Doc.SelectSingleNode("ufinterface/sendresult/resultdescription");
                    result.Code = 2001;
                    result.Message = errornode.InnerText;
                }

                loResponseStream.Close();
                loWebResponse.Close();
                ms.Close();
                requestStream.Close();
                #endregion
            }
            catch (Exception err)
            {
                result.Code = 3001;
                result.Message = string.Format("调用用友ERP接口报错：{0}", err.Message);
            }
            return result;
        }

        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            parentNode.AppendChild(node);
        }

        public DataTable GetWorkOrder(string OrderNumber)
        {
            DataTable dt = new DataTable();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWorkOrder(OrderNumber);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        public DataTable GetUnit(string MaterialCode)
        {
            DataTable dt = new DataTable();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetUnitByMaterialCode(MaterialCode);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        public string GetCodeByName(string Name,string ListCode)
        {
            string Code = "";
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetCodeByName(Name, ListCode);
                if (ds.Code == 0 && ds.Data.Tables[0].Rows.Count > 0)
                {
                    Code = ds.Data.Tables[0].Rows[0]["CODE"].ToString();
                }
            }
            return Code;
        }

        public string GetEffi(string ObjectNumber)
        {
            string Effi = "0";
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetPackageInfo(ObjectNumber);
                string lowerEffi = ds.Data.Tables[0].Rows[0]["SPM_VALUE"].ToString();
                if (lowerEffi != null)
                {
                    Effi = (Convert.ToDouble(lowerEffi) * 0.24336).ToString("f3");
                }
            }
            return Effi;
        }

        public DataTable GetERPWR(string WRCode)
        {
            DataTable dt = new DataTable();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWR(WRCode);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        public DataTable GetERPWRDetail(string ObjectNumber, string WRCode, string OrderNumber)
        {
            DataTable dt = new DataTable();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWRDetail(ObjectNumber, WRCode, OrderNumber);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        public DataTable GetERPWRDetail(string WRCode)
        {
            DataTable dt = new DataTable();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWRDetailInfo(WRCode);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        private CookieCollection _oCookies = null;

        protected CookieCollection oCookies
        {
            get
            {
                return _oCookies;
            }
            set
            {
                _oCookies = value;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
        {
            if (ModelState.IsValid)
            {
                int pageNo = currentPageNo ?? 0;
                int pageSize = currentPageSize ?? 20;
                if (Request["PageNo"] != null)
                {
                    pageNo = Convert.ToInt32(Request["PageNo"]);
                }
                if (Request["PageSize"] != null)
                {
                    pageSize = Convert.ToInt32(Request["PageSize"]);
                }

                using (WOReportClient client = new WOReportClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = pageNo,
                            PageSize = pageSize,
                            Where = where ?? string.Empty,
                            OrderBy = orderBy ?? string.Empty
                        };
                        MethodReturnResult<IList<WOReport>> result = client.GetWOReport(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial");
        }
//-------------------------------------------------------------------------------------------------------------------
        public ActionResult sIndex(string BillCode)
        {
            return View();
        }

        public ActionResult sQuery(WOReportQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WOReportClient client = new WOReportClient())
                {
                    StringBuilder where = new StringBuilder();
                    where.AppendFormat(" ScrapType ='1' AND BillState = 2 ");
                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.BillCode))
                        {
                            where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.BillCode);
                        }
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "CreateTime desc",
                        Where = where.ToString()
                    };

                    MethodReturnResult<IList<WOReport>> result = client.GetWOReport(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                }
            }
            return PartialView("_sListPartial");
        }

        public ActionResult sDetail(string BillCode)
        {
            WOReportDetailQueryViewModel model = new WOReportDetailQueryViewModel();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<WOReport> rst = client.GetWOReport(BillCode);

                if (rst.Code > 0 || rst.Data == null)
                {
                    return RedirectToAction("sIndex", "ERPWIReport");
                }
                else
                {
                    model.BillCode = rst.Data.Key;
                    model.BillDate = rst.Data.BillDate;
                    model.BillMakedDate = rst.Data.BillMakedDate;
                    model.MixType = rst.Data.MixType;
                    model.ScrapType = ServiceCenter.MES.Model.ERP.EnumScrapType.True;
                    model.MaterialCode = rst.Data.MaterialCode;
                    model.OrderNumber = rst.Data.OrderNumber;
                    model.Note = rst.Data.Note;
                    model.INCode = rst.Data.INCode;
                }
                List<SelectListItem> StoreList = new List<SelectListItem>();
                MethodReturnResult<DataSet> ds = client.sGetStore();
                if (ds.Data.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Data.Tables[0].Rows.Count; i++)
                    {
                        StoreList.Add(new SelectListItem() { Text = ds.Data.Tables[0].Rows[i]["STORNAME"].ToString(), Value = ds.Data.Tables[0].Rows[i]["STORCODE"].ToString() });
                    }
                }
                //StoreList.Add(new SelectListItem() { Text = "废料仓", Value = "FP001" });
                ViewBag.Store = StoreList;

                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ItemNo",
                    Where = string.Format(" Key.BillCode = '{0}'"
                                                , BillCode)
                };
                MethodReturnResult<IList<WOReportDetail>> result = client.GetWOReportDetail(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;

                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_sDetailListPartial", new WOReportDetailViewModel() { BillCode = BillCode });
            }
            else
            {
                return View(model);
            }

        }

        public ActionResult sCreateXML(WOReportDetailQueryViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<WOReport> rst = null;
            MethodReturnResult<DataSet> rst1 = null;
            using (WOReportClient client = new WOReportClient())
            {
                rst = client.GetWOReport(model.BillCode);

                if (string.IsNullOrEmpty(rst.Data.WRCode))
                {
                    result.Code = 1006;
                    result.Message = string.Format(StringResource.WIReport_Error_Approve, model.BillCode);
                    return Json(result);
                }
                if (!string.IsNullOrEmpty(rst.Data.INCode))
                {
                    result.Code = 1005;
                    result.Message = string.Format(StringResource.WOReport_StockInApply_Error_Again, model.BillCode);
                    return Json(result);
                }
                rst1 = client.GetReportDetailByObjectNumber(model.BillCode, model.ScrapType.ToString());
            }

            try
            {
                MethodReturnResult re = new MethodReturnResult();
                DataSet dsData = new DataSet();
                MethodReturnResult re1 = new MethodReturnResult();
                DataSet dsData1 = new DataSet();
                using (ERPClient cl = new ERPClient())
                {
                    MethodReturnResult<DataSet> dsResult = cl.GetERPWorkOrder(rst.Data.OrderNumber);
                    if (dsResult.Data.Tables[0].Rows.Count <= 0)
                    {
                        re.Code = dsResult.Code;
                        re.Message = dsResult.Message;
                    }
                    dsData = dsResult.Data;
                }

                if (dsData.Tables[0].Rows.Count > 0)
                {
                    MethodReturnResult<DataSet> dsResult = null;
                    using (ERPClient cl1 = new ERPClient())
                    {
                        dsResult = cl1.GetERPOrderType(dsData.Tables[0].Rows[0]["vtrantypecode"].ToString());
                    }
                    if (dsResult.Data.Tables[0].Rows.Count <= 0)
                    {
                        re1.Code = dsResult.Code;
                        re1.Message = dsResult.Message;
                    }
                    else
                    {
                        dsData1 = dsResult.Data;
                        if (dsData1.Tables[0].Rows.Count > 0)
                        {
                            #region Call ERP Interface

                            MethodReturnResult<string> resultOfCreateXml = sCreateXmlFile(
                                    rst.Data, rst1.Data, model.Store,
                                    dsData1.Tables[0].Rows[0]["ctrantypeid3"].ToString(),
                                    dsData1.Tables[0].Rows[0]["transtype3"].ToString()
                                );

                            if (resultOfCreateXml.Code == 0)
                            {
                                WOReportParameter pram = new WOReportParameter()
                                {
                                    BillCode = model.BillCode,
                                    Editor = User.Identity.Name,
                                    //INCode = resultOfCreateXml.Data,
                                    Store = model.Store
                                };

                                using (WOReportClient client = new WOReportClient())
                                {
                                    result = client.WI(pram);
                                }

                                if (result.Code == 0)
                                {
                                    result.Message = string.Format(StringResource.WOReport_StockInApply_Success, model.BillCode);
                                }
                            }
                            else
                            {
                                result.Code = resultOfCreateXml.Code;
                                result.Message = resultOfCreateXml.Message;
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    result.Message = "请检查ERP中工单号[" + rst.Data.OrderNumber + "]是否已经完工！";
                }

            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = string.Format(StringResource.WOReport_StockInApply_Error, model.BillCode) + e.Message;
            }

            return Json(result);
        }

        public ActionResult sAntiState(string BillCode, string INCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPINCodeById(INCode);

                if (ds.Data.Tables[0].Rows.Count > 0)
                {
                    result.Code = 1004;
                    result.Message = string.Format("撤销");
                }
                else
                {
                    MethodReturnResult resultwo = new MethodReturnResult();
                    using (WOReportClient clientwo = new WOReportClient())
                    {
                        WOReportParameter pram = new WOReportParameter()
                        {
                            BillCode = BillCode,
                            Editor = User.Identity.Name,
                            ERPWorkReportKey = null,
                            Store = null
                        };

                        result = clientwo.WI(pram);
                        if (result.Code == 0)
                        {
                            result.Message = string.Format(StringResource.WOReport_StockInApply_Success, BillCode);
                        }
                    }
                }
            }
            return Json(result);
        }
        public MethodReturnResult<string> sCreateXmlFile(WOReport woReport, DataSet lstDetail, string Store, string ctrantypeid3, string transtype3)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>
            {
                Code = 0
            };

            DataTable dt = GetWorkOrder(woReport.OrderNumber);

            DataTable dt_wr = GetERPWR(woReport.WRCode);
            if (dt_wr.Rows.Count <= 0)
            {
                result.Code = 1001;
                result.Message = string.Format("报工单{0}在ERP中不存在", woReport.WRCode);
                return result;
            }

            DataTable dt_WRDetail = GetERPWRDetail(woReport.WRCode);

            string startTime = Convert.ToDateTime(woReport.BillDate).ToShortDateString() + " 00:00:00";
            string endTime = Convert.ToDateTime(woReport.BillDate).ToShortDateString() + " 23:59:59";

            string ERPAccount = "";                     //ERP账套代码
            string ERPGroupCode = "";                   //ERP集团代码
            string ERPOrg = "";                         //ERP组织代码

            #region 根据ERP接口字符串取得ERP账套相关信息
            //取得ERP连接字符串
            string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];

            //创建WEB访问对象
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            //取得查询语句
            string ERPQuery = httpWebRequest.RequestUri.Query;

            //取得ERP账套代码
            ERPAccount = GetValueDataByString(ERPQuery, "account", "=", "&");
            ERPGroupCode = GetValueDataByString(ERPQuery, "groupcode", "=", "&");
            ERPOrg = GetValueDataByString(ERPQuery, "orgcode", "=", "&");

            #endregion

            XmlDocument xmlDoc = new XmlDocument();
            if (woReport.ScrapType.ToString() == "True")
            {
                //创建类型声明节点  
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "GB2312", "");
                xmlDoc.AppendChild(node);
                //创建根节点  
                XmlElement root = xmlDoc.CreateElement("ufinterface");
                root.SetAttribute("sender", "mes");
                root.SetAttribute("roottag", "");
                root.SetAttribute("replace", "Y");
                root.SetAttribute("receiver", ERPOrg);
                root.SetAttribute("isexchange", "Y");
                root.SetAttribute("groupcode", ERPGroupCode);
                root.SetAttribute("filename", "");
                root.SetAttribute("billtype", "4X");
                root.SetAttribute("account", ERPAccount);
                xmlDoc.AppendChild(root);

                XmlElement Node = xmlDoc.CreateElement("bill");//创建节点ufinterface子节点bill   
                Node.SetAttribute("id", "");
                xmlDoc.DocumentElement.AppendChild(Node);

                XmlNode billheadNode = xmlDoc.CreateNode(XmlNodeType.Element, "billhead", null);
                Node.AppendChild(billheadNode);

                CreateNode(xmlDoc, billheadNode, "cgeneralhid", "");

                XmlElement productNode = xmlDoc.CreateElement("cgeneralbid");
                billheadNode.AppendChild(productNode);

                if (lstDetail.Tables[0].Rows.Count > 0)
                {
                    #region
                    //int i = 1;
                    foreach (var item in lstDetail.Tables[0].AsEnumerable())
                    {
                        //string Effi = GetEffi(item.Key.ObjectNumber);

                        DataTable dt_Unit = GetUnit(item["MATERIAL_CODE"].ToString());

                        //dt_WRDetail


                        //根据批次号、工单获取ERP报工明细记录
                        //DataRow[] wrDetailRows = dt_WRDetail.Select(string.Format(" vbbatchcode = '{0}' and pk_wr = '{1}' and VBMOBILLCODE='{2}'"
                        //        , item["PACKAGE_NO"].ToString(), woReport.WRCode, item["ORDER_NUMBER"].ToString())
                        //        );

                        DataRow[] wrDetailRows = dt_WRDetail.Select(
                                    string.Format(" vbbatchcode = '{0}' and pk_wr = '{1}' and VBMOBILLCODE='{2}'"//and vbfree1='{3}' and vbfree2='{4}' and vbfree3='{5}' "
                                                                                                , item["PACKAGE_NO"].ToString()
                                                                                                , woReport.WRCode
                                                                                                , item["ORDER_NUMBER"].ToString()
                                                                                                , item["PM_NAME"].ToString()
                                                                                                , item["PS_SUBCODE_Name"].ToString()
                                                                                                , item["GRADE"].ToString())
                                                                                                );
                        if (wrDetailRows == null || wrDetailRows.Length == 0)
                        {
                            result.Code = 1001;
                            result.Message = string.Format("报工单{0}中工单{1}明细批号{2}在ERP中不存在", woReport.WRCode, item["ORDER_NUMBER"].ToString(), item["PACKAGE_NO"].ToString());
                            return result;
                        }

                        XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        productNode.AppendChild(itemNode);

                        CreateNode(xmlDoc, itemNode, "cgeneralbid", "");
                        CreateNode(xmlDoc, itemNode, "cgenerallid", ""); //行号,最大长度为20,类型为:String
                        CreateNode(xmlDoc, itemNode, "crowno", "");   //行号,最大长度为20,类型为:String
                        CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);
                        CreateNode(xmlDoc, itemNode, "corpoid", ERPOrg);//公司最新版本
                        CreateNode(xmlDoc, itemNode, "corpvid", ERPOrg);//公司
                        CreateNode(xmlDoc, itemNode, "cmaterialoid", woReport.MaterialCode);//物料OID
                        CreateNode(xmlDoc, itemNode, "cmaterialvid", woReport.MaterialCode);//物料版本号
                        CreateNode(xmlDoc, itemNode, "cunitid", dt_Unit.Rows[0]["MEASCODE"].ToString());//主单位
                        CreateNode(xmlDoc, itemNode, "castunitid", dt_Unit.Rows[0]["ASTMEASCODE"].ToString());//单位
                        CreateNode(xmlDoc, itemNode, "vchangerate", "1/1");//+ item["SPM_VALUE"].ToString());
                        //<!--库存状态,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "cstateid", "");
                        //<!--生产厂商,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "cproductorid", "");
                        CreateNode(xmlDoc, itemNode, "vbatchcode", item["PACKAGE_NO"].ToString());
                        //<!--批次主键,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "pk_batchcode", "");
                        //<!--质量等级,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "cqualitylevelid", "");
                        //<!--生产日期,最大长度为19,类型为:UFDate-->
                        CreateNode(xmlDoc, itemNode, "dproducedate", woReport.BillDate.ToString());
                        //<!--失效日期,最大长度为19,类型为:UFDate-->
                        CreateNode(xmlDoc, itemNode, "dvalidate", woReport.BillDate.ToString());
                        //<!--供应商批次,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "vvendbatchcode", "");
                        CreateNode(xmlDoc, itemNode, "nshouldnum", item["Qty"].ToString());//应收主数量
                        CreateNode(xmlDoc, itemNode, "nshouldassistnum", "1");//(Convert.ToDecimal(item["Qty"].ToString()) * Convert.ToDecimal(item["SPM_VALUE"].ToString())).ToString("f3"));//应收数量
                        CreateNode(xmlDoc, itemNode, "nnum", item["Qty"].ToString());//组装数量
                        CreateNode(xmlDoc, itemNode, "nassistnum", "1");//(Convert.ToDecimal(item["Qty"].ToString()) * Convert.ToDecimal(item["SPM_VALUE"].ToString())).ToString("f3"));//数量
                        //<!--单价,最大长度为28,类型为:UFDouble-->
                        CreateNode(xmlDoc, itemNode, "ncostprice", "");
                        //<!--金额,最大长度为28,类型为:UFDouble-->
                        CreateNode(xmlDoc, itemNode, "ncostmny", "");
                        CreateNode(xmlDoc, itemNode, "dbizdate", DateTime.Now.ToString());//业务日期
                        CreateNode(xmlDoc, itemNode, "vproductbatch", "");//wrDetailRows[0]["VBFIRSTMOCODE"].ToString());//生产订单号
                        CreateNode(xmlDoc, itemNode, "csourcebillhid", dt_wr.Rows[0]["PK_WR"].ToString());//来源单据表头主键
                        CreateNode(xmlDoc, itemNode, "csourcebillbid", wrDetailRows[0]["PK_WR_QUALITY"] == null ? "" : wrDetailRows[0]["PK_WR_QUALITY"].ToString());
                        //<!--来源单据类型,最大长度为20,类型为:String-->
                        CreateNode(xmlDoc, itemNode, "csourcetype", "55A4");
                        CreateNode(xmlDoc, itemNode, "csourcetranstype", "55A4-01");
                        CreateNode(xmlDoc, itemNode, "vsourcebillcode", dt_wr.Rows[0]["VBILLCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "vsourcerowno", wrDetailRows[0]["PK_WR_PRODUCT"] == null ? "" : wrDetailRows[0]["PK_WR_PRODUCT"].ToString());
                        CreateNode(xmlDoc, itemNode, "cfirstbillhid", wrDetailRows[0]["VBSRCID"].ToString());
                        CreateNode(xmlDoc, itemNode, "cfirstbillbid", wrDetailRows[0]["VBSRCID"].ToString());
                        //<!--源头单据类型,最大长度为20,类型为:String-->
                        //<cfirsttype>55C2</cfirsttype>
                        CreateNode(xmlDoc, itemNode, "cfirsttype", wrDetailRows[0]["CBSRCTYPE"].ToString());
                        //<!--源头单据交易类型,最大长度为20,类型为:String-->
                        //<cfirsttranstype>55C2-Cxx-01</cfirsttranstype>
                        CreateNode(xmlDoc, itemNode, "cfirsttranstype", wrDetailRows[0]["CBSRCTRANSTYPE"].ToString());
                        //<!--源头单据号,最大长度为40,类型为:String-->
                        //<vfirstbillcode>1MO-15100001</vfirstbillcode>
                        CreateNode(xmlDoc, itemNode, "vfirstbillcode", wrDetailRows[0]["VBSRCCODE"].ToString());
                        //<!--源头单据行号,最大长度为20,类型为:String-->
                        //<cfirstrowno></cfirstrowno>
                        CreateNode(xmlDoc, itemNode, "cfirstrowno", "");
                        CreateNode(xmlDoc, itemNode, "vfree1", "260");
                        CreateNode(xmlDoc, itemNode, "vfree2", "03");
                        CreateNode(xmlDoc, itemNode, "vfree3", "01");
                        CreateNode(xmlDoc, itemNode, "vfree4", "");
                        CreateNode(xmlDoc, itemNode, "vfree5", "");
                        CreateNode(xmlDoc, itemNode, "vfree6", "");
                        CreateNode(xmlDoc, itemNode, "vbdef1", "");
                        CreateNode(xmlDoc, itemNode, "ccheckstateid", "");
                        CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);
                        //<!--行备注,最大长度为181,类型为:String-->
                        //<vnotebody></vnotebody>
                        CreateNode(xmlDoc, itemNode, "vnotebody", "");
                        //<!--批次档案备注,最大长度为50,类型为:String-->
                        //<vbatchcodenote></vbatchcodenote>
                        CreateNode(xmlDoc, itemNode, "vbatchcodenote", "");
                        //<!--检验时间,最大长度为19,类型为:UFDateTime-->
                        //<tchecktime>2015-10-28 12:05:46</tchecktime>
                        CreateNode(xmlDoc, itemNode, "tchecktime", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        //<!--批次形成时间,最大长度为19,类型为:UFDateTime-->
                        //<tbcts>2015-10-28 12:05:46</tbcts>
                        CreateNode(xmlDoc, itemNode, "tbcts", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        //<!--首次入库日期,最大长度为19,类型为:UFDate-->
                        //<dinbounddate>2015-10-28 12:05:46</dinbounddate>
                        CreateNode(xmlDoc, itemNode, "dinbounddate", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        CreateNode(xmlDoc, itemNode, "cproductid", item["MATERIAL_CODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "fproductclass", "1");
                        CreateNode(xmlDoc, itemNode, "vprodfree1", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree2", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree3", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree4", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree5", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree6", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree7", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree8", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree9", "");
                        CreateNode(xmlDoc, itemNode, "vprodfree10", "");


                    }
                    #endregion
                }
                CreateNode(xmlDoc, billheadNode, "pk_group", ERPAccount);
                CreateNode(xmlDoc, billheadNode, "corpoid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "corpvid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "vbillcode", "");
                CreateNode(xmlDoc, billheadNode, "dbilldate", woReport.BillDate.ToString());
                CreateNode(xmlDoc, billheadNode, "cbiztype", "");
                CreateNode(xmlDoc, billheadNode, "cwarehouseid", Store);
                CreateNode(xmlDoc, billheadNode, "ctrantypeid", "0001A110000000001MQL");// "ctrantypeid",ctrantypeid3);// "0001A110000000001MQL");入库单PK值(合同类型)
                CreateNode(xmlDoc, billheadNode, "vtrantypecode", "4X-01");//transtype3);//"4X-01");入库单交易类型（合同类型编码）
                //CreateNode(xmlDoc, billheadNode, "cprocalbodyoid", ERPOrg);
                //CreateNode(xmlDoc, billheadNode, "cprocalbodyvid", ERPOrg);
                //CreateNode(xmlDoc, billheadNode, "cprowarehouseid", Store);
                CreateNode(xmlDoc, billheadNode, "pk_org", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "pk_org_v", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "cwhsmanagerid", "");
                CreateNode(xmlDoc, billheadNode, "cdptid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cdptvid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cbizid", "");
                CreateNode(xmlDoc, billheadNode, "fbillflag", "2");
                CreateNode(xmlDoc, billheadNode, "vnote", woReport.Note);
                CreateNode(xmlDoc, billheadNode, "vdef1", "");
                CreateNode(xmlDoc, billheadNode, "vdef2", "");
                CreateNode(xmlDoc, billheadNode, "vdef3", "");
                CreateNode(xmlDoc, billheadNode, "creator", woReport.Creator);
                CreateNode(xmlDoc, billheadNode, "billmaker", woReport.BillMaker);
                CreateNode(xmlDoc, billheadNode, "creationtime", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                //<!--签字人,最大长度为20,类型为:String-->
                //<approver></approver>
                CreateNode(xmlDoc, billheadNode, "approver", "");
                //<!--签字日期,最大长度为19,类型为:UFDate-->
                //<taudittime></taudittime>
                CreateNode(xmlDoc, billheadNode, "taudittime", "");
                //<!--最后修改人,最大长度为20,类型为:String-->
                //<modifier></modifier>
                CreateNode(xmlDoc, billheadNode, "modifier", "");
                //<!--最后修改时间,最大长度为19,类型为:UFDateTime-->
                //<modifiedtime></modifiedtime>
                CreateNode(xmlDoc, billheadNode, "modifiedtime", "");
                //<!--制单日期,最大长度为19,类型为:UFDate-->
                //<dmakedate>2015-10-28 12:05:46</dmakedate>
                CreateNode(xmlDoc, billheadNode, "dmakedate", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {

                //创建类型声明节点  
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                xmlDoc.AppendChild(node);
                //创建根节点  
                XmlElement root = xmlDoc.CreateElement("ufinterface");
                root.SetAttribute("receiver", ERPOrg);
                root.SetAttribute("sender", "mes");
                root.SetAttribute("roottag", "");
                root.SetAttribute("replace", "Y");
                root.SetAttribute("isexchange", "Y");
                root.SetAttribute("groupcode", "");
                root.SetAttribute("filename", "");
                root.SetAttribute("billtype", "46");
                root.SetAttribute("account", ERPAccount);
                xmlDoc.AppendChild(root);

                XmlElement Node = xmlDoc.CreateElement("bill");//创建节点ufinterface子节点bill   
                Node.SetAttribute("id", "");
                xmlDoc.DocumentElement.AppendChild(Node);

                XmlNode billheadNode = xmlDoc.CreateNode(XmlNodeType.Element, "billhead", null);
                Node.AppendChild(billheadNode);

                CreateNode(xmlDoc, billheadNode, "cgeneralhid", "");
                CreateNode(xmlDoc, billheadNode, "pk_group", ERPAccount);
                CreateNode(xmlDoc, billheadNode, "corpoid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "corpvid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "vbillcode", "");
                CreateNode(xmlDoc, billheadNode, "dbilldate", woReport.BillDate.ToString());
                CreateNode(xmlDoc, billheadNode, "pk_org", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "pk_org_v", ERPOrg);

                XmlElement productNode = xmlDoc.CreateElement("cgeneralbid");
                billheadNode.AppendChild(productNode);

                if (lstDetail.Tables[0].Rows.Count > 0)
                {
                    #region
                    int i = 1;
                    foreach (var item in lstDetail.Tables[0].AsEnumerable())
                    {
                        //string Effi = GetEffi(item.Key.ObjectNumber);

                        DataTable dt_Unit = GetUnit(item["MATERIAL_CODE"].ToString());

                        //dt_WRDetail


                        //根据批次号、工单获取ERP报工明细记录
                        //DataRow[] wrDetailRows = dt_WRDetail.Select(string.Format(" vbbatchcode = '{0}' and pk_wr = '{1}' and VBMOBILLCODE='{2}'"
                        //        , item["PACKAGE_NO"].ToString(), woReport.WRCode, item["ORDER_NUMBER"].ToString())
                        //        );

                        DataRow[] wrDetailRows = dt_WRDetail.Select(
                                    string.Format(" vbbatchcode = '{0}' and pk_wr = '{1}' and VBMOBILLCODE='{2}' and vbfree1='{3}' and vbfree2='{4}' and vbfree3='{5}' "
                                                                                                , item["PACKAGE_NO"].ToString()
                                                                                                , woReport.WRCode
                                                                                                , item["ORDER_NUMBER"].ToString()
                                                                                                , item["PM_NAME"].ToString()
                                                                                                , item["PS_SUBCODE_Name"].ToString()
                                                                                                , item["GRADE"].ToString())
                                                                                                );
                        if (wrDetailRows == null || wrDetailRows.Length == 0)
                        {
                            result.Code = 1001;
                            result.Message = string.Format("报工单{0}中工单{1}明细批号{2}在ERP中不存在", woReport.WRCode, item["ORDER_NUMBER"].ToString(), item["PACKAGE_NO"].ToString());
                            return result;
                        }

                        XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        productNode.AppendChild(itemNode);

                        CreateNode(xmlDoc, itemNode, "cgeneralbid", "");
                        CreateNode(xmlDoc, itemNode, "crowno", (i++).ToString());
                        CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);
                        CreateNode(xmlDoc, itemNode, "corpoid", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "corpvid", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "cstateid", "2");
                        CreateNode(xmlDoc, itemNode, "cmaterialoid", woReport.MaterialCode);
                        CreateNode(xmlDoc, itemNode, "cmaterialvid", woReport.MaterialCode);
                        CreateNode(xmlDoc, itemNode, "vfree1", GetCodeByName(item["PM_NAME"].ToString(),"JN0001"));
                        CreateNode(xmlDoc, itemNode, "vfree2", GetCodeByName(item["PS_SUBCODE_Name"].ToString(),"JN0002"));
                        CreateNode(xmlDoc, itemNode, "vfree3", GetCodeByName(item["GRADE"].ToString(),"JN0003"));
                        CreateNode(xmlDoc, itemNode, "vfree4", "");
                        CreateNode(xmlDoc, itemNode, "vfree5", "");
                        CreateNode(xmlDoc, itemNode, "vfree6", "");
                        CreateNode(xmlDoc, itemNode, "cunitid", dt_Unit.Rows[0]["MEASCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "castunitid", dt_Unit.Rows[0]["ASTMEASCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "vchangerate", "1/" + item["SPM_VALUE"].ToString());
                        CreateNode(xmlDoc, itemNode, "vbatchcode", item["PACKAGE_NO"].ToString());
                        CreateNode(xmlDoc, itemNode, "nshouldnum", item["Qty"].ToString());
                        CreateNode(xmlDoc, itemNode, "nshouldassistnum", (Convert.ToDecimal(item["Qty"].ToString()) * Convert.ToDecimal(item["SPM_VALUE"].ToString())).ToString("f3"));
                        CreateNode(xmlDoc, itemNode, "nnum", item["Qty"].ToString());
                        CreateNode(xmlDoc, itemNode, "nassistnum", (Convert.ToDecimal(item["Qty"].ToString()) * Convert.ToDecimal(item["SPM_VALUE"].ToString())).ToString("f3"));
                        CreateNode(xmlDoc, itemNode, "dbizdate", DateTime.Now.ToString());
                        CreateNode(xmlDoc, itemNode, "vproductbatch", wrDetailRows[0]["VBFIRSTMOCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "csourcebillhid", dt_wr.Rows[0]["PK_WR"].ToString());
                        CreateNode(xmlDoc, itemNode, "csourcebillbid", wrDetailRows[0]["PK_WR_QUALITY"] == null ? "" : wrDetailRows[0]["PK_WR_QUALITY"].ToString());
                        CreateNode(xmlDoc, itemNode, "csourcetype", "55A4");
                        CreateNode(xmlDoc, itemNode, "csourcetranstype", dt_wr.Rows[0]["VTRANTYPEID"].ToString());
                        CreateNode(xmlDoc, itemNode, "csourcebillbid", wrDetailRows[0]["PK_WR_QUALITY"] == null ? "" : wrDetailRows[0]["PK_WR_QUALITY"].ToString());
                        CreateNode(xmlDoc, itemNode, "vsourcebillcode", dt_wr.Rows[0]["VBILLCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "vsourcerowno", wrDetailRows[0]["PK_WR_PRODUCT"] == null ? "" : wrDetailRows[0]["PK_WR_PRODUCT"].ToString());
                        CreateNode(xmlDoc, itemNode, "cprojectid", "");
                        CreateNode(xmlDoc, itemNode, "casscustid", "");
                        CreateNode(xmlDoc, itemNode, "cfirsttype", wrDetailRows[0]["CBSRCTYPE"].ToString());
                        CreateNode(xmlDoc, itemNode, "cfirsttranstype", wrDetailRows[0]["CBSRCTRANSTYPE"].ToString());
                        CreateNode(xmlDoc, itemNode, "cfirstbillhid", wrDetailRows[0]["VBSRCID"].ToString());
                        CreateNode(xmlDoc, itemNode, "vfirstbillcode", wrDetailRows[0]["VBSRCCODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "vfirstrowno", "");//, dt_wr.Rows[0]["VBSRCROWNO"].ToString()
                        CreateNode(xmlDoc, itemNode, "cfirstbillbid", wrDetailRows[0]["VBSRCID"].ToString());
                        CreateNode(xmlDoc, itemNode, "vnotebody", "");
                        CreateNode(xmlDoc, itemNode, "bbarcodeclose", "N");
                        CreateNode(xmlDoc, itemNode, "bonroadflag", "N");
                        CreateNode(xmlDoc, itemNode, "dproducedate", woReport.BillDate.ToString());
                        CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);
                        CreateNode(xmlDoc, itemNode, "cbodywarehouseid", Store);
                        CreateNode(xmlDoc, itemNode, "flargess", "N");
                        CreateNode(xmlDoc, itemNode, "pk_batchcode", "");
                        CreateNode(xmlDoc, itemNode, "csrcmaterialoid", "");//, item.MaterialCode
                        CreateNode(xmlDoc, itemNode, "csrcmaterialvid", "");//, item.MaterialCode
                        CreateNode(xmlDoc, itemNode, "cproductid", item["MATERIAL_CODE"].ToString());
                        CreateNode(xmlDoc, itemNode, "fproductclass", "1");
                        CreateNode(xmlDoc, itemNode, "vbdef1", item["sumCOEF_PMAX"].ToString());

                    }
                    #endregion
                }

                CreateNode(xmlDoc, billheadNode, "cwarehouseid", Store);
                CreateNode(xmlDoc, billheadNode, "ctrantypeid", ctrantypeid3);//入库单PK值
                CreateNode(xmlDoc, billheadNode, "vtrantypecode", transtype3);//入库单交易类型
                CreateNode(xmlDoc, billheadNode, "cprocalbodyoid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "cprocalbodyvid", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "cprowarehouseid", Store);
                CreateNode(xmlDoc, billheadNode, "cwhsmanagerid", "");
                CreateNode(xmlDoc, billheadNode, "cdptid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cdptvid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cbizid", "");
                CreateNode(xmlDoc, billheadNode, "vnote", woReport.Note);
                CreateNode(xmlDoc, billheadNode, "fbillflag", "2");
                CreateNode(xmlDoc, billheadNode, "creator", woReport.Creator);
                CreateNode(xmlDoc, billheadNode, "billmaker", woReport.BillMaker);
                CreateNode(xmlDoc, billheadNode, "creationtime", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            string path = Server.MapPath("~/XMLFile/");
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            path = path + woReport.Key + "IN.xml";
            xmlDoc.Save(path);

            try
            {
                #region Call Servelet ，Get Result
                FileStream ms = new FileStream(path, FileMode.Open, FileAccess.Read);

                //string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];
                HttpWebRequest loHttp = (HttpWebRequest)WebRequest.Create(url);  //URL为XChangeServlet地址
                loHttp.Method = "POST";
                // *** Set any header related and operational properties
                loHttp.Timeout = 30000;  // 30 secs
                loHttp.UserAgent = "Code Sample Web Client";

                // *** reuse cookies if available
                loHttp.CookieContainer = new CookieContainer();

                if (this.oCookies != null && this.oCookies.Count > 0)
                {
                    loHttp.CookieContainer.Add(this.oCookies);
                }

                //loHttp.ContentLength = ms.Length;
                Stream requestStream = loHttp.GetRequestStream();

                byte[] buffer = new Byte[(int)ms.Length];
                int bytesRead = 0;
                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) != 0)
                    requestStream.Write(buffer, 0, bytesRead);
                //requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);  
                requestStream.Close();

                // *** Return the Response data
                HttpWebResponse loWebResponse = (HttpWebResponse)loHttp.GetResponse();

                if (loWebResponse.Cookies.Count > 0)
                    if (this.oCookies == null)
                    {
                        this.oCookies = loWebResponse.Cookies;
                    }
                    else
                    {
                        // ** If we already have cookies update the list
                        foreach (Cookie oRespCookie in loWebResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.oCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Name;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.oCookies.Add(oRespCookie);
                        }
                    }

                Encoding enc = Encoding.GetEncoding("gb2312");  // Windows-1252 or iso-
                if (loWebResponse.ContentEncoding.Length > 0)
                {
                    enc = Encoding.GetEncoding(loWebResponse.ContentEncoding);
                }

                StreamReader loResponseStream =
                    new StreamReader(loWebResponse.GetResponseStream());

                string ResponseText = loResponseStream.ReadToEnd();

                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(ResponseText);

                //获取ERP回执
                XmlNode xnode = Doc.SelectSingleNode("ufinterface/sendresult/content");
                string returnCode = xnode.InnerText;
                result.Data = xnode.InnerText;

                //获取ERP错误信息提示
                if (returnCode == "")
                {
                    XmlNode errornode = Doc.SelectSingleNode("ufinterface/sendresult/resultdescription");
                    result.Code = 2001;
                    result.Message = errornode.InnerText;
                }

                loResponseStream.Close();
                loWebResponse.Close();
                ms.Close();
                requestStream.Close();
                #endregion
            }
            catch (Exception err)
            {
                result.Code = 3001;
                result.Message = string.Format("调用用友ERP接口报错：{0}", err.Message);
            }
            return result;
        }

        public void sCreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        public void sCreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            parentNode.AppendChild(node);
        }

        public DataTable sGetWorkOrder(string OrderNumber)
        {
            DataTable dt = new DataTable();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWorkOrder(OrderNumber);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        public DataTable sGetUnit(string MaterialCode)
        {
            DataTable dt = new DataTable();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetUnitByMaterialCode(MaterialCode);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        public string sGetCodeByName(string Name,string ListCode)
        {
            string Code = "";
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetCodeByName(Name, ListCode);
                if (ds.Code == 0 && ds.Data.Tables[0].Rows.Count > 0)
                {
                    Code = ds.Data.Tables[0].Rows[0]["CODE"].ToString();
                }
            }
            return Code;
        }

        public string sGetEffi(string ObjectNumber)
        {
            string Effi = "0";
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetPackageInfo(ObjectNumber);
                string lowerEffi = ds.Data.Tables[0].Rows[0]["SPM_VALUE"].ToString();
                if (lowerEffi != null)
                {
                    Effi = (Convert.ToDouble(lowerEffi) * 0.24336).ToString("f3");
                }
            }
            return Effi;
        }

        public DataTable sGetERPWR(string WRCode)
        {
            DataTable dt = new DataTable();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWR(WRCode);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }
        public DataTable sGetERPWRDetail(string ObjectNumber, string WRCode, string OrderNumber)
        {
            DataTable dt = new DataTable();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWRDetail(ObjectNumber, WRCode, OrderNumber);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        public DataTable sGetERPWRDetail(string WRCode)
        {
            DataTable dt = new DataTable();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPWRDetailInfo(WRCode);

                dt = ds.Data.Tables[0];
            }
            return dt;
        }

        //private CookieCollection _soCookies = null;
        protected CookieCollection soCookies
        {
            get
            {
                return _oCookies;
            }
            set
            {
                _oCookies = value;
            }

        }





        //----------------------------------------------------------------------------------------
        public ActionResult wIndex(string BillCode)
        {
            return View(new WOReportQueryViewModel()
            {
                BillState = EnumBillState.Apply.GetHashCode().ToString()
            });
        }

        public ActionResult wQuery(WOReportQueryViewModel model)
        {            
            using (WOReportClient client = new WOReportClient())
            {
                StringBuilder where = new StringBuilder();

                //仅查询在制品入库单并且入库状态为申报完成及入库完成单据
                where.AppendFormat(" BillType = 2 AND ( BillState = 1 or BillState = 2 ) ");

                if (!string.IsNullOrEmpty(model.BillCode))
                {
                    where.AppendFormat(" {0} Key LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BillCode);
                }

                //状态
                if (model.BillState != null && model.BillState != "")
                {
                    where.AppendFormat(" {0} BillState = {1}"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.BillState);
                }
                
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "EditTime desc",
                    Where = where.ToString()
                };

                MethodReturnResult<IList<WOReport>> result = client.GetWOReport(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }

            return PartialView("_wListPartial");

        }

        public ActionResult wDetail(string BillCode)
        {
            WOReportDetailQueryViewModel model = new WOReportDetailQueryViewModel();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<WOReport> rst = client.GetWOReport(BillCode);

                if (rst.Code > 0 || rst.Data == null)
                {
                    return RedirectToAction("wIndex", "ERPWIReport");
                }
                else
                {
                    model.BillCode = rst.Data.Key;
                    model.BillDate = rst.Data.BillDate;
                    model.BillMakedDate = rst.Data.BillMakedDate;
                    model.MixType = rst.Data.MixType;
                    model.ScrapType = ServiceCenter.MES.Model.ERP.EnumScrapType.False;
                    model.MaterialCode = rst.Data.MaterialCode;
                    model.OrderNumber = rst.Data.OrderNumber;
                    model.Note = rst.Data.Note;
                    model.INCode = rst.Data.INCode;
                    model.BillState = rst.Data.BillState;           //单据状态
                    model.BillType = rst.Data.BillType;             //入库类型
                }

                List<SelectListItem> StoreList = new List<SelectListItem>();

                MethodReturnResult<DataSet> ds = client.wGetStore();
                if (ds.Data.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Data.Tables[0].Rows.Count; i++)
                    {
                        StoreList.Add(new SelectListItem() { Text = ds.Data.Tables[0].Rows[i]["STORNAME"].ToString(), Value = ds.Data.Tables[0].Rows[i]["STORCODE"].ToString() });
                    }
                }

                //StoreList.Add(new SelectListItem() { Text = "废料仓", Value = "FP001" });
                ViewBag.Store = StoreList;

                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ItemNo",
                    Where = string.Format(" Key.BillCode = '{0}'"
                                                , BillCode)
                };
                MethodReturnResult<IList<WOReportDetail>> result = client.GetWOReportDetail(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;

                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_wDetailListPartial", new WOReportDetailViewModel() { BillCode = BillCode });
            }
            else
            {
                return View(model);
            }
        }
    }
}