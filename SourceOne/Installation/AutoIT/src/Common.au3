#include <Date.au3>
#include "CommonConfig.au3"

Opt("MustDeclareVars", 0)
AutoItSetOption("MustDeclareVars",0)
Local $Domain = "qaes1.com"
Local $ServiceAccount = "es1service"
Local $Password = "emcsiax@QA"
Local $SecurityGroup = "es1 security group"
Local $NotesPassword = "emcsiax@QA"


Func IsPreInstall()
   
   WinActivate($WizardTitle)
   local $IsInstallVisible = 0
   local $Retry =0
   
   local $IsNextVisible =0 
   local $NextRetry =0 
   
   while $IsInstallVisible <1
	  If $Retry <5 Then
		 FileWriteLine($LogFile,_Now() &":INFO:Check Install button for retry with: " & $Retry)
		 FileWriteLine($LogFile,_Now() &":INFO:Still wait for Install button is visible, wait 5 seconds...")
		 Sleep(5000)
		 $Retry = $Retry +1
		 $IsInstallVisible = ControlCommand($WizardTitle,"","[CLASS:Button; TEXT:Install; INSTANCE:1]","IsVisible","")
		 If $IsInstallVisible=1 Then
			Local $IsInstallEnabled = ControlCommand($WizardTitle,"","[CLASS:Button; TEXT:Install; INSTANCE:1]","IsEnabled","")
			FileWriteLine($LogFile, _Now() & ":INFO: Install button status..." & $IsInstallEnabled )
   
			If $IsInstallEnabled =1 Then
			ControlClick($WizardTitle,"","[CLASS:Button; TEXT:Install; INSTANCE:1]")
			FileWriteLine($LogFile,_Now() &":INFO:Successfully click Install button...")
			Return
			Else
			FileWriteLine($LogFile, _Now() & ":INFO: Install button is not available and continue...")
			Return
			EndIf
		 EndIf
	  Else
		 FileWriteLine($LogFile,_Now() &":INFO:Retry over 5 and continue...")
		 Return
	  EndIf
   WEnd
   Sleep(5000)
   
   ;Wait until install is completed
   while $IsNextVisible <1
	  If $NextRetry <50 Then
		 FileWriteLine($LogFile,_Now() &":INFO:Retry with: " & $NextRetry)
		 FileWriteLine($LogFile,_Now() &":INFO:Still wait for completion, wait 5 seconds...")
		 Sleep(5000)
		 $NextRetry = $NextRetry +1
		 $IsNextVisible = ControlCommand($WinTitle,"","[CLASS:Button; TEXT:&Next >; INSTANCE:1]","IsVisible","")
		 Else
		 FileWriteLine($LogFile,_Now() &":ERROR:Retry over 5 and continue...")
		 FlushLogging()
		 Exit
	  EndIf
   WEnd
		 
   ;TBD How to handle upgrade for example: 6.74 issue.
   
   
   EndFunc

Func ClickFinishButton()
   
   _ScreenCapture_Capture($BuildLocation & "BeforeClickFinishButton.jpg")
   
   local $IsVisible = 0
   local $Retry =0
   
   while $IsVisible <1
	  If $Retry <50 Then
		 FileWriteLine($LogFile,_Now() &":WARNING:Retry with: " & $Retry)
		 FileWriteLine($LogFile,_Now() &":WARNING:Still wait for completion, wait 5 seconds...")
		 Sleep(10000)
		 $Retry = $Retry +1
		 $IsVisible = ControlCommand($WinTitle,"","[CLASS:Button; TEXT:&Finish; INSTANCE:1]","IsVisible","")
	  Else
		 FileWriteLine($LogFile,_Now() &":ERROR:Retry over 50 and exit process...")
		 FlushLogging()
		 Exit
		 EndIf
   WEnd
   
   Local $IsFinishEnabled = ControlCommand($WinTitle,"","[CLASS:Button; TEXT:&Finish; INSTANCE:1]","IsEnabled","")
   FileWriteLine($LogFile, _Now() & ":INFO: Finish button status..." & $IsFinishEnabled )
   
   If $IsFinishEnabled =1 Then
	  ControlClick($WinTitle,"","[CLASS:Button; TEXT:&Finish; INSTANCE:1]")
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully click Finish button...")
   Else
	  FileWriteLine($LogFile, _Now() & ":ERROR: Finish button is not available and exit...")
	  Exit
   EndIf
   
   Sleep(5000)
   EndFunc

