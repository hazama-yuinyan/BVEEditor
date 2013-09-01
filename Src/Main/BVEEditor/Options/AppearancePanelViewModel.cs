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
        #region Binding sources
        /// <summary>
        /// Gets available UI languages.
        /// </summary>
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

        /// <summary>
        /// Gets all themes installed and loaded.
        /// </summary>
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
        #endregion

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
            SelectedUILanguage = UILanguages.First(lang => lang.Name == LocalizeDictionary.CurrentCulture.Name);
        }

        public override bool SaveOptions()
        {
            LocalizeDictionary.Instance.Culture = SelectedUILanguage;
            return true;
        }
    }
}
