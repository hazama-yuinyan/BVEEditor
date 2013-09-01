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
            LogManager.GetLog = type => new BVEEditor.Logging.Log4netLogger(type);
        }

        public App()
        {
            InitializeComponent();
        }
    }
}
