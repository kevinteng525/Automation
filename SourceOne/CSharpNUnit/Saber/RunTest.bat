::	Author:			Neil Wang
::	Description:	
::		This tool sync the latest code from the TFS server, build the solution then run the test by specifying the WebID of the test case in RQM server.
::	Usage:			RunTest.bat 1
::
::
set NUnitConsoleExe="C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console-x86.exe"
set MSBuildExe="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set MSTFExe="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe"
IF "%1"=="" (
	GOTO :EOF
)
C:
cd C:\SaberAgent
echo Sync the latest code
rd AutomationFramework /S /Q
%MSTFExe% workspace /delete SaberTest /login:es1\wangn6,wangn6 /noprompt
%MSTFExe% workspace /new SaberTest /collection:http://tfs.es1.com:8080/tfs/defaultcollection /login:es1\wangn6,wangn6 /noprompt
%MSTFExe% get AutomationFramework/ES1Automation/Main/Saber /version:T /login:es1\wangn6,wangn6 /noprompt /recursive
echo Build the code
cd C:\SaberAgent\AutomationFramework\ES1Automation\Main\Saber
%MSBuildExe% Saber.sln
echo Run the test
%NUnitConsoleExe% S1AutomationTest.nunit /include=%1 /result=%1.xml /work=C:\SaberAgent\Result
echo %errorlevel%
:EOF