using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using S1ValidationWinService.Maintains;

namespace S1ValidationWinService.NetTransmission
{
    public abstract class BlockWaitServer : Server
    {
        private static readonly object locker = new object();

        protected TcpListener listener;
        protected readonly int port;
        protected readonly string ipAdderss;

        #region Constructors

        protected BlockWaitServer(string ipAdderss, int port)
        {
            this.ipAdderss = ipAdderss;
            this.port = port;
        }

        protected BlockWaitServer(int port)
        {
            this.port = port;
        }

        #endregion Constructors

        protected abstract void StartBackGroundListener();

        public override void StartListener()
        {
            try
            {
                lock (locker)
                {
                    if (string.IsNullOrEmpty(ipAdderss))
                    {
                        listener = new TcpListener(IPAddress.Any, port);
                    }
                    else
                    {
                        IPAddress ip = IPAddress.Parse(ipAdderss);
                        listener = new TcpListener(ip, port);
                    }

                    listener.Start();
                }

                Thread listernThread = new Thread(StartBackGroundListener) { IsBackground = true };
                listernThread.Start();

                EventLogger.LogEvent(string.Format("TCP Start Listing! PORT: {0} IP : {1}", port, ipAdderss));
            }
            catch (Exception ex)
            {
                EventLogger.LogError(ex.Message);
            }
        }

        public override void StopListener()
        {
            lock (locker)
            {
                listener.Stop();
            }
        }
    }
}
