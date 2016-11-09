using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using System.IO;
using System.Collections;
using System.Data;

using Saber.TestEnvironment;

namespace Saber.TestData.EWS
{
    public class EWS
    {
        private SenderInfo info = new SenderInfo("es1service@usc.com",@"emcsiax@QA");
        private String defaultPWD = "emcsiax@QA";

        private ArrayList emailList = new ArrayList();
        private ArrayList meetingList = new ArrayList();
        private ArrayList taskList = new ArrayList();

        private static ExchangeVersion exServerVersion = ExchangeVersion.Exchange2007_SP1;
        //private ExchangeService service = new ExchangeService(exServerVersion);
        private ExchangeService service = null;

        private int emailInfoLength = 8;
        private int meetingInfoLength = 10;
        private int taskInfoLength = 9;

        public EWS()
        {
            service = Service.ConnectToService(info);
        }

        

        #region Email
        private void ScanEmailFromTSV(string tsvFile)
        {
            using (StreamReader reader = new StreamReader(tsvFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(@"::"))
                        continue;
                    else
                    {
                        ArrayList emailInfo = new ArrayList();
                        string[] email = line.Split(new char[] { (char)9 });
                        foreach (string info in email)
                        {
                            emailInfo.Add(info);
                        }
                        int remaining = emailInfoLength - email.Length;
                        for (int j = 0; j < remaining; j++)
                        {
                            emailInfo.Add("");
                        }
                        emailList.Add(emailInfo);                    
                    } 

                }
            }
        }
        
