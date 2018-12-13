using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Windows.Foundation;

namespace KSP_WPF
{
    /// <summary>
    /// SettingsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            var startupTask = Windows.ApplicationModel.StartupTask.GetAsync("KSP-WPF");
            while (startupTask.Status != AsyncStatus.Completed) { }
            startupTask.Completed = (arg1, arg2) =>
            {
                switch (startupTask.GetResults().State)
                {
                    case Windows.ApplicationModel.StartupTaskState.Disabled:
                        BT_Start.Content = "부팅시 자동 시작 설정하기";
                        break;
                    case Windows.ApplicationModel.StartupTaskState.DisabledByUser:
                        BT_Start.IsEnabled = false;
                        BT_Start.Content = "사용자 설정에 의해 비활성화됨";
                        break;
                    case Windows.ApplicationModel.StartupTaskState.Enabled:
                        BT_Start.Content = "부팅시 자동 시작 해제하기";
                        break;
                }
            };
            CB_Mute.IsChecked = Properties.Settings.Default.Disable_Message;
            CB_Like.IsChecked = Properties.Settings.Default.Disable_Like;
            CB_VIP.IsChecked = Properties.Settings.Default.Disable_VIP;
            CB_Memory.IsChecked = Properties.Settings.Default.MemoryControl;
            //CB_ImageClick.IsChecked = Properties.Settings.Default.ImageClick;
            CB_FullScreen.IsChecked = Properties.Settings.Default.FullScreen;
            CB_TimelineScroll.IsChecked = Properties.Settings.Default.ScrollTimeline;
            CB_NoGIF.IsChecked = !Properties.Settings.Default.UseGIF;
            CB_DefaultFriendOnly.IsChecked = Properties.Settings.Default.DefaultFriendOnly;
            CB_HideScrollBar.IsChecked = Properties.Settings.Default.HideScrollBar;
            CB_ShowComment.IsChecked = Properties.Settings.Default.ShowComment;
            CB_PositionPostToTop.IsChecked = Properties.Settings.Default.PositionPostToTop;
            CB_PositionTimelineToTop.IsChecked = Properties.Settings.Default.PositionTimelineToTop;
            CB_DefaultMinimize.IsChecked = Properties.Settings.Default.DefaultMinimize;
            CB_AutoMinimize.IsChecked = Properties.Settings.Default.AutoMinimize;
            if (Properties.Settings.Default.HideScrollBar)
                SV_Main.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.settingsWindow = null;
        }
        private void CB_Click(object sender, RoutedEventArgs e)
        {
            bool check = (bool)((CheckBox)sender).IsChecked;

            if (sender.Equals(CB_VIP))
                Properties.Settings.Default.Disable_VIP = check;
            else if (sender.Equals(CB_Like))
                Properties.Settings.Default.Disable_Like = check;
            else if (sender.Equals(CB_Mute))
                Properties.Settings.Default.Disable_Message = check;
            else if (sender.Equals(CB_Memory))
                Properties.Settings.Default.MemoryControl = check;
            //else if (sender.Equals(CB_ImageClick))
            //    Properties.Settings.Default.ImageClick = check;
            else if (sender.Equals(CB_FullScreen))
                Properties.Settings.Default.FullScreen = check;
            else if (sender.Equals(CB_TimelineScroll))
                Properties.Settings.Default.ScrollTimeline = check;
            else if (sender.Equals(CB_NoGIF))
                Properties.Settings.Default.UseGIF = !check;
            else if (sender.Equals(CB_DefaultFriendOnly))
                Properties.Settings.Default.DefaultFriendOnly = check;
            else if (sender.Equals(CB_HideScrollBar))
            {
                Properties.Settings.Default.HideScrollBar = check;
                SV_Main.VerticalScrollBarVisibility = (check ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto);
            }
            else if (sender.Equals(CB_ShowComment))
                Properties.Settings.Default.ShowComment = check;
            else if (sender.Equals(CB_PositionPostToTop))
                Properties.Settings.Default.PositionPostToTop = check;
            else if (sender.Equals(CB_PositionTimelineToTop))
                Properties.Settings.Default.PositionTimelineToTop = check;
            else if (sender.Equals(CB_DefaultMinimize))
                Properties.Settings.Default.DefaultMinimize = check;
            else if (sender.Equals(CB_AutoMinimize))
                Properties.Settings.Default.AutoMinimize= check;

            Properties.Settings.Default.Save();
        }

        private void BT_Start_Click(object sender, RoutedEventArgs e)
        {
            var startupTask = Windows.ApplicationModel.StartupTask.GetAsync("KSP-WPF");
            while (startupTask.Status != AsyncStatus.Completed) { }
            if (startupTask.GetResults().State == Windows.ApplicationModel.StartupTaskState.Enabled)
            {
                startupTask.GetResults().Disable();
                BT_Start.Content = "부팅시 자동 시작 설정하기";
            }
            else
            {
                var request = startupTask.GetResults().RequestEnableAsync();
                while (request.Status != AsyncStatus.Completed) { }
                var state = startupTask.GetResults().State;
                switch (state)
                {
                    case Windows.ApplicationModel.StartupTaskState.DisabledByUser:
                        MessageBox.Show("사용자 설정에 의해 작업이 비활성화 됐습니다.");
                        BT_Start.IsEnabled = false;
                        BT_Start.Content = "사용자 설정에 의해 비활성화됨";
                        break;
                    case Windows.ApplicationModel.StartupTaskState.Enabled:
                        BT_Start.Content = "부팅시 자동 시작 해제하기";
                        break;
                }
            }
        }
        private void BT_DeleteBlinded_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.IsLoggedIn && !MainWindow.isOffline)
            {
                List<FriendInitData.Friend> blinded = new List<FriendInitData.Friend>();
                foreach (var friend in MainWindow.FriendData.friends)
                {
                    if (friend.blocked == true)
                    {
                        blinded.Add(friend);
                    }
                }
                if (blinded.Count > 0)
                {
                    if (MessageBox.Show($"{blinded.Count.ToString()}명의 제한된 사용자를 삭제하시겠습니까?\n이 작업은 돌이킬 수 없습니다.", "경고", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        ProgressWindow pw = new ProgressWindow(blinded)
                        {
                            Owner = this
                        };
                        pw.TB_Content.Text = "초기화중...";
                        pw.ShowDialog();
                    }
                }
                else
                    MessageBox.Show("친구 목록에 제한된 사용자가 없습니다.");
            }
            else
                MainWindow.ShowOfflineMessage();
        }

        private void BT_Credits_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://kagamine-rin.com/story");
        }

        private void BT_DeletePosts_Click(object sender, RoutedEventArgs e)
        {
            new StoryDeleteWindow()
            {
                Owner = this
            }.ShowDialog();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            TimeLineWindow.HandleScroll(sender, e);
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
