#!/bin/sh

echo "--> 获取并设置插件参数" 
#插件配置的参数有:
#Build_Target         项目的打包平台  安卓 苹果
#Build_Package        是否打包
#Build_Bundles        是否打包资源
#Git_Operate          对git当前分支的处理     Stash 缓存当前分支  Clean 清理当前分支
#Git_Branch           git分支
#Unity_Server         服务器标识
#Unity_Channel        项目渠道标识
#Unity_SDK            渠道SDK标识
#Bundles_Version      资源包版本
#Development_Build 
#Publish_Mode         发布模式
#Write_Macro          项目打包需要写入的宏
#Upload_Bundles       是否上传资源包
#Upload_Fir           是否上传第三方托管平台
#Upload_Bugly         是否上传符号表

#接收参数
JP_BUILD_TARGET=$Build_Target
JP_BUILD_PACKAGE=$Build_Package
JP_BUILD_BUNDLES=$Build_Bundles
JP_GIT_OPERATE=$Git_Operate
JP_GIT_BRANCH=$Git_Branch
JP_UNITY_SERVER=$Unity_Server;
JP_UNITY_CHANNEL=$Unity_Channel;
JP_UNITY_SDK=$Unity_SDK;
JP_UNITY_DEFINE=$Unity_Define;
JP_BUNDLES_VERSION=$Bundles_Version;
JP_DEVELOP_BUILD=$Development_Build;
JP_FIR_TOKEN=$Fir_Token
JP_UPLOAD_FIR=$Upload_Fir
JP_UPLOAD_BUGLY=$Upload_Bugly
JP_UPLOAD_BUNDLES=$Upload_Bundles
JP_EXPORT_METHOD=$Ipa_Export_Method
JP_UPLOAD_PACKAGE=$Upload_Package

echo "--> 检测插件参数" 
#参数非空判断
if [[ ! $JP_BUILD_TARGET || ! $JP_GIT_OPERATE || ! $JP_GIT_BRANCH || ! $JP_UNITY_SERVER || ! $JP_UNITY_CHANNEL || ! $JP_UNITY_SDK || ! $JP_BUNDLES_VERSION || ! $JP_DEVELOP_BUILD ]]; then
  echo "Error : 插件配置的参数有至少一个为空, 请仔细检查"
    exit 1
fi

echo "--> 检测打包平台参数" 
SUPPORT_TARGETS=("Android" "iOS" "PC")
if echo "${SUPPORT_TARGETS[@]}" | grep -w $JP_BUILD_TARGET &>/dev/null; then
  echo "--> 当前打包平台参数: ${JP_BUILD_TARGET}" 
else
  echo "Error : 不支持的打包平台参数 : ${JP_BUILD_TARGET}"
  exit 1
fi

#region #设置项目参数
echo "--> 设置项目工程参数" 
UNITY_PATH="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
PROJ_BUILDER_TIME="`date +%Y%m%d_%H%M`"
if [[ $JP_BUILD_TARGET = "Android" ]]; then
  PROJ_GIT_PATH="/Users/mac2144/Documents/YLProjects/yl_develop_android/ylqtclient"
elif  [[ $JP_BUILD_TARGET = "iOS" ]]; then
  PROJ_GIT_PATH="/Users/mac2144/Documents/YLProjects/yl_develop_ios/ylqtclient"
elif  [[ $JP_BUILD_TARGET = "PC" ]]; then
  PROJ_GIT_PATH="/Users/mac2144/Documents/YLProjects/yl_develop_pc/ylqtclient"  
fi
PROJ_PATH="${PROJ_GIT_PATH}/YL"

PROJ_BUILDER_ROOT_PATH="${PROJ_GIT_PATH}/ProjectBuilder"
PROJ_BUILDER_TOOL_PATH="${PROJ_BUILDER_ROOT_PATH}/PackTools"
#REDIRECT_UNITY_OUT_PY_PATH="${PROJ_BUILDER_ROOT_PATH}/PackTools/ExportOptions/RedirectUnityOut.py"

