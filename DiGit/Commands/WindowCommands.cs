using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using DiGit.Helpers;
using DiGit.Model;
using DiGit.View;

//using RadioK.UI.Models;

namespace DiGit.Commands
{
    public class ExitCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (!ConfigurationHelper.Save())
                if (!Msg.ShowQ("Do you want to exit DiGit without saving?"))
                    return;

            HotkeyHelper.Dispose();
            BubblesManager.CloseAll();
            
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.Close();

            Application.Current.Shutdown();
        }
    }

    public class MinimizeCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
                return true;
            return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;

        }
    }

    public class RestoreCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
                return true;
            return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.ShowInTaskbar = true;
            Application.Current.MainWindow.Focus();
        }
    }

}
