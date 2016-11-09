using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Common.Network
{
    public class SocketHostServer
    {
        private const int headLength = 4;

        private static readonly object locker = new object();

        protected TcpListener listener;
        protected readonly int port;
        protected readonly string ipAdderss;
        protected Func<string, string> handleMethod;

        public SocketHostServer(int port, Func<string, string> handleMethod)
        {
            this.port = port;
            this.handleMethod = handleMethod;
        }

        public SocketHostServer(string ipAdderss, int port, Func<string, string> handleMethod)
        {
            this.ipAdderss = ipAdderss;
            this.port = port;
            this.handleMethod = handleMethod;
        }

        public void StartListener()
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
        }

        public void StopListener()
        {
            lock (locker)
            {
                listener.Stop();
            }
        }

        protected void StartBackGroundListener()
        {
            while (true)
            {
                try
                {
                    // A blocking operation was interrupted by a call to WSACancelBlockingCall
                    Socket client = listener.AcceptSocket();

                    ThreadPool.QueueUserWorkItem(HandleMsg, client);
                }
                catch (SocketException)
                {
                    return;
                }
            }
        }

        protected void HandleMsg(object client)
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
                        int hasRecv = socketClient.Receive(readBuffer, headLength + needRecvLength - notRecvLength, notRecvLength, SocketFlags.None);
                        notRecvLength -= hasRecv;

                    } while (notRecvLength != 0);

                    request = Encoding.UTF8.GetString(readBuffer, headLength, needRecvLength);

                    response = handleMethod(request);
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
                        int nSend = socketClient.Send(sendByte, sendByte.Length - needSendLength, needSendLength, SocketFlags.None);
                        needSendLength -= nSend;

                    } while (needSendLength != 0);
                }
            }
            catch (SocketException)
            {
                if (socketClient.Connected)
                {
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                }
            }
        }
    }
}
