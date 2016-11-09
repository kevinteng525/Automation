#include <common.au3>
#include <Date.au3>
#include <ScreenCapture.au3>
AutoItSetOption("TrayIconDebug",0)

Local $LogFile = FileOpen("DMClient_Logging.txt",1)
Local $ID = "DiscoveryManagerClient_"

Local $ActivityDB = "ES1Activity"
Local $SearchDB = "ES1Search"
Local $DBServer = "ES1"
Local $BuildLocation = "C:\Share\Builds\"
Local $WinTitle = "EMC SourceOne Discovery Manager Client Installation"
Local $WizardTitle = "EMC SourceOne Discovery Manager Client - InstallShield Wizard"
Local $WorkerServer = "work01"

_main()

;WinActivate("EMC SourceOne Discovery Manager Client Installation")
;Sleep (5000)

Func _main()
   
   FileWriteLine($LogFile, _Now() & ":INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile, _Now() & ":INFO: Get OS Arch" & @OSArch)
   
   StartUpgrade()
   _ScreenCapture_Capture($BuildLocation & $ID & "DiscoveryManagerClientEntryWizzard.jpg")
   
   If $CmdLine[0] = 2 Then
	  If $CmdLine[1] = "Param1" Then
		 If $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Upgrade operation has been started..")
			ClickUpgradeButton()
			;ClickNextButton()
		 Else
			FileWriteLine($LogFile,_Now() & ":INFO: New install operation has been started..")
			ClickNextButton()
		 EndIf
	  EndIf
   EndIf

   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigWorker.jpg")
   ConfigWorker()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigWorker.jpg")
   
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigShortcut.jpg")
   ConfigShortCut()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigShortcut.jpg")
   
   ClickNextButton()
   ClickInstallButton()
   ClickFinishButton()
   EndFunc

Func StartUpgrade()
   Run($BuildLocation & "ES1_DiscoveryMgrClientSetup.exe")
   WinActivate($WizardTitle)
   IsPreInstall()
   WinActivate($WinTitle)
   WinWaitActive($WinTitle)
   Sleep (30000)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully launch discovery manager client installer and waiting for windows activation...")
   EndFunc

Func ConfigShortCut()
   ;Check Desktop
   ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:1]","CHECK","")
   FileWriteLine($LogFile,_Now() &":INFO:Successfully check Desktop option...")
   ;Check Quick Launch
   ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:2]","UNCHECK","")
   FileWriteLine($LogFile,_Now() &":INFO:Successfully uncheck quick launch option...")
   EndFunc






