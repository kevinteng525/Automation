using System;
using System.Collections.Generic;
using Core.Model;
using System.Xml.Linq;
using Common.FileCommon;
using Common.Network;

namespace Core.Providers.BuildProviders
{
    public class BuildFileServer : IBuildProvider
    {
        private string localMapFolder = Core.LocalMappedFolder.CommonBuildFolder;

        private string mainlineBuildNamingPattern = "Mainline*";

        private string featureBranchBuildNamingPatern = "Janus*";

        private string maintBranchBuildNamingPattern = "Mainline*";

        private string mainlineBuildFolder = "ES1";

        private string featureBranchBuildFolder = "ES1__Branch";
        
        //Currently we only handle the 7.0 maint branch.
        private string maintBranchBuildFolderPattern = "ES1_7*";

        protected string Path { get; private set; }

        protected string Username { get; private set; }

        protected string Password { get; private set; }

        protected string ProductName { get; private set; }

        public Provider Provider { get; set; }

        public void ApplyConfig(string config)
        {
            XElement root = XElement.Parse(config);

            Path = root.Element("path").Value;
            Username = root.Element("username").Value;
            Password = root.Element("password").Value;
            ProductName = root.Element("product").Value;

            NetUseHelper.NetUserRemoteFolerToLocalPath(Path,localMapFolder, Username, Password);
        }


        public void DeleteBuild(Build build)
        {
            Build.Delete(build);
        }

        public void SyncAllBuilds()
        {
            foreach (Build buildOnBuildFileServer in GetAllBuildsOnBuildFileServer())
            {
                Build buildOnDB = Build.GetBuildByLocation(buildOnBuildFileServer.Location);
                if (buildOnDB != null)
                {
                }
                else
                {
                    buildOnDB = new Build { 
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

        public IList<Build> GetAllBuildsOnBuildFileServer()
        {
            List<Build> builds = new List<Build>();
            //update SourceOne builds
            const string BuildFolderIndexName = "PATH";
            const string BuildPatternIndexName="PATTERN";
            const string BuildBranchIndexName = "BRANCH";

            List<Dictionary<string, string>> buildFoldersList = new List<Dictionary<string, string>>();

            string es1Main = System.IO.Path.Combine(localMapFolder + @"\", mainlineBuildFolder);
            Dictionary<string, string> temp = new Dictionary<string, string>();
            temp[BuildFolderIndexName] = es1Main;
            temp[BuildPatternIndexName] = mainlineBuildNamingPattern;
            temp[BuildBranchIndexName] = "Mainline";
            buildFoldersList.Add(temp);

            string featureBranch = System.IO.Path.Combine(localMapFolder + @"\", featureBranchBuildFolder);
            temp = new Dictionary<string, string>();
            temp[BuildFolderIndexName] = featureBranch;
            temp[BuildPatternIndexName] = featureBranchBuildNamingPatern;
            temp[BuildBranchIndexName] = string.Empty;
            buildFoldersList.Add(temp);

            string[] maintBranches = System.IO.Directory.GetDirectories(localMapFolder + @"\", maintBranchBuildFolderPattern);  
            foreach (string maintBranch in maintBranches)
            {
                temp = new Dictionary<string, string>();
                temp[BuildFolderIndexName] = maintBranch;
                temp[BuildPatternIndexName] = maintBranchBuildNamingPattern;
                temp[BuildBranchIndexName] = new System.IO.DirectoryInfo(maintBranch).Name;//The branch name is the folder name
                buildFoldersList.Add(temp);
            }


            foreach (Dictionary<string, string> buildFolder in buildFoldersList)
            {
                foreach (string d in System.IO.Directory.GetDirectories(buildFolder[BuildFolderIndexName], buildFolder[BuildPatternIndexName]))
                {
                    if (!d.Contains("BAD"))
                    {
                        System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(d);

                        System.IO.DirectoryInfo remoteInfo = new System.IO.DirectoryInfo(d.Replace(localMapFolder, Path));

                        string branchName = string.IsNullOrEmpty(buildFolder[BuildBranchIndexName]) ? System.Text.RegularExpressions.Regex.Split(info.Name, @"_[0-9]\.[0-9]\.[0-9]\.[0-9]+")[0] : buildFolder[BuildBranchIndexName];
                        string releaseAndNumber = System.Text.RegularExpressions.Regex.Match(info.Name, @"[0-9]\.[0-9]\.[0-9]\.[0-9]+").Value;
                        string number = releaseAndNumber.Substring(releaseAndNumber.LastIndexOf('.') + 1);
                        string releaseName = releaseAndNumber.Substring(0, releaseAndNumber.LastIndexOf('.'));
                        Branch tempBranch = new Branch
                        {
                            Name = branchName,
                            Path = string.Empty,
                            Description = branchName,
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
                                ProviderId = this.Provider.ProviderId,
                            };
                            builds.Add(b);
                        }
                    }
                }
            }
            return builds;
        }

        private bool IsBuildFinished(System.IO.DirectoryInfo localInfo)
        {
            try
            {
                foreach (System.IO.FileInfo file in localInfo.GetFiles())
                {
                    if (file.FullName.EndsWith("BuildDuration.txt"))
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
                if (System.IO.Directory.Exists(build.Location.Replace(Path, localMapFolder)) &&
                    System.IO.Directory.Exists(System.IO.Path.Combine(build.Location.Replace(Path, localMapFolder),"install")))
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
