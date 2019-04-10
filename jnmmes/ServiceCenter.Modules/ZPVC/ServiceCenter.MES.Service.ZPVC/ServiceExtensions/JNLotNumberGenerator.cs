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

namespace ServiceCenter.MES.Service.ZPVC.ServiceExtensions
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

        public IList<string> Generate(EnumLotType lotType, string orderNumber, int count, string prefix)
        {
            //电池片批次号规则说明
            //C3 P 140629 - 0001

            IList<string> lstLotNumber = new List<string>();
            //获取工单信息
            WorkOrder wo = this.WorkOrderDataEngine.Get(orderNumber);
            if (wo == null)
            {
                return lstLotNumber;
            }
            //110101	P型156单晶自制电池片
            //110102	P型156多晶自制电池片
            string type=string.Empty;
            if (wo.MaterialCode.StartsWith("110101"))
            {
                type = "M";
            }
            else if (wo.MaterialCode.StartsWith("110102"))
            {
                type = "P";
            }
            else
            {
                type = "S";
            }
            DateTime now = DateTime.Now;
            //7点为分割时间，7点前创建的批次使用前一天的流水号。
            DateTime splitTime = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0);
            if (now < splitTime)
            {
                now = now.AddDays(-1);
            }
            int year = Convert.ToInt32(now.ToString("yy"));
            string prefixLotNumber = string.Format("{0}{1}{2}{3}{4}-"
                                                    , prefix
                                                    , type
                                                    , year.ToString("00")
                                                    , now.Month.ToString("00")
                                                    , now.Day.ToString("00"));
            int seqNo = 1;
            PagingConfig cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format(@"Key LIKE '{0}%' 
                                        AND IsMainLot=1"
                                        , prefixLotNumber),
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
