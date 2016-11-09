#include <Date.au3>
#include "CommonConfig.au3"
#include <array.au3>

Opt("MustDeclareVars", 0)
AutoItSetOption("MustDeclareVars",0)


Dim $LogFile

Func FlushLogging()
	FileClose($LogFile)
EndFunc   ;==>FlushLogging


Func RunInstaller($InstallerFullPath)

	FileWriteLine($LogFile, _Now() & ":INFO: Func RunInstaller")
    Run($InstallerFullPath)
	FileWriteLine($LogFile,_Now() &":INFO: " &@ScriptName &" setup is runned")

EndFunc

Func WaitAndClickButton($_WinTitle, $_Text )

	$bLoadingComplete = IsInstallerPageLoaded($_WinTitle, $_Text, $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ": Error: Button is NOT ready, something wrong with the installer, have to exit...")
		Exit
	Else
		ClickButtonByName($_WinTitle, $_Text)
	EndIf
EndFunc





Func ClickButtonByName($_WinTitle, $_Text )
	Local $hitID, $hWndTmp, $ctrlIDList,$Retry
	$hitID = Null
	$Retry = 0
	FileWriteLine($LogFile,_Now() & "INFO: ClickButtonByName is called.")
	Sleep($CLICK_DELAY_TIME)

	While ($hitID == Null)
		$Retry+=1
		If $Retry > 20 Then
			FileWriteLine($LogFile,_Now() & "Error: Cannot find button ! Exiting... ")
			Exit
		EndIf
		FileWriteLine($LogFile,_Now() & "INFO: Finding button ID times: " & $Retry)
		WinWaitActive ($_WinTitle,"",$WINDOW_WAIT_TIMEOUT)
		$hWndTmp = WinGetHandle($_WinTitle, "")
		FileWriteLine($LogFile,_Now() & "INFO: WinGetHandle is: " & $hWndTmp )
		$ctrlIDList = _GetControlIDs($hWndTmp, $_Text)
		$hitID = FindControlByName($ctrlIDList, $_Text,$_WinTitle)
	WEnd

	ClickButtonByID($hitID,$_WinTitle)

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
;~ 		FileWriteLine($LogFile,_Now() & "INFO: $avClassList is: " & $avClassList[$c] )
        While $NN < 65535

            $hCtrl = ControlGetHandle($_hWndTmp, "", $avClassList[$c] & $NN)
            If @error Then
                ExitLoop
            Else
				$idList &= _WinAPI_GetDlgCtrlID ( $hCtrl ) & @LF
;~ 				FileWriteLine($LogFile,_Now() & "INFO: $idList is: " & _WinAPI_GetDlgCtrlID ( $hCtrl ) )
				$NN += 1
            EndIf
        WEnd
    Next

;~ 	MsgBox(64, "idList", $idList)
	$idList = StringSplit($idList, @LF)

    Return $idList
EndFunc   ;==>_WinGetClassNameList




Func FindControlByName($_ctrlID , $sControlName, $sWinTitle)

	Local $c , $idFound
	$idFound = Null
	For $c = 1 To $_ctrlID[0]-1
        If Not StringCompare(ControlGetText( $sWinTitle, "", Int($_ctrlID[$c])), $sControlName , 0 ) Then
			$idFound = $_ctrlID[$c]
			FileWriteLine($LogFile,_Now() & "INFO: ID matched: " & $idFound & " as Name: " & $sControlName)
			ExitLoop
		EndIf
	Next
;~ 	FileWriteLine($LogFile,_Now() & "INFO: For Control Name: " & $sControlName & ", Hit Control ID: " & $idFound)

	If $idFound == Null Then
		FileWriteLine($LogFile,_Now() & " :INFO: For Control Name: " & $sControlName & ", Hit Control ID: is Null. Not found yet.")
		Return Null
	EndIf

	Return Int($idFound)

EndFunc

Func ClickButtonByID($_ctrlID,$sWinTitle)
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

			$IsButtonVisible = ControlCommand($sWinTitle, "", $_ctrlID, "IsVisible", "")
			$IsButtonEnabled = ControlCommand($sWinTitle, "", $_ctrlID, "IsEnabled", "")
			FileWriteLine($LogFile, _Now() & ":INFO: Loading's done, button visible = " & $IsButtonVisible)
			FileWriteLine($LogFile, _Now() & ":INFO: Loading's done, button enable = " & $IsButtonEnabled)

			$ClickResult = ControlClick($sWinTitle, "", $_ctrlID)
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
			FileWriteLine($LogFile, _Now() & " : INFO: " & $_S1RegValue & " NOT exist on this machine.")
			Return False
		Else
			FileWriteLine($LogFile, _Now() & " : INFO: " & $_S1RegValue & " exist on this machine.")
			Return True
		EndIf
	ElseIf @OSArch == "X86" Then
		FileWriteLine($LogFile, _Now() & " : INFO: OS is x86.")
		RegRead($S1_REG_VERSION_X86,$_S1RegValue)
		If @error <> 0 Then
			FileWriteLine($LogFile, _Now() & " : INFO: " & $_S1RegValue & " NOT exist on this machine.")
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



Func CheckRegKeyExist( $_RegKey , $_S1RegValue )

		FileWriteLine($LogFile, _Now() & " : INFO: Func CheckRegKeyExist is invoked.")
		FileWriteLine($LogFile, _Now() & " : INFO: Check RegValue under RegKey." & $_RegKey )
		RegRead($_RegKey,$_S1RegValue)
		If @error <> 0 Then
			FileWriteLine($LogFile, _Now() & " : INFO: " & $_S1RegValue & " NOT exist on this machine.")
			Return False
		Else
			FileWriteLine($LogFile, _Now() & " : INFO: " & $_S1RegValue & " exist on this machine.")
			Return True
		EndIf

