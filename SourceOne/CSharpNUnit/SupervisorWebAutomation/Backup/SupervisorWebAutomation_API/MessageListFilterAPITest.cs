using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RestSharp;

using SupervisorWebAutomation_API.Config;
using SupervisorWebAutomation_API.Common;
using System.Xml.Linq;

namespace SupervisorWebAutomation_API
{
    [TestClass]
    public class MessageListFilterAPITest:BaseTest
    {
        XElement account = SupervisorWebConfig.Config.Root.Element("testdata").Element("account");

        [TestInitialize]
        public void Setup()
        {
            string username = account.Attribute("username").Value;
            string password = account.Attribute("password").Value;
            Login(username, password);
        }

        [TestMethod]
        public void TestMessageListFilter_GetDefaultMessagelistFilter()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            Assert.AreEqual(int.Parse(account.Element("defaultmessagefilter").Element("DBID").Value), defaultMessageListFilterResponse.Data.DBID, "DBID is not correct");

            Assert.AreEqual(int.Parse(account.Element("defaultmessagefilter").Element("NumDaysUnreviewed").Value), defaultMessageListFilterResponse.Data.NumDaysUnreviewed, "NumDaysUnreviewed is not correct");

            Assert.AreEqual(int.Parse(account.Element("defaultmessagefilter").Element("NumDaysRecentlyReviewed").Value), defaultMessageListFilterResponse.Data.NumDaysRecentlyReviewed, "NumDaysRecentlyReviewed is not correct");

            Assert.AreEqual(bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value), defaultMessageListFilterResponse.Data.UseArchiveDateRange, "UseArchiveDateRange is not correct");

