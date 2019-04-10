using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resource;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Service.WIP.LotTrackInServiceExtend
{
    /// <summary>
    /// 扩展批次进站前检查，用于检查批次数量是否符合进站数量。
    /// </summary>
    class LotTrackInCheckQuantity : ILotTrackCheck
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
        /// 工序属性数据访问类。
        /// </summary>
        public IRouteOperationAttributeDataEngine RouteOperationAttributeDataEngine
        {
            get;
            set;
        }

        public MethodReturnResult Check(TrackParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            DateTime now = DateTime.Now;
            //判断工序是否配置了进行进站时检查数量是否和创批时数量一样，不满足条件不允许进站。
            RouteOperationAttribute attr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = p.RouteOperationName,
                AttributeName = "IsCheckQuantityAtTrackIn"
            });
            
            bool isCheck = false;
            if (attr != null)
            {
                bool.TryParse(attr.Value, out isCheck);
            }
            //进行固化时间检查。
            if (isCheck)
            {
                //获取批次数据
                Lot obj = this.LotDataEngine.GetByLotNumber(p.LotNumber);
                if (obj != null && obj.Quantity!=obj.InitialQuantity)
                {
                    result.Code = 1304;
                    result.Message = String.Format(StringResource.TrackIn_QuanlityIsError,
                                                   p.LotNumber,obj.Quantity);
                }
            }
            return result;
        }
    }
}
