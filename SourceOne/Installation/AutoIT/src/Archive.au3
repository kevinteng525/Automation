#include <common.au3>
#include <Date.au3>
#include <ScreenCapture.au3>
AutoItSetOption("TrayIconDebug",0)

Local $LogFile = FileOpen("C:\Scripts\NativeArchive_Logging.txt",1)
Local $ID = "nativearchive_"


Local $ActivityDB = "ES1Activity"
Local $ArchiveDB = "ES1Archive"
Local $DBServer = "ES1"

Local $BuildLocation = "C:\Share\Builds\"
Local $WinTitle = "EMC SourceOne Native Archive Services Installation"
Local $WizardTitle = "EMC SourceOne Native Archive Services - InstallShield Wizard"
Local $WarningTitle = "EMC SourceOne Native Archive Services"

_main()

;WinActivate("EMC SourceOne Native Archive Services Installation")
;Sleep (5000)

Func _main()
   
   ;Local $IsNew = MsgBox(4,"Install or Upgrade","Click Yes to New install, Click No to Upgrade")
   
   StartUpgrade()
   _ScreenCapture_Capture($BuildLocation & $ID & "NativeArchiveEntryWizzard.jpg")
   
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
			
			;Add more logic to handle if only C disk is installed. This should be additional testing for installer.
			;If WinExists($WarningTitle) =1  Then
			;   FileWriteLine($LogFile,_Now() & ":INFO: C Disk Warning Detected")
			;   WinActivate($WarningTitle)
			;   Sleep(5000)
			;  Local $IsNoEnabled = ControlCommand($WarningTitle,"","[CLASS:Button; TEXT:&No; INSTANCE:2]","IsEnabled","")
			;   If $IsNoEnabled =1 Then
			;	  ControlClick($WarningTitle,"","[CLASS:Button; TEXT:&No; INSTANCE:2]")
			;	  FileWriteLine($LogFile,_Now() &":INFO:Successfully click No button...")
			;   EndIf
			;   Sleep(5000)
			;EndIf
			
			ClickNextButton()
			
			;Add more logic to handle if only C disk is installed. This should be additional testing for installer.
			;If WinExists($WarningTitle) Then
			;   WinActivate($WarningTitle)
			;   Sleep(5000)
			;   ControlClick($WarningTitle,"","[CLASS:Button; TEXT:OK; INSTANCE:1]")
			;   Sleep(5000)
			;EndIf
			
			ClickNextButton()
			
			;Add more logic to handle if only C disk is installed. This should be additional testing for installer.
			;If WinExists($WarningTitle) Then
			;   WinActivate($WarningTitle)
			;   ControlClick($WarningTitle,"","[CLASS:Button; TEXT:OK; INSTANCE:1]")
			;   Sleep(15000)
			;EndIf
			
		 EndIf
	  EndIf
   EndIf
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigAccount.jpg")
   ConfigAccount()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigAccount.jpg")
   
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigArchiveDB.jpg")
   ConfigArchiveDB()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigArchiveDB.jpg")
   
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigIPM.jpg")
   ConfigIPM()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigIPM.jpg")
   
   ClickNextButton()
   ClickInstallButton()
   ClickFinishButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "HandleRestart.jpg")
   HandleRestart()
   EndFunc

Func StartUpgrade()
   Run($BuildLocation & "ES1_ArchiveSetup.exe")
   WinActivate($WizardTitle)
   IsPreInstall()
   WinActivate($WinTitle)
   WinWaitActive($WinTitle)
   Sleep (30000)
   FileWriteLine($LogFile,"Update."&_Now() &":INFO:Successfully launch native archive installer and waiting for windows activation...")
   EndFunc
   

Func ConfigIPM()
   ;Check IPM Archive
   ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:1]","UNCHECK","")
   EndFunc




