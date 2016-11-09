using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Forms = System.Windows.Forms;
using System.Threading.Tasks;
using System.ComponentModel;

namespace EMLMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly object Locker = new object();

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
            var openFileDialog = new Forms.OpenFileDialog
                                     {
                                         InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Dictionary",
                                         Title = "Select File",
                                         Filter = "txt files|*.txt",
                                         FileName = string.Empty,
                                         FilterIndex = 1,
                                         RestoreDirectory = true,
                                         DefaultExt = "txt"
                                     };

            Forms.DialogResult result = openFileDialog.ShowDialog();

            if (result == Forms.DialogResult.OK)
            {
                var control = sender as Control;

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
            var folderBrowserDialog = new Forms.FolderBrowserDialog
                                          {
                                              SelectedPath = AppDomain.CurrentDomain.BaseDirectory,
                                              ShowNewFolderButton = false,
                                              Description = "Select Folder"
                                          };

            Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == Forms.DialogResult.OK)
            {
                var control = sender as Control;

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

        #region BackGroudWork
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
                backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

                backgroundWorker.RunWorkerAsync();

                backgroundWorker.DoWork += BackgroundWorker_DoWork;

                backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => { btnCancel.IsEnabled = true; }));

            App.MakerContext.IsRunning = true;

            App.MakerContext.FinishCount = 0;

            App.MakerContext.FailedCount = 0;

            var tasks = new Task[App.ThreadsNum];

            for (int i = 0; i < App.ThreadsNum; i++)
            {
                Action action =
                    () =>
                    {
                        while (App.MakerContext.FinishCount < App.MakerContext.MailsCount)
                        {
                            if (backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;

                                return;
                            }

                            try
                            {
                                lock (Locker)
                                {
                                    if (App.MakerContext.FinishCount >= App.MakerContext.MailsCount)
                                    {
                                        return;
                                    }

                                    App.MakerContext.FinishCount++;
                                }

                                App.MakerContext.ExportEML();
                            }
                            catch (Exception)
                            {
                                App.MakerContext.FailedCount++;
                            }
                        }
                    };

                tasks[i] = Task.Factory.StartNew(action);
            }

            Task.WaitAll(tasks);
        }

        private void BtnCancelExportClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => { btnCancel.IsEnabled = false; }));

            backgroundWorker.CancelAsync();
        }
        #endregion
    }
}
