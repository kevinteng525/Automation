using ATFExchangeCentre.Requests;
using Common.Network;
using Common.Windows;

namespace ATFExchangeCentre.Agents
{
    /// <summary>
    /// A socket listener host on a windows service
    /// </summary>
    public class WinServiceAgent : Agent
    {
        private readonly SocketHostServer server;

        public WinServiceAgent(int port)
        {
            server = new SocketHostServer(port, Request.ProcessRequest);
        }

        public override void StartAgent()
        {
            server.StartListener();
        }

        public override void StopAgent()
        {
            server.StopListener();
        }

        public static string SentRequest(string server, int port, Request request)
        {
            Log.logger.Debug("request: " + request.ToString());

            SocketUserClient client = new SocketUserClient(server, port);

            return client.SentMsgToServer(request.ToString());
        }

        public static void InstallAgent
            (
                string server,
                string username,
                string password,
                string serviceBinaryName,
                string serviceName,
                string serviceDisplayName,
                string sourceInstallPath,
                string targetSharePath,
                string targetIntallPath
            )
        {
            WindowsServices.RemoteInstallWinService(server, username, password, serviceBinaryName, serviceName, sourceInstallPath, targetSharePath, targetIntallPath, serviceDisplayName);
        }

        public static void UninstallAgent
            (
                string server,
                string username,
                string password,
                string serviceBinaryName,
                string serviceName,
                string sourceInstallPath,
                string targetSharePath,
                string targetIntallPath
            )
        {
            WindowsServices.RemoteUninstallWinService(server, username, password, serviceBinaryName, serviceName, sourceInstallPath, targetSharePath, targetIntallPath);
        }

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
