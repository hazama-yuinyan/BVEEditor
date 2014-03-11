using System;


namespace BVEEditor.Editor.CodeCompletion
{
    public enum EventType
    {
        KeyPress,
        KeyUp,
        SelectionChanged,
        ItemClicked,
        PopupStateChanged,
        PositionInvalidated,
        CancellableInput
    }

    public enum EventSource
    {
        Popup,
        Editor
    }
}