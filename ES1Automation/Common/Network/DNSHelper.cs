using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.ScriptCommon;
using Common.FileCommon;

namespace Common.Network
{
    public static class DNSHelper
    {
        public static bool FlushDNS()
        {
            try
            {
                string tempBatFileName = System.IO.Path.GetTempFileName() + ".bat";
                TXTHelper.ClearTXTContent(tempBatFileName);
                string cmd = string.Format(@"ipconfig /flushdns");
                TXTHelper.WriteNewLine(tempBatFileName, cmd, Encoding.Default);
                cmd = string.Format(@"ipconfig /registerdns");
                TXTHelper.WriteNewLine(tempBatFileName, cmd, Encoding.Default);
                CMDScript.RumCmd(tempBatFileName, string.Empty);
                FileHelper.DeleteFile(tempBatFileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
