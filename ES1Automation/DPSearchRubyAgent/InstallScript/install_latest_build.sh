#Stop nginx process
sudo killall nginx

curr_dir=${PWD}
#Delete content index in elasticsearch
echo "start deleting content index"
ruby delete_index.rb
echo "end deleting content index"

#mount build location
if [ -d ${curr_dir}/build ];then
   echo "folder ${curr_dir}/build exist"
else
   mkdir -p ${curr_dir}/build
fi
sudo umount ${curr_dir}/build/
sudo mount -t cifs //10.98.22.30/releng/Builds/DPSearch/v100 -o username="$1" ${curr_dir}/build

#copy latest build
cd ${curr_dir}/build
folder=`ls -t|head -n 1`
cd $folder
#find ./ -name "*.tar.gz" -exec cp '{}' ${curr_dir} ';'
build=`find ./ -name "*.tar.gz"|head -n 1`
rm ${curr_dir}/DPSearch*.tar.gz
echo "copy $build to ${curr_dir}"
cp -f $build ${curr_dir}
cd ${curr_dir}
rm -rf ./dpsearch

#extract
echo "tar $build"
tar -zxvf $build
sudo chmod -R 777 ./dpsearch

#invoke dp_init.sh
cp ./auto_expect ./dpsearch/install/
cd ./dpsearch/install/
expect ./auto_expect

#Start nginx process
sudo /opt/nginx/sbin/nginx

#work around jsvc issue
sudo /sbin/service dpworker stop
sudo sed -i "s#300000#10000#" /usr/local/dpsearch/etc/dpworker.config
cd /usr/local/dpsearch
sudo java -cp .:lib/* -Dlog4j.configurationFile=/usr/local/dpsearch/etc/log4j2.xml com.emc.dps.search.worker.Worker
