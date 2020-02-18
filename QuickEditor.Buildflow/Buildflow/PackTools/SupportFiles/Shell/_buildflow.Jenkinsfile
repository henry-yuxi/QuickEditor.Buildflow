pipeline {
  agent any
  options { 
    disableConcurrentBuilds() //不允许同时执行流水线
    skipDefaultCheckout()  //默认跳过来自源代码控制的代码
    timeout(time: 10, unit: 'MINUTES') //设置流水线运行的超时时间
    timestamps() //预定义由Pipeline生成的所有控制台输出时间
  }
  parameters {
    string(
      name: 'UnityPath', 
      defaultValue: '', 
      description: 'Unity安装目录'
      )
    string(
      name: 'UnityProjectPath', 
      defaultValue: '', 
      description: 'Unity项目目录'
      )
    string(
      name: 'UnityProjectGitPath', 
      defaultValue: '', 
      description: 'Unity项目Git目录'
      )
    choice(
      name: 'GitBranch', 
      choices: ['master', 'develop'], 
      description: 'Git分支 用于切换分支,更新分支,自动打包'
      )
    choice(
      name: 'BuildTarget', 
      choices: ['Android', 'iOS', 'StandaloneWindows', 'StandaloneOSX'], 
      description: '打包平台'
      )
    choice(
      name: 'BuildType', 
      choices: ['Release', 'Develop', 'Beta'], 
      description: '打包环境'
      )
    choice(
      name: 'BuildAction', 
      choices: ['Refresh', 'SetScriptingDefineSymbols', 'BuildBuildle', 'BuildPackage', 'BuildWholePackage'], 
      description: 'Unity打包行为'
      )    
    string(
      name: 'BundleVersion', 
      defaultValue: '1.0.0', 
      description: '应用版本信息'
      )
    string(
      name: 'BuildNumber', 
      defaultValue: '1', 
      description: '应用迭代版本号，每次更新应用商店的包，这数值都增加'
      )
    string(
      name: 'UnityBatchExecuteMethod', 
      defaultValue: 'QuickEditor.Buildflow.UnityBatchBuildTools.BuildPackage', 
      description: 'Unity命令行打包执行的方法'
      )

    //以下为支持的参数类型
    //string(
    //  name: 'deploy_hostname', 
    //  defaultValue: 'host131', 
    //  description: '你需要在哪台机器上进行部署 ?',
    //)
    //text(
    //  name: 'BIOGRAPHY', 
    //  defaultValue: '', 
    //  description: 'Enter some information about the person'
    //  )
    //booleanParam(
    //  name: 'TOGGLE', 
    //  defaultValue: true, 
    //  description: 'Toggle this value'
    //  )
    //choice(
    //  name: 'CHOICE', 
    //  choices: ['One', 'Two', 'Three'], 
    //  description: 'Pick something'
    //  )
    //password(
    //  name: 'PASSWORD', 
    //  defaultValue: 'SECRET', 
    //  description: 'Enter a password'
    //  )
    //file(
    //  name: "FILE", 
    //  description: "Choose a file to upload"
    //  )
  }
  stages {
    stage('Build') {
      steps {
        echo 'Hello World'
      }
    }
  }
}