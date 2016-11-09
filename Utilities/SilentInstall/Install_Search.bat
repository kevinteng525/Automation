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
:: 	Install_Search.bat
::
:: CREATED
::   	09.20.2007
::
:: AUTHOR
::    	DLens
::
:: DESCRIPTION
::   	Silently installs the Web Search Application.
::
:: LAST UPDATED 
::   	12.10.2012 Simon Shi
::
:: VERSION	
::	 7.0.0.1
::
:: USAGE
:: 	SILENT MODE : Command Prompt> Install_Search.bat
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
SET EX_DOMAIN="%~5%"
SET EX_USER="%~6%"
SET EX_PASS="%~7%"
SET EX_GRP_SEC=%~8%
SET EX_WEBSVCS="%~9%"
SET EX_USE_SSL="%~10%"

CALL %~dp0\DefaultValues.bat

::Set default values
IF %BATCHLOG%=="" 	SET BATCHLOG=%SEARCH_BATCHLOG%
IF %INSTALLLOG%=="" 	SET INSTALLLOG=%SEARCH_INSTALLLOG%
IF %INSTALLDIR%==""	SET INSTALLDIR=%DEFAULT_INSTALLDIR%
IF %EX_DOMAIN%==""	SET EX_DOMAIN=%DEFAULT_EX_DOMAIN%
IF %EX_USER%==""	SET EX_USER=%DEFAULT_EX_USER%
IF %EX_PASS%=="" 	SET EX_PASS=%DEFAULT_EX_PASS%
IF %EX_GRP_SEC%==""		SET EX_GRP_SEC=%DEFAULT_EX_GRP_SEC%
IF %EX_WEBSVCS%==""	SET EX_WEBSVCS=%DEFAULT_EX_WEBSVCS%
IF %EX_USE_SSL%==""	SET EX_USE_SSL=%DEFAULT_EX_USE_SSL%

::Clear Log files
IF EXIST %BATCHLOG% DEL %BATCHLOG%
IF EXIST %INSTALLLOG% DEL %INSTALLLOG%

::Create batch log
ECHO SourceOne Search App Install Log >>%BATCHLOG%
ECHO. >>%BATCHLOG%
ECHO INSTALLLOG= %INSTALLLOG% >>%BATCHLOG%
ECHO INSTALLDIR= %INSTALLDIR% >>%BATCHLOG%
ECHO EX_DOMAIN= %EX_DOMAIN% >>%BATCHLOG%
ECHO EX_USER= %EX_USER% >>%BATCHLOG%
ECHO EX_PASS= %EX_PASS% >>%BATCHLOG%
ECHO EX_GRP_SEC= %EX_GRP_SEC% >>%BATCHLOG%
ECHO EX_WEBSVCS= %EX_WEBSVCS% >>%BATCHLOG%
ECHO EX_USE_SSL= %EX_USE_SSL% >>%BATCHLOG%
ECHO. >>%BATCHLOG%

::Install Search App
@ECHO Installing Search Application.  Please wait...
ECHO Search App Installation >>%BATCHLOG%
ECHO   Started at %DATE% %TIME% >>%BATCHLOG%

ES1_SearchSetup.exe /s /v"/qn /L*v %INSTALLLOG% INSTALLDIR=%INSTALLDIR% EX_USE_SSL=%EX_USE_SSL% EX_NET_TCPSERVER=%EX_WEBSVCS% EX_SHORTCUT_DESKTOP=1 EX_SHORTCUT_QUICKLAUNCH=1 EX_TF_DOMAIN=%EX_DOMAIN% EX_TF_USERNAME=%EX_USER% EX_TF_PASSWORD=%EX_PASS% EX_TF_GROUP=%EX_GRP_SEC% REBOOT=ReallySuppress "

::Log completion
ECHO   Finished at %DATE% %TIME% >>%BATCHLOG%
ECHO. >>%BATCHLOG%
