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
    /// An action that hides the code completion popup when the cursor gets in the end of an expression.
    /// </summary>
    public class CloseOnSteppingIntoEndOfExpression : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>
    {
        readonly Func<bool> EndOfExpressionPredicate;

        public CloseOnSteppingIntoEndOfExpression(Func<bool> isEndOfExpression)
        {
            this.EndOfExpressionPredicate = isEndOfExpression;
        }

        bool IsTriggered(KeyEventArgs key)
        {
            return EndOfExpressionPredicate() && key.Key == Key.Left || key.Key == Key.Right;
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupView view)
        {}

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupView view)
        {
            var current = events.First();

            if(current.Type != EventType.KeyUp || view.Target == null)
                return;

            if(!IsTriggered(current.EventArgs as KeyEventArgs))
                return;

            CompletionPopupActions.Hide(view);
        }
    }
}
