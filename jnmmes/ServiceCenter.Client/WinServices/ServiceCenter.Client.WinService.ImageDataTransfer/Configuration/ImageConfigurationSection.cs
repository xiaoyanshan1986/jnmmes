using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ServiceCenter.Client.WinService.ImageDataTransfer.Transfer;

namespace ServiceCenter.Client.WinService.ImageDataTransfer.Configuration
{
    /// <summary>
    /// EL/IV图片配置节。
    /// </summary>
    public class ImageConfigurationSection:ConfigurationSection
    {
        /// <summary>
        /// EL/IV图片设备集合。
        /// </summary>
        [ConfigurationProperty("devices", IsDefaultCollection = false)]
        public ImageDeviceElementCollection Devices
        {
            get
            {
                ImageDeviceElementCollection collections = this["devices"] as ImageDeviceElementCollection;
                return collections;
            }
        }
    }
    /// <summary>
    /// EL/IV图片设备集合。
    /// </summary>
    public class ImageDeviceElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 当在派生的类中重写时，创建一个新的 <see cref="T:System.Configuration.ConfigurationElement"/>。
        /// </summary>
        /// <returns>
        /// 新的 <see cref="T:System.Configuration.ConfigurationElement"/>。
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ImageDeviceElement();
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
            return ((ImageDeviceElement)element).Name;
        }

        /// <summary>
        /// 返回指定索引的配置元素
        /// </summary>
        /// <param name="index">索引号</param>
        /// <returns>配置元素</returns>
        public ImageDeviceElement this[int index]
        {
            get
            {
                return (ImageDeviceElement)BaseGet(index);
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
        new public ImageDeviceElement this[string name]
        {
            get
            {
                return (ImageDeviceElement)BaseGet(name);

            }
        }
        /// <summary>
        /// 返回配置元素的索引号
        /// </summary>
        /// <param name="element">配置元素</param>
        /// <returns>配置元素的索引号</returns>
        public int IndexOf(ImageDeviceElement element)
        {
            return BaseIndexOf(element);
        }
        /// <summary>
        /// 添加配置元素到集合
        /// </summary>
        /// <param name="element">指定配置元素</param>
        public void Add(ImageDeviceElement element)
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
        public void Remove(ImageDeviceElement element)
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
    /// EL/IV图片设备。
    /// </summary>
    public class ImageDeviceElement:ConfigurationElement
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public ImageDeviceElement()
        {
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
        /// 设备类型。
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return this["type"] as string;
            }
            set
            {
                this["type"] = value;
            }
        }
        /// <summary>
        /// 设备数据所在文件夹根路径。
        /// </summary>
        [ConfigurationProperty("sourcePathRoot", IsRequired = true)]
        public string SourcePathRoot
        {
            get
            {
                return this["sourcePathRoot"] as string;
            }
            set
            {
                this["sourcePathRoot"] = value;
            }
        }
        /// <summary>
        /// 设备数据所在文件夹路径格式化参数。 {0:yyyy-mm-dd}
        /// </summary>
        [ConfigurationProperty("sourcePathFormat", IsRequired = true)]
        public string SourcePathFormat
        {
            get
            {
                return this["sourcePathFormat"] as string;
            }
            set
            {
                this["sourcePathFormat"] = value;
            }
        }

        /// <summary>
        /// 设备数据文件扩展名。jpg/png/gif
        /// </summary>
        [ConfigurationProperty("fileExtensionName", IsRequired = true)]
        public string FileExtensionName
        {
            get
            {
                return this["fileExtensionName"] as string;
            }
            set
            {
                this["fileExtensionName"] = value;
            }
        }


        /// <summary>
        /// 是否删除源文件。如果不删除源文件会将源文件移动到源文件夹下的一个特殊文件夹LocalFiles下。
        /// </summary>
        [ConfigurationProperty("isDeleteSourceFile", IsRequired = true)]
        public bool IsDeleteSourceFile
        {
            get
            {
                bool isDeleteSourceFile = false;
                string obj = Convert.ToString(this["isDeleteSourceFile"]);
                bool.TryParse(obj, out isDeleteSourceFile);
                return isDeleteSourceFile;
            }
            set
            {
                this["isDeleteSourceFile"] = value;
            }
        }
        /// <summary>
        /// 设备数据目标文件夹根路径。
        /// </summary>
        [ConfigurationProperty("targetPathRoot", IsRequired = true)]
        public string TargetPathRoot
        {
            get
            {
                return this["targetPathRoot"] as string;
            }
            set
            {
                this["targetPathRoot"] = value;
            }
        }

        /// <summary>
        /// HTTP访问根路径。
        /// </summary>
        [ConfigurationProperty("httpPathRoot", IsRequired = true)]
        public string HttpPathRoot
        {
            get
            {
                return this["httpPathRoot"] as string;
            }
            set
            {
                this["httpPathRoot"] = value;
            }
        }
    }
}
