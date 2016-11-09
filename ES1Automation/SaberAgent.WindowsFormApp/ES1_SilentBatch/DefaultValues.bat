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

:: The components you want to install on this machine, will be overrided by the Saber Agent
SET DEFAULT_PLAT_INSTALL=y
SET INS_DB=n
SET INS_NA=n
SET INS_WORKER=n
SET INS_MASTER=n
SET INS_WEBSERVICE=n
SET INS_SEARCH=n
SET INS_CONSOLE=n
SET INS_MOBILE=n
SET INS_DMWEB=n
SET INS_DMSERVER=n
SET INS_DMCLIENT=n
SET DEFAULT_SPBCE_INSTALL=n
SET DEFAULT_FABCE_INSTALL=n
SET DEFAULT_DM_INSTALL=n
SET DEFAULT_SUPERVISOR_INSTALL=n

:: General information, domain user and password will be set by test agent centrally
:: SET DEFAULT_EX_DOMAIN=Domain
:: SET DEFAULT_DB_DOMAIN=Domain
:: SET DEFAULT_EX_USER=ES1Service
:: SET DEFAULT_EX_PASS=qampass1!
:: SET DEFAULT_EX_SERVER_NATIVEARCHIVE=ES1W01
:: SET DEFAULT_EX_SERVER_WORKER=ES1W01
:: SET DEFAULT_EX_SERVER_MASTER=ES1W01
:: SET DEFAULT_EX_SERVER_SQL=SQL01
:: SET DEFAULT_EX_SERVER_DC=DC01
:: SET DEFAULT_EX_SERVER_MAIL=MAIL01



:: Below part is fixed for all S1 component holders
SET DEFAULT_INS_PAC_LOC=Y:\
SET DEFAULT_BATCH_LOC=C:\SaberAgent\ES1_SilentBatch
SET DEFAULT_EX_GRP_SEC=\"ES1 Security Group\"
SET DEFAULT_EX_GRP_CON=\"ES1 Admins Group\"
SET DEFAULT_DB_GRP_SEC="'%DEFAULT_DB_DOMAIN%\ES1 Security Group'"
SET DEFAULT_DB_GRP_CON="'%DEFAULT_DB_DOMAIN%\ES1 Admins Group'"

SET DEFAULT_INSTALLDIR=\"C:\Program Files (x86)\EMC SourceOne\"

SET DEFAULT_EX_WORKDIR=\"C:\EMC SourceOne Work\"
SET DEFAULT_EX_NDXWORKDIR=\"C:\EMC SourceOne IndexWork\"
SET DEFAULT_EX_JOBLOGDIR=\"\\%DEFAULT_EX_SERVER_WORKER%\Share\JobLog\"

SET DEFAULT_EX_WEBSVCS=%DEFAULT_EX_SERVER_WORKER%
SET DEFAULT_EX_USE_SSL=""
SET DEFAULT_EX_DBSERVER="%DEFAULT_EX_SERVER_SQL%"
SET DEFAULT_EX_JDFNAME="ES1Activity"
SET DEFAULT_EX_JDFSRV="%DEFAULT_EX_SERVER_SQL%"
SET DEFAULT_EX_PBANAME="ES1Archive"
SET DEFAULT_EX_PBASRV="%DEFAULT_EX_SERVER_SQL%"
SET DEFAULT_EX_SRCHNAME="ES1Search"
SET DEFAULT_EX_SRCHSRV="%DEFAULT_EX_SERVER_SQL%"
SET DEFAULT_NOTESPW="password"

SET DEFAULT_EX_DSCODBSERVER="%DEFAULT_EX_SERVER_SQL%"
SET DEFAULT_EX_DSCONAME="DiscoveryManager"
SET DEFAULT_EX_DSCOSRV="%DEFAULT_EX_SERVER_WORKER%"

::The default values for the installation of supervisor, may need to be updated when install on different machine.
SET DEFAULT_SUPERVISOR_INSTALLDIR=\"C:\Program Files (x86)\EMC SourceOne\Email Supervisor\"
SET DEFAULT_SUPERVISOR_COMPONENTS_TO_INSTALL=Server,Admin,Core,Common,Web,Database,Database_Execute
SET DEFAULT_SUPERVISOR_DB_NAME=Supervisor
SET DEFAULT_SUPERVISOR_PLATFORM=exchange
SET DEFAULT_SUPERVISOR_EXCHANGE_FROFILE=EMCSourceOne
SET DEFAULT_SUPERVISOR_EXCHANGE_ACCOUNT=\"ES1 Service\"
SET DEFAULT_SUPERVISOR_SSL_ENALBLE=1
SET DEFAULT_SUPERVISOR_SQL_AUTHENTICATION=0
SET DEFAULT_SUPERVISOR_SQL_USER=Domain\es1service
SET DEFAULT_SUPERVISOR_SQL_PASS=%DEFAULT_EX_PASS%
SET DEFAULT_ES1_EMAIL_ACCOUNT_ADDRESS=\"/o=First Organization/ou=Exchange Administrative Group (FYDIBOHF23SPDLT)/cn=Recipients/cn=ES1 Service832\"




