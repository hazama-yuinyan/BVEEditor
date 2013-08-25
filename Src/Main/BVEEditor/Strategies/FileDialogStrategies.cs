using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Events;
using BVEEditor.Result;
using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor.Strategies
{
    public class FileDialogStrategies : IFileDialogStrategies
    {
        readonly IResultFactory result_factory;
        readonly IEventAggregator event_aggregator;

        public FileDialogStrategies(IResultFactory resultFactory, IEventAggregator eventAggregator)
        {
            result_factory = resultFactory;
            event_aggregator = eventAggregator;
        }

        #region IFileDialogStrategies メンバー

        public IEnumerable<IResult> Save(ViewDocumentViewModel document, bool doQuickSave, System.Action<string> fileSelected)
        {
            if(doQuickSave && !string.IsNullOrEmpty(document.FileName)){
                fileSelected(document.FilePath);
                event_aggregator.Publish(new StatusBarMessageChangedEvent(StringParser.Parse("${res:BVEEditor:StringResources:StatusBar.Messages.SavedFile}",
                    new StringTagPair("fileName", document.FileName)))
                );
                yield return new DummyResult();
            }else{
                var result = result_factory.ShowFileDialog(StringParser.Parse("${res:BVEEditor:StringResources:Common.Dialogs.Captions.SaveAs}"),
                    "All files(*.*)|*.*", FileDialogMode.Save, document.FileName);
                yield return result;

                if(!string.IsNullOrEmpty(result.File)){
                    fileSelected(result.File);
                    event_aggregator.Publish(new StatusBarMessageChangedEvent(StringParser.Parse(
                        "${res:BVEEditor:StringResources:StatusBar.Messages.SavedFileAs}",
                        new StringTagPair("newFileName", result.File)))
                    );
                }
                yield return new DummyResult();
            }
        }

        public IEnumerable<IResult> Open(System.Action<string> fileSelected)
        {
            var result = result_factory.ShowFileDialog(StringParser.Parse("${res:BVEEditor:StringResources:Common.Dialogs.Captions.Open}"),
                "All files(*.*)|*.*", FileDialogMode.Open);
            yield return result;

            if(!string.IsNullOrEmpty(result.File))
                fileSelected(result.File);

            yield return new DummyResult();
        }

        #endregion
    }
}
