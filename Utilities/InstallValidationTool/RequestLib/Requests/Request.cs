using System;
using System.Xml.Linq;

namespace RequestLib.Requests
{
    public abstract class Request
    {
        private readonly SocketUserClient client;

        protected string Type;

        protected XElement RequestXML = new XElement("request");

        public string ResultString { get; protected set; }

        protected Request(string server, int port)
        {
            client = new SocketUserClient(server, port);
        }

        protected virtual void SetParams()
        {
            RequestXML.Add(new XAttribute("type", Type));
        }

        public string RequestServer()
        {
            SetParams();

            ResultString = client.SentMsgToServer(RequestXML.ToString());

            if (ResultString.StartsWith("ERROR"))
            {
                throw new Exception(ResultString);
            }

            return ResultString;
        }
    }
}
