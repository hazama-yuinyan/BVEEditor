using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BVEEditor.Editor;
using BVEEditor.Editor.CodeCompletion;
using BVEEditor.Editor.CodeCompletion.Actions;
using BVEEditor.Editor.CodeCompletion.Events;
using BVEEditor.Messages;
using BVEEditor.Util;
using Caliburn.Micro;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.AvalonEdit.Document;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// CompletionPopupView.xaml の相互作用ロジック
    /// </summary>
    public partial class CompletionPopupView : Popup, IHandle<PopupShowMessage>, IHandle<PopupHideMessage>,
        IHandle<InvalidatePositionMessage>, IHandle<ScrollIntoViewMessage>, IHandle<PopupLocateMessage>
    {
        readonly FixedSizeStack<IPopupEvent> events;

        IEventAggregator event_aggregator;
        // This property should be explicit because property injection doesn't work for view elements
        // created by the xaml loader(it requires view elements created to have a default constructor)
        // FIXME: if it is possible
        public IEventAggregator EventAggregator{
            get{
                return event_aggregator;
            }
            
            set{
                if(event_aggregator != value){
                    if(event_aggregator != null)
                        event_aggregator.Unsubscribe(this);
                    
                    value.Subscribe(this);
                    event_aggregator = value;
                }
            }
        }

        public CompletionPopupView()
        {
            InitializeComponent();

            events = new FixedSizeStack<IPopupEvent>(15);

            completionItems.PreviewKeyDown += (sender, args) => Publish(new CancellableKeyEvent(args, EventSource.Popup));
            completionItems.ItemClicked += (sender, args) => Publish(new ItemClickedEvent(args.Arg2, (ICompletionItem)args.Arg1));

            Opened += (obj, args) => Publish(new PopupStateChangedEvent(PopupState.Open));
            Closed += (obj, args) => Publish(new PopupStateChangedEvent(PopupState.Closed));
            DataContextChanged += OnDataContextChanged;
        }

        void OnObserversChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action){
            case NotifyCollectionChangedAction.Add:
                break;

            default:
                throw new InvalidOperationException("Observers doesn't support operations other than Add.");
            }
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(!(e.OldValue is CompletionPopupViewModel) || !(e.NewValue is CompletionPopupViewModel))
                return;

            Publish(new PositionInvalidatedEvent());
            Observers.CollectionChanged += OnObserversChanged;
        }

        #region Dependency properties
        [TypeConverter(typeof(EditorAdaptorConverter))]
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(ITextEditor), typeof(CompletionPopupView), new PropertyMetadata(default(ITextEditor), OnTargetChanged));

        public static readonly DependencyProperty ObserversProperty =
            DependencyProperty.Register("Observers", typeof(BindableCollection<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>>),
                typeof(CompletionPopupView), new PropertyMetadata(default(BindableCollection<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>>)));

        public ITextEditor Target{
            get{return (ITextEditor)GetValue(TargetProperty);}
            set{SetValue(TargetProperty, value);}
        }

        public BindableCollection<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>> Observers{
            get{return (BindableCollection<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>>)GetValue(ObserversProperty);}
            set{SetValue(ObserversProperty, value);}
        }

        static void OnTargetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue == null || obj == null)
                return;

            var target = e.NewValue as ITextEditor;
            var old_target = e.OldValue as ITextEditor;
            var view = obj as CompletionPopupView;

            EventHandler selection_changed_handler = (sender, args) => view.Publish(new SelectionChangedEvent(target.Caret.Offset));
            KeyEventHandler preview_key_down_handler = (sender, args) => view.Publish(new CancellableKeyEvent(args, EventSource.Editor));
            KeyEventHandler key_up_handler = (sender, args) => view.Publish(new KeyUpEvent(args, EventSource.Editor));
            KeyEventHandler key_down_handler = (sender, args) => view.Publish(new KeyEvent(args, EventSource.Editor));
            TextCompositionEventHandler preview_text_input_handler = (sender, args) => view.Publish(new CancellableInputEvent(args));
            EventHandler<TextChangeEventArgs> text_changing_handler = (sender, args) => view.Publish(new TextChangingEvent(args));

            if(target != null){
                target.PreviewTextInput += preview_text_input_handler;
                target.SelectionChanged += selection_changed_handler;
                target.PreviewKeyDown += preview_key_down_handler;
                target.KeyDown += key_down_handler;
                target.KeyUp += key_up_handler;
                target.Document.TextChanging += text_changing_handler;
            }

            if(old_target != null){
                old_target.PreviewTextInput -= preview_text_input_handler;
                old_target.SelectionChanged -= selection_changed_handler;
                old_target.PreviewKeyDown -= preview_key_down_handler;
                old_target.KeyDown -= key_down_handler;
                old_target.KeyUp -= key_up_handler;
                old_target.Document.TextChanging -= text_changing_handler;
            }
        }
        #endregion

        void Publish(IPopupEvent @event)
        {
            if(DataContext == null)
                return;

            System.Diagnostics.Debug.WriteLine("publishing:" + @event.Type);

            events.Push(@event);

            foreach(var observer in Observers)
                observer.Handle(events, (CompletionPopupViewModel)DataContext);
        }

        void Publish(ICancellablePopupEvent @event)
        {
            foreach(var observer in Observers)
                observer.Preview(events, @event, (CompletionPopupViewModel)DataContext);

            if(@event.IsCancelled && !@event.IsTransient)
                return;

            events.Push(@event);

            foreach(var observer in Observers)
                observer.Handle(events, (CompletionPopupViewModel)DataContext);
        }

        internal void InvalidatePosition()
        {
            Publish(new PositionInvalidatedEvent());
        }

        void Show()
        {
            IsOpen = true;
        }

        #region IHandle<PopupShowMessage> メンバー

        public void Handle(PopupShowMessage message)
        {
            if(object.ReferenceEquals(message.Sender, DataContext)){
                if(completionItems.HasItems)
                    Show();
            }
        }

        #endregion

        #region IHandle<PopupHideMessage> メンバー

        public void Handle(PopupHideMessage message)
        {
            if(object.ReferenceEquals(message.Sender, DataContext)){
                IsOpen = false;
                event_aggregator.Publish(new FocusMessage(this));
            }
        }

        #endregion

        #region IHandle<InvalidatePositionMessage> メンバー

        public void Handle(InvalidatePositionMessage message)
        {
            if(object.ReferenceEquals(message.Sender, DataContext))
                InvalidatePosition();
        }

        #endregion

        #region IHandle<ScrollIntoViewMessage> メンバー

        public void Handle(ScrollIntoViewMessage message)
        {
            if(object.ReferenceEquals(message.Sender, DataContext)){
                System.Diagnostics.Debug.Assert(message.TargetItem != null);
                completionItems.ScrollIntoView(message.TargetItem);
            }
        }

        #endregion

        #region IHandle<PopupLocateMessage> メンバー

        public void Handle(PopupLocateMessage message)
        {
            if(object.ReferenceEquals(message.Sender, DataContext)){
                Rect rect = Target.GetCursorCoordinates();

                PlacementRectangle = rect.IsEmpty ? default(Rect) : new Rect(CalculatePoint(rect, Target.EditorElement), rect.Size);
            }
        }

        #endregion

        ScrollViewer FindScrollAncestor(UIElement element)
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
