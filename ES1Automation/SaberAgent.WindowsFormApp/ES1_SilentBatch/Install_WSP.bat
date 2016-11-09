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
:: 	Install_WSP.bat
::
:: CREATED
::   	07.21.2012
::
:: AUTHOR
::    	Simon Shi
::
:: DESCRIPTION
::   	Silently installs all the SharePoint wsps
::
:: LAST UPDATED 
::   	12.10.2012 Simon Shi
::
:: VERSION	
::	 7.0.0.1
::
:: USAGE
:: 	SILENT MODE : Command Prompt> Install_WSP.bat
::	
::	Arguments can be specified via command line arguments or entered directly
::	within the batch script below.
::
:: ===========================================================================================

@ECHO OFF
SETLOCAL
PUSHD .

GOTO LInitialize

@REM -----------------------------------------------------------------
@REM LInitialize
@REM -----------------------------------------------------------------
:LInitialize
    SET SPLocation12=%CommonProgramFiles%\Microsoft Shared\web server extensions\12
    SET SPLocation14=%CommonProgramFiles%\Microsoft Shared\web server extensions\14
    SET SPLocation=
	
    IF EXIST "%SPLocation12%" (
        SET SPVersion=12
        SET SPLocation=%SPLocation12%
    )
    IF EXIST "%SPLocation14%" (
        SET SPVersion=14
        SET SPLocation=%SPLocation14%
    )
    IF "%SPLocation%" == "" (
        ECHO SharePoint is not installed on current server.
        GOTO LTerminate
    )

    SET SPAdminTool=%SPLocation%\BIN\stsadm.exe
    SET SPTemplateLocation=%SPLocation%\template
    SET SPFeaturesLocation=%SPTemplateLocation%\features
    SET SPSiteTemplateLocation=%SPTemplateLocation%\sitetemplates
    SET Install=
    SET Uninstall=
    SET Force=
    SET RootFolder=C:\Installations\SP_CDRom
    SET TargetSite=http://localhost
    SET TargetSitePort=80
    SET AdminSitePort=
    SET TargetAdminSiteUrl=
    SET TargetSiteUrl=
    SET ValidationFailed=
    SET LCID=
    SET LCIDS=
    SET Language=
    SET Installed=
    SET Automated=1
    SET ArchiveSite=SourceOneArchive
	
    SET AdministrationWSP=EMCES1.Administration.wsp
    SET ArchiveWSP=EMCES1.Archiving.wsp
    SET SearchWSP=EMCES1.ArchiveSearch.wsp
    SET SearchConfigUIWSP=EMCES1.ArchiveSearchConfigUI.wsp
    SET RestoreWSP=EMCES1.Restore.wsp

    goto LParseArgs
	
