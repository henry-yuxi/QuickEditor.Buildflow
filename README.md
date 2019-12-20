# QuickEditor.Buildflow

QuickEditor.Buildflow是基于Unity游戏引擎的自动化一键打包工具流, 配合另外一个开源项目 [QuickEngine.SDK.Native](https://github.com/henry-yuxi/QuickEngine.SDK.Native) (开源统一SDK接入框架), 能快速接入各种渠道SDK, 并方便导出各种渠道包。

同时整套方案都是开源, 方便二次开发定制。

## How to use?

[使用文档](https://github.com/henry-yuxi/QuickEditor.Buildflow/wiki)

## How to install?

### UPM Install via manifest.json

In Packages folder, you will see a file named manifest.json. 

using this package add lines into ./Packages/manifest.json like next sample:
```
{
  "dependencies": {
    "com.sourcemuch.quickeditor.buildflow": "https://github.com/henry-yuxi/QuickEditor.Buildflow.git#0.0.1",
  }
}
```

### Unity 2019.3 Git URL

In Unity 2019.3 or greater, Package Manager is include the new feature that able to install the package via Git.

Open the package manager window (menu: Window > Package Manager), select "Add package from git URL...", fill in this in the pop-up textbox: 
https://github.com/henry-yuxi/QuickEditor.Buildflow.git#0.0.1.


### Unity UPM Git Extension (For 2019.2 and older version)

If you doesn't have this package before, please redirect to this git https://github.com/mob-sakai/UpmGitExtension then follow the instruction in README.md to install the UPM Git Extension to your Unity.

If you already installed. Open the Package Manager UI, you will see the git icon around the bottom left connor, Open it then follow the instruction using this git URL to perform package install.

请确保使用的UPM包是最终版本。