        public bool SendEmailByTSV(string tsvPath)
        {
            try
            {
                ScanEmailFromTSV(tsvPath);
                foreach (ArrayList emailinfo in emailList)
                {
                    SendEmail(emailinfo[0].ToString(), emailinfo[1].ToString(), emailinfo[2].ToString(),
                        emailinfo[3].ToString(), emailinfo[4].ToString(), emailinfo[5].ToString(),
                        emailinfo[6].ToString(), emailinfo[7].ToString());
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool SendEmailByMDB(string mdbPath)
        {
            try
            {
                MDBUtil mdb = new MDBUtil(mdbPath);
                string from = "";
                string to = "";
                string cc = "";
                string bcc = "";
                if (mdb.isMessageSuccess && mdb.isUserNameSuccess)
                {
                    for (int i = 0; i < mdb.MessageRowCount; i++)
                    {
                        from = mdb.GetEmailAddress(mdb.MessagesTable.Rows[i][mdb.From].ToString());
                        to = mdb.GetEmailAddress(mdb.MessagesTable.Rows[i][mdb.To].ToString());
                        cc = mdb.GetEmailAddress(mdb.MessagesTable.Rows[i][mdb.CC].ToString());
                        bcc = mdb.GetEmailAddress(mdb.MessagesTable.Rows[i][mdb.BCC].ToString());
                        SendEmail(from, to, cc, bcc, mdb.MessagesTable.Rows[i][mdb.Subject].ToString(), 
                            mdb.MessagesTable.Rows[i][mdb.Body].ToString());
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Send an email without attachments and using default password
        public void SendEmail(string from, string to, string cc, string bcc, string subject, string body)
        {
            SendEmail(from, to, cc, bcc, subject, body, null);
        }

        // Send an email using default password
        public void SendEmail(string from, string to, string cc, string bcc, string subject, string body, string attachments)
        {
            SendEmail(from, to, cc, bcc, subject, body, attachments,null);
        }

        // Send an email, if to/cc/bcc equals null, it will not set this value
        public void SendEmail(string from, string to, string cc, string bcc, string subject, string body, string attachments, string password)
        {
            from = ParseValue(from);
            to = ParseValue(to);
            cc = ParseValue(cc);
            bcc = ParseValue(bcc);
            subject = ParseValue(subject);
            body = ParseValue(body);
            attachments = ParseValue(attachments);
            password = ParseValue(password);

            if (password == null)
            {
                password = defaultPWD;
            }
            
            SenderInfo sender = new SenderInfo(from.Trim(), password);
            service = Service.ConnectToService(sender, new TraceListener());

            EmailMessage message = new EmailMessage(service);

            // Specify the email recipient, if containing mutliple recipients, it will split by ";"
            if (to != null)
            {
                if (to.Contains(";"))
                {
                    string[] toList = SplitToList(to);
                    foreach (string receipient in toList)
                    {
                        message.ToRecipients.Add(receipient.Trim());
                    }
                }
                else
                {
                    message.ToRecipients.Add(to.Trim());
                }
            }
            // Specify the email cc, if containing mutliple cc, it will split by ";"
            if (cc != null)
            {
                if (cc.Contains(";"))
                {
                    string[] ccList = SplitToList(cc);
                    foreach (string receipient in ccList)
                    {
                        message.CcRecipients.Add(receipient.Trim());
                    }
                }
                else
                {
                    message.CcRecipients.Add(cc.Trim());
                }
            }
            // Specify the email bcc, if containing mutliple bcc, it will split by ";"
            if (bcc != null)
            {
                if (bcc.Contains(";"))
                {
                    string[] bccList = SplitToList(bcc);
                    foreach (string receipient in bccList)
                    {
                        message.BccRecipients.Add(receipient.Trim());
                    }
                }
                else
                {
                    message.BccRecipients.Add(bcc.Trim());
                }
            }
            // Specify the email subject, body
            message.Subject = subject;

            message.Body = body;
            
            // Specify the email attachments
            if (attachments !=null)
            {
                if (attachments.Contains(";"))
                {
                    string[] attachmentList = SplitToList(attachments);
                    foreach (string attachment in attachmentList)
                    {
                        message.Attachments.AddFileAttachment(attachment.Trim());
                    }
                }
                else
                {
                    message.Attachments.AddFileAttachment(attachments.Trim());
                }
            }
            message.SendAndSaveCopy();
        }
        #endregion

        #region Meeting Request

        private void ScanMeetingFromTSV(string tsvFile)
        {
            using (StreamReader reader = new StreamReader(tsvFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(@"::"))
                        continue;
                    else
                    {
                        ArrayList meetingInfo = new ArrayList();
                        string[] meeting = line.Split(new char[] { (char)9 });
                        foreach (string info in meeting)
                        {
                            meetingInfo.Add(info);
                        }

                        int remaining = meetingInfoLength - meeting.Length;
                        for (int j = 0; j < remaining; j++)
                        {
                            meetingInfo.Add("");
                        }
                        meetingList.Add(meetingInfo);
                    }
                }
            }
        }


        public bool SendMeetingByTSV(string tsvPath)
        {
            try
            {
                ScanMeetingFromTSV(tsvPath);
                foreach (ArrayList meetingInfo in meetingList)
                {
                    SendMeeting(meetingInfo[0].ToString(), meetingInfo[1].ToString(), meetingInfo[2].ToString(),
                        meetingInfo[3].ToString(), meetingInfo[4].ToString(), meetingInfo[5].ToString(),
                        meetingInfo[6].ToString(), meetingInfo[7].ToString(), meetingInfo[8].ToString(),
                        meetingInfo[9].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        // Send an email without attachments and using default password
        public void SendMeeting(string from, string required, string optional, string start, string duration, string subject, string body, string location)
        {
            SendMeeting(from, required, optional, start, duration, subject, body, location, null);
        }

        // Send an email using default password
        public void SendMeeting(string from, string required, string optional, string start, string duration, string subject, string body, string location, string attachments)
        {
            SendMeeting(from, required, optional, start, duration, subject, body, location, attachments, null);
        }

        // Send an meeting request, if required/optional equals null, it will not set this value
        public void SendMeeting(string from, string required, string optional, string start, string duration, string subject, string body, string location, string attachments, string password)
        {
            from = ParseValue(from);
            required = ParseValue(required);
            optional = ParseValue(optional);
            start = ParseValue(start);
            duration = ParseValue(duration);
            subject = ParseValue(subject);
            body = ParseValue(body);
            location = ParseValue(location);
            attachments = ParseValue(attachments);
            password = ParseValue(password);

            if (password == null)
            {
                password = defaultPWD;
            }

            SenderInfo sender = new SenderInfo(from.Trim(), password);
            service = Service.ConnectToService(sender, new TraceListener());

            Appointment meeting = new Appointment(service);

            // Specify the meeting recipient, if containing mutliple recipients, it will split by ";"
            if (required != null)
            {
                if (required.Contains(";"))
                {
                    string[] toList = SplitToList(required);
                    foreach (string receipient in toList)
                    {
                        meeting.RequiredAttendees.Add(receipient.Trim());
                    }
                }
                else
                {
                    meeting.RequiredAttendees.Add(required.Trim());
                }
            }
            // Specify the meeting optional, if containing mutliple optional, it will split by ";"
            if (optional != null)
            {
                if (optional.Contains(";"))
                {
                    string[] ccList = SplitToList(optional);
                    foreach (string receipient in ccList)
                    {
                        meeting.OptionalAttendees.Add(receipient.Trim());
                    }
                }
                else
                {
                    meeting.OptionalAttendees.Add(optional.Trim());
                }
            }
            // Specify the meeting start time, if null, then set as Now.
            DateTime startTime;
            if (start != null)
            {
                startTime = Convert.ToDateTime(start);
            }
            else
            {
                startTime = DateTime.Now;
            }
            meeting.Start = startTime;
            // Specify the meeting end time, if null, then set as start time plus 1 hour.
            if (duration != null)
            {
                meeting.End = startTime.AddHours(int.Parse(duration));
            }
            else
            {
                meeting.End = startTime.AddHours(1);
            }
            // Specify the meeting subject, body, location
            meeting.Subject = subject;
            meeting.Body = body;
            meeting.Location = location;

            // Specify the meeting attachments
            if (attachments != null)
            {
                if (attachments.Contains(";"))
                {
                    string[] attachmentList = SplitToList(attachments);
                    foreach (string attachment in attachmentList)
                    {
                        meeting.Attachments.AddFileAttachment(attachment.Trim());
                    }
                }
                else
                {
                    meeting.Attachments.AddFileAttachment(attachments.Trim());
                }
            }
            meeting.Save(SendInvitationsMode.SendToAllAndSaveCopy);
        }
        #endregion

        #region Task
        private void ScanTaskFromTSV(string tsvFile)
        {
            using (StreamReader reader = new StreamReader(tsvFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(@"::"))
                        continue;
                    else
                    {
                        ArrayList taskInfo = new ArrayList();
                        string[] task = line.Split(new char[] { (char)9 });
                        foreach (string info in task)
                        {
                            taskInfo.Add(info);
                        }
                        int remaining = taskInfoLength - task.Length;
                        for (int j = 0; j < remaining; j++)
                        {
                            taskInfo.Add("");
                        }
                        taskList.Add(taskInfo);
                    }
                }
            }
        }

        public bool SendTaskByTSV(string tsvPath)
        {
            try
            {
                ScanTaskFromTSV(tsvPath);
                foreach (ArrayList taskInfo in taskList)
                {
                    SendTask(taskInfo[0].ToString(), taskInfo[1].ToString(), taskInfo[2].ToString(),
                        taskInfo[3].ToString(), taskInfo[4].ToString(), taskInfo[5].ToString(),
                        taskInfo[6].ToString(), taskInfo[7].ToString(), taskInfo[8].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Create a task without attachments and using default password
        public void SendTask(string from, string startDate, string dueDate, string subject, string body, string status, string percentComplete)
        {
            SendTask(from, startDate, dueDate, subject, body, status, percentComplete, null);
        }

        // Create a task using default password
        public void SendTask(string from, string startDate, string dueDate, string subject, string body, string status, string percentComplete, string attachments)
        {
            SendTask(from, startDate, dueDate, subject, body, status, percentComplete, attachments, null);
        }

        // Create a task, if to equals null, it will not set this value
        public void SendTask(string from, string startDate, string dueDate, string subject, string body, string status, string percentComplete, string attachments, string password)
        {
            from = ParseValue(from);
            startDate = ParseValue(startDate);
            dueDate = ParseValue(dueDate);
            subject = ParseValue(subject);
            body = ParseValue(body);
            
            percentComplete = ParseValue(percentComplete);
            attachments = ParseValue(attachments);
            password = ParseValue(password);

            if (password == null)
            {
                password = defaultPWD;
            }

            SenderInfo sender = new SenderInfo(from.Trim(), password);
            service = Service.ConnectToService( sender, new TraceListener());

            Task task = new Task(service);

            // Specify the task start time, if null, then set as Now.
            if (startDate != null)
            {
                task.StartDate = Convert.ToDateTime(startDate);
            }
            else
            {
                task.StartDate = DateTime.Now;
            }
            // Specify the task due date, if null, then set as Now.
            if (dueDate != null)
            {
                task.DueDate = Convert.ToDateTime(dueDate);
            }
            else
            {
                task.DueDate = DateTime.Now;
            }
            // Specify the task subject, body
            task.Subject = subject;
            task.Body = body;
            // Specify the task status, default is "Not Started"
            switch (status.ToLower())
            {
                case "not started":
                    task.Status = TaskStatus.NotStarted;
                    break;
                case "in progress":
                    task.Status = TaskStatus.InProgress;
                    break;
                case "completed":
                    task.Status = TaskStatus.Completed;
                    break;
                case "waiting on someone else":
                    task.Status = TaskStatus.WaitingOnOthers;
                    break;
                case "deferred":
                    task.Status = TaskStatus.Deferred;
                    break;
                default:
                    task.Status = TaskStatus.NotStarted;
                    break;
            }

            // Specify the task percent of complete, default is 0%
            if (percentComplete != null)
            {
                task.PercentComplete = double.Parse(percentComplete);
            }
            else
            {
                task.PercentComplete = 0;
            }
            // Specify the task attachments
            if (attachments != null)
            {
                if (attachments.Contains(";"))
                {
                    string[] attachmentList = SplitToList(attachments);
                    foreach (string attachment in attachmentList)
                    {
                        task.Attachments.AddFileAttachment(attachment.Trim());
                    }
                }
                else
                {
                    task.Attachments.AddFileAttachment(attachments.Trim());
                }
            }
            task.Save();
        }
        #endregion

        private string[] SplitToList(string value)
        {
            string[] list = value.Split(new char[] { ';' });
            return list;
        }

        private string ParseValue(string value)
        {
            if (value == "" || value == null || value.ToLower() == "null" || value.ToLower() == "none" || value.ToLower() == "n/a")
            {
                return null;
            }
            else
            {
                return value;
            }
        }

        private void Test()
        {
            
        }
    }
}
