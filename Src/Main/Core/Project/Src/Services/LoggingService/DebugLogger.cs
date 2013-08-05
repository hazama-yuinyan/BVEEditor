using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ICSharpCode.Core
{
    /// <summary>
    /// An <see cref="Caliburn.Micro.ILog"/> implementation that writes logs to <see cref="System.Diagnostics.Debug"/>.
    /// </summary>
    public class DebugLogger : ILog
    {
        readonly Type type;

        public DebugLogger(Type type)
        {
            this.type = type;
        }

        public static DebugLogger Instance{
            get{
                return (DebugLogger)LogManager.GetLog(typeof(DebugLogger));
            }
        }

        #region Helper method
        static string CreateLogMessage(string format, params object[] args)
        {
            return string.Format("[{0}] {1}", DateTime.Now.ToString("o"), string.Format(format, args));
        }
        #endregion

        #region ILog メンバー

        public void Error(Exception exception)
        {
            if(exception.InnerException != null)
                Debug.Fail(exception.Message, exception.InnerException.Message);
            else
                Debug.Fail(exception.Message);
        }

        public void Info(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "INFO");
        }

        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "WARNING");
        }

        #endregion

        public void Error(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "ERROR");
        }
    }
}
