using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BVEEditor.Editor.CodeCompletion.Events
{
    /// <summary>
    /// Event occurred when an item in the code completion popup is clicked.
    /// </summary>
    public class ItemClickedEvent : ICancellablePopupEvent
    {
        readonly ItemClickedEventArgs args;

        public ItemClickedEvent(MouseEventArgs args, ICompletionItem item)
        {
            this.args = new ItemClickedEventArgs(args, item);
        }

        #region ICancellablePopupEvent メンバー

        public void Cancel()
        {
            args.Args.Handled = true;
        }

        public bool IsCancelled{
            get{return args.Args.Handled;}
        }

        public bool IsTransient{
            get{return false;}
        }

        #endregion

        #region IPopupEvent メンバー

        public EventType Type{
            get{return EventType.ItemClicked;}
        }

        public EventSource Source{
            get{return EventSource.Popup;}
        }

        public object EventArgs{
            get{return args;}
        }

        #endregion
    }
}
