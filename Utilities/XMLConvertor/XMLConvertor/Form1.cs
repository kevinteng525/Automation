using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace XMLConvertor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Convert_Click(object sender, EventArgs e)
        {
            System.Xml.Xsl.XslCompiledTransform xslt = new System.Xml.Xsl.XslCompiledTransform();
            string xmlSourcePath = SourceXMLTextBox.Text;
            string xsltFilePath = XSLTFileTextBox.Text;
            if (xmlSourcePath == string.Empty || xsltFilePath == string.Empty)
            {
                MessageBox.Show("Please select the XML file and XSLT file path", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    xslt.Load(xsltFilePath);
                    xslt.Transform(xmlSourcePath, @"C:\Result.html");
                    MessageBox.Show("Convert successfully!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.FileName = "iexplore.exe";
                    info.Arguments = @"C:\Result.html";
                    info.WorkingDirectory = @"C:\";
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SelXMLBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.ShowDialog();
            SourceXMLTextBox.Text = openFileDialog1.FileName;
        }

        private void SelXSLTBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.ShowDialog();
            XSLTFileTextBox.Text = openFileDialog1.FileName;
        }
    }
}
