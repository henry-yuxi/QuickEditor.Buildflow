# ==============================================
# This tool is for git control
# ----------------------------------------------
# Author: Mr.Hu
# Data:   2020.02.01
# ==============================================
#!/bin/bash

# ==============================================
# git operation
# 切到某个节点 传过来的是commit的sha值
# ----------------------------------------------
# arg1: Git_Workspace
# arg2: Git_Commit_ID
# ==============================================

Git_Commit_ID=""
Git_Workspace=""

function usage() {
  echo "USAGE:"
  echo "  git_tag.sh [-s <Git_Commit_ID>] [-p <Git_Workspace>]"
  echo "Description:"
  echo "  Git_Commit_ID commit的sha值"
  echo "  Git_Workspace git项目目录"
  exit -1
}

while getopts ":s:p:" arg
do
  case $arg in
    s) Git_Commit_ID="$OPTARG"
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
git clean -dfq

if [ $? -ne 0 ];then
  echo "Error: Git clean failed, Git Path : ${Git_Workspace}"
  exit 1
fi

git checkout -q .
git checkout ${Git_Commit_ID}

if [ $? -ne 0 ];then
    echo "Error : Git checkout failed, commit-id : ${Git_Commit_ID}"
    exit 1
fi

git log -1

