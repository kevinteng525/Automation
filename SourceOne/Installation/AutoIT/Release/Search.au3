#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common.au3"
#include <Date.au3>
#include <WinAPI.au3>


AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\Search_Logging.txt",9)

Dim $hWnd
Dim $WinTitle = "EMC SourceOne Search Installation"
Dim $WinWizardTitle = "EMC SourceOne Search - InstallShield Wizard"


; Read the INI file for the value, IniRead ( "filename", "section", "key", "default" )
; Overwrite the S1 arguments according to ini. If not found, then use default value.

Local $InstallerLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")

Local $Domain = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Domain", "qaes1.com")
Local $ServiceAccount = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ServiceAccount", "es1service")
Local $Password = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Password", "emcsiax@QA")
Local $SecurityGroup = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SecurityGroup", "es1 security group")
Local $WebServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "WebServer", "work01")


FileWriteLine($LogFile,_Now() & ":INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
Local $Setup=0
Local $hSearch = FindFile($InstallerLocation, "ES1_SearchSetup.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_SearchSetup.exe is NOT found")
	MsgBox(64,"Warning!","ES1_SearchSetup.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_SearchSetup.exe is found: " & $Setup )


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
Dim $IdSearchRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdSearchRepairCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdWebServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"


ElseIf $InstallerVersion == "7.12" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.12 IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdSearchRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdSearchRepairCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdWebServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"

Else

FileWriteLine($LogFile,_Now() & ":INFO: Set other version IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdSearchRemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdSearchRepairCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdWebServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"

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
			FileWriteLine($LogFile,_Now() & ":INFO: Search Install operation has been started..")
			SearchInstall()
		ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Search Uninstall operation has been started..")
			If CheckS1RegKeyExist($RegWebSearch) == True Then
				SearchUninstall()
			Else
				FileWriteLine($LogFile,_Now() & ":INFO: Search Registry is not found, hence exited...")
				Exit
			EndIf
		ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Search Repair operation has been started..")
			SearchRepair()
		ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Search Upgrade operation has been started..")
			SearchUpgrade()
		EndIf
	  EndIf
   EndIf

EndFunc

Func SearchInstall()
	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;handle prerequisites window
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinWizardTitle,"EMC SourceOne Search requires the following items to be installed on your computer.") Then
		WinActivate($WinWizardTitle)
		FileWriteLine($LogFile,_Now() &":INFO: Web Search prerequisites window found and activated.")
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


	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigWebServicesPage()
	ClickButtonByName($WinTitle, "&Next >")

	;wait for web server connect done
	WinWaitActive($WinTitle,"Select the additional shortcuts you would like installed.",$WINDOW_WAIT_TIMEOUT)
	WinActivate($WinTitle)
	;/wait for web server connect done

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

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
	FileWriteLine($LogFile, _Now() & "INFO: Search Install Done!")

EndFunc


Func SearchUpgrade()

	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")


	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigWebServicesPage()
	ClickButtonByName($WinTitle, "&Next >")

	;wait for web server connect done
	WinWaitActive($WinTitle,"Select the additional shortcuts you would like installed.",$WINDOW_WAIT_TIMEOUT)
	WinActivate($WinTitle)
	;/wait for web server connect done

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
	FileWriteLine($LogFile, _Now() & "INFO: Search Upgrade Done!")

EndFunc

Func SearchUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdSearchRemoveCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Remove")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Search Remove Done!")

EndFunc

Func SearchRepair()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsSearchCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	ClickPageCheckBoxByID($WinTitle, $IdSearchRepairCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigWebServicesPage()
	ClickButtonByName($WinTitle, "&Next >")

	;wait for web server connect done
	WinWaitActive($WinTitle,"Select the additional shortcuts you would like installed.",$WINDOW_WAIT_TIMEOUT)
	WinActivate($WinTitle)
	;/wait for web server connect done

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
	FileWriteLine($LogFile, _Now() & "INFO: Search Repair Done!")

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



Func ConfigWebServicesPage()

	Sleep($CONFIG_DELAY_TIME)

   ;Input Web Server
   ControlFocus($WinTitle,"",$IdWebServerEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($WebServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Web server with:" & $WebServer)

   Sleep($CONFIG_DELAY_TIME)

EndFunc