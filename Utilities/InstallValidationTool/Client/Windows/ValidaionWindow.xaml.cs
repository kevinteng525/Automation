using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Common.FileCommon;
using Microsoft.WindowsAPICodePack.Dialogs;
using RequestLib;
using RequestLib.Requests;

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for ValidaionWindow.xaml
    /// </summary>
    public partial class ValidaionWindow : Window
    {
        private BackgroundWorker bkDoValidation;

        private const string TempleteConfigPath = @"TemplateConfigs";

        private ValidationRequest request;

        private XElement verifyConfig;

        public ValidaionWindow()
        {
            InitializeComponent();

            this.DataContext = App.Context.CurrentMachine;

            // load default value
            txtVersion.Text = ConfigurationManager.AppSettings.Get("verify_defaultVersion");

            txtProgramFilePathX86.Text = App.Context.CurrentMachine.ProgramFilesX86;

            txtProgramData.Text = App.Context.CurrentMachine.ProgramData;

            txtSqlServer.Text = App.Context.CurrentMachine.ServerName;

            txtSqlServerUid.Text = ConfigurationManager.AppSettings.Get("verify_defaultSqlServerUsername");

            txtSQLServerPassword.Text = ConfigurationManager.AppSettings.Get("verify_defaultSqlServerPassword");

            // load templetes
            LoadTempleteConfigs();

            txtConfigPath.Text = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configs\VerifyItems.xml");

            radioComponents.IsChecked = true;
        }

        #region Validation Configs

        private void LoadTempleteConfigs()
        {
            var templateControls = from f in FileHelper.GetAllFiles(TempleteConfigPath)
                                   where f.Value.ToLower().EndsWith(".xml")
                                   select new CheckBox
                                   {
                                       Content = f.Value.TrimEnd(".xml".ToCharArray()),
                                       Margin = new Thickness(0, 5, 0, 5),
                                       Width = f.Value.TrimEnd(".xml".ToCharArray()).Length > 35 ? 500 : 250
                                   };

            foreach (var c in templateControls)
            {
                wpComponents.Children.Add(c);
                c.Checked += CheckChanged;
                c.Unchecked += CheckChanged;
            }
        }

        private static void MergeXML(XElement groupAll, XElement groupToAdd)
        {
            foreach (XElement groupInRoot2 in groupToAdd.Elements("Group"))
            {
                var query = from g in groupAll.Elements("Group")
                            where g.Attribute("type").Value == groupInRoot2.Attribute("type").Value
                            && g.Attribute("groupName").Value == groupInRoot2.Attribute("groupName").Value
                            select g;

                if (!query.Any())
                {
                    groupAll.Add(groupInRoot2);
                }
            }
        }

        private XElement LoadFromCustomized()
        {
            if(FileHelper.IsExistsFile(txtConfigPath.Text))
            {
                return XElement.Load(txtConfigPath.Text);
            }
            else
            {
                return new XElement("Verify");
            }
        }

        private XElement LoadFromComponents()
        {
            var root = new XElement("Verfiy");

            foreach (var check in wpComponents.Children)
            {
                var checkBox = check as CheckBox;

                if (checkBox.IsChecked != null && checkBox.IsChecked.Value)
                {
                    string filenName = string.Format(@"{0}\{1}.xml", TempleteConfigPath, checkBox.Content);

                    if (FileHelper.IsExistsFile(filenName))
                    {
                        XElement component = XElement.Load(filenName);

                        MergeXML(root, component);
                    }
                }
            }

            return root;
        }

        private void CheckChanged(object sender, RoutedEventArgs e)
        {
            onConfigChange();
        }

        private void txtConfigPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            onConfigChange();
        }

        private void onConfigChange()
        {
            verifyConfig = radioComponents.IsChecked.Value ? LoadFromComponents() : LoadFromCustomized();

            // Hide Advance Settings if not used
            string tempConfg = verifyConfig.ToString();

            // sql setting
            if (!tempConfg.Contains("SQLServerInstance") && !tempConfg.Contains("SQLServerUsername") && !tempConfg.Contains("SQLServerPassword"))
            {
                sp_advance_sqlSettings.Visibility = Visibility.Collapsed;
            }
            else
            {
                sp_advance_sqlSettings.Visibility = Visibility.Visible;
            }

            // program file X86 path
            if (!tempConfg.Contains("ProgramFilePathX86"))
            {
                gridProgramFileX86Path.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridProgramFileX86Path.Visibility = Visibility.Visible;
            }

            // app data path
            if (!tempConfg.Contains("ProgramDataPath"))
            {
                gridProgramDataPath.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridProgramDataPath.Visibility = Visibility.Visible;
            }

            spAdvance.Visibility = Visibility.Collapsed;

            foreach (UIElement ui in spAdvanceSettings.Children)
            {
                if (ui.Visibility == Visibility.Visible)
                {
                    spAdvance.Visibility = Visibility.Visible;
                    break;
                }
            }
        }

        private void btnSourceFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            var folder = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                Title = "Select Config File:",
                InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configs")
            };

            var result = folder.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                txtConfigPath.Text = folder.FileName;
            }
        }

        #endregion

        #region Do Validation

        private void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            // background worker
            bkDoValidation = new BackgroundWorker();
            bkDoValidation.DoWork += DoValidationOnDoWork;
            bkDoValidation.RunWorkerCompleted += DoValidationOnRunWorkerCompleted;
            bkDoValidation.WorkerSupportsCancellation = true;
            bkDoValidation.RunWorkerAsync();
        }

        private void DoValidationOnDoWork(object sender, DoWorkEventArgs e)
        {
            if (!verifyConfig.Elements().Any())
            {
                MessageBox.Show("Please select items to verify", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);

                bkDoValidation.CancelAsync();
                e.Cancel = true;

                return;
            }

            // Request Validation Service
            this.Dispatcher.Invoke
               (
                   new Action
                       (() =>
                       {
                           gridValidationResult.Visibility = Visibility.Collapsed;
                           btnVerify.Visibility = Visibility.Collapsed;
                           txtWaiting.Visibility = Visibility.Visible;

                           request = new ValidationRequest(App.Context.CurrentMachine.ServerIP, 5000)
                           {
                               View = (ResultView)Enum.Parse(typeof(ResultView), cmbResultView.Text, true),

                               Version = txtVersion.Text,
                               ValidationConfig = verifyConfig.ToString(),

                               ProgramFilePathX86 = txtProgramFilePathX86.Text,
                               ProgramDataPath = txtProgramData.Text,

                               SQLServerInstance = txtSqlServer.Text,
                               SQLServerUsername = txtSqlServerUid.Text,
                               SQLServerPassword = txtSQLServerPassword.Text,
                           };
                       }
                       )
                );

            e.Result = ValidationRequest.ToValidationGroup(request.RequestServer());
        }

        private void DoValidationOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }

            this.Dispatcher.Invoke
                (
                    new Action(() =>
                                {
                                    btnVerify.Visibility = Visibility.Visible;
                                    txtWaiting.Visibility = Visibility.Collapsed;
                                }
                              )
                );

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                request = null;

                return;
            }

            this.Dispatcher.Invoke
                (
                    new Action
                        (() =>
                        {
                            listResult.ItemsSource = e.Result as List<ValidationGroup>;

                            if (listResult.Items.Count > 0)
                            {
                                expanderSettings.IsExpanded = false;

                                gridValidationResult.Visibility = Visibility.Visible;
                            }
                        }
                        )
                );
        }

        #endregion

        private void ExportClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (request == null)
            {
                MessageBox.Show("No Result Data To Export!", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var export = new ExportWindow(request.ResultString) { Owner = this };
            export.ShowDialog();
        }
    }
}
