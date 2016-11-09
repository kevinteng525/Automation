using System;

namespace S1ValidationWinService.NetTransmission
{
    public static class ServerFactory
    {
        public static Server CreateServer(string serverName, string ipAdderss, int port)
        {
            switch (serverName)
            {
                case "SocketServer":
                    return new SocketServer(ipAdderss, port);
                case "TcpServer":
                    return new TcpServer(ipAdderss, port);
                default:
                    throw new Exception("Server Type Error.");
            }
        }
    }
}
