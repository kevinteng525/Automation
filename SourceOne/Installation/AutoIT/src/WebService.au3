#requireadmin
#include <common.au3>
#include <Date.au3>
#include <ScreenCapture.au3>
AutoItSetOption("TrayIconDebug",0)

Local $LogFile = FileOpen("C:\Scripts\WebService_Logging.txt",1)
Local $ID = "webservice_"

Local $ActivityDB = "ES1Activity"
Local $SearchDB = "ES1Search"
Local $DBServer = "ES1"
Local $BuildLocation = "C:\Share\Builds\"
Local $WinTitle = "EMC SourceOne Web Services Installation"
Local $WizardTitle = "EMC SourceOne Web Services - InstallShield Wizard"
Local $IISTitle = "EMC SourceOne Warning"
Local $WarningTitle

_main()

;WinActivate("EMC SourceOne Web Services Installation")
;Sleep (5000)

Func _main()
   
   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Arch" & @OSArch)
   
   StartUpgrade()
   _ScreenCapture_Capture($BuildLocation & $ID & "WebServiceEntryWizzard.jpg")
   
   If $CmdLine[0] = 3 Then
	  
	  ;Specify DB Server 
	  $DBServer = $CmdLine[3]
	  FileWriteLine($LogFile,_Now() & ":INFO: Get Database Server from parameter.." & $DBServer)
	  
	  If $CmdLine[1] = "Param1" Then
		 If $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ":INFO: Upgrade operation has been started..")
			ClickUpgradeButton()
			;ClickNextButton()
			
			_ScreenCapture_Capture($BuildLocation & $ID & "BeforeVerifyWorkerIsInstalled.jpg")
			VerifyWorkerIsInstalled()
			_ScreenCapture_Capture($BuildLocation & $ID & "AfterVerifyWorkerIsInstalled.jpg")
			
		 Else
			FileWriteLine($LogFile,_Now() & ":INFO: New install operation has been started..")
			ClickNextButton()
			;Worker component is a must to be installed on same machine for web service installed
			
			_ScreenCapture_Capture($BuildLocation & $ID & "BeforeVerifyWorkerIsInstalled.jpg")
			VerifyWorkerIsInstalled()
			_ScreenCapture_Capture($BuildLocation & $ID & "AfterVerifyWorkerIsInstalled.jpg")
			
			_ScreenCapture_Capture($BuildLocation & $ID & "BeforeEnableIIS.jpg")
			EnableIIS()
			_ScreenCapture_Capture($BuildLocation & $ID & "AfterEnableIIS.jpg")
			
			_ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigFolder.jpg")
			ConfigFolder()
			_ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigFolder.jpg")
			
		 EndIf
	  EndIf
   EndIf
   
   _ScreenCapture_Capture($BuildLocation & $ID & "BeforeConfigAccount.jpg")   
   ConfigAccount()
   _ScreenCapture_Capture($BuildLocation & $ID & "AfterConfigAccount.jpg")
   
   ClickNextButton()
   ClickInstallButton()
   ClickFinishButton()
   HandleRestart()
   EndFunc

Func EnableIIS()
   
   If WinActive($IISTitle) Then
	  WinActivate($IISTitle)
	  ControlClick($IISTitle,"","[CLASS:Button; TEXT:&Yes; INSTANCE:1]")
	  FileWriteLine($LogFile,_Now() &":INFO:Successfully click button for enabling 32bit for IIS application...")
   Else
	  FileWriteLine($LogFile,_Now() &":INFO:No need to click button for enabling 32bit for IIS application...")
   EndIf
   
   EndFunc

Func StartUpgrade()
   Run($BuildLocation & "ES1_WebServicesSetup.exe")
   WinActivate($WizardTitle)
   IsPreInstall()
   WinActivate($WinTitle)
   WinWaitActive($WinTitle)
   Sleep (30000)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully launch web services installer and waiting for windows activation...")
   EndFunc

Func ConfigFolder()
   
    Dim $WorkDirectory = ControlGetText($WinTitle,"","[CLASS:Static; INSTANCE:3]")
			If StringCompare($WorkDirectory, "C:\",2) >0 Then
			   ClickNextButton()
			   ; Add logic of Click OK button for confirmation
			   If WinActive($WarningTitle) Then
				  WinActivate($WarningTitle)
				  ControlClick($WarningTitle,"","[CLASS:Button; TEXT:OK; INSTANCE:1]")
				  Sleep(15000)
			   Else
				  FileWriteLine($LogFile,_Now() &":INFO:No Warning message for configuring as C disk...")
			   EndIf
			EndIf
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configurate worker folder...")

   EndFunc

Func VerifyWorkerIsInstalled()
   
   Local $IsErrorVisible = ControlCommand($WinTitle,"","[CLASS:Static; INSTANCE:12]","IsVisible","")
   
   If $IsErrorVisible = 1 Then
	  Dim $ErrorMsg = ControlGetText($WinTitle,"","[CLASS:Static; INSTANCE:12]")
	  FileWriteLine($LogFile,_Now() &":INFO:Reading error message.." & $ErrorMsg)
	  If StringInStr($ErrorMsg, "EMC SourceOne Worker") >0 Then
		 ClickNextButton()
		 ClickFinishButton()
		 FileWriteLine($LogFile,_Now() &":ERROR:Web service is NOT installed due to incorrect worker installed on this machine...")
		 Exit
	  EndIf
   Else
	  FileWriteLine($LogFile,_Now() &":INFO:Worker is correctly installed on this machine...")
   EndIf
   
   EndFunc



