using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using System.Collections;
using SupervisorWebAutomation_API.Config;
using SupervisorWebAutomation_API.Common;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace SupervisorWebAutomation_API
{
    public class BaseTest
    {
        private Hashtable users = new Hashtable();
        protected IRestClient client = null;

        private TestContext instance;

        private string _visited_url_prefix = string.Empty;

        private string _verification_token = string.Empty;
       
        protected IRestClient RestClient
        {
             get
             {
                 return client;
             }
             set
             {
                 client = value;
             }
        }

        public TestContext TestContext
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public void BeginTimer(string name)
        {
            if (TestContext.Properties.Contains("$LoadTestUserContext"))
            {
                TestContext.BeginTimer(name);
            }
        }

        public void EndTimer(string name)
        {
            if (TestContext.Properties.Contains("$LoadTestUserContext"))
            {
                TestContext.EndTimer(name);
            }
        }

        #region Supervisor Common

        #region Login/Logout/Token

        public RestRequest CreateNewReqest(string url, Method method)
        {
            RestRequest request = new RestRequest(url, method);

            string referer = string.Empty;

            if (url.ToLower().StartsWith("account"))
            {
                if (_visited_url_prefix == string.Empty)//first login
                {
                    _visited_url_prefix = "account";
                    _verification_token = GetVerificationToken("account");                    
                }
                else if(_visited_url_prefix == "account")
                {
                    _visited_url_prefix = "account";
                    _verification_token = GetVerificationToken("reviewer");  
                }
                if (url.ToLower().Contains("loginajax"))
                {
                    request.AddHeader("__RequestVerificationToken", _verification_token);
                }
                else
                {
                    request.AddParameter("__RequestVerificationToken", _verification_token);
                }
                referer = string.Format("{0}/Account#", SupervisorWebConfig.URL);
            }
            else if (url.ToLower().StartsWith("reviewer"))
            {
                if (_visited_url_prefix != "reviewer")//accessed from other page
                {
                    _visited_url_prefix = "reviewer";
                    _verification_token = GetVerificationToken("reviewer");
                }
                request.AddHeader("__RequestVerificationToken", _verification_token);
                referer = string.Format("{0}/Reviewer#", SupervisorWebConfig.URL);
            }
            else if (url.ToLower().StartsWith("reports"))//accessed from other page
            {
                if (_visited_url_prefix != "reports")
                {
                    _visited_url_prefix = "reports";
                    _verification_token = GetVerificationToken("reports");
                    if (null == _verification_token)//when get the review report, we need to take the verification token from the reviewer page instead of reports page(no permission).
                    {
                        _verification_token = GetVerificationToken("reviewer");
                    }
                }
                request.AddHeader("__RequestVerificationToken", _verification_token);
                referer = string.Format("{0}/Reports#", SupervisorWebConfig.URL);
            }
            else
            {
                //currently, the code should not come here.
            }

            request.AddHeader("Referer", referer);

            return request;
        }

        public bool Login(string username, string password)
        {
            IRestResponse response = GetLoginResponse(username, password);

            if (response.StatusCode == System.Net.HttpStatusCode.OK && IsSupAuthSet())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        protected IRestResponse GetLoginResponse(string username, string password)
        {
            client = new RestClient(SupervisorWebConfig.URL);

            client.Timeout = 100000000;
            client.ReadWriteTimeout = 100000000;

            client.CookieContainer = new System.Net.CookieContainer();

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            var request = CreateNewReqest("Account/Login", Method.POST);

            Account account = new Account() { UserName = username, Password = password };

            request.AddParameter("UserName", account.UserName);

            request.AddParameter("Password", account.Password);

            request.AddUrlSegment("ReturnUrl", @"/SupervisorWeb/");

            string timeForLogin = "TimeForLogin";

            BeginTimer(timeForLogin);
            IRestResponse response = client.Execute(request);
            EndTimer(timeForLogin);

            return response;
        }        

        public bool Login()
        {
            return Login(SupervisorWebConfig.User, SupervisorWebConfig.Password);

        }

        protected void GetUserPWFromCSV()
        {            
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            String root = Directory.GetCurrentDirectory();            
            using (TextFieldParser parser = new TextFieldParser(@"C:\SaberAgent\Config\user.csv"))            
            {

                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    users.Add(fields[0], fields[1]);
                }

            }            
        }

        protected List<long> GetRandReviewGroup()
        {
            string rg = GetRandRGNo();

            string[] selectedReviewGroupList = rg.Split(new char[] { ',' });

            List<long> selectedReviewGroup = new List<long>();

            foreach (string rg1 in selectedReviewGroupList)
            {
                selectedReviewGroup.Add(long.Parse(rg1));
            }
            return selectedReviewGroup;
        }      

        protected ArrayList GetRandUserPW()
        {
            int userCount = users.Count;
            Random rand = new Random();
            int randCount = rand.Next(userCount);
            int count = 0;
            ArrayList al = new ArrayList();
            foreach (DictionaryEntry de in users)
            {
                
                if (count == randCount)
                {
                    al.Add(de.Key);
                    al.Add(de.Value);
                    break;
                }
                count++;
            }
            return al;
        }

        protected ArrayList GetRandUserPW(int threadNum)
        {
            int userCount = users.Count;

            int count = 0;
            ArrayList al = new ArrayList();
            foreach (DictionaryEntry de in users)
            {
                if (count == (threadNum % userCount))
                {
                    al.Add(de.Key);
                    al.Add(de.Value);
                    break;
                }
                count++;
            }
            return al;
        }

        protected string GetRandFlagMessageRange()
        {
            Random rand = new Random();
            int randCount = rand.Next(0, 1901);
            //e.g. "0-99"            
            string range = randCount.ToString() + "-" + (randCount + 99).ToString();
            return range;
        }

        protected int GetRandCommentFlagIndex()
        {
            Random rand = new Random();
            int randCount = rand.Next(0, 100);
            //e.g. 88
            return randCount;
        }

        protected int GetRandSleepSec()
        {
            Random rand = new Random();
            int randCount = rand.Next(1, 601);
            //e.g. 88
            return randCount;
        }

        protected string GetRandRGNo()
        {
            string str = "1,2,3,4,5,6,7,8,9,11,12,15";
            Random rand = new Random();
            int randCount = rand.Next(0, 12);

            char deli = ',';
            string[] strArray = str.Split(deli);
            return strArray[randCount];
        }

        protected string GetRandSortCondition()
        {            
            string[] str = new string [] { "+Subject", "-Subject", "+HasComments", "-HasComments", "+HasAttachments", "-HasAttachments", "+ReviewDueDate", "-ReviewDueDate", "+Status", "-Status", "+ReceivedDate", "-ReceivedDate", "+To", "-To", "+MemberName", "-MemberName" };                      

            Random rand = new Random();
            int randCount = rand.Next(0, 16);            
            return str[randCount];
        }

        public bool Logout()
        {
            var request = CreateNewReqest("Account/LogOff", Method.POST);

            string timeForLogout = "TimeForLogout";

            BeginTimer(timeForLogout);
            IRestResponse response = client.Execute(request);
            EndTimer(timeForLogout);  

            if (response.StatusCode == System.Net.HttpStatusCode.OK && !IsSupAuthSet())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected string GetVerificationToken(string pageURL)
        {
            var prerequest = new RestRequest(pageURL, Method.GET);

            string requestVerificationToken;

            try
            {
                IRestResponse preresponse = client.Execute(prerequest);

                if (preresponse.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                requestVerificationToken = preresponse.Content;

                if (!requestVerificationToken.Contains("name=\"__RequestVerificationToken\" type=\"hidden\" value"))
                {
                    return null;
                }

                int start = requestVerificationToken.LastIndexOf("name=\"__RequestVerificationToken\" type=\"hidden\" value") + 55;

                requestVerificationToken = requestVerificationToken.Substring(start);

                int end = requestVerificationToken.IndexOf("\" />");

                requestVerificationToken = requestVerificationToken.Substring(0, end);
            }
            catch
            {
                requestVerificationToken = null;
            }

            return requestVerificationToken;
        }
        
        protected  bool IsSupAuthSet()
        {

            bool isSupAuthSet = false;

            foreach (System.Net.Cookie cookie in client.CookieContainer.GetCookies(new System.Uri(SupervisorWebConfig.URL)))
            {
                if (cookie.Name == ".SUPAUTH" && cookie.Value.Length > 0)
                {
                    isSupAuthSet = true;
                    break;
                }
            }

            return isSupAuthSet;
        }
        #endregion

        #region MessageList
        public IRestResponse<MessageListFilter> APIGetDefaultMessageListFilter(bool recordTime = false)
        {
            var request = CreateNewReqest("Reviewer/GetDefaultMessageListFilter", Method.GET);
            IRestResponse<MessageListFilter> response = null;            

            if (recordTime)
            {
                string getDefaultMessageListFilter = "GetDefaultMessageListFilter";

                BeginTimer(getDefaultMessageListFilter);
                response = client.Execute<MessageListFilter>(request);
                EndTimer(getDefaultMessageListFilter);
            }
            else
            {
                response = client.Execute<MessageListFilter>(request);
            }
            return response;
        }

        public IRestResponse APISaveMessageListFilter(
            MessageListFilter defaultMessageListFilterData, 
            DateTime archiveDateStart,
            DateTime archiveDateEnd,
            int numDaysRecentlyReviewed = 0,
            int numDaysUnreviewed = 0,
            bool includeEscalated = false,
            bool useArchiveDateRange = false,
            bool recordTime = false
            )
        {
            RestRequest saveMessageListFilterRequest = CreateNewReqest("Reviewer/SaveMessageListFilter", Method.POST);

            saveMessageListFilterRequest.RequestFormat = DataFormat.Json;

            MessageListFilter filter = defaultMessageListFilterData;

            filter.NumDaysRecentlyReviewed = numDaysRecentlyReviewed;

            filter.NumDaysUnreviewed = numDaysUnreviewed;

            filter.IncludeEscalated = includeEscalated;

            filter.UseArchiveDateRange = useArchiveDateRange;

            filter.ArchiveDateStart = archiveDateStart;

            filter.ArchiveDateEnd = archiveDateEnd;

            dynamic o = new { filter = SimpleJson.SerializeObject(filter) };

            saveMessageListFilterRequest.AddJsonBody(o);

            saveMessageListFilterRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            IRestResponse response = null;  

            if (recordTime)
            {
                string saveMessageListFilter = "SaveMessageListFilter";

                BeginTimer(saveMessageListFilter);
                response = client.Execute(saveMessageListFilterRequest);
                EndTimer(saveMessageListFilter);
            }
            else
            {
                response = client.Execute(saveMessageListFilterRequest);
            }
            return response;
        }

        // Add review group for saveMessageListFilterRequest

        public IRestResponse APISaveMessageListFilter(
            MessageListFilter defaultMessageListFilterData,
            DateTime archiveDateStart,
            DateTime archiveDateEnd,
            string reviewGroups,
            int numDaysRecentlyReviewed = 0,
            int numDaysUnreviewed = 0,
            bool includeEscalated = false,
            bool useArchiveDateRange = false,
            bool includeAllRevGroups = true,
            bool recordTime = false             
             )
        {
            RestRequest saveMessageListFilterRequest = CreateNewReqest("Reviewer/SaveMessageListFilter", Method.POST);

            saveMessageListFilterRequest.RequestFormat = DataFormat.Json;

            MessageListFilter filter = defaultMessageListFilterData;

            filter.NumDaysRecentlyReviewed = numDaysRecentlyReviewed;

            filter.NumDaysUnreviewed = numDaysUnreviewed;

            filter.IncludeEscalated = includeEscalated;

            filter.UseArchiveDateRange = useArchiveDateRange;

            filter.ArchiveDateStart = archiveDateStart;

            filter.ArchiveDateEnd = archiveDateEnd;

            filter.IncludeAllRevGroups = includeAllRevGroups;

            string[] selectedReviewGroupList = reviewGroups.Split(new char[] { ',' });

            List<long> selectedReviewGroup = new List<long>();

            foreach (string rg in selectedReviewGroupList)
            {
                selectedReviewGroup.Add(long.Parse(rg));
            }

            filter.ReviewGroups = selectedReviewGroup; 

            dynamic o = new { filter = SimpleJson.SerializeObject(filter) };

            saveMessageListFilterRequest.AddJsonBody(o);

            saveMessageListFilterRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            IRestResponse response = null;

            if (recordTime)
            {
                string saveMessageListFilter = "SaveMessageListFilter";

                BeginTimer(saveMessageListFilter);
                response = client.Execute(saveMessageListFilterRequest);
                EndTimer(saveMessageListFilter);
            }
            else
            {
                response = client.Execute(saveMessageListFilterRequest);
            }            

            return response;
        }

        public IRestResponse APISaveMessageListFilter(
      MessageListFilter defaultMessageListFilterData,
      DateTime archiveDateStart,
      DateTime archiveDateEnd,
      List<long> reviewGroups,
      int numDaysRecentlyReviewed = 0,
      int numDaysUnreviewed = 0,
      bool includeEscalated = false,
      bool useArchiveDateRange = false,
      bool includeAllRevGroups = true

       )
        {
            RestRequest saveMessageListFilterRequest = CreateNewReqest("Reviewer/SaveMessageListFilter", Method.POST);

            saveMessageListFilterRequest.RequestFormat = DataFormat.Json;

            MessageListFilter filter = defaultMessageListFilterData;

            filter.NumDaysRecentlyReviewed = numDaysRecentlyReviewed;

            filter.NumDaysUnreviewed = numDaysUnreviewed;

            filter.IncludeEscalated = includeEscalated;

            filter.UseArchiveDateRange = useArchiveDateRange;

            filter.ArchiveDateStart = archiveDateStart;

            filter.ArchiveDateEnd = archiveDateEnd;

            filter.IncludeAllRevGroups = includeAllRevGroups;

            //string[] selectedReviewGroupList = reviewGroups.Split(new char[] { ',' });

            //List<long> selectedReviewGroup = new List<long>();

            //foreach (string rg in selectedReviewGroupList)
            //{
            //    selectedReviewGroup.Add(long.Parse(rg));
            //}

            filter.ReviewGroups = reviewGroups;

            dynamic o = new { filter = SimpleJson.SerializeObject(filter) };

            saveMessageListFilterRequest.AddJsonBody(o);

            saveMessageListFilterRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            IRestResponse response = client.Execute(saveMessageListFilterRequest);

            return response;
        }

        public IRestResponse<PrepareMessageList> APIPrepareMessageList(
            MessageListFilter defaultMessageListFilterData,
            DateTime archiveDateStart,
            DateTime archiveDateEnd,
            int numDaysRecentlyReviewed = 0,
            int numDaysUnreviewed = 0,
            bool includeEscalated = false,
            bool useArchiveDateRange = false,
            bool recordTime = false
            )
        {
            RestRequest prepareMessageListRequest = CreateNewReqest("Reviewer/PrepareMessageList", Method.POST);

            prepareMessageListRequest.RequestFormat = DataFormat.Json;

            MessageListFilter filter = defaultMessageListFilterData;

            filter.NumDaysRecentlyReviewed = numDaysRecentlyReviewed;

            filter.NumDaysUnreviewed = numDaysUnreviewed;

            filter.IncludeEscalated = includeEscalated;

            filter.UseArchiveDateRange = useArchiveDateRange;

            filter.ArchiveDateStart = archiveDateStart;

            filter.ArchiveDateEnd = archiveDateEnd;

            dynamic o = new { filter = SimpleJson.SerializeObject(filter) };

            prepareMessageListRequest.AddJsonBody(o);

            prepareMessageListRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            IRestResponse<PrepareMessageList> response = null;
            if (recordTime)
            {
                string prepareMessageList = "PrepareMessageList";

                BeginTimer(prepareMessageList);
                response = client.Execute<PrepareMessageList>(prepareMessageListRequest);
                EndTimer(prepareMessageList);
            }
            else
            {
                response = client.Execute<PrepareMessageList>(prepareMessageListRequest);
            }

            return response;
        }



        public IRestResponse<List<FlaggedMessage>> APIGetFlaggedMessagesByPage(
            string range,
            int totalCount,            
            string sortParam = "+ReviewDueDate",
            bool recordTime = false
            )
        {
            IRestRequest getFlaggedMessageByPageRequest = CreateNewReqest("Reviewer/GetFlaggedMessagesByPage", Method.GET);

            getFlaggedMessageByPageRequest.AddQueryParameter("sortParam", sortParam);

            getFlaggedMessageByPageRequest.AddHeader("Range", "items=" + range);

            getFlaggedMessageByPageRequest.AddHeader("X-Range", "items=" + range);

            getFlaggedMessageByPageRequest.AddHeader("totalCount", totalCount.ToString());

            IRestResponse<List<FlaggedMessage>> response = null;
            if (recordTime)
            {
                string getFlaggedMessagesByPage = "GetFlaggedMessagesByPage";

                BeginTimer(getFlaggedMessagesByPage);
                response = client.Execute<List<FlaggedMessage>>(getFlaggedMessageByPageRequest);
                EndTimer(getFlaggedMessagesByPage);
            }
            else
            {
                response = client.Execute<List<FlaggedMessage>>(getFlaggedMessageByPageRequest);
            }

            return response;
        }

        public IRestResponse<List<FlaggedMessage>> APIGetFlaggedMessagesByPage2(
            string range,
            int totalCount,
            bool recordTime = false
            )
        {
            IRestRequest getFlaggedMessageByPageRequest = CreateNewReqest("Reviewer/GetFlaggedMessagesByPage", Method.GET);

            getFlaggedMessageByPageRequest.AddHeader("Range", "items=" + range);

            getFlaggedMessageByPageRequest.AddHeader("X-Range", "items=" + range);

            getFlaggedMessageByPageRequest.AddHeader("totalCount", totalCount.ToString());

            IRestResponse<List<FlaggedMessage>> response = null;
            if (recordTime)
            {
                string getFlaggedMessagesByPage = "GetFlaggedMessagesByPage";

                BeginTimer(getFlaggedMessagesByPage);
                response = client.Execute<List<FlaggedMessage>>(getFlaggedMessageByPageRequest);
                EndTimer(getFlaggedMessagesByPage);
            }
            else
            {
                response = client.Execute<List<FlaggedMessage>>(getFlaggedMessageByPageRequest);
            }

            return response;
        }
       
        public IRestResponse<List<BusinessPolicyFlag>> APIGetAllBusinessPolicies(MessageListFilter filter)
        {
            IRestRequest getAllBPRequest = CreateNewReqest("Reviewer/GetAllBusinessPolicies", Method.POST);

            dynamic o = new { filter = SimpleJson.SerializeObject(filter) };

            getAllBPRequest.AddJsonBody(o);

            getAllBPRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            getAllBPRequest.AddHeader("Accept-Language", "en-US,es-ES;q=0.8,de;q=0.5,es;q=0.3");

            getAllBPRequest.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");

            //getAllBPRequest.AddHeader("Connection", "Keep-Alive");

            getAllBPRequest.AddHeader("Cache-Control", "no-cache");

            string getAllBusinessPolicies = "GetAllBusinessPolicies";

            BeginTimer(getAllBusinessPolicies);
            IRestResponse<List<BusinessPolicyFlag>> response = client.Execute<List<BusinessPolicyFlag>>(getAllBPRequest);
            EndTimer(getAllBusinessPolicies);

            return response;
        }

        #endregion

        #region Preview

        public List<long> GetSRIDs(FlaggedMessage message)
        {
            List<long> sRIDs = new List<long>();
            foreach(FlaggedMessageInfo messageInfo in message.FlaggedMessageInfoList)
            {
                sRIDs.Add(messageInfo.SampleRecID);
            }
            return sRIDs;
        }

        public string GetSRIDstring(FlaggedMessage message)
        {
            List<long> sRIDList = GetSRIDs(message);

            string sRIDs = "";

            foreach (long srid in sRIDList)
            {
                sRIDs = sRIDs + srid.ToString() + ",";
            }
            sRIDs = sRIDs.TrimEnd(new char[] { ',' });

            return sRIDs;
        }

        public IRestResponse<MessagePreview> APIGetMessagePreview( ulong messageId, string SRIDs)
        {
            IRestRequest getMessagePreviewRequest = CreateNewReqest("Reviewer/GetMessagePreview", Method.GET);

            getMessagePreviewRequest.AddQueryParameter("messageId", messageId.ToString());

            getMessagePreviewRequest.AddQueryParameter("SRIDs", SRIDs);

            string getMessagePreview = "GetMessagePreview";

            BeginTimer(getMessagePreview);
            IRestResponse<MessagePreview> response = client.Execute<MessagePreview>(getMessagePreviewRequest);
            EndTimer(getMessagePreview);

            return response;
        }

        public IRestResponse APIGetMessagePreviewHtml(ulong messageId, int fileID, string SRIDs)
        {
            IRestRequest getMessagePreviewHTMLRequest = CreateNewReqest("Reviewer/GetMessagePreviewHtml", Method.GET);

            getMessagePreviewHTMLRequest.AddQueryParameter("messageId", messageId.ToString());

            getMessagePreviewHTMLRequest.AddQueryParameter("fileId", fileID.ToString());

            getMessagePreviewHTMLRequest.AddQueryParameter("SRIDs", SRIDs);

            string getMessagePreviewHtml = "GetMessagePreviewHtml";

            BeginTimer(getMessagePreviewHtml);
            IRestResponse response = client.Execute(getMessagePreviewHTMLRequest);
            EndTimer(getMessagePreviewHtml);

            return response;
        }

        #endregion

        #region Mark/Comment

        public IRestResponse<MessageHistory> APIGetMessageHistory( FlaggedMessage message)
        {
            IRestRequest getMessageHistoryRequest =CreateNewReqest("Reviewer/GetMessageHistory", Method.GET);

            getMessageHistoryRequest.AddQueryParameter("messageId", message.MessageId.ToString());

            List<long> sRIDList = GetSRIDs(message);

            foreach (long sRID in sRIDList)
            {
                getMessageHistoryRequest.AddQueryParameter("sampleRecordIds", sRID.ToString());
            }

            IRestResponse<MessageHistory> response = client.Execute<MessageHistory>(getMessageHistoryRequest);

            return response;
        }

        public IRestResponse APIDownloadMessage( FlaggedMessage message, int fileID)
        {
            IRestRequest downloadMessageRequest = CreateNewReqest("Reviewer/DownloadMessage", Method.GET);

            downloadMessageRequest.AddQueryParameter("messageId", message.MessageId.ToString());

            downloadMessageRequest.AddQueryParameter("fileId", fileID.ToString());

            IRestResponse response = client.Execute(downloadMessageRequest);

            return response;
        }

        public enum CommentType
        {
            Date,
            BPName,
            ReviewerName,
            Description
        }

        public string GetLatestCommentBy(FlaggedMessage message, CommentType type)
        {
            IRestResponse<MessageHistory> getMessageHistoryResponse = APIGetMessageHistory(message);

            string latestComment = null;

            int index = getMessageHistoryResponse.Data.Actions.Count - 1;

            switch (type)
            {
                case CommentType.Date:
                    latestComment = getMessageHistoryResponse.Data.Actions[index].Date.ToString();
                    break;
                case CommentType.BPName:
                    latestComment = getMessageHistoryResponse.Data.Actions[index].BpName;
                    break;
                case CommentType.Description:
                    latestComment = getMessageHistoryResponse.Data.Actions[index].Description;
                    break;
                case CommentType.ReviewerName:
                    latestComment = getMessageHistoryResponse.Data.Actions[index].ReviewerName;
                    break;
            }

            return latestComment;
        }

        public HistoryType GetLatestCommentBy(FlaggedMessage message)
        {
            IRestResponse<MessageHistory> getMessageHistoryResponse = APIGetMessageHistory(message);

            HistoryType latestComment = new HistoryType();
              
            int index = getMessageHistoryResponse.Data.Actions.Count - 1;

            if (index >= 0)
            {

                latestComment.Date = getMessageHistoryResponse.Data.Actions[index].Date.ToString();

                latestComment.BPName = getMessageHistoryResponse.Data.Actions[index].BpName;

                latestComment.Description = getMessageHistoryResponse.Data.Actions[index].Description;

                latestComment.ReviewerName = getMessageHistoryResponse.Data.Actions[index].ReviewerName;
            }
            else
            {
                latestComment.Date = "";

                latestComment.BPName = "";

                latestComment.Description = "";

                latestComment.ReviewerName = "";
            }
            
            return latestComment;
        }

        public IRestResponse APICommentMessages(
            int messageId,
            string messageComment
            )
        {
            IRestRequest commentMessagesRequest = CreateNewReqest("Reviewer/CommentMessages", Method.POST);

            MessageComment comment = new MessageComment() { MessageId = messageId, Comment = messageComment };

            List<MessageComment> messageComments = new List<MessageComment>();

            messageComments.Add(comment);

            dynamic o = new { messageComments = SimpleJson.SerializeObject(messageComments) };

            commentMessagesRequest.RequestFormat = DataFormat.Json;

            commentMessagesRequest.AddJsonBody(o);

            commentMessagesRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            string commentMessages = "CommentMessages";

            BeginTimer(commentMessages);
            IRestResponse response = client.Execute(commentMessagesRequest);
            EndTimer(commentMessages);

            return response;
        }
    
        public IRestResponse APIBatchCommentMessages(
           string comment,
            List<string> messageIds
           )
        {
            IRestRequest batchCommentMessagesRequest = CreateNewReqest("Reviewer/BatchCommentMessages", Method.POST);

            dynamic o = new { comment = comment, messageIds = SimpleJson.SerializeObject(messageIds) };

            batchCommentMessagesRequest.AddJsonBody(o);

            batchCommentMessagesRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            string batchCommentMessages = "BatchCommentMessages";

            BeginTimer(batchCommentMessages);
            IRestResponse batchCommentMessagesResponse = client.Execute(batchCommentMessagesRequest);
            EndTimer(batchCommentMessages);

            return batchCommentMessagesResponse;
        }

        public IRestResponse APICommentAllFlaggedMessages(
            string messageComment
            )
        {
            IRestRequest commentMessagesRequest = CreateNewReqest("Reviewer/CommentAllFlaggedMessages", Method.POST);

            commentMessagesRequest.RequestFormat = DataFormat.Json;

            dynamic o = new { comment = messageComment };

            commentMessagesRequest.AddJsonBody(o);

            commentMessagesRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            string commentAllFlaggedMessages = "CommentAllFlaggedMessages";

            BeginTimer(commentAllFlaggedMessages);
            IRestResponse response = client.Execute(commentMessagesRequest);
            EndTimer(commentAllFlaggedMessages);

            return response;
        }

        // Mark a code for single BP, single message or mutiple messages
        public IRestResponse<Result> APICodeFlaggedMessage(List<FlaggedMessage> messageList, CodeID codeID, int BPindex, string comment, MessageListFilter messageListFilterData)
        {
            IRestRequest codeFlaggedMessageRequest = CreateNewReqest("Reviewer/CodeFlaggedMessages2", Method.POST);

            codeFlaggedMessageRequest.RequestFormat = DataFormat.Json;            

            List<FlaggedMsgMarkup> markUpList = new List<FlaggedMsgMarkup>();

            foreach (FlaggedMessage message in messageList)
            {
                FlaggedMsgMarkup markUp = new FlaggedMsgMarkup();

                //markUp.SampleRecIDs = message.FlaggedMessageInfoList.Select(info => info.SampleRecID).ToList();
                
                markUp.SampleRecIDs = new List<long>();

                markUp.SampleRecIDs.Add(message.FlaggedMessageInfoList[BPindex].SampleRecID);

                markUp.CodeID = ((int)codeID).ToString();

                markUp.Comment = comment;

                markUp.BusinessPolicyID = message.FlaggedMessageInfoList[BPindex].BPID.ToString();

                markUp.MessageID = message.MessageId;

                markUp.SamplePeriodIDs = new List<long>();

                markUp.SamplePeriodIDs.Add(message.FlaggedMessageInfoList[BPindex].SamplePeriodID);

                markUpList.Add(markUp);
            }

            object o = new { data = SimpleJson.SerializeObject(markUpList), filter = SimpleJson.SerializeObject(messageListFilterData) };

            codeFlaggedMessageRequest.AddJsonBody(o);

            codeFlaggedMessageRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
           
            IRestResponse<Result> response = client.Execute<Result>(codeFlaggedMessageRequest);            

            return response;
        }

        // Mark a code for single BP, single message or mutiple messages, get the action information from xml
        public IRestResponse<Result> APICodeFlaggedMessage(List<FlaggedMessage> messageList, Dictionary<string, VerifyMessage> messages, Dictionary<string, string> BPMap, MessageListFilter messageListFilterData)
        {
            IRestRequest codeFlaggedMessageRequest = CreateNewReqest("Reviewer/CodeFlaggedMessages2", Method.POST);

            codeFlaggedMessageRequest.RequestFormat = DataFormat.Json;           

            List<FlaggedMsgMarkup> markUpList = new List<FlaggedMsgMarkup>();

            foreach (FlaggedMessage message in messageList)
            {   
                string BPid;
                               
                FlaggedMsgMarkup markUp = new FlaggedMsgMarkup();

                VerifyMessage vMessage = null;

                messages.TryGetValue(message.MessageId.ToString(), out vMessage);

                BPMap.TryGetValue(vMessage.bp, out BPid);

                //markUp.SampleRecIDs = message.FlaggedMessageInfoList.Select(info => info.SampleRecID).ToList();

                markUp.SampleRecIDs = new List<long>();

                markUp.SamplePeriodIDs = new List<long>();

                List<FlaggedMessageInfo> targetMessageInfo = message.FlaggedMessageInfoList.Where(info => info.BPID == long.Parse(BPid)).ToList();

                for(int i = 0; i<targetMessageInfo.Count; i ++)
                {
                    markUp.SampleRecIDs.Add(targetMessageInfo[i].SampleRecID);

                    markUp.CodeID = "" + (int)vMessage.code;

                    markUp.Comment = vMessage.comment;

                    markUp.BusinessPolicyID = targetMessageInfo[i].BPID.ToString();

                    markUp.MessageID = message.MessageId;

                    markUp.SamplePeriodIDs.Add(targetMessageInfo[i].SamplePeriodID);

                    markUpList.Add(markUp);
                }
            }

            object o = new { data = SimpleJson.SerializeObject(markUpList), filter = SimpleJson.SerializeObject(messageListFilterData) };

            codeFlaggedMessageRequest.AddJsonBody(o);

            codeFlaggedMessageRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            IRestResponse<Result> response = client.Execute<Result>(codeFlaggedMessageRequest);

            return response;
        }

        // Mark a code for all BPs, single message or mutiple messages
        public IRestResponse<Result> APICodeFlaggedMessage(List<FlaggedMessage> messageList, CodeID codeID, string comment, MessageListFilter messageListFilterData)
        {
            IRestRequest codeFlaggedMessageRequest = CreateNewReqest("Reviewer/CodeFlaggedMessages2", Method.POST);

            codeFlaggedMessageRequest.RequestFormat = DataFormat.Json;

            List<FlaggedMsgMarkup> markUpList = new List<FlaggedMsgMarkup>();

            foreach (FlaggedMessage message in messageList)
            {
                for (int BPindex = 0; BPindex < message.FlaggedMessageInfoList.Count; BPindex++ )
                {

                    FlaggedMsgMarkup markUp = new FlaggedMsgMarkup();

                    markUp.SampleRecIDs = new List<long>();

                    markUp.SampleRecIDs.Add(message.FlaggedMessageInfoList[BPindex].SampleRecID);

                    markUp.CodeID = ((int)codeID).ToString();

                    markUp.Comment = comment;

                    markUp.BusinessPolicyID = message.FlaggedMessageInfoList[BPindex].BPID.ToString();

                    markUp.MessageID = message.MessageId;

                    markUp.SamplePeriodIDs = new List<long>();

                    markUp.SamplePeriodIDs.Add(message.FlaggedMessageInfoList[BPindex].SamplePeriodID);

                    markUpList.Add(markUp);
                }
            }

            object o = new { data = SimpleJson.SerializeObject(markUpList), filter = SimpleJson.SerializeObject(messageListFilterData) };

            codeFlaggedMessageRequest.AddJsonBody(o);

            codeFlaggedMessageRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            string codeFlaggedMessages2 = "CodeFlaggedMessages2";

            BeginTimer(codeFlaggedMessages2);
            IRestResponse<Result> response = client.Execute<Result>(codeFlaggedMessageRequest);
            EndTimer(codeFlaggedMessages2);

            return response;
        }

        // Mark a code for all BPs, single message or mutiple messages, get the action information from xml
        public IRestResponse<Result> APICodeFlaggedMessage(List<FlaggedMessage> messageList, Dictionary<string, VerifyMessage> messages, MessageListFilter messageListFilterData)
        {
            IRestRequest codeFlaggedMessageRequest = CreateNewReqest("Reviewer/CodeFlaggedMessages2", Method.POST);

            codeFlaggedMessageRequest.RequestFormat = DataFormat.Json;

            List<FlaggedMsgMarkup> markUpList = new List<FlaggedMsgMarkup>();

            foreach (FlaggedMessage message in messageList)
            {
                for (int BPindex = 0; BPindex < message.FlaggedMessageInfoList.Count; BPindex++)
                {

                    VerifyMessage vMessage = null;

                    messages.TryGetValue(message.MessageId.ToString(), out vMessage);
                    
                    FlaggedMsgMarkup markUp = new FlaggedMsgMarkup();

                    markUp.SampleRecIDs = new List<long>();

                    markUp.SampleRecIDs.Add(message.FlaggedMessageInfoList[BPindex].SampleRecID);

                    markUp.CodeID = ""+(int)vMessage.code;

                    markUp.Comment = vMessage.comment;

                    markUp.BusinessPolicyID = message.FlaggedMessageInfoList[BPindex].BPID.ToString();

                    markUp.MessageID = message.MessageId;

                    markUp.SamplePeriodIDs = new List<long>();

                    markUp.SamplePeriodIDs.Add(message.FlaggedMessageInfoList[BPindex].SamplePeriodID);

                    markUpList.Add(markUp);
                }
            }

            object o = new { data = SimpleJson.SerializeObject(markUpList), filter = SimpleJson.SerializeObject(messageListFilterData) };

            codeFlaggedMessageRequest.AddJsonBody(o);

            codeFlaggedMessageRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            string codeFlaggedMessages2 = "CodeFlaggedMessages2";

            BeginTimer(codeFlaggedMessages2);
            IRestResponse<Result> response = client.Execute<Result>(codeFlaggedMessageRequest);
            EndTimer(codeFlaggedMessages2);

            return response;
        }

        public IRestResponse<Result> APICodeAllFlaggedMessages(int[] BPIDs,CodeID codeID, string comment)
        {
            IRestRequest codeAllFlaggedMessagesRequest = CreateNewReqest("Reviewer/CodeAllFlaggedMessages", Method.POST);

            codeAllFlaggedMessagesRequest.RequestFormat = DataFormat.Json;

            object o = new { bpIds = BPIDs.ToList(), codeId = ((int)codeID).ToString(), comment = comment };

            codeAllFlaggedMessagesRequest.AddJsonBody(o);

            codeAllFlaggedMessagesRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            string codeAllFlaggedMessages = "CodeAllFlaggedMessages";

            BeginTimer(codeAllFlaggedMessages);
            IRestResponse<Result> response = client.Execute<Result>(codeAllFlaggedMessagesRequest);
            EndTimer(codeAllFlaggedMessages);

            return response;
        }

        public Dictionary<string, string> BPMap(string jsonString)
        {
            JsonObject result = SimpleJson.DeserializeObject<JsonObject>(jsonString);

            object name, id, bpFlags;

            result.TryGetValue("bpFlags", out bpFlags);

            JsonArray jArray = (JsonArray)bpFlags;

            Dictionary<string, string> resultMap = new Dictionary<string, string>();

            for (int i = 0; i < jArray.Count; i++)
            {

                string jString = jArray[i].ToString();

                result = SimpleJson.DeserializeObject<JsonObject>(jString);

                result.TryGetValue("Name", out name);

                result.TryGetValue("Id", out id);

                resultMap.Add(name.ToString(), id.ToString());

            }

            return resultMap;
        }

        public IRestResponse<Result> APICodeAllFlaggedMessages(List<int> BPIDs, string code, string comment)
        {
            IRestRequest codeAllFlaggedMessagesRequest = CreateNewReqest("Reviewer/CodeAllFlaggedMessages", Method.POST);

            codeAllFlaggedMessagesRequest.RequestFormat = DataFormat.Json;

            CodeID codeid = (CodeID)Enum.Parse(typeof(CodeID), code, true);

            int id = (int)codeid;

            object o = new { bpIds = BPIDs, codeId = id.ToString(), comment = comment };

            codeAllFlaggedMessagesRequest.AddJsonBody(o);

            codeAllFlaggedMessagesRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            string codeAllFlaggedMessages = "CodeAllFlaggedMessages";

            BeginTimer(codeAllFlaggedMessages);
            IRestResponse<Result> response = client.Execute<Result>(codeAllFlaggedMessagesRequest);
            EndTimer(codeAllFlaggedMessages);

            return response;
        }

        public IRestResponse APIGetIncomingMessagesCount(
           MessageListFilter filter
           )
        {
            IRestRequest getIncomingMessageCountRequest = CreateNewReqest("Reviewer/GetIncomingMessageCount", Method.POST);

            dynamic o = new { filter = SimpleJson.SerializeObject(filter) };

            getIncomingMessageCountRequest.AddJsonBody(o);

            getIncomingMessageCountRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            IRestResponse getIncomingMessageCountResponse = client.Execute(getIncomingMessageCountRequest);

            return getIncomingMessageCountResponse;
        }
        
        public Dictionary<string, VerifyMessage> collectMessagesFromXML(IEnumerable<XElement> enumElement)
        {
            Dictionary<string, VerifyMessage> result = new Dictionary<string, VerifyMessage>();

            foreach (XElement message in enumElement)
            {
                VerifyMessage m = new VerifyMessage();

                m.id = message.Attribute("messageId").Value;

                m.subject = message.Attribute("subject").Value;

                m.bp = message.Attribute("BP").Value;

                Enum.TryParse<CodeID>(message.Attribute("code").Value, true, out m.code);

                m.status = message.Attribute("status").Value;

                m.comment = message.Attribute("comment").Value;

                result.Add(m.id, m);

            }

            return result;
        }

        public Dictionary<string, VerifyMessage> collectCommentMessagesFromXML(IEnumerable<XElement> enumElement)
        {
            Dictionary<string, VerifyMessage> result = new Dictionary<string, VerifyMessage>();

            foreach (XElement message in enumElement)
            {
                VerifyMessage m = new VerifyMessage();

                m.id = message.Attribute("messageId").Value;

                m.subject = message.Attribute("subject").Value;

 //               m.status = message.Attribute("status").Value;

 //               m.comment = message.Attribute("comment").Value;

                result.Add(m.id, m);

            }

            return result;
        }

        public Dictionary<string, string> messageStatus(string jsonString)
        {

            JsonObject result = SimpleJson.DeserializeObject<JsonObject>(jsonString);

            object data, status, id;

            result.TryGetValue("data", out data);

            JsonArray jArray = (JsonArray)data;

            Dictionary<string, string> resultMap = new Dictionary<string, string>();

            for (int i = 0; i < jArray.Count; i++)
            {

                string jString = jArray[i].ToString();

                result = SimpleJson.DeserializeObject<JsonObject>(jString);

                result.TryGetValue("MessageId", out id);

                result.TryGetValue("Status", out status);

                if (resultMap.ContainsKey(id.ToString()))
                {
                    Assert.AreEqual(status, resultMap[id.ToString()], "The message status is incorrect");
                }
                else
                {
                    resultMap.Add(id.ToString(), status.ToString());
                }
            }

            return resultMap;
        }

        public Dictionary<string, string> RGMap(string jsonString)
        {
            JsonObject result = SimpleJson.DeserializeObject<JsonObject>(jsonString);

            object rgname, id, reviewgroupsoptions;

            result.TryGetValue("ReviewGroupsOptions", out reviewgroupsoptions);

            JsonArray jArray = (JsonArray)reviewgroupsoptions;

            Dictionary<string, string> resultMap = new Dictionary<string, string>();

            for (int i = 0; i < jArray.Count; i++)
            {

                string jString = jArray[i].ToString();

                result = SimpleJson.DeserializeObject<JsonObject>(jString);

                result.TryGetValue("label", out rgname);

                result.TryGetValue("value", out id);

                resultMap.Add(rgname.ToString(), id.ToString());

            }

            return resultMap;
        }

        public List<long> reviewGroupsList(string reviewGroups, Dictionary<string, string> rgMap)
        {
            List<long> rgList = new List<long>();

            string[] selectedReviewGroupList = reviewGroups.Split(new char[] { ',' });

            List<string> selectedReviewGroups = new List<string>();

            foreach (string rg in selectedReviewGroupList)
            {
                selectedReviewGroups.Add(rg);
            }

            foreach (string rg in selectedReviewGroups)
            {
                if (rgMap.ContainsKey(rg))
                {
                    string rgindex;

                    rgMap.TryGetValue(rg.ToString(), out rgindex);

                    rgList.Add(long.Parse(rgindex));
                }
            }
            return rgList;
        }

        #endregion

        #region Manager Report
        public IRestResponse<ReportDateRange> APIGetDefaultReportDates4Manager()
        {
            //GetDefaultReportDates4Manager

            var getDefaultReportDates4ManagerRequest = CreateNewReqest("Reports/GetDefaultReportDates4Manager", Method.GET);

            IRestResponse<ReportDateRange> getDefaultReportDates4ManagerResponse = client.Execute<ReportDateRange>(getDefaultReportDates4ManagerRequest);

            return getDefaultReportDates4ManagerResponse;
        }

        public IRestResponse<List<ReviewGroup>> APIGetReportReviewGroups4Manager(DateTime startDate, DateTime endDate)
        {
            //GetReportReviewGroups4Manager

            var getReportReviewGroups4ManagerRequest = CreateNewReqest(string.Format(@"Reports/GetReportReviewGroups4Manager?startDate={0}&endDate={1}", startDate.ToShortDateString(), endDate.ToShortDateString()), Method.POST);

            getReportReviewGroups4ManagerRequest.RequestFormat = DataFormat.Json;

            getReportReviewGroups4ManagerRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            IRestResponse<List<ReviewGroup>> getReportReviewGroups4ManagerResponse = client.Execute<List<ReviewGroup>>(getReportReviewGroups4ManagerRequest);

            return getReportReviewGroups4ManagerResponse;
        }

        public IRestResponse<List<ReviewGroup>> APIGetReportReviewGroups4Manager(DateTime startDate, DateTime endDate, List<BusinessPolicy> businessPolicies)
        {
            //GetReportReviewGroups4Manager

            var getReportReviewGroups4ManagerRequest = CreateNewReqest(string.Format(@"Reports/GetReportReviewGroups4Manager?startDate={0}&endDate={1}", startDate.ToShortDateString(), endDate.ToShortDateString()), Method.POST);

            object o = new { businessPolicies = SimpleJson.SerializeObject(businessPolicies) };

            getReportReviewGroups4ManagerRequest.AddJsonBody(o);
            
            getReportReviewGroups4ManagerRequest.RequestFormat = DataFormat.Json;

            getReportReviewGroups4ManagerRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            IRestResponse<List<ReviewGroup>> getReportReviewGroups4ManagerResponse = client.Execute<List<ReviewGroup>>(getReportReviewGroups4ManagerRequest);

            return getReportReviewGroups4ManagerResponse;
        }

        public IRestResponse<ReportBusinessPolicies> APIGetReportBusinessPolicies(DateTime startDate, DateTime endDate)
        {
            //GetReportBusinessPolicies

            var getReportBusinessPoliciesRequest = CreateNewReqest(string.Format(@"Reports/GetReportBusinessPolicies?startDate={0}&endDate={1}", startDate.ToShortDateString(), endDate.ToShortDateString()), Method.GET);

            getReportBusinessPoliciesRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            IRestResponse<ReportBusinessPolicies> getReportBusinessPoliciesResponse = client.Execute<ReportBusinessPolicies>(getReportBusinessPoliciesRequest);

            return getReportBusinessPoliciesResponse;
        }

        public IRestResponse<ReviewGroupMembers> APIGetReportReviewGroupMembers4Manager(DateTime startDate, DateTime endDate, List<ReviewGroup> reviewGroups)
        {
            //GetReportReviewGroupMembers4Manager

            var getReportReviewGroupMembers4ManagerRequest = CreateNewReqest("Reports/GetReportReviewGroupMembers4Manager", Method.POST);

            object o = new { startDate = startDate.ToLongDateString(), endDate = endDate.ToLongDateString(), reviewGroups = SimpleJson.SerializeObject(reviewGroups) };

            getReportReviewGroupMembers4ManagerRequest.AddJsonBody(o);

            getReportReviewGroupMembers4ManagerRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            getReportReviewGroupMembers4ManagerRequest.RequestFormat = DataFormat.Json;

            IRestResponse<ReviewGroupMembers> getReportReviewGroupMembers4ManagerResponse = client.Execute<ReviewGroupMembers>(getReportReviewGroupMembers4ManagerRequest);
            
            return getReportReviewGroupMembers4ManagerResponse;
        }

        public IRestResponse<ReportSupervisoryCodes> APIGetReportSupervisoryCodes(DateTime startDate, DateTime endDate, List<ReviewGroup> reviewGroups)
        {
            //GetReportSupervisoryCodes

            var getReportSupervisoryCodesRequest = CreateNewReqest("Reports/GetReportSupervisoryCodes", Method.POST);

            object o = new { startDate = startDate.ToLongDateString(), endDate = endDate.ToLongDateString(), reviewGroups = SimpleJson.SerializeObject(reviewGroups) };

            getReportSupervisoryCodesRequest.AddJsonBody(o);

            getReportSupervisoryCodesRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            getReportSupervisoryCodesRequest.RequestFormat = DataFormat.Json;

            IRestResponse<ReportSupervisoryCodes> getReportSupervisoryCodesResponse = client.Execute<ReportSupervisoryCodes>(getReportSupervisoryCodesRequest);
            
            return getReportSupervisoryCodesResponse;
        }

        public IRestResponse<ReviewGroupMembers> APIGetReportReviewGroupReviewers4Manager(DateTime startDate, DateTime endDate, List<ReviewGroup> reviewGroups)
        {
            //GetReportReviewGroupReviewers4Manager

            var getReportReviewGroupReviewers4ManagerRequest = CreateNewReqest("Reports/GetReportReviewGroupReviewers4Manager", Method.POST);

            object o = new { startDate = startDate.ToLongDateString(), endDate = endDate.ToLongDateString(), reviewGroups = SimpleJson.SerializeObject(reviewGroups) };

            getReportReviewGroupReviewers4ManagerRequest.AddJsonBody(o);

            getReportReviewGroupReviewers4ManagerRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            getReportReviewGroupReviewers4ManagerRequest.RequestFormat = DataFormat.Json;

            IRestResponse<ReviewGroupMembers> getReportReviewGroupReviewers4ManagerResponse = client.Execute<ReviewGroupMembers>(getReportReviewGroupReviewers4ManagerRequest);

            return getReportReviewGroupReviewers4ManagerResponse;
        }

        public IRestResponse<ReportResponse> APIGenerateManagerReport(ManagerReportType reportType, DateTime startDate, DateTime endDate, List<BusinessPolicy> bps, List<Member> groupMembers, List<ReviewGroup> reviewGroups, List<Member> reviewers,  List<SupervisoryCode> supervisoryCodes, DetailLevel detailLevel = DetailLevel.AllDetails, ColumnOrder columnOrder = ColumnOrder.BusinessPolicyFirst)
        {
            //GenerateManagerReport

            var generateManagerReportRequest = CreateNewReqest("Reports/GenerateManagerReport", Method.POST);

            generateManagerReportRequest.RequestFormat = DataFormat.Json;

            generateManagerReportRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            generateManagerReportRequest.AddQueryParameter("type", ((int)reportType).ToString());

            ManagerReportCriteria criterial = new ManagerReportCriteria();

            criterial.StartDate = startDate;

            criterial.EndDate = endDate;

            criterial.Groups = reviewGroups;

            criterial.Bps = bps;

            criterial.DetailLevel = (int)detailLevel;

            criterial.Members = groupMembers;

            criterial.Reviewers = reviewers;

            criterial.Codes = supervisoryCodes;

            criterial.ColumnOrder = (int)columnOrder;

            object o = new { criteria = SimpleJson.SerializeObject(criterial) };

            generateManagerReportRequest.AddJsonBody(o);

            IRestResponse<ReportResponse> generateManagerReportResponse = client.Execute<ReportResponse>(generateManagerReportRequest);

            return generateManagerReportResponse;
        }


        public IRestResponse APIGetReportData(string dataId)
        {
            //GetReportDataById

            var getReportDataRequest = new RestRequest(string.Format("Reports/Data/{0}",dataId), Method.GET);

            IRestResponse getReportDataResponse = client.Execute(getReportDataRequest);

            return getReportDataResponse;
        }

        #endregion

        #region Reviewer Report

        public IRestResponse<ReportDateRange> APIGetDefaultReportDates()
        {
            var getDefaultReportDatesRequest = CreateNewReqest("Reports/GetDefaultReportDates", Method.GET);

            IRestResponse<ReportDateRange> getDefaultReportDatesResponse = client.Execute<ReportDateRange>(getDefaultReportDatesRequest);

            return getDefaultReportDatesResponse;
        }

        public IRestResponse<List<ReviewGroup>> APIGetReportReviewGroups(DateTime startDate, DateTime endDate)
        {

            var getReportReviewGroupsRequest = CreateNewReqest(string.Format(@"Reports/GetReportReviewGroups?startDate={0}&endDate={1}", startDate.ToShortDateString(), endDate.ToShortDateString()), Method.GET);

            getReportReviewGroupsRequest.RequestFormat = DataFormat.Json;

            getReportReviewGroupsRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            IRestResponse<List<ReviewGroup>> getReportReviewGroupsResponse = client.Execute<List<ReviewGroup>>(getReportReviewGroupsRequest);

            return getReportReviewGroupsResponse;
        }

        //https://10.98.27.216/SupervisorWeb/Reports/GetReportReviewGroupMembers

        public IRestResponse<ReviewGroupMembers> APIGetReportReviewGroupMembers(DateTime startDate, DateTime endDate, List<ReviewGroup> reviewGroups)
        {
            //GetReportReviewGroupMembers

            var getReportReviewGroupMembersRequest = CreateNewReqest("Reports/GetReportReviewGroupMembers", Method.POST);

            object o = new { startDate = startDate.ToLongDateString(), endDate = endDate.ToLongDateString(), reviewGroups = SimpleJson.SerializeObject(reviewGroups) };

            getReportReviewGroupMembersRequest.AddJsonBody(o);

            getReportReviewGroupMembersRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            getReportReviewGroupMembersRequest.RequestFormat = DataFormat.Json;

            IRestResponse<ReviewGroupMembers> getReportReviewGroupMembersResponse = client.Execute<ReviewGroupMembers>(getReportReviewGroupMembersRequest);

            return getReportReviewGroupMembersResponse;
        }

        public IRestResponse<ReportResponse> APIGenerateReviewerReport(ManagerReportType reportType, DateTime startDate, DateTime endDate, List<BusinessPolicy> bps, List<Member> groupMembers, List<ReviewGroup> reviewGroups, List<Member> reviewers, List<SupervisoryCode> supervisoryCodes, DetailLevel detailLevel = DetailLevel.AllDetails, ColumnOrder columnOrder = ColumnOrder.BusinessPolicyFirst)
        {

            var generateReviewerReportRequest = CreateNewReqest("Reports/GenerateReviewerReport", Method.POST);

            generateReviewerReportRequest.RequestFormat = DataFormat.Json;

            generateReviewerReportRequest.AddHeader("X-Requested-With", "XMLHttpRequest");

            generateReviewerReportRequest.AddQueryParameter("type", ((int)reportType).ToString());

            ManagerReportCriteria criterial = new ManagerReportCriteria();

            criterial.StartDate = startDate;

            criterial.EndDate = endDate;

            criterial.Groups = reviewGroups;

            criterial.Bps = bps;

            criterial.DetailLevel = (int)detailLevel;

            criterial.Members = groupMembers;

            criterial.Reviewers = reviewers;

            criterial.Codes = supervisoryCodes;

            criterial.ColumnOrder = (int)columnOrder;

            object o = new { criteria = SimpleJson.SerializeObject(criterial) };

            generateReviewerReportRequest.AddJsonBody(o);

            IRestResponse<ReportResponse> generateReviewerReportResponse = client.Execute<ReportResponse>(generateReviewerReportRequest);

            return generateReviewerReportResponse;
        }

        #endregion

        #endregion

        #region RestSharp common


        protected string GetHeader(IRestResponse response, string headerName)
        {
            string headerValue = string.Empty;

            foreach (Parameter p in response.Headers)
            {
                if (p.Name == headerName)
                {
                    headerValue = p.Value.ToString();
                    break;
                }
            }

            return headerValue;
        }

        

        protected string SaveResponseAsFile(IRestResponse response)
        {
            string fileName = string.Empty;

            try
            {
                string contentDisposition = GetHeader(response, "Content-Disposition");
                //: attachment; filename="00000000C97E0CC3E74A57632F64BB0D268F285E95A6475100.smtp

                if (contentDisposition != string.Empty && contentDisposition.IndexOf('=') > 0)
                {
                    fileName = contentDisposition.Substring(contentDisposition.IndexOf('=') + 1).Replace("\"", "");

                    fileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName);
                }
                else
                {
                    fileName = System.IO.Path.GetTempFileName();
                }

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
                {
                    writer.Write(response.Content);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return fileName;
        }

        public DateTime ChangeStringToDate(string date)
        {
            string[] dateSplit = date.Split(new char[] { '-' });
            DateTime finalDate = new DateTime(int.Parse(dateSplit[0]), int.Parse(dateSplit[1]), int.Parse(dateSplit[2]));
            return finalDate;
        }

        #endregion
    }

}