            Assert.AreEqual(bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value), defaultMessageListFilterResponse.Data.IncludeEscalated, "IncludeEscalated is not correct");

            Assert.AreEqual(bool.Parse(account.Element("defaultmessagefilter").Element("IncludeAllRevGroups").Value), defaultMessageListFilterResponse.Data.IncludeAllRevGroups, "IncludeAllRevGroups is not correct");

            Assert.AreEqual(bool.Parse(account.Element("defaultmessagefilter").Element("RemoveWhenReviewComplete").Value), defaultMessageListFilterResponse.Data.RemoveWhenReviewComplete, "RemoveWhenReviewComplete is not correct");

            Assert.AreEqual(bool.Parse(account.Element("defaultmessagefilter").Element("IsDefaultFilter").Value), defaultMessageListFilterResponse.Data.IsDefaultFilter, "IsDefaultFilter is not correct");

            Assert.AreEqual(bool.Parse(account.Element("defaultmessagefilter").Element("CanBeChanged").Value), defaultMessageListFilterResponse.Data.CanBeChanged, "CanBeChanged is not correct");

            Assert.AreEqual(int.Parse(account.Element("defaultmessagefilter").Element("ReviewGroupsCount").Value), defaultMessageListFilterResponse.Data.ReviewGroupsOptions.Count, "Review Groups count is not correct");

            List<FilterSelectorOption> numDaysRecentlyReviewedOptions = defaultMessageListFilterResponse.Data.NumDaysRecentlyReviewedOptions;

            Assert.AreEqual(int.Parse(account.Element("defaultmessagefilter").Element("umDaysRecentlyReviewedOptionsCount").Value), numDaysRecentlyReviewedOptions.Count, "Count of recently reviewed Days options count is not correct");

            List<FilterSelectorOption> numDaysUnreviewedOptions = defaultMessageListFilterResponse.Data.NumDaysUnreviewedOptions;

            Assert.AreEqual(int.Parse(account.Element("defaultmessagefilter").Element("numDaysUnreviewedOptionsCount").Value), numDaysUnreviewedOptions.Count, "Count of unreviewed days is not correct");
        }

        [TestMethod]
        public void TestMessageListFilter_SaveMessagelistFilter_ChangeValidNumDaysUnreviewed()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            string numDaysUnreviewed = account.Element("changemessagefilter").Element("ValidNumDaysUnreviewed").Value;

            string[] numDaysUnreviewedList = numDaysUnreviewed.Split(new char[] { ',' });

            foreach (string unreviewedDay in numDaysUnreviewedList)
            {
                int unreviewedDayNum = int.Parse(unreviewedDay);

                IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now,
                    int.Parse(account.Element("defaultmessagefilter").Element("NumDaysRecentlyReviewed").Value), unreviewedDayNum,
                    bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value));

                Assert.IsTrue(SaveMessageListFilterResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list status code is not correct");

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

                Assert.AreEqual(unreviewedDayNum, defaultMessageListFilterResponse2.Data.NumDaysUnreviewed, "NumDaysUnreviewed is not correct");
            }

        }

        [TestMethod]
        public void TestMessageListFilter_SaveMessagelistFilter_ChangeInvalidNumDaysUnreviewed()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            string numDaysUnreviewed = account.Element("changemessagefilter").Element("InvalidNumDaysUnreviewed").Value;

            string[] numDaysUnreviewedList = numDaysUnreviewed.Split(new char[] { ',' });

            foreach (string unreviewedDay in numDaysUnreviewedList)
            {
                int unreviewedDayNum = int.Parse(unreviewedDay);

                IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now,
                    int.Parse(account.Element("defaultmessagefilter").Element("NumDaysRecentlyReviewed").Value), unreviewedDayNum,
                    bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value));

                Assert.IsTrue(SaveMessageListFilterResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list status code is not correct");

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

                Assert.AreEqual(unreviewedDayNum, defaultMessageListFilterResponse2.Data.NumDaysUnreviewed, "NumDaysUnreviewed is not correct");
            }

        }

        [TestMethod]
        public void TestMessageListFilter_SaveMessagelistFilter_ChangeValidNumDaysRecentlyReviewed()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            string numDaysRecentlyReviewed = account.Element("changemessagefilter").Element("ValidNumDaysRecentlyReviewed").Value;

            string[] numDaysRecentlyReviewedList = numDaysRecentlyReviewed.Split(new char[] { ',' });

            foreach (string recentlyReviewedDay in numDaysRecentlyReviewedList)
            {
                int recentlyReviewedDayNum = int.Parse(recentlyReviewedDay);

                IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now,
                    recentlyReviewedDayNum, int.Parse(account.Element("defaultmessagefilter").Element("NumDaysUnreviewed").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value));

                Assert.IsTrue(SaveMessageListFilterResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list status code is not correct");

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

                Assert.AreEqual(recentlyReviewedDayNum, defaultMessageListFilterResponse2.Data.NumDaysRecentlyReviewed, "NumDaysRecentlyReviewed is not correct");
            }

        }

        [TestMethod]
        public void TestMessageListFilter_SaveMessagelistFilter_ChangeInvalidNumDaysRecentlyReviewed()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            string numDaysRecentlyReviewed = account.Element("changemessagefilter").Element("InvalidNumDaysRecentlyReviewed").Value;

            string[] numDaysRecentlyReviewedList = numDaysRecentlyReviewed.Split(new char[] { ',' });

            foreach (string recentlyReviewedDay in numDaysRecentlyReviewedList)
            {
                int recentlyReviewedDayNum = int.Parse(recentlyReviewedDay);

                IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now,
                    recentlyReviewedDayNum, int.Parse(account.Element("defaultmessagefilter").Element("NumDaysUnreviewed").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value));

                Assert.IsTrue(SaveMessageListFilterResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list status code is not correct");

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

                Assert.AreEqual(recentlyReviewedDayNum, defaultMessageListFilterResponse2.Data.NumDaysRecentlyReviewed, "NumDaysRecentlyReviewed is not correct");
            }

            
        }

        [TestMethod]
        public void TestMessageListFilter_SaveMessagelistFilter_ChangeIncludeEscalated()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now,
                    int.Parse(account.Element("defaultmessagefilter").Element("NumDaysRecentlyReviewed").Value),
                    int.Parse(account.Element("defaultmessagefilter").Element("NumDaysUnreviewed").Value),
                    bool.Parse(account.Element("changemessagefilter").Element("IncludeEscalated").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value));

            Assert.IsTrue(SaveMessageListFilterResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list status code is not correct");

            IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

            Assert.AreEqual(bool.Parse(account.Element("changemessagefilter").Element("IncludeEscalated").Value), defaultMessageListFilterResponse2.Data.IncludeEscalated, "IncludeEscalated is not correct");
        }

        
        [TestMethod]
        public void TestMessageListFilter_SaveMessagelistFilter_ChangeValidArchiveDate()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            foreach (XElement validArchiveDatePair in account.Element("changemessagefilter").Element("ArchiveDate").Elements("ValidArchiveDate"))
            {
                string start = validArchiveDatePair.Element("start").Value;

                string end = validArchiveDatePair.Element("end").Value;

                DateTime startDate = ChangeStringToDate(start);

                DateTime endDate = ChangeStringToDate(end);

                IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, startDate, endDate,
                        int.Parse(account.Element("defaultmessagefilter").Element("NumDaysRecentlyReviewed").Value),
                        int.Parse(account.Element("defaultmessagefilter").Element("NumDaysUnreviewed").Value),
                        bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                        bool.Parse(account.Element("changemessagefilter").Element("UseArchiveDateRange").Value));

                Assert.IsTrue(SaveMessageListFilterResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list status code is not correct");

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

                Assert.AreEqual(bool.Parse(account.Element("changemessagefilter").Element("UseArchiveDateRange").Value), defaultMessageListFilterResponse2.Data.UseArchiveDateRange, "UseArchiveDateRange is not correct when start date is " + start + " and end date is " + end);

                TimeSpan startSpan = startDate - defaultMessageListFilterResponse2.Data.ArchiveDateStart;

                TimeSpan endSpan = endDate - defaultMessageListFilterResponse2.Data.ArchiveDateEnd;

                Assert.AreEqual(0, startSpan.Days, "ArchiveDateStart is not correct when start date is " + start + " and end date is " + end);

                Assert.AreEqual(0, endSpan.Days, "ArchiveDateEnd is not correct when start date is " + start + " and end date is " + end);
            }

        }

        [TestMethod]
        public void TestMessageListFilter_SaveMessagelistFilter_ChangeInvalidArchiveDate()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            foreach (XElement invalidArchiveDatePair in account.Element("changemessagefilter").Element("ArchiveDate").Elements("InvalidArchiveDate"))
            {
                string start = invalidArchiveDatePair.Element("start").Value;

                string end = invalidArchiveDatePair.Element("end").Value;

                DateTime startDate = ChangeStringToDate(start);

                DateTime endDate = ChangeStringToDate(end);

                IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, startDate, endDate,
                        int.Parse(account.Element("defaultmessagefilter").Element("NumDaysRecentlyReviewed").Value),
                        int.Parse(account.Element("defaultmessagefilter").Element("NumDaysUnreviewed").Value),
                        bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                        bool.Parse(account.Element("changemessagefilter").Element("UseArchiveDateRange").Value));

                Assert.IsTrue(SaveMessageListFilterResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError, "Save message list status code is not correct when " + invalidArchiveDatePair.Element("message").Value);

            }

        }

        [TestMethod]
        public void TestMessageListFilter_SaveMessagelistFilter_ChangeBPs()
        {
            //TBD
        }

        [TestCleanup]
        public void Teardown()
        {

            //restore to default
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now,
                    int.Parse(account.Element("defaultmessagefilter").Element("NumDaysRecentlyReviewed").Value),
                    int.Parse(account.Element("defaultmessagefilter").Element("NumDaysUnreviewed").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value));

            Assert.IsTrue(SaveMessageListFilterResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list filter status code is not correct");

            Logout();
        }
    }
}
