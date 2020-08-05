using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml;
using DiGit.Helpers;

namespace DiGit.Converters
{
    public class IsFirstElementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            XmlElement eVersion = value as XmlElement;
            return eVersion.ParentNode.SelectNodes(eVersion.Name)[0] == eVersion;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
