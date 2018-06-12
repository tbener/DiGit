using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiGit.Model;
using DiGit.Properties;
using DiGit.Versioning;
using DiGit.Versioning;
using SchedulerK;

namespace DiGit.Helpers
{
    public static class BackgroundTasksManager
    {
        public static void Start()
        {
            Scheduler sch = Scheduler.SharedInstance;
            sch.TimerElapsedEvent += SchedulerOnTimerElapsedEvent;
            
            // Daily update check
            // Check for updates a few seconds after startup and then every day on the same time
            RecurrenceClass rec = new RecurrenceClass(new TimeSpan(1, 0, 0, 0), DateTime.Now.AddSeconds(Settings.Default.ReadInfoDelaySec))
            {
                Description = "Check update",
                Tag = "check_update"
            };
            //rec.OnActivated += @class => UpdateManager.CheckRemoteAsync();
            rec.OnActivated += RecurrenceCheckUpdate_OnActivated;
            sch.Add(rec);

            //// Update user info on server a few seconds after startup
            //EventClass evt = new EventClass(DateTime.Now.AddSeconds(Settings.Default.WriteInfoDelaySec))
            //{
            //    Description = "Update user info"
            //};
            //evt.OnActivated += @class => Task.Factory.StartNew(UserManager.UpdateInfo).ContinueWith(task => ConfigurationHelper.Configuration.ver = AppInfo.AppVersion.ToString());
            //sch.Add(evt);

            sch.Start();

            /*
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
             */
        }

        private static void RecurrenceCheckUpdate_OnActivated(IEventClass evt)
        {
            //When to check for updates:
            // On the first day - every hour.
            // After that - every day.
            evt.Interval = ConfigurationHelper.Configuration.versionUpdated.AddDays(1) > DateTime.Now ? new TimeSpan(0, 1, 0, 0) : new TimeSpan(1, 0, 0, 0);
            evt.Compute();

            UserManager.UpdateInfo("nextScheduledEvent", evt.ToString());
            UpdateManager.CheckRemoteAsync();
        }

        private static void SchedulerOnTimerElapsedEvent(object sender, IEventClass evt)
        {
            Scheduler s = Scheduler.SharedInstance;
            IEventClass nextEvent = s.GetNextEvent();
            string message = string.Format("Next event: '{0}' on {1}", nextEvent.Description,
                evt.DateTimeEvent.ToString("g"));
            UserManager.AddLog("Scheduler event", evt.Description, message);

            
        }
    }
}
