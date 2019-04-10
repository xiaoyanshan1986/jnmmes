using System;
using System.Net;
using WinClient.Socket.ReaderServer.Configuration;
using System.Collections.Generic;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES;
using ServiceCenter.Model;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.Client.WinService.ImageDataTransfer;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.Service.Client;
using System.IO;
using System.Text;



namespace WinClient.Socket.ReaderServer
{



    /// <summary>
    /// IV测试数据转到MES数据库中。
    /// </summary>
    public class LotReaderAction
    {
        public EventHandler<LotReaderFinishedArgs> OnLotReaderFinished;//声明自定义的事件委托，用来执行事件的声明，
        private System.DateTime dMaxUploadTestTime;
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="accessConnString">Access数据库连接字符串。</param>
        public LotReaderAction()
        {

        }
        /// <summary>
        /// Access数据库连接字符串。
        /// </summary>
        private string AccessConnectionString
        {
            get;
            set;
        }
        /// <summary>
        /// 上一次执行转置的数据数量。
        /// </summary>
        public int ReceiveLotCount
        {
            get;
            private set;
        }

        public System.DateTime MaxUploadTestTime
        {
            get
            {
                return dMaxUploadTestTime;
            }
            private set
            {
                dMaxUploadTestTime = value;
            }
        }
        public void Execute(LotReaderDeviceElement lotinfo)
        {
            LotReaderFinishedArgs Args = new LotReaderFinishedArgs();
            try
            {

                if (lotinfo.LotNumber.Length < 3)
                {
                    Args.TransferMsg = string.Format("未扫到码，车间{0}线别{1}"
                                                        , lotinfo.WorkShop
                                                        , lotinfo.LineCode);


                    Args.TransferMsg = Args.TransferMsg + "-------------";
                    ErrorLog(null, Args.TransferMsg);
                    if (OnLotReaderFinished != null)
                    {
                        CommonFun.eventInvoket(() => { OnLotReaderFinished(this, Args); });
                    }
                }
                else
                {
                    //根据条码获取批次信息
                    Lot lot = null;
                    MethodReturnResult resultLot = new MethodReturnResult();
                    try
                    {
                        string lotNumber = lotinfo.LotNumber.ToUpper();
                        lot = GetLot(lotNumber);
                        if (lot == null)
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        resultLot.Code = 1000;
                        resultLot.Message = ex.Message;
                        resultLot.Detail = ex.ToString();
                        ErrorLog(ex, resultLot.Message);
                    }





                    //判断进出站状态、设备及工序
                    TrackInParameter pIn = new TrackInParameter()
                    {
                        Creator = "system",
                        RouteOperationName = "",
                        LineCode = lotinfo.LineCode,
                        LotNumbers = new List<string>(),
                        OperateComputer = lotinfo.ReaderIP,
                        Operator = "system",
                        EquipmentCode = ""
                    };
                    pIn.LotNumbers.Add(lotinfo.LotNumber);
                    TrackOutParameter pOut = new TrackOutParameter()
                    {
                        Creator = "system",
                        RouteOperationName = "",
                        LineCode = lotinfo.LineCode,
                        LotNumbers = new List<string>(),
                        OperateComputer = lotinfo.ReaderIP,
                        Operator = "system",
                        EquipmentCode = ""
                    };
                    pOut.LotNumbers.Add(lotinfo.LotNumber);
                    //工序，状态
                    if (lot.RouteStepName == lotinfo.FirstStepCode)
                    {
                        if (lot.StateFlag == EnumLotState.WaitTrackIn)
                        {
                            //第一站进站赋值进站属性
                            pIn.RouteOperationName = lotinfo.FirstStepCode;
                            pIn.EquipmentCode = lotinfo.FirstEquipmentCode;
                            if (stepIn(lotinfo, lot, Args, pIn))//第一站进站 
                            {
                                //第一站出站赋值出站属性
                                pOut.RouteOperationName = lotinfo.FirstStepCode;
                                pOut.EquipmentCode = lotinfo.FirstEquipmentCode;
                                lot.StateFlag = EnumLotState.WaitTrackOut;
                                if (stepOut(lotinfo, lot, Args, pOut))//第一站出站
                                {
                                    //第二站进站赋值进站属性
                                    pIn.RouteOperationName = lotinfo.SecondStepCode;
                                    pIn.EquipmentCode = lotinfo.SecondEquipmentCode;

                                    lot.StateFlag = EnumLotState.WaitTrackIn;
                                    if (stepIn(lotinfo, lot, Args, pIn)) //第二站进站
                                    {
                                        //第二站出站赋值进站属性
                                        pOut.RouteOperationName = lotinfo.SecondStepCode;
                                        pOut.EquipmentCode = lotinfo.SecondEquipmentCode;

                                        lot.RouteStepName = lotinfo.SecondStepCode;
                                        lot.EquipmentCode = lotinfo.SecondEquipmentCode;
                                        lot.StateFlag = EnumLotState.WaitTrackOut;
                                        if (stepOut(lotinfo, lot, Args, pOut))//第二站出站
                                        {
                                            //启线
                                            //m_retFlag = m_soap.SetLineState(m_workShopId, m_flowId, m_flowSubId, 1);
                                            //StartLine(lotinfo.WorkShopId, lotinfo.FlowId, lotinfo.FlowSubId, lotinfo.LineCode, lotinfo.SecondEquipmentCode);
                                        }
                                    }
                                }
                            }
                        }
                        else if (lot.StateFlag == EnumLotState.WaitTrackOut)
                        {
                            //第一站出站赋值出站属性
                            pOut.RouteOperationName = lotinfo.FirstStepCode;
                            pOut.EquipmentCode = lotinfo.FirstEquipmentCode;

                            lot.StateFlag = EnumLotState.WaitTrackOut;
                            if (stepOut(lotinfo, lot, Args, pOut))//第一站出站
                            {
                                //第二站进站赋值进站属性
                                pIn.RouteOperationName = lotinfo.SecondStepCode;
                                pIn.EquipmentCode = lotinfo.SecondEquipmentCode;

                                lot.StateFlag = EnumLotState.WaitTrackIn;
                                if (stepIn(lotinfo, lot, Args, pIn)) //第二站进站
                                {
                                    //第二站出站赋值进站属性
                                    pOut.RouteOperationName = lotinfo.SecondStepCode;
                                    pOut.EquipmentCode = lotinfo.SecondEquipmentCode;

                                    lot.RouteStepName = lotinfo.SecondStepCode;
                                    lot.EquipmentCode = lotinfo.SecondEquipmentCode;
                                    lot.StateFlag = EnumLotState.WaitTrackOut;
                                    if (stepOut(lotinfo, lot, Args, pOut))//第二站出站
                                    {
                                        //启线
                                        //m_retFlag = m_soap.SetLineState(m_workShopId, m_flowId, m_flowSubId, 1);
                                        //StartLine(lotinfo.WorkShopId, lotinfo.FlowId, lotinfo.FlowSubId, lotinfo.LineCode, lotinfo.SecondEquipmentCode);
                                    }
                                }
                            }
                        }
                    }
                    else if (lot.RouteStepName == lotinfo.SecondStepCode)
                    {
                        if (lot.StateFlag == EnumLotState.WaitTrackIn)
                        {
                            //第二站进站赋值进站属性
                            pIn.RouteOperationName = lotinfo.SecondStepCode;
                            pIn.EquipmentCode = lotinfo.SecondEquipmentCode;

                            lot.StateFlag = EnumLotState.WaitTrackIn;
                            if (stepIn(lotinfo, lot, Args, pIn)) //第二站进站
                            {
                                //第二站出站赋值进站属性
                                pOut.RouteOperationName = lotinfo.SecondStepCode;
                                pOut.EquipmentCode = lotinfo.SecondEquipmentCode;

                                lot.RouteStepName = lotinfo.SecondStepCode;
                                lot.EquipmentCode = lotinfo.SecondEquipmentCode;
                                lot.StateFlag = EnumLotState.WaitTrackOut;
                                if (stepOut(lotinfo, lot, Args, pOut))//第二站出站
                                {
                                    //启线
                                    //m_retFlag = m_soap.SetLineState(m_workShopId, m_flowId, m_flowSubId, 1);
                                    //StartLine(lotinfo.WorkShopId, lotinfo.FlowId, lotinfo.FlowSubId, lotinfo.LineCode, lotinfo.SecondEquipmentCode);
                                }
                            }
                        }
                        if (lot.StateFlag == EnumLotState.WaitTrackOut)
                        {
                            //第二站出站赋值进站属性
                            pOut.RouteOperationName = lotinfo.SecondStepCode;
                            pOut.EquipmentCode = lotinfo.SecondEquipmentCode;

                            lot.RouteStepName = lotinfo.SecondStepCode;
                            lot.EquipmentCode = lotinfo.SecondEquipmentCode;
                            lot.StateFlag = EnumLotState.WaitTrackOut;
                            if (stepOut(lotinfo, lot, Args, pOut))//第二站出站
                            {
                                string msg = null;
                                //启线
                                //m_retFlag = m_soap.SetLineState(m_workShopId, m_flowId, m_flowSubId, 1);
                                //StartLine(lotinfo.WorkShopId, lotinfo.FlowId, lotinfo.FlowSubId, lotinfo.LineCode, lotinfo.SecondEquipmentCode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Args.TransferMsg = Args.TransferMsg + "-------------" + ex;
                ErrorLog(ex, Args.TransferMsg);
            }
        }

        /// <summary>
        /// 记录系统错误到系统错误日志
        /// </summary>
        /// <param name="logfilepath">本地日志文件路径</param>
        /// <param name="e">异常类</param>
        public static void ErrorLog(Exception e, string re)
        {
            string logfilepath = @"C:\MESlog";
            try
            {
                if (!System.IO.Directory.Exists(logfilepath))
                {
                    System.IO.Directory.CreateDirectory(logfilepath);
                }
                logfilepath = logfilepath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + " Log.txt";//
                string str = null;
                if (e != null)
                {
                    str = e.Source + e.Message + e.StackTrace;
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("==========" + DateTime.Now.ToString() + "==========");
                sb.Append("\r\n");
                sb.Append("日志记录如下" + re + ":");
                sb.Append("\r\n");
                sb.Append(str);
                sb.Append("\r\n");
                System.IO.StreamWriter sw = System.IO.File.AppendText(logfilepath);
                sw.WriteLine(sb.ToString());
                sw.Close();
                sw.Dispose();
            }
            catch
            {
            }
        }
        public static void StartLine(string WorkShopId, string FlowId, string FlowSubId, string LineCode, string SecondEquipmentCode)
        {
           Service.TradLevelSoapClient m_soap = new Service.TradLevelSoapClient();
            //区域 102A=1  102B=2
            int m_workShopId = Convert.ToInt32(WorkShopId);
            //流水线 A=1 B=2 C=3
            int m_flowId = Convert.ToInt32(FlowId);
            //每条线的左右两个读头 右边=1  左边=2
            int m_flowSubId = Convert.ToInt32(FlowSubId);
            //产线动作 停线=0 启动=1
            bool m_retFlag = false;
            string msg = null;
            //启线
            m_retFlag = m_soap.SetLineState(m_workShopId, m_flowId, m_flowSubId, 1);
            if (m_retFlag)
            {
                msg = string.Format("线别{0}设备号{1}启动成功", LineCode,SecondEquipmentCode);
            }
            else
            {
                msg = string.Format("线别{0}设备号{1}启动失败", LineCode,SecondEquipmentCode);

            }
            ErrorLog(null, msg);
        
        }

        public bool stepIn(LotReaderDeviceElement lotinfo, Lot lot, LotReaderFinishedArgs Args, TrackInParameter p1)
        {
            bool flag = true;
            //进站
            MethodReturnResult result = null;

            using (WipEngineerServiceClient client = new WipEngineerServiceClient())
            {
                if (lot.StateFlag == EnumLotState.WaitTrackIn)
                {
                    result = client.TrackInLot(p1);
                    if (result.Code == 0)
                    {
                        Args.TransferMsg = string.Format("批次：{0} 进站成功", lotinfo.LotNumber);
                        flag = true;
                    }
                    else
                    {
                        Args.TransferMsg = string.Format("批次：{0} 进站失败", lotinfo.LotNumber);
                        //存储读头扫码结果信息
                        Args.TransferMsg = Args.TransferMsg + "-------------";
                        if (OnLotReaderFinished != null)
                        {
                            CommonFun.eventInvoket(() => { OnLotReaderFinished(this, Args); });
                        }
                        flag = false;
                    }
                }
            }
            ErrorLog(null, Args.TransferMsg);
            return flag;
        }

        public bool stepOut(LotReaderDeviceElement lotinfo, Lot lot, LotReaderFinishedArgs Args, TrackOutParameter p)
        {
            bool flag = true;
            //出站
            MethodReturnResult result = null;

            using (WipEngineerServiceClient client = new WipEngineerServiceClient())
            {
                if (lot.StateFlag == EnumLotState.WaitTrackOut)
                {
                    IDictionary<string, IList<TransactionParameter>> dicParams = new Dictionary<string, IList<TransactionParameter>>();
                    //获取工序参数列表。
                    IList<RouteStepParameter> lstRouteStepParameter = GetParameterList(lot.RouteName, lot.RouteStepName, EnumLotState.WaitTrackOut);

                    if (lstRouteStepParameter != null)
                    {
                        #region 组织批次附加代码
                        foreach (RouteStepParameter item in lstRouteStepParameter)
                        {
                            if (!dicParams.ContainsKey(lot.Key))
                            {
                                dicParams.Add(lot.Key, new List<TransactionParameter>());
                            }
                            string val = null;
                            if (item.Key.ParameterName == "电池片批号" || item.Key.ParameterName == "电池片小包装号")
                            {
                                val = GetCellLotList(item.MaterialType, lot.LineCode, lot.RouteStepName, lot.OrderNumber, p.EquipmentCode);
                            }
                            else
                            {
                                val = GetParameterLotList(item.MaterialType, lot.LineCode, lot.RouteStepName, lot.OrderNumber, p.EquipmentCode);
                            }
                            TransactionParameter tp = new TransactionParameter()
                            {
                                Index = item.ParamIndex,
                                Name = item.Key.ParameterName,
                                Value = val
                            };
                            dicParams[lot.Key].Add(tp);
                        }

                        p.Paramters = dicParams;
                        #endregion
                    }

                    MethodReturnResult resultTrackOut = client.TrackOutLot(p);
                    if (resultTrackOut.Code == 0)
                    {
                        Args.TransferMsg = string.Format("批次：{0} {1}出站成功", lotinfo.LotNumber, p.RouteOperationName);
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                        Args.TransferMsg = string.Format("批次：{0} {1}出站失败 =>  ", lotinfo.LotNumber, p.RouteOperationName) + resultTrackOut.Message;
                    }
                    //存储读头扫码结果信息

                    Args.TransferMsg = Args.TransferMsg + "-------------";
                    if (OnLotReaderFinished != null)
                    {
                        CommonFun.eventInvoket(() => { OnLotReaderFinished(this, Args); });
                    }

                }

            }
            ErrorLog(null, Args.TransferMsg);
            return flag;
        }
        private Lot GetLot(string lotNumber)
        {
            MethodReturnResult<Lot> rst = null;
            Lot obj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                rst = client.Get(lotNumber);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    obj = rst.Data;
                }
            }
            return obj;
        }

        private IList<RouteStepParameter> GetParameterList(string routeName, string routeStepName, EnumLotState stateFlag)
        {
            if (stateFlag != EnumLotState.WaitTrackIn && stateFlag != EnumLotState.WaitTrackOut)
            {
                return null;
            }
            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ParamIndex",
                    Where = string.Format(@"DataFrom='{0}' AND DCType='{1}' AND IsDeleted=0
                                           AND Key.RouteName='{2}'
                                           AND Key.RouteStepName='{3}'"
                                           , Convert.ToInt32(EnumDataFrom.Manual)
                                           , stateFlag == EnumLotState.WaitTrackIn
                                                ? Convert.ToInt32(EnumDataCollectionAction.TrackIn)
                                                : Convert.ToInt32(EnumDataCollectionAction.TrackOut)
                                           , routeName
                                           , routeStepName)
                };
                MethodReturnResult<IList<RouteStepParameter>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    return result.Data;
                }
            }
            return null;
        }


