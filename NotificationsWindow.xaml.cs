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
using System.Windows.Threading;

namespace KSP_WPF
{
    /// <summary>
    /// NotificationsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NotificationsWindow : MetroWindow
    {
        private DispatcherTimer refreshTimer;

        public void InitRefreshTImer()
        {
            string text = TB_Refresh.Text;
            if (text.Length == 0)
                text = "3";
            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromSeconds(Math.Max(Double.Parse(text), 3));
            refreshTimer.Tick += (s, e) =>
            {
                if (CB_AutoRefresh.IsChecked == true)
                    BT_Refresh_Click(null, null);
            };
            refreshTimer.Start();
        }

        public NotificationsWindow()
        {
            InitializeComponent();
            InitRefreshTImer();
            MainWindow.SetClickObject(BT_Refresh);
            Dispatcher.InvokeAsync(() =>
            {
                Refresh();
            });
        }

        public async void Refresh()
        {
            BT_Refresh.IsEnabled = false;
            TB_RefreshBT.Text = "갱신중..";
            IC_Refresh.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressClock;
            List<CSNotification> notifications = await KakaoRequestClass.RequestNotification(true);
            SP_Content.Children.Clear();
            foreach (var notification in notifications)
            {
                string thumbnailURL = notification.actor?.profile_thumbnail_url ?? "";
                if (Properties.Settings.Default.GIFProfile && notification.actor?.profile_video_url_square_small != null)
                    thumbnailURL = notification.actor?.profile_video_url_square_small;
                string message = notification.message;
                string timestamp = PostWindow.GetTimeString(notification.created_at);
                NotificationControl content = new NotificationControl();
                MainWindow.SetClickObject(content);
                content.TB_Content.Text = message;
                content.TB_Content.ToolTip = content.TB_Content.Text;
                string contentMessage = notification.content ?? "내용 없음";
                if (contentMessage.Contains("\n"))
                    contentMessage = contentMessage.Split(new string[] { "\n" }, StringSplitOptions.None)[0];

                content.TB_Message.Text = contentMessage;
                content.TB_Message.ToolTip = content.TB_Message.Text;
                if(!notification.is_new)
                {
                    content.GD_Main.Background = Brushes.Transparent;
                }
                if(content.TB_Message.Text.Trim().Length == 0)
                {
                    content.TB_Message.Text = "내용 없음";
                }
                content.TB_Date.Text = timestamp;
                string uri = "https://story.kakao.com/";
                GlobalHelper.AssignImage(content.IMG_Thumbnail, thumbnailURL);
                MainWindow.SetClickObject(content.IMG_Thumbnail);
                content.IMG_Thumbnail.MouseLeftButtonDown += (s, e) =>
                {
                    try
                    {
                        TimeLineWindow tlw = new TimeLineWindow(notification.actor.id);
                        tlw.Show();
                        tlw.Focus();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("접근이 불가능한 스토리입니다.");
                    }
                    e.Handled = true;
                };
                if (notification.scheme.Contains("?profile_id="))
                {
                    var ObjStr = notification.scheme.Split(new string[] { "?profile_id=" }, StringSplitOptions.None);
                    var Profile = ObjStr[1];
                    var Identity = ObjStr[0].Split('.')[1];
                    uri += Profile + "/" + Identity + "!" + ObjStr[0];
                    var feedID = ObjStr[0].Split(new string[] { "activities/" }, StringSplitOptions.None)[1];
                    content.MouseLeftButtonDown += async (s, e) =>
                    {
                        string target = uri.Split(new string[] { "!" }, StringSplitOptions.None)[0];
                        try
                        {
                            var post = await KSPNotificationActivator.GetPost(feedID);
                            PostWindow.ShowPostWindow(post, feedID);
                            content.GD_Main.Background = Brushes.Transparent;
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("접근할 수 없는 포스트입니다.");
                        }
                        e.Handled = true;
                    };
                }
                else if (notification.scheme.Contains("kakaostory://profiles/"))
                {
                    content.MouseLeftButtonDown += (s, e) =>
                    {
                        try
                        {
                            string id = notification.scheme.Replace("kakaostory://profiles/", "");
                            TimeLineWindow tlw = new TimeLineWindow(id);
                            tlw.Show();
                            tlw.Focus();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("접근할 수 없는 스토리입니다.");
                        }
                        e.Handled = true;
                    };
                }
                SP_Content.Children.Add(content);
            }
            BT_Refresh.IsEnabled = true;
            TB_RefreshBT.Text = "새로고침";
            IC_Refresh.Kind = MaterialDesignThemes.Wpf.PackIconKind.Refresh;

            SV_Content.ScrollToTop();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Topmost = CB_Topmost.IsChecked == true;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalHelper.HandleScroll(sender, e);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.notificationsWindow = null;
        }

        private void BT_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Close();
            }
            else if (e.Key == Key.M && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                BT_Menu_Click(null, null);
            }
            else if (e.Key == Key.F5 || (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control))
            {
                Refresh();
            }
            else if (e.Key == Key.Escape)
            {
                FL_Menu.IsOpen = false;
            }
        }

        private void TB_Refresh_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            double period;
            bool isSuccess = Double.TryParse(TB_Refresh.Text, out period);
            if (isSuccess)
                refreshTimer.Interval = TimeSpan.FromSeconds(Math.Max(period, 3));
        }

        private void BT_Menu_Click(object sender, RoutedEventArgs e)
        {
            FL_Menu.IsOpen = !FL_Menu.IsOpen;
        }

        private void MetroWindow_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is System.Windows.Controls.TextBox))
                e.Handled = true;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SP_Content.Children.Clear();
        }
    }
}
