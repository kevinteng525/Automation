using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.FileCommon;
using Common.ScriptCommon;

namespace Common.Network
{
    public static class NetUseHelper
    {
        /// <summary>
        /// Net use the C$ on the remote machine.
        /// </summary>
        /// <param name="machineIPOrName">machine name or ip</param>
        /// <param name="administrator">administrator of the remote machine or the domain administrator</param>
        /// <param name="password">administrator password</param>
        /// <returns>true if success, else false</returns>
        public static bool NetUserMachine(string machineIPOrName, string administrator, string password)
        {
            if (!System.IO.Directory.Exists(string.Format(@"\\{0}\C$", machineIPOrName)))
            {
                try
                {
                    string tempBatFileName = System.IO.Path.GetTempFileName() + ".bat";
                    TXTHelper.ClearTXTContent(tempBatFileName);
                    string cmd = string.Format(@"net use \\{0} /delete /y", machineIPOrName);
                    TXTHelper.WriteNewLine(tempBatFileName, cmd, Encoding.Default);
                    cmd = string.Format(@"net use \\{0} {1} /user:{2}", machineIPOrName, password, administrator);
                    TXTHelper.WriteNewLine(tempBatFileName, cmd, Encoding.Default);
                    CMDScript.RumCmd(tempBatFileName, string.Empty);
                    FileHelper.DeleteFile(tempBatFileName);
                    if (!System.IO.Directory.Exists(string.Format(@"\\{0}\C$", machineIPOrName)))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public static string NetUserRemoteFolerToLocalPath(string remotePath, string localPath, string administrator, string password)
        {
            if (!System.IO.Directory.Exists(localPath))
            {
                try
                {
                    string tempBatFileName = System.IO.Path.GetTempFileName() + ".bat";
                    TXTHelper.ClearTXTContent(tempBatFileName);
                    string cmd = string.Format(@"net use {0} /delete /y", localPath);
                    TXTHelper.WriteNewLine(tempBatFileName, cmd, Encoding.Default);
                    cmd = string.Format(@"net use {0} {1} {2} /user:{3}", localPath, remotePath, password, administrator);
                    TXTHelper.WriteNewLine(tempBatFileName, cmd, Encoding.Default);
                    string result = CMDScript.RumCmd(tempBatFileName, string.Empty);
                    FileHelper.DeleteFile(tempBatFileName);
                    if (!System.IO.Directory.Exists(localPath))
                    {
                        return result;
                    }
                    else
                    {
                        return "Success";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message + ex.StackTrace;
                }
            }
            else
            {
                return "Success";
            }
        }
    }
}