@REM -----------------------------------------------------------------
@REM LParseArgs
@REM -----------------------------------------------------------------
:LParseArgs
	@REM ---help---
	IF "%1" == "/?"		GOTO LHelp
	IF "%1" == "-?" 	GOTO LHelp
	IF "%1" == "/h"		GOTO LHelp
	IF "%1" == "-h" 	GOTO LHelp
	IF "%1" == "/help"	GOTO LHelp
	IF "%1" == "-help" 	GOTO LHelp
	
	@REM ---Fix execute task---
	IF "%1" == "/i"			(set Install=1)		& SHIFT & GOTO LParseArgs
	IF "%1" == "-i"			(set Install=1)		& SHIFT & GOTO LParseArgs
	IF "%1" == "/install"	(set Install=1)		& SHIFT & GOTO LParseArgs
	IF "%1" == "-install"	(set Install=1)		& SHIFT & GOTO LParseArgs
	IF "%1" == "/u"			(set Uninstall=1)	& SHIFT & GOTO LParseArgs
	IF "%1" == "-u"			(set Uninstall=1)	& SHIFT & GOTO LParseArgs
	IF "%1" == "/uninstall"	(set Uninstall=1)	& SHIFT & GOTO LParseArgs
	IF "%1" == "-uninstall"	(set Uninstall=1)	& SHIFT & GOTO LParseArgs
	
	@REM ---Fix url---
	IF "%1" == "/rootfolder"	(set RootFolder=%2)	& SHIFT & SHIFT & GOTO LParseArgs
	IF "%1" == "-rootfolder"	(set RootFolder=%2)	& SHIFT & SHIFT & GOTO LParseArgs
	
	@REM ---Fix url---
	IF "%1" == "/site"	(set TargetSite=%2)	& SHIFT & SHIFT & GOTO LParseArgs
	IF "%1" == "-site"	(set TargetSite=%2)	& SHIFT & SHIFT & GOTO LParseArgs
	
	@REM ---Fix url---
	IF "%1" == "/port"	(set TargetSitePort=%2)	& SHIFT & SHIFT & GOTO LParseArgs
	IF "%1" == "-port"	(set TargetSitePort=%2)	& SHIFT & SHIFT & GOTO LParseArgs
	
	@REM ---Fix url---
	IF "%1" == "/adminsiteurl"	(set TargetAdminSiteUrl=%2)	& SHIFT & SHIFT & GOTO LParseArgs
	IF "%1" == "-adminsiteurl"	(set TargetAdminSiteUrl=%2)	& SHIFT & SHIFT & GOTO LParseArgs
	
	@REM ---Fix url---
	IF "%1" == "/siteurl"	(set TargetSiteUrl=%2)	& SHIFT & SHIFT & GOTO LParseArgs
	IF "%1" == "-siteurl"	(set TargetSiteUrl=%2)	& SHIFT & SHIFT & GOTO LParseArgs
	
	@REM ---LCID---
	IF "%1" == "/lcid"	(set LCIDS=%~2)	& SHIFT & SHIFT & GOTO LParseArgs
	IF "%1" == "-lcid"	(set LCIDS=%~2)	& SHIFT & SHIFT & GOTO LParseArgs
	
	
	@REM ---Force---
	IF "%1" == "/force"		(set Force=1)		& SHIFT & GOTO LParseArgs
	IF "%1" == "-force"		(set Force=1)		& SHIFT & GOTO LParseArgs
	
	@REM ---Check invalid arguments---
	IF NOT "%1" == "" (
		ECHO Invalid argument.
		GOTO LHelp
	)
	
	IF "%RootFolder%" == "" SET RootFolder="."
	
	@REM ---Check arguments---
	IF "%Install%" == "1" (
		IF "%Uninstall%" == "1" (
			GOTO LHelp
		)
	)
	
	IF "%Install%" == "" (
		IF "%Uninstall%" == "" (
			SET Install=1
		)
	)
	

	GOTO LMain
	
@REM -----------------------------------------------------------------
@REM LHelp
@REM -----------------------------------------------------------------
:LHelp
	ECHO Usage:
	ECHO Install_WSP.bat [/install or uninstall] [/rootfolder] [/siteurl ^<url^>] or [/site ^<url^>] [/port ^<port^>] [/adminsiteurl ^<url^>] 
	ECHO           		 [/lcid] [/force]
	ECHO 		         [/help]
	ECHO Options:
	ECHO /install or /uninstall
	ECHO Install specific solution package to the SharePoint server
	ECHO or uninstall specified solution from the SharePoint server.
	ECHO Default value: install
	ECHO /rootfolder
	ECHO Specify SharePoint SP_CDRom path.
	ECHO /site
	ECHO Specify Site of the SharePoint server.
	ECHO /port
	ECHO Specify Site port of the SharePoint server.
	ECHO Default value: 80
	ECHO /adminsiteurl
	ECHO Specify Site url of the SharePoint server.
	ECHO /siteurl
	ECHO Specify Site url of the SharePoint server.
	ECHO /lcid
	ECHO Specify LCID for solution deployment.
	ECHO /force
	ECHO Install or unistall by force.
	ECHO /help
	ECHO Show this information.
	ECHO.
	
	GOTO LTerminate
	
@REM -----------------------------------------------------------------
@REM LMain
@REM -----------------------------------------------------------------
:LMain
	IF "%Install%" == "1" (
		IF NOT "%Force%" == "1" (
			CALL :LValidate
		)
	)

	IF "%Install%" == "1" (
		IF NOT "%ValidationFailed%" == "1" (
			CALL :LGetPort
			CALL :LGetUrl
			CALL :LDeploy
		)
	)
	
	IF "%Uninstall%" == "1" (
		CALL :LGetPort
		CALL :LGetUrl
		CALL :LDetect
		CALL :LRetract
	)
	
	GOTO LTerminate
	
