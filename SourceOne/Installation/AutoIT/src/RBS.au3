#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common2.au3"
#include <Date.au3>
#include <WinAPI.au3>


AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\RBS_Logging.txt",9)

Dim $hWnd
Dim $WinTitle = "EMC SourceOne RBS for SharePoint - InstallShield Wizard"



Dim $IdRBSModifyCheckBox = 2869
Dim $IdRBSRemoveCheckBox = 624
Dim $IdRBSRepairCheckBox = 1731


; Read the INI file for the value, IniRead ( "filename", "section", "key", "default" )
; Overwrite the S1 arguments according to ini. If not found, then use default value.
Local $Domain = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Domain", "qaes1.com")
Local $ServiceAccount = IniRead(@ScriptDir & "\Config.ini", "S1Account", "ServiceAccount", "es1service")
Local $Password = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Password", "emcsiax@QA")
Local $SecurityGroup = IniRead(@ScriptDir & "\Config.ini", "S1Account", "Security", "es1 security group")
Local $WebServicesServerName = IniRead(@ScriptDir & "\Config.ini", "S1Account", "WebServicesServerName", "Work01")

Local $RBSLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")


FileWriteLine($LogFile,_Now() & ":INFO: ES1_RBSSetup location: " & $RBSLocation )

$hSearch = FileFindFirstFile($RBSLocation & "ES1_RBSSetup.exe")

; Check if the search was successful, if not display a message and return False.
    If $hSearch = -1 Then
         FileWriteLine($LogFile,_Now() & ":INFO: No ES1_RBSSetup.exe is found")
        Exit
	Else
		$RBSBundle = FileFindNextFile($hSearch)
		FileWriteLine($LogFile,_Now() & ":INFO: ES1_RBSSetup is found at: " & $RBSBundle )
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
			FileWriteLine($LogFile,_Now() & ":INFO: RBS Install operation has been started..")
			RBSInstall()
		ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ":INFO: RBS Uninstall operation has been started..")
			RBSUninstall()
		ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ":INFO: RBS Repair operation has been started..")
			RBSRepair()
		ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: RBS Upgrade operation has been started..")
			RBSUpgrade()
		ElseIf $CmdLine[2] = "M" Then
			FileWriteLine($LogFile,_Now() & ":INFO: RBS Modify operation has been started..")
			RBSModify()
		EndIf
	  EndIf
   EndIf


EndFunc

Func RunInstaller()

	FileWriteLine($LogFile, _Now() & ":INFO: Func RunInstaller")

    Run($RBSLocation & $RBSBundle)
	FileWriteLine($LogFile,_Now() &":INFO: RBS setup is runned")
	WinWaitActive($WinTitle, "", 60000)
	$hWnd = WinGetHandle($WinTitle, "")
	FileWriteLine($LogFile,_Now() &":INFO:Got RBS handler")
EndFunc



Func RBSInstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsRBSCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")


	$btnFinishID= IsRBSFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: RBS Install Done!")

EndFunc


Func RBSUpgrade()


	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsRBSCompleteLoading($WinTitle,"&Update >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Update >")

	ConfigServiceAccountPage()
	ClickButtonByName($WinTitle, "&Next >")

	ConfigWebServicesPage()
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")


	$btnFinishID= IsRBSFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ControlClick($WinTitle, "", $btnFinishID)

	FileWriteLine($LogFile, _Now() & "INFO: RBS Upgrade Done!")

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

EndFunc

Func RBSUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsRBSCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickRBSSelection($WinTitle, $IdRBSRemoveCheckBox)

	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Remove")


	$btnFinishID= IsRBSFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: RBS Remove Done!")

EndFunc

Func RBSRepair()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsRBSCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickRBSSelection($WinTitle, $IdRBSRepairCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$btnFinishID= IsRBSFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ControlClick($WinTitle, "", $btnFinishID)

	FileWriteLine($LogFile, _Now() & "INFO: RBS Repair Done!")

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

EndFunc


Func RBSModify()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsRBSCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickRBSSelection($WinTitle, $IdRBSModifyCheckBox)
	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Install")

	$btnFinishID= IsRBSFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ControlClick($WinTitle, "", $btnFinishID)

	FileWriteLine($LogFile, _Now() & "INFO: RBS Modify Done!")

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

EndFunc








Func IsRBSCompleteLoading($_WinTitle = "", $_WinText = "")
	FileWriteLine($LogFile, _Now() & ":INFO: Func IsRBSCompleteLoading")
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





Func IsRBSFinish($_WinTitle = "", $_WinText = "")

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



Func ClickRBSSelection($_WinTitle = "", $Id = "")

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
