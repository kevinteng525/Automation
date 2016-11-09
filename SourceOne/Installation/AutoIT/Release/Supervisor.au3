#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common.au3"
#include <Date.au3>
#include <WinAPI.au3>



AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

FileDelete("C:\Scripts\Supervisor_Logging.txt")
Dim $LogFile = FileOpen("C:\Scripts\Supervisor_Logging.txt",9)

;~ Dim $hWnd
Dim $WinTitle = "EMC SourceOne Email Supervisor Installation"
Dim $WinWizardTitle = "EMC SourceOne Email Supervisor - InstallShield Wizard"
Dim $WinRestartPopup = "EMC SourceOne Email Supervisor Installation"
Dim $WinCdrivePopup = "EMC SourceOne Email Supervisor"


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

Local $SupDBServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupDBServer", "SQL01")
Local $SupDBName =  IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupDBName", "Supervisor")
Local $SupAuthentication = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupAuthentication", "Windows")
Local $SupDomain = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupDomain", "domain")
Local $SupDBUserName = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupDBUserName", "es1service")
Local $SupPassword = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupPassword", "qampass1!")
Local $SupDisplayName = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupDisplayName", "es1 service")
Local $SupAccount = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupAccount", "es1service")
Local $SupRequireSSL = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupRequireSSL", "True")
Local $SupServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupServer", "True")
Local $SupManager = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupManager", "True")
Local $SupDatabase = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupDatabase", "True")
Local $SupWeb = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupWeb", "True")
;Local $SupReg = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupReg", "Supervisor")
Local $SupUpgradeWeb = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupUpgradeWeb", "True")

; Load SourceOne Platform configs from config.ini
Local $EnableExchange = IniRead(@ScriptDir & "\Config.ini", "Platform", "Exchange", "false")
Local $EnableDomino = IniRead(@ScriptDir & "\Config.ini", "Platform", "Domino", "false")
Local $EnableO365 = IniRead(@ScriptDir & "\Config.ini", "Platform", "O365", "false")