echo "--> Set PROJ_GIT_PATH : ${PROJ_GIT_PATH}" 
echo "--> Set PROJ_PATH : ${PROJ_PATH}" 
echo "--> Set PROJ_BUILDER_TOOL_PATH : ${PROJ_BUILDER_TOOL_PATH}" 
echo "--> Set PROJ_BUILDER_ROOT_PATH : ${PROJ_BUILDER_ROOT_PATH}" 

#endregion

#region #Git相关操作
echo "--> 执行Git相关操作"
echo "--> 切换目录到 ${PROJ_GIT_PATH}"
cd ${PROJ_GIT_PATH}

SUPPORT_GIT_OPERATES=("Stash" "Clean")
if echo "${SUPPORT_GIT_OPERATES[@]}" | grep -w $JP_GIT_OPERATE &>/dev/null; then
  echo "--> 当前Git Operate参数: ${JP_GIT_OPERATE}" 
else
  echo "Error : 不支持的Git Operate参数 : ${JP_GIT_OPERATE}"
  exit 1
fi

if [[ $JP_GIT_OPERATE = "Stash" ]]; then
  echo "--> 缓存当前分支"
  git fetch
  git stash save "自动打包 本地缓存 _${PROJ_BUILDER_TIME}"
  if [ $? -ne 0 ];then
    echo "Error : Stash ${PROJ_GIT_PATH}"
    exit 1
  fi
elif  [[ $JP_GIT_OPERATE = "Clean" ]]; then
  echo "--> 清理当前分支"
  git fetch
  git reset --hard
  git clean -df
  if [ $? -ne 0 ];then
    echo "Error : Clean ${PROJ_GIT_PATH}"
    exit 1
  fi
fi

echo "--> 切换目录到分支 ${JP_GIT_BRANCH}"
git checkout -B ${JP_GIT_BRANCH} --track origin/${JP_GIT_BRANCH}
git pull

if [ $? -ne 0 ];then
    echo "Error : Git Checkout Error, Branch : ${JP_GIT_BRANCH}"
    exit 1
fi
#endregion

#region #设置日志参数
echo "--> 设置项目日志参数" 
if [[ $JP_BUILD_TARGET = "Android" ]]; then
  REMOTE_TARGET_NAME="ANDROID"
  PROJ_BUILDER_OUTPUT_ROOT_PATH="${PROJ_BUILDER_ROOT_PATH}/Output/ANDROID"
  PROJ_BUILDER_OUTPUT_PATH="${PROJ_BUILDER_OUTPUT_ROOT_PATH}/${JP_UNITY_SERVER}"
  PROJ_BUILDER_LOG_PATH="${PROJ_BUILDER_OUTPUT_PATH}/project_builder.log"
elif [[ $JP_BUILD_TARGET = "iOS" ]]; then
  REMOTE_TARGET_NAME="IOS"  
  PROJ_BUILDER_OUTPUT_ROOT_PATH="${PROJ_BUILDER_ROOT_PATH}/Output/IOS"
  PROJ_BUILDER_OUTPUT_PATH="${PROJ_BUILDER_OUTPUT_ROOT_PATH}/Xcode_${JP_EXPORT_METHOD}_Output"
  PROJ_BUILDER_LOG_PATH="${PROJ_BUILDER_OUTPUT_PATH}/project_builder.log"
elif [[ $JP_BUILD_TARGET = "PC" ]]; then
  REMOTE_TARGET_NAME="PC"  
  PROJ_BUILDER_OUTPUT_ROOT_PATH="${PROJ_BUILDER_ROOT_PATH}/Output/PC"
  PROJ_BUILDER_OUTPUT_PATH="${PROJ_BUILDER_OUTPUT_ROOT_PATH}/${JP_UNITY_SERVER}"
  PROJ_BUILDER_LOG_PATH="${PROJ_BUILDER_OUTPUT_PATH}/project_builder.log"
