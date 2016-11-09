namespace RequestLib.Requests
{
    public class RunPSRequest : Request
    {
        public string PSScript { get; set; }

        public RunPSRequest(string server, int port)
            : base(server, port)
        {
            Type = "PS";
        }

        protected override void SetParams()
        {
            base.SetParams();

            RequestXML.Value = PSScript;
        }
    }
}
