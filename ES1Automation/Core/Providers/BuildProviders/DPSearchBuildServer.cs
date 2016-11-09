using System;
using System.Collections.Generic;
using Core.Model;
using System.Xml.Linq;
using Common.FileCommon;
using Common.Network;

namespace Core.Providers.BuildProviders
{
    class DPSearchBuildServer : IBuildProvider
    {
        private string localMapFolder = Core.LocalMappedFolder.BPSearchBuildFolder;

        private string productFolderName = "DPSearch";

        private string ProductName = "Data Protection Search";

        private string DPSearchBuildNamePattern = @"^[0-9]\.[0-9]\.[0-9]\.[0-9]{1,4}$";
        protected string Path { get; private set; }

        protected string Username { get; private set; }

        protected string Password { get; private set; }

        public Provider Provider { get; set; }

        public void ApplyConfig(string config)
        {
            XElement root = XElement.Parse(config);

            Path = root.Element("path").Value;
            Username = root.Element("username").Value;
            Password = root.Element("password").Value;

            NetUseHelper.NetUserRemoteFolerToLocalPath(Path, localMapFolder, Username, Password);
        }


        public void DeleteBuild(Build build)
        {
            Build.Delete(build);
        }

        public void SyncAllBuilds()
        {
            foreach (Build buildOnBuildFileServer in GetAllBuildsOnDPSearchSBuildServer())
            {
                Build buildOnDB = Build.GetBuildByLocation(buildOnBuildFileServer.Location);
                if (buildOnDB != null)
                {
                }
                else
                {
                    buildOnDB = new Build
                    {
                        Name = buildOnBuildFileServer.Name,
                        Location = buildOnBuildFileServer.Location,
                        Description = buildOnBuildFileServer.Description,
                        BuildStatus = BuildStatus.Success,
                        BranchId = buildOnBuildFileServer.BranchId,
                        ReleaseId = buildOnBuildFileServer.ReleaseId,
                        BuildType = buildOnBuildFileServer.BuildType,
                        Number = buildOnBuildFileServer.Number,
                        ProductId = buildOnBuildFileServer.ProductId,
                        ProviderId = this.Provider.ProviderId,
                    };
                    Build.Add(buildOnDB);
                }
            }
        }

        public IList<Build> GetAllBuildsOnDPSearchSBuildServer()
        {
            List<Build> builds = new List<Build>();

            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(DPSearchBuildNamePattern);

            string rootDirectory = localMapFolder + @"\" + productFolderName + @"\";

            string branchFolder = "";

            getBuildsInFolder(builds, rootDirectory, branchFolder, reg);

            return builds;

        }

        private void getBuildsInFolder(IList<Build> builds, string rootFolder, string branchFolder, System.Text.RegularExpressions.Regex buildFolderPattern)
        {
            foreach (string d in System.IO.Directory.GetDirectories(rootFolder))
            {
                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(d);

                string buildName = info.Name;

                if (buildFolderPattern.IsMatch(buildName))
                {

                    string releaseName = buildName.Substring(0, buildName.LastIndexOf('.'));

                    string number = info.Name.Substring(buildName.LastIndexOf('.') + 1);

                    System.IO.DirectoryInfo remoteInfo = new System.IO.DirectoryInfo(d.Replace(localMapFolder, Path));

                    Branch tempBranch = new Branch
                    {
                        Name = branchFolder,
                        Path = string.Empty,
                        Description = branchFolder,
                        Type = (int)SyncSourceType.FromBuildServer,
                        ProductId = Product.GetProductByName(ProductName).ProductId,
                    };

                    Branch branch = Branch.AddOrUpdateBranch(tempBranch);

                    Release tempRelease = new Release
                    {
                        Name = releaseName,
                        Description = releaseName,
                        Type = (int)SyncSourceType.FromBuildServer,
                        Path = string.Empty,
                        ProductId = Product.GetProductByName(ProductName).ProductId,
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
                            ProductId = Product.GetProductByName(ProductName).ProductId,
                            BuildStatus = BuildStatus.Success,
                        };

                        builds.Add(b);
                    }
                }
                else
                {
                    string tempBranchFolder = string.IsNullOrEmpty(branchFolder) ? info.Name : branchFolder + @"\" + info.Name;

                    getBuildsInFolder(builds, d, tempBranchFolder, buildFolderPattern);
                }
            }
        }

        private bool IsBuildFinished(System.IO.DirectoryInfo localInfo)
        {
            try
            {
                foreach (System.IO.FileInfo file in localInfo.GetFiles())
                {
                    if (file.FullName.EndsWith("tar.gz") || file.FullName.EndsWith("tgz"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)//sometimes we don't have permission to get the access the files in one folder
            {
                return false;
            }
            return false;
        }


        public void UpdateBuildStatus(Build build)
        {
            if (build.BuildStatus == BuildStatus.Success)
            {
                if (System.IO.Directory.Exists(build.Location.Replace(Path, localMapFolder)))
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
                if (System.IO.Directory.Exists(build.Location.Replace(Path, localMapFolder)))
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
    }
}
    