fi

echo "--> Set PROJ_BUILDER_OUTPUT_ROOT_PATH : ${PROJ_BUILDER_OUTPUT_ROOT_PATH}"
echo "--> Set PROJ_BUILDER_OUTPUT_PATH : ${PROJ_BUILDER_OUTPUT_PATH}"
echo "--> Set PROJ_BUILDER_LOG_PATH : ${PROJ_BUILDER_LOG_PATH}" 

if [ ! -d $PROJ_BUILDER_OUTPUT_PATH  ]; then
  mkdir $PROJ_BUILDER_OUTPUT_PATH
fi

#如果日志文件已存在 删除日志文件
if [[ -f $PROJ_BUILDER_LOG_PATH ]]; then
  #rm -f $PROJ_BUILDER_LOG_PATH
  echo -n "" > $PROJ_BUILDER_LOG_PATH
  echo "--> 日志文件清除成功"
fi

#监听日志文件 输出日志到Jenkins控制台
touch ${PROJ_BUILDER_LOG_PATH}
tail -f ${PROJ_BUILDER_LOG_PATH} &

#endregion

#region #判断是否需要打包资源
if [[ $JP_BUILD_BUNDLES = "true" ]]; then
  echo "--> 设置Unity的资源打包参数" 

  UNITY3D_BUILD_METHOD="JenkinsBuilder.JenkinsBuildBundles"
  echo "--> Set UNITY3D_BUILD_METHOD : ${UNITY3D_BUILD_METHOD}" 

  echo "--> 执行Unity的资源打包方法"
  echo "--> 资源打包中, 请耐心等待..."
  $UNITY_PATH -quit -batchmode -nographics -projectPath $PROJ_PATH -executeMethod ${UNITY3D_BUILD_METHOD} Build_Target-$JP_BUILD_TARGET Bundles_Version-$JP_BUNDLES_VERSION Unity_Server-$JP_UNITY_SERVER Unity_Channel-$JP_UNITY_CHANNEL Unity_SDK-$JP_UNITY_SDK -logFile $PROJ_BUILDER_LOG_PATH
  echo "--> 资源打包完成"
fi
#endregion

