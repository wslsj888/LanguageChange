using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomLanguage
{
    public static class LanguageManager
    {
        #region 公共字段
        /// <summary>
        /// 所有语言名称列表
        /// </summary>
        public static List<string> LanuageNames;
        /// <summary>
        /// 当前语言名称
        /// </summary>
        public static string CurrentLanuage = "zh-cn";

        public static event Action<string> OnCurrentLanguageChanged;

        #endregion

        #region 私有字段        
        /// <summary>
        /// 全局语言绑定列表
        /// </summary>
        private static List<LanguageBind> AllBinds;

        /// <summary>
        /// 全局语言映射列表
        /// </summary>
        private static List<List<LanguageMap>> AllMaps;
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        static LanguageManager()
        {
            AllBinds = new List<LanguageBind>();
            AllMaps = new List<List<LanguageMap>>();
            LanuageNames = new List<string>();
            InitLanguageMap();
        }

        /// <summary>
        /// 初始化/读取语言设置
        /// </summary>
        private static void InitLanguageMap()
        {
            var path = "./language";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var filepath = path + "/ini.txt";
            string initContent = "";
            string currentLang = "";
            if (!File.Exists(filepath))
            {
                initContent = "zh-cn;en-us\r\n主界面;Main Window\r\n确定;Confirm\r\n取消;Cancel";
                File.AppendAllText(filepath, initContent);
            }
            else
            {
                initContent = File.ReadAllText(filepath);
            }
            if (!string.IsNullOrEmpty(initContent))
            {
                var list = initContent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (list.Count > 0)
                {
                    var names = list[0].Split(';').ToList();
                    currentLang = names[0];
                    for (var i = 1; i < list.Count; i++)
                    {
                        List<LanguageMap> maps = new List<LanguageMap>();
                        var langs = list[i].Split(';');
                        for (var j = 0; j < langs.Length; j++)
                        {
                            maps.Add(new LanguageMap(names[j], langs[j]));
                        }
                        AllMaps.Add(maps);
                    }
                    LanuageNames = names;
                }
            }
            //读取当前语言
            filepath = path + "/current.txt";
            if (!File.Exists(filepath))
            {
                File.WriteAllText(filepath, currentLang);
            }
            else
            {
                currentLang = File.ReadAllText(filepath);
            }
            CurrentLanuage = currentLang;
        }
        #endregion

        #region 公开接口

        /// <summary>
        /// 添加动态语言项
        /// </summary>
        /// <param name="map"></param>
        public static void AddDynamicLanguageMap(List<LanguageMap> map)
        {
            AllMaps.Add(map);
        }

        public static bool ExistsMap(string chinese)
        {
            return AllMaps.Any(p => p.Exists(o => o.LanguageName == "zh-cn" && o.Text == chinese));
        }

        /// <summary>
        /// 绑定窗体/控件以及其子控件的语言
        /// </summary>
        /// <param name="ctrl"></param>
        public static void BindControlLanguage(Control ctrl)
        {
            AddLanguageBind(ctrl);
            foreach (Control c in ctrl.Controls)
            {
                BindControlLanguage(c);
            }
        }

        /// <summary>
        /// 改变语言
        /// </summary>
        /// <param name="languageName"></param>
        public static void ChangeLanguage(string languageName)
        {
            AllBinds.ForEach(p => p.Text = GetTextByLanguage(CurrentLanuage, languageName, p.Text));
            CurrentLanuage = languageName;
            //保存到磁盘
            var filepath = "./language/current.txt";
            File.WriteAllText(filepath, languageName);

            OnCurrentLanguageChanged?.Invoke(languageName);
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 新增语言绑定
        /// </summary>
        private static void AddLanguageBind(Control ctrl)
        {
            if (ctrl is DataGridView)
            {
                foreach (DataGridViewColumn col in (ctrl as DataGridView).Columns)
                {
                    LanguageBind bind = new LanguageBind(col);
                    bind.PropertyChanged += (s, e) =>
                    {
                        col.HeaderText = bind.Text;
                    };
                    bind.Text = GetTextByLanguage("zh-cn", CurrentLanuage, col.HeaderText);
                    AllBinds.Add(bind);
                    col.Disposed += (s, e) =>
                    {
                        RemoveLanguageBind(bind);
                    };
                }
            }
            else if (ctrl is TabControl)
            {
                ctrl.FindForm().Load += (s, e) =>
                {
                    var tab = ctrl as TabControl;
                    var selPage = tab.SelectedTab;
                    foreach (TabPage t in tab.TabPages)
                    {
                        tab.SelectedTab = t;
                    }
                    tab.SelectedTab = selPage;
                };
            }
            else if (ctrl is ToolStrip toolStrip)
            {
                foreach (ToolStripItem item in toolStrip.Items)
                {
                    LanguageBind bind = new LanguageBind(item);
                    bind.PropertyChanged += (s, e) =>
                    {
                        item.Text = bind.Text;
                    };
                    bind.Text = GetTextByLanguage("zh-cn", CurrentLanuage, item.Text);
                    AllBinds.Add(bind);
                    item.Disposed += (s, e) =>
                    {
                        RemoveLanguageBind(bind);
                    };
                }
            }
            else
            {
                LanguageBind bind = new LanguageBind(ctrl)
                {
                    Text = GetTextByLanguage("zh-cn", CurrentLanuage, ctrl.Text)
                };
                ctrl.DataBindings.Add(new Binding("Text", bind, "Text"));
                AllBinds.Add(bind);
                ctrl.Disposed += (s, e) =>
                {
                    RemoveLanguageBind(bind);
                };
            }
        }

        /// <summary>
        /// 获取绑定的对象
        /// </summary>
        public static LanguageBind GetLanguageBind(object obj)
        {
            var bind = AllBinds.FirstOrDefault(p => p.BindedObj.Equals(obj));
            return bind;
        }

        public static void SetBindText(object obj, string newText)
        {
            var bind = GetLanguageBind(obj);
            if (bind != null)
                bind.Text = GetTextByLanguage("zh-cn", CurrentLanuage, newText);
        }

        /// <summary>
        /// 移除语言绑定
        /// </summary>
        private static void RemoveLanguageBind(LanguageBind bind)
        {
            if (AllBinds.Contains(bind))
            {
                AllBinds.Remove(bind);
            }
        }

        /// <summary>
        /// 获取新语言文本
        /// </summary>
        /// <returns></returns>
        private static string GetTextByLanguage(string oldLanguageName, string newLanguageName, string oldText)
        {
            var map = AllMaps.FirstOrDefault(p => p.Exists(o => o.LanguageName == oldLanguageName && o.Text == oldText));
            if (map != null && map.Count > 0)
            {
                var newLanuage = map.FirstOrDefault(p => p.LanguageName == newLanguageName);
                if (newLanuage != null)
                    return newLanuage.Text;
            }
            return oldText;
        }

        /// <summary>
        /// 通过中文获取当前语言的文本
        /// </summary>
        public static string GetTextByChinese(string chineseText)
        {
            return GetTextByLanguage("zh-cn", CurrentLanuage, chineseText);
        }

        #endregion
    }

    /// <summary>
    /// 语言映射类
    /// </summary>
    public class LanguageMap
    {
        public string LanguageName { get; set; }
        public string Text { get; set; }

        public LanguageMap(string languageName, string text)
        {
            LanguageName = languageName;
            Text = text;
        }
    }
}
