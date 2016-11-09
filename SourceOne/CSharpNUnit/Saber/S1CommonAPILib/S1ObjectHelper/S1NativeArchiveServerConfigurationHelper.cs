using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.Interop.ExAsAdminAPI;
using EMC.Interop.ExSTLContainers;

namespace Saber.S1CommonAPILib
{
    public enum S1NativeArchiveServerRole
    {
        Archive = 1,
        Index = 2,
        Search = 4,
        Retrieval = 8,
    }

    public class S1NativeArchiveServerConfigurationHelper
    {
        public static void ConfigNativeArchiveServer(String archiveConnection, String serverName, S1NativeArchiveServerConfiguration config)
        {
            IExASAdminAPI adminApi = new CoExASAdminAPI();
            adminApi.Initialize();
            IExASRepository repository = (IExASRepository)adminApi.GetRepository(archiveConnection);
            IExASServer server = null;

            try
            {
                server = repository.GetServer(serverName);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get the server with the name: " + serverName);
            }
            if (server != null)
            {
                ConfigNativeArchiveServer(server, config);
            }
        }

        internal static void ConfigNativeArchiveServer(IExASServer server, S1NativeArchiveServerConfiguration config)
        {
            uint personality = 0;
            if (config.Archive_Enabled)
            {
                personality |= (uint)S1NativeArchiveServerRole.Archive;
            }
            if (config.Index_Enabled)
            {
                personality |= (uint)S1NativeArchiveServerRole.Index;
            }
            if (config.Search_Enabled)
            {
                personality |= (uint)S1NativeArchiveServerRole.Search;
            }
            if (config.Retrieval_Enabled)
            {
                personality |= (uint)S1NativeArchiveServerRole.Retrieval;
            }
            server.ServerPersonality = personality;
            //archive
            if (config.Archive_MessageCenterLocation != null)
                server.MsgCenterRoot = config.Archive_MessageCenterLocation;
            if (config.Archive_VolumeIdleTime != uint.MaxValue)
                server.MaxVolumeIdleTime = config.Archive_VolumeIdleTime;
            //index
            if (config.Index_RunThreshold != uint.MaxValue)
                server.IndexIterationTime = config.Index_RunThreshold;
            server.ArchiveServerNames.Clear();
            foreach (String archiveServer in config.Index_ArchiveServersToIndex)
            {
                server.ArchiveServerNames.Add(archiveServer);
            }

            //search
            if (config.Search_MemoryAllocated != uint.MaxValue)
            {
                server.QueryMemorySize = config.Search_MemoryAllocated;
            }
            server.Validate();
            server.Save();
        }

        internal static void DefaultConfigNativeArchiveServer(IExASServer server, string messageCenterPath)
        {
            S1NativeArchiveServerConfiguration config = new S1NativeArchiveServerConfiguration(messageCenterPath);
            //archive
            config.Archive_Enabled = true;
            config.Archive_VolumeIdleTime = 172800;
            
            //index
            config.Index_Enabled = true;
            config.Index_RunThreshold = 45;
            List<String> arServersToIndex = new List<String>();
            //only index the archive server itself
            arServersToIndex.Add(server.FullName);
            config.Index_ArchiveServersToIndex = arServersToIndex;
            //search
            config.Search_Enabled = true;
            config.Search_MemoryAllocated = 50;

            //retrieval
            config.Retrieval_Enabled = true;          

            ConfigNativeArchiveServer(server, config);
        }

        public static void DefaultConfigAllNativeArchiveServers(String archiveConnection, String messageCenterRoot)
        {
            String path = messageCenterRoot;
            IExASAdminAPI adminApi = new CoExASAdminAPI();
            adminApi.Initialize();
            IExASRepository repository = (IExASRepository)adminApi.GetRepository(archiveConnection);
            IExASServerSet serverSet = (IExASServerSet)repository.EnumerateServers();
            if (serverSet.Count < 1)
                throw new Exception("No arhive server found! [Config native archive server]");
            foreach (IExASServer server in serverSet)
            {
                DefaultConfigNativeArchiveServer(server, path);
            }
        }
    }
}
