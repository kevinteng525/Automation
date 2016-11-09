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
:: 	Install_SPBCE.bat
::
:: CREATED
::   	06.08.2012
::
:: AUTHOR
::    	Simon Shi
::
:: DESCRIPTION
::   	Silently installs the SharePoint BCE.
::
:: LAST UPDATED 
::   	12.10.2012 Simon Shi
::
:: VERSION	
::	 7.0.0.1
::
:: USAGE
:: 	SILENT MODE : Command Prompt> Install_SPBCE.bat
::	
::	Arguments can be specified via command line arguments or entered directly
::	within the batch script below.
::
:: ===========================================================================================
@ECHO OFF

::Define variables
SET BATCHLOG="%~1%"
SET INSTALLLOG="%~2%"
SET INSTALLDIR="%~3%"
SET INS_PAC_LOC=%~4%
SET BCEWORKDIR=%~5%
SET EX_DOMAIN=%~6%
SET EX_GRP_SEC=%~7%

CALL %~dp0\DefaultValues.bat

IF %BATCHLOG%=="" 	SET BATCHLOG=%SPBCE_BATCHLOG%
IF %INSTALLLOG%==""   SET INSTALLLOG=%SPBCE_INSTALLLOG%
IF %INSTALLDIR%==""	SET INSTALLDIR=%DEFAULT_INSTALLDIR%
IF %BCEWORKDIR%==""	SET BCEWORKDIR=%DEFAULT_SPBCE_WORKDIR%
IF %EX_DOMAIN%==""	SET EX_DOMAIN=%DEFAULT_EX_DOMAIN%
IF %EX_GRP_SEC%==""		SET EX_GRP_SEC=%DEFAULT_EX_GRP_SEC%
IF %INS_PAC_LOC%==""	SET INS_PAC_LOC=%DEFAULT_INS_PAC_LOC%

::Set default values

::Clear Log files
IF EXIST %BATCHLOG% DEL %BATCHLOG%
IF EXIST %INSTALLLOG% DEL %INSTALLLOG%

::Create batch log
ECHO SharePoint BCE Install Log >>%BATCHLOG%
ECHO. >>%BATCHLOG%
ECHO INSTALLLOG= %INSTALLLOG% >>%BATCHLOG%
ECHO INSTALLDIR= %INSTALLDIR% >>%BATCHLOG%
ECHO INS_PAC_LOC= %INS_PAC_LOC% >>%BATCHLOG%
ECHO EX_DOMAIN= %EX_DOMAIN% >>%BATCHLOG%
ECHO EX_GRP_SEC= %EX_GRP_SEC% >>%BATCHLOG%
ECHO BCEWORKDIR= %BCEWORKDIR% >>%BATCHLOG%
ECHO. >>%BATCHLOG%

::Install SharePoint BCE
@ECHO Installing SharePoint BCE.  Please wait...
ECHO SharePoint BCE Installation >>%BATCHLOG%
ECHO   Started at %DATE% %TIME% >>%BATCHLOG%

%INS_PAC_LOC%\ThirdParties\ClientOM\SPClient_x64.msi /norestart /qn /Lv* "C:\SPClientInstall.log"

%INS_PAC_LOC%\ThirdParties\ClientOM\spclient2010sp1-kb2508825-x64-fullfile-en-us.exe /quiet /norestart /log:C:\SPClientSP1Install.log

%INS_PAC_LOC%\ThirdParties\WIF\Windows6.1-KB974405-x64.msu /quiet /norestart /log:C:\WIFInstall.log

%INS_PAC_LOC%\SP_CDRom\SourceOneArchive\ES1_SharePointBCESetup.exe /s /v"/qn /L*v %INSTALLLOG% INSTALLDIR=%INSTALLDIR% EX_TF_DOMAIN=%EX_DOMAIN% EX_TF_GROUP=%EX_GRP_SEC% EX_WORKING_DIR=%BCEWORKDIR% INSTALLLEVEL=300 REBOOT=ReallySuppress " >>%BATCHLOG%

::Log completion
ECHO   Finished at %DATE% %TIME% >>%BATCHLOG%
ECHO. >>%BATCHLOG%

