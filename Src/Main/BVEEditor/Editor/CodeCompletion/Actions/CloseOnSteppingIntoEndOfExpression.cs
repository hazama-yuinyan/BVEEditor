﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BVEEditor.Editor.CodeCompletion;
using BVEEditor.Util;
using BVEEditor.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that hides the code completion popup when the cursor gets out of the scope of an expression.
    /// </summary>
    public class CloseOnSteppingIntoEndOfExpression : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>
    {
        readonly ICodeCompletionBinding CodeCompletionBinding;

        public CloseOnSteppingIntoEndOfExpression(ICodeCompletionBinding completionBinding)
        {
            this.CodeCompletionBinding = completionBinding;
        }

        bool IsTriggered(KeyEventArgs key, ITextEditor editor)
        {
            return CodeCompletionBinding.ShouldMarkEndOfExpression(editor) && key.Key == Key.Left || key.Key == Key.Right;
        }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
            var current = events.First();

            if(current.Type != EventType.KeyUp || viewModel.Target == null)
                return;

            if(!IsTriggered(current.EventArgs as KeyEventArgs, viewModel.Target))
                return;

            viewModel.Hide();
        }
    }
}
