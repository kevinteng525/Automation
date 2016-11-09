::	Description:	
::		This tool sync the latest code from the Git
::		
::	Usage:			SyncGITCode.bat /C "ssh://suns@code.otg.com:29418/DPSearch" "c:\temp" 
::  Precondition:
::      Git is installed and generated the pub key

IF "X%~2"=="X" (
    ECHO Please specify the path of code to sync
	GOTO :ERROR
)
IF "X%~3"=="X" (
    ECHO Please specify the destination path of the code
	GOTO :ERROR
)

IF "X%~4"=="X" (
    cmd /c git clone %2 %3 >c:\SaberAgent\syncprogress.log 2>&1
) 
IF NOT "X%~4"=="X" (
    cmd /c git clone %2 %~3 %4 >c:\SaberAgent\syncprogress.log 2>&1
)
ECHO The test scripts have been synced from Git Server successfully
GOTO :END

:ERROR
ECHO Description:	
ECHO	This tool sync the latest code from the Git
ECHO Usage:			SyncGITCode.bat /C "ssh://suns@code.otg.com:29418/DPSearch" "c:\temp" 
:END