EndFunc

;==================================================
; Function IsInstallerPageLoaded(($_WinTitle , $_WinText ,$_RetryTimes)
;   Returns Bool value on whether the $_WinTitle is loaded completed for the desired button name.
;	If the desired button is visible and enabled both, this function consider the page is loaded
;	And, we may consider the installation or loading completes.
;   $_RetryTimes is the optional parameter which 200 is default value.
;==================================================

Func IsInstallerPageLoaded($_WinTitle, $_WinText ,$_RetryTimes = 500 )
	FileWriteLine($LogFile, _Now() & ":INFO: Func IsInstallerPageLoaded")
	WinWaitActive($_WinTitle,"",$WINDOW_WAIT_TIMEOUT)
	WinActivate($_WinTitle)

	Local $hWndTmp = WinGetHandle($_WinTitle, "")

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

			If $Retry <> 1 Then
				Sleep($RETRY_TIME)
			EndIf

			WinWaitActive($_WinTitle,"",$WINDOW_WAIT_TIMEOUT)
			$hWndTmp = WinGetHandle($_WinTitle, "")

			FileWriteLine($LogFile, _Now() & " :INFO:Handle Return with: " & $hWndTmp)

			Local $ctrlIDList = _GetControlIDs($hWndTmp, $_WinText)
			Local $hitID = FindControlByName($ctrlIDList, $_WinText,$_WinTitle)

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





Func ClickPageCheckBoxByID($_WinTitle = "", $Id = "")

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


 ;Options are :
                 	 ;1- Show Search status window.
                 	 ;2- Return Path(s) only
                 	 ;4- Search Sub directory's
                 	 ;8- Cancel by first found

Func FindFile($StartDir, $FileToFind, $Options)

Dim $DirNames[200000]
Local $Files
Local $TotalDirCount
Local $CurrDirIndex
Local $DirHandle
Local $Result
Local $FileIndex
Local $ResultFiles

;Show a status screen

If BitAND($Options,1) = 1 then
	SplashTextOn("Datei wird gesucht ...", $StartDir, 600, 120, -1, -1, 22, "Arial", 10)
EndIf

;Remove the end \ If specified

If StringRight($StartDir,1) = "\" Then $StartDir = StringTrimRight($StartDir,1)

;Set Start dir in Array

$TotalDirCount = $TotalDirCount + 1
$DirNames[$TotalDirCount] = $StartDir

While $TotalDirCount > $CurrDirIndex
	$CurrDirIndex = $CurrDirIndex + 1
	$DirHandle = FileFindFirstFile($DirNames[$CurrDirIndex] & "\*.*")
	If @Error then ExitLoop

	If BitAND($Options,1) = 1 then
		ControlSetText("Datei wird gesucht ...", _
						"", _
						"Static1", _
						@CRLF & "Aktuelles Verzeichnis :" & _
						@CRLF & $DirNames[$CurrDirIndex] & _
						@CRLF & @CRLF & _
						@CRLF & "Suche nach" & @TAB & @TAB & $FileToFind & _
						@CRLF & "Gefundene Datei(en)" & @TAB & $FileIndex)
	EndIf

	While 1
		$Result = FileFindNextFile($DirHandle)
		If @error Then ExitLoop

;Skip these references

		If $Result = "." Or $Result = ".." Or $Result = "RECYCLER" Or $Result = "System Volume Information" Then
			ContinueLoop

;Add this Directory to Array

			ElseIf StringInStr(FileGetAttrib($DirNames[$CurrDirIndex] & "\" & $Result),"D") > 0 And BitAND($Options,4) = 4 Then
				$TotalDirCount = $TotalDirCount + 1
				$DirNames[$TotalDirCount] = $DirNames[$CurrDirIndex] & "\" & $Result

;Check if it is an file matches the filter

			ElseIf FileFilter($Result, $FileToFind) > 0 Then
				$FileIndex = $FileIndex + 1

				If BitAND($Options,2) = 2 Then
					$Files = $Files & $DirNames[$CurrDirIndex] & "\" & @CR
				Else
					$Files = $Files & $DirNames[$CurrDirIndex] & "\" & $Result & @CR
				EndIf
			EndIf
	Wend
	FileClose($DirHandle)
	If $FileIndex = 1 AND BitAND($Options,8) = 8 Then ExitLoop
Wend

If BitAND($Options,1) = 1 then
	SplashOff()
EndIf

If $FileIndex > 0 then
	$Files = StringTrimRight($Files,1)
	$ResultFiles = StringSplit($Files,@CR)
	Return $ResultFiles
Else
	Return 0
EndIf

EndFunc

Func FileFilter($Value, $Filter)

Select
	Case $Filter = "*" or $Filter = "*.*"      ;Finds everything.
	Return 1

	Case StringLeft($Filter, 1) = "*"          ;Finds all Files with given Extension.
	If StringRight($Filter, 4) = StringRight($Value, 4) then
		Return 1
	Else
		Return 0
	EndIf

	Case StringRight($Filter, 1) = "*"         ;Finds all file names with any extension.
	If StringTrimRight($Filter, 2) = StringTrimRight($Value, 4) then
		Return 1
	Else
		Return 0
	EndIf

	Case Else
	If $Value = $Filter then
		Return 1
	Else
		Return 0
	EndIf
EndSelect

EndFunc




