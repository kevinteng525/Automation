using System.Xml.Linq;

namespace ATFExchangeCentre.Requests
{
    public abstract class Request
    {
        protected XElement RequestRoot;

        protected XElement ResponseRoot;

        protected XElement Root;

        public string Type { get; protected set; }

        #region define request type

        public const string CMD = "CMD";

        public const string SYSTEM = "SYSTEM";

        public const string RUNJOB = "RUNJOB";

        public const string REPORTJOB = "REPORTJOB";

        #endregion

        #region constructor

        public Request() { }

        public Request(XElement root)
        {
            Root = root;
            Type = RequestFactory.GetAttributeValue(Root, "type");
            RequestRoot = Root.Element("request");
            ResponseRoot = Root.Element("response");
        }

        #endregion

        protected virtual void CheckParams() { }

        protected abstract void HandleRequest();

        /// <summary>
        /// host server to process request
        /// </summary>
        /// <param name="requestString"></param>
        /// <returns></returns>
        public static string ProcessRequest(string requestString)
        {
            var request = RequestFactory.CreateRequest(requestString);

            request.HandleRequest();

            return request.ToString();
        }

        public virtual XElement ToXML()
        {
            Root = new XElement("root");
            Root.Add(new XAttribute("type", Type));

            RequestRoot = new XElement("request");
            ResponseRoot = new XElement("response");

            Root.Add(RequestRoot);
            Root.Add(ResponseRoot);

            return Root;
        }

        public override string ToString()
        {
            return ToXML().ToString();
        }
    }
}