#region #Build_Package
if  [[ $JP_BUILD_PACKAGE = "true" ]]; then
  echo "--> 设置Unity的打包参数" 
  if [[ $JP_BUILD_TARGET = "Android" ]]; then
    REMOTE_TARGET_NAME="ANDROID"
    PACK_NAME="Android Package: ";
    PROJ_BUILDER_FILE_NAME="ylqt_Android_${JP_UNITY_SERVER}_${PROJ_BUILDER_TIME}.apk"
  elif [[ $JP_BUILD_TARGET = "iOS" ]]; then
    REMOTE_TARGET_NAME="IOS"
    PACK_NAME="Xcode Project: ";
    PROJ_BUILDER_FILE_NAME="ylqt_iPhone_${JP_UNITY_SERVER}_${PROJ_BUILDER_TIME}"   
  elif [[ $JP_BUILD_TARGET = "PC" ]]; then
    REMOTE_TARGET_NAME="PC"
    PACK_NAME="PC Project: ";
    PROJ_BUILDER_FILE_NAME="ylqt_PC_${JP_UNITY_SERVER}_${PROJ_BUILDER_TIME}/ylqt_Setup.exe"   
  fi

  echo "--> Set PROJ_BUILDER_FILE_NAME : ${PROJ_BUILDER_FILE_NAME}" 

  #UNITY3D_REFRESH_METHOD="JenkinsBuilder.Refresh"
  #echo 刷新Unity工程
  #$UNITY_PATH -quit -batchmode -nographics -projectPath $PROJ_PATH -executeMethod ${UNITY3D_REFRESH_METHOD} 

  if [[ $JP_BUILD_TARGET = "iOS" ]]; then
    if [ -d "$PROJ_BUILDER_OUTPUT_PATH/$PROJ_BUILDER_FILE_NAME" ] ; then
      rm -rf $PROJ_BUILDER_OUTPUT_PATH/$PROJ_BUILDER_FILE_NAME
      echo "--> ${PACK_NAME}${PROJ_BUILDER_FILE_NAME} 删除成功 "
    fi
  fi

  echo "--> 执行Unity的写宏方法"
  UNITY3D_BUILD_METHOD="JenkinsBuilder.ScriptingJenkinsDefineSymbols"
  echo "--> Set UNITY3D_BUILD_METHOD : ${UNITY3D_BUILD_METHOD}" 
  $UNITY_PATH -quit -batchmode -nographics -projectPath $PROJ_PATH -executeMethod ${UNITY3D_BUILD_METHOD} Build_Target-$JP_BUILD_TARGET Unity_Define-$JP_UNITY_DEFINE -logFile $PROJ_BUILDER_LOG_PATH

  echo "--> 执行Unity的打包方法"
  UNITY3D_BUILD_METHOD="JenkinsBuilder.JenkinsBuildPackage"
  echo "--> Set UNITY3D_BUILD_METHOD : ${UNITY3D_BUILD_METHOD}" 
  $UNITY_PATH -quit -batchmode -nographics -projectPath $PROJ_PATH -executeMethod ${UNITY3D_BUILD_METHOD} Build_Target-$JP_BUILD_TARGET  Unity_Define-$JP_UNITY_DEFINE Ipa_Export_Method-$JP_EXPORT_METHOD Bundles_Version-$JP_BUNDLES_VERSION Unity_Server-$JP_UNITY_SERVER Unity_Channel-$JP_UNITY_CHANNEL Unity_SDK-$JP_UNITY_SDK Development_Build-$JP_DEVELOP_BUILD Build_Path-$PROJ_BUILDER_OUTPUT_PATH Build_FileName-$PROJ_BUILDER_FILE_NAME -logFile $PROJ_BUILDER_LOG_PATH

  if [[ $JP_BUILD_TARGET = "iOS" ]]; then
    PROJ_EXPORT_FILE_PATH="${PROJ_BUILDER_OUTPUT_PATH}/${PROJ_BUILDER_FILE_NAME}"
    if [ -d "$PROJ_EXPORT_FILE_PATH" ] ; then
      echo "--> ${PACK_NAME}${PROJ_BUILDER_FILE_NAME} 生成成功 "
    else
      echo "--> Error : ${PACK_NAME}${PROJ_BUILDER_FILE_NAME} 生成失败, Error building Player because scripts had compiler errors "
      exit 1
    fi
  else
    PROJ_EXPORT_FILE_PATH="${PROJ_BUILDER_OUTPUT_PATH}/${PROJ_BUILDER_FILE_NAME}"
    if [ -f "$PROJ_EXPORT_FILE_PATH" ] ; then
      echo "--> ${PACK_NAME}${PROJ_BUILDER_FILE_NAME} 生成成功 "
    else
      echo "--> Error : ${PACK_NAME}${PROJ_BUILDER_FILE_NAME} 生成失败, Error building Player because scripts had compiler errors "
      exit 1
    fi
  fi

  if [[ $JP_BUILD_TARGET = "iOS" ]]; then
    #Xcode打包处理

    #Xcode路径相关参数
    CONFIGURATION=Release
    SCHEME="Unity-iPhone"
    IPA_OUTPUT_PATH="${PROJ_BUILDER_OUTPUT_ROOT_PATH}/IPA_${JP_EXPORT_METHOD}_Output"
    XCODE_PROJ_PATH="${PROJ_BUILDER_OUTPUT_PATH}/${PROJ_BUILDER_FILE_NAME}/${SCHEME}.xcodeproj"
    ARCHIVE_PATH="${IPA_OUTPUT_PATH}/${PROJ_BUILDER_FILE_NAME}/${SCHEME}.xcarchive"
    EXPORT_PATH="${IPA_OUTPUT_PATH}/${PROJ_BUILDER_FILE_NAME}"

    if [ ! -d $IPA_OUTPUT_PATH ];then
      mkdir $IPA_OUTPUT_PATH
    fi

    #Xcode打包必需参数
    IPA_COMMAND_SETTINGS=${PROJ_BUILDER_ROOT_PATH}/PackTools/ExportOptions/iOSPackSettings.ini
    if [ ! -f "$IPA_COMMAND_SETTINGS" ];then
      echo "--> Error : IPA 配置 -> ${IPA_COMMAND_SETTINGS} 不存在"
      exit 1
    fi

    EXPORT_OPTION_PLIST=${PROJ_BUILDER_TOOL_PATH}/ExportOptions/exportOptions_${JP_EXPORT_METHOD}.plist

    BUNDLE_IDENTIFIER_KEY=BUNDLE_IDENTIFIER
    CODE_SIGN_IDENTITY_KEY=CODE_SIGN_IDENTITY
    PROVISIONING_PROFILE_NAME_KEY=PROVISIONING_PROFILE_NAME
    
    BUNDLE_IDENTIFIER=$(awk -F '=' '/\['${JP_EXPORT_METHOD}'\]/{a=1} (a==1 && "'${BUNDLE_IDENTIFIER_KEY}'"==$1){a=0;print $2}' ${IPA_COMMAND_SETTINGS}) 
    CODE_SIGN_IDENTITY=$(awk -F '=' '/\['${JP_EXPORT_METHOD}'\]/{a=1} (a==1 && "'${CODE_SIGN_IDENTITY_KEY}'"==$1){a=0;print $2}' ${IPA_COMMAND_SETTINGS}) 
    PROVISIONING_PROFILE_NAME=$(awk -F '=' '/\['${JP_EXPORT_METHOD}'\]/{a=1} (a==1 && "'${PROVISIONING_PROFILE_NAME_KEY}'"==$1){a=0;print $2}' ${IPA_COMMAND_SETTINGS}) 

    #CODE_SIGN_IDENTITY="iPhone Distribution: Shanghai Blademaster Network Technology Co., Ltd."
    #PROVISIONING_PROFILE_NAME="ylqt_inhouse"
    #BUNDLE_IDENTIFIER=com.baiyao.ylqt

    #CODE_SIGN_IDENTITY="iPhone Developer: pengfei yang"

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
    python ${PROJ_BUILDER_TOOL_PATH}/ExportOptions/DeleteAutoManageSigning.py ${PROJ_BUILDER_OUTPUT_PATH}/${PROJ_BUILDER_FILE_NAME}/

    echo '--> 正在清理工程'
    xcodebuild clean -project ${XCODE_PROJ_PATH} -scheme ${SCHEME} -configuration ${CONFIGURATION} \
    >>${PROJ_BUILDER_LOG_PATH}

    echo '--> 工程清理完成-->>>--正在编译工程:'${CONFIGURATION}
    xcodebuild archive \
    -project ${XCODE_PROJ_PATH} \
    -scheme ${SCHEME} \
    -configuration ${CONFIGURATION} \
    -archivePath ${ARCHIVE_PATH} \
    CODE_SIGN_IDENTITY="${CODE_SIGN_IDENTITY}" PROVISIONING_PROFILE_SPECIFIER="${PROVISIONING_PROFILE_NAME}" \
    >>${PROJ_BUILDER_LOG_PATH}

    if [ $? -ne 0 ] ; then
      echo '--> 项目编译失败'
      exit 1
    else
      echo '--> 项目编译成功'
    fi

    echo '--> 项目编译完成-->>>--开始IPA打包'
    xcodebuild -exportArchive \
    -archivePath ${ARCHIVE_PATH} \
    -configuration ${CONFIGURATION} \
    -exportPath ${EXPORT_PATH} \
    -exportOptionsPlist "$EXPORT_OPTION_PLIST" \
    >>${PROJ_BUILDER_LOG_PATH}

    if [ $? -ne 0 ] ; then
      echo '--> 项目构建失败'
      exit 1
    else
      echo '--> 项目构建成功'
    fi
  fi

