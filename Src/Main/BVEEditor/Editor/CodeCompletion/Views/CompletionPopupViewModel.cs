using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Editor.CodeCompletion;
using Caliburn.Micro;

namespace BVEEditor.CodeCompletion
{
    /// <summary>
    /// The view model for the code completion window.
    /// </summary>
    public class CompletionPopupViewModel : PropertyChangedBase
    {
        BindableCollection<ICompletionItem> completion_items;
        ICompletionItem cur_completion_item;

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
        #endregion

        public IList<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>> Observers{
            get; private set;
        }

        public CompletionPopupViewModel()
        {
            completion_items = new BindableCollection<ICompletionItem>();
            Observers = new List<IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupView>>();
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
    }
}
