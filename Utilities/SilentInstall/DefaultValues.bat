::===========================================================================================
::
:: Copyright 2013 EMC Corporation.  All rights reserved.  This software 
:: (including documentation) is subject to the terms and conditions set forth 
:: in the end user license agreement or other applicable agreement, and you 
:: may use this software only if you accept all the terms and conditions of 
:: the license agreement.  This software comprises proprietary and confidential 
:: information of EMC.  Unauthorized use, disclosure, and distribution are 
:: strictly prohibited.  Use, duplication, or disclosure of the software and 
:: documentation by the U.S. Government are subject to restrictions set forth 
:: in subparagraph (c)(1)(ii) of the Rights in Technical Data and Computer 
:: Software clause at DFARS 252.227-7013 or subparagraphs (c)(1) and (2) of the 
:: Commercial Computer Software - Restricted Rights at 48 CFR 52.227-19, as 
:: applicable. Manufacturer is EMC Corporation, 176 South St., Hopkinton, MA  01748.
:: 
:: FILE
:: 	DefaultValues.bat
::
:: CREATED
::   	12.10.2012
::
:: AUTHOR
::    	Simon Shi
::
:: DESCRIPTION
::   	Set default values for other scripts.
::
:: LAST UPDATED 
::   	12.10.2012 Simon Shi
::
:: VERSION	
::	 7.0.0.1
::
:: ===========================================================================================

:: General
SET DEFAULT_EX_SERVER_ALL=localhost

SET DEFAULT_INS_PAC_LOC=C:\Installations
SET DEFAULT_BATCH_LOC=C:\ES1_SilentBatch
SET DEFAULT_EX_DOMAIN=es1dev.com
SET DEFAULT_EX_USER=s1service
SET DEFAULT_EX_PASS=P@ssw0rd
SET DEFAULT_EX_GRP_SEC=\"s1 security\"
SET DEFAULT_EX_GRP_CON=\"s1 admins\"
SET DEFAULT_DB_GRP_SEC="'ES1DEV\s1 security'"
SET DEFAULT_DB_GRP_CON="'ES1DEV\s1 admins'"

SET DEFAULT_INSTALLDIR=\"C:\Program Files (x86)\EMC SourceOne\"

SET DEFAULT_EX_WORKDIR=\"D:\EMC SourceOne Work\"
SET DEFAULT_EX_NDXWORKDIR=\"D:\EMC SourceOne IndexWork\"
SET DEFAULT_EX_JOBLOGDIR=\"\\%DEFAULT_EX_SERVER_ALL%\JobLogs\"

SET DEFAULT_EX_WEBSVCS="%DEFAULT_EX_SERVER_ALL%"
SET DEFAULT_EX_USE_SSL=""
SET DEFAULT_EX_DBSERVER="%DEFAULT_EX_SERVER_ALL%"
SET DEFAULT_EX_JDFNAME="ES1Activity"
SET DEFAULT_EX_JDFSRV="%DEFAULT_EX_SERVER_ALL%"
SET DEFAULT_EX_PBANAME="ES1Archive"
SET DEFAULT_EX_PBASRV="%DEFAULT_EX_SERVER_ALL%"
SET DEFAULT_EX_SRCHNAME="ES1Search"
SET DEFAULT_EX_SRCHSRV="%DEFAULT_EX_SERVER_ALL%"
SET DEFAULT_NOTESPW="password"

SET DEFAULT_EX_DSCODBSERVER="%DEFAULT_EX_SERVER_ALL%"
SET DEFAULT_EX_DSCONAME="DiscoveryManager"
SET DEFAULT_EX_DSCOSRV="%DEFAULT_EX_SERVER_ALL%"

:: Platform
SET DEFAULT_PLAT_INSTALL=y
SET DEFAULT_ES1_EMAIL_PLT_NOTES=False
SET DEFAULT_ES1_EMAIL_PLT_EXCH=True
SET DEFAULT_ES1_EMAIL_PLT_O365=False

:: SharePoint BCE
SET DEFAULT_SPBCE_INSTALL=y
SET DEFAULT_SPBCE_WORKDIR=\"\\%DEFAULT_EX_SERVER_ALL%\Shared\BCE\"

:: Files BCE
SET DEFAULT_FABCE_INSTALL=y

:: Discovery Manager
SET DEFAULT_DM_INSTALL=y

:: Log
SET DB_BATCHLOG="C:\EMC_Database_Batch.log"
SET DB_INSTALLLOG="C:\EMC_Database_Install.log"

SET MASTER_BATCHLOG="C:\EMC_Master_Batch.log"
SET MASTER_INSTALLLOG="C:\EMC_Master_Install.log"

SET ARCHIVE_BATCHLOG="C:\EMC_Archive_Batch.log"
SET ARCHIVE_INSTALLLOG="C:\EMC_Archive_Install.log"

SET CONSOLE_BATCHLOG="C:\EMC_Console_Batch.log"
SET CONSOLE_INSTALLLOG="C:\EMC_Console_Install.log"

SET WORKER_BATCHLOG="C:\EMC_Worker_Batch.log"
SET WORKER_INSTALLLOG="C:\EMC_Worker_Install.log"

SET WEBSERVICE_BATCHLOG="C:\EMC_WebSvcs_Batch.log"
SET WEBSERVICE_INSTALLLOG="C:\EMC_WebSvcs_Install.log"

SET SEARCH_BATCHLOG="C:\EMC_Search_Batch.log"
SET SEARCH_INSTALLLOG="C:\EMC_Search_Install.log"

SET MOBILE_BATCHLOG="C:\EMC_Mobile_Batch.log"
SET MOBILE_INSTALLLOG="C:\EMC_Mobile_Install.log"

SET SPBCE_BATCHLOG="C:\EMC_SPBCE_Batch.log"
SET SPBCE_INSTALLLOG="C:\EMC_SPBCE_Install.log"

SET FABCE_BATCHLOG="C:\EMC_FABCE_Batch.log"
SET FABCE_INSTALLLOG="C:\EMC_FABCE_Install.log"

SET DMDB_BATCHLOG="C:\EMC_DMDB_Batch.log"
SET DMDB_INSTALLLOG="C:\EMC_DMDB_Install.log"

SET DMSERVER_BATCHLOG="C:\EMC_DMServer_Batch.log"
SET DMSERVER_INSTALLLOG="C:\EMC_DMServer_Install.log"

SET DMCLIENT_BATCHLOG="C:\EMC_DMClient_Batch.log"
SET DMCLIENT_INSTALLLOG="C:\EMC_DMClient_Install.log"
