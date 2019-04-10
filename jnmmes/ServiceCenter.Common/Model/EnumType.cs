using ServiceCenter.Common.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.Model
{
    /// <summary>
    /// 对象状态枚举。
    /// </summary>
    public enum EnumObjectStatus
    {
        /// <summary>
        /// 禁用。
        /// </summary>
        [Display(Name = "EnumObjectStatus_Disabled", ResourceType = typeof(StringResource))]
        Disabled=0,
        /// <summary>
        /// 启用。
        /// </summary>
        [Display(Name = "EnumObjectStatus_Available", ResourceType = typeof(StringResource))]
        Available=1
    }

    /// <summary>
    /// 打印机类型。
    /// </summary>
    public enum EnumPrinterType
    {
        /// <summary>
        /// 网络打印机。
        /// </summary>
        [Display(Name = "EnumPrinterType_Network", ResourceType = typeof(StringResource))]
        Network = 0,
        /// <summary>
        /// 串口打印机。
        /// </summary>
        [Display(Name = "EnumPrinterType_Serial", ResourceType = typeof(StringResource))]
        Serial = 1,
        /// <summary>
        /// 并口打印机。
        /// </summary>
        [Display(Name = "EnumPrinterType_Parallel", ResourceType = typeof(StringResource))]
        Parallel = 2,
        /// <summary>
        /// 本地打印机。
        /// </summary>
        [Display(Name = "EnumPrinterType_RAW", ResourceType = typeof(StringResource))]
        RAW = 3
    }

    /// <summary>
    /// 参数类型。
    /// </summary>
    public enum EnumParameterType
    {
        /// <summary>
        /// 工序参数。
        /// </summary>
        [Display(Name = "EnumParameterType_Route", ResourceType = typeof(StringResource))]
        Route = 0,
        /// <summary>
        /// 采集参数。
        /// </summary>
        [Display(Name = "EnumParameterType_EDC", ResourceType = typeof(StringResource))]
        EDC = 1,
        /// <summary>
        /// 检验参数。
        /// </summary>
        [Display(Name = "EnumParameterType_Check", ResourceType = typeof(StringResource))]
        Check = 2
    }
    /// <summary>
    /// 参数数据类型。
    /// </summary>
    public enum EnumDataType
    {
        /// <summary>
        /// 字符串类型。
        /// </summary>
        [Display(Name = "EnumDataType_String", ResourceType = typeof(StringResource))]
        String = 0,
        /// <summary>
        /// 整数类型。
        /// </summary>
        [Display(Name = "EnumDataType_Integer", ResourceType = typeof(StringResource))]
        Integer = 1,
        /// <summary>
        /// 日期类型。
        /// </summary>
        [Display(Name = "EnumDataType_Date", ResourceType = typeof(StringResource))]
        Date = 2,
        /// <summary>
        /// 日期时间类型。
        /// </summary>
        [Display(Name = "EnumDataType_DateTime", ResourceType = typeof(StringResource))]
        DateTime = 3,
        /// <summary>
        /// 布尔类型。
        /// </summary>
        [Display(Name = "EnumDataType_Boolean", ResourceType = typeof(StringResource))]
        Boolean = 4,
        /// <summary>
        /// 浮点类型。
        /// </summary>
        [Display(Name = "EnumDataType_Float", ResourceType = typeof(StringResource))]
        Float = 5
    }

    /// <summary>
    /// 参数对应设备类型。
    /// </summary>
    public enum EnumDeviceType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Display(Name = "EnumDeviceType_None", ResourceType = typeof(StringResource))]
        None = 0,
        /// <summary>
        /// RS232设备
        /// </summary>
        [Display(Name = "EnumDeviceType_RS232", ResourceType = typeof(StringResource))]
        RS232 = 1,
        /// <summary>
        /// TCPIP设备
        /// </summary>
        [Display(Name = "EnumDeviceType_TCPIP", ResourceType = typeof(StringResource))]
        TCPIP = 2,
        /// <summary>
        /// 其他设备
        /// </summary>
        [Display(Name = "EnumDeviceType_Other", ResourceType = typeof(StringResource))]
        Other = 9
    }
}
