#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common2.au3"
#include <Date.au3>
#include <WinAPI.au3>


AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\DMServer_Logging.txt",9)

Dim $hWnd
Dim $WinTitle = "EMC SourceOne Discovery Manager Server Installation"
Dim $WinRestartPopup = "EMC SourceOne Discovery Manager Server Installation"

; Read the INI file for the value, IniRead ( "filename", "section", "key", "default" )
; Overwrite the S1 arguments according to ini. If not found, then use default value.

Local $InstallerLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")

Local $Domain = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Domain", "qaes1.com")
Local $ServiceAccount = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ServiceAccount", "es1service")
Local $Password = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Password", "emcsiax@QA")
Local $SecurityGroup = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SecurityGroup", "es1 security group")
Local $WebServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "WebServer", "work01")


Local $DMDB = IniRead(@ScriptDir & "\Config.ini", "S1Account", "DMDB", "work01")
Local $DMDBServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "DMDBServer", "work01")



FileWriteLine($LogFile,_Now() & ":INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
Local $Setup=0
Local $hSearch = FindFile($InstallerLocation, "ES1_DiscoveryMgrServerSetup.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_DiscoveryMgrServerSetup.exe is NOT found")
	MsgBox(64,"Warning!","ES1_DiscoveryMgrServerSetup.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_DiscoveryMgrServerSetup.exe is found: " & $Setup )
	FlushLogging()

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
Dim $IdDMServerRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdDMServerRepairCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdWebServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdDMDB = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdDMDBServer = "[CLASS:RichEdit20W; INSTANCE:1]"


ElseIf $InstallerVersion == "7.12" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.12 IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdDMServerRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdDMServerRepairCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdWebServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdDMDB = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdDMDBServer = "[CLASS:RichEdit20W; INSTANCE:1]"

Else

FileWriteLine($LogFile,_Now() & ":INFO: Set other version IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdDMServerRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdDMServerRepairCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdWebServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdDMDB = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdDMDBServer = "[CLASS:RichEdit20W; INSTANCE:1]"

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
			FileWriteLine($LogFile,_Now() & ":INFO: DMServer Install operation has been started..")
			DMServerInstall()
		ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ":INFO: DMServer Uninstall operation has been started..")
			If CheckS1RegKeyExist($RegDMServer) == True Then
				DMServerUninstall()
			Else
				FileWriteLine($LogFile,_Now() & ":INFO: DMServer Registry is not found, hence exited...")
				Exit
			EndIf
		ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ":INFO: DMServer Repair operation has been started..")
			DMServerRepair()
		ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: DMServer Upgrade operation has been started..")
			DMServerUpgrade()
		EndIf
	  EndIf
   EndIf

EndFunc

Func DMServerInstall()
	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Next >")


	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDMDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: DMServer Install Done!")

	; dont restart
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc


Func DMServerUpgrade()

	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Update >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Update >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDMDBPage()
	ClickButtonByName($WinTitle, "&Next >")


	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: DMServer Upgrade Done!")

	; dont restart
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc

Func DMServerUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdDMServerRemoveCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Remove")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: DMServer Remove Done!")

	; dont restart
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc

Func DMServerRepair()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)


	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdDMServerRepairCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDMDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: DMServer Repair Done!")

	; dont restart
	ClickButtonByName($WinRestartPopup, "&NO")

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
   ;Input Activity DB
   ControlFocus($WinTitle,"",$IdActDBNameEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ActDBName)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Activity DB with:" & $ActDBName)

   ;Input Activity DB Server
   ControlFocus($WinTitle,"",$IdActDBServerEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ActDBServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Activity DB server with:" & $ActDBServer)

   Sleep($CONFIG_DELAY_TIME)

EndFunc



Func ConfigDMDBPage()

	Sleep($CONFIG_DELAY_TIME)

   ;Input DMDB
   ControlFocus($WinTitle,"",$IdDMDB)
   Sleep($CONFIG_DELAY_TIME)
   Send($DMDB)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure DMDB with:" & $DMDB)

   ;Input DMDB Server
   ControlFocus($WinTitle,"",$IdDMDBServer)
   Sleep($CONFIG_DELAY_TIME)
   Send($DMDBServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure DMDB with:" & $DMDBServer)

   Sleep($CONFIG_DELAY_TIME)

EndFunc