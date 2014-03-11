using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BVEEditor.Util;
using BVEEditor.Views.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that opens the code completion popup when a new character is entered.
    /// </summary>
    public class OpenOnWriteAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>
    {
        Func<char, bool> BeginningOfExpressionPredicate;

        HashSet<char> triggers;

        public OpenOnWriteAction(Func<char, bool> isBeginningOfExpression)
        {
            triggers = new HashSet<char>(Enumerable.Range(65, 26).Union(Enumerable.Range(97, 26)).Select(x => (char)x))
            {
                '(',
                '.'
            };
            this.BeginningOfExpressionPredicate = isBeginningOfExpression;
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupView view)
        {
            if(current.Type != EventType.CancellableInput || view.IsOpen)
                return;

            var args = current.EventArgs as TextCompositionEventArgs;

            if(args.Text.Length == 1 && triggers.Contains(args.Text[0]) && BeginningOfExpressionPredicate(args.Text[0]))
                CompletionPopupActions.Show(view);
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupView view)
        {
        }
    }
}
