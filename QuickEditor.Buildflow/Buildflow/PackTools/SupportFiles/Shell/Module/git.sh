# ==============================================
# This tool is for git control
# ----------------------------------------------
# Author: Mr.Hu
# Data:   2020.02.01
# ==============================================
#!/bin/bash

# ==============================================
# git operation
# 1.清理本地环境 
# 2.切到指定分支 
# 3.拉分支最新 
# 4.显示最新一条log
# ----------------------------------------------
# arg1: Git_Workspace
# arg2: Git_Branch
# ==============================================

Git_Branch=""
Git_Workspace=""

function usage() {
  echo "USAGE:"
  echo "  git.sh [-b <Git_Branch>] [-p <Git_Workspace>]"
  echo "Description:"
  echo "  Git_Branch git分支"
  echo "  Git_Workspace git项目目录"
  exit -1
}

while getopts ":b:p:" arg
do
  case $arg in
    b) Git_Branch="$OPTARG"
    ;;
    p) Git_Workspace="$OPTARG"
    ;;
    \?)
    echo "Invalid Option: -$OPTARG"
    usage
    ;;
    esac
done

echo "--> 执行Git相关操作"
cd ${Git_Workspace}
echo "--> 切换目录到 ${Git_Workspace}"
echo "--> Git 开始清理本地环境..."

git prune
git fetch -p
git reset --hard
git clean -dfq

if [ $? -ne 0 ];then
  echo "Error: Git clean failed, Git Path : ${Git_Workspace}"
  exit 1
fi

#git checkout -q .
#git checkout ${Git_Branch}
git checkout -B ${Git_Branch} --track origin/${Git_Branch}
git pull -q

if [ $? -ne 0 ];then
    echo "Error : Git checkout failed, Branch : ${Git_Branch}"
    exit 1
fi

git log -1



