using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.Common
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum obj)
        {
            FieldInfo[] fileds = obj.GetType().GetFields();
            foreach (FieldInfo f in fileds)
            {
                if (f.Name == obj.ToString())
                {
                    object[] attrs = f.GetCustomAttributes(typeof(DisplayAttribute), false);
                    if (attrs.Length > 0)
                    {
                        string displayName=string.Empty;
                        DisplayAttribute attr = attrs[0] as DisplayAttribute;
                        if (attr.ResourceType != null)
                        {
                            displayName = Convert.ToString(attr.ResourceType
                                                            .GetProperty(attr.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                            .GetGetMethod(true)
                                                            .Invoke(null, null));
                        }
                        else
                        {
                            displayName = attr.Name;
                        }
                        return displayName;
                    }
                }
            }
            
            return obj.ToString();
        }
        public static IDictionary<T, string> GetDisplayNameDictionary<T>()
        {
            
            IDictionary<T, string> dic=new Dictionary<T,string>();
            FieldInfo[] fs = typeof(T).GetFields();
            foreach (FieldInfo f in fs)
            {
                if (f.FieldType.BaseType != typeof(T).BaseType) continue;
                string description=f.Name;
                object[] attrs = f.GetCustomAttributes(typeof(DisplayAttribute), false);
                if (attrs.Length > 0)
                {
                    DisplayAttribute attr = attrs[0] as DisplayAttribute;
                    if (attr.ResourceType!=null)
                    {
                        description = Convert.ToString(attr.ResourceType
                                                        .GetProperty(attr.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                        .GetGetMethod(true)
                                                        .Invoke(null,null));
                    }
                    else
                    {
                        description = attr.Name;
                    }
                }
                dic.Add(new KeyValuePair<T, string>((T)Enum.Parse(typeof(T), f.Name),description));
            }
            return dic;
        }
    }

    public static class DateTimeExtensions
    {
        /// <summary>
        /// 今年第几周,年的第一周从年的第一天开始，到指定周的下一个首日结束。
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public static int GetWeekOfYearFirstDay(this DateTime datetime, DayOfWeek dayOfWeek)
        {
            int weekYear = new System.Globalization.GregorianCalendar()
                                .GetWeekOfYear(datetime,
                                               System.Globalization.CalendarWeekRule.FirstDay,
                                               dayOfWeek);
            return weekYear;
        }
        /// <summary>  
        /// 得到本周第一天(以星期天为第一天)  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        public static DateTime GetWeekFirstDaySun(this DateTime datetime)
        {
            //星期天为第一天  
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            int daydiff = (-1) * weeknow;

            //本周第一天  
            string FirstDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(FirstDay);
        }

        /// <summary>  
        /// 得到本周第一天(以星期一为第一天)  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        public static DateTime GetWeekFirstDayMon(this DateTime datetime)
        {
            //星期一为第一天  
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);

            //因为是以星期一为第一天，所以要判断weeknow等于0时，要向前推6天。  
            weeknow = (weeknow == 0 ? (7 - 1) : (weeknow - 1));
            int daydiff = (-1) * weeknow;

            //本周第一天  
            string FirstDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(FirstDay);
        }

        /// <summary>  
        /// 得到本周最后一天(以星期六为最后一天)  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        public static DateTime GetWeekLastDaySat(this DateTime datetime)
        {
            //星期六为最后一天  
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            int daydiff = (7 - weeknow) - 1;

            //本周最后一天  
            string LastDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(LastDay);
        }

        /// <summary>  
        /// 得到本周最后一天(以星期天为最后一天)  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        public static DateTime GetWeekLastDaySun(this DateTime datetime)
        {
            //星期天为最后一天  
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            weeknow = (weeknow == 0 ? 7 : weeknow);
            int daydiff = (7 - weeknow);

            //本周最后一天  
            string LastDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(LastDay);
        }
    }
}
