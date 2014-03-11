using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion.Events
{
    public enum PopupState{
        Open,
        Closed
    }

    /// <summary>
    /// Event occurred when the state of a popup is changed.
    /// </summary>
    public class PopupStateChangedEvent : IPopupEvent
    {
        PopupState state;

        public PopupStateChangedEvent(PopupState state)
        {
            this.state = state;
        }

        #region IPopupEvent メンバー

        public EventType Type{
            get{return EventType.PopupStateChanged;}
        }

        public EventSource Source{
            get{return EventSource.Popup;}
        }

        public object EventArgs{
            get{return state;}
        }

        #endregion
    }
}
