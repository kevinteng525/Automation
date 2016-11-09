using System;
using System.Threading;
using System.Reflection;
using System.Runtime;

namespace ES1SPAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            RBSAgentParameter parameter = null;
            try
            {
                parameter = RBSAgentParameter.Parse(args);
            }
            catch (ArgumentNullException) { PrintHelp(); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                PrintHelp();
            }

            if (parameter == null) return;

            try
            {
                // create one instance
                RBSAgent agentInstance = new RBSAgent(parameter);
                agentInstance.DoOperations();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"
The commands:
  '-o', '-operation':         ActivateRBS,DeactivateRBS,CreateStore,SelectStore,DeleteStore,SetMinFileSize,SetCacheExpireDays,CreateStores,DeleteStores
  '-db', '-contentdabase':    the content database name
  '-sf', '-storefile':        the store creation configuration file
  '-sn', '-storename' :       the store name
  '-mfs', '-minfilesize':     the min file size (in K)
  '-ced', '-cacheexpiredays': the cache expire days (in days)
  '-jn', '-jobname':          the job name
  '-sfs', '-storefiles':      the store creation configuration files, sperated by '|'
  '-sns', '-storenames':      the store names, sperated by ','
  '-h', '/?':                 get help
            
The operation needs parameters:
  ActivateRBS: -db
  DeactivateRBS: -db
  CreateStore: -db -sf
  SelectStore: -db -sn
  DeleteStore: -db -sn
  SetMinFileSize: -db -mfs
  SetCacheExpireDays: -db -sn -ced
  RunJob: -jn
  CreateStores: -sfs
  DeleteStores: -sns

The store file xml:
  CIFS store xml:
    <?xml version='1.0' encoding='UTF-8' ?>
    <Store>
        <StoreName>store1</StoreName>
        <StorageLocation>\\qspxdev\D$\RBS\store1</StorageLocation>
        <PoolCapacity>10</PoolCapacity>
        <EncryptionType>None</EncryptionType>
        <IsCompressed>false</IsCompressed>
        <UserName>es1dev\s1service</UserName>
        <Password>P@ssw0rd</Password>
        <IsAtmosStore>false</IsAtmosStore>
    </Store>

  ATMOS store xml:
    <?xml version='1.0' encoding='UTF-8' ?>
    <Store>
        <StoreName>store2_atmos</StoreName>
        <StorageLocation>\\qspxdev\D$\RBS\store2_atmos</StorageLocation>
        <PoolCapacity>10</PoolCapacity>
        <EncryptionType>None</EncryptionType>
        <IsCompressed>false</IsCompressed>
        <UserName>es1dev\s1service</UserName>
        <Password>P@ssw0rd</Password>
        <IsAtmosStore>true</IsAtmosStore>
        <AtmosServerUrl>10.37.13.180</AtmosServerUrl>
        <AtmosSubTenant></AtmosSubTenant>
        <AtmosPort>80</AtmosPort>
        <AtmosSharedSecret>DOZHxkOBSm5714EmnalL3sJQHBY=</AtmosSharedSecret>
        <AtmosUid>RBSStore</AtmosUid>
        <AtmosCacheExpireDay>2</AtmosCacheExpireDay>
    </Store>

For example:
  ES1SPAgent.exe -operation ActivateRBS,CreateStore,SelectStore,SetMinFileSize,SetCacheExpireDays -contentdabase WSS_Content_81 -storefile C:\storefile.xml -sn store1 -mfs 80 -ced 3
  ES1SPAgent.exe -operation SetCacheExpireDays -contentdabase WSS_Content_81 -sn store1 -ced 3
  ES1SPAgent.exe -operation SetMinFileSize -contentdabase WSS_Content_81 -mfs 20
  ES1SPAgent.exe -operation RunJob -jobname ""EMC SourceOne Atmos Externalization Job""
  ES1SPAgent.exe -operation CreateStores -sfs ""C:\storefile1.xml|C:\storefile2.xml|C:\store file3.xml""
  ES1SPAgent.exe -operation DeleteStores -sns storename1,storename2,storename3
");
        }
    }

    public class RBSAgentParameter
    {
        public int Operation { get; set; }
        public string ContentDBName { get; set; }
        public string StoreName { get; set; }
        public string StoreFile { get; set; }
        public int MinFileSize { get; set; }
        public int CacheExpireDays { get; set; }
        public string JobName { get; set; }
        public string[] StoreFiles { get; protected set; }
        public string[] StoreNames { get; protected set; }

        public RBSAgentParameter()
        {
            Operation = 0;
            ContentDBName = null;
            StoreName = null;
            StoreFile = null;
            MinFileSize = -1;
            CacheExpireDays = -1;
            JobName = null;
            StoreFiles = null;
            StoreNames = null;
        }

        public static RBSAgentParameter Parse(string[] args)
        {
            RBSAgentParameter parameter = new RBSAgentParameter();

            if (args.Length == 0 || args.Length % 2 != 0)
                throw new ArgumentNullException();

            for (int i = 0; i < args.Length  && i + 1 < args.Length; i += 2)
            {
                string key = args[i];
                string value = args[i + 1];
                switch (key.ToLower())
                {
                    case "-o":
                    case "-operation":
                        string[] operations = value.Split(new char[] {',', ';', '|'}, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string operation in operations)
                        {
                            RBSAgentOperation op = (RBSAgentOperation)Enum.Parse(typeof(RBSAgentOperation), operation, true);
                            parameter.Operation |= (int)op;
                        }
                        break;

                    case "-db":
                    case "-contentdabase":
                        parameter.ContentDBName = value;
                        break;

                    case "-sf":
                    case "-storefile":
                        parameter.StoreFile = value;
                        break;

                    case "-sn":
                    case "-storename":
                        parameter.StoreName = value;
                        break;

                    case "-mfs":
                    case "-minfilesize":
                        parameter.MinFileSize = int.Parse(value.Trim());
                        break;

                    case "-ced":
                    case "-cacheexpiredays":
                        parameter.CacheExpireDays = int.Parse(value.Trim());
                        break;

                    case "-jn":
                    case "-jobname":
                        parameter.JobName = value;
                        break;

                    case "-sfs":
                    case "-storefiles":
                        parameter.StoreFiles = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        break;

                    case "-sns":
                    case "-storenames":
                        parameter.StoreNames = value.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                        break;

                    default:
                        throw new ArgumentException(string.Format("the argument '{0}' is not recogonized", key));
                }
            }

            // check if argument is enough
            if (string.IsNullOrEmpty(parameter.ContentDBName) &&
                ((int)parameter.Operation & (int)RBSAgentOperation.RunJob) != (int)RBSAgentOperation.RunJob)
            {
                throw new ArgumentException("the '-db contentdatabase' is needed for any operation except RunJob");
            }
            if (((int)parameter.Operation & (int)RBSAgentOperation.CreateStore) == (int)RBSAgentOperation.CreateStore)
            {
                if (string.IsNullOrEmpty(parameter.StoreFile))
                    throw new ArgumentException("the '-sf storefile' is needed for operation CreateStore");
            }
            if (((int)parameter.Operation & (int)RBSAgentOperation.SelectStore) == (int)RBSAgentOperation.SelectStore ||
                ((int)parameter.Operation & (int)RBSAgentOperation.DeleteStore) == (int)RBSAgentOperation.DeleteStore)
            {
                if (string.IsNullOrEmpty(parameter.StoreName))
                    throw new ArgumentException("the '-sn storename' is needed for operation SelectStore or DeleteStore");
            }
            if (((int)parameter.Operation & (int)RBSAgentOperation.SetMinFileSize) == (int)RBSAgentOperation.SetMinFileSize)
            {
                if (-1 == parameter.MinFileSize)
                    throw new ArgumentException("the '-mfs minfilesize' is needed for operation SetMinFileSize");
            }
            if (((int)parameter.Operation & (int)RBSAgentOperation.SetCacheExpireDays) == (int)RBSAgentOperation.SetCacheExpireDays)
            {
                if (string.IsNullOrEmpty(parameter.StoreName))
                    throw new ArgumentException("the '-sn storename' is needed for operation SetCacheExpireDays");
                if (-1 == parameter.CacheExpireDays)
                    throw new ArgumentException("the '-ced cacheexpiredays' is needed for operation SetCacheExpireDays");
            }
            if (((int)parameter.Operation & (int)RBSAgentOperation.RunJob) == (int)RBSAgentOperation.RunJob)
            {
                if (string.IsNullOrEmpty(parameter.JobName))
                    throw new ArgumentException("the '-jn jobname' is needed for operation RunJob");
            }
            if (((int)parameter.Operation & (int)RBSAgentOperation.CreateStores) == (int)RBSAgentOperation.CreateStores)
            {
                if (parameter.StoreFiles == null || parameter.StoreFiles.Length == 0)
                    throw new ArgumentException("the '-sfs C:\\storepath1|C:\\storepath2' is needed for operation CreateStores");
            }
            if (((int)parameter.Operation & (int)RBSAgentOperation.DeleteStores) == (int)RBSAgentOperation.DeleteStores)
            {
                if (parameter.StoreNames == null || parameter.StoreNames.Length == 0)
                    throw new ArgumentException("the '-sns storename1,storename2' is needed for operation DeleteStores");
            }

            return parameter;
        }
    }

    public enum RBSAgentOperation : int
    {
        ActivateRBS = 1,
        DeactivateRBS = 2,
        CreateStore = 4,
        SelectStore = 8,
        DeleteStore = 16,
        SetMinFileSize = 32,
        SetCacheExpireDays = 64,
        RunJob = 128,
        CreateStores = 256,
        DeleteStores = 512
    }
}
