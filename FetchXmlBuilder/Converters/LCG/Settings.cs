using System.Collections.Generic;
using System.IO;

namespace Rappen.XTB.LCG
{
    public class Settings
    {
        public Settings()
        {
            commonsettings = new CommonSettings();
        }

        public NameType ConstantName { get; set; } = NameType.DisplayName;
        public bool ConstantCamelCased { get; set; }
        public bool DoStripPrefix { get; set; }
        public string StripPrefix { get; set; }

        internal CommonSettings commonsettings;
    }

    public enum NameType
    {
        DisplayName = 0,
        SchemaName = 1,
        LogicalName = 2
    }
}