fi
#endregion


#region #应用上传Fir第三方托管
#公司Fir账号的Token  2c5a5ecc1df1a247ce2ce9616dcbb4f6
#个人Fir账号的Token  95c0c894b9ee035d1cc67761272822fb
#处理上传流程

if  [[ $JP_UPLOAD_FIR = "true" && $JP_BUILD_PACKAGE = "true" && $JP_BUILD_TARGET != "PC" ]]; then

  echo '--> 发布应用到 fir.im平台'
  echo "--> Set JP_FIR_TOKEN : ${JP_FIR_TOKEN}" 
  
  if [[ $JP_UNITY_SDK = "bilibili" ]]; then
  	FIR_COMMAND_SETTINGS=${PROJ_BUILDER_ROOT_PATH}/PackTools/FirOptions/BilibiliFirSettings.ini
  else
    FIR_COMMAND_SETTINGS=${PROJ_BUILDER_ROOT_PATH}/PackTools/FirOptions/FirSettings.ini
  fi
  
  if [ ! -f "$FIR_COMMAND_SETTINGS" ];then
    echo "--> Error : Fir 配置 -> ${FIR_COMMAND_SETTINGS} 不存在"
    exit 1
  fi

  FIR_TOKEN_ID_KEY=FIR_TOKEN_ID
  FIR_SHORT_NAME_ANDROID_KEY=FIR_SHORT_NAME_ANDROID
  FIR_SHORT_NAME_IOS_KEY=FIR_SHORT_NAME_IOS

  FIR_TOKEN_ID=$(awk -F '=' '/\['${JP_FIR_TOKEN}'\]/{a=1} (a==1 && "'${FIR_TOKEN_ID_KEY}'"==$1){a=0;print $2}' ${FIR_COMMAND_SETTINGS}) 
  if [[ $JP_BUILD_TARGET = "Android" ]]; then
    FIR_SHORT_NAME=$(awk -F '=' '/\['${JP_FIR_TOKEN}'\]/{a=1} (a==1 && "'${FIR_SHORT_NAME_ANDROID_KEY}'"==$1){a=0;print $2}' ${FIR_COMMAND_SETTINGS}) 
  else
    FIR_SHORT_NAME=$(awk -F '=' '/\['${JP_FIR_TOKEN}'\]/{a=1} (a==1 && "'${FIR_SHORT_NAME_IOS_KEY}'"==$1){a=0;print $2}' ${FIR_COMMAND_SETTINGS}) 
  fi

  echo "--> Set FIR_TOKEN_ID : ${FIR_TOKEN_ID}" 
  echo "--> Set FIR_SHORT_NAME : ${FIR_SHORT_NAME}" 
  
  if [[ $JP_BUILD_TARGET = "Android" ]]; then
    UPLOAD_PACKAGE_PATH=${PROJ_BUILDER_OUTPUT_PATH}/${PROJ_BUILDER_FILE_NAME}
  else
    UPLOAD_PACKAGE_PATH=${IPA_OUTPUT_PATH}/${PROJ_BUILDER_FILE_NAME}/${SCHEME}.ipa
  fi

  echo "--> Set UPLOAD_PACKAGE_PATH : ${UPLOAD_PACKAGE_PATH}" 

  # 需要先在本地安装 fir 插件,安装fir插件命令: gem install fir-cli
  fir login -T ${FIR_TOKEN_ID}  # fir.im token
  fir publish  ${UPLOAD_PACKAGE_PATH} --short=${FIR_SHORT_NAME}

  if [ $? = 0 ];then
    echo "--> 提交fir.im成功 "
  else
    echo "--> Error : 提交fir.im失败 "
    exit 0
  fi
