using System;
using System.Collections.Generic;
using Common.MailCommon;

namespace SmtpMailSender.AppCode
{
    public static class Helper
    {
        private static Random random = new Random();

        public static string[] RamdonGenerateString(IList<string> stringList, int length, bool isRepeated)
        {
            if (length > stringList.Count)
            {
                isRepeated = true;
            }

            string[] generateStringList = new string[length];

            List<int> exsistIndex = new List<int>(length);

            for (int i = 0; i < generateStringList.Length; i++)
            {
                int index = random.Next(0, stringList.Count);

                do
                {
                    if (isRepeated)
                    {
                        break;
                    }

                    index = random.Next(0, stringList.Count);

                } while (exsistIndex.Contains(index));

                exsistIndex.Add(index);

                generateStringList[i] = stringList[index];
            }

            return generateStringList;
        }

        public static string RamdonGenerateString(IList<string> stringList, int length, string spiltString, bool isRepeated)
        {
            string[] generateStringList = RamdonGenerateString(stringList, length, isRepeated);

            return string.Join(spiltString, generateStringList);
        }

        public static void SendMail
            (
                string smtpServer,
                string mailPassword,
                IList<string> wordsList, 
                int subjectLength, 
                int bodyLength, 
                IList<string> senders, 
                IList<string> receivers, 
                int receiversCount
            )
        {
            string mailSubject = RamdonGenerateString(wordsList, subjectLength, " ", false) + App.SubjectSuffix;

            string mailBody = RamdonGenerateString(wordsList, bodyLength, " ", false);

            string mailSenders = RamdonGenerateString(senders, 1, "", false);

            if (receiversCount <= 0)
            {
                receiversCount = 1;
            }

            receiversCount = receiversCount > receivers.Count ? receivers.Count : receiversCount;

            string[] mailReceivers = RamdonGenerateString(receivers, receiversCount, false);

            string[] attachments = null;

            if (App.MailSender.EnableAttachments)
            {
                int chance = new Random().Next(0, 101);

                if (chance <= App.AttachmentChance && App.MailSender.Attachments != null)
                {
                    attachments = RamdonGenerateString(App.MailSender.Attachments, 1, false);
                }
            }

            Mail mail = new Mail(mailSenders, mailSenders, mailPassword, smtpServer);

            mail.SendMail(mailSubject, mailReceivers, mailBody, attachments, System.Net.Mail.MailPriority.Normal);
        }
    }
}
