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
:: 	SilentInstall_All.bat
::
:: CREATED
::   	06.01.2012
::
:: AUTHOR
::    	Simon Shi
::
:: DESCRIPTION
::   	Silently installs all SourceOne components.
::
:: LAST UPDATED 
::   	10.16.2014 Neil Wang
::
:: VERSION	
::	 7.0.0.1
::
:: USAGE
:: 	SILENT MODE : Command Prompt> SilentInstall_All.bat
::	
::	Arguments can be specified via command line arguments or entered directly
::	within the batch script below.
::
:: ===========================================================================================

:: How to use the script to batch install all sourceone components
:: Please modify parameters before you run this script
::
:: ---- Parameters Usage ---------
:: INS_PAC_LOC :  where you  installer live
:: BATCH_LOC% :  where batch files live
:: EX_WORKDIR_ALL: sourceone worker temp dir
:: EX_DOMAIN_ALL:  Domain name
:: EX_USER_ALL:    Sourceone service account
:: EX_PASS_ALL:    Password of sourceone service account
:: EX_GRP_ALL:     Sourceone security group
:: EX_GRP_CON:     Sourceone Admin Group
:: EX_JOBLOGDIR_ALL:  Job Log dir
:: EX_WEBSVCS_ALL:    Web services machine
:: EX_USE_SSL_ALL:    Use SSL
:: EX_NDXWORKDIR_ALL: Sourceone index work dir
::

:: We'll create an indicator file when a component is installed.
:: Other components can check the indicator if they depends on this.


:: @ECHO OFF

:: Define variables
:: Location information
SET INS_DB="%~1%"
SET INS_PAC_LOC="%~2%"
SET BATCH_LOC="%~3%"

::Sourceone variables
SET EX_WORKDIR_ALL=""
SET EX_JOBLOGDIR_ALL=""
SET EX_DOMAIN_ALL=""
SET EX_USER_ALL=""
SET EX_PASS_ALL=""

SET EX_SERVER_SQL=""
SET EX_SERVER_NA=""
SET EX_SERVER_WEBSERVICE=""
SET EX_SERVER_MASTER=""
SET EX_SERVER_WORKER=""
SET EX_SERVER_SEARCH=""
SET EX_SERVER_DMSERVER=""

SET EX_GRP_SEC=""
SET EX_GRP_CON=""

::-----------------------------------------------------------------------------------
SET EX_PLATINSTALL=""
SET EX_WEBSVCS_ALL=""
SET EX_USE_SSL_ALL=""
SET EX_NDXWORKDIR_ALL=""
SET EX_SPBCEINSTALL=""
SET EX_SPBCEWORK_ALL=""
SET EX_FABCEINSTALL=""

CALL %~dp0\DefaultValues.bat

:: Define intaller location and Batch file location
IF %INS_PAC_LOC%==""  SET INS_PAC_LOC=%DEFAULT_INS_PAC_LOC%
IF %BATCH_LOC%==""  SET BATCH_LOC=%DEFAULT_BATCH_LOC%

:: Set default values for sourceone components
IF %EX_PLATINSTALL%=="" SET EX_PLATINSTALL=%DEFAULT_PLAT_INSTALL%
IF %EX_WORKDIR_ALL%==""	SET EX_WORKDIR_ALL=%DEFAULT_EX_WORKDIR%
IF %EX_DOMAIN_ALL%==""	SET EX_DOMAIN_ALL=%DEFAULT_EX_DOMAIN%
IF %EX_USER_ALL%==""	SET EX_USER_ALL=%DEFAULT_EX_USER%
IF %EX_PASS_ALL%=="" 	SET EX_PASS_ALL=%DEFAULT_EX_PASS%
IF %EX_GRP_SEC%==""		SET EX_GRP_SEC=%DEFAULT_EX_GRP_SEC%
IF %EX_GRP_CON%==""  	SET EX_GRP_CON=%DEFAULT_EX_GRP_CON%

