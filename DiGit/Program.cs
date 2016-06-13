using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.Model;
using DiGit.View;

namespace DiGit
{
    /// <summary>
    /// This class was written with the help of these links:
    /// http://blog.clauskonrad.net/2010/08/wpf-where-is-main-method-or-how-to.html
    /// 1) Select your project’s current startup file (app.xaml)
    /// 2) Set BuildAction = Page
    /// 3) Add a file as below (Program.cs)
    /// Note: Do not call this file Application.cs and the class Application. It will conflict with the inner workings of WPF
    /// http://blog.clauskonrad.net/2011/04/wpf-how-to-make-your-application-single.html (forgot to release)
    /// http://stackoverflow.com/questions/19147/what-is-the-correct-way-to-create-a-single-instance-application
    /// </summary>
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            UserStat us = new UserStat();
            us.Activities.Add(new UserStat.ActivityInfoClass(){ActivityType = UserStat.ActivityType.Startup, Date = DateTime.Now});
            us.Update();
            return;

            string step = "Init Mutex";
            Mutex mutex = null;
            try
            {
                mutex = new Mutex(true, "DiGit_tbener");

                if (!mutex.WaitOne(TimeSpan.Zero, true))
                {
                    Msg.Show("DiGit is already running");
                    return;
                }

                step = "Init App class";
                App app = new App();
                app.InitializeComponent();
                step = "";
                app.Run();
            }
            catch (Exception ex)
            {
                if (step != "") step = string.Format(" ({0})", step);
                ErrorHandler.Handle(ex, "An unexpected error ocurred{0}.", step);
                try
                {
                    new ExitCommand().Execute(null);
                }
                catch{}
            }
            finally
            {
                try
                {
                    mutex.ReleaseMutex();
                }
                catch
                {
                }

            }
        }

    }

}
