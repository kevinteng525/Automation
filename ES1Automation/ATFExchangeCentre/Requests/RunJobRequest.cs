using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Core.Model;

namespace ATFExchangeCentre.Requests
{
    public class RunJobRequest : Request
    {
        public delegate void RunCaseInAgent(object sender, EventArgs e);

        public static event RunCaseInAgent runCaseInAgentEvent;

        public string ExecutionEngineIp { get; private set; }

        public int ExecutionEnginePort { get; private set; }

        public int JobId { get; private set; }

        public JobType JobType { get; private set; }

        public IList<string> TestCasesToRun { get; private set; }

        public RunJobRequest(string  executionEngineIp, int executionEnginePort, int jobId, JobType jobType, IList<string> testcases)
        {
            Type = Request.RUNJOB;
            ExecutionEngineIp = executionEngineIp;
            ExecutionEnginePort = executionEnginePort;
            JobId = jobId;
            JobType = jobType;
            TestCasesToRun = testcases;
        }

        public RunJobRequest(XElement root)
            : base(root)
        {
            ExecutionEngineIp = RequestFactory.GetAttributeValue(RequestRoot, "executionEngineIp");
            ExecutionEnginePort = int.Parse(RequestFactory.GetAttributeValueOptional(RequestRoot, "executionEnginePort"));
            JobId = int.Parse(RequestFactory.GetAttributeValue(RequestRoot, "jobId"));
            JobType = (JobType)Enum.Parse(typeof(JobType), RequestFactory.GetAttributeValueOptional(RequestRoot, "jobType"), true);
            TestCasesToRun = RequestRoot.Value.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
        }

        protected override void CheckParams()
        {
            if (string.IsNullOrWhiteSpace(ExecutionEngineIp))
            {
                throw new Exception("ExecutionEngineIp should not be null");
            }
        }

        protected override void HandleRequest()
        {
            if (runCaseInAgentEvent != null)
            {
                runCaseInAgentEvent.Invoke(this, new EventArgs());
            }
        }

        public override XElement ToXML()
        {
            base.ToXML();

            // request
            RequestRoot.Add(new XAttribute("executionEngineIp", ExecutionEngineIp));
            RequestRoot.Add(new XAttribute("executionEnginePort", ExecutionEnginePort));
            RequestRoot.Add(new XAttribute("jobId", JobId));
            RequestRoot.Add(new XAttribute("jobType", JobType));

            RequestRoot.Value = string.Join(";", TestCasesToRun);

            return Root;
        }
    }
}
