#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common.au3"
#include <Date.au3>
#include <WinAPI.au3>


AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\Worker_Logging.txt",9)

Dim $hWnd
Dim $WinTitle = "EMC SourceOne Worker Services Installation"
Dim $WinWizardTitle = "EMC SourceOne Worker Services - InstallShield Wizard"
Dim $WinCdrivePopup = "EMC SourceOne Worker Services"

; Read the INI file for the value, IniRead ( "filename", "section", "key", "default" )
; Overwrite the S1 arguments according to ini. If not found, then use default value.
Local $Domain = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Domain", "qaes1.com")
Local $ServiceAccount = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ServiceAccount", "es1service")
Local $Password = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Password", "emcsiax@QA")
Local $NotesPassword = IniRead(@ScriptDir & "\Config.ini", "S1Account", "NotesPassword", "emcsiax@QA")
Local $SecurityGroup = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SecurityGroup", "es1 security group")
Local $InstallerLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")
Local $ActDBName = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ActivityDB ", "ES1Activity")
Local $ActDBServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ActDBServer", "ES1")

Local $SearchDBName = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SearchDB ", "ES1Search")
Local $SearchDBServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SearchDBServer", "ES1")

Local $WorkerJobLogs = IniRead(@ScriptDir & "\Config.ini", "S1Account", "JobLogs", "\\")



FileWriteLine($LogFile,_Now() & ":INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
Local $Setup=0
Local $hSearch = FindFile($InstallerLocation, "ES1_WorkerSetup.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_WorkerSetup.exe is NOT found")
	MsgBox(64,"Warning!","ES1_WorkerSetup.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_WorkerSetup.exe is found: " & $Setup )


EndIf


Local $InstallerVersion = StringLeft(FileGetVersion($Setup, "FileVersion"),4)
FileWriteLine($LogFile,_Now() & ":INFO: Installer version is " & $InstallerVersion )

; Get IDs from per different branch
If  $InstallerVersion == "7.20" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.20 IDs" )
; To Maintain in future

Dim $IdJoblogsEdit="[CLASS:RichEdit20W; INSTANCE:1]"

Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdActDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdActDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"

Dim $IdSearchDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdSearchDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:4]"


Dim $IdWorkerRemoveCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdWorkerRepairCheckBox = "[CLASS:Button; INSTANCE:3]"

ElseIf $InstallerVersion == "7.12" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.12 IDs" )
; To Maintain in future

Dim $IdJoblogsEdit="[CLASS:RichEdit20W; INSTANCE:1]"

Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdActDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdActDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"

Dim $IdSearchDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdSearchDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:4]"


Dim $IdWorkerRemoveCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdWorkerRepairCheckBox = "[CLASS:Button; INSTANCE:3]"

Else

FileWriteLine($LogFile,_Now() & ":INFO: Set other version IDs" )
; To Maintain in future

Dim $IdJoblogsEdit="[CLASS:RichEdit20W; INSTANCE:1]"

Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdActDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdActDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"

Dim $IdSearchDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdSearchDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:4]"


Dim $IdWorkerRemoveCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdWorkerRepairCheckBox = "[CLASS:Button; INSTANCE:3]"

EndIf




_main()



Func _main()

   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Arch" & @OSArch)

    If IsAdmin() Then
		FileWriteLine($LogFile,_Now() & "INFO: IsAdmin: Admin rights are detected.")
	EndIf

	If $CmdLine[0] = 2 Then
	  If $CmdLine[1] = "Param1" Then
		If $CmdLine[2] = "I" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Worker Install operation has been started..")
			WorkerInstall()
		ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Worker Uninstall operation has been started..")
			If CheckS1RegKeyExist($RegWorker) == True Then
				WorkerUninstall()
			Else
				FileWriteLine($LogFile,_Now() & ":INFO: Worker Registry is not found, hence exited...")
				Exit
			EndIf
		ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Worker Repair operation has been started..")
			WorkerRepair()
		ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Worker Upgrade operation has been started..")
			WorkerUpgrade()
		EndIf
	  EndIf
   EndIf

EndFunc

