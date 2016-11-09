#requireadmin
#include <common.au3>
#include <Date.au3>
#include <ScreenCapture.au3>
AutoItSetOption("TrayIconDebug",0)
AutoItSetOption("WinTitleMatchMode", 2)

Local $LogFile = FileOpen("C:\Scripts\Worker_Logging.txt",1)
Local $ID = "worker_"

Local $ActivityDB = "ES1Activity"
Local $SearchDB = "ES1Search"
Local $DBServer = "ES1"
Local $BuildLocation = "C:\Share\Builds\"
Local $WinTitle = "EMC SourceOne Worker Services Installation"
Local $WizardTitle = "EMC SourceOne Worker Services - InstallShield Wizard"
Local $WarningTitle = "EMC SourceOne Worker Services"
Local $NotesConfigurationWizzard = "Client Configuration"
Local $Joblog = "\\work01\joblog"
Local $IsNotes = 0
Local $IsUpgrade =0 

_main()

;WinActivate("EMC SourceOne Worker Services Installation")
;Sleep (5000)

Func _main()
   
   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Arch" & @OSArch)
   
   StartUpgrade()
   ;Screenshot for worker entry page wizzard
   _ScreenCapture_Capture($BuildLocation & $ID & "WorkerEntryWizzard.jpg")
   
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
			
			_ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigFolder.jpg")
			ConfigFolder()
			_ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigFolder.jpg")
			
		 EndIf
	  EndIf
   EndIf
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeWorkerLog.jpg")
   ConfigWorkerLog()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterWorkerLog.jpg")
   
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigAccount.jpg")
   ConfigAccount()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigAccount.jpg")
   
   ClickNextButton()
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigActivityDBSearchDB.jpg")
   ConfigActivityDB()
   ConfigSearchDB()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigActivityDBSearchDB.jpg")
      
   ClickNextButton()
   
   ;Add logic to handle Notes
   ;Look for notes account
   
   Local $text = ControlGetText($WinTitle,"","[CLASS:Static; INSTANCE:2]") 
   
   If $text == "Please provide your IBM Lotus Notes credentials below." Then
	  $IsNotes = 1
	  FileWriteLine($LogFile, _Now() & ":INFO: Look for notes account is visible.." & $IsNotes)
   Else
   EndIf
   
   If $IsNotes = 1 Then
	  ConfigNotesCredential()
	  ClickNextButton()
   Else
   EndIf
   
   ClickInstallButton()
   ClickFinishButton()
   
   If $IsUpgrade =0 Then
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
   Run($BuildLocation & "ES1_WorkerSetup.exe")
   WinActivate($WizardTitle)
   IsPreInstall()
   WinWaitActive($WinTitle)
   Sleep (30000)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully launch worker installer and waiting for windows activation...")
   EndFunc


Func ConfigFolder()
   
   Dim $WorkDirectory = ControlGetText($WinTitle,"","[CLASS:Static; INSTANCE:3]")
			If StringInStr($WorkDirectory, "C:\") >0 Then
			   ClickNextButton()
			   ; Add logic of Click OK button for confirmation
			   
			   _ScreenCapture_Capture($BuildLocation & $ID & "CDiskConfirmation.jpg")
			   WinActivate($WarningTitle)
			   ControlClick($WarningTitle,"","[CLASS:Button; TEXT:OK; INSTANCE:1]")
			   Sleep(15000)
			EndIf
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configurate worker folder...")
   EndFunc

Func ConfigWorkerLog()
   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]")
   Send($Joblog)
   Sleep(5000)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure worker log location with:" & $Joblog)
   EndFunc






