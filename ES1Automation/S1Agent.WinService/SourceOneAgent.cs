using System;
using System.ServiceProcess;
using ATFExchangeCentre.Agents;
using ATFExchangeCentre.Requests;
using ES1Common.Logs;

namespace S1Agent.WinService
{
    partial class SourceOneAgent : ServiceBase
    {
        public static readonly AutomationLog Log = new AutomationLog("SourceOneAgentLog");

        public static readonly Agent agent = new WinServiceAgent(1234);

        public SourceOneAgent()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Log.logger.Info("Start Agent Service ...");
                agent.StartAgent();
                RunJobRequest.runCaseInAgentEvent += RunCaseInAgentEventHandler;
                Log.logger.Info("Socket Agent Started");
            }
            catch (Exception ex)
            {
                Log.logger.Error(ex.Message);
            }
        }

        protected override void OnStop()
        {
            agent.StopAgent();
        }

        private void RunCaseInAgentEventHandler(object sender, EventArgs e)
        {
            var request = sender as RunJobRequest;

            foreach(var testcase in request.TestCasesToRun)
            {
                Log.logger.Info("Update test case status");

                var testCaseExecutionReport = new ReportJobRequest(ReportJobRequest.TESTCASEEXECUTION, int.Parse(testcase), "Running");
                WinServiceAgent.SentRequest(request.ExecutionEngineIp, request.ExecutionEnginePort, testCaseExecutionReport);

                try
                {
                    bool isTestCasePass = false;

                    string[] pair = testcase.Split(new[] { "$" }, StringSplitOptions.RemoveEmptyEntries);
                    string testCaseId = pair[0];
                    int providerId = int.Parse(pair[1]);

                    //add the code to run the test case, parse the result and send the result file into the file server.




                    if (isTestCasePass)
                    {
                        var testCaseResultReport = new ReportJobRequest(ReportJobRequest.TESTCASERESULT, int.Parse(testcase), "Pass");
                        WinServiceAgent.SentRequest(request.ExecutionEngineIp, request.ExecutionEnginePort, testCaseResultReport);
                    }
                    else
                    {
                        var testCaseResultReport = new ReportJobRequest(ReportJobRequest.TESTCASERESULT, int.Parse(testcase), "Failed");
                        WinServiceAgent.SentRequest(request.ExecutionEngineIp, request.ExecutionEnginePort, testCaseResultReport);
                    }
                }
                catch (Exception ex)
                {
                    var testCaseResultReport = new ReportJobRequest(ReportJobRequest.TESTCASERESULT, int.Parse(testcase), "Exception", ex.Message);
                    WinServiceAgent.SentRequest(request.ExecutionEngineIp, request.ExecutionEnginePort, testCaseResultReport);
                }

                testCaseExecutionReport = new ReportJobRequest(ReportJobRequest.TESTCASEEXECUTION, int.Parse(testcase), "Complete");
                WinServiceAgent.SentRequest(request.ExecutionEngineIp, request.ExecutionEnginePort, testCaseExecutionReport);
            }

            var jobStatusReport = new ReportJobRequest(ReportJobRequest.AUTOMATIONJOBSTATUS, request.JobId, "Complete");
            WinServiceAgent.SentRequest(request.ExecutionEngineIp, request.ExecutionEnginePort, jobStatusReport);
        }
    }
}