fi
#endregion

#region #应用上传Bugly符号表
if [[ $JP_UPLOAD_BUGLY = "true" && $JP_BUILD_PACKAGE = "true" ]]; then
BUGLY_ID="3a46b040bc"
BUGLY_KEY="2501780d-1a32-48c4-80f7-457a1a76df04"
echo '--> 上传Bugly符号表'
  COMMAND_PATH="${PROJ_BUILDER_TOOL_PATH}/buglySymboliOS"
  SETTINGS_PATH="${PROJ_BUILDER_TOOL_PATH}/buglySymboliOS/settings.txt"
  DSYM_NAME="${ARCHIVE_PATH}/dSYMs/ylqt.app.dSYM"
  #sh ${COMMAND_PATH}/buglySymboliOS.sh -i ${DSYM_NAME} -u true -d true -id ${BUGLY_ID} -key ${BUGLY_KEY}
  java -jar ${COMMAND_PATH}/buglySymboliOS.jar -i ${DSYM_NAME} -u true -d true -id ${BUGLY_ID} -key ${BUGLY_KEY} -package ${BUNDLE_IDENTIFIER} -version 1.2.28 -symbol
    if [ $? = 0 ];then
      echo "--> 上传符号表成功 "
    else
      echo "--> Error : 上传符号表失败 "
    fi
