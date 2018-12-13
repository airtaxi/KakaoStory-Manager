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
    /// EmotionWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EmotionWindow : MetroWindow
    {
        private string feedID;
        private PostWindow instance;
        private bool isForceExit = true;

        public EmotionWindow(string feedID, PostWindow instance)
        {
            this.feedID = feedID;
            this.instance = instance;
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = ((Button) sender);
            bool isSucces = await KakaoRequestClass.LikeFeed(feedID, (string) button.Tag);
            instance.Dispatcher.Invoke(() =>
            {
                if (isSucces)
                {
                    instance.TB_LikeBTN.Text = "느낌 취소";
                    instance.IC_LikeBTN.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cancel;
                }
                else
                {
                    instance.TB_LikeBTN.Text = "느낌 달기";
                    instance.IC_LikeBTN.Kind = MaterialDesignThemes.Wpf.PackIconKind.Heart;
                }
                instance.BT_Like.IsEnabled = true;
                instance.data.liked = true;
                isForceExit = false;
                Close();
            });
        }
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isForceExit)
            {
                instance.Dispatcher.Invoke(() =>
                {
                    instance.TB_LikeBTN.Text = "느낌 달기";
                    instance.IC_LikeBTN.Kind = MaterialDesignThemes.Wpf.PackIconKind.Heart;
                    instance.BT_Like.IsEnabled = true;
                });
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

        private void MetroWindow_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is System.Windows.Controls.TextBox))
                e.Handled = true;
        }
    }
}
