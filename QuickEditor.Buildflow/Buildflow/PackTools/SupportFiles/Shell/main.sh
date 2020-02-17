#!/bin/sh

echo "--> 获取并设置插件参数" 
JP_BUILD_TARGET="Android"

echo "--> 检测打包平台参数" 
#SUPPORT_TARGETS=("Android" "iOS" "PC")
#if echo "${SUPPORT_TARGETS[@]}" | grep -w $JP_BUILD_TARGET &>/dev/null; then
#  echo "--> 当前打包平台参数: ${JP_BUILD_TARGET}" 
#else
#  echo "Error : 不支持的打包平台参数 : ${JP_BUILD_TARGET}"
#  exit 1
#fi

echo "--> 设置项目工程参数" 
UNITY_PATH="D:\Program Files\Unity\2018.2.21f1\Editor\Unity.exe"
PROJ_GIT_PATH="D:\GitProjects\QuickUSDK"
PROJ_PATH="${PROJ_GIT_PATH}\UnityProjects\QuickUSDK"

echo "--> 设置Unity的打包参数" 
echo 刷新Unity工程
UNITY3D_REFRESH_METHOD="QuickEditor.Buildflow.BatchModeCommands.Refresh"
$UNITY_PATH -quit -batchmode -nographics -projectPath $PROJ_PATH -executeMethod ${UNITY3D_REFRESH_METHOD} 

#echo BuildPackage
#UNITY3D_BUILD_METHOD="QuickEditor.Buildflow.BatchModeCommands.BuildPackage"
#PROJ_BUILDER_FILE_NAME="QuickEditor.Buildflow_Android.apk"
#echo "--> Set UNITY3D_BUILD_METHOD : ${UNITY3D_BUILD_METHOD}" 
#$UNITY_PATH -quit -batchmode -nographics -projectPath $PROJ_PATH -executeMethod ${UNITY3D_BUILD_METHOD} Build_Target-$JP_BUILD_TARGET Build_FileName-#$PROJ_BUILDER_FILE_NAME