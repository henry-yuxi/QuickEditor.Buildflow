#!/bin/bash

J_BUILD_TARGET=$1
J_BUILD_PATH=$2
J_EXECUTABLE_NAME=$3
J_BUILD_VERSION=$4
J_BUILD_NUMBER=$5
J_BUILD_CHANNEL=$6
J_BUILD_SERVER=$7
J_BUILD_TYPE=$8

autoBuild(){
	echo "--> 执行Unity的打包方法"
	echo "--> 打包中, 请耐心等待..."
	echo "--> Set J_BUILD_TARGET : ${J_BUILD_TARGET}"
	echo "--> Set J_BUILD_PATH : ${J_BUILD_PATH}"
	echo "--> Set J_EXECUTABLE_NAME : ${J_EXECUTABLE_NAME}"
	echo "--> Set J_BUILD_VERSION : ${J_BUILD_VERSION}"
	echo "--> Set J_BUILD_NUMBER : ${J_BUILD_NUMBER}"
	echo "--> Set J_BUILD_CHANNEL : ${J_BUILD_CHANNEL}"
	echo "--> Set J_BUILD_SERVER : ${J_BUILD_SERVER}"
	echo "--> Set J_BUILD_TYPE : ${J_BUILD_TYPE}"
	echo "--> Set UNITY_EXECUTE_METHOD : ${UNITY_EXECUTE_METHOD}"
	$UNITY_PATH -quit -batchmode -nographics -projectPath $UNITY_PROJ_PATH \
	-executeMethod ${UNITY_EXECUTE_METHOD} \
	buildTarget-$JP_BUILD_TARGET \
	buildPath-$J_BUILD_PATH \
	executableName-$J_EXECUTABLE_NAME \
	bundleVersion-$J_BUILD_VERSION \
	buildNumber-$J_BUILD_NUMBER \
	buildChannel-$J_BUILD_CHANNEL \
	buildServer-$J_BUILD_SERVER \
	buildType-$J_BUILD_TYPE\
	-logFile $UNITY_PROJ_LOG_PATH
}

autoRefresh(){
	echo "--> 执行Unity的刷新方法"
	echo "--> 刷新中, 请耐心等待..."
	echo "--> Set UNITY_EXECUTE_METHOD : ${UNITY_EXECUTE_METHOD}"
	echo "--> 刷新Unity工程"
	$UNITY_PATH -quit -batchmode -nographics -projectPath $UNITY_PROJ_PATH \
	-executeMethod ${UNITY_EXECUTE_METHOD} 
}

#UNITY_EXECUTE_METHOD="QuickEditor.Buildflow.BatchModeBuild.Refresh"

#autoRefresh

UNITY_EXECUTE_METHOD="QuickEditor.Buildflow.BatchModeBuild.BuildPackage"

autoBuild

echo "${J_BUILD_PATH}${J_EXECUTABLE_NAME}"

if [ -d "${J_BUILD_PATH}${J_EXECUTABLE_NAME}" ] ; then
	echo "--> Build completed successfully"
	exit 0
else
	echo "--> Error: Build failed. Exited with $?."
	echo "--> Error: Error building Player because scripts had compiler errors "
	exit 1
fi