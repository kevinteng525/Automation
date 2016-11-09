#include <common.au3>
#include <Date.au3>
#include <ScreenCapture.au3>
AutoItSetOption("TrayIconDebug",0)

Local $LogFile = FileOpen("WebSearch_Logging.txt",1)
Local $ID = "websearch_"

Local $ActivityDB = "ES1Activity"
Local $SearchDB = "ES1Search"
Local $DBServer = "ES1"
Local $BuildLocation = "C:\Share\Builds\"
Local $WinTitle = "EMC SourceOne Search Installation"
Local $IISTitle = "EMC SourceOne Warning"
Local $WorkerServer = "work01"
Local $WizardTitle = "EMC SourceOne Search - InstallShield Wizard"

_main()

;WinActivate("EMC SourceOne Mobile Services Installation")
;Sleep (5000)

Func _main()
   
   ;Local $IsNew = MsgBox(4,"Install or Upgrade","Click Yes to New install, Click No to Upgrade")
 
   StartUpgrade()
   _ScreenCapture_Capture($BuildLocation & $ID & "WebSearchEntryWizzard.jpg")
   
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
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigSSL.jpg")
   ConfigSSL()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigSSL.jpg")
      
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigShortcut.jpg")
   ConfigShortCut()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigShortcut.jpg")
   
   ClickNextButton()
   ClickInstallButton()
   ClickFinishButton()
   EndFunc

Func EnableIIS()
   WinActivate($IISTitle)
   ControlClick($IISTitle,"","[CLASS:Button; TEXT:&Yes; INSTANCE:1]")
   EndFunc

Func StartUpgrade()
   
   Run($BuildLocation & "ES1_SearchSetup.exe")
   WinActivate($WizardTitle)
   IsPreInstall()
   WinActivate($WinTitle)
   WinWaitActive($WinTitle)
   Sleep (30000)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully launch web search installer and waiting for windows activation...")
   EndFunc

Func ConfigSSL()
   ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:1]","UNCHECK","")
   FileWriteLine($LogFile,_Now() &":INFO:Unchecked SSL option...")
   EndFunc

Func ConfigShortCut()
   ;Check Desktop
   ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:4]","CHECK","")
   FileWriteLine($LogFile,_Now() &":INFO:Successfully check Desktop option...")
   ;Check Quick Launch
   ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:5]","UNCHECK","")
   FileWriteLine($LogFile,_Now() &":INFO:Successfully uncheck quick launch option...")
   EndFunc






