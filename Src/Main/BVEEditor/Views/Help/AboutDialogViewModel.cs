using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Result;
using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor.Views.Help
{
    public class AboutDialogViewModel : ShellPresentationViewModel
    {
        #region Binding sources
        /// <summary>
        /// Gets the application version as a string.
        /// </summary>
        public string AppVersion{
            get{
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        /// <summary>
        /// Gets the assembly copyright.
        /// </summary>
        public string AssemblyCopyright{
            get{
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

                //If there aren't any copyright attributes, then return an empty string
                if(!attributes.Any())
                    return string.Empty;

                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public SortedList<string, string> Modules{
            get{
                var list = new SortedList<string, string>();
                foreach(var assembly in Assembly.GetEntryAssembly().GetReferencedAssemblies()){
                    list.Add(assembly.Name, string.Format("{0}, {1}={2}", assembly.Name, "version", assembly.Version));
                }
                return list;
            }
        }
        #endregion

        public AboutDialogViewModel(IResultFactory resultFactory) : base(resultFactory)
        {
            DisplayName = StringParser.Parse("${res:BVEEditor:StringResources:Common.Dialogs.Captions.About}");
        }

        public IEnumerable<IResult> PressedClose()
        {
            yield return Result.Close();
        }
    }
}