IF %EX_SERVER_SQL%==""	SET EX_SERVER_SQL=%DEFAULT_EX_SERVER_SQL%
IF %EX_SERVER_NA%==""	SET EX_SERVER_NA=%DEFAULT_EX_SERVER_NATIVEARCHIVE%
IF %EX_SERVER_WORKER%==""	SET EX_SERVER_WORKER=%DEFAULT_EX_SERVER_WORKER%
IF %EX_SERVER_MASTER%==""	SET EX_SERVER_MASTER=%DEFAULT_EX_SERVER_MASTER%
IF %EX_SERVER_WEBSERVICE%==""	SET EX_SERVER_WEBSERVICE=%DEFAULT_EX_SERVER_WORKER%
IF %EX_SERVER_DMSERVER%==""	SET EX_SERVER_DMSERVER=%DEFAULT_EX_SERVER_WORKER%
IF %EX_SERVER_SEARCH%==""	SET EX_SERVER_SEARCH=%DEFAULT_EX_SERVER_WORKER%

IF %EX_JOBLOGDIR_ALL%==""	SET EX_JOBLOGDIR_ALL=%DEFAULT_EX_JOBLOGDIR%
IF %EX_WEBSVCS_ALL%==""	SET EX_WEBSVCS_ALL=%DEFAULT_EX_WEBSVCS%
IF %EX_USE_SSL_ALL%==""	SET EX_USE_SSL_ALL=%DEFAULT_EX_USE_SSL%
IF %EX_NDXWORKDIR_ALL%==""	SET EX_NDXWORKDIR_ALL=%DEFAULT_EX_NDXWORKDIR%
IF %EX_SPBCEWORK_ALL%==""	SET EX_SPBCEWORK_ALL=%DEFAULT_SPBCE_WORKDIR%
IF %EX_SPBCEINSTALL%=="" SET EX_SPBCEINSTALL=%DEFAULT_SPBCE_INSTALL%
IF %EX_FABCEINSTALL%=="" SET EX_FABCEINSTALL=%DEFAULT_FABCE_INSTALL%

net use X: /DELETE /y
net use X: \\%DEFAULT_EX_SERVER_WORKER%\C$\SaberAgent\ES1_SilentBatch %DEFAULT_EX_PASS% /User:%DEFAULT_EX_DOMAIN%\%DEFAULT_EX_USER%
net use Y: /DELETE /y
net use Y: \\%DEFAULT_EX_SERVER_WORKER%\C$\SaberAgent\Installations %DEFAULT_EX_PASS% /User:%DEFAULT_EX_DOMAIN%\%DEFAULT_EX_USER%


:: Check Installation folder
If NOT EXIST %INS_PAC_LOC% (
	Echo Make sure the install package folder exist before continue.
	Pause
	GOTO :EOF
)
Y:

:: Check SharePoint ThirdParties folder
IF "%EX_SPBCEINSTALL%" == "y" (
	If NOT EXIST %INS_PAC_LOC%\ThirdParties (
		Echo Copy ThirdParties to installation folder to install SharePoint Online prerequsites, otherwise just continue...
		Pause
	)
)

:: Go to intaller directory--
CD %INS_PAC_LOC%\ES1_CDRom\Setup\Windows
Echo Switch current folder to %INS_PAC_LOC%\ES1_CDRom\Setup\Windows
Echo.

:: Install Platform
IF "%EX_PLATINSTALL%" == "y" (
	CALL :InstallPlatform
)

:: Install SharePoint BCE
IF "%EX_SPBCEINSTALL%" == "y" (
	CALL :InstallSPBCE
)

:: Install Files BCE
IF "%EX_FABCEINSTALL%" == "y" (
	CALL :InstallFABCE
)

:: Install Discovery Manager
IF "%DEFAULT_DM_INSTALL%" == "y" (
	CALL :InstallDiscoveryManager
)

:: Install Supervisor
IF "%DEFAULT_SUPERVISOR_INSTALL%" == "y" (
	CALL :InstallSupervisor
)

CD %BATCH_LOC%

