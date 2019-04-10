using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.EDC.Resources;

namespace ServiceCenter.MES.Model.EDC
{
    /// <summary>
    /// 采集数据事务状态
    /// </summary>
    public enum EnumAcquisitionTransActivity
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Display(Name = "AcquisitionDataTransActivity_New", ResourceType = typeof(StringResource))]
        New = 0,
        /// <summary>
        /// 修改
        /// </summary>
        [Display(Name = "AcquisitionDataTransActivity_Modify", ResourceType = typeof(StringResource))]
        Modify = 1,
        /// <summary>
        /// 审核
        /// </summary>
        [Display(Name = "AcquisitionDataTransActivity_Audit", ResourceType = typeof(StringResource))]
        Audit = 2,
        /// <summary>
        /// 取消审核
        /// </summary>
        [Display(Name = "AcquisitionDataTransActivity_CancelAudit", ResourceType = typeof(StringResource))]
        CancelAudit = -2,
        /// <summary>
        /// 删除
        /// </summary>
        [Display(Name = "AcquisitionDataState_Delete", ResourceType = typeof(StringResource))]
        Delete = -1
    }

    /// <summary>
    /// 表示项目数据主键
    /// </summary>
    public struct DataAcquisitionTransKey
    {
        /// <summary>
        /// 采集事务主键
        /// </summary>
        public string TransactionKey { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        [DataMember]
        public DateTime EDCTime { get; set; }

        /// <summary>
        /// 采集项目代码
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 采集字段
        /// </summary>
        public string FieldCode { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        public string LineCode { get; set; }

        /// <summary>
        /// 设备代码
        /// </summary>
        public string EquipmentCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}", this.EDCTime, this.ItemCode, this.FieldCode, this.LocationName, this.LineCode, this.EquipmentCode);
        }
    }

    /// <summary>
    /// 表示采集数据数据
    /// </summary>
    [DataContract]
    public class DataAcquisitionTrans : BaseModel<DataAcquisitionTransKey>
    {
        public DataAcquisitionTrans()
        {
        }

        /// <summary>
        /// 根据数据采集类创建对应的事务对象
        /// </summary>
        /// <param name="transactionKey">事务主键</param>
        /// <param name="obj">数据采集对象</param>
        /// <param name="activity">事务类型</param>
        public DataAcquisitionTrans(string transactionKey, DataAcquisitionData obj, EnumAcquisitionTransActivity activity)
        {
            this.Key = new DataAcquisitionTransKey()
                        {
                            TransactionKey = transactionKey,
                            ItemCode = obj.Key.ItemCode,
                            EDCTime = obj.Key.EDCTime,
                            FieldCode = obj.Key.FieldCode,
                            LocationName = obj.Key.LocationName,
                            LineCode = obj.Key.LineCode,
                            EquipmentCode = obj.Key.EquipmentCode
                        };
            this.Activity = activity;
            this.DataValue = obj.DataValue;
            this.DataState = obj.DataState;
            this.CreateTime = obj.CreateTime;
            this.Creator = obj.Creator;
            this.AuditTime = obj.AuditTime;
            this.Auditor = obj.Auditor;
            this.EditTime = obj.EditTime;
            this.Editor = obj.Editor;
        }

        /// <summary>
        /// 事务类型
        /// </summary>
        [DataMember]
        public virtual EnumAcquisitionTransActivity Activity { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [DataMember]
        public virtual string DataValue { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public virtual EnumAcquisitionDataState DataState { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateTime { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public virtual string Auditor { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        [DataMember]
        public virtual DateTime? AuditTime { get; set; }

        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual string Editor { get; set; }

        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? EditTime { get; set; }
    }
}
