using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.ZPVM.ServiceExtensions
{
    public class JNLotNumberGenerator : ILotNumberGenerate
    {
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单数据访问类。
        /// </summary>
        public IWorkOrderDataEngine WorkOrderDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 物料数据访问类。
        /// </summary>
        public IMaterialDataEngine MaterialDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单属性访问类
        /// </summary>
        public IWorkOrderAttributeDataEngine WorkOrderAttributeDataEngine
        {
            get;
            set;
        }

        public IList<string> Generate(EnumLotType lotType, string orderNumber, int count, string other)
        {
            //标准组件序列号编码规则说明
            //第1、2位：公司名称（JN表示晋能）
            //第3位：生产车间（1表示102A车间，2表示102B车间）
            //第4、5位：组件生产年份加上8【如14年生产的组件，代码为22（14+8=22）】
            //第6、7位：组件生产月份【如7月份生产的组件，代码为15（7+8=15）】
            //第8、9位：组件生产日期【如30号生产的组件，代码为38（30+8）】
            //第10位：电池片类型：（1表示单晶，2表示多晶）
            //第11位：表示电池片数量：（1表示6*10组件，2表示6*12组件）
            //第12、13、14、15位：组件流水码
            //示例：JN1221538110001表示在2014年7月30日在晋能102A车间生产的一块单晶6*10的组件，它的流水码是0001

            bool flagJN = false;

            IList<string> lstLotNumber = new List<string>();
            //获取车间名称
            WorkOrder wo = this.WorkOrderDataEngine.Get(orderNumber);
            if (wo == null)
            {
                return lstLotNumber;
            }
            //第3位：生产车间（1表示102A车间，2表示102B车间）
            string locationName = string.Empty;
            if (wo.LocationName == "102A")
            {
                locationName = "1";
            }
            else if (wo.LocationName == "102B")
            {
                locationName = "2";
            }
            //第10位：电池片类型：（1表示单晶，2表示多晶）
            //1201	单晶
            //120101	125组件
            //120102	156组件
            //1202	多晶
            //120201	125
            //120202	156
            string type = string.Empty;
            if (wo.MaterialCode.StartsWith("1201"))
            {
                type = "1";
            }
            else if (wo.MaterialCode.StartsWith("1202"))
            {
                type = "2";
            }
            //根据物料号获取物料类型
            Material m = this.MaterialDataEngine.Get(wo.MaterialCode);
            if (m == null)
            {
                return lstLotNumber;
            }
            //第11位：表示电池片数量：（1表示6*10组件，2表示6*12组件）
            string qty = string.Empty;
            if (m.MainRawQtyPerLot == 60)
            {
                qty = "1";
            }
            else if (m.MainRawQtyPerLot == 72)
            {
                qty = "2";
            }

            if (string.IsNullOrEmpty(locationName)
                || string.IsNullOrEmpty(type)
                || string.IsNullOrEmpty(qty))
            {
                return lstLotNumber;
            }

            DateTime now = DateTime.Now;
            //7点为分割时间，6点前创建的批次使用前一天的流水号。
            DateTime splitTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);
            if (now < splitTime)
            {
                now = now.AddDays(-1);
            }

            int year = Convert.ToInt32(now.ToString("yy"));

            ///判断工单是否创批时加JN
            PagingConfig cfgJN = new PagingConfig()
            {
                Where = string.Format(" Key.OrderNumber = '{0}' and Key.AttributeName='NoJN'"
                                            , orderNumber),
                OrderBy = "Key.OrderNumber,Key.AttributeName"
            };
            MethodReturnResult<IList<WorkOrderAttribute>> resultFlagJN = new MethodReturnResult<IList<WorkOrderAttribute>>();

            resultFlagJN.Data = this.WorkOrderAttributeDataEngine.Get(cfgJN);
            if (resultFlagJN.Code == 0 && resultFlagJN.Data.Count > 0)
            {
                WorkOrderAttribute obj = resultFlagJN.Data[0];
                if (Convert.ToBoolean(obj.AttributeValue))
                {
                    flagJN = true;
                }
            }
            string prefixLotNumber = null;
            if (flagJN)
            {

                prefixLotNumber = string.Format("JN{0}{1}{2}{3}{4}{5}"
                                       , locationName
                                       , (year + 8).ToString("00")
                                       , (now.Month + 8).ToString("00")
                                       , (now.Day + 8).ToString("00")
                                       , type
                                       , qty);
            }
            else
            {
                prefixLotNumber = string.Format("{0}{1}{2}{3}{4}{5}"
                                        , locationName
                                        , (year + 8).ToString("00")
                                        , (now.Month + 8).ToString("00")
                                        , (now.Day + 8).ToString("00")
                                        , type
                                        , qty);
            }



            int seqNo = 1;
            string minLotNumber = string.Format("{0}0001", prefixLotNumber);
            string maxLotNumber = string.Format("{0}9999", prefixLotNumber);
            //按照线别生成不同的批次。
            if (other == "102A-A" || other == "102B-A")
            {
                seqNo = 1;
                minLotNumber = string.Format("{0}0001", prefixLotNumber);
                maxLotNumber = string.Format("{0}1999", prefixLotNumber);
            }
            else if (other == "102A-B" || other == "102B-B")
            {
                seqNo = 2000;
                minLotNumber = string.Format("{0}2000", prefixLotNumber);
                maxLotNumber = string.Format("{0}3999", prefixLotNumber);
            }
            else if (other == "102B-C")
            {
                seqNo = 4000;
                minLotNumber = string.Format("{0}4000", prefixLotNumber);
                maxLotNumber = string.Format("{0}5999", prefixLotNumber);
            }
            else if (other == "102A-C")
            {
                seqNo = 4000;
                minLotNumber = string.Format("{0}4000", prefixLotNumber);
                maxLotNumber = string.Format("{0}6999", prefixLotNumber);
            }
            else
            {
                seqNo = 6000;
                minLotNumber = string.Format("{0}6000", prefixLotNumber);
                maxLotNumber = string.Format("{0}9999", prefixLotNumber);
            }
            PagingConfig cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format(@"Key>='{0}' AND Key<'{1}'
                                        AND IsMainLot=1"
                                        , minLotNumber
                                        , maxLotNumber),
                OrderBy = "Key DESC"
            };

            IList<Lot> lstLot = this.LotDataEngine.Get(cfg);
            if (lstLot.Count > 0)
            {
                string maxSeqNo = lstLot[0].Key.Replace(prefixLotNumber, "");
                if (int.TryParse(maxSeqNo, out seqNo))
                {
                    seqNo = seqNo + 1;
                }
            }

            for (int i = 0; i < count; i++)
            {
                lstLotNumber.Add(string.Format("{0}{1:0000}"
                                                , prefixLotNumber
                                                , seqNo + i));
            }
            return lstLotNumber;
        }
    }
}
