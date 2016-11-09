using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Common.MailCommon;

namespace EMLMaker.AppCode
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

        public static void ExportEML()
        {
            string sender = RamdonGenerateString(App.MakerContext.SenderList, 1, "", false);

            string[] recipients = RamdonGenerateString(App.MakerContext.RecipientList, App.MakerContext.RecipientsCount, false);

            string mailSubject = RamdonGenerateString(App.MakerContext.SubjectList, App.MakerContext.SubjectLength, " ", false) + App.SubjectSuffix;

            string mailBody = RamdonGenerateString(App.MakerContext.BodyList, App.MakerContext.BodyLength, " ", false);

            string[] attachments = null;

            if (App.MakerContext.EnableAttachments)
            {
                attachments = RamdonGenerateString(App.MakerContext.Attachments, 1, false);
            }

            MailMessage msg = Mail.CreateMessage(mailSubject, sender, recipients, null, null, mailBody, attachments, MailPriority.Normal);

            string emlName = string.Format("{0}{1}.eml", DateTime.Now.ToString("yyyy-MM-dd."), Guid.NewGuid().ToString());

            Mail.SaveToEml(msg, App.MakerContext.EmlExportPath, emlName);
        }
    }
}
