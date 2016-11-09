using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace VerifyLib
{
    public class VerifyEnvironment
    {
        public string Version { get; private set; }

        public ResultView ResultView { get; set; }

        public XElement VerifyConfig { get; set; }

        // Files Path
        public string ProgramFilePathX86 { get; set; }

        public string AppDataPath { get; set; }

        public string ProgramDataPath { get; set; }

        // SQL Server
        public string SQLServerInstance { get; set; }

        public string SQLServerUsername { get; set; }

        public string SQLServerPassword { get; set; }

        // Version
        public string MajorVersion { get; private set; }

        public string MinorVersion { get; private set; }

        public string BuildVersion { get; private set; }

        public string RevisionVersion { get; private set; }

        // GAC
        public Dictionary<string, List<string>> GACAssemblyVersions { get; set; }

        public VerifyEnvironment()
        {
            ResultView = ResultView.All;
        }

        public void SetVersion(string version)
        {
            Version = version;

            string[] versionSplit = Version.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            if (versionSplit.Count() != 4)
            {
                throw new Exception("Version String Format Error!");
            }

            MajorVersion = versionSplit[0];

            MinorVersion = versionSplit[1];

            BuildVersion = versionSplit[2];

            RevisionVersion = versionSplit[3];
        }

        public XElement CreateConfigByEnvironment()
        {
            string config = VerifyConfig.ToString();

            // replace param in config file
            config = config.Replace("[ProgramFilePathX86]", ProgramFilePathX86);
            config = config.Replace("[ProgramDataPath]", ProgramDataPath);

            config = config.Replace("[MajorVersion]", MajorVersion);
            config = config.Replace("[MinorVersion]", MinorVersion);
            config = config.Replace("[BuildVersion]", BuildVersion);
            config = config.Replace("[RevisionVersion]", RevisionVersion);

            config = config.Replace("[SQLServerInstance]", SQLServerInstance);
            config = config.Replace("[SQLServerUsername]", SQLServerUsername);
            config = config.Replace("[SQLServerPassword]", SQLServerPassword);

            return XElement.Parse(config);
        }
    }
}
