using System;
using DiGit.Model;
using DiGit.Properties;
using DiGit.Versioning;
using SchedulerK;

namespace DiGit.Helpers
{
    public static class BackgroundTasksManager
    {
        public static void Start()
        {
            Scheduler sch = Scheduler.SharedInstance;

            StartCheckUpdateScheduler(sch);

            //// Update user info on server a few seconds after startup
            //EventClass evt = new EventClass(DateTime.Now.AddSeconds(Settings.Default.WriteInfoDelaySec))
            //{
            //    Description = "Update user info"
            //};
            //evt.OnActivated += @class => Task.Factory.StartNew(UserManager.UpdateInfo).ContinueWith(task => ConfigurationHelper.Configuration.ver = AppInfo.AppVersion.ToString());
            //sch.Add(evt);

            if (sch.EventList.Count > 0)
            {
                sch.TimerElapsedEvent += SchedulerOnTimerElapsedEvent;

                sch.Start();
            }
        }
        
        private static void StartCheckUpdateScheduler(Scheduler scheduler)
        {
            if (string.IsNullOrEmpty(Settings.Default.VersionInfoUrl) || string.IsNullOrEmpty(Settings.Default.InfoBaseFileName))
                return;

            // Daily update check
            // Check for updates a few seconds after startup and then every day on the same time
            RecurrenceClass rec = new RecurrenceClass(new TimeSpan(1, 0, 0, 0), DateTime.Now.AddSeconds(Settings.Default.ReadInfoDelaySec))
            {
                Description = "Check update",
                Tag = "check_update"
            };
            //rec.OnActivated += @class => UpdateManager.CheckRemoteAsync();
            rec.OnActivated += RecurrenceCheckUpdate_OnActivated;
            scheduler.Add(rec);
        }

        private static void RecurrenceCheckUpdate_OnActivated(IEventClass evt)
        {
            //When to check for updates:
            // On the first day - every hour.
            // After that - every day.
            evt.Interval = ConfigurationHelper.Configuration.versionUpdated.AddDays(1) > DateTime.Now ? new TimeSpan(0, 1, 0, 0) : new TimeSpan(1, 0, 0, 0);
            evt.Compute();

            UserManager.UpdateInfo("nextScheduledEvent", evt.ToString());
            UpdateManager.CheckUpdateAsync();
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
