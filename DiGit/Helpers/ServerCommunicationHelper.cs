using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using DiGit.Model;

namespace DiGit.Helpers
{
    internal static class ServerCommunicationHelper
    {
        static HttpWebRequest httpWebRequest;

        static ServerCommunicationHelper()
        {
            Init();
        }

        private static void Init()
        {
            httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:8000/add_event/");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 90000000;
        }

        internal static void AddEvent()
        {
            const string template = "\"{0}\":\"{1}\"";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = string.Format(template, "user", Environment.MachineName) + "," +
                              string.Format(template, "activity", "startup");
                json = "{" + json + "}";

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Debug.Print(result);
            }
        }

        internal static void Update(UserStat userStat)
        {
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = new JavaScriptSerializer().Serialize(userStat);

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Debug.Print(result);
            }
        }
    }
}
