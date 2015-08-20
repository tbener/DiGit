using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using DiGit.Helpers;
using DiGit.View;
using DiGit.ViewModel;

namespace DiGit.Commands
{
    /// <summary>
    /// This Command is used to display a single instance of a given View.
    /// All instances of this command manage the same static collection of Views.
    /// Initialize the Command with a View type, and the Execute method will always show
    /// a single instance of this view.
    /// </summary>
    public class ShowSingleViewCommand : ICommand
    {
        private static readonly Dictionary<Type, Window> Views = new Dictionary<Type, Window>();

        private readonly Type _viewType;

        public ShowSingleViewCommand(Type viewType)
        {
            _viewType = viewType;

            if (!Views.ContainsKey(_viewType))
            {
                Views.Add(_viewType, null);
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            try
            {
                if (Views[_viewType] == null)
                {
                    Views[_viewType] = Activator.CreateInstance(_viewType) as Window;
                    BaseViewModel vm = Views[_viewType].DataContext as BaseViewModel;
                    if (vm != null)
                        vm.RequestClose += (sender, args) => Views[_viewType].Close();
                    Views[_viewType].Closed += (s, e) => Views[_viewType] = null;
                    if (_viewType.BaseType.IsEquivalentTo(typeof(FirstFloor.ModernUI.Windows.Controls.ModernDialog)))
                        Views[_viewType].ShowDialog();
                    else
                        Views[_viewType].Show();
                }
                else
                {
                    if (Views[_viewType].WindowState == WindowState.Minimized) Views[_viewType].WindowState = WindowState.Normal;
                    Views[_viewType].Activate();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error displaying window of type {0}.", _viewType.Name);
            }

        }
    }
}
