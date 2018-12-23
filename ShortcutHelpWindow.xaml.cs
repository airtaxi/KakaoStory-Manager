using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace KSP_WPF
{
    /// <summary>
    /// ShortcutHelpWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShortcutHelpWindow : MetroWindow
    {
        public ShortcutHelpWindow()
        {
            InitializeComponent();
            if (!Properties.Settings.Default.HideScrollBar)
            {
                SV_Post.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Close();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalHelper.HandleScroll(sender, e);
        }
    }
}
