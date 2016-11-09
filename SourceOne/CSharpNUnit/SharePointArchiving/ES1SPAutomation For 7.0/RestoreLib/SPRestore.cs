using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.SourceOne.SharePoint.SearchRestore;
using EMC.SourceOne.SearchWS;
using EMC.SourceOne.SharePoint.Utilities;
using EMC.SourceOne.SharePoint.Scif;
using System.Xml;
using System.Diagnostics;

namespace RestoreLib
{
    public class SPRestore
    {
        protected SearchProxy searchClientProxy = null;
        protected string _webAppUrl;
        protected RestoreItem _item;
        public List<RestoreItem> _items = new List<RestoreItem>();
        static protected string _resultAttrs;

        public SPRestore(string webAppUrl, string searchServiceURL, string userName, string password)
        {
            _webAppUrl = webAppUrl;
            this.searchClientProxy = new SearchProxy();
            this.searchClientProxy.Initialize(searchServiceURL, false, new Credentials(userName, false), new EncryptDecryptSecureString(password));
        }

        static protected string ResultAttrs
        {
            get
            {
                if (_resultAttrs == null)
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(Environment.CurrentDirectory + "\\ResultColumns.xml"))
                    {
                        StringBuilder sb = new StringBuilder();

                        try
                        {
                            while (xmlReader.Read())
                                sb.Append(xmlReader.ReadOuterXml());
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        _resultAttrs = sb.ToString();
                    }
                }
                return _resultAttrs;
            }
        }

        public void AddItem(string id, string sourceType, string version, string target, string targetWebUrl, string targetSiteUrl)
        {
            RestoreItem item = new RestoreItem();
            item.Id = id;
            item.SourceType = sourceType;
            item.Version = version;
            item.Target = target;
            item.TargetWebUrl = targetWebUrl;
            item.TargetSiteUrl = targetSiteUrl;

            _items.Add(item);
        }

        public void AddItem(RestoreItem item)
        {
            _items.Add(item);
        }

        public void WaitForRestore()
        {
            if (WaitForProcessStarted("ExJBRestore", 300))
            {
                if (WaitForProcessStopped("ExJBRestore", 600))
                {
                    return;
                }
                else
                {
                    throw new Exception("ExJBRestore can not stop within timeout!");
                }
            }
            else
            {
                throw new Exception("ExJBRestore can not start within timeout!");
            }
        }

        protected bool WaitForProcessStarted(String proName, int timeout)
        {
            int i = 0;
            while (i < timeout)
            {
                Process[] proList = Process.GetProcessesByName(proName);
                if (proList.Length > 0)
                    break;
                System.Threading.Thread.Sleep(1000);
                i++;
            }
            if (i == timeout)
                return false;
            return true;
        }

        protected bool WaitForProcessStopped(String proName, int timeout)
        {
            int i = 0;
            while (i < timeout)
            {
                Process[] proList = Process.GetProcessesByName(proName);
                if (proList.Length == 0)
                    break;
                System.Threading.Thread.Sleep(1000);
                i++;
            }
            if (i == timeout)
                return false;
            return true;
        }

    }
}
