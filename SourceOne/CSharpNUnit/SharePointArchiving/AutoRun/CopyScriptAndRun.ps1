$automationFolder = "\\10.32.121.199\public\tengk\Automation\"
$script1 = $automationFolder+"SPOAutoDeploy.ps1"
$script2 = $automationFolder+"RunTest.ps1"
$script3 = $automationFolder+"SendReport.ps1"
$script4 = $automationFolder+"RestartESService.bat"
$localscript = "C:\SPOscript"
Copy-Item $script1 -Destination $localscript 
Copy-Item $script2 -Destination $localscript 
Copy-Item $script3 -Destination $localscript 
Copy-Item $script4 -Destination $localscript 
sl $localscript
.\SPOAutoDeploy.ps1
cmd /c "RestartESService.bat"
.\RunTest.ps1
.\SendReport.ps1