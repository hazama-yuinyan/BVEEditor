using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace BVEEditor.Settings
{
    /// <summary>
    /// Creates GlobalSettingDescriptor objects.
    /// GlobalSettings are the settings that are automatically loaded on startup
    /// and saved at shutdown.
    /// </summary>
    /// <attribute name="className" use="required">
    /// Name of the ISettingSnippet.
    /// </attribute>
    /// <usage>Only in /BVEEditor/GlobalSettings</usage>
    /// <returns>
    /// A GlobalSettingDescriptor object.
    /// </returns>
    /// <example title="Global setting: UILanguage">
    /// &lt;Path name = "/BVEEditor/GlobalSettings"&gt;
    ///   &lt;GlobalSetting id    = "UILanguage"
    ///                   className = "Settings.GlobalSettingSnippet"
    /// &lt;/Path&gt;
    /// </example>
    public class GlobalSettingDoozer : IDoozer
    {
        #region IDoozer メンバー

        public bool HandleConditions{
            get{return false;}
        }

        public object BuildItem(BuildItemArgs args)
        {
            return new GlobalSettingDescriptor(args.Codon);
        }

        #endregion
    }
}
