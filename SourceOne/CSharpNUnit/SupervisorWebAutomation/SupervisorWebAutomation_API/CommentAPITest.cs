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
    public class CommentAPITest : BaseTest
    {
        public XElement account = SupervisorWebConfig.Config.Root.Element("testdata").Elements("account").ToList()[1];

        [TestInitialize]
        public void Setup()
        {
            string username = account.Attribute("username").Value;
            string password = account.Attribute("password").Value;
            Login(username, password);
        }

        [TestCategory("webid=5467")]
        [TestMethod]
        public void CommentMessage()
        {
            for (int j = 0; j < account.Element("CommentMessage").Elements("CommentSingleMessage").ToList().Count; j++)
            {
                XElement messagesFromXML;

                messagesFromXML = account.Element("CommentMessage").Elements("CommentSingleMessage").ToList()[j];

                IRestResponse<MessageListFilter> messageListFilterResult = GetMessageListFilterResult(messagesFromXML);

                List<FlaggedMessage> flaggedMessageList = FindOutMessagesNeedToComment(messageListFilterResult, messagesFromXML);

                string comment = messagesFromXML.Attribute("comment").Value.ToString();

                int id = int.Parse(messagesFromXML.Element("message").Attribute("messageId").Value.ToString());

                IRestResponse commentMessageResponse = APICommentMessages(id,comment);

                Assert.IsTrue(commentMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Comment Message status code is not correct");

                string reviewername = account.Attribute("reviewername").Value.ToString();

                flaggedMessageList = FindOutMessagesNeedToComment(messageListFilterResult, messagesFromXML);

                foreach (FlaggedMessage m in flaggedMessageList)
                {
                    Assert.IsTrue(GetLatestCommentBy(m, CommentType.Description).Contains(comment), "Comment Message failed, the description is incorrect");

                    Assert.IsTrue(GetLatestCommentBy(m, CommentType.ReviewerName).Contains(reviewername), "Comment Message failed, the reviewer name is incorrect");

                    Assert.IsTrue(m.HasComments.ToString() == "True", "Comment Message failed, the message status is incorrect");
                }      
            }
        }

        [TestCategory("webid=5468")]
        [TestMethod]
        public void CommentBatchMessages()
        {
            for (int j = 0; j < account.Element("CommentMessage").Elements("CommentBatchMessages").ToList().Count; j++)
            {
               // List<XElement> messagesFromXML;

                XElement messagesFromXML;

                messagesFromXML = account.Element("CommentMessage").Elements("CommentBatchMessages").ToList()[j];

                IRestResponse<MessageListFilter> messageListFilterResult = GetMessageListFilterResult(messagesFromXML);

                List<FlaggedMessage> flaggedMessageList = FindOutMessagesNeedToComment(messageListFilterResult, messagesFromXML);

                string comment = messagesFromXML.Attribute("comment").Value.ToString();
               
                List<XElement> messagesxml = messagesFromXML.Elements("message").ToList();

                List<string> messageIds = new List<string>();

                foreach (XElement m in messagesxml)
                {
                    string id = m.Attribute("messageId").Value.ToString();
                    
                    messageIds.Add(id);
                }

                IRestResponse batchCommentMessagesResponse = APIBatchCommentMessages(comment, messageIds);

                Assert.IsTrue(batchCommentMessagesResponse.StatusCode == System.Net.HttpStatusCode.OK, "Batch Comment Message status code is not correct");

                string reviewername = account.Attribute("reviewername").Value.ToString();

                flaggedMessageList = FindOutMessagesNeedToComment(messageListFilterResult, messagesFromXML);

                foreach (FlaggedMessage m in flaggedMessageList)
                {
                    Assert.IsTrue(GetLatestCommentBy(m, CommentType.Description).Contains(comment), "Comment Message failed, the description is incorrect");

                    Assert.IsTrue(GetLatestCommentBy(m, CommentType.ReviewerName).Contains(reviewername), "Comment Message failed, the reviewer name is incorrect");

                    Assert.IsTrue(m.HasComments.ToString() == "True", "Comment Message failed, the message status is incorrect");
                }               
            }
        }

        [TestCategory("webid=5469")]
        [TestMethod]
        public void CommentAllFlaggedMessage()
        {
            for (int j = 0; j < account.Element("CommentMessage").Elements("CommentAllFlaggedMessage").ToList().Count; j++)
            {

                XElement messagesFromXML;

                messagesFromXML = account.Element("CommentMessage").Elements("CommentAllFlaggedMessage").ToList()[j];

                IRestResponse<MessageListFilter> messageListFilterResult = GetMessageListFilterResult(messagesFromXML);

                IRestResponse prepareMessageListResponse = APIPrepareMessageList(messageListFilterResult.Data, DateTime.Now, DateTime.Now);

                JsonObject totalCount = SimpleJson.DeserializeObject<JsonObject>(prepareMessageListResponse.Content);

                object count;

                totalCount.TryGetValue("totalCount", out count);

                int totalMessageCount = int.Parse(count.ToString());

                IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage("0-99", totalMessageCount);

                List<FlaggedMessage> flaggedMessagesList; 

                string comment = messagesFromXML.Attribute("comment").Value.ToString();

                IRestResponse CommentAllFlaggedMessagesResponse = APICommentAllFlaggedMessages(comment);

                Assert.IsTrue(CommentAllFlaggedMessagesResponse.StatusCode == System.Net.HttpStatusCode.OK, "Comment All Flagged Message status code is not correct");
    
                string reviewername = account.Attribute("reviewername").Value.ToString();

                IRestResponse prepareMessageListResponse2 = APIPrepareMessageList(messageListFilterResult.Data, DateTime.Now, DateTime.Now);

                IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse2 = APIGetFlaggedMessagesByPage("0-99", totalMessageCount);

                flaggedMessagesList = getFlaggedMessagesByPageResponse2.Data;

                for (int i = 0, step = 5; i < flaggedMessagesList.Count; i += step)
                {
                    Assert.IsTrue(GetLatestCommentBy(flaggedMessagesList[i], CommentType.Description).Contains(comment), "Comment Message failed, the description is incorrect");

                    Assert.IsTrue(GetLatestCommentBy(flaggedMessagesList[i], CommentType.ReviewerName).Contains(reviewername), "Comment Message failed, the reviewer name is incorrect");

                    Assert.IsTrue(flaggedMessagesList[i].HasComments.ToString() == "True", "Comment Message failed, the message status is incorrect");
                }
               
            }
        }
        
        public IRestResponse<MessageListFilter> GetMessageListFilterResult(XElement messagesFromXML)
        {

            string reviewGroups = messagesFromXML.Attribute("reviewgroup").Value;

            bool includeAllRevGroups = bool.Parse(messagesFromXML.Attribute("includeallrevgroups").Value);

            Dictionary<string, VerifyMessage> messages = collectCommentMessagesFromXML(messagesFromXML.Elements("message"));

            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            Dictionary<string, string> rgMapResult = RGMap(defaultMessageListFilterResponse.Content);

            List<long> reviewGroupList = new List<long>();

            reviewGroupList = reviewGroupsList(reviewGroups, rgMapResult);

            IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, DateTime.Now, DateTime.Now, reviewGroupList, 0, 0, false, false, includeAllRevGroups);

            IRestResponse<MessageListFilter> defaultMessageListFilterResponse2 = APIGetDefaultMessageListFilter();

            return defaultMessageListFilterResponse2;
        }

        public List<FlaggedMessage> FindOutMessagesNeedToComment(IRestResponse<MessageListFilter> defaultMessageListFilterResponse, XElement messagesFromXML)
        { 
            Dictionary<string, VerifyMessage> messages = collectCommentMessagesFromXML(messagesFromXML.Elements("message"));
            
            IRestResponse prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, new DateTime(), new DateTime(), 0, 0, true, false);

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

            return flaggedMessagesList;
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
