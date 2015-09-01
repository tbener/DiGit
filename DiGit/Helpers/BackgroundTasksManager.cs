using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiGit.Model;
using DiGit.Properties;
using DiGit.Versioning;
using DiGit.Versioning;

namespace DiGit.Helpers
{
    public static class BackgroundTasksManager
    {
        public static void Start()
        {
            var t = new System.Timers.Timer();
            t.Interval = Settings.Default.ReadInfoDelaySec * 1000;
            t.AutoReset = false;
            t.Elapsed += (sender, args) => UpdateManager.CheckRemoteAsync();
            t.Start();

            t = new System.Timers.Timer();
            t.Interval = Settings.Default.WriteInfoDelaySec * 1000;
            t.AutoReset = false;
            t.Elapsed += (sender, args) => Task.Factory.StartNew(UserManager.UpdateInfo).ContinueWith(task => ConfigurationHelper.Configuration.ver = AppInfo.AppVersion.ToString());
            t.Start();

            t = new System.Timers.Timer();
            t.Interval = TimeSpan.FromHours(Settings.Default.CheckUpdateIntervalHr).TotalMilliseconds;
            t.AutoReset = true;
            t.Elapsed += (sender, args) => UpdateManager.CheckRemoteAsync();
            t.Start();
        }
    }
}