        private string GetParameterLotList(string materialType, string lineCode, string routeStepName, string orderNumber, string equipmentCode)
        {
            string materialLot = null;
            string sql2 = string.Format(@"SELECT TOP 1 t1.MATERIAL_LOT
                                          FROM [dbo].[LSM_MATERIAL_LOADING_DETAIL] t1 
                                              INNER JOIN dbo.LSM_MATERIAL_LOADING t2  ON t2.LOADING_KEY=t1.LOADING_KEY
                                              INNER JOIN [dbo].[FMM_MATERIAL] t3 ON t3.MATERIAL_CODE=t1.MATERILA_CODE
                                          WHERE t3.MATERIAL_TYPE = '{0}' AND t2.LINE_CODE='{1}'
		                                             AND t2.ROUTE_OPERATION_NAME='{2}' AND t1.ORDER_NUMBER='{3}'
		                                             AND t2.EQUIPMENT_CODE='{4}' AND t1.CURRENT_QTY>0
										  ORDER BY t1.EDIT_TIME DESC,t1.ITEM_NO ASC"
                                         , materialType
                                         , lineCode
                                         , routeStepName
                                         , orderNumber
                                         , equipmentCode
                                         );
            DataTable dt2 = new DataTable();
            using (DBServiceClient client = new DBServiceClient())
            {
                MethodReturnResult<DataTable> dtResult = client.ExecuteQuery(sql2);
                if (dtResult.Code <= 0 && dtResult.Data.Rows.Count > 0)
                {
                    dt2 = dtResult.Data;
                    materialLot = dt2.Rows[0][0].ToString();
                }
            }
            return materialLot;
        }