FileWriteLine($LogFile,_Now() & ": INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
$SuperviosrLocation = $InstallerLocation & "Setup"
Local $Setup=0
Local $hSearch = FindFile($SuperviosrLocation, "ES1_SupervisorSetup.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ": INFO: ES1_SupervisorSetup.exe is NOT found")
	MsgBox(64,"Warning!","ES1_SupervisorSetup.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ": INFO: ES1_SupervisorSetup.exe is found: " & $Setup )
	FlushLogging()

EndIf


Local $InstallerVersion = StringLeft(FileGetVersion($Setup, "FileVersion"),4)
FileWriteLine($LogFile,_Now() & ": INFO: Installer version is " & $InstallerVersion )

; Get IDs from per different branch
If  $InstallerVersion == "7.21" Then
FileWriteLine($LogFile,_Now() & ": INFO: Set 7.21 IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdActDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdActDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdDominoCheckBox = "[CLASS:Button; INSTANCE:4]"
Dim $IdExchangeCheckBox = "[CLASS:Button; INSTANCE:5]"
Dim $IdO365CheckBox = "[CLASS:Button; INSTANCE:6]"
Dim $IdMasterRemoveCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdMasterRepairCheckBox = "[CLASS:Button; INSTANCE:3]"

Dim $IdSupDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:4]"
Dim $IdSupWinAuthenticationBtn = "[CLASS:Button; INSTANCE:4]"
Dim $IdSupSqlAuthenticationBtn = "[CLASS:Button; INSTANCE:5]"
Dim $IdSupDBDomainEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdSupDBUserNameEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdSupPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSupDisplayNameBrowseBtn = "[CLASS:Button; INSTANCE:6]"
Dim $IdSupProfileBrowseBtn = "[CLASS:Button; INSTANCE:5]"
Dim $IdSupExchNameSearchEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupDominoNameSearchEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupIBMNotesPwdEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSupServiceAccountDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdSupServiceAccountPwdEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSupIISAccountDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupIISAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdSupIISAccountPwdEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSupRequireSSLBtn = "[CLASS:Button; INSTANCE:3]"

ElseIf  $InstallerVersion == "7.20" Then
FileWriteLine($LogFile,_Now() & ": INFO: Set 7.20 IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdActDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdActDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdDominoCheckBox = "[CLASS:Button; INSTANCE:4]"
Dim $IdExchangeCheckBox = "[CLASS:Button; INSTANCE:5]"
Dim $IdO365CheckBox = "[CLASS:Button; INSTANCE:6]"
Dim $IdMasterRemoveCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdMasterRepairCheckBox = "[CLASS:Button; INSTANCE:3]"

Dim $IdSupDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:4]"
Dim $IdSupWinAuthenticationBtn = "[CLASS:Button; INSTANCE:4]"
Dim $IdSupSqlAuthenticationBtn = "[CLASS:Button; INSTANCE:5]"
Dim $IdSupDBDomainEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdSupDBUserNameEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdSupPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSupDisplayNameBrowseBtn = "[CLASS:Button; INSTANCE:6]"
Dim $IdSupProfileBrowseBtn = "[CLASS:Button; INSTANCE:5]"
Dim $IdSupExchNameSearchEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupDominoNameSearchEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupIBMNotesPwdEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSupServiceAccountDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdSupServiceAccountPwdEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSupIISAccountDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupIISAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdSupIISAccountPwdEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSupRequireSSLBtn = "[CLASS:Button; INSTANCE:3]"

ElseIf $InstallerVersion == "7.12" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.12 IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdActDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdActDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdDominoCheckBox = "[CLASS:Button; INSTANCE:4]"
Dim $IdExchangeCheckBox = "[CLASS:Button; INSTANCE:5]"
Dim $IdO365CheckBox = "[CLASS:Button; INSTANCE:6]"
Dim $IdMasterRemoveCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdMasterRepairCheckBox = "[CLASS:Button; INSTANCE:3]"

Else

FileWriteLine($LogFile,_Now() & ":INFO: Set other version IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdActDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdActDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdDominoCheckBox = "[CLASS:Button; INSTANCE:4]"
Dim $IdExchangeCheckBox = "[CLASS:Button; INSTANCE:5]"
Dim $IdO365CheckBox = "[CLASS:Button; INSTANCE:6]"
Dim $IdMasterRemoveCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdMasterRepairCheckBox = "[CLASS:Button; INSTANCE:3]"

EndIf




_main()



Func _main()

   FileWriteLine($LogFile,_Now() & ": INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile,_Now() & ": INFO: Get OS Arch" & @OSArch)

	If IsAdmin() Then
		FileWriteLine($LogFile,_Now() & ": INFO: IsAdmin: Admin rights are detected.")
	EndIf

	If $CmdLine[0] = 2 Then
		If $CmdLine[1] = "Param1" Then
			If $CmdLine[2] = "I" Then
				FileWriteLine($LogFile,_Now() & ": INFO: Supervisor Install operation has been started..")
				SupervisorInstall()
			ElseIf $CmdLine[2] = "Rm" Then
				FileWriteLine($LogFile,_Now() & ": INFO: Supervisor Uninstall operation has been started..")
				If CheckS1RegKeyExist($RegSup) == True Then
					SupervisorUninstall()
				Else
					FileWriteLine($LogFile,_Now() & ": INFO: Supervisor Registry is not found, hence exited...")
					Exit
				EndIf
			ElseIf $CmdLine[2] = "U" Then
				FileWriteLine($LogFile,_Now() & ": INFO: Supervisor Upgrade operation has been started..")
				SupervisorUpgrade()
			EndIf
		EndIf
   EndIf

EndFunc


Func SupervisorInstall()
	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;handle prerequisites window
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinWizardTitle,"EMC SourceOne Email Supervisor requires the following items to be installed on your computer. ") Then
		WinActivate($WinWizardTitle)
		FileWriteLine($LogFile,_Now() &": INFO: Email Supervisor prerequisites window found and activated.")
		Sleep($CONFIG_DELAY_TIME)
		ClickButtonByName($WinWizardTitle, "Install")
		FileWriteLine($LogFile,_Now() &": INFO: Install button clicked.")
	EndIf



	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
	EndIf
	ClickButtonByName($WinTitle, "&Next >")

	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle,"Installation Features") Then
		InstallationFeaturesPage()
		ClickButtonByName($WinTitle, "&Next >")
	else
		FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Installation Features page, have to exit...")
		Exit
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If $InstallerVersion = "7.20" Then
		If WinExists($WinTitle," 1. SourceOne SRE 7.20 or above must be installed on the system.") Then
			FileWriteLine($LogFile, _Now() & ": ERROR: SourceOne SRE 7.20 should be installed first , Supervisor install is not complete.")
			Exit
		EndIf
	ElseIf $InstallerVersion = "7.21" Then
		If WinExists($WinTitle," 1. SourceOne SRE 7.20 or above must be installed on the system.") Then
			FileWriteLine($LogFile, _Now() & ": ERROR: SourceOne SRE 7.20 should be installed first , Supervisor install is not complete.")
			Exit
		EndIf
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle,"Database Server") Then
		ConfigDBPage()
		ClickButtonByName($WinTitle, "&Next >")
	else
		FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Database Server page, have to exit...")
		Exit
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If $InstallerVersion = "7.20" Then
		If WinExists($WinCdrivePopup, "The database version of EMC SourceOne Email Supervisor is '7.20'.") Then
			FileWriteLine($LogFile, _Now() & ": INFO: The database version of EMC SourceOne Email Supervisor is '7.20'.")
			ClickButtonByName($WinCdrivePopup, "OK")
		EndIf
	ElseIf $InstallerVersion = "7.21" Then
		If WinExists($WinCdrivePopup, "The database version of EMC SourceOne Email Supervisor is '7.21'.") Then
			FileWriteLine($LogFile, _Now() & ": INFO: The database version of EMC SourceOne Email Supervisor is '7.21'.")
			ClickButtonByName($WinCdrivePopup, "OK")
		EndIf
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle, "Email Account Information") Then
		ConfigAdminEmailAccountPage()
		ClickButtonByName($WinTitle, "&Next >")
	Else
		FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Email Account Information page, have to exit...")
		Exit
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle, "Service Account Credentials") Then
		ConfigServiceAccountPage()
		ClickButtonByName($WinTitle, "&Next >")
	Else
		FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Service Account Credentials page, have to exit...")
		Exit
	EndIf

	;Only selected Web App, show the IIS config page
	Sleep($CONFIG_DELAY_TIME)
	If $SupWeb = "True" Then
		If WinExists($WinTitle, "IIS Application Pool Identity and HTTPS Options settings") Then
			ConfigIISPage()
			ClickButtonByName($WinTitle, "&Next >")
		Else
			FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load IIS config page, have to exit...")
			Exit
		EndIf
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle, "Ready to Install the Program") Then
		ClickButtonByName($WinTitle, "&Install")
	Else
		FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Ready to Install page, have to exit...")
		Exit
	EndIf

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ": INFO: Finish button Clicked!")

	; If Notes configuration pops, cancel it.
	Sleep($NOTES_CONFIGURATION_LOAD_TIME)
	If WinExists("IBM Notes Social Edition Client Configuration") Then
		WinActivate("IBM Notes Social Edition Client Configuration")
		FileWriteLine($LogFile,_Now() &": INFO: Notes configuration window found and activated.")
		Sleep($CONFIG_DELAY_TIME)
		ClickButtonByName("IBM Notes Social Edition Client Configuration", "Cancel")
		FileWriteLine($LogFile,_Now() &": INFO: Notes configuration skipped.")
		FileWriteLine($LogFile, _Now() & ": INFO: Supervisor Install Done with Notes!")
		Sleep($CONFIG_DELAY_TIME)
	Else
		FileWriteLine($LogFile, _Now() & ": INFO: Supervisor Install Done without Notes!")
	EndIf

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc

