#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common2.au3"
#include <Date.au3>
#include <WinAPI.au3>


;Opt("WinTitleMatchMode", 3)  ;3 = Exact title match , especially for Archive popup window identify

AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\Archive_Logging.txt",9)

Dim $hWnd
Dim $WinTitle = "EMC SourceOne Native Archive Services Installation"
Dim $WinWizardTitle = "EMC SourceOne Native Archive Services - InstallShield Wizard"

Dim $WinLess20GBPopup = "EMC SourceOne Native Archive Services"
Dim $WinCdrivePopup = "EMC SourceOne Native Archive Services"

Dim $WinRestartPopup = "EMC SourceOne Native Archive Services Installation"

; Read the INI file for the value, IniRead ( "filename", "section", "key", "default" )
; Overwrite the S1 arguments according to ini. If not found, then use default value.

Local $InstallerLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")

Local $Domain = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Domain", "qaes1.com")
Local $ServiceAccount = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ServiceAccount", "es1service")
Local $Password = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Password", "emcsiax@QA")
Local $SecurityGroup = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SecurityGroup", "es1 security group")

Local $ArchiveDB = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ArchiveDB", "ES1Archive")
Local $ArchiveDBServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ArchiveDBServer", "SQL01")



FileWriteLine($LogFile,_Now() & ":INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
Local $Setup=0
Local $hSearch = FindFile($InstallerLocation, "ES1_ArchiveSetup.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_ArchiveSetup.exe is NOT found")
	MsgBox(64,"Warning!","ES1_ArchiveSetup.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_ArchiveSetup.exe is found: " & $Setup )
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
Dim $IdArchiveRemoveCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdArchiveRepairCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdArchiveDBEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdArchiveDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"

ElseIf $InstallerVersion == "7.12" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.12 IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdArchiveRemoveCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdArchiveRepairCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdArchiveDBEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdArchiveDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"

Else

FileWriteLine($LogFile,_Now() & ":INFO: Set other version IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdArchiveRemoveCheckBox = "[CLASS:Button; INSTANCE:2]"
Dim $IdArchiveRepairCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdArchiveDBEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdArchiveDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:2]"

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
			FileWriteLine($LogFile,_Now() & ":INFO: Archive Install operation has been started..")
			ArchiveInstall()
		ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Archive Uninstall operation has been started..")
			If CheckS1RegKeyExist($RegArchive) == True Then
				ArchiveUninstall()
			Else
				FileWriteLine($LogFile,_Now() & ":INFO: Archive Registry is not found, hence exited...")
				Exit
			EndIf
		ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Archive Repair operation has been started..")
			ArchiveRepair()
		ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Archive Upgrade operation has been started..")
			ArchiveUpgrade()
		EndIf
	  EndIf
   EndIf

EndFunc


Func ArchiveInstall()
	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;handle prerequisites window
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinWizardTitle,"EMC SourceOne Native Archive Services requires the following items to be installed on your computer. ") Then
		WinActivate($WinWizardTitle)
		FileWriteLine($LogFile,_Now() &":INFO: Native Archive prerequisites window found and activated.")
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

	;handle 20GB popup window, click no to proceed
	Sleep($CONFIG_DELAY_TIME)
	
	If WinExists($WinLess20GBPopup,"None of the local non-Windows") Then   

		ClickButtonByName($WinLess20GBPopup,"&No")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	;handle  warning popup windows, click ok to proceed
	Sleep($CONFIG_DELAY_TIME)
	
	If WinExists($WinCdrivePopup,"Microsoft Windows is also installed on drive 'C:\'") Then
		ClickButtonByName($WinCdrivePopup,"OK")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	;handle C:\ warning popup windows, click ok to proceed
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinCdrivePopup,"Microsoft Windows is also installed on drive 'C:\'.") Then
		ClickButtonByName($WinCdrivePopup,"OK")
	EndIf

	;handle insufficient disk space popup windows, click ok to proceed
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinCdrivePopup,"Insufficient disk space to support indexing on this drive!") Then
		ClickButtonByName($WinCdrivePopup,"OK")
	EndIf

   
	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	; In Place Migration Page
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Archive Install Done!")

	; dont restart
	ClickButtonByName($WinRestartPopup, "&No")

EndFunc


Func ArchiveUpgrade()

	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	;handle 20GB popup window, click no to proceed
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinLess20GBPopup,"None of the local non-Windows") Then
		ClickButtonByName($WinLess20GBPopup,"&No")
	EndIf




	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	; In Place Migration Page
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Archive Upgrade Done!")

	; dont restart
	ClickButtonByName($WinRestartPopup, "&No")

EndFunc

Func ArchiveUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	;handle 20GB popup window, click no to proceed
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinLess20GBPopup,"None of the local non-Windows") Then
		ClickButtonByName($WinLess20GBPopup,"&No")
	EndIf

	ClickPageCheckBoxByID($WinTitle, $IdArchiveRemoveCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Remove")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Archive Remove Done!")

	; dont restart
	ClickButtonByName($WinRestartPopup, "&NO")

EndFunc

Func ArchiveRepair()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsArchiveCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	ClickButtonByName($WinTitle, "&Next >")

	;handle 20GB popup window, click no to proceed
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WinLess20GBPopup,"None of the local non-Windows") Then
		ClickButtonByName($WinLess20GBPopup,"&No")
	EndIf

	ClickPageCheckBoxByID($WinTitle, $IdArchiveRepairCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigDBPage()
	ClickButtonByName($WinTitle, "&Next >")

	; In Place Migration Page
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: Archive Repair Done!")

	; dont restart
	ClickButtonByName($WinRestartPopup, "&No")

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
   ControlFocus($WinTitle,"",$IdArchiveDBEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ArchiveDB)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Archive DB with:" & $ArchiveDB)

   ;Input Activity DB Server
   ControlFocus($WinTitle,"",$IdArchiveDBServerEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ArchiveDBServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Archive DB server with:" & $ArchiveDBServer)

   Sleep($CONFIG_DELAY_TIME)

EndFunc

