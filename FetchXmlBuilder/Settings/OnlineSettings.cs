using Rappen.AI.WinForm;
using Rappen.XTB.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using XrmToolBox.Extensibility;
using XrmToolBox.ToolLibrary.AppCode;

namespace Rappen.XTB.FXB.Settings
{
    public class OnlineSettings
    {
        private const string FileName = "Rappen.XTB.FXB.Settings.xml";
        private static readonly Uri ToolSettingsURLPath = new Uri("https://rappen.github.io/Tools/");
        private static OnlineSettings instance;

        public int SettingsVersion = 1;
        public List<string> DeprecatedNames = new List<string>();
        public List<string> IntegratedToTools = new List<string>();
        public AiSupport AiSupport = new AiSupport();
        public ToolColors Colors = new ToolColors();

        private OnlineSettings()
        { }

        public static OnlineSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Uri(ToolSettingsURLPath, FileName).DownloadXml<OnlineSettings>() ?? new OnlineSettings();
                    //instance.Save();
                }
                return instance;
            }
        }

        public void Save()
        {
            if (!Directory.Exists(Paths.SettingsPath))
            {
                Directory.CreateDirectory(Paths.SettingsPath);
            }
            string path = Path.Combine(Paths.SettingsPath, FileName);
            XmlSerializerHelper.SerializeToFile(this, path);
        }
    }

    public class ToolColors
    {
        public Color Dark { get; set; } = Color.FromArgb(0, 66, 173);
        public Color Medium { get; set; } = Color.FromArgb(0, 99, 255);
        public Color Bright { get; set; } = Color.FromArgb(255, 255, 0);
    }
}