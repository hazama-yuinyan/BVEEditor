using ICSharpCode.NRefactory.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion.Events
{
    public class TextChangingEvent : ICancellablePopupEvent
    {
        TextChangeEventArgs args;

        public TextChangingEvent(TextChangeEventArgs args)
        {
            this.args = args;
        }

        #region IPopupEvent メンバー

        public EventType Type{
            get{return EventType.TextChanging;}
        }

        public EventSource Source{
            get{return EventSource.Editor;}
        }

        public object EventArgs{
            get{return args;}
        }

        #endregion

        #region ICancellablePopupEvent メンバー

        public void Cancel()
        {
        }

        public bool IsCancelled{
            get{return false;}
        }

        public bool IsTransient{
            get{return false;}
        }

        #endregion
    }
}
