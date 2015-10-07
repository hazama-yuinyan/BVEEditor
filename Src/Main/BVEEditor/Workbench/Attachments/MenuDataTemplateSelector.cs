using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// DataTemplateSelector for menus.
    /// </summary>
    public class MenuDataTemplateSelector : ItemContainerTemplateSelector
    {
        const string BaseUri = "/Workbench/Attachments/Views/";
        static readonly ILog Logger = LogManager.GetLog(typeof(MenuDataTemplateSelector));

        public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
        {
            var menu_item = item as IMenu;
            if(menu_item != null){
                Logger.Info("Creating view for the menu '{0}'.", menu_item.MenuName);
                string base_uri = (menu_item.ReferenceAssemblyName != null) ? menu_item.ReferenceAssemblyName + ";component/" : BaseUri;
                return ResolveDataTemplate(base_uri, menu_item.MenuName);
            }

            Logger.Warn("This class is applying to a non-menu item(named {0}). Maybe something wrong is happening?", item);
            return null;
        }

        DataTemplate ResolveDataTemplate(string baseUri, string menuName)
        {
            if(!baseUri.StartsWith("/"))
                baseUri = "/" + baseUri;

            string template_file_name = menuName + "MenuTemplate.xaml";
            var template_uri = new Uri(baseUri + template_file_name, UriKind.Relative);
            var template = Application.LoadComponent(template_uri) as DataTemplate;
            if(template == null){
                Logger.Warn("The menu template wasn't resolved.");
                return null;
            }
            return template;
        }
    }
}