fi
#endregion


#region #判断是否需要上传资源文件到CDN
if [[ ($JP_UPLOAD_BUNDLES = "true" && $JP_BUILD_BUNDLES = "true" && $JP_UNITY_SERVER != "official") ]]; then

  echo "--> 开始上传资源包到CDN" 

  echo "--> 上传 ${JP_BUILD_TARGET} Bundles"
  LOCAL_PATH=${PROJ_GIT_PATH}/AssetsBundle/${JP_UNITY_SERVER}/${REMOTE_TARGET_NAME}/Uploads/${REMOTE_TARGET_NAME}
  REMOTE_PATH=/${JP_UNITY_SERVER}/

  echo "--> Set LOCAL_PATH  : ${LOCAL_PATH}" 
  echo "--> Set REMOTE_PATH : ${REMOTE_PATH}" 

  if [ ! -d $LOCAL_PATH ];then
    echo "--> Error : 本地没有需要上传的资源目录 : ${LOCAL_PATH}"
    exit 1
  fi

lftp -u hulinchao,hulinchao@123 -p 2122 192.168.10.181 <<EOF
set ftp:list-empty-ok yes
cd /
mkdir -p ${REMOTE_PATH}
cd ${REMOTE_PATH}
lcd ${LOCAL_PATH}
mirror -R -n ${LOCAL_PATH}
exit
EOF

  if [ $? = 0 ];then
    echo "--> 上传${JP_BUILD_TARGET} Bundles Succeed " 

    echo "--> 开始刷新CDN" 
    python ${PROJ_BUILDER_TOOL_PATH}/Qcloud_CDN_API/QcloudCdnTools_V2.py RefreshCdnDir -u AKIDCCzXq6L0f5GG1XNrlcP3ShgPs52koNIx -p ZtxlIfnfHdjY7QTFl4A2e2B4g27wf8LI --dirs http://res.ylqt.2144gy.com/${JP_UNITY_SERVER}/${REMOTE_TARGET_NAME}/
    if [ $? = 0 ];then
      echo "--> CDN刷新成功 "
    else
      echo "--> Error : CDN刷新失败 "
    fi
  else
    echo "--> Error : 上传${JP_BUILD_TARGET} Bundles Failed "
    exit 1
  fi
fi
#endregion

