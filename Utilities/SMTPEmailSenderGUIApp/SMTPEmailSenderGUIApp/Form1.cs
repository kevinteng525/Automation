using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;

namespace SMTPEmailSenderGUIApp
{
    public partial class Form1 : Form
    {
        int mailboxNumForEach;
        int patternLength;
        int totalMailboxNum;
        int totalMailNum;
        string[] patterns;
        string[] attachmentsArray;
        int failedNum;
        string logfilepath;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void InitData()
        {
            //Read all the patterns from file.
            patterns = File.ReadAllLines(patternTextBox.Text);

            //Mail number for each pattern.
            mailboxNumForEach = 0;
            mailboxNumForEach = Decimal.ToInt32(mailboxlNum.Value);

            //Total mail box number.
            patternLength = patterns.Length;
            totalMailboxNum = mailboxNumForEach * patternLength;

            //Total mail number.
            totalMailNum = Decimal.ToInt32(totalMailNumUpDown.Value);
            
            //Get all the attachments path array.
            attachmentsArray = Directory.GetFiles(folderBrowserTextbox.Text);

            //init the progressbar.
            progressBar1.Maximum = totalMailNum;
            progressBar1.Step = 1;

            //log file path
            logfilepath = LogFilePathTextBox.Text;

            failedNum = 0;            
        }