Echo Sourceone components installed successfully
Echo you need to restart the machine and to make them work

GOTO :LTerminate

:: -----------------------------------------------------------------------------
:: Subroutine to install Platform
:: -----------------------------------------------------------------------------
:InstallPlatform

:: Install DB
If  %INS_DB% EQU y (
Call %BATCH_LOC%\Install_DB.bat "" "" %EX_SERVER_SQL% %BATCH_LOC%
Echo DB is installed
Echo.

::--- Install Discovery Manager Database begin ---
Call %BATCH_LOC%\Install_DMDB.bat "" "" %EX_SERVER_SQL% %BATCH_LOC%
Echo Discovery Manager and EM Database is installed>"X:\%INS_DB_INSTALLED_INDICATOR%"
Echo.
)

::--- Install Archive begin ---
If  %INS_NA% EQU y (
Call %BATCH_LOC%\Install_WaitUntillFileExists.bat "X:\%INS_DB_INSTALLED_INDICATOR%"
Call %BATCH_LOC%\Install_Archive.bat "" "" "" %EX_WORKDIR_ALL% %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_SERVER_SQL%
Echo Archive is installed>"X:\%INS_NA_INSTALLED_INDICATOR%"
Echo.
)

::--- Install Master begin ---
If  %INS_MASTER% EQU y (
Call %BATCH_LOC%\Install_WaitUntillFileExists.bat "X:\%INS_DB_INSTALLED_INDICATOR%"
Call %BATCH_LOC%\Install_Master.bat "" "" "" %EX_WORKDIR_ALL% %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_SERVER_SQL%
Echo Master is installed>"X:\%INS_MASTER_INSTALLED_INDICATOR%"
Echo.
)

::--- Install Console begin ---
If  %INS_CONSOLE% EQU y (
Call %BATCH_LOC%\Install_WaitUntillFileExists.bat "X:\%INS_DB_INSTALLED_INDICATOR%"
Call %BATCH_LOC%\Install_Console.bat "" "" "" %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_CON% %EX_SERVER_SQL%
Echo Console is installed>"X:\%INS_CONSOLE_INSTALLED_INDICATOR%"
Echo.
)

::--- Install Worker begin ---
If  %INS_WORKER% EQU y (
Call %BATCH_LOC%\Install_WaitUntillFileExists.bat "X:\%INS_MASTER_INSTALLED_INDICATOR%"
Call %BATCH_LOC%\Install_Worker.bat "" "" "" %EX_WORKDIR_ALL% %EX_JOBLOGDIR_ALL% %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_SERVER_SQL%
Echo Worker is installed>"X:\%INS_WORKER_INSTALLED_INDICATOR%"
Echo.
)

::--- Install Webservices begin ---
If  %INS_WEBSERVICE% EQU y (
Call %BATCH_LOC%\Install_WaitUntillFileExists.bat "X:\%INS_WORKER_INSTALLED_INDICATOR%"
Call %BATCH_LOC%\Install_WebServices.bat "" "" "" "" %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC%
Echo WebServices is installed>"X:\%INS_WEBSERVICE_INSTALLED_INDICATOR%"
Echo.
)

::--- Install Search begin ---
If  %INS_SEARCH% EQU y (
Call %BATCH_LOC%\Install_WaitUntillFileExists.bat "X:\%INS_WEBSERVICE_INSTALLED_INDICATOR%"
Call %BATCH_LOC%\Install_Search.bat "" "" "" "" %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_WEBSVCS_ALL% %EX_USE_SSL_ALL%
Echo Search is installed>"X:\%INS_SEARCH_INSTALLED_INDICATOR%"
Echo.
)

::--- Install Mobile begin ---
If  %INS_MOBILE% EQU y (
Call %BATCH_LOC%\Install_WaitUntillFileExists.bat "X:\%INS_WEBSERVICE_INSTALLED_INDICATOR%"
Call %BATCH_LOC%\Install_Mobile.bat "" "" "" "" %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_WEBSVCS_ALL%
Echo Mobile is installed>"X:\%INS_MOBILE_INSTALLED_INDICATOR%"
Echo.
)

