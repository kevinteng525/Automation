#requireadmin
#include "Common2.au3"
#include <Date.au3>
#include <WinAPI.au3>

AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

$LogFile = FileOpen("C:\Hotfix\Hotfix_Logging.txt",9)

Dim $hWnd


;$HotfixLocation = "C:\Hotfix\Mainline_7.1.2.2234_HF\"
$HotfixLocation = @ScriptDir & "\"
FileWriteLine($LogFile,_Now() & ":INFO: Hotfix location: " & $HotfixLocation )
;$HotfixBundle = "7.1.2.2234_ES1_Hotfix.exe"
$hSearch = FileFindFirstFile("*ES1_Hotfix.exe")

; Check if the search was successful, if not display a message and return False.
    If $hSearch = -1 Then
         FileWriteLine($LogFile,_Now() & ":INFO: No hotfix bundle")
        Exit
	Else
		$HotfixBundle = FileFindNextFile($hSearch)
		FileWriteLine($LogFile,_Now() & ":INFO: Hotfix bundle found: " & $HotfixBundle )
    EndIf


_main()
	ConsoleWrite('@@ (87) :(' & @MIN & ':' & @SEC & ') _main()' & @CR) ;### Function Trace



Func _main()
	ConsoleWrite('@@ (96) :(' & @MIN & ':' & @SEC & ') _main()' & @CR) ;### Function Trace

   ;Local $IsNew = MsgBox(4,"Install or Upgrade","Click Yes to New install, Click No to Upgrade")

   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Arch" & @OSArch)
	If IsAdmin() Then
		FileWriteLine($LogFile,_Now() & "INFO: IsAdmin: Admin rights are detected.")
	EndIf

    ;------Like Test case Here---

	HotfixInstall()   ; Case one : intall
	Sleep($TESTCASE_INTERVAL)

    HotfixRollback()  ; Case two : rollback


EndFunc

Func StartUpgrade()
	ConsoleWrite('@@ (194) :(' & @MIN & ':' & @SEC & ') StartUpgrade()' & @CR) ;### Function Trace
   Run($HotfixLocation & $HotfixBundle)
   ;WinActivate($WizardTitle)

	FileWriteLine($LogFile,_Now() &":INFO:Runned")

	Sleep(2000)

	WinWaitActive($WinTitle, "", 60000)
	$hWnd = WinGetHandle($WinTitle, "")
	FileWriteLine($LogFile,_Now() &":INFO:Got hotfix handler")

    Sleep (1000)
    FileWriteLine($LogFile,_Now() &":INFO:Successfully launch Hotfix installer and waiting for windows activation...")

	;ControlClick($WinTitle, "", "[CLASS:Button; INSTANCE:1]") ; Click first next

EndFunc



Func HotfixInstall()
	Local $btnFinishID
	StartUpgrade()

	; Hotfix simple Workflow
	ClickButtonByName($WinTitle, "&Next >")
	ClickButtonByName($WinTitle, "< &Back")
	ClickButtonByName($WinTitle, "&Next >")
	ClickButtonByName($WinTitle, "< &Back")
	ClickButtonByName($WinTitle, "&Next >")
	ClickButtonByName($WinTitle, "&Next >")
	ClickButtonByName($WinTitle, "&Next >")


	$btnFinishID= IsHotfixFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ControlClick($WinTitle, "", $btnFinishID)

	FileWriteLine($LogFile, _Now() & "INFO: Done!")

EndFunc



Func HotfixRollback()
	ConsoleWrite('@@ (136) :(' & @MIN & ':' & @SEC & ') HotfixRollback()' & @CR) ;### Function Trace
	Local $btnFinishID
	StartUpgrade()

	ClickButtonByName($WinTitle, "&Next >")
	ClickButtonByName($WinTitle, "< &Back")
	ClickButtonByName($WinTitle, "&Next >")
	ClickButtonByName($WinTitle, "< &Back")
	ClickButtonByName($WinTitle, "&Next >")

    ClickRollBackButton()

	ClickButtonByName($WinTitle, "&Next")   ; This is a bug
	ClickButtonByName($WinTitle, "&Next >")

	$btnFinishID= IsHotfixFinish($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

	ControlClick($WinTitle, "", $btnFinishID)

	FileWriteLine($LogFile, _Now() & "INFO: Done!")

EndFunc



Func ClickRollBackButton()
	ConsoleWrite('@@ (156) :(' & @MIN & ':' & @SEC & ') ClickRollBackButton()' & @CR) ;### Function Trace

	Local $IsRollbackVisible = 0
	Local $RollbackRetry = 0

	;Wait until install is completed
	While $IsRollbackVisible < 1
		If $RollbackRetry < 50 Then
			FileWriteLine($LogFile, _Now() & ":INFO:Retry with: " & $RollbackRetry)
			FileWriteLine($LogFile, _Now() & ":INFO:Still wait for completion, wait 3 seconds...")
			Sleep(3000)
			$RollbackRetry = $RollbackRetry + 1
			$IsRollbackVisible = ControlCommand($WinTitle, "", "[ID:1007]", "IsVisible", "")
		Else
			FileWriteLine($LogFile, _Now() & ":ERROR:Retry over 50 and continue...")
			FlushLogging()
			Exit
		EndIf
	WEnd
	;Sleep(4000)
	;FileWriteLine($LogFile,_Now() &":INFO:Rollback button visible...")
	;ControlClick($WinTitle, "", "[CLASS:Button; TEXT:RollBack; INSTANCE:5]")

	ControlClick($WinTitle, "", "[ID:1007]")

	FileWriteLine($LogFile,_Now() &":INFO:Rollback button Clicked...")

EndFunc   ;==>ClickFinishButton




Func IsHotfixFinish($_WinTitle = "", $_WinText = "")

	Local $hWndTmp = WinGetHandle($_WinTitle, "")
	WinActivate($_WinTitle)
	ConsoleWrite('@@ (216) :(' & @MIN & ':' & @SEC & ') IsHotfixFinish()' & @CR) ;### Function Trace
	FileWriteLine($LogFile,_Now() & "INFO: WinGetHandle is: " & $hWndTmp )
	WinActivate($WizardTitle)


	Local $IsFinishVisible = 0
	Local $FinishRetry = 0

	;Wait until install is completed
	While $IsFinishVisible < 1
		If $FinishRetry < 200 Then

			$FinishRetry += 1
			FileWriteLine($LogFile, _Now() & ":INFO:Retry with: " & $FinishRetry)
			FileWriteLine($LogFile, _Now() & ":INFO:Still wait for completion, wait 5 seconds...")
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

			$IsFinishVisible = ControlCommand($hWndTmp, "", $hitID, "IsVisible", "")
			FileWriteLine($LogFile, _Now() & ":INFO: visible = " & $IsFinishVisible)

			EndIf

		Else
			FileWriteLine($LogFile, _Now() & ":ERROR:Retry over 50 and continue...")
			FlushLogging()
			Exit
		EndIf
	WEnd


	FileWriteLine($LogFile,_Now() &":INFO:Hotfix completed, Finish button visible...")

	Return $hitID


EndFunc   ;==>IsPreInstall




