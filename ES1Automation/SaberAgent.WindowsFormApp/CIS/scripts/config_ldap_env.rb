require 'json'
require 'rest-client'
class CIS_Client
        def initialize
          @base_url = 'https://localhost:445/cis/v2.0'
        end
	def newRestClient(url, token=nil)
	 if(token != nil)
	   headers   = {:'x-auth-token' => token}
	 elsif(@current_token != nil)
	   headers = {:'x-auth-token' => @current_token}
	 else
	    headers = {}
	 end
		    	
	 return RestClient::Resource.new(url,
		:verify_ssl => false,
		:headers => headers,
		:content_type => :json,
		:accept => :json)
	end

	def logon(username, password, rememberMe=false)
	  url = "#{@base_url}/auth/authenticate"
	  body = { :username => username, :password => password }
	  result = newRestClient(url).post(body.to_json)
          puts result
	  if (rememberMe)
	    # Set the token context so we don't need to pass in the token everytime
	    @current_token = result.headers[:http_x_auth_token]
	  end
          puts result.headers[:http_x_auth_token]
	  return result.headers[:http_x_auth_token]
	end	  

	def addLdapInfo(body)
	  newRestClient("#{@base_url}/ldap_info").put(body.to_json) do |response, request, result|  
	     puts response     
	     return response
	  end
	end
        def getLdapInfo()
           newRestClient("#{@base_url}/ldap_info").get() do |response, request, result|
              puts response
              return response
           end
        end
        def assignUserRole(user_name, body)
           newRestClient("#{@base_url}/appuserroles/#{user_name}").post(body.to_json) do |response, request, result|
              puts response
           end
        end

end
default_ad = [
    {
	  "name"    => "AD", 
	  "host"    => "192.168.2.10",
	  "port"    => "389",
	  "type"    => "AD",
	  "timeout" => "2500",
	  "basedn"  => "dc=domain,dc=com",
	  "username"=> "administrator",
	  "password"=> "emcsiax@QA"
    },
    {
          "name"    => "DefaultOpenLDAP",
          "host"    => "127.0.0.1",
          "port"    => "389",
          "type"    => "OPENLDAP",
          "timeout" => "2500",
          "basedn"  => "dc=example,dc=com",
          "username"=> "cisadmin",
          "password"=> "emc",
          "default" => true
    }
]
cis_client = CIS_Client.new
cis_client.logon('cisadmin', 'emc', true)
cis_client.getLdapInfo()
cis_client.assignUserRole('administrator',{"add"=>"ADMIN"})
