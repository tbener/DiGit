using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiGit.Helpers
{
    internal static class ErrorHandler
    {

        public static Exception Handle(Exception error, bool displayMessage = true)
        {
            Exception evtLogException = null;
            try
            {
                EventLog.WriteEntry("DiGit", error.ToString(), EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                evtLogException = ex;
            }

#if DEBUG
            Msg.ShowE(error);
#else
            if (displayMessage)
                if (evtLogException == null)
                    Msg.ShowE(error.Message);
                else
                    Msg.ShowE(error);
#endif

            if (evtLogException != null)
                Msg.ShowE(evtLogException, "An error occured while trying to write to Event Log.");

            return error;
        }

        internal static void Handle(Exception error, string errorPrefix, params object[] args)
        {
            Handle(new Exception(string.Format(errorPrefix + "\n\r", args), error));
        }

        internal static void Handle(Exception error, string errorPrefix)
        {
            Handle(new Exception(errorPrefix + "\n\r", error));
        }
    }
}
