using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Util;
using BVEEditor.CodeCompletion;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that hides the code completion popup in response to selection change.
    /// </summary>
    public class SelectionChangedHideAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>
    {
        readonly ICodeCompletionBinding CodeCompletionBinding;
        int last_index;

        public SelectionChangedHideAction(ICodeCompletionBinding completionBinding)
        {
            CodeCompletionBinding = completionBinding;
        }

        bool IsTriggered(EventType type, object args, CompletionPopupViewModel viewModel)
        {
            if(type != EventType.SelectionChanged || viewModel == null)
                return false;

            int caret_index = (int)args;

            return CodeCompletionBinding.ShouldMarkEndOfExpression(viewModel.Editor, viewModel.StartOffset);
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
            var current = events.First();

            if(current.Type != EventType.SelectionChanged || !viewModel.IsOpen)
                return;

            if(IsTriggered(current.Type, current.EventArgs, viewModel))
                viewModel.Hide();

            last_index = (int)current.EventArgs;
        }
    }
}
