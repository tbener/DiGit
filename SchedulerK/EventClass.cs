using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SchedulerK
{

    public delegate void DeletedEventHandler(object sender);
    public delegate bool ActivatedEventHandler(object sender);

    public class EventClass : IComparable
    {

        public event DeletedEventHandler OnDeleted;
        public event ActivatedEventHandler OnActivated;

        public EventClass()
        {
            IsActive = true;
        }

        public EventClass(DateTime dateTime) : this()
        {
            EventTime = dateTime;
        }
        public EventClass(TimeSpan timeSpan) : this(DateTime.Now.Add(timeSpan)){}

        public Int32 EventID;

        private object _Tag;
        private string _Description;

        /// <summary>
        /// Gets or sets the destination time for the event
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// The offset from EventTime of which the event should occure
        /// </summary>
        public TimeSpan Offset { get; set; }

        /// <summary>
        /// Determines whether this event is of a recurrence type
        /// </summary>
        public bool IsRecurrence { get; set; }

        /// <summary>
        /// Determines whether this event is active and will raise an event
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the recurrence class of this event
        /// </summary>
        public RecurrenceClass Recurrence { get; set; }

        public object Tag
        {
            // if part of recurrence get its Tag.
            get
            {
                if (_Tag == null && IsRecurrence)
                {
                    if (Recurrence.Tag != null)
                    {
                        _Tag = Recurrence.Tag;
                    }
                }
                return _Tag;
            }
            set { _Tag = value; }
        }

        /// <summary>
        /// Get or set the description of the event
        /// </summary>
        public string Description
        {
            // if part of recurrence get its Description.
            get
            {
                if (_Description == "" && IsRecurrence)
                {
                    if (Recurrence.Description != "")
                    {
                        _Description = Recurrence.Description;
                    }
                }
                return _Description;
            }
            set { _Description = value; }
        }


        /// <summary>
        /// Returns the destination time of this event
        /// </summary>
        public DateTime DateTimeEvent
        {
            get
            {
                return EventTime.Add(Offset);
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
            EventClass otherEvt = (EventClass)evt;
            return DateTimeEvent.CompareTo(otherEvt.DateTimeEvent);
        }
        /// <summary>
        /// This method is called by Scheduler object to raise a specific event for
        /// the current event class.
        /// </summary>
        internal bool Activate()
        {

            if (OnActivated != null)
                return OnActivated(this);
            return true;
        }

        ~EventClass()
        {
            if (OnDeleted != null)
                OnDeleted(this);
        }

    }



}