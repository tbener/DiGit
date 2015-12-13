using System;

namespace SchedulerK
{
    public sealed class RecurrenceClass : EventClass
    {
        public override event ActivatedEventHandler OnActivated;

        #region CTOR

        /// <summary>
        /// Create a Recurrence class with given interval with a given first event.
        /// </summary>
        /// <param name="interval">Recurrence interval</param>
        /// <param name="firstEventTime">Indicates first event time. If it occurs in the past it will be calculated to the next future event</param>
        public RecurrenceClass(TimeSpan interval, DateTime firstEventTime)
            : base(interval)
        {
            DateTimeEvent = firstEventTime;
        }

        /// <summary>
        /// Create a Recurrence class with interval that will begin when the scheduler is activated
        /// </summary>
        /// <param name="interval">Recurrence interval</param>
        public RecurrenceClass(TimeSpan interval)
            : this(interval, DateTime.MinValue)
        { }


        #endregion

        public override void Activate()
        {
            if (OnActivated != null)
                OnActivated(this);
            Compute();
        }

        /// <summary>
        /// Compute next event time
        /// </summary>
        /// 
        /// 
        public override void Compute()
        {
            DateTime dt = DateTimeEvent;

            if (dt == DateTime.MinValue)
                dt = DateTime.Now;

            while (dt <= DateTime.Now)
                dt = dt.Add(Interval);

            DateTimeEvent = dt;
        }


        public override string ToString()
        {
            return string.Format("Recurrence, next event: {0}, every {1}", DateTimeEvent, Interval);
        }
    }
}
