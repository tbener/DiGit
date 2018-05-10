
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System;
using DiGit.Commands;
using DiGit.Helpers;

namespace DiGit.ViewModel.Base
{
    /// <summary>
    /// This abstract class used for windows that appear for a limited time.
    /// </summary>
    abstract class BaseNotificationViewModel : BaseViewModel
    {
        public event EventHandler OnTimerEnd;
        public event EventHandler OnStart;

        public ICommand CloseCommand { get; private set; }

        private readonly Window _view;
        private readonly DispatcherTimer _dispTimer;

        protected BaseNotificationViewModel(Window view)
        {
            _view = view;
            
            Action action = () => CloseView(view);
            view.GetType().GetProperty("OnFadeComplete").SetValue(view, action, null);

            CloseCommand = new RelayCommand(OnCloseCommand);

            _dispTimer = new DispatcherTimer();
            _dispTimer.Tick += timer_tick;
            _dispTimer.Interval = TimeSpan.FromSeconds(1);
        }

        public void SetPosition()
        {
            Hardcodet.Wpf.TaskbarNotification.Interop.Point p = Hardcodet.Wpf.TaskbarNotification.Interop.TrayInfo.GetTrayLocation();
            _view.Left = p.X - _view.Width - 5;
            _view.Top = p.Y - _view.Height - 5;

        }

        public abstract Point OnSetPosition(Window view);

        public abstract void UpdateProperties();

        public virtual void OnCloseCommand()
        {
            CloseWindow();
        }

        public void StartTimer()
        {
            _dispTimer.Start();
        }

        public void StartShow()
        {
            BeforeShow();
            SetPosition();
            UpdateProperties();
            _view.Show();
            CountDownValue = Duration;
            StartTimer();
            OnStart?.Invoke(this, new EventArgs());
        }

        public void CloseWindow()
        {
            _dispTimer.Stop();
            CloseView(_view);
        }

        public void HideWindow()
        {
            _view.Hide();
        }

        public virtual void CloseView(Window view)
        {
            view.Hide();
        }

        protected virtual void BeforeShow() { }

        public void StopTimer()
        {
            _dispTimer.Stop();
            CountDownValue = Duration;
        }

        public void PauseTimer()
        {
            _dispTimer.Stop();
        }

        void timer_tick(object sender, EventArgs e)
        {
            CountDownValue -= 1;
            if (CountDownValue <= 0)
            {
                _dispTimer.Stop();
                OnTimerEnd?.Invoke(this, new EventArgs());
            }
        }

        public Visibility Visibility
        {
            get { return _view.Visibility; }
            set { _view.Visibility = value; }
        }

        #region Dependency properties

        /// <summary>
        /// Holds the count-down value. When this value reaches 0, the timer stops
        /// and OnTimerEnd event fired.
        /// It can be used for display purposes as well.
        /// </summary>
        public int CountDownValue
        {
            get { return (int)GetValue(CountDownValueProperty); }
            set { SetValue(CountDownValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CountDownValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountDownValueProperty =
            DependencyProperty.Register("CountDownValue", typeof(int), typeof(BaseNotificationViewModel), new UIPropertyMetadata(0));


        /// <summary>
        /// Determines how long the message will appear before closing
        /// </summary>
        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(int), typeof(BaseNotificationViewModel), new UIPropertyMetadata(10));

        #endregion

    }
}
