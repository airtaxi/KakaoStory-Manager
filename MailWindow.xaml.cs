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
    /// MailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MailWindow : MetroWindow
    {
        public MailWindow()
        {
            InitializeComponent();
            if (!Properties.Settings.Default.HideScrollBar)
                SV_Main.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Refresh();
        }

        public async void Refresh()
        {
            var mails = await KakaoRequestClass.GetMails();
            SP_Main.Children.Clear();
            foreach (var mail in mails)
            {
                var mc = new MailControl();
                mc.TB_Name.Text = mail.sender.display_name;
                mc.TB_Content.Text = mail.summary;
                mc.TB_Date.Text = PostWindow.GetTimeString(mail.created_at);

                string imgUri = mail.sender.profile_thumbnail_url;
                if (Properties.Settings.Default.GIFProfile && mail.sender.profile_video_url_square_small != null)
                    imgUri = mail.sender.profile_video_url_square_small;
                GlobalHelper.AssignImage(mc.IMG_Profile, imgUri);

                if (mail.read_at == null || mail.read_at.Value.Year < 1)
                    mc.RA_BG.Fill = Brushes.Teal;

                mc.Grid.PreviewMouseLeftButtonDown += (s, e) =>
                {
                    e.Handled = true;
                    mc.RA_BG.Fill = Brushes.Transparent;
                    var window = new MailReadWindow(mail.id);
                    window.Show();
                    window.Activate();
                };

                MainWindow.SetClickObject(mc.Grid);

                SP_Main.Children.Add(mc);
                var sep = new Separator();
                sep.Foreground = Brushes.Gray;
                SP_Main.Children.Add(sep);
            }
        }

        private void BT_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Close();
                e.Handled = true;
            }
        }
        private void MetroWindow_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is TextBox) && !(e.Source is PasswordBox))
                e.Handled = true;
        }

        private void SV_Main_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalHelper.HandleScroll(sender, e);
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            MainWindow.mailWindow = null;
        }

        private void BT_Send_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("현재 쪽지 보내기 기능은 준비중입니다", "안내");
        }
    }
}
