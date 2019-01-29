using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KSP_WPF
{
    /// <summary>
    /// SettingsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public void InitStartOption()
        {
            var startupTask = Windows.ApplicationModel.StartupTask.GetAsync("KSP-WPF");
            while (startupTask.Status != Windows.Foundation.AsyncStatus.Completed) { }
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
        }
        public SettingsWindow()
        {
            InitializeComponent();
            if (Environment.OSVersion.Version.Major == 10)
            {
                BT_StartUp.Visibility = Visibility.Collapsed;
                BT_Homepage.Visibility = Visibility.Collapsed;
                TB_Version.Visibility = Visibility.Collapsed;
                InitStartOption();
            }
            else
            {
                TB_Version.Text = "버전 : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                BT_Start.Visibility = Visibility.Collapsed;
            }
            CB_Mute.IsChecked = Properties.Settings.Default.Disable_Message;
            CB_Like.IsChecked = Properties.Settings.Default.Disable_Like;
            CB_VIP.IsChecked = Properties.Settings.Default.Disable_VIP;
            CB_Memory.IsChecked = Properties.Settings.Default.MemoryControl;
            CB_FullScreen.IsChecked = Properties.Settings.Default.FullScreen;
            CB_TimelineScroll.IsChecked = Properties.Settings.Default.ScrollTimeline;
            CB_NoGIF.IsChecked = !Properties.Settings.Default.UseGIF;
            CB_GIFProfile.IsChecked = Properties.Settings.Default.GIFProfile;
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
            else if (sender.Equals(CB_FullScreen))
                Properties.Settings.Default.FullScreen = check;
            else if (sender.Equals(CB_TimelineScroll))
                Properties.Settings.Default.ScrollTimeline = check;
            else if (sender.Equals(CB_NoGIF))
                Properties.Settings.Default.UseGIF = !check;
            else if (sender.Equals(CB_GIFProfile))
                Properties.Settings.Default.GIFProfile = check;
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
                Properties.Settings.Default.AutoMinimize = check;

            Properties.Settings.Default.Save();
        }

        private void BT_Start_Click(object sender, RoutedEventArgs e)
        {
            if (Environment.OSVersion.Version.Major == 10)
            {
                var startupTask = Windows.ApplicationModel.StartupTask.GetAsync("KSP-WPF");
                while (startupTask.Status != Windows.Foundation.AsyncStatus.Completed) { }
                if (startupTask.GetResults().State == Windows.ApplicationModel.StartupTaskState.Enabled)
                {
                    startupTask.GetResults().Disable();
                    BT_Start.Content = "부팅시 자동 시작 설정하기";
                }
                else
                {
                    var request = startupTask.GetResults().RequestEnableAsync();
                    while (request.Status != Windows.Foundation.AsyncStatus.Completed) { }
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
            GlobalHelper.HandleScroll(sender, e);
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

        private void BT_Homepage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://kagamine-rin.com/");
        }

        private void BT_BatchEditPosts_Click(object sender, RoutedEventArgs e)
        {
            StoryModifyWindow window = new StoryModifyWindow()
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void BT_FriendMenu_Click(object sender, RoutedEventArgs e)
        {
            FriendManageWindow window = new FriendManageWindow()
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void BT_StartUp_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if(registryKey.GetValue("KSP-WPF") == null)
            {
                registryKey.SetValue("KSP-WPF", System.Windows.Forms.Application.ExecutablePath);
                MessageBox.Show("자동 시작 프로그램에 등록됐습니다.", "정보");
            }
            else
            {
                registryKey.DeleteValue("KSP-WPF");
                MessageBox.Show("프로그램이 자동 시작 프로그램에서 삭제됐습니다.", "정보");
            }
        }
    }

}