Func SupervisorUpgrade()

	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
	EndIf
	ClickButtonByName($WinTitle, "&Next >")

	If WinExists($WinTitle, "Installation Features") Then
		If $SupUpgradeWeb = "False" Then
			ClickButtonByName($WinTitle, "Web Application (Reviewer and Reports)")
			ClickButtonByName($WinTitle, "&Next >")
		Else
			ClickButtonByName($WinTitle, "&Next >")
		EndIf
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle,"Database Server") Then
		ConfigDBPage()
		ClickButtonByName($WinTitle, "&Next >")
	else
		FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Database Server page, have to exit...")
		Exit
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinCdrivePopup, "The database version of EMC SourceOne Email Supervisor is '7.01'.") Then
		FileWriteLine($LogFile, _Now() & ": INFO: The database version of EMC SourceOne Email Supervisor currently is '7.01'.")
		ClickButtonByName($WinCdrivePopup, "OK")
	ElseIf WinExists($WinCdrivePopup, "The database version of EMC SourceOne Email Supervisor is '7.20'.") Then
		FileWriteLine($LogFile, _Now() & ": INFO: The database version of EMC SourceOne Email Supervisor currently is '7.20'.")
		ClickButtonByName($WinCdrivePopup, "OK")
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle, "Email Account Information") Then
		ConfigAdminEmailAccountPage()
		ClickButtonByName($WinTitle, "&Next >")
	Else
		FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Email Account Information page, have to exit...")
		Exit
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle, "Service Account Credentials") Then
		ControlFocus($WinTitle,"",$IdSupServiceAccountPwdEdit )
		Sleep($CONFIG_DELAY_TIME)
		Send($SupPassword,1)
		FileWriteLine($LogFile,_Now() &": INFO: Successfully configure service account password with:" & $SupPassword)
		ClickButtonByName($WinTitle, "&Next >")
	Else
		FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Service Account Credentials page, have to exit...")
		Exit
	EndIf

	;Only selected Web App, show the IIS config page
	Sleep($CONFIG_DELAY_TIME)
	If $SupUpgradeWeb = "True" Then
		If WinExists($WinTitle, "IIS Application Pool Identity and HTTPS Options settings") Then
			ControlFocus($WinTitle,"",$IdSupIISAccountPwdEdit )
			Sleep($CONFIG_DELAY_TIME)
			Send($SupPassword,1)
			FileWriteLine($LogFile,_Now() &": INFO: Successfully configure service account password with:" & $SupPassword)
			ClickButtonByName($WinTitle, "&Next >")
		Else
			FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load IIS config page, have to exit...")
			Exit
		EndIf
	EndIf

	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinTitle, "Ready to Install the Program") Then
		ClickButtonByName($WinTitle, "&Install")
	Else
		FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Ready to Install page, have to exit...")
		Exit
	EndIf

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ": INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & ": INFO: Supervisor Upgrade Done!")

	; If Notes configuration pops, cancel it.
	Sleep($NOTES_CONFIGURATION_LOAD_TIME)
	If WinExists("IBM Notes Social Edition Client Configuration") Then
		WinActivate("IBM Notes Social Edition Client Configuration")
		FileWriteLine($LogFile,_Now() &": INFO: Notes configuration window found and activated.")
		Sleep($CONFIG_DELAY_TIME)
		ClickButtonByName("IBM Notes Social Edition Client Configuration", "Cancel")
		FileWriteLine($LogFile,_Now() &": INFO: Notes configuration skipped.")
		FileWriteLine($LogFile, _Now() & ": INFO: Supervisor Install Done with Notes!")
		Sleep($CONFIG_DELAY_TIME)
	Else
		FileWriteLine($LogFile, _Now() & ": INFO: Supervisor Install Done without Notes!")
	EndIf

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc

