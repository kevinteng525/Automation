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
    public class CodeFlaggedMessageAPITest : BaseTest
    {
        public XElement account = SupervisorWebConfig.Config.Root.Element("testdata").Elements("account").ToList()[1];

        [TestInitialize]
        public void Setup()
        {
            string username = account.Attribute("username").Value;
            string password = account.Attribute("password").Value;
            Login(username, password);
        }

        [TestCategory("webid=5457")]
        [TestMethod]
        public void CodeMessages()
        {
            for (int j = 0; j < account.Element("CodeFlaggedMessage").Elements("messages").ToList().Count; j++ )
            {
                List<XElement> messagesFromXML;
                
                messagesFromXML = account.Element("CodeFlaggedMessage").Elements("messages").ToList();

                #region Get filter information from xml

                string reviewGroups = messagesFromXML[j].Attribute("reviewgroup").Value;

                bool includeAllRevGroups = bool.Parse(messagesFromXML[j].Attribute("includeallrevgroups").Value);

                Dictionary<string, VerifyMessage> messages = collectMessagesFromXML(messagesFromXML[j].Elements("message"));

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

                Dictionary<string, string> rgMapResult = RGMap(defaultMessageListFilterResponse.Content);

                List<long> reviewGroupList = new List<long>();

                reviewGroupList = reviewGroupsList(reviewGroups, rgMapResult);

                #endregion

                IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now, reviewGroupList, 0, 0, false, false, includeAllRevGroups);

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();
                
                IRestResponse prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse2.Data, new DateTime(), new DateTime(), 0, 0, true, false);

                #region Find the messages need to verify from the message list

                JsonObject totalCount = SimpleJson.DeserializeObject < JsonObject > (prepareMessageListResponse.Content);

                object count;
                
                totalCount.TryGetValue("totalCount", out count);
                
                int totalMessageCount = int.Parse(count.ToString());

                Dictionary<string, string> BPMapResult = new Dictionary<string, string>();

                BPMapResult = BPMap(prepareMessageListResponse.Content);

                List<FlaggedMessage> flaggedMessagesList = new List<FlaggedMessage>();

                for (int i = 0, step = 100; i < totalMessageCount; i += step)
                {
                    int messageCountPerRequest = 0;

                    if (i + step < totalMessageCount)
                    {
                        messageCountPerRequest = step;
                    }
                    else
                    {
                        messageCountPerRequest = totalMessageCount - i;
                    }

                    IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(string.Format("{0}-{1}", i, i + messageCountPerRequest - 1), messageCountPerRequest);

                    foreach (FlaggedMessage message in getFlaggedMessagesByPageResponse.Data)
                    {
                        if (messages.ContainsKey(message.MessageId.ToString()))
                        {
                            flaggedMessagesList.Add(message);
                        }
                    }
                }

                Assert.IsNotNull(flaggedMessagesList, "Can not find the message in the message list");

                #endregion

                IRestResponse<Result> codeFlaggedMessageResponse = APICodeFlaggedMessage(flaggedMessagesList, messages, BPMapResult, defaultMessageListFilterResponse2.Data);

                Assert.IsTrue(codeFlaggedMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Flag Messages status code is not correct");

                #region Verify the message status and history

                Dictionary<string, string> resultMap = messageStatus(codeFlaggedMessageResponse.Content);

                Assert.IsNotNull(resultMap, "Not return the message status");

                foreach (FlaggedMessage message in flaggedMessagesList)
                {
                    string tempStatus = null;

                    VerifyMessage vMessage = null;

                    if (resultMap.ContainsKey(message.MessageId.ToString())) 
                    {
                        resultMap.TryGetValue(message.MessageId.ToString(), out tempStatus);

                        messages.TryGetValue(message.MessageId.ToString(), out vMessage);

                        Assert.AreEqual(vMessage.status, tempStatus, "[" + vMessage.id + vMessage.subject + "] Flag message failed, the message status is incorrect");

                        HistoryType history = new HistoryType();

                        history = GetLatestCommentBy(message);

                        string reviewerName = account.Attribute("reviewername").Value;

                        Assert.AreEqual(history.BPName, vMessage.bp, "[" + vMessage.id + vMessage.subject + "] Flag message failed, the BP name is incorrect");

                        Assert.IsTrue(history.Description.Contains(vMessage.comment), "[" + vMessage.id + vMessage.subject + "] Flag messages failed, the comment is incorrect");

                        Assert.AreEqual(history.ReviewerName, reviewerName, "[" + vMessage.id + vMessage.subject + "] Flag message failed, the reviewer name is incorrect");
                    }
                    else
                    {
                        tempStatus = "Blank";

                        messages.TryGetValue(message.MessageId.ToString(), out vMessage);

                        Assert.AreEqual(vMessage.status, tempStatus, "[" + vMessage.id + vMessage.subject + "] The message status is incorrect");

                        HistoryType history = new HistoryType();

                        history = GetLatestCommentBy(message);

                        Assert.IsFalse(history.Description.Contains(vMessage.comment), "[" + vMessage.id + vMessage.subject + "] Flag messages failed, the comment is incorrect");
                        
                        //IRestResponse<MessageHistory> getMessageHistoryResponse = APIGetMessageHistory(message);

                        //JsonObject history = SimpleJson.DeserializeObject<JsonObject>(getMessageHistoryResponse.Content);

                        //object actions;

                        //history.TryGetValue("Actions", out actions);

                        //JsonArray actionsArray = (JsonArray)actions;

                        //Assert.IsTrue(actionsArray.Count == 0, "[" + vMessage.id + vMessage.subject + "] Flag message failed, the message history should be null");
                    }
                }
                #endregion
            }
        }

        [TestMethod]
        public void CodeMessages_CodeForAllBPs()
        {
            for (int j = 0; j < account.Element("CodeFlaggedMessage").Elements("codeforallbps").ToList().Count; j++)
            {
                List<XElement> messagesFromXML;

                messagesFromXML = account.Element("CodeFlaggedMessage").Elements("codeforallbps").ToList();

                #region Get filter information from xml

                string reviewGroups = messagesFromXML[j].Attribute("reviewgroup").Value;

                bool includeAllRevGroups = bool.Parse(messagesFromXML[j].Attribute("includeallrevgroups").Value);

                Dictionary<string, VerifyMessage> messages = collectMessagesFromXML(messagesFromXML[j].Elements("message"));

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

                Dictionary<string, string> rgMapResult = RGMap(defaultMessageListFilterResponse.Content);

                List<long> reviewGroupList = new List<long>();

                reviewGroupList = reviewGroupsList(reviewGroups, rgMapResult);

                #endregion

                IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now, reviewGroupList, 0, 0, false, false, includeAllRevGroups);

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

                IRestResponse prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse2.Data, new DateTime(), new DateTime(), 0, 0, true, false);

                #region Find the messages need to verify from the message list

                JsonObject totalCount = SimpleJson.DeserializeObject<JsonObject>(prepareMessageListResponse.Content);

                object count;

                totalCount.TryGetValue("totalCount", out count);

                int totalMessageCount = int.Parse(count.ToString());

                Dictionary<string, string> BPMapResult = new Dictionary<string, string>();

                BPMapResult = BPMap(prepareMessageListResponse.Content);

                List<FlaggedMessage> flaggedMessagesList = new List<FlaggedMessage>();

                for (int i = 0, step = 100; i < totalMessageCount; i += step)
                {
                    int messageCountPerRequest = 0;

                    if (i + step < totalMessageCount)
                    {
                        messageCountPerRequest = step;
                    }
                    else
                    {
                        messageCountPerRequest = totalMessageCount - i;
                    }

                    IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(string.Format("{0}-{1}", i, i + messageCountPerRequest - 1), messageCountPerRequest);

                    foreach (FlaggedMessage message in getFlaggedMessagesByPageResponse.Data)
                    {
                        if (messages.ContainsKey(message.MessageId.ToString()))
                        {
                            flaggedMessagesList.Add(message);
                        }
                    }
                }

                Assert.IsNotNull(flaggedMessagesList, "Can not find the message in the message list");

                #endregion

                IRestResponse<Result> codeFlaggedMessageResponse = APICodeFlaggedMessage(flaggedMessagesList, messages, defaultMessageListFilterResponse2.Data);

                Assert.IsTrue(codeFlaggedMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Flag Messages status code is not correct");

                #region Verify the message status and history

                Dictionary<string, string> resultMap = messageStatus(codeFlaggedMessageResponse.Content);

                Assert.IsNotNull(resultMap, "Not return the message status");

                foreach (FlaggedMessage message in flaggedMessagesList)
                {
                    string tempStatus = null;

                    VerifyMessage vMessage = null;

                    if (resultMap.ContainsKey(message.MessageId.ToString()))
                    {
                        resultMap.TryGetValue(message.MessageId.ToString(), out tempStatus);

                        messages.TryGetValue(message.MessageId.ToString(), out vMessage);

                        Assert.AreEqual(vMessage.status, tempStatus, "[" + vMessage.id + vMessage.subject + "] Flag message failed, the message status is incorrect");

                        HistoryType history = new HistoryType();

                        history = GetLatestCommentBy(message);

                        string reviewerName = account.Attribute("reviewername").Value;

 //                       Assert.AreEqual(history.BPName, vMessage.bp, "[" + vMessage.id + vMessage.subject + "] Flag message failed, the BP name is incorrect");

                        Assert.IsTrue(history.Description.Contains(vMessage.comment), "[" + vMessage.id + vMessage.subject + "] Flag messages failed, the comment is incorrect");

                        Assert.AreEqual(history.ReviewerName, reviewerName, "[" + vMessage.id + vMessage.subject + "] Flag message failed, the reviewer name is incorrect");
                    }
                    else
                    {
                        tempStatus = "Blank";

                        messages.TryGetValue(message.MessageId.ToString(), out vMessage);

                        Assert.AreEqual(vMessage.status, tempStatus, "[" + vMessage.id + vMessage.subject + "] The message status is incorrect");

                        IRestResponse<MessageHistory> getMessageHistoryResponse = APIGetMessageHistory(message);

                        JsonObject history = SimpleJson.DeserializeObject<JsonObject>(getMessageHistoryResponse.Content);

                        object actions;

                        history.TryGetValue("Actions", out actions);

                        JsonArray actionsArray = (JsonArray)actions;

                        Assert.IsTrue(actionsArray.Count == 0, "[" + vMessage.id + vMessage.subject + "] Flag message failed, the message history should be null");
                    }
                }
                #endregion
            }
        }

        [TestMethod]
        public void CodeMessages_CodeAllFlaggedMessages()
        {
            for (int j = 0; j < account.Element("CodeFlaggedMessage").Elements("codeallflaggedmessages").ToList().Count; j++)
            {
                               
                List<XElement> messagesFromXML;

                messagesFromXML = account.Element("CodeFlaggedMessage").Elements("codeallflaggedmessages").ToList();

                MarkAction action = new MarkAction();

                action.code = messagesFromXML[j].Attribute("code").Value;

                action.bp = messagesFromXML[j].Attribute("BP").Value;

                action.comment = messagesFromXML[j].Attribute("comment").Value;

                action.reviewgroups = messagesFromXML[j].Attribute("reviewgroup").Value;

                action.includeallrevgroups = bool.Parse(messagesFromXML[j].Attribute("includeallrevgroups").Value);

                #region Get filter information from xml

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

                Dictionary<string, string> rgMapResult = RGMap(defaultMessageListFilterResponse.Content);

                List<long> reviewGroupList = new List<long>();

                reviewGroupList = reviewGroupsList(action.reviewgroups, rgMapResult);

                #endregion

                IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now, reviewGroupList, 0, 0, false, false, action.includeallrevgroups);

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

                IRestResponse prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse2.Data, new DateTime(), new DateTime(), 0, 0, true, false);

                Dictionary<string, string> BPMapResult = new Dictionary<string, string>();

                BPMapResult = BPMap(prepareMessageListResponse.Content);

                List<int> bpIdList = new List<int>();

                string bpid;

                BPMapResult.TryGetValue(action.bp, out bpid);

                if (bpid != null)
                {
                    bpIdList.Add(int.Parse(bpid));
                }
                else
                {
                    List<string>List = BPMapResult.Values.ToList();

                    foreach (string l in List)
                    {
                        bpIdList.Add(int.Parse(l));
                    }
                }

                IRestResponse<Result> codeAllFlaggedMessages = APICodeAllFlaggedMessages(bpIdList,action.code,action.comment);

                Assert.IsTrue(codeAllFlaggedMessages.StatusCode == System.Net.HttpStatusCode.OK, "Flag Messages status code is not correct");
            }
        }

        [TestMethod]
        public void CodeMessage_WithFilter()
        {
            for (int j = 0; j < account.Element("CodeFlaggedMessage").Elements("codemessageswithfilter").ToList().Count; j++)
            {

                List<XElement> actionsFromXML;

                actionsFromXML = account.Element("CodeFlaggedMessage").Elements("codemessageswithfilter").ToList();

                MarkAction action = new MarkAction();

                action.numDaysRecentlyReviewed = int.Parse(actionsFromXML[j].Attribute("numDaysRecentlyReviewed").Value);

                action.numDaysUnreviewed = int.Parse(actionsFromXML[j].Attribute("numDaysUnreviewed").Value);

                action.includeEscalated = bool.Parse(actionsFromXML[j].Attribute("includeEscalated").Value);

                action.reviewgroups = actionsFromXML[j].Attribute("reviewgroup").Value;

                action.includeallrevgroups = bool.Parse(actionsFromXML[j].Attribute("includeallrevgroups").Value);

                action.code = actionsFromXML[j].Attribute("code").Value;

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

                Dictionary<string, string> rgMapResult = RGMap(defaultMessageListFilterResponse.Content);

                List<long> reviewGroupList = new List<long>();

                reviewGroupList = reviewGroupsList(action.reviewgroups, rgMapResult);

                IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now, reviewGroupList, action.numDaysRecentlyReviewed, action.numDaysUnreviewed, action.includeEscalated, false, action.includeallrevgroups);

                IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

                IRestResponse prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse2.Data, new DateTime(), new DateTime(), action.numDaysRecentlyReviewed, action.numDaysUnreviewed, action.includeEscalated);
                
                JsonObject totalCount = SimpleJson.DeserializeObject<JsonObject>(prepareMessageListResponse.Content);

                object count;

                totalCount.TryGetValue("totalCount", out count);

                int totalMessageCount = int.Parse(count.ToString());

                IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage("0-99", totalMessageCount);                

                List<FlaggedMessage> message = new List<FlaggedMessage>();

                message.Add(getFlaggedMessagesByPageResponse.Data[0]);

                CodeID codeID = (CodeID)Enum.Parse(typeof(CodeID), action.code, true);

                //int codeid = (int)codeID;

                IRestResponse<Result> codeFlaggedMessageResponse = APICodeFlaggedMessage(message, codeID, "mark ok", defaultMessageListFilterResponse2.Data);

                IRestResponse prepareMessageListResponse2 = APIPrepareMessageList(defaultMessageListFilterResponse2.Data, new DateTime(), new DateTime(), action.numDaysRecentlyReviewed, action.numDaysUnreviewed, action.includeEscalated);

                JsonObject currentTotalCount = SimpleJson.DeserializeObject<JsonObject>(prepareMessageListResponse2.Content);

                object currentcount;

                currentTotalCount.TryGetValue("totalCount", out currentcount);

                int currentMessageCount = int.Parse(currentcount.ToString());

                Assert.IsTrue(currentMessageCount == (totalMessageCount - 1), "numDaysRecentlyReviewed=" + action.numDaysRecentlyReviewed.ToString() + ", numDaysUnreviewed=" + action.numDaysUnreviewed.ToString() + ", includeEscalated=" + action.includeEscalated.ToString() + "  mark message with filter works incorrectly");
            }
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

    public class VerifyMessage {
        public string id;
        public string subject;
        public string bp;
        public CodeID code;
        public string status;
        public string comment;
    }

    public class HistoryType
    {
        public string Date;
        public string BPName;
        public string ReviewerName;
        public string Description;
    }

    public class MarkAction
    {
        public string reviewgroups;
        public bool includeallrevgroups;
        public string code;
        public string bp;
        public string comment;
        public int numDaysRecentlyReviewed;
        public int numDaysUnreviewed;
        public bool includeEscalated;
    }
}
