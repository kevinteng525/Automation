require 'json'


role = ARGV[0]

config_file = 'install_config.json'

config = JSON.parse(File.read(config_file))

File.rename(config_file, config_file + ".bak")

if role.downcase == "master"
	puts "#{Time.now}: Start to update the setup config for CIS master node"
	#config["install_settings"]["install_puppet"] 			= "true"
	#config["install_settings"]["install_elasticsearch"] 	= "true"
	#config["install_settings"]["install_openldap"] 		= "true"
	#config["install_settings"]["install_cis"] 				= "true"
	
	#config["puppet_settings"]["puppet_master_host"] 		= "127.0.0.1"
	#config["puppet_settings"]["puppet_role"] 				= "master"
	
	config["elasticsearch_settings"]["cluster_name"]		= "cluster"
	config["elasticsearch_settings"]["node_name"]			= "node1"
	#config["elasticsearch_settings"]["heap_size"]			= "1"
	config["elasticsearch_settings"]["hosts"]				= "192.168.2.32"
	#config["elasticsearch_settings"]["master_node"]		= "true"
	#config["elasticsearch_settings"]["data_node"]			= "true"
	
	config["openldap_settings"]["role"]						= "master"
	#config["openldap_settings"]["master_address"]			= "192.168.2.31"
	
	#config["cis_settings"]["install_path"]		 			= "/usr/local/CIS"
	#config["cis_settings"]["cis_port"] 					= "445"
	#config["cis_settings"]["pass_through_port"] 			= "442"
	#config["cis_settings"]["global_config_hosts"]			= "127.0.0.1:9200"
	config["cis_settings"]["reset_global_config"] 			= "true"
	
	config["ldap_settings"]["ldap_type"] 					= "OPENLDAP"
	config["ldap_settings"]["ldap_host"] 					= "127.0.0.1"
	config["ldap_settings"]["ldap_port"] 					= "389"
	config["ldap_settings"]["ldap_base_dn"] 				= "dc=example,dc=com"
	config["ldap_settings"]["ldap_username"] 				= "cisadmin"
	config["ldap_settings"]["ldap_password"] 				= "emc"
else
	puts "#{Time.now}: Start to update the setup config for CIS slave node"
	config["install_settings"]["install_puppet"] 			= "false"
	#config["install_settings"]["install_elasticsearch"] 	= "true"
	#config["install_settings"]["install_openldap"] 		= "true"
	#config["install_settings"]["install_cis"] 				= "true"
	
	config["puppet_settings"]["puppet_master_host"] 		= "192.168.2.31"
	config["puppet_settings"]["puppet_role"] 				= "master"
	
	config["elasticsearch_settings"]["cluster_name"]		= "cluster"
	config["elasticsearch_settings"]["node_name"]			= "node2"
	#config["elasticsearch_settings"]["heap_size"]			= "1"
	config["elasticsearch_settings"]["hosts"]				= "192.168.2.31"
	config["elasticsearch_settings"]["master_node"]			= "false"
	config["elasticsearch_settings"]["data_node"]			= "true"
	
	config["openldap_settings"]["role"]						= "agent"
	config["openldap_settings"]["master_address"]			= "192.168.2.31"
	
	#config["cis_settings"]["install_path"]		 			= "/usr/local/CIS"
	#config["cis_settings"]["cis_port"] 					= "445"
	#config["cis_settings"]["pass_through_port"] 			= "442"
	#config["cis_settings"]["global_config_hosts"]			= "127.0.0.1:9200"
	config["cis_settings"]["reset_global_config"] 			= "false"
	
	config["ldap_settings"]["ldap_type"] 					= "OPENLDAP"
	config["ldap_settings"]["ldap_host"] 					= "127.0.0.1"
	config["ldap_settings"]["ldap_port"] 					= "389"
	config["ldap_settings"]["ldap_base_dn"] 				= "dc=example,dc=com"
	config["ldap_settings"]["ldap_username"] 				= "cisadmin"
	config["ldap_settings"]["ldap_password"] 				= "emc"
end
File.write(config_file, JSON.pretty_generate(config))
puts "#{Time.now}: Finish the config file updating before installation"