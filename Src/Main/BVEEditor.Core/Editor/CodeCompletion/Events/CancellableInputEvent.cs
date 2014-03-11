using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BVEEditor.Editor.CodeCompletion.Events
{
    public class CancellableInputEvent : ICancellablePopupEvent
    {
        TextCompositionEventArgs args;

        public CancellableInputEvent(TextCompositionEventArgs args)
        {
            this.args = args;
        }

        #region ICancellablePopupEvent メンバー

        public void Cancel()
        {
            args.Handled = true;
        }

        public bool IsCancelled{
            get{return args.Handled;}
        }

        public bool IsTransient{
            get{return true;}
        }

        #endregion

        #region IPopupEvent メンバー

        public EventType Type{
            get{return EventType.CancellableInput;}
        }

        public EventSource Source{
            get{return EventSource.Editor;}
        }

        public object EventArgs{
            get{return args;}
        }

        #endregion
    }
}
