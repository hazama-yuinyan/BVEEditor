using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion.Events
{
    /// <summary>
    /// Event occurred when the selection of text is changed.
    /// </summary>
    public class SelectionChangedEvent : IPopupEvent
    {
        int offset;

        public SelectionChangedEvent(int offset)
        {
            this.offset = offset;
        }

        #region IPopupEvent メンバー

        public EventType Type{
            get{return EventType.SelectionChanged;}
        }

        public EventSource Source{
            get{return EventSource.Popup;}
        }

        public object EventArgs{
            get{return offset;}
        }

        #endregion
    }
}