Func WorkerInstall()
	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;handle prerequisites window
	Sleep($BIG_DELAY_TIME)
	If WinExists($WinWizardTitle,"EMC SourceOne Worker Services requires the following items to be installed on your computer.") Then
		WinActivate($WinWizardTitle)
		FileWriteLine($LogFile,_Now() &":INFO: Worker prerequisites window found and activated.")
	    Sleep($CONFIG_DELAY_TIME)
	    ClickButtonByName($WinWizardTitle, "Install")
	    FileWriteLine($LogFile,_Now() &": INFO: Install button clicked.")
	EndIf


	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Next >")

   	Sleep($CONFIG_DELAY_TIME)
   	If WinExists($WinCdrivePopup,"Microsoft Windows is also installed on drive 'C:\'.") Then
		ClickButtonByName($WinCdrivePopup,"OK")
	EndIf


	ConfigJobLogsPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")


	;Handle Domino password configuration
	Sleep($BIG_DELAY_TIME)
	If WinExists($WinTitle,"Please provide your IBM Lotus Notes credentials below.") Then
		; to do , handle Notes password as a function
		Sleep($CONFIG_DELAY_TIME)
	   ;Input Domain textbox
	   ControlFocus($WinTitle,"","[CLASS:Edit; INSTANCE:1]")
	   Sleep($CONFIG_DELAY_TIME)
	   Send($NotesPassword)
	   FileWriteLine($LogFile,_Now() &":INFO:Successfully send IBM Notes password with:" & $NotesPassword)

	   ClickButtonByName($WinTitle, "&Next >")
	   Sleep($CONFIG_DELAY_TIME)
	EndIf

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Install", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf
	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf


	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

	; If Notes configuration pops, cancel it.
	Sleep($NOTES_CONFIGURATION_LOAD_TIME)
	If WinExists("IBM Notes Client Configuration") Then
		WinActivate("IBM Notes Client Configuration")
		FileWriteLine($LogFile,_Now() &":INFO: Notes configuration window found and activated.")
	   Sleep($CONFIG_DELAY_TIME)
	   ClickButtonByName("IBM Notes Client Configuration", "Cancel")
	   FileWriteLine($LogFile,_Now() &":INFO: Notes configuration skipped.")
	   FileWriteLine($LogFile, _Now() & "INFO: Worker Install Done with Notes!")
	   Sleep($CONFIG_DELAY_TIME)
	Else
	  FileWriteLine($LogFile, _Now() & "INFO: Worker Install Done without Notes!")
	EndIf

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName("EMC SourceOne Worker Services Installation", "&NO")

EndFunc


Func WorkerUpgrade()

	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ConfigJobLogsPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	;Handle Domino password configuration
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle,"Please provide your IBM Lotus Notes credentials below.") Then
		; to do , handle Notes password as a function
		Sleep($CONFIG_DELAY_TIME)
	   ;Input Domain textbox
	   ControlFocus($WinTitle,"","[CLASS:Edit; INSTANCE:1]")
	   Sleep($CONFIG_DELAY_TIME)
	   Send($NotesPassword)
	   FileWriteLine($LogFile,_Now() &":INFO:Successfully send IBM Notes password with:" & $NotesPassword)

	   ClickButtonByName($WinTitle, "&Next >")
	   Sleep($CONFIG_DELAY_TIME)
	EndIf


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Install", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf
	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

	; If Notes configuration pops, cancel it.
	Sleep($NOTES_CONFIGURATION_LOAD_TIME)
	If WinExists("IBM Notes Client Configuration") Then
		WinActivate("IBM Notes Client Configuration")
		FileWriteLine($LogFile,_Now() &":INFO: Notes configuration window found and activated.")
	   Sleep($CONFIG_DELAY_TIME)
	   ClickButtonByName("IBM Notes Client Configuration", "Cancel")
	   FileWriteLine($LogFile,_Now() &":INFO: Notes configuration skipped.")
	   FileWriteLine($LogFile, _Now() & "INFO: Worker Upgrade Done with Notes!")
	   Sleep($CONFIG_DELAY_TIME)
	Else
	  FileWriteLine($LogFile, _Now() & "INFO: Worker Upgrade Done without Notes!")
	EndIf

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName("EMC SourceOne Worker Services Installation", "&NO")

EndFunc