        private string GetCellLotList(string materialType, string lineCode, string routeStepName, string orderNumber, string equipmentCode)
        {
            string materialLot = null;
            string sql2 = string.Format(@"SELECT TOP 1 t1.MATERIAL_LOT
                                          FROM [dbo].[LSM_MATERIAL_LOADING_DETAIL] t1 
                                              INNER JOIN dbo.LSM_MATERIAL_LOADING t2  ON t2.LOADING_KEY=t1.LOADING_KEY
                                              INNER JOIN [dbo].[FMM_MATERIAL] t3 ON t3.MATERIAL_CODE=t1.MATERILA_CODE
                                          WHERE t3.MATERIAL_TYPE like '{0}%' AND t2.LINE_CODE='{1}'
		                                             AND t2.ROUTE_OPERATION_NAME='{2}' AND t1.ORDER_NUMBER='{3}'
		                                             AND t2.EQUIPMENT_CODE='{4}' AND t1.CURRENT_QTY>0
                                          ORDER BY t1.EDIT_TIME DESC,t1.ITEM_NO ASC"
                                         , materialType
                                         , lineCode
                                         , routeStepName
                                         , orderNumber
                                         , equipmentCode
                                         );
            DataTable dt2 = new DataTable();
            using (DBServiceClient client = new DBServiceClient())
            {
                MethodReturnResult<DataTable> dtResult = client.ExecuteQuery(sql2);
                if (dtResult.Code <= 0 && dtResult.Data.Rows.Count > 0)
                {
                    dt2 = dtResult.Data;
                    materialLot = dt2.Rows[0][0].ToString();
                }
            }
            return materialLot;
        }

    }


