#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common2.au3"
#include <Date.au3>
#include <WinAPI.au3>


AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\FileBCE_Logging.txt",9)

Dim $hWnd
Dim $WinTitle = "EMC SourceOne Business Component Extensions for File Archiving"
; Read the INI file for the value, IniRead ( "filename", "section", "key", "default" )
; Overwrite the S1 arguments according to ini. If not found, then use default value.
Local $Domain = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Domain", "qaes1.com")
Local $ServiceAccount = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ServiceAccount", "es1service")
Local $Password = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Password", "emcsiax@QA")
Local $SecurityGroup = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Security", "es1 security group")
Local $InstallerLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")





FileWriteLine($LogFile,_Now() & ":INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
Local $Setup=0
Local $hSearch = FindFile($InstallerLocation, "ES1_FileArchiveBCESetup.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_FileArchiveBCESetup.exe is NOT found")
	MsgBox(64,"Warning!","ES1_FileArchiveBCESetup.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_FileArchiveBCESetup.exe is found: " & $Setup )
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
Dim $UpgradeIdServiceAccountPasswordEdit = "[CLASS:RichEdit20W; INSTANCE:3]"

Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $UpgradeIdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:4]"

Dim $IdFileBCERemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdFileBCERepairCheckBox = "[CLASS:Button; INSTANCE:2]"

ElseIf $InstallerVersion == "7.12" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.12 IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdFileBCERemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdFileBCERepairCheckBox = "[CLASS:Button; INSTANCE:2]"

ElseIf
   FileWriteLine($LogFile,_Now() & ":INFO: Set Other IDs" )
; To Maintain in future
Dim $IdDomainEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdServiceAccountEdit = "[CLASS:RichEdit20W; INSTANCE:2]"
Dim $IdServiceAccountPasswordEdit = "[CLASS:Edit; INSTANCE:1]"
Dim $IdSecurityGroupEdit = "[CLASS:RichEdit20W; INSTANCE:3]"
Dim $IdFileBCERemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdFileBCERepairCheckBox = "[CLASS:Button; INSTANCE:2]"

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

			FileBCEInstall()
		ElseIf $CmdLine[2] = "Rm" Then

			FileBCEUninstall()
		ElseIf $CmdLine[2] = "Re" Then

			FileBCERepair()
		ElseIf $CmdLine[2] = "U" Then

			FileBCEUpgrade()
		EndIf
	  EndIf
   EndIf

EndFunc

Func FileBCEInstall()
   FileWriteLine($LogFile,_Now() & ":INFO: FileBCE Install operation has been started..")
	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	If CheckS1RegKeyExist("Worker+") == True Then

		ClickButtonByName($WinTitle, "&Next >")

		ConfigSecurityGroupPage()

		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Install")

;~ 		$btnFinishID= IsFileBCEFinish($WinTitle, "&Finish")
;~ 		FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		 EndIf

		 Sleep($CONFIG_DELAY_TIME)

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: FileBCE Install Done!")


	ElseIf CheckS1RegKeyExist("Worker+") == False And CheckS1RegKeyExist("Console+") == True  Then




		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Install")

 		;$btnFinishID= IsFileBCEFinish($WinTitle, "&Finish")
 		;FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit

		EndIf

		Sleep($CONFIG_DELAY_TIME)

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: FileBCE Install Done!")

	 ElseIf CheckS1RegKeyExist("Worker+") == False And CheckS1RegKeyExist("Console+") == False  Then

		 ClickButtonByName($WinTitle, "&Next >")
		 FileWriteLine($LogFile, _Now() & ":miss Worker+ and Console+, have to exit...")
		 MsgBox($MB_OK, "Warnning", "miss Worker+ and Console+, have to exit....", 5)


	  Exit

	EndIf

EndFunc


Func FileBCEUpgrade()
   FileWriteLine($LogFile,_Now() & ":INFO: FileBCE Upgrade operation has been started..")


	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)


	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Update >", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	If CheckS1RegKeyExist("Worker+") == True Then

		ClickButtonByName($WinTitle, "&Update >")

		ConfigSecurityGroupPage()

		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Install")

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		EndIf

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: FileBCE Upgrade Done!")



	ElseIf CheckS1RegKeyExist("Worker+") == False And CheckS1RegKeyExist("Console+") == True Then

		ClickButtonByName($WinTitle, "&Update >")

		ClickButtonByName($WinTitle, "&Install")

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		EndIf

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: FileBCE Upgrade Done!")

   ElseIf CheckS1RegKeyExist("Worker+") == False And CheckS1RegKeyExist("Console+") == False  Then


	  FileWriteLine($LogFile, _Now() & ":miss Worker+ and Console+, have to exit...")
	  MsgBox($MB_OK, "Warnning", "miss Worker+ and Console+, have to exit....", 5)

	  Exit
	EndIf

EndFunc

Func FileBCEUninstall()
   FileWriteLine($LogFile,_Now() & ":INFO: FileBCE Uninstall operation has been started..")
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")
    ClickPageCheckBoxByID($WinTitle, $IdFileBCERemoveCheckBox)

	ClickButtonByName($WinTitle, "&Next >")


	ClickButtonByName($WinTitle, "&Remove")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: FileBCE Remove Done!")



EndFunc

Func FileBCERepair()
   FileWriteLine($LogFile,_Now() & ":INFO: FileBCE Repair operation has been started..")
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller($Setup)

	;Wait for installer loading completes

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsFileBCECompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf



	If CheckS1RegKeyExist("Worker+") == True Then

		ClickButtonByName($WinTitle, "&Next >")

		ClickPageCheckBoxByID($WinTitle, $IdFileBCERepairCheckBox)
		ClickButtonByName($WinTitle, "&Next >")

		ConfigSecurityGroupPage()
		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Install")

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		EndIf

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: FileBCE Repair Done!")


	ElseIf CheckS1RegKeyExist("Worker+") == False And CheckS1RegKeyExist("Console+") == True Then

		ClickButtonByName($WinTitle, "&Next >")

		ClickPageCheckBoxByID($WinTitle, $IdFileBCERepairCheckBox)
		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Install")

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		EndIf

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: FileBCE Repair Done!")
   ElseIf CheckS1RegKeyExist("Worker+") == False And CheckS1RegKeyExist("Console+") == False  Then

	  ClickButtonByName($WinTitle, "&Next >")
	  FileWriteLine($LogFile, _Now() & ":miss Worker+ and Console+, have to exit...")
	  MsgBox($MB_OK, "Warnning", "miss Worker+ and Console+, have to exit....", 5)
	  Exit


	EndIf
EndFunc





Func ConfigSecurityGroupPage()
   ;Abandon ControlSetText method since it's not supported in SourceOne account password validation

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
