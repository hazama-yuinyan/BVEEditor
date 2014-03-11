using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Editor.LanguageBinding;
using ICSharpCode.Core;

namespace BVEEditor
{
    public interface ILanguageService
    {
        ILanguageBinding GetLanguageByFileName(FileName name);
        ILanguageBinding GetLanguageByExtension(string extension);
        ILanguageBinding GetLanguageByName(string name);
    }

    public class LanguageBindingService : ILanguageService
    {
        const string LanguageBindingPath = "/BVEEditor/Workbench/LanguageBindings";
        readonly List<LanguageBindingDescriptor> bindings;

        public LanguageBindingService()
		{
			bindings = AddInTree.BuildItems<LanguageBindingDescriptor>(LanguageBindingPath, null, false);
		}
		
		public ILanguageBinding GetLanguageByFileName(FileName fileName)
		{
			return GetLanguageByExtension(Path.GetExtension(fileName));
		}
		
		public ILanguageBinding GetLanguageByExtension(string extension)
		{
			foreach(var language in bindings){
				if(language.CanAttach(extension))
					return language.Binding;
			}
			return DefaultLanguageBinding.DefaultInstance;
		}
		
		public ILanguageBinding GetLanguageByName(string name)
		{
			foreach(var language in bindings){
				if(language.Name == name)
					return language.Binding;
			}
			return DefaultLanguageBinding.DefaultInstance;
		}
    }
}
