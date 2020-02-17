# ==============================================
# This tool is for upload application to fir 
# ----------------------------------------------
# Author: Mr.Hu
# Data:   2020.02.01
# ==============================================
#!/bin/sh

#region #应用上传fir
#上传测试平台

UPLOAD_FIR=false
UPLOAD_PATH=""
FIR_TOKEN_ID=""
FIR_SHORT_NAME=""

function usage() {
  echo "USAGE:"
  echo "  fir.sh [-u] [-t <FIR_TOKEN_ID>] [-n <FIR_SHORT_NAME>] [-f <UPLOAD_PATH>]"
  echo "Description:"
  echo "  UPLOAD_FIR 安装包是否需要上传"
  echo "  FIR_TOKEN_ID fir.im token 官网API TOKEN 生成"
  echo "  FIR_SHORT_NAME fir.im 短链接格式（3-16 个字符）"
  echo "  UPLOAD_PATH 需要上传的ipa或者apk文件"
  exit -1
}

while getopts ":ut:n:p:" arg
do
  case $arg in
    u) UPLOAD_FIR=true
    ;;
    f) UPLOAD_PATH="$OPTARG"
    ;;
    t) FIR_TOKEN_ID="$OPTARG"
    ;;
    n) FIR_SHORT_NAME="$OPTARG"
    ;;   
    \?)
    echo "Invalid Option: -$OPTARG"
    usage
    ;;
    esac
done

autoUpload(){
  echo '--> 发布应用到 fir.im平台'
  echo "--> Set FIR_TOKEN_ID : ${FIR_TOKEN_ID}" 
  echo "--> Set FIR_SHORT_NAME : ${FIR_SHORT_NAME}" 
  echo "--> Set UPLOAD_PATH : ${UPLOAD_PATH}" 
  
  # 需要先在本地安装 fir 插件,安装fir插件命令: gem install fir-cli
  fir login -T ${FIR_TOKEN_ID}  # fir.im token
  fir publish  ${UPLOAD_PATH} --short=${FIR_SHORT_NAME}

  #判断上次命令是否执行成功 成功返回0 失败返回非0
  if [ $? = 0 ];then
    echo "--> 安装包 ${UPLOAD_PATH} 发布到fir成功"
  else
    echo "--> Error : 安装包 ${UPLOAD_PATH} 发布到fir失败 "
    exit 0
  fi
}

if  [[ $UPLOAD_FIR = "true" ]]; then
  autoUpload
fi
#endregion
