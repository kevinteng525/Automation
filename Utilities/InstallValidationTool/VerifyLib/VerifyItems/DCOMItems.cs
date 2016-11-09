using System;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Common.RegistryCommon;
using Microsoft.Win32;
using Common.ScriptCommon;

namespace VerifyLib.VerifyItems
{
    public class DCOMItem : VerifyItem
    {
        private const string DefaultPermission = "DefaultPermission";

        private const string CustomizePermission = "CustomizePermission";

        public string ExpectedLaunchPermission;

        public Guid AppID;

        public string ExpectedName;

        //public RegistryKey RootApp = RegistryEdit.GetClassRootKey("AppID");

        public DCOMItem(XElement node, VerifyGroup group)
            : base(node, group)
        {
            AppID = new Guid(GetAttributeValue("appID"));

            ExpectedLaunchPermission = GetAttributeValue("launchPermission");

            ExpectedName = GetAttributeValue("name");

        }

        protected override void PrepareTest()
        {
            base.PrepareTest();

            //ExpectValue = ExpectedLaunchPermission;

            DisplayName = string.Format("{0} - launchPermission:{1}", Name, ExpectedLaunchPermission);
        }

        protected override void Verify()
        {
            RegistryKey appID = RegistryEdit.GetClassRootKey(string.Format(@"AppID\{{{0}}}", AppID));

            string defaultName = RegistryEdit.ReadHKCR(string.Format(@"AppID\{{{0}}}", AppID), string.Empty);
            defaultName = defaultName == null ? string.Empty : defaultName.TrimStart('{').TrimEnd('}');

            string launchPermission = RegistryEdit.ReadHKCR(string.Format(@"AppID\{{{0}}}", AppID), "LaunchPermission");
            launchPermission = launchPermission == null ? null : launchPermission.TrimStart('{').TrimEnd('}');
            
            if (appID == null)
            {
                ExpectValue = "appID not on this machine";

                ActualValue = "Not available";

                Information = "This row is skipped.";

                VerifyResult = VerifyResult.Skip;

            }
            else
            {   
                if (AppID != null && launchPermission != null)
                {

                    ExpectValue = ExpectedLaunchPermission;

                    ActualValue = CustomizePermission;                   
                    
                    // string output = CMDScript.DCOMPerm("-al {0093DFF5-231C-4809-A026-D4FE388C24F3} list"); //test
                    string output = CMDScript.DCOMPerm(string.Format(@"-al {{{0}}} list", AppID));

                    output = Regex.Replace(output, @"Access permitted to NT AUTHORITY\\SYSTEM.","");
                    output = Regex.Replace(output, @"Access permitted to BUILTIN\\Administrators.","");
                    output = Regex.Replace(output, @"Access permitted to NT AUTHORITY\\INTERACTIVE.","");
                                   
                    Information = output;

                    //Information = defaultName + "  LaunchPermission = " + launchPermission.ToString() + " " + AppID + "  " + output;
                }
                else
                {
                    ExpectValue = ExpectedLaunchPermission;

                    ActualValue = DefaultPermission;

                    string output = CMDScript.DCOMPerm(string.Format(@"-al {{{0}}} list", AppID));

                    Information = output;
                }

                VerifyResult = ExpectValue == ActualValue && ExpectedName == defaultName ? VerifyResult.Pass : VerifyResult.Failed;
            }
            
        }
    }
}
