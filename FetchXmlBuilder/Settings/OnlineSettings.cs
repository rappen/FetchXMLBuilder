using Rappen.AI.WinForm;
using Rappen.XTB.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
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
#if DEBUG
                    instance = Load<OnlineSettings>(FileName) ?? new OnlineSettings();
                    //instance.Save();
#else
                    instance = new Uri(ToolSettingsURLPath, FileName).DownloadXml<OnlineSettings>() ?? new OnlineSettings();
#endif
                }
                return instance;
            }
        }

        public static void Reset() => instance = null;

        private static T Load<T>(string filename)
        {
            var path = Path.Combine(Paths.SettingsPath, FileName);
            if (!File.Exists(path))
            {
                MessageBoxEx.Show($"DEBUG MODE:\n\nSettings file '{path}' not found. Now it's created by default.\nIt can be found here:\n{ToolSettingsURLPath}{FileName}");
                return default(T);
            }
            var file = File.ReadAllText(path);
            return (T)XmlSerializerHelper.Deserialize(file, typeof(T));
        }

        public void Save()
        {
            if (!Directory.Exists(Paths.SettingsPath))
            {
                Directory.CreateDirectory(Paths.SettingsPath);
            }
            var path = Path.Combine(Paths.SettingsPath, FileName);
            XmlSerializerHelper.SerializeToFile(this, path);
        }
    }

    public class ToolColors
    {
        public string DarkColor = "FF0042AD"; // Default dark color
        public string MediumColor = "FF0063FF"; // Default medium color
        public string BrightColor = "FFFFFF00"; // Default bright color
        public Color Dark => GetColor(DarkColor, "FF0042AD");
        public Color Medium => GetColor(MediumColor, "FF0063FF");
        public Color Bright => GetColor(BrightColor, "FFFFFF00");

        private Color GetColor(string color, string defaultColor)
        {
            int intColor;
            try
            {
                intColor = int.Parse(color, System.Globalization.NumberStyles.HexNumber);
            }
            catch
            {
                intColor = int.Parse(defaultColor, System.Globalization.NumberStyles.HexNumber);
            }
            return Color.FromArgb(intColor);
        }
    }
}