#!/bin/sh

export UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
export UNITY_PROJ_GIT_PATH="/Users/mac/Documents/UnityProjects/develop_android/client"
export UNITY_PROJ_NAME="QuickEditor.Buildflow"
export UNITY_PROJ_PATH="${UNITY_PROJ_GIT_PATH}/${UNITY_PROJ_NAME}"
export UNITY_PROJ_BUILDFLOW_PATH="${UNITY_PROJ_PATH}/Buildflow"
export UNITY_PROJ_OUTPUT_PATH="${UNITY_PROJ_BUILDFLOW_PATH}/BuildOutput"
export UNITY_PROJ_PACK_TOOLS_PATH="${UNITY_PROJ_BUILDFLOW_PATH}/PackTools"
export UNITY_PROJ_PACK_SHELL_TOOLS_PATH="${UNITY_PROJ_PACK_TOOLS_PATH}/SupportFiles/Shell"

echo "--> 获取并设置插件参数" 
#插件配置的参数有:
#VersionControl      版本管理 选项{Git,SVN}
#GitBranch           Git分支
#BuildTarget         项目的打包平台 选项{Android,iOS,StandaloneWindows}
#BuildType           打包环境 选项{Release,Develop,Beta}
#BuildAction         Unity打包行为 选项{Refresh,SetScriptingDefineSymbols,BuildBuildle,BuildPackage,BuildWholePackage}
#AppVersion          应用版本信息 
#VersionCode         应用迭代版本号,每次更新应用商店的包,这数值都增加
#BundlesVersion      资源包版本

#UnityChannel        渠道SDK标识
#UnityServer         服务器标识

#UploadBundles       是否上传资源包
#UploadPackage       是否上传应用包
#UploadFir           是否上传应用包到第三方托管平台

#接收参数
JP_VERSION_CONTROL=$VersionControl
JP_Git_Branch=$GitBranch
JP_BUILD_TARGET=$BuildTarget
JP_BUILD_TYPE=$BuildType
JP_BUILD_ACTION=$BuildAction

JP_APP_VERSION=$AppVersion
JP_VERSION_CODE=$VersionCode
JP_BUNDLES_VERSION=$BundlesVersion

JP_UNITY_CHANNEL=$UnityChannel
JP_UNITY_SERVER=$UnityServer

JP_EXPORT_METHOD=$ExportMethod

JP_UPLOAD_BUNDLES=$UploadBundles
JP_UPLOAD_PACKAGE=$UploadPackage

#JP_FIR_TOKEN=$FirToken
JP_UPLOAD_FIR=$UploadFir

echo "--> 检测打包平台参数" 
SUPPORT_TARGETS=("Android" "iOS" "StandaloneWindows")
if echo "${SUPPORT_TARGETS[@]}" | grep -w $JP_BUILD_TARGET &>/dev/null; then
	echo "--> 当前打包平台参数: ${JP_BUILD_TARGET}" 
else
	echo "Error : 不支持的打包平台参数 : ${JP_BUILD_TARGET}"
	exit 1
fi

echo "--> Set UNITY_PROJ_GIT_PATH : ${UNITY_PROJ_GIT_PATH}" 
echo "--> Set UNITY_PROJ_PATH : ${UNITY_PROJ_PATH}" 
echo "--> Set UNITY_PROJ_OUTPUT_PATH : ${UNITY_PROJ_OUTPUT_PATH}" 
echo "--> Set UNITY_PROJ_PACK_SHELL_TOOLS_PATH : ${UNITY_PROJ_PACK_SHELL_TOOLS_PATH}" 

if [[ $JP_VERSION_CONTROL = "Git" ]]; then
	bash ${UNITY_PROJ_PACK_SHELL_TOOLS_PATH}/Module/git.sh -p ${UNITY_PROJ_GIT_PATH} -b ${JP_Git_Branch}
	if [ $? -ne 0 ];then
		echo "Error : Git Operation failed"
		exit 1
	fi
elif  [[ $JP_VERSION_CONTROL = "SVN" ]]; then
#todo
fi

UNITY_PROJ_BUILD_TIME="`date +%Y%m%d_%H%M`"
UNITY_PROJ_OUTPUT_PLATFORM_PATH="${UNITY_PROJ_OUTPUT_PATH}/${JP_BUILD_TARGET}"
UNITY_PROJ_LOG_PATH="${UNITY_PROJ_OUTPUT_PLATFORM_PATH}/build_${UNITY_PROJ_BUILD_TIME}.log"

#如果日志文件已存在 删除日志文件
if [[ -f $UNITY_PROJ_LOG_PATH ]]; then
  #rm -f $UNITY_PROJ_LOG_PATH
  echo -n "" > $UNITY_PROJ_LOG_PATH
  echo "--> 日志文件清除成功"
fi

#监听日志文件 输出日志到Jenkins控制台
touch ${UNITY_PROJ_LOG_PATH}
tail -f ${UNITY_PROJ_LOG_PATH} &

JP_BUILD_BUNDLES=false
JP_BUILD_PACKAGE=false
if [[ $JP_BUILD_ACTION = "BuildBuildle" || $JP_BUILD_ACTION = "BuildWholePackage" ]]; then
	JP_BUILD_BUNDLES=true
fi
if [[ $JP_BUILD_ACTION = "BuildPackage" || $JP_BUILD_ACTION = "BuildWholePackage" ]]; then
	JP_BUILD_PACKAGE=true
fi

