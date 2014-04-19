using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BVEEditor.Util;
using BVEEditor.CodeCompletion;
using ICSharpCode.NRefactory.Editor;

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

            var args = (TextCompositionEventArgs)current.EventArgs;

            if(args.Text.Length == 1){
                var result = binding.HandleKeyPress(viewModel, args.Text[0]);
                if(result == CodeCompletionKeyPressResult.None || result == CodeCompletionKeyPressResult.EatKey)
                    return;

                viewModel.LocatePopup();
                viewModel.MarkStartOfExpression();
                if(result == CodeCompletionKeyPressResult.Completed){
                    viewModel.ExpectInsertionBeforeStart = true;
                    viewModel.FilterItems(string.Empty);
                    viewModel.CurrentText = args.Text;
                }else if(result == CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion){
                    viewModel.FilterItems(args.Text);
                }

                viewModel.Show();
            }
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
        }
    }
}
