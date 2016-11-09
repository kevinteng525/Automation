using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.Interop.ExAsAdminAPI;
using EMC.Interop.ExSTLContainers;

namespace ES1.ES1SPAutoLib
{
    public class NativeServerOperator
    {
        public class NativeServerProperties
        {
            public uint ServerPersonality = uint.MaxValue;
            public String MsgCenterRoot;
            public uint MaxVolumeIdleTime = uint.MaxValue;
            public CoExVector ArchiveServerNames;
            public uint IndexIterationTime = uint.MaxValue;
            public uint QueryMemorySize = uint.MaxValue;
        }

        public static void ConfigNativeServer(IExASServer server, NativeServerProperties props)
        {
            if (props.ServerPersonality != uint.MaxValue)
                server.ServerPersonality = props.ServerPersonality;
            if (props.MsgCenterRoot != null)
                server.MsgCenterRoot = props.MsgCenterRoot;
            if (props.MaxVolumeIdleTime != uint.MaxValue)
                server.MaxVolumeIdleTime = props.MaxVolumeIdleTime;
            if (props.ArchiveServerNames != null)
                server.ArchiveServerNames = props.ArchiveServerNames;
            if (props.IndexIterationTime != uint.MaxValue)
                server.IndexIterationTime = props.IndexIterationTime;
            if (props.QueryMemorySize != uint.MaxValue)
                server.QueryMemorySize = props.QueryMemorySize;
            server.Validate();
            server.Save();
        }

        public static void DefaultConfigNativeServer(IExASServer server, string messageCenterPath)
        {
            NativeServerProperties props = new NativeServerProperties();
            props.ServerPersonality = 15;
            props.MsgCenterRoot = messageCenterPath;
            props.MaxVolumeIdleTime = 172800;
            CoExVector arServerNames = new CoExVector();
            arServerNames.Add(server.FullName);
            props.ArchiveServerNames = arServerNames;
            props.IndexIterationTime = 2;
            props.QueryMemorySize = 50;
            ConfigNativeServer(server, props);
        }

        public static void DefaultConfigAllNativeServer(String archiveConnection, String messageCenterRoot)
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
                DefaultConfigNativeServer(server, path);
            }
        }
    }
}
