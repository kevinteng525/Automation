using System;
using System.Xml.Linq;
using S1ValidationWinService.Maintains;
using VerifyLib;

namespace S1ValidationWinService.NetTransmission
{
    public abstract class Server
    {
        protected string GetAttributeValue(XElement requestRoot, string attrName)
        {
            XAttribute attr = requestRoot.Attribute(attrName);

            if (attr != null)
            {
                return attr.Value;
            }

            throw new Exception(string.Format("Config Error: attribute:{0} is not find !", attrName));
        }

        protected string GetAttributeValueOptional(XElement requestRoot, string attrName)
        {
            XAttribute attr = requestRoot.Attribute(attrName);

            if (attr != null)
            {
                return attr.Value;
            }

            return string.Empty;
        }

        #region Request Methods

        protected string InvokeRequest(string request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request))
                {
                    new Exception("Empty Request");
                }

                var requestRoot = XElement.Parse(request);

                switch (GetAttributeValue(requestRoot, "type").ToUpper())
                {
                    case "STATUS":
                        return InvokeServerStatus(requestRoot);

                    case "VALIDATION":
                        return InvokeValidation(requestRoot);

                    case "CMD":
                        return InvokeCMD(requestRoot);

                    case "PS":
                        return InvokePS(requestRoot);

                    default:
                        return "UNKNOW REQUEST TYPE";
                }
            }
            catch (Exception ex)
            {
                EventLogger.LogError(ex.Message);

                EventLogger.LogError(request);

                return string.Format("ERROR: {0}", ex.Message);
            }
        }

        protected string InvokeServerStatus(XElement requestRoot)
        {
            return Machine.GetStatus();
        }

        protected string InvokeValidation(XElement request)
        {
            var verify = new Verify();

            verify.VerifyEnvironment.SetVersion(GetAttributeValue(request, "version"));
            verify.VerifyEnvironment.ResultView = (ResultView)Enum.Parse(typeof(ResultView), GetAttributeValue(request, "view").Trim(), true);

            verify.VerifyEnvironment.ProgramFilePathX86 = GetAttributeValueOptional(request, "programFilePathX86");
            verify.VerifyEnvironment.ProgramDataPath = GetAttributeValueOptional(request, "programDataPath");

            verify.VerifyEnvironment.SQLServerInstance = GetAttributeValueOptional(request, "sqlServerInstance");
            verify.VerifyEnvironment.SQLServerUsername = GetAttributeValueOptional(request, "sqlServerUsername");
            verify.VerifyEnvironment.SQLServerPassword = GetAttributeValueOptional(request, "sqlServerPassword");

            verify.VerifyEnvironment.VerifyConfig = XElement.Parse(request.Value);

            verify.DoVerify();

            return verify.ToXML();
        }

        protected string InvokeCMD(XElement requestRoot)
        {
            string fileName = GetAttributeValue(requestRoot, "filename");

            string cmdArgs = requestRoot.Value;

            string domain = GetAttributeValueOptional(requestRoot, "domain");

            string username = GetAttributeValueOptional(requestRoot, "username");

            string password = GetAttributeValueOptional(requestRoot, "password");

            if (string.IsNullOrWhiteSpace(username))
            {
                return Machine.RunCMDSript(fileName, cmdArgs);
            }

            return Machine.RunCMDSript(fileName, cmdArgs, domain, username, password);
        }

        protected string InvokePS(XElement requestRoot)
        {
            string script = requestRoot.Value;

            return Machine.RunPSSript(script);
        }

        #endregion

        public abstract void StartListener();

        public abstract void StopListener();
    }
}
