using System.Diagnostics;
using System.Xml.Linq;
using Common.FileCommon;
using System;
using System.Linq;

namespace VerifyLib.VerifyItems
{
    public class VersionItem : VerifyItem
    {
        private const string File = "FILE";

        private const string Directory = "DIRECTORY";

        public string Path;

        public string Type;

        public string VersionType;

        public VersionItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            Path = GetAttributeValue("path");

            Type = GetAttributeValue("type").ToUpper();

            if (Type != File && Type != Directory)
            {
                throw new Exception("Config Error: In VersionGroup, type should be file or directory");
            }

            VersionType = GetAttributeValue("versionType").ToUpper();

            if (VersionType != "PRODUCTVERSION" && VersionType != "FILEVERSION")
            {
                throw new Exception("Config Error: In DllGroup, versionType should be ProductVersion or FileVersion");
            }

            // If file to verify is exist
            if (Type == File)
            {
                var items = VerifyGroup.VerifyItems.OfType<VersionItem>()
                                       .Where
                                       (
                                           versionItem => versionItem.Path == Path
                                                       && versionItem.Type == File
                                                       && versionItem.VersionType == VersionType
                                                       && VerifyGroup.VerifyItems.IndexOf(versionItem) != VerifyGroup.VerifyItems.Count - 1
                                       );

                if (items.Count() > 0)
                {
                    VerifyGroup.VerifyItems.Remove(items.First());
                }
            }

            // Change Directory to files
            if (Type == Directory && FileHelper.IsExistsFolder(Path))
            {
                var files = FileHelper.GetAllFiles(Path).Where(f => f.Value.ToLower().EndsWith(".dll") || f.Value.ToLower().EndsWith(".exe"));

                foreach (var file in files)
                {
                    // skip the file if exist
                    if (! VerifyGroup.VerifyItems.OfType<VersionItem>()
                                    .Any
                                        (
                                            versionItem => versionItem.Path == file.Key 
                                                        && versionItem.Type == File 
                                                        && versionItem.VersionType == VersionType
                                        )
                                    )
                    {
                        var fileNode = new XElement("item");
                        fileNode.Add(new XAttribute("name", file.Value));
                        fileNode.Add(new XAttribute("path", file.Key));
                        fileNode.Add(new XAttribute("type", File));
                        fileNode.Add(new XAttribute("versionType", VersionType));
                        fileNode.Value = CheckValue;

                        VerifyGroup.GenerateVerifyItem(VerifyType.Version, fileNode);
                    }
                }

                VerifyGroup.VerifyItems.Remove(this);
            }
        }

        private void VerifyFile()
        {
            FileVersionInfo versionInfo = FileHelper.GetVersionFileInfo(Path);

            if (versionInfo != null)
            {
                switch (VersionType)
                {
                    case "PRODUCTVERSION":
                        ActualValue = versionInfo.ProductVersion == null ? string.Empty : versionInfo.ProductVersion.Trim();
                        break;

                    case "FILEVERSION":
                        ActualValue = versionInfo.FileVersion == null ? string.Empty : versionInfo.FileVersion.Trim();
                        break;

                    default:
                        VerifyResult = VerifyResult.Failed;
                        break;
                }
            }
            else
            {
                ActualValue = NotExist;
                VerifyResult = VerifyResult.Failed;
                Information = "NOT EXIST THIS ITEM";
            }

            VerifyResult = ActualValue == CheckValue ? VerifyResult.Pass : VerifyResult.Failed;

            if (VerifyResult == VerifyResult.Failed)
            {
                Information = Path;
            }
        }

        private void VeifyDirectory()
        {
            if (!FileHelper.IsExistsFolder(Path))
            {
                VerifyResult = VerifyResult.Failed;

                Information = string.Format("Directory: {0}  NOT EXIST", Path);
            }
            else
            {
                VerifyResult = VerifyResult.Pass;
            }
        }

        protected override void PrepareTest()
        {
            base.PrepareTest();

            switch (Type)
            {
                case File:
                    DisplayName = string.Format("{0} {1}", Name, VersionType);
                    break;

                case Directory:
                    DisplayName = string.Format("Directory: {0} -- Check {1}", Name, VersionType);
                    break;
            }
        }

        protected override void Verify()
        {
            switch (Type)
            {
                case File:
                    VerifyFile();
                    break;

                case Directory:
                    VeifyDirectory();
                    break;

                default:
                    VerifyResult = VerifyResult.Failed;
                    break;
            }
        }
    }
}
