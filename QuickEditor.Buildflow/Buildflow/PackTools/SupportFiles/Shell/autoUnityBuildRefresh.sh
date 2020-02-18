# ==============================================
# This tool is for Create Android application
# ----------------------------------------------
# Author: Mr.Hu
# Data:   2020.02.01
# ==============================================

#!/bin/sh
​
J_BUILD_TARGET=$1

autoRefresh(){
  echo "--> 执行Unity的刷新方法"
  echo "--> 刷新中, 请耐心等待..."
  echo "--> Set UNITY_EXECUTE_METHOD : ${UNITY_EXECUTE_METHOD}"
  echo "--> 刷新Unity工程"
  $UNITY_PATH -quit -batchmode -nographics -projectPath $UNITY_PROJ_PATH \
  -executeMethod ${UNITY_EXECUTE_METHOD} \
  buildTarget-$JP_BUILD_TARGET \
  -logFile $UNITY_PROJ_LOG_PATH
}

UNITY_EXECUTE_METHOD="QuickEditor.Buildflow.UnityBatchBuildTools.Refresh"

autoRefresh

exit 0