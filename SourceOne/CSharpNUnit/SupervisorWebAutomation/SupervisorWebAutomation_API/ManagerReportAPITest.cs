using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RestSharp;
using SupervisorWebAutomation_API.Common;
using SupervisorWebAutomation_API.Config;
using System.Xml.Linq;

namespace SupervisorWebAutomation_API
{
    [TestClass]
    public class ManagerReportAPITest : BaseTest
    {
        int DateFrom = int.Parse(SupervisorWebConfig.Config.Root.Element("testdata").Element("account").Element("ReportDateRange").Element("From").Value);

        [TestInitialize]
        public void Setup()
        {
            Account supervisor = SupervisorWebConfig.GetAccountByRole(AccountRole.Supervisor);
            Login(supervisor.UserName, supervisor.Password);
        }

        [TestCategory("webid=5480")]
        [TestMethod]
        public void ManagerReport_ConfigurationReport()
        {
            //GetDefaultReportDates4Manager

            IRestResponse<ReportDateRange> getDefaultReportDates4ManagerResponse = APIGetDefaultReportDates4Manager();

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.Data.To > getDefaultReportDates4ManagerResponse.Data.From, "Default report date is not correct.");

            //GetReportReviewGroups4Manager

            getDefaultReportDates4ManagerResponse.Data.From = getDefaultReportDates4ManagerResponse.Data.From.AddYears(DateFrom);

            IRestResponse<List<ReviewGroup>> getReportReviewGroups4ManagerResponse = APIGetReportReviewGroups4Manager(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To);

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code of Get Review Groups should be OK.");

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.Data.Count > 0, "There should be at least review groups in system.");

            //GenerateManagerReport

            IRestResponse<ReportResponse> generateManagerReportResponse = APIGenerateManagerReport(ManagerReportType.Configuration, getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, null, null, getReportReviewGroups4ManagerResponse.Data, null, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateManagerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Manager Report is not OK.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Length > 0, "The response of Generate Manager Report is empty.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Contains("Email Supervisor Configuration Report"), "The report tile should be Email Supervisor Configuration Report");
       
            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateManagerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor Configuration Report"), "The report tile should be Email Supervisor Configuration Report");
        }

        [TestCategory("webid=5481")]
        [TestMethod]
        public void ManagerReport_MessageDetailReport()
        {
            //GetDefaultReportDates4Manager

            IRestResponse<ReportDateRange> getDefaultReportDates4ManagerResponse = APIGetDefaultReportDates4Manager();

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.Data.To > getDefaultReportDates4ManagerResponse.Data.From, "Default report date is not correct.");

            //GetReportReviewGroups4Manager

            getDefaultReportDates4ManagerResponse.Data.From = getDefaultReportDates4ManagerResponse.Data.From.AddYears(DateFrom);

