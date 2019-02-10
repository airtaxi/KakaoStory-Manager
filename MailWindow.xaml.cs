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
            RA_Loading.Visibility = Visibility.Visible;
            PR_Loading.Visibility = Visibility.Visible;
            Dispatcher.InvokeAsync(async () =>
            {
                await Refresh();
                RA_Loading.Visibility = Visibility.Collapsed;
            });
        }

        public async Task Refresh()
        {
            BT_Refresh.IsEnabled = false;
            PR_Loading.IsActive = true;
            TB_RefreshBT.Text = "갱신중..";
            IC_Refresh.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressClock;
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
                MainWindow.SetClickObject(mc.IMG_Profile);
                mc.IMG_Profile.PreviewMouseLeftButtonDown += (s, e) =>
                {
                    e.Handled = true;
                    try
                    {
                        TimeLineWindow tlw = new TimeLineWindow(mail.sender.id);
                        tlw.Show();
                        tlw.Activate();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("접근이 불가능한 스토리입니다.");
                    }
                };

                if (mail.read_at == null || mail.read_at.Value.Year < 1)
                    mc.RA_BG.Fill = Brushes.Teal;

                mc.Grid.MouseLeftButtonDown += (s, e) =>
                {
                    e.Handled = true;
                    mc.RA_BG.Fill = Brushes.Transparent;
                    var window = new MailReadWindow(mail.id);
                    window.Show();
                };

                mc.IC_Reply.PreviewMouseLeftButtonDown += (s, e) =>
                {
                    e.Handled = true;
                    mc.RA_BG.Fill = Brushes.Transparent;
                    var window = new MailWriteWindow(mail.sender.id, mail.sender.display_name);
                    window.Show();
                };

                MainWindow.SetClickObject(mc.Grid);

                SP_Main.Children.Add(mc);
                var sep = new Separator
                {
                    Foreground = new SolidColorBrush(Color.FromRgb(221, 221, 221))
                };
                SP_Main.Children.Add(sep);
            }
            PR_Loading.IsActive = false;
            BT_Refresh.IsEnabled = true;
            TB_RefreshBT.Text = "새로고침";
            IC_Refresh.Kind = MaterialDesignThemes.Wpf.PackIconKind.Refresh;
        }

        private async void BT_Refresh_Click(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }

        private async void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Close();
            }
            else if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                await Refresh();
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
            MainWindow.MailWindow = null;
        }

        private void BT_Send_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("쪽지 보내기 기능은 현재 베타입니다.\n정상적 작동을 보장하지 않습니다.", "경고");
            MailWriteWindow mrw = new MailWriteWindow();
            mrw.Show();
        }
    }
}
