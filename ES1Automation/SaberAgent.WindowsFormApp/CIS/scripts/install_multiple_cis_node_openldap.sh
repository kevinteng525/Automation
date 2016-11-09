#!/bin/bash

function wait_untill_es_is_ready {
  until curl http://$1:9200 | grep "You Know, for Search"
  do
    echo "ES is not ready yet"
    echo "Wait 5 seconds"
    sleep 5
  done
  echo "ES is up and ready"
}

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
curl -XDELETE "http://localhost:9200/_snapshot/app2_repo_of_app2"
curl -XDELETE http://localhost:9200/_all
service elasticsearch stop
#create report folder for galaxy parsing
mkdir -p $saber_agent_report_dir
mkdir -p $saber_agent_log_dir
mkdir -p $saber_agent_elasticsearch_log_dir
mkdir -p $saber_agent_nginx_log_dir
mkdir -p $saber_agent_cis_log_dir


# Clean the remote folder
ssh root@192.168.2.31 "rm -rf $saber_agent_dir"
ssh root@192.168.2.31 "mkdir -p $saber_agent_dir"
# remove first elasticsearch and its backup data
ssh root@192.168.2.31 "curl -XDELETE http://localhost:9200/_all"
ssh root@192.168.2.31 'service elasticsearch stop'
ssh root@192.168.2.31 'rm -rf /usr/share/elasticsearch/logs/*'
ssh root@192.168.2.31 'rm -rf /usr/share/elasticsearch/data/cis_es'
ssh root@192.168.2.31 'mkdir -p /usr/share/elasticsearch/data/cis_es'
ssh root@192.168.2.31 'chown elasticsearch:elasticsearch /usr/share/elasticsearch/data/cis_es'
ssh root@192.168.2.31 'rm -rf /home/bkrepository/*'
ssh root@192.168.2.31 "sed -i '/path\.repo/d' /etc/elasticsearch/elasticsearch.yml"
ssh root@192.168.2.31 'service elasticsearch restart'

wait_untill_es_is_ready 192.168.2.31

# extract the remote cis package
cd $saber_agent_dir
cis_package_file=`ls cis*.tgz -rt | tail -n 1`
scp $saber_agent_dir/$cis_package_file root@192.168.2.31:$saber_agent_dir
extract_folder_name=`basename $cis_package_file .tgz`
ssh root@192.168.2.31 "mkdir -p $saber_agent_dir/$extract_folder_name"

ssh root@192.168.2.31 "tar -zxf $saber_agent_dir/$cis_package_file -C $saber_agent_dir/$extract_folder_name"

# Modify the remote config.json for silent installation
echo Modify the remote config.json for silent installation
scp "$saber_agent_dir/update_install_config.rb" root@192.168.2.31:$saber_agent_dir
ssh root@192.168.2.31 "cd $saber_agent_dir/$extract_folder_name/install/setup; ruby  $saber_agent_dir/update_install_config.rb master"

# Stop unicorn
ssh root@192.168.2.31 'pkill unicorn'
ssh root@192.168.2.31 'rm /etc/init.d/unicorn'
ssh root@192.168.2.31 'rm /etc/init.d/cis-core'

# Install the CIS Master node
ssh root@192.168.2.31 "cd $saber_agent_dir/$extract_folder_name/install/setup; sh cis_install.sh  --file=install_config.json"

# Update Remote elasticsearch yml
ssh root@192.168.2.31 "sed -i '1i\path.repo: [\"/home/bkrepository\"]' /etc/elasticsearch/elasticsearch.yml"
ssh root@192.168.2.31 'service elasticsearch restart'

wait_untill_es_is_ready 192.168.2.31

# Add predefined users to OpenLDAP master server for automation
echo "Add predefined users to OpenLDAP master server for automation"
scp "$saber_agent_dir/ldapuser.ldif" root@192.168.2.31:$saber_agent_dir
ssh root@192.168.2.31 "cd $saber_agent_dir; ldapadd -x -D cn=Manager,dc=example,dc=com -w secret -f ldapuser.ldif"
echo "Users have been added into the embedded OpanLDAP"

# extract second cis package
cd $saber_agent_dir
cis_package_file=`ls cis*.tgz -rt | tail -n 1`
extract_folder_name=`basename $cis_package_file .tgz`
mkdir $extract_folder_name
tar -zxf $cis_package_file -C $extract_folder_name

# Modify local config.json for silent installation
echo Modify local config.json for silent installation
cd $saber_agent_dir/$extract_folder_name/install/setup;
ruby  $saber_agent_dir/update_install_config.rb slave

# extract automation package
cd $saber_agent_dir
automation_package_file=`ls automation*.tgz -rt | tail -n 1`
puppet_unicorn=$extract_folder_name/CIS
if [ ! -d $puppet_unicorn/test ]; then
	mkdir $puppet_unicorn/test
fi
tar -zxf $automation_package_file -C $puppet_unicorn/test

# stop unicorn
#service unicorn stop
pkill unicorn
rm /etc/init.d/unicorn
rm /etc/init.d/cis-core

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
sed -i "/path\.repo/d" /etc/elasticsearch/elasticsearch.yml

# remove /usr/local/cst
rm -rf /usr/local/cst

rm -rf $report_dir
# install cis
service elasticsearch restart

wait_untill_es_is_ready 192.168.2.32

cd $saber_agent_dir/$extract_folder_name/install/setup
sh cis_install.sh  --file=install_config.json

# Update elasticsearch yml
sed -i "1i\path.repo: [\"/home/bkrepository\"]" /etc/elasticsearch/elasticsearch.yml

# restart elasticsearch
service elasticsearch restart

wait_untill_es_is_ready 192.168.2.32

###########
# Execute
###########
service unicorn stop
service cis-core stop
cp $saber_agent_dir/codecoverage.rb $cis_dir/RestAPI/
cd $cis_dir/RestAPI/
unicorn -c codecoverage.rb -D

# Add the AD into CIS and assign the ADMIN role to the administrator
cd $saber_agent_dir
ruby config_ldap_env.rb  

cd $cis_test_dir1
bash run.sh
cp $report_dir/* $saber_agent_report_dir

cd $cis_test_dir2
bash run.sh
cp $report_dir/* $saber_agent_report_dir

cp $saber_agent_dir/cis_execution.log $saber_agent_log_dir
ps aux|grep "unicorn master"|awk '! /grep/ {print $2}' |xargs kill -QUIT
sleep 30
cp -a $cis_dir/RestAPI/coverage/ $saber_agent_report_dir
# stop nginx
sudo killall nginx
# stop unicorn


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
service cis-core start

# Restore local elasticsearch yml
sed -i "/path\.repo/d" /etc/elasticsearch/elasticsearch.yml
# start elasticsearch
service elasticsearch restart

# Restore remote #elastcisearch.yml
ssh root@192.168.2.31 "sed -i '/path\.repo/d' /etc/elasticsearch/elasticsearch.yml"
ssh root@192.168.2.31 'service elasticsearch restart'
###########
# Cleanup
###########
cd $saber_agent_dir
