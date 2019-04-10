using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.Interface.QAM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.QAM;

namespace ServiceCenter.MES.Service.QAM.ServiceExtensions
{
    /// <summary>
    /// 扩展批次进站，检查是否进行进站后检验操作。
    /// </summary>
    class LotTrackInForCheck : ILotTrackIn
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
        /// 检验设置数据访问类。
        /// </summary>
        public ICheckSettingDataEngine CheckSettingDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 在批次进站时，更新对应的批次为等待检验。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(TrackInParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            /*changed by victor ignore 
            DateTime now=DateTime.Now;
           
            foreach(string lotNumber in p.LotNumbers)
            {
                Lot lot=this.LotDataEngine.Get(lotNumber);
                if(lot==null){
                    continue;
                }
                //获取工序检验设置数据
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging=false,
                    Where = string.Format(@"RouteOperationName='{0}' 
                                           AND ActionName='{1}' 
                                           AND Status=1
                                           AND EXISTS(SELECT Key 
                                                      FROM CheckSettingPoint as p
                                                      WHERE p.Key.CheckSettingKey=self.Key
                                                      AND Status=1)"
                                          , p.RouteOperationName
                                          ,Convert.ToInt32(EnumCheckAction.TrackIn))
                };
                IList<CheckSetting> lstCheckSetting = this.CheckSettingDataEngine.Get(cfg);
                int count = 0;
                if(lstCheckSetting.Count>=0)
                {
                    //判断工序+批次物料+生产线+设备+工艺流程是否需要进行检验。
                    var lnq = from item in lstCheckSetting
                          where item.EquipmentCode == p.EquipmentCode
                          && item.MaterialCode == lot.MaterialCode
                          && item.ProductionLineCode == p.LineCode
                          && item.RouteStepName == lot.RouteStepName
                          select item;
                    count = lnq.Count();

                    if (count == 0)
                    {
                        //判断工序+批次物料类型+生产线+设备是否需要进行检验。
                        lnq = from item in lstCheckSetting
                                where item.EquipmentCode == p.EquipmentCode
                                && lot.MaterialCode.StartsWith(item.MaterialType??string.Empty)
                                && item.ProductionLineCode == p.LineCode
                                && item.RouteStepName == lot.RouteStepName
                                select item;
                        count = lnq.Count();
                    }
                    if (count == 0)
                    {
                        //判断工序+设备是否需要进行检验。
                        lnq = from item in lstCheckSetting
                              where item.EquipmentCode == p.EquipmentCode
                                  && string.IsNullOrEmpty(item.MaterialCode)
                                  && string.IsNullOrEmpty(item.MaterialType)
                                  && string.IsNullOrEmpty(item.ProductionLineCode)
                                  && string.IsNullOrEmpty(item.RouteStepName)
                              select item;
                        count = lnq.Count();
                    }
                    if (count == 0)
                    {
                        //判断工序+生产线是否需要进行检验。
                        lnq = from item in lstCheckSetting
                              where item.ProductionLineCode == p.LineCode
                                  && string.IsNullOrEmpty(item.EquipmentCode)
                                  && string.IsNullOrEmpty(item.MaterialCode)
                                  && string.IsNullOrEmpty(item.MaterialType)
                                  && string.IsNullOrEmpty(item.RouteStepName)
                              select item;
                        count = lnq.Count();
                    }
                    if (count == 0)
                    {
                        //判断工序+批次物料是否需要进行检验。
                        lnq = from item in lstCheckSetting
                                where item.MaterialCode == lot.MaterialCode
                                    && string.IsNullOrEmpty(item.EquipmentCode)
                                    && string.IsNullOrEmpty(item.ProductionLineCode)
                                    && string.IsNullOrEmpty(item.RouteStepName)
                                select item;
                        count = lnq.Count();
                    }
                    if (count == 0)
                    {
                        //判断工序+批次物料类型是否需要进行检验。
                        lnq = from item in lstCheckSetting
                                where lot.MaterialCode.StartsWith(item.MaterialType??string.Empty)
                                    && string.IsNullOrEmpty(item.EquipmentCode)
                                    && string.IsNullOrEmpty(item.ProductionLineCode)
                                    && string.IsNullOrEmpty(item.RouteStepName)
                                select item;
                        count = lnq.Count();
                    }
                    if (count == 0)
                    {
                        //判断工序是否需要进行检验。
                        lnq = from item in lstCheckSetting
                              where string.IsNullOrEmpty(item.EquipmentCode)
                                    && string.IsNullOrEmpty(item.MaterialCode)
                                    && string.IsNullOrEmpty(item.MaterialType)
                                    && string.IsNullOrEmpty(item.ProductionLineCode)
                                    && string.IsNullOrEmpty(item.RouteStepName)
                              select item;
                        count = lnq.Count();
                    }
                }

                if (count > 0)
                {
                    Lot lotUpdate = lot.Clone() as Lot;
                    lotUpdate.StateFlag = EnumLotState.WaitCheck;
                    this.LotDataEngine.Update(lotUpdate);
                }
            }*/
            return result;
        }

    }
}