            IRestResponse<List<ReviewGroup>> getReportReviewGroups4ManagerResponse = APIGetReportReviewGroups4Manager(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To);

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code of Get Review Groups should be OK.");

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.Data.Count > 0, "There should be at least review groups in system.");

            //GetReportSupervisoryCodes

            IRestResponse<ReportSupervisoryCodes> getReportSupervisoryCodesResponse = APIGetReportSupervisoryCodes(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, getReportReviewGroups4ManagerResponse.Data);

            Assert.IsTrue(getReportSupervisoryCodesResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Get Report Supervisory Codes should be OK.");

            Assert.IsTrue(getReportSupervisoryCodesResponse.Data.Codes.Count >= 1, "There should be more than 1 available supervisory codes");

            //GenerateManagerReport

            List<SupervisoryCode> supervisoryCodes = new List<SupervisoryCode>();

            foreach (SupervisoryCode code in getReportSupervisoryCodesResponse.Data.Codes)
            {
                code.Selected = true;
                supervisoryCodes.Add(code);
            }

            foreach (SupervisoryCode code in getReportSupervisoryCodesResponse.Data.DeletedCodes)
            {
                code.Selected = true;
                supervisoryCodes.Add(code);
            }

            IRestResponse<ReportResponse> generateManagerReportResponse = APIGenerateManagerReport(ManagerReportType.MessageDetail, getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, null, null, getReportReviewGroups4ManagerResponse.Data, null, supervisoryCodes, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateManagerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Manager Report is not OK.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Length > 0, "The response of Generate Manager Report is empty.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Contains("Email Supervisor Message Detail Report"), "The report tile should be Email Supervisor Message Detail Report");

            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateManagerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor Message Detail Report"), "The report tile should be Email Supervisor Message Detail Report");
        }

        [TestCategory("webid=5482")]
        [TestMethod]
        public void ManagerReport_BusinessPolicyActivityReport()
        {
            //GetDefaultReportDates4Manager

            IRestResponse<ReportDateRange> getDefaultReportDates4ManagerResponse = APIGetDefaultReportDates4Manager();

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.Data.To > getDefaultReportDates4ManagerResponse.Data.From, "Default report date is not correct.");

            //GetReportBusinessPolicies

            getDefaultReportDates4ManagerResponse.Data.From = getDefaultReportDates4ManagerResponse.Data.From.AddYears(DateFrom);

            IRestResponse<ReportBusinessPolicies> getReportBusinessPoliciesResponse = APIGetReportBusinessPolicies(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To);

            Assert.IsTrue(getReportBusinessPoliciesResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response status code of Get Report Business Policies is not OK.");

            Assert.IsTrue(getReportBusinessPoliciesResponse.Data.BusinessPolicies.Count > 0, "There should be at least business policies in system.");

            List<BusinessPolicy> bps = new List<BusinessPolicy>();

            foreach (BusinessPolicy bp in getReportBusinessPoliciesResponse.Data.BusinessPolicies)
            {
                bp.IsDeleted = false;
                bp.Selected = true;
                bps.Add(bp);
            }
            foreach (BusinessPolicy bp in getReportBusinessPoliciesResponse.Data.DeletedBusinessPolicies)
            {
                bp.IsDeleted = true;
                bp.Selected = true;
                bps.Add(bp);
            }

            //GetReportReviewGroups4Manager

            IRestResponse<List<ReviewGroup>> getReportReviewGroups4ManagerResponse = APIGetReportReviewGroups4Manager(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To,bps);

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code of Get Review Groups should be OK.");

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.Data.Count > 0, "There should be at least review groups in system.");

            //GenerateManagerReport

            IRestResponse<ReportResponse> generateManagerReportResponse = APIGenerateManagerReport(ManagerReportType.BusinessPolicyActivity, getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, bps, null, getReportReviewGroups4ManagerResponse.Data, null, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateManagerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Manager Report is not OK.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Length > 0, "The response of Generate Manager Report is empty.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Contains("Email Supervisor Business Policy Activity Report"), "The title of the report should be Email Supervisor Business Policy Activity Report");

            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateManagerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor Business Policy Activity Report"), "The report tile should be Email Supervisor Business Policy Activity Report");
        }

        [TestCategory("webid=5483")]
        [TestMethod]
        public void ManagerReport_GroupActivityReport()
        {
            //GetDefaultReportDates4Manager

            IRestResponse<ReportDateRange> getDefaultReportDates4ManagerResponse = APIGetDefaultReportDates4Manager();

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.Data.To > getDefaultReportDates4ManagerResponse.Data.From, "Default report date is not correct.");

            //GetReportReviewGroups4Manager

            getDefaultReportDates4ManagerResponse.Data.From = getDefaultReportDates4ManagerResponse.Data.From.AddYears(DateFrom);

            IRestResponse<List<ReviewGroup>> getReportReviewGroups4ManagerResponse = APIGetReportReviewGroups4Manager(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To);

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code of Get Review Groups should be OK.");

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.Data.Count > 0, "There should be at least review groups in system.");

            //GenerateManagerReport

            IRestResponse<ReportResponse> generateManagerReportResponse = APIGenerateManagerReport(ManagerReportType.GroupActivity, getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, null, null, getReportReviewGroups4ManagerResponse.Data, null, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateManagerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Manager Report is not OK.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Length > 0, "The response of Generate Manager Report is empty.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Contains("Email Supervisor Group Activity Report"), "The title of the report should be Email Supervisor Group Activity Report");

            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateManagerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor Group Activity Report"), "The report tile should be Email Supervisor Group Activity Report");
        }

        [TestCategory("webid=5484")]
        [TestMethod]
        public void ManagerReport_MemberActivityReport()
        {
            //GetDefaultReportDates4Manager

            IRestResponse<ReportDateRange> getDefaultReportDates4ManagerResponse = APIGetDefaultReportDates4Manager();

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.Data.To > getDefaultReportDates4ManagerResponse.Data.From, "Default report date is not correct.");

            //GetReportReviewGroups4Manager

            getDefaultReportDates4ManagerResponse.Data.From = getDefaultReportDates4ManagerResponse.Data.From.AddYears(DateFrom);

            IRestResponse<List<ReviewGroup>> getReportReviewGroups4ManagerResponse = APIGetReportReviewGroups4Manager(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To);

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code of Get Review Groups should be OK.");

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.Data.Count > 0, "There should be at least review groups in system.");

            //GetReportReviewGroupMembers4Manager

            IRestResponse<ReviewGroupMembers> getReportReviewGroupMembers4ManagerResponse = APIGetReportReviewGroupMembers4Manager(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, getReportReviewGroups4ManagerResponse.Data);

            Assert.IsTrue(getReportReviewGroupMembers4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Get Review Group Members is not OK.");

            Assert.IsTrue(getReportReviewGroupMembers4ManagerResponse.Data.Members.Count > 0, "There should be at least one member in the system.");

            List<Member> groupMembers = new List<Member>();

            foreach (Member m in getReportReviewGroupMembers4ManagerResponse.Data.Members)
            {
                m.IsDeleted = false;
                m.Selected = true;
                groupMembers.Add(m);
            }

            foreach (Member m in getReportReviewGroupMembers4ManagerResponse.Data.DeletedMembers)
            {
                m.IsDeleted = true;
                m.Selected = true;
                groupMembers.Add(m);
            }

            //GenerateManagerReport

            IRestResponse<ReportResponse> generateManagerReportResponse = APIGenerateManagerReport(ManagerReportType.MemberActivity, getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, null, groupMembers, getReportReviewGroups4ManagerResponse.Data, null, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateManagerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Manager Report is not OK.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Length > 0, "The response of Generate Manager Report is empty.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Contains("Email Supervisor Member Activity Report"), "The title of the report should be Email Supervisor Member Activity Report");

            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateManagerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor Member Activity Report"), "The report tile should be Email Supervisor Member Activity Report");
        }

        [TestCategory("webid=5485")]
        [TestMethod]
        public void ManagerReport_ReviewerActivityReport()
        {
            //GetDefaultReportDates4Manager

            IRestResponse<ReportDateRange> getDefaultReportDates4ManagerResponse = APIGetDefaultReportDates4Manager();

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.Data.To > getDefaultReportDates4ManagerResponse.Data.From, "Default report date is not correct.");

            //GetReportReviewGroups4Manager

            getDefaultReportDates4ManagerResponse.Data.From = getDefaultReportDates4ManagerResponse.Data.From.AddYears(DateFrom);

            IRestResponse<List<ReviewGroup>> getReportReviewGroups4ManagerResponse = APIGetReportReviewGroups4Manager(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To);

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code of Get Review Groups should be OK.");

            Assert.IsTrue(getReportReviewGroups4ManagerResponse.Data.Count > 0, "There should be at least review groups in system.");

            //GetReportReviewGroupReviewers4Manager

            IRestResponse<ReviewGroupMembers> getReportReviewGroupReviewers4ManagerResponse = APIGetReportReviewGroupReviewers4Manager(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, getReportReviewGroups4ManagerResponse.Data);

            Assert.IsTrue(getReportReviewGroupReviewers4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Get Review Group Reviewer is not OK.");

            Assert.IsTrue(getReportReviewGroupReviewers4ManagerResponse.Data.Members.Count > 0, "There should be at least one member in the system.");

            List<Member> groupReviewers = new List<Member>();

            foreach (Member m in getReportReviewGroupReviewers4ManagerResponse.Data.Members)
            {
                m.IsDeleted = false;
                m.Selected = true;
                groupReviewers.Add(m);
            }
            foreach (Member m in getReportReviewGroupReviewers4ManagerResponse.Data.DeletedMembers)
            {
                m.IsDeleted = true;
                m.Selected = true;
                groupReviewers.Add(m);
            }

            //GenerateManagerReport

            IRestResponse<ReportResponse> generateManagerReportResponse = APIGenerateManagerReport(ManagerReportType.ReviewerActivity, getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, null, null, getReportReviewGroups4ManagerResponse.Data, groupReviewers, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateManagerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Manager Report is not OK.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Length > 0, "The response of Generate Manager Report is empty.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Contains("Email Supervisor Reviewer Activity Report"), "The title of the report should be Email Supervisor Reviewer Activity Report");

            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateManagerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor Reviewer Activity Report"), "The report tile should be Email Supervisor Reviewer Activity Report");
        }

        [TestCategory("webid=5486")]
        [TestMethod]
        public void ManagerReport_QueryActivityReport()
        {
            //GetDefaultReportDates4Manager

            IRestResponse<ReportDateRange> getDefaultReportDates4ManagerResponse = APIGetDefaultReportDates4Manager();

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.Data.To > getDefaultReportDates4ManagerResponse.Data.From, "Default report date is not correct.");

            //GetReportBusinessPolicies

            getDefaultReportDates4ManagerResponse.Data.From = getDefaultReportDates4ManagerResponse.Data.From.AddYears(DateFrom);

            IRestResponse<ReportBusinessPolicies> getReportBusinessPoliciesResponse = APIGetReportBusinessPolicies(getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To);

            Assert.IsTrue(getReportBusinessPoliciesResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response status code of Get Report Business Policies is not OK.");

            Assert.IsTrue(getReportBusinessPoliciesResponse.Data.BusinessPolicies.Count > 0, "There should be at least business policies in system.");

            List<BusinessPolicy> bps = new List<BusinessPolicy>();

            foreach (BusinessPolicy bp in getReportBusinessPoliciesResponse.Data.BusinessPolicies)
            {
                bp.IsDeleted = false;
                bp.Selected = true;
                bps.Add(bp);
            }

            foreach (BusinessPolicy bp in getReportBusinessPoliciesResponse.Data.DeletedBusinessPolicies)
            {
                bp.IsDeleted = true;
                bp.Selected = true;
                bps.Add(bp);
            }

            //GenerateManagerReport

            IRestResponse<ReportResponse> generateManagerReportResponse = APIGenerateManagerReport(ManagerReportType.QueryActivity, getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, bps, null, null, null, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateManagerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Manager Report is not OK.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Length > 0, "The response of Generate Manager Report is empty.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Contains("Email Supervisor Query Activity Report"), "The title of the report should be Email Supervisor Query Activity Report");

            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateManagerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor Query Activity Report"), "The report tile should be Email Supervisor Query Activity Report");
        }

        [TestCategory("webid=5487")]
        [TestMethod]
        public void ManagerReport_LicenseAvailabilityReport()
        {
            //GetDefaultReportDates4Manager

            IRestResponse<ReportDateRange> getDefaultReportDates4ManagerResponse = APIGetDefaultReportDates4Manager();

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.StatusCode == System.Net.HttpStatusCode.OK, "Response code is not OK.");

            Assert.IsTrue(getDefaultReportDates4ManagerResponse.Data.To > getDefaultReportDates4ManagerResponse.Data.From, "Default report date is not correct.");

            //GenerateManagerReport

            getDefaultReportDates4ManagerResponse.Data.From = getDefaultReportDates4ManagerResponse.Data.From.AddYears(DateFrom);

            IRestResponse<ReportResponse> generateManagerReportResponse = APIGenerateManagerReport(ManagerReportType.LicenseAvailability, getDefaultReportDates4ManagerResponse.Data.From, getDefaultReportDates4ManagerResponse.Data.To, null, null, null, null, null, DetailLevel.AllDetails, ColumnOrder.BusinessPolicyFirst);

            Assert.IsTrue(generateManagerReportResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Generate Manager Report is not OK.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Length > 0, "The response of Generate Manager Report is empty.");

            Assert.IsTrue(generateManagerReportResponse.Data.Content.Contains("Email Supervisor License Availability Report"), "The title of the report should be Email Supervisor License Availability Report");

            //GetReportData

            IRestResponse getReportDataResponse = APIGetReportData(generateManagerReportResponse.Data.DataId);

            Assert.IsTrue(getReportDataResponse.StatusCode == System.Net.HttpStatusCode.OK, "The response status of Data should be OK.");

            Assert.IsTrue(getReportDataResponse.Content.Contains("Email Supervisor License Availability Report"), "The report tile should be Email Supervisor License Availability Report");
        }

        [TestCleanup]
        public void Teardown()
        {
            Logout();
        }
    }
}
