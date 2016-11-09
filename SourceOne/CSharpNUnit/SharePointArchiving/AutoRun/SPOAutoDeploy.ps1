clear
#Basic path
[System.String] $basicPath = "C:\Program Files (x86)\EMC SourceOne\SharePointOnline\"
[System.String] $localInstallPath = "C:\SPOInstallPack\"
[System.String] $localAutoBinPath = "C:\Automation\TestCode\"
[System.String] $testResultPath = $localAutoBinPath + "TestResults\"

[System.String] $dropFolder = "\\10.32.121.199\SPOBuilds\NashuaBuilds\"
[System.String] $silentInstallFile = "\\10.32.121.199\SPOBuilds\Tools\Install_SPOBCE.bat"

[System.String] $automationFolder = "\\10.32.121.199\public\tengk\Automation\"

#Get the new build location.
$lastestBuild = (Get-ChildItem $dropFolder * | Sort-Object 'LastWriteTime')[-1]
$lastBuildPath = $dropFolder + $lastestBuild
$localWorkingPath = $localInstallPath + $lastestBuild + "\SourceOneOnline\Archiving\"


try {
#Copy the lastest build from the SPO drop folder to local.
Write-Host "Copy the lastest build from the SPO drop folder to local..."
	Copy-Item $lastBuildPath -Destination $localInstallPath -Recurse
#Copy the Silent install bat from the SPO drop folder to local.
Write-Host "Copy the Silent install bat from the SPO drop folder to local..."
	Copy-Item $silentInstallFile -Destination $localWorkingPath 
}
catch [System.Exception] {
	Write-Host "The following exceptions occurs: `n" + $_.Exception.ToString() 
	break
}

sleep 1

#Install the current latest version of SharePointOnline BCE.
Write-Host "Installing current version of SharePointOnline BCE..."
Set-Location $localWorkingPath

cmd /c 'Install_SPOBCE.bat'
sleep 1
$archivingClientConfig = $automationFolder + "SPOConfig\EMC.EX.SharePoint.ArchivingClient.dll.config"
$autobinFolder = $automationFolder + "TestCode\"
$autoServerConfig = $automationFolder + "ES1All\Configuration.xml"

#Copy modified EMC.EX.SharePoint.ArchivingClient.dll.config
Copy-Item $archivingClientConfig -Destination $basicPath -Force

#Deploy SharePointOnline Automation binary

Copy-Item $autobinFolder -Destination $localAutoBinPath -Recurse -Force

$localServerConfig = $localAutoBinPath + "SPOTestWithNunit\bin\Debug\Configuration.xml"
Copy-Item $autoServerConfig -Destination $localServerConfig -Force

#Copy SPO library
$localAutoLibPath = $localAutoBinPath + "SPOTestWithNunit\bin\Debug\"
Copy-Item $basicPath -Destination $localAutoLibPath -Recurse -Force
sleep 1

Write-Host "All Deployment finished!"

sl "C:\SPOscript" 



