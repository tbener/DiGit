using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerK
{
    public class RecurrenceClass
    {
        public Int32 RecurrenceID;
        public eRecurrenceTypes RecurrenceType;
        public object Tag;
        public string Description;

        public DateTime dateTimeValue;
        public Int32 numberValue;

        public TimeSpan Interval;
        public DateTime StartDate;
        public DateTime EndDate;
        public Int32 EndAfterCount = 0;
        public TimeSpan Offset;

        public RecurrenceClass()
        {
        }

        #region GetOccurrences

        public DateTime GetOccurrences()
        {
            return GetOccurrences(DateTime.Now, DateTime.Now)[0];
        }

        public DateTime[] GetOccurrences(DateTime fromTime, DateTime toTime)
        {
            DateTime dtStart = fromTime;
            if (StartDate > fromTime) dtStart = StartDate;

            bool boolHasRange = (toTime > fromTime);
            TimeSpan timeSpan;

            DateTime[] dtResult = new DateTime[0];

            switch (RecurrenceType)
            {
                case eRecurrenceTypes.eRecurrenceTypes_Hourly:
                    dtResult[0] = dtStart.Date.AddHours(dtStart.Hour).AddMinutes(dateTimeValue.Minute);
                    if (dtResult[0] < DateTime.Now) dtResult[0] = dtResult[0].AddHours(1);
                    timeSpan = new TimeSpan(1, 0, 0);
                    break;
                case eRecurrenceTypes.eRecurrenceTypes_Daily:
                    timeSpan = new TimeSpan(1, 0, 0, 0);
                    break;
                case eRecurrenceTypes.eRecurrenceTypes_Weekly:
                    timeSpan = new TimeSpan(7, 0, 0, 0);
                    break;
                default:
                    timeSpan = new TimeSpan(1, 0, 0);
                    break;
            }

            DateTime next;
            if (boolHasRange)
            {
                int i = 0;
                do
                {
                    next = dtResult[i].Add(timeSpan);
                    if (next < toTime)
                        dtResult[++i] = next;
                    else
                        break;
                } while (i < 100);
            }

            return dtResult;

        }


        #endregion
    }
}