Func WorkerUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdWorkerRemoveCheckBox)
	ClickButtonByName($WinTitle, "&Next >")


	ClickButtonByName($WinTitle, "&Remove")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Worker Remove Done!")

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName("EMC SourceOne Worker Services Installation", "&NO")

EndFunc

Func WorkerRepair()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsWorkerCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdWorkerRepairCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ConfigJobLogsPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	;Handle Domino password configuration
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle,"Please provide your IBM Lotus Notes credentials below.") Then
		; to do , handle Notes password as a function
		Sleep($CONFIG_DELAY_TIME)
	   ;Input Domain textbox
	   ControlFocus($WinTitle,"","[CLASS:Edit; INSTANCE:1]")
	   Sleep($CONFIG_DELAY_TIME)
	   Send($NotesPassword)
	   FileWriteLine($LogFile,_Now() &":INFO:Successfully send IBM Notes password with:" & $NotesPassword)

	   ClickButtonByName($WinTitle, "&Next >")
	   Sleep($CONFIG_DELAY_TIME)
	EndIf


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Install", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf
	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

	; If Notes configuration pops, cancel it.
	Sleep($NOTES_CONFIGURATION_LOAD_TIME)
	If WinExists("IBM Notes Client Configuration") Then
		WinActivate("IBM Notes Client Configuration")
		FileWriteLine($LogFile,_Now() &":INFO: Notes configuration window found and activated.")
	   Sleep($CONFIG_DELAY_TIME)
	   ClickButtonByName("IBM Notes Client Configuration", "Cancel")
	   FileWriteLine($LogFile,_Now() &":INFO: Notes configuration skipped.")
	   FileWriteLine($LogFile, _Now() & "INFO: Worker Repair Done with Notes!")
	   Sleep($CONFIG_DELAY_TIME)
	Else
	  FileWriteLine($LogFile, _Now() & "INFO: Worker Repair Done without Notes!")
	EndIf

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName("EMC SourceOne Worker Services Installation", "&NO")

EndFunc


Func ConfigJobLogsPage()
   Sleep($CONFIG_DELAY_TIME)
   ;Input Joblogs textbox
   ControlFocus($WinTitle,"",$IdJoblogsEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($WorkerJobLogs)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure JobLogs with:" & $WorkerJobLogs)
   Sleep($CONFIG_DELAY_TIME)

EndFunc

Func ConfigServiceAccountPage()
   ;Abandon ControlSetText method since it's not supported in SourceOne account password validation
	Sleep($CONFIG_DELAY_TIME)
   ;Input Domain textbox
   ControlFocus($WinTitle,"",$IdDomainEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($Domain)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure domain with:" & $Domain)

   ;Input Service Account
   ControlFocus($WinTitle,"",$IdServiceAccountEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ServiceAccount)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure service account with:" & $ServiceAccount)


	;Input Service Password
	ControlFocus($WinTitle,"",$IdServiceAccountPasswordEdit )
	Sleep($CONFIG_DELAY_TIME)
	Send($Password)
	FileWriteLine($LogFile,_Now() &":INFO:Successfully configure service account password with:" & $Password)

    ;Input Security Group
	ControlFocus($WinTitle,"",$IdSecurityGroupEdit )
	Sleep($CONFIG_DELAY_TIME)
	Send($SecurityGroup)
	FileWriteLine($LogFile,_Now() &":INFO:Successfully configure security group with:" & $SecurityGroup)

    Sleep($CONFIG_DELAY_TIME)
EndFunc


Func ConfigDBPage()

	Sleep($CONFIG_DELAY_TIME)
   ;Input Domain textbox
   ControlFocus($WinTitle,"",$IdActDBNameEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ActDBName)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Activity DB with:" & $ActDBName)

   ;Input Service Account
   ControlFocus($WinTitle,"",$IdActDBServerEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ActDBServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Activity DB server with:" & $ActDBServer)

   ControlFocus($WinTitle,"",$IdSearchDBNameEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($SearchDBName)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Search DB with:" & $SearchDBName)

   ;Input Service Account
   ControlFocus($WinTitle,"",$IdSearchDBServerEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($SearchDBServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Search DB server with:" & $SearchDBServer)


	Sleep($CONFIG_DELAY_TIME)
EndFunc