#region #判断是否需要上传安装包到CDN
if [[ $JP_UPLOAD_PACKAGE = "true" && $JP_BUILD_PACKAGE = "true" ]]; then

  echo "--> 开始上传安装包到CDN" 

  echo "--> 上传 ${JP_BUILD_TARGET} Installation Package "
  if [[ $JP_BUILD_TARGET = "Android" ]]; then
    PROJ_BUILDER_FILE_NAME=${PROJ_BUILDER_FILE_NAME}
    UPLOAD_ROOT_PATH=${PROJ_BUILDER_OUTPUT_PATH}
    UPLOAD_FILE_NAME=${PROJ_BUILDER_FILE_NAME}
    UPLOAD_PACKAGE_PATH=${UPLOAD_ROOT_PATH}/${UPLOAD_FILE_NAME}
    MV_FILE_NAME=${PROJ_BUILDER_FILE_NAME}
  elif [[ $JP_BUILD_TARGET = "iOS" ]]; then
    PROJ_BUILDER_FILE_NAME=${PROJ_BUILDER_FILE_NAME}
    UPLOAD_ROOT_PATH=${IPA_OUTPUT_PATH}/${PROJ_BUILDER_FILE_NAME}/
    UPLOAD_FILE_NAME=${SCHEME}.ipa
    UPLOAD_PACKAGE_PATH=${UPLOAD_ROOT_PATH}/${UPLOAD_FILE_NAME}
    MV_FILE_NAME=${PROJ_BUILDER_FILE_NAME}.ipa
  else
    PROJ_BUILDER_FILE_NAME="ylqt_PC_${JP_UNITY_SERVER}_${PROJ_BUILDER_TIME}"
    UPLOAD_ROOT_PATH=${PROJ_BUILDER_OUTPUT_PATH}/${PROJ_BUILDER_FILE_NAME}/
    UPLOAD_FILE_NAME=${PROJ_BUILDER_FILE_NAME}.exe
    UPLOAD_PACKAGE_PATH=${UPLOAD_ROOT_PATH}/${UPLOAD_FILE_NAME}
    MV_FILE_NAME=${PROJ_BUILDER_FILE_NAME}.exe
  fi


  REMOTE_PATH=/Packages/${JP_BUILD_TARGET}/${JP_UNITY_SERVER}/

  echo "--> Set UPLOAD_PACKAGE_PATH  : ${UPLOAD_PACKAGE_PATH}" 
  echo "--> Set REMOTE_PATH : ${REMOTE_PATH}" 

  if [ ! -f $UPLOAD_PACKAGE_PATH ];then
    echo "--> Error : 本地没有需要上传的安装包 : ${UPLOAD_PACKAGE_PATH}"
    exit 1
  fi

lftp -u hulinchao,hulinchao@123 -p 2122 192.168.10.181 <<EOF
set ftp:list-empty-ok yes
cd /
mkdir -p ${REMOTE_PATH}
cd ${REMOTE_PATH}
lcd ${UPLOAD_ROOT_PATH}
put ${UPLOAD_PACKAGE_PATH}
mv ${UPLOAD_FILE_NAME} ${MV_FILE_NAME}
exit
EOF

  if [ $? = 0 ];then
    echo "--> 上传${JP_BUILD_TARGET} Installation Package Succeed " 

    echo "--> " 
    echo "--> Installation Package Download Url: http://res.ylqt.2144gy.com${REMOTE_PATH}${MV_FILE_NAME} " 
    echo "--> " 

    echo "--> 开始刷新CDN" 
    python ${PROJ_BUILDER_TOOL_PATH}/Qcloud_CDN_API/QcloudCdnTools_V2.py RefreshCdnDir -u AKIDCCzXq6L0f5GG1XNrlcP3ShgPs52koNIx -p ZtxlIfnfHdjY7QTFl4A2e2B4g27wf8LI --dirs http://res.ylqt.2144gy.com/${REMOTE_PATH}/
    if [ $? = 0 ];then
      echo "--> CDN刷新成功 "
    else
      echo "--> Error : CDN刷新失败 "
    fi
  else
    echo "--> Error : 上传${JP_BUILD_TARGET} Installation Package Failed "
    exit 1
  fi
fi
#endregion

echo "--> " 
echo "--> ALL Scripts Execute Done" 
echo "--> " 
