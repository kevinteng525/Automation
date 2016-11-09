using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using S1ValidationWinService.Maintains;

namespace S1ValidationWinService.NetTransmission
{
    public class SocketServer : BlockWaitServer
    {
        private const int headLength = 4;

        #region Constructors

        public SocketServer(string ipAdderss, int port)
            : base(ipAdderss, port)
        {
        }

        public SocketServer(int port)
            : base(port)
        {
        }

        #endregion

        protected override void StartBackGroundListener()
        {
            while (true)
            {
                try
                {
                    // A blocking operation was interrupted by a call to WSACancelBlockingCall
                    Socket client = listener.AcceptSocket();

                    ThreadPool.QueueUserWorkItem(HandleClientComm, client);
                }
                catch (SocketException)
                {
                    return;
                }
            }
        }

        private void HandleClientComm(object client)
        {
            var socketClient = (Socket)client;
            socketClient.NoDelay = true;

            var lingerOption = new LingerOption(true, 3);
            socketClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, lingerOption);

            string request = string.Empty;

            string response = string.Empty;

            try
            {
                byte[] headBuffer = new byte[4];
                // head of reveive length
                socketClient.Receive(headBuffer, 4, SocketFlags.None);
                int needRecvLength = BitConverter.ToInt32(headBuffer, 0);

                if (needRecvLength != 0)
                {
                    // not receive lenght
                    int notRecvLength = needRecvLength;

                    byte[] readBuffer = new byte[needRecvLength + headLength];

                    // receive
                    do
                    {
                        int hasRecv = socketClient.Receive(readBuffer, headLength + needRecvLength - notRecvLength,
                                                           notRecvLength, SocketFlags.None);
                        notRecvLength -= hasRecv;

                    } while (notRecvLength != 0);

                    request = Encoding.UTF8.GetString(readBuffer, headLength, needRecvLength);

                    response = InvokeRequest(request);
                }

                if (socketClient.Connected)
                {
                    // sent
                    byte[] contentByte = Encoding.UTF8.GetBytes(response);
                    byte[] headBytes = BitConverter.GetBytes(contentByte.Length);

                    byte[] sendByte = new byte[headBytes.Length + contentByte.Length];

                    headBytes.CopyTo(sendByte, 0);
                    contentByte.CopyTo(sendByte, headLength);

                    int needSendLength = sendByte.Length;

                    do
                    {
                        int nSend = socketClient.Send(sendByte, sendByte.Length - needSendLength, needSendLength,
                                                      SocketFlags.None);
                        needSendLength -= nSend;

                    } while (needSendLength != 0);
                }
            }
            catch (SocketException ex)
            {
                EventLogger.LogError(ex.Message);

                if (socketClient.Connected)
                {
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.LogError(ex.Message);
            }
        }
    }
}
