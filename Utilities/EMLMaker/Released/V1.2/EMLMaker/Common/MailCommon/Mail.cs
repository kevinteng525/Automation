using System;
using System.Net.Mail;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Text;

namespace Common.MailCommon
{
    public class Mail
    {
        public static MailMessage CreateMessage
            (
                string mailSubject,
                string mailFrom,
                string[] toAddressesList,
                string[] ccAddressesList,
                string[] bccAddressesList,
                string mailBody,
                string[] attachmentsUrls,
                MailPriority priority
            )
        {
            MailMessage mailMessage = new MailMessage();

            if (toAddressesList != null && toAddressesList.Length > 0)
            {
                foreach (string t in toAddressesList)
                {
                    mailMessage.To.Add(t);
                }
            }
            else
            {
                throw new Exception("To list is empty");
            }

            if (ccAddressesList != null)
            {
                foreach (string address in ccAddressesList)
                {
                    mailMessage.CC.Add(address);
                }
            }

            if (bccAddressesList != null)
            {
                foreach (string address in bccAddressesList)
                {
                    mailMessage.Bcc.Add(address);
                }
            }

            // mail address，sender dispaly name，encoding
            mailMessage.From = new MailAddress(mailFrom, mailFrom, Encoding.UTF8);

            // mail subject
            mailMessage.Subject = mailSubject;

            // subject encoding
            mailMessage.SubjectEncoding = Encoding.UTF8;

            // mail content
            mailMessage.Body = mailBody;

            // mail body encoding
            mailMessage.BodyEncoding = Encoding.UTF8;

            // attachments
            if (attachmentsUrls != null)
            {
                foreach (string attachmentsUrl in attachmentsUrls)
                {
                    if (!string.IsNullOrEmpty(attachmentsUrl.Trim()))
                    {
                        mailMessage.Attachments.Add(new Attachment(attachmentsUrl));
                    }
                }
            }

            mailMessage.IsBodyHtml = true;

            // mail priority
            mailMessage.Priority = priority;

            return mailMessage;
        }

        public static void SaveToEml(MailMessage msg, string emlFileAbsolutePath, string emlName)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

            string fileName = Path.Combine(emlFileAbsolutePath, emlName);

            using (var ms = new MemoryStream())
            {
                Assembly assembly = typeof(SmtpClient).Assembly;
                Type tMailWriter = assembly.GetType("System.Net.Mail.MailWriter");
                object mailWriter = Activator.CreateInstance(tMailWriter, flags, null, new object[] { ms }, CultureInfo.InvariantCulture);
                msg.GetType().GetMethod("Send", flags).Invoke(msg, new[] { mailWriter, true });

                File.WriteAllText(fileName, Encoding.Default.GetString(ms.ToArray()), Encoding.Default);
            }
        }
    }
}
