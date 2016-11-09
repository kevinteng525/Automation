#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common2.au3"
#include <Date.au3>
#include <WinAPI.au3>


AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Local $ID = "OfflineAccess_64_"
Local $LogFile = FileOpen("C:\Scripts\OA64_Logging.txt",1)

;Local $BuildLocation = "C:\Share\Builds\"
Local $InstallerLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")
Local $WinTitle = "EMC SourceOne Offline Access"
Local $WizardTitle = "EMC SourceOne Offline Access - InstallShield Wizard"
Local $WarningTitle = "EMC SourceOne Warning"

local $RealTimeRetrievalServer = "WORK01"
Local $BackfillServer = "WORK01"



FileWriteLine($LogFile,_Now() & ":INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
Local $Setup=0
Local $hSearch = FindFile($InstallerLocation, "ES1_OfflineAccessx64.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_OfflineAccessx64.exe is NOT found")
	MsgBox(64,"Warning!","ES1_OfflineAccessx64.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ":INFO: ES1_OfflineAccessx64.exe is found: " & $Setup )
	FlushLogging()

EndIf


Local $InstallerVersion = StringLeft(FileGetVersion($Setup, "FileVersion"),4)
FileWriteLine($LogFile,_Now() & ":INFO: Installer version is " & $InstallerVersion )

; Get IDs from per different branch
If  $InstallerVersion == "7.20" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.20 IDs" )
; To Maintain in future
Dim $IdOARemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdOARepairCheckBox = "[CLASS:Button; INSTANCE:2]"

ElseIf $InstallerVersion == "7.12" Then

FileWriteLine($LogFile,_Now() & ":INFO: Set 7.12 IDs" )
; To Maintain in future
Dim $IdOARemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdOARepairCheckBox = "[CLASS:Button; INSTANCE:2]"

Else

FileWriteLine($LogFile,_Now() & ":INFO: Set other version IDs" )
; To Maintain in future
Dim $IdOARemoveCheckBox = "[CLASS:Button; INSTANCE:3]"
Dim $IdOARepairCheckBox = "[CLASS:Button; INSTANCE:2]"

EndIf



_main()


Func _main()

 FileWriteLine($LogFile,_Now() & ":INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile,_Now() & ":INFO: Get OS Arch" & @OSArch)

    If IsAdmin() Then
		FileWriteLine($LogFile,_Now() & "INFO: IsAdmin: Admin rights are detected.")
	EndIf

	If $CmdLine[0] = 2 Then
	  If $CmdLine[1] = "Param1" Then
		If $CmdLine[2] = "I" Then

			OAInstall()
		ElseIf $CmdLine[2] = "Rm" Then

			OAUninstall()
		ElseIf $CmdLine[2] = "Re" Then
			OARepair()
		ElseIf $CmdLine[2] = "U" Then

			OAUpgrade()
		EndIf
	  EndIf
   EndIf
   EndFunc

Func OAInstall()
   FileWriteLine($LogFile,_Now() & ":INFO: OfflineAccess64 Install operation has been started..")
   Local $btnFinishID
   Local $bLoadingComplete

   RunInstaller($Setup)

   ;handle prerequisites window
	Sleep($CONFIG_DELAY_TIME)
	If WinExists($WizardTitle,"EMC SourceOne Offline Access requires the following items to be installed on your computer.") Then
		WinActivate($WizardTitle)
		FileWriteLine($LogFile,_Now() &":INFO: OfflineAccess prerequisites window found and activated.")
	    Sleep($CONFIG_DELAY_TIME)
	    ClickButtonByName($WizardTitle, "Install")
	    FileWriteLine($LogFile,_Now() &": INFO: Install button clicked.")
		Sleep($CONFIG_DELAY_TIME)
   EndIf

   ;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsMasterCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	 EndIf

   Sleep($CONFIG_DELAY_TIME)
   ClickButtonByName($WinTitle, "&Next >")
   ClickButtonByName($WinTitle, "&Next >")
   ConfigWebServiceHost()
   Sleep($CONFIG_DELAY_TIME)
   ClickButtonByName($WinTitle, "&Next >")
   ClickButtonByName($WinTitle, "&Install")

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	 EndIf

	 Sleep($CONFIG_DELAY_TIME)

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

   FileWriteLine($LogFile, _Now() & ":OA64 install done")

EndFunc

Func OARepair()
   FileWriteLine($LogFile,_Now() & ":INFO: OfflineAccess64 Repair operation has been started..")
   Local $btnFinishID
   Local $bLoadingComplete

   RunInstaller($Setup)
   ;Wait for installer loading completes

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsMasterCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	 EndIf

   Sleep($CONFIG_DELAY_TIME)
   ClickButtonByName($WinTitle, "&Next >")
   ClickPageCheckBoxByID($WinTitle, $IdOARepairCheckBox)
   ClickButtonByName($WinTitle, "&Next >")
   ConfigWebServiceHost()
   Sleep($CONFIG_DELAY_TIME)
   ClickButtonByName($WinTitle, "&Next >")
   ClickButtonByName($WinTitle, "&Install")

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	 EndIf

	 Sleep($CONFIG_DELAY_TIME)

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

   FileWriteLine($LogFile, _Now() & ":OA64 Repair done")
EndFunc

Func OAUninstall()
   FileWriteLine($LogFile,_Now() & ":INFO: OfflineAccess64 Uninstall operation has been started..")
   Local $btnFinishID
   Local $bLoadingComplete

   RunInstaller($Setup)
   ;Wait for installer loading completes

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsMasterCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	 EndIf

	 Sleep($CONFIG_DELAY_TIME)

   ClickButtonByName($WinTitle, "&Next >")
   ClickPageCheckBoxByID($WinTitle, $IdOARemoveCheckBox)
   ClickButtonByName($WinTitle, "&Next >")
   ClickButtonByName($WinTitle, "&Remove")

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	 EndIf

	 Sleep($CONFIG_DELAY_TIME)

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

   FileWriteLine($LogFile, _Now() & ":OA64 Remove done")
EndFunc

Func OAUpgrade()
   FileWriteLine($LogFile,_Now() & ":INFO: OfflineAccess64 Upgrade operation has been started..")
   Local $btnFinishID
   Local $bLoadingComplete

   RunInstaller($Setup)
   ;Wait for installer loading completes

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Upgrade > >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsMasterCompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	 EndIf

   Sleep($CONFIG_DELAY_TIME)
   ClickButtonByName($WinTitle, "&Upgrade >")
   ConfigWebServiceHost()
   Sleep($CONFIG_DELAY_TIME)
   ClickButtonByName($WinTitle, "&Next >")
   ClickButtonByName($WinTitle, "&Install")

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	 EndIf

	 Sleep($CONFIG_DELAY_TIME)

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")

   FileWriteLine($LogFile, _Now() & ":OA64 Upgrade done")
   EndFunc



Func ConfigWebServiceHost()


   ;Input Real time item retieval server
   Sleep($CONFIG_DELAY_TIME)

   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:1]")
   Send($RealTimeRetrievalServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Real time item retrieval server with:" & $RealTimeRetrievalServer)

   ;Input Real time item retieval server port
   Sleep($CONFIG_DELAY_TIME)

   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:2]")
   Send("8001")
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure Real time item retrieval server port with 8001")

   ;Input back fill server
   Sleep($CONFIG_DELAY_TIME)

   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:3]")
   Send($BackfillServer)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure back fill server with:" & $BackfillServer)

   ;Input back fill server port
   Sleep($CONFIG_DELAY_TIME)

   ControlFocus($WinTitle,"","[CLASS:RichEdit20W; INSTANCE:4]")
   Send("8002")
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure fack fill server port with 8002")

   EndFunc




