::	Author:			Neil Wang
::	Description:	
::		This tool sync the latest code from the clearcase server
::	Usage:			SyncClearCaseCode.bat /C "SourceOne" "http://10.98.17.63:16080/ccrc" otg\user password "C\SaberAgent\CSPs\SourceOne.csp"
::  Precondition:
::      The CCRC tool is installed on the machine

IF "X%~2"=="X" (
    ECHO Please specify root folder of the code
	GOTO :USAGE
)
IF "X%~3"=="X" (
    ECHO Please specify the ClearCase server URL
	GOTO :USAGE
)
IF "X%~4"=="X" (
    ECHO Please specify the login user for ClearCase server
	GOTO :USAGE   
)
IF "X%~5"=="X" (
	ECHO Please specify the login password for ClearCase server
	GOTO :USAGE
)
IF "X%~6"=="X" (
	ECHO Please specify the config spec for the view
	GOTO :USAGE
)

for /f "tokens=1-4 delims=:." %%a in ('ECHO %time%') do (
SET HHMMSS=%%a%%b%%c%%d
)

IF EXIST "C:\IBM\RationalSDLC\clearcase\RemoteClient\rcleartool.bat" (
SET RClearToolPath="C:\IBM\RationalSDLC\clearcase\RemoteClient\rcleartool.bat"
)

IF EXIST "C:\Program Files (x86)\IBM\RationalSDLC\clearcase\RemoteClient\rcleartool.bat" (
SET RClearToolPath="C:\Program Files (x86)\IBM\RationalSDLC\clearcase\RemoteClient\rcleartool.bat"
)

SET CodePath="C:\SaberAgent\AutomationScripts"

SET TempCodePath="C:\SaberAgent\TempCode_%HHMMSS%"

IF EXIST %TempCodePath% (
rd %TempCodePath% /S /Q
)

SET TempViewName="Galaxy_Automation_Temp_%HHMMSS%"

call %RClearToolPath% logoff -server %~3
call %RClearToolPath% rmview -lname %~4 -pass %5 -server %~3 -tag %TempViewName% -force

call %RClearToolPath% mkview -lname %~4 -pass %5 -server %~3 -tag %TempViewName% %TempCodePath%
C:
cd %TempCodePath%
call %RClearToolPath% setcs -lname %~4 -pass %5 -server %~3 %~6

IF EXIST %CodePath% (
rd %CodePath% /S /Q
)

IF NOT EXIST %CodePath% (
mkdir %CodePath%
)

robocopy %TempCodePath% %CodePath% /MIR

call %RClearToolPath% rmview -lname %~4 -pass %5 -server %~3 -tag %TempViewName% -force
call %RClearToolPath% logoff -server %~3

IF EXIST %TempCodePath% (
cd ..
rd %TempCodePath% /S /Q
)

ECHO Successfully sync the latest code from ClearCase server
goto :END

:USAGE
ECHO	Author:			Neil Wang
ECHO	Description:	
ECHO		This tool sync the latest code from the clearcase server
ECHO	Usage:			SyncClearCaseCode.bat /C "SourceOne" "http://10.98.17.63:16080/ccrc" otg\user password "C\SaberAgent\CSPs\SourceOne.csp"
ECHO    Precondition:
ECHO      The CCRC tool is installed on the machine
goto :END
:END