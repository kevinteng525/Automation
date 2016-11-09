using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.FileCommon
{
    public static class TXTHelper
    {
        /// <summary>
        /// Get txt content
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetTXT(string path)
        {
            Encoding code = Encoding.GetEncoding("utf-8");

            var sr = new StreamReader(path, code);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }

        /// <summary>
        /// Read txt file by lines
        /// </summary>
        /// <param name="FileFullName"></param>
        /// <returns></returns>
        public static IList<string> GetTXTByLines(string FileFullName)
        {
            IList<string> TxtLines = new List<string>();
            Encoding code = Encoding.GetEncoding("utf-8");
            var sr = new StreamReader(FileFullName, code);
            while (sr.Peek() >= 0)
            {
                TxtLines.Add(sr.ReadLine());
            }
            return TxtLines;
        }

        /// <summary>
        /// Write txt file by lines
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="FileFullName"></param>
        /// <param name="encoding"> </param>
        public static void WriteTXTByLines(IList<string> Lines, string FileFullName, Encoding encoding)
        {
            //clear it first
            ClearTXTContent(FileFullName);

            foreach (string line in Lines)
            {
                WriteNewLine(FileFullName, line, encoding);
            }
        }

        /// <summary>
        /// clear txt content
        /// </summary>
        /// <param name="FileFullName"></param>
        public static void ClearTXTContent(string FileFullName)
        {
            var stream = new FileStream(FileFullName, FileMode.Create);
            stream.Close();
        }

        public static void WriteNewLine(string FileFullName, string line, Encoding encoding)
        {
            var fs = new FileStream(FileFullName, FileMode.Append, FileAccess.Write, FileShare.Read);
            var sw = new StreamWriter(fs, encoding);
            sw.WriteLine(line);
            sw.Flush();
            sw.Dispose();
            fs.Close();
        }
    }
}
