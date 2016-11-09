#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common.au3"
#include <Date.au3>
#include <WinAPI.au3>

AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

FileDelete("C:\Scripts\SupervisorBCE_Logging.txt")
Dim $LogFile = FileOpen("C:\Scripts\SupervisorBCE_Logging.txt",9)

Dim $WinTitle = "EMC SourceOne Email Supervisor Business Component Extensions Installation"

Dim $IdSupQueueDBServerEdit = "[CLASS:RichEdit20W; INSTANCE:1]"
Dim $IdSupQueueDBNameEdit = "[CLASS:RichEdit20W; INSTANCE:2]"

Local $SupQueueDBServer = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupQueueDBServer", "SQL01")
Local $SupQueueDBName = IniRead(@ScriptDir & "\Config.ini", "S1Account", "SupQueueDBName", "Supervisor_Queue")

Local $InstallerLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")
FileWriteLine($LogFile,_Now() & ": INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
Local $Setup=0
Local $SupervisorBCELocation = $InstallerLocation & "Setup"
Local $hSearch = FindFile($SupervisorBCELocation, "ES1_SupervisorBCE.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ": INFO: ES1_SupervisorBCE.exe is NOT found")
	MsgBox(64,"Warning!","ES1_SupervisorBCE.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ": INFO: ES1_SupervisorBCE.exe is found: " & $Setup )
	FlushLogging()

EndIf


Local $InstallerVersion = StringLeft(FileGetVersion($Setup, "FileVersion"),4)
FileWriteLine($LogFile,_Now() & ": INFO: Installer version is " & $InstallerVersion )

_main()



Func _main()

   FileWriteLine($LogFile,_Now() & ": INFO: Get OS Version" & @OSVersion)
   FileWriteLine($LogFile,_Now() & ": INFO: Get OS Arch" & @OSArch)

   If IsAdmin() Then
	  FileWriteLine($LogFile,_Now() & ": INFO: IsAdmin: Admin rights are detected.")
   EndIf

   If $CmdLine[0] = 2 Then
	  If $CmdLine[1] = "Param1" Then
		 If $CmdLine[2] = "I" Then
			FileWriteLine($LogFile,_Now() & ": INFO: Supervisor BCE Install operation has been started..")
			SupervisorBCEInstall()
		 ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ": INFO: Supervisor BCE Uninstall operation has been started..")
			If CheckS1RegKeyExist($RegSupBCE) == True Then
			   SupervisorBCEUninstall()
			Else
			   FileWriteLine($LogFile,_Now() & ": INFO: Supervisor BCE Registry is not found, hence exited...")
			   Exit
			EndIf
		 ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ": INFO: Supervisor BCE Repair operation has been started..")
			If CheckS1RegKeyExist($RegSupBCE) == True Then
			   SupervisorBCERepair()
			Else
			   FileWriteLine($LogFile,_Now() & ": INFO: Supervisor BCE Registry is not found, hence exited...")
			   Exit
			EndIf
		 ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ": INFO: Supervisor BCE Upgrade operation has been started..")
			SupervisorBCEUpgrade()
		 EndIf
	  EndIf
   EndIf

EndFunc

Func SupervisorBCEInstall()

   Local $bLoadingComplete

   RunInstaller($Setup)

   ;Wait for installer loading completes
   $bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
   If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
   EndIf
   ClickButtonByName($WinTitle, "&Next >")

   Sleep($CONFIG_DELAY_TIME)
   If WinExists($WinTitle,"Database Input") Then
	  ConfigDatabaseInput()
	  ClickButtonByName($WinTitle, "&Next >")
   else
	  FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Database Input page, have to exit...")
	  Exit
   EndIf

   Sleep($CONFIG_DELAY_TIME)
   If WinExists($WinTitle,"Ready to Install") Then
	  ClickButtonByName($WinTitle, "&Install")
   else
	  FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Ready to Install page, have to exit...")
	  Exit
   EndIf

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
   If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
   EndIf
   ClickButtonByName($WinTitle, "&Finish")

   FileWriteLine($LogFile, _Now() & ": INFO: Supervisor BCE installed successfully !")

EndFunc

Func SupervisorBCEUninstall()

   Local $bLoadingComplete

   RunInstaller($Setup)

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
   If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
   EndIf
   ClickButtonByName($WinTitle, "&Next >")

   Sleep($CONFIG_DELAY_TIME)
   If WinExists($WinTitle,"Program Maintenance") Then
	  ClickButtonByName($WinTitle, "&Remove")
	  ClickButtonByName($WinTitle, "&Next >")
	  ClickButtonByName($WinTitle, "&Remove")
   else
	  FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Program Maintenance page, have to exit...")
	  Exit
   EndIf

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
   If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
   EndIf
   ClickButtonByName($WinTitle, "&Finish")

   FileWriteLine($LogFile, _Now() & ": INFO: Supervisor BCE uninstalled successfully !")

EndFunc

Func SupervisorBCERepair()

   Local $bLoadingComplete

   RunInstaller($Setup)

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
   If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
   EndIf
   ClickButtonByName($WinTitle, "&Next >")

   Sleep($CONFIG_DELAY_TIME)
   If WinExists($WinTitle,"Program Maintenance") Then
	  ClickButtonByName($WinTitle, "Re&pair")
	  ClickButtonByName($WinTitle, "&Next >")
	  ClickButtonByName($WinTitle, "&Install")
   else
	  FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Program Maintenance page, have to exit...")
	  Exit
   EndIf

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
   If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
   EndIf
   ClickButtonByName($WinTitle, "&Finish")

   FileWriteLine($LogFile, _Now() & ": INFO: Supervisor BCE repaired successfully !")
EndFunc

Func SupervisorBCEUpgrade()

   Local $bLoadingComplete

   RunInstaller($Setup)

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Update >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
   If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
   EndIf
   ClickButtonByName($WinTitle, "&Update >")

   Sleep($CONFIG_DELAY_TIME)
   If WinExists($WinTitle,"Database Input") Then
	  ConfigDatabaseInput()
	  ClickButtonByName($WinTitle, "&Next >")
   else
	  FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Database Input page, have to exit...")
	  Exit
   EndIf

   Sleep($CONFIG_DELAY_TIME)
   If WinExists($WinTitle,"Ready to Install") Then
	  ClickButtonByName($WinTitle, "&Install")
   else
	  FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Ready to Install page, have to exit...")
	  Exit
   EndIf

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
   If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
   EndIf
   ClickButtonByName($WinTitle, "&Finish")

   FileWriteLine($LogFile, _Now() & ": INFO: Supervisor BCE upgraded successfully !")
EndFunc

Func ConfigDatabaseInput()
   FileWriteLine($LogFile,_Now() &": INFO: ConfigDatabaseInput called" )

   ;Input Queue database server
   ControlFocus($WinTitle,"",$IdSupQueueDBServerEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($SupQueueDBServer)
   FileWriteLine($LogFile,_Now() &": INFO: Successfully configure Queue Database server with:" & $SupQueueDBServer)

   ;Input Queue database name
   ControlFocus($WinTitle,"",$IdSupQueueDBNameEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($SupQueueDBName)
   FileWriteLine($LogFile,_Now() &": INFO: Successfully configure Queue Database server with:" & $SupQueueDBName)
EndFunc