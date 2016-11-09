using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using Core.Model;
using ES1Common.Logs;
using System.IO;
using Core.Management;
using Core;

namespace ExecutionAgent
{
    public partial class ExecutionAgentWinService : ServiceBase
    {
        System.Timers.Timer timer = new System.Timers.Timer(1000 * 5);
        static bool b = false;

        public ExecutionAgentWinService()  
        {            
            InitializeComponent();
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            timer.Stop();
            ATFEnvironment.Log.logger.Info("OnTimedEvent");

            while (b)
            {
                Thread.Sleep(10 * 1000);
            }

            // Check Jobs
            List<AutomationJob> jobs = null;
            List<AutomationJob> jobsReady = AutomationJob.GetJobs((int)JobStatus.Ready);
            List<AutomationJob> jobsRunning = AutomationJob.GetJobs((int)JobStatus.Running);
            if ((jobsReady == null || jobsReady.Count <= 0) &&
                (jobsRunning == null || jobsRunning.Count <= 0))
            {
                ATFEnvironment.Log.logger.Info("No Jobs ready to run");
                timer.Start();
                return;            
            }
            else
            {
                if(jobsReady != null && jobsReady.Count > 0)
                {
                    jobs = jobsReady;
                    if(jobsRunning != null && jobsRunning.Count > 0)
                        jobs.AddRange(jobsRunning);
                }
                else
                    jobs = jobsRunning;
            }
           
            AutomationJob job = jobs.First();
            List<TestCaseExecution> exes = null;
            foreach (AutomationJob j in jobs)
            {
                exes = TestCaseExecution.GetExecutions(j.JobId, (int)ExecutionStatus.NotRunning);
                if (exes == null || exes.Count() <= 0)
                {
                    ATFEnvironment.Log.logger.Info("No Execution to run for Job#" + j.JobId.ToString());
                    continue;
                }
                else
                {   
                    job = j;
                    break;
                }
            }

            if ((exes != null && exes.Count() > 0))
            {
                try
                {
                    job.JobStatus = JobStatus.Running;
                    job.Update();
                    ATFEnvironment.Log.logger.Info("Change status Ready -> Running on Job#" + job.JobId.ToString());
                    ATFEnvironment.Log.logger.Info("Start to run executions for Job#" + job.JobId.ToString() + " Total exes:" + exes.Count());

                    foreach (TestCaseExecution exe in exes)
                    {
                        ATFEnvironment.Log.logger.Info("Start execution#" + exe.ExecutionId.ToString());
                        exe.Status = (int)ExecutionStatus.Running;
                        exe.StartTime = System.DateTime.Now;
                        exe.Update();

                        TestCase tc = TestCase.GetTestCase(exe.TestCaseId);
                        string batPath = (tc != null && tc.ScriptPath != null && File.Exists(@"C:\Automation\ExecutionAgent\" + tc.ScriptPath)) ? 
                                                        (@"C:\Automation\ExecutionAgent\" + tc.ScriptPath) :
                                                        (@"C:\Automation\ExecutionAgent\mockExe\test1.bat"); // Default value

                        ATFEnvironment.Log.logger.Info("Running test case");
                        if (!Directory.Exists(@"C:\Automation\ExecutionAgent\Logs"))
                            Directory.CreateDirectory(@"C:\Automation\ExecutionAgent\Logs");    

                        RunBat(batPath, exe.ExecutionId.ToString());
                        ATFEnvironment.Log.logger.Info("Finished Running");

                        ResultType result = ResultParser.GetResult(@"C:\Automation\ExecutionAgent\Logs\" + exe.ExecutionId.ToString() + ".result");
                        ATFEnvironment.Log.logger.Info("Job#" + job.JobId.ToString() + ",Execution#" + exe.ExecutionId.ToString() + " Result: " + result.ToString());

                        TestResult res = new TestResult()
                        {
                            ExecutionId = exe.ExecutionId,
                            Result = (int)result,
                            IsTriaged = false,
                            Description = null,
                        };
                        TestResult r = TestResult.CreateRunResult(res);
                        ATFEnvironment.Log.logger.Info("Create result for Job#"+job.JobId.ToString()+"Execution#" + exe.ExecutionId.ToString());

                        exe.EndTime = System.DateTime.Now;
                        exe.Status = (int)ExecutionStatus.Complete;
                        exe.Update();
                        ATFEnvironment.Log.logger.Info("Complete execution#" + exe.ExecutionId.ToString());


                        ATFEnvironment.Log.logger.Info("Copying log files to the server, clear local files");
                        ResultParser.CopyResultFilesToServer();

                    }

                }
                catch (Exception exc)
                {
                    ATFEnvironment.Log.logger.Error("Error On Change Job status or run exections for Job#" + job.JobId.ToString(), exc);                
                }                
            
            }        

            timer.Start();
        }

        private static void RunBat(string batPath, string arg)
        {
            try 
            {

                Process pro = new Process();
                FileInfo file = new FileInfo(batPath);
                pro.StartInfo.WorkingDirectory = file.Directory.FullName;
                pro.StartInfo.Arguments = arg;
                pro.StartInfo.FileName = batPath;
                pro.StartInfo.UseShellExecute = true;
                pro.StartInfo.CreateNoWindow = false;
                pro.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                //pro.StartInfo.RedirectStandardOutput = true;
                pro.Start();
                //StreamReader reader = pro.StandardOutput;
                //string line = reader.ReadLine();
                //while (!reader.EndOfStream)
                //{
                //    output += " " + line;
                //    line = reader.ReadLine();
                //}
                pro.WaitForExit();
                pro.Close();
                //reader.Close();    
            
            }
            catch(Exception e)
            {
                ATFEnvironment.Log.logger.Error("Error running RunBat()", e);
            }

        }
    }
}
