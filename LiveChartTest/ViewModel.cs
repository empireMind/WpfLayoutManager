using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class ViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
