#requireadmin
#include <common.au3>
#include <Date.au3>
#include <ScreenCapture.au3>
AutoItSetOption("TrayIconDebug",0)

Local $LogFile = FileOpen("C:\Scripts\Console_Logging.txt",1)
Local $ID = "console_"

Local $ActivityDB = "ES1Activity"
Local $DBServer = "ES1"
Local $Domain = "qaes1.com"
Local $AdminGroup = "es1 admins group"
Local $BuildLocation = "C:\Share\Builds\"
Local $WinTitle = "EMC SourceOne Console Installation"
Local $WizardTitle = "EMC SourceOne Console - InstallShield Wizard"


_main()

;WinActivate("EMC SourceOne Console Installation")
;Sleep (5000)

Func _main()
   
   FileWriteLine($LogFile, _Now() & ":INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile, _Now() & ":INFO: Get OS Arch" & @OSArch)
   
   StartUpgrade()
   _ScreenCapture_Capture($BuildLocation & $ID & "ConsoleEntryWizzard.jpg")
   
   If $CmdLine[0] = 3 Then
	  
	  ;Specify DB Server 
	  $DBServer = $CmdLine[3]
	  FileWriteLine($LogFile,_Now() & ":INFO: Get Database Server from parameter.." & $DBServer)
	  
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

   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigAdminGroup.jpg")
   ConfigAdminGroup()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigAdminGroup.jpg")
   
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigActivityDB.jpg")
   ConfigActivityDB()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigActivityDB.jpg")
   
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigShortcut.jpg")
   ConfigShortCut()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigShortcut.jpg")
   
   ClickNextButton()
   ClickInstallButton()
   ClickFinishButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "HandleRestart.jpg")
   HandleRestart()
   FileWriteLine($LogFile,_Now() &":INFO:Completed.")
   EndFunc

Func StartUpgrade()
   Run($BuildLocation & "ES1_ConsoleSetup.exe")
   WinActivate($WizardTitle)
   IsPreInstall()
   WinActivate($WinTitle)
   WinWaitActive($WinTitle)
   Sleep (30000)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully launch console installer and waiting for windows activation...")
   EndFunc

Func ConfigAdminGroup()
   ;Abandon ControlSetText method since it's not supported in SourceOne account password validation
   
	  ;Input Domain textbox
	  ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]")
	  Send($Domain)
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully configure domain with:" & $Domain)
	  
	  ;Input Admin Group
	  ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:2]")
	  Send($AdminGroup)
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully configure admins group with:" & $AdminGroup)
   
   
   Sleep(5000)
   
   EndFunc

Func ConfigShortCut()
   ;Check Desktop
   ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:4]","CHECK","")
   FileWriteLine($LogFile,_Now() &":INFO:Successfully checked Desktop shortcut...")
   ;Check Quick Launch
   
   If @OSVersion = "WIN_2003" Then
	  ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:5]","UNCHECK","")
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully unchecked Quick Launch shortcut...")
   EndIf
   
   Sleep(5000)
   
   EndFunc




