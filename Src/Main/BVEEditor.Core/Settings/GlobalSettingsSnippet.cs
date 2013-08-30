using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;
using WPFLocalizeExtension.Engine;

namespace BVEEditor.Settings
{
    /// <summary>
    /// A SettingSnippet that manipulates application-wide settings.
    /// </summary>
    public class GlobalSettingsSnippet : ISettingSnippet
    {
        #region ISettingSnippet メンバー

        public string SnippetName{get; set;}

        public void Load(ISettingsManager manager)
        {
            var user_culture_name = manager.GetValue("Generics.UserCultureName", "en-US");
            LocalizeDictionary.Instance.Culture = CultureInfo.CreateSpecificCulture(user_culture_name);
        }

        public bool Save(ISettingsManager manager)
        {
            manager.SetValue("Generics.UserCultureName", LocalizeDictionary.CurrentCulture.Name);
            return true;
        }

        #endregion
    }
}
