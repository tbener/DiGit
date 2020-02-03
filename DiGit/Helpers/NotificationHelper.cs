using DiGit.View.NotifyIconBalloons;
using DiGit.ViewModel.NotifyIconBalloons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using DiGit.View;
using DiGit.Commands;
using System.Windows;

namespace DiGit.Helpers
{
    

    public static class NotificationHelper
    {
        private static NotifyBalloonView _balloonView;
        private static NotifyBalloonViewModel _balloonViewModel;

        static void InitView()
        {
            try
            {
                if (_balloonView == null)
                {
                    _balloonView = new NotifyBalloonView();
                    _balloonViewModel = new NotifyBalloonViewModel(_balloonView);
                    _balloonView.DataContext = _balloonViewModel;

                    _balloonView.Closed += (s, e) =>
                    {
                        _balloonView = null;
                        _balloonViewModel = null;
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error while initialize notification handler");
            }
        }

        private static void ShowNotification()
        {
            if (ThreadHelper.NeedInvoke())
            {
                ThreadHelper.InvokeOnUiThread(new Action(ShowNotification));
                return;
            }

            InitView();

            _balloonViewModel.ClickCommand = Command;
            _balloonViewModel.Header = Header;
            _balloonViewModel.Message = Message;
            _balloonViewModel.Duration = Duration;

            _balloonViewModel.StartShow();

        }
        

        public static void ShowNotification(string header, string message, ICommand clickCommand, int duration=5)
        {
            Command = clickCommand;
            Header = header;
            Message = message;
            Duration = duration;

            ShowNotification();
        }

        public static void ShowUpdateNotification(string newVersion)
        {
            Header = "New DiGit Version Available";
            Message = $"Version {newVersion} is available.\nClick to open the Update window.";
            Command = new ShowSingleViewCommand(typeof(UpdateView));
            Duration = 5;

            ShowNotification();
        }

        

        public static ICommand Command { get; set; }
        public static string Header { get; set; }
        public static string Message { get; set; }
        public static int Duration { get; set; }
    }
}
