#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common.au3"
#include <Date.au3>
#include <WinAPI.au3>

AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

FileDelete("C:\Scripts\SupervisorSRE_Logging.txt")
Dim $LogFile = FileOpen("C:\Scripts\SupervisorSRE_Logging.txt",9)

Dim $WinTitle = "EMC SourceOne SRE Installation"

Local $InstallerLocation = IniRead(@ScriptDir & "\Config.ini", "Common", "InstallerLocation", "C:\Share\Builds\")
FileWriteLine($LogFile,_Now() & ": INFO: Installer location: " & $InstallerLocation )

; Search Installer under $InstallerLocation, only return first one.
$SupervisorSRELocation = $InstallerLocation & "Redistributable"
Local $Setup=0
Local $hSearch = FindFile($SupervisorSRELocation, "ES1_SRESetup.exe",12)

If $hSearch == 0 Then
	FileWriteLine($LogFile,_Now() & ": INFO: ES1_SRESetup.exe is NOT found")
	MsgBox(64,"Warning!","ES1_SRESetup.exe is NOT found, exit.")
	Exit
ElseIf  $hSearch[0] == 1 Then
	$Setup = $hSearch[1]
	FileWriteLine($LogFile,_Now() & ": INFO: ES1_SRESetup.exe is found: " & $Setup )
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
			FileWriteLine($LogFile,_Now() & ": INFO: Supervisor SRE Install operation has been started..")
			SupervisorSREInstall()
		 ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ": INFO: Supervisor SRE Uninstall operation has been started..")
			If CheckS1RegKeyExist($RegSupSRE) == True Then
			   SupervisorSREUninstall()
			Else
			   FileWriteLine($LogFile,_Now() & ": INFO: Supervisor SRE Registry is not found, hence exited...")
			   Exit
			EndIf
		 ElseIf $CmdLine[2] = "Re" Then
			FileWriteLine($LogFile,_Now() & ": INFO: Supervisor SRE Repair operation has been started..")
			If CheckS1RegKeyExist($RegSupSRE) == True Then
			   SupervisorSRERepair()
			Else
			   FileWriteLine($LogFile,_Now() & ": INFO: Supervisor SRE Registry is not found, hence exited...")
			   Exit
			EndIf
		 ElseIf $CmdLine[2] = "U" Then
			FileWriteLine($LogFile,_Now() & ": INFO: Supervisor SRE Upgrade operation has been started..")
			SupervisorSREUpgrade()
		 EndIf
	  EndIf
   EndIf

EndFunc

Func SupervisorSREInstall()

   Local $bLoadingComplete

   RunInstaller($Setup)

   ;Wait for installer loading completes
   $bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
   If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
   EndIf
   ClickButtonByName($WinTitle, "&Next >")

   Sleep($CONFIG_DELAY_TIME)
   If WinExists($WinTitle,"Destination Folder") Then
	  ClickButtonByName($WinTitle, "&Next >")
   else
	  FileWriteLine($LogFile, _Now() & ": ERROR: Something wrong to load Destination Folder page, have to exit...")
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

EndFunc

Func SupervisorSREUninstall()

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

EndFunc

Func SupervisorSRERepair()

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

EndFunc

Func SupervisorSREUpgrade()

   Local $bLoadingComplete

   RunInstaller($Setup)

   $bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
   If $bLoadingComplete == False Then
	  FileWriteLine($LogFile, _Now() & ": Warning: Something wrong with the installer, have to exit...")
   EndIf
   ClickButtonByName($WinTitle, "&Next >")

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

EndFunc