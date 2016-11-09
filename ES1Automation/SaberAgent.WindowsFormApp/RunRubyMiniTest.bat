IF "X%~2"=="X" (
	ECHO Please specify the WebId of the test case to run
	GOTO :ERROR
)
IF "X%~3"=="X" (
	ECHO Please specify the ruby file to run
	GOTO :ERROR
)
IF "X%~4"=="X" (
	ECHO Please specify the working directory -- The root folder of the test scripts
	GOTO :ERROR
)
IF "X%~5"=="X" (
	ECHO Please specify the log file path -- The detailed log will be written into this file
	GOTO :ERROR
)

C:
cd "%~4"
ruby.exe "%~3" -n %~2>"%~5"
GOTO :END

:ERROR
ECHO Invaid command parameters.
ECHO Sample command:
ECHO RunRubyMiniTest.bat /C /WebId_21/ "C:\SaberAgent\AutomationScrips\AutomationFramework\Saber\sample_test.rb" "C:\SaberAgent\AutomationScrips\AutomationFramework\Saber" "C:\SaberAgent\Results\4\webid_10.log"

:END