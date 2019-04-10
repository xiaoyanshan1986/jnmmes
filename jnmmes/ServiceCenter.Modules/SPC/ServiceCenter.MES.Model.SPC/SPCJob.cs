using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.SPC.Resources;

namespace ServiceCenter.MES.Model.SPC
{
    //public enum EnumJobType
    //{

    //    /// <summary>
    //    /// 实时作业。
    //    /// </summary>
    //    [Display(Name = "EnumJobType_RealTimeJob", ResourceType = typeof(StringResource))]
    //    RealTimeJob = 0,
    //    [Display(Name = "EnumJobType_NorRealTimeJob", ResourceType = typeof(StringResource))]
    //    NorRealTimeJob = 1

    //}

   [DataContract]
    public class SPCJob : BaseModel<string>
    {



      //[JOB_ID],[JOB_NAME],[JOB_TYPE],[LINE_CODE],[ROUTE_STEP_NAME],[EQUIPMENT_CODE]


        /// <summary>
        /// 主键（ID）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public virtual string JobName { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public virtual string Type { set; get; }
        /// <summary>
        /// 线别
        /// </summary>
        [DataMember]
        public virtual string LineCode { set; get; }
        /// <summary>
        /// 工步
        /// </summary>
        [DataMember]
        public virtual string RouteStepName { set; get; }
        /// <summary>
        /// 设备号
        /// </summary>
        [DataMember]
        public virtual string EquipmentCode { set; get; }
        //[SLOT_CODE],[ATTR_1] ,[ATTR_2],[ATTR_3],[ATTR_4],[ATTR_5],[JOB_STARTTIME]
        /// <summary>
        /// 位置
        /// </summary>
        [DataMember]
        public virtual string SlotCode { set; get; }

        [DataMember]
        public virtual string Attr1 { set; get; }
        [DataMember]
        public virtual string Attr2 { set; get; }
        [DataMember]
        public virtual string Attr3 { set; get; }
        [DataMember]
        public virtual string Attr4 { set; get; }
        [DataMember]
        public virtual string Attr5 { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public virtual DateTime? JobStartTime { set; get; }
        //[JOB_ENDTIME] ,[PARAM_NAME],[CHART_TYPE] ,[POINT_TYPE] ,[STATUS],[ISVALID]
        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public virtual DateTime? JobEndTime { set; get; }
        /// <summary>
        /// 参数名称
        /// </summary>
        [DataMember]
        public virtual string ParamName { set; get; }
        /// <summary>
        /// 图表类型
        /// </summary>
        [DataMember]
        public virtual string ChartType { set; get; }

        /// <summary>
        /// 要点类型
        /// </summary>
        [DataMember]
        public virtual string PointType { set; get; }



        [DataMember]
        public virtual string LinkAction { set; get; }
        [DataMember]
        public virtual string LinkController { set; get; }
        [DataMember]
        public virtual string LinkArea { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public virtual int? Status { set; get; }
        /// <summary>
        /// 是否有效
        /// </summary>
        [DataMember]
        public virtual int? IsvalID { set; get; }
        //[CREATOR] ,[CREATE_TIME] ,[EDITOR],[EDIT_TIME]

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public virtual string Creator { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateTime { set; get; }
        /// <summary>
        /// 编辑人
        /// </summary>
        [DataMember]
        public virtual string Editor { set; get; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        [DataMember]
        public virtual DateTime? EditTime { set; get; }
    }
}
