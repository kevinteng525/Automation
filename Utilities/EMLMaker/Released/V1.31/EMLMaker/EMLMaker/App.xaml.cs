using System.Windows;
using EMLMaker.AppCode;

namespace EMLMaker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public const string NewLine = "\r\n";

        public static readonly int ThreadsNum = 10;

        public static readonly string SubjectSuffix = "[EML Maker V1.3]";

        public static MakerContext MakerContext = new MakerContext();
    }
}
