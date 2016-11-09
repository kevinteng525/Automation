using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using RequestLib.Installation;

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for InstallServiceWindow.xaml
    /// </summary>
    public partial class InstallServiceWindow
    {
        private readonly string action;

        private readonly BackgroundWorker performAction;

        public InstallServiceWindow(string action)
        {
            InitializeComponent();

            this.action = action;

            this.DataContext = App.Context.CurrentMachine;

            txtUsername.Text = ConfigurationManager.AppSettings.Get("service_targetDefaultUsername");
            txtPassword.Password = ConfigurationManager.AppSettings.Get("service_targetDefaultPassword");
            txtSourcePath.Text = System.IO.Path.GetFullPath(ConfigurationManager.AppSettings.Get("service_sourceInstallPath"));
            txtTargetPath.Text = ConfigurationManager.AppSettings.Get("service_targetInstallPath");

            switch (action)
            {
                case "Uninstall":
                    Title = "Uninstall Service";
                    spSourcePath.IsEnabled = false;
                    break;

                case "Re-Install":
                    Title = "Re-Install Service";
                    break;
            }

            performAction = new BackgroundWorker();
            performAction.DoWork += performAction_DoWork;
            performAction.RunWorkerCompleted += performAction_RunWorkerCompleted;
            performAction.WorkerSupportsCancellation = true;
        }

        private void confrim_Click(object sender, RoutedEventArgs e)
        {
            performAction.RunWorkerAsync();
        }

        void performAction_DoWork(object sender, DoWorkEventArgs e)
        {
            ValidationService service = null;

            this.Dispatcher.Invoke
                (
                    new Action
                        (
                            () =>
                            {
                                this.spInstallSettings.IsEnabled = false;
                                this.spConfirm.Visibility = Visibility.Collapsed;
                                this.gridProcess.Visibility = Visibility.Visible;

                                service = new ValidationService(txtServerIP.Text, txtUsername.Text, txtPassword.Password, txtSourcePath.Text, txtTargetPath.Text, txtTargeShare.Text);
                            }
                        )
                );

            switch (action)
            {
                case "Install":
                    e.Result = service.InstallWinService();
                    break;

                case "Uninstall":
                    e.Result = service.UninstallWinService();
                    break;

                case "Re-Install":
                    e.Result = service.ReinstallWinService();
                    break;
            }
        }

        void performAction_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.Invoke
                (
                    new Action
                        (() =>
                            {
                                this.spInstallSettings.IsEnabled = true;
                                this.spConfirm.Visibility = Visibility.Visible;
                                this.gridProcess.Visibility = Visibility.Collapsed;
                            }
                        )
                );

            if (e.Cancelled)
            {
                MessageBox.Show("Canceled Action");
            }
            else if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e.Result == null || !(bool)e.Result)
            {
                MessageBox.Show(string.Format("{0} failed!", action), "Fail", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(string.Format("{0} success!", action), "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSourceFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            var folder = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Source Folder:",
                InitialDirectory = txtSourcePath.Text
            };

            if (folder.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtSourcePath.Text = folder.FileName;
            }
        }

        private void txtTargetPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtTargeShare.Text = txtTargetPath.Text.Replace(":", "$");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (performAction != null && performAction.IsBusy)
            {
                e.Cancel = true;
            }
        }
    }
}
