using System;
using System.Net.Mail;

namespace Common.MailCommon
{
    public class Mail
    {
        private readonly string clientName;
        private readonly string clientAddress;
        private readonly string clientPassword;
        private readonly string smtpServerName;
        private readonly int port;
        private readonly bool enableSSL;

        public Mail(string clientName, string clientAddress, string clientPassword, string smtpServerName, int port, bool enableSSL)
        {
            this.clientName = clientName;
            this.clientAddress = clientAddress;
            this.clientPassword = clientPassword;
            this.smtpServerName = smtpServerName;
            this.port = port;
            this.enableSSL = enableSSL;
        }

        public Mail(string clientName, string clientAddress, string clientPassword, string smtpServerName)
        {
            this.clientName = clientName;
            this.clientAddress = clientAddress;
            this.clientPassword = clientPassword;
            this.smtpServerName = smtpServerName;
        }

        public void SendMail(string mailSubject,
            string[] toAddressesList,
            string[] ccAddressesList,
            string[] bccAddressesList,
            string mailBody,
            string[] attachmentsUrls,
            MailPriority priority)
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
            mailMessage.From = new MailAddress(clientAddress, clientName, System.Text.Encoding.UTF8);

            // mail subject
            mailMessage.Subject = mailSubject;

            // subject encoding
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

            // mail content
            mailMessage.Body = mailBody;

            // mail body encoding
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

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

            mailMessage.IsBodyHtml = false;

            // mail priority
            mailMessage.Priority = priority;

            SmtpClient client = new SmtpClient
            {
                Credentials = new System.Net.NetworkCredential(clientAddress, clientPassword),
                Host = smtpServerName,
            };

            // avoid high CPU
            client.ServicePoint.MaxIdleTime = 2;

            if (port > 0)
            {
                client.Port = port;
            }

            if (enableSSL)
            {
                client.EnableSsl = enableSSL;
            }

            try
            {
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                client.Dispose();
                mailMessage.Dispose();
            }
        }

        public void SendMail(string mailSubject, string[] toAddressesList, string mailBody)
        {
            SendMail(mailSubject, toAddressesList, null, null, mailBody, null, MailPriority.Normal);
        }

        public void SendMail(string mailSubject, string[] toAddressesList, string mailBody, MailPriority priority)
        {
            SendMail(mailSubject, toAddressesList, null, null, mailBody, null, priority);
        }

        public void SendMail(string mailSubject, string[] toAddressesList, string mailBody, string[] attachmentsUrls, MailPriority priority)
        {
            SendMail(mailSubject, toAddressesList, null, null, mailBody, attachmentsUrls, priority);
        }
    }
}
