using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using Common.FileCommon;

namespace S1ValidationWinService.Maintains
{
    public class EventLogger
    {
        private static readonly object writerLocker = new object();

        public const string ERROR = "ERROR";

        public const string WARNING = "WARNING";

        public const string EVENT = "EVENT";

        private const string fileNameFormat = "{0} {1}.log";

        private static readonly string errorPath;

        private static readonly string warningPath;

        private static readonly string eventPath;

        public static readonly string rootLogPath = ConfigurationManager.AppSettings["LogPath"];

        static EventLogger()
        {
            errorPath = Path.Combine(rootLogPath, ERROR);
            warningPath = Path.Combine(rootLogPath, WARNING);
            eventPath = Path.Combine(rootLogPath, EVENT);

            // Create Folder
            FileHelper.CreateFolder(rootLogPath);
            FileHelper.CreateFolder(errorPath);
            FileHelper.CreateFolder(warningPath);
            FileHelper.CreateFolder(eventPath);
        }

        public static string[] GetLogByDate(string date)
        {
            IList<string> files = new List<string>(3)
                                     {
                                         Path.Combine(eventPath ,GetFileName(date, EVENT)),
                                         Path.Combine(errorPath ,GetFileName(date, ERROR)),
                                         Path.Combine(warningPath ,GetFileName(date, WARNING))
                                     };

            for (int i = files.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(files[i]))
                {
                    files.RemoveAt(i);
                }
            }

            string[] fileList = new string[files.Count];
            files.CopyTo(fileList, 0);

            return fileList;
        }

        public static void LogWarning(string msg)
        {
            string fileName = GetFileName(DateTime.Now.ToString("yyyy-MM-dd"), WARNING);

            Log(warningPath, fileName, msg);
        }

        public static void LogError(string msg)
        {
            string fileName = GetFileName(DateTime.Now.ToString("yyyy-MM-dd"), ERROR);

            Log(errorPath, fileName, msg);
        }

        public static void LogEvent(string msg)
        {
            string fileName = GetFileName(DateTime.Now.ToString("yyyy-MM-dd"), EVENT);

            Log(eventPath, fileName, msg);
        }

        private static string GetFileName(string date, string fileType)
        {
            return string.Format(fileNameFormat, date, fileType);
        }

        private static void Log(string logPath, string fileName, string msg)
        {
            FileHelper.CreateFolder(logPath);

            string path = Path.Combine(logPath, fileName);

            string content = string.Format("{0}   {1}", DateTime.Now, msg);

            lock (writerLocker)
            {
                TXTHelper.WriteNewLine(path, content, Encoding.UTF8);
            }
        }
    }
}
