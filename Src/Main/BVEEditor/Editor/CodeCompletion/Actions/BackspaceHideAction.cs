using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BVEEditor.Views.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that hides the code completion popup when the backspace key is pressed.
    /// </summary>
    public class BackspaceHideAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>
    {
        static readonly string WhitespaceCharacters = " 　";

        #region IEventObserver<IPopupEvent,ICancellablePopupEvent,CompletionPopupView> メンバー

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupView view)
        {
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupView view)
        {
            var current = events.First();

            if(!IsTriggered(current.Type, current.EventArgs as Selection, view.Target))
                return;

            view.IsOpen = false;
        }

        #endregion

        bool IsTriggered(EventType type, Selection args, EditorAdaptorBase editor)
        {
            if(editor == null || args == null || type != EventType.SelectionChanged)
                return false;
            
            return args.GetText().Contains(WhitespaceCharacters);
        }
    }
}
