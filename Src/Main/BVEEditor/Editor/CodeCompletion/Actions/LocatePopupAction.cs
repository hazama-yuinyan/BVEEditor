using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BVEEditor.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that locates the code completion popup according to the cursor position.
    /// </summary>
    public class LocatePopupAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>
    {
        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
            if(!events.Any())
                return;

            var last = events.First();

            if(last.Type != EventType.PositionInvalidated && last.Type != EventType.SelectionChanged && last.Type != EventType.PopupStateChanged)
                return;

            if(last.Type == EventType.SelectionChanged && viewModel.IsOpen)
                return;

            viewModel.LocatePopup();
        }
    }
}
