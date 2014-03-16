using System;
using System.Collections.Generic;
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
using BVEEditor.Util;

namespace BVEEditor.Views.CodeCompletion
{
    /// <summary>
    /// CompletionPopupView.xaml の相互作用ロジック
    /// </summary>
    public partial class CompletionPopupView : Popup
    {
        readonly FixedSizeStack<IPopupEvent> events;

        public CompletionPopupView()
        {
            InitializeComponent();
            events = new FixedSizeStack<IPopupEvent>(15);

            CompletionItems.PreviewKeyDown += (sender, args) => Publish(new CancellableKeyEvent(args, EventSource.Popup));
            CompletionItems.ItemClicked += (sender, args) => Publish(new ItemClickedEvent(args.Arg2, (ICompletionItem)args.Arg1));

            Opened += (obj, args) => Publish(new PopupStateChangedEvent(PopupState.Open));
            Closed += (obj, args) => Publish(new PopupStateChangedEvent(PopupState.Closed));
            DataContextChanged += OnDataContextChanged;
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue as CompletionPopupViewModel == null)
                return;

            AddObservers(Model.Observers);
            Publish(new PositionInvalidatedEvent());
        }

        void AddObservers(IList<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>> observers)
        {
            observers.Add(new CustomKeyAction{
                Action = x => {
                    x.InvalidatePosition();
                    x.IsOpen = true;
                },
                Key = Key.Space,
                Modifiers = new[]{Key.LeftCtrl},
                ShouldSwallowKeyPress = true
            });

            observers.Add(new SelectionChangedHideAction());
            observers.Add(new InsertionAction(Key.Enter){ShouldSwallow = true});
            observers.Add(new InsertionAction(Key.OemPeriod));
            observers.Add(new InsertOnItemClicked());
            observers.Add(new LocatePopupAction());
            observers.Add(new CustomKeyAction(x => CompletionPopupActions.Hide(this), Enumerable.Empty<Key>(), Key.Escape));
            observers.Add(new ElementChangedKeyAction{Key = Key.Up, IsTargetSource = IsEditor});
            observers.Add(new ElementChangedKeyAction{Key = Key.Down, IsTargetSource = IsEditor});
        }

        bool IsEditor(EventSource source)
        {
            return source == EventSource.Editor;
        }

        [TypeConverter(typeof(EditorAdaptorConverter))]
        public EditorAdaptorBase Target{
            get{return (EditorAdaptorBase)GetValue(TargetProperty);}
            set{SetValue(TargetProperty, value);}
        }

        public CompletionPopupViewModel Model{
            get{return this.DataContext as CompletionPopupViewModel;}
        }

        [TypeConverter(typeof(EditorAdaptorConverter))]
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(EditorAdaptorBase), typeof(CompletionPopupView), new PropertyMetadata(default(EditorAdaptorBase), OnTargetChanged));

        static void OnTargetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue == null || obj == null)
                return;

            var target = e.NewValue as EditorAdaptorBase;
            var old_target = e.OldValue as EditorAdaptorBase;
            var view = obj as CompletionPopupView;

            EventHandler selection_changed_handler = (sender, args) => view.Publish(new SelectionChangedEvent(target.CaretOffset));
            KeyEventHandler preview_key_down_handler = (sender, args) => view.Publish(new CancellableKeyEvent(args, EventSource.Editor));
            KeyEventHandler key_up_handler = (sender, args) => view.Publish(new KeyUpEvent(args, EventSource.Editor));
            KeyEventHandler key_down_handler = (sender, args) => view.Publish(new KeyEvent(args, EventSource.Editor));
            TextCompositionEventHandler preview_text_input_handler = (sender, args) => view.Publish(new CancellableInputEvent(args));

            if(target != null){
                target.PreviewTextInput += preview_text_input_handler;
                target.SelectionChanged += selection_changed_handler;
                target.PreviewKeyDown += preview_key_down_handler;
                target.KeyDown += key_down_handler;
                target.KeyUp += key_up_handler;
            }

            if(old_target != null){
                old_target.SelectionChanged -= selection_changed_handler;
                old_target.PreviewKeyDown -= preview_key_down_handler;
                old_target.KeyDown -= key_down_handler;
                old_target.KeyUp -= key_up_handler;
            }
        }

        void Publish(IPopupEvent @event)
        {
            if(Model == null)
                return;

            System.Diagnostics.Debug.WriteLine("publishing:" + @event.Type);

            events.Push(@event);

            foreach(var observer in Model.Observers)
                observer.Handle(events, this);
        }

        void Publish(ICancellablePopupEvent @event)
        {
            foreach(var observer in Model.Observers)
                observer.Preview(events, @event, this);

            if(@event.IsCancelled && !@event.IsTransient)
                return;

            events.Push(@event);

            foreach(var observer in Model.Observers)
                observer.Handle(events, this);
        }

        internal void InvalidatePosition()
        {
            Publish(new PositionInvalidatedEvent());
        }
    }
}
