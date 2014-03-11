using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Editor.CodeCompletion.Events;
using BVEEditor.Util;
using BVEEditor.Views.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that inserts the content of the selected completion item when an item is clicked.
    /// </summary>
    class InsertOnItemClicked : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>
    {
        void InsertItem(ICompletionItem item, CompletionContext context)
        {
            if(item == null)
                throw new InvalidOperationException("ICompletionItem is null. Something is wrong with the hackish ItemClicked event.");

            item.Complete(context);
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupView view)
        {
            if(current.Source != EventSource.Popup || current.Type != EventType.ItemClicked)
                return;

            var event_args = current.EventArgs as ItemClickedEventArgs;

            var context = new CompletionContext();
            
            InsertItem(event_args.Item, new CompletionContext());

            CompletionPopupActions.Hide(view);

            current.Cancel();
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupView view)
        {}

        #region IEventObserver<IPopupEvent,ICancellablePopupEvent,CompletionPopupView> メンバー

        void IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>.Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupView view)
        {
            throw new NotImplementedException();
        }

        void IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>.Handle(IEnumerable<IPopupEvent> events, CompletionPopupView view)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
