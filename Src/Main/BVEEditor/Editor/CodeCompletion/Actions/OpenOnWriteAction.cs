using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BVEEditor.Util;
using BVEEditor.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that opens the code completion popup when a new character is entered.
    /// </summary>
    public class OpenOnWriteAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>
    {
        readonly ICodeCompletionBinding binding;

        public OpenOnWriteAction(ICodeCompletionBinding binding)
        {
            this.binding = binding;
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
            if(current.Type != EventType.CancellableInput || viewModel.IsOpen)
                return;

            var args = current.EventArgs as TextCompositionEventArgs;

            if(args.Text.Length == 1 && binding.ShouldOpenPopup(viewModel.Target, args.Text[0]))
                viewModel.Show();
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
        }
    }
}