@REM -----------------------------------------------------------------
@REM LValidate
@REM -----------------------------------------------------------------
:LValidate
	ECHO Validating the existing of features ...
	ECHO.
	
	IF EXIST "%SPFeaturesLocation%\EMCES1.AdminSiteElements" (
		ECHO Error: Feature folder EMCES1.AdminSiteElements already exists in current SharePoint.
		SET ValidationFailed=1
	)
	
	IF EXIST "%SPFeaturesLocation%\EMCES1.Services" (
		ECHO Error: Feature folder EMCES1.Services already exists in current SharePoint.
		SET ValidationFailed=1
	)
	
	IF EXIST "%SPFeaturesLocation%\EMCES1.ArchiveSearchConfig" (
		ECHO Error: Feature folder EMCES1.ArchiveSearchConfig already exists in current SharePoint.
		SET ValidationFailed=1
	)

	IF EXIST "%SPFeaturesLocation%\EMCES1.ArchiveSearchCore" (
		ECHO Error: Feature folder EMCES1.ArchiveSearchCore already exists in current SharePoint.
		SET ValidationFailed=1
	)
	
	IF EXIST "%SPFeaturesLocation%\EMCES1.RestoreServices" (
		ECHO Error: Feature folder EMCES1.RestoreServices already exists in current SharePoint.
		SET ValidationFailed=1
	)
	
	GOTO :EOF

@REM -----------------------------------------------------------------
@REM LGetPort
@REM -----------------------------------------------------------------
:LGetPort
	"%SPAdminTool%" -o getadminport > SPAdminPort.txt

	SET k=0
	SET x=
	FOR /f "delims=" %%a IN (SPAdminPort.txt) DO (
		SET /a k+=1
		@REM IF !k!==2 SET x=%%a 
		SET x=%%a
	)

	FOR /f "delims=: tokens=1-3" %%b IN ("%x%") DO (
		SET AdminSitePort=%%c
	)

	ECHO Central Administration Site Port is %AdminSitePort%
	ECHO.
	GOTO :EOF
	
@REM -----------------------------------------------------------------
@REM LGetUrl
@REM -----------------------------------------------------------------
:LGetUrl
	IF "%TargetAdminSiteUrl%" == "" (
		SET TargetAdminSiteUrl=%TargetSite%:%AdminSitePort%
	)
	
	IF "%TargetSiteUrl%" == "" (
		SET TargetSiteUrl=%TargetSite%:%TargetSitePort%
	)
	
	GOTO :EOF
	
@REM -----------------------------------------------------------------
@REM LDetect
@REM -----------------------------------------------------------------
:LDetect
	"%SPAdminTool%" -o displaysolution -name "%AdministrationWSP%" > SPSolution.txt 2>nul
	FINDSTR /C:"Deployed" SPSolution.txt >nul 2>nul
	IF %ERRORLEVEL% EQU 0 SET Installed=1
	
	"%SPAdminTool%" -o displaysolution -name "%ArchiveWSP%" > SPSolution.txt 2>nul
	FINDSTR /C:"Deployed" SPSolution.txt >nul 2>nul
	IF %ERRORLEVEL% EQU 0 SET Installed=1
	
	"%SPAdminTool%" -o displaysolution -name "%SearchConfigUIWSP%" > SPSolution.txt 2>nul
	FINDSTR /C:"Deployed" SPSolution.txt >nul 2>nul
	IF %ERRORLEVEL% EQU 0 SET Installed=1
	
	"%SPAdminTool%" -o displaysolution -name "%SearchWSP%" > SPSolution.txt 2>nul
	FINDSTR /C:"Deployed" SPSolution.txt >nul 2>nul
	IF %ERRORLEVEL% EQU 0 SET Installed=1
	
	"%SPAdminTool%" -o displaysolution -name "%RestoreWSP%" > SPSolution.txt 2>nul
	FINDSTR /C:"Deployed" SPSolution.txt >nul 2>nul
	IF %ERRORLEVEL% EQU 0 SET Installed=1
	
	GOTO :EOF

@REM -----------------------------------------------------------------
@REM LGetLanguage
@REM -----------------------------------------------------------------
:LGetLanguage	
	SET LCID=%1
	
	IF NOT "%LCID%" == "" (
		GOTO CASE_%LCID%
		:CASE_1036
			SET Language="fr-FR"
			GOTO END_SWITCH
		:CASE_1031
			SET Language="de-DE"
			GOTO END_SWITCH
		:CASE_3082
			SET Language="es-ES"
			GOTO END_SWITCH
		:CASE_1040
			SET Language="it-IT"
			GOTO END_SWITCH
	)

	:END_SWITCH

	GOTO :EOF

