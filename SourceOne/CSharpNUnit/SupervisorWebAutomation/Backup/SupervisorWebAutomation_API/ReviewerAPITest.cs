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
    public class ReviewerAPITest : BaseTest
    {
        [TestInitialize]
        public void Setup()
        {
            string username = SupervisorWebConfig.Config.Root.Element("testdata").Element("account").Attribute("username").Value;
            string password = SupervisorWebConfig.Config.Root.Element("testdata").Element("account").Attribute("password").Value;
            Login(username,password);
        }

        [TestMethod]
        public void TestReviewer_Workflow()
        {
            //Get Default Message list Filter
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Response code is OK.");

            Assert.AreEqual(2, defaultMessageListFilterResponse.Data.DBID, "DBID is not correct");

            Assert.AreEqual(0, defaultMessageListFilterResponse.Data.NumDaysUnreviewed, "NumDaysUnreviewed is not correct");

            Assert.AreEqual(0, defaultMessageListFilterResponse.Data.NumDaysRecentlyReviewed, "NumDaysRecentlyReviewed is not correct");

            Assert.AreEqual(false, defaultMessageListFilterResponse.Data.UseArchiveDateRange, "UseArchiveDateRange is not correct");

            Assert.AreEqual(false, defaultMessageListFilterResponse.Data.IncludeEscalated, "IncludeEscalated is not correct");

            Assert.AreEqual(true, defaultMessageListFilterResponse.Data.IncludeAllRevGroups, "IncludeAllRevGroups is not correct");

            Assert.AreEqual(false, defaultMessageListFilterResponse.Data.RemoveWhenReviewComplete, "RemoveWhenReviewComplete is not correct");

            Assert.AreEqual(true, defaultMessageListFilterResponse.Data.IsDefaultFilter, "IsDefaultFilter is not correct");

            Assert.AreEqual(true, defaultMessageListFilterResponse.Data.CanBeChanged, "CanBeChanged is not correct");

            Assert.AreEqual(15, defaultMessageListFilterResponse.Data.ReviewGroupsOptions.Count, "Review Groups count is not correct");

            List<FilterSelectorOption> numDaysRecentlyReviewedOptions = defaultMessageListFilterResponse.Data.NumDaysRecentlyReviewedOptions;

            Assert.AreEqual(6, numDaysRecentlyReviewedOptions.Count, "Count of recently reviewed Days options count is not correct");

            List<FilterSelectorOption> numDaysUnreviewedOptions = defaultMessageListFilterResponse.Data.NumDaysUnreviewedOptions;

            Assert.AreEqual(6, numDaysUnreviewedOptions.Count, "Count of unreviewed days is not correct");

            //Save filter
            DateTime archiveStartDate = new DateTime(2015, 5, 3);

            DateTime archiveEndDate = new DateTime(2015, 6, 3);

            //Change message filter
            IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, archiveStartDate, archiveEndDate, 5, 15, true, true);

            Assert.IsTrue(SaveMessageListFilterResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list status code is not correct");

            //Validate the change takes effect
            IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

            Assert.AreEqual(15, defaultMessageListFilterResponse.Data.NumDaysUnreviewed, "NumDaysUnreviewed is not correct");

            Assert.AreEqual(5, defaultMessageListFilterResponse.Data.NumDaysRecentlyReviewed, "NumDaysRecentlyReviewed is not correct");

            Assert.AreEqual(true, defaultMessageListFilterResponse.Data.UseArchiveDateRange, "UseArchiveDateRange is not correct");

            Assert.AreEqual(true, defaultMessageListFilterResponse.Data.IncludeEscalated, "IncludeEscalated is not correct");

            //restore the default message list
            IRestResponse SaveMessageListFilterResponse2 = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now);

            Assert.IsTrue(SaveMessageListFilterResponse2.StatusCode == System.Net.HttpStatusCode.OK, "Save message list filter status code is not correct");

            IRestResponse<MessageListFilter> defaultMessageListFilterResponse3 = APIGetDefaultMessageListFilter();

            //validate default message list is restored successfully
            Assert.AreEqual(0, defaultMessageListFilterResponse.Data.NumDaysUnreviewed, "NumDaysUnreviewed is not correct");

            Assert.AreEqual(0, defaultMessageListFilterResponse.Data.NumDaysRecentlyReviewed, "NumDaysRecentlyReviewed is not correct");

            Assert.AreEqual(false, defaultMessageListFilterResponse.Data.UseArchiveDateRange, "UseArchiveDateRange is not correct");

            Assert.AreEqual(false, defaultMessageListFilterResponse.Data.IncludeEscalated, "IncludeEscalated is not correct");

            //Prepare message list

            IRestResponse<PrepareMessageList> prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, archiveStartDate, archiveEndDate, 5, 15, true, true);

            Assert.IsTrue(prepareMessageListResponse.StatusCode == System.Net.HttpStatusCode.OK, "Prepare message list status code is not correct");

            int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

            Assert.AreEqual(0, totalMessageCount, "Total message count is not correct");

            IRestResponse<PrepareMessageList> prepareMessageListResponse2 = APIPrepareMessageList(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now);

            Assert.IsTrue(prepareMessageListResponse2.StatusCode == System.Net.HttpStatusCode.OK, "Prepare message list status code is not correct");

            totalMessageCount = prepareMessageListResponse2.Data.TotalCount;

            Assert.AreEqual(1873, totalMessageCount, "Total message count is not correct");

            //Get flagged messages by page

            IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage("0-99", totalMessageCount);

            Assert.IsTrue(getFlaggedMessagesByPageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message list By Page status code is not correct");

            Assert.IsTrue(GetHeader(getFlaggedMessagesByPageResponse, "Content-Range") == string.Format("items {0}/{1}", "0-99", totalMessageCount), "Content range of get message list by page is not correct");

            List<FlaggedMessage> flaggedMessagesList = getFlaggedMessagesByPageResponse.Data;

            //Get all business policies

            IRestResponse<PrepareMessageList> prepareMessageListResponse3 = APIPrepareMessageList(defaultMessageListFilterResponse.Data, archiveStartDate, archiveEndDate);

            List<BusinessPolicyFlag> allBPs = prepareMessageListResponse3.Data.bpFlags;

            Assert.AreEqual(39, allBPs.Count, "BP count is not correct");

            //Message preview

            FlaggedMessage message = flaggedMessagesList[0];

            //Get message preview

            string sRIDs = GetSRIDstring(message);

            IRestResponse<MessagePreview> getMessagePreviewResponse = APIGetMessagePreview(message.MessageId, sRIDs);

            Assert.IsTrue(getMessagePreviewResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message preview status code is not correct");

            Assert.AreEqual(message.MessageId, ulong.Parse(getMessagePreviewResponse.Data.MessageId.ToString()), "Message ID of get message preview is not correct");

            Assert.AreEqual(message.Subject, getMessagePreviewResponse.Data.Subject, "Message subject of get message preview is not correct");

            //Get message preview HTML

            IRestResponse getMessagePreviewHtmlResponse = APIGetMessagePreviewHtml(message.MessageId, 0, sRIDs);

            Assert.IsTrue(getMessagePreviewHtmlResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message preview html status code is not correct");

            Assert.IsTrue(getMessagePreviewHtmlResponse.Content.Length > 0, "Get message preview html content length is 0");

            Assert.IsTrue(getMessagePreviewHtmlResponse.Content.Contains("Subject"), "Get message preview html content is not correct!");

            //Get message history

            IRestResponse<MessageHistory> getMessageHistoryResponse = APIGetMessageHistory(message);

            Assert.IsTrue(getMessageHistoryResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message history status code is not correct");

            Assert.IsTrue(getMessageHistoryResponse.Data.Actions.Count > 0, "Message history is 0");

            //Download Message

            IRestResponse downloadMessageResponse = APIDownloadMessage(message, 0);

            Assert.IsTrue(downloadMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Download message status code is not correct");

            Assert.IsTrue(downloadMessageResponse.Content.Length > 0, "Message content is empty");

            string emailFile = SaveResponseAsFile(downloadMessageResponse);

            Assert.IsTrue(emailFile != string.Empty, "Message file name is empty");

            Assert.IsTrue(System.IO.File.Exists(emailFile), "Cannot save the message file");

            //Comment Single Message

            IRestResponse commentMessageResponse = APICommentMessages((int)message.MessageId, "Single comment added by Automation");

            Assert.IsTrue(commentMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Comment Message status code is not correct");

            Assert.IsTrue(GetLatestCommentBy(message, CommentType.Description).Contains("Single comment added by Automation"), "Comment Message failed");


            //Get Incoming Messages

            IRestResponse getIncomingMessageCountResponse = APIGetIncomingMessagesCount(defaultMessageListFilterResponse.Data);

            Assert.IsTrue(getIncomingMessageCountResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get Incoming Messages status code is not correct");

            JsonObject result = SimpleJson.DeserializeObject<JsonObject>(getIncomingMessageCountResponse.Content);

            Assert.IsTrue(result["count"].ToString() == "0", "Incoming Messages Count is not correct.");

            //Code Flagged Messages

            List<FlaggedMessage> messageList = new List<FlaggedMessage>();

            messageList.Add(message);

            IRestResponse<Result> codeFlaggedMessageResponse = APICodeFlaggedMessage(messageList, CodeID.Reviewed, 0,"Mark as Reviewed by Automation", defaultMessageListFilterResponse.Data);

            Assert.IsTrue(codeFlaggedMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Flag Messages status code is not correct");

            Assert.IsTrue(GetLatestCommentBy(message, CommentType.Description).Contains("Mark as Reviewed by Automation"), "Flag Messages failed");


            //Batch Comment Messages

            List<string> messageIds = new List<string>();

            messageIds.AddRange(new string[] { message.MessageId.ToString(), "2", "3", "4", "5" });

            IRestResponse batchCommentMessagesResponse = APIBatchCommentMessages("Batch Comment Messages by Automation", messageIds);

            Assert.IsTrue(batchCommentMessagesResponse.StatusCode == System.Net.HttpStatusCode.OK, "Batch Comment Messages status code is not correct");

            JsonObject result1 = SimpleJson.DeserializeObject<JsonObject>(batchCommentMessagesResponse.Content);

            Assert.IsTrue(result1["Count"].ToString() == "5", "Batch Comment Messages Count is not correct.");

            Assert.IsTrue(GetLatestCommentBy(message, CommentType.Description).Contains("Batch Comment Messages by Automation"), "Comment Message failed");

            //Comment All Messages

            IRestResponse commentAllMessageResponse = APICommentAllFlaggedMessages("All comment added by Automation");

            Assert.IsTrue(commentAllMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Comment All Message status code is not correct");

            string latestcomment = GetLatestCommentBy(message, CommentType.Description);

            Assert.IsTrue(latestcomment.Contains("All comment added by Automation"), "Comment All Message failed");

            //GetONMInstaller

            IRestRequest request15 = new RestRequest("Reviewer/GetONMInstaller", Method.GET);

            IRestResponse response15 = client.Execute(request15);

            Assert.IsTrue(response15.StatusCode == System.Net.HttpStatusCode.OK, "error message here");

            string onmInstaller = SaveResponseAsFile(response15);

            //Assert.IsTrue(response15.Data.Count > 0, "error message here");
        }

        [TestCleanup]
        public void Teardown()
        {
            Logout();
        }
    }
}