    public class LotReaderInfo
    {
        System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        private string _readerIP = "";
        private string _lotNumber = "";
        private string _strLineCode = "";
        private string _strFirstEquipmentCode = "";
        private string _strFirstStepCode = "";
        private string _strSecondEquipmentCode = "";
        private string _strSecondStepCode = "";
        private string _strWorkShop = "";
        private string _strWorkShopId = "";
        private string _strFlowId = "";
        private string _strFlowSubId = "";

        public LotReaderInfo()
        {

        }
        public string ReaderIP
        {
            get
            {
                return _readerIP;
            }
            set
            {
                _readerIP = value;
            }
        }
        public string LotNumber
        {
            get
            {
                return _lotNumber;
            }
            set
            {
                _lotNumber = value;
            }
        }
        public string LineCode
        {
            get
            {
                return _strLineCode;
            }
            set
            {
                _strLineCode = value;
            }
        }
        public string FirstEquipmentCode
        {
            get
            {
                return _strFirstEquipmentCode;
            }
            set
            {
                _strFirstEquipmentCode = value;
            }
        }
        public string FirstStepCode
        {
            get
            {
                return _strFirstStepCode;
            }
            set
            {
                _strFirstStepCode = value;
            }
        }
        public string SecondEquipmentCode
        {
            get
            {
                return _strSecondEquipmentCode;
            }
            set
            {
                _strSecondEquipmentCode = value;
            }
        }
        public string SecondStepCode
        {
            get
            {
                return _strSecondStepCode;
            }
            set
            {
                _strSecondStepCode = value;
            }
        }
        public string WorkShop
        {
            get
            {
                return _strWorkShop;
            }
            set
            {
                _strWorkShop = value;
            }
        }
        public string WorkShopId
        {
            get
            {
                return _strWorkShopId;
            }
            set
            {
                _strWorkShopId = value;
            }
        }
        public string FlowId
        {
            get
            {
                return _strFlowId;
            }
            set
            {
                _strFlowId = value;
            }
        }
        public string FlowSubId
        {
            get
            {
                return _strFlowSubId;
            }
            set
            {
                _strFlowSubId = value;
            }
        }
    }

    public class LotReaderFinishedArgs : EventArgs
    {
        private string strTransferMsg = "";
        private string strTransferDbFile = "";
        private bool blTransferDataResult = false;
        private DateTime dMaxTestDateTime;


        public LotReaderFinishedArgs()
        {

        }
        public LotReaderFinishedArgs(bool TransferDataResult, string transferMsg, DateTime maxTestDateTime)
        {
            blTransferDataResult = TransferDataResult;
            strTransferMsg = transferMsg;
            dMaxTestDateTime = maxTestDateTime;
        }
        public string TransferMsg
        {
            get
            {
                return strTransferMsg;
            }
            set
            {
                strTransferMsg = value;
            }
        }

        public string TransferDbFile
        {
            get
            {
                return strTransferDbFile;
            }
            set
            {
                strTransferDbFile = value;
            }
        }

        public bool TransferDataResult
        {
            get
            {
                return blTransferDataResult;
            }
            set
            {
                blTransferDataResult = value;
            }
        }

        public DateTime MaxTestDateTime
        {
            get
            {
                return dMaxTestDateTime;
            }
            set
            {
                dMaxTestDateTime = value;
            }
        }

    }

}