using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Themes;
using ICSharpCode.Core;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Providers;

namespace BVEEditor.Options
{
    public class AppearancePanelViewModel : OptionPanelViewModel
    {
        public IList<CultureInfo> UILanguages{
            get{return ResxLocalizationProvider.Instance.AvailableCultures;}
        }

        public CultureInfo SelectedUILanguage{
            get{return LocalizeDictionary.CurrentCulture;}
            set{
                if(LocalizeDictionary.CurrentCulture != value){
                    LocalizeDictionary.Instance.Culture = value;
                    NotifyOfPropertyChange(() => SelectedUILanguage);
                }
            }
        }

        public IList<Theme> Themes{
            get{return new List<Theme>();}
        }

        Theme current_theme;
        public Theme SelectedTheme{
            get{return current_theme;}
            set{
                if(current_theme != value){
                    current_theme = value;
                    NotifyOfPropertyChange(() => SelectedTheme);
                }
            }
        }

        public AppearancePanelViewModel(IPropertyService propertyService) : base(propertyService)
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        public override void LoadOptions()
        {
            var props = app_settings.NestedProperties("Generics");
            var culture_name = props.Get<string>("UserCultureName", "en-US");
            LocalizeDictionary.Instance.Culture = CultureInfo.CreateSpecificCulture(culture_name);
        }

        public override bool SaveOptions()
        {
            var props = app_settings.NestedProperties("Generics");
            props.Set("UserCultureName", LocalizeDictionary.CurrentCulture.Name);
            return true;
        }
    }
}