@REM -----------------------------------------------------------------
@REM LDeployLanguagePack
@REM -----------------------------------------------------------------
:LDeployLanguagePack	
	SET Installed=
	FINDSTR /C:"Locale=\"%LCID%\"" SPSolution.txt >nul 2>nul
	IF %ERRORLEVEL% EQU 0 SET Installed=1
	
	IF NOT "%Installed%" == "1" (
		ECHO Adding solution %AdministrationWSP% - %LCID% to SharePoint ...
		"%SPAdminTool%" -o addsolution -filename "%RootFolder%\SourceOneArchive\SourceOneAdmin\%Language%\%AdministrationWSP%" -lcid %LCID%
		
		ECHO Adding solution %ArchiveWSP% - %LCID% to SharePoint ...
		"%SPAdminTool%" -o addsolution -filename "%RootFolder%\SourceOneArchive\Archiving\%Language%\%ArchiveWSP%" -lcid %LCID%
		
		ECHO Adding solution %SearchConfigUIWSP% - %LCID% to SharePoint ...
		"%SPAdminTool%" -o addsolution -filename "%RootFolder%\SourceOneSearch\ArchiveSearchConfigUI\%Language%\%SearchConfigUIWSP%" -lcid %LCID%
		
		ECHO Adding solution %SearchWSP% - %LCID% to SharePoint ...
		"%SPAdminTool%" -o addsolution -filename "%RootFolder%\SourceOneSearch\ArchiveSearch\%Language%\%SearchWSP%" -lcid %LCID%
		
	) ELSE (
	
		ECHO Upgrading solution %AdministrationWSP% - %LCID% to SharePoint ...
		"%SPAdminTool%" -o upgradesolution -name "%AdministrationWSP%" -filename "%RootFolder%\SourceOneArchive\SourceOneAdmin\%Language%\%AdministrationWSP%" -lcid %LCID% -immediate -allowGacDeployment
		
		ECHO Upgrading solution %ArchiveWSP% - %LCID% to SharePoint ...
		"%SPAdminTool%" -o upgradesolution -name "%ArchiveWSP%" -filename "%RootFolder%\SourceOneArchive\Archiving\%Language%\%ArchiveWSP%" -lcid %LCID% -immediate -allowGacDeployment
		
		ECHO Upgrading solution %SearchConfigUIWSP% - %LCID% to SharePoint ...
		"%SPAdminTool%" -o upgradesolution -name "%SearchConfigUIWSP%" -filename "%RootFolder%\SourceOneSearch\ArchiveSearchConfigUI\%Language%\%SearchConfigUIWSP%" -lcid %LCID% -immediate -allowGacDeployment
		
		ECHO Upgrading solution %SearchWSP% - %LCID% to SharePoint ...
		"%SPAdminTool%" -o upgradesolution -name "%SearchWSP%" -filename "%RootFolder%\SourceOneSearch\ArchiveSearch\%Language%\%SearchWSP%" -lcid %LCID% -immediate -allowGacDeployment
		
	)
	
	ECHO Deploying solution %AdministrationWSP% - %LCID% ...
	ECHO.
	"%SPAdminTool%" -o deploysolution -name "%AdministrationWSP%" -immediate -allowGacDeployment -lcid %LCID% -force >nul 2>nul
	
	ECHO Deploying solution %ArchiveWSP% - %LCID% ...
	ECHO.
	"%SPAdminTool%" -o deploysolution -name "%ArchiveWSP%" -immediate -allowGacDeployment -lcid %LCID% -force >nul 2>nul
	
	ECHO Deploying solution %SearchConfigUIWSP% - %LCID% ...
	ECHO.
	"%SPAdminTool%" -o deploysolution -name "%SearchConfigUIWSP%" -immediate -lcid %LCID% -force >nul 2>nul
	
	ECHO Deploying solution %SearchWSP% - %LCID% ...
	ECHO.
	"%SPAdminTool%" -o deploysolution -name "%SearchWSP%" -immediate -lcid %LCID% -force >nul 2>nul
	
	"%SPAdminTool%" -o execadmsvcjobs >nul 2>nul

	GOTO :EOF

