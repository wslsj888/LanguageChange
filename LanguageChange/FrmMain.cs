﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomLanguage;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace LanguageChange
{
    public partial class FrmMain : Form
    {
        LogList logList = new LogList();

        public FrmMain()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            comboBox1.Text = LanguageManager.CurrentLanuage;
            comboBox1.Items.AddRange(LanguageManager.LanuageNames.ToArray());
            LanguageManager.BindControlLanguage(this);

            this.InitLogLanguage();
            this.txtLogs.DataBindings.Clear();
            this.txtLogs.DataBindings.Add(new Binding("Text", logList, nameof(logList.Text)));

            logList.AddLog("系统开始自检");
            logList.AddLog("系统自检完成");
            logList.AddLog("测试未添加的语言项");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LanguageManager.ChangeLanguage(comboBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmTest test = new FrmTest();
            test.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LanguageManager.SetBindText(sender, "确定");
        }

        private void InitLogLanguage()
        {
            // 如果已经配置在ini.txt文件，就不需要动态添加
            List<List<LanguageMap>> languageMaps = new List<List<LanguageMap>>()
            {
                new List<LanguageMap>(){ new LanguageMap("zh-cn", "系统开始自检"), new LanguageMap("en-us", "System Start Self-Check") },
                new List<LanguageMap>(){ new LanguageMap("zh-cn", "系统自检完成"), new LanguageMap("en-us", "System End Self-Check") }
            };

            // 可以在运行时添加中英文字典
            languageMaps.ForEach(languageMap =>
            {
                LanguageManager.AddDynamicLanguageMap(languageMap);
            });
        }

        public class LogList : INotifyPropertyChanged
        {
            List<LogItem> LogItems = new List<LogItem>();

            public LogList()
            {
                LanguageManager.OnCurrentLanguageChanged += (s) => this.RaisePropertyChanged(nameof(Text));
            }

            public void AddLog(string logContent)
            {
                LogItems.Add(new LogItem() { LogTime = DateTime.Now, LogContent = logContent });
                this.RaisePropertyChanged(nameof(Text));
            }

            public string Text
            {
                get => string.Join("\r\n", LogItems.Select(p => $"{p.LogTime.ToString("yyyy-MM-dd HH:mm:ss")} {LanguageManager.GetTextByChinese(p.LogContent)}"));
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void RaisePropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private class LogItem
        {
            public DateTime LogTime { get; set; }

            public string LogContent { get; set; }
        }
    }
}
