using System;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Editor
{
    public class AvalonEditorAdaptor : EditorAdaptorBase
    {
        readonly TextArea area;

        public AvalonEditorAdaptor(TextArea textArea)
        {
            area = textArea;
            textArea.Caret.PositionChanged += OnCaretPositionChanged;
            textArea.PreviewKeyDown += OnTextAreaPreviewKeyDown;
            textArea.KeyDown += OnKeyDown;
            textArea.KeyUp += OnKeyUp;
            textArea.PreviewTextInput += OnPreviewTextInput;
        }

        void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(PreviewTextInput != null)
                PreviewTextInput(sender, e);
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(KeyDown != null)
                KeyDown(sender, e);
        }

        void OnKeyUp(object sender, KeyEventArgs e)
        {
            if(KeyUp != null)
                KeyUp(sender, e);
        }

        void OnTextAreaPreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnPreviewKeyDown(sender, e);
        }

        protected virtual void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(PreviewKeyDown != null)
                PreviewKeyDown(sender, e);
        }

        void OnCaretPositionChanged(object sender, EventArgs e)
        {
            OnSelectionChanged(sender, e);
        }

        protected virtual void OnSelectionChanged(object sender, EventArgs e)
        {
            if(SelectionChanged != null)
                SelectionChanged(sender, e);
        }

        public override IDocument Document{
            get{return area.Document;}
        }

        public override int CaretOffset{
            get{return area.Caret.Offset;}
        }

        public override string Text{
            get{
                return area.Document.Text;
            }
        }

        public override UIElement UIElement{
            get{return area;}
        }

        public override event EventHandler SelectionChanged;
        public override event KeyEventHandler PreviewKeyDown;
        public override event KeyEventHandler KeyDown;
        public override event KeyEventHandler KeyUp;
        public override event TextCompositionEventHandler PreviewTextInput;

        public override Rect GetVisualPosition()
        {
            return area.Caret.CalculateCaretRectangle();
        }

        public override bool IsSameLine(int charIndex1, int charIndex2)
        {
            int size = area.Document.TextLength;


            //Clamp to within text length to prevent out of bounds errors.
            charIndex1 = Math.Min(charIndex1, size);
            charIndex2 = Math.Min(charIndex2, size);

            return area.Document.GetLineByOffset(charIndex1) == area.Document.GetLineByOffset(charIndex2);
        }

        public override void Focus()
        {
            area.Focus();
        }
    }
}
