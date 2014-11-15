using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using HttpHttpsProxy.Http;
using SSLStripProxy.Http;

namespace SSLStripProxy
{
    class Logger
    {
        public class LoggingFilter
        {
            public string Host { get; set; }
            public HashSet<string> Methods { get; set; }
            public HashSet<string> ContentTypes { get; set; }
        }

        public static LoggingFilter Filter { get; set; }
        private static readonly Dictionary<String, HashSet<string>> Sets;
        private static readonly string[] SetNames = { "tags", "statuses", "connect", "content_type" };

        public enum LogType
        {
            Request,
            Response,
            Uri,
            Client,
            ToFile
        }

        public static MainForm MainForm { get; set; }

        static Logger()
        {
            Sets = new Dictionary<string, HashSet<string>>();

            foreach (var setName in SetNames)
            {
                Sets[setName] = new HashSet<string>();
                LoadSet(setName);
            }
        }

        private static void LoadSet(string setName)
        {
            try
            {
                using (var streamReader = new StreamReader(new FileStream(setName + ".txt", FileMode.Open)))
                {
                    while (streamReader.Peek() >= 0)
                    {
                        Sets[setName].Add(streamReader.ReadLine());
                    }
                }
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }

        }

        private static void SaveSet(string setName)
        {
            using (var streamWriter = new StreamWriter(new FileStream(setName + ".txt", FileMode.Create)))
            {
                foreach (var str in Sets[setName])
                {
                    streamWriter.WriteLine(str);
                }
            }
        }

        public static void SaveSets()
        {
            foreach (var setName in SetNames)
            {
                SaveSet(setName);
            }
        }


        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void Log(object data, LogType logType)
        {
            Pair pair;
            switch (logType)
            {
                case LogType.Client:
                    pair = (Pair) data;
                    MainForm.LogClient((int)pair.First, (string)pair.Second);
                    break;
                case LogType.Uri:
                    pair = (Pair)data;
                    MainForm.LogUri((int)pair.First, (string)pair.Second);
                    break;

                case LogType.Request:
                    pair = (Pair)data;
                    MainForm.LogRequest((int)pair.First, (HttpRequest)pair.Second);
                    break;

                case LogType.Response:
                    pair = (Pair)data;
                    MainForm.LogResponse((int)pair.First, (HttpResponse)pair.Second);
                    break;

                case LogType.ToFile:
                    Action operation = delegate()
                    {
                        var parms = (object[]) data;
                        var client = (string)parms[0];
                        var from = (string)parms[1];
                        var request = (HttpRequest)parms[2];
                        var response = (HttpResponse)parms[3];
                        if (Filter!=null
                            &&(Filter.Host == new Uri(from).Host.ToLower() || Filter.Host == "*")
                            && (Filter.Methods.Contains(request.Method.ToLower()) || Filter.Methods.Contains("*"))
                            && ((response.Headers[HttpHeaderField.ContentType.Name] != null && Filter.ContentTypes.Contains(new ContentType(response.Headers[HttpHeaderField.ContentType.Name]).MediaType.ToLower())) || Filter.ContentTypes.Contains("*")))
                            AddToLogFile(client, from, request, response);
                    };
                    Task task = new Task(operation);
                    task.Start();
                    break;
            }




        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void AddToLogFile(string client, string from, HttpRequest httpRequest, HttpResponse httpResponse)
        {
            using (var streamWriter = new StreamWriter(new FileStream("log.txt", FileMode.Append)))
            {
                streamWriter.Write("timestamp: " + DateTime.Now.Ticks);
                streamWriter.Write("\r\n");
                streamWriter.Write(client);
                streamWriter.Write("\r\n");
                streamWriter.Write(from);
                streamWriter.Write("\r\n");
                streamWriter.Write(" ---------------   Request   --------------- ");
                streamWriter.Write("\r\n");
                streamWriter.Write(Encoding.UTF8.GetString(httpRequest.Compile()));
                streamWriter.Write(" ---------------   Response   --------------- ");
                streamWriter.Write("\r\n");
                streamWriter.Write(Encoding.UTF8.GetString(httpResponse.Compile()));
                streamWriter.Write("\r\n");
                streamWriter.Write("\r\n");
            }
        }

        public static void LogSetValue(string setName, string value)
        {
            Sets[setName].Add(value);
        }
    }
}
