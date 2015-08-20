using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections;

namespace SchedulerK
{
    public delegate void TimeElapsed(object sender, EventClass evt);

    public class SchedulerClass
    {
        #region Members

        private Timer _timer;
        private bool _Enabled = false;

        private SortedDictionary<EventClass, Int32> _sortedEvents = new SortedDictionary<EventClass, Int32>();
        private Hashtable _events = new Hashtable();
        private Int32 _newEventId = 0;
        private Int32 _newRecurrenceId = 0;

        #endregion

        #region Constructors + Destructors

        public SchedulerClass()
        {
            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        ~SchedulerClass()
        {
            Stop();
        }

        #endregion

        #region Public properties

        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }

        public EventClass GetEvent(int evtId)
        {
            if (_events.ContainsKey(evtId))
                return _events[evtId] as EventClass;
            return null;
        }

        #endregion

        #region Public delegates

        public event TimeElapsed timerElapsedEvent;

        #endregion

        #region Public Methods

        #region AddEvent / AddEventRecurrence

        public EventClass SetEvent(EventClass evt)
        {
            try
            {
                _sortedEvents[evt] = evt.EventID;
                _events[evt.EventID] = evt;

                // todo: this should be considered to be done on a different thread
                SetNextTimer();

                return evt;
            }
            catch { return null; }
        }

        public EventClass AddEvent(DateTime dateTime, string tag, string description)
        {
            EventClass evt = new EventClass(dateTime);
            evt.EventID = _newEventId++;
            evt.Tag = tag;
            evt.Description = description;

            return SetEvent(evt);
        }

        //public RecurrenceClass AddEventRecurrence(RecurrenceClass rec)
        //{
        //    //AddEvent(rec.NextEvent);
        //}

        //RecurrenceType As EvtTimer_RecurrenceTypeConst, DateValue As Date, Optional NumberValue As Long = -1, Optional Tag As String, Optional ID As String, Optional Description As String, Optional EndBy As Date = 0, Optional Count As Long = 0, Optional SecondsOffset As Long = 0) 
        public RecurrenceClass AddEventRecurrence(eRecurrenceTypes recurType, DateTime dt, Int32 num, string tag, string description, DateTime startDate, DateTime endDate, Int32 endAfterCount, TimeSpan offset)
        {
            RecurrenceClass rec = new RecurrenceClass();
            rec.RecurrenceType = recurType;
            rec.RecurrenceID = _newRecurrenceId++;
            rec.dateTimeValue = dt;
            rec.numberValue = num;
            rec.Tag = tag;
            rec.Description = description;
            rec.StartDate = startDate;
            rec.EndDate = endDate;
            rec.EndAfterCount = endAfterCount;
            rec.Offset = offset;

            // todo:

            // Compute next event

            // add to event list
            //rec.NextEvent =  AddEvent(...);

            // trigger ?


            return rec;
        }

        #endregion


        #region Control Methods

        public void Start()
        {
            _Enabled = true;
            SetNextTimer();
        }


        public void Stop()
        {
            _Enabled = false;
            _timer.Stop();
        }


        #endregion

        #endregion


        #region Private Methods

        private void SetNextTimer()
        {
            TimeSpan timeSpan;
            EventClass evt = null;

            _timer.Stop();
            if (!this._Enabled) return;

            // we will raise event for every scheduled event
            // that is in range - 
            // the range means in the past or in the next 0.5 second.
            while (_sortedEvents.Count > 0)
            {
                evt = _sortedEvents.Keys.First();
                if (evt.DateTimeEvent <= DateTime.Now)
                {
                    // RAISE THE SCHEDULER EVENTS!!!
                    if (timerElapsedEvent != null && evt.IsActive)
                    {
                        if (evt.Activate())     // event specific
                            timerElapsedEvent(this, evt);   // scheduler event
                    }

                    // remove from list
                    RemoveEvent(evt);
                    evt = null;
                }
                else
                {
                    break;
                }
            }

            // set the next timer
            if (evt != null)
            {
                try
                {
                    timeSpan = evt.DateTimeEvent - DateTime.Now;
                    if (timeSpan.TotalMilliseconds > 0)
                        _timer.Interval = timeSpan.TotalMilliseconds;
                    else
                    {
                        SetNextTimer();
                        return;
                    }
                }
                catch
                {
                    _timer.Interval = 60000;
                }

                _timer.Start();
            }

        }

        #endregion

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetNextTimer();
        }

        public void RemoveEvent(EventClass evt)
        {
            try
            {
                if (_sortedEvents.ContainsKey(evt))
                {
                    _sortedEvents.Remove(evt);
                    _events.Remove(evt.EventID);
                    SetNextTimer();
                }
            }
            catch
            { }
        }

        public void RemoveEvent(int evtId)
        {
            var evt = _events[evtId];
            if (evt != null)
                RemoveEvent((EventClass)evt);
        }

        private class Utils
        {

        }
    }

}