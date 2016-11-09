using System;
using System.Windows;
using System.Windows.Threading;
using System.Configuration;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string RequestServiceVersion = ConfigurationManager.AppSettings.Get("client_RequestServiceVersion");

        public static AppContext Context = new AppContext();

        public void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
        }
    }
}
