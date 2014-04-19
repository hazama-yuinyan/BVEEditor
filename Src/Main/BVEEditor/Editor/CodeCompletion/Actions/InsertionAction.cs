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
    public class InsertionAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>
    {
        public InsertionAction(Key key)
        {
            this.Key = key;
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
            if(IsTriggered(current, viewModel)){
                InsertElement(viewModel);
                if(ShouldSwallow)
                    current.Cancel();
            }
        }

        public Key Key{get; set;}

        public bool ShouldSwallow{get; set;}
        static IEnumerable<Key> modifiers = new[]{Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt};

        bool IsTriggered(ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
            if(current.Type != EventType.KeyPress || !viewModel.IsOpen || viewModel.SelectedCompletionItem == null)
                return false;

            var args = current.EventArgs as KeyEventArgs;

            return args.Key == Key && modifiers.All(args.KeyboardDevice.IsKeyUp);
        }

        void InsertElement(CompletionPopupViewModel viewModel)
        {
            InsertItem(viewModel.SelectedCompletionItem, viewModel);
        }

        void InsertItem(ICompletionItem item, CompletionPopupViewModel viewModel)
        {
            item.Insert(viewModel.Editor);
            viewModel.Hide();
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
        }
    }
}
