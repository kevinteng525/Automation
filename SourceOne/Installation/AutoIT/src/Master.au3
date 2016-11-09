#requireadmin
#include <common.au3>
#include <Date.au3>
#include <ScreenCapture.au3>
AutoItSetOption("TrayIconDebug",0)
AutoItSetOption("WinTitleMatchMode", 2)

Local $LogFile = FileOpen("C:\Scripts\Master_Logging.txt",1)
Local $ID = "Master_"

Local $ActivityDB = "ES1Activity"
Local $DBServer = "ES1"
Local $BuildLocation = "C:\Share\Builds\"
Local $WinTitle = "EMC SourceOne Master Services Installation"
Local $WizardTitle = "EMC SourceOne Master Services - InstallShield Wizard"
Local $WarningTitle = "EMC SourceOne Master Services"
Local $NotesConfigurationWizzard = "Client Configuration"

Local $IsNextEnabled = 0
Local $IsCancelEnabled = 0
Local $IsBackEnabled = 0

Local $IsDominoOptionAvailable = 0

Local $IsUpgrade =0 

_main()

Func _main()
   
   ;Local $IsNew = MsgBox(4,"Install or Upgrade","Click Yes to New install, Click No to Upgrade")
   
   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Arch" & @OSArch)
   
   StartUpgrade()
   
   ;Screenshot for Entry Wizzard
   _ScreenCapture_Capture($BuildLocation & $ID & "MasterEntryWizzard.jpg")
   
   ;Read Input parameters and determine if it's upgrade or new install
   
   If $CmdLine[0] = 3 Then
	  ;Specify DB Server 
	  $DBServer = $CmdLine[3]
	  FileWriteLine($LogFile,_Now() & ":INFO: Get Database Server from parameter.." & $DBServer)
	  
	  If $CmdLine[1] = "Param1" Then
		 If $CmdLine[2] = "U" Then
			$IsUpgrade = 1 
			FileWriteLine($LogFile,_Now() & ":INFO: Upgrade operation has been started..")
			;ClickUpgradeButton()
			ClickNextButton()
			
		 Else
			FileWriteLine($LogFile,_Now() & ":INFO: New install operation has been started..")
			ClickNextButton()
			;Add logic to see if work directory is located in C disk
			Dim $WorkDirectory = ControlGetText($WinTitle,"","[CLASS:Static; INSTANCE:3]")
			If StringInStr($WorkDirectory, "C:\") >0 Then
			   ClickNextButton()
			   ; Add logic of Click OK button for confirmation
			   WinActivate($WarningTitle)
			   ControlClick($WarningTitle,"","[CLASS:Button; TEXT:OK; INSTANCE:1]")
			   Sleep(15000)
			   Else
			   ClickNextButton()
			EndIf
		 EndIf
	  EndIf
   EndIf
   
   
   
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigAccount.jpg")
   ConfigAccount()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigAccount.jpg")
   
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigActivityDB.jpg")
   ConfigActivityDB()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigActivityDB.jpg")
   
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigMail.jpg")
   ConfigMail()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigMail.jpg")
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigDomino.jpg")
   ConfigDomino()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigDomino.jpg")
   
   ClickNextButton()
   
   If $IsDominoOptionAvailable = 1 Then
	  ConfigNotesCredential()
   Else
   EndIf
   
   ClickNextButton()
   ClickInstallButton()
   ClickFinishButton()
   
   If $IsDominoOptionAvailable =1 And $IsUpgrade =0 Then
	  ;Detect Notes configuration wizzard
	  If WinExists($NotesConfigurationWizzard) Then
		 WinActivate($NotesConfigurationWizzard)
		 WinWaitActive($NotesConfigurationWizzard)
		 Sleep (5000)
		 _ScreenCapture_Capture($BuildLocation & $ID & "NotesConfiguration.jpg")
		 ControlClick($NotesConfigurationWizzard,"","[CLASS:Button; TEXT:Cancel; INSTANCE:1]")
		 FileWriteLine($LogFile,_Now() & ":INFO: Successfully pop up notes configuration winzzard..")
	  Else
		 FileWriteLine($LogFile,_Now() & ":ERROR: No pop up notes configuration winzzard..")
		 Exit
	  EndIf
   EndIf
   
   _ScreenCapture_Capture($BuildLocation & $ID & "HandleRestart.jpg")
   HandleRestart()
   FileWriteLine($LogFile,_Now() &":INFO:Completed.")
   EndFunc

Func StartUpgrade()
   Run($BuildLocation & "ES1_MasterSetup.exe")
   WinActivate($WizardTitle)
   IsPreInstall()
   WinActivate($WinTitle)
   WinWaitActive($WinTitle)
   Sleep (30000)
   FileWriteLine($LogFile,"Update."&_Now() &":INFO:Successfully launch master installer and waiting for windows activation...")
   EndFunc

Func ConfigMail()
   ;Check Exchange
   ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:5]","CHECK","")
   FileWriteLine($LogFile,"Update."& _Now() &":INFO:Successfully checked Exchange mail platform...")
   ;Check Office365
   ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:6]","CHECK","")
   FileWriteLine($LogFile,"Update."&_Now() &":INFO:Successfully checked Office 365 mail platform...")
   Sleep(5000)
   EndFunc

Func ConfigDomino ()
   ;First to check the checkbox is available for check or not.
   $IsDominoOptionAvailable = ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:4]","IsEnabled","")
   FileWriteLine($LogFile,_Now() &":INFO:Validate Domino Option to see if it can be checked or not" & $IsDominoOptionAvailable)
   
   If $IsDominoOptionAvailable = 1 Then
	  ControlCommand($WinTitle,"","[CLASS:Button; INSTANCE:4]","CHECK","")
	  FileWriteLine($LogFile,"Update."&_Now() &":INFO:Successfully checked IBM Lotus Domino mail platform...")
   Else
	  FileWriteLine($LogFile,"Update."&_Now() &":INFO: No available option for IBM Lotus Domino mail platform...")
   EndIf
   
   EndFunc




