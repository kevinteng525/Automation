$convertTool = "\\10.32.121.199\public\tengk\Automation\ConvertTestResult.exe"
$xsl = "\\10.32.121.199\public\tengk\Automation\SPOTestResult.xsl"
$localscript = "C:\SPOscript\"
Copy-Item $convertTool -Destination $localscript 
Copy-Item $xsl -Destination $localscript 
cmd /c "ConvertTestResult.exe SPOTestResult.xml SPOTestResult.xsl SPOTestResult.html"
$dropFolder = "\\10.32.121.199\SPOBuilds\NashuaBuilds\"
$lastestBuild = (Get-ChildItem $dropFolder * | Sort-Object 'LastWriteTime')[-1]
#Get attached latest test result.
$testResult = $localscript + "SPOTestResult.html"
$MailBody = Get-Content $testResult
 
#Send mail notification
$CS = Gwmi Win32_ComputerSystem -computerName "."
if ($CS.Name -eq "ES1ALL"){
	$SmtpClient = New-Object System.Net.Mail.SmtpClient
	$MailMessage = New-Object System.Net.Mail.MailMessage
	$SmtpClient.Host = "localhost"
	$MailMessage.from = ("ES1 SPO provider BVT Test Result (DO NOT REPLY) <ES1SPOBVT@emc.com>")
	$MailMessage.To.add("kevin.teng@emc.com")
	$MailMessage.To.add("Travis.Liu@emc.com")
	$MailMessage.To.add("Jenny.Wu@emc.com")
	$MailMessage.CC.add("yue.li@emc.com")
    	$MailMessage.IsBodyHtml = $true
	$MailMessage.subject = "SPO provider Build: <" + $lastestBuild + "> BVT Result" 
	$MailMessage.Body = $MailBody 
	$SmtpClient.Send($MailMessage)
}