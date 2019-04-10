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
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using System.Data;
using System.Xml;
using System.IO;
using System.Net;
using ServiceCenter.MES.Service.Contract.ERP;
using ServiceCenter.Client.Mvc.RDLC;
using Microsoft.Reporting.WebForms;
using System.Threading.Tasks;
using ServiceCenter.Common;
using System.Collections;
using System.Configuration;

namespace ServiceCenter.Client.Mvc.Areas.ERP.Controllers
{
    public class ERPWOReportDetailController : Controller
    {
        //显示非报废入库单据明细
        public ActionResult Index(string BillCode)
        {
            WOReportDetailQueryViewModel model = new WOReportDetailQueryViewModel();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<WOReport> rst = client.GetWOReport(BillCode);
                if (rst.Code > 0 || rst.Data == null)
                {
                    return RedirectToAction("Index", "ERPWOReport");
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
                    model.WRCode = rst.Data.WRCode;
                    model.BillState = rst.Data.BillState;       //入库单状态
                    model.BillType = rst.Data.BillType;         //入库单类型
                }

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
                    ViewBag.WRCode = rst.Data.WRCode;
                }
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new WOReportDetailViewModel() { BillCode = BillCode });
            }
            else
            {
                return View(model);
            }

        }

        //刷新数据列表
        public ActionResult Query(WOReportDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WOReportClient client = new WOReportClient())
                {
                    StringBuilder where = new StringBuilder();
                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.ObjectNumber))
                        {
                            where.AppendFormat(" {0} Key.ObjectNumber LIKE '{1}%'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.ObjectNumber);
                        }
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key",
                        Where = where.ToString()
                    };

                    MethodReturnResult<IList<WOReportDetail>> result = client.GetWOReportDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                }
            }
            return PartialView("_ListPartial");
        }

        /// <summary>
        /// 保存入库单明细
        /// </summary>
        /// <param name="BillCode">入库单号</param>
        /// <param name="ObjectNumber">托号、批次号</param>
        /// <param name="ScrapType">报废单标志</param>
        /// <returns></returns>
        public ActionResult SaveStockInDetail(string BillCode, string ObjectNumber, EnumScrapType ScrapType)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                using (WOReportClient client = new WOReportClient())
                {
                    string operateComputer = Request.UserHostAddress;      //客户端
                    //创建入库单明细对象添加到入库单中
                    WOReportDetail woReportDetail = new WOReportDetail()
                    {
                        Key = new WOReportDetailKey()
                        {
                            BillCode = BillCode             //入库单号

                        },
                        ObjectNumber = ObjectNumber,        //托号或批次号
                        Creator = User.Identity.Name,       //编辑人
                        Editor = User.Identity.Name,        //修改人
                        OperateComputer = operateComputer,  //客户端
                    };

                    result = client.AddWOReportDetail(woReportDetail, ScrapType);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format(StringResource.WOReportDetail_Add_Success, ObjectNumber);
                    }
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = string.Format(@"托{0}新增值入库单{1}失败",ObjectNumber, BillCode) + e.Message;
            }
            return Json(result);

            #region 备份于2016-11-29 18:56:37
            //MethodReturnResult result = new MethodReturnResult();
            //using (WOReportClient client = new WOReportClient())
            //{
            //    MethodReturnResult<DataSet> ds = client.GetPackageInfoEx(ObjectNumber, ScrapType);
            //    if (ds.Data != null && ds.Data.Tables[0].Rows.Count > 0)
            //    {
            //        WOReportDetailKey Key = new WOReportDetailKey()
            //        {
            //            BillCode = BillCode,
            //            ObjectNumber = ObjectNumber
            //        };

            //        MethodReturnResult<WOReport> rst = client.GetWOReport(Key.BillCode);
            //        if (rst.Code == 0)
            //        {
            //            //if (MixType != "True")
            //            //{
            //            //    if (rst.Data.OrderNumber != ds.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString())
            //            //    {
            //            //        result.Code = 999;
            //            //        result.Message = string.Format(StringResource.ERPWOReportDetail_Error_OrderNumer, Key.ObjectNumber);
            //            //        return Json(result);
            //            //    }
            //            //}
            //        }
            //        PagingConfig cfg = new PagingConfig()
            //        {
            //            OrderBy = "ItemNo",
            //            Where = string.Format(" Key.BillCode = '{0}'"
            //                                        , BillCode)
            //        };
            //        MethodReturnResult<IList<WOReportDetail>> rst1 = client.GetWOReportDetail(ref cfg);

            //        if (rst1.Code == 0 && rst1.Data.Count > 0)
            //        {
            //            if (rst.Data.MixType == 0)
            //            {
            //                if (rst1.Data[0].MaterialCode != ds.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString())
            //                {
            //                    result.Code = 1001;
            //                    result.Message = string.Format(StringResource.ERPWOReportDetail_Error_MaterialCode, Key.ObjectNumber);
            //                    return Json(result);
            //                }
            //                //if (rst1.Data[0].Color != ds.Data.Tables[0].Rows[0]["COLOR"].ToString())
            //                //{
            //                //    result.Code = 1002;
            //                //    result.Message = string.Format(StringResource.ERPWOReportDetail_Error_Color, Key.ObjectNumber);
            //                //    return Json(result);
            //                //}
            //                if (rst1.Data[0].Grade != ds.Data.Tables[0].Rows[0]["GRADE"].ToString())
            //                {
            //                    result.Code = 1003;
            //                    result.Message = string.Format(StringResource.ERPWOReportDetail_Error_Grade, Key.ObjectNumber);
            //                    return Json(result);
            //                }
            //                if (rst1.Data[0].EffiCode != ds.Data.Tables[0].Rows[0]["SPM_VALUE"].ToString())
            //                {
            //                    result.Code = 1004;
            //                    result.Message = string.Format(StringResource.ERPWOReportDetail_Error_EffiCode, Key.ObjectNumber);
            //                    return Json(result);
            //                }

            //                if (rst1.Data[0].EffiName != ds.Data.Tables[0].Rows[0]["PM_NAME"].ToString())
            //                {
            //                    result.Code = 1005;
            //                    result.Message = string.Format(StringResource.ERPWOReportDetail_Error_EffiName, Key.ObjectNumber);
            //                    return Json(result);
            //                }
            //            }
            //        }

            //        //判断该箱号在其他单子里是否已经添加
            //        PagingConfig cfgEx = new PagingConfig()
            //        {
            //            OrderBy = "ItemNo",
            //            Where = string.Format("Key.ObjectNumber = '{0}' AND Key.BillCode !='{1}'"
            //                                        , ObjectNumber, BillCode)
            //        };
            //        MethodReturnResult<IList<WOReportDetail>> rstEx = client.GetWOReportDetail(ref cfgEx);
            //        if (rstEx.Code == 0 && rstEx.Data.Count > 0)
            //        {
            //            result.Message = string.Format("【{0}】已经存在于别的入库申请单，无法再添加！", Key.ObjectNumber);
            //            return Json(result);
            //        }

            //        WOReportDetail woReportDetail = new WOReportDetail()
            //        {
            //            Key = Key,
            //            MaterialCode = ds.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString(),
            //            MaterialName = ds.Data.Tables[0].Rows[0]["MATERIAL_NAME"].ToString(),
            //            //Color = ds.Data.Tables[0].Rows[0]["COLOR"].ToString(),
            //            Grade = ds.Data.Tables[0].Rows[0]["GRADE"].ToString(),
            //            EffiCode = ds.Data.Tables[0].Rows[0]["SPM_VALUE"].ToString(),
            //            EffiName = ds.Data.Tables[0].Rows[0]["PM_NAME"].ToString(),
            //            Qty = Convert.ToDecimal(ds.Data.Tables[0].Rows[0]["QTY"].ToString()),
            //            SumCoefPMax = Convert.ToDecimal(ds.Data.Tables[0].Rows[0]["SumCOEF_PMAX"].ToString())
            //        };
            //        result = client.AddWOReportDetail(woReportDetail,ScrapType);
            //        if (result.Code == 0)
            //        {
            //            result.Message = string.Format(StringResource.WOReportDetail_Add_Success, Key.ObjectNumber);
            //        }
            //    }
            //    else
            //    {
            //        result.Code = 999;
            //        result.Message = string.Format(StringResource.ERPWOReportDetail_Error_Object, ObjectNumber);
            //    }
            //}
            //return Json(result);
            #endregion
        }

        /// <summary>
        /// 删除入库单明细（对应托号所有信息都将删除）
        /// </summary>
        /// <param name="billCode">入库单号</param>
        /// <param name="itemNo">项目号</param>
        /// <param name="objectNumber">托号、批次号</param>
        /// <returns></returns>
        public ActionResult DeleteStockInDetail(string billCode, int itemNo, string objectNumber)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                using (WOReportClient client = new WOReportClient())
                {
                    WOReportDetailKey Key = new WOReportDetailKey()
                    {
                        BillCode = billCode, 
                        ItemNo = itemNo
                    };

                    WOReportDetail woReportDetail = new WOReportDetail()
                    {
                        Key = new WOReportDetailKey()
                        {
                            BillCode = billCode             //入库单号
                        },
                        ObjectNumber = objectNumber,        //托号
                        Creator = User.Identity.Name,       //编辑人
                        Editor = User.Identity.Name,        //修改人
                    };

                    result = client.DeleteWOReportDetail(woReportDetail, Key);

                    if (result.Code == 0)
                    {
                        result.Message = string.Format(StringResource.WOReportDetail_Delete_Success, objectNumber);
                    }
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
            }

            return Json(result);
        }

        /// <summary>
        /// 产成品及在制品入库申请操作
        /// </summary>
        /// <param name="BillCode">入库单号</param>
        /// <param name="ScrapType">是否报废</param>
        /// <returns></returns>
        public ActionResult StockInApply(string BillCode, string ScrapType)
        {
            MethodReturnResult result = new MethodReturnResult();
            DateTime dtStartTime = DateTime.Now;
            string strERPTime = "";
            try
            {
                using (WOReportClient client = new WOReportClient())
                {
                    #region 1.取得入库单表头对象
                    MethodReturnResult<WOReport> rstInWOrder = client.GetWOReport(BillCode);

                    if (!string.IsNullOrEmpty(rstInWOrder.Data.WRCode))
                    {
                        result.Code = 1004;
                        result.Message = string.Format(StringResource.WOReport_StockInApply_Error_Again, BillCode);
                        return Json(result);
                    }
                    #endregion

                    #region 2.判断ERP生产报告是否已经生成
                    result = client.GetERPWorkReprotBillCode(rstInWOrder.Data.Key);

                    if (result.Code > 0)
                    {
                        return Json(result);
                    }
                    else
                    {
                        //if (!string.IsNullOrEmpty(result.ObjectNo))
                        //{
                        //    result.Code = 1005;
                        //    result.Message = string.Format("ERP报工单已经生成，报工单号[{0}]。", result.ObjectNo);

                        //    return Json(result);
                        //}
                    }
                    #endregion

                    #region 3.取得MES入库单明细数据
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(" Key.BillCode = '{0}'"
                                                    , BillCode)
                    };

                    MethodReturnResult<IList<WOReportDetail>> resultIWDetail = client.GetWOReportDetail(ref cfg);

                    if (resultIWDetail.Code > 0)
                    {
                        return Json(resultIWDetail);
                    }
                    #endregion

                    #region 4.判断托号是否已在其他待入库单据
                    else
                    {
                        foreach (WOReportDetail item in resultIWDetail.Data)
                        {
                            //判断托号是否已经存在其他入库单中
                            PagingConfig cfg1 = new PagingConfig()
                            {
                                IsPaging = false,
                                OrderBy = "ItemNo DESC",
                                Where = string.Format(" ObjectNumber = '{0}' and Key.BillCode <> '{1}'"
                                                        , item.ObjectNumber, BillCode)
                            };

                            MethodReturnResult<IList<WOReportDetail>> lstWOReportDetail1 = client.GetWOReportDetail(ref cfg1);

                            if (lstWOReportDetail1.Data!=null && lstWOReportDetail1.Data.Count> 0)
                            {
                                for (int i = 0; i < lstWOReportDetail1.Data.Count; i++)
                                {
                                    PagingConfig cfg3 = new PagingConfig()
                                    {
                                        IsPaging = false,
                                        OrderBy = "EditTime DESC",
                                        Where = string.Format(" Key = '{0}' and (BillState = 0 or BillState = 1)"
                                                                , lstWOReportDetail1.Data[i].Key.BillCode)
                                    };

                                    MethodReturnResult<IList<WOReport>> lstWOReport = client.GetWOReport(ref cfg3);
                                    if (lstWOReport.Data != null && lstWOReport.Data.Count > 0)
                                    {
                                        result.Code = 1003;
                                        result.Message = String.Format("项目[{0}]已经在入库单{1}中！",
                                                                       item.ObjectNumber,
                                                                       lstWOReportDetail1.Data[i].Key.BillCode);

                                        return Json(result);
                                    } 
                                }                                                               
                            }
                        }
                    }
                    #endregion

                    if (string.IsNullOrEmpty(result.ObjectNo))
                    {
                        #region 5.创建ERP接口XML，返回ERP报工单主键
                        DateTime dtERPStartTime = DateTime.Now;

                        result = CreateERPWorkReport(rstInWOrder.Data, resultIWDetail.Data);

                        if (result.Code > 0)
                        {
                            return Json(result);
                        }

                        DateTime dtERPEndTime = DateTime.Now;

                        strERPTime = " ERP报工单创建时间：" + (dtERPEndTime - dtERPStartTime).TotalSeconds.ToString("F1") + " 秒，[ "
                                             + dtERPStartTime.ToString() + " - "
                                             + dtERPEndTime.ToString() + " ]";

                        if (result.Code > 0)
                        {
                            return Json(result);
                        }
                        #endregion
                    }
                    
                    #region 6.根据MES入库单号取得ERP报工单号
                    string ERPWorkReprotBillCode = "";
                    string ERPWorkReprotBillKey = result.ObjectNo;

                    result = client.GetERPWorkReprotBillCode(rstInWOrder.Data.Key);

                    if (result.Code > 0)
                    {
                        return Json(result);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(result.ObjectNo))
                        {
                            result.Code = 1002;
                            result.Message = string.Format("ERP报工单主键[{0}]对应报工单号提取异常！", ERPWorkReprotBillKey);

                            return Json(result);
                        }
                    }
                    
                    //取得生成的ERP报工单号
                    ERPWorkReprotBillCode = result.ObjectNo;

                    #endregion

                    #region 7.根据入库单号创建批次事物
                    WOReportParameter p = new WOReportParameter() 
                    {
                        BillCode = BillCode,                        //入库单号
                        Editor = User.Identity.Name,                //编辑人
                        OperateComputer = Request.UserHostAddress,  //客户端
                        BillState = EnumBillState.Apply,            //单据状态入库申请
                        ERPWorkReportCode = ERPWorkReprotBillCode,  //ERP报工单号
                        ERPWorkReportKey = ERPWorkReprotBillKey,    //ERP报工单主键
                        OperationType = 0                           //操作状态 0 - 新增申请
                    };

                    //入库申报-MES系统数据处理
                    result = client.StockInApply(p);

                    if (result.Code == 0)
                    {
                        DateTime dtEndTime = DateTime.Now;

                        result.Message = string.Format(StringResource.WOReport_StockInApply_Success, BillCode) +
                                         "执行时间：" + (dtEndTime - dtStartTime).TotalSeconds.ToString("F1") + " 秒，[ " 
                                         + dtStartTime.ToString() + " - "
                                         + dtEndTime.ToString() + " ] "
                                         + strERPTime;
                    }
                    #endregion
                }                
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = string.Format(StringResource.WOReport_StockInApply_Error, BillCode) + e.Message;
            }

            return Json(result);
        }

        /// <summary>
        /// 通过XML接口生成ERP报工单（ERP报工单为MES入库申请）
        /// </summary>
        /// <param name="iWOrder">入库单表头对象</param>
        /// <param name="lstIWDetail">入库单表体明细对象数组</param>
        /// <returns>Code > 0 失败 ，ObjectNo - 返回ERP报工单主键</returns>
        public MethodReturnResult CreateERPWorkReport(WOReport iWOrder, IList<WOReportDetail> lstIWDetail)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            XmlDocument xmlDoc = new XmlDocument();     //XML对象
            DateTime now = DateTime.Now;
           
            string ProductType = "1";                   //ERP产品类型
            string ERPDEPTCode = "";                    //ERP部门编码

            string ERPAccount = "";                     //ERP账套代码
            string ERPGroupCode = "";                   //ERP集团代码
            string ERPOrg = "";                         //ERP组织代码

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
                root.SetAttribute("billtype", "55A4");          //单据类型
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
                CreateNode(xmlDoc, billheadNode, "pk_wr", "");              //生产报告主键

                //3.2.2创建产品节点
                XmlElement productNode = xmlDoc.CreateElement("product");
                billheadNode.AppendChild(productNode);

                #endregion

                #region 4.产品明细节点
                int rowNo = 0;              //行号
                DataTable dtWorkOrder;      //ERP工单属性

                string startTime = Convert.ToDateTime(iWOrder.BillDate).ToShortDateString() + " 00:00:00";
                string endTime = Convert.ToDateTime(iWOrder.BillDate).ToShortDateString() + " 23:59:59";

                foreach (WOReportDetail iWDetail in lstIWDetail)
                {
                    //4.1创建项目明细主节点
                    XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);

                    productNode.AppendChild(itemNode);

                    #region 4.2创建项目明细属性节点
                    CreateNode(xmlDoc, itemNode, "pk_wr_product", "");                                  //生产报告产出信息
                    CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);                             //集团（ERP组织编码）
                    CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);                                     //工厂（ERP接收组织）
                    CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);                                   //工厂版本（ERP接收组织）
                    CreateNode(xmlDoc, itemNode, "vbrowno", (rowNo++).ToString());                      //行号

                    //取得ERP工单属性
                    dtWorkOrder = GetWorkOrder(iWDetail.OrderNumber);

                    if (dtWorkOrder.Rows.Count == 0)
                    {
                        result.Code = 1000;
                        result.Message = string.Format("工单[{0}]在ERP中不存在，请核对！", iWDetail.OrderNumber);

                        return result;
                    }

                    CreateNode(xmlDoc, itemNode, "cbmoid", dtWorkOrder.Rows[0]["PK_DMO"].ToString());       //生产订单主键
                    CreateNode(xmlDoc, itemNode, "vbmobillcode", iWDetail.OrderNumber);                     //生产订单号

                    CreateNode(xmlDoc, itemNode, "cbfirstmoid", dtWorkOrder.Rows[0]["PK_DMO"].ToString());  //源头生产订单 
                    CreateNode(xmlDoc, itemNode, "vbfirstmocode", iWDetail.OrderNumber);                    //源头生产订单号                    
                    CreateNode(xmlDoc, itemNode, "vbsrcmocode", iWDetail.OrderNumber);                      //来源生产订单号

                    CreateNode(xmlDoc, itemNode, "fbproducttype", ProductType);                             //产品类型
                    CreateNode(xmlDoc, itemNode, "cbmaterialid", iWDetail.MaterialCode);                    //产品代码
                    CreateNode(xmlDoc, itemNode, "cbmaterialvid", iWDetail.MaterialCode);                   //产品版本
                    CreateNode(xmlDoc, itemNode, "cbbomversionid", dtWorkOrder.Rows[0]["CBOMVERSIONID"].ToString());    //生产BOM版本
                    CreateNode(xmlDoc, itemNode, "vbbomversioncode", dtWorkOrder.Rows[0]["VBOMVERSION"].ToString());    //生产BOM版本号                    
                    CreateNode(xmlDoc, itemNode, "cbmainbomid", dtWorkOrder.Rows[0]["CBOMVERSIONID"].ToString());       //主产品BOM版本
                    CreateNode(xmlDoc, itemNode, "vbmainbomcode", dtWorkOrder.Rows[0]["VBOMVERSION"].ToString());       //主产品BOM版本号
                    CreateNode(xmlDoc, itemNode, "cbdeptid", dtWorkOrder.Rows[0]["DEPTCODE"].ToString());   //生产部门
                    CreateNode(xmlDoc, itemNode, "cbdeptvid", dtWorkOrder.Rows[0]["DEPTCODE"].ToString());  //生产部门版本
                    CreateNode(xmlDoc, itemNode, "tbstarttime", startTime);                                 //开始时间
                    CreateNode(xmlDoc, itemNode, "tbendtime", endTime);                                     //结束时间
                    CreateNode(xmlDoc, itemNode, "vbbatchcode", iWDetail.ObjectNumber);                     //生产批次号(托号)
                    CreateNode(xmlDoc, itemNode, "fbsourcetype", "2");                                      //来源类型
                    CreateNode(xmlDoc, itemNode, "cbunitid", "Pcs");                                        //主单位
                    CreateNode(xmlDoc, itemNode, "cbastunitid", "WA");                                      //单位

                    //如果为D级组件，则换算率为1/1
                    if (iWDetail.Grade == "D")
                    {
                        CreateNode(xmlDoc, itemNode, "vbchangerate", "1/1");
                    }
                    else
                    {
                        CreateNode(xmlDoc, itemNode, "vbchangerate", "1/" + iWDetail.EffiCode);             //换算率(标称功率)
                    }

                    CreateNode(xmlDoc, itemNode, "nbwrnum", iWDetail.Qty.ToString());                       //完工主数量（块）

                    //当组件为D级组件时，完工数量为完工主数量（标称功率）
                    if (iWDetail.Grade == "D")
                    {
                        CreateNode(xmlDoc, itemNode, "nbwrastnum", iWDetail.Qty.ToString());                //完工数量等于完工主数量（标称功率）
                    }
                    else
                    {
                        CreateNode(xmlDoc, itemNode, "nbwrastnum", Convert.ToDecimal(Convert.ToDecimal(iWDetail.EffiCode) * iWDetail.Qty).ToString("f3"));
                    }

                    CreateNode(xmlDoc, itemNode, "nbsldchecknum", "0");
                    CreateNode(xmlDoc, itemNode, "nbsldcheckastnum", "0");
                    CreateNode(xmlDoc, itemNode, "nbchecknum", "0");
                    CreateNode(xmlDoc, itemNode, "nbcheckastnum", "0");
                    CreateNode(xmlDoc, itemNode, "bbhasbckfled", "N");
                    CreateNode(xmlDoc, itemNode, "bbhaspicked", "N");
                    CreateNode(xmlDoc, itemNode, "bbstockbycheck", "N");
                    CreateNode(xmlDoc, itemNode, "bbisempass", "N");
                    CreateNode(xmlDoc, itemNode, "bbchkflag", "N");
                    CreateNode(xmlDoc, itemNode, "bbinstock", "N");
                    CreateNode(xmlDoc, itemNode, "bbotherreject", "N");
                    CreateNode(xmlDoc, itemNode, "bbsetmark", "N");
                    //CreateNode(xmlDoc, itemNode, "vbsalebillcode", "");//dt.Rows[0]["VSALEBILLCODE"].ToString()
                    //CreateNode(xmlDoc, itemNode, "vbsalebillid", "");//dt.Rows[0]["VSRCID"].ToString()
                    CreateNode(xmlDoc, itemNode, "vbsrctranstype", dtWorkOrder.Rows[0]["VTRANTYPECODE"].ToString());    //来源交易类型编码
                    CreateNode(xmlDoc, itemNode, "cbsrctranstype", dtWorkOrder.Rows[0]["VTRANTYPEID"].ToString());      //来源交易类型
                    CreateNode(xmlDoc, itemNode, "cbsrctype", "55C2");
                    CreateNode(xmlDoc, itemNode, "vbsrccode", dtWorkOrder.Rows[0]["VBILLCODE"].ToString());             //工单号
                    CreateNode(xmlDoc, itemNode, "vbsrcid", dtWorkOrder.Rows[0]["PK_DMO"].ToString());                  //来源单据

                    //源头交易类型编码
                    CreateNode(xmlDoc, itemNode, "vbfirstranstype", string.IsNullOrEmpty(dtWorkOrder.Rows[0]["VFIRSTCODE"].ToString()) ? dtWorkOrder.Rows[0]["VTRANTYPECODE"].ToString() : dtWorkOrder.Rows[0]["VFIRSTTRANTYPECODE"].ToString());
                    CreateNode(xmlDoc, itemNode, "cbfirstranstype", string.IsNullOrEmpty(dtWorkOrder.Rows[0]["VFIRSTCODE"].ToString()) ? dtWorkOrder.Rows[0]["VTRANTYPEID"].ToString() : dtWorkOrder.Rows[0]["VFIRSTTRANTYPEID"].ToString());
                    CreateNode(xmlDoc, itemNode, "vbfirsttype", string.IsNullOrEmpty(dtWorkOrder.Rows[0]["VFIRSTCODE"].ToString()) ? "55C2" : dtWorkOrder.Rows[0]["VFIRSTTYPE"].ToString());
                    CreateNode(xmlDoc, itemNode, "vbfirstcode", string.IsNullOrEmpty(dtWorkOrder.Rows[0]["VFIRSTCODE"].ToString()) ? dtWorkOrder.Rows[0]["VBILLCODE"].ToString() : dtWorkOrder.Rows[0]["VFIRSTCODE"].ToString());
                    CreateNode(xmlDoc, itemNode, "vbfirstid", string.IsNullOrEmpty(dtWorkOrder.Rows[0]["VFIRSTCODE"].ToString()) ? dtWorkOrder.Rows[0]["PK_DMO"].ToString() : dtWorkOrder.Rows[0]["VFIRSTID"].ToString());
                    CreateNode(xmlDoc, itemNode, "bbhasfbill", "N");

                    //组件功率编码（D级组件功率为000）
                    //if (iWDetail.Grade == "D")
                    //{
                    //    CreateNode(xmlDoc, itemNode, "vbfree1", "000");
                    //}
                    //else
                    //{
                    CreateNode(xmlDoc, itemNode, "vbfree1", GetCodeByName(iWDetail.EffiName, "JN0001"));
                    //}

                        CreateNode(xmlDoc, itemNode, "vbfree2", GetCodeByName(iWDetail.PsSubcode, "JN0002"));         //电流档
                    CreateNode(xmlDoc, itemNode, "vbfree3", GetCodeByName(iWDetail.Grade, "JN0003"));             //产品等级                    
                    #endregion

                    //取得ERP部门代码（临时方案，取最后一个工单的部门代码）
                    if (dtWorkOrder.Rows[0]["VBILLCODE"].ToString().ToUpper().Substring(0, 3) == "2MO" || dtWorkOrder.Rows[0]["VBILLCODE"].ToString().ToUpper().Substring(0, 3) == "8MO")
                    {
                        ERPDEPTCode = dtWorkOrder.Rows[0]["CJCODE"].ToString();
                    }
                    else
                    {
                        ERPDEPTCode = dtWorkOrder.Rows[0]["DEPTCODE"].ToString();
                    }                   
                }
                #endregion

                #region 5.入库单属性节点
                CreateNode(xmlDoc, billheadNode, "pk_group", ERPGroupCode);
                CreateNode(xmlDoc, billheadNode, "pk_org", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "pk_org_v", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "vbillcode", "");
                CreateNode(xmlDoc, billheadNode, "fbillstatus", "1");

                CreateNode(xmlDoc, billheadNode, "vdef17", iWOrder.Key.ToString());                         //MES入库单号

                CreateNode(xmlDoc, billheadNode, "vtrantypeid", "0001A110000000001MSI");                    //报工单交易类型PK
                CreateNode(xmlDoc, billheadNode, "vtrantypecode", "55A4-01");                               //报告类型为空，报工单交易类型
                CreateNode(xmlDoc, billheadNode, "fprodmode", "2");                                         //生产模式
                CreateNode(xmlDoc, billheadNode, "cdeptid", ERPDEPTCode);                                   //生产部门最新
                CreateNode(xmlDoc, billheadNode, "cdeptvid", ERPDEPTCode);                                  //生产部门
                CreateNode(xmlDoc, billheadNode, "dbilldate", iWOrder.BillDate.ToString());                 //报产日期
                CreateNode(xmlDoc, billheadNode, "vnote", iWOrder.Note);                                    //备注
                CreateNode(xmlDoc, billheadNode, "billmaker", User.Identity.Name);                                              //ERP制单人为本次操作人员
                CreateNode(xmlDoc, billheadNode, "auditer", User.Identity.Name);                                                //ERP审核人为本次操作人员
                CreateNode(xmlDoc, billheadNode, "dmakedate", iWOrder.BillMakedDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));     //制单日期
                CreateNode(xmlDoc, billheadNode, "creator", iWOrder.Creator);                                            //创建人                
                CreateNode(xmlDoc, billheadNode, "creationtime", now.ToString("yyyy-MM-dd HH:mm:ss"));                          //报工单创建日期
                #endregion

                //XML路径
                string path = Server.MapPath("~/XMLFile/");
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                path = path + iWOrder.Key + ".xml";
                xmlDoc.Save(path);

                FileStream ms = new FileStream(path, FileMode.Open, FileAccess.Read);

                //string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];
                //HttpWebRequest loHttp = (HttpWebRequest)WebRequest.Create(url);                             //URL为XChangeServlet地址
                httpWebRequest.Method = "POST";

                // *** Set any header related and operational properties
                httpWebRequest.Timeout = 600000;                         // 10 secs  超时控制
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

                LogHelper.WriteLogError("End Send Xml File");
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
        /// 撤销入库申报
        /// </summary>
        /// <param name="billCode">入库单号</param>
        /// <returns></returns>
        public ActionResult RevokeStockInApply(string billCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            DateTime dtStartTime = DateTime.Now;

            try
            {
                using (WOReportClient client = new WOReportClient())
                {
                    //取得入库单表头对象
                    MethodReturnResult<WOReport> rstInWOrder = client.GetWOReport(billCode);

                    if (rstInWOrder.Data.BillState != EnumBillState.Apply)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("入库单状态为[{0}],不能撤销", rstInWOrder.Data.BillState.GetDisplayName());

                        return Json(result);
                    }

                    //判断ERP入库单是否删除
                    result = client.GetERPWorkReprotBillCode(rstInWOrder.Data.Key);

                    if (result.Code > 0)
                    {
                        return Json(result);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(result.ObjectNo))
                        {
                            result.Code = 1002;
                            result.Message = string.Format("ERP报工单[{0}]还未删除！", result.ObjectNo);

                            return Json(result);
                        }
                    }

                    WOReportParameter pram = new WOReportParameter()
                    {
                        BillCode = billCode,
                        BillState = EnumBillState.Apply,
                        OperateComputer = Request.UserHostAddress,      //客户端
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name,
                        OperationType = -1                              //操作状态 -1 - 撤销申报
                    };

                    result = client.StockInApply(pram);

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
        /// 入库单打印
        /// </summary>
        /// <param name="BillCode">入库单号</param>
        /// <returns></returns>
        public ActionResult Print(string BillCode)
        {
            try
            {
                if (string.IsNullOrEmpty(BillCode))
                {
                    return Content(string.Empty);
                }

                return ShowStgInReport(BillCode);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [AllowAnonymous]
        public ActionResult ShowStgInReport(string BillCode)
        {
            MethodReturnResult result = new MethodReturnResult();

            DataSet dsData = new DataSet();
            
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> dsResult = client.GetWOReportFromDB(BillCode);

                if (dsResult.Code > 0)
                {
                    result.Code = dsResult.Code;
                    result.Message = dsResult.Message;
                    //return result;
                }
                dsData = dsResult.Data;
            }

            DataTable dtStgIn = dsData.Tables["StgIn"];

            PackageListDataSet ds = new PackageListDataSet();
            PackageListDataSet.StgInRow row = ds.StgIn.NewStgInRow();
            if (dtStgIn.Rows.Count > 0)
            {
                //DataTable dtERP = new DataTable();
                //using (WOReportClient clientERP = new WOReportClient())
                //{
                //    MethodReturnResult<DataSet> dsERP = clientERP.GetERPReportCodeById(dtStgIn.Rows[0]["ERP_WR_CODE"] == null ? "" : dtStgIn.Rows[0]["ERP_WR_CODE"].ToString());
                //    if (dsERP.Code > 0)
                //    {
                //        result.Code = dsERP.Code;
                //        result.Message = dsERP.Message;
                //        //return result;
                //    }
                //    else
                //    {
                //        dtERP = dsERP.Data.Tables[0];
                //    }

                //}
                //if (dtERP.Rows.Count > 0)
                //{
                //    row.ERPBillCode = dtERP.Rows[0]["VBILLCODE"].ToString();
                //}
                row.ERPBillCode = dtStgIn.Rows[0]["ERP_WR_CODE"].ToString();

                row.BillCode = dtStgIn.Rows[0]["BILL_CODE"].ToString();
                row.BillDate = dtStgIn.Rows[0]["BILL_DATE"].ToString();
                row.BillMaker = dtStgIn.Rows[0]["BILL_MAKER"].ToString();
                row.OrderNumber = dtStgIn.Rows[0]["ORDER_NUMBER"].ToString();
                row.Store = dtStgIn.Rows[0]["STORE"].ToString();
                row.Note = dtStgIn.Rows[0]["NOTE"].ToString();
                ds.StgIn.AddStgInRow(row);
            }


            if (dsData.Tables.Contains("StgInDetail"))
            {
                double dQty = 0;
                DataTable dtStgInDetail = dsData.Tables["StgInDetail"];
                PackageListDataSet.StgInDetailRow rowDetail = ds.StgInDetail.NewStgInDetailRow();
                for (int i = 0; i < dtStgInDetail.Rows.Count; i++)
                {
                    rowDetail = ds.StgInDetail.NewStgInDetailRow();
                    //rowDetail.ItemNo = (i + 1).ToString();
                    rowDetail.ItemNo = dtStgInDetail.Rows[i]["ItemNo"].ToString();
                    rowDetail.MaterialCode = dtStgInDetail.Rows[i]["MATERIAL_CODE"].ToString();
                    rowDetail.MaterialName = dtStgInDetail.Rows[i]["MATERIAL_NAME"].ToString();

                    rowDetail.ObjectNumber = dtStgInDetail.Rows[i]["PACKAGE_NO"].ToString();
                    rowDetail.OrderNumber = dtStgInDetail.Rows[i]["ORDER_NUMBER"].ToString();
                    if (dtStgInDetail.Rows[i]["GRADE"].ToString() == "D")
                    {
                        rowDetail.sumCoefPMax = "0.00";
                    }
                    else
                    {
                        rowDetail.sumCoefPMax = Math.Round(Convert.ToDecimal(dtStgInDetail.Rows[i]["sumCOEF_PMAX"]), 2).ToString();
                    }
                    if (dtStgInDetail.Rows[i]["GRADE"].ToString() == "D")
                    {
                        rowDetail.PmName = "0W";
                    }
                    else
                    {
                        rowDetail.PmName = dtStgInDetail.Rows[i]["PM_NAME"].ToString();
                    }
                    rowDetail.PsSubCodeName = dtStgInDetail.Rows[i]["PS_SUBCODE_Name"].ToString();
                    if (dtStgInDetail.Rows[i]["GRADE"].ToString() == "D")
                    {
                        rowDetail.SpmValue = "1/1";
                    }
                    else
                    {
                        rowDetail.SpmValue = "1/" + Math.Round(Convert.ToDecimal(dtStgInDetail.Rows[i]["SPM_VALUE"]), 0).ToString();
                    }
                    rowDetail.Grade = dtStgInDetail.Rows[i]["GRADE"].ToString();
                    //rowDetail.Color = dtStgInDetail.Rows[i]["COLOR"].ToString();



                    if (Double.TryParse(dtStgInDetail.Rows[i]["QTY"].ToString(), out dQty) == false)
                    {
                        dQty = 0;
                    }
                    rowDetail.Qty = dQty;
                    ds.StgInDetail.AddStgInDetailRow(rowDetail);
                }

            }


            //
            using (LocalReport localReport = new LocalReport())
            {
                localReport.ReportPath = Server.MapPath("~/RDLC/StInList.rdlc");

                ReportDataSource reportDataSourcePackage = new ReportDataSource("StgIn", ds.Tables[ds.StgIn.TableName]);
                localReport.DataSources.Add(reportDataSourcePackage);
                ReportDataSource reportDataSourcePackageDetail = new ReportDataSource("StgInDetail", ds.Tables[ds.StgInDetail.TableName]);
                localReport.DataSources.Add(reportDataSourcePackageDetail);
                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;
                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                string deviceInfo =
                                "<DeviceInfo>" +
                                "  <OutputFormat>PDF</OutputFormat>" +
                                "  <PageWidth>24cm</PageWidth>" +
                                "  <PageHeight>14cm</PageHeight>" +
                                "  <MarginTop>0.5cm</MarginTop>" +
                                "  <MarginLeft>0.5cm</MarginLeft>" +
                                "  <MarginRight>0cm</MarginRight>" +
                                "  <MarginBottom>0.5cm</MarginBottom>" +
                                "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                //Render the report
                renderedBytes = localReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);
                //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
                return File(renderedBytes, mimeType);
            }

        }

        /// <summary>
        /// 根据属性名称查找字符串中设置的属性值
        /// </summary>
        /// <param name="valueString"></param>
        /// <param name="valueName"></param>
        /// <param name="operators"></param>
        /// <param name="terminator"></param>
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
 
        //public ActionResult AntiState(string BillCode, string WRCode)
        //{
        //    MethodReturnResult result = new MethodReturnResult();
        //    using (WOReportClient client = new WOReportClient())
        //    {
        //        MethodReturnResult<DataSet> ds = client.GetERPReportCodeById(BillCode);

        //        if (ds.Data.Tables[0].Rows.Count > 0)
        //        {
        //            result.Code = 1004;
        //            result.Message = "请先删除ERP中的报工单！";
        //        }
        //        else
        //        {
        //            MethodReturnResult resultwo = new MethodReturnResult();
        //            using (WOReportClient clientwo = new WOReportClient())
        //            {
        //                WOReportParameter pram = new WOReportParameter()
        //                {
        //                    BillCode = BillCode,
        //                    BillState = EnumBillState.Apply,
        //                    OperateComputer = Request.UserHostAddress,  //客户端
        //                    Editor = User.Identity.Name,
        //                    WRCode = "",
        //                    OperationType = -1
        //                };

        //                result = clientwo.StockInApply(pram);
        //                if (result.Code == 0)
        //                {
        //                    result.Message = string.Format(StringResource.WOReport_StockInApply_Success, BillCode);
        //                }
        //            }
        //        }
        //    }
        //    return Json(result);
        //}
        

        //[AllowAnonymous]
        //public ActionResult ShowStgInReport(string BillCode)
        //{
        //    MethodReturnResult result = new MethodReturnResult();
        //    DataSet dsData = new DataSet();

        //    using (WOReportClient client = new WOReportClient())
        //    {
        //        MethodReturnResult<DataSet> dsResult = client.GetWOReportFromDB(BillCode);

        //        if (dsResult.Code > 0)
        //        {
        //            result.Code = dsResult.Code;
        //            result.Message = dsResult.Message;
        //            //return result;
        //        }
        //        dsData = dsResult.Data;
        //    }
        //    DataTable dtStgIn = dsData.Tables["StgIn"];

        //    PackageListDataSet ds = new PackageListDataSet();
        //    PackageListDataSet.StgInRow row = ds.StgIn.NewStgInRow();
        //    if (dtStgIn.Rows.Count > 0)
        //    {
        //        DataTable dtERP = new DataTable();
        //        using (WOReportClient clientERP = new WOReportClient())
        //        {
        //            MethodReturnResult<DataSet> dsERP = clientERP.GetERPReportCodeById(dtStgIn.Rows[0]["ERP_WR_CODE"] == null ? "" : dtStgIn.Rows[0]["ERP_WR_CODE"].ToString());
        //            if (dsERP.Code > 0)
        //            {
        //                result.Code = dsERP.Code;
        //                result.Message = dsERP.Message;
        //                //return result;
        //            }
        //            else
        //            {
        //                dtERP = dsERP.Data.Tables[0];
        //            }

        //        }
        //        if (dtERP.Rows.Count > 0)
        //        {
        //            row.ERPBillCode = dtERP.Rows[0]["VBILLCODE"].ToString();
        //        }
        //        row.BillCode = dtStgIn.Rows[0]["BILL_CODE"].ToString();
        //        row.BillDate = dtStgIn.Rows[0]["BILL_DATE"].ToString();
        //        row.BillMaker = dtStgIn.Rows[0]["BILL_MAKER"].ToString();
        //        row.OrderNumber = dtStgIn.Rows[0]["ORDER_NUMBER"].ToString();
        //        row.Store = dtStgIn.Rows[0]["STORE"].ToString();
        //        row.Note = dtStgIn.Rows[0]["NOTE"].ToString();
        //        ds.StgIn.AddStgInRow(row);
        //    }


        //    if (dsData.Tables.Contains("StgInDetail"))
        //    {
        //        double dQty = 0;
        //        DataTable dtStgInDetail = dsData.Tables["StgInDetail"];
        //        PackageListDataSet.StgInDetailRow rowDetail = ds.StgInDetail.NewStgInDetailRow();
        //        for (int i = 0; i < dtStgInDetail.Rows.Count; i++)
        //        {

                   
        //            rowDetail = ds.StgInDetail.NewStgInDetailRow();
        //            rowDetail.ItemNo = (i + 1).ToString();
        //            rowDetail.MaterialCode = dtStgInDetail.Rows[i]["MATERIAL_CODE"].ToString();
        //            rowDetail.MaterialName = dtStgInDetail.Rows[i]["MATERIAL_NAME"].ToString();
                   
        //            rowDetail.ObjectNumber = dtStgInDetail.Rows[i]["PACKAGE_NO"].ToString();
        //            rowDetail.OrderNumber = dtStgInDetail.Rows[i]["ORDER_NUMBER"].ToString();
        //            if (dtStgInDetail.Rows[i]["GRADE"].ToString() == "D")
        //            {
        //                rowDetail.sumCoefPMax = "0.00";
        //            }
        //            else 
        //            {
        //                rowDetail.sumCoefPMax = Math.Round(Convert.ToDecimal(dtStgInDetail.Rows[i]["sumCOEF_PMAX"]), 2).ToString();
        //            }
        //            if (dtStgInDetail.Rows[i]["GRADE"].ToString() == "D")
        //            {
        //                rowDetail.PmName = "0W";
        //            }
        //            else
        //            {
        //                rowDetail.PmName = dtStgInDetail.Rows[i]["PM_NAME"].ToString();
        //            }
        //            rowDetail.PsSubCodeName = dtStgInDetail.Rows[i]["PS_SUBCODE_Name"].ToString();
        //            if (dtStgInDetail.Rows[i]["GRADE"].ToString() == "D")
        //            {
        //                rowDetail.SpmValue = "1/1"; 
        //            }
        //            else
        //            {
        //                rowDetail.SpmValue = "1/" + Math.Round(Convert.ToDecimal(dtStgInDetail.Rows[i]["SPM_VALUE"]), 0).ToString();
        //            }
        //            rowDetail.Grade = dtStgInDetail.Rows[i]["GRADE"].ToString();
        //            //rowDetail.Color = dtStgInDetail.Rows[i]["COLOR"].ToString();



        //            if (Double.TryParse(dtStgInDetail.Rows[i]["QTY"].ToString(), out dQty) == false)
        //            {
        //                dQty = 0;
        //            }
        //            rowDetail.Qty = dQty;
        //            ds.StgInDetail.AddStgInDetailRow(rowDetail);
        //            }

        //    }


        //    //
        //    using (LocalReport localReport = new LocalReport())
        //    {
        //        localReport.ReportPath = Server.MapPath("~/RDLC/StInList.rdlc");

        //        ReportDataSource reportDataSourcePackage = new ReportDataSource("StgIn", ds.Tables[ds.StgIn.TableName]);
        //        localReport.DataSources.Add(reportDataSourcePackage);
        //        ReportDataSource reportDataSourcePackageDetail = new ReportDataSource("StgInDetail", ds.Tables[ds.StgInDetail.TableName]);
        //        localReport.DataSources.Add(reportDataSourcePackageDetail);
        //        string reportType = "PDF";
        //        string mimeType;
        //        string encoding;
        //        string fileNameExtension;
        //        //The DeviceInfo settings should be changed based on the reportType
        //        //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
        //        string deviceInfo =
        //                        "<DeviceInfo>" +
        //                        "  <OutputFormat>PDF</OutputFormat>" +
        //                        "  <PageWidth>24cm</PageWidth>" +
        //                        "  <PageHeight>14cm</PageHeight>" +
        //                        "  <MarginTop>0.5cm</MarginTop>" +
        //                        "  <MarginLeft>0.5cm</MarginLeft>" +
        //                        "  <MarginRight>0cm</MarginRight>" +
        //                        "  <MarginBottom>0.5cm</MarginBottom>" +
        //                        "</DeviceInfo>";
        //        Warning[] warnings;
        //        string[] streams;
        //        byte[] renderedBytes;
        //        //Render the report
        //        renderedBytes = localReport.Render(
        //            reportType,
        //            deviceInfo,
        //            out mimeType,
        //            out encoding,
        //            out fileNameExtension,
        //            out streams,
        //            out warnings);
        //        //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
        //        return File(renderedBytes, mimeType);
        //    }
        
        //}
        private string GetEffiRate(string lowerEffi)
        {
            string Effi = "0";
            double dlowerEffi = 0;

            if (lowerEffi != null)
            {
                if (double.TryParse(lowerEffi, out dlowerEffi) == true)
                {
                    Effi = (dlowerEffi * 0.24336).ToString("f3");
                }
            }
            return Effi;
        }

        /// <summary> 创建ERP接口XML，返回ERP报工单主键 -1 - 失败 </summary>
        /// <param name="woReport">入库单表头对象</param>
        /// <param name="lstDetail">入库单表体对象</param>
        /// <param name="msg">返回错误信息</param>
        /// <returns></returns>
        public string CreateXmlFile(WOReport woReport, DataSet lstDetail, out string msg)
        {
            string strResult = "-1";
            MethodReturnResult<DataSet> dsResult;

            DataSet dsWOData = new DataSet();
            MethodReturnResult result = new MethodReturnResult();
            DataSet dsOrderType = new DataSet();

            msg = "";

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

            using (ERPClient client = new ERPClient())
            {
                //取得ERP工单信息
                dsResult = client.GetERPWorkOrder(woReport.OrderNumber);

                if (dsResult.Code > 0)
                {
                    msg = dsResult.Code + dsResult.Message;

                    return strResult;
                }

                dsWOData = dsResult.Data;
            }
                        
            using (ERPClient client = new ERPClient())
            {
                //取得ERP工单类型
                dsResult = client.GetERPOrderType(dsWOData.Tables[0].Rows[0]["vtrantypecode"].ToString());

                if (dsResult.Code > 0)
                {
                    msg = dsResult.Code + dsResult.Message;

                    return strResult;
                }

                dsOrderType = dsResult.Data;
            }

            
            DataTable dt = GetWorkOrder(woReport.OrderNumber);
            string strPreorderWord = woReport.OrderNumber;

            Hashtable htTables = new Hashtable();
            htTables.Add(woReport.OrderNumber, dt);

            if (dsOrderType.Tables[0].Rows.Count > 0)
            {
                string startTime = Convert.ToDateTime(woReport.BillDate).ToShortDateString() + " 00:00:00";
                string endTime = Convert.ToDateTime(woReport.BillDate).ToShortDateString() + " 23:59:59";
                
                XmlDocument xmlDoc = new XmlDocument();
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
                root.SetAttribute("groupcode", ERPGroupCode);
                root.SetAttribute("filename", "");
                root.SetAttribute("billtype", "55A4");
                root.SetAttribute("account", ERPAccount);
                xmlDoc.AppendChild(root);

                XmlElement Node = xmlDoc.CreateElement("bill");//创建节点ufinterface子节点bill   
                Node.SetAttribute("id", "");
                xmlDoc.DocumentElement.AppendChild(Node);

                XmlNode billheadNode = xmlDoc.CreateNode(XmlNodeType.Element, "billhead", null);
                Node.AppendChild(billheadNode);

                CreateNode(xmlDoc, billheadNode, "pk_wr", "");

                XmlElement productNode = xmlDoc.CreateElement("product");
                billheadNode.AppendChild(productNode);

                if (lstDetail.Tables[0].Rows.Count > 0)
                {

                    //t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.PACKAGE_NO ,t2.COLOR,t2.GRADE ,
                    //t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER
                    #region 创建xml
                    int i = 1;
                    foreach (var item in lstDetail.Tables[0].AsEnumerable())
                    {
                        //dt = GetWorkOrder(item["ORDER_NUMBER"].ToString().ToUpper());
                        //if(dt==null || dt.Rows.Count==0)
                        //{

                        //}
                        // string Effi = GetEffi(item["PACKAGE_NO"].ToString());
                        if (htTables.ContainsKey(item["ORDER_NUMBER"].ToString().ToUpper()))
                        {
                            dt = (DataTable)htTables[item["ORDER_NUMBER"].ToString().ToUpper()];
                        }
                        else
                        {
                            dt = GetWorkOrder(item["ORDER_NUMBER"].ToString().ToUpper());
                            if (dt==null || dt.Rows.Count==0)
                            {
                                msg = string.Format("工单{0}在ERP中不存在。", item["ORDER_NUMBER"].ToString().ToUpper());
                                return "";
                            }
                            htTables.Add(item["ORDER_NUMBER"].ToString().ToUpper(),dt);
                        }





                        DataTable dt_Unit = GetUnit(item["MATERIAL_CODE"].ToString());

                        XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        productNode.AppendChild(itemNode);
                        if (woReport.ScrapType.ToString() == "True")
                        { 
                            CreateNode(xmlDoc, itemNode, "pk_wr_product", "");
                            XmlNode qualityvosNode = xmlDoc.CreateNode(XmlNodeType.Element, "qualityvos", null);
                            itemNode.AppendChild(qualityvosNode);
                            XmlNode item1Node = xmlDoc.CreateNode(XmlNodeType.Element, "item1", null);
                            qualityvosNode.AppendChild(item1Node);
                            XmlNode vginstockbcodeNode = xmlDoc.CreateNode(XmlNodeType.Element, "vginstockbcode", null);
                            item1Node.AppendChild(vginstockbcodeNode);
                            CreateNode(xmlDoc, item1Node, "vginstockbcode", item["PACKAGE_NO"].ToString());
                            XmlNode fgprocessmethodNode = xmlDoc.CreateNode(XmlNodeType.Element, "fgprocessmethod", null);
                            item1Node.AppendChild(fgprocessmethodNode);
                            CreateNode(xmlDoc, item1Node, "fgprocessmethod", "2");


                            CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);
                            CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);
                            CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);
                            CreateNode(xmlDoc, itemNode, "vbrowno", (i++).ToString());
                            CreateNode(xmlDoc, itemNode, "cbmoid", dt.Rows[0]["PK_DMO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmobillcode", dt.Rows[0]["VBILLCODE"].ToString());//woReport.OrderNumber
                            CreateNode(xmlDoc, itemNode, "cbmobid", "");//空
                            CreateNode(xmlDoc, itemNode, "vbmorowno", "");
                            CreateNode(xmlDoc, itemNode, "vbmoparentbillcode", "");//, dt.Rows[0]["VPARENTMOCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "cbfirstmoid", dt.Rows[0]["PK_DMO"].ToString());// 
                            //CreateNode(xmlDoc, itemNode, "vbfirstmocode", woReport.OrderNumber);
                            CreateNode(xmlDoc, itemNode, "vbfirstmocode", dt.Rows[0]["VBILLCODE"].ToString());

                            CreateNode(xmlDoc, itemNode, "cbfirstmobid", "");
                            CreateNode(xmlDoc, itemNode, "vbfirstmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cbsrcmoid", "");// dt.Rows[0]["VORIGMOID"].ToString()
                        
                            //CreateNode(xmlDoc, itemNode, "vbsrcmocode", woReport.OrderNumber);
                            CreateNode(xmlDoc, itemNode, "vbsrcmocode", dt.Rows[0]["VBILLCODE"].ToString());
                        
                            CreateNode(xmlDoc, itemNode, "cbsrcmobid", "");
                            CreateNode(xmlDoc, itemNode, "vbsrcmorowno", "");//空
                            CreateNode(xmlDoc, itemNode, "fbproducttype", "1");
                            CreateNode(xmlDoc, itemNode, "cbmaterialid", item["MATERIAL_CODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbmaterialvid", item["MATERIAL_CODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbbomversionid", dt.Rows[0]["CBOMVERSIONID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbbomversioncode", dt.Rows[0]["VBOMVERSION"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmainmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cbmainbomid", dt.Rows[0]["CBOMVERSIONID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmainbomcode", dt.Rows[0]["VBOMVERSION"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbpackbomid", "");
                            CreateNode(xmlDoc, itemNode, "cbmainmaterialid", "");
                            CreateNode(xmlDoc, itemNode, "cbmainmaterialvid", "");
                            CreateNode(xmlDoc, itemNode, "cbdeptid", dt.Rows[0]["DEPTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbdeptvid", dt.Rows[0]["DEPTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "tbstarttime", startTime);
                            CreateNode(xmlDoc, itemNode, "tbendtime", endTime);
                            CreateNode(xmlDoc, itemNode, "vbbatchid", "");
                            CreateNode(xmlDoc, itemNode, "vbbatchcode", item["PACKAGE_NO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbinbatchid", "");
                            CreateNode(xmlDoc, itemNode, "vbinbatchcode", "");
                            CreateNode(xmlDoc, itemNode, "fbsourcetype", "2");
                            CreateNode(xmlDoc, itemNode, "cbunitid", dt_Unit.Rows[0]["MEASCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbastunitid", dt_Unit.Rows[0]["ASTMEASCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbchangerate", "1/260" );
                            CreateNode(xmlDoc, itemNode, "nbplanwrnum", "");//dt.Rows[0]["NPLANPUTNUM"].ToString()
                            CreateNode(xmlDoc, itemNode, "nbplanwrastnum", "");// dt.Rows[0]["NPLANPUTASTNUM"].ToString()
                            CreateNode(xmlDoc, itemNode, "nbwrnum", item["Qty"].ToString());//
                            CreateNode(xmlDoc, itemNode, "nbwrastnum", (Convert.ToDecimal(item["SPM_VALUE"].ToString()) * Convert.ToDecimal(item["Qty"])).ToString("f3"));
                            CreateNode(xmlDoc, itemNode, "nbsldchecknum", "0");
                            CreateNode(xmlDoc, itemNode, "nbsldcheckastnum", "0");
                            CreateNode(xmlDoc, itemNode, "nbchecknum", "0");
                            CreateNode(xmlDoc, itemNode, "nbcheckastnum", "0");
                            CreateNode(xmlDoc, itemNode, "bbhasbckfled", "N");
                            CreateNode(xmlDoc, itemNode, "bbhaspicked", "N");
                            CreateNode(xmlDoc, itemNode, "bbstockbycheck", "N");
                            CreateNode(xmlDoc, itemNode, "bbisempass", "N");
                            CreateNode(xmlDoc, itemNode, "bbchkflag", "N");
                            CreateNode(xmlDoc, itemNode, "bbinstock", "N");
                            CreateNode(xmlDoc, itemNode, "bbotherreject", "N");
                            CreateNode(xmlDoc, itemNode, "bbsetmark", "N");
                            CreateNode(xmlDoc, itemNode, "cbempass_bid", "");
                            CreateNode(xmlDoc, itemNode, "vbsalebillcode", "");//dt.Rows[0]["VSALEBILLCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbsalebillid", "");//dt.Rows[0]["VSRCID"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbsrctranstype", dt.Rows[0]["VTRANTYPECODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbsrctranstype", dt.Rows[0]["VTRANTYPEID"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbsrctype", "55C2");
                            CreateNode(xmlDoc, itemNode, "vbsrccode", dt.Rows[0]["VBILLCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbsrcid", dt.Rows[0]["PK_DMO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbsrcrowid", "");
                            CreateNode(xmlDoc, itemNode, "vbsrcrowno", "");
                            CreateNode(xmlDoc, itemNode, "vbfirstranstype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VTRANTYPECODE"].ToString() : dt.Rows[0]["VFIRSTTRANTYPECODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbfirstranstype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VTRANTYPEID"].ToString() : dt.Rows[0]["VFIRSTTRANTYPEID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirsttype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? "55C2" : dt.Rows[0]["VFIRSTTYPE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstcode", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VBILLCODE"].ToString() : dt.Rows[0]["VFIRSTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstid", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["PK_DMO"].ToString() : dt.Rows[0]["VFIRSTID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstrowid", "");//dt.Rows[0]["VFIRSTBID"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbfirstrowno", "");// dt.Rows[0]["VFIRSTCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbnote", "");
                            CreateNode(xmlDoc, itemNode, "tbsourcebillts", "");
                            CreateNode(xmlDoc, itemNode, "bbhasfbill", "N");
                            CreateNode(xmlDoc, itemNode, "cbprojectid", "");
                            CreateNode(xmlDoc, itemNode, "vbfree1", "260");//功率PM_NAME
                            CreateNode(xmlDoc, itemNode, "vbfree2", "03");//电流PS_SUB
                            CreateNode(xmlDoc, itemNode, "vbfree3", "01");//等级GRADE
                            CreateNode(xmlDoc, itemNode, "vbfree4", "");
                            CreateNode(xmlDoc, itemNode, "vbfree5", "");
                            CreateNode(xmlDoc, itemNode, "vbfree6", "");
                            CreateNode(xmlDoc, itemNode, "vbmainidentify", "");
                            CreateNode(xmlDoc, itemNode, "vbidentify", "");
                            CreateNode(xmlDoc, itemNode, "vbparentmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cwr_productid", "");
                        }
                        else
                        {
                            CreateNode(xmlDoc, itemNode, "pk_wr_product", "");
                            CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);
                            CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);
                            CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);
                            CreateNode(xmlDoc, itemNode, "vbrowno", (i++).ToString());
                            CreateNode(xmlDoc, itemNode, "cbmoid", dt.Rows[0]["PK_DMO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmobillcode", dt.Rows[0]["VBILLCODE"].ToString());//woReport.OrderNumber
                            CreateNode(xmlDoc, itemNode, "cbmobid", "");//空
                            CreateNode(xmlDoc, itemNode, "vbmorowno", "");
                            CreateNode(xmlDoc, itemNode, "vbmoparentbillcode", "");//, dt.Rows[0]["VPARENTMOCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "cbfirstmoid", dt.Rows[0]["PK_DMO"].ToString());// 
                            //CreateNode(xmlDoc, itemNode, "vbfirstmocode", woReport.OrderNumber);
                            CreateNode(xmlDoc, itemNode, "vbfirstmocode", dt.Rows[0]["VBILLCODE"].ToString());

                            CreateNode(xmlDoc, itemNode, "cbfirstmobid", "");
                            CreateNode(xmlDoc, itemNode, "vbfirstmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cbsrcmoid", "");// dt.Rows[0]["VORIGMOID"].ToString()

                            //CreateNode(xmlDoc, itemNode, "vbsrcmocode", woReport.OrderNumber);
                            CreateNode(xmlDoc, itemNode, "vbsrcmocode", dt.Rows[0]["VBILLCODE"].ToString());

                            CreateNode(xmlDoc, itemNode, "cbsrcmobid", "");
                            CreateNode(xmlDoc, itemNode, "vbsrcmorowno", "");//空
                            CreateNode(xmlDoc, itemNode, "fbproducttype", "1");
                            CreateNode(xmlDoc, itemNode, "cbmaterialid", item["MATERIAL_CODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbmaterialvid", item["MATERIAL_CODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbbomversionid", dt.Rows[0]["CBOMVERSIONID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbbomversioncode", dt.Rows[0]["VBOMVERSION"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmainmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cbmainbomid", dt.Rows[0]["CBOMVERSIONID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmainbomcode", dt.Rows[0]["VBOMVERSION"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbpackbomid", "");
                            CreateNode(xmlDoc, itemNode, "cbmainmaterialid", "");
                            CreateNode(xmlDoc, itemNode, "cbmainmaterialvid", "");
                            CreateNode(xmlDoc, itemNode, "cbdeptid", dt.Rows[0]["DEPTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbdeptvid", dt.Rows[0]["DEPTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "tbstarttime", startTime);
                            CreateNode(xmlDoc, itemNode, "tbendtime", endTime);
                            CreateNode(xmlDoc, itemNode, "vbbatchid", "");
                            CreateNode(xmlDoc, itemNode, "vbbatchcode", item["PACKAGE_NO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbinbatchid", "");
                            CreateNode(xmlDoc, itemNode, "vbinbatchcode", "");
                            CreateNode(xmlDoc, itemNode, "fbsourcetype", "2");
                            CreateNode(xmlDoc, itemNode, "cbunitid", dt_Unit.Rows[0]["MEASCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbastunitid", dt_Unit.Rows[0]["ASTMEASCODE"].ToString());
                            //如果为D级组件，则换算率为1/1
                            if (item["GRADE"].ToString() == "D")
                            {
                                CreateNode(xmlDoc, itemNode, "vbchangerate", "1/1");
                            }
                            else 
                            {
                                CreateNode(xmlDoc, itemNode, "vbchangerate", "1/" + item["SPM_VALUE"].ToString());
                            }
                            CreateNode(xmlDoc, itemNode, "nbplanwrnum", "");//dt.Rows[0]["NPLANPUTNUM"].ToString()
                            CreateNode(xmlDoc, itemNode, "nbplanwrastnum", "");// dt.Rows[0]["NPLANPUTASTNUM"].ToString()
                            CreateNode(xmlDoc, itemNode, "nbwrnum", item["Qty"].ToString());//
                            //当组件为D级组件时，实际功率为0

                            if (item["GRADE"].ToString() == "D")
                            {
                                CreateNode(xmlDoc, itemNode, "nbwrastnum", "0.000");
                            }
                            else
                            {
                                CreateNode(xmlDoc, itemNode, "nbwrastnum", (Convert.ToDecimal(item["SPM_VALUE"].ToString()) * Convert.ToDecimal(item["Qty"])).ToString("f3"));
                            }

                            
                            CreateNode(xmlDoc, itemNode, "nbsldchecknum", "0");
                            CreateNode(xmlDoc, itemNode, "nbsldcheckastnum", "0");
                            CreateNode(xmlDoc, itemNode, "nbchecknum", "0");
                            CreateNode(xmlDoc, itemNode, "nbcheckastnum", "0");
                            CreateNode(xmlDoc, itemNode, "bbhasbckfled", "N");
                            CreateNode(xmlDoc, itemNode, "bbhaspicked", "N");
                            CreateNode(xmlDoc, itemNode, "bbstockbycheck", "N");
                            CreateNode(xmlDoc, itemNode, "bbisempass", "N");
                            CreateNode(xmlDoc, itemNode, "bbchkflag", "N");
                            CreateNode(xmlDoc, itemNode, "bbinstock", "N");
                            CreateNode(xmlDoc, itemNode, "bbotherreject", "N");
                            CreateNode(xmlDoc, itemNode, "bbsetmark", "N");
                            CreateNode(xmlDoc, itemNode, "cbempass_bid", "");
                            CreateNode(xmlDoc, itemNode, "vbsalebillcode", "");//dt.Rows[0]["VSALEBILLCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbsalebillid", "");//dt.Rows[0]["VSRCID"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbsrctranstype", dt.Rows[0]["VTRANTYPECODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbsrctranstype", dt.Rows[0]["VTRANTYPEID"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbsrctype", "55C2");
                            CreateNode(xmlDoc, itemNode, "vbsrccode", dt.Rows[0]["VBILLCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbsrcid", dt.Rows[0]["PK_DMO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbsrcrowid", "");
                            CreateNode(xmlDoc, itemNode, "vbsrcrowno", "");
                            CreateNode(xmlDoc, itemNode, "vbfirstranstype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VTRANTYPECODE"].ToString() : dt.Rows[0]["VFIRSTTRANTYPECODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbfirstranstype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VTRANTYPEID"].ToString() : dt.Rows[0]["VFIRSTTRANTYPEID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirsttype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? "55C2" : dt.Rows[0]["VFIRSTTYPE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstcode", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VBILLCODE"].ToString() : dt.Rows[0]["VFIRSTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstid", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["PK_DMO"].ToString() : dt.Rows[0]["VFIRSTID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstrowid", "");//dt.Rows[0]["VFIRSTBID"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbfirstrowno", "");// dt.Rows[0]["VFIRSTCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbnote", "");
                            CreateNode(xmlDoc, itemNode, "tbsourcebillts", "");
                            CreateNode(xmlDoc, itemNode, "bbhasfbill", "N");
                            CreateNode(xmlDoc, itemNode, "cbprojectid", "");
                            //判断是否为D级组件，如果为D级组件，则默认功率为0

                            if (item["GRADE"].ToString() == "D")
                            {
                                CreateNode(xmlDoc, itemNode, "vbfree1", "000");//功率PM_NAME
                            }
                            else
                            {
                                CreateNode(xmlDoc, itemNode, "vbfree1", GetCodeByName(item["PM_NAME"].ToString(), "JN0001"));//功率PM_NAME
                            }
                            CreateNode(xmlDoc, itemNode, "vbfree2", GetCodeByName(item["PS_SUBCODE_Name"].ToString(), "JN0002"));//电流PS_SUB
                            CreateNode(xmlDoc, itemNode, "vbfree3", GetCodeByName(item["GRADE"].ToString(), "JN0003"));//等级GRADE
                            CreateNode(xmlDoc, itemNode, "vbfree4", "");
                            CreateNode(xmlDoc, itemNode, "vbfree5", "");
                            CreateNode(xmlDoc, itemNode, "vbfree6", "");
                            CreateNode(xmlDoc, itemNode, "vbmainidentify", "");
                            CreateNode(xmlDoc, itemNode, "vbidentify", "");
                            CreateNode(xmlDoc, itemNode, "vbparentmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cwr_productid", "");
                        }
                    }
                    #endregion
                }


                CreateNode(xmlDoc, billheadNode, "pk_group", ERPGroupCode);
                CreateNode(xmlDoc, billheadNode, "pk_org", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "pk_org_v", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "vbillcode", "");
                CreateNode(xmlDoc, billheadNode, "fbillstatus", "1");

                CreateNode(xmlDoc, billheadNode, "vtrantypeid", dsOrderType.Tables[0].Rows[0]["ctrantypeid2"].ToString());//报告类型为空dt.Rows[0]["VTRANTYPEID"].ToString()，报工单交易类型PK
                CreateNode(xmlDoc, billheadNode, "vtrantypecode", dsOrderType.Tables[0].Rows[0]["transtype2"].ToString());//报告类型为空，报工单交易类型
                CreateNode(xmlDoc, billheadNode, "fprodmode", "2");
                CreateNode(xmlDoc, billheadNode, "cdeptid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cdeptvid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "dbilldate", woReport.BillDate.ToString());
                CreateNode(xmlDoc, billheadNode, "vnote", woReport.Note);
                CreateNode(xmlDoc, billheadNode, "billmaker", woReport.BillMaker);
                CreateNode(xmlDoc, billheadNode, "dmakedate",woReport.BillMakedDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                CreateNode(xmlDoc, billheadNode, "creator", woReport.Creator);
                CreateNode(xmlDoc, billheadNode, "creationtime", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                CreateNode(xmlDoc, billheadNode, "cwrid", "");

                string path = Server.MapPath("~/XMLFile/");
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                path = path + woReport.Key + ".xml";
                xmlDoc.Save(path);

                FileStream ms = new FileStream(path, FileMode.Open, FileAccess.Read);

                //string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];
                HttpWebRequest loHttp = (HttpWebRequest)WebRequest.Create(url);  //URL为XChangeServlet地址

                loHttp.Method = "POST";
                // *** Set any header related and operational properties
                loHttp.Timeout = 10000;  // 10 secs
                loHttp.UserAgent = "Code Sample Web Client";

                // *** reuse cookies if available
                loHttp.CookieContainer = new CookieContainer();

                if (this.oCookies != null && this.oCookies.Count > 0)
                {
                    loHttp.CookieContainer.Add(this.oCookies);
                }

                #region send Xml To NCS
                LogHelper.WriteLogError("Begin Send Xml File");
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

                LogHelper.WriteLogError("End Send Xml File");
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(ResponseText);

                //获取ERP回执
                XmlNode xnode = Doc.SelectSingleNode("ufinterface/sendresult/content");
                strResult = xnode.InnerText;

                //获取ERP错误信息提示
                if (strResult == "")
                {
                    XmlNode errornode = Doc.SelectSingleNode("ufinterface/sendresult/resultdescription");
                    msg = errornode.InnerText;
                }

                loResponseStream.Close();
                loWebResponse.Close();
                ms.Close();
                requestStream.Close();
                #endregion
            }
            else
            {
                msg = string.Format(StringResource.ERPWorkOrderQuery_Error_Query, woReport.OrderNumber);
            }

            return strResult;
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
                if (ds != null && ds.Data != null && ds.Data.Tables.Count > 0)
                {
                    dt = ds.Data.Tables[0];
                }
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
                        MethodReturnResult<IList<WOReportDetail>> result = client.GetWOReportDetail(ref cfg);
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
        public ActionResult sIndex(string BillCode)
        {
            WOReportDetailQueryViewModel model = new WOReportDetailQueryViewModel();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<WOReport> rst = client.GetWOReport(BillCode);
                if (rst.Code > 0 || rst.Data == null)
                {
                    return RedirectToAction("sIndex", "ERPWOReport");
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
                    model.WRCode = rst.Data.WRCode;
                    model.INCode = rst.Data.INCode;
                }

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
                    ViewBag.WRCode = rst.Data.WRCode;
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_sListPartial", new WOReportDetailViewModel() { BillCode = BillCode });
            }
            else
            {
                return View(model);
            }

        }

        public ActionResult sQuery(WOReportDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WOReportClient client = new WOReportClient())
                {
                    StringBuilder where = new StringBuilder();
                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.ObjectNumber))
                        {
                            where.AppendFormat(" {0} Key.ObjectNumber LIKE '{1}%'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , model.ObjectNumber);
                        }
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "Key",
                        Where = where.ToString()
                    };

                    MethodReturnResult<IList<WOReportDetail>> result = client.GetWOReportDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                }
            }
            return PartialView("_sListPartial");
        }

        public ActionResult sSave(string ObjectNumber, string BillCode, string MixType, EnumScrapType ScrapType)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetPackageInfoEx(ObjectNumber, ScrapType);

                if (ds.Data != null && ds.Data.Tables[0].Rows.Count > 0)
                {
                    WOReportDetailKey Key = new WOReportDetailKey()
                    {
                        BillCode = BillCode
                        //BillCode = BillCode,
                        //ObjectNumber = ObjectNumber
                    };

                    MethodReturnResult<WOReport> rst = client.GetWOReport(Key.BillCode);
                    if (rst.Code == 0)
                    {
                        //if (MixType != "True")
                        //{
                        //    if (rst.Data.OrderNumber != ds.Data.Tables[0].Rows[0]["ORDER_NUMBER"].ToString())
                        //    {
                        //        result.Code = 999;
                        //        result.Message = string.Format(StringResource.ERPWOReportDetail_Error_OrderNumer, Key.ObjectNumber);
                        //        return Json(result);
                        //    }
                        //}
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "ItemNo",
                        Where = string.Format(" Key.BillCode = '{0}'"
                                                    , BillCode)
                    };
                    MethodReturnResult<IList<WOReportDetail>> rst1 = client.GetWOReportDetail(ref cfg);

                    if (rst1.Code == 0 && rst1.Data.Count > 0)
                    {
                        if (rst.Data.MixType == 0)
                        {
                            if (rst1.Data[0].MaterialCode != ds.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString())
                            {
                                result.Code = 1001;
                                result.Message = string.Format(StringResource.ERPWOReportDetail_Error_MaterialCode, ObjectNumber);
                                return Json(result);
                            }
                            //if (rst1.Data[0].Color != ds.Data.Tables[0].Rows[0]["COLOR"].ToString())
                            //{
                            //    result.Code = 1002;
                            //    result.Message = string.Format(StringResource.ERPWOReportDetail_Error_Color, Key.ObjectNumber);
                            //    return Json(result);
                            //}
                            if (rst1.Data[0].Grade != ds.Data.Tables[0].Rows[0]["GRADE"].ToString())
                            {
                                result.Code = 1003;
                                result.Message = string.Format(StringResource.ERPWOReportDetail_Error_Grade, ObjectNumber);
                                return Json(result);
                            }
                            if (rst1.Data[0].EffiCode != ds.Data.Tables[0].Rows[0]["SPM_VALUE"].ToString())
                            {
                                result.Code = 1004;
                                result.Message = string.Format(StringResource.ERPWOReportDetail_Error_EffiCode, ObjectNumber);
                                return Json(result);
                            }

                            //if (rst1.Data[0].EffiName != ds.Data.Tables[0].Rows[0]["PM_NAME"].ToString())
                            {
                                result.Code = 1005;
                                result.Message = string.Format(StringResource.ERPWOReportDetail_Error_EffiName, ObjectNumber);
                                return Json(result);
                            }
                        }
                    }

                    //判断该箱号在其他单子里是否已经添加
                    PagingConfig cfgEx = new PagingConfig()
                    {
                        OrderBy = "ItemNo",
                        Where = string.Format("Key.ObjectNumber = '{0}' AND Key.BillCode !='{1}'"
                                                    , ObjectNumber, BillCode)
                    };
                    MethodReturnResult<IList<WOReportDetail>> rstEx = client.GetWOReportDetail(ref cfgEx);
                    if (rstEx.Code == 0 && rstEx.Data.Count > 0)
                    {
                        result.Message = string.Format("【{0}】已经存在于别的入库申请单，无法再添加！", ObjectNumber);
                        return Json(result);
                    }

                    WOReportDetail woReportDetail = new WOReportDetail()
                    {
                        Key = Key,
                        MaterialCode = ds.Data.Tables[0].Rows[0]["MATERIAL_CODE"].ToString(),
                        //MaterialName = ds.Data.Tables[0].Rows[0]["MATERIAL_NAME"].ToString(),
                        //Color = ds.Data.Tables[0].Rows[0]["COLOR"].ToString(),
                        Grade = ds.Data.Tables[0].Rows[0]["GRADE"].ToString(),
                        EffiCode = ds.Data.Tables[0].Rows[0]["SPM_VALUE"].ToString(),
                        //EffiName = ds.Data.Tables[0].Rows[0]["PM_NAME"].ToString()
                    };
                    result = client.AddWOReportDetail(woReportDetail, ScrapType);
                    if (result.Code == 0)
                    {
                        result.Message = string.Format(StringResource.WOReportDetail_Add_Success, ObjectNumber);
                    }
                }
                else
                {
                    result.Code = 999;
                    result.Message = string.Format(StringResource.ERPWOReportDetail_Error_Object, ObjectNumber);
                }
            }
            return Json(result);
        }

        /// <summary>
        /// 删除入库单明细
        /// </summary>        
        /// <param name="billCode">入库单号</param>
        /// <param name="itemNo">项目号</param>
        /// <param name="objectNumber">托号</param>
        /// <returns></returns> public ActionResult sDelete(string objectNumber, string billCode, string orderNumber, string effiName, string color, string psSubcode)
        //public ActionResult sDelete(string billCode, int itemNo, string objectNumber)
        //{
        //    MethodReturnResult result = new MethodReturnResult();

        //    using (WOReportClient client = new WOReportClient())
        //    {
        //        WOReportDetailKey Key = new WOReportDetailKey()
        //        {
        //            BillCode = billCode,
        //            ItemNo = itemNo
        //        };

        //        WOReportDetail woReportDetail = new WOReportDetail()
        //        {
        //            Key = new WOReportDetailKey()
        //            {
        //                BillCode = billCode             //入库单号
        //            },
        //            ObjectNumber = objectNumber,        //托号
        //            Creator = User.Identity.Name,       //编辑人
        //            Editor = User.Identity.Name,        //修改人
        //        };

        //        result = client.DeleteWOReportDetail(woReportDetail,Key);

        //        if (result.Code == 0)
        //        {
        //            result.Message = string.Format(StringResource.WOReportDetail_Delete_Success, objectNumber);
        //        }
        //    }

        //    return Json(result);
        //}

        public ActionResult sCreateXML(string BillCode, string ScrapType)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WOReportClient client = new WOReportClient())
            {

                MethodReturnResult<bool> check = client.CheckReportDetail(BillCode);

                if (!check.Data)
                {
                    result.Code = 1003;
                    result.Message = string.Format("【{0}】中数据有误，请核对单据与实物！", BillCode);
                    return Json(result);
                }



                MethodReturnResult<WOReport> rst = client.GetWOReport(BillCode);

                if (!string.IsNullOrEmpty(rst.Data.WRCode))
                {
                    result.Code = 1004;
                    result.Message = string.Format(StringResource.WOReport_StockInApply_Error_Again, BillCode);
                    return Json(result);
                }


                MethodReturnResult<DataSet> rst1 = client.GetReportDetailByObjectNumber(BillCode, ScrapType);
                try
                {
                    string msg = "";
                    string code = sCreateXmlFile(rst.Data, rst1.Data, out msg);
                    if (!string.IsNullOrEmpty(code))
                    {
                        WOReportParameter pram = new WOReportParameter()
                        {
                            BillState = EnumBillState.Receive,
                            BillCode = BillCode,
                            Editor = User.Identity.Name,
                            //WRCode = code
                        };

                        result = client.WO(pram, ScrapType);
                        if (result.Code == 0)
                        {
                            result.Message = string.Format(StringResource.WOReport_StockInApply_Success, BillCode);
                        }
                    }
                    else
                    {
                        result.Code = 1001;
                        result.Message = string.Format(StringResource.WOReport_StockInApply_Error, BillCode) + msg;
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteLogError("End Send Xml File:Error" + e.Message);
                    result.Code = 1002;
                    result.Message = string.Format(StringResource.WOReport_StockInApply_Error, BillCode) + e.Message;
                }
            }
            return Json(result);
        }

        public ActionResult sAntiState(string BillCode, string WRCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> ds = client.GetERPReportCodeById(BillCode);

                if (ds.Data.Tables[0].Rows.Count > 0)
                {
                    result.Code = 1004;
                    result.Message = "请先删除ERP中的报工单！";
                }
                else
                {
                    MethodReturnResult resultwo = new MethodReturnResult();
                    using (WOReportClient clientwo = new WOReportClient())
                    {
                        WOReportParameter pram = new WOReportParameter()
                        {
                            BillCode = BillCode,
                            BillState = EnumBillState.Apply,
                            Editor = User.Identity.Name,
                            ERPWorkReportKey = null
                        };

                        //result = clientwo.AntiState(pram);
                        if (result.Code == 0)
                        {
                            result.Message = string.Format(StringResource.WOReport_StockInApply_Success, BillCode);
                        }
                    }
                }
            }
            return Json(result);
        }
        public ActionResult sPrint(string BillCode)
        {
            try
            {
                if (string.IsNullOrEmpty(BillCode))
                {
                    return Content(string.Empty);
                }
                return ShowStgInReport(BillCode);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [AllowAnonymous]
        public ActionResult sShowStgInReport(string BillCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            DataSet dsData = new DataSet();
            using (WOReportClient client = new WOReportClient())
            {
                MethodReturnResult<DataSet> dsResult = client.GetWOReportFromDB(BillCode);
                if (dsResult.Code > 0)
                {
                    result.Code = dsResult.Code;
                    result.Message = dsResult.Message;
                    //return result;
                }
                dsData = dsResult.Data;
            }
            DataTable dtStgIn = dsData.Tables["StgIn"];

            PackageListDataSet ds = new PackageListDataSet();
            PackageListDataSet.StgInRow row = ds.StgIn.NewStgInRow();
            if (dtStgIn.Rows.Count > 0)
            {
                DataTable dtERP = new DataTable();
                using (WOReportClient clientERP = new WOReportClient())
                {
                    MethodReturnResult<DataSet> dsERP = clientERP.GetERPReportCodeById(dtStgIn.Rows[0]["ERP_WR_CODE"] == null ? "" : dtStgIn.Rows[0]["ERP_WR_CODE"].ToString());
                    if (dsERP.Code > 0)
                    {
                        result.Code = dsERP.Code;
                        result.Message = dsERP.Message;
                        //return result;
                    }
                    else
                    {
                        dtERP = dsERP.Data.Tables[0];
                    }

                }
                if (dtERP.Rows.Count > 0)
                {
                    row.ERPBillCode = dtERP.Rows[0]["VBILLCODE"].ToString();
                }
                row.BillCode = dtStgIn.Rows[0]["BILL_CODE"].ToString();
                row.BillDate = dtStgIn.Rows[0]["BILL_DATE"].ToString();
                row.BillMaker = dtStgIn.Rows[0]["BILL_MAKER"].ToString();
                row.OrderNumber = dtStgIn.Rows[0]["ORDER_NUMBER"].ToString();
                row.Store = dtStgIn.Rows[0]["STORE"].ToString();
                row.Note = dtStgIn.Rows[0]["NOTE"].ToString();
                ds.StgIn.AddStgInRow(row);
            }


            if (dsData.Tables.Contains("StgInDetail"))
            {
                double dQty = 0;
                DataTable dtStgInDetail = dsData.Tables["StgInDetail"];
                PackageListDataSet.StgInDetailRow rowDetail = ds.StgInDetail.NewStgInDetailRow();
                for (int i = 0; i < dtStgInDetail.Rows.Count; i++)
                {


                    rowDetail = ds.StgInDetail.NewStgInDetailRow();
                    rowDetail.ItemNo = (i + 1).ToString();
                    rowDetail.MaterialCode = dtStgInDetail.Rows[i]["MATERIAL_CODE"].ToString();
                    rowDetail.MaterialName = dtStgInDetail.Rows[i]["MATERIAL_NAME"].ToString();

                    rowDetail.ObjectNumber = dtStgInDetail.Rows[i]["PACKAGE_NO"].ToString();
                    rowDetail.OrderNumber = dtStgInDetail.Rows[i]["ORDER_NUMBER"].ToString();
                    rowDetail.sumCoefPMax = Math.Round(Convert.ToDecimal(dtStgInDetail.Rows[i]["sumCOEF_PMAX"]), 2).ToString();
                    rowDetail.PmName = dtStgInDetail.Rows[i]["PM_NAME"].ToString();
                    rowDetail.PsSubCodeName = dtStgInDetail.Rows[i]["PS_SUBCODE_Name"].ToString();
                    rowDetail.SpmValue = "1/" + Math.Round(Convert.ToDecimal(dtStgInDetail.Rows[i]["SPM_VALUE"]), 0).ToString();
                    rowDetail.Grade = dtStgInDetail.Rows[i]["GRADE"].ToString();
                    //rowDetail.Color = dtStgInDetail.Rows[i]["COLOR"].ToString();



                    if (Double.TryParse(dtStgInDetail.Rows[i]["QTY"].ToString(), out dQty) == false)
                    {
                        dQty = 0;
                    }
                    rowDetail.Qty = dQty;
                    ds.StgInDetail.AddStgInDetailRow(rowDetail);
                }

            }


            //
            using (LocalReport localReport = new LocalReport())
            {
                localReport.ReportPath = Server.MapPath("~/RDLC/StInList.rdlc");

                ReportDataSource reportDataSourcePackage = new ReportDataSource("StgIn", ds.Tables[ds.StgIn.TableName]);
                localReport.DataSources.Add(reportDataSourcePackage);
                ReportDataSource reportDataSourcePackageDetail = new ReportDataSource("StgInDetail", ds.Tables[ds.StgInDetail.TableName]);
                localReport.DataSources.Add(reportDataSourcePackageDetail);
                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;
                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                string deviceInfo =
                                "<DeviceInfo>" +
                                "  <OutputFormat>PDF</OutputFormat>" +
                                "  <PageWidth>24cm</PageWidth>" +
                                "  <PageHeight>14cm</PageHeight>" +
                                "  <MarginTop>0.5cm</MarginTop>" +
                                "  <MarginLeft>0.5cm</MarginLeft>" +
                                "  <MarginRight>0cm</MarginRight>" +
                                "  <MarginBottom>0.5cm</MarginBottom>" +
                                "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;
                //Render the report
                renderedBytes = localReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);
                //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
                return File(renderedBytes, mimeType);
            }

        }
        private string sGetEffiRate(string lowerEffi)
        {
            string Effi = "0";
            double dlowerEffi = 0;

            if (lowerEffi != null)
            {
                if (double.TryParse(lowerEffi, out dlowerEffi) == true)
                {
                    Effi = (dlowerEffi * 0.24336).ToString("f3");
                }
            }
            return Effi;
        }

        public string sCreateXmlFile(WOReport woReport, DataSet lstDetail, out string msg)
        {
            MethodReturnResult result = new MethodReturnResult();
            DataSet dsData = new DataSet();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> dsResult = client.GetERPWorkOrder(woReport.OrderNumber);
                if (dsResult.Code > 0)
                {
                    result.Code = dsResult.Code;
                    result.Message = dsResult.Message;
                    //return result;
                }
                dsData = dsResult.Data;

            }

            MethodReturnResult result1 = new MethodReturnResult();
            DataSet dsData1 = new DataSet();
            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> dsResult = client.GetERPOrderType(dsData.Tables[0].Rows[0]["vtrantypecode"].ToString());
                if (dsResult.Code > 0)
                {
                    result1.Code = dsResult.Code;
                    result1.Message = dsResult.Message;
                    //return result;
                }
                dsData1 = dsResult.Data;

            }

            msg = "";
            string returnCode = "";

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



            DataTable dt = GetWorkOrder(woReport.OrderNumber);
            string strPreorderWord = woReport.OrderNumber;

            Hashtable htTables = new Hashtable();
            htTables.Add(woReport.OrderNumber, dt);

            if (dsData1.Tables[0].Rows.Count > 0)
            {
                string startTime = Convert.ToDateTime(woReport.BillDate).ToShortDateString() + " 00:00:00";
                string endTime = Convert.ToDateTime(woReport.BillDate).ToShortDateString() + " 23:59:59";

                XmlDocument xmlDoc = new XmlDocument();
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
                root.SetAttribute("groupcode", ERPGroupCode);
                root.SetAttribute("filename", "");
                root.SetAttribute("billtype", "55A4");
                root.SetAttribute("account", ERPAccount);
                xmlDoc.AppendChild(root);

                XmlElement Node = xmlDoc.CreateElement("bill");//创建节点ufinterface子节点bill   
                Node.SetAttribute("id", "");
                xmlDoc.DocumentElement.AppendChild(Node);

                XmlNode billheadNode = xmlDoc.CreateNode(XmlNodeType.Element, "billhead", null);
                Node.AppendChild(billheadNode);

                CreateNode(xmlDoc, billheadNode, "pk_wr", "");

                XmlElement productNode = xmlDoc.CreateElement("product");
                billheadNode.AppendChild(productNode);

                if (lstDetail.Tables[0].Rows.Count > 0)
                {

                    //t2.MATERIAL_CODE, t2.MATERIAL_NAME, t2.PACKAGE_NO ,t2.COLOR,t2.GRADE ,
                    //t2.SPM_VALUE ,t2.PM_NAME ,t2.PS_SUBCODE_Name,t2.ORDER_NUMBER
                    #region 创建xml
                    int i = 1;
                    foreach (var item in lstDetail.Tables[0].AsEnumerable())
                    {
                        //dt = GetWorkOrder(item["ORDER_NUMBER"].ToString().ToUpper());
                        //if(dt==null || dt.Rows.Count==0)
                        //{

                        //}
                        // string Effi = GetEffi(item["PACKAGE_NO"].ToString());
                        if (htTables.ContainsKey(item["ORDER_NUMBER"].ToString().ToUpper()))
                        {
                            dt = (DataTable)htTables[item["ORDER_NUMBER"].ToString().ToUpper()];
                        }
                        else
                        {
                            dt = GetWorkOrder(item["ORDER_NUMBER"].ToString().ToUpper());
                            if (dt == null || dt.Rows.Count == 0)
                            {
                                msg = string.Format("工单{0}在ERP中不存在。", item["ORDER_NUMBER"].ToString().ToUpper());
                                return "";
                            }
                            htTables.Add(item["ORDER_NUMBER"].ToString().ToUpper(), dt);
                        }
                        DataTable dt_Unit = GetUnit(item["MATERIAL_CODE"].ToString());

                        XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        productNode.AppendChild(itemNode);
                        if (woReport.ScrapType.ToString() == "True")
                        {

                            CreateNode(xmlDoc, itemNode, "pk_wr_product", "");
                            XmlNode qualityvosNode = xmlDoc.CreateNode(XmlNodeType.Element, "qualityvos", null);
                            itemNode.AppendChild(qualityvosNode);
                            XmlNode item1Node = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                            qualityvosNode.AppendChild(item1Node);
                            //XmlNode vginstockbcodeNode = xmlDoc.CreateNode(XmlNodeType.Element, "vginstockbcode", null);
                            //item1Node.AppendChild(vginstockbcodeNode);
                            CreateNode(xmlDoc, item1Node, "vginstockbcode", item["PACKAGE_NO"].ToString());
                            //XmlNode fgprocessmethodNode = xmlDoc.CreateNode(XmlNodeType.Element, "fgprocessmethod", null);
                            //item1Node.AppendChild(fgprocessmethodNode);
                            CreateNode(xmlDoc, item1Node, "fgprocessmethod", "2");


                            CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);
                            CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);
                            CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);
                            CreateNode(xmlDoc, itemNode, "vbrowno", (i++).ToString());
                            CreateNode(xmlDoc, itemNode, "cbmoid", dt.Rows[0]["PK_DMO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmobillcode", dt.Rows[0]["VBILLCODE"].ToString());//woReport.OrderNumber
                            CreateNode(xmlDoc, itemNode, "cbmobid", "");//空
                            CreateNode(xmlDoc, itemNode, "vbmorowno", "");
                            CreateNode(xmlDoc, itemNode, "vbmoparentbillcode", "");//, dt.Rows[0]["VPARENTMOCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "cbfirstmoid", dt.Rows[0]["PK_DMO"].ToString());// 
                            //CreateNode(xmlDoc, itemNode, "vbfirstmocode", woReport.OrderNumber);
                            CreateNode(xmlDoc, itemNode, "vbfirstmocode", dt.Rows[0]["VBILLCODE"].ToString());

                            CreateNode(xmlDoc, itemNode, "cbfirstmobid", "");
                            CreateNode(xmlDoc, itemNode, "vbfirstmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cbsrcmoid", "");// dt.Rows[0]["VORIGMOID"].ToString()

                            //CreateNode(xmlDoc, itemNode, "vbsrcmocode", woReport.OrderNumber);
                            CreateNode(xmlDoc, itemNode, "vbsrcmocode", dt.Rows[0]["VBILLCODE"].ToString());

                            CreateNode(xmlDoc, itemNode, "cbsrcmobid", "");
                            CreateNode(xmlDoc, itemNode, "vbsrcmorowno", "");//空
                            CreateNode(xmlDoc, itemNode, "fbproducttype", "1");
                            CreateNode(xmlDoc, itemNode, "cbmaterialid", item["MATERIAL_CODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbmaterialvid", item["MATERIAL_CODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbbomversionid", dt.Rows[0]["CBOMVERSIONID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbbomversioncode", dt.Rows[0]["VBOMVERSION"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmainmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cbmainbomid", dt.Rows[0]["CBOMVERSIONID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmainbomcode", dt.Rows[0]["VBOMVERSION"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbpackbomid", "");
                            CreateNode(xmlDoc, itemNode, "cbmainmaterialid", "");
                            CreateNode(xmlDoc, itemNode, "cbmainmaterialvid", "");
                            CreateNode(xmlDoc, itemNode, "cbdeptid", dt.Rows[0]["DEPTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbdeptvid", dt.Rows[0]["DEPTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "tbstarttime", startTime);
                            CreateNode(xmlDoc, itemNode, "tbendtime", endTime);
                            CreateNode(xmlDoc, itemNode, "vbbatchid", "");
                            CreateNode(xmlDoc, itemNode, "vbbatchcode", item["PACKAGE_NO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbinbatchid", "");
                            CreateNode(xmlDoc, itemNode, "vbinbatchcode", "");
                            CreateNode(xmlDoc, itemNode, "fbsourcetype", "2");
                            CreateNode(xmlDoc, itemNode, "cbunitid", dt_Unit.Rows[0]["MEASCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbastunitid", dt_Unit.Rows[0]["ASTMEASCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbchangerate", "1/1");
                            CreateNode(xmlDoc, itemNode, "nbplanwrnum", "");//dt.Rows[0]["NPLANPUTNUM"].ToString()
                            CreateNode(xmlDoc, itemNode, "nbplanwrastnum", "");// dt.Rows[0]["NPLANPUTASTNUM"].ToString()
                            CreateNode(xmlDoc, itemNode, "nbwrnum", item["Qty"].ToString());//
                            CreateNode(xmlDoc, itemNode, "nbwrastnum", "1");// + (Convert.ToDecimal(item["Qty"])).ToString("f3"));
                            CreateNode(xmlDoc, itemNode, "nbsldchecknum", "0");
                            CreateNode(xmlDoc, itemNode, "nbsldcheckastnum", "0");
                            CreateNode(xmlDoc, itemNode, "nbchecknum", "0");
                            CreateNode(xmlDoc, itemNode, "nbcheckastnum", "0");
                            CreateNode(xmlDoc, itemNode, "bbhasbckfled", "N");
                            CreateNode(xmlDoc, itemNode, "bbhaspicked", "N");
                            CreateNode(xmlDoc, itemNode, "bbstockbycheck", "N");
                            CreateNode(xmlDoc, itemNode, "bbisempass", "N");
                            CreateNode(xmlDoc, itemNode, "bbchkflag", "N");
                            CreateNode(xmlDoc, itemNode, "bbinstock", "N");
                            CreateNode(xmlDoc, itemNode, "bbotherreject", "N");
                            CreateNode(xmlDoc, itemNode, "bbsetmark", "N");
                            CreateNode(xmlDoc, itemNode, "cbempass_bid", "");
                            CreateNode(xmlDoc, itemNode, "vbsalebillcode", "");//dt.Rows[0]["VSALEBILLCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbsalebillid", "");//dt.Rows[0]["VSRCID"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbsrctranstype", dt.Rows[0]["VTRANTYPECODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbsrctranstype", dt.Rows[0]["VTRANTYPEID"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbsrctype", "55C2");
                            CreateNode(xmlDoc, itemNode, "vbsrccode", dt.Rows[0]["VBILLCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbsrcid", dt.Rows[0]["PK_DMO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbsrcrowid", "");
                            CreateNode(xmlDoc, itemNode, "vbsrcrowno", "");
                            CreateNode(xmlDoc, itemNode, "vbfirstranstype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VTRANTYPECODE"].ToString() : dt.Rows[0]["VFIRSTTRANTYPECODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbfirstranstype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VTRANTYPEID"].ToString() : dt.Rows[0]["VFIRSTTRANTYPEID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirsttype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? "55C2" : dt.Rows[0]["VFIRSTTYPE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstcode", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VBILLCODE"].ToString() : dt.Rows[0]["VFIRSTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstid", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["PK_DMO"].ToString() : dt.Rows[0]["VFIRSTID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstrowid", "");//dt.Rows[0]["VFIRSTBID"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbfirstrowno", "");// dt.Rows[0]["VFIRSTCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbnote", "");
                            CreateNode(xmlDoc, itemNode, "tbsourcebillts", "");
                            CreateNode(xmlDoc, itemNode, "bbhasfbill", "N");
                            CreateNode(xmlDoc, itemNode, "cbprojectid", "");
                            CreateNode(xmlDoc, itemNode, "vbfree1", "260");//功率PM_NAME
                            CreateNode(xmlDoc, itemNode, "vbfree2", "03");//电流PS_SUB
                            CreateNode(xmlDoc, itemNode, "vbfree3", "01");//等级GRADE
                            CreateNode(xmlDoc, itemNode, "vbfree4", "");
                            CreateNode(xmlDoc, itemNode, "vbfree5", "");
                            CreateNode(xmlDoc, itemNode, "vbfree6", "");
                            CreateNode(xmlDoc, itemNode, "vbmainidentify", "");
                            CreateNode(xmlDoc, itemNode, "vbidentify", "");
                            CreateNode(xmlDoc, itemNode, "vbparentmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cwr_productid", "");
                        }
                        else
                        {
                            CreateNode(xmlDoc, itemNode, "pk_wr_product", "");
                            CreateNode(xmlDoc, itemNode, "pk_group", ERPGroupCode);
                            CreateNode(xmlDoc, itemNode, "pk_org", ERPOrg);
                            CreateNode(xmlDoc, itemNode, "pk_org_v", ERPOrg);
                            CreateNode(xmlDoc, itemNode, "vbrowno", (i++).ToString());
                            CreateNode(xmlDoc, itemNode, "cbmoid", dt.Rows[0]["PK_DMO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmobillcode", dt.Rows[0]["VBILLCODE"].ToString());//woReport.OrderNumber
                            CreateNode(xmlDoc, itemNode, "cbmobid", "");//空
                            CreateNode(xmlDoc, itemNode, "vbmorowno", "");
                            CreateNode(xmlDoc, itemNode, "vbmoparentbillcode", "");//, dt.Rows[0]["VPARENTMOCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "cbfirstmoid", dt.Rows[0]["PK_DMO"].ToString());// 
                            //CreateNode(xmlDoc, itemNode, "vbfirstmocode", woReport.OrderNumber);
                            CreateNode(xmlDoc, itemNode, "vbfirstmocode", dt.Rows[0]["VBILLCODE"].ToString());

                            CreateNode(xmlDoc, itemNode, "cbfirstmobid", "");
                            CreateNode(xmlDoc, itemNode, "vbfirstmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cbsrcmoid", "");// dt.Rows[0]["VORIGMOID"].ToString()

                            //CreateNode(xmlDoc, itemNode, "vbsrcmocode", woReport.OrderNumber);
                            CreateNode(xmlDoc, itemNode, "vbsrcmocode", dt.Rows[0]["VBILLCODE"].ToString());

                            CreateNode(xmlDoc, itemNode, "cbsrcmobid", "");
                            CreateNode(xmlDoc, itemNode, "vbsrcmorowno", "");//空
                            CreateNode(xmlDoc, itemNode, "fbproducttype", "1");
                            CreateNode(xmlDoc, itemNode, "cbmaterialid", item["MATERIAL_CODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbmaterialvid", item["MATERIAL_CODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbbomversionid", dt.Rows[0]["CBOMVERSIONID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbbomversioncode", dt.Rows[0]["VBOMVERSION"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmainmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cbmainbomid", dt.Rows[0]["CBOMVERSIONID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbmainbomcode", dt.Rows[0]["VBOMVERSION"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbpackbomid", "");
                            CreateNode(xmlDoc, itemNode, "cbmainmaterialid", "");
                            CreateNode(xmlDoc, itemNode, "cbmainmaterialvid", "");
                            CreateNode(xmlDoc, itemNode, "cbdeptid", dt.Rows[0]["DEPTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbdeptvid", dt.Rows[0]["DEPTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "tbstarttime", startTime);
                            CreateNode(xmlDoc, itemNode, "tbendtime", endTime);
                            CreateNode(xmlDoc, itemNode, "vbbatchid", "");
                            CreateNode(xmlDoc, itemNode, "vbbatchcode", item["PACKAGE_NO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbinbatchid", "");
                            CreateNode(xmlDoc, itemNode, "vbinbatchcode", "");
                            CreateNode(xmlDoc, itemNode, "fbsourcetype", "2");
                            CreateNode(xmlDoc, itemNode, "cbunitid", dt_Unit.Rows[0]["MEASCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbastunitid", dt_Unit.Rows[0]["ASTMEASCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbchangerate", "1/" + item["SPM_VALUE"].ToString());
                            CreateNode(xmlDoc, itemNode, "nbplanwrnum", "");//dt.Rows[0]["NPLANPUTNUM"].ToString()
                            CreateNode(xmlDoc, itemNode, "nbplanwrastnum", "");// dt.Rows[0]["NPLANPUTASTNUM"].ToString()
                            CreateNode(xmlDoc, itemNode, "nbwrnum", item["Qty"].ToString());//
                            CreateNode(xmlDoc, itemNode, "nbwrastnum", (Convert.ToDecimal(item["SPM_VALUE"].ToString()) * Convert.ToDecimal(item["Qty"])).ToString("f3"));
                            CreateNode(xmlDoc, itemNode, "nbsldchecknum", "0");
                            CreateNode(xmlDoc, itemNode, "nbsldcheckastnum", "0");
                            CreateNode(xmlDoc, itemNode, "nbchecknum", "0");
                            CreateNode(xmlDoc, itemNode, "nbcheckastnum", "0");
                            CreateNode(xmlDoc, itemNode, "bbhasbckfled", "N");
                            CreateNode(xmlDoc, itemNode, "bbhaspicked", "N");
                            CreateNode(xmlDoc, itemNode, "bbstockbycheck", "N");
                            CreateNode(xmlDoc, itemNode, "bbisempass", "N");
                            CreateNode(xmlDoc, itemNode, "bbchkflag", "N");
                            CreateNode(xmlDoc, itemNode, "bbinstock", "N");
                            CreateNode(xmlDoc, itemNode, "bbotherreject", "N");
                            CreateNode(xmlDoc, itemNode, "bbsetmark", "N");
                            CreateNode(xmlDoc, itemNode, "cbempass_bid", "");
                            CreateNode(xmlDoc, itemNode, "vbsalebillcode", "");//dt.Rows[0]["VSALEBILLCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbsalebillid", "");//dt.Rows[0]["VSRCID"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbsrctranstype", dt.Rows[0]["VTRANTYPECODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbsrctranstype", dt.Rows[0]["VTRANTYPEID"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbsrctype", "55C2");
                            CreateNode(xmlDoc, itemNode, "vbsrccode", dt.Rows[0]["VBILLCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbsrcid", dt.Rows[0]["PK_DMO"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbsrcrowid", "");
                            CreateNode(xmlDoc, itemNode, "vbsrcrowno", "");
                            CreateNode(xmlDoc, itemNode, "vbfirstranstype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VTRANTYPECODE"].ToString() : dt.Rows[0]["VFIRSTTRANTYPECODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "cbfirstranstype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VTRANTYPEID"].ToString() : dt.Rows[0]["VFIRSTTRANTYPEID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirsttype", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? "55C2" : dt.Rows[0]["VFIRSTTYPE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstcode", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["VBILLCODE"].ToString() : dt.Rows[0]["VFIRSTCODE"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstid", string.IsNullOrEmpty(dt.Rows[0]["VFIRSTCODE"].ToString()) ? dt.Rows[0]["PK_DMO"].ToString() : dt.Rows[0]["VFIRSTID"].ToString());
                            CreateNode(xmlDoc, itemNode, "vbfirstrowid", "");//dt.Rows[0]["VFIRSTBID"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbfirstrowno", "");// dt.Rows[0]["VFIRSTCODE"].ToString()
                            CreateNode(xmlDoc, itemNode, "vbnote", "");
                            CreateNode(xmlDoc, itemNode, "tbsourcebillts", "");
                            CreateNode(xmlDoc, itemNode, "bbhasfbill", "N");
                            CreateNode(xmlDoc, itemNode, "cbprojectid", "");
                            CreateNode(xmlDoc, itemNode, "vbfree1", GetCodeByName(item["PM_NAME"].ToString(), "JN0001"));//功率PM_NAME
                            CreateNode(xmlDoc, itemNode, "vbfree2", GetCodeByName(item["PS_SUBCODE_Name"].ToString(), "JN0002"));//电流PS_SUB
                            CreateNode(xmlDoc, itemNode, "vbfree3", GetCodeByName(item["GRADE"].ToString(), "JN0003"));//等级GRADE
                            CreateNode(xmlDoc, itemNode, "vbfree4", "");
                            CreateNode(xmlDoc, itemNode, "vbfree5", "");
                            CreateNode(xmlDoc, itemNode, "vbfree6", "");
                            CreateNode(xmlDoc, itemNode, "vbmainidentify", "");
                            CreateNode(xmlDoc, itemNode, "vbidentify", "");
                            CreateNode(xmlDoc, itemNode, "vbparentmorowno", "");
                            CreateNode(xmlDoc, itemNode, "cwr_productid", "");
                        }
                    }
                    #endregion
                }


                CreateNode(xmlDoc, billheadNode, "pk_group", ERPGroupCode);
                 CreateNode(xmlDoc, billheadNode, "pk_org", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "pk_org_v", ERPOrg);
                CreateNode(xmlDoc, billheadNode, "vbillcode", "");
                CreateNode(xmlDoc, billheadNode, "fbillstatus", "1");

                CreateNode(xmlDoc, billheadNode, "vtrantypeid", dsData1.Tables[0].Rows[0]["ctrantypeid2"].ToString());//报告类型为空dt.Rows[0]["VTRANTYPEID"].ToString()，报工单交易类型PK
                CreateNode(xmlDoc, billheadNode, "vtrantypecode", dsData1.Tables[0].Rows[0]["transtype2"].ToString());//报告类型为空，报工单交易类型
                CreateNode(xmlDoc, billheadNode, "fprodmode", "2");
                CreateNode(xmlDoc, billheadNode, "cdeptid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "cdeptvid", dt.Rows[0]["DEPTCODE"].ToString());
                CreateNode(xmlDoc, billheadNode, "dbilldate", woReport.BillDate.ToString());
                CreateNode(xmlDoc, billheadNode, "vnote", woReport.Note);
                CreateNode(xmlDoc, billheadNode, "billmaker", woReport.BillMaker);
                CreateNode(xmlDoc, billheadNode, "dmakedate", woReport.BillMakedDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                CreateNode(xmlDoc, billheadNode, "creator", woReport.Creator);
                CreateNode(xmlDoc, billheadNode, "creationtime", woReport.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                CreateNode(xmlDoc, billheadNode, "cwrid", "");

                string path = Server.MapPath("~/XMLFile/");
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                path = path + woReport.Key + ".xml";
                xmlDoc.Save(path);

                FileStream ms = new FileStream(path, FileMode.Open, FileAccess.Read);

                //string url = System.Configuration.ConfigurationManager.AppSettings["HttpWebRequestUrl"];
                HttpWebRequest loHttp = (HttpWebRequest)WebRequest.Create(url);  //URL为XChangeServlet地址

                loHttp.Method = "POST";
                // *** Set any header related and operational properties
                loHttp.Timeout = 10000;  // 10 secs
                loHttp.UserAgent = "Code Sample Web Client";

                // *** reuse cookies if available
                loHttp.CookieContainer = new CookieContainer();

                if (this.soCookies != null && this.soCookies.Count > 0)
                {
                    loHttp.CookieContainer.Add(this.soCookies);
                }

                #region send Xml To NCS
                LogHelper.WriteLogError("Begin Send Xml File");
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
                    if (this.soCookies == null)
                    {
                        this.soCookies = loWebResponse.Cookies;
                    }
                    else
                    {
                        // ** If we already have cookies update the list
                        foreach (Cookie oRespCookie in loWebResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.soCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Name;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.soCookies.Add(oRespCookie);
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

                LogHelper.WriteLogError("End Send Xml File");
                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(ResponseText);

                //获取ERP回执
                XmlNode xnode = Doc.SelectSingleNode("ufinterface/sendresult/content");
                returnCode = xnode.InnerText;

                //获取ERP错误信息提示
                if (returnCode == "")
                {
                    XmlNode errornode = Doc.SelectSingleNode("ufinterface/sendresult/resultdescription");
                    msg = errornode.InnerText;
                }

                loResponseStream.Close();
                loWebResponse.Close();
                ms.Close();
                requestStream.Close();
                #endregion
            }
            else
            {
                msg = string.Format(StringResource.ERPWorkOrderQuery_Error_Query, woReport.OrderNumber);
            }
            return returnCode;
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
                if (ds != null && ds.Data != null && ds.Data.Tables.Count > 0)
                {
                    dt = ds.Data.Tables[0];
                }
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
                MethodReturnResult<DataSet> ds = client.GetCodeByName(Name,ListCode);
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

        private CookieCollection _soCookies = null;
        protected CookieCollection soCookies
        {
            get
            {
                return _soCookies;
            }
            set
            {
                _soCookies = value;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> sPagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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
                        MethodReturnResult<IList<WOReportDetail>> result = client.GetWOReportDetail(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_sListPartial");
        }
        
    }
}
