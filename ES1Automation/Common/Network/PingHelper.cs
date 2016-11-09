using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;

namespace Common.Network
{
    public static class PingHelper
    {
        /// <summary>
        /// Check whether the ip is reachable
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsPingable(string address)
        {
            bool reachable = false;
            using (Ping png = new Ping())
            {
                try
                {
                    if (png.Send(address).Status == IPStatus.Success)
                    {
                        reachable = true;
                    }
                    else
                    {
                        reachable = false;
                    }
                }
                catch
                {
                    reachable = false;
                } 
            }
            return reachable;
        }

        /// <summary>
        /// Wait untill the machine is pingable
        /// </summary>
        /// <param name="address">The machine name or Ip address</param>
        /// <param name="timeOutSeconds">The wait time, default is 5 minutes</param>
        /// <returns></returns>
        public static bool WaitUntillPingable(string address, int timeOutSeconds = 300)
        {
            int i = 0;
            while (i < timeOutSeconds)
            {
                if (IsPingable(address))
                {
                    return true;
                }
                else 
                {
                    Thread.Sleep(1000 * 10);
                    i += 10;
                }
            }
            return false;
        }
    }
}