Func SupervisorUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"Welcome","&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
	Else
		FileWriteLine($LogFile, _Now() & ": INFO: Ready to Uninstall")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")
	ClickButtonByName($WinTitle, "&Remove")
	ClickButtonByName($WinTitle, "&Next >")
	ClickButtonByName($WinTitle, "&Remove")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish",$DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ": INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & ": INFO: Supervisor Remove Done!")

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc

Func InstallationFeaturesPage()
	FileWriteLine($LogFile,_Now() &": INFO: Intallation Feature page called")
	Sleep($CONFIG_DELAY_TIME)
	If($SupServer = "false")Then
		Send("{SPACE}")
		;Sleep(1000)
		Send("{DOWN 3}")
		;Sleep(1000)
		Send("{ENTER}")
		FileWriteLine($LogFILE,_Now() &": INFO: Server component is diselected")
	EndIf

	If($SupManager = "false")Then
		Send("{DOWN 1}")
		;Sleep(1000)
		Send("{SPACE}")
		;Sleep(1000)
		Send("{DOWN 3}")
		;Sleep(1000)
		Send("{ENTER}")
		Send("{UP}")
		FileWriteLine($LogFILE,_Now() &": INFO: Manager component is diselected")
	EndIf

	If($SupDatabase = "false")Then
		Send("{DOWN 2}")
		;Sleep(1000)
		Send("{SPACE}")
		;Sleep(1000)
		Send("{DOWN 3}")
		;Sleep(1000)
		Send("{ENTER}")
		Send("{UP 2}")
		FileWriteLine($LogFILE,_Now() &": INFO: Database component is diselected")
	EndIf

	If($SupWeb = "false")Then
		Send("{DOWN 3}")
		;Sleep(1000)
		Send("{SPACE}")
		;Sleep(1000)
		Send("{DOWN 3}")
		;Sleep(1000)
		Send("{ENTER}")
		FileWriteLine($LogFILE,_Now() &": INFO: Web component is diselected")
	EndIf
EndFunc

