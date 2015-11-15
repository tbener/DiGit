using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DiGit.Helpers;

namespace DiGit.Converters
{
    public class VersionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Version checkVer = value as Version;
            Version curVer = Version.Parse(ConfigurationHelper.Configuration.ver);
            if (checkVer > curVer) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
