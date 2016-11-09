USE ES1ACTIVITY

delete from Policy;

delete from sysSQLIndexMaintenanceLog;
delete from sysSQLStatsMaintenanceLog;
delete from EmailAddress;
delete from Resources;
delete from LDAPServers;
delete from BusinessFolder;
delete from DataSrcGroupMembers;
delete from DataSources;
delete from DataSrcLDAPServer;
delete from DataSourceCacheApps;
delete from BusinessFolderACL;
delete from Activity;
delete from BCEConfig;
delete from BCEConfigBusinessFolders;
delete from ActivityBCE;
delete from DataSourceMembers;
delete from Jobs;
delete from JobStatistics;
delete from JobResources;
delete from Tasks;
delete from TaskDataSources;
delete from TaskResources;
delete from sysSQLIndexWorkPad;
delete from sysSQLStatsWorkPad;
;delete from ProviderConfig;

DBCC CHECKIDENT ( 'Activity', RESEED, 1 );
DBCC CHECKIDENT ( 'BCEConfig', RESEED, 1 );
DBCC CHECKIDENT ( 'DataSources', RESEED, 1 );
DBCC CHECKIDENT ( 'Jobs', RESEED, 1 );
DBCC CHECKIDENT ( 'LDAPServers', RESEED, 1 );
DBCC CHECKIDENT ( 'Policy', RESEED, 1 );
DBCC CHECKIDENT ( 'Resources', RESEED, 1 );
DBCC CHECKIDENT ( 'Tasks', RESEED, 1 );
DBCC CHECKIDENT ( 'sysSQLIndexMaintenanceLog', RESEED, 1 );
DBCC CHECKIDENT ( 'sysSQLStatsMaintenanceLog', RESEED, 1 );

/**ES1Archive**/
USE ES1ARCHIVE
delete from FolderConvertCmd
delete from sysSQLIndexMaintenanceLog
delete from sysSQLStatsMaintenanceLog
delete from WorkQueue;
delete from Nums;
update MsgCounter set Counter = 0;
delete from FolderPlan where ParentId <> 0;
delete from MetadataIDLookup;
delete from Domains;
delete from EmailAddress;
delete from MessageExtension;
delete from Message;
delete from MetadataLookup;
delete from Metadata;
delete from MetadataType;
delete from FTIndex;
delete from FTIndexUpdate;
delete from Volume;
delete from FolderMessage;
delete from Route;
delete from sysSQLIndexWorkPad
delete from sysSQLStatsWorkPad

DBCC CHECKIDENT ( 'FTIndexUpdate', RESEED, 1 );
DBCC CHECKIDENT ( 'MetadataIDLookup', RESEED, 1 );
DBCC CHECKIDENT ( 'Volume', RESEED, 1 );
DBCC CHECKIDENT ( 'WorkQueue', RESEED, 1 );
DBCC CHECKIDENT ( 'sysSQLIndexMaintenanceLog', RESEED, 1 );
DBCC CHECKIDENT ( 'sysSQLStatsMaintenanceLog', RESEED, 1 );


/*--- Replace value of xml in ServerInfo to original values*/

UPDATE dbo.ServerInfo
SET ConfigXML.modify('replace value of (/ExAsSrvCfg/GeneralCfgProps/ExAsPersonality[1]/text())[1] with "0"')
WHERE MacAddress like '__-__-__-__-__-__'

UPDATE dbo.ServerInfo
SET ConfigXML.modify('replace value of (/ExAsSrvCfg/ArchiveService/MsgCenterPath[1]/text())[1] with ""')
WHERE MacAddress like '__-__-__-__-__-__'

UPDATE dbo.ServerInfo
SET ConfigXML.modify('replace value of (/ExAsSrvCfg/ArchiveService/ArchiveUnpackArea[1]/text())[1] with ""')
WHERE MacAddress like '__-__-__-__-__-__'

UPDATE dbo.ServerInfo
SET ConfigXML.modify('replace value of (/ExAsSrvCfg/ArchiveService/LostAndFoundPath[1]/text())[1] with ""')
WHERE MacAddress like '__-__-__-__-__-__'

UPDATE dbo.ServerInfo
SET ConfigXML.modify('replace value of (/ExAsSrvCfg/IndexService/IndexQueues[1]/text())[1] with ""')
WHERE MacAddress like '__-__-__-__-__-__'

UPDATE dbo.ServerInfo
SET ConfigXML.modify('replace value of (/ExAsSrvCfg/IndexService/IndexIterationTime[1]/text())[1] with "45"')
WHERE MacAddress like '__-__-__-__-__-__'

UPDATE dbo.ServerInfo
SET ConfigXML.modify('replace value of (/ExAsSrvCfg/IndexService/CurrentIndexComponents[1]/text())[1] with "0"')
WHERE MacAddress like '__-__-__-__-__-__'

UPDATE dbo.ServerInfo
SET ConfigXML.modify('replace value of (/ExAsSrvCfg/QueryService/QuerySharedMemorySzMB[1]/text())[1] with "100"')
WHERE MacAddress like '__-__-__-__-__-__'

/*--- Drop temp table in ES1Search--*/
/*--- Drop All table prefixed with 'results_' and 'Status_'--*/
USE ES1Search
DECLARE CurRes cursor for 
    select 'drop table ['+name +']; '
    from sysobjects 
    where (xtype = 'u' and (name like 'results_%' or name like 'status_%'))
open CurRes
declare @CurRes varchar(8000)
fetch next from CurRes into @CurRes
while(@@fetch_status=0)
    begin
        exec(@CurRes)
        fetch next from CurRes into @CurRes
    end
close CurRes
deallocate CurRes

