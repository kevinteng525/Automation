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
::   	12.10.2012 Simon Shi
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
@ECHO OFF

:: Define variables
:: Location information
SET INS_DB="%~1%"
SET INS_PAC_LOC="%~2%"
SET BATCH_LOC="%~3%"

::Sourceone variables
SET EX_WORKDIR_ALL="%~4%"
SET EX_JOBLOGDIR_ALL="%~5%"
SET EX_DOMAIN_ALL="%~6%"
SET EX_USER_ALL="%~7%"
SET EX_PASS_ALL="%~8%"
SET EX_SERVER_ALL="%~9%"
SHIFT /3
SET EX_GRP_SEC="%~7%"
SET EX_GRP_CON="%~8%"

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
IF %EX_SERVER_ALL%==""	SET EX_SERVER_ALL=%DEFAULT_EX_SERVER_ALL%
IF %EX_JOBLOGDIR_ALL%==""	SET EX_JOBLOGDIR_ALL=%DEFAULT_EX_JOBLOGDIR%
IF %EX_WEBSVCS_ALL%==""	SET EX_WEBSVCS_ALL=%DEFAULT_EX_WEBSVCS%
IF %EX_USE_SSL_ALL%==""	SET EX_USE_SSL_ALL=%DEFAULT_EX_USE_SSL%
IF %EX_NDXWORKDIR_ALL%==""	SET EX_NDXWORKDIR_ALL=%DEFAULT_EX_NDXWORKDIR%
IF %EX_SPBCEWORK_ALL%==""	SET EX_SPBCEWORK_ALL=%DEFAULT_SPBCE_WORKDIR%
IF %EX_SPBCEINSTALL%=="" SET EX_SPBCEINSTALL=%DEFAULT_SPBCE_INSTALL%
IF %EX_FABCEINSTALL%=="" SET EX_FABCEINSTALL=%DEFAULT_FABCE_INSTALL%

:: Check ThirdParties folder
If NOT EXIST %INS_PAC_LOC% (
	Echo Make sure the install package folder exist before continue.
	Pause
	GOTO :EOF
)

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

CD %BATCH_LOC%

Echo Sourceone components installed successfully
Echo you need to restart the machine and to make them work

GOTO :LTerminate

:: -----------------------------------------------------------------------------
:: Subroutine to install Platform
:: -----------------------------------------------------------------------------
:InstallPlatform

:: Do you want to install DB
if %INS_DB%=="" SET /P INS_DB= Do you want to install DB first? (default is y):
if %INS_DB%=="" SET INS_DB=y
if %INS_DB%=="y" SET INS_DB=y
if %INS_DB%=="Y" SET INS_DB=y
if %INS_DB%==y SET INS_DB=y
if %INS_DB%==Y SET INS_DB=y

:: Install DB
If  %INS_DB% EQU y (
Call %BATCH_LOC%\Install_DB.bat "" "" %EX_SERVER_ALL% %BATCH_LOC%
Echo DB is installed
Echo.
)

::--- Install Archive begin ---
Call %BATCH_LOC%\Install_Archive.bat "" "" "" %EX_WORKDIR_ALL% %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_SERVER_ALL%
Echo Archive is installed
Echo.

::--- Install Master begin ---
Call %BATCH_LOC%\Install_Master.bat "" "" "" %EX_WORKDIR_ALL% %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_SERVER_ALL%
Echo Master is installed
Echo.

::--- Install Console begin ---
Call %BATCH_LOC%\Install_Console.bat "" "" "" %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_CON% %EX_SERVER_ALL%
Echo Console is installed
Echo.

::--- Install Worker begin ---
Call %BATCH_LOC%\Install_Worker.bat "" "" "" %EX_WORKDIR_ALL% %EX_JOBLOGDIR_ALL% %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_SERVER_ALL%
Echo Worker is installed
Echo.

::--- Install Webservices begin ---
Call %BATCH_LOC%\Install_WebServices.bat "" "" "" "" %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC%
Echo WebServices is installed
Echo.

::--- Install Search begin ---
Call %BATCH_LOC%\Install_Search.bat "" "" "" "" %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_WEBSVCS_ALL% %EX_USE_SSL_ALL%
Echo Search is installed
Echo.

::--- Install Mobile begin ---
Call %BATCH_LOC%\Install_Mobile.bat "" "" "" "" %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_WEBSVCS_ALL%
Echo Mobile is installed
Echo.

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

	::--- Install Discovery Manager Database begin ---
	Call %BATCH_LOC%\Install_DMDB.bat "" "" %EX_SERVER_ALL% %BATCH_LOC%
	Echo Discovery Manager Database is installed
	Echo.
	
	::---Switch Discovery Manager Server intaller directory--
	CD %INS_PAC_LOC%\Disco_CDRom\Setup\Windows
	Echo Switch current folder to %INS_PAC_LOC%\Disco_CDRom\Setup\Windows
	Echo.
	
	::--- Install Discovery Manager Database begin ---
	Call %BATCH_LOC%\Install_DMServer.bat "" "" "" %EX_DOMAIN_ALL% %EX_USER_ALL% %EX_PASS_ALL% %EX_GRP_SEC% %EX_SERVER_ALL%
	Echo Discovery Manager Server is installed
	Echo.
	
	::---Switch back to intaller directory--
	CD %INS_PAC_LOC%\ES1_CDRom\Setup\Windows
	Echo Switch current folder to %INS_PAC_LOC%\ES1_CDRom\Setup\Windows
	Echo.
	
	::--- Install Discovery Manager Client begin ---
	Call %BATCH_LOC%\Install_DMClient.bat "" "" "" %EX_SERVER_ALL% %BATCH_LOC%
	Echo Discovery Manager Client is installed
	Echo.
	
GOTO :EOF

:LTerminate

