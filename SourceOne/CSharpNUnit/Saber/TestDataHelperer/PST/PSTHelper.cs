using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Redemption;

namespace Saber.TestData.PST
{
    public class PSTHelper
    {
        private static RDOSession session  = new RDOSession();
        private static RDOStore exchangeStore = null;
        private String pstFilePath = String.Empty;

        public PSTHelper(String pstFilePath)
        {
            this.pstFilePath = pstFilePath;
            session.Logon("EMCSourceOne","emcsiax@QA");
            foreach (RDOStore store in session.Stores)
            {
                if (store.StoreKind == TxStoreKind.skPrimaryExchangeMailbox)
                {
                    exchangeStore = store;
                }
            }          
        }

        private static RDOPstStore AddPSTStore(String pstFilePath)
        {
            return session.Stores.AddPSTStore(pstFilePath);
        }

        private static RDOPstStore AddPSTStoreWithPassword(String pstFilePath, String password)
        {
            return session.Stores.AddPstStoreWithPassword(pstFilePath, rdoStoreType.olStoreUnicode, Type.Missing, password);
        }

        private static void RemovePSTStore(String pstFilePath)
        {
            foreach (IRDOStore store in session.Stores)
            {
                if (store.StoreKind == TxStoreKind.skPstAnsi || store.StoreKind == TxStoreKind.skPstUnicode)
                {
                    IRDOPstStore s = store as IRDOPstStore;
                    if (s.PstPath == pstFilePath)
                    {
                        s.Remove();
                        return;
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        private static void CopyMailContentFromPST2ExchangeMailBox(RDOFolder pstFolder, RDOFolder exchangeFolder)
        {
            if (pstFolder == null || exchangeFolder == null)//no such folder in pst
            {
                return;
            }
            foreach (IRDOMail mail in pstFolder.Items)
            {
                mail.CopyTo(exchangeFolder);
            }
            foreach (RDOFolder f1 in pstFolder.Folders)
            {
                RDOFolder f2 = GetFolderOnExchange(f1.Name, exchangeFolder);
                if (null == f2)
                {
                    f2 = exchangeFolder.Folders.Add(f1.Name);
                }
                CopyMailContentFromPST2ExchangeMailBox(f1, f2);
            }
        }

        private static RDOFolder GetFolderOnExchange(String folderName, RDOFolder exchangeFolder)
        {
            foreach (RDOFolder folder in exchangeFolder.Folders)
            {
                if (folder.Name.ToLower() == folderName.ToLower())
                {
                    return folder;
                }
            }
            return null;
        }

        public void IngestPSTMails2MailBoxOfCurrentUser()
        {
            if (!System.IO.File.Exists(pstFilePath))
            {
                throw new Exception("File doesn't exist! Please check the file path: " + pstFilePath);
            }
            RDOPstStore pstStore = session.Stores.AddPSTStore(pstFilePath);
            //Inbox
            RDOFolder pstInbox = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderInbox);
            RDOFolder exchangeInbox = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderInbox);
            CopyMailContentFromPST2ExchangeMailBox(pstInbox, exchangeInbox);
            //Outbox
            RDOFolder pstOutbox = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderOutbox);
            RDOFolder exchangeOutbox = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderOutbox);
            CopyMailContentFromPST2ExchangeMailBox(pstOutbox, exchangeOutbox);
            //Drafts
            RDOFolder pstDraft = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderDrafts);
            RDOFolder exchangeDraft = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderDrafts);
            CopyMailContentFromPST2ExchangeMailBox(pstDraft, exchangeDraft);
            //Sent Mails
            RDOFolder pstSentItems = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderSentMail);
            RDOFolder exchangeSentitems = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderSentMail);
            CopyMailContentFromPST2ExchangeMailBox(pstSentItems, exchangeSentitems);
            //Delete Items
            RDOFolder pstDeletedItems = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderDeletedItems);
            RDOFolder exchangeDeletedItems = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderDeletedItems);
            CopyMailContentFromPST2ExchangeMailBox(pstDeletedItems, exchangeDeletedItems);
            //Junk Emails
            RDOFolder pstJunkEmails = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderJunk);
            RDOFolder exchangeJunkEmails = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderJunk);
            CopyMailContentFromPST2ExchangeMailBox(pstJunkEmails, exchangeJunkEmails);
            //RSS Feeds
            RDOFolder pstRSSFeeds = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderRssSubscriptions);
            RDOFolder exchangeRSSFeeds = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderRssSubscriptions);
            CopyMailContentFromPST2ExchangeMailBox(pstRSSFeeds, exchangeRSSFeeds);
            //Contacts
            RDOFolder pstContacts = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderContacts);
            RDOFolder exchangeContacts = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderContacts);
            CopyMailContentFromPST2ExchangeMailBox(pstContacts, exchangeContacts);
            //Calender
            RDOFolder pstCalender = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderCalendar);
            RDOFolder exchangeCalender = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderCalendar);
            CopyMailContentFromPST2ExchangeMailBox(pstCalender, exchangeCalender);
            //Tasks
            RDOFolder pstTask = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderTasks);
            RDOFolder exchangeTask = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderTasks);
            CopyMailContentFromPST2ExchangeMailBox(pstTask, exchangeTask);
            //Notes
            RDOFolder pstNotes = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderNotes);
            RDOFolder exchangeNotes = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderNotes);
            CopyMailContentFromPST2ExchangeMailBox(pstNotes, exchangeNotes);
            //Journals
            RDOFolder pstJournals = pstStore.GetDefaultFolder(rdoDefaultFolders.olFolderJournal);
            RDOFolder exchangeJournals = exchangeStore.GetDefaultFolder(rdoDefaultFolders.olFolderJournal);
            CopyMailContentFromPST2ExchangeMailBox(pstJournals, exchangeJournals);

            //TODO, add more folders that needed to import from PST

            //Remove the pst store
            RemovePSTStore(pstFilePath);
                
        }

        public void IngestPSTMails2MailBoxOfUser(String userInfo)
        {
            IRDOProfiles profiles = new RDOProfiles();
            
            String profileName = "Saber Temporary Profile";
            int count = profiles.Count;
            for (int i = 0; i < count; i++)
            {
                profiles.Item(i);
                
            }
        }
    }
}
