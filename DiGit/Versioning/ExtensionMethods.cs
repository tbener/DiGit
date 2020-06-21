using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiGit.Versioning
{
    public static class ExtensionMethods
    {
        public static bool IsHigherThan(this Version orgVersion, Version version)
        {
            return orgVersion.CompareTo(version, false) > 0;
        }

        public static int CompareTo(this Version orgVersion, Version version, bool includeRevision = true)
        {
            if (includeRevision)
                return orgVersion.CompareTo(version);

            Version v1 = new Version(orgVersion.Major, orgVersion.Minor, orgVersion.Build);
            Version v2 = new Version(version.Major, version.Minor, version.Build);
            return v1.CompareTo(v2);
        }
    }
}
