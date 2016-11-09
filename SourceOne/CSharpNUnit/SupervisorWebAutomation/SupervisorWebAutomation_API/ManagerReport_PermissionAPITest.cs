using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RestSharp;
using SupervisorWebAutomation_API.Common;
using SupervisorWebAutomation_API.Config;

namespace SupervisorWebAutomation_API
{
    [TestClass]
    public class ManagerReportPermissionAPITest : BaseTest
    {
        [TestInitialize]
        public void Setup()
        {
            Account reviewer = SupervisorWebConfig.GetAccountByRole(AccountRole.Reviewer);
            Login(reviewer.UserName, reviewer.Password);
        }

        [TestCategory("webid=5479")]
        [TestMethod]
        public void ManagerReport_CallManagerReportAPIsWithoutPermission_Negative()
        {
            DateTime startDate = new DateTime(2015, 5, 1);
            DateTime endDate = new DateTime(2015, 6, 1);
            List<ReviewGroup> reviewGroups = new List<ReviewGroup>();

            //GetDefaultReportDates4Manager

            IRestResponse<ReportDateRange> getDefaultReportDates4ManagerResponse = APIGetDefaultReportDates4Manager();

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError, "Response code is not InternalServerError.");

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.Content.Contains("You do not have sufficient access rights to perform the current operation."), "The error message is not right.");

            //GetReportBusinessPolicies

            IRestResponse<ReportBusinessPolicies> getReportBusinessPoliciesResponse = APIGetReportBusinessPolicies(startDate, endDate);
                        
            Assert.IsTrue(getReportBusinessPoliciesResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError, "Response code is not InternalServerError.");

            Assert.IsTrue(getReportBusinessPoliciesResponse.Content.Contains("You do not have sufficient access rights to perform the current operation."), "The error message is not right.");
                        
            //GetReportReviewGroups4Manager

            IRestResponse<List<ReviewGroup>> getReportReviewGroups4ManagerResponse = APIGetReportReviewGroups4Manager(startDate, endDate);
            
            Assert.IsTrue(getReportReviewGroups4ManagerResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError, "Response code is not InternalServerError.");

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.Content.Contains("You do not have sufficient access rights to perform the current operation."), "The error message is not right.");
 
            //GetReportReviewGroupMembers4Manager

            IRestResponse<ReviewGroupMembers> getReportReviewGroupMembers4ManagerResponse = APIGetReportReviewGroupMembers4Manager(startDate, endDate, reviewGroups);

            Assert.IsTrue(getReportReviewGroupMembers4ManagerResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError, "Response code is not InternalServerError.");

            Assert.IsTrue(getReportReviewGroupMembers4ManagerResponse.Content.Contains("You do not have sufficient access rights to perform the current operation."), "The error message is not right.");
 
            //GetReportReviewGroupReviewers4Manager

            IRestResponse<ReviewGroupMembers> getReportReviewGroupReviewers4ManagerResponse = APIGetReportReviewGroupReviewers4Manager(startDate, endDate, reviewGroups);

            Assert.IsTrue(getReportReviewGroupReviewers4ManagerResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError, "Response code is not InternalServerError.");

            Assert.IsTrue(getReportReviewGroupReviewers4ManagerResponse.Content.Contains("You do not have sufficient access rights to perform the current operation."), "The error message is not right.");
 
            //GetReportSupervisoryCodes

            IRestResponse<ReportSupervisoryCodes> getReportSupervisoryCodesResponse = APIGetReportSupervisoryCodes(startDate, endDate, reviewGroups);

            Assert.IsTrue(getReportSupervisoryCodesResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError, "Response code is not InternalServerError.");

            Assert.IsTrue(getReportSupervisoryCodesResponse.Content.Contains("You do not have sufficient access rights to perform the current operation."), "The error message is not right.");
 
            //GenerateManagerReport

            for (int i = 1; i <= 8; i++)
            {
                ManagerReportType reportType = (ManagerReportType)i;

                IRestResponse<ReportResponse> generateManagerReportResponse = APIGenerateManagerReport(reportType, startDate, endDate, null, null, null, null, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

                Assert.IsTrue(generateManagerReportResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError, string.Format( "Response code is not InternalServerError when call generate manager [{0}] report.", reportType.ToString()));

                Assert.IsTrue(generateManagerReportResponse.Content.Contains("You do not have sufficient access rights to perform the current operation."), "The error message is not right.");
            }

        }

        [TestCleanup]
        public void Teardown()
        {
            Logout();
        }

    }

}
