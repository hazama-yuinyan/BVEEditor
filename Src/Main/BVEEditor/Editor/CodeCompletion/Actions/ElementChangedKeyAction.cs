using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BVEEditor.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// A key action that performs item change.
    /// </summary>
    public class ElementChangedKeyAction : KeyAction
    {
        protected override void DoAct(CompletionPopupViewModel viewModel, KeyEventArgs args)
        {
            PerformElementChanged(viewModel, args);
        }

        void PerformElementChanged(CompletionPopupViewModel viewModel, KeyEventArgs args)
        {
            if(viewModel.CompletionItems.Count <= 0)
                return;

            if(args.Key == Key.Down)
                viewModel.SelectNextCompletionItem();
            else
                viewModel.SelectPreviousCompletionItem();

            viewModel.ScrollIntoSelectedItem();
        }

        protected override bool ShouldSwallow(CompletionPopupViewModel viewModel, KeyEventArgs args)
        {
            return viewModel.CompletionItems.Count > 0;
        }

        protected override bool IsTriggeredAddon(IPopupEvent @event, CompletionPopupViewModel viewModel)
        {
            return viewModel.IsOpen;
        }
    }
}
