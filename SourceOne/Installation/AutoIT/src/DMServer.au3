#include <common.au3>
#include <Date.au3>
#include <ScreenCapture.au3>
AutoItSetOption("TrayIconDebug",0)

Local $LogFile = FileOpen("DMServer_Logging.txt",1)
Local $ID = "DiscoveryManagerServer_"

Local $DiscoveryManagerDB = "DiscoveryManager"
Local $DBServer = "ES1"
Local $BuildLocation = "C:\Share\Builds\"
Local $WinTitle = "EMC SourceOne Discovery Manager Server Installation"
Local $IISTitle = "EMC SourceOne Warning"
Local $WizardTitle = "EMC SourceOne Discovery Manager Server - InstallShield Wizard"

_main()

;WinActivate("EMC SourceOne Discovery Manager Server Installation")
;Sleep (5000)

Func _main()
   
   FileWriteLine($LogFile, _Now() & ":INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile, _Now() & ":INFO: Get OS Arch" & @OSArch)
   
   StartUpgrade()
   _ScreenCapture_Capture($BuildLocation & $ID & "DiscoveryManagerServerEntryWizzard.jpg")
   
   If $CmdLine[0] = 3 Then
	  
	  ;Specify DB Server 
	  $DBServer = $CmdLine[3]
	  FileWriteLine($LogFile,_Now() & ":INFO: Get Database Server from parameter.." & $DBServer)
	  
	  If $CmdLine[1] = "Param1" Then
		 If $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Upgrade operation has been started..")
			ClickUpgradeButton()
			;ClickNextButton()
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
   ConfigDMDB()
   ClickNextButton()
   ClickInstallButton()
   ClickFinishButton()
   HandleRestart()
   EndFunc

Func StartUpgrade()
   Run($BuildLocation & "ES1_DiscoveryMgrServerSetup.exe")
   WinActivate($WizardTitle)
   IsPreInstall()
   WinActivate($WinTitle)
   WinWaitActive($WinTitle)
   Sleep (30000)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully launch discovery manager server installer and waiting for windows activation...")
   EndFunc

Func EnableIIS()
   WinActivate($IISTitle)
   ControlClick($IISTitle,"","[CLASS:Button; TEXT:&Yes; INSTANCE:1]")
   EndFunc





