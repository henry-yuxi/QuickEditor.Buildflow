# ==============================================
# This tool is for Create iOS application
# ----------------------------------------------
# Author: Mr.Hu
# Data:   2020.02.01
# ==============================================
#!/bin/sh

#Xcode打包处理

#Xcode工程导出IPA相关参数
J_UNITY_XCODE_PROJ_BUILD_PATH=$1
J_UNITY_IPA_PROJ_BUILD_PATH=$2
J_UNITY_PROJ_BUILD_EXECUTABLE_NAME=$3
J_UNITY_XCODE_CONFIGURATION=$4
J_UNITY_XCODE_SCHEME=$5

if [ ! -d $J_UNITY_IPA_PROJ_BUILD_PATH ];then
	mkdir $` 
fi

UNITY_XCODE_PROJ_PATH="${J_UNITY_XCODE_PROJ_BUILD_PATH}/${J_UNITY_PROJ_BUILD_EXECUTABLE_NAME}/${J_UNITY_XCODE_SCHEME}.xcodeproj"
UNITY_XCODE_ARCHIVE_PATH="${J_UNITY_IPA_PROJ_BUILD_PATH}/${J_UNITY_PROJ_BUILD_EXECUTABLE_NAME}/${J_UNITY_XCODE_SCHEME}.xcarchive"

#Xcode打包必需参数
IPA_COMMAND_SETTINGS=Settings/ExportOptions/iOSPackSettings.ini
if [ ! -f "$IPA_COMMAND_SETTINGS" ];then
	echo "--> Error : IPA导出配置 -> ${IPA_COMMAND_SETTINGS} not found"
	exit_code=-1
fi

exportOptionsPlistPath=Settings/ExportOptions/exportOptions_${JP_EXPORT_METHOD}.plist

BUNDLE_IDENTIFIER_KEY=BUNDLE_IDENTIFIER
CODE_SIGN_IDENTITY_KEY=CODE_SIGN_IDENTITY
PROVISIONING_PROFILE_NAME_KEY=PROVISIONING_PROFILE_NAME
    
BUNDLE_IDENTIFIER=$(awk -F '=' '/\['${JP_EXPORT_METHOD}'\]/{a=1} (a==1 && "'${BUNDLE_IDENTIFIER_KEY}'"==$1){a=0;print $2}' ${IPA_COMMAND_SETTINGS}) 
CODE_SIGN_IDENTITY=$(awk -F '=' '/\['${JP_EXPORT_METHOD}'\]/{a=1} (a==1 && "'${CODE_SIGN_IDENTITY_KEY}'"==$1){a=0;print $2}' ${IPA_COMMAND_SETTINGS}) 
PROVISIONING_PROFILE_NAME=$(awk -F '=' '/\['${JP_EXPORT_METHOD}'\]/{a=1} (a==1 && "'${PROVISIONING_PROFILE_NAME_KEY}'"==$1){a=0;print $2}' ${IPA_COMMAND_SETTINGS}) 

#CODE_SIGN_IDENTITY="iPhone Distribution: Shanghai Blademaster Network Technology Co., Ltd."
#PROVISIONING_PROFILE_NAME="ylqt_inhouse"
#BUNDLE_IDENTIFIER=com.baiyao.ylqt

echo "--> Set CODE_SIGN_IDENTITY : ${CODE_SIGN_IDENTITY}" 
echo "--> Set PROVISIONING_PROFILE_NAME : ${PROVISIONING_PROFILE_NAME}"
echo "--> Set BUNDLE_IDENTIFIER : ${BUNDLE_IDENTIFIER}" 

#(K8EN9Z764W)
# 对于ssh连上mac的终端,签名的时候会提示：User interaction is not allowed.
# 所以要先对证书解锁 直接在终端执行可以不必要
#UNLOCK=`security show-keychain-info ~/Library/Keychains/login.keychain 2>&1|grep no-timeout`
#if [ -z "$UNLOCK" ]; then
	# -p 后面跟的是密码,各机器可能不一样,要修改
	#security unlock-keychain -p 123u123u ~/Library/Keychains/login.keychain
	# 修改unlock-keychain过期时间,最好大于一次打包时间
	#security set-keychain-settings -t 3600000 -l ~/Library/Keychains/login.keychain
#fi
  
echo "--> Delete Auto Manage Signing"
python Settings/ExportOptions/DeleteAutoManageSigning.py ${J_UNITY_XCODE_PROJ_BUILD_PATH}/${J_UNITY_PROJ_BUILD_EXECUTABLE_NAME}/

echo '--> 正在清理工程'
xcodebuild clean \
-project ${UNITY_XCODE_PROJ_PATH} \
-scheme ${J_UNITY_XCODE_SCHEME} \
-configuration ${J_UNITY_XCODE_CONFIGURATION} \
>>${UNITY_PROJ_LOG_PATH}

echo '--> 工程清理完成-->>>--正在编译工程:'${J_UNITY_XCODE_CONFIGURATION}
xcodebuild archive \
-project ${UNITY_XCODE_PROJ_PATH} \
-scheme ${J_UNITY_XCODE_SCHEME} \
-configuration ${J_UNITY_XCODE_CONFIGURATION} \
-archivePath ${UNITY_XCODE_ARCHIVE_PATH} \
CODE_SIGN_IDENTITY="${CODE_SIGN_IDENTITY}" \
PROVISIONING_PROFILE_SPECIFIER="${PROVISIONING_PROFILE_NAME}" \
>>${UNITY_PROJ_LOG_PATH}

if [ $? -ne 0 ] ; then
	echo '--> 项目编译失败'
	exit_code=1
else
	echo '--> 项目编译成功'
	exit_code=0
fi

echo '--> 项目编译完成-->>>--开始IPA打包'
xcodebuild -exportArchive \
-archivePath ${UNITY_XCODE_ARCHIVE_PATH} \
-configuration ${J_UNITY_XCODE_CONFIGURATION} \
-exportPath ${J_UNITY_IPA_PROJ_BUILD_PATH}/${J_UNITY_PROJ_BUILD_EXECUTABLE_NAME} \
-exportOptionsPlist "$exportOptionsPlistPath" \
>>${UNITY_PROJ_LOG_PATH}

if [ $? -ne 0 ] ; then
	echo '--> 项目构建失败'
	exit_code=1
else
	echo '--> 项目构建成功'
	exit_code=0
fi

exit $exit_code