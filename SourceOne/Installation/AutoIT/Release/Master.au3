#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common.au3"
#include <Date.au3>
#include <WinAPI.au3>


AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\Master_Logging.txt",9)

;~ Dim $hWnd
Dim $WinTitle = "EMC SourceOne Master Services Installation"
Dim $WinWizardTitle = "EMC SourceOne Master Services - InstallShield Wizard"
Dim $WinRestartPopup = "EMC SourceOne Master Services Installation"
Dim $WinCdrivePopup = "EMC SourceOne Master Services"


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

; Load SourceOne Platform configs from config.ini
Local $EnableExchange = IniRead(@ScriptDir & "\Config.ini", "Platform", "Exchange", "false")
Local $EnableDomino = IniRead(@ScriptDir & "\Config.ini", "Platform", "Domino", "false")
Local $EnableO365 = IniRead(@ScriptDir & "\Config.ini", "Platform", "O365", "false")


FileWriteLine($LogFile,_Now() & ":INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
Local $Setup=0
Local $hSearch = FindFile($InstallerLocation, "ES1_MasterSetup.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_MasterSetup.exe is NOT found")
	MsgBox(64,"Warning!","ES1_MasterSetup.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_MasterSetup.exe is found: " & $Setup )


EndIf


Local $InstallerVersion = StringLeft(FileGetVersion($Setup, "FileVersion"),4)
FileWriteLine($LogFile,_Now() & ":INFO: Installer version is " & $InstallerVersion )

; Get IDs from per different branch
If  $InstallerVersion == "7.20" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.20 IDs" )
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

   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Arch" & @OSArch)

    If IsAdmin() Then
		FileWriteLine($LogFile,_Now() & "INFO: IsAdmin: Admin rights are detected.")
	EndIf

	If $CmdLine[0] = 2 Then
	  If $CmdLine[1] = "Param1" Then
		If $CmdLine[2] = "I" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Master Install operation has been started..")
			MasterInstall()
		ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Master Uninstall operation has been started..")
			If CheckS1RegKeyExist($RegMaster) == True Then
				MasterUninstall()
			Else
				FileWriteLine($LogFile,_Now() & ":INFO: Master Registry is not found, hence exited...")
				Exit
			EndIf
		ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Master Repair operation has been started..")
			MasterRepair()
		ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Master Upgrade operation has been started..")
			MasterUpgrade()
		EndIf
	  EndIf
   EndIf

EndFunc


Func MasterInstall()
	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;handle prerequisites window
	Sleep($BIG_DELAY_TIME)
	If WinExists($WinWizardTitle,"EMC SourceOne Master Services requires the following items to be installed on your computer. ") Then
		WinActivate($WinWizardTitle)
		FileWriteLine($LogFile,_Now() &":INFO: Master prerequisites window found and activated.")
	    Sleep($CONFIG_DELAY_TIME)
	    ClickButtonByName($WinWizardTitle, "Install")
	    FileWriteLine($LogFile,_Now() &": INFO: Install button clicked.")
	EndIf



	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	Snapshot($WinTitle,"Master_InstallerPageLoaded.jpg")

	ClickButtonByName($WinTitle, "&Next >")

	Snapshot($WinTitle,"Master_Next1.jpg")

	ClickButtonByName($WinTitle, "&Next >")



	Sleep(5000)
   	If WinExists($WinCdrivePopup,"Microsoft Windows is also installed on drive 'C:\'.") Then
		FileWriteLine($LogFile, _Now() & " :INFO: WinCdrivePopup window is found , clicking OK.")

		Snapshot($WinCdrivePopup, "Master Cdrive popup.jpg")

		ClickButtonByName($WinCdrivePopup,"OK")
	EndIf


	Sleep($CONFIG_DELAY_TIME)
	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigPlatformPage($WinTitle)
	ClickButtonByName($WinTitle, "&Next >")



	;Handle Domino password configuration
	Sleep($CONFIG_DELAY_TIME)
	If $EnableDomino == "true" Then
		FileWriteLine($LogFile,_Now() &" :INFO: Notes password should be provided.")

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

		Else
			FileWriteLine($LogFile,_Now() &" : Error: Something wrong with Notes Client, or Domino should not be selected, Exiting.")
			Exit
		EndIf
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
	   FileWriteLine($LogFile, _Now() & "INFO: Master Install Done with Notes!")
	   Sleep($CONFIG_DELAY_TIME)
	Else
	  FileWriteLine($LogFile, _Now() & "INFO: Master Install Done without Notes!")
	EndIf

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc


Func MasterUpgrade()

	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigPlatformPage($WinTitle)
	ClickButtonByName($WinTitle, "&Next >")

	;Handle Domino password configuration
	Sleep($CONFIG_DELAY_TIME)
	If $EnableDomino == "true" Then
		FileWriteLine($LogFile,_Now() &" :INFO: Notes password should be provided.")

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

		Else
			FileWriteLine($LogFile,_Now() &" : Error: Something wrong with Notes Client, or Domino should not be selected, Exiting.")
			Exit
		EndIf
	EndIf



	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Master Upgrade Done!")

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc

Func MasterUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdMasterRemoveCheckBox)
	ClickButtonByName($WinTitle, "&Next >")


	ClickButtonByName($WinTitle, "&Remove")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Master Remove Done!")

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc

