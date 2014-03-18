using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Events;
using BVEEditor.Result;
using BVEEditor.Workbench;
using BVEEditor.AvalonDock;
using Caliburn.Micro;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.Core;

namespace BVEEditor.Search
{
    public class FindReplaceDialogViewModel : ShellPresentationViewModel, IHandle<ActiveViewDocumentChangedEvent>
    {
        ISearchStrategy strategy;
        AvalonEditEditorViewModel active_doc;

        #region Binding sources
        public ICollection<string> RecentSearchTexts{
            get; private set;
        }

        public ICollection<string> RecentReplaceTexts{
            get; private set;
        }

        public TextSegmentCollection<SearchResult> SearchResults{
            get; set;
        }

        bool includes_subfolders;
        public bool IncludesSubfolders{
            get{return includes_subfolders;}
            set{
                if(includes_subfolders != value){
                    includes_subfolders = value;
                    NotifyOfPropertyChange(() => IncludesSubfolders);
                }
            }
        }

        bool ignore_case;
        public bool IgnoreCase{
            get{return ignore_case;}
            set{
                if(ignore_case != value){
                    ignore_case = value;
                    NotifyOfPropertyChange(() => IgnoreCase);
                }
            }
        }

        bool whole_words;
        public bool MatchWholeWords{
            get{return whole_words;}
            set{
                if(whole_words != value){
                    whole_words = value;
                    NotifyOfPropertyChange(() => MatchWholeWords);
                }
            }
        }

        bool use_regex;
        public bool UseRegex{
            get{return use_regex;}
            set{
                if(use_regex != value){
                    use_regex = value;
                    NotifyOfPropertyChange(() => UseRegex);
                }
            }
        }
        #endregion

        public FindReplaceDialogViewModel(IResultFactory resultFactory) : base(resultFactory)
        {
            RecentSearchTexts = new List<string>();
            RecentReplaceTexts = new List<string>();
            DisplayName = StringParser.Parse("${res:BVEEditor:StringResources:FindReplaceDialog.Caption.FindReplace}");
        }

        void UpdateResults()
        {
            if(SearchResults == null){
                SearchResults = new TextSegmentCollection<SearchResult>();
                foreach(var result in strategy.FindAll(active_doc, 0, active_doc.TextLength))
                    SearchResults.Add((SearchResult)result);
            }
        }

        public IEnumerable<IResult> FindNext()
        {
        }

        public IEnumerable<IResult> FindPrevious()
        {
        }

        public IEnumerable<IResult> FindAll()
        {
        }

        public IEnumerable<IResult> Replace()
        {
        }

        public IEnumerable<IResult> ReplaceAll()
        {
        }

        public IEnumerable<IResult> SkipFile()
        {
        }

        #region IHandle<ActiveViewDocumentChangedEvent> メンバー

        public void Handle(ActiveViewDocumentChangedEvent message)
        {
            active_doc = message.ViewDocument;
        }

        #endregion
    }
}
