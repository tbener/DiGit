using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace SchedulerK
{
    public delegate void EventActivated(object sender, IEventClass evt);

    public class Scheduler
    {
        private const string TEMPLATE_ERR_OCCURRED = "An error occurred while trying to {0}";

        #region Members

        private readonly Timer _timer;
        private bool _enabled;

        private readonly List<IEventClass> _eventList = new List<IEventClass>();

        #endregion

        #region Constructors + Destructors

        public Scheduler()
        {
            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Elapsed += timer_Elapsed;
            DefaultInterval = new TimeSpan(1, 0, 0);    // set default interval to 1 hour
        }

        ~Scheduler()
        {
            Stop();
        }

        #endregion

        #region Static Handling

        private static Scheduler _staticScheduler;

        public static Scheduler SharedInstance
        {
            get { return _staticScheduler ?? (_staticScheduler = new Scheduler()); }
        }

        #endregion

        #region Public properties

        public TimeSpan DefaultInterval { get; set; }

        #endregion

        #region Public delegates

        public event EventActivated TimerElapsedEvent;

        #endregion

        #region Public Methods

        public IEventClass Add(IEventClass evt)
        {
            try
            {
                if (!_eventList.Contains(evt))
                {
                    evt.OnActivated += OnEventActivated;
                    _eventList.Add(evt);
                    if (_enabled)
                    {
                        evt.Compute();
                        SetNextTimer();
                    }
                }

                return evt;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(TEMPLATE_ERR_OCCURRED, "add an event"), ex);
            }
        }

        public void Remove(IEventClass evt)
        {
            try
            {
                if (_eventList.Contains(evt))
                {
                    evt.OnActivated -= OnEventActivated;
                    _eventList.Remove(evt);
                    if (_enabled) SetNextTimer();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(TEMPLATE_ERR_OCCURRED, "remove an event"), ex);
            }
        }

        //public EventClass AddEvent(DateTime dateTime, string tag, string description)
        //{
        //    EventClass evt = new EventClass(dateTime);
        //    evt.EventID = _newEventId++;
        //    evt.Tag = tag;
        //    evt.Description = description;

        //    return SetEvent(evt);
        //}

        //public RecurrenceClass AddEventRecurrence(RecurrenceClass rec)
        //{
        //    //AddEvent(rec.NextEvent);
        //}


        public IEventClass GetNextEvent()
        {
            if (_eventList.Count > 0)
            {
                _eventList.Sort();
                return _eventList.First();
            }
            return null;
        }

        #endregion


        #region Control Methods

        public Scheduler Start()
        {
            _eventList.ForEach(e => e.Compute());
            _enabled = true;
            SetNextTimer();
            return this;
        }


        public void Stop()
        {
            _enabled = false;
            _timer.Stop();
        }

        public void Clear()
        {
            Stop();
            _eventList.Clear();
        }

        #endregion


        #region Private Methods

        private void SetNextTimer()
        {
            IEventClass evt;

            _timer.Stop();
            if (!_enabled) return;

            // we will raise event for every scheduled event that its time has passed
            while (_eventList.Count > 0)
            {
                _eventList.Sort();
                evt = _eventList.First();

                if (evt.DateTimeEvent <= DateTime.Now)
                {
                    evt.Activate();

                    if (evt.IsDone)
                    {
                        // reccurence - add back to re-sort
                        _eventList.Remove(evt);
                    }

                }
                else
                {
                    break;
                }
            }

            // set the next timer
            if (_eventList.Count > 0)
            {
                _eventList.Sort();
                evt = _eventList.First();
                try
                {
                    TimeSpan timeSpan = evt.DateTimeEvent - DateTime.Now;
                    if (timeSpan.TotalMilliseconds > 0)
                    {
                        if (timeSpan > DefaultInterval) timeSpan = DefaultInterval;
                        _timer.Interval = timeSpan.TotalMilliseconds;
                        Debug.Print("Set timer to {0}", DateTime.Now.Add(timeSpan).ToShortTimeString());
                    }
                    else
                    {
                        SetNextTimer();
                        return;
                    }
                }
                catch
                {
                    _timer.Interval = DefaultInterval.Milliseconds;
                    Debug.Print("Set timer to {0}", DateTime.Now.Add(DefaultInterval).ToShortTimeString());
                }

                _timer.Start();
            }

        }

        private void OnEventActivated(IEventClass evt)
        {
            if (TimerElapsedEvent != null)
                TimerElapsedEvent(this, evt);
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetNextTimer();
        }

        #endregion

    }

}