Func MasterRepair()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsMasterCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdMasterRepairCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigPlatformPage($WinTitle)
	ClickButtonByName($WinTitle, "&Next >")

	;Handle Domino password configuration
	Sleep($CONFIG_DELAY_TIME)
	If $EnableDomino == "true" Then
		FileWriteLine($LogFile,_Now() &" :INFO: Notes password should be provided.")

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

		Else
			FileWriteLine($LogFile,_Now() &" : Error: Something wrong with Notes Client, or Domino should not be selected, Exiting.")
			Exit
		EndIf
	EndIf


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Install", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf
	ClickButtonByName($WinTitle, "&Install")


;~ 		$btnFinishID= IsMasterFinish($WinTitle, "&Finish")
;~ 		FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

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
	   FileWriteLine($LogFile, _Now() & "INFO: Master Repair Done with Notes!")
	   Sleep($CONFIG_DELAY_TIME)
	Else
	  FileWriteLine($LogFile, _Now() & "INFO: Master Repair Done without Notes!")
	EndIf

	; dont restart
	Sleep($CONFIG_DELAY_TIME)
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc


Func ConfigServiceAccountPage()
   FileWriteLine($LogFile,_Now() &": configureserviceAccountpage called" )

   Snapshot($WinTitle,"Master_ConfigServiceAccountPage_In.jpg")

   ;Abandon ControlSetText method since it's not supported in SourceOne account password validation
	Sleep($CONFIG_DELAY_TIME)
   ;Input Domain textbox
   ControlFocus($WinTitle,"",$IdDomainEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($Domain)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure domain with:" & $Domain)

   ;Input Service Account
   Sleep($CONFIG_DELAY_TIME)
   ControlFocus($WinTitle,"",$IdServiceAccountEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ServiceAccount)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure service account with:" & $ServiceAccount)


	;Input Service Password
	Sleep($CONFIG_DELAY_TIME)
	ControlFocus($WinTitle,"",$IdServiceAccountPasswordEdit )
	Sleep($CONFIG_DELAY_TIME)
	Send($Password)
	FileWriteLine($LogFile,_Now() &":INFO:Successfully configure service account password with:" & $Password)

    ;Input Security Group
	Sleep($CONFIG_DELAY_TIME)
	ControlFocus($WinTitle,"",$IdSecurityGroupEdit )
	Sleep($CONFIG_DELAY_TIME)
	Send($SecurityGroup)
	FileWriteLine($LogFile,_Now() &":INFO:Successfully configure security group with:" & $SecurityGroup)

    Sleep($CONFIG_DELAY_TIME)

	Snapshot($WinTitle,"Master_ConfigServiceAccountPage_Out.jpg")

EndFunc


Func ConfigDBPage()

   Snapshot($WinTitle,"Master_ConfigDBPage_In.jpg")

   ;Input Domain textbox
   ControlFocus($WinTitle,"",$IdActDBNameEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ActDBName)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Activity DB with:" & $ActDBName)

   ;Input Service Account
   Sleep($CONFIG_DELAY_TIME)
   ControlFocus($WinTitle,"",$IdActDBServerEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ActDBServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Activity DB server with:" & $ActDBServer)

   Sleep($CONFIG_DELAY_TIME)

   Snapshot($WinTitle,"Master_ConfigDBPage_Out.jpg")
EndFunc



Func ConfigPlatformPage($sWinTitle)

	FileWriteLine($LogFile,_Now() &" :INFO: Exchange platform is set to " & $EnableExchange)
	FileWriteLine($LogFile,_Now() &" :INFO: Domino platform is set to " & $EnableDomino)
	FileWriteLine($LogFile,_Now() &" :INFO: O365 platform is set to " & $EnableO365)

	Snapshot($WinTitle,"Master_ConfigPlatformPage_In.jpg")

	If $EnableExchange == "true" Then
		ClickPageCheckBoxByID($sWinTitle, $IdExchangeCheckBox)
		FileWriteLine($LogFile,_Now() &" :INFO: Exchange platform clicked.")
	EndIf
	If $EnableDomino == "true" Then
		ClickPageCheckBoxByID($sWinTitle, $IdDominoCheckBox)
		FileWriteLine($LogFile,_Now() &" :INFO: Domino platform clicked.")
	EndIf
	If $EnableO365 == "true" Then
		ClickPageCheckBoxByID($sWinTitle, $IdO365CheckBox)
		FileWriteLine($LogFile,_Now() &" :INFO: O365 platform clicked.")
	EndIf

	Snapshot($WinTitle,"Master_ConfigPlatformPage_Out.jpg")

EndFunc


