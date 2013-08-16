using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Events;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor.Views
{
    /// <summary>
    /// The view model for the status bar.
    /// </summary>
    public class StatusBarViewModel : PropertyChangedBase, IHandle<CaretPositionChangedEvent>
    {
        #region Binding sources
        public string MessageText{
            get; set;
        }

        public string CursorPositionText{
            get; set;
        }

        public string ModeName{
            get; set;
        }
        #endregion

        #region IHandle<CaretPositionChangedEvent> メンバー

        public void Handle(CaretPositionChangedEvent message)
        {
            CursorPositionText = StringParser.Parse(
                "${res:MainWindow_StatusBar_CursorPanelTex}",
                new StringTagPair("Line", string.Format("{0,-10}", message.Line)),
                new StringTagPair("Column", string.Format("{0,-5}", message.Column)),
                new StringTagPair("Character", string.Format("{0,-5}", message.CharNumber))
            );
        }

        #endregion
    }
}