Func ConfigServiceAccountPage()
	FileWriteLine($LogFile,_Now() &": INFO: ConfigServiceAccountPage called" )
	;Abandon ControlSetText method since it's not supported in SourceOne account password validation

	;Input Domain textbox
	ControlFocus($WinTitle,"",$IdSupServiceAccountDomainEdit)
	Sleep($CONFIG_DELAY_TIME)
	Send($SupDomain)
	FileWriteLine($LogFile,_Now() &": INFO: Successfully configure domain with:" & $SupDomain)

	;Input Service Account
	ControlFocus($WinTitle,"",$IdSupServiceAccountEdit)
	Sleep($CONFIG_DELAY_TIME)
	Send($SupAccount)
	FileWriteLine($LogFile,_Now() &": INFO: Successfully configure service account with:" & $SupAccount)


	;Input Service Password
	ControlFocus($WinTitle,"",$IdSupServiceAccountPwdEdit )
	Sleep($CONFIG_DELAY_TIME)
	Send($SupPassword,1)
	FileWriteLine($LogFile,_Now() &": INFO: Successfully configure service account password with:" & $SupPassword)

EndFunc

Func ConfigDBPage()

	;Input DB server
	ControlFocus($WinTitle,"",$IdSupDBServerEdit)
	Sleep($CONFIG_DELAY_TIME)
	Send($SupDBServer)
	FileWriteLine($LogFile,_Now() &": INFO: Successfully configure DB Server with:" & $SupDBServer)

	;Input Supervisor DB name
	ControlFocus($WinTitle,"",$IdSupDBNameEdit)
	Sleep($CONFIG_DELAY_TIME)
	Send($SupDBName)
	FileWriteLine($LogFile,_Now() &": INFO: Successfully configure Supervisor DB Name with:" & $SupDBName)

	;Select authentication mode
	If $SupAuthentication = "Windows" Then
		ControlFocus($WinTitle,"", $IdSupWinAuthenticationBtn)
		Sleep($CONFIG_DELAY_TIME)
		ClickButtonByID($IdSupWinAuthenticationBtn, $WinTitle)
		FileWriteLine($LogFile,_Now() &": INFO: Select Windows Authentication" )

		ControlFocus($WinTitle,"",$IdSupDBDomainEdit)
		Sleep($CLICK_DELAY_TIME)
		Send($SupDomain)
		FileWriteLine($LogFile,_Now() &": INFO: Successfully configure Supervisor Domain Name with:" & $SupDomain)

		ControlFocus($WinTitle,"",$IdSupDBUserNameEdit)
		Sleep($CLICK_DELAY_TIME)
		Send($SupDBUserName)
		FileWriteLine($LogFile,_Now() &": INFO: Successfully configure Supervisor User Name with:" & $SupDBUserName)

		ControlFocus($WinTitle,"",$IdSupPasswordEdit)
		Sleep($CLICK_DELAY_TIME)
		Send($SupPassword,1)
		FileWriteLine($LogFile,_Now() &": INFO: Successfully configure Supervisor Password with:" & $SupPassword)

	ElseIf $SupAuthentication = "SQL" Then

	ControlFocus($WinTitle,"", $IdSupSqlAuthenticationBtn)
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByID($IdSupSqlAuthenticationBtn, $WinTitle)
	FileWriteLine($LogFile,_Now() &": INFO: Select Sql Authentication" )

	If ControlCommand($WinTitle, "", $IdSupDBDomainEdit, "IsEnable", "") Then
		FileWriteLine($LogFile,_Now() &": ERROR: $IdSupDBDomainEdit is Enable with SQL Authenticaion")
		Exit
	EndIf

	ControlFocus($WinTitle,"",$IdSupDBUserNameEdit)
	Sleep($CLICK_DELAY_TIME)
	Send($SupDBUserName)
	FileWriteLine($LogFile,_Now() &": INFO: Successfully configure Supervisor User Name with:" & $SupDBUserName)

	ControlFocus($WinTitle,"",$IdSupPasswordEdit)
	Sleep($CLICK_DELAY_TIME)
	Send($SupPassword,1)
	FileWriteLine($LogFile,_Now() &": INFO: Successfully configure Supervisor Password with:" & $SupPassword)
	Else
		FileWriteLine($LogFile,_Now() &": ERROR: Failed to configure Supervisor Authentication" )
		Exit
	EndIf
EndFunc

