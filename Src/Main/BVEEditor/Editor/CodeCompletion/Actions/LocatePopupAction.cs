using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BVEEditor.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    /// <summary>
    /// An action that locates the code completion popup according to the cursor position.
    /// </summary>
    public class LocatePopupAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>
    {
        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupView view)
        { }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupView view)
        {
            if(!events.Any())
                return;

            var last = events.First();

            if(last.Type != EventType.PositionInvalidated && last.Type != EventType.SelectionChanged && last.Type != EventType.PopupStateChanged)
                return;

            if(last.Type == EventType.SelectionChanged && view.IsOpen)
                return;

            Rect rect = view.Target.GetVisualPosition();

            view.PlacementRectangle = rect.IsEmpty ? default(Rect) : new Rect(CalculatePoint(rect, view.Target.UIElement), new Size(rect.Width, rect.Height));
        }

        private ScrollViewer FindScrollAncestor(UIElement element)
        {
            DependencyObject obj = element;
            while((obj = LogicalTreeHelper.GetParent(obj)) != null){
                var scroll_viewer = obj as ScrollViewer;
                if(scroll_viewer != null)
                    return scroll_viewer;
            }

            return null;
        }

        Point CalculatePoint(Rect rect, UIElement textArea)
        {
            var scroll = FindScrollAncestor(textArea);

            if(scroll == null)
                return new Point(rect.X, rect.Y + 1);

            return new Point(rect.X - scroll.HorizontalOffset, rect.Y - scroll.VerticalOffset + 1);
        }
    }
}
