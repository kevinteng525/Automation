using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RestSharp;
using SupervisorWebAutomation_API.Common;
using SupervisorWebAutomation_API.Config;
using System.Xml.Linq;
using System.Net;

namespace SupervisorWebAutomation_API
{
    /// <summary>
    /// Summary description for HelpControllerAPITest
    /// </summary>
    [TestClass]
    public class HelpControllerAPITest : BaseTest
    {
        [TestInitialize]
        public void Setup()
        {
            
            
        }

        [TestMethod]
        public void VisitHelpPage_With_DifferentExts_WithLogin()
        {
            Account reviewer = SupervisorWebConfig.GetAccountByRole(AccountRole.Reviewer);
            Login(reviewer.UserName, reviewer.Password);
            XElement url = SupervisorWebConfig.Config.Root.Element("testdata").Element("help").Element("url");
            XElement extensions = SupervisorWebConfig.Config.Root.Element("testdata").Element("help").Element("helpExtensions");

            string[] extensionList = extensions.Value.Split(new char[] { ',' });

            foreach (string extension in extensionList)
            {
                var request = CreateNewReqest(url.Value + "test." + extension, Method.GET);
                IRestResponse response = client.Execute(request);
                Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode, "Response code is OK.");
            }
            Logout();
        }

        [TestMethod]
        public void VisitHelpPage_With_DifferentExts_WithoutLogin()
        {

            XElement url = SupervisorWebConfig.Config.Root.Element("testdata").Element("help").Element("url");
            XElement extensions = SupervisorWebConfig.Config.Root.Element("testdata").Element("help").Element("helpExtensions");

            string[] extensionList = extensions.Value.Split(new char[] { ',' });

            foreach (string extension in extensionList)
            {
                RestClient client = new RestClient(SupervisorWebConfig.URL);

                client.Timeout = 100000000;
                client.ReadWriteTimeout = 100000000;

                client.CookieContainer = new System.Net.CookieContainer();

                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                var request = CreateNewReqest(url.Value + "test." + extension, Method.GET);
                IRestResponse response = client.Execute(request);
                Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

                Assert.IsTrue(response.ResponseUri.OriginalString.Contains("Account/Login"), "It should redirect to login page.");
            }

        }

        [TestCleanup]
        public void Teardown()
        {
            
        }

    }
}
