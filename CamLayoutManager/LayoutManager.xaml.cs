using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CamLayoutManager
{
    /// <summary>
    /// LayoutManager.xaml 的交互逻辑
    /// </summary>
    public partial class LayoutManager : UserControl
    {
        private DependencyObject panel = new DependencyObject();

        public LayoutManager()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)CameraLayout.Items).CollectionChanged += CameraLayoutControl_CollectionChanged;
        }

        /// <summary>
        /// 当队列已满时（目前是9个），新插入的元素会从第一项开始，覆盖旧的元素
        /// </summary>
        int newHead = 0;
        private void CameraLayoutControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)             
            {
                if (CameraLayout.Items.Count > ItemCount)
                {
                    var viewItem = CameraLayout.ItemContainerGenerator.ContainerFromIndex(CameraLayout.Items.Count - 1) as ListViewItem;
                    var newItem = viewItem.DataContext;
                    var moveMethod = this.DataContext.GetType().GetMethod("Move");
                    var removeMethod = this.DataContext.GetType().GetMethod("RemoveAt");
                    //移除newHead项
                    var newHeadViewItem = CameraLayout.ItemContainerGenerator.ContainerFromIndex(newHead) as ListViewItem;
                    var newHeadItem = newHeadViewItem.DataContext;  //获取移除项的dataContext
                    object[] parameters = new object[] { newHead };
                    removeMethod.Invoke(this.DataContext, parameters);
                    RoutedEventArgs arg = new RoutedEventArgs(OnRemoveEvent, newHeadItem);
                    this.RaiseEvent(arg);                           //激发路由事件
                    //最后一项移动到newHead项
                    parameters = new object[] { CameraLayout.Items.Count - 1, newHead };
                    moveMethod.Invoke(this.DataContext, parameters);
                    newHead = (newHead + 1) % ItemCount;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if(CameraLayout.Items.Count < ItemCount)
                    newHead = 0;
                if(!enlarge)
                {
                    //如果Item正处于放大状态，需要先恢复原尺寸
                    m_MyWrapPanel.Margin = M;
                    var animationX = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(100), To = 1 };
                    var animationY = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(100), To = 1 };
                    selectedItem.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
                    selectedItem.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
                    Panel.SetZIndex(selectedItem, 1);   //取消当前item置顶
                    ChangeVisibility(selectedItem, false);
                    enlarge = true;
                }
            }
            //当只有两个Item，通过改变Margin实现居中显示
            if(CameraLayout.Items.Count ==2)
            {
                if(m_MyWrapPanel != null)
                    m_MyWrapPanel.Margin = new Thickness(0, CameraLayout.ActualHeight / 4, 0, CameraLayout.ActualHeight / 4);
            }
            else
            {
                if (m_MyWrapPanel != null)
                    m_MyWrapPanel.Margin = new Thickness(0);
            }
        }

        public static readonly RoutedEvent OnRemoveEvent = 
            EventManager.RegisterRoutedEvent("OnRemove", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LayoutManager));

        public event RoutedEventHandler OnRemove
        {
            add { this.AddHandler(OnRemoveEvent, value); }
            remove { this.RemoveHandler(OnRemoveEvent, value); }
        }

        public int ItemCount
        {
            get { return (int)GetValue(ItemCountProperty); }
            set { SetValue(ItemCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemCountProperty =
            DependencyProperty.Register("ItemCount", typeof(int), typeof(LayoutManager), new PropertyMetadata(0));

        public DataTemplate DataPresenter
        {
            get { return (DataTemplate)GetValue(DataPresenterProperty); }
            set { SetValue(DataPresenterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataPresenter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataPresenterProperty =
            DependencyProperty.Register("DataPresenter", typeof(DataTemplate), typeof(LayoutManager), new PropertyMetadata(null));

        private void CameraLayout_Loaded(object sender, RoutedEventArgs e)
        {
            CameraLayout.ItemTemplate = DataPresenter;
        }

        bool enlarge = true;
        Thickness M = new Thickness(0, 0, 0, 0);
        ListViewItem selectedItem;
        private void CameraLayout_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //int test = 1;
            //var item = VisualTreeHelper.HitTest(CameraLayout, e.GetPosition(CameraLayout)).VisualHit;
            //while (item != null && !(item is ListViewItem))
            //    item = VisualTreeHelper.GetParent(item);
            selectedItem = sender as ListViewItem;
            double x = 0, y = 0;
            CountOriginPoint(selectedItem, ref x, ref y);
            if (selectedItem != null)
            {
                //while (item != null && !(item is WrapPanel))
                //    item = VisualTreeHelper.GetParent(item);
                //WrapPanel wrapPanel = item as WrapPanel;              
                if (enlarge)
                {
                    M = m_MyWrapPanel.Margin;
                    m_MyWrapPanel.Margin = new Thickness(0, 0, 0, 0);
                    //M = wrapPanel.Margin;
                    //SetMargin(wrapPanel, new Thickness(0, 0, 0, 0));
                    double ScaleX = m_MyWrapPanel.ActualWidth / m_MyWrapPanel.ItemWidth;
                    double ScaleY = m_MyWrapPanel.ActualHeight / m_MyWrapPanel.ItemHeight;
                    if (CameraLayout.Items.Count == 2)
                    {
                        ScaleY *= 2;
                    }
                    var animationX = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(100), To = ScaleX };
                    var animationY = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(100), To = ScaleY };
                    selectedItem.RenderTransform = new ScaleTransform(1.0, 1.0);
                    selectedItem.RenderTransformOrigin = new Point(x, y);
                    Panel.SetZIndex(selectedItem, 10);  //将当前item置顶
                    selectedItem.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
                    selectedItem.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
                    ChangeVisibility(selectedItem, true);
                }
                else
                {
                    m_MyWrapPanel.Margin = M;
                    //SetMargin(wrapPanel, M);
                    var animationX = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(100), To = 1 };
                    var animationY = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(100), To = 1 };
                    selectedItem.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
                    selectedItem.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
                    Panel.SetZIndex(selectedItem, 1);   //取消当前item置顶
                    ChangeVisibility(selectedItem, false);                   
                }
                enlarge = !enlarge;
            }
        }

        /// <summary>
        /// 根据选中listviewItem的位置计算中心点，从而实现不同方向的缩放
        /// </summary>
        /// <param name="select"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void CountOriginPoint(ListViewItem select, ref double x, ref double y)
        {
            if (select == null)
                return;
            int count = CameraLayout.Items.Count;
            int idx = CameraLayout.ItemContainerGenerator.IndexFromContainer(select);
            if(count <= 1)
            {
                x = y = 0;         //x正方向，y正方向 
            }
            //else if (count <= 2)
            //{
            //    if (idx == 0)
            //    {
            //        //x = y = 0;      //x正方向，y正方向
            //        x = 0.25; y = 0.5;
            //    }
            //    else if (idx == 1)
            //    {
            //        //x = 1; y = 0;   //x负方向，y正方向
            //        x = 0.75; y = 0.5;
            //    }
            //}
            else if(count <= 4)
            {
                if (idx == 0)
                {
                    x = y = 0;      //x正方向，y正方向
                }
                else if (idx == 1)
                {
                    x = 1; y = 0;   //x负方向，y正方向
                }
                else if (idx == 2)
                {
                    x = 0; y = 1;   //x正方向，y负方向
                }
                else if (idx == 3)
                {
                    x = 1; y = 1;   //x负方向，y负方向
                }
            }
            else if (count <= 9)
            {
                if (idx == 0)
                {
                    x = y = 0;      //x正方向，y正方向
                }
                else if (idx == 1)
                {
                    x = 0.5; y = 0;
                }
                else if (idx == 2)
                {
                    x = 1; y = 0;   //x负方向，y正方向
                }
                else if (idx == 3)
                {
                    x = 0;y = 0.5;
                }
                else if (idx == 4)
                {
                    x = 0.5; y = 0.5;
                }
                else if (idx == 5)
                {
                    x = 1; y = 0.5;
                }
                else if (idx == 6)
                {
                    x = 0; y = 1;   //x正方向，y负方向
                }
                else if (idx == 7)
                {
                    x = 0.5; y = 1;
                }
                else if (idx == 8)
                {
                    x = 1; y = 1;   //x负方向，y负方向
                }
            }
        }

        private void ChangeVisibility(ListViewItem item, bool isHidden)
        {
            int count = CameraLayout.Items.Count;
            for(int i=0; i<count; i++)
            {
                ListViewItem viewItem = CameraLayout.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if(!ReferenceEquals(item, viewItem))
                {
                    if(isHidden)
                        viewItem.Visibility = Visibility.Hidden;
                    else
                        viewItem.Visibility = Visibility.Visible;
                }                    
            }
        }

        private WrapPanel m_MyWrapPanel;
        private void MyPanel_Loaded(object sender, RoutedEventArgs e)
        {
            m_MyWrapPanel = sender as WrapPanel;
        }
    }
}
