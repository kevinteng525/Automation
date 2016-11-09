#include <common.au3>
#include <Date.au3>
#include <ScreenCapture.au3>

AutoItSetOption("TrayIconDebug",0)

Local $LogFile = FileOpen("Mobile_Logging.txt",1)
Local $ID = "mobile_"

Local $ActivityDB = "ES1Activity"
Local $SearchDB = "ES1Search"
Local $DBServer = "ES1"
Local $BuildLocation = "C:\Share\Builds\"
Local $WinTitle = "EMC SourceOne Mobile Services Installation"
Local $WizardTitle = "EMC SourceOne Mobile Services - InstallShield Wizard"
Local $IISTitle = "EMC SourceOne Warning"
Local $WorkerServer = "work01"

_main()

;WinActivate("EMC SourceOne Mobile Services Installation")
;Sleep (5000)

Func _main()
   
   ;Local $IsNew = MsgBox(4,"Install or Upgrade","Click Yes to New install, Click No to Upgrade")
   
   StartUpgrade()
   _ScreenCapture_Capture($BuildLocation & $ID & "MobileEntryWizzard.jpg")
   
   If $CmdLine[0] = 2 Then
	  If $CmdLine[1] = "Param1" Then
		 If $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Upgrade operation has been started..")
			;ClickUpgradeButton()
			ClickNextButton()
			
		 Else
			FileWriteLine($LogFile,_Now() & ":INFO: New install operation has been started..")
			ClickNextButton()
			ClickNextButton()
		 EndIf
	  EndIf
   EndIf
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigAccount.jpg")
   ConfigAccount()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigAccount.jpg")
   
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigWorker.jpg")
   ConfigWorker()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigWorker.jpg")
   
   ClickNextButton()
   ClickInstallButton()
   ClickFinishButton()
   EndFunc

Func EnableIIS()
   WinActivate($IISTitle)
   ControlClick($IISTitle,"","[CLASS:Button; TEXT:&Yes; INSTANCE:1]")
   EndFunc

Func StartUpgrade()
   Run($BuildLocation & "ES1_MobileSetup.exe")
   WinActivate($WizardTitle)
   IsPreInstall()
   WinActivate($WinTitle)
   WinWaitActive($WinTitle)
   Sleep (30000)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully launch mobile installer and waiting for windows activation...")
   EndFunc







