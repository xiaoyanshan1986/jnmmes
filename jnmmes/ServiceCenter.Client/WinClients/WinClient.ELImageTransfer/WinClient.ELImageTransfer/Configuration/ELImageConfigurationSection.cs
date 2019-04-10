using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WinClient.ELImageTransfer.Configuration
{

    /// <summary>
    /// IV测试配置节。
    /// </summary>
    public class ELImageConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// IV测试设备集合。
        /// </summary>
        [ConfigurationProperty("devices", IsDefaultCollection = false)]
        public ELImageDeviceElementCollection Devices
        {
            get
            {
                ELImageDeviceElementCollection collections = this["devices"] as ELImageDeviceElementCollection;
                return collections;
            }
        }
    }
    /// <summary>
    /// IV测试设备集合。
    /// </summary>
    public class ELImageDeviceElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 当在派生的类中重写时，创建一个新的 <see cref="T:System.Configuration.ConfigurationElement"/>。
        /// </summary>
        /// <returns>
        /// 新的 <see cref="T:System.Configuration.ConfigurationElement"/>。
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ELImageDeviceElement();
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
            return ((ELImageDeviceElement)element).Name;
        }

        /// <summary>
        /// 返回指定索引的配置元素
        /// </summary>
        /// <param name="index">索引号</param>
        /// <returns>配置元素</returns>
        public ELImageDeviceElement this[int index]
        {
            get
            {
                return (ELImageDeviceElement)BaseGet(index);
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
        new public ELImageDeviceElement this[string name]
        {
            get
            {
                return (ELImageDeviceElement)BaseGet(name);

            }
        }
        /// <summary>
        /// 返回配置元素的索引号
        /// </summary>
        /// <param name="element">配置元素</param>
        /// <returns>配置元素的索引号</returns>
        public int IndexOf(ELImageDeviceElement element)
        {
            return BaseIndexOf(element);
        }
        /// <summary>
        /// 添加配置元素到集合
        /// </summary>
        /// <param name="element">指定配置元素</param>
        public void Add(ELImageDeviceElement element)
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
        public void Remove(ELImageDeviceElement element)
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
    /// <summary>
    /// IV测试设备。
    /// </summary>
    public class ELImageDeviceElement : ConfigurationElement
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public ELImageDeviceElement()
        {
        }
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="name">设备代码名称。</param>
        /// <param name="path">设备数据所在文件夹路径。</param>
        public ELImageDeviceElement(string name, string path)
        {
            this.Name = name;
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


        /// <summary>
        /// 设备数据所在文件的格式化字符串。
        /// </summary>
        [ConfigurationProperty("format", IsRequired = false)]
        public string Format
        {
            get
            {
                return this["format"] as string;
            }
            set
            {
                this["format"] = value;
            }
        }


        [ConfigurationProperty("eqpName", IsRequired = false)]
        public string EqpName
        {
            get
            {
                return this["eqpName"] as string;
            }
            set
            {
                this["eqpName"] = value;
            }
        }


        [ConfigurationProperty("sourceImagePathRoot", IsRequired = false)]
        public string SourceImagePathRoot
        {
            get
            {
                return this["sourceImagePathRoot"] as string;
            }
            set
            {
                this["sourceImagePathRoot"] = value;
            }
        }


        [ConfigurationProperty("sourceImagePathFormat", IsRequired = false)]
        public string SourceImagePathFormat
        {
            get
            {
                return this["sourceImagePathFormat"] as string;
            }
            set
            {
                this["sourceImagePathFormat"] = value;
            }
        }


        [ConfigurationProperty("imageExtensionName", IsRequired = false)]
        public string ImageExtensionName
        {
            get
            {
                return this["imageExtensionName"] as string;
            }
            set
            {
                this["imageExtensionName"] = value;
            }
        }


        [ConfigurationProperty("isDeleteSourceImage", IsRequired = false)]
        public string IsDeleteSourceImage
        {
            get
            {
                return this["isDeleteSourceImage"] as string;
            }
            set
            {
                this["isDeleteSourceImage"] = value;
            }
        }



        [ConfigurationProperty("ftpServer", IsRequired = false)]
        public string FtpServer
        {
            get
            {
                return this["ftpServer"] as string;
            }
            set
            {
                this["ftpServer"] = value;
            }
        }


        [ConfigurationProperty("ftpUser", IsRequired = false)]
        public string FtpUser
        {
            get
            {
                return this["ftpUser"] as string;
            }
            set
            {
                this["ftpUser"] = value;
            }
        }


        [ConfigurationProperty("ftpPassword", IsRequired = false)]
        public string FtpPassword
        {
            get
            {
                return this["ftpPassword"] as string;
            }
            set
            {
                this["ftpPassword"] = value;
            }
        }


        [ConfigurationProperty("ftpTargetFolder", IsRequired = false)]
        public string FtpTargetFolder
        {
            get
            {
                return this["ftpTargetFolder"] as string;
            }
            set
            {
                this["ftpTargetFolder"] = value;
            }
        }
    }
}
