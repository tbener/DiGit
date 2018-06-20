using System;
using System.Diagnostics;

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



            if (displayMessage)
                if (evtLogException == null)
                    Msg.ShowE(error.Message);
                else
                    Msg.ShowE(error);
//#if DEBUG
//            else
//                if (error.GetType() != typeof(System.IO.IOException))
//                    Msg.ShowE(error, "THIS MESSAGE IS DISPLAYED ONLY IN DEBUG MODE\n");
//#endif

            // TODO: document this error!!
            //if (evtLogException != null)
            //    Msg.ShowE(evtLogException, "An error occured while trying to write to Event Log.");

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
