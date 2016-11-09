require_relative '..\Common\Project_Path.rb'
#Supervisor Web site IP address.
#require_relative '..\XmlHelper.rb'

#Read the IP Address from the configure file environment.xml
#config = XmlHelper.new("C:\\SaberAgent\\Config\\environment.xml")
#_IPAddress = config.get_test_agent_ip("Master")


#Web Browser time out seconds.
$TIMEOUT = 30

#Read the user's domain from the configure file environment.xml
#$_UserDomain = config.get_user_domain
$_UserDomain = 'Domain'

#Supervisor Web URL.
#$SUPWebSite = 'https://'+_IPAddress.to_s+'/DiscoveryManagerWeb/'
$SUPWebSite = 'https://10.98.27.67/SupervisorWeb'

#login error info.
$loginerror1 = 'Login failed. The username or password is incorrect.'

#login error info.
$loginerror2 = 'Login failed. You have exceeded the maximum number of allowed login attempts. Try again after 30 minutes.'

#data excel path
$excel_dir = ProjectPath.get_root_path+'TestData\supervisor_test_data.xlsx'





