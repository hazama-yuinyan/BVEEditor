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
        int last_index;

        bool IsTriggered(EventType type, object args, ITextEditor editor)
        {
            if(type != EventType.SelectionChanged || editor == null)
                return false;

            int caret_index = (int)args;
            var doc = editor.Document;

            return Math.Abs(caret_index - last_index) > 1 || !IsSameLine(doc, caret_index, last_index);
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
            var current = events.First();

            if(current.Type != EventType.SelectionChanged)
                return;

            if(IsTriggered(current.Type, current.EventArgs, viewModel.Target))
                viewModel.Hide();

            last_index = (int)current.EventArgs;
        }

        bool IsSameLine(IDocument doc, int lastIndex, int currentIndex)
        {
            int last_line = doc.GetLineByOffset(last_index).LineNumber;
            int current_line = doc.GetLineByOffset(currentIndex).LineNumber;
            return last_line == current_line;
        }
    }
}
