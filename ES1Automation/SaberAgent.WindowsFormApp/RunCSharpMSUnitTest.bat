::	Author:			Neil Wang
::	Description:	
::		This tool builds the solution then run the test by specifying the WebID of the test case in RQM server.
::	Usage:			RunTest.bat WebId=1,WebId=2 "C:\TestResult.xml" "C:\SaberAgent\AutomationFramework\Main\CSharpNUnit\Saber"
::
::
set MSTestExe="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\MSTest.exe"
set MSBuildExe="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set MSTFExe="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\tf.exe"
IF "X%~2"=="X" (
	ECHO Please specify the WebId to run
	GOTO :ERROR
)
IF "X%~3"=="X" (
	ECHO Please specify the result file name, typically, it's the id of the test case
	GOTO :ERROR
)
IF "X%~4"=="X" (
	ECHO Please specify the results directory
	GOTO :ERROR
)
IF "X%~5"=="X" (
	ECHO Please specify SVC system's root path, such as AutomationFramework/ES1Automation/Main/SourceOne/CSharpNUnit/SupervisorWebAutomation
	GOTO :ERROR
)
IF "X%~6"=="X" (
	ECHO Please specify solution file, such as SupervisorWebAutomation.sln
	GOTO :ERROR
)
IF "X%~7"=="X" (
	ECHO Please specify the test container, such as SupervisorWebAutomation_API\bin\Debug\SupervisorWebAutomation_API.dll
	GOTO :ERROR
)
IF "X%~8"=="X" (
	ECHO Please specify the test settings, such as TraceAndTestImpact.testsettings
	GOTO :ERROR
)
C:
set CodePath=C:\SaberAgent\AutomationScripts

cd  %CodePath%\%~5
%MSBuildExe% %~6

ECHO Run the test
%MSTestExe% /testcontainer:%~7 /testsettings:%~8 /category:"%~2" /resultsfile:"%~4\%~3"


ECHO %errorlevel%
ECHO The test cases has been finished successfully

GOTO :EOF
:ERROR
ECHO Error met when run test cases
GOTO :EOF
:EOF