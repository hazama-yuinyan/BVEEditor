using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace BVEEditor.Settings
{
    public class GlobalSettingDescriptor
    {
        AddIn addin;
        string name;

        public string ClassName{get; private set;}

        public GlobalSettingDescriptor(Codon codon)
        {
            ClassName = codon.Properties["className"];
            name = codon.Properties.Contains("name") ? codon.Properties["name"] : codon.Id;
            addin = codon.AddIn;
        }

        public ISettingSnippet CreateSnippet()
        {
            var snippet = (ISettingSnippet)addin.CreateObject(ClassName);
            snippet.SnippetName = name;
            return snippet;
        }
    }
}
