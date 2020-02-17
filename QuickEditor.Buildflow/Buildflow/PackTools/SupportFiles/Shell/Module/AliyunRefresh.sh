# ==============================================
# This tool is for Refresh Aliyun CDN Dir
# ----------------------------------------------
# Author: Mr.Hu
# Data:   2020.02.01
# ==============================================
#!/bin/sh

Git_Branch=""
RefreshDir=""

function usage() {
  echo "USAGE:"
  echo "  AliyunRefresh.sh [-b <Git_Branch>] [-p <Git_Workspace>] [-d <RefreshDir>]"
  echo "Description:"
  echo "  Git_Branch git分支"
  echo "  Git_Branch git分支"
  echo "  RefreshDir 需要刷新的目录"
  exit -1
}

while getopts ":u:p:d:" arg
do
  case $arg in
    u) Git_Branch="$OPTARG"
    ;;
    p) Git_Workspace="$OPTARG"
    ;;
    d) RefreshDir="$OPTARG"
    ;;
    \?)
    echo "Invalid Option: -$OPTARG"
    usage
    ;;
    esac
done

echo "--> Start Refresh Aliyun CDN Dir" 
python CDN_API/QcloudCdnTools_V2.py RefreshCdnDir \
-u AKIDCCzXq6L0f5GG1XNrlcP3ShgPs52koNIx \
-p ZtxlIfnfHdjY7QTFl4A2e2B4g27wf8LI \
--dirs $RefreshDir

if [ $? = 0 ];then
  echo "--> Aliyun CDN refresh completed successfully"
else
  echo "--> Error : Aliyun CDN refresh failed"
fi