@REM -----------------------------------------------------------------
@REM LRetractLanguagePack
@REM -----------------------------------------------------------------
:LRetractLanguagePack

	ECHO Retracting solution %SearchWSP% - %LCID% ...
	ECHO.
	"%SPAdminTool%" -o retractsolution -name "%SearchWSP%" -immediate -lcid %LCID% >nul 2>nul
	
	ECHO Retracting solution %SearchConfigUIWSP% - %LCID% ...
	ECHO.
	"%SPAdminTool%" -o retractsolution -name "%SearchConfigUIWSP%" -immediate -lcid %LCID% >nul 2>nul
	
	ECHO Retracting solution %ArchiveWSP% - %LCID% ...
	ECHO.
	"%SPAdminTool%" -o retractsolution -name "%ArchiveWSP%" -immediate -lcid %LCID% >nul 2>nul
	
	ECHO Retracting solution %AdministrationWSP% - %LCID% ...
	ECHO.
	"%SPAdminTool%" -o retractsolution -name "%AdministrationWSP%" -immediate -lcid %LCID% >nul 2>nul
	
	"%SPAdminTool%" -o execadmsvcjobs >nul 2>nul

	ECHO Deleting solution %SearchWSP% - %LCID% from SharePoint ...
	ECHO.
	"%SPAdminTool%" -o deletesolution -name "%SearchWSP%" -lcid %LCID% -override >nul 2>nul

	ECHO Deleting solution %SearchConfigUIWSP% - %LCID% from SharePoint ...
	ECHO.
	"%SPAdminTool%" -o deletesolution -name "%SearchConfigUIWSP%" -lcid %LCID% -override >nul 2>nul

	ECHO Deleting solution %ArchiveWSP% - %LCID% from SharePoint ...
	ECHO.
	"%SPAdminTool%" -o deletesolution -name "%ArchiveWSP%" -lcid %LCID% -override >nul 2>nul

	ECHO Deleting solution %AdministrationWSP% - %LCID% from SharePoint ...
	ECHO.
	"%SPAdminTool%" -o deletesolution -name "%AdministrationWSP%" -lcid %LCID% -override >nul 2>nul

	"%SPAdminTool%" -o execadmsvcjobs >nul 2>nul
	
	GOTO :EOF	
	

@REM -----------------------------------------------------------------
@REM LIISReset
@REM -----------------------------------------------------------------
:LIISReset
	IF "%Automated%"=="1" (
		SET ResetIIS=y
	)
	
	IF "%ResetIIS%"=="" SET /P ResetIIS=Run iisreset /noforce now (Y/N) (default is N)?
	IF "%ResetIIS%"=="" SET ResetIIS=n
	IF "%ResetIIS%"=="y" SET ResetIIS=y
	IF "%ResetIIS%"=="Y" SET ResetIIS=y

	IF %ResetIIS%==y (
		iisreset.exe /NOFORCE
		ECHO.
	)
	
	IF NOT %ResetIIS%==y (
		ECHO.
		ECHO To finish deployment, you must run "iisreset /noforce" on each Web server. 
		ECHO. 
	)
	
	GOTO :EOF
	
