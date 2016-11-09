::	Author:			Neil Wang
::	Description:	
::		This tool sync the latest code from the TFS server
::		At the same time, it will sync the Common folder in TFS which contains the support functionalites for Saber.
::	Usage:			SyncTFSCode.bat /C "AutomationFramework/ES1Automation/Main/CISTest" "http://10.37.11.121:8080/tfs/defaultcollection" otg\username password
::  Precondition:
::      VSTS 2010SP1 is installed on this machine

IF "X%~2"=="X" (
    ECHO Please specify the path of code to sync
	GOTO :ERROR
)
IF "X%~3"=="X" (
    ECHO Please specify the server URL
	GOTO :ERROR
)
IF "X%~4"=="X" (
    ECHO Please specify the login user
	GOTO :ERROR
)
IF "X%~5"=="X" (
    ECHO Please specify the login password
	GOTO :ERROR
)

C:
cd C:\SaberAgent

ECHO Sync the latest code

set MSTFExe="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe"

for /f "tokens=1-4 delims=:." %%a in ('ECHO %time%') do (
SET HHMMSS=%%a%%b%%c%%d
)

set TempCodePath=C:\SaberAgent\TempScripts_%HHMMSS%

set CodePath=C:\SaberAgent\AutomationScripts

IF EXIST %TempCodePath% (
rd %TempCodePath% /S /Q
)

IF NOT EXIST %TempCodePath% (
mkdir %TempCodePath%
)

cd %TempCodePath%

SET TempWorkSpaceName="Temp_Workspace_%HHMMSS%"

%MSTFExe% workspace /delete %TempWorkSpaceName% /collection:%~3 /login:%~4,%~5 /noprompt
%MSTFExe% workspace /new %TempWorkSpaceName% /collection:%~3 /login:%~4,%~5 /noprompt
%MSTFExe% get "%~2" /version:T /login:%~4,%~5 /noprompt /recursive /force

ECHO get the common folder which contains the common module for all the testing
%MSTFExe% get "AutomationFramework/ES1Automation/Main/SourceOne/Common" /version:T /login:%~4,%~5 /noprompt /recursive /force

%MSTFExe% workspace /delete %TempWorkSpaceName% /collection:%~3 /login:%~4,%~5 /noprompt

IF EXIST %CodePath% (
rd %CodePath% /S /Q
)

IF NOT EXIST %CodePath% (
mkdir %CodePath%
)

::xcopy %TempCodePath% %CodePath% /Q /E

robocopy %TempCodePath% %CodePath% /MIR

IF EXIST %TempCodePath% (
cd ..
rd %TempCodePath% /S /Q
)

ECHO The test scripts have been synced from TFS Server successfully
GOTO :EOF

:ERROR
ECHO	Author:			Neil Wang
ECHO	Description:	
ECHO		This tool sync the latest code from the TFS server
ECHO	Usage:			SyncTFSCode.bat /C "AutomationFramework/ES1Automation/Main/CISTest"
ECHO    Precondition:
ECHO      VSTS 2010SP1 is installed on this machine
GOTO :EOF
:EOF