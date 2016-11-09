using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;

using Common.Network;
using Core.Model;

namespace Core.Providers.BuildProviders
{
    public class SupervisorWebBuildProvider:IBuildProvider
    {
        private string path = string.Empty;
        private string user = string.Empty;
        private string password = string.Empty;
        private string productName = string.Empty;

        private string localMapFolder = Core.LocalMappedFolder.SupervisorBuildFolder;

        private string buildPattern = "SUP_WebUI_*";

        public void SyncAllBuilds()
        {
            string mainlineFolder = string.Format(@"{0}\SUP_Main", localMapFolder);
            foreach (string d in Directory.GetDirectories(mainlineFolder, buildPattern))
            {
                if (!d.Contains("BAD"))
                {
                    DirectoryInfo info = new System.IO.DirectoryInfo(d);
                    DirectoryInfo remoteInfo = new System.IO.DirectoryInfo(d.Replace(localMapFolder, path));

                    string branchName = System.Text.RegularExpressions.Regex.Split(info.Name, @"_[0-9]\.[0-9][0-9]\.[0-9]+")[0];
                    string releaseAndNumber = System.Text.RegularExpressions.Regex.Match(info.Name, @"[0-9]\.[0-9][0-9]\.[0-9]+").Value;
                    string number = releaseAndNumber.Substring(releaseAndNumber.LastIndexOf('.') + 1);
                    string releaseName = releaseAndNumber.Substring(0, releaseAndNumber.LastIndexOf('.'));
                    Branch tempBranch = new Branch
                    {
                        Name = branchName,
                        Path = string.Empty,
                        Description = branchName,
                        Type = (int)SyncSourceType.FromBuildServer,
                        ProductId = Product.GetProductByName(productName).ProductId,
                    };
                    Branch branch = Branch.AddOrUpdateBranch(tempBranch);
                    Release tempRelease = new Release
                    {
                        Name = releaseName,
                        Description = releaseName,
                        Type = (int)SyncSourceType.FromBuildServer,
                        Path = string.Empty,
                        ProductId = Product.GetProductByName(productName).ProductId,
                    };
                    Release release = Release.AddOrUpdateRelease(tempRelease);

                    if (IsBuildFinished(info))
                    {
                        Build b = new Build
                        {
                            Name = info.Name,
                            Description = info.Name,
                            Location = remoteInfo.FullName,
                            BranchId = branch.BranchId,
                            ReleaseId = release.ReleaseId,
                            Number = number,
                            BuildType = BuildType.BuildMachine,
                            ProductId = Product.GetProductByName(productName).ProductId,
                            BuildStatus = BuildStatus.Success,
                            ProviderId = Provider.ProviderId,
                        };
                        if (null == Build.GetBuildByLocation(b.Location))
                        {
                            Build.Add(b);
                        }
                    }
                }
            }
        }

        private bool IsBuildFinished(DirectoryInfo info)
        {
            //check whether the file exist, SUP_MAIN_7.2.0.2126_SUP_WebUI.htm
            if (info.GetFiles("*.htm").Count() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateBuildStatus(Model.Build build)
        {
            if (build.BuildStatus == BuildStatus.Success)
            {
                if (System.IO.Directory.Exists(build.Location.Replace(path, localMapFolder)) &&
                    System.IO.Directory.Exists(System.IO.Path.Combine(build.Location.Replace(path, localMapFolder), "install")))
                {
                    //check whether the install folder exist or not

                    //keep as it is
                }
                else
                {
                    build.BuildStatus = BuildStatus.Delete;
                    build.Update();
                }
            }
            else if (build.BuildStatus == BuildStatus.NotExist)
            {
                if (System.IO.Directory.Exists(build.Location.Replace(path, localMapFolder)))
                {
                    build.BuildStatus = BuildStatus.Success;
                    build.Update();
                }
                else
                {
                    //keep as it is
                }
            }
        }

        public void DeleteBuild(Model.Build build)
        {
            Build.Delete(build);
        }

        public Model.Provider Provider
        {
            get;
            set;
        }

        public void ApplyConfig(string config)
        {
            XElement root = XElement.Parse(config);

            path = root.Element("path").Value;
            user = root.Element("username").Value;
            password = root.Element("password").Value;
            productName = root.Element("product").Value;

            NetUseHelper.NetUserRemoteFolerToLocalPath(path, localMapFolder, user, password);
        }
    }
}
