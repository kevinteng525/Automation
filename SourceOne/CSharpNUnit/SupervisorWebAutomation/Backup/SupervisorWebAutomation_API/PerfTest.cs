using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using SupervisorWebAutomation_API.Config;
using SupervisorWebAutomation_API.Common;
using Microsoft.VisualBasic.FileIO;
using System.Collections;
using System.IO;
namespace SupervisorWebAutomation_API
{

    [TestClass]
    public class PerformanceTest : BaseTest
    {
        private IRestResponse<MessageListFilter> defaultMessageListFilterResponse;
        private IRestResponse<PrepareMessageList> prepareMessageListResponse;
        private IRestClient tmpClient;
        private IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse;       

        static private string fileNameUsers = @"c:\users.txt";        
        static private string fileNameLOGIN_NULL = @"c:\LOGIN_NULL.txt";        
        static private string fileNameGDMLF_NULL = @"c:\GDMLF_NULL.txt";        
        static private string fileNameSMLF_NULL = @"c:\SMLF_NULL.txt";
        static private string fileNamePML_NULL = @"c:\PML_NULL.txt";
        static private string fileNameGFMBP_NULL = @"c:\GFMBP_NULL.txt";
        static private string fileNameGMPH_NULL = @"c:\GMPH_NULL.txt";
        static private string fileNameCSM_NULL = @"c:\CSM_NULL.txt";
        static private string fileNameFSM_NULL = @"c:\FSM_NULL.txt";
        static private string fileNameFCM_NULL = @"c:\FCM_NULL.txt";
        static private string fileNameFCALL_NULL = @"c:\FCALL_NULL.txt";
        static private string fileNamePrepare_NULL = @"c:\PREPARE_NULL.txt";
        static private string fileNameGetFlag_NULL = @"c:\GETFLAG_NULL.txt";
        static private string fileNameSleep = @"c:\sleep.txt";
        static private string fileNameSleepDone = @"c:\sleepdone.txt";
        static private string fileNameCount = @"c:\count.txt";           
        static private string fileNameGetFlag_count = @"c:\GetFlag_count.txt";        
        
        static Hashtable htFile = new Hashtable();
        
        static PerformanceTest(){
            string[] fileNames = { fileNameUsers, fileNameLOGIN_NULL, fileNameGDMLF_NULL, fileNameSMLF_NULL, fileNamePML_NULL, fileNameGFMBP_NULL, fileNameGMPH_NULL, fileNameCSM_NULL, fileNameFSM_NULL, fileNameFCM_NULL, fileNameFCALL_NULL, fileNamePrepare_NULL, fileNameGetFlag_NULL, fileNameSleep, fileNameSleepDone, fileNameCount, fileNameGetFlag_count};
            foreach(string fileName in fileNames) {
                htFile.Add(fileName, File.AppendText(fileName));
            }
        }                    

        static private Hashtable htClient = new Hashtable();
        static private Hashtable htDefaultMessageListFilterResponse = new Hashtable();
        static private Hashtable htPrepareMessageListResponse = new Hashtable();
        static private Hashtable htGetFlaggedMessagesByPageResponse = new Hashtable();
        static private int threadNum = -1;               
        
        [TestMethod]
        public void LoginAndPrepare()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {                           
                WriteLog(id, fileNameLOGIN_NULL);
                
                LoginPrepareMessage(id);
            }                     
        }       

        [TestMethod]
        public void GetDefaultMessageListFilter()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {                             
                WriteLog(id, fileNameGDMLF_NULL);                
                LoginPrepareMessage(id);
                
                return;
            }          

            //Get Default Message list Filter                                   
            defaultMessageListFilterResponse = APIGetDefaultMessageListFilter(true);

