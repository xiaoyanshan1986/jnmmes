using ServiceCenter.Client.Mvc.Areas.ERP.Models;
using ServiceCenter.Client.Mvc.Resources.ERP;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.ERP;
using ServiceCenter.MES.Service.Contract.ERP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.ERP.Controllers
{
    public class ERPWorkOrderController : Controller
    {
        /// <summary> 初始化 </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary> ERP工单信息查询 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Query(WorkOrderViewModel model)
        {
            MethodReturnResult ReturnResult = new MethodReturnResult();

            using (ERPClient client = new ERPClient())
            {
                MethodReturnResult<DataSet> result = client.GetERPWorkOrder(model.OrderNumber);

                if (result.Code == 0)
                {
                    if (result.Data.Tables[0].Rows.Count <= 0)
                    {
                        ReturnResult.Code = 1001;
                        ReturnResult.Message = string.Format(StringResource.ERPWorkOrderQuery_Error_Query, model.OrderNumber);
                    }
                }
                else
                {
                    ReturnResult.Code = result.Code;
                    ReturnResult.Message = result.Message;
                }
            }

            return Json(ReturnResult);
        }

        /// <summary> 取得ERP工单信息 </summary>
        /// <param name="OrderNumber"></param>
        /// <returns></returns>
        public async Task<ActionResult> Detail(string OrderNumber)
        {
            MethodReturnResult<ERPWorkOrderController> result = new MethodReturnResult<ERPWorkOrderController>();

            WorkOrderViewModel model = new WorkOrderViewModel();

            try
            {
                string strWOState = "";
                string strERPOrderType = "";
                string strOrderType = "";
                string strERPDept = "";
                string strLocationName = "";
                MethodReturnResult<DataSet> resultWO = new MethodReturnResult<DataSet>();

                using (ERPClient client = new ERPClient())
                {                    
                    //取得ERP工单信息
                    await Task.Run(() =>
                        {
                            resultWO = client.GetERPWorkOrder(OrderNumber);
                        });
                                        
                    if (resultWO.Code == 0 && resultWO.Data.Tables[0].Rows.Count > 0)
                    {
                        //根据ERP工单状态取得MES工单状态
                        strWOState = resultWO.Data.Tables[0].Rows[0]["FBILLSTATE"].ToString();                                  //ERP工单状态

                        //订单需要是投放状态（自由 - -1、审批 - 1、投放 - 2、完工 - 3、关闭 - 4）
                        if (strWOState != "2")
                        {
                            result.Code = 1002;
                            result.Message = "订单需要为投放状态！";
                            result.Detail = "";
                            
                        }

                        //根据ERP工单类型取得MES工单类型
                        strERPOrderType = resultWO.Data.Tables[0].Rows[0]["VTRANTYPECODE"].ToString();                          //ERP工单类型

                        strOrderType = model.GetMESOrderType(strERPOrderType);

                        //根据ERP部门代码取得MES对应的工厂代码
                        strERPDept = resultWO.Data.Tables[0].Rows[0]["CJCODE"].ToString();

                        strLocationName = model.GetLocationName(strERPDept);
                        
                        model.OrderNumber = resultWO.Data.Tables[0].Rows[0]["VBILLCODE"].ToString();                            //工单号
                        model.OrderType = strOrderType;                                                                         //工单类型
                        model.MaterialCode = resultWO.Data.Tables[0].Rows[0]["MATERIALCODE"].ToString();                        //物料代码
                        model.ERPWorkOrderKey = resultWO.Data.Tables[0].Rows[0]["PK_DMO"].ToString();                           //ERP工单号主键
                        model.OrderQuantity = Convert.ToDouble(resultWO.Data.Tables[0].Rows[0]["NNUM"].ToString());             //数量
                        model.LocationName = strLocationName;                                                                   //生产车间
                        model.PlanStartTime = DateTime.Parse(resultWO.Data.Tables[0].Rows[0]["TPLANSTARTTIME"].ToString());     //计划开始日期
                        model.PlanFinishTime = DateTime.Parse(resultWO.Data.Tables[0].Rows[0]["TPLANENDTIME"].ToString());      //计划结束日期
                        model.Description = resultWO.Data.Tables[0].Rows[0]["VNOTE"].ToString();                                //备注
                    }

                    ViewBag.WorkOrder = model;
                }
            }
            catch (Exception e)
            {
                result.Code = 1002;
                result.Message = e.Message;
                result.Detail = e.ToString();

                return Json(result);
            }

            return View(model);

            //MethodReturnResult<ERPWorkOrderController> result = new MethodReturnResult<ERPWorkOrderController>();

            //WorkOrderViewModel model = new WorkOrderViewModel();

            //try
            //{
            //    string strWOState = "";

            //    using (ERPClient client = new ERPClient())
            //    {
            //        await Task.Run(() =>
            //        {
            //            //取得ERP工单信息
            //            MethodReturnResult<DataSet> resultWO = client.GetERPWorkOrder(OrderNumber);

            //            if (resultWO.Code == 0 && resultWO.Data.Tables[0].Rows.Count > 0)
            //            {
            //                model.OrderNumber = resultWO.Data.Tables[0].Rows[0]["VBILLCODE"].ToString();                            //工单号
            //                model.MaterialCode = resultWO.Data.Tables[0].Rows[0]["MATERIALCODE"].ToString();                        //物料代码

            //                //根据ERP工单状态取得MES工单状态
            //                strWOState = resultWO.Data.Tables[0].Rows[0]["FBILLSTATUS"].ToString();                                 //工单状态

            //                //订单需要是投放状态（自由 - -1、审批 - 1、投放 - 2、完工 - 3、关闭 - 4）
            //                if (strWOState != "2")
            //                {
            //                    result.Code = 1002;
            //                    result.Message = "订单需要为投放状态！";
            //                    result.Detail = "";

            //                    //return Json(result);
            //                }
            //                //model.OrderState = resultWO.Data.Tables[0].Rows[0]["FBILLSTATUS"].ToString();                         //工单状态


            //                model.OrderType = resultWO.Data.Tables[0].Rows[0]["MATERIALTYPE"].ToString();                         //工单类型

            //                model.OrderQuantity = Convert.ToDouble(resultWO.Data.Tables[0].Rows[0]["NNUM"].ToString());           //数量
            //                model.LocationName = resultWO.Data.Tables[0].Rows[0]["CJNAME"].ToString();                            //生产车间
            //                model.PlanStartTime = DateTime.Parse(resultWO.Data.Tables[0].Rows[0]["TPLANSTARTTIME"].ToString());   //计划开始日期
            //                model.PlanFinishTime = DateTime.Parse(resultWO.Data.Tables[0].Rows[0]["TPLANENDTIME"].ToString());    //计划结束日期
            //            }
            //        });

            //        ViewBag.WorkOrder = model;
            //    }
            //}
            //catch (Exception e)
            //{
            //    result.Code = 1002;
            //    result.Message = e.Message;
            //    result.Detail = e.ToString();

            //    return Json(result);
            //}

            //return View(model);
        }

        /// <summary> 取得ERP工单BOM信息 </summary>
        /// <param name="OrderNumber"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetERPWorkOrderBOM(string OrderNumber)
        {
            using (ERPClient client = new ERPClient())
            {
                List<WorkOrderBOMViewModel> list = new List<WorkOrderBOMViewModel>();

                await Task.Run(() =>
                {
                    MethodReturnResult<DataSet> result = client.GetERPWorkOrderBOM(OrderNumber);

                    if (result.Code == 0 && result.Data.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < result.Data.Tables[0].Rows.Count; i++)
                        {
                            WorkOrderBOMViewModel workOrderBOMModel = new WorkOrderBOMViewModel();
                            workOrderBOMModel.RowNo = Int32.Parse(result.Data.Tables[0].Rows[i]["行号"].ToString());                //行号
                            workOrderBOMModel.ItemNo = Int32.Parse(result.Data.Tables[0].Rows[i]["序号"].ToString());               //序号
                            workOrderBOMModel.MaterialCode = result.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString();              //物料代码
                            workOrderBOMModel.MaterialName = result.Data.Tables[0].Rows[i]["MATERIALNAME"].ToString();              //物料名称
                            workOrderBOMModel.Qty = Convert.ToDecimal(result.Data.Tables[0].Rows[i]["NUNITNUM"].ToString());        //消耗量
                            workOrderBOMModel.MaterialUnit = result.Data.Tables[0].Rows[i]["MEAS"].ToString();                      //计量单位
                            workOrderBOMModel.MinUnit = Convert.ToDecimal(result.Data.Tables[0].Rows[i]["MINUNIT"].ToString());     //最小扣料单位
                            //workOrderBOMModel.ReplaceMaterial = result.Data.Tables[0].Rows[i]["MATERIALCODE"].ToString();         //可替换料

                            list.Add(workOrderBOMModel);
                        }
                    }

                    ViewBag.BOMList = list;
                });
            }

            return PartialView("_BOMListPartial");
        }

        /// <summary>
        /// 导入ERP工单信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(WorkOrderViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            using (ERPClient client = new ERPClient())
            {
                ERPWorkOrderParameter param = new ERPWorkOrderParameter();

                param.Creator = User.Identity.Name;         //操作员
                param.OrderNumber = model.OrderNumber;      //工单号

                result = client.AddERPWorkOrder(param);

                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.ERPWorkOrder_Add_Success);
                }
            }
            return Json(result);
        }

        /// <summary>
        /// 更新工单BOM
        /// </summary>
        /// <param name="OrderNumber">工单号</param>
        /// <param name="Priority"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        //public ActionResult SaveBOM(string OrderNumber, EnumWorkOrderPriority Priority, string Description)
        public ActionResult SaveBOM(WorkOrderViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            using (ERPClient client = new ERPClient())
            {
                ERPWorkOrderParameter param = new ERPWorkOrderParameter() 
                {
                    OrderNumber = model.OrderNumber,    //工单号    
                    Creator = User.Identity.Name        //创建人
                };

                //调用BOM更新服务
                result = client.UpdateBaseInfo(param);

                if (result.Code == 0)
                {
                    result.Message = string.Format("ERP工单{0}BOM更新成功!", model.OrderNumber);
                }
            }

            return Json(result);
        }

    }
}