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

namespace DiGit.Helpers
{
    public static class NotificationHelper
    {
        private static NotifyBalloonView _balloonView;
        private static NotifyBalloonViewModel _balloonViewModel;

        public static void Initialize()
        {
            try
            {
                _balloonView = new NotifyBalloonView();
                _balloonViewModel = new NotifyBalloonViewModel(_balloonView);
                _balloonView.DataContext = _balloonViewModel;

                
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error while initialize notification handler");
            }
        }

        //private void ShowUpdateAvailableBalloon()
        //{
        //    NotifyBalloonView balloonView = new NotifyBalloonView();
        //    NotifyBalloonViewModel balloonViewModel = new NotifyBalloonViewModel()
        //    {
        //        Header = "DiGit update available",
        //        Message = $"Version {UpdateManager.LastVersionInfo.version} found. Click for more details.",
        //        NotesText = $"Current version: {AppInfo.AppVersionString}"
        //    };
        //    balloonView.DataContext = balloonViewModel;
        //    balloonViewModel.ClickCommand = new ShowSingleViewCommand(typeof(UpdateView));

        //    _taskbarIcon.ShowCustomBalloon(balloonView, PopupAnimation.Slide, 5000);
        //}

        private static void ShowNotification()
        {
            if (!_balloonView.Dispatcher.CheckAccess())
            {
                _balloonView.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                        new Action(ShowNotification));
                return;
            }

            _balloonViewModel.ClickCommand = Command;
            _balloonViewModel.Header = Header;
            _balloonViewModel.Message = Message;
            _balloonViewModel.Duration = Duration;

            _balloonViewModel.StartShow();
        }

        public static void ShowNotification(string header, string message, ICommand clickCommand)
        {
            Command = clickCommand;
            Header = header;
            Message = message;

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
