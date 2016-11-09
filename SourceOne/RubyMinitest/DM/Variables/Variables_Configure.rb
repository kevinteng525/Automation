#DM Web site IP address.
require_relative '..\XmlHelper.rb'

#Read the IP Address from the configure file environment.xml
config = XmlHelper.new("C:\\SaberAgent\\Config\\environment.xml")
_IPAddress = config.get_test_agent_ip("Master")

#Read the user's domain from the configure file environment.xml
$_UserDomain = config.get_user_domain

#DM Web URL.
$DMWebSite = 'https://'+_IPAddress.to_s+'/DiscoveryManagerWeb/'

#Login user name.
$username = 'User1'
$username1 = 'user2'

#es1service user name
$es1service = 'es1service'

#default red tag name
$tagname='Responsive'

#Add user's full domain name, small letter version
$deleteusername = $username+' ('+$username.downcase+'@'+$_UserDomain+')'

#Add user's full domain name, original version
$deleteUserNameO = $username+' ('+$username+'@'+$_UserDomain+')'
$deleteUserName1 = $username1+' ('+$username1+'@'+$_UserDomain+')'

#Login Password.
$password = 'emcsiax@QA'

#Matter name.
$mattername = 'test_gff'

#Search name.
$searchname = 'search_gff'


#Folder name.
$foldername = 'test_hjz'

#All the items can be searched.
$searchnumber = '50'

#Identity name.
$identityname = 'identity_gff'

#Identity flag
$identityflag = '0'

#Export File location.
$exportfilelocation = '\\\work01\share\joblog'





