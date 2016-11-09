using System.Net.NetworkInformation;
using System.Management;

namespace Common.SystemCommon
{
    public static class ServerStatus
    {
        public static bool? IsServerAvailiable(string server)
        {
            PingReply pingReply;

            using (var ping = new Ping())
            {
                try
                {
                    pingReply = ping.Send(server);
                }
                catch
                {
                    return false;
                }
            }

            return pingReply.Status == IPStatus.Success;
        }

        public static string GetOSFriendlyName()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }
            return result;
        }
    }
}
