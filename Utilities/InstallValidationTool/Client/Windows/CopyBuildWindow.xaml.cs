using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using Common.FileCommon;
using Common.ScriptCommon;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for CopyBuildWindow.xaml
    /// </summary>
    public partial class CopyBuildWindow : Window
    {
        public CopyBuildWindow()
        {
            InitializeComponent();

            txtSourcePath.Text = ConfigurationManager.AppSettings.Get("copy_sourceDefaultPath");

            txtTargetUsername.Text = ConfigurationManager.AppSettings.Get("copy_targetDefaultUsername");
            txtTargetPassword.Password = ConfigurationManager.AppSettings.Get("copy_targetDefaultPassword");
            txtTargetPath.Text = string.Format(ConfigurationManager.AppSettings.Get("copy_targetDefaultPath"), App.Context.CurrentMachine.ServerIP);
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            GrantAccess();

            var browseBtn = sender as Button;

            if (browseBtn != null)
            {
                var folder = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = "Select Path:",
                };

                switch (browseBtn.Tag.ToString())
                {
                    case "source":
                        folder.InitialDirectory = txtSourcePath.Text;
                        break;
                    case "target":
                        folder.InitialDirectory = txtTargetPath.Text;
                        break;
                }

                if (folder.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    switch (browseBtn.Tag.ToString())
                    {
                        case "source":
                            txtSourcePath.Text = folder.FileName;
                            break;
                        case "target":
                            txtTargetPath.Text = folder.FileName;
                            break;
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            GrantAccess();

            try
            {
                FileHelper.CopyDirectoryWithParentFolder(txtSourcePath.Text, txtTargetPath.Text);

                MessageBox.Show("Copy Complete!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GrantAccess()
        {
            if (spSourceCredential.IsEnabled && !string.IsNullOrWhiteSpace(txtSourceUsername.Text))
            {
                CMDScript.RumCmd("cmd.exe", string.Format(@"net  use {0} /delete", txtSourcePath.Text));
                CMDScript.RumCmd("cmd.exe", string.Format(@"net  use {0}  {1} /user:{2}", txtSourcePath.Text, txtSourcePassword.Password, txtSourceUsername.Text));
            }

            if (spTargetCredential.IsEnabled && !string.IsNullOrWhiteSpace(txtTargetUsername.Text))
            {
                CMDScript.RumCmd("cmd.exe", string.Format(@"net  use {0} /delete", txtTargetPath.Text));
                CMDScript.RumCmd("cmd.exe", string.Format(@"net  use {0}  {1} /user:{2}", txtTargetPath.Text, txtTargetPassword.Password, txtTargetUsername.Text));
            }
        }
    }
}