:: Install successfully indicators
SET INS_DB_INSTALLED_INDICATOR=DB_Installed_Successfully.txt
SET INS_NA_INSTALLED_INDICATOR=NA_Installed_Successfully.txt
SET INS_WORKER_INSTALLED_INDICATOR=Worker_Installed_Successfully.txt
SET INS_MASTER_INSTALLED_INDICATOR=Master_Installed_Successfully.txt
SET INS_WEBSERVICE_INSTALLED_INDICATOR=WebService_Installed_Successfully.txt
SET INS_SEARCH_INSTALLED_INDICATOR=Search_Installed_Successfully.txt
SET INS_CONSOLE_INSTALLED_INDICATOR=Console_Installed_Successfully.txt
SET INS_MOBILE_INSTALLED_INDICATOR=Mobile_Installed_Successfully.txt
SET INS_DMSERVER_INSTALLED_INDICATOR=DMServer_Installed_Successfully.txt
SET INS_DMCLIENT_INSTALLED_INDICATOR=DMClient_Installed_Successfully.txt
SET INS_DMWEB_INSTALLED_INDICATOR=DMWeb_Installed_Successfully.txt
SET INS_SUPERVISOR_INSTALLED_INDICATOR=Supervisor_Installed_Successfully.txt

:: Platform
SET DEFAULT_ES1_EMAIL_PLT_NOTES=False
SET DEFAULT_ES1_EMAIL_PLT_EXCH=True
SET DEFAULT_ES1_EMAIL_PLT_O365=False

:: SharePoint BCE

SET DEFAULT_SPBCE_WORKDIR=\"\\%DEFAULT_EX_SERVER_WORKER%\Share\BCE\"

:: Files BCE


:: Discovery Manager


:: Log
SET DB_BATCHLOG="%~dp0\EMC_Database_Batch.log"
SET DB_INSTALLLOG="%~dp0\EMC_Database_Install.log"

SET MASTER_BATCHLOG="%~dp0\EMC_Master_Batch.log"
SET MASTER_INSTALLLOG="%~dp0\EMC_Master_Install.log"

SET ARCHIVE_BATCHLOG="%~dp0\EMC_Archive_Batch.log"
SET ARCHIVE_INSTALLLOG="%~dp0\EMC_Archive_Install.log"

SET CONSOLE_BATCHLOG="%~dp0\EMC_Console_Batch.log"
SET CONSOLE_INSTALLLOG="%~dp0\EMC_Console_Install.log"

SET WORKER_BATCHLOG="%~dp0\EMC_Worker_Batch.log"
SET WORKER_INSTALLLOG="%~dp0\EMC_Worker_Install.log"

SET WEBSERVICE_BATCHLOG="%~dp0\EMC_WebSvcs_Batch.log"
SET WEBSERVICE_INSTALLLOG="%~dp0\EMC_WebSvcs_Install.log"

SET SEARCH_BATCHLOG="%~dp0\EMC_Search_Batch.log"
SET SEARCH_INSTALLLOG="%~dp0\EMC_Search_Install.log"

SET MOBILE_BATCHLOG="%~dp0\EMC_Mobile_Batch.log"
SET MOBILE_INSTALLLOG="%~dp0\EMC_Mobile_Install.log"

SET SPBCE_BATCHLOG="%~dp0\EMC_SPBCE_Batch.log"
SET SPBCE_INSTALLLOG="%~dp0\EMC_SPBCE_Install.log"

SET FABCE_BATCHLOG="%~dp0\EMC_FABCE_Batch.log"
SET FABCE_INSTALLLOG="%~dp0\EMC_FABCE_Install.log"

SET DMDB_BATCHLOG="%~dp0\EMC_DMDB_Batch.log"
SET DMDB_INSTALLLOG="%~dp0\EMC_DMDB_Install.log"

SET DMSERVER_BATCHLOG="%~dp0\EMC_DMServer_Batch.log"
SET DMSERVER_INSTALLLOG="%~dp0\EMC_DMServer_Install.log"

SET DMCLIENT_BATCHLOG="%~dp0\EMC_DMClient_Batch.log"
SET DMCLIENT_INSTALLLOG="%~dp0\EMC_DMClient_Install.log"

SET DMWEB_BATCHLOG="%~dp0\EMC_DMWeb_Batch.log"
SET DMWEB_INSTALLLOG="%~dp0\EMC_DMWeb_Install.log"

SET SUPERVISOR_BATCHLOG="%~dp0\EMC_SUPERVISOR_Batch.log"
SET SUPERVISOR_INSTALLLOG="%~dp0\EMC_SUPERVISOR_Install.log"