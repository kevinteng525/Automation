#run BVT testing script
$localAutoBinPath = "C:\Automation\TestCode\SPOTestWithNunit\bin\Debug"
sl $localAutoBinPath

Write-Host "Starting Tests..."
#nunit-console-x86.exe SPOTestWithNunit.dll /xml:C:\SPOscript\SPOTestResult.xml /include:BVT
nunit-console-x86.exe SPOTestWithNunit.dll /xml:C:\SPOscript\SPOTestResult.xml
sl "C:\SPOscript" 