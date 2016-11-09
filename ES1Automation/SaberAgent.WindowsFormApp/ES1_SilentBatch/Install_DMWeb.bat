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
:: 	Install_DMWeb.bat
::
:: CREATED
::   	10.16.2014
::
:: AUTHOR
::    	Neil Wang
::
:: DESCRIPTION
::   	Silently installs the Discovery Manager Web Server.
::
:: LAST UPDATED 
::   	10.16.2014 Neil Wang
::
:: VERSION	
::	 7.2.0.0
::
:: USAGE
:: 	SILENT MODE : Command Prompt> Install_DMWeb.bat
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
SET DM_SERVER_NAME="%~4%"

CALL %~dp0\DefaultValues.bat

::Set default values
IF %BATCHLOG%=="" 	SET BATCHLOG=%DMCLIENT_BATCHLOG%
IF %INSTALLLOG%=="" 	SET INSTALLLOG=%DMWEB_INSTALLLOG%
IF %INSTALLDIR%=="" 	SET INSTALLDIR=%DEFAULT_INSTALLDIR%
IF %DM_SERVER_NAME%==""	SET DM_SERVER_NAME=%DEFAULT_EX_DSCOSRV%

::Clear Log files
IF EXIST %BATCHLOG% DEL %BATCHLOG%
IF EXIST %INSTALLLOG% DEL %INSTALLLOG%

::Create batch log
ECHO SourceOne Discovery Manager Web Server Install Log >>%BATCHLOG%
ECHO. >>%BATCHLOG%
ECHO INSTALLLOG= %INSTALLLOG% >>%BATCHLOG%
ECHO INSTALLDIR= %INSTALLDIR% >>%BATCHLOG%
ECHO DM_SERVER_NAME= %DM_SERVER_NAME% >>%BATCHLOG%
ECHO. >>%BATCHLOG%

::Install Discovery Manager Web Server
@ECHO Installing Discovery Manager Web Server.  Please wait...
ECHO Discovery Manager Web Server Installation >>%BATCHLOG%
ECHO   Started at %DATE% %TIME% >>%BATCHLOG%

ES1_DiscoveryMgrWebSetup.exe /S /V"/qn INSTALLDIR=%INSTALLDIR% DM_SERVER_NAME=%DM_SERVER_NAME% REQUIRE_SSL=true" >>%BATCHLOG%

::Log completion
ECHO   Finished at %DATE% %TIME% >>%BATCHLOG%
ECHO. >>%BATCHLOG%
