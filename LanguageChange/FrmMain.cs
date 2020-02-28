using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomLanguage;

namespace LanguageChange
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            comboBox1.Text = LanguageManager.CurrentLanuage;
            comboBox1.Items.AddRange(LanguageManager.LanuageNames.ToArray());
            LanguageManager.BindControlLanguage(this);
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
            LanguageManager.SetBindText(sender,"确定");
        }
    }
}
