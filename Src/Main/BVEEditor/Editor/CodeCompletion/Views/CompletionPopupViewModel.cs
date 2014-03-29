using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BVEEditor.Editor.CodeCompletion;
using BVEEditor.Editor.CodeCompletion.Actions;
using BVEEditor.Messages;
using Caliburn.Micro;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// The view model for the code completion window.
    /// </summary>
    public class CompletionPopupViewModel : PropertyChangedBase
    {
        readonly IEventAggregator event_aggregator;
        readonly ILanguageService language_service;
        BindableCollection<ICompletionItem> completion_items;
        ICompletionItem cur_completion_item;
        bool is_open;
        ITextEditor target;

        #region Binding sources
        public BindableCollection<ICompletionItem> CompletionItems{
            get{
                return completion_items;
            }

            set{
                completion_items = value;
                NotifyOfPropertyChange(() => CompletionItems);
                SelectedCompletionItem = CompletionItems.FirstOrDefault();
            }
        }

        public ICompletionItem SelectedCompletionItem{
            get{
                return cur_completion_item;
            }

            set{
                cur_completion_item = value;
                NotifyOfPropertyChange(() => SelectedCompletionItem);
            }
        }

        public BindableCollection<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>> Observers{
            get; private set;
        }

        public bool IsOpen{
            get{
                return is_open;
            }

            set{
                if(is_open != value){
                    is_open = value;
                    NotifyOfPropertyChange(() => IsOpen);
                }
            }
        }

        public ITextEditor Target{
            get{
                return target;
            }

            set{
                if(target != value){
                    target = value;
                    NotifyOfPropertyChange(() => Target);
                }
            }
        }
        #endregion

        public CompletionPopupViewModel(IEventAggregator eventAggregator, ILanguageService languageService)
        {
            event_aggregator = eventAggregator;
            language_service = languageService;
            completion_items = new BindableCollection<ICompletionItem>();
            Observers = new BindableCollection<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>>();

            AddObservers(Observers);
        }

        public void SelectNextCompletionItem()
        {
            int index = CompletionItems.IndexOf(SelectedCompletionItem);

            if(index < CompletionItems.Count)
                SelectedCompletionItem = CompletionItems[index + 1];
        }

        public void SelectPreviousCompletionItem()
        {
            int index = CompletionItems.IndexOf(SelectedCompletionItem);

            if(index >= 0)
                SelectedCompletionItem = CompletionItems[index - 1];
        }

        public bool CanHandleEditorChanged(ITextEditor editor)
        {
            return true;
        }

        public void HandleEditorChanged(ITextEditor editor)
        {
            if(editor != null){
                Observers.Add(new OpenOnWriteAction(language_service.GetCodeCompletionBindingWithEditor(editor)));
                Observers.Add(new CloseOnSteppingIntoEndOfExpression(language_service.GetCodeCompletionBindingWithEditor(editor)));
                Observers.Add(new CloseOnWritingEndOfExpression(language_service.GetCodeCompletionBindingWithEditor(editor)));
            }
        }

        public void Show()
        {
            event_aggregator.Publish(new PopupShowMessage(this, false));
        }

        public void Hide()
        {
            event_aggregator.Publish(new PopupHideMessage(this, true));
        }

        public void Close()
        {
            event_aggregator.Publish(new PopupHideMessage(this, false));
        }

        public void ForceShow()
        {
            event_aggregator.Publish(new PopupShowMessage(this, true));
        }

        public void ScrollIntoSelectedItem()
        {
            event_aggregator.Publish(new ScrollIntoViewMessage(this, SelectedCompletionItem));
        }

        public void LocatePopup()
        {
            event_aggregator.Publish(new PopupLocateMessage(this, null));
        }

        public void InvalidatePosition()
        {
            event_aggregator.Publish(new InvalidatePositionMessage(this));
        }

        bool IsEditor(EventSource source)
        {
            return source == EventSource.Editor;
        }

        void AddObservers(BindableCollection<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>> observers)
        {
            observers.Add(new CustomKeyAction{
                Action = x => {
                    x.InvalidatePosition();
                    x.Show();
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
            observers.Add(new CustomKeyAction(x => {x.Hide();}, Enumerable.Empty<Key>(), Key.Escape));
            observers.Add(new ElementChangedKeyAction{Key = Key.Up, IsTargetSource = IsEditor});
            observers.Add(new ElementChangedKeyAction{Key = Key.Down, IsTargetSource = IsEditor});
        }
    }
}
