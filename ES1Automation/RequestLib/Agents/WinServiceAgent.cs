using System;
using System.Text;
using System.IO;
using Common.FileCommon;
using Common.ScriptCommon;
using Common.Network;

namespace RequestLib.Agents
{
    public class WinServiceAgent : Agent
    {
        public static bool IsAgentAvaliable(string server, int port)
        {
            try
            {
                var userClient = new SocketUserClient(server, port);

                return userClient.CanConnectServer();
            }
            catch
            {
                return false;
            }
        }

        public static bool IsAgentAvaliable(string server, int port, int timeout)
        {
            try
            {
                var userClient = new SocketUserClient(server, port);

                return userClient.CanConnectServer(timeout);
            }
            catch
            {
                return false;
            }
        }
    }
}
