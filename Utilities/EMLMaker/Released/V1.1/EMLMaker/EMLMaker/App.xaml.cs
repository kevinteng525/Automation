using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using EMLMaker.AppCode;

namespace EMLMaker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string NewLine = "\r\n";

        public static readonly int ThreadsNum = 10;

        public static readonly string SubjectSuffix = "[EML Maker V1.0]";

        public static MakerContext MakerContext = new MakerContext();
    }
}
