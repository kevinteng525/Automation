#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common2.au3"
#include <Date.au3>
#include <WinAPI.au3>


AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\OWA2010_Logging.txt",9)

Dim $hWnd
Dim $WinTitle = "EMC SourceOne Extensions for Microsoft Office OWA 2010 Installation"


; To Maintain in future
Dim $IdDomainEdit = 2933
Dim $IdServiceAccountEdit = 2952
Dim $IdServiceAccountPasswordEdit = 2956
Dim $IdSecurityGroupEdit = 2955
Dim $IdWebSerNameEdit = 3009
Dim $IdOWARemoveCheckBox = 624
Dim $IdOWARepairCheckBox = 1554

; Read the INI file for the value, IniRead ( "filename", "section", "key", "default" )
; Overwrite the S1 arguments according to ini. If not found, then use default value.
Local $Domain = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Domain", "qaes1.com")
Local $ServiceAccount = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ServiceAccount", "es1service")
Local $Password = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Password", "emcsiax@QA")
Local $SecurityGroup = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Security", "es1 security group")
Local $WebServicesServerName = IniRead(@ScriptDir & "\Config.ini", "S1Account", "WebServicesServerName", "Work01")

Local $OWALocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")



FileWriteLine($LogFile,_Now() & ":INFO: ES1_OWA2010Setup location: " & $OWALocation )

$hSearch = FileFindFirstFile($OWALocation & "ES1_OWA2010Setup.exe")

; Check if the search was successful, if not display a message and return False.
    If $hSearch = -1 Then
         FileWriteLine($LogFile,_Now() & ":INFO: No ES1_OWA2010Setup.exe is found")
        Exit
	Else
		$OWABundle = FileFindNextFile($hSearch)
		FileWriteLine($LogFile,_Now() & ":INFO: ES1_OWA2010Setup is found at: " & $OWABundle )
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
			FileWriteLine($LogFile,_Now() & ":INFO: OWA Install operation has been started..")
			OWAInstall()
		ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ":INFO: OWA Uninstall operation has been started..")
			OWAUninstall()
		ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ":INFO: OWA Repair operation has been started..")
			OWARepair()
		ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: OWA Upgrade operation has been started..")
			OWAUpgrade()
		EndIf
	  EndIf
   EndIf

;~ 	OWAUninstall()

;~ 	OWAInstall()


;~     ;------Like Test case Here---


;~ 	Sleep($TESTCASE_INTERVAL)

;~ 	OWARepair()

;~ 	OWAUpgrade()   ; Case one : intall

;~ 	Sleep($TESTCASE_INTERVAL)

;~     OWAUninstall()  ; Case two : rollback


EndFunc

Func RunInstaller()

	FileWriteLine($LogFile, _Now() & ":INFO: Func RunInstaller")

    Run($OWALocation & $OWABundle)
	FileWriteLine($LogFile,_Now() &":INFO: OWA setup is runned")
	WinWaitActive($WinTitle, "", 60000)
	$hWnd = WinGetHandle($WinTitle, "")
	FileWriteLine($LogFile,_Now() &":INFO:Got OWA handler")
EndFunc



Func OWAInstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsOWACompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Next >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigWebServicesPage()
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")


	$btnFinishID= IsOWAFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ControlClick($WinTitle, "", $btnFinishID)

	FileWriteLine($LogFile, _Now() & "INFO: OWA Install Done!")

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

EndFunc


