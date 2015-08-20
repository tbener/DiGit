using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiGit.Commands
{
    class OpenFolderCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            try
            {
                string dir = parameter as string;
                // TODO: Check dir
                Process.Start(dir);
            }
            catch 
            {
                // TODO: Handle error
                throw;
            }
        }
    }
}
