#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common2.au3"
#include <Date.au3>
#include <WinAPI.au3>


AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\Console_Logging.txt",9)

Dim $hWnd
Dim $WinTitle = "EMC SourceOne Console Installation"
Dim $WinWizardTitle = "EMC SourceOne Console - InstallShield Wizard"

; Read the INI file for the value, IniRead ( "filename", "section", "key", "default" )
; Overwrite the S1 arguments according to ini. If not found, then use default value.

Local $Domain = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Domain", "qaes1.com")
Local $AdminsGroup = IniRead(@ScriptDir & "\Config.ini", "S1Account", "AdminsGroup", "es1 security group")
Local $InstallerLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")
Local $ActDBName = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ActivityDB ", "ES1Activity")
Local $ActDBServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ActDBServer", "ES1")

FileWriteLine($LogFile,_Now() & ":INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
Local $Setup=0
Local $hSearch = FindFile($InstallerLocation, "ES1_ConsoleSetup.exe.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_ConsoleSetup.exe.exe is NOT found")
	MsgBox(64,"Warning!","ES1_ConsoleSetup.exe.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_ConsoleSetup.exe.exe is found: " & $Setup )
	FlushLogging()

EndIf


Local $InstallerVersion = StringLeft(FileGetVersion($Setup, "FileVersion"),4)
FileWriteLine($LogFile,_Now() & ":INFO: Installer version is " & $InstallerVersion )

; Get IDs from per different branch
If  $InstallerVersion == "7.20" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.20 IDs" )
; To Maintain in future

Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdAdminsGroupEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdActDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdActDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"

Dim $IdConsoleRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdConsoleRepairCheckBox = "[CLASS:Button; INSTANCE:2]"

ElseIf $InstallerVersion == "7.12" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.12 IDs" )
; To Maintain in future

Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdAdminsGroupEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdActDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdActDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"

Dim $IdConsoleRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdConsoleRepairCheckBox = "[CLASS:Button; INSTANCE:2]"

Else

FileWriteLine($LogFile,_Now() & ":INFO: Set other version IDs" )
; To Maintain in future

Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdAdminsGroupEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdActDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdActDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"

Dim $IdConsoleRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdConsoleRepairCheckBox = "[CLASS:Button; INSTANCE:2]"

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
			FileWriteLine($LogFile,_Now() & ":INFO: Console Install operation has been started..")
			ConsoleInstall()
		ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Console Uninstall operation has been started..")
			If CheckS1RegKeyExist($RegConsole) == True Then
				ConsoleUninstall()
			Else
				FileWriteLine($LogFile,_Now() & ":INFO: Console Registry is not found, hence exited...")
				Exit
			EndIf
		ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Console Repair operation has been started..")
			ConsoleRepair()
		ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Console Upgrade operation has been started..")
			ConsoleUpgrade()
		EndIf
	  EndIf
   EndIf

EndFunc

Func ConsoleInstall()
	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;handle prerequisites window
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinWizardTitle,"EMC SourceOne Console requires the following items to be installed on your computer.") Then
		WinActivate($WinWizardTitle)
		FileWriteLine($LogFile,_Now() &":INFO: Console prerequisites window found and activated.")
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


	ConfigAdmisGroupPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	; Shortcuts Page
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Console Install Done!")

	; dont restart
	ClickButtonByName("EMC SourceOne Console Installation", "&NO")



EndFunc


Func ConsoleUpgrade()

	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ConfigAdmisGroupPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	; Shortcuts Page
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Console Upgrade Done!")

	; dont restart
	ClickButtonByName("EMC SourceOne Console Installation", "&NO")

EndFunc

Func ConsoleUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdConsoleRemoveCheckBox)
	ClickButtonByName($WinTitle, "&Next >")


	ClickButtonByName($WinTitle, "&Remove")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Console Remove Done!")

	; dont restart
	ClickButtonByName("EMC SourceOne Console Installation", "&NO")

EndFunc

Func ConsoleRepair()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsConsoleCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdConsoleRepairCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ConfigAdmisGroupPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	; Shortcuts Page
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Console Repair Done!")

	; dont restart
	ClickButtonByName("EMC SourceOne Console Installation", "&NO")

EndFunc

Func ConfigAdmisGroupPage()
   ;Abandon ControlSetText method since it's not supported in SourceOne account password validation
	Sleep($CONFIG_DELAY_TIME)
   ;Input Domain textbox
   ControlFocus($WinTitle,"",$IdDomainEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($Domain)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure domain with:" & $Domain)

    ;Input Security Group
	ControlFocus($WinTitle,"",$IdAdminsGroupEdit )
	Sleep($CONFIG_DELAY_TIME)
	Send($AdminsGroup)
	FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Admins group with:" & $AdminsGroup)

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

