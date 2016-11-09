using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SendDominoNotes
{
    public static class UtilManager
    {
        #region ini operation
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);


        public static void WriteIni(string section, string key, string value, string inipath)
        {
            WritePrivateProfileString(section, key, value, inipath);
        }

        public static string ReadIni(string section, string key, string inipath)
        {            
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, inipath);
            return temp.ToString();
        }
        #endregion
    }
}
