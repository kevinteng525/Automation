using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Model;
using ES1Common.Exceptions;
using ATFExchangeCentre;
using ATFExchangeCentre.Agents;
using ATFExchangeCentre.Requests;
using Common.Network;
using System.Configuration;

namespace ExecutionEngine.WinService
{
    public partial class ExecutionService : ServiceBase
    {
        private System.Timers.Timer jobMonitor;

        private Agent agent;

        public ExecutionService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ATFEnvironment.Log.logger.Info("Execution Service Start");

            try
            {
                // init agent
                agent = new WinServiceAgent(1234);
                agent.StartAgent();

                // init monitor
                ATFEnvironment.Log.logger.Info("Initialization execution monitor");
                jobMonitor = new System.Timers.Timer(1000 * 10) { Enabled = true };
                jobMonitor.Start();
                jobMonitor.Elapsed += MoniorAutomationJob;
            }
            catch (Exception ex)
            {
                ATFEnvironment.Log.logger.Error(ex.Message);
            }
        }

        protected override void OnStop()
        {
            // stop agent
            agent.StopAgent();

            // stop monitor
            ATFEnvironment.Log.logger.Info("execution monitor stop");
            jobMonitor.Stop();
            jobMonitor.Dispose();
        }

        private void MoniorAutomationJob(object sender, EventArgs e)
        {
            AutomationJob job = AutomationJob.GetJobsByStatus(JobStatus.Ready).FirstOrDefault();

            if(job != null)
            {
                try
                {
                    Request request = new RunJobRequest
                    (
                        ConfigurationManager.AppSettings["executionEngineIp"],
                        int.Parse(ConfigurationManager.AppSettings["executionEnginePort"]),
                        job.JobId,
                        job.JobType,
                        (
                            from execution in TestCaseExecution.GetTestCaseExectionByJob(job.JobId)
                            select string.Format("{0}${1}", execution.TestCaseId.ToString(), execution.TestCase.ProviderId)
                        ).ToList()
                    );
                    //TODO, add the code the code to determine which server does the request send to.
                    string targetServer = "";

                    
                    WinServiceAgent.SentRequest(targetServer, 1234, request);
                }
                catch (Exception ex)
                {
                    job.Description = ex.Message;
                    job.SetJobsStatus(JobStatus.Failed);
                    ATFEnvironment.Log.logger.Error(ex.Message);
                }
            }
        }

        #region Automation Jobs

        private void RunAutomationJob(AutomationJob job, CancellationToken token)
        {
            ATFEnvironment.Log.logger.Info("Run automation job #" + job.JobId);
            job.SetJobsStatus(JobStatus.Running);

            try
            {

                var testCaseExectionList = TestCaseExecution.GetTestCaseExectionByJob(job.JobId);

                switch (job.JobType)
                {
                    case JobType.Sequence:
                        RunAutomationCasesInSequence(testCaseExectionList);
                        break;

                    case JobType.Concurrency:
                        RunAutomationCasesInConcurrency(testCaseExectionList);
                        break;

                    default:
                        break;
                }

                if (token.IsCancellationRequested)
                {
                    ATFEnvironment.Log.logger.Info("Timeout automation job #" + job.JobId);
                    job.SetJobsStatus(JobStatus.Failed);
                }
                else
                {
                    ATFEnvironment.Log.logger.Info("Complete automation job #" + job.JobId);
                    job.SetJobsStatus(JobStatus.Complete);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Failed to run automation job #{0}, error: {1}", job.JobId, ex.Message);
                ATFEnvironment.Log.logger.Error(errorMsg);
                job.SetJobsStatus(JobStatus.Failed);
            }
        }

        private void RunAutomationCasesInSequence(IList<TestCaseExecution> testCaseExecutionList)
        {
            foreach (var testCaseExecution in testCaseExecutionList)
            {
                var cts = new CancellationTokenSource();

                Task caseToRun = Task.Factory.StartNew(() => { RunAutomationCase(testCaseExecution, cts.Token); });

                if (!caseToRun.Wait(testCaseExecution.Timeout, cts.Token))
                {
                    cts.Cancel();

                    testCaseExecution.SetStatus(ExecutionStatus.Fail);

                    var result = TestResult.GetTestResultByExecutionId(testCaseExecution.ExecutionId);

                    if (result != null)
                    {
                        result.SetResult(ResultType.TimeOut);
                    }
                }
            }
        }

        private void RunAutomationCasesInConcurrency(IList<TestCaseExecution> testCaseExecutionList)
        {
            if (testCaseExecutionList.Count == 0)
            {
                return;
            }

            int timeout = (from t in testCaseExecutionList select t.Timeout).Max();

            var cts = new CancellationTokenSource();
            Task[] testCases = new Task[testCaseExecutionList.Count];

            for (int i = 0; i < testCaseExecutionList.Count; i++)
            {
                testCases[i] = Task.Factory.StartNew(() => { RunAutomationCase(testCaseExecutionList[i], cts.Token); });
            }

            if (!Task.WaitAll(testCases, timeout, cts.Token))
            {
                cts.Cancel();

                // set the status to timeout
                foreach (var testCaseExecution in testCaseExecutionList)
                {
                    if (!testCaseExecution.IsFinished())
                    {
                        testCaseExecution.SetStatus(ExecutionStatus.Fail);

                        var result = TestResult.GetTestResultByExecutionId(testCaseExecution.ExecutionId);

                        if (result != null)
                        {
                            result.SetResult(ResultType.TimeOut);
                        }
                    }
                }
            }
        }

        private void RunAutomationCase(TestCaseExecution testCaseExecution, CancellationToken token)
        {
            ATFEnvironment.Log.logger.Info("Run automation case #" + testCaseExecution.TestCaseId);

            testCaseExecution.SetStatus(ExecutionStatus.Running);

            var result = new TestResult
            {
                ExecutionId = testCaseExecution.ExecutionId,
                Result = (int)ResultType.NotRun,
                IsTriaged = false,
                TriagedBy = null,
                Files = null,
            };

            TestResult.CreateRunResult(result);

            try
            {
                // todo: call agent to run test cases
                // change test result
                Thread.Sleep(60 * 1000);

                testCaseExecution.SetStatus(ExecutionStatus.Complete);
                result.SetResult(ResultType.Pass);


                if (token.IsCancellationRequested)
                {
                    ATFEnvironment.Log.logger.Info("Timeout to run automation case #" + testCaseExecution.TestCaseId);
                    testCaseExecution.SetStatus(ExecutionStatus.Complete);
                    result.SetResult(ResultType.TimeOut);
                }
            }
            catch (EnvironmentException ex)
            {
                string errorMsg = string.Format("Failed to run automation case #{0}, error: {1}", testCaseExecution.TestCaseId, ex.Message);
                ATFEnvironment.Log.logger.Error(errorMsg);
                testCaseExecution.SetStatus(ExecutionStatus.Fail);
                result.SetResult(ResultType.Exception);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Failed to run automation case #{0}, error: {1}", testCaseExecution.TestCaseId, ex.Message);
                ATFEnvironment.Log.logger.Error(errorMsg);
                testCaseExecution.SetStatus(ExecutionStatus.Fail);
                result.SetResult(ResultType.Failed);
            }
        }

        #endregion
    }
}
