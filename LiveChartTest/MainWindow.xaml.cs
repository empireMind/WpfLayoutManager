using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<UIClass> viewList = new ObservableCollection<UIClass>();
        BitmapImage lbj = new BitmapImage(new Uri(@"Image/LeBron James.jpg", UriKind.Relative));

        public MainWindow()
        {
            InitializeComponent();

            camManager.DataContext = viewList;
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            var time = string.Format("{0:T}", DateTime.Now);
            UIClass record = new UIClass { Photo = lbj, Name = "LeBron James", Time = time };
            viewList.Add(record);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            viewList.RemoveAt(0);            
        }

        private void camManager_OnRemove(object sender, RoutedEventArgs e)
        {
            UIClass uic = e.OriginalSource as UIClass;
            ;
        }
    }
}
