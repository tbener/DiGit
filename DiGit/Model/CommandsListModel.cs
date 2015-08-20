using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiGit.Helpers;
using LibGit2Sharp;
using System.Windows.Input;
using DiGit.Commands;

namespace DiGit.Model
{
    internal class CommandsListModel   
    {
        private readonly ObservableCollection<MenuCommand> _listCommands;

        private const string GIT_CMD_FORMAT = "/command:{0} /path:{1}{2}";
        private const string GIT_CMD_EXE = @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe";

        public CommandsListModel(Repository repo)
        {
            Repo = repo;
            _listCommands = new ObservableCollection<MenuCommand>();

            AddGitCommand("Switch", "switch");
            AddGitCommand("Push", "push");
            AddGitCommand("Pull", "pull");
            AddGitCommand("Commit", "commit");
            AddGitCommand("Merge", "merge");
            AddGitCommand("Show log", "log");
            AddGitCommand("Sync", "sync");

            //_listCommands.Add(new MenuCommand(GIT_CMD_EXE, ) { GitCommand = "push", Header = "Push...", HelpText = "Push my code" });
            //_listCommands.Add(new MenuCommand { GitCommand = "pull", Header = "Pull...", HelpText = "Pull..." });
            //_listCommands.Add(new MenuCommand { GitCommand = "commit", Header = "Commit", HelpText = "Open Commit window" });


        }

        private void teset()
        {
            Msg.Show("Helloooo");
        }

        public ObservableCollection<MenuCommand> GetCommandsList
        {
            get { return _listCommands; }
        }

        //public string BuildGitCommand(MenuCommand mc)
        //{
        //    string cmd = string.Format(GIT_CMD_FORMAT, mc.GitCommand, Repo.Info.WorkingDirectory, mc.Options);
        //    return cmd;
        //}

        private void AddGitCommand(string header, string cmd, string options = "")
        {
            _listCommands.Add(new MenuCommand(header, GIT_CMD_EXE, string.Format(GIT_CMD_FORMAT, cmd, Repo.Info.WorkingDirectory, options)));
        }

        //public void ExecuteCommand(MenuCommand mc)
        //{
        //    Process p = new Process();
        //    p.StartInfo.FileName = @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe";
        //    p.StartInfo.Arguments = BuildGitCommand(mc);
        //    p.Start();
        //}

        public Repository Repo { get; set; }
    }

    
}
