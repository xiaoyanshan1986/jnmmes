using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ZPVM.Resources;

namespace ServiceCenter.MES.Model.ZPVM
{
    ///// <summary>
    ///// 表示I铭牌数据主键。
    ///// </summary>
    //public struct MinPaiDataKey
    //{
    //    /// <summary>
    //    /// 批次号。
    //    /// </summary>
    //    public string LotNumber { get; set; }
    //    /// <summary>
    //    /// 组件标签模板路径地址  需要以http格式进行提供。
    //    /// </summary>
    //    public string LablePath { get; set; }
    //    /// <summary>
    //    /// 打印数量。
    //    /// </summary>
    //    public int PrintQty { get; set; }
    //}
    /// <summary>
    /// 描述铭牌数据的模型
    /// </summary>
    [DataContract]
    //public class MinPaiData : BaseModel<MinPaiDataKey>
    public class MinPaiData
    {
        ///// <summary>
        ///// 车间名称。
        ///// </summary>
        //[DataMember]
        //public virtual string FactoryName { get; set; }       
        ///// <summary>
        ///// 产品类型。
        ///// </summary>
        //[DataMember]
        //public virtual string ProductModel { get; set; }       
        ///// <summary>
        ///// 满托盘数量。
        ///// </summary>
        //[DataMember]
        //public virtual int FullPalletQty { get; set; }
        ///// <summary>
        ///// 组件电池片数量。
        ///// </summary>
        //[DataMember]
        //public virtual int LotCellQty { get; set; }              
        ///// <summary>
        ///// 功率分档代码对应的最小功率。
        ///// </summary>
        //[DataMember]
        //public virtual string MinPower { get; set; }
        ///// <summary>
        ///// 功率分档代码对应的最大功率。
        ///// </summary>
        //[DataMember]
        //public virtual string MaxPower { get; set; }               



        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string LotNumber { get; set; }
        /// <summary>
        /// 组件标签模板路径地址  需要以http格式进行提供。
        /// </summary>

        [DataMember]
        public virtual string LablePath { get; set; }
        /// <summary>
        /// 打印数量。
        /// </summary>

        [DataMember]
        public virtual int PrintQty { get; set; }

        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public virtual string WorkOrderNumber { get; set; }

        /// <summary>
        /// 产品编码。
        /// </summary>
        [DataMember]
        public virtual string ProductNumber { get; set; }


        /// <summary>
        /// 档位名称。
        /// </summary>
        [DataMember]
        public virtual string PowerName { get; set; }

        /// <summary>
        /// 组件等级。
        /// </summary>
        [DataMember]
        public virtual string Grade { get; set; }

        /// <summary>
        /// 组件花色。
        /// </summary>
        [DataMember]
        public virtual string Color { get; set; }

        /// <summary>
        /// 产品型号。
        /// </summary>
        [DataMember]
        public virtual string ProductType { get; set; }

        /// <summary>
        /// 尺寸规格。
        /// </summary>
        [DataMember]
        public virtual string ProductSpec { get; set; }

        /// <summary>
        /// 最大功率(Pmax) 。
        /// </summary>
        [DataMember]
        public virtual double StandardPower { get; set; }

        /// <summary>
        /// 功率分档代码对应的标准短路电流。
        /// </summary>
        [DataMember]
        public virtual double StandardIsc { get; set; }

        /// <summary>
        /// 功率分档代码对应的标准工作电流。
        /// </summary>
        [DataMember]
        public virtual double StandardIPM { get; set; }

        /// <summary>
        /// 功率分档代码对应的标准开路电压。
        /// </summary>
        [DataMember]
        public virtual double StandardVoc { get; set; }

        /// <summary>
        /// 功率分档代码对应的标准工作电压。
        /// </summary>
        [DataMember]
        public virtual double StandardVPM { get; set; }

