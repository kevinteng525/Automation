using System.Xml.Linq;
using Core.Model;
using ES1Common.Exceptions;

namespace ATFExchangeCentre.Requests
{
    public class ReportJobRequest : Request
    {
        public const string TESTCASEEXECUTION = "TESTCASEEXECUTION";

        public const string TESTCASERESULT = "TESTCASERESULT";

        public const string AUTOMATIONJOBSTATUS = "AUTOMATIONJOBSTATUS";

        public string ReportType { get; private set; }

        public int Id { get; private set; }

        public string Status { get; private set; }

        public string Infomation { get; private set; }

        public ReportJobRequest(string type, int id, string status)
        {
            Type = Request.REPORTJOB;
            ReportType = type;
            Id = id;
            Status = status;
        }

        public ReportJobRequest(string type, int id, string status, string info)
        {
            Type = Request.REPORTJOB;
            ReportType = type;
            Id = id;
            Status = status;
            Infomation = info;
        }

        public ReportJobRequest(XElement root)
            : base(root)
        {
            ReportType = RequestFactory.GetAttributeValue(RequestRoot, "reportType");
            Id = int.Parse(RequestFactory.GetAttributeValue(RequestRoot, "id"));
            Status = RequestFactory.GetAttributeValue(RequestRoot, "status");
            Infomation = RequestRoot.Value;
        }

        protected override void HandleRequest()
        {
            switch (ReportType.ToLower().Trim())
            {
                case TESTCASEEXECUTION:
                    HandleTestCaseExecutionRequest();
                    break;

                case TESTCASERESULT:
                    HandleTestCaseResultRequest();
                    break;

                case AUTOMATIONJOBSTATUS:
                    HandleJobStatusRequest();
                    break;

                default:
                    throw new FrameworkException("Agent", "no defined report type");
            }
        }

        protected void HandleTestCaseExecutionRequest()
        {
            var testCaseExecution = TestCaseExecution.GetTestCaseExecution(Id);

            if (testCaseExecution != null)
            {
                switch (Status)
                {
                    case "Running":
                        testCaseExecution.SetStatus(ExecutionStatus.Running);
                        break;

                    case "Complete":
                        testCaseExecution.SetStatus(ExecutionStatus.Complete);
                        break;

                    case "Cancelled":
                        testCaseExecution.SetStatus(ExecutionStatus.Cancelled);
                        break;

                    case "Fail":
                        testCaseExecution.SetStatus(ExecutionStatus.Fail);
                        break;
                }
            }
        }

        protected void HandleTestCaseResultRequest()
        {
            var testCaseResult = TestResult.CreateRunResult
                                (
                                    new TestResult
                                    {
                                        ExecutionId = Id,
                                        Result = (int)ResultType.NotRun,
                                        IsTriaged = false,
                                        TriagedBy = null,
                                        Files = null,
                                    }
                                );

            switch (Status)
            {
                case "NotRun":
                    testCaseResult.SetResult(ResultType.NotRun);
                    break;

                case "Pass":
                    testCaseResult.SetResult(ResultType.Pass);
                    break;

                case "Failed":
                    testCaseResult.SetResult(ResultType.Failed);
                    break;

                case "TimeOut":
                    testCaseResult.SetResult(ResultType.TimeOut);
                    break;

                case "Exception":
                    testCaseResult.SetResult(ResultType.Exception);
                    break;
            }
        }

        protected void HandleJobStatusRequest()
        {
            var job = AutomationJob.GetAutomationJob(Id);

            if (job != null)
            {
                switch (Status)
                {
                    case "Complete":
                        job.SetJobsStatus(JobStatus.Complete);
                        break;
                }
            }
        }

        public override XElement ToXML()
        {
            base.ToXML();

            // request
            RequestRoot.Add(new XAttribute("reportType", ReportType));
            RequestRoot.Add(new XAttribute("id", Id));
            RequestRoot.Add(new XAttribute("status", Status));
            RequestRoot.Value = Infomation;

            return Root;
        }

    }
}
