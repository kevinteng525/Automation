using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Forms = System.Windows.Forms;
using System.Threading.Tasks;
using System.ComponentModel;

namespace EMLMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();

            LayoutRoot.DataContext = App.MakerContext;
        }

        #region title bar

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BtnMinWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnCloseWindow(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        private void PathSelection(object sender, RoutedEventArgs e)
        {
            Forms.OpenFileDialog openFileDialog = new Forms.OpenFileDialog();
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Dictionary";
            openFileDialog.Title = "Select File";
            openFileDialog.Filter = "txt files|*.txt";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "txt";

            Forms.DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Control control = sender as Control;

                switch (control.Tag.ToString())
                {
                    case "SenderPath":
                        App.MakerContext.SenderFilePath = openFileDialog.FileName;
                        break;
                    case "RecipientPath":
                        App.MakerContext.RecipientFilePath = openFileDialog.FileName;
                        break;
                    case "SubjectPath":
                        App.MakerContext.SubjectFilePath = openFileDialog.FileName;
                        break;
                    case "BodyPath":
                        App.MakerContext.BodyFilePath = openFileDialog.FileName;
                        break;
                }
            }
        }

        private void DirSelection(object sender, RoutedEventArgs e)
        {
            Forms.FolderBrowserDialog folderBrowserDialog = new Forms.FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.Description = "Select Folder";

            Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Control control = sender as Control;

                switch (control.Tag.ToString())
                {
                    case "AttachmentPath":
                        App.MakerContext.AttachmentDir = folderBrowserDialog.SelectedPath.Trim();
                        break;
                    case "ExportPath":
                        App.MakerContext.EmlExportPath = folderBrowserDialog.SelectedPath.Trim();
                        break;
                }
            }
        }

        private void BtnStartMakeClick(object sender, RoutedEventArgs e)
        {
            string msg;

            if (!App.MakerContext.DoVerify(out msg))
            {
                MessageBox.Show(msg, "Check Settings!", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            if (MessageBox.Show(msg, "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                backgroundWorker = new BackgroundWorker();

                backgroundWorker.WorkerSupportsCancellation = true;

                backgroundWorker.RunWorkerAsync();

                backgroundWorker.DoWork += backgroundWorker_DoWork;

                backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            App.MakerContext.IsRunning = false;

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            if (e.Cancelled)
            {
                MessageBox.Show("User Canceled Task");

                return;
            }

            MessageBox.Show
                (
                    string.Format(
                                    "{0}{1} EML have been export!  {0}{2} mails failed!",
                                    App.NewLine,
                                    App.MakerContext.MailsCount,
                                    App.MakerContext.FailedCount
                                  ),
                    "DONE",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => { btnCancel.IsEnabled = true; }));

            App.MakerContext.IsRunning = true;

            App.MakerContext.FinishCount = 0;

            App.MakerContext.FailedCount = 0;

            while (App.MakerContext.FinishCount < App.MakerContext.MailsCount)
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                int taskLeftNum = App.MakerContext.MailsCount - App.MakerContext.FinishCount;

                int threadsNum = taskLeftNum > App.MaxThreadsQueue ? App.MaxThreadsQueue : taskLeftNum;

                var tasks = new Task[threadsNum];

                for (int i = 0; i < threadsNum; i++)
                {
                    Action action =
                        () =>
                        {
                            try
                            {
                                App.MakerContext.ExportEML();
                            }
                            catch (Exception)
                            {
                                App.MakerContext.FailedCount++;
                            }

                            App.MakerContext.FinishCount++;
                        };

                    tasks[i] = Task.Factory.StartNew(action);
                }

                Task.WaitAll(tasks, 1000 * 60 * 10);
            }
        }

        private void BtnCancelExportClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => { btnCancel.IsEnabled = false; }));

            backgroundWorker.CancelAsync();
        }
    }
}
