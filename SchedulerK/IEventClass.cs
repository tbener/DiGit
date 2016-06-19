using System;

namespace SchedulerK
{

    public delegate void DeletedEventHandler(IEventClass evt);
    public delegate void ActivatedEventHandler(IEventClass evt);
    public delegate void EventTimeChangedEventHandler(IEventClass evt);

    public interface IEventClass : IComparable
    {
        event DeletedEventHandler OnDeleted;
        event ActivatedEventHandler OnActivated;
        event EventTimeChangedEventHandler OnTimeChanged;

        object Tag { get; set; }

        /// <summary>
        /// Get or set the description of the event
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Returns the destination time of this event
        /// </summary>
        DateTime DateTimeEvent { get; }
        TimeSpan Interval { get; set; }

        /// <summary>
        /// This method is called by Scheduler object to raise a specific event for
        /// the current event class.
        /// </summary>
        void Activate();

        int EventId { get; set; }

        void Compute();

        bool IsDone { get; set; }
    }
}