GOTO :EOF

:: -----------------------------------------------------------------------------
::	Subroutine to install SharePoint BCE
:: -----------------------------------------------------------------------------
:InstallSPBCE

	::--- Install SharePoint BCE begin ---
	Call %BATCH_LOC%\Install_SPBCE.bat "" "" "" %INS_PAC_LOC% %EX_SPBCEWORK_ALL% %EX_DOMAIN_ALL% %EX_GRP_SEC%
	Echo SharePoint BCE is installed
	Echo.
	
GOTO :EOF


:: -----------------------------------------------------------------------------
::	Subroutine to install Files BCE
:: -----------------------------------------------------------------------------
:InstallFABCE

	::--- Install Files BCE begin ---
	Call %BATCH_LOC%\Install_FABCE.bat "" "" "" %INS_PAC_LOC% %EX_DOMAIN_ALL% %EX_GRP_SEC%
	Echo Files BCE is installed
	Echo.
	
GOTO :EOF

:: -----------------------------------------------------------------------------
::	Subroutine to install Discovery Manager
:: -----------------------------------------------------------------------------
:InstallDiscoveryManager
	
	::---Switch Discovery Manager Server intaller directory--
	CD %INS_PAC_LOC%\Disco_CDRom\Setup\Windows
	Echo Switch current folder to %INS_PAC_LOC%\Disco_CDRom\Setup\Windows
	Echo.
	
	::--- Install Discovery Manager Server begin ---
	If  %INS_DMSERVER% EQU y (
	Call %BATCH_LOC%\Install_WaitUntillFileExists.bat "X:\%INS_WEBSERVICE_INSTALLED_INDICATOR%"
	Call %BATCH_LOC%\Install_DMServer.bat "" "" "" %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_SERVER_SQL%
	Echo Discovery Manager Server is installed>"X:\%INS_DMSERVER_INSTALLED_INDICATOR%"
	Echo.
	)

	::---Switch back to intaller directory--
	CD %INS_PAC_LOC%\ES1_CDRom\Setup\Windows
	Echo Switch current folder to %INS_PAC_LOC%\ES1_CDRom\Setup\Windows
	Echo.	
	
	::--- Install Discovery Manager Client begin ---
	If  %INS_DMCLIENT% EQU y (
	Call %BATCH_LOC%\Install_WaitUntillFileExists.bat "X:\%INS_DMSERVER_INSTALLED_INDICATOR%"
	Call %BATCH_LOC%\Install_DMClient.bat "" "" "" %EX_SERVER_DMSERVER% %BATCH_LOC%
	Echo Discovery Manager Client is installed>"X:\%INS_DMCLIENT_INSTALLED_INDICATOR%"
	Echo.	
	)

	::--- Install Discovery Manager Web begin ---
	If  %INS_DMWEB% EQU y (
	Call %BATCH_LOC%\Install_WaitUntillFileExists.bat "X:\%INS_DMSERVER_INSTALLED_INDICATOR%"
	Call %BATCH_LOC%\Install_DMWeb.bat "" "" "" %EX_SERVER_DMSERVER% %BATCH_LOC%
	Echo Discovery Manager Web is installed>"X:\%INS_DMWEB_INSTALLED_INDICATOR%"
	Echo.
	)

GOTO :EOF

:: -----------------------------------------------------------------------------
::	Subroutine to install Supervisor
:: -----------------------------------------------------------------------------
:InstallSupervisor
	
	::---Switch Supervisor intaller directory--
	CD %INS_PAC_LOC%\CDRom\Setup
	Echo Switch current folder to %INS_PAC_LOC%\CDRom\Setup
	Echo.
	
	::--- Install Supervisor begin ---
	Call %BATCH_LOC%\Install_Supervisor.bat
	Echo SourceOne Supervisor is installed>"X:\%INS_SUPERVISOR_INSTALLED_INDICATOR%"
	Echo.

GOTO :EOF

:LTerminate
