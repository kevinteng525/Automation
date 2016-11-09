using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RequestLib.Requests
{
    public class ValidationRequest : Request
    {
        public const string LogFormat = "{0,-100}{1,-30}{2,-30}{3,-10}{4,-30}";

        public const string CSVFormat = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"";

        public string Version { get; set; }

        public ResultView View { get; set; }

        public string ValidationConfig { get; set; }

        // file path
        public string ProgramFilePathX86 { get; set; }

        public string ProgramDataPath { get; set; }

        // sql server
        public string SQLServerInstance { get; set; }

        public string SQLServerUsername { get; set; }

        public string SQLServerPassword { get; set; }

        public ValidationRequest(string server, int port)
            : base(server, port)
        {
            Type = "Validation";
        }

        protected override void SetParams()
        {
            base.SetParams();

            RequestXML.Add(new XAttribute("version", Version));
            RequestXML.Add(new XAttribute("view", View));

            RequestXML.Add(new XAttribute("programFilePathX86", ProgramFilePathX86));
            RequestXML.Add(new XAttribute("programDataPath", ProgramDataPath));

            RequestXML.Add(new XAttribute("sqlServerInstance", SQLServerInstance));
            RequestXML.Add(new XAttribute("sqlServerUsername", SQLServerUsername));
            RequestXML.Add(new XAttribute("sqlServerPassword", SQLServerPassword));

            RequestXML.Value = ValidationConfig;
        }

        public static List<ValidationGroup> ToValidationGroup(string responseString)
        {
            List<ValidationGroup> groups = new List<ValidationGroup>();

            XElement resultRoot = XElement.Parse(responseString);

            foreach (XElement groupNode in resultRoot.Elements("testGroup"))
            {
                ValidationGroup group = new ValidationGroup(groupNode.Attribute("name").Value);

                foreach (XElement itemNode in groupNode.Elements("result"))
                {
                    group.ValidationResults.Add
                        (
                            new ValidationResult
                            {
                                Name = itemNode.Attribute("displayName").Value,
                                ExpectValue = itemNode.Attribute("expectValue").Value,
                                ActualValue = itemNode.Attribute("actualValue").Value,
                                Information = itemNode.Attribute("information").Value,
                                VerifyResult = (VerifyResult)Enum.Parse(typeof(VerifyResult), itemNode.Attribute("verifyResult").Value, true)
                            }
                        );
                }

                groups.Add(group);
            }

            return groups;
        }

        public static string ToXML(string responseString)
        {
            return responseString;
        }

        public static string ToLog(string responseString)
        {
            List<ValidationGroup> groups = ToValidationGroup(responseString);

            var logConents = new List<string>
                {
                    "===============================================================================================================================",
                    "                                                 Validation Log",
                    "===============================================================================================================================",
                    string.Format("Validaion on: {0}", DateTime.Now),
                    string.Empty
                };

            foreach (var group in groups)
            {
                logConents.Add(string.Format("-----------------------------------------------{0}------------------------------------------------------", group.GroupName));

                logConents.Add(string.Format(LogFormat, "CHECK ITEM", "EXPECT", "ACTUAL", "RESULT", "INFORMATION"));

                foreach (var item in group.ValidationResults)
                {
                    logConents.Add(string.Format(LogFormat, item.Name, item.ExpectValue, item.ActualValue, item.VerifyResult, item.Information));
                }

                logConents.Add(string.Empty);
            }

            return string.Join(Environment.NewLine, logConents);
        }

        public static string ToCSV(string responseString)
        {
            List<ValidationGroup> groups = ToValidationGroup(responseString);

            var csvConents = new List<string>
                {
                    "Validation Log",
                    string.Format("Validation on: {0}", DateTime.Now),
                    string.Empty
                };

            foreach (var group in groups)
            {
                csvConents.Add(group.GroupName);

                csvConents.Add(string.Format(CSVFormat, "CHECK ITEM", "EXPECT", "ACTUAL", "RESULT", "INFORMATION"));

                foreach (var item in group.ValidationResults)
                {
                    csvConents.Add(string.Format(CSVFormat, item.Name, item.ExpectValue, item.ActualValue, item.VerifyResult, item.Information));
                }

                csvConents.Add(string.Empty);
            }

            return string.Join(Environment.NewLine, csvConents);
        }
    }

    public class ValidationGroup
    {
        public string GroupName { get; set; }

        public List<ValidationResult> ValidationResults { get; set; }

        public ValidationGroup(string name)
        {
            GroupName = name;

            ValidationResults = new List<ValidationResult>();
        }
    }

    public class ValidationResult
    {
        public string Name { get; set; }

        public string ExpectValue { get; set; }

        public string ActualValue { get; set; }

        public string Information { get; set; }

        public VerifyResult VerifyResult { get; set; }
    }
}
