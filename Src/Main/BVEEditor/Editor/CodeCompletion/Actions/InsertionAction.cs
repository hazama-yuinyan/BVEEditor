using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BVEEditor.Util;
using BVEEditor.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that performs completion item insertion.
    /// </summary>
    public class InsertionAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>
    {
        public InsertionAction(Key key)
        {
            this.Key = key;
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupView view)
        {
            if(IsTriggered(current, view)){
                InsertElement(view);
                if(ShouldSwallow)
                    current.Cancel();
            }
        }

        public Key Key{get; set;}

        public bool ShouldSwallow{get; set;}
        static IEnumerable<Key> modifiers = new[] {Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt};

        bool IsTriggered(ICancellablePopupEvent current, CompletionPopupView view)
        {
            if(current.Type != EventType.KeyPress || !view.IsOpen || view.CompletionItems.SelectedItem == null)
                return false;

            var args = current.EventArgs as KeyEventArgs;

            return args.Key == Key && modifiers.All(args.KeyboardDevice.IsKeyUp);
        }

        void InsertElement(CompletionPopupView view)
        {
            InsertItem(view.Model.SelectedCompletionItem, view);
        }

        void InsertItem(ICompletionItem item, CompletionPopupView view)
        {
            item.Insert(view.Target);
            CompletionPopupActions.Hide(view);
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupView view)
        {}
    }
}
