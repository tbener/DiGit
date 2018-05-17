using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiGit.Helpers
{

    public static class ThreadHelper
    {
        public static void InvokeOnUiThread(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(action);
            }
        }

        public static void BeginInvokeOnUiThread(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(action);
            }
        }

        public static bool NeedInvoke()
        {
            return !Application.Current.Dispatcher.CheckAccess();
        }

    }
}