@REM -----------------------------------------------------------------
@REM LDeploy
@REM -----------------------------------------------------------------
:LDeploy
	ECHO Start Windows SharePoint Services Administration
	ECHO.
	IF "%SPVersion%" == "12" (
		NET START "Windows SharePoint Services Administration" >nul 2>nul
	)
	IF "%SPVersion%" == "14" (
		NET START "SharePoint 2010 Administration" >nul 2>nul
	)
	IF "%Force%" == "1" (
		CALL :LDetect
	)

	IF NOT "%Installed%" == "1" (
	
		ECHO Adding solution %AdministrationWSP% to SharePoint ...
		"%SPAdminTool%" -o addsolution -filename "%RootFolder%\SourceOneArchive\SourceOneAdmin\%AdministrationWSP%"
		
		ECHO Adding solution %ArchiveWSP% to SharePoint ...
		"%SPAdminTool%" -o addsolution -filename "%RootFolder%\SourceOneArchive\Archiving\%ArchiveWSP%"
		
		ECHO Adding solution %SearchConfigUIWSP% to SharePoint ...
		"%SPAdminTool%" -o addsolution -filename "%RootFolder%\SourceOneSearch\ArchiveSearchConfigUI\%SearchConfigUIWSP%"
		
		ECHO Adding solution %SearchWSP% to SharePoint ...
		"%SPAdminTool%" -o addsolution -filename "%RootFolder%\SourceOneSearch\ArchiveSearch\%SearchWSP%"
		
		ECHO Adding solution %RestoreWSP% to SharePoint ...
		"%SPAdminTool%" -o addsolution -filename "%RootFolder%\SourceOneRestore\Restore\%RestoreWSP%"
		
	) ELSE (
		ECHO Upgrading solution %AdministrationWSP% to SharePoint ...
		"%SPAdminTool%" -o upgradesolution -name "%AdministrationWSP%" -filename "%RootFolder%\SourceOneArchive\SourceOneAdmin\%AdministrationWSP%" -immediate -allowGacDeployment
		
		ECHO Upgrading solution %ArchiveWSP% to SharePoint ...
		"%SPAdminTool%" -o upgradesolution -name "%ArchiveWSP%" -filename "%RootFolder%\SourceOneArchive\Archiving\%ArchiveWSP%" -immediate -allowGacDeployment
		
		ECHO Upgrading solution %SearchConfigUIWSP% to SharePoint ...
		"%SPAdminTool%" -o upgradesolution -name "%SearchConfigUIWSP%" -filename "%RootFolder%\SourceOneSearch\ArchiveSearchConfigUI\%SearchConfigUIWSP%" -immediate -allowGacDeployment
		
		ECHO Upgrading solution %SearchWSP% to SharePoint ...
		"%SPAdminTool%" -o upgradesolution -name "%SearchWSP%" -filename "%RootFolder%\SourceOneSearch\ArchiveSearch\%SearchWSP%" -immediate -allowGacDeployment
		
		ECHO Upgrading solution %RestoreWSP% to SharePoint ...
		"%SPAdminTool%" -o upgradesolution -name "%RestoreWSP%" -filename "%RootFolder%\SourceOneRestore\Restore\%RestoreWSP%" -immediate -allowGacDeployment
	)
	
	ECHO Deploying solution %AdministrationWSP% ...
	ECHO.
	"%SPAdminTool%" -o deploysolution -name "%AdministrationWSP%" -immediate -allowGacDeployment -url %TargetAdminSiteUrl% -force >nul 2>nul
	
	ECHO Deploying solution %ArchiveWSP% ...
	ECHO.
	"%SPAdminTool%" -o deploysolution -name "%ArchiveWSP%" -immediate -allowGacDeployment -url %TargetSiteUrl% -force >nul 2>nul
	
	ECHO Deploying solution %SearchConfigUIWSP% ...
	ECHO.
	"%SPAdminTool%" -o deploysolution -name "%SearchConfigUIWSP%" -immediate -allowGacDeployment -url %TargetAdminSiteUrl% -force >nul 2>nul
	
	ECHO Deploying solution %SearchWSP% ...
	ECHO.
	"%SPAdminTool%" -o deploysolution -name "%SearchWSP%" -immediate -allowGacDeployment -url %TargetSiteUrl% -force >nul 2>nul
	
	ECHO Deploying solution %RestoreWSP% ...
	ECHO.
	"%SPAdminTool%" -o deploysolution -name "%RestoreWSP%" -immediate -allowGacDeployment -url %TargetSiteUrl% -force >nul 2>nul
	
	"%SPAdminTool%" -o execadmsvcjobs >nul 2>nul
	
	IF NOT "%LCIDS%"=="" (
		FOR %%a IN (%LCIDS%) DO (
			CALL :LGetLanguage %%a
			CALL :LDeployLanguagePack
		)
	)
	
	CALL :LIISReset
	
	IF "%Automated%"=="1" (
		SET ActivateFeatureNow=y
	)
	IF NOT "%Automated%"=="1" (
		SET ActivateFeatureNow=
	)
	
	IF "%ActivateFeatureNow%"=="" SET /P ActivateFeatureNow=Activate EMCES1.AdminSiteProvisioning feature now(Y/N)?
	IF "%ActivateFeatureNow%"=="" SET ActivateFeatureNow=y
	IF "%ActivateFeatureNow%"=="y" SET ActivateFeatureNow=y
	IF "%ActivateFeatureNow%"=="Y" SET ActivateFeatureNow=y

	IF %ActivateFeatureNow%==y (
		ECHO.
		ECHO Activate EMCES1.AdminSiteProvisioning feature ...
		"%SPAdminTool%" -o activatefeature -filename EMCES1.AdminSiteProvisioning\feature.xml -force
	)
	
	IF "%Automated%"=="1" (
		SET ActivateFeatureNow=y
	)
	IF NOT "%Automated%"=="1" (
		SET ActivateFeatureNow=
	)
	
	ECHO Please make sure every front-end server has EMCES1.Services installed.
	ECHO.
	
	IF "%ActivateFeatureNow%"=="" SET /P ActivateFeatureNow=Activate EMCES1.Services feature now(Y/N)?
	IF "%ActivateFeatureNow%"=="" SET ActivateFeatureNow=y
	IF "%ActivateFeatureNow%"=="y" SET ActivateFeatureNow=y
	IF "%ActivateFeatureNow%"=="Y" SET ActivateFeatureNow=y

	IF %ActivateFeatureNow%==y (
		ECHO.
		ECHO Activate EMCES1.Services feature ...
		"%SPAdminTool%" -o activatefeature -filename EMCES1.Services\feature.xml -url %TargetSiteUrl% -force
	)
	
	IF %ActivateFeatureNow%==n (
		ECHO.
		ECHO Please activate EMCES1.Services feature manually later.
		ECHO.
	)
	
	IF "%Automated%"=="1" (
		SET ActivateFeatureNow=y
	)
	IF NOT "%Automated%"=="1" (
		SET ActivateFeatureNow=
	)
	
	IF "%ActivateFeatureNow%"=="" SET /P ActivateFeatureNow=Activate EMCES1.ArchiveSearchConfig feature now(Y/N)?
	IF "%ActivateFeatureNow%"=="" SET ActivateFeatureNow=y
	IF "%ActivateFeatureNow%"=="y" SET ActivateFeatureNow=y
	IF "%ActivateFeatureNow%"=="Y" SET ActivateFeatureNow=y

	IF %ActivateFeatureNow%==y (
		ECHO.
		ECHO Activate EMCES1.ArchiveSearchConfig feature ...
		"%SPAdminTool%" -o activatefeature -filename EMCES1.ArchiveSearchConfig\feature.xml -url %TargetAdminSiteUrl%/%ArchiveSite% -force
	)
	
	IF %ActivateFeatureNow%==n (
		ECHO.
		ECHO Please activate EMCES1.ArchiveSearchConfig feature manually later.
		ECHO.
	)
	
	IF "%Automated%"=="1" (
		SET ActivateFeatureNow=y
	)
	IF NOT "%Automated%"=="1" (
		SET ActivateFeatureNow=
	)
	
	IF "%ActivateFeatureNow%"=="" SET /P ActivateFeatureNow=Activate EMCES1.RestoreServices feature now(Y/N)?
	IF "%ActivateFeatureNow%"=="" SET ActivateFeatureNow=y
	IF "%ActivateFeatureNow%"=="y" SET ActivateFeatureNow=y
	IF "%ActivateFeatureNow%"=="Y" SET ActivateFeatureNow=y

	IF %ActivateFeatureNow%==y (
		ECHO.
		ECHO Activate EMCES1.RestoreServices feature ...
		"%SPAdminTool%" -o activatefeature -filename EMCES1.RestoreServices\feature.xml -url %TargetSiteUrl% -force
	)
	
	IF %ActivateFeatureNow%==n (
		ECHO.
		ECHO Please activate EMCES1.RestoreServices feature manually later.
		ECHO.
	)

	GOTO :EOF
