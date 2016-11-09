::	Author:			Neil Wang
::	Description:	
::		This tool builds the solution then run the test by specifying the WebID of the test case in RQM server.
::	Usage:			RunTest.bat WebId=1,WebId=2 "C:\TestResult.xml" "C:\SaberAgent\AutomationFramework\Main\CSharpNUnit\Saber"
::
::
set NUnitConsoleExe="C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console-x86.exe"
set MSBuildExe="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set MSTFExe="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe"
IF "X%~2"=="X" (
	ECHO Please specify the WebId to run
	GOTO :ERROR
)
IF "X%~3"=="X" (
	ECHO Please specify the result file name, typically, it's the id of the job
	GOTO :ERROR
)
IF "X%~4"=="X" (
	ECHO Please specify the results directory
	GOTO :ERROR
)
IF "X%~5"=="X" (
	ECHO Please specify SVC system's root path, such as AutomationFramework/ES1Automation/Main/SourceOne/CSharpNUnit/Saber
	GOTO :ERROR
)

C:
set CodePath=C:\SaberAgent\AutomationScripts

cd  %CodePath%\%~5
%MSBuildExe% Saber.sln

ECHO Run the test
%NUnitConsoleExe% S1AutomationTest.nunit /include="%~2" /result="%~3" /work="%~4"
ECHO %errorlevel%
ECHO The test cases has been finished successfully

GOTO :EOF
:ERROR
ECHO Error met when run test cases
GOTO :EOF
:EOF