        //Generate a big list to put all the email address in it.
        private string[] GenerateMailAddressArray()
        {
            List<string> addressList = new List<string>();
            string currentPattern = string.Empty;
            string mailAddress = string.Empty;
            for (int i = 0; i < patternLength; i++)
            {
                currentPattern = patterns[i];
                for (int j = 0; j < mailboxNumForEach; j++)
                {
                    //Construct the valid email address.
                    mailAddress = patterns[i] + string.Format("{0:000000}", j);
                    mailAddress = mailAddress + "@" + domainTextBox.Text;
                    //add the email address to the list.
                    addressList.Add(mailAddress);
                }
            }

            //Convert the list to an array, for the performance consideration.            
            return addressList.ToArray<string>();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (patternTextBox.Text == string.Empty || mailboxlNum.Text == string.Empty || domainTextBox.Text == string.Empty || folderBrowserTextbox.Text == string.Empty)
            {
                MessageBox.Show("Please input the needed information.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //Init
            InitData();
            //Disable all the input component before send mail.
            patternTextBox.Enabled = false;
            mailboxlNum.Enabled = false;
            domainTextBox.Enabled = false;
            folderBrowserTextbox.Enabled = false;
            totalMailNumUpDown.Enabled = false;
            csvButton.Enabled = false;
            folderBrowserButton.Enabled = false;
            sendButton.Enabled = false;

            //Generate the address Array.
            string[] mailAddressArray = GenerateMailAddressArray();


            //Send mail.
            SendEmails(mailAddressArray, attachmentsArray);

            //Enable all the input component after sent mail.
            patternTextBox.Enabled = true;
            mailboxlNum.Enabled = true;
            domainTextBox.Enabled = true;
            folderBrowserTextbox.Enabled = true;
            totalMailNumUpDown.Enabled = true;
            csvButton.Enabled = true;
            folderBrowserButton.Enabled = true;
            sendButton.Enabled = true;
        }

        
          
        private void SendEmails(string[] mailAddressArray, string[] attachmentsArray)
        {
            SmtpClient smtpClient = new SmtpClient("localhost");
            int totalAddressNumber = mailAddressArray.Length;
            int attachmentfilesNumber = attachmentsArray.Length;

            string [] mailContentSentencesArray = getMailContent();
            int mailContentSentencesNumber = mailContentSentencesArray.Length - 1;

            Random random = new Random();
            DateTime beginTime = DateTime.Now;
            
            for (int i = 0; i < totalMailNum; i++)
            {                
                //Add three receivers for each mail.
                string to = mailAddressArray[random.Next(0, totalAddressNumber - 1)]
                    + "," + mailAddressArray[random.Next(0, totalAddressNumber - 1)]
                    + "," + mailAddressArray[random.Next(0, totalAddressNumber - 1)];
                string from = mailAddressArray[random.Next(0, totalAddressNumber - 1)];
                MailMessage mailMessage = new MailMessage(from, to);
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = mailContentSentencesArray[random.Next(0, mailContentSentencesNumber)];
                mailMessage.Body = (new StringBuilder().Append(mailContentSentencesArray[random.Next(0, mailContentSentencesNumber)])
                    .Append(mailContentSentencesArray[random.Next(0, mailContentSentencesNumber)])
                    .Append(mailContentSentencesArray[random.Next(0, mailContentSentencesNumber)]))
                    .ToString();
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

                //20% of mail contains attachments.
                if (i % 10 == 1 || i % 10 == 2)
                {
                    mailMessage.Attachments.Add(new Attachment(attachmentsArray[random.Next(0, attachmentfilesNumber - 1)]));
                }
                                
                //smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                //smtpClient.SendAsync(mailMessage, "Mail" + i); 
                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    File.AppendAllText(logfilepath, ex.Message + "\n\n");
                    failedNum++;
                }
                progressBar1.PerformStep();
            }

            DateTime endTime = DateTime.Now;
            TimeSpan dur = endTime - beginTime;
            MessageBox.Show("All task finished in " + dur + ", failed number: " + failedNum, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            File.AppendAllText(logfilepath, "All task finished in " + dur + ", failed number: " + failedNum + ", Successfully sent number: " + (totalMailNum - failedNum));
            //reset
            failedNum = 0;
            progressBar1.Value = 0;           
        }

        //Generate a string array contains some sentences.
        private string[] getMailContent()
        {
            string content = @"Thanks for joining 2011 Q4 Women’s Afternoon Tea. The feedback we received was positive with 97% of attendees feeling satisfied with the content and the arrangement. Our guest speaker Helen Wang and Jasmine Yan shared their moving and inspiring stories. Balance work and life is never an easy task. It’s a choice. You have to discover your passion, understand what is important to you, prioritize your key role, and do not forget renewal yourself both mind and body. Hope you all had a great time. I want to take this opportunity to say a big thank you to Jack and Jasmine. Thanks for your preparation and your sharing! Your efforts make this event a success. Here is a friendly reminder. The Women’s Afternoon Tea starts at 2pm. Today’s guest speaker is Jenny Wang and Jasmine Yan. Please come to Perseverance to join the event. Look forward to seeing you then. In the meantime, we would like to draw your attention to the following guidelines regarding security and safety at any time. Your Discount Connection has negotiated exclusive offers with many top electronics merchants. Take advantage of employee pricing at some of the most popular ones below. Our use of the lab currently got off to a bumpy start because of connectivity issues. We should start by having you go through the approval process with IT in advance of using this site. I will ask Soma if he can share with you how to do that. Our use of the lab currently got off to a bumpy start because of connectivity issues. Social media has become a popular channel to spread news and engage with all of our audiences. Whether you use Social Media for business or for purely personal reasons, it's important to understand the notion of social etiquette. Please take a few minutes to watch the video and familiarize yourself with the key points. We should start by having you go through the approval process with IT in advance of using this site. I will ask Soma if he can share with you how to do that. Is there anything you need installed or configured in the on-premise systems? For example, we requested a server running Exchange and another server running IIS. Also do you need client systems to represent SharePoint end users? How early in your day would you be able to join a concall with Microsoft? They like to get this initial information from us but then follow up with a concall to ask questions. For example if we on the east coast did 8pm, then the MS west coast team would be 5pm, and it would be 9 AM your time. I’ll send the information I have just to get the process underway along with any additional testing that we would like to do with Office 365. I’m thinking that we may want to set up two SourceOne systems with email on one and SharePoint on the other. Or we give you the lab for 5 to 10 days and then have email do the next 5 days. The benefits of Social Media are great, but with great reward come great responsibility. With that in mind, I wanted to take a moment to introduce you to a new EMC Social Media educational video, as well as call your attention to our updated Social Media policy. If you have questions about Social Media at EMC, the best place to start is the Social Media Club on EMC|ONE. Here you'll find hundreds of employees talking about the opportunities afforded through Social Media. You can also use this space to connect with EMC's Social Engagement Team. So what are you waiting for? EMC Corporation is a global leader in enabling businesses and service providers to transform their operations and deliver IT as a service.";
            string[] sentencesArray = content.Split('.');
            int sentencesNum = sentencesArray.Length;
            for (int i = 0; i < sentencesNum; i++)
            {
                sentencesArray[i] += ".";
            }
            return sentencesArray;
        }


        private void csvButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.ShowDialog();
            patternTextBox.Text = openFileDialog1.FileName;
        }

        private void folderBrowserButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            folderBrowserTextbox.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}
