using ICSharpCode.NRefactory.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    public class StartOffsetAdjustAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>
    {
        #region IEventObserver<IPopupEvent,ICancellablePopupEvent,CompletionPopupViewModel> メンバー

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
            if(!events.Any())
                return;

            var current = events.First();
            if(current.Type != EventType.TextChanging || !viewModel.IsOpen)
                return;

            var args = (TextChangeEventArgs)current.EventArgs;
            if(args.Offset + args.RemovalLength == viewModel.StartOffset && args.RemovalLength > 0){
                // If we remove the segment just before completion segment, then close the popup.
                // This is neccesary when pressing backspace after dot-completion.
                viewModel.Hide();
            }

            if(args.Offset == viewModel.StartOffset && args.RemovalLength == 0 && viewModel.ExpectInsertionBeforeStart){
                viewModel.StartOffset = args.GetNewOffset(viewModel.StartOffset, AnchorMovementType.AfterInsertion);
                viewModel.ExpectInsertionBeforeStart = false;
            }else{
                viewModel.StartOffset = args.GetNewOffset(viewModel.StartOffset, AnchorMovementType.BeforeInsertion);
            }
        }

        #endregion
    }
}
