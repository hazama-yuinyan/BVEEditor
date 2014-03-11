using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion.Events
{
    /// <summary>
    /// Event occurred when a TextPosition gets invalid.
    /// </summary>
    public class PositionInvalidatedEvent : IPopupEvent
    {
        #region IPopupEvent メンバー

        public EventType Type{
            get{return EventType.PositionInvalidated;}
        }

        public EventSource Source{
            get{return EventSource.Popup;}
        }

        public object EventArgs{
            get{throw new InvalidOperationException("No state associated with this event!");}
        }

        #endregion
    }
}
