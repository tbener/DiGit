using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiGit.Helpers;

namespace DiGit.Model
{
    public class UserStat
    {
        private DateTime lastUpdate = DateTime.MinValue;
        public List<ActivityInfoClass> Activities;
        public UserInfoClass UserInfo;

        public UserStat()
        {
            Activities = new List<ActivityInfoClass>();
        }

        public enum ActivityType
        {
            Startup,
            Shutdown
        }

        public class ActivityInfoClass
        {
            public DateTime Date
            {
                get { return _date; }
                set
                {
                    _date = value;
                    DateString = _date.ToString("yyyy-MM-dd HH:MM:ss");
                }
            }

            public ActivityType ActivityType { 
                set { Activity = Enum.GetName(typeof (ActivityType), value); } 
            } 
            public string Activity;
            private DateTime _date;
            public string DateString;
        }

        public class UserInfoClass
        {
            public string MachineName;
            public string Version;
        }

        public void Update()
        {
            
            lock (this)
            {
                if (UserInfo == null) UserInfo = new UserInfoClass()
                {
                    MachineName = Environment.MachineName,
                    Version = AppInfo.AppVersionString
                };

                ServerCommunicationHelper.Update(this);
                lastUpdate = DateTime.Now;
            }
        }
    }
}
