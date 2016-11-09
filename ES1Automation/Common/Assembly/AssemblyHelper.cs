using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Common.AssemblyCommon
{
    public class AssemblyHelper
    {
        /// <summary>
        /// Get the running assembly's path.
        /// </summary>
        /// <returns></returns>
        static public String GetAssemblePath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

    }
}
