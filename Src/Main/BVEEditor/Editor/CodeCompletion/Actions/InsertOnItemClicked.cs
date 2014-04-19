using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Editor.CodeCompletion.Events;
using BVEEditor.Util;
using BVEEditor.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that inserts the content of the selected completion item when an item is clicked.
    /// </summary>
    class InsertOnItemClicked : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>
    {
        void InsertItem(ICompletionItem item, CompletionPopupViewModel viewModel)
        {
            if(item == null)
                throw new InvalidOperationException("ICompletionItem is null. Something is wrong with the hackish ItemClicked event.");

            item.Insert(viewModel.Editor);
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
            if(current.Source != EventSource.Popup || current.Type != EventType.ItemClicked)
                return;

            var event_args = current.EventArgs as ItemClickedEventArgs;

            InsertItem(event_args.Item, viewModel);

            viewModel.Hide();

            current.Cancel();
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
        }
    }
}