Func ConfigAdminEmailAccountPage()
	FileWriteLine($LogFile,_Now() &": INFO: ConfigAdminEmailAccountPage called" )
	If $EnableExchange ="True" Then
		If ControlCommand($WinTitle, "", $IdSupProfileBrowseBtn, "IsVisible", "") Then
			ClickButtonByName($WinTitle,"B&rowse...")
		Else
			FileWriteLine($LogFile,_Now() &": ERROR: Microsoft Exchange Server is disabled")
			Exit
		EndIf

		Sleep($CLICK_DELAY_TIME)
		If WinActive("Choose Profile", "") Then
			ClickButtonByName("Choose Profile","OK")
			If WinActive($WinCdrivePopup, "Exchange Cache Mode is ON.") Then
				FileWriteLine($LogFile,_Now() &": ERROR: Exchange Cache Mode is ON.")
				Exit
			EndIf
		Else
			FileWriteLine($LogFile,_Now() &": ERROR: Failed to choose profile")
			Exit
		EndIf

		If ClickButtonByID($IdSupDisplayNameBrowseBtn,$WinTitle)then
			Sleep($CLICK_DELAY_TIME)
			If WinExists("Please select one from address book", "Name") Then
				ControlFocus("Please select one from address book","",$IdSupExchNameSearchEdit)
				Sleep($CONFIG_DELAY_TIME)
				send($SupDisplayName)
				ClickButtonByName("Please select one from address book","OK")
			Else
				FileWriteLine($LogFile,_Now() &": ERROR: Failed to choose the display name")
				Exit
			EndIf
		Else
			Exit
		EndIf

	ElseIf $EnableDomino = "True" Then
		FileWriteLine($LogFile,_Now() &": INFO: Domino is enabled")

		Send("{DOWN}")
		Sleep($CONFIG_DELAY_TIME)

		If ControlCommand($WinTitle, "", $IdSupIBMNotesPwdEdit, "IsEnabled", "") Then
			ControlFocus($WinTitle,"",$IdSupIBMNotesPwdEdit)
			Sleep($CONFIG_DELAY_TIME)
			Send($SupPassword,1)
			FileWriteLine($LogFile,_Now() &": INFO: Successfully configure Notes Password with:" & $SupPassword)
		Else
			FileWriteLine($LogFile,_Now() &": ERROR: IBM Notes Password control is disabled")
			Exit
		EndIf

		If ClickButtonByID($IdSupDisplayNameBrowseBtn,$WinTitle)then
			Sleep($CONFIG_DELAY_TIME)
			If WinExists("Select User") Then
				ControlFocus("Select User","",$IdSupDominoNameSearchEdit)
				Sleep($CONFIG_DELAY_TIME)
				send($SupAccount)
				ClickButtonByName("Select User","OK")
			Else
				FileWriteLine($LogFile,_Now() &": ERROR: Failed to choose the display name")
				Exit
			EndIf
		Else
			Exit
		EndIf
	Else
	  FileWriteLine($LogFile,_Now() &": INFO: Don't know the platform, have to exit..." )
	  Exit
	EndIf

	FileWriteLine($LogFile,_Now() &": INFO: Successfully to ConfigAdminEmailAccountPage")
	Return True
EndFunc

Func ConfigIISPage()
	FileWriteLine($LogFile,_Now() &": INFO: ConfigIISPage called" )

	;Input Domain textbox
	ControlFocus($WinTitle,"",$IdSupIISAccountDomainEdit)
	Sleep($CONFIG_DELAY_TIME)
	Send($SupDomain)
	FileWriteLine($LogFile,_Now() &": INFO: Successfully configure domain with:" & $SupDomain)

	;Input Service Account
	ControlFocus($WinTitle,"",$IdSupIISAccountEdit)
	Sleep($CONFIG_DELAY_TIME)
	Send($SupAccount)
	FileWriteLine($LogFile,_Now() &": INFO: Successfully configure service account with:" & $SupAccount)


	;Input Service Password
	ControlFocus($WinTitle,"",$IdSupIISAccountPwdEdit )
	Sleep($CONFIG_DELAY_TIME)
	Send($SupPassword,1)
	FileWriteLine($LogFile,_Now() &": INFO: Successfully configure service account password with:" & $SupPassword)

	If ($SupRequireSSL == False) Then
	ClickPageCheckBoxByID($WinTitle,$IdSupRequireSSLBtn)
	FileWriteLine($LogFile,_Now() &": INFO: Require SSL checkbox is unchecked")
	EndIf
EndFunc