Func ClickInstallButton()
   
   _ScreenCapture_Capture($BuildLocation & "BeforeClickInstallButton.jpg")
   Local $IsInstallEnabled = ControlCommand($WinTitle,"","[CLASS:Button; TEXT:&Install;INSTANCE:1]","IsEnabled","")
   
   If $IsInstallEnabled =1 Then
	  ControlClick($WinTitle,"","[CLASS:Button; TEXT:&Install; INSTANCE:1]")
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully click Install button...")
   Else
	  FileWriteLine($LogFile, _Now() & ":ERROR: Install button is not available and exit...")
	  Exit
   EndIf
   
   _ScreenCapture_Capture($BuildLocation & "AfterClickInstallButton.jpg")
   Sleep(5000)
   EndFunc

Func ClickNextButton()
   
   WinActivate($WinTitle)
   Local $IsNextEnabled = ControlCommand($WinTitle,"","[CLASS:Button;TEXT:&Next >;INSTANCE:1]","IsEnabled","")
   Local $IsCancelEnabled = ControlCommand($WinTitle,"","[CLASS:Button;TEXT:&Cancel;INSTANCE:1]","IsEnabled","")
   Local $IsBackEnabled = ControlCommand($WinTitle,"","[CLASS:Button;TEXT:< &Back;INSTANCE:1]","IsEnabled","")
	  
   FileWriteLine($LogFile, _Now() & ":INFO: Next button status..." & $IsNextEnabled )
   FileWriteLine($LogFile, _Now() & ":INFO: Cancel button status..." & $IsCancelEnabled )
   FileWriteLine($LogFile, _Now() & ":INFO: Back button status..." & $IsBackEnabled )
   
   If $IsNextEnabled =1 Then
	  ControlClick($WinTitle,"","[CLASS:Button; TEXT:&Next >; INSTANCE:1]")
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully click Next button...")
   Else
	  FileWriteLine($LogFile, _Now() & ":ERROR: Next button is not available and exit...")
	  Exit
   EndIf
   
   Sleep(10000)
   EndFunc

Func ClickUpgradeButton()
   WinActivate($WinTitle)
   
   _ScreenCapture_Capture($BuildLocation & "BeforeClickUpgradeButton.jpg")
   Local $IsUpgradeEnabled = ControlCommand($WinTitle,"","[CLASS:Button; TEXT:&Update >; INSTANCE:1]","IsEnabled","")
   FileWriteLine($LogFile, _Now() & ":INFO: Upgrade button status..." & $IsUpgradeEnabled )
   
   If $IsUpgradeEnabled =1 Then
	  ControlClick($WinTitle,"","[CLASS:Button; TEXT:&Update >; INSTANCE:1]")
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully click Upgrade button...")
   Else
	  FileWriteLine($LogFile, _Now() & ":ERROR: Upgrade button is not available and exit...")
	  Exit
   EndIf
   
   Sleep (5000)
   EndFunc


Func ClickUpgradeButton_OA()
   WinActivate($WinTitle)
   
   _ScreenCapture_Capture($BuildLocation & "BeforeClickUpgradeButton.jpg")
   Local $IsUpgradeEnabled = ControlCommand($WinTitle,"","[CLASS:Button; TEXT:&Upgrade >; INSTANCE:1]","IsEnabled","")
   FileWriteLine($LogFile, _Now() & ":INFO: Upgrade button status..." & $IsUpgradeEnabled )
   
   If $IsUpgradeEnabled =1 Then
	  ControlClick($WinTitle,"","[CLASS:Button; TEXT:&Upgrade >; INSTANCE:1]")
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully click Upgrade button...")
   Else
	  FileWriteLine($LogFile, _Now() & ":ERROR: Upgrade button is not available and exit...")
	  Exit
   EndIf
   
   Sleep (5000)
   EndFunc