Func OWAUpgrade()


	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsOWACompleteLoading($WinTitle,"&Update >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Update >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigWebServicesPage()
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")


	$btnFinishID= IsOWAFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ControlClick($WinTitle, "", $btnFinishID)

	FileWriteLine($LogFile, _Now() & "INFO: OWA Upgrade Done!")

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

EndFunc

Func OWAUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsOWACompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickOWASelection($WinTitle, $IdOWARemoveCheckBox)
	ClickButtonByName($WinTitle, "&Next >")


	ClickButtonByName($WinTitle, "&Remove")



	$btnFinishID= IsOWAFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ControlClick($WinTitle, "", $btnFinishID)

	FileWriteLine($LogFile, _Now() & "INFO: OWA Remove Done!")

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

EndFunc

Func OWARepair()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsOWACompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickOWASelection($WinTitle, $IdOWARepairCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigWebServicesPage()
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$btnFinishID= IsOWAFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ControlClick($WinTitle, "", $btnFinishID)

	FileWriteLine($LogFile, _Now() & "INFO: OWA Repair Done!")

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

EndFunc








Func IsOWACompleteLoading($_WinTitle = "", $_WinText = "")
	FileWriteLine($LogFile, _Now() & ":INFO: Func IsOWACompleteLoading")
	Local $hWndTmp = WinGetHandle($_WinTitle, "")
	FileWriteLine($LogFile, _Now() & ":INFO: !!! ORI Handle Return with: " & $hWndTmp)


	WinActivate($_WinTitle)

	FileWriteLine($LogFile,_Now() & "INFO: WinGetHandle is: " & $hWndTmp )
	WinActivate($WizardTitle)


	Local $IsButtonVisible = 0
	Local $IsButtonEnabled = 0
	Local $Retry = 0

	;Wait until install is completed
	While $IsButtonVisible < 1 Or $IsButtonEnabled < 1
		If $Retry < 200 Then

			$Retry += 1
			FileWriteLine($LogFile, _Now() & ":INFO:Retry with: " & $Retry)
			FileWriteLine($LogFile, _Now() & ":INFO:Still wait for loading, wait 5 seconds...")
			Sleep(5000)

			$hWndTmp = WinGetHandle($_WinTitle, "")

			FileWriteLine($LogFile, _Now() & ":INFO:Handle Return with: " & $hWndTmp)

			Local $ctrlIDList = _GetControlIDs($hWndTmp, $_WinText)
			Local $hitID = FindControlByName($ctrlIDList, $_WinText)

			If $hitID == Null Then
				FileWriteLine($LogFile, _Now() & ":INFO: ID not found yet")
				ContinueLoop
			Else

			FileWriteLine($LogFile, _Now() & ":INFO: Found ID is: " & $hitID)

			$IsButtonVisible = ControlCommand($hWndTmp, "", $hitID, "IsVisible", "")
			$IsButtonEnabled = ControlCommand($hWndTmp, "", $hitID, "IsEnabled", "")
			FileWriteLine($LogFile, _Now() & ":INFO: Loading's done, button visible = " & $IsButtonVisible)
			FileWriteLine($LogFile, _Now() & ":INFO: Loading's done, button enable = " & $IsButtonEnabled)
			EndIf

		Else
			FileWriteLine($LogFile, _Now() & ":ERROR:Retry over 50 and continue...")
			FlushLogging()
			Return False

		EndIf
	WEnd

	Return True


EndFunc   ;==>IsPreInstall





Func IsOWAFinish($_WinTitle = "", $_WinText = "")

	Local $hWndTmp = WinGetHandle($_WinTitle, "")
	WinActivate($_WinTitle)

	FileWriteLine($LogFile,_Now() & "INFO: WinGetHandle is: " & $hWndTmp )

	Local $IsButtonVisible = 0
	Local $IsButtonEnabled = 0
	Local $Retry = 0

	;Wait until install is completed
	While $IsButtonVisible < 1 Or $IsButtonEnabled < 1
		If $Retry < 200 Then

			$Retry += 1
			FileWriteLine($LogFile, _Now() & ":INFO:Retry times: " & $Retry)
			FileWriteLine($LogFile, _Now() & ":INFO:Still wait for loading, wait " & $RETRY_TIME/1000 & " seconds...")
			Sleep($RETRY_TIME)

			$hWndTmp = WinGetHandle($_WinTitle, "")

			FileWriteLine($LogFile, _Now() & ":INFO:Handle Return with: " & $hWndTmp)

			Local $ctrlIDList = _GetControlIDs($hWndTmp, $_WinText)
			Local $hitID = FindControlByName($ctrlIDList, $_WinText)

			If $hitID == Null Then
				FileWriteLine($LogFile, _Now() & ":INFO: ID not found yet")
				ContinueLoop
			Else

			FileWriteLine($LogFile, _Now() & ":INFO: Found ID is: " & $hitID)


			$IsButtonVisible = ControlCommand($hWndTmp, "", $hitID, "IsVisible", "")
			$IsButtonEnabled = ControlCommand($hWndTmp, "", $hitID, "IsEnabled", "")
			FileWriteLine($LogFile, _Now() & ":INFO: Loading's done, button visible = " & $IsButtonVisible)
			FileWriteLine($LogFile, _Now() & ":INFO: Loading's done, button enable = " & $IsButtonEnabled)


			EndIf

		Else
			FileWriteLine($LogFile, _Now() & ":ERROR:Retry over 200 and exit...")
			FlushLogging()
			Return False
		EndIf
	WEnd

	Return True


EndFunc



Func ConfigServiceAccountPage()
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


Func ConfigWebServicesPage()
   ;Abandon ControlSetText method since it's not supported in SourceOne account password validation

   ;Input Domain textbox

   ControlFocus($WinTitle,"",$IdWebSerNameEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($WebServicesServerName)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Web Server Name with:" & $IdWebSerNameEdit)


   Sleep($CONFIG_DELAY_TIME)
EndFunc


Func ClickOWASelection($_WinTitle = "", $Id = "")

  Local $IsControlVisible = 0
	Local $Retry = 0

	;Wait until install is completed
	While $IsControlVisible < 1
		If $Retry < 50 Then
			FileWriteLine($LogFile, _Now() & ":INFO:Retry with: " & $Retry)
			$Retry = $Retry + 1
			$IsControlVisible = ControlCommand($_WinTitle, "", $Id, "IsVisible", "")
			FileWriteLine($LogFile, _Now() & ":INFO:Still wait for completion, wait" & $CONFIG_DELAY_TIME/1000 & " seconds...")
			Sleep($CONFIG_DELAY_TIME)
		Else
			FileWriteLine($LogFile, _Now() & ":ERROR:Retry over 50 and continue...")
			FlushLogging()
			Exit
		EndIf
	WEnd

	ControlClick($_WinTitle, "", $Id)
	FileWriteLine($LogFile,_Now() &":INFO: Control Clicked...")
	Sleep($CONFIG_DELAY_TIME)
EndFunc
