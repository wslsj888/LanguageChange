using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LanguageChange
{
    public partial class FrmTest : Form
    {
        public FrmTest()
        {
            InitializeComponent();
            CustomLanguage.LanguageManager.BindControlLanguage(this);
        }
    }
}
