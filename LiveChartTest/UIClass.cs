using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Demo
{
    class UIClass: ViewModel
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string time;
        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                NotifyPropertyChanged("Time");
            }
        }

        private BitmapImage photo;
        public BitmapImage Photo
        {
            get { return photo; }
            set
            {
                photo = value;
                NotifyPropertyChanged("Photo");
            }
        }
    }
}
