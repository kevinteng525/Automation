using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.IO;


namespace Common.MailCommon
{
    public static class MailHelper
    {

        public static void sendMail(string smtpClient, string from, List<string> to, List<string> cc, string subject, string body, string attachment = "")
        {
            if (string.IsNullOrEmpty(smtpClient) || string.IsNullOrEmpty(from) || to == null || to.Count == 0)
            {
                throw new Exception("The smtpClient, from and to can not be null.");
            }

            MailMessage mail = new MailMessage();

            if (attachment != "")
            {
                Attachment att = new Attachment(attachment);
                mail.Attachments.Add(att);
            }

            mail.From = new MailAddress(from);
            //mail.From = new MailAddress("galaxy@emc.com");

            foreach (string mailAddress in to)
            {
                mail.To.Add(new MailAddress(mailAddress));
            }

            if (cc != null)
            {
                foreach (string mailAddress in cc)
                {
                    mail.CC.Add(new MailAddress(mailAddress));
                }
            }

            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;

            SmtpClient smtp = new SmtpClient(smtpClient);
            smtp.Send(mail);
        }
        
        public static void quickSendMail(string smtpClient, string from, string to, string subject, string body)
        {
            List<string> tos = new List<string>();
            tos.Add(to);
            sendMail(smtpClient, from, tos, null, subject, body);     
        }
        
    }
    
}
