using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.FileCommon
{
    public static class TxtHelper
    {
        /// <summary>
        /// Get txt content
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetTxt(string path)
        {
            Encoding code = Encoding.GetEncoding("utf-8");

            StreamReader sr = new StreamReader(path, code);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }

        /// <summary>
        /// Read txt file by lines
        /// </summary>
        /// <param name="FileFullName"></param>
        /// <returns></returns>
        public static IList<string> GetTxtByLines(string FileFullName)
        {
            IList<string> TxtLines = new List<string>();
            Encoding code = Encoding.GetEncoding("utf-8");
            StreamReader sr = new StreamReader(FileFullName, code);
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
        public static void WriteTxtByLines(IList<string> Lines, string FileFullName)
        {
            //clear it first
            ClearTxtContent(FileFullName);

            foreach (string line in Lines)
            {
                WriteNewLine(FileFullName, line);
            }
        }

        /// <summary>
        /// clear txt content
        /// </summary>
        /// <param name="FileFullName"></param>
        public static void ClearTxtContent(string FileFullName)
        {
            FileStream stream = new FileStream(FileFullName, FileMode.Create);
            stream.Close();
        }

        public static void WriteNewLine(string FileFullName, string line)
        {
            FileStream fs = new FileStream(FileFullName, FileMode.Append, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine(line);
            sw.Flush();
            sw.Dispose();
            fs.Close();
        }
    }
}
