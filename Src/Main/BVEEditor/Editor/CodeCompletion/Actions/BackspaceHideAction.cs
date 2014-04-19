using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BVEEditor.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that hides the code completion popup when the backspace key is pressed.
    /// </summary>
    public class BackspaceHideAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>
    {
        static readonly string WhitespaceCharacters = " 　";

        #region IEventObserver<IPopupEvent,ICancellablePopupEvent,CompletionPopupViewModel> メンバー

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
            var current = events.First();

            if(!IsTriggered(current.Type, current.EventArgs as Selection, viewModel.Editor))
                return;

            viewModel.Hide();
        }

        #endregion

        bool IsTriggered(EventType type, Selection args, ITextEditor editor)
        {
            if(editor == null || args == null || type != EventType.SelectionChanged)
                return false;
            
            return args.GetText().Contains(WhitespaceCharacters);
        }
    }
}
