using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DiGit.Configuration;
using DiGit.Helpers;
using LibGit2Sharp;

namespace DiGit.Commands
{
    public class UserCommand : ICommand
    {
        private readonly DiGitConfigCommand _command;
        private readonly Repository _repo;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
        

        public UserCommand(DiGitConfigCommand cmd, LibGit2Sharp.Repository repository)
        {
            this._command = cmd;
            this._repo = repository;
        }

        public void Execute(object parameter)
        {
            string fileName = "", args = "";
            try
            {
                fileName = _command.fileName.Replace("{rep_path}", _repo.Info.WorkingDirectory);
                if (!String.IsNullOrEmpty(_command.arguments))
                    args = _command.arguments.Replace("{rep_path}", "\"" + _repo.Info.WorkingDirectory + "\"");
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex,
                    "An error occurred while trying to build the command '{0}'. FileName result: {1}. Arguments result: {2}",
                    _command.header, fileName, args);
            }
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = fileName;
                if (!String.IsNullOrEmpty(_command.arguments))
                {
                    p.StartInfo.Arguments = args;
                }
                if (!String.IsNullOrEmpty(_command.windowStyle))
                    p.StartInfo.WindowStyle = (ProcessWindowStyle)Enum.Parse(typeof(ProcessWindowStyle), _command.windowStyle);
                p.Start();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "An error occurred while trying to run command '{0}'. FileName: {1}. Arguments: {2}", _command.header, fileName, args);
            }
        }

        public override string ToString()
        {
            return _command.header;
        }
    }
}