#region #判断是否需要刷新工程
if [[ $JP_BUILD_BUNDLES = "true" || $JP_BUILD_PACKAGE = "true" ]]; then
	bash $UNITY_PROJ_PACK_SHELL_TOOLS_PATH/autoUnityBuildRefresh.sh ${JP_BUILD_TARGET}
fi
#endregion

#region #判断是否需要打包资源
if [[ $JP_BUILD_BUNDLES = "true" ]]; then
	echo "--> 设置Unity的资源打包参数"
	bash $UNITY_PROJ_PACK_SHELL_TOOLS_PATH/autoUnityBuildBundles.sh ${JP_BUILD_TARGET} \
	${JP_BUNDLES_VERSION} ${JP_UNITY_SERVER} ${JP_UNITY_CHANNEL} ${JP_BUILD_TYPE} 
	echo "--> 资源打包完成"
fi
#endregion

#Unity打包导出目录
UNITY_PROJ_BUILD_PATH=${UNITY_PROJ_OUTPUT_PLATFORM_PATH}
UNITY_XCODE_PROJ_BUILD_PATH=${UNITY_PROJ_OUTPUT_PLATFORM_PATH}/XCodeProjects

echo "--> Set UNITY_PROJ_BUILD_PATH : ${UNITY_PROJ_BUILD_PATH}"
echo "--> Set UNITY_XCODE_PROJ_BUILD_PATH : ${UNITY_XCODE_PROJ_BUILD_PATH}"

if [ ! -d $UNITY_PROJ_BUILD_PATH  ]; then
	mkdir $UNITY_PROJ_BUILD_PATH
fi

#region #判断是否需要打安装包
if  [[ $JP_BUILD_PACKAGE = "true" ]]; then
	echo "--> 设置Unity的打包参数" 
	EXECUTABLE_NAME=="${UNITY_PROJ_NAME}_${JP_BUILD_TARGET}_${JP_UNITY_CHANNEL}_${JP_UNITY_SERVER}_${UNITY_PROJ_BUILD_TIME}"

	if [[ $JP_BUILD_TARGET = "Android" ]]; then
		UNITY_PROJ_BUILD_EXECUTABLE_NAME="${EXECUTABLE_NAME}.apk"
	elif [[ $JP_BUILD_TARGET = "iOS" ]]; then
		UNITY_PROJ_BUILD_EXECUTABLE_NAME="${EXECUTABLE_NAME}"
	elif [[ $JP_BUILD_TARGET = "PC" ]]; then
		UNITY_PROJ_BUILD_EXECUTABLE_NAME="${EXECUTABLE_NAME}/Setup.exe"
	fi
	
	echo "--> Set UNITY_PROJ_BUILD_EXECUTABLE_NAME : ${UNITY_PROJ_BUILD_EXECUTABLE_NAME}"

	#本地如果已有相同目录的Xcode工程 先删除重新生成
	if [[ $JP_BUILD_TARGET = "iOS" ]]; then
		if [ -d "$UNITY_PROJ_BUILD_PATH/$UNITY_PROJ_BUILD_EXECUTABLE_NAME" ] ; then
			rm -rf $UNITY_PROJ_BUILD_PATH/$PROJ_BUILDER_FILE_NAME
			echo "--> Xcode Project: ${UNITY_PROJ_BUILD_EXECUTABLE_NAME} 删除成功 "
		fi
	fi

	if [[ $JP_BUILD_TARGET = "iOS" ]]; then
		bash $UNITY_PROJ_PACK_SHELL_TOOLS_PATH/autoUnityBuild_Xcode.sh ${JP_BUILD_TARGET} \
		${UNITY_PROJ_BUILD_PATH} ${UNITY_PROJ_BUILD_EXECUTABLE_NAME} \
		${JP_APP_VERSION} ${JP_VERSION_CODE} ${JP_UNITY_CHANNEL} ${JP_UNITY_SERVER} ${JP_BUILD_TYPE}
	else
		bash $UNITY_PROJ_PACK_SHELL_TOOLS_PATH/autoUnityBuildPackage.sh ${JP_BUILD_TARGET} \
		${UNITY_PROJ_BUILD_PATH} ${UNITY_PROJ_BUILD_EXECUTABLE_NAME} \
		${JP_APP_VERSION} ${JP_VERSION_CODE} ${JP_UNITY_CHANNEL} ${JP_UNITY_SERVER} ${JP_BUILD_TYPE}
	fi
	
	if [ $? -ne 0 ] ; then
		echo '--> Unity项目导出失败'
		exit 1
    else
    	echo '--> Unity项目导出成功'
    fi

    if [[ $JP_BUILD_TARGET = "iOS" ]]; then
    	UNITY_XCODE_CONFIGURATION=Release
    	UNITY_XCODE_SCHEME="Unity-iPhone"

    	bash $UNITY_PROJ_PACK_SHELL_TOOLS_PATH/autoXCodeBuildIPA.sh ${UNITY_XCODE_PROJ_BUILD_PATH} \
		${UNITY_PROJ_BUILD_PATH} ${UNITY_PROJ_BUILD_EXECUTABLE_NAME} \
		${UNITY_XCODE_CONFIGURATION} ${UNITY_XCODE_SCHEME}

		if [ $? -ne 0 ] ; then
			echo '--> IPA导出失败'
			exit 1
		else
			echo '--> IPA导出成功'
		fi
    fi

fi
#endregion

echo "--> " 
echo "--> ALL Scripts Execute Done" 
echo "--> " 
