using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

using SupervisorWebAutomation_API.Config;
using SupervisorWebAutomation_API.Common;

namespace SupervisorWebAutomation_API
{
    [TestClass]
    public class RestSharp_Demo
    {
        IRestClient client = null;

        public RestSharp_Demo()
        {

        }

        [TestInitialize]
        public void Setup()
        {
            client = new RestClient(SupervisorWebConfig.URL);

            client.CookieContainer = new System.Net.CookieContainer();
        }

        [TestMethod]
        public void TestRestSharp_Request()
        {
            //json
            var request = new RestRequest("Account/Login", Method.POST);

            Account account = new Account() { UserName = SupervisorWebConfig.User, Password = SupervisorWebConfig.Password };

            request.AddJsonBody(account);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            IRestResponse response = client.Execute(request);




            request = new RestRequest("Account/Login", Method.POST);

            request.AddObject(account);

            request.RequestFormat = DataFormat.Json;

            request.AddQueryParameter("ReturnUrl", @"/SupervisorWeb/");

            response = client.Execute(request);


            request = new RestRequest("Account/Login", Method.POST);

            request.RequestFormat = DataFormat.Json;

            request.AddObject(account);

            request.AddQueryParameter("ReturnUrl", @"/SupervisorWeb/");

            response = client.Execute(request);


            request = new RestRequest("Account/Login", Method.POST);

            request.AddBody(account);

            request.RequestFormat = DataFormat.Json;

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            response = client.Execute(request);


            request = new RestRequest("Account/Login", Method.POST);

            request.RequestFormat = DataFormat.Json;

            request.AddBody(account);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            response = client.Execute(request);


            request = new RestRequest("Account/Login", Method.GET);

            request.RequestFormat = DataFormat.Json;

            request.AddBody(account);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            response = client.Execute(request);

        }


        [TestMethod]
        public void TestRestSharp_Response()
        {
            var request = new RestRequest("Account/Login", Method.POST);

            Account account = new Account() { UserName = SupervisorWebConfig.User, Password = SupervisorWebConfig.Password };

            request.AddJsonBody(account);

            request.RequestFormat = DataFormat.Json;

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            IRestResponse response = client.Execute(request);

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK, "Response code is OK.");

            //Assert.IsTrue(IsSupAuthSet(), "The .SUPAUTH should be set on cookies.");
        }


        [TestCleanup]
        public void Cleanup()
        {
            var request = new RestRequest("Account/LogOff", Method.POST);

            IRestResponse response = client.Execute(request);
        }

    }

}
