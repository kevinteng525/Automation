#RequireAdmin
#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Run_AU3Check=n
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#include "Common2.au3"
#include <Date.au3>
#include <WinAPI.au3>

Opt("WinTitleMatchMode", 2) ;1=start, 2=subStr, 3=exact, 4=advanced, -1 to -4=Nocase

AutoItSetOption("TrayIconDebug",0)
Opt("MustDeclareVars", 0)

Dim $LogFile = FileOpen("C:\Scripts\SharePointBCE_Logging.txt",9)

Dim $hWnd
;Dim $WinTitle = "EMC SourceOne Business Component Extensions for Microsoft SharePoint - InstallShield Wizard"
Dim $WinTitle = "EMC SourceOne Business Component Extensions for Microsoft SharePoint"



; To Maintain in future
Dim $IdDomainEdit = 2930
Dim $IdServiceAccountEdit = 2935
Dim $IdServiceAccountPasswordEdit = 2940
Dim $IdSecurityGroupEdit = 2945

Dim $IdWorkingDirectory = 2978

Dim $IdSharePointBCERemoveCheckBox = 624
;Dim $IdSharePointBCERepairCheckBox = 1488

Local $Domain = "qaes1.com"
Local $ServiceAccount = "es1service"
Local $Password = "emcsiax@QA"
Local $SecurityGroup = "es1 security group"

Local $WorkingDirectory = "\\master01\share"


; For in-place run
;~ $FileBCELocation = @ScriptDir & "\"

Local $SharePointBCELocation = "C:\Share\Builds\"




FileWriteLine($LogFile,_Now() & ":INFO: ES1_SharePointBCESetup location: " & $SharePointBCELocation )

$hSearch = FileFindFirstFile($SharePointBCELocation & "ES1_SharePointBCESetup.exe")

; Check if the search was successful, if not display a message and return False.
    If $hSearch = -1 Then
         FileWriteLine($LogFile,_Now() & ":INFO: No ES1_SharePointBCESetup.exe is found")
        Exit
	Else
		$SharePointBCEBundle = FileFindNextFile($hSearch)
		FileWriteLine($LogFile,_Now() & ":INFO: ES1_SharePointBCESetup is found at: " & $SharePointBCEBundle )
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
			FileWriteLine($LogFile,_Now() & ":INFO: SharePointBCE Install operation has been started..")
			SharePointBCEInstall()
		ElseIf $CmdLine[2] = "Rm" Then
			FileWriteLine($LogFile,_Now() & ":INFO: SharePointBCE Uninstall operation has been started..")
			SharePointBCEUninstall()
		ElseIf $CmdLine[2] = "Re" Then
;			FileWriteLine($LogFile,_Now() & ":INFO: SharePointBCE Repair operation has been started..")
;			SharePointBCERepair()
		ElseIf $CmdLine[2] = "U" Then
;			FileWriteLine($LogFile,_Now() & ":INFO: SharePointBCE Upgrade operation has been started..")
;			SharePointBCEUpgrade()
		EndIf
	  EndIf
   EndIf

EndFunc

Func RunInstaller()

	FileWriteLine($LogFile, _Now() & ":INFO: Func RunInstaller")

    Run($SharePointBCELocation & $SharePointBCEBundle)
	FileWriteLine($LogFile,_Now() &":INFO: SharePointBCE setup is runned")
	WinWaitActive($WinTitle, "", 60000)
	$hWnd = WinGetHandle($WinTitle, "")
	FileWriteLine($LogFile,_Now() &":INFO:Got SharePointBCE handler")
EndFunc



Func SharePointBCEInstall()
	Local $btnFinishID
	Local $bLoadingComplete

	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle, "&Next >", $DEFAULT_PAGE_LOADING_RETRY_TIMES )
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf

	If CheckS1RegKeyExist("Worker+") == True Then

		ClickButtonByName($WinTitle, "&Next >")

		FileWriteLine($LogFile, _Now() & ":Debug: default to install sharepoint archiving and sharepoint restore components")

		ClickButtonByName($WinTitle, "&Next >")

		;ConfigSecurityGroupPage()

		ClickButtonByName($WinTitle, "&Next >")

;~ 		ConfigWorkingDirectoryPage()
		ConfigWorkingDirectoryPage()

		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Install")

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		EndIf

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: SharePointBCE Install Done!")


	ElseIf CheckS1RegKeyExist("Worker+") == False Then

		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Next >")

		ConfigWorkingDirectoryPage()

		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Install")

;~ 		$btnFinishID= IsFileBCEFinish($WinTitle, "&Finish")
;~ 		FileWriteLine($LogFile, _Now() & ":INFO: Finish ID returns : " & $btnFinishID)

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish", $DEFAULT_PAGE_LOADING_RETRY_TIMES)
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		EndIf

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: SharePointBCE Install Done!")
	EndIf

EndFunc


Func SharePointBCEUpgrade()


	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()


	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Update >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	If CheckS1RegKeyExist("Worker+") == True Then

		ClickButtonByName($WinTitle, "&Update >")

		ConfigSecurityGroupPage()

		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Install")

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		EndIf

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: SharePointBCE Upgrade Done!")



	ElseIf CheckS1RegKeyExist("Worker+") == False Then

		ClickButtonByName($WinTitle, "&Update >")

		ClickButtonByName($WinTitle, "&Install")

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		EndIf

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: SharePointBCE Upgrade Done!")
	EndIf

