using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Views.CodeCompletion;

namespace BVEEditor.Util
{
    public static class CompletionPopupActions
    {
        public static void Hide(CompletionPopupView view)
        {
            view.IsOpen = false;
            view.Target.Focus();
        }

        public static void ForceShow(CompletionPopupView view)
        {
            view.IsOpen = true;
        }

        public static void InvalidatePosition(CompletionPopupView view)
        {
            view.InvalidatePosition();
        }

        public static void Show(CompletionPopupView view)
        {
            if(view.CompletionItems.HasItems)
                ForceShow(view);
        }
    }
}
