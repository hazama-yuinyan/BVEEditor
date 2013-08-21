using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Result;
using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor.Strategies
{
    public class FileDialogStrategies : IFileDialogStrategies
    {
        IResultFactory result_factory;

        public FileDialogStrategies(IResultFactory resultFactory)
        {
            result_factory = resultFactory;
        }

        #region IFileDialogStrategies メンバー

        public IEnumerable<IResult> SaveAs(ViewDocumentViewModel document, bool quickSave, System.Action<string> fileSelected)
        {
            if(quickSave && !string.IsNullOrEmpty(document.FileName)){
                fileSelected(document.FileName);
            }else{
                var result = result_factory.ShowFileDialog(StringParser.Parse("${res:BVEEditor:StringResources:Common.Dialogs.Captions.SaveAs}"),
                    "All files(*.*)|*.*", FileDialogMode.Save, document.FileName);
                yield return result;

                if(!string.IsNullOrEmpty(result.File))
                    fileSelected(result.File);
            }
        }

        public IEnumerable<IResult> Open(System.Action<string> fileSelected)
        {
            var result = result_factory.ShowFileDialog(StringParser.Parse("${res:BVEEditor:StringResources:Common.Dialogs.Captions.Open}"),
                "All files(*.*)|*.*", FileDialogMode.Open);
            yield return result;

            if(!string.IsNullOrEmpty(result.File))
                fileSelected(result.File);
        }

        #endregion
    }
}
