using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using DiGit.Helpers;
using DiGit.Properties;

namespace DiGit.Model
{
    public static class UserManager
    {
        public static void UpdateInfo()
        {
            try
            {
                bool needUpdate = !Settings.Default.UserRegistered;
                if (!needUpdate)
                {
                    Version configVersion;
                    bool firstRun = true;
                    if (Version.TryParse(ConfigurationHelper.Configuration.ver, out configVersion))
                        firstRun = configVersion != AppInfo.AppVersion;
                    needUpdate = firstRun;
                }
                if (needUpdate)
                {
                    string fileName = Path.Combine(Settings.Default.InfoUrl, Environment.MachineName + ".xml");
                    XDocument xDoc = new XDocument();
                    xDoc.Add(new XElement("DiGit",
                        new XAttribute("ver", AppInfo.AppVersion.ToString()),
                        new XAttribute("date", DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("isBetaUser", ConfigurationHelper.Configuration.isBetaUser)
                        ));
                    xDoc.Save(fileName);
                    Settings.Default.UserRegistered = false;
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, false);
            }
        }
    }
}
