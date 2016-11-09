using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using System.Xml.Serialization;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExProviderGW;
using EMC.Interop.ExBase;

namespace ES1.ES1SPAutoLib
{
    public sealed class SourceOneContext
    {

        static readonly object SyncRoot = new object();

        private static IExJDFAPIMgr2 _jdfapiMgr;
        private static IExProviderGW_2 _providerGW;
        private static IExFolderMgr_2 _folderMgr;

        public static IExJDFAPIMgr2 JDFAPIMgr
        {
            get
            {
                if (_jdfapiMgr == null)
                {
                    lock (SyncRoot)
                    {
                        if (_jdfapiMgr == null)
                        {

                            _jdfapiMgr = new CoExJDFAPIMgr();
                        }
                    }
                }
                return _jdfapiMgr;
            }

        }

        public static IExProviderGW_2 ProviderGW
        {
            get
            {
                if (_providerGW == null)
                {
                    lock (SyncRoot)
                    {
                        if (_providerGW == null)
                        {

                            _providerGW = new CoExProviderGW();
                        }
                    }
                }
                return _providerGW;
            }
        }

        public static IExFolderMgr_2 FolderMgr
        {
            get
            {
                if (_folderMgr == null)
                {
                    lock (SyncRoot)
                    {
                        if (_providerGW == null)
                            _providerGW = new CoExProviderGW();

                        if (_folderMgr == null)
                            _folderMgr = _providerGW.GetFolderMgr();
                    }
                }
                return _folderMgr;
            }

        }
    }
}
