using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace VerifyLib
{
    public class Verify
    {
        public readonly VerifyEnvironment VerifyEnvironment = new VerifyEnvironment();

        public List<VerifyGroup> VerifyGroups;

        private void GenerateVerifyGroups(XElement root)
        {
            VerifyGroups = new List<VerifyGroup>();

            foreach (var groupNode in root.Elements("Group"))
            {
                VerifyGroups.Add(new VerifyGroup(groupNode, VerifyEnvironment));
            }
        }

        public void DoVerify()
        {
            GenerateVerifyGroups(VerifyEnvironment.CreateConfigByEnvironment());

            foreach (var group in VerifyGroups)
            {
                group.Verify();
            }
        }

        public string ToXML()
        {
            var root = new XElement("TestResult");
            root.Add(new XAttribute("date", DateTime.Now));

            foreach (var group in VerifyGroups)
            {
                var testGroup = new XElement("testGroup");

                testGroup.Add(new XAttribute("name", group.GroupName));

                foreach (VerifyItem item in group.VerifyItems)
                {
                    switch (VerifyEnvironment.ResultView)
                    {
                        case ResultView.All:

                            testGroup.Add(item.ToXML());
                            break;

                        case ResultView.PassOnly:

                            if (item.VerifyResult == VerifyResult.Pass)
                            {
                                testGroup.Add(item.ToXML());
                            }
                            break;

                        case ResultView.FailOnly:

                            if (item.VerifyResult == VerifyResult.Failed)
                            {
                                testGroup.Add(item.ToXML());
                            }
                            break;
                    }
                }

                root.Add(testGroup);
            }

            return root.ToString();
        }
    }
}
