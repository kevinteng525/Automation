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

###########
# Prepare

###########
service unicorn stop
#curl -XDELETE "http://localhost:9200/_snapshot/app2_repo_of_app2"
curl -XDELETE http://localhost:9200/_all
service elasticsearch stop
#create report folder for galaxy parsing
mkdir -p $saber_agent_report_dir
mkdir -p $saber_agent_log_dir
mkdir -p $saber_agent_elasticsearch_log_dir
mkdir -p $saber_agent_nginx_log_dir
mkdir -p $saber_agent_cis_log_dir


# extract second cis package
cd $saber_agent_dir
cis_package_file=`ls cis*.tgz -rt | tail -n 1`
extract_folder_name=`basename $cis_package_file .tgz`
mkdir $extract_folder_name
tar -zxf $cis_package_file -C $extract_folder_name

# modify config.json for silent installation
sed -i "s#10.37.15.71#192.168.2.10#" $extract_folder_name/install/setup/install_config.json
sed -i "s#emc#emcsiax@QA#" $extract_folder_name/install/setup/install_config.json
sed -i "s#CISCluster#localcluster#" $extract_folder_name/install/setup/install_config.json
sed -i "s#CISNode#localnode#" $extract_folder_name/install/setup/install_config.json
sed -i '3c "install_puppet"       : "false",' $extract_folder_name/install/setup/install_config.json
sed -i '18c "reset_global_config" : "true",' $extract_folder_name/install/setup/install_config.json


# modify the globalConfig.json to reflect the environment
# specify the local cluster name
sed -i '8c "localClusters": ["localcluster"],' $extract_folder_name/CIS/Common/CISGlobalConfig.json
# remove the connection information
sed -i '2,7d' $extract_folder_name/CIS/Common/CISGlobalConfig.json
# specify the tribe node information
sed -i '1a "tribe_nodes": [{"ipaddr":"192.168.2.30","port":"9200"}],' $extract_folder_name/CIS/Common/CISGlobalConfig.json


# extract automation package
automation_package_file=`ls automation*.tgz -rt | tail -n 1`
puppet_unicorn=$extract_folder_name/CIS
if [ ! -d $puppet_unicorn/test ]; then
	mkdir $puppet_unicorn/test
fi
tar -zxf $automation_package_file -C $puppet_unicorn/test

# stop unicorn
service unicorn stop



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
rm -rf /home/bkrepository/*




# remove /usr/local/cst
rm -rf /usr/local/cst

rm -rf $report_dir
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




