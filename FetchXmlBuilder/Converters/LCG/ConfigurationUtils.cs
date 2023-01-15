using McTools.Xrm.Connection;
using System.IO;

namespace Rappen.XTB.Helper
{
    public static class ConfigurationUtils
    {
        public static T GetEmbeddedConfiguration<T>(string filename, string begintoken, string endtoken)
        {
            var csfile = File.ReadAllText(filename);
            var configstr = csfile.GetTextBetween(begintoken, endtoken, false);
            if (string.IsNullOrEmpty(configstr))
            {
                throw new FileLoadException("Could not find configuration token in file.", filename);
            }
            var configname = GetSimpleClassName<T>();
            configstr = configstr.GetTextBetween($"<{configname}", $"</{configname}>", true);
            if (string.IsNullOrEmpty(configstr))
            {
                throw new FileLoadException($"Could not find {configname} XML in file.", filename);
            }
            var inlinesettings = (T)XmlSerializerHelper.Deserialize(configstr, typeof(T));
            return inlinesettings;
        }

        private static string GetSimpleClassName<T>()
        {
            var configname = typeof(T).ToString();
            var confignameparts = configname.Split('.');
            configname = confignameparts[confignameparts.Length - 1];
            return configname;
        }

        private static string GetTextBetween(this string text, string begin, string end, bool includebeginend)
        {
            var beginpos = text.IndexOf(begin);
            if (beginpos < 0)
            {
                return string.Empty;
            }
            text = text.Substring(beginpos + (includebeginend ? 0 : begin.Length));
            var endpos = text.IndexOf(end);
            if (endpos < 0)
            {
                return string.Empty;
            }
            text = text.Substring(0, endpos + (includebeginend ? end.Length : 0)).Trim();
            return text;
        }
    }
}
