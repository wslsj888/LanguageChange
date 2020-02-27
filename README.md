# LanguageChange
WinForm语言转换，如中英文切换

* 简单的语言管理器,可以自定义语言转换
* 只需引用项目中的CustomLanguage
* 调用方式：LanguageManager.BindControlLanguage(this);//this是窗体对象
* 调用之后会默认在运行目录新增language/(ini.txt、current.txt)文件,可以参照初始格式进行定义
* 注：只适用于简单的窗体/控件的语言转换
