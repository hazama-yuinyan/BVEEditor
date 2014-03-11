using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BVEEditor.Editor.CodeCompletion;
using BVEEditor.Util;
using BVEEditor.Views.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that hides the code completion popup when the user finishes entering an expression.
    /// </summary>
    public class CloseOnWritingEndOfExpression : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>
    {
        readonly Func<char, bool> EndOfExpressionPredicate;

        public CloseOnWritingEndOfExpression(Func<char, bool> isEndOfExpression)
        {
            EndOfExpressionPredicate = isEndOfExpression;
        }

        private bool IsTriggered(TextCompositionEventArgs args)
        {
            return args.Text.Length == 1 && EndOfExpressionPredicate(args.Text[0]);
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupView view)
        {
            if(current.Type != EventType.CancellableInput || view.Target == null)
                return;

            if(!IsTriggered(current.EventArgs as TextCompositionEventArgs))
                return;

            CompletionPopupActions.Hide(view);
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupView view)
        {

        }
    }
}
