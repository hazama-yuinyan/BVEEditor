using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace BVEEditor.Settings
{
    /// <summary>
    /// Interface for Settings manager.
    /// </summary>
    public interface ISettingsManager
    {
        void LoadSettings();
        bool SaveSettings();
        T GetValue<T>(string key, T defaultValue);
        void SetValue<T>(string key, T value);
    }

    /// <summary>
    /// Represents a snippet of settings. It is responsible for loading a piece of settings on startup
    /// and saving them at shutdown.
    /// </summary>
    public interface ISettingSnippet
    {
        string SnippetName{get; set;}
        void Load(ISettingsManager manager);
        bool Save(ISettingsManager manager);
    }
}
