using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KSP_WPF
{
    /// <summary>
    /// TimeLinePageControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TimeLinePageControl : UserControl
    {
        public TimeLinePageControl()
        {
            InitializeComponent();
            //if (!Properties.Settings.Default.HideScrollBar)
            //    SV_Content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        //private void SV_Content_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    if (sender == null) return;
        //    int threshold = 48;
        //    ScrollViewer scrollViewer = (ScrollViewer)sender;
        //    if (scrollViewer == null) return;
        //    double target = scrollViewer.VerticalOffset - Math.Min(Math.Max(e.Delta, -threshold), threshold);
        //    scrollViewer.ScrollToVerticalOffset(target);
        //    if ((scrollViewer.VerticalOffset < scrollViewer.ScrollableHeight || (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight && e.Delta > 0)) && (scrollViewer.VerticalOffset > 0 || (scrollViewer.VerticalOffset == 0 && e.Delta < 0)))
        //    {
        //        e.Handled = true;
        //    }
        //    else
        //    {
        //        TimeLineWindow parentWindow = (TimeLineWindow)Window.GetWindow(scrollViewer);
        //        if (parentWindow == null) return;
        //        parentWindow.isScrollOver = false;
        //        e.Handled = true;
        //    }
        //}
        
        //private void SV_Content_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (sender == null) return;
        //    ScrollViewer scrollViewer = (ScrollViewer)sender;
        //    if (scrollViewer == null) return;
        //    if (scrollViewer.IsMouseOver)
        //    {
        //        TimeLineWindow parentWindow = (TimeLineWindow)Window.GetWindow(scrollViewer);
        //        if (parentWindow == null) return;
        //        parentWindow.isScrollOver = true;
        //    }
        //    else
        //    {
        //        TimeLineWindow parentWindow = (TimeLineWindow)Window.GetWindow(scrollViewer);
        //        if (parentWindow == null) return;
        //        parentWindow.isScrollOver = false;
        //    }
        //}
    }
}
