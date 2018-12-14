using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTB.AppInsights
{
    public static class Extensions
    {
        public static string PaddedVersion(this Version version, int majorpad, int minorpad, int buildpad, int revisionpad)
        {
            return string.Format($"{{0:D{majorpad}}}.{{1:D{minorpad}}}.{{2:D{buildpad}}}.{{3:D{revisionpad}}}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
