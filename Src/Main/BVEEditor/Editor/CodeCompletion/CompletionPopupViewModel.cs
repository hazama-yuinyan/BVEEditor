using BVEEditor.Editor.CodeCompletion;
using BVEEditor.Editor.CodeCompletion.Actions;
using BVEEditor.Messages;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// The view model for the code completion window.
    /// </summary>
    public class CompletionPopupViewModel : PropertyChangedBase, ICompletionHandler
    {
        readonly IEventAggregator event_aggregator;
        readonly ILanguageService language_service;
        BindableCollection<ICompletionItem> completion_items;
        ICompletionItem current_completion_item;
        bool is_open;
        int description_window_width, description_window_height;
        ITextEditor target;
        InsightWindowPopupViewModel insight_window;
        string current_text;
        DescriptionElementConverter parser = new DescriptionElementConverter();

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
                return current_completion_item;
            }

            set{
                current_completion_item = value;
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

        public int DescriptionWindowWidth{
            get{
                return description_window_width;
            }

            set{
                if(description_window_width != value){
                    description_window_width = value;
                    NotifyOfPropertyChange(() => DescriptionWindowWidth);
                }
            }
        }

        public int DescriptionWindowHeight{
            get{
                return description_window_height;
            }

            set{
                if(description_window_height != value){
                    description_window_height = value;
                    NotifyOfPropertyChange(() => DescriptionWindowHeight);
                }
            }
        }

        public ITextEditor Editor{
            get{
                return target;
            }

            set{
                if(target != value){
                    target = value;
                    NotifyOfPropertyChange(() => Editor);
                }
            }
        }

        public IInsightWindowHandler InsightWindowHandler{
            get{
                return insight_window;
            }
        }
        #endregion

        /// <summary>
        /// Holds all completion items.
        /// </summary>
        public IList<ICompletionItem> ItemsCache{
            get; set;
        }

        public bool IsFiltering{
            get; set;
        }

        public int StartOffset{
            get; set;
        }

        /// <summary>
		/// Gets/sets whether the completion window should expect text insertion at the start offset,
		/// which not go into the completion region, but before it.
		/// </summary>
		/// <remarks>
        /// This property allows only a single insertion, it is reset to false
		/// when that insertion has occurred.
        /// </remarks>
        internal bool ExpectInsertionBeforeStart{
            get; set;
        }

        internal string CurrentText{
            set{
                current_text = value;
            }
        }

        public CompletionPopupViewModel(IEventAggregator eventAggregator, ILanguageService languageService)
        {
            event_aggregator = eventAggregator;
            language_service = languageService;
            completion_items = new BindableCollection<ICompletionItem>();
            Observers = new BindableCollection<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>>();
            current_text = null;

            IsFiltering = true;
            StartOffset = -1;
            DescriptionWindowWidth = 300;

            AddObservers(Observers);
        }

        public bool CanHandleEditorChanged()
        {
            return true;
        }

        public void HandleEditorChanged(DependencyPropertyChangedEventArgs e)
        {
            var editor = e.NewValue as ITextEditor;
            if(editor != null){
                Observers.Add(new OpenOnWriteAction(language_service.GetCodeCompletionBindingWithEditor(editor)));
                Observers.Add(new CloseOnSteppingIntoEndOfExpression(language_service.GetCodeCompletionBindingWithEditor(editor)));
                Observers.Add(new CloseOnWritingEndOfExpression(language_service.GetCodeCompletionBindingWithEditor(editor)));
                Observers.Add(new SelectionChangedHideAction(language_service.GetCodeCompletionBindingWithEditor(editor)));
                Editor = editor;
                editor.Caret.LocationChanged += OnCaretPositionChanged;
            }
        }

        public void OnTextAreaKeyDown(KeyEventArgs e)
        {
            if(StartOffset != -1 && !e.Handled){
                if(e.Key == Key.Back){
                    if(current_text.Length > 0){
                        string new_current_text = current_text.Substring(0, current_text.Length - 1);
                        FilterItems(new_current_text);
                    }else{
                        FilterItems(string.Empty);
                    }
                }
            }
        }

        public void OnCaretPositionChanged(object sender, EventArgs e)
        {
            if(StartOffset != -1 && Editor.Caret.Offset > StartOffset)
                FilterItems(Editor.Document.GetText(StartOffset, Editor.Caret.Offset - StartOffset));
        }

        public void MarkStartOfExpression()
        {
            // We just set StartOffset because this causes OnTextAreaTextEntered to start working
            StartOffset = Editor.Caret.Offset;
        }

        internal void MarkEndOfExpression()
        {
            current_text = null;
            StartOffset = -1;
            CompletionItems.Clear();
        }

        public void SelectNextCompletionItem()
        {
            int index = CompletionItems.IndexOf(SelectedCompletionItem);

            if(index < CompletionItems.Count - 1)
                SelectedCompletionItem = CompletionItems[index + 1];
        }

        public void SelectPreviousCompletionItem()
        {
            int index = CompletionItems.IndexOf(SelectedCompletionItem);

            if(index > 0)
                SelectedCompletionItem = CompletionItems[index - 1];
        }

        public void Show()
        {
            event_aggregator.Publish(new PopupShowMessage(this, false));
        }

        public void Hide()
        {
            MarkEndOfExpression();
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
            if(SelectedCompletionItem != null){
                var description = SelectedCompletionItem.Description;
                var first_line = description.TakeStart(description.IndexOf('\n'));
                DescriptionWindowWidth = first_line.Length * 12;
                DescriptionWindowHeight = (description.Count(c => c == '\n') + 1) * 30;
            }
            event_aggregator.Publish(new PopupLocateMessage(this, null));
        }

        public void InvalidatePosition()
        {
            event_aggregator.Publish(new InvalidatePositionMessage(this));
        }

        void ReplaceItem()
        {
            ICompletionItem fancy_item;
        }

        bool IsEditor(EventSource source)
        {
            return source == EventSource.Editor;
        }

        void AddObservers(BindableCollection<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>> observers)
        {
            observers.Add(new CustomKeyAction{
                Action = x => {
                    var binding = language_service.GetCodeCompletionBindingWithEditor(x.Editor);
                    x.InvalidatePosition();
                    var result = binding.HandleCtrlSpace(x);
                    if(result == CodeCompletionKeyPressResult.Completed || result == CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion){
                        x.FilterItems(string.Empty);
                        x.Show();
                    }
                },
                Key = Key.Space,
                Modifiers = new[]{Key.LeftCtrl},
                ShouldSwallowKeyPress = true
            });

            observers.Add(new InsertionAction(Key.Enter){ShouldSwallow = true});
            observers.Add(new InsertionAction(Key.OemPeriod));
            observers.Add(new InsertOnItemClicked());
            observers.Add(new LocatePopupAction());
            observers.Add(new CustomKeyAction(x => {x.Hide();}, Enumerable.Empty<Key>(), Key.Escape));
            observers.Add(new ElementChangedKeyAction{Key = Key.Up, IsTargetSource = IsEditor});
            observers.Add(new ElementChangedKeyAction{Key = Key.Down, IsTargetSource = IsEditor});
            observers.Add(new StartOffsetAdjustAction());
        }

        internal void FilterItems(string text)
        {
            if(text == current_text)
                return;

            if(text == string.Empty){
                CompletionItems = new BindableCollection<ICompletionItem>(ItemsCache);
                return;
            }

            if(IsFiltering)
                FilterItemsWithQuery(text);
            else
                FilterItemsWithStartsWith(text);

            CurrentText = text;
        }

        void FilterItemsWithQuery(string query)
        {
            // if the user just typed one more character, don't filter all data but just filter what we are already displaying
			var list_to_filter = (CompletionItems.Count != 0 && (!string.IsNullOrEmpty(current_text)) && (!string.IsNullOrEmpty(query)) &&
			                    query.StartsWith(current_text, StringComparison.Ordinal)) ?
				CompletionItems : ItemsCache;

            var matching_items =
                from item in list_to_filter
                let quality = GetMatchQuality(item.Text, query)
                where quality > 0
                select new{Item = item, Quality = quality};

            var suggested_item = SelectedCompletionItem;

            var filtered_items = new BindableCollection<ICompletionItem>();
            int best_index = -1, best_quality = -1;
            double best_priority = 0;
            int i = 0;
            foreach(var matching_item in matching_items){
                double priority = (matching_item.Item == suggested_item) ? double.PositiveInfinity : matching_item.Item.Priority;
                int quality = matching_item.Quality;
                if(quality > best_quality || (quality == best_quality && priority > best_priority)){
                    best_index = i;
                    best_quality = quality;
                    best_priority = priority;
                }

                filtered_items.Add(matching_item.Item);
                ++i;
            }

            CompletionItems = filtered_items;
            SelectItemCentered(best_index);
        }

        void FilterItemsWithStartsWith(string query)
        {
            if(string.IsNullOrEmpty(query))
                return;

            int suggested_index = CompletionItems.IndexOf(SelectedCompletionItem);

            int best_index = -1, best_quality = -1;
            double best_priority = 0;
            for(int i = 0; i < ItemsCache.Count; ++i){
                int quality = GetMatchQuality(ItemsCache[i].Text, query);
                if(quality < 0)
                    continue;

                double priority = ItemsCache[i].Priority;
                bool use_this_item;
                if(best_quality < quality){
                    use_this_item = true;
                }else{
                    if(best_index == suggested_index){
                        use_this_item = false;
                    }else if(i == best_index){
                        // prefer recommended item, regardless of its priority
                        use_this_item = best_quality == quality;
                    }else{
                        use_this_item = best_quality == quality && best_priority < priority;
                    }
                }

                if(use_this_item){
                    best_index = i;
                    best_priority = priority;
                    best_quality = quality;
                }
            }

            SelectItemCentered(best_index);
        }

        void SelectItemCentered(int bestIndex)
        {
            if(bestIndex >= 0){
                SelectedCompletionItem = CompletionItems[bestIndex];
                ScrollIntoSelectedItem();
            }else{
                SelectedCompletionItem = null;
            }
        }

        int GetMatchQuality(string itemText, string query)
        {
            if(itemText == null)
                throw new ArgumentNullException("itemText", "ICompletionItem.Text returns null.");

            // Qualities:
			//  	8 = full match case sensitive
			// 		7 = full match
			// 		6 = match start case sensitive
			//		5 = match start
			//		4 = match CamelCase when length of query is 1 or 2 characters
			// 		3 = match substring case sensitive
			//		2 = match sustring
			//		1 = match CamelCase
			//	   -1 = no match
            if(query == itemText)
                return 8;
            if(string.Equals(itemText, query, StringComparison.InvariantCultureIgnoreCase))
                return 7;

            if(itemText.StartsWith(query, StringComparison.InvariantCultureIgnoreCase))
                return 6;
            if(itemText.StartsWith(query, StringComparison.InvariantCulture))
                return 5;

            bool? pascal_case_match = null;
            if(query.Length <= 2){
                pascal_case_match = PascalCaseMatch(itemText, query);
                if(pascal_case_match == true)
                    return 4;
            }

            // search by substring, if filtering (i.e. new behavior) turned on
            if(IsFiltering){
                if(itemText.IndexOf(query, StringComparison.InvariantCulture) > 0)
                    return 3;
                if(itemText.IndexOf(query, StringComparison.InvariantCultureIgnoreCase) > 0)
                    return 2;
            }

            if(!pascal_case_match.HasValue)
                pascal_case_match = PascalCaseMatch(itemText, query);

            return (pascal_case_match == true) ? 1 : -1;
        }

        static bool PascalCaseMatch(string text, string query)
        {
            int i = 0;
            foreach(char upper in text.Where(c => char.IsUpper(c))){
                if(i > query.Length - 1)
                    return true;            // return true here for PascalCase partial match("CQ" matches "CodeQualityAnalisys")

                if(char.ToUpper(query[i], CultureInfo.InvariantCulture) != upper)
                    return false;

                ++i;
            }
            return (i >= query.Length) ? true : false;
        }
    }
}
