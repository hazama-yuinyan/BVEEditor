using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BVEEditor.AvalonEdit.AddIn.Search;
using ICSharpCode.Core;
using BVEEditor.AvalonEdit.AddIn;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Document;

namespace BVEEditor.Tests
{
    [TestClass]
    public class FindReplace
    {
        static readonly string Text = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789
abc abc abcdefg abc abc abcdefg";
        static IDocument OriginalDocument;
        IPropertyService prop_service;

        [TestInitialize]
        public void Initialize()
        {
            OriginalDocument = new StringBuilderDocument(Text);
            prop_service = new PropertyServiceImpl();
        }

        void DoTest(TextSegmentCollection<SearchResult> segments, int[] expectedIndexes)
        {
            var result = segments.FirstSegment;
            const string message = "Expected {0} th match : {1}; actually {2}";
            for(int i = 0; i < segments.Count; ++i){
                Assert.AreEqual(expectedIndexes[i], result.StartOffset, message, i + 1, expectedIndexes[i], result.StartOffset);
                result = segments.GetNextSegment(result);
            }
        }

        [TestMethod]
        public void SimpleFind()
        {
            var worker = new FindReplaceWorker(prop_service);
            worker.Document = OriginalDocument;
            worker.SearchText = "abc";

            var expected_offsets = new int[]{0, 64, 68, 72, 80, 84, 88};

            worker.FindNext();
            DoTest(worker.SearchResults, expected_offsets);
        }

        [TestMethod]
        public void IgnoreCaseFind()
        {
            var worker = new FindReplaceWorker(prop_service);
            worker.Document = OriginalDocument;
            worker.IgnoreCase = true;
            worker.SearchText = "abc";

            var expected_offsets = new int[]{0, 26, 64, 68, 72, 80, 84, 88};

            worker.FindNext();
            DoTest(worker.SearchResults, expected_offsets);
        }

        [TestMethod]
        public void RegExFind()
        {
            var worker = new FindReplaceWorker(prop_service);
            worker.Document = OriginalDocument;
            worker.UseRegex = true;
            worker.SearchText = @"abc\s+";

            var expected_offsets = new int[]{64, 68, 80, 84};

            worker.FindNext();
            DoTest(worker.SearchResults, expected_offsets);
        }

        public void Replace()
        {
            var worker = new FindReplaceWorker(prop_service);
            worker.Document = new StringBuilderDocument(OriginalDocument);
            worker.SearchText = "abc";

            
        }
    }
}
