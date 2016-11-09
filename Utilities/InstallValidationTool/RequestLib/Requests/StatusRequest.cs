using System;
using System.Collections.Generic;
using System.Linq;

namespace RequestLib.Requests
{
    public class StatusRequest : Request
    {
        public StatusRequest(string server, int port)
            : base(server, port)
        {
            Type = "STATUS";
        }

        public Dictionary<string, string> GetStatus()
        {
            string statusResult = RequestServer();

            string[] statusList = statusResult.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            var status = new Dictionary<string, string>();

            foreach (string rawStatus in statusList)
            {
                string[] pairs = rawStatus.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                if (pairs.Count() == 2)
                {
                    status.Add(pairs[0].Trim(), pairs[1].Trim());
                }
            }

            return status;
        }
    }
}
