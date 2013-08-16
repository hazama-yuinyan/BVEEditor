using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BVEEditor.Logging
{
    public class Log4netLogger : ILog
    {
        readonly log4net.ILog inner_logger;

        public Log4netLogger(Type type)
        {
            inner_logger = log4net.LogManager.GetLogger(type);
        }

        public static Log4netLogger Instance{
            get{
                return (Log4netLogger)LogManager.GetLog(typeof(Log4netLogger));
            }
        }

        #region ILog メンバー

        public void Error(Exception exception)
        {
            inner_logger.Error(exception.Message, exception);
        }

        public void Info(string format, params object[] args)
        {
            inner_logger.InfoFormat(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            inner_logger.WarnFormat(format, args);
        }

        #endregion

        public void Error(string format, params object[] args)
        {
            inner_logger.ErrorFormat(format, args);
        }

        public void Debug(string format, params object[] args)
        {
            inner_logger.DebugFormat(format, args);
        }
    }
}
