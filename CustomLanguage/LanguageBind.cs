using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomLanguage
{
    public class LanguageBind : INotifyPropertyChanged
    {
        public object BindedObj { get; set; }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;        

        public LanguageBind()
        {

        }

        public LanguageBind(object obj)
        {
            BindedObj = obj;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
