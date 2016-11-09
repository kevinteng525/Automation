using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.FileCommon;
using Core.Model;
using Core.Management;
using System.IO;
using Core.Providers.EnvrionmentProviders;
using System.Xml;
using System.Configuration;
using Common.Network;

namespace SaberAgent.WindowsFormApp
{
    public abstract class ResultParser
    {
        private static string ResultRootPath = System.Configuration.ConfigurationManager.AppSettings["ResultRootPath"];
        private static string ResultHost = System.Configuration.ConfigurationManager.AppSettings["SaberAgentInstallerHostMachine"];
        private static string Administrator = System.Configuration.ConfigurationManager.AppSettings["SaberAgentInstallerHostMachineAdmin"];
        private static string Password = System.Configuration.ConfigurationManager.AppSettings["SaberAgentInstallerHostMachineAdminPassword"];
        public abstract void ParseResult(string resultFilePath, int jobId);


        protected abstract ResultType GetResult(XmlNode resultNode);


        protected abstract string GetSourceId(XmlNode resultNode);
        

        protected abstract string GetFailureInfo(XmlNode resultNode);
        

        public string CopyResultFilesToServer(string filePath, int jobId)
        {
            string remoteResultPath = string.Empty;
            int taskId = JobManagement.GetAutomationTaskOfJob(AutomationJob.GetAutomationJob(jobId)).TaskId;
            string result = NetUseHelper.NetUserRemoteFolerToLocalPath(ResultRootPath, Core.LocalMappedFolder.ResultTempFolder, Administrator, Password);
            if (result == "Success")
            {
                SaberAgent.Log.logger.Info(string.Format("The remote folder [{0}] is mapped to local folder [{1}] successfully", ResultRootPath, Core.LocalMappedFolder.ResultTempFolder));
            }
            else
            {
                SaberAgent.Log.logger.Error(string.Format("Failed to map folder [{0}] to local [{1}], result: [{2}]", ResultRootPath, Core.LocalMappedFolder.ResultTempFolder, result));
                return string.Empty;
            }
            try
            {
                string taskResultPath = Core.LocalMappedFolder.ResultTempFolder + @"\" + taskId.ToString();
                if (!FileHelper.IsExistsFolder(taskResultPath))
                {
                    FileHelper.CreateFolder(taskResultPath);
                }
                string jobResultPath = taskResultPath + @"\" + jobId.ToString();

                if (!FileHelper.IsExistsFolder(jobResultPath))
                {
                    FileHelper.CreateFolder(jobResultPath);
                }
                FileHelper.CopyDirectory(filePath, jobResultPath);

                remoteResultPath = ResultRootPath + @"\" + taskId.ToString();
                remoteResultPath = remoteResultPath + @"\" + jobId.ToString();
                return remoteResultPath;
            }
            catch (Exception ex)
            {
                SaberAgent.Log.logger.Error(ex);
                return string.Empty;
            }
        }
    }
}
