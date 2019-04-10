using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resource;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Service.WIP.LotTrackInServiceExtend
{
    /// <summary>
    /// 扩展批次进站前检查，用于检查校准板周期是否符合进站要求。
    /// </summary>
    class LotTrackInCheckCalibrationCycle : ILotTrackCheck
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
        /// <summary>
        /// 校准板数据访问类。
        /// </summary>
        public IDeviceCalibrationDataEngine DeviceCalibrationDataEngine
        {
            get;
            set;
        }
        /// <summary>
        //  工单产品数据访问类。
        /// </summary>
        public IWorkOrderProductDataEngine WorkOrderProductDataEngine
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
            Lot obj = this.LotDataEngine.GetByLotNumber(p.LotNumber);
            //判断工序是否配置了进行校准板检查
            RouteOperationAttribute attr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = p.RouteOperationName,
                AttributeName = "IsCheckCalibrationCycle"
            });
            bool isCheck = false;
            if (attr != null)
            {
                bool.TryParse(attr.Value, out isCheck);
            }
            //if 进行检查
            if (isCheck)
            {
                //根据设备代码抓取校准版号、当前时间以及最后测试时间。
                DeviceCalibration deviceCalibObj = this.DeviceCalibrationDataEngine.Get(p.EquipmentName);
                //    根据工单号和产品料号抓取校准板周期(300分钟)
                WorkOrderProduct wpObj = this.WorkOrderProductDataEngine.Get(obj.OrderNumber,obj.PartNumber);
                //    判断 当前时间-最后测试时间是否大于校准板周期
                DateTime dtCalibrationTime=DateTime.MinValue;
                if(deviceCalibObj!=null && deviceCalibObj.CalibrationTime!=null)
                {
                    dtCalibrationTime=deviceCalibObj.CalibrationTime.Value;
                }
                double cycle = 300;
                if (wpObj.CalibrationCycle!=null)
                {
                    cycle=wpObj.CalibrationCycle.Value;
                }
                //if 大于
                if ((now - dtCalibrationTime).TotalMinutes > cycle)
                {
                    result.Code = 1301;
                    result.Message = string.Format(StringResource.TrackIn_CalibrationIsTimeout,
                                                 p.EquipmentName,
                                                 cycle);

                }//end
            }//endif 
            return result;
        }
    }
}
