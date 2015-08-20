using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DiGit.Helpers;
using LibGit2Sharp;

namespace DiGit.Commands
{
    class CloseBubbleCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true; // (parameter != null);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var view = parameter as Window;
            if (view != null) BubblesManager.ShowView(view, false);
        }
    }

    class ShowAllCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            BubblesManager.ShowAll(true);
        }
    }

    class HideAllCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            BubblesManager.ShowAll(false);
        }
    }

    class ToggleShowHideCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            BubblesManager.ToggleShowHide();
        }
    }
}
