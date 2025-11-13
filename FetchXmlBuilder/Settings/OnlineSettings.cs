using Rappen.AI.WinForm;
using Rappen.XTB.Helpers;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FXB.Settings
{
    public class OnlineSettings
    {
        private const string FileName = "Rappen.XTB.FXB.Settings.xml";
        private static readonly string ToolSettingsURLPath = "https://rappen.github.io/Tools/";
        private static OnlineSettings instance;

        public int SettingsVersion = 1;
        public List<string> DeprecatedNames = new List<string>();
        public List<string> IntegratedToTools = new List<string>();
        public AiSupport AiSupport = new AiSupport();
        public ToolColors Colors = new ToolColors();

        public OnlineSettings()
        { }

        public static OnlineSettings Instance
        {
            get
            {
                if (instance == null)
                {
#if DEBUG
                    var path = Path.Combine(Paths.SettingsPath, FileName);
                    if (!File.Exists(path))
                    {
                        MessageBoxEx.Show($"DEBUG MODE:\n\nSettings file '{path}' not found. Now it's created by default.\nIt can be found here:\n{ToolSettingsURLPath}{FileName}");
                    }
#endif
                    instance = XmlAtomicStore.DownloadXml<OnlineSettings>(ToolSettingsURLPath, FileName, Paths.SettingsPath);
                }
                return instance;
            }
        }

        public static void Reset() => instance = null;

        public void Save() => XmlAtomicStore.Serialize(this, Path.Combine(Paths.SettingsPath, FileName));
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