using System;

namespace SchedulerK
{


    public class EventClass : IEventClass
    {

        public event DeletedEventHandler OnDeleted;
        public virtual event ActivatedEventHandler OnActivated;
        public event EventTimeChangedEventHandler OnTimeChanged;

        public EventClass()
        {
            IsActive = true;
        }

        public EventClass(DateTime dateTime)
            : this()
        {
            _eventTime = dateTime;
        }

        public EventClass(TimeSpan interval)
        {
            Interval = interval;
            _eventTime = DateTime.MinValue;
        }

        /// <summary>
        /// Gets or sets the destination time for the event
        /// </summary>
        private DateTime _eventTime;

        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Determines whether this event is active and will raise an event
        /// </summary>
        public bool IsActive { get; set; }

        public object Tag { get; set; }

        public string Description { get; set; }

        public int EventId { get; set; }

        public virtual void Compute()
        {
            if (_eventTime == DateTime.MinValue)
                DateTimeEvent = _eventTime.Add(Interval);
        }

        public bool IsDone { get; set; }


        /// <summary>
        /// Returns the destination time of this event
        /// </summary>
        public DateTime DateTimeEvent
        {
            get
            {
                return _eventTime;
            }
            set
            {
                if (_eventTime != value)
                {
                    _eventTime = value;
                    if (OnTimeChanged != null)
                        OnTimeChanged(this);
                }
            }
        }



        /// <summary>
        /// CompareTo (IComparable) implements comparing
        /// the dates of the events
        /// </summary>
        /// <param name="evt">An EventClass object to compare</param>
        /// <returns></returns>
        public int CompareTo(object evt)
        {
            var otherEvt = (IEventClass)evt;
            return DateTimeEvent.CompareTo(otherEvt.DateTimeEvent);
        }
        /// <summary>
        /// This method is called by Scheduler object to raise a specific event for
        /// the current event class.
        /// </summary>
        public virtual void Activate()
        {
            IsDone = true;
            if (OnActivated != null)
                OnActivated(this);
        }

        ~EventClass()
        {
            if (OnDeleted != null)
                OnDeleted(this);
        }

        public override string ToString()
        {
            return string.Format("Event at: {0}", DateTimeEvent);
        }
    }



}