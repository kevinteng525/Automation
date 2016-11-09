using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RestSharp;
using HtmlAgilityPack;
using SupervisorWebAutomation_API.Common;
using SupervisorWebAutomation_API.Config;

namespace SupervisorWebAutomation_API
{
    [TestClass]
    public class MessagePreviewAPITest : BaseTest
    {
        private List<FlaggedMessage> flaggedMessagesList = null;
        private List<XElement> toCheckMessagesList = null;
        private const string HightLightTag = "exmns:";
        private const string HightLightTagTemp = "exmns-";
        //This seperator may need to be updated if we choose different set of messages for testing. Such as some hitted words maybe seperated by ',.;'...
        private char[] WordSeperaters = new char[] { ' ', ',', '.' };

        [TestInitialize]
        public void Setup()
        {
            Account reviewer = SupervisorWebConfig.GetAccountByRole(AccountRole.Reviewer);

            Login(reviewer.UserName, reviewer.Password);

            toCheckMessagesList = SupervisorWebConfig.GetAllMessages();

            IRestResponse<MessageListFilter> defaultMessageListFilterResponse = APIGetDefaultMessageListFilter();

            MessageListFilter defaultFilter = defaultMessageListFilterResponse.Data;

            defaultFilter.IncludeAllRevGroups = true;

            defaultFilter.IncludeEscalated = true;

            defaultFilter.NumDaysRecentlyReviewed = 0;

            defaultFilter.NumDaysUnreviewed = 0;

            defaultFilter.UseArchiveDateRange = false;

            IRestResponse<PrepareMessageList> prepareMessageListResponse = APIPrepareMessageList(defaultFilter, new DateTime(), new DateTime(), 0, 0, true, false);

            int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

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
                    //Get message preview
                    if (toCheckMessagesList.Select(m => ulong.Parse(m.Attribute("messageId").Value)).Contains(message.MessageId))
                    {
                        if (flaggedMessagesList == null)
                        {
                            flaggedMessagesList = new List<FlaggedMessage>();
                        }

                        flaggedMessagesList.Add(message);

                    }
                }
            }
        }

        [TestCategory("webid=5506")]
        [TestMethod]
        public void MessagePreview_MessagePreview()
        {
            Assert.IsTrue(flaggedMessagesList.Count > 0, "There should be more than 1 messages available for preview. Check the setup to diagnostic.");

            foreach (FlaggedMessage message in flaggedMessagesList)
            {
                //Get message preview

                XElement toCheckMessage = toCheckMessagesList.Find(m => ulong.Parse(m.Attribute("messageId").Value) == message.MessageId);

                List<string> hitKeyWords = new List<string>();

                foreach (XElement bp in toCheckMessage.Element("bps").Elements("bp"))
                {
                    hitKeyWords.AddRange(bp.Attribute("hitKeyWords").Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                }

                hitKeyWords = hitKeyWords.Select(k => k.ToLower()).ToList();

                string sRIDs = GetSRIDstring(message);

                int highlightedWordsCountInMessage = 0;

                int highlightedWordsCountInAttachments = 0;

                IRestResponse<MessagePreview> getMessagePreviewResponse = APIGetMessagePreview(message.MessageId, sRIDs);

                Assert.IsTrue(getMessagePreviewResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message preview status code is not correct");

                Assert.AreEqual(message.MessageId, ulong.Parse(getMessagePreviewResponse.Data.MessageId.ToString()), "Message ID of get message preview is not correct");

                Assert.AreEqual(message.Subject, getMessagePreviewResponse.Data.Subject, "Message subject of get message preview is not correct");

                Assert.AreEqual(message.HasAttachments, getMessagePreviewResponse.Data.Attachments != null && getMessagePreviewResponse.Data.Attachments.Count > 0, "Message HasAttachments of get message preview is not correct");

                Assert.AreEqual(message.ReceivedDate, getMessagePreviewResponse.Data.ReceivedDate, "Message ReceivedDate of get message preview is not correct");

                //Get message preview HTML

                List<HtmlNode> highlightedNodesInMessage = new List<HtmlNode>();

                List<HtmlNode> highlightedNodesInAttachments = new List<HtmlNode>();

                IRestResponse getMessagePreviewHtmlResponse = APIGetMessagePreviewHtml(message.MessageId, 0, sRIDs);

                Assert.IsTrue(getMessagePreviewHtmlResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message preview html status code is not correct");

                Assert.IsTrue(getMessagePreviewHtmlResponse.Content.Length > 0, "Get message preview html content length is 0");

                Assert.IsTrue(getMessagePreviewHtmlResponse.Content.Contains("Subject:"), string.Format("Get message preview html content is not correct! Subject of message [{0}] missing.", message.Subject));

                Assert.IsTrue(getMessagePreviewHtmlResponse.Content.Contains("From:"), string.Format("Get message preview html content is not correct! From of message [{0}] missing.", message.Subject));

                Assert.IsTrue(getMessagePreviewHtmlResponse.Content.Contains("Received:"), string.Format("Get message preview html content is not correct! Received of message [{0}] missing.", message.Subject));

                highlightedNodesInMessage = GetHighlightedNodesOfPreviewResponse(getMessagePreviewHtmlResponse, hitKeyWords, toCheckMessage);

                foreach (HtmlNode node in highlightedNodesInMessage)
                {
                    foreach (string hitWord in node.InnerText.Split(WordSeperaters, StringSplitOptions.RemoveEmptyEntries))
                    {
                        //TODO, some message may hit different phase of a word, such as the "tests" may be hitted when the lexicon is "test", this need to be adjust according to the messages
                        Assert.IsTrue(hitKeyWords.Contains(hitWord.ToLower()), string.Format("The highlighted word [{0}] is not among the key words.", hitWord));

                        highlightedWordsCountInMessage += 1;
                    }
                }

                //get Attachment Preview
                foreach (MessageAttachment attachment in getMessagePreviewResponse.Data.Attachments)
                {
                    if (!string.IsNullOrEmpty(attachment.MatchLocation))
                    {
                        IRestResponse getAttachmentPreviewHTMLResponse = APIGetMessagePreviewHtml(message.MessageId, int.Parse(attachment.FileId), sRIDs);

                        Assert.IsTrue(getAttachmentPreviewHTMLResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message preview html status code is not correct");

                        Assert.IsTrue(getAttachmentPreviewHTMLResponse.Content.Length > 0, "Get message preview html content length is 0");

                        List<HtmlNode> highlightedNodesInAttachment = GetHighlightedNodesOfPreviewResponse(getAttachmentPreviewHTMLResponse, hitKeyWords, toCheckMessage);

                        foreach (HtmlNode node in highlightedNodesInAttachment)
                        {
                            foreach (string hitWord in node.InnerText.Split(WordSeperaters, StringSplitOptions.RemoveEmptyEntries))
                            {
                                //TODO, some message may hit different phase of a word, such as the "tests" may be hitted when the lexicon is "test", this need to be adjust according to the messages
                                Assert.IsTrue(hitKeyWords.Contains(hitWord.ToLower()), string.Format("The highlighted word [{0}] is not among the key words.", hitWord));

                                highlightedWordsCountInAttachments += 1;
                            }
                        }

                        highlightedNodesInAttachments = highlightedNodesInAttachments.Concat(highlightedNodesInAttachment).ToList();
                    }
                    else
                    {
                        string responseMessage = "This file is corrupt and cannot be previewed. If you want to review it, please open it natively by double clicking the original message.";

                        IRestResponse getAttachmentPreviewHTMLResponse = APIGetMessagePreviewHtml(message.MessageId, int.Parse(attachment.FileId), sRIDs);

                        Assert.IsTrue(getAttachmentPreviewHTMLResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message preview html status code is not correct");

                        Assert.AreEqual(responseMessage, SimpleJson.DeserializeObject(getAttachmentPreviewHTMLResponse.Content), "User should not be able to preview the file which is with no lexicon hit.");

                    }
                }

                Assert.IsTrue(highlightedNodesInMessage.Concat(highlightedNodesInAttachments).Count() > 0, "There're no matching phases in either message or attachments.");

                Assert.AreEqual(int.Parse(toCheckMessage.Attribute("hitCount").Value), highlightedWordsCountInMessage + highlightedWordsCountInAttachments, "The totally hitted count is not correct.");

            }
        }

        private List<HtmlNode> GetHighlightedNodesOfPreviewResponse(IRestResponse response, List<string> hitKeyWords, XElement toCheckMessage)
        {
            string messagePreviewHTML = SimpleJson.DeserializeObject(response.Content).ToString();

            HtmlDocument doc = new HtmlDocument();

            //below is to workaroud the issue that the HtmlAgilityPack doesn't support the XPath with namespace. We convert the nameSpcase:tagName to nameSpace-tagName

            for (int i = 0; i < toCheckMessage.Element("bps").Elements("bp").Count(); i++)
            {
                string highlightedStyle = toCheckMessage.Element("bps").Elements("bp").ElementAt(i).Attribute("highlightStyle").Value;
               
                messagePreviewHTML = messagePreviewHTML.Replace(HightLightTag + highlightedStyle, HightLightTagTemp + highlightedStyle);
            }

            doc.LoadHtml(messagePreviewHTML);

            List<HtmlNode> highlightedNodes = new List<HtmlNode>();

            for (int i = 0; i < toCheckMessage.Element("bps").Elements("bp").Count(); i++)
            {
                string highlightedStyle = toCheckMessage.Element("bps").Elements("bp").ElementAt(i).Attribute("highlightStyle").Value;

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//" + HightLightTagTemp + highlightedStyle);

                if (nodes != null)
                {
                    highlightedNodes = highlightedNodes.Concat(nodes).ToList();
                }
            }

            return highlightedNodes;
        }

        [TestCleanup]
        public void Teardown()
        {
            Logout();
        }

    }
}
