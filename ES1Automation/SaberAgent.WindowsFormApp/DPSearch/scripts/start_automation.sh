if [[ $# -lt 2 ]]; then
 echo "usage: bash start_automation.sh branch_name ui|api"
 echo "e.g,   bash start_automation.sh DPSEARCH_DEVELOP ui"
 exit
fi

dpsearchfn=`ls -t . | egrep "dpsearch-[0-9].*.tgz" | head -n 1`
if [[ "$dpsearchfn" != "" ]]; then
  echo "got dpsearch package: ${dpsearchfn}"
else
  echo "there is no dpsearch package, exit"
  exit
fi

afn=`ls -t . | egrep "automation-[0-9].*.tgz" | head -n 1`

if [[ "$afn" != "" ]]; then
  echo "got automation package:${afn}"
  base_dir="dpsearch_automation"
  [[ -d $base_dir ]] || mkdir $base_dir
  tar -xvf $afn -C $base_dir
  cd $base_dir/scripts
  ruby deploy_dpsearch.rb $1 $2
else
  echo "there is no automation package, exit"
  exit
fi
