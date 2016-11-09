#!/bin/bash
cis_dir=/usr/local/CIS
report_dir=$cis_dir/test/automation/Reports
cis_test_dir1=$cis_dir/test/automation/UnitTest
cis_test_dir2=$cis_dir/test/automation/FunctionalTest
saber_agent_dir=/home/administrator/download/saberAgent
saber_agent_report_dir=$saber_agent_dir/report
saber_agent_log_dir=$saber_agent_dir/logs/execution_log

saber_agent_elasticsearch_log_dir=$saber_agent_dir/logs/elasticsearch_log
saber_agent_nginx_log_dir=$saber_agent_dir/logs/nginx_log
saber_agent_cis_log_dir=$saber_agent_dir/logs/cis_log
nginx_dir=/usr/local/nginx
elasticsearch_log_dir=/var/log/


# stop nginx
echo stopping nginx
sudo killall nginx
# clean up log
rm -rf $nginx_dir/logs/*

#start nginx
$nginx_dir/sbin/nginx

###########
# Prepare

###########
#create report folder for galaxy parsing
mkdir -p $saber_agent_report_dir
mkdir -p $saber_agent_log_dir
mkdir -p $saber_agent_elasticsearch_log_dir
mkdir -p $saber_agent_nginx_log_dir
mkdir -p $saber_agent_cis_log_dir

# extract cis package
cd $saber_agent_dir
cis_package_file=`ls cis*.tgz -rt | tail -n 1`
extract_folder_name=`basename $cis_package_file .tgz`
mkdir $extract_folder_name
tar -zxf $cis_package_file -C $extract_folder_name

# modify config.json for silent installation
sed -i "s#10.37.15.71#192.168.2.10#" $extract_folder_name/install/setup/install_config.json
#sed -i "s#dc=domain#dc=domain#" $extract_folder_name/setup/install_config.json
#sed -i "s#administrator@sourceone.com#cisa#" $extract_folder_name/install/setup/install_config.json
sed -i "s#emc#emcsiax@QA#" $extract_folder_name/install/setup/install_config.json
sed -i "s#CISCluster#cluster#" $extract_folder_name/install/setup/install_config.json
sed -i "s#CISNode#node#" $extract_folder_name/install/setup/install_config.json

# turn off authentication in cis_constants.rb
#sed -i "s#false#true#" $extract_folder_name/CIS/Common/cis_constants.rb
#sed -i "s#B_IGNORE_TOKEN 		= false#B_IGNORE_TOKEN 		= true#" $cis_dir/cis_constants.rb
#sed -i "s#B_IGNORE_ROLES 		= false#B_IGNORE_ROLES 		= true#" $cis_dir/cis_constants.rb

# extract automation package
automation_package_file=`ls automation*.tgz -rt | tail -n 1`
puppet_unicorn=$extract_folder_name/CIS
if [ ! -d $puppet_unicorn/test ]; then
	mkdir $puppet_unicorn/test
fi
tar -zxf $automation_package_file -C $puppet_unicorn/test
#sed -i "s#emc#emcsiax@QA#" $puppet_unicorn/test/automation/common/config/environments.yml




# stop unicorn
service unicorn stop

# remove all indexes
cd $saber_agent_dir
ruby clean_es.rb

# stop elasticsearch
service elasticsearch stop

# clean elastcisearch log
rm -rf $elasticsearch_log_dir/elasticsearch/*
rm -rf $elasticsearch_log_dir/elasticsearch.log

# remove elasticsearch and its backup data
rm -rf /usr/share/elasticsearch/logs/*
rm -rf /usr/share/elasticsearch/data/cis_es
mkdir -p /usr/share/elasticsearch/data/cis_es
chown elasticsearch:elasticsearch /usr/share/elasticsearch/data/cis_es
rm -rf /home/bkrepository
mkdir /home/bkrepository
chmod 777 /home/bkrepository

# remove /usr/local/puppet_unicorn
rm -rf /usr/local/puppet_unicorn

# remove /usr/local/cst
rm -rf /usr/local/cst

# install cis
cd $saber_agent_dir/$extract_folder_name/install/setup
sh cis_install.sh  --file=install_config.json

# Update elasticsearch yml
sed -i "1i\path.repo: [\"/home/bkrepository\"]" /etc/elasticsearch/elasticsearch.yml

# restart elasticsearch
service elasticsearch restart
sleep 20



###########
# Execute
###########
cd $cis_test_dir1
bash run.sh
cp $report_dir/* $saber_agent_report_dir

cd $cis_test_dir2
bash run.sh
cp $report_dir/* $saber_agent_report_dir

cp $saber_agent_dir/cis_execution.log $saber_agent_log_dir

# stop nginx
sudo killall nginx
# stop unicorn
service unicorn stop

# stop elasticsearch
service elasticsearch stop
sudo cp $nginx_dir/logs/* $saber_agent_nginx_log_dir
sudo cp $elasticsearch_log_dir/elasticsearch.log $saber_agent_elasticsearch_log_dir
sudo cp $elasticsearch_log_dir/elasticsearch/* $saber_agent_elasticsearch_log_dir
sudo cp -r $cis_dir/log/* $saber_agent_cis_log_dir
#start nginx
$nginx_dir/sbin/nginx
# start unicorn
service unicorn start

# start elasticsearch
service elasticsearch start
###########
# Cleanup
###########
cd $saber_agent_dir


