using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiGit.Model
{
    public class MenuCommand : ICommand
    {
        public string Header { get; set; }
        public string CommandFile { get; set; }
        public string CommandArgs { get; set; }
        public string HelpText { get; set; }

        public ICommand Command { get; set; }

        public MenuCommand(string header, string commandFile, string commandArgs)
        {
            Header = header;
            CommandFile = commandFile;
            CommandArgs = commandArgs;
        }

        public override string ToString()
        {
            return Header;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Process p = new Process();
            p.StartInfo.FileName = CommandFile;
            p.StartInfo.Arguments = CommandArgs;
            p.Start();
        }
    }
}
