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
            return orgVersion.CompareTo(version) > 0;
        }
    }
}
