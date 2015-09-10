using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DiGit.Helpers;
using DiGit.Model;

namespace DiGit.Commands
{
    class OpenFolderCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (PathClass != null)
                return PathClass.Enabled;
            return true;
        }

        public event EventHandler CanExecuteChanged;
        public readonly Model.PathClass PathClass;

        public OpenFolderCommand(Model.PathClass pathClass)
        {
            PathClass = pathClass;
            PathClass.OnChange += (sender, args) => { if (CanExecuteChanged != null) CanExecuteChanged(this, null); };
        }

        public OpenFolderCommand()
        {
            
        }

        public void Execute(object parameter)
        {
            try
            {
                string fullPath;
                if (PathClass != null && PathClass.Exists)
                    fullPath = PathClass.FullPath;
                else
                {
                    PathClass pathClass = parameter as PathClass;
                    if (pathClass != null)
                        fullPath = pathClass.FullPath;
                    else
                    {
                        fullPath = parameter as string;
                        if (fullPath == null) return;
                    }
                }
                Process.Start(fullPath);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }
    }
}
