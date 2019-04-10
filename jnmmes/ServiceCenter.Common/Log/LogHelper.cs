using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace ServiceCenter.Common
{
    public class LogHelper
    {
        private static readonly ILog logger = LogManager.GetLogger("log");
        public static void WriteLogInfo(string info)
        {

            if (log4net.LogManager.GetLogger("log").IsInfoEnabled)
            {
                log4net.LogManager.GetLogger("log").Info(info);
                //log4net.Core.Level.Error
            }
        }

        public static void WriteLogError(string info, Exception se)
        {
            if (log4net.LogManager.GetLogger("log").IsErrorEnabled)
            {
                log4net.LogManager.GetLogger("log").Error(info, se);     
                
            }
        }

        public static void WriteLogError(string errInfo)
        {
            if (log4net.LogManager.GetLogger("log").IsErrorEnabled)
            {
                log4net.LogManager.GetLogger("log").Error(errInfo);

            }
        }  
    }
}
