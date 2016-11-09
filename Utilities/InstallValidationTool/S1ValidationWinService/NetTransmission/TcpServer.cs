using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using S1ValidationWinService.Maintains;

namespace S1ValidationWinService.NetTransmission
{
    public class TcpServer : BlockWaitServer
    {
        #region Constructors

        public TcpServer(string ipAdderss, int port)
            : base(ipAdderss, port)
        {
        }

        public TcpServer(int port)
            : base(port)
        {
        }

        #endregion Constructors

        protected override void StartBackGroundListener()
        {
            while (true)
            {
                try
                {
                    // A blocking operation was interrupted by a call to WSACancelBlockingCall
                    TcpClient client = listener.AcceptTcpClient();

                    ThreadPool.QueueUserWorkItem(HandleClientComm, client);
                }
                catch (Exception ex)
                {
                    if (ex is SocketException)
                    {
                        return;
                    }

                    throw;
                }
            }
        }

        private void HandleClientComm(object client)
        {
            var tcpClient = (TcpClient)client;
            tcpClient.NoDelay = true;

            NetworkStream clientStream = tcpClient.GetStream();

            var request = new StringBuilder();

            try
            {
                var message = new byte[1024 * 100];

                if (clientStream.CanRead)
                {
                    do
                    {
                        int messageLength = clientStream.Read(message, 0, 1024 * 100);
                        request.Append(Encoding.UTF8.GetString(message, 0, messageLength));

                    } while (clientStream.DataAvailable);
                }

                string response = InvokeRequest(request.ToString());

                if (tcpClient.Connected)
                {
                    byte[] responesBytes = Encoding.UTF8.GetBytes(response);
                    clientStream.Write(responesBytes, 0, responesBytes.Length);
                }
            }
            #region Error Handle
            catch (SocketException ex)
            {
                EventLogger.LogError(ex.Message);

                if (client != null)
                {
                    tcpClient.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.LogError(ex.Message);
            }
            #endregion
        }
    }
}