@REM -----------------------------------------------------------------
@REM LRetract
@REM -----------------------------------------------------------------
:LRetract
	IF NOT "%Installed%" == "1" (
		ECHO %ArchiveWSP% solution not deployed.
		GOTO :EOF
	)
		
	ECHO Start Windows SharePoint Services Administration
	ECHO.
	IF "%SPVersion%" == "12" (
		NET START "Windows SharePoint Services Administration" >nul 2>nul
	)
	IF "%SPVersion%" == "14" (
		NET START "SharePoint 2010 Administration" >nul 2>nul
	)	
	
	ECHO Deactivate EMCES1.RestoreServices feature ...
	"%SPAdminTool%" -o deactivatefeature -filename EMCES1.RestoreServices\feature.xml -url %TargetSiteUrl% -force
	
	ECHO Uninstalling feature EMCES1.RestoreServices ...
	"%SPAdminTool%" -o uninstallfeature -filename EMCES1.RestoreServices\feature.xml -force
	
	ECHO Uninstalling feature EMCES1.ArchiveSearchCore ...
	"%SPAdminTool%" -o uninstallfeature -filename EMCES1.ArchiveSearchCore\feature.xml -force
	
	ECHO Uninstalling feature EMCES1.ArchiveSearchWebParts ...
	"%SPAdminTool%" -o uninstallfeature -filename EMCES1.ArchiveSearchWebParts\feature.xml -force
	
	ECHO Deactivate EMCES1.ArchiveSearchConfig feature ...
	"%SPAdminTool%" -o deactivatefeature -filename EMCES1.ArchiveSearchConfig\feature.xml -url %TargetAdminSiteUrl%/%ArchiveSite% -force
	
	ECHO Uninstalling feature EMCES1.ArchiveSearchConfig ...
	"%SPAdminTool%" -o uninstallfeature -filename EMCES1.ArchiveSearchConfig\feature.xml -force
	
	ECHO Deactivate EMCES1.Services feature ...
	"%SPAdminTool%" -o deactivatefeature -filename EMCES1.Services\feature.xml -url %TargetSiteUrl% -force
	
	ECHO Uninstalling feature EMCES1.Services ...
	"%SPAdminTool%" -o uninstallfeature -filename EMCES1.Services\feature.xml -force
	
	ECHO Deactivate EMCES1.AdminSiteProvisioning feature ...
	"%SPAdminTool%" -o deactivatefeature -filename EMCES1.AdminSiteProvisioning\feature.xml -force
	
	ECHO Uninstalling feature EMCES1.AdminSiteProvisioning ...
	"%SPAdminTool%" -o uninstallfeature -filename EMCES1.AdminSiteProvisioning\feature.xml -force
	
	ECHO Uninstalling feature EMCES1.AdminSiteElements ...
	"%SPAdminTool%" -o uninstallfeature -filename EMCES1.AdminSiteElements\feature.xml -force
	
	IF NOT "%LCIDS%"=="" (
		FOR %%a IN (%LCIDS%) DO (
			CALL :LGetLanguage %%a
			CALL :LRetractLanguagePack
		)
	)
	
	ECHO Deactivate EMCES1.AdminSiteProvisioning feature ...
	"%SPAdminTool%" -o deactivatefeature -filename EMCES1.AdminSiteProvisioning\feature.xml -force
	
	ECHO Uninstalling feature EMCES1.AdminSiteProvisioning ...
	"%SPAdminTool%" -o uninstallfeature -filename EMCES1.AdminSiteProvisioning\feature.xml -force
	
	ECHO Uninstalling feature EMCES1.AdminSiteElements ...
	"%SPAdminTool%" -o uninstallfeature -filename EMCES1.AdminSiteElements\feature.xml -force
	
	IF NOT "%LCIDS%"=="" (
		FOR %%a IN (%LCIDS%) DO (
			CALL :LGetLanguage %%a
			CALL :LRetractLanguagePack
		)
	)
	
	ECHO Retracting solution %RestoreWSP% ...
	ECHO.
	"%SPAdminTool%" -o retractsolution -name "%RestoreWSP%" -immediate >nul 2>nul
	
	ECHO Retracting solution %SearchWSP% ...
	ECHO.
	"%SPAdminTool%" -o retractsolution -name "%SearchWSP%" -immediate -url %TargetSiteUrl% >nul 2>nul
	
	ECHO Retracting solution %SearchConfigUIWSP% ...
	ECHO.
	"%SPAdminTool%" -o retractsolution -name "%SearchConfigUIWSP%" -immediate -url %TargetSiteUrl% >nul 2>nul
	
	ECHO Retracting solution %ArchiveWSP% ...
	ECHO.
	"%SPAdminTool%" -o retractsolution -name "%ArchiveWSP%" -immediate -url %TargetSiteUrl% >nul 2>nul
	
	ECHO Retracting solution %AdministrationWSP% ...
	ECHO.
	"%SPAdminTool%" -o retractsolution -name "%AdministrationWSP%" -immediate -url %TargetSiteUrl% >nul 2>nul
	
	"%SPAdminTool%" -o execadmsvcjobs >nul 2>nul
	
	ECHO Deleting solution %RestoreWSP% from SharePoint ...
	ECHO.
	"%SPAdminTool%" -o deletesolution -name "%RestoreWSP%" -override >nul 2>nul
	
	ECHO Deleting solution %SearchWSP% from SharePoint ...
	ECHO.
	"%SPAdminTool%" -o deletesolution -name "%SearchWSP%" -override >nul 2>nul
	
	ECHO Deleting solution %SearchConfigUIWSP% from SharePoint ...
	ECHO.
	"%SPAdminTool%" -o deletesolution -name "%SearchConfigUIWSP%" -override >nul 2>nul

	ECHO Deleting solution %ArchiveWSP% from SharePoint ...
	ECHO.
	"%SPAdminTool%" -o deletesolution -name "%ArchiveWSP%" -override >nul 2>nul
	
	ECHO Deleting solution %AdministrationWSP% from SharePoint ...
	ECHO.
	"%SPAdminTool%" -o deletesolution -name "%AdministrationWSP%" -override >nul 2>nul
	
	"%SPAdminTool%" -o execadmsvcjobs >nul 2>nul
	
	CALL :LIISReset
	
	GOTO :EOF
	
@REM -----------------------------------------------------------------
@REM LTerminate
@REM -----------------------------------------------------------------
:LTerminate
    SET UserInput=
    SET /P UserInput=Hit enter key to quit.

    SET SPLocation=
    SET SPVersion=
    SET SPAdminTool=
    SET Install=
    SET Uninstall=
    SET TargetAdminSiteUrl=
    SET TargetSiteUrl=
    SET SPTemplateLocation=
    SET SPFeaturesLocation=
    SET SPSiteTemplateLocation=
    SET SPWebTempFileLocation=
    SET ValidationFailed=
    SET UserInput=
    SET LCID=
    SET LCIDS=
    SET Language=
    SET Force=
    SET Installed=
    SET RootFolder=
    SET AdministrationWSP=
    SET ArchiveWSP=
    SET SearchWSP=
    SET SearchConfigUIWSP=
    SET RestoreWSP=
    SET ArchiveSite=
	
popd
endlocal
