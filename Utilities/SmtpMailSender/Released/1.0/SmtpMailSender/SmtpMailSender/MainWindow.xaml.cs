using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Common.ADCommon;
using Forms = System.Windows.Forms;

namespace SmtpMailSender
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

            gridLogin.Visibility = Visibility.Visible;

            LayoutRoot.DataContext = App.MailSender;
        }

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

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string msg;

            if (!App.MailSender.DoVerify(out msg))
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
            App.MailSender.IsRunning = false;

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            if (e.Cancelled)
            {
                MessageBox.Show("User Canceled Sending Task");

                return;
            }

            MessageBox.Show
                (
                    string.Format(
                                    "{0}{1} mails have been sent!  {0}{2} mails failed!",
                                    App.NewLine,
                                    App.MailSender.MailsCount,
                                    App.MailSender.FailedCount
                                  ),
                    "DONE",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => { btnCancel.IsEnabled = true; }));

            App.MailSender.IsRunning = true;

            App.MailSender.FinishCount = 0;

            App.MailSender.FailedCount = 0;

            while(App.MailSender.FinishCount < App.MailSender.MailsCount)
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                int taskLeftNum = App.MailSender.MailsCount - App.MailSender.FinishCount;

                int threadsNum = taskLeftNum > App.MaxThreadsThreshold ? App.MaxThreadsThreshold : taskLeftNum;

                var tasks = new Task[threadsNum];

                for(int i = 0 ; i< threadsNum; i++)
                {
                    Action action =
                        () =>
                        {
                            try
                            {
                                App.MailSender.SendMail();
                            }
                            catch (Exception)
                            {
                                App.MailSender.FailedCount++;
                            }

                            App.MailSender.FinishCount++;
                        };

                    tasks[i] = Task.Factory.StartNew(action);
                }

                Task.WaitAll(tasks, 1000 * 30);
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ADHelper.ValidateDomainUser(App.MailSender.MailUser, App.MailSender.Password, App.MailSender.Domain))
                {
                    App.MailSender.GetMailboxes();

                    App.MailSender.SmtpServer = string.Format("{0}.{1}", "mail01", App.MailSender.Domain);

                    checkedListView.DataContext = App.MailSender.Mailboxes;

                    gridLogin.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Failed to login!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancelSend_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => { btnCancel.IsEnabled = false; }));

            backgroundWorker.CancelAsync();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnDictionarySelection(object sender, RoutedEventArgs e)
        {
            Forms.OpenFileDialog openFileDialog = new Forms.OpenFileDialog();
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Dictionary";
            openFileDialog.Title = "Select Dictionary File";
            openFileDialog.Filter = "txt files|*.txt";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "txt";

            Forms.DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                App.MailSender.DictionaryFile = openFileDialog.FileName;
            }
        }

        private void btnAttachmentsSelection(object sender, RoutedEventArgs e)
        {
            Forms.FolderBrowserDialog folderBrowserDialog = new Forms.FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory + "Attachments";
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.Description = "Select Attachments Folder";

            Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                App.MailSender.AttachmentDir = folderBrowserDialog.SelectedPath.Trim();
            }
        }

        private void FromSelection(object sender, RoutedEventArgs e)
        {
            checkedListView.SelectedItems.Clear();

            popMailboxSelection.IsOpen = true;

            txtType.Text = "Select Senders";

            foreach (string item in App.MailSender.Senders)
            {
                checkedListView.SelectedItems.Add(item);
            }
        }

        private void ToSelection(object sender, RoutedEventArgs e)
        {
            checkedListView.SelectedItems.Clear();

            popMailboxSelection.IsOpen = true;

            txtType.Text = "Select Receivers";

            foreach (string item in App.MailSender.Receivers)
            {
                checkedListView.SelectedItems.Add(item);
            }
        }

        private void OnUncheckItem(object sender, RoutedEventArgs e)
        {
            selectAll.IsChecked = false;
        }

        private void OnSelectAllChanged(object sender, RoutedEventArgs e)
        {
            if (selectAll.IsChecked.HasValue && selectAll.IsChecked.Value)
                checkedListView.SelectAll();
            else
                checkedListView.UnselectAll();
        }

        private void popOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtType.Text == "Select Senders")
            {
                App.MailSender.Senders.Clear();

                foreach (string item in checkedListView.SelectedItems)
                {
                    App.MailSender.Senders.Add(item);
                }

                txtFromCount.Text = App.MailSender.Senders.Count.ToString();
            }

            if (txtType.Text == "Select Receivers")
            {
                App.MailSender.Receivers.Clear();

                foreach (string item in checkedListView.SelectedItems)
                {
                    App.MailSender.Receivers.Add(item);
                }

                txtToCount.Text = App.MailSender.Receivers.Count.ToString();
            }

            popMailboxSelection.IsOpen = false;
        }

        private void popCancel_Click(object sender, RoutedEventArgs e)
        {
            popMailboxSelection.IsOpen = false;
        }
    }
}
