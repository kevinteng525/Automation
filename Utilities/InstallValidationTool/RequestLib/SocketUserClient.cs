using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RequestLib
{
    public class SocketUserClient
    {
        private Socket client;

        private readonly string hostName;

        private readonly int serverPort;

        private readonly IPEndPoint ipep;

        public SocketUserClient(string hostName, int serverPort)
        {
            this.hostName = hostName;
            this.serverPort = serverPort;

            ipep = new IPEndPoint(IPAddress.Parse(hostName), serverPort);
        }

        public bool CanConnectServer()
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                client.Connect(hostName, serverPort);

                return client.Connected;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (client.Connected)
                {
                    client.Close();
                }
            }
        }

        public bool CanConnectServer(int timeout)
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IAsyncResult result = client.BeginConnect(hostName, serverPort, null, null);

                return result.AsyncWaitHandle.WaitOne(500, true);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (client.Connected)
                {
                    client.Close();
                }
            }
        }

        public string SentMsgToServer(string msg)
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(hostName, serverPort);

                // sent content to server
                byte[] contentByte = Encoding.UTF8.GetBytes(msg);
                byte[] headBytes = BitConverter.GetBytes(contentByte.Length);

                byte[] sendByte = new byte[headBytes.Length + contentByte.Length];

                headBytes.CopyTo(sendByte, 0);
                contentByte.CopyTo(sendByte, 4);
                client.SendTo(sendByte, sendByte.Length, SocketFlags.None, ipep);

                // receive response
                string response = string.Empty;

                byte[] headBuffer = new byte[4];
                // read head for length
                client.Receive(headBuffer, 4, SocketFlags.None);
                int needRecvLength = BitConverter.ToInt32(headBuffer, 0);

                if (needRecvLength != 0)
                {
                    int notRecvLength = needRecvLength;
                    byte[] readBuffer = new byte[needRecvLength + 4];

                    do
                    {
                        int hasRecv = client.Receive(readBuffer, 4 + needRecvLength - notRecvLength, notRecvLength, SocketFlags.None);
                        notRecvLength -= hasRecv;

                    } while (notRecvLength != 0);

                    response = Encoding.UTF8.GetString(readBuffer, 4, needRecvLength);
                }

                return response;
            }
            finally
            {
                if (client.Connected)
                {
                    client.Close();
                }
            }
        }
    }
}
