using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BVEEditor.AvalonDock;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// WorkbenchView.xaml の相互作用ロジック
    /// </summary>
    public partial class WorkbenchView : Window, IDockingManagerProvider
    {
        public WorkbenchView()
        {
            InitializeComponent();
        }

        #region IDockingManagerProvider メンバー

        public Xceed.Wpf.AvalonDock.DockingManager DockingManager
        {
            get { return dock_manager; }
        }

        #endregion
    }
}
