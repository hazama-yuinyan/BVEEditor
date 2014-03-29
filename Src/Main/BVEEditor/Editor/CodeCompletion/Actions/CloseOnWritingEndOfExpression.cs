using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BVEEditor.Editor.CodeCompletion;
using BVEEditor.Util;
using BVEEditor.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that hides the code completion popup when the user finishes entering an expression.
    /// </summary>
    public class CloseOnWritingEndOfExpression : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>
    {
        readonly ICodeCompletionBinding CodeCompletionBinding;

        public CloseOnWritingEndOfExpression(ICodeCompletionBinding completionBinding)
        {
            CodeCompletionBinding = completionBinding;
        }

        bool IsTriggered(TextCompositionEventArgs args, ITextEditor editor)
        {
            return args.Text.Length == 1 && CodeCompletionBinding.ShouldMarkEndOfExpression(editor, args.Text[0]);
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
            if(current.Type != EventType.CancellableInput || viewModel.Target == null)
                return;

            if(!IsTriggered(current.EventArgs as TextCompositionEventArgs, viewModel.Target))
                return;

            viewModel.Hide();
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
        }
    }
}