Func ConfigAccount()
   ;Abandon ControlSetText method since it's not supported in SourceOne account password validation

   ;Input Domain textbox
   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]")
   Send($Domain)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure domain with:" & $Domain)
   
   ;Input Service Account
   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:2]")
   Send($ServiceAccount)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure service account with:" & $ServiceAccount)
   
   If @OSVersion = "WIN_2003" Then
   
	  ;Input Service Password
	  ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:3]")
	  Send($Password)
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully configure service account password with:" & $Password)
	  
   Else
	  ;Handle WIN_2008 special case
	  ControlFocus($WinTitle,"","[CLASS:Edit; INSTANCE:1]")
	  Send($Password)
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully configure service account password with:" & $Password)
   EndIf
   
   If @OSVersion = "WIN_2003" Then
	  
	  ;Input Security Group
	  ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:4]")
	  Send($SecurityGroup)
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully configure security group with:" & $SecurityGroup)
   Else
   ;Handle WIN_2008 special case
	  ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:3]")
	  Send($SecurityGroup)
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully configure security group with:" & $SecurityGroup)
   EndIf

   Sleep(5000)
   EndFunc

Func HandleRestart()
   WinActivate($WinTitle)
   
   _ScreenCapture_Capture($BuildLocation & "HandleRestart.jpg")
   Local $IsNoEnabled = ControlCommand($WinTitle,"","[CLASS:Button; TEXT:&No; INSTANCE:1]","IsEnabled","")
   FileWriteLine($LogFile, _Now() & ":INFO: No button status..." & $IsNoEnabled )
   If $IsNoEnabled =1 Then
	  ControlClick($WinTitle,"","[CLASS:Button; TEXT:&No; INSTANCE:1]")
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully click No button to hold off restart...")
   Else
	  FileWriteLine($LogFile, _Now() & ":ERROR: No button is not available and exit...")
	  Exit
   EndIf
EndFunc

Func ConfigActivityDB()
   
   _ScreenCapture_Capture($BuildLocation & "BeforeConfigActivityDB.jpg")
   
   ;Input Activity DB textbox
   
   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]")
   Send($ActivityDB)
   
   ;ControlSetText($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]",$ActivityDB)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Activity Database with:" & $ActivityDB)
   ;Input Activity DB Server
   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:2]")
   Send($DBServer)
   
   ;ControlSetText($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:2]", $DBServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Activity Database Server with:" & $DBServer)
   _ScreenCapture_Capture($BuildLocation & "AfterConfigActivityDB.jpg")
   
   Sleep(5000)
   EndFunc


Func ConfigSearchDB()
   
   _ScreenCapture_Capture($BuildLocation & "BeforeConfigSearchDB.jpg")
   
   ;Input Search DB textbox
   
   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:3]")
   Send($SearchDB)
   
   ;ControlSetText($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]",$ActivityDB)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Search Database with:" & $SearchDB)
   ;Input Activity DB Server
   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:4]")
   Send($DBServer)
   
   ;ControlSetText($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:2]", $DBServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Activity Database Server with:" & $DBServer)
   _ScreenCapture_Capture($BuildLocation & "AfterConfigSearchDB.jpg")
   Sleep(5000)
   EndFunc



Func ConfigArchiveDB()
   
   _ScreenCapture_Capture($BuildLocation & "BeforeConfigArchiveDB.jpg")
   ;Input Activity DB textbox
   ControlSetText($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]",$ArchiveDB)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Archive Database with:" & $ArchiveDB)
   ;Input Activity DB Server
   ControlSetText($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:2]", $DBServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Archive Database Server with:" & $DBServer)
   _ScreenCapture_Capture($BuildLocation & "AfterConfigArchiveDB.jpg")
   Sleep(5000)
   EndFunc

Func ConfigDMDB()
   _ScreenCapture_Capture($BuildLocation & "BeforeConfigDMDB.jpg")
   ;Input Discovery Manager DB textbox
   ControlSetText($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]",$DBServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Archive Database Server with:" & $DBServer)
   ;Input DM DB Server
   ControlSetText($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:2]", $DiscoveryManagerDB)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure DiscoveryManager Database with:" & $DiscoveryManagerDB)
   _ScreenCapture_Capture($BuildLocation & "AfterConfigDMDB.jpg")
   Sleep(5000)
   EndFunc

Func ConfigWorker()
   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]")
   Send($WorkerServer)
   Sleep(10000)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure worker server with: " & $WorkerServer)
   EndFunc

Func ConfigNotesCredential()
   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]")
   Send($NotesPassword)
   Sleep(10000)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure notes password with: " & $NotesPassword)
   EndFunc

Func FlushLogging()
   FileClose($LogFile)
   EndFunc
   
