using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using System.Xml.Serialization;
using EMC.Interop.ExBase;

using System.Runtime.InteropServices;

using EMC.Interop.ExAsAdminAPI;
using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExProviderGW;

namespace Saber.S1CommonAPILib
{

    enum DBTypes
    {
        SQLServer = 1,
        Oracle = 2
    };

    public enum EmailAddressFormatType
    {
        EX = 1,
        SMTP = 2,
    }

    public enum EmailAddressType
    {
        MailBox = 1,
        DistributeList = 2,
    }

    public enum ADObjectType
    {
        Group = 1,
        User = 2,
    }

    public enum SupportedFolderType
    {
        JournalSupported = 1,
        MailBoxTaskSupported = 13,
    }

    public enum QueryOptions
    {
        FromLDAPServer = 1,
        FromGCServer = 2,
    }

    public sealed class S1Context
    {

        static readonly object SyncRoot = new object();

        private static IExJDFAPIMgr4 _jdfapiMgr;//IExJDFAPIMgr2
        private static IExProviderGW_5 _providerGW;//IExProviderGW_2
        private static IExFolderMgr_2 _folderMgr;//IExFolderMgr_2

        private static IExASAdminAPI _adminApi;

        public static void ReleaseComObject(object comObj)
        {
            if (comObj == null)
                return;
            try
            {
                Marshal.ReleaseComObject(comObj);
            }
            catch (Exception e)
            {
                //TODO not critial exception,logerthis exception to monitor
            }

        }
        public static void FinalReleaseComObject(object comObj)
        {
            if (comObj == null)
                return;
            try
            {
                Marshal.FinalReleaseComObject(comObj);
            }
            catch (Exception e)
            {
                //TODO not critial exception,logerthis exception to monitor
            }
        }


        public static IExASAdminAPI AdminAPI
        {
            get
            {
                if (_adminApi == null)
                {
                    lock (SyncRoot)
                    {
                        if (_adminApi == null)
                        {
                            _adminApi = new CoExASAdminAPI();
                            _adminApi.Initialize();
                        }
                    }
                }
                return _adminApi;
            }
        }

        public static IExJDFAPIMgr4 JDFAPIMgr
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
        public static IExProviderGW_5 ProviderGW
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
