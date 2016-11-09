using System.Windows;
using EMLMaker.AppCode;
using System.Configuration;

namespace EMLMaker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public const string NewLine = "\r\n";

        public static readonly int ThreadsNum = int.Parse(ConfigurationManager.AppSettings["threadsNum"]);

        public static readonly string SubjectSuffix = "[EML Maker V1.3]";

        public static MakerContext MakerContext = new MakerContext();
    }
}
