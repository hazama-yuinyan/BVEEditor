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
            var user_culture_id = manager.GetValue("Generics.UserCultureID", CultureInfo.InvariantCulture.LCID);
            LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(user_culture_id);
        }

        public bool Save(ISettingsManager manager)
        {
            manager.SetValue("Generics.UserCultureID", LocalizeDictionary.CurrentCulture.LCID);
            return true;
        }

        #endregion
    }
}
