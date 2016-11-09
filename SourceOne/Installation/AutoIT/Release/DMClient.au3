#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common.au3"
#include <Date.au3>
#include <WinAPI.au3>


AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\DMClient_Logging.txt",9)

Dim $hWnd
Dim $WinTitle = "EMC SourceOne Discovery Manager Client Installation"

; Read the INI file for the value, IniRead ( "filename", "section", "key", "default" )
; Overwrite the S1 arguments according to ini. If not found, then use default value.

Local $InstallerLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")

Local $DMWebServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "WebServer ", "Work01")


FileWriteLine($LogFile,_Now() & ":INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
Local $Setup=0
Local $hSearch = FindFile($InstallerLocation, "ES1_DiscoveryMgrClientSetup.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_DiscoveryMgrClientSetup.exe is NOT found")
	MsgBox(64,"Warning!","ES1_DiscoveryMgrClientSetup.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_DiscoveryMgrClientSetup.exe is found: " & $Setup )
	FlushLogging()

EndIf


Local $InstallerVersion = StringLeft(FileGetVersion($Setup, "FileVersion"),4)
FileWriteLine($LogFile,_Now() & ":INFO: Installer version is " & $InstallerVersion )

; Get IDs from per different branch
If  $InstallerVersion == "7.20" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.20 IDs" )
; To Maintain in future

Dim $IdDMWebServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"

Dim $IdDMClientRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdDMClientRepairCheckBox = "[CLASS:Button; INSTANCE:2]"

ElseIf $InstallerVersion == "7.12" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.12 IDs" )
; To Maintain in future

Dim $IdDMWebServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"

Dim $IdDMClientRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdDMClientRepairCheckBox = "[CLASS:Button; INSTANCE:2]"

Else

FileWriteLine($LogFile,_Now() & ":INFO: Set other version IDs" )
; To Maintain in future

Dim $IdDMWebServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"

Dim $IdDMClientRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdDMClientRepairCheckBox = "[CLASS:Button; INSTANCE:2]"

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
			FileWriteLine($LogFile,_Now() & ":INFO: DMClient Install operation has been started..")
			DMClientInstall()
		ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ":INFO: DMClient Uninstall operation has been started..")
			If CheckRegKeyExist($S1_REG_CURRENT_USER , $RegDMClient ) == True Then
				DMClientUninstall()
			Else
				FileWriteLine($LogFile,_Now() & ":INFO: DMClient Registry is not found, hence exited...")
				Exit
			EndIf
		ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ":INFO: DMClient Repair operation has been started..")
			DMClientRepair()
		ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: DMClient Upgrade operation has been started..")
			DMClientUpgrade()
		EndIf
	  EndIf
   EndIf

EndFunc


Func DMClientInstall()
	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ConfigWebServicesPage()
	ClickButtonByName($WinTitle, "&Next >")

	WaitAndClickButton($WinTitle,"&Next >")

	WaitAndClickButton($WinTitle,"&Install")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf


	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
    FileWriteLine($LogFile, _Now() & "INFO: DMClient Install Done!")


EndFunc


Func DMClientUpgrade()

	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Update >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Update >")

	ConfigWebServicesPage()
	ClickButtonByName($WinTitle, "&Next >")

	WaitAndClickButton($WinTitle,"&Next >")

	WaitAndClickButton($WinTitle,"&Install")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf


	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
    FileWriteLine($LogFile, _Now() & "INFO: DMClient Upgrade Done!")



EndFunc

Func DMClientUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdDMClientRemoveCheckBox)
	ClickButtonByName($WinTitle, "&Next >")


	ClickButtonByName($WinTitle, "&Remove")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: DM Client Remove Done!")

EndFunc

Func DMClientRepair()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsDMClientCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdDMClientRepairCheckBox)
	ClickButtonByName($WinTitle, "&Next >")


	ConfigWebServicesPage()
	ClickButtonByName($WinTitle, "&Next >")

	WaitAndClickButton($WinTitle,"&Next >")

	WaitAndClickButton($WinTitle,"&Install")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf


	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
    FileWriteLine($LogFile, _Now() & "INFO: DMClient Repair Done!")




EndFunc


Func ConfigWebServicesPage()

	Sleep($CONFIG_DELAY_TIME)

   ;Input Web Server
   ControlFocus($WinTitle,"",$IdDMWebServerEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($DMWebServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure DM Web server with:" & $DMWebServer)

   Sleep($CONFIG_DELAY_TIME)

EndFunc



