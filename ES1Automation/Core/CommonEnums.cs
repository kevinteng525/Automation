using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public enum Platform
    {
        All = 0,
        Exchange = 1,
        Domino = 2,
        Undefined = 8,
    }

    public enum Runtime
    {
        Undefined = 0,
        RubyMiniTest = 1,
        CSharpNUnit = 2,
        CSharpMSUnit,
    }

    public enum VCSType
    {
        TFS = 1,
        ClearCase = 2,
        ShareFolder = 3,
        Git = 4,
        NotSync = 5,
    }

    public class AgentType
    {
        public static string SaberAgent = "SaberAgent";//The agent(Windows Form Application) installed by Galaxy on windows environment
        public static string RemoteAgent = "RemoteAgent";//The agent on the SUT(system under test) or Test Agent which are Linux environment, the Saber Agent will call shell scripts to install the product or run our tests on Linux
    }

    public class AgentName
    {
        public static string WindowsFormApp = "SaberAgent.WindowsFormApp.exe";
    }

    public class LocalMappedFolder
    {
        public static string CommonBuildFolder = "W:";
        public static string BPSearchBuildFolder = "O:";
        public static string CISBuildFolder = "P:";
        public static string SupervisorBuildFolder = "Q:";
        public static string S1BuildFolder = "R:";
        public static string RevealBuildFolder = "S:";
        public static string ResultTempFolder  = "T:";
        public static string ResultTempFolder2 = "X:";
        public static string ZincBuildFolder = "Z:";
    }
}