Func ClickButtonByName($_WinTitle, $_Text )
	Sleep($CLICK_DELAY_TIME)
	WinWaitActive ($_WinTitle,"",$WINDOW_WAIT_TIMEOUT)
	Local $hWndTmp = WinGetHandle($_WinTitle, "")

	FileWriteLine($LogFile,_Now() & "INFO: WinGetHandle is: " & $hWndTmp )
	Local $ctrlIDList = _GetControlIDs($hWndTmp, $_Text)
	Local $hitID = FindControlByName($ctrlIDList, $_Text)
	ClickButtonByID($hitID)

EndFunc


;==================================================
; Function _GetControlIDs()
;   Returns a string containing a list of all control IDs in a window.
;
;   On failure returns 0 and sets @error.
;==================================================
Func _GetControlIDs($_hWndTmp, $sWinText)

    Local $c, $NN, $hCtrl
	Local $idList
    ; Get list of classes in the window
    Local $sClassList = WinGetClassList($_hWndTmp)
		If @error Then Return SetError(1, 0, 0)
    Local $avClassList = StringSplit($sClassList, @LF)

    For $c = 1 To $avClassList[0]-1
		$NN = 1
		If Not StringCompare($avClassList[$c], "Static",0) Then
;~ 			FileWriteLine($LogFile,_Now() & "INFO: Skipped Static control" )
			ContinueLoop
		EndIf
		FileWriteLine($LogFile,_Now() & "INFO: $avClassList is: " & $avClassList[$c] )
        While $NN < 65535

            $hCtrl = ControlGetHandle($_hWndTmp, "", $avClassList[$c] & $NN)
            If @error Then
                ExitLoop
            Else
				$idList &= _WinAPI_GetDlgCtrlID ( $hCtrl ) & @LF
				FileWriteLine($LogFile,_Now() & "INFO: $idList is: " & _WinAPI_GetDlgCtrlID ( $hCtrl ) )
				$NN += 1
            EndIf
        WEnd
    Next

;~ 	MsgBox(64, "idList", $idList)
	$idList = StringSplit($idList, @LF)

    Return $idList
EndFunc   ;==>_WinGetClassNameList




Func FindControlByName($_ctrlID , $sControlName)

	Local $c , $idFound
	$idFound = Null
	For $c = 1 To $_ctrlID[0]-1
        If Not StringCompare(ControlGetText( $WinTitle, "", Int($_ctrlID[$c])), $sControlName , 0 ) Then
			$idFound = $_ctrlID[$c]
			FileWriteLine($LogFile,_Now() & "INFO: ID matched: " & $idFound & " as Name: " & $sControlName)
			ExitLoop
		EndIf
	Next
	FileWriteLine($LogFile,_Now() & "INFO: For Control Name: " & $sControlName & ", Hit Control ID: " & $idFound)

	If $idFound == Null Then
		FileWriteLine($LogFile,_Now() & "INFO: For Control Name: " & $sControlName & ", Hit Control ID: is Null")
		Return Null
	EndIf

	Return Int($idFound)

EndFunc

Func ClickButtonByID($_ctrlID)
;~ 	Sleep($CLICK_DELAY_TIME)
	Local $ClickResult=0
	Local $Retry=0

	Local $IsButtonVisible = 0
	Local $IsButtonEnabled = 0



	While $IsButtonVisible < 1 Or $IsButtonEnabled < 1 Or $ClickResult < 1

		If $Retry < 20 Then
			$Retry +=1
			FileWriteLine($LogFile,_Now() & "INFO: Click Retry = " & $Retry)
			Sleep($CLICK_DELAY_TIME)

			$IsButtonVisible = ControlCommand($WinTitle, "", $_ctrlID, "IsVisible", "")
			$IsButtonEnabled = ControlCommand($WinTitle, "", $_ctrlID, "IsEnabled", "")
			FileWriteLine($LogFile, _Now() & ":INFO: Loading's done, button visible = " & $IsButtonVisible)
			FileWriteLine($LogFile, _Now() & ":INFO: Loading's done, button enable = " & $IsButtonEnabled)

			$ClickResult = ControlClick($WinTitle, "", $_ctrlID)
			FileWriteLine($LogFile,_Now() & "INFO: Clicked Result= " & $ClickResult)


		Else
			FileWriteLine($LogFile, _Now() & " :ERROR:Retry over 20 but failed, exiting...")
			FlushLogging()
			Exit

		EndIf
	WEnd

	Return True
EndFunc


