using System;
using System.Xml.Linq;
using Common.Network;

namespace RequestLib.Requests
{
    public abstract class SocketRequest : Request
    {
        private readonly SocketUserClient client;

        public SocketRequest(string server, int port)
            : base(server, port)
        {
            client = new SocketUserClient(server, port);
        }

        protected override string RequestServer()
        {
            ResultString = client.SentMsgToServer(RequestXML.ToString());

            if (ResultString.StartsWith("ERROR"))
            {
                throw new Exception(ResultString);
            }

            return ResultString;
        }
    }
}
