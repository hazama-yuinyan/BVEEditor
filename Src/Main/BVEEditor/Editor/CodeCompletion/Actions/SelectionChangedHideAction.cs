using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Util;
using BVEEditor.Views.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that hides the code completion popup when the selection is changed.
    /// </summary>
    public class SelectionChangedHideAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>
    {
        int last_index;

        bool IsTriggered(EventType type, object args, EditorAdaptorBase editor)
        {
            if(type != EventType.SelectionChanged || editor == null)
                return false;

            int caretIndex = (int)args;

            return Math.Abs(caretIndex - last_index) > 1 || !editor.IsSameLine(caretIndex, last_index);
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupView view)
        {}

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupView view)
        {
            var current = events.First();

            if(current.Type != EventType.SelectionChanged)
                return;

            if(IsTriggered(current.Type, current.EventArgs, view.Target))
                CompletionPopupActions.Hide(view);

            last_index = (int)current.EventArgs;
        }
    }
}
