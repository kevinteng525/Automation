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
    public class ReviewerReportAPITest:BaseTest
    {

        [TestInitialize]
        public void Setup()
        {
            Account reviewer = SupervisorWebConfig.GetAccountByRole(AccountRole.Reviewer);
            Login(reviewer.UserName, reviewer.Password);
        }

        [TestMethod]
        public void ReviewerReport_GroupActivityReport()
        {
            //GetDefaultReportDates

            IRestResponse<ReportDateRange> getDefaultReportDatesResponse = APIGetDefaultReportDates();

            Assert.IsTrue(getDefaultReportDatesResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDatesResponse.Data.To > getDefaultReportDatesResponse.Data.From, "Default report date is not correct.");

            //GetReportReviewGroups

            IRestResponse<List<ReviewGroup>> getReportReviewGroupsResponse = APIGetReportReviewGroups(getDefaultReportDatesResponse.Data.From, getDefaultReportDatesResponse.Data.To);

            Assert.IsTrue(getReportReviewGroupsResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code of Get Review Groups should be OK.");

            Assert.IsTrue(getReportReviewGroupsResponse.Data.Count > 0, "There should be at least review groups in system.");

            //GenerateReviewerReport

            IRestResponse<ReportResponse> generateReviewerReportResponse = APIGenerateReviewerReport(ManagerReportType.GroupActivity, getDefaultReportDatesResponse.Data.From, getDefaultReportDatesResponse.Data.To, null, null, getReportReviewGroupsResponse.Data, null, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateReviewerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Reviewer Report is not OK.");

            Assert.IsTrue(generateReviewerReportResponse.Data.DataId.Length > 0, "The report data Id of Generate Reviewer Report should not be empty.");
            
            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateReviewerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor Group Activity Report"), "The report tile should be Email Supervisor Group Activity Report");
        
        }

        [TestMethod]
        public void ReviewerReport_MemberActivityReport()
        {
            //GetDefaultReportDates

            IRestResponse<ReportDateRange> getDefaultReportDatesResponse = APIGetDefaultReportDates();

            Assert.IsTrue(getDefaultReportDatesResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDatesResponse.Data.To > getDefaultReportDatesResponse.Data.From, "Default report date is not correct.");

            //GetReportReviewGroups

            IRestResponse<List<ReviewGroup>> getReportReviewGroupsResponse = APIGetReportReviewGroups(getDefaultReportDatesResponse.Data.From, getDefaultReportDatesResponse.Data.To);

            Assert.IsTrue(getReportReviewGroupsResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code of Get Review Groups should be OK.");

            Assert.IsTrue(getReportReviewGroupsResponse.Data.Count > 0, "There should be at least review groups in system.");

            //GetReportReviewGroupMembers

            IRestResponse<ReviewGroupMembers> getReportReviewGroupMembersResponse = APIGetReportReviewGroupMembers(getDefaultReportDatesResponse.Data.From, getDefaultReportDatesResponse.Data.To, getReportReviewGroupsResponse.Data);

            Assert.IsTrue(getReportReviewGroupMembersResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Get Review Group Members is not OK.");

            Assert.IsTrue(getReportReviewGroupMembersResponse.Data.Members.Count > 0, "There should be at least one member in the system.");

            List<Member> groupMembers = new List<Member>();

            foreach (Member m in getReportReviewGroupMembersResponse.Data.Members)
            {
                m.IsDeleted = false;
                m.Selected = true;
                groupMembers.Add(m);
            }

            if (null != getReportReviewGroupMembersResponse.Data.DeletedMembers)
            {
                foreach (Member m in getReportReviewGroupMembersResponse.Data.DeletedMembers)
                {
                    m.IsDeleted = true;
                    m.Selected = true;
                    groupMembers.Add(m);
                }
            }

            //GenerateReviewerReport

            IRestResponse<ReportResponse> generateReviewerReportResponse = APIGenerateReviewerReport(ManagerReportType.MemberActivity, getDefaultReportDatesResponse.Data.From, getDefaultReportDatesResponse.Data.To, null, groupMembers, getReportReviewGroupsResponse.Data, null, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateReviewerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Reviewer Report is not OK.");

            Assert.IsTrue(generateReviewerReportResponse.Data.DataId.Length > 0, "The report data Id should not be empty.");            

            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateReviewerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor Member Activity Report"), "The report tile should be Email Supervisor Member Activity Report");
        
        }

        [TestMethod]
        public void ReviewerReport_ReviewerActivityReport()
        {
            //GetDefaultReportDates

            IRestResponse<ReportDateRange> getDefaultReportDatesResponse = APIGetDefaultReportDates();

            Assert.IsTrue(getDefaultReportDatesResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDatesResponse.Data.To > getDefaultReportDatesResponse.Data.From, "Default report date is not correct.");
                        
            //GenerateReviewerReport

            IRestResponse<ReportResponse> generateReviewerReportResponse = APIGenerateReviewerReport(ManagerReportType.ReviewerActivity, getDefaultReportDatesResponse.Data.From, getDefaultReportDatesResponse.Data.To, null, null, null, null, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateReviewerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Reviewer Report is not OK.");

            Assert.IsTrue(generateReviewerReportResponse.Data.DataId.Length > 0, "The response of Generate Reviewer Report is empty.");

            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateReviewerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor Reviewer Activity Report"), "The report tile should be Email Supervisor Reviewer Activity Report");
       
        }

        [TestCleanup]
        public void Teardown()
        {
            Logout();
        }
    }
}