            string errorMsg = " Content:" + defaultMessageListFilterResponse.Content + " ErrorMsg: " + defaultMessageListFilterResponse.ErrorMessage + " ErrorExc: " + defaultMessageListFilterResponse.ErrorException;           
            
            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "DefaultMessageListFilter status code is not correct. Status code: " + defaultMessageListFilterResponse.StatusCode.ToString() + errorMsg); 
        }

        [TestMethod]
        public void SaveMessageListFilter()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {
                WriteLog(id, fileNameSMLF_NULL);                               
                LoginPrepareMessage(id);
              
                return;
            }        

            defaultMessageListFilterResponse = APIGetDefaultMessageListFilter(true);

            //Save Message List Filter            
            DateTime archiveStartDate = new DateTime(2015, 3, 29);
            DateTime archiveEndDate = new DateTime(2015, 6, 30);

            //Get random review group
            string rg = GetRandRGNo();
            IRestResponse SaveMessageListFilterResponse = APISaveMessageListFilter(defaultMessageListFilterResponse.Data, archiveStartDate, archiveEndDate, rg, 0, 0, false, true, false, true);

            string errorMsg = " Content:" + SaveMessageListFilterResponse.Content + " ErrorMsg: " + SaveMessageListFilterResponse.ErrorMessage + " ErrorExc: " + SaveMessageListFilterResponse.ErrorException;                       

            Assert.IsTrue(SaveMessageListFilterResponse.StatusCode == System.Net.HttpStatusCode.OK, "Save message list status code is not correct. Status code: " + SaveMessageListFilterResponse.StatusCode.ToString() + errorMsg);
        }

        [TestMethod]
        public void PrepareMessageList()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            PrepareMessageList(id);
        }            

        [TestMethod]
        public void GetFlaggedMessagesByPage()
        {
            //Get flagged messages by page            
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {                
                WriteLog(id, fileNameGFMBP_NULL);                              
                LoginPrepareMessage(id);
               
                return;
            }          

            prepareMessageListResponse = (IRestResponse<PrepareMessageList>)htPrepareMessageListResponse[id.ToString()];

            if (null == prepareMessageListResponse)
            {
                WriteLog(id, fileNamePrepare_NULL);
                PrepareMessage(id);
                return;
            }

            int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

            string range = GetRandFlagMessageRange();
            string sortCon = GetRandSortCondition();
            getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(range, totalMessageCount, sortCon, true);

            string errorMsg = " Content:" + getFlaggedMessagesByPageResponse.Content + " ErrorMsg: " + getFlaggedMessagesByPageResponse.ErrorMessage + " ErrorExc: " + getFlaggedMessagesByPageResponse.ErrorException;

            Assert.IsTrue(getFlaggedMessagesByPageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get Flagged Messages By Page status code is not correct. Status code: " + getFlaggedMessagesByPageResponse.StatusCode.ToString() + errorMsg);                         

            int count = ((List<FlaggedMessage>)getFlaggedMessagesByPageResponse.Data).Count;
            if (count > 0)
            {
                htGetFlaggedMessagesByPageResponse[id.ToString()] = getFlaggedMessagesByPageResponse;
            }
            else
            {                
                WriteLog(fileNameGetFlag_count, "GetFlagged: total: " + totalMessageCount.ToString() + " range:  " + range + " sort: " + sortCon, id);                       
            }
            
        }       
              
        [TestMethod]
        public void GetMessagePreviewAndHtmL()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {                
                WriteLog(id, fileNameGMPH_NULL);                               
                LoginPrepareMessage(id);
              
                return;
            }         

            getFlaggedMessagesByPageResponse = (IRestResponse<List<FlaggedMessage>>)htGetFlaggedMessagesByPageResponse[id.ToString()];

            if (null == getFlaggedMessagesByPageResponse)
            {
                WriteLog(id, fileNameGetFlag_NULL);
                PrepareMessage(id);
                return;
            }

            FlaggedMessage message = GetFlaggeMessage(getFlaggedMessagesByPageResponse);
            string sRIDs = GetSRIDstring(message);

            //Get message preview
            IRestResponse<MessagePreview> getMessagePreviewResponse = APIGetMessagePreview(message.MessageId, sRIDs);

            string errorMsg = " Content:" + getMessagePreviewResponse.Content + " ErrorMsg: " + getMessagePreviewResponse.ErrorMessage + " ErrorExc: " + getMessagePreviewResponse.ErrorException;            
            
            Assert.IsTrue(getMessagePreviewResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message preview status code is not correct. Status code: " + getMessagePreviewResponse.StatusCode.ToString() + errorMsg);

            //Get message preview HTML

            IRestResponse getMessagePreviewHtmlResponse = APIGetMessagePreviewHtml(message.MessageId, 0, sRIDs);

            errorMsg = " Content:" + getMessagePreviewHtmlResponse.Content + " ErrorMsg: " + getMessagePreviewHtmlResponse.ErrorMessage + " ErrorExc: " + getMessagePreviewHtmlResponse.ErrorException;            
            
            Assert.IsTrue(getMessagePreviewHtmlResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get message preview html status code is not correct. Status code: " + getMessagePreviewHtmlResponse.StatusCode.ToString() + errorMsg);
        }

        [TestMethod]
        public void CommentSingleMessage()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {               
                WriteLog(id, fileNameCSM_NULL);                                
                LoginPrepareMessage(id);
              
                return;
            }           

            getFlaggedMessagesByPageResponse = (IRestResponse<List<FlaggedMessage>>)htGetFlaggedMessagesByPageResponse[id.ToString()];

            if (null == getFlaggedMessagesByPageResponse)
            {
                WriteLog(id, fileNameGetFlag_NULL);
                PrepareMessage(id);
                return;
            }

            FlaggedMessage message = GetFlaggeMessage(getFlaggedMessagesByPageResponse);

            //Comment Single Message                      
            String timeStamp = DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff");
            IRestResponse commentMessageResponse = APICommentMessages((int)message.MessageId, "Comment a single message by Performance: " + timeStamp);

            string errorMsg = " Content:" + commentMessageResponse.Content + " ErrorMsg: " + commentMessageResponse.ErrorMessage + " ErrorExc: " + commentMessageResponse.ErrorException;                       

            Assert.IsTrue(commentMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Comment single Message status code is not correct. Status code: " + commentMessageResponse.StatusCode.ToString() + errorMsg);
        }           

        [TestMethod]
        public void FlagSingleMessage()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {               
                WriteLog(id, fileNameFSM_NULL);                
                LoginPrepareMessage(id);
              
                return;
            }           

            defaultMessageListFilterResponse = (IRestResponse<MessageListFilter>)htDefaultMessageListFilterResponse[id.ToString()];
            getFlaggedMessagesByPageResponse = (IRestResponse<List<FlaggedMessage>>)htGetFlaggedMessagesByPageResponse[id.ToString()];

            if (null == getFlaggedMessagesByPageResponse)
            {
                WriteLog(id, fileNameGetFlag_NULL);
                PrepareMessage(id);
                return;
            }

            FlaggedMessage message = GetFlaggeMessage(getFlaggedMessagesByPageResponse);

            //Flag single Message
            List<FlaggedMessage> messageList = new List<FlaggedMessage>();

            messageList.Add(message);

            String timeStamp = DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff");
            IRestResponse<Result> codeFlaggedMessageResponse = APICodeFlaggedMessage(messageList, CodeID.Reviewed, "Mark a single message as reviewed by Performance: " + timeStamp, defaultMessageListFilterResponse.Data);

            string errorMsg = " Content:" + codeFlaggedMessageResponse.Content + " ErrorMsg: " + codeFlaggedMessageResponse.ErrorMessage + " ErrorExc: " + codeFlaggedMessageResponse.ErrorException;            

            Assert.IsTrue(codeFlaggedMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Flag single message status code is not correct. Status code: " + codeFlaggedMessageResponse.StatusCode.ToString() + errorMsg);
        }

        [TestMethod]
        public void CommentMultiple_Two()
        {
            CommentMultiple(2);
        }
      
        [TestMethod]
        public void CommentMultiple_Ten()
        {
            CommentMultiple(10);
        }      

        [TestMethod]
        public void CommentMultiple_OneHundred()
        {
            CommentMultiple(100);
        }        

        [TestMethod]
        public void CommentMultiple_FiveHundred()
        {
            CommentMultiple(500);
        }

        [TestMethod]
        public void CommentAll()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {
                WriteLog(id, fileNameFCALL_NULL);
                LoginPrepareMessage(id);
               
                return;
            }          

            String timeStamp;

            //Comment All Messages
            timeStamp = DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff");
            IRestResponse commentAllMessageResponse = APICommentAllFlaggedMessages("Comment all messages by Performance: " + timeStamp);

            string errorMsg = " Content:" + commentAllMessageResponse.Content + " ErrorMsg: " + commentAllMessageResponse.ErrorMessage + " ErrorExc: " + commentAllMessageResponse.ErrorException;

            Assert.IsTrue(commentAllMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Comment All Messages status code is not correct. Status code: " + commentAllMessageResponse.StatusCode.ToString() + errorMsg);           
        }

        [TestMethod]
        public void FlagMultiple_Two()
        {
            FlagMultiple(2);
        }

        [TestMethod]
        public void FlagMultiple_Five()
        {
            FlagMultiple(5);
        }

        [TestMethod]
        public void FlagMultiple_Ten()
        {
            FlagMultiple(10);
        }

        [TestMethod]
        public void FlagMultiple_Twenty()
        {
            FlagMultiple(20);
        }

        [TestMethod]
        public void FlagMultiple_Fifty()
        {
            FlagMultiple(50);
        }

        [TestMethod]
        public void FlagMultiple_OneHundred()
        {
            FlagMultiple(100);
        }

        [TestMethod]
        public void FlagMultiple_ThreeHundred()
        {
            FlagMultiple(300);
        }

        [TestMethod]
        public void FlagMultiple_FiveHundred()
        {
            FlagMultiple(500);
        }                   

        [TestMethod]
        public void FlagAll()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {
                WriteLog(id, fileNameFCALL_NULL);
                LoginPrepareMessage(id);
               
                return;
            }                      

            prepareMessageListResponse = (IRestResponse<PrepareMessageList>)htPrepareMessageListResponse[id.ToString()];

            if (null == prepareMessageListResponse)
            {
                WriteLog(id, fileNamePrepare_NULL);
                PrepareMessage(id);
                return;
            }

            String timeStamp;          

            //Code all flagged messages         
            Dictionary<string, string> BPMapResult = new Dictionary<string, string>();
            BPMapResult = BPMap(prepareMessageListResponse.Content);

            List<int> bpIdList = new List<int>();

            string bpid;

            BPMapResult.TryGetValue("All", out bpid);

            if (bpid != null)
            {
                bpIdList.Add(int.Parse(bpid));
            }
            else
            {
                List<string> List = BPMapResult.Values.ToList();

                foreach (string l in List)
                {
                    bpIdList.Add(int.Parse(l));
                }
            }

            timeStamp = DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff");
            int[] bpIDs = bpIdList.ToArray();

            IRestResponse<Result> codeAllFlaggedMessagesResponse = APICodeAllFlaggedMessages(bpIDs, CodeID.OK, "Mark all messages as Reviewed by Performance: " + timeStamp);

            string errorMsg = " Content:" + codeAllFlaggedMessagesResponse.Content + " ErrorMsg: " + codeAllFlaggedMessagesResponse.ErrorMessage + " ErrorExc: " + codeAllFlaggedMessagesResponse.ErrorException;

            Assert.IsTrue(codeAllFlaggedMessagesResponse.StatusCode == System.Net.HttpStatusCode.OK, "Flag all Messages status code is not correct. Status code: " + codeAllFlaggedMessagesResponse.StatusCode.ToString() + errorMsg);

            //Prepare
            PrepareMessageList(id);
        }

        [TestMethod]
        public void LogoutWeb()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {
                LoginWeb(id);
            }

            Logout();
        }        

        public void CommentMultiple(int count)
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {                
                WriteLog(id, fileNameFCM_NULL);               
                LoginPrepareMessage(id);
               
                return;
            }          
            
            getFlaggedMessagesByPageResponse = (IRestResponse<List<FlaggedMessage>>)htGetFlaggedMessagesByPageResponse[id.ToString()];

            if (null == getFlaggedMessagesByPageResponse)
            {
                WriteLog(id, fileNameGetFlag_NULL);
                PrepareMessage(id);
                return;
            }           

            List<string> messageIds = new List<string>();
            List<FlaggedMessage> messageList = new List<FlaggedMessage>();            

            if (count >= 100)
            {
                GetMessageListAndID( messageIds, messageList, 100, getFlaggedMessagesByPageResponse);
            }
            else
            {
                GetMessageListAndID( messageIds, messageList, count, getFlaggedMessagesByPageResponse);
            }

            if (count > 100)
            {
                for (int j = 0; j < (count / 100 -1 ); j++)
                {
                    GetMessageListAndID(id, messageIds, messageList);
                }
            }                         

            //Comment multiple Messages   
            String timeStamp = DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff");
            IRestResponse batchCommentMessagesResponse = APIBatchCommentMessages("Comment " + count.ToString() + " Messages by Performance: " + timeStamp, messageIds);

            string errorMsg = " Content:" + batchCommentMessagesResponse.Content + " ErrorMsg: " + batchCommentMessagesResponse.ErrorMessage + " ErrorExc: " + batchCommentMessagesResponse.ErrorException;

            Assert.IsTrue(batchCommentMessagesResponse.StatusCode == System.Net.HttpStatusCode.OK, "Comment multiple messages status code is not correct. Status code: " + batchCommentMessagesResponse.StatusCode.ToString() + errorMsg);                       
        }

        public void FlagMultiple(int count)
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {
                WriteLog(id, fileNameFCM_NULL);
                LoginPrepareMessage(id);
                
                return;
            }          

            defaultMessageListFilterResponse = (IRestResponse<MessageListFilter>)htDefaultMessageListFilterResponse[id.ToString()];
            getFlaggedMessagesByPageResponse = (IRestResponse<List<FlaggedMessage>>)htGetFlaggedMessagesByPageResponse[id.ToString()];

            if (null == getFlaggedMessagesByPageResponse)
            {
                WriteLog(id, fileNameGetFlag_NULL);
                PrepareMessage(id);
                return;
            }

            List<string> messageIds = new List<string>();
            List<FlaggedMessage> messageList = new List<FlaggedMessage>();

            if (count >= 100)
            {
                GetMessageListAndID(messageIds, messageList, 100, getFlaggedMessagesByPageResponse);
            }
            else
            {
                GetMessageListAndID(messageIds, messageList, count, getFlaggedMessagesByPageResponse);
            }

            if (count > 100)
            {
                for (int j = 0; j < (count / 100 - 1); j++)
                {
                    GetMessageListAndID(id, messageIds, messageList);
                }
            }                                         

            //Flag multiple Messages                                    
            string timeStamp = DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff");
            IRestResponse<Result> codeFlaggedMessageResponse = APICodeFlaggedMessage(messageList, CodeID.Reviewed, "Mark " + count.ToString() + " messages as Reviewed by Performance: " + timeStamp, defaultMessageListFilterResponse.Data);

            string errorMsg = " Content:" + codeFlaggedMessageResponse.Content + " ErrorMsg: " + codeFlaggedMessageResponse.ErrorMessage + " ErrorExc: " + codeFlaggedMessageResponse.ErrorException;

            Assert.IsTrue(codeFlaggedMessageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Flag multiple messages status code is not correct. Status code: " + codeFlaggedMessageResponse.StatusCode.ToString() + errorMsg);
        }       

        private FlaggedMessage GetFlaggeMessage(IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse)
        {            
            List<FlaggedMessage> flaggedMessagesList = getFlaggedMessagesByPageResponse.Data;
            int index = GetRandCommentFlagIndex();
            FlaggedMessage message = flaggedMessagesList[index];
            return message;
        }

        private List<FlaggedMessage> GetFlaggedMessageList(int id)
        {
            prepareMessageListResponse = (IRestResponse<PrepareMessageList>)htPrepareMessageListResponse[id.ToString()];

            int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

            string range = GetRandFlagMessageRange();
            string sortCon = GetRandSortCondition();
            getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(range, totalMessageCount, sortCon, true);

            Assert.IsTrue(getFlaggedMessagesByPageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Flag Multiple : Get Flagged Messages By Page status code is not correct: " + getFlaggedMessagesByPageResponse.StatusCode.ToString());

            List<FlaggedMessage> flaggedMessagesList = getFlaggedMessagesByPageResponse.Data;
            return flaggedMessagesList;
        }      

        private void GetMessageListAndID(List<string> messageIds, List<FlaggedMessage> messageList, int count, IRestResponse<List<FlaggedMessage>> getFlaggedMessagesByPageResponse)
        {            
            List<FlaggedMessage> flaggedMessagesList;
            FlaggedMessage message;
            int randCount;          
          
            flaggedMessagesList = getFlaggedMessagesByPageResponse.Data;
            if (count == 100)
            {
                for (int i = 0; i < 100; i++)
                {                    
                    message = flaggedMessagesList[i];
                    messageIds.Add(message.MessageId.ToString());
                    messageList.Add(message);
                }
            }
            else
            { 
                for (int i = 0; i < count; i++)
                {
                    randCount = GetRandCommentFlagIndex();                                                         
                    message = flaggedMessagesList[randCount];
                    messageIds.Add(message.MessageId.ToString());
                    messageList.Add(message);                                                        
                }            
            }                    
        }

        private void GetMessageListAndID(int id, List<string> messageIds, List<FlaggedMessage> messageList)
        {
            List<FlaggedMessage> flaggedMessagesList;
            FlaggedMessage message;        
      
            flaggedMessagesList = GetFlaggedMessageList(id);

            int count = flaggedMessagesList.Count;
            while (count == 0)
            {
                WriteLog(fileNameGetFlag_count, "Flag Multiple: GetFlagged", id);
                flaggedMessagesList = GetFlaggedMessageList(id);
                count = flaggedMessagesList.Count;
            }                         

            for (int i = 0; i < 100; i++)
            {
                message = flaggedMessagesList[i];
                messageIds.Add(message.MessageId.ToString());
                messageList.Add(message);
            }           
        }

        private void LoginWeb(int id)
        {
            int sleepSec = GetRandSleepSec();
            WriteSleepLog(id, fileNameSleep, sleepSec.ToString());
            System.Threading.Thread.Sleep(sleepSec * 1000);

            threadNum++;
            WriteLog(id, fileNameSleepDone, sleepSec.ToString(), threadNum.ToString());

            //Login
            GetUserPWFromCSV();
            ArrayList al = null;                       
            
            WriteLog(id, fileNameUsers, threadNum.ToString());                                               
            
            al = GetRandUserPW(threadNum);

            IRestResponse responseLogin = GetLoginResponse(al[0].ToString(), al[1].ToString());
            Assert.IsTrue(responseLogin.StatusCode == System.Net.HttpStatusCode.OK, "Login status code is not correct: " + responseLogin.StatusCode.ToString());

            bool isSupAuthSet = IsSupAuthSet();
            Assert.IsTrue(isSupAuthSet, "Login: SupAuthSet is not correct");                       

            tmpClient = RestClient;
            if (!htClient.ContainsKey(id.ToString()))
            {
                htClient.Add(id.ToString(), tmpClient);
            }
            else
            {
                htClient[id.ToString()] = tmpClient;
            }                     
        }

        public void PrepareMessageList(int id)
        {
            RestClient = (IRestClient)htClient[id.ToString()];

            if (null == RestClient)
            {
                WriteLog(id, fileNamePML_NULL);
                LoginPrepareMessage(id);

                return;
            }

            defaultMessageListFilterResponse = (IRestResponse<MessageListFilter>)htDefaultMessageListFilterResponse[id.ToString()];

            DateTime archiveStartDate = new DateTime(2015, 3, 29);
            DateTime archiveEndDate = new DateTime(2015, 6, 30);

            //Get random review group
            List<long> selectedReviewGroup = GetRandReviewGroup();

            defaultMessageListFilterResponse.Data.ReviewGroups = selectedReviewGroup;

            //Prepare message list
            prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, archiveStartDate, archiveEndDate, 0, 0, false, true, true);

            string errorMsg = " Content:" + prepareMessageListResponse.Content + " ErrorMsg: " + prepareMessageListResponse.ErrorMessage + " ErrorExc: " + prepareMessageListResponse.ErrorException;

            Assert.IsTrue(prepareMessageListResponse.StatusCode == System.Net.HttpStatusCode.OK, "Prepare message list status code is not correct. Status code: " + prepareMessageListResponse.StatusCode.ToString() + errorMsg);

            htPrepareMessageListResponse[id.ToString()] = prepareMessageListResponse;

            //Get Flagged Message by page
            int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

            WriteLog(fileNameCount, selectedReviewGroup[0].ToString(), totalMessageCount.ToString(), id);

            string range = GetRandFlagMessageRange();
            string sortCon = GetRandSortCondition();
            getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(range, totalMessageCount, sortCon, true);

            errorMsg = " Content:" + getFlaggedMessagesByPageResponse.Content + " ErrorMsg: " + getFlaggedMessagesByPageResponse.ErrorMessage + " ErrorExc: " + getFlaggedMessagesByPageResponse.ErrorException;

            Assert.IsTrue(getFlaggedMessagesByPageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get Flagged Messages By Page status code is not correct. Status code: " + getFlaggedMessagesByPageResponse.StatusCode.ToString() + errorMsg);

            int count = ((List<FlaggedMessage>)getFlaggedMessagesByPageResponse.Data).Count;
            while (count == 0)
            {
                WriteLog(fileNameGetFlag_count, "PrepareMessageList: total: " + totalMessageCount.ToString() + " range:  " + range + " sort: " + sortCon, id);
                range = GetRandFlagMessageRange();
                sortCon = GetRandSortCondition();
                getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(range, totalMessageCount, sortCon, true);

                errorMsg = " Content:" + getFlaggedMessagesByPageResponse.Content + " ErrorMsg: " + getFlaggedMessagesByPageResponse.ErrorMessage + " ErrorExc: " + getFlaggedMessagesByPageResponse.ErrorException;

                Assert.IsTrue(getFlaggedMessagesByPageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get Flagged Messages By Page status code is not correct. Status code: " + getFlaggedMessagesByPageResponse.StatusCode.ToString() + errorMsg);

                count = ((List<FlaggedMessage>)getFlaggedMessagesByPageResponse.Data).Count;
            }

            htGetFlaggedMessagesByPageResponse[id.ToString()] = getFlaggedMessagesByPageResponse;
        }       
        
        private void PrepareMessage(int id)
        {
            //Get Default Message list Filter

            defaultMessageListFilterResponse = APIGetDefaultMessageListFilter(true);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, defaultMessageListFilterResponse.StatusCode, "Login: DefaultMessageListFilter status code is not correct: " + defaultMessageListFilterResponse.StatusCode.ToString());

            DateTime archiveStartDate = new DateTime(2015, 3, 29);
            DateTime archiveEndDate = new DateTime(2015, 6, 30);

            //Get random review group
            List<long> selectedReviewGroup = GetRandReviewGroup();

            defaultMessageListFilterResponse.Data.ReviewGroups = selectedReviewGroup;
            
            if (!htDefaultMessageListFilterResponse.ContainsKey(id.ToString()))
            {
                htDefaultMessageListFilterResponse.Add(id.ToString(), defaultMessageListFilterResponse);
            }
            else
            {
                htDefaultMessageListFilterResponse[id.ToString()] = defaultMessageListFilterResponse;
            }

            //Prepare message list           

            prepareMessageListResponse = APIPrepareMessageList(defaultMessageListFilterResponse.Data, archiveStartDate, archiveEndDate, 0, 0, false, true, true);           

            Assert.IsTrue(prepareMessageListResponse.StatusCode == System.Net.HttpStatusCode.OK, "Login: Prepare message list status code is not correct: " + prepareMessageListResponse.StatusCode.ToString());

            if (!htPrepareMessageListResponse.ContainsKey(id.ToString()))
            {
                htPrepareMessageListResponse.Add(id.ToString(), prepareMessageListResponse);
            }
            else
            {
                htPrepareMessageListResponse[id.ToString()] = prepareMessageListResponse;
            }

            //Get Flagged Message by page
            int totalMessageCount = prepareMessageListResponse.Data.TotalCount;

            WriteLog(fileNameCount, "login:" + selectedReviewGroup[0].ToString(), totalMessageCount.ToString(), id);

            string range = GetRandFlagMessageRange();
            string sortCon = GetRandSortCondition();
            getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(range, totalMessageCount, sortCon, true);

            Assert.IsTrue(getFlaggedMessagesByPageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get Flagged Messages By Page status code is not correct: " + getFlaggedMessagesByPageResponse.StatusCode.ToString());          

            int count = ((List<FlaggedMessage>)getFlaggedMessagesByPageResponse.Data).Count;
            while (count == 0)
            {
                WriteLog(fileNameGetFlag_count, "PreparMessage: total: " + totalMessageCount.ToString() + " range:  " + range + " sort: " + sortCon, id);                
                range = GetRandFlagMessageRange();
                sortCon = GetRandSortCondition();
                getFlaggedMessagesByPageResponse = APIGetFlaggedMessagesByPage(range, totalMessageCount, sortCon, true);

                Assert.IsTrue(getFlaggedMessagesByPageResponse.StatusCode == System.Net.HttpStatusCode.OK, "Get Flagged Messages By Page status code is not correct: " + getFlaggedMessagesByPageResponse.StatusCode.ToString());
                count = ((List<FlaggedMessage>)getFlaggedMessagesByPageResponse.Data).Count;
            }

            if (!htGetFlaggedMessagesByPageResponse.ContainsKey(id.ToString()))
            {
                htGetFlaggedMessagesByPageResponse.Add(id.ToString(), getFlaggedMessagesByPageResponse);
            }
            else
            {
                htGetFlaggedMessagesByPageResponse[id.ToString()] = getFlaggedMessagesByPageResponse;
            }                                     
        }

        public void LoginPrepareMessage(int id)
        {            
            LoginWeb(id);
            PrepareMessage(id);
        }

        private static void WriteLog(int id, string fileName)
        {
            StreamWriter w = (StreamWriter)htFile[fileName];
            w.WriteLine("Thread : " + id.ToString() + " Time: " + DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff"));
            w.Flush();          
        }

        private static void WriteLog(int id, string fileName, string threadNum)
        {            
            StreamWriter w = (StreamWriter)htFile[fileName];
            w.WriteLine("Thread count: " + threadNum + " , Thread : " + id.ToString() + " Time: " + DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff"));
            w.Flush();
        }

        private static void WriteLog(string fileName, string msg, int id)
        {
            StreamWriter w = (StreamWriter)htFile[fileName];
            w.WriteLine("Thread : " + id.ToString() + msg + " Time: " + DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff"));
            w.Flush();
        }

        private static void WriteSleepLog(int id, string fileName, string sec)
        {
            StreamWriter w = (StreamWriter)htFile[fileName];
            w.WriteLine("Sleep " + sec + " seconds, Thread : " + id.ToString() + " Time: " + DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff"));
            w.Flush();
        }

        private static void WriteLog(int id, string fileName, string sec, string threadNum) 
        {
            StreamWriter w = (StreamWriter)htFile[fileName];
            w.WriteLine("Done, " + "Thread count: " + threadNum + " Sleep " + sec + " seconds, Thread : " + id.ToString() + " Time: " + DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff"));
            w.Flush();
        }

        private static void WriteLog( string fileName, string rg, string count, int id)
        {
            StreamWriter w = (StreamWriter)htFile[fileName];
            w.WriteLine("Message Count: " + count + " Review group: " + rg + " Thread : " + id.ToString() + " Time: " + DateTime.Now.ToString("yyyy.MM.dd:HH:mm:ss:ffff"));
            w.Flush();
        }        
    }
}
