using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

using SupervisorWebAutomation_API.Config;
using SupervisorWebAutomation_API.Common;
using RestSharp;

namespace SupervisorWebAutomation_API
{
    [TestClass]
    public class MessageListAPITest : BaseTest
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
        public void TestMessageList_PrepareMessageList_ChangeNumDaysUnreviewed()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            string numDaysUnreviewed = account.Element("changemessagefilter").Element("ValidNumDaysUnreviewed").Value;

            string numDaysUnreviewedItems = account.Element("changemessagefilter").Element("NumDaysUnreviewedItems").Value;

            string[] numDaysUnreviewedList = numDaysUnreviewed.Split(new char[] { ',' });

            string[] numDaysUnreviewedItemsList = numDaysUnreviewedItems.Split(new char[] { ',' });

            for(int i =0;i<numDaysUnreviewedList.Count();i++)
            {
                int unreviewedDayNum = int.Parse(numDaysUnreviewedList[i]);

                int unreviewedDayNumItems = int.Parse(numDaysUnreviewedItemsList[i]);

                IRestResponse<PrepareMessageList> prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now,
                    -1, unreviewedDayNum,
                    bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value));

                Assert.IsTrue(prepareMessageListResponse.StatusCode == System.Net.HttpStatusCode.OK, "Prepare message list status code is not correct");

                int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

                Assert.AreEqual(unreviewedDayNumItems, totalMessageCount, "Total message count is not correct when NumDaysUnreviewed is " + unreviewedDayNum );
            }

        }

        [TestMethod]
        public void TestMessageList_PrepareMessageList_ChangeNumDaysRecentlyReviewed()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            string numDaysRecentlyReviewed = account.Element("changemessagefilter").Element("ValidNumDaysRecentlyReviewed").Value;

            string numDaysRecentlyReviewedItems = account.Element("changemessagefilter").Element("NumDaysRecentlyReviewedItems").Value;

            string[] numDaysRecentlyReviewedList = numDaysRecentlyReviewed.Split(new char[] { ',' });

            string[] numDaysRecentlyReviewedItemsList = numDaysRecentlyReviewedItems.Split(new char[] { ',' });

            for (int i = 0; i < numDaysRecentlyReviewedList.Count(); i++)
            {
                int recentlyReviewedDayNum = int.Parse(numDaysRecentlyReviewedList[i]);

                int recentlyReviewedDayNumItems = int.Parse(numDaysRecentlyReviewedItemsList[i]);

                IRestResponse<PrepareMessageList> prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now,
                    recentlyReviewedDayNum, -1,
                    bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value));

                Assert.IsTrue(prepareMessageListResponse.StatusCode == System.Net.HttpStatusCode.OK, "Prepare message list status code is not correct");

                int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

                Assert.AreEqual(recentlyReviewedDayNumItems, totalMessageCount, "Total message count is not correct when NumDaysRecentlyReviewed is " + recentlyReviewedDayNum);
            }

        }

        [TestMethod]
        public void TestMessageListFilter_PrepareMessageList_ChangeIncludeEscalated()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            IRestResponse<PrepareMessageList> prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now,
                    int.Parse(account.Element("defaultmessagefilter").Element("NumDaysRecentlyReviewed").Value),
                    int.Parse(account.Element("defaultmessagefilter").Element("NumDaysUnreviewed").Value),
                    bool.Parse(account.Element("changemessagefilter").Element("IncludeEscalated").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value));

            Assert.IsTrue(prepareMessageListResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list status code is not correct");

            int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

            string expectedCount = account.Element("changemessagefilter").Element("IncludeEscalated").Attribute("itemcount").Value;

            Assert.AreEqual(int.Parse(expectedCount), totalMessageCount, "Total message count is not correct");
        }

        [TestMethod]
        public void TestMessageListFilter_PrepareMessageList_ChangeValidArchiveDate()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            foreach (XElement validArchiveDatePair in account.Element("changemessagefilter").Element("ArchiveDate").Elements("ValidArchiveDate"))
            {
                string start = validArchiveDatePair.Element("start").Value;

                string end = validArchiveDatePair.Element("end").Value;

                string itemcount = validArchiveDatePair.Element("itemcount").Value;

                DateTime startDate = ChangeStringToDate(start);

                DateTime endDate = ChangeStringToDate(end);

                IRestResponse<PrepareMessageList> prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, startDate, endDate,
                        int.Parse(account.Element("defaultmessagefilter").Element("NumDaysRecentlyReviewed").Value),
                        int.Parse(account.Element("defaultmessagefilter").Element("NumDaysUnreviewed").Value),
                        bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                        bool.Parse(account.Element("changemessagefilter").Element("UseArchiveDateRange").Value));

                Assert.IsTrue(prepareMessageListResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list status code is not correct");

                int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

                Assert.AreEqual(int.Parse(itemcount), totalMessageCount, "Total message count is not correct when start date is " +start+" and end date is "+end);
            }

        }

        [TestMethod]
        public void TestMessageListFilter_PrepareMessageList_ChangeBPs()
        {
            //Get Default Message list Filter
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();
            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            string allReviewGroups = "";
            foreach (XElement reviewGroupPair in account.Element("changemessagefilter").Element("ReviewGroup").Elements("Group"))
            {
                string groupValue = reviewGroupPair.Element("ID").Value;
                allReviewGroups = allReviewGroups + groupValue + ",";
                string[] BPList = reviewGroupPair.Element("BP").Value.Split(new char[]{','});
                //change review group, then save filter settings
                IRestResponse prepareMessageListResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now, groupValue, 0, 0, true, false, false);

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

                IRestResponse<PrepareMessageList> prepareMessageListResponse2 = APIPrepareMessageList(defaultMessageListFilterResponse2.Data, DateTime.Now, DateTime.Now);
                                                           
                List<BusinessPolicyFlag> businessPolicyFlag = prepareMessageListResponse2.Data.bpFlags;
                //get all BPs, the GetAllBusinessPolicies API has been removed by Dev
               //IRestResponse<List<BusinessPolicyFlag>> getAllBPResponse = APIGetAllBusinessPolicies(defaultMessageListFilterResponse2.Data);

                Assert.AreEqual(BPList.Length, businessPolicyFlag.Count, "BP list count is not correct");

                //compare BP name
                for (int i = 0; i < BPList.Length; i++)
                {
                    Assert.AreEqual(BPList[i], businessPolicyFlag[i].Name, "BP Name is not correct");
                }
       
            }

            allReviewGroups = allReviewGroups.TrimEnd(new char[] { ',' });

            IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now,allReviewGroups,
                    int.Parse(account.Element("defaultmessagefilter").Element("NumDaysRecentlyReviewed").Value),
                    int.Parse(account.Element("defaultmessagefilter").Element("NumDaysUnreviewed").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("IncludeEscalated").Value),
                    bool.Parse(account.Element("defaultmessagefilter").Element("UseArchiveDateRange").Value));
         
       }

        [TestMethod]
        public void TestMessageListFilter_PrepareMessageList_SortColumns()
        {
            //Get Default Message list Filter
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            //Prepare message list, prerequist for GetAppBPs

            IRestResponse<PrepareMessageList> prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now);

            //Get Flagged Message by page
            int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

            string vaildPages = account.Element("VaildPages").Value;

            string[] validPageList = vaildPages.Split(new char[] { ',' });

            string vaildPagesReturnCount = account.Element("VaildPagesReturnCount").Value;

            string[] validPageReturnCountList = vaildPagesReturnCount.Split(new char[] { ',' });

            for (int i = 0; i < validPageList.Count(); i++)
            {
                string range = validPageList[i];
                totalMessageCount = int.Parse(validPageReturnCountList[i]);
                string sortCon = GetRandSortCondition();
                IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(range, totalMessageCount, sortCon, true);

                Assert.IsTrue(getFlaggedMessagesByPageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get Flagged Messages By Page status code is not correct: " + getFlaggedMessagesByPageResponse.StatusCode.ToString());

                Assert.IsTrue(getFlaggedMessagesByPageResponse.Data.Count == totalMessageCount, "Total count of sort column by " + sortCon + " is not correct.");
            }
        }

        [TestMethod]
        public void TestMessageListFilter_GetAllBP()
        {
            //Get Default Message list Filter
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            //Prepare message list, prerequist for GetAppBPs

            IRestResponse<PrepareMessageList> prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now);

            //Get all business policies


            List<BusinessPolicyFlag> allBPList = prepareMessageListResponse.Data.bpFlags;

            Assert.IsTrue(prepareMessageListResponse.StatusCode == System.Net.HttpStatusCode.OK, "Prepare message list status code is not correct");

            Assert.AreEqual(int.Parse(account.Element("BP").Attribute("count").Value), allBPList.Count, "BP count is not correct");

        }

        [TestMethod]
        public void TestMessageList_GetValidFlaggedMessagesByPage()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            //Prepare message list

            IRestResponse<PrepareMessageList> prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now);

            int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

            //Get flagged messages by page
            string vaildPages = account.Element("VaildPages").Value;

            string[] validPageList = vaildPages.Split(new char[] { ',' });

            foreach (string validPage in validPageList)
            {

                IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(validPage, totalMessageCount);

                Assert.IsTrue(getFlaggedMessagesByPageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message list By Page status code is not correct");

                Assert.AreEqual(string.Format("items {0}/{1}", validPage, totalMessageCount), GetHeader(getFlaggedMessagesByPageResponse, "Content-Range"), "Content range of get message list by page is not correct");

            }
        }

        [TestMethod]
        public void TestMessageList_GetInvalidFlaggedMessagesByPage()
        {
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            //Prepare message list

            IRestResponse<PrepareMessageList> prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now);

            int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

            //Get flagged messages by page
            string invaildPages = account.Element("InvaildPages").Value;

            string[] invalidPageList = invaildPages.Split(new char[] { ',' });

            foreach (string invalidPage in invalidPageList)
            {

                IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(invalidPage, totalMessageCount);

                Assert.IsTrue((getFlaggedMessagesByPageResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)||(getFlaggedMessagesByPageResponse.StatusCode == 0), "Get message list By Page status code is not correct when page is " + invalidPage);

                //Assert.AreEqual(string.Format("items {0}/{1}", invalidPage, totalMessageCount), GetHeader(getFlaggedMessagesByPageResponse, "Content-Range"), "Content range of get message list by page is not correct");

            }
        }

        [TestCleanup]
        public void Teardown()
        {
            Logout();
        }
    }
}
