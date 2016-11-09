using System.Configuration;
using System.Windows;
using SmtpMailSender.AppCode;

namespace SmtpMailSender
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly int MaxThreadsThreshold = int.Parse(ConfigurationManager.AppSettings["maxThreadsThreshold"]);

        public static readonly int MaxMailboxQuery = int.Parse(ConfigurationManager.AppSettings["maxMailboxQuery"]);

        public static readonly int ReceiversCount = int.Parse(ConfigurationManager.AppSettings["receiversCount"]);

        public static readonly string SubjectSuffix = ConfigurationManager.AppSettings["subjectSuffix"];

        public static readonly int AttachmentChance = int.Parse(ConfigurationManager.AppSettings["attachmentChance"]);

        public const string NewLine = "\r\n";

        public static MailSender MailSender = new MailSender();
    }
}
