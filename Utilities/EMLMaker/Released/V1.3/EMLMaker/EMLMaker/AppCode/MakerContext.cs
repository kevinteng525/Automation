using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Common.FileCommon;


namespace EMLMaker.AppCode
{
    public class MakerContext : INotifyPropertyChanged
    {
        #region Properties

        public IList<string> SubjectList;

        public IList<string> BodyList;

        public IList<string> Attachments;

        public IList<string> SenderList;

        public IList<string> RecipientList;

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

        private string subjectFilePath;

        public string SubjectFilePath
        {
            get { return subjectFilePath; }
            set
            {
                subjectFilePath = value;

                NotifyPropertyChanged("SubjectFilePath");
            }
        }

        private string bodyFilePath;

        public string BodyFilePath
        {
            get { return bodyFilePath; }
            set
            {
                bodyFilePath = value;

                NotifyPropertyChanged("BodyFilePath");
            }
        }

        private string senderFilePath;

        public string SenderFilePath
        {
            get { return senderFilePath; }
            set
            {
                senderFilePath = value;

                NotifyPropertyChanged("SenderFilePath");
            }
        }

        private string recipientFilePath;

        public string RecipientFilePath
        {
            get { return recipientFilePath; }
            set
            {
                recipientFilePath = value;

                NotifyPropertyChanged("RecipientFilePath");
            }
        }

        private string emlExportPath;

        public string EmlExportPath
        {
            get { return emlExportPath; }
            set
            {
                emlExportPath = value;

                NotifyPropertyChanged("EmlExportPath");
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

        private int recipientsCount;

        public int RecipientsCount
        {
            get { return recipientsCount; }
            set
            {
                recipientsCount = value;

                NotifyPropertyChanged("RecipientsCount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public MakerContext()
        {
            IsRunning = false;

            MailsCount = 100;

            FinishCount = 0;

            FailedCount = 0;

            SubjectLength = 1;

            SubjectFilePath = AppDomain.CurrentDomain.BaseDirectory + "Dictionary\\Subject.txt";

            BodyLength = 30;

            BodyFilePath = AppDomain.CurrentDomain.BaseDirectory + "Dictionary\\Body.txt";

            SenderFilePath = AppDomain.CurrentDomain.BaseDirectory + "Dictionary\\Sender.txt";

            RecipientsCount = 1;

            RecipientFilePath = AppDomain.CurrentDomain.BaseDirectory + "Dictionary\\Recipient.txt";

            EnableAttachments = false;

            AttachmentDir = AppDomain.CurrentDomain.BaseDirectory + "Attachments";

            EmlExportPath = AppDomain.CurrentDomain.BaseDirectory + "Exports";
        }

        public bool DoVerify(out string msg)
        {
            var confirm = new StringBuilder();

            #region sender
            if (!File.Exists(SenderFilePath))
            {
                msg = "SendersFilePath not exists";

                return false;
            }

            SenderList = TxtHelper.GetTxtByLines(SenderFilePath);

            if (SenderList.Count == 0)
            {
                msg = "no lines in SenderFilePath";

                return false;
            }
            #endregion

            #region recipient
            if (RecipientsCount <= 0)
            {
                msg = "Receivers not specified";

                return false;
            }
            confirm.AppendLine(string.Format("Receivers: {0}", RecipientsCount));

            if (!File.Exists(RecipientFilePath))
            {
                msg = "RecipientFilePath not exists";

                return false;
            }

            RecipientList = TxtHelper.GetTxtByLines(RecipientFilePath);

            if (RecipientList.Count == 0)
            {
                msg = "no lines in RecipientFilePath";

                return false;
            }
            #endregion

            #region subject
            if (SubjectLength <= 0)
            {
                msg = "Subject length must more than 0";

                return false;
            }
            confirm.AppendLine(string.Format("Subject length: {0} words", SubjectLength));

            if (!File.Exists(SubjectFilePath))
            {
                msg = "SubjectFilePath not exists";

                return false;
            }

            SubjectList = TxtHelper.GetTxtByLines(SubjectFilePath);

            if (SubjectList.Count == 0)
            {
                msg = "no lines in SubjectFilePath";

                return false;
            }
            #endregion

            #region body
            if (BodyLength <= 0)
            {
                msg = "Body length must more than 0";

                return false;
            }
            confirm.AppendLine(string.Format("Body length: {0} words", BodyLength));

            if (!File.Exists(BodyFilePath))
            {
                msg = "BodyFilePath not exists";

                return false;
            }

            BodyList = TxtHelper.GetTxtByLines(BodyFilePath);

            if (BodyList.Count == 0)
            {
                msg = "no lines in BodyFilePath";

                return false;
            }
            #endregion

            if (enableAttachments)
            {
                if (!Directory.Exists(AttachmentDir))
                {
                    msg = "Attachment path is not exists";

                    return false;
                }
            }

            Attachments = Directory.GetFiles(AttachmentDir);

            if (MailsCount <= 0)
            {
                msg = "Export mails must more than 0";

                return false;
            }
            confirm.AppendLine(string.Format("Export: {0} EML files", MailsCount));

            msg = confirm.ToString();

            FileHelper.CreateFolder(EmlExportPath);

            return true;
        }

        public void ExportEML()
        {
            Helper.ExportEML();
        }
    }
}