EndFunc

Func SharePointBCEUninstall()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes
	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf


	ClickButtonByName($WinTitle, "&Next >")

	ClickSharePointBCESelection($WinTitle, $IdSharePointBCERemoveCheckBox)

	ClickButtonByName($WinTitle, "&Next >")

	ClickButtonByName($WinTitle, "&Remove")


	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
		Exit
	EndIf

	ClickButtonByName($WinTitle, "&Finish")
	FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
	FileWriteLine($LogFile, _Now() & "INFO: SharePointBCE Remove Done!")



EndFunc

Func SharePointBCERepair()
	Local $btnFinishID
	Local $bLoadingComplete
	RunInstaller()

	;Wait for installer loading completes

	$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Next >",$DEFAULT_PAGE_LOADING_RETRY_TIMES)

;~ 	$bLoadingComplete = IsFileBCECompleteLoading($WinTitle,"&Next >")
	If $bLoadingComplete == False Then
		FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
	EndIf



	If CheckS1RegKeyExist("Worker+") == True Then

		ClickButtonByName($WinTitle, "&Next >")

		ClickFileBCESelection($WinTitle, $IdFileBCERepairCheckBox)
		ClickButtonByName($WinTitle, "&Next >")

		ConfigSecurityGroupPage()
		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Install")

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		EndIf

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: SharePointBCE Repair Done!")


	ElseIf CheckS1RegKeyExist("Worker+") == False Then

		ClickButtonByName($WinTitle, "&Next >")

		ClickFileBCESelection($WinTitle, $IdFileBCERepairCheckBox)
		ClickButtonByName($WinTitle, "&Next >")

		ClickButtonByName($WinTitle, "&Install")

		$bLoadingComplete = IsInstallerPageLoaded($WinTitle,"&Finish")
		If $bLoadingComplete == False Then
			FileWriteLine($LogFile, _Now() & ":Warning: Something wrong with the installer, have to exit...")
			Exit
		EndIf

		ClickButtonByName($WinTitle, "&Finish")
		FileWriteLine($LogFile, _Now() & "INFO: Finish button Clicked!")
		FileWriteLine($LogFile, _Now() & "INFO: SharePointBCE Repair Done!")


	EndIf
EndFunc





Func ConfigSecurityGroupPage()
   ;Abandon ControlSetText method since it's not supported in SourceOne account password validation

   ;Input Domain textbox
   ControlFocus($WinTitle,"",$IdDomainEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($Domain)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure domain with:" & $Domain)

   ;Input Service Account
   ControlFocus($WinTitle,"",$IdServiceAccountEdit)
   Sleep($CONFIG_DELAY_TIME)
   Send($ServiceAccount)
   FileWriteLine($LogFile,_Now() &":INFO:Successfully configure service account with:" & $ServiceAccount)


	;Input Service Password
	ControlFocus($WinTitle,"",$IdServiceAccountPasswordEdit )
	Sleep($CONFIG_DELAY_TIME)
	Send($Password)
	FileWriteLine($LogFile,_Now() &":INFO:Successfully configure service account password with:" & $Password)

    ;Input Security Group
	ControlFocus($WinTitle,"",$IdSecurityGroupEdit )
	Sleep($CONFIG_DELAY_TIME)
	Send($SecurityGroup)
	FileWriteLine($LogFile,_Now() &":INFO:Successfully configure security group with:" & $SecurityGroup)

    Sleep($CONFIG_DELAY_TIME)
	EndFunc


 Func ConfigWorkingDirectoryPage()

	;Input Working Directory
	Sleep($CONFIG_DELAY_TIME)
	WinWaitActive($WinTitle)
	ControlFocus($WinTitle,"",$IdWorkingDirectory)
	Sleep($CONFIG_DELAY_TIME)
	Send($WorkingDirectory)
	Sleep($CONFIG_DELAY_TIME)
	FileWriteLine($LogFile,_Now() &":INFO:Successfully configure working directory:" & $WorkingDirectory)
	Sleep($CONFIG_DELAY_TIME)
EndFunc


Func ClickSharePointBCESelection($_WinTitle = "", $Id = "")

  Local $IsControlVisible = 0
	Local $Retry = 0

	;Wait until install is completed
	While $IsControlVisible < 1
		If $Retry < 50 Then
			FileWriteLine($LogFile, _Now() & ":INFO:Retry with: " & $Retry)
			$Retry = $Retry + 1
			$IsControlVisible = ControlCommand($_WinTitle, "", $Id, "IsVisible", "")
			FileWriteLine($LogFile, _Now() & ":INFO:Still wait for completion, wait" & $CONFIG_DELAY_TIME/1000 & " seconds...")
			Sleep($CONFIG_DELAY_TIME)
		Else
			FileWriteLine($LogFile, _Now() & ":ERROR:Retry over 50 and continue...")
			FlushLogging()
			Exit
		EndIf
	WEnd

	ControlClick($_WinTitle, "", $Id)
	FileWriteLine($LogFile,_Now() &":INFO: Control Clicked...")
	Sleep($CONFIG_DELAY_TIME)
EndFunc
