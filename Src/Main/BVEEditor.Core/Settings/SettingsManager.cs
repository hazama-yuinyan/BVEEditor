using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Logging;
using ICSharpCode.Core;
using Caliburn.Micro;

namespace BVEEditor.Settings
{
    /// <summary>
    /// Manages settings. It is responsible for loading settings on startup and saving settings at shutdown.
    /// So you should use this class on properties which should be loaded on startup and saved at shutdown.
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        const string GlobalSettingsPath = "/BVEEditor/GlobalSettings";
        static readonly ILog Logger = LogManager.GetLog(typeof(SettingsManager));

        Properties settings_property;
        List<ISettingSnippet> settings = new List<ISettingSnippet>();

        public SettingsManager(IPropertyService propertyService)
        {
            settings_property = propertyService.NestedProperties("ApplicationSettings");
            var iss_descriptors = AddInTree.BuildItems<GlobalSettingDescriptor>(GlobalSettingsPath, null);
            foreach(var descriptor in iss_descriptors)
                settings.Add(descriptor.CreateSnippet());
        }

        #region ISettingsManager メンバー

        public void LoadSettings()
        {
            foreach(var item in settings){
                item.Load(this);
                Logger.Info("Loaded settings into {0} from PropertyService.", item.SnippetName);
            }
        }

        public bool SaveSettings()
        {
            foreach(var item in settings){
                if(item.Save(this))
                    Logger.Info("Saved settings in {0} to PropertyService.", item.SnippetName);
                else
                    return false;
            }

            return true;
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            var names = key.Split('.');
            Properties parent = settings_property;
            T val = default(T);

            for(int i = 0; i < names.Length; ++i){
                if(i < names.Length - 1)
                    parent = parent.NestedProperties(names[i]);
                else
                    val = parent.Get(names[i], defaultValue);
            }

            return val;
        }

        public void SetValue<T>(string key, T value)
        {
            var names = key.Split('.');
            Properties parent = settings_property;

            for(int i = 0; i < names.Length; ++i){
                if(i < names.Length - 1)
                    parent = parent.NestedProperties(names[i]);
                else
                    parent.Set(names[i], value);
            }
        }

        #endregion
    }
}
