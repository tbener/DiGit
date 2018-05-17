using DiGit.Commands;
using DiGit.View;
using DiGit.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DiGit.ViewModel.NotifyIconBalloons
{
    internal class NotifyBalloonViewModel : BaseNotificationViewModel
    {
        public ICommand ClickCommand { get; set; }

        public NotifyBalloonViewModel(Window view) : base(view)
        {

        }

        public string Header { get; set; }
        public string Message { get; set; }
        public string NotesText { get; set; }

        public override void CloseView(Window view)
        {
            view.Close();
        }


        public override Point OnSetPosition(Window view)
        {
            return new Point(SystemParameters.PrimaryScreenWidth - 50 - view.Width, 50);
        }

        public void StartTimerIfEnabled()
        {
            if (!DisableTimer) StartTimer();
        }

        private void OnPauseCommand()
        {
            PauseTimer();
        }

        private void OnDisableTimerCommand()
        {
            StopTimer();
            DisableTimer = true;
        }

        private bool CanPauseCommand()
        {
            return !DisableTimer;
        }

        public override void UpdateProperties()
        {
            OnPropertyChanged("Header");
            OnPropertyChanged("Message");
            OnPropertyChanged("ClickCommand");
            OnPropertyChanged("MouseCursor");
        }

        public bool DisableTimer { get; set; }

        public Cursor MouseCursor
        {
            get { return ClickCommand == null ? Cursors.Arrow : Cursors.Hand; }
        }
    }
}
