using System;
using System.Windows;
using System.Windows.Input;
using DiGit.View;
using DiGit.ViewModel;

namespace DiGit.Commands
{
    public class ShowSettingsCommand : ICommand
    {
        private static SettingsView _view;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_view == null)
            {
                _view = new SettingsView();
                _view.Closed += (s, e) =>
                {
                    _view = null;
                };
                var viewModel = new SettingsViewModel();
                _view.DataContext = viewModel;
                _view.Show();
            }
            else
            {
                if (_view.WindowState == WindowState.Minimized) _view.WindowState = WindowState.Normal;
                _view.Activate();
            }

        }
    }
}
