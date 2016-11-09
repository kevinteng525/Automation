using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System.Net;
using SupervisorWebAutomation_API.Config;
using SupervisorWebAutomation_API.Common;
using System.Xml.Linq;

namespace SupervisorWebAutomation_API
{
    [TestClass]
    public class AccountAPITest : BaseTest
    {
        public XElement account = SupervisorWebConfig.Config.Root.Element("testdata").Elements("account").ToList()[1];
        string domain;
        string username;
        string password;

        [TestInitialize]
        public void Setup()
        {
            client = new RestClient(SupervisorWebConfig.URL);

            client.CookieContainer = new System.Net.CookieContainer();

            ServicePointManager.ServerCertificateValidationCallback +=(sender, certificate, chain, sslPolicyErrors) => true;
            domain = account.Attribute("domain").Value;
            username = account.Attribute("username").Value;
            password = account.Attribute("password").Value;
            
        }

        [TestCategory("webid=5452")]
        [TestMethod]
        public void TestLogin_Positive()
        {
            string timer = "Positive Login";

            BeginTimer(timer);

            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = username, Password = password, RememberMe = true };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/"); 
         
            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsTrue(IsSupAuthSet(),"The .SUPAUTH should be set on cookies.");

            EndTimer(timer);
        }
        
        [TestCategory("webid=5470")]
        [TestMethod]
        public void TestLogin_Positive_WithDomain()
        {
            string timer = "Positive Login";

            BeginTimer(timer);

            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = domain + "\\" + username, Password = password, RememberMe = true };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsTrue(IsSupAuthSet(), "The .SUPAUTH should be set on cookies.");

            EndTimer(timer);
        }

        [TestCategory("webid=5471")]
        [TestMethod]
        public void TestLogin_Unauthorized()
        {

            XElement unauthorizedaccount = SupervisorWebConfig.Config.Root.Element("testdata").Element("unauthorizedaccount");

            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = unauthorizedaccount.Attribute("username").Value, Password = unauthorizedaccount.Attribute("password").Value, RememberMe = true };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsFalse(IsSupAuthSet(), "Unauthorized User should not login.");

        }

        [TestCategory("webid=5472")]
        [TestMethod]
        public void TestLogin_Supervisor()
        {

            XElement unauthorizedaccount = SupervisorWebConfig.Config.Root.Element("testdata").Element("accounthassuproleonly");

            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = unauthorizedaccount.Attribute("username").Value, Password = unauthorizedaccount.Attribute("password").Value, RememberMe = true };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsTrue(IsSupAuthSet(), "User has supervisor role only should login.");

        }

        [TestCategory("webid=5453")]
        [TestMethod]
        public void TestLogin_Wrong_Password()
        {
            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = username, Password = password + "invalid", RememberMe = true };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsFalse(IsSupAuthSet(), "User with invalid password can not login.");
        }

        [TestCategory("webid=5473")]
        [TestMethod]
        public void TestLogin_Empty_Password()
        {
            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = username, Password ="", RememberMe = true };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsFalse(IsSupAuthSet(), "User with empty password can not login.");
        }

        [TestCategory("webid=5454")]
        [TestMethod]
        public void TestLogin_Wrong_User()
        {
            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = username + "invalid", Password = password, RememberMe = true };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsFalse(IsSupAuthSet(), "User with invalid username can not login.");
        }

        [TestCategory("webid=5474")]
        [TestMethod]
        public void TestLogin_Empty_User()
        {
            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = "", Password = password, RememberMe = true };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsFalse(IsSupAuthSet(), "User with empty username can not login.");
        }

        [TestCategory("webid=5475")]
        [TestMethod]
        public void TestLogin_Empty_NameAndPassword()
        {
            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = "", Password = "", RememberMe = true };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsFalse(IsSupAuthSet(), "Empty name and password can not login.");
        }

        [TestCategory("webid=5476")]
        [TestMethod]
        public void TestLogin_Wrong_User_Retry5Times()
        {
            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = username + "invalid", Password = password, RememberMe = true };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            for (int i = 0; i < 6; i++)
            {

                IRestResponse response = client.Execute(request);

                Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

                Assert.IsFalse(IsSupAuthSet(), "User with invalid password can not login.");
            }

        }

        [TestCategory("webid=5455")]
        [TestMethod]
        public void TestLogout()
        {
            TestLogin_Positive();

            BeginTimer("Logout");

            var request = CreateNewReqest("Account/LogOff", Method.POST);

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsFalse(IsSupAuthSet(), "The .SUPAUTH is unset after logout.");

            EndTimer("Logout");
        }

        [TestCategory("webid=5456")]
        [TestMethod]
        public void TestLoginAjax_Positive()
        {
            TestLogin_Positive();

            var request = CreateNewReqest("Account/LoginAjax", Method.POST);

            Account account = new Account() { UserName = username, Password = password };

            request.AddHeader("X-Requested-With", "XMLHttpRequest");

            request.AddJsonBody(account);

            request.RequestFormat = DataFormat.Json;

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Response code is OK.");

            Assert.IsTrue(IsSupAuthSet(), "The .SUPAUTH should be set on cookies.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            string requestVerificationToken = GetVerificationToken("Reviewer");

            var request = new RestRequest("Account/LogOff", Method.POST);

            request.AddParameter("__RequestVerificationToken", requestVerificationToken);

            IRestResponse response = client.Execute(request);
        }

    }

}
