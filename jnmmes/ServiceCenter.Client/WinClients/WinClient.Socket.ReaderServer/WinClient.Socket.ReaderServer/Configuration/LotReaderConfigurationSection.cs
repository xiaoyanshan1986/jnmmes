using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WinClient.Socket.ReaderServer.Configuration
{
    /// <summary>
    /// EL/IV图片配置节。
    /// </summary>
    public class LotReaderConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// EL/IV图片设备集合。
        /// </summary>
        [ConfigurationProperty("devices", IsDefaultCollection = false)]
        public LotReaderDeviceElementCollection Devices
        {
            get
            {
                LotReaderDeviceElementCollection collections = this["devices"] as LotReaderDeviceElementCollection;
                return collections;
            }
        }
    }
    /// <summary>
    /// EL/IV图片设备集合。
    /// </summary>
    public class LotReaderDeviceElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 当在派生的类中重写时，创建一个新的 <see cref="T:System.Configuration.ConfigurationElement"/>。
        /// </summary>
        /// <returns>
        /// 新的 <see cref="T:System.Configuration.ConfigurationElement"/>。
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LotReaderDeviceElement();
        }

        /// <summary>
        /// 在派生类中重写时获取指定配置元素的元素键。
        /// </summary>
        /// <param name="element">要为其返回键的 <see cref="T:System.Configuration.ConfigurationElement"/>。</param>
        /// <returns>
        /// 一个 <see cref="T:System.Object"/>，用作指定 <see cref="T:System.Configuration.ConfigurationElement"/> 的键。
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LotReaderDeviceElement)element).Name;
        }

        /// <summary>
        /// 返回指定索引的配置元素
        /// </summary>
        /// <param name="index">索引号</param>
        /// <returns>配置元素</returns>
        public LotReaderDeviceElement this[int index]
        {
            get
            {
                return (LotReaderDeviceElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// 返回具有指定键的配置元素
        /// </summary>
        /// <param name="name">配置元素名称</param>
        /// <returns>配置元素</returns>
        new public LotReaderDeviceElement this[string name]
        {
            get
            {
                return (LotReaderDeviceElement)BaseGet(name);

            }
        }
        /// <summary>
        /// 返回配置元素的索引号
        /// </summary>
        /// <param name="element">配置元素</param>
        /// <returns>配置元素的索引号</returns>
        public int IndexOf(LotReaderDeviceElement element)
        {
            return BaseIndexOf(element);
        }
        /// <summary>
        /// 添加配置元素到集合
        /// </summary>
        /// <param name="element">指定配置元素</param>
        public void Add(LotReaderDeviceElement element)
        {
            BaseAdd(element);
        }
        /// <summary>
        /// 添加配置元素到集合
        /// </summary>
        /// <param name="element">指定配置元素</param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
        /// <summary>
        /// 从集合中移除指定配置元素
        /// </summary>
        /// <param name="element">指定配置元素</param>
        public void Remove(LotReaderDeviceElement element)
        {
            if (BaseIndexOf(element) > 0)
                BaseRemove(element.Name);
        }
        /// <summary>
        /// 从集合中移除指定配置元素
        /// </summary>
        /// <param name="index">索引号</param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
        /// <summary>
        /// 从集合中移除指定配置元素
        /// </summary>
        /// <param name="name">配置元素名</param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }
        /// <summary>
        /// 从集合中移除所有配置元素对象。
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }
    }
    public class LotReaderDeviceElement : ConfigurationElement
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public LotReaderDeviceElement()
        {
        }
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="name">设备代码名称。</param>
        /// <param name="path">设备数据所在文件夹路径。</param>
        public LotReaderDeviceElement(string name, string path)
        {
            this.Name = name;
            //this.Path = path;///
        }
        /// <summary>
        /// 设备代码名称。唯一标识设备的属性。
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("ReaderIP", IsRequired = false)]
        public string ReaderIP
        {
            get
            {
                return this["ReaderIP"] as string;
            }
            set
            {
                this["ReaderIP"] = value;
            }
        }

        [ConfigurationProperty("LotNumber", IsRequired = false)]
        public string LotNumber
        {
            get
            {
                return this["LotNumber"] as string;
            }
            set
            {
                this["LotNumber"] = value;
            }
        }
        [ConfigurationProperty("FirstEquipmentCode", IsRequired = false)]
        public string FirstEquipmentCode
        {
            get
            {
                return this["FirstEquipmentCode"] as string;
            }
            set
            {
                this["FirstEquipmentCode"] = value;
            }
        }
        [ConfigurationProperty("LineCode", IsRequired = false)]
        public string LineCode
        {
            get
            {
                return this["LineCode"] as string;
            }
            set
            {
                this["LineCode"] = value;
            }
        }
        [ConfigurationProperty("FirstStepCode", IsRequired = false)]
        public string FirstStepCode
        {
            get
            {
                return this["FirstStepCode"] as string;
            }
            set
            {
                this["FirstStepCode"] = value;
            }
        }
        [ConfigurationProperty("SecondEquipmentCode", IsRequired = false)]
        public string SecondEquipmentCode
        {
            get
            {
                return this["SecondEquipmentCode"] as string;
            }
            set
            {
                this["SecondEquipmentCode"] = value;
            }
        }
        
        [ConfigurationProperty("SecondStepCode", IsRequired = false)]
        public string SecondStepCode
        {
            get
            {
                return this["SecondStepCode"] as string;
            }
            set
            {
                this["SecondStepCode"] = value;
            }
        }
        [ConfigurationProperty("WorkShop", IsRequired = false)]
        public string WorkShop
        {
            get
            {
                return this["WorkShop"] as string;
            }
            set
            {
                this["WorkShop"] = value;
            }
        }
        [ConfigurationProperty("WorkShopId", IsRequired = false)]
        public string WorkShopId
        {
            get
            {
                return this["WorkShopId"] as string;
            }
            set
            {
                this["WorkShopId"] = value;
            }
        }
        [ConfigurationProperty("FlowId", IsRequired = false)]
        public string FlowId
        {
            get
            {
                return this["FlowId"] as string;
            }
            set
            {
                this["FlowId"] = value;
            }
        }
        [ConfigurationProperty("FlowSubId", IsRequired = false)]
        public string FlowSubId
        {
            get
            {
                return this["FlowSubId"] as string;
            }
            set
            {
                this["FlowSubId"] = value;
            }
        }

    }
}
