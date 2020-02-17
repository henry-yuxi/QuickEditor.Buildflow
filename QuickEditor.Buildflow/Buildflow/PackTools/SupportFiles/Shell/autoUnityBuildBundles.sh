#!/bin/bash

J_BUILD_TARGET=$1
J_BUNDLES_VERSION=$2
J_BUILD_SERVER=$3
J_BUILD_CHANNEL=$4
J_BUILD_TYPE=$5

autoBuild(){
	echo "--> 执行Unity的资源打包方法"
	echo "--> 资源打包中, 请耐心等待..."
	echo "--> Set J_BUILD_TARGET : ${J_BUILD_TARGET}"
	echo "--> Set J_BUNDLES_VERSION : ${J_BUNDLES_VERSION}"
	echo "--> Set J_BUILD_SERVER : ${J_BUILD_SERVER}"
	echo "--> Set J_BUILD_CHANNEL : ${J_BUILD_CHANNEL}"
	echo "--> Set J_BUILD_TYPE : ${J_BUILD_TYPE}"
	echo "--> Set UNITY_EXECUTE_METHOD : ${UNITY_EXECUTE_METHOD}"
	$UNITY_PATH -quit -batchmode -nographics -projectPath $UNITY_PROJ_PATH \
	-executeMethod ${UNITY_EXECUTE_METHOD} \
	buildTarget-$JP_BUILD_TARGET \
	bundlesVersion-$J_BUNDLES_VERSION \
	buildChannel-$J_BUILD_CHANNEL \
	buildServer-$J_BUILD_SERVER \
	buildType-$J_BUILD_TYPE \
	-logFile $UNITY_PROJ_LOG_PATH
}

autoRefresh(){
	echo "--> 执行Unity的打包方法"
	echo "--> 刷新中, 请耐心等待..."
	echo "--> Set UNITY_EXECUTE_METHOD : ${UNITY_EXECUTE_METHOD}"
	echo "--> 刷新Unity工程"
	$UNITY_PATH -quit -batchmode -nographics -projectPath $UNITY_PROJ_PATH \
	-executeMethod ${UNITY_EXECUTE_METHOD} 
}

UNITY_EXECUTE_METHOD="QuickEditor.Buildflow.BatchModeBuild.Refresh"

autoRefresh

UNITY_EXECUTE_METHOD="QuickEditor.Buildflow.BatchModeBuild.BuildBundles"

autoBuild

echo "${J_BUILD_PATH}${J_EXECUTABLE_NAME}"

if [ -d "${J_BUILD_PATH}${J_EXECUTABLE_NAME}" ] ; then
	echo "--> Build Bundles completed successfully"
	exit 0
else
	echo "--> Error: Build Bundles failed. Exited with $?."
	echo "--> Error: Error building Player because scripts had compiler errors "
	exit 1
fi