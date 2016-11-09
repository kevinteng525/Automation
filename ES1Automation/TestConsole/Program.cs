using System;
using System.Collections.Generic;
using ATFExchangeCentre.Agents;
using ATFExchangeCentre.Requests;
using Core.Model;
using Core.Providers.TestCaseProviders;
using ES1Common.RQM;
using ES1Common.Virtualization;
using com.vmware.vcloud.sdk;
using Core.DTO;
namespace TestConsole
{
    class Program
    {

        static void Main(string[] args)
        {
          
            Console.WriteLine(Provider.GetProvidersByCategory((ProviderCategory)2).ToDTOs().Count);
        
        }
       
        static void RQM()
        {
            //ITestCaseProvider provider = Provider.GetProviderById(2).CreateProvider() as ITestCaseProvider;

            //provider.SyncAllTestCase();

            //This is for the test purpose.

            RQMServer server = new RQMServer("https://jazzapps.otg.com:9443/qm");

            server.UserAuthentication("svc_s1auto", "emcsiax@QA");

            //var projects = server.GetAllProjects();

            //foreach (var project in projects)
            //{
            //    var testcases = server.GetTestCasesByProject(project);
            //    var executionWorkItems = server.GetExecutionWorkItemByProject(project);
            //}

            var testCases = server.GetTestCasesByProject("SourceOne");
        }

        static void CallAgent()
        {

            //string ipAddress = "10.62.40.20";
            //string domain = "qaes1";
            //string username = "es1service";
            //string password = "emcsiax@QA";
            // install in remote server
            //WinServiceAgent.UninstallAgent(
            //    ipAddress, domain + @"\" + username, password, "S1Agent.WinService.exe", "S1Agent",
            //    @"C:\Personal\Projects\ES1.TFS\AutomationFramework\ES1Automation\Main\ES1Automation\S1Agent.WinService\bin\Debug",
            //    @"\\" + ipAddress + @"\Share\Agent", @"C:\Share\Agent");

            //WinServiceAgent.InstallAgent(
            //    ipAddress, domain + @"\" + username, password, "S1Agent.WinService.exe", "S1Agent",
            //    @"C:\Personal\Projects\ES1.TFS\AutomationFramework\ES1Automation\Main\ES1Automation\S1Agent.WinService\bin\Debug",
            //    @"\\" + ipAddress + @"\Share\Agent", @"C:\Share\Agent");

            //// for local debug -- host service in console
            string ipAddress = "127.0.0.1";
            //string domain = "corp";
            //string username = "tangt4";
            //string password = "***";
            //// start agent
            //WinServiceAgent winService = new WinServiceAgent(1234);
            //winService.StartAgent();

            Console.WriteLine(WinServiceAgent.IsAgentAvaliable(ipAddress, 1234));

            //Request request = new RunTestCaseRequest()
            //{
            //    ExecutionEngineIp = "127.0.0.1",
            //    ExecutionEnginePort = 1234,
            //    JobType = JobType.Sequence,
            //    TestCasesToRun = new List<string> { "1", "2", "3" }
            //};

            //Request request = new SystemRequest() { };

          //  Request result = RequestFactory.CreateRequest(WinServiceAgent.SentRequest(ipAddress, 1234, request));
         //   Console.WriteLine(result.ToXML());

            Console.ReadLine();
        }

        static void VApp()
        {
            var vCloud = new VCloud("https://brs-dur-vmdvcd1.brs.lab.emc.com", "automation@Mozy", "emcsiax@QA");


            var vApps = vCloud.GetVappsInOrginazation("Mozy");

            var vApp = vCloud.GetVappByName("Mozy", "SourceOne-Mozy-OVDC1", "TEST02");

            vCloud.ConfigVAppNetworkFirework(vApp, "vAppNetwork", true);
        }
    }
}
