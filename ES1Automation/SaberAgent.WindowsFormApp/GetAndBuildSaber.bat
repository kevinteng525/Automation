::	Author:			Neil Wang
::	Description:	
::		This tool sync the latest Saber code from the TFS server, build the solution.
::	Usage:			GetAndBuildSaber.bat http://10.37.11.121:8080/tfs/defaultcollection es1\wangn6 emcsiax@QA AutomationFramework/ES1Automation/Main/Saber
::
::
set MSBuildExe="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set MSTFExe="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe"

IF "X%~2"=="X" (
	echo Please specify the SVC system's server address, such as http://10.37.11.121:8080/tfs/defaultcollection
	GOTO :ERROR
)
IF "X%~3"=="X" (
	echo Please specify SVC system's login user, such as es1\wangn6
	GOTO :ERROR
)
IF "X%~4"=="X" (
	echo Please specify SVC system's login password, such as emcsiax@QA
	GOTO :ERROR
)
IF "X%~5"=="X" (
	echo Please specify SVC system's root path, such as AutomationFramework/ES1Automation/Main/SourceOne/CSharpNUnit/Saber
	GOTO :ERROR
)
C:
cd C:\SaberAgent
echo Sync the latest code
set CodePath=C:\SaberAgent\Saber
IF EXIST %CodePath% (
rd  %CodePath% /S /Q
)
IF NOT EXIST %CodePath% (
mkdir  %CodePath%
)
cd  %CodePath%
%MSTFExe% workspace /delete SaberModule /collection:"%~2" /login:%~3,%~4 /noprompt
%MSTFExe% workspace /new SaberModule /collection:"%~2" /login:%~3,%~4 /noprompt
%MSTFExe% get %~5 /version:T /login:%~3,%~4 /noprompt /recursive /force
echo Build the code
cd  %CodePath%\%~5
%MSBuildExe% Saber.sln
GOTO :EOF
:ERROR
echo Error met when run test cases
GOTO :EOF
:EOF