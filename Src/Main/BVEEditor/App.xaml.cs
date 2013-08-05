using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace BVEEditor
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            var old_get_log = LogManager.GetLog;
            LogManager.GetLog = type => {
                if(type == typeof(ICSharpCode.Core.DebugLogger))
                    return new ICSharpCode.Core.DebugLogger(type);
                else if(type == typeof(BVEEditor.Logging.Log4netLogger))
                    return new BVEEditor.Logging.Log4netLogger(type);
                else
                    return old_get_log(type);
            };
        }

        public App()
        {
            InitializeComponent();
        }
    }
}