;==================================================
; Function CheckS1RegKeyExist(String S1 Registry value)
;   Returns Bool value on whether the Registry value exists in SourceOne Registry.
;	$S1_REG_VERSION_X64 and $S1_REG_VERSION_X86 are pre-defined in CommonConfig
;
;==================================================

Func CheckS1RegKeyExist($_S1RegValue )

	If @OSArch == "X64" Then
		FileWriteLine($LogFile, _Now() & " : INFO: OS is x64.")
		RegRead($S1_REG_VERSION_X64,$_S1RegValue)
		If @error <> 0 Then
			FileWriteLine($LogFile, _Now() & " : INFO: " & $_S1RegValue & "NOT exist on this machine.")
			Return False
		Else
			FileWriteLine($LogFile, _Now() & " : INFO: " & $_S1RegValue & " exist on this machine.")
			Return True
		EndIf
	ElseIf @OSArch == "X86" Then
		FileWriteLine($LogFile, _Now() & " : INFO: OS is x86.")
		RegRead($S1_REG_VERSION_X86,$_S1RegValue)
		If @error <> 0 Then
			FileWriteLine($LogFile, _Now() & " : INFO: " & $_S1RegValue & "NOT exist on this machine.")
			Return False
		Else
			FileWriteLine($LogFile, _Now() & " : INFO: " & $_S1RegValue & " exist on this machine.")
			Return True
		EndIf
	Else
		FileWriteLine($LogFile, _Now() & " : ERROR: OS is not x86 or x64, not support, exiting...")
		Exit
	EndIf

EndFunc

;==================================================
; Function IsInstallerPageLoaded(($_WinTitle , $_WinText ,$_RetryTimes)
;   Returns Bool value on whether the $_WinTitle is loaded completed for the desired button name.
;	If the desired button is visible and enabled both, this function consider the page is loaded
;	And, we may consider the installation or loading completes.
;   $_RetryTimes is the optional parameter which 200 is default value.
;==================================================

Func IsInstallerPageLoaded($_WinTitle, $_WinText ,$_RetryTimes = 200 )
	FileWriteLine($LogFile, _Now() & ":INFO: Func IsFileBCECompleteLoading")
	Local $hWndTmp = WinGetHandle($_WinTitle, "")
	FileWriteLine($LogFile, _Now() & ":INFO: !!! ORI Handle Return with: " & $hWndTmp)
	WinActivate($_WinTitle)
	FileWriteLine($LogFile,_Now() & "INFO: WinGetHandle is: " & $hWndTmp )

	Local $IsButtonVisible = 0
	Local $IsButtonEnabled = 0
	Local $Retry = 0

	;Wait until installer page is loaded, button is both visible and enabled
	While $IsButtonVisible < 1 Or $IsButtonEnabled < 1
		If $Retry < $_RetryTimes Then

			$Retry += 1
			FileWriteLine($LogFile, _Now() & " :INFO:Retry with: " & $Retry)
			FileWriteLine($LogFile, _Now() & " :INFO:Still waiting, retry after " & $RETRY_TIME/1000 & " seconds...")
			Sleep($RETRY_TIME)

			$hWndTmp = WinGetHandle($_WinTitle, "")

			FileWriteLine($LogFile, _Now() & " :INFO:Handle Return with: " & $hWndTmp)

			Local $ctrlIDList = _GetControlIDs($hWndTmp, $_WinText)
			Local $hitID = FindControlByName($ctrlIDList, $_WinText)

			If $hitID == Null Then
				FileWriteLine($LogFile, _Now() & " :INFO: ID not found yet")
				ContinueLoop
			Else

			FileWriteLine($LogFile, _Now() & " :INFO: Found ID is: " & $hitID)

			$IsButtonVisible = ControlCommand($hWndTmp, "", $hitID, "IsVisible", "")
			$IsButtonEnabled = ControlCommand($hWndTmp, "", $hitID, "IsEnabled", "")
			FileWriteLine($LogFile, _Now() & " :INFO: Loading's done, button visible = " & $IsButtonVisible)
			FileWriteLine($LogFile, _Now() & " :INFO: Loading's done, button enable = " & $IsButtonEnabled)

			EndIf
		Else
			FileWriteLine($LogFile, _Now() & " :ERROR: Retry over " & $_RetryTimes & "times but failed, return False")
			Return False
		EndIf
	WEnd
	FileWriteLine($LogFile, _Now() & " :INFO: The Installer Page successfully loaded after retry " & $Retry & "times, return True")
	Return True
EndFunc






