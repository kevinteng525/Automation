using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using Common.ADCommon;
using Common.FileCommon;

namespace SmtpMailSender.AppCode
{
    public class MailSender : INotifyPropertyChanged
    {
        #region Properties

        public IList<string> Words;

        public IList<string> Attachments;

        public IList<string> Mailboxes;

        public ObservableCollection<string> Senders = new ObservableCollection<string>();

        public ObservableCollection<string> Receivers = new ObservableCollection<string>();

        private string mailUser;

        public string MailUser
        {
            get { return mailUser; }
            set
            {
                mailUser = value;

                NotifyPropertyChanged("MailUser");
            }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;

                NotifyPropertyChanged("Password");
            }
        }

        private string domain;

        public string Domain
        {
            get { return domain; }
            set
            {
                domain = value;

                NotifyPropertyChanged("Domain");
            }
        }

        private string smtpServer;

        public string SmtpServer
        {
            get { return smtpServer; }
            set
            {
                smtpServer = value;

                NotifyPropertyChanged("SmtpServer");
            }
        }

        private string attachmentDir;

        public string AttachmentDir
        {
            get { return attachmentDir; }
            set
            {
                attachmentDir = value;

                NotifyPropertyChanged("AttachmentDir");
            }
        }

        private string dictionaryFile;

        public string DictionaryFile
        {
            get { return dictionaryFile; }
            set
            {
                dictionaryFile = value;

                NotifyPropertyChanged("DictionaryFile");
            }
        }

        private bool isRunning;

        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;

                NotifyPropertyChanged("IsRunning");
            }
        }

        private bool enableAttachments;

        public bool EnableAttachments
        {
            get { return enableAttachments; }
            set
            {
                enableAttachments = value;

                NotifyPropertyChanged("EnableAttachments");
            }
        }

        private int mailsCount;

        public int MailsCount
        {
            get { return mailsCount; }
            set
            {
                mailsCount = value;

                NotifyPropertyChanged("MailsCount");
            }
        }

        private int finishCount;

        public int FinishCount
        {
            get { return finishCount; }
            set
            {
                finishCount = value;

                NotifyPropertyChanged("FinishCount");
            }
        }

        private int failedCount;

        public int FailedCount
        {
            get { return failedCount; }
            set
            {
                failedCount = value;

                NotifyPropertyChanged("FailedCount");
            }
        }

        private int subjectLength;

        public int SubjectLength
        {
            get { return subjectLength; }
            set
            {
                subjectLength = value;

                NotifyPropertyChanged("SubjectLength");
            }
        }

        private int bodyLength;

        public int BodyLength
        {
            get { return bodyLength; }
            set
            {
                bodyLength = value;

                NotifyPropertyChanged("BodyLength");
            }
        }

        #endregion

        public MailSender()
        {
            MailUser = "es1service";

            Password = "emcsiax@QA";

            Domain = "qaes1.com";

            EnableAttachments = false;

            IsRunning = false;

            FinishCount = 0;

            MailsCount = 1000;

            SubjectLength = 7;

            BodyLength = 100;

            AttachmentDir = AppDomain.CurrentDomain.BaseDirectory + "Attachments";

            DictionaryFile = AppDomain.CurrentDomain.BaseDirectory + "Dictionary\\EnglishWords.txt";
        }

        public void GetMailboxes()
        {
            Mailboxes = ADHelper.GetMailBox(domain, mailUser, password, App.MaxMailboxQuery);
        }

        public bool DoVerify(out string msg)
        {
            msg = string.Empty;

            StringBuilder confirm = new StringBuilder();

            if (string.IsNullOrWhiteSpace(SmtpServer))
            {
                msg = "Smtp Server not specified";

                return false;
            }
            confirm.AppendLine(string.Format("Smtp Server: {0}", SmtpServer));

            if (Senders.Count == 0)
            {
                msg = "Sends not specified";

                return false;
            }
            confirm.AppendLine(string.Format("Senders: {0}", Senders.Count));

            if (Receivers.Count == 0)
            {
                msg = "Receivers not specified";

                return false;
            }
            confirm.AppendLine(string.Format("Receivers: {0}", Receivers.Count));

            if (SubjectLength <= 0)
            {
                msg = "Subject length must more than 0";
                return false;
            }
            confirm.AppendLine(string.Format("Subject length: {0} words", SubjectLength));

            if (BodyLength <= 0)
            {
                msg = "Body length must more than 0";
                return false;
            }
            confirm.AppendLine(string.Format("Body length: {0} words", BodyLength));

            if (!File.Exists(DictionaryFile))
            {
                msg = "Words dictionary not exists";

                return false;
            }

            if (enableAttachments)
            {
                if (!Directory.Exists(AttachmentDir))
                {
                    msg = "Attachment path is not exists";

                    return false;
                }
            }

            if (MailsCount <= 0)
            {
                msg = "Random Send must more than 0";
                return false;
            }
            confirm.AppendLine(string.Format("Random Send: {0} mails", MailsCount));

            msg = confirm.ToString();

            return true;
        }

        public void SendMail()
        {
            Words = TxtHelper.GetTxtByLines(DictionaryFile);

            Attachments = Directory.GetFiles(App.MailSender.AttachmentDir);

            Helper.SendMail(SmtpServer, Password, Words, SubjectLength, BodyLength, Senders, Receivers, App.ReceiversCount);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
