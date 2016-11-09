using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.DirectoryServices;
using System.Xml;
using System.Reflection;

namespace S1Tool
{
    public partial class WebConfigDebugOn : Form
    {
        public WebConfigDebugOn()
        {
            InitializeComponent();

            try
            {
                LoadWebConfigs();

                foreach (KeyValuePair<string, string> webConfig in _webConfigs)
                {
                    _chkListBoxWebConfigs.Items.Add(webConfig.Key, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error on load!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Dictionary<string, string> _webConfigs = new Dictionary<string, string>();

        private void LoadWebConfigs()
        {
            // get all roots
            List<string> iisRootPaths = this.GetIisRootPaths();
            string netFramworkRootPath = this.GetNetFramworkRootPath();
            string spRootPath = this.GetSPRootPath();

            List<string> roots = new List<string>();
            roots.AddRange(iisRootPaths);
            roots.Add(netFramworkRootPath);
            roots.Add(spRootPath);

            // get all web.config
            foreach (string root in roots)
            {
                if (string.IsNullOrEmpty(root) == false && Directory.Exists(root))
                {
                    string[] strArray = Directory.GetFiles(root, "web.config", SearchOption.AllDirectories);
                    foreach (string webConfig in strArray)
                    {
                        _webConfigs[webConfig] = null;
                    }
                }
            }
        }

        private void DebugOnButton_Click(object sender, EventArgs e)
        {
            DebugON(true);
        }

        private void DebugOffButton_Click(object sender, EventArgs e)
        {
            DebugON(false);
        }

        private void DebugON(bool debugon)
        {
            try
            {
                // do it
                List<string> doneList = new List<string>();
                List<string> failedList = new List<string>();
                foreach (string file in _chkListBoxWebConfigs.CheckedItems)
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(file);

                        string backupPath = file + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss_") + (debugon ? "DebugOn" : "DebugOff");
                        doc.Save(backupPath);

                        XmlNode customErrors = doc.SelectSingleNode("//customErrors[@mode]");
                        XmlNode SafeMode = doc.SelectSingleNode("//SafeMode[@CallStack]");
                        XmlNode compilation = doc.SelectSingleNode("//compilation[@debug]");

                        if (customErrors != null)
                        {
                            try
                            {
                                customErrors.Attributes["mode"].Value = debugon ? "Off" : "On";
                            }
                            catch { }
                        }
                        if (SafeMode != null)
                        {
                            try
                            {
                                SafeMode.Attributes["CallStack"].Value = debugon ? "true" : "false";
                            }
                            catch { }
                        }
                        if (compilation != null)
                        {
                            try
                            {
                                compilation.Attributes["debug"].Value = debugon ? "true" : "false";
                            }
                            catch { }
                        }

                        doc.Save(file);
                        doneList.Add(file);
                    }
                    catch (Exception ex)
                    {
                        failedList.Add(file + ", ex: " + ex.Message);
                    }
                }
                MessageBox.Show("Well Done: " + doneList.Count + "\r\nFailed: " + failedList.Count, "Result", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand);
            }
        }

        private List<string> GetIisRootPaths()
        {
            List<string> list = new List<string>();

            try
            {
                DirectoryEntry entry = new DirectoryEntry("IIS://localhost/W3SVC");
                foreach (DirectoryEntry entry2 in entry.Children)
                {
                    if (entry2.SchemaClassName == "IIsWebServer")
                    {
                        int num = Convert.ToInt32(entry2.Name);
                        DirectoryEntry entry3 = new DirectoryEntry("IIS://localhost/W3SVC/" + num + "/Root");
                        string item = entry3.Properties["Path"].Value.ToString();
                        list.Add(item);
                    }
                }
            }
            catch { }

            return list;
        }

        private string GetNetFramworkRootPath()
        {
            string name = @"Software\Microsoft\.NetFramework";
            return Registry.LocalMachine.OpenSubKey(name, false).GetValue("InstallRoot", string.Empty).ToString();
        }

        private string GetSPRootPath()
        {
            // @"C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\14\");
            string spSetupPath = string.Empty;

            try
            {
                // SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\14.0[@Location]
                RegistryKey wse = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions");
                if (wse != null)
                {
                    // get the highest version
                    double theVerNum = 0.0;
                    string theVerString = "0.0";

                    string[] subKeyNames = wse.GetSubKeyNames();
                    foreach (string keyName in subKeyNames)
                    {
                        double currVer;
                        if (true == double.TryParse(keyName, out currVer))
                        {
                            if (theVerNum < currVer)
                            {
                                theVerNum = currVer;
                                theVerString = keyName;
                            }
                        }
                    }

                    RegistryKey verSP = wse.OpenSubKey(theVerString);
                    if (verSP != null)
                        spSetupPath = verSP.GetValue(@"Location") as string;
                }
            }
            catch { }
            
            return spSetupPath;
        }

        private void _chkListBoxWebConfigs_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = _chkBoxCheckAll.Checked;
            int itemCount = _chkListBoxWebConfigs.Items.Count;

            // set item checked.
            //_chkListBoxWebConfigs.ItemCheck -= _chkListBoxWebConfigs_ItemCheck;
            for (int i = 0; i < itemCount; ++i)
            {
                _chkListBoxWebConfigs.SetItemChecked(i, isChecked);
            }
            //_chkListBoxWebConfigs.ItemCheck += _chkListBoxWebConfigs_ItemCheck;
        }

        private void _chkListBoxWebConfigs_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //if (_chkListBoxWebConfigs.CheckedItems.Count == 0)
            //{
            //    _chkBoxCheckAll.Checked = false;
            //}
            //else if (_chkListBoxWebConfigs.CheckedItems.Count == _chkListBoxWebConfigs.Items.Count)
            //{
            //    _chkBoxCheckAll.Checked = true;
            //}
        }

        private void _panelMain_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
