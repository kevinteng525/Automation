using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EMC.Interop.ExJDFAPI;
using EMC.Interop.ExBase;
using EMC.Interop.ExProviderGW;
using EMC.Interop.ExSTLContainers;
using EMC.Interop.ExAsAdminAPI;
using EMC.Interop.ExASBaseAPI;

namespace Saber.S1CommonAPILib
{
    public class S1IndexHelper
    {
        public static bool WaitForIndexAccomplishOfArchiveFolder(String connectionName, String folderName, int waitMinutes = 30)
        {
            IExASArchiveFolder4 f = S1NativeArchiveFolderHelper.GetArchiveFolderByName(connectionName, folderName);
            int wait = 0;
            while (f.TotalMsgInVolumes > f.TotalMsgInIndexes && wait < waitMinutes)
            {
                System.Threading.Thread.Sleep(1 * 60 * 1000);
                wait++;
                f = S1NativeArchiveFolderHelper.GetArchiveFolderByName(connectionName, folderName);
            }
            if (f.TotalMsgInIndexes == f.TotalMsgInVolumes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
