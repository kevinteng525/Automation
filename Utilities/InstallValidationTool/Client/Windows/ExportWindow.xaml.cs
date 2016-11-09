using System;
using System.Text;
using System.Windows;
using Common.FileCommon;
using Microsoft.WindowsAPICodePack.Dialogs;
using RequestLib.Requests;

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow
    {
        private readonly string result;

        public ExportWindow(string result)
        {
            InitializeComponent();

            this.result = result;

            txtExportPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string contents;

                string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");


                if (ckLOG.IsChecked != null && ckLOG.IsChecked.Value)
                {
                    contents = ValidationRequest.ToLog(result);
                    fileName = fileName + ".log";
                }
                else if (ckCSV.IsChecked != null && ckCSV.IsChecked.Value)
                {
                    contents = ValidationRequest.ToCSV(result);
                    fileName = fileName + ".csv";
                }
                else
                {
                    contents = ValidationRequest.ToXML(result);
                    fileName = fileName + ".xml";
                }

                string exportFileName = System.IO.Path.Combine(txtExportPath.Text, fileName);

                TXTHelper.WriteTXTByLines(new[] { contents }, exportFileName, Encoding.UTF8);

                MessageBox.Show("Export to file successfully", "Export", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var folder = new CommonOpenFileDialog
                             {
                                 IsFolderPicker = true,
                                 Title = "Export File Path:",
                                 InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                             };

            if (folder.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtExportPath.Text = folder.FileName;
            }
        }
    }
}
