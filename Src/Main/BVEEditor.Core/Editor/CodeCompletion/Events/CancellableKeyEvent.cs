using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BVEEditor.Editor.CodeCompletion.Events
{
    /// <summary>
    /// An cancellable event that is occurred when a key is pressed.
    /// </summary>
    public class CancellableKeyEvent : KeyEvent, ICancellablePopupEvent
    {
        public CancellableKeyEvent(KeyEventArgs args, EventSource source) : base(args, source)
        {}

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
    }

    public class KeyUpEvent : KeyEvent
    {
        public override EventType Type{
            get{return EventType.KeyUp;}
        }

        public KeyUpEvent(KeyEventArgs args, EventSource source) : base(args, source)
        {}
    }

    /// <summary>
    /// Event occurred when a key is pressed.
    /// </summary>
    public class KeyEvent : IPopupEvent
    {
        protected readonly KeyEventArgs args;
        protected readonly EventSource source;

        #region IPopupEvent メンバー

        public virtual EventType Type{
            get{return EventType.KeyPress;}
        }

        public EventSource Source{
            get{return source;}
        }

        public object EventArgs{
            get{return args;}
        }

        #endregion

        public KeyEvent(KeyEventArgs args, EventSource source)
        {
            this.args = args;
            this.source = source;
        }
    }
}
