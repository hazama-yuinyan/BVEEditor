using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace BVEEditor.Editor.LanguageBinding
{
    /// <summary>
    /// Creates LanguageBindingDescriptor objects for the language binding service.
    /// </summary>
    /// <attribute name="extensions" use="required">
    /// Semicolon-separated list of file extensions that are handled by the language binding (e.g. .xaml)
    /// </attribute>
    /// <attribute name="class" use="required">
    /// Name of the ILanguageBinding class.
    /// </attribute>
    /// <usage>Only in /SharpDevelop/Workbench/LanguageBindings</usage>
    /// <returns>
    /// The ILanguageBinding object.
    /// </returns>
    public class LanguageBindingDoozer : IDoozer
    {
        #region IDoozer メンバー

        public bool HandleConditions{
            get{return false;}
        }

        public object BuildItem(BuildItemArgs args)
        {
            return new LanguageBindingDescriptor(args.Codon);
        }

        #endregion
    }

    class LanguageBindingDescriptor
    {
        ILanguageBinding binding;
        Codon codon;
        string[] extensions;

        public ILanguageBinding Binding{
            get{
                if(binding == null)
                    binding = (ILanguageBinding)codon.AddIn.CreateObject(codon.Properties["class"]);

                return binding;
            }
        }

        public Codon Codon{
            get{return codon;}
        }

        public LanguageBindingDescriptor(Codon codon)
        {
            this.codon = codon;
        }

        public string[] Extensions{
            get{
                if(extensions == null){
                    if(codon.Properties["extensions"].Length == 0)
                        extensions = new string[0];
                    else
                        extensions = codon.Properties["extensions"].ToLowerInvariant().Split(';');
                }

                return extensions;
            }
        }

        public string Name{
            get{return codon.Properties["id"];}
        }

        public bool CanAttach(string extension)
        {
            if(Extensions.Length == 0)
                return true;

            if(string.IsNullOrEmpty(extension))
                return false;

            foreach(var ext in Extensions){
                if(string.Equals(extension, ext, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
