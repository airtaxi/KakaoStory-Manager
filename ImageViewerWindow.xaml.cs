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
    /// ImageViewerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageViewerWindow : MetroWindow
    {
        public ImageViewerWindow()
        {
            InitializeComponent();
            IMG_Main.PreviewMouseRightButtonDown += MainWindow.CopyImageHandler;
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Close();
            }
            else if (e.Key == Key.F5 || (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control))
            {
                ZB_Main.Reset();
            }
        }

        private void BT_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Image img = IMG_Main;
                MainWindow.SaveImageToFile(img);
            }
            catch (Exception)
            {
                MainWindow.SaveGIFImage(IMG_Main);
            }
        }

        private void BT_Reset_Click(object sender, RoutedEventArgs e)
        {
            ZB_Main.Reset();
        }

        private void MetroWindow_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is System.Windows.Controls.TextBox))
                e.Handled = true;
        }

        private void BT_Clipboard_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CopyImageHandler(IMG_Main, null);
        }
    }
}