        /// <summary>
        /// 功率分档代码对应的标准填充因子。
        /// </summary>
        [DataMember]
        public virtual double StandardFuse { get; set; }
        /// <summary>
        /// 分档方式。 功率误差
        /// </summary>
        [DataMember]
        public virtual string PowerDifference { get; set; }

        /// <summary>
        /// 实测功率(Pmax) 。
        /// </summary>
        [DataMember]
        public virtual double PM { get; set; }

        /// <summary>
        /// 组件实测工作电流。
        /// </summary>
        [DataMember]
        public virtual double IPM { get; set; }

        /// <summary>
        /// 组件实测短路电流。
        /// </summary>
        [DataMember]
        public virtual double ISC { get; set; }

        /// <summary>
        /// 组件实测开路电压。
        /// </summary>
        [DataMember]
        public virtual double VOC { get; set; }

        ///// <summary>
        ///// 组件实测工作电压。
        ///// </summary>
        [DataMember]
        public virtual double VPM { get; set; }

        /// <summary>
        /// 组件实测填充因子。
        /// </summary>
        [DataMember]
        public virtual double FF { get; set; }

        /// <summary>
        /// 组件实测时间。
        /// </summary>
        [DataMember]
        public virtual string TestTime { get; set; }
        /// <summary>
        /// 组件测试时的温度。
        /// </summary>
        [DataMember]
        public virtual string TestTmemperature { get; set; }

        /// <summary>
        /// 子分挡项目号。
        /// </summary>
        [DataMember]
        public virtual int PowersetItemNo { get; set; }
        /// <summary>
        /// 子分挡代码。
        /// </summary>
        [DataMember]
        public virtual string PowersetSubCode { get; set; }

        /// <summary>
        /// 功率分档代码。
        /// </summary>
        [DataMember]
        public virtual string PowersetCode { get; set; }

        /// <summary>
        /// 铭牌JSON数据。
        /// </summary>
        [DataMember]
        public virtual string MinPaiJson { get; set; }
        
        /// <summary>
        /// 相关错误信息。
        /// </summary>
        [DataMember]
        public virtual string ErrorMessage { get; set; }
       
    }

    public class MinPaiPrint
    {
        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string LotNumber { get; set; }

        /// <summary>
        /// 分档方式。 功率误差
        /// </summary>
        [DataMember]
        public virtual string PowerDifference { get; set; }

        /// <summary>
        /// 尺寸规格。
        /// </summary>
        [DataMember]
        public virtual string ProductSpec { get; set; }

        /// <summary>
        /// 产品型号。
        /// </summary>
        [DataMember]
        public virtual string ProductType { get; set; }

        /// <summary>
        /// 最大功率(Pmax) 。
        /// </summary>
        [DataMember]
        public virtual double StandardPower { get; set; }

        /// <summary>
        /// 功率分档代码对应的标准短路电流。
        /// </summary>
        [DataMember]
        public virtual double StandardIsc { get; set; }

        /// <summary>
        /// 功率分档代码对应的标准工作电流。
        /// </summary>
        [DataMember]
        public virtual double StandardIPM { get; set; }

        /// <summary>
        /// 功率分档代码对应的标准开路电压。
        /// </summary>
        [DataMember]
        public virtual double StandardVoc { get; set; }

        /// <summary>
        /// 功率分档代码对应的标准工作电压。
        /// </summary>
        [DataMember]
        public virtual double StandardVPM { get; set; }

        /// <summary>
        /// 功率分档代码对应的标准填充因子。
        /// </summary>
        [DataMember]
        public virtual double StandardFuse { get; set; }

        /// <summary>
        /// 子分挡代码。
        /// </summary>
        [DataMember]
        public virtual string PowersetSubCode { get; set; }

        /// <summary>
        /// 电池片效率。
        /// </summary>
        [DataMember]
        public virtual double ModuleEfficiency { get; set; }
    }
    public class WebForBin
    {
        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string LotNumber { get; set; }

        /// <summary>
        /// 线别。
        /// </summary>
        [DataMember]
        public virtual string LineName { get; set; }
    }
}
