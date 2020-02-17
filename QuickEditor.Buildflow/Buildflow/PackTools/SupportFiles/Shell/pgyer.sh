# ==============================================
# This tool is for upload application to pgyer 
# ----------------------------------------------
# Author: Mr.Hu
# Data:   2020.02.01
# ==============================================
#!/bin/sh

#region #应用上传pgyer
#上传测试平台
#上传蒲公英的参数可通过查看蒲公英的API进行了解 http://www.pgyer.com/doc/api

function usage() {
  echo "USAGE:"
  echo "  pgyer.sh [-u] [-f <UPLOAD_PATH>] [-k <PGYER_UKEY>] [-a <PGYER_APIKEY>] [-t <PGYER_INSTALL_TYPE>] [-p <PGYER_PASSWORD>]"
  echo "Description:"
  echo "  UPLOAD_PGYER -> 安装包是否需要上传"
  echo "  UPLOAD_PATH 需要上传的ipa或者apk文件"
  echo "  #以下为上传蒲公英的参数 可通过查看蒲公英的API进行了解 http://www.pgyer.com/doc/api"
  echo "  PGYER_UKEY => 用户Key"
  echo "  PGYER_APIKEY -> API Key"
  echo "  PGYER_INSTALL_TYPE -> 应用安装方式，值为(1,2,3)。1：公开，2：密码安装，3：邀请安装。默认为1公开"
  echo "  PGYER_PASSWORD -> 设置App安装密码，如果不想设置密码，请传空字符串，或不传"
  
  exit -1
}

UPLOAD_PGYER=false
UPLOAD_PATH=""
PGYER_UKEY=""
PGYER_APIKEY=""
PGYER_INSTALL_TYPE=""
PGYER_PASSWORD=""

#getopts命令格式: getopts OPTSTRING VARNAME
#OPTSTRING: 告诉getopts会有哪些选项和参数(用选项后面加:来表示选项后面需要加参数)
#VARNAME: 保存getopts获取到的选项
while getopts ":uk:a:f:t:p:" arg #OPTSTRING如果以:开头表示忽略错误,选项后面的冒号表示该选项需要参数
do
  case $arg in
    u) UPLOAD_PGYER=true
    ;;
    f) UPLOAD_PATH="$OPTARG"
    ;;
    k) PGYER_UKEY="$OPTARG"
    ;;
    a) PGYER_APIKEY="$OPTARG"
    ;;
    t) PGYER_INSTALL_TYPE="$OPTARG"
    ;;
    p) PGYER_PASSWORD="$OPTARG"
    ;;
    \?)
    echo "Invalid Option: -$OPTARG"
    usage
    ;;
    esac
done

autoUpload(){
  echo '--> 发布应用到 pgyer平台'
  echo "--> Set PGYER_UKEY : ${PGYER_UKEY}" 
  echo "--> Set PGYER_APIKEY : ${PGYER_APIKEY}" 
  echo "--> Set PGYER_INSTALL_TYPE : ${PGYER_INSTALL_TYPE}" 
  echo "--> Set PGYER_PASSWORD : ${PGYER_PASSWORD}" 
  echo "--> Set UPLOAD_PATH : ${UPLOAD_PATH}" 

  #开始上传
  curl -F "file=@$UPLOAD_PATH" \
  -F "uKey=$PGYER_UKEY" \
  -F "_api_key=$PGYER_APIKEY" \
  -F "password=$PGYER_PASSWORD" \
  -F "installType=$PGYER_INSTALL_TYPE" \
  https://www.pgyer.com/apiv1/app/upload --verbose

  #判断上次命令是否执行成功 成功返回0 失败返回非0
  if [ $? = 0 ];then
    echo "--> 安装包 ${UPLOAD_PATH} 发布到pgyer成功"
  else
    echo "--> Error : 安装包 ${UPLOAD_PATH} 发布到pgyer失败 "
    exit 0
  fi
}

if  [[ $UPLOAD_PGYER = "true" ]]; then
  autoUpload
fi
#endregion
