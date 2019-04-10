using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.ZPVM.ServiceExtensions
{
    public class JNPackageNoGenerator : IPackageNoGenerate
    {
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine { get; set; }

        /// <summary>
        /// 包装数据访问类。
        /// </summary>
        public IPackageDataEngine PackageDataEngine { get; set; }

        /// <summary>
        /// 工单属性访问类
        /// </summary>
        public IWorkOrderAttributeDataEngine WorkOrderAttributeDataEngine { get; set; }

        /// <summary>
        /// 物料属性数据访问读写。
        /// </summary>
        public IMaterialAttributeDataEngine MaterialAttributeDataEngine { get; set; }

        /// <summary>
        /// 批次测试数据属性访问类
        /// </summary>
        public IIVTestDataDataEngine IVTestDataDataEngine { get; set; }

        /// 工单分档属性访问类
        /// </summary>
        public IWorkOrderPowersetDataEngine WorkOrderPowersetDataEngine
        {
            get;
            set;
        }

        #region 2017-3 -18取消原根据批次创建托号
        //public string Generate(string lotNumber, bool isLastestPackage)
        //{
        //    IList<WorkOrderAttribute> lstWorkOrderAttribute = null;
        //    string prefixPackageNo = string.Empty;
        //    DateTime now = DateTime.Now;
        //    PagingConfig cfg = null;

        //    //取得批次对象
        //    Lot lot = this.LotDataEngine.Get(lotNumber);

        //    if (lot.PackageFlag == false)
        //    {
        //        #region 1.创建托号
        //        //取得工单打印格式代码
        //        cfg = new PagingConfig()
        //        {
        //            Where = string.Format(" Key.OrderNumber = '{0}' and Key.AttributeName = 'CreateCodeFormatNum'",
        //                                    lot.OrderNumber),
        //            OrderBy = "Key.OrderNumber,Key.AttributeName"
        //        };

        //        lstWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(cfg);

        //        if (lstWorkOrderAttribute == null || lstWorkOrderAttribute.Count == 0)
        //        {
        //            #region 默认格式
        //            //托盘编码规则说明
        //            //第1、2位：组件包装年份；
        //            //第3、4位：组件包装月份；
        //            //第5、6位：工单号第7、8位；
        //            //第7、8位：工单号后两位；
        //            //第9、10、11、12位：托盘流水码
        //            //示例：150807010001表示在2015年7月第一个工单车间生产组件，在8月份包装的一拖组件。

        //            if (lot.OrderNumber.Length > 8)
        //            {
        //                DateTime splitTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);

        //                if (now < splitTime)
        //                {
        //                    now = now.AddDays(-1);
        //                }
        //                int year = Convert.ToInt32(now.ToString("yy"));

        //                //工单7,8位
        //                //1MO-15090001
        //                string sub01 = lot.OrderNumber.Substring(6, 2);

        //                //工单后两位
        //                string sub02 = lot.OrderNumber.Substring(lot.OrderNumber.Length - 2, 2);


        //                prefixPackageNo = string.Format("{0}{1}{2}{3}"
        //                                                , year.ToString("00")
        //                                                , now.Month.ToString("00")
        //                                                , sub01
        //                                                , sub02
        //                                                );
        //            }

        //            int seqNo = 1;
        //            cfg = new PagingConfig()
        //            {
        //                PageNo = 0,
        //                PageSize = 1,
        //                Where = string.Format(@"Key LIKE '{0}%'"
        //                                        , prefixPackageNo),
        //                OrderBy = "Key DESC"
        //            };

        //            IList<Package> lstPackage = this.PackageDataEngine.Get(cfg);
        //            if (lstPackage.Count > 0)
        //            {
        //                string maxSeqNo = lstPackage[0].Key.Replace(prefixPackageNo, "");
        //                if (int.TryParse(maxSeqNo, out seqNo))
        //                {
        //                    seqNo = seqNo + 1;
        //                }
        //            }

        //            return string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
        //            #endregion
        //        }
        //        else
        //        {
        //            switch (lstWorkOrderAttribute[0].AttributeValue)
        //            {
        //                case "DF01":
        //                    #region 东方日升格式
        //                    #region 1.颜色
        //                    string Color = "";
        //                    string PS_Code = "";

        //                    switch (lot.Color)
        //                    {
        //                        case "深蓝":
        //                            Color = "1";

        //                            break;
        //                        case "正蓝":
        //                            Color = "2";

        //                            break;
        //                        case "浅蓝":
        //                            Color = "3";

        //                            break;
        //                        default:
        //                            //result.Code = 2006;
        //                            //result.Message = string.Format("批次格式：[{0}]无法识别！", lstWorkOrderAttribute[0].AttributeValue);

        //                            return "";
        //                    }
        //                    #endregion

        //                    #region 2.取得测试数据
        //                    cfg = new PagingConfig()
        //                    {
        //                        IsPaging = false,
        //                        Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
        //                        OrderBy = "Key.TestTime Desc"
        //                    };

        //                    IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);

        //                    if (lstTestData.Count > 0)
        //                    {
        //                        IVTestData testData = lstTestData[0].Clone() as IVTestData;
        //                        PS_Code = testData.PowersetSubCode;
        //                    }

        //                    if (PS_Code == "I1")
        //                    {
        //                        PS_Code = "1";
        //                    }

        //                    if (PS_Code == "I2")
        //                    {
        //                        PS_Code = "2";
        //                    }

        //                    if (PS_Code == "I3")
        //                    {
        //                        PS_Code = "3";
        //                    }

        //                    if (PS_Code == "I4")
        //                    {
        //                        PS_Code = "4";
        //                    }
        //                    #endregion
                            
        //                    prefixPackageNo = string.Format("ahec{0}1{1}{2}{3}"
        //                                        , (now.Day).ToString("00")            //日
        //                                        , (now.Month).ToString("00")
        //                                        , Color
        //                                        , PS_Code);
        //                    int seqNo = 1;

        //                    cfg = new PagingConfig()
        //                    {
        //                        PageNo = 0,
        //                        PageSize = 1,
        //                        Where = string.Format(@"Key LIKE '%{0}'"
        //                                                , prefixPackageNo),
        //                        OrderBy = "Key DESC"
        //                    };

        //                    IList<Package> lstPackage = this.PackageDataEngine.Get(cfg);
        //                    if (lstPackage.Count > 0)
        //                    {
        //                        //string maxSeqNo = lstPackage[0].Key.Replace(prefixPackageNo, "");
        //                        string maxSeqNo = lstPackage[0].Key.Substring(7,3);

        //                        if (int.TryParse(maxSeqNo, out seqNo))
        //                        {
        //                            seqNo = seqNo + 1;
        //                        }
        //                    }

        //                    prefixPackageNo = string.Format("61{0}{1}{2}", lot.OrderNumber.Substring(4, 5), seqNo.ToString("000"), prefixPackageNo);

        //                    return prefixPackageNo;

        //                    #endregion
        //                default:
        //                    return string.Format("批次格式：[{0}]无法识别！", lstWorkOrderAttribute[0].AttributeValue);
        //            }
        //        }
        //        #endregion
        //    }
        //    else
        //    {
        //        return lot.PackageNo;
        //    }
        //}
        #endregion
        
        public MethodReturnResult<string> CreatePackageNo(string lotNumber)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();

            try
            {
                IList<WorkOrderAttribute> lstWorkOrderAttribute = null;
                PagingConfig cfg = null;
                string prefixPackageNo = string.Empty;
                string packageNo = string.Empty;
                DateTime now = DateTime.Now;
                string PsItemNO = "";
                string PsName = "";
                string minPackageNo = string.Empty;
                string maxPackageNo = string.Empty;
                IList<Package> lstPackage = null;
                int seqNo = 1;
                int year = 0;
                MethodReturnResult<WorkOrderAttribute> workOrderAttrForTTSpecialDateOfEND = new MethodReturnResult<WorkOrderAttribute>();
                MethodReturnResult<MaterialAttribute> materialAttrForTTSpecialDateOfEND = new MethodReturnResult<MaterialAttribute>();

                //取得批次对象
                Lot lot = this.LotDataEngine.Get(lotNumber);

                #region 1.判断批次属性合规性
                if (lot.PackageFlag == true)
                {
                    result.Code = 2001;
                    result.Message = string.Format("批次：[{0}]已包装！", lotNumber);

                    return result;
                }
                #endregion

                #region 2.取得工单打印格式代码
                cfg = new PagingConfig()
                {
                    Where = string.Format(" Key.OrderNumber = '{0}' and Key.AttributeName = 'CreateCodeFormatNum'",
                                            lot.OrderNumber),
                    OrderBy = "Key.OrderNumber,Key.AttributeName"
                };

                lstWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(cfg);
                #endregion

                if (lstWorkOrderAttribute == null || lstWorkOrderAttribute.Count == 0 || lstWorkOrderAttribute[0].AttributeValue == "SE01" )
                {
                    #region 默认格式
                    //托盘编码规则说明
                    //第1、2位：组件包装年份；
                    //第3、4位：组件包装月份；
                    //第5、6位：工单号第7、8位；
                    //第7、8位：工单号后两位；
                    //第9、10、11、12位：托盘流水码
                    //示例：150807010001表示在2015年7月第一个工单车间生产组件，在8月份包装的一拖组件。

                    if (lot.OrderNumber.Length > 8)
                    {
                        DateTime splitTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);

                        if (now < splitTime)
                        {
                            now = now.AddDays(-1);
                        }

                        year = Convert.ToInt32(now.ToString("yy"));

                        //工单7,8位
                        string sub01 = lot.OrderNumber.Substring(6, 2);

                        //工单后两位
                        string sub02 = lot.OrderNumber.Substring(lot.OrderNumber.Length - 2, 2);

                        prefixPackageNo = string.Format("{0}{1}{2}{3}"
                                                        , (year + 8).ToString("00")
                                                        , (now.Month + 8).ToString("00")
                                                        , sub01
                                                        , sub02
                                                        );
                    }                  

                    if (lot.LocationName == "103A")
                    {
                        seqNo = 5000;
                        if (prefixPackageNo == "26150631")
                        {
                            prefixPackageNo = "26152631";
                        }
                    }


                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format(@"Key LIKE '{0}%'"
                                                , prefixPackageNo),
                        OrderBy = "Key DESC"
                    };

                    lstPackage = this.PackageDataEngine.Get(cfg);
                    if (lstPackage.Count > 0)
                    {
                        string maxSeqNo = lstPackage[0].Key.Replace(prefixPackageNo, "");

                        if (int.TryParse(maxSeqNo, out seqNo))
                        {
                            seqNo = seqNo + 1;
                        }
                    }

                    //生成批次号
                    packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));

                    //设置返回批次号
                    result.Data = packageNo;
                    #endregion
                }
                else
                {
                    switch (lstWorkOrderAttribute[0].AttributeValue)
                    {
                        case "DF01":
                            #region 东方日升格式
                                    #region 1.颜色
                                    string lotColor = "";

                                    switch (lot.Color)
                                    {
                                        case "深蓝":
                                            lotColor = "1";

                                            break;
                                        case "正蓝":
                                            lotColor = "2";

                                            break;
                                        case "浅蓝":
                                            lotColor = "3";

                                            break;
                                        default:                                            
                                            result.Code = 2003;
                                            result.Message = string.Format("批次颜色[{0}]不满足托号格式[{1}]要求！",
                                                                            lot.Color,
                                                                            lstWorkOrderAttribute[0].AttributeValue);

                                            return result;                                            
                                    }
                                    #endregion

                                    #region 2.取得测试电流分档数据
                                    cfg = new PagingConfig()
                                    {
                                        IsPaging = false,
                                        Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                        OrderBy = "Key.TestTime Desc"
                                    };

                                    IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);

                                    string PS_Code = "";

                                    if (lstTestData.Count > 0)
                                    {
                                        PS_Code = lstTestData[0].PowersetSubCode;
                                    }

                                    switch (PS_Code)
                                    {
                                        case "I1":
                                            PS_Code = "1";

                                            break;
                                        case "I2":
                                            PS_Code = "2";

                                            break;
                                        case "I3":
                                            PS_Code = "3";

                                            break;
                                        case "I4":
                                            PS_Code = "4";

                                            break;
                                        default:                                            
                                            result.Code = 2003;
                                            result.Message = string.Format("批次功率分档[{0}]不满足托号格式[{1}]要求！", 
                                                                            PS_Code,
                                                                            lstWorkOrderAttribute[0].AttributeValue);

                                            return result;
                                    }
                                    #endregion

                                    #region 3.生成托号
                                    prefixPackageNo = string.Format("ahec{0}1{1}{2}{3}"
                                                        , (now.Day).ToString("00")          //日
                                                        , (now.Month).ToString("00")        //月
                                                        , lotColor                          //颜色
                                                        , PS_Code);                         //电流档

                                    seqNo = 1;

                                    if (lot.LocationName == "103A")
                                    {
                                        seqNo = 500;
                                    }

                                    cfg = new PagingConfig()
                                    {
                                        PageNo = 0,
                                        PageSize = 1,
                                        Where = string.Format(@"Key LIKE '61" + lot.OrderNumber.Substring(4, 5) + "___{0}'"
                                                                , prefixPackageNo),
                                        OrderBy = "Key DESC"
                                    };

                                    lstPackage = this.PackageDataEngine.Get(cfg);

                                    if (lstPackage.Count > 0)
                                    {
                                        string maxSeqNo = lstPackage[0].Key.Substring(7, 3);

                                        if (int.TryParse(maxSeqNo, out seqNo))
                                        {
                                            seqNo = seqNo + 1;
                                        }
                                    }

                                    //生成批次号
                                    packageNo = string.Format("61{0}{1}{2}", lot.OrderNumber.Substring(4, 5), seqNo.ToString("000"), prefixPackageNo);
                                    #endregion

                                    //设置返回批次号
                                    result.Data = packageNo;

                            #endregion

                            break;
                        case "DF02":
                            #region 东方日升格式
                            #region 1.颜色
                            lotColor = "";

                            switch (lot.Color)
                            {
                                case "深蓝":
                                    lotColor = "1";

                                    break;
                                case "正蓝":
                                    lotColor = "2";

                                    break;
                                case "浅蓝":
                                    lotColor = "3";

                                    break;
                                default:
                                    result.Code = 2003;
                                    result.Message = string.Format("批次颜色[{0}]不满足托号格式[{1}]要求！",
                                                                    lot.Color,
                                                                    lstWorkOrderAttribute[0].AttributeValue);

                                    return result;
                            }
                            #endregion

                            #region 2.取得测试电流分档数据
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            PS_Code = "";

                            if (lstTestData.Count > 0)
                            {
                                PS_Code = lstTestData[0].PowersetSubCode;
                            }

                            switch (PS_Code)
                            {
                                case "I1":
                                    PS_Code = "1";

                                    break;
                                case "I2":
                                    PS_Code = "2";

                                    break;
                                case "I3":
                                    PS_Code = "3";

                                    break;
                                case "I4":
                                    PS_Code = "4";

                                    break;
                                default:
                                    result.Code = 2003;
                                    result.Message = string.Format("批次功率分档[{0}]不满足托号格式[{1}]要求！",
                                                                    PS_Code,
                                                                    lstWorkOrderAttribute[0].AttributeValue);

                                    return result;
                            }
                            #endregion

                            #region 3.生成托号
                            prefixPackageNo = string.Format("dsdd{0}1{1}{2}{3}"
                                                , (now.Day).ToString("00")          //日
                                                , (now.Month).ToString("00")        //月
                                                , lotColor                          //颜色
                                                , PS_Code);                         //电流档

                            seqNo = 1;

                            if (lot.LocationName == "103A")
                            {
                                seqNo = 500;
                            }

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key LIKE '61" + lot.OrderNumber.Substring(4, 5) + "___{0}'"
                                                        , prefixPackageNo),
                                OrderBy = "Key DESC"
                            };

                            lstPackage = this.PackageDataEngine.Get(cfg);

                            if (lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(7, 3);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("61{0}{1}{2}", lot.OrderNumber.Substring(4, 5), seqNo.ToString("000"), prefixPackageNo);
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;
                        case "DF03":
                            #region 东方日升格式
                            #region 1.颜色
                            lotColor = "";

                            switch (lot.Color)
                            {
                                case "深蓝":
                                    lotColor = "2";

                                    break;
                                case "正蓝":
                                    lotColor = "2";

                                    break;
                                case "浅蓝":
                                    lotColor = "2";

                                    break;
                                default:
                                    result.Code = 2003;
                                    result.Message = string.Format("批次颜色[{0}]不满足托号格式[{1}]要求！",
                                                                    lot.Color,
                                                                    lstWorkOrderAttribute[0].AttributeValue);

                                    return result;
                            }
                            #endregion

                            #region 2.取得测试电流分档数据
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            PS_Code = "";

                            if (lstTestData.Count > 0)
                            {
                                PS_Code = lstTestData[0].PowersetSubCode;
                            }

                            switch (PS_Code)
                            {
                                case "I1":
                                    PS_Code = "1";

                                    break;
                                case "I2":
                                    PS_Code = "2";

                                    break;
                                case "I3":
                                    PS_Code = "3";

                                    break;
                                case "I4":
                                    PS_Code = "4";

                                    break;
                                default:
                                    result.Code = 2003;
                                    result.Message = string.Format("批次功率分档[{0}]不满足托号格式[{1}]要求！",
                                                                    PS_Code,
                                                                    lstWorkOrderAttribute[0].AttributeValue);

                                    return result;
                            }
                            #endregion

                            #region 3.生成托号
                            prefixPackageNo = string.Format("gahd{0}1{1}{2}{3}"
                                                , (now.Day).ToString("00")          //日
                                                , (now.Month).ToString("00")        //月
                                                , lotColor                          //颜色
                                                , PS_Code);                         //电流档

                            seqNo = 1;

                            if (lot.LocationName == "103A")
                            {
                                seqNo = 500;
                            }

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key LIKE '61" + lot.OrderNumber.Substring(4, 5) + "___{0}'"
                                                        , prefixPackageNo),
                                OrderBy = "Key DESC"
                            };

                            lstPackage = this.PackageDataEngine.Get(cfg);

                            if (lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(7, 3);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("61{0}{1}{2}", lot.OrderNumber.Substring(4, 5), seqNo.ToString("000"), prefixPackageNo);
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;
                        case "XX01":
                            #region 协鑫格式(多晶72组件)
                            
                            #region 2.取得测试功率
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            if (lstTestData.Count > 0)
                            {
                                PsItemNO = lstTestData[0].PowersetItemNo.ToString();
                            }

                            //取得分档代码名称
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.OrderNumber='{0}' AND Key.ItemNo='{1}'", lot.OrderNumber, PsItemNO),
                          
                            };

                            IList<WorkOrderPowerset> lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg);

                            if (lstWorkOrderPowerset.Count > 0)
                            {
                                WorkOrderPowerset WorkOrderPowerset = lstWorkOrderPowerset[0].Clone() as WorkOrderPowerset;
                                PsName = WorkOrderPowerset.PowerName;
                            }

                            #endregion

                            #region 3.生成托号
                            
                            prefixPackageNo = string.Format("64P672{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)        //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")   //月      
                                               , (now.Day).ToString("00")     //日
                                               );

                            seqNo = 1;
                                                        
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key LIKE '{0}%'"
                                                        , prefixPackageNo),
                                OrderBy = "Key DESC"
                            };

                            lstPackage = this.PackageDataEngine.Get(cfg);

                            if (lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(15, 4);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;
                        case "XX02":
                            #region 协鑫格式2(多晶60组件)-晋能创托规则

                            #region 2.取得测试功率
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            if (lstTestData.Count > 0)
                            {
                                PsItemNO = lstTestData[0].PowersetItemNo.ToString();
                            }

                            //取得分档代码名称
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.OrderNumber='{0}' AND Key.ItemNo='{1}'", lot.OrderNumber, PsItemNO),

                            };

                            IList<WorkOrderPowerset> lstWorkOrderPowersets = this.WorkOrderPowersetDataEngine.Get(cfg);

                            if (lstWorkOrderPowersets.Count > 0)
                            {
                                WorkOrderPowerset WorkOrderPowerset = lstWorkOrderPowersets[0].Clone() as WorkOrderPowerset;
                                PsName = WorkOrderPowerset.PowerName;
                            }

                            #endregion

                            #region 3.生成托号
                            
                            prefixPackageNo = string.Format("64P660{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)        //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")   //月      
                                               , (now.Day).ToString("00")     //日
                                               );

                            seqNo = 1;

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key LIKE '{0}%'"
                                                        , prefixPackageNo),
                                OrderBy = "Key DESC"
                            };

                            lstPackage = this.PackageDataEngine.Get(cfg);

                            if (lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(15, 4);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;
                        case "XX03":
                            #region 协鑫格式2(多晶60组件)-协鑫创托规则

                            #region 2.取得测试功率
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            if (lstTestData.Count > 0)
                            {
                                PsItemNO = lstTestData[0].PowersetItemNo.ToString();
                            }

                            //取得分档代码名称
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.OrderNumber='{0}' AND Key.ItemNo='{1}'", lot.OrderNumber, PsItemNO),

                            };

                            IList<WorkOrderPowerset> lstWorkOrderPowersetss = this.WorkOrderPowersetDataEngine.Get(cfg);

                            if (lstWorkOrderPowersetss.Count > 0)
                            {
                                WorkOrderPowerset WorkOrderPowerset = lstWorkOrderPowersetss[0].Clone() as WorkOrderPowerset;
                                PsName = WorkOrderPowerset.PowerName;
                            }

                            #endregion

                            #region 3.生成托号

                            prefixPackageNo = string.Format("64P660{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)        //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")   //月      
                                               , (now.Day).ToString("00")     //日
                                               );

                            seqNo = 1;

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key LIKE '{0}%'"
                                                        , prefixPackageNo),
                                OrderBy = "Key DESC"
                            };

                            lstPackage = this.PackageDataEngine.Get(cfg);

                            if (lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(15, 4);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;
                        case "XX04":
                        //case "XX05":
                            #region 协鑫格式(单晶72组件)

                            #region 2.取得测试功率
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            if (lstTestData.Count > 0)
                            {
                                PsItemNO = lstTestData[0].PowersetItemNo.ToString();
                            }

                            //取得分档代码名称
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.OrderNumber='{0}' AND Key.ItemNo='{1}'", lot.OrderNumber, PsItemNO),

                            };

                            IList<WorkOrderPowerset> lstWorkOrderPowerset10 = this.WorkOrderPowersetDataEngine.Get(cfg);

                            if (lstWorkOrderPowerset10.Count > 0)
                            {
                                WorkOrderPowerset WorkOrderPowerset = lstWorkOrderPowerset10[0].Clone() as WorkOrderPowerset;
                                PsName = WorkOrderPowerset.PowerName;
                            }

                            #endregion

                            #region 3.生成托号

                            prefixPackageNo = string.Format("64M672{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)        //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")   //月      
                                               , (now.Day).ToString("00")     //日
                                               );

                            seqNo = 1;

                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key LIKE '{0}%'"
                                                        , prefixPackageNo),
                                OrderBy = "Key DESC"
                            };

                            lstPackage = this.PackageDataEngine.Get(cfg);

                            if (lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(15, 4);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;
                        case "XX05":
                            #region 协鑫格式3(多晶60组件)-永能创托规则

                            #region 2.取得测试功率
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            if (lstTestData.Count > 0)
                            {
                                PsItemNO = lstTestData[0].PowersetItemNo.ToString();
                            }

                            //取得分档代码名称
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.OrderNumber='{0}' AND Key.ItemNo='{1}'", lot.OrderNumber, PsItemNO),

                            };

                            IList<WorkOrderPowerset> lstWorkOrderPowerset11 = this.WorkOrderPowersetDataEngine.Get(cfg);

                            if (lstWorkOrderPowerset11.Count > 0)
                            {
                                WorkOrderPowerset WorkOrderPowerset = lstWorkOrderPowerset11[0].Clone() as WorkOrderPowerset;
                                PsName = WorkOrderPowerset.PowerName;
                            }

                            #endregion

                            #region 3.生成托号

                            prefixPackageNo = string.Format("05P660{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)                 //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")             //月      
                                               , (now.Day).ToString("00")               //日
                                               );

                            if (lot.LocationName == "103A")
                            {
                                minPackageNo = string.Format("{0}0001", prefixPackageNo);
                                maxPackageNo = string.Format("{0}0192", prefixPackageNo);
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key >= '{0}' AND Key < '{1}'"
                                                        , minPackageNo
                                                        , maxPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }
                            if (lot.LocationName == "102A" || lot.LocationName == "102B")
                            {
                                seqNo = 193;
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key LIKE '{0}%'"
                                                            , prefixPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }
                         
                            if (lstPackage != null && lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(15, 4);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;

                        case "XX06":
                            #region 协鑫格式(单晶60组件-晋能代加工)

                            #region 2.取得测试功率
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            if (lstTestData.Count > 0)
                            {
                                PsItemNO = lstTestData[0].PowersetItemNo.ToString();
                            }

                            //取得分档代码名称
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.OrderNumber='{0}' AND Key.ItemNo='{1}'", lot.OrderNumber, PsItemNO),

                            };

                            IList<WorkOrderPowerset> lstWorkOrderPowerset12 = this.WorkOrderPowersetDataEngine.Get(cfg);

                            if (lstWorkOrderPowerset12.Count > 0)
                            {
                                WorkOrderPowerset WorkOrderPowerset = lstWorkOrderPowerset12[0].Clone() as WorkOrderPowerset;
                                PsName = WorkOrderPowerset.PowerName;
                            }

                            #endregion

                            #region 3.生成托号

                            prefixPackageNo = string.Format("64M660{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)                 //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")             //月      
                                               , (now.Day).ToString("00")               //日
                                               );

                            if (lot.LocationName == "103A")
                            {
                                seqNo = 5001;
                                minPackageNo = string.Format("{0}5000", prefixPackageNo);
                                maxPackageNo = string.Format("{0}9999", prefixPackageNo);
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key >= '{0}' AND Key < '{1}'"
                                                        , minPackageNo
                                                        , maxPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }
                            if (lot.LocationName == "102A" || lot.LocationName == "102B")
                            {
                                seqNo = 1;
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key LIKE '{0}%'"
                                                            , prefixPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }

                            if (lstPackage != null && lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(15, 4);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;

                        case "XX07":
                            #region 协鑫格式(单晶72组件-晋能-张家港代加工)

                            #region 1.取得测试功率
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            if (lstTestData.Count > 0)
                            {
                                PsItemNO = lstTestData[0].PowersetItemNo.ToString();
                            }

                            //取得分档代码名称
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.OrderNumber='{0}' AND Key.ItemNo='{1}'", lot.OrderNumber, PsItemNO),

                            };

                            IList<WorkOrderPowerset> lstWorkOrderPowerset17 = this.WorkOrderPowersetDataEngine.Get(cfg);

                            if (lstWorkOrderPowerset17.Count > 0)
                            {
                                WorkOrderPowerset WorkOrderPowerset = lstWorkOrderPowerset17[0].Clone() as WorkOrderPowerset;
                                PsName = WorkOrderPowerset.PowerName;
                            }

                            #endregion

                            #region 2.生成托号

                            prefixPackageNo = string.Format("27M672{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)                 //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")             //月      
                                               , (now.Day).ToString("00")               //日
                                               );

                            if (lot.LocationName == "102A" || lot.LocationName == "102B")
                            {
                                seqNo = 5001;
                                minPackageNo = string.Format("{0}5001", prefixPackageNo);
                                maxPackageNo = string.Format("{0}9999", prefixPackageNo);
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key >= '{0}' AND Key <= '{1}'"
                                                        , minPackageNo
                                                        , maxPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }
                            if (lot.LocationName == "103A")
                            {
                                seqNo = 1;
                                minPackageNo = string.Format("{0}0001", prefixPackageNo);
                                maxPackageNo = string.Format("{0}5000", prefixPackageNo);
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key >= '{0}' AND Key <= '{1}'"
                                                        , minPackageNo
                                                        , maxPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }

                            if (lstPackage != null && lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(15, 4);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;

                        case "XX08":
                            #region 协鑫格式(72常规-晋能（文水加晋中）代加工)

                            #region 1.取得测试功率
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            if (lstTestData.Count > 0)
                            {
                                PsItemNO = lstTestData[0].PowersetItemNo.ToString();
                            }

                            //取得分档代码名称
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.OrderNumber='{0}' AND Key.ItemNo='{1}'", lot.OrderNumber, PsItemNO),

                            };

                            IList<WorkOrderPowerset> lstWorkOrderPowerset18 = this.WorkOrderPowersetDataEngine.Get(cfg);

                            if (lstWorkOrderPowerset18.Count > 0)
                            {
                                WorkOrderPowerset WorkOrderPowerset = lstWorkOrderPowerset18[0].Clone() as WorkOrderPowerset;
                                PsName = WorkOrderPowerset.PowerName;
                            }

                            #endregion

                            #region 2.生成托号

                            if (lot.LocationName == "102A" || lot.LocationName == "102B")
                            {
                                prefixPackageNo = string.Format("64P672{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)                 //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")             //月      
                                               , (now.Day).ToString("00")               //日
                                               );

                                seqNo = 5001;
                                minPackageNo = string.Format("{0}5001", prefixPackageNo);
                                maxPackageNo = string.Format("{0}9999", prefixPackageNo);
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key >= '{0}' AND Key <= '{1}'"
                                                        , minPackageNo
                                                        , maxPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }
                            if (lot.LocationName == "103A")
                            {
                                prefixPackageNo = string.Format("64M672{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)                 //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")             //月      
                                               , (now.Day).ToString("00")               //日
                                               );

                                seqNo = 1;
                                minPackageNo = string.Format("{0}0001", prefixPackageNo);
                                maxPackageNo = string.Format("{0}5000", prefixPackageNo);
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key >= '{0}' AND Key <= '{1}'"
                                                        , minPackageNo
                                                        , maxPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }

                            if (lstPackage != null && lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(15, 4);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;

                        case "XX09":
                            #region 协鑫格式(72单晶-晋能（文水加晋中）代加工)

                            #region 1.取得测试功率
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber = '{0}' AND IsDefault = 1", lotNumber),
                                OrderBy = "Key.TestTime Desc"
                            };

                            lstTestData = this.IVTestDataDataEngine.Get(cfg);

                            if (lstTestData.Count > 0)
                            {
                                PsItemNO = lstTestData[0].PowersetItemNo.ToString();
                            }

                            //取得分档代码名称
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.OrderNumber='{0}' AND Key.ItemNo='{1}'", lot.OrderNumber, PsItemNO),

                            };

                            IList<WorkOrderPowerset> lstWorkOrderPowerset19 = this.WorkOrderPowersetDataEngine.Get(cfg);

                            if (lstWorkOrderPowerset19.Count > 0)
                            {
                                WorkOrderPowerset WorkOrderPowerset = lstWorkOrderPowerset19[0].Clone() as WorkOrderPowerset;
                                PsName = WorkOrderPowerset.PowerName;
                            }

                            #endregion

                            #region 2.生成托号

                            if (lot.LocationName == "102A" || lot.LocationName == "102B")
                            {
                                prefixPackageNo = string.Format("64M672{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)                 //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")             //月      
                                               , (now.Day).ToString("00")               //日
                                               );

                                seqNo = 5001;
                                minPackageNo = string.Format("{0}5001", prefixPackageNo);
                                maxPackageNo = string.Format("{0}9999", prefixPackageNo);
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key >= '{0}' AND Key <= '{1}'"
                                                        , minPackageNo
                                                        , maxPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }
                            if (lot.LocationName == "103A")
                            {
                                prefixPackageNo = string.Format("64M672{0}{1}{2}{3}"
                                               , PsName.Substring(0, 3)                 //功率档
                                               , Convert.ToInt32(now.ToString("yy"))    //年
                                               , (now.Month).ToString("00")             //月      
                                               , (now.Day).ToString("00")               //日
                                               );

                                seqNo = 1;
                                minPackageNo = string.Format("{0}0001", prefixPackageNo);
                                maxPackageNo = string.Format("{0}5000", prefixPackageNo);
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key >= '{0}' AND Key <= '{1}'"
                                                        , minPackageNo
                                                        , maxPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }

                            if (lstPackage != null && lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(15, 4);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion

                            break;

                        case "TT01":
                            #region 塔塔格式(月份+7)
                            if (lot.OrderNumber.Length > 8)
                            {
                                DateTime splitTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);

                                if (now < splitTime)
                                {
                                    now = now.AddDays(-1);
                                }
                                year = Convert.ToInt32(now.ToString("yy"));

                                //工单7,8位
                                string sub01 = lot.OrderNumber.Substring(6, 2);

                                //工单后两位
                                string sub02 = lot.OrderNumber.Substring(lot.OrderNumber.Length - 2, 2);

                                prefixPackageNo = string.Format("{0}{1}{2}{3}"
                                                            , (year + 8).ToString("00")
                                                            , (now.Month + 8).ToString("00")
                                                            , sub01
                                                            , sub02
                                                            );

                                //查找工单月份+8创批截至日期
                                WorkOrderAttributeKey workOrderAttributeKey0fDateAdd = new WorkOrderAttributeKey()
                                {
                                    OrderNumber = lot.OrderNumber,
                                    AttributeName = "RuleChangeDate"
                                };
                                workOrderAttrForTTSpecialDateOfEND.Data = this.WorkOrderAttributeDataEngine.Get(workOrderAttributeKey0fDateAdd);
                                if (workOrderAttrForTTSpecialDateOfEND.Data != null)
                                {
                                    //工单月份加8创批规则截至日期                                                               

                                    string endYearOfEndDate = workOrderAttrForTTSpecialDateOfEND.Data.AttributeValue.Trim().Substring(0, 2);
                                    string endMonthOfEndDate = workOrderAttrForTTSpecialDateOfEND.Data.AttributeValue.Trim().Substring(2, 2);
                                    string endDayOfEndDate = workOrderAttrForTTSpecialDateOfEND.Data.AttributeValue.Trim().Substring(4, 2);

                                    //(当前年份) > (月份加8创批规则截至年份)
                                    if (year > Convert.ToInt32(endYearOfEndDate))
                                    {
                                        //创建批次号固定部分
                                        prefixPackageNo = GetprefixPackageNo(year, now.Month, sub01, sub02);
                                    }
                                    //(当前年份) == (月份加8创批规则截至年份)
                                    if (year == Convert.ToInt32(endYearOfEndDate))
                                    {
                                        //(当前月份) > (月份加8创批规则截至月份)
                                        if (now.Month > Convert.ToInt32(endMonthOfEndDate))
                                        {
                                            //创建批次号固定部分
                                            prefixPackageNo = GetprefixPackageNo(year, now.Month, sub01, sub02);
                                        }
                                        //(当前月份) == (月份加8创批规则截至月份)
                                        if (now.Month == Convert.ToInt32(endMonthOfEndDate))
                                        {
                                            //(当前天数) > (月份加8创批规则截至天份)
                                            if (now.Day < Convert.ToInt32(endDayOfEndDate))
                                            {
                                                //创建批次号固定部分
                                                prefixPackageNo = GetprefixPackageNo(year, now.Month, sub01, sub02);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ////查找产品编码非正常创批截至日期
                                    MaterialAttributeKey materialAttributeKey0fDateAdd = new MaterialAttributeKey()
                                    {
                                        MaterialCode = lot.MaterialCode,
                                        AttributeName = "RuleChangeDate"
                                    };
                                    materialAttrForTTSpecialDateOfEND.Data = this.MaterialAttributeDataEngine.Get(materialAttributeKey0fDateAdd);
                                    if (materialAttrForTTSpecialDateOfEND.Data != null)
                                    {
                                        //工单月份加8创批规则截至日期                                                               
                                        string endYearOfEndDate = materialAttrForTTSpecialDateOfEND.Data.Value.Trim().Substring(0, 2);
                                        string endMonthOfEndDate = materialAttrForTTSpecialDateOfEND.Data.Value.Trim().Substring(2, 2);
                                        string endDayOfEndDate = materialAttrForTTSpecialDateOfEND.Data.Value.Trim().Substring(4, 2);

                                        //(当前年份) > (月份加8创批规则截至年份)
                                        if (year > Convert.ToInt32(endYearOfEndDate))
                                        {
                                            //创建批次号固定部分
                                            prefixPackageNo = GetprefixPackageNo(year, now.Month, sub01, sub02);
                                        }
                                        //(当前年份) == (月份加8创批规则截至年份)
                                        if (year == Convert.ToInt32(endYearOfEndDate))
                                        {
                                            //(当前月份) > (月份加8创批规则截至月份)
                                            if (now.Month > Convert.ToInt32(endMonthOfEndDate))
                                            {
                                                //创建批次号固定部分
                                                prefixPackageNo = GetprefixPackageNo(year, now.Month, sub01, sub02);
                                            }
                                            //(当前月份) == (月份加8创批规则截至月份)
                                            if (now.Month == Convert.ToInt32(endMonthOfEndDate))
                                            {
                                                //(当前天数) > (月份加8创批规则截至天份)
                                                if (now.Day < Convert.ToInt32(endDayOfEndDate))
                                                {
                                                    //创建批次号固定部分
                                                    prefixPackageNo = GetprefixPackageNo(year, now.Month, sub01, sub02);
                                                }
                                            }
                                        }
                                    }
                                }
                                if (lot.LocationName == "103A")
                                {
                                    seqNo = 5000;
                                    if (prefixPackageNo == "26191039")
                                    {
                                        prefixPackageNo = "26193039";
                                    }
                                }

                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key LIKE '{0}%'"
                                                            , prefixPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                                if (lstPackage.Count > 0)
                                {
                                    string maxSeqNo = lstPackage[0].Key.Replace(prefixPackageNo, "");

                                    if (int.TryParse(maxSeqNo, out seqNo))
                                    {
                                        seqNo = seqNo + 1;
                                    }
                                }

                                //生成批次号
                                packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));

                                //设置返回批次号
                                result.Data = packageNo;
                            }
                            #endregion

                            break;

                        case "YL01":
                            #region 英利多晶项目创托规则-1

                            #region 1.生成托号

                            prefixPackageNo = string.Format("{0}"
                                               , "S1851348738A1115");

                            if (lot.LocationName == "102A" || lot.LocationName == "102B")
                            {
                                seqNo = 1;
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key LIKE '{0}%'"
                                                            , prefixPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }

                            if (lstPackage != null && lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(16, 5);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("00000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion
                            
                            break;

                        case "YL02":
                            #region 英利多晶项目创托规则-2

                            #region 1.生成托号

                            prefixPackageNo = string.Format("{0}"
                                               , "S1851348735A1115");

                            minPackageNo = string.Format("{0}01000", prefixPackageNo);
                            maxPackageNo = string.Format("{0}02828", prefixPackageNo);

                            if (lot.LocationName == "102A" || lot.LocationName == "102B")
                            {
                                seqNo = 1000;
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key>='{0}' AND Key<='{1}'
                                        AND IsMainLot=1"
                                                            , minPackageNo
                                                            , maxPackageNo),
                                    OrderBy = "Key DESC"
                                };

                                lstPackage = this.PackageDataEngine.Get(cfg);
                            }

                            if (lstPackage != null && lstPackage.Count > 0)
                            {
                                string maxSeqNo = lstPackage[0].Key.Substring(16, 5);

                                if (int.TryParse(maxSeqNo, out seqNo))
                                {
                                    seqNo = seqNo + 1;
                                }
                            }

                            //生成托号
                            packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("00000"));
                            #endregion

                            //设置返回批次号
                            result.Data = packageNo;

                            #endregion
                            
                            break;

                        default:                            
                            result.Code = 2003;
                            result.Message = string.Format("托号生成格式：[{0}]无法识别！", lstWorkOrderAttribute[0].AttributeValue);

                            return result;             
                    }

                }
            }
            catch (Exception ex)
            {
                result.Code = 2000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }

        string GetprefixPackageNo(int year, int month, string sub01, string sub02)
        {
            string prefixLotNumber = "";
            prefixLotNumber = string.Format("{0}{1}{2}{3}"
                                        , (year + 8).ToString("00")
                                        , (month + 7).ToString("00")
                                        , sub01
                                        , sub02);
            return prefixLotNumber;
        }

        /// <summary>
        /// OEM创建托号
        /// </summary>
        /// <param name="lotNumber"></param>
        /// <returns></returns>
        public MethodReturnResult<string> CreatePackageNo(OemData oemData,WorkOrder oemWorkOrder)
        {
            MethodReturnResult<string> result = new MethodReturnResult<string>();

            try
            {
                IList<WorkOrderAttribute> lstWorkOrderAttribute = null;
                PagingConfig cfg = null;
                string prefixPackageNo = string.Empty;
                string packageNo = string.Empty;
                DateTime now = DateTime.Now;

                #region 1.判断批次属性合规性
                if (oemData.PackageNo != null && oemData.PackageNo != "")
                {
                    result.Code = 2001;
                    result.Message = string.Format("批次：[{0}]已包装！", oemData.Key);

                    return result;
                }
                #endregion

                #region 2.取得工单打印格式代码
                cfg = new PagingConfig()
                {
                    Where = string.Format(" Key.OrderNumber = '{0}' and Key.AttributeName = 'CreateCodeFormatNum'",
                                            oemWorkOrder.Key),
                    OrderBy = "Key.OrderNumber,Key.AttributeName"
                };

                lstWorkOrderAttribute = this.WorkOrderAttributeDataEngine.Get(cfg);
                #endregion

                if (lstWorkOrderAttribute == null || lstWorkOrderAttribute.Count == 0)
                {
                    #region 默认格式
                    //托盘编码规则说明
                    //第1、2位：组件包装年份；
                    //第3、4位：组件包装月份；
                    //第5、6位：工单号第7、8位；
                    //第7、8位：工单号后两位；
                    //第9、10、11、12位：托盘流水码
                    //示例：150807010001表示在2015年7月第一个工单车间生产组件，在8月份包装的一拖组件。

                    if (oemWorkOrder.Key.Length > 8)
                    {
                        DateTime splitTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);

                        if (now < splitTime)
                        {
                            now = now.AddDays(-1);
                        }

                        int year = Convert.ToInt32(now.ToString("yy"));

                        //工单7,8位
                        string sub01 = oemWorkOrder.Key.Substring(6, 2);

                        //工单后两位
                        string sub02 = oemWorkOrder.Key.Substring(oemWorkOrder.Key.Length - 2, 2);

                        prefixPackageNo = string.Format("{0}{1}{2}{3}"
                                                        , (year + 8).ToString("00")
                                                        , (now.Month + 8).ToString("00")
                                                        , sub01
                                                        , sub02
                                                        );
                    }

                    int seqNo = 1;
                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format(@"Key LIKE '{0}%'"
                                                , prefixPackageNo),
                        OrderBy = "Key DESC"
                    };

                    IList<Package> lstPackage = this.PackageDataEngine.Get(cfg);
                    if (lstPackage.Count > 0)
                    {
                        string maxSeqNo = lstPackage[0].Key.Replace(prefixPackageNo, "");

                        if (int.TryParse(maxSeqNo, out seqNo))
                        {
                            seqNo = seqNo + 1;
                        }
                    }

                    //生成批次号
                    packageNo = string.Format("{0}{1}", prefixPackageNo, seqNo.ToString("0000"));

                    //设置返回批次号
                    result.Data = packageNo;
                    #endregion
                }
                else
                {
                    switch (lstWorkOrderAttribute[0].AttributeValue)
                    {                                                
                        default:
                            result.Code = 2003;
                            result.Message = string.Format("托号生成格式：[{0}]无法识别！", lstWorkOrderAttribute[0].AttributeValue);

                            return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 2000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }                
    }
}
