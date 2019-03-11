using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Threading.Tasks;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro;

namespace KSP_WPF
{
    public partial class MainWindow : MetroWindow
    {
        public static bool IsLoggedIn = false;
        public static MainWindow Instance;
        public static string APP_ID = "kck4156.KSP.WPF";

        public bool IsClose = false;
        public static bool NotificationRequested = true;
        public static bool IsOffline = false;
        public static bool IsDND = false;
        public readonly NotifyIcon TrayNotifyIcon = null;
        public static StoryWriteWindow StoryWriteWindow = null;
        public static TimeLineWindow TimeLineWindow = null;
        public static TimeLineWindow ProfileTimeLineWindow = null;
        public static NotificationsWindow NotificationsWindow = null;
        public static SettingsWindow SettingsWindow = null;
        public static FriendSelectWindow FriendListWindow = null;
        public static Dictionary<string, PostWindow> Posts = new Dictionary<string, PostWindow>();
        public static MailWindow MailWindow = null;
        public static ProfileWindow ProfileWindow = null;

        public static FriendData.Friends UserFriends { get; set; }
        public static UserProfile.ProfileData UserProfile { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            TSW_DarkMode.IsChecked = Properties.Settings.Default.DarkMode;
            TSW_DarkMode_Click(null, null);

            Dispatcher.InvokeAsync(async () =>
            {
                if(Environment.OSVersion.Version.Major != 10)
                {
                    bool isLatest = await GlobalHelper.CheckUpdate();
                    if (!isLatest)
                        if (MessageBox.Show("새로운 업데이트를 확인했습니다.\n 업데이트를 받으시겠습니까?", "안내", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                            System.Diagnostics.Process.Start("https://kagamine-rin.com/?p=186");
                }
            });

            CB_AutoLogin.IsChecked = Properties.Settings.Default.AutoLogin;

            Environment.CurrentDirectory = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            TrayNotifyIcon = new NotifyIcon
            {
                Icon = new Icon("icon.ico"),
                Visible = true
            };
            TrayNotifyIcon.MouseDoubleClick += (s, e) =>
            {
                if (Properties.Settings.Default.DefaultMinimize)
                    WindowState = WindowState.Normal;
                else
                    Show();
                
                Activate();
            };
            TrayNotifyIcon.BalloonTipClicked += (s, e) =>
            {
                if(TrayNotifyIcon.Tag != null)
                {
                    KSPNotificationActivator.ActivateHandler((string) TrayNotifyIcon.Tag, null);
                    TrayNotifyIcon.Tag = null;
                }
                else
                {
                    Show();
                    //Activate();
                }
            };

            ContextMenu menu = new ContextMenu();
            MenuItem timeline = new MenuItem
            {
                Index = 0,
                Text = "타임라인"
            };
            timeline.Click += (s, a) =>
            {
                BT_TimeLine_Click(null, null);
            };
            menu.MenuItems.Add(timeline);

            MenuItem write = new MenuItem
            {
                Index = 0,
                Text = "게시글 작성"
            };
            write.Click += (s, a) =>
            {
                BT_Write_Click(null, null);
            };
            menu.MenuItems.Add(write);

            MenuItem notification = new MenuItem
            {
                Index = 0,
                Text = "알림 확인"
            };
            notification.Click += (s, a) =>
            {
                BT_Notifiations_Click(null, null);
            };
            menu.MenuItems.Add(notification);

            MenuItem profile = new MenuItem
            {
                Index = 0,
                Text = "내 프로필"
            };
            profile.Click += (s, a) =>
            {
                BT_MyProfile_Click(null, null);
            };
            menu.MenuItems.Add(profile);

            MenuItem settings = new MenuItem
            {
                Index = 0,
                Text = "설정"
            };
            settings.Click += (s, a) =>
            {
                BT_Settings_Click(null, null);
            };
            menu.MenuItems.Add(settings);

            MenuItem exit = new MenuItem
            {
                Index = 0,
                Text = "종료"
            };
            exit.Click += (s, a) =>
            {
                Hide();
                IsClose = true;
                Environment.Exit(0);
            };
            menu.MenuItems.Add(exit);

            TrayNotifyIcon.ContextMenu = menu;
            
            if (Properties.Settings.Default.AutoLogin)
            {
                TBX_Email.Text = Properties.Settings.Default.AutoEmail;
                TBX_Password.Password = Properties.Settings.Default.AutoPassword;
                Task.Run(() =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        TryLogin();
                    });
                });
            }

            SetClickObject(GD_Friends);
            SetClickObject(GD_Write);
            SetClickObject(GD_Timeline);
            SetClickObject(GD_Notifications);
            SetClickObject(GD_Mail);
            SetClickObject(GD_Settings);
            SetClickObject(GD_Friends);
            SetClickObject(GD_ProfileSettings);
            SetClickObject(BT_Login);
            SetClickObject(EL_Profile);
            SetClickObject(TB_MyProfile);
            SetClickObject(TB_Tray);
            SetClickObject(IMG_Power);

            Dispatcher.InvokeAsync(async() =>
            {
                await KakaoRequestClass.RequestNotification(false);
            });
        }

        public static void ClickEventMouseMove(object s, System.Windows.Input.MouseEventArgs e)
        {
            if(((IInputElement) s).IsEnabled && Mouse.OverrideCursor != System.Windows.Input.Cursors.Hand)
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
        }
        public static void ClickEventMouseLeave(object s, System.Windows.Input.MouseEventArgs e)
        {
            if (((IInputElement)s).IsEnabled)
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }
        public static void SetClickObject(IInputElement element)
        {
            element.MouseMove += ClickEventMouseMove;
            element.MouseLeave += ClickEventMouseLeave;
        }
        public static void UnSetClickObject(IInputElement element)
        {
            element.MouseMove -= ClickEventMouseMove;
            element.MouseLeave -= ClickEventMouseLeave;
        }

        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsClose)
            {
                e.Cancel = true;
                if (Properties.Settings.Default.DefaultMinimize)
                    WindowState = WindowState.Minimized;
                //else
                    //Hide();
                if (!Properties.Settings.Default.Disable_Message && Environment.OSVersion.Version.Major == 10)
                    GlobalHelper.ShowNotification("안내", "프로그램이 최소화됐습니다.\r\n시스템 트레이의 프로그램 아이콘을 더블클릭하여 창을 복구할 수 있습니다.", null);
            }
        }

        public static async Task<bool> UpdateProfile()
        {
            string friendRawData = await KakaoRequestClass.GetFriendData();
            string profileRawData = await KakaoRequestClass.GetProfileData();
            UserFriends = JsonConvert.DeserializeObject<FriendData.Friends>(friendRawData);
            UserProfile = JsonConvert.DeserializeObject<UserProfile.ProfileData>(profileRawData);
            return true;
        }

        public void StartTimer()
        {
            Dispatcher.InvokeAsync(async() =>
            {
                IsLoggedIn = true;
                await UpdateProfile();
                PR_Login.Visibility = Visibility.Collapsed;
                GD_Login.Visibility = Visibility.Collapsed;
                GD_Profile.Visibility = Visibility.Visible;
                TB_Name.Text = UserProfile.display_name;
                TB_Email.Text = TBX_Email.Text;
                TB_Login.Visibility = Visibility.Collapsed;
                TB_LoginProgress.Visibility = Visibility.Collapsed;
                TB_Logout.Visibility = Visibility.Visible;
                IMG_Login.Visibility = Visibility.Collapsed;
                EL_Profile.Fill = new ImageBrush(new BitmapImage(new Uri(UserProfile.profile_image_url)));
                BT_Login.IsEnabled = true;
                if(Properties.Settings.Default.AutoMinimize)
                {
                    TB_Tray_MouseLeftButtonDown(null, null);
                }
            });
        }

        public void Logout()
        {
            if (IsLoggedIn)
                Button_Click(BT_Login, null);
        }

        public void Login()
        {
            if (!IsLoggedIn)
                Button_Click(BT_Login, null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BT_Login.IsEnabled)
            {
                if (!IsLoggedIn)
                {
                    TBX_Email.IsEnabled = false;
                    TBX_Password.IsEnabled = false;
                    TryLogin();
                }
                else
                {
                    TBX_Email.IsEnabled = true;
                    TBX_Password.IsEnabled = true;
                    GD_Login.Visibility = Visibility.Visible;
                    GD_Profile.Visibility = Visibility.Collapsed;
                    TB_Login.Visibility = Visibility.Visible;
                    IMG_Login.Visibility = Visibility.Visible;
                    TB_LoginProgress.Visibility = Visibility.Collapsed;
                    TB_Logout.Visibility = Visibility.Collapsed;
                    BT_Login.IsEnabled = true;
                    IsLoggedIn = false;
                }
            }
        }

        private void TryLogin()
        {
            if (BT_Login.IsEnabled)
            {
                PR_Login.Visibility = Visibility.Visible;
                TB_Login.Visibility = Visibility.Collapsed;
                TB_LoginProgress.Visibility = Visibility.Visible;
                TB_Logout.Visibility = Visibility.Collapsed;
                IMG_Login.Visibility = Visibility.Collapsed;
                BT_Login.IsEnabled = false;
                Instance = this;
                if (Properties.Settings.Default.AutoLogin)
                {
                    Properties.Settings.Default.AutoEmail = TBX_Email.Text;
                    Properties.Settings.Default.AutoPassword = TBX_Password.Password;
                    Properties.Settings.Default.Save();
                }
                
                try
                {
                    WebViewWindow webViewWindow = new WebViewWindow();
                    webViewWindow.Show();
                    webViewWindow.Hide();
                }
                catch (Exception e)
                {
                    TB_Login.Text = "로그인";
                    BT_Login.IsEnabled = true;
                    MessageBox.Show("로그인 에러가 발생했습니다.\n"+ e.StackTrace);
                }
            }
        }

        private void BT_Write_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn && !IsOffline)
                GlobalHelper.ShowOfflineMessage();
            else
            {
                if (StoryWriteWindow == null)
                {
                    StoryWriteWindow = new StoryWriteWindow();
                    StoryWriteWindow.Show();
                    //storyWriteWindow.Activate();
                }
                else
                {
                    StoryWriteWindow.Show();
                    StoryWriteWindow.Activate();
                }
            }
        }

        private async void BT_TimeLine_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn && !IsOffline)
            {
                if (TimeLineWindow == null)
                {
                    TimeLineWindow = new TimeLineWindow
                    {
                        fromMainMenu = true
                    };
                    TimeLineWindow.Show();
                    //timeLineWindow.Activate();
                }
                else
                {
                    await TimeLineWindow.RefreshTimeline(null, true);
                    TimeLineWindow.Show();
                    TimeLineWindow.Activate();
                }
            }
            else
                GlobalHelper.ShowOfflineMessage();
        }

        public async void BT_Notifiations_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn && !IsOffline)
            {
                if (NotificationsWindow == null)
                {
                    NotificationsWindow = new NotificationsWindow();
                    NotificationsWindow.Show();
                    //notificationsWindow.Activate();
                }
                else
                {
                    await NotificationsWindow.Refresh();
                    NotificationsWindow.Show();
                    NotificationsWindow.Activate();
                }
            }
            else
                GlobalHelper.ShowOfflineMessage();
        }
        
        private async void BT_MyProfile_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn && !IsOffline)
            {
                if(ProfileTimeLineWindow == null)
                {
                    var window = new TimeLineWindow(UserProfile.id);
                    window.Show();
                }
                else
                {
                    await ProfileTimeLineWindow.RefreshTimeline(null, true);
                    ProfileTimeLineWindow.Show();
                    ProfileTimeLineWindow.Activate();
                }
            }
            else
                GlobalHelper.ShowOfflineMessage();
        }

        private void BT_Settings_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsWindow == null)
            {
                SettingsWindow = new SettingsWindow();
                SettingsWindow.Show();
                //settingsWindow.Activate();
            }
            else
            {
                SettingsWindow.Show();
                SettingsWindow.Activate();
            }
        }

        private void CB_AutoLogin_Click(object sender, RoutedEventArgs e)
        {
            bool check = (bool)((System.Windows.Controls.CheckBox)sender).IsChecked;
            Properties.Settings.Default.AutoLogin = check;
        }

        private void BT_Friends_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn && !IsOffline)
            {
                if (FriendListWindow == null)
                {
                    FriendListWindow = new FriendSelectWindow(null, true);
                    FriendListWindow.BT_Submit.Visibility = Visibility.Collapsed;
                    FriendListWindow.RD_Submit.Height = new GridLength(0);
                    FriendListWindow.Show();
                    //friendListWindow.Activate();
                }
                else
                {
                    FriendListWindow.Show();
                    FriendListWindow.Activate();
                }
            }
            else
                GlobalHelper.ShowOfflineMessage();
        }

        private void TB_Tray_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.DefaultMinimize)
                WindowState = WindowState.Minimized;
            else
                Hide();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hide();
            IsClose = true;
            Environment.Exit(0);
        }

        private void TB_MyProfile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_MyProfile_Click(null, null);
        }

        private void GD_Friends_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_Friends_Click(null, null);
        }

        private void BT_Login_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button_Click(null, null);
        }

        private void GD_Write_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_Write_Click(null, null);
        }

        private void GD_Timeline_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_TimeLine_Click(null, null);
        }

        private void GD_Notifications_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_Notifiations_Click(null, null);
        }

        private void GD_Settings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_Settings_Click(null, null);
        }

        private void MetroWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Properties.Settings.Default.DefaultMinimize)
                    WindowState = WindowState.Minimized;
                else
                    Hide();

                e.Handled = true;
            }
        }

        private void TBX_Email_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Return)
                TBX_Password.Focus();
        }

        private void TBX_Password_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                Button_Click(null, null);
        }

        private void BT_Memory_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            GC.Collect(2);
            GC.WaitForFullGCComplete();
            GC.Collect();
            GC.Collect(2);
        }

        private void MetroWindow_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is System.Windows.Controls.TextBox) && !(e.Source is System.Windows.Controls.PasswordBox))
                e.Handled = true;
        }

        private void TSW_DarkMode_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DarkMode = TSW_DarkMode.IsChecked == true;
            if (Properties.Settings.Default.DarkMode == true)
                ThemeManager.ChangeAppStyle(System.Windows.Application.Current, ThemeManager.GetAccent("Teal"), ThemeManager.GetAppTheme("BaseDark"));
            else
                ThemeManager.ChangeAppStyle(System.Windows.Application.Current, ThemeManager.GetAccent("Teal"), ThemeManager.GetAppTheme("BaseLight"));
            Properties.Settings.Default.Save();
        }

        private void BT_Favorite_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn && !IsOffline)
                GlobalHelper.ShowOfflineMessage();
            else
            {
                TimeLineWindow.showBookmarkedGlobal = true;
                TimeLineWindow timeLineWindow = new TimeLineWindow(UserProfile.id);
                timeLineWindow.Show();
                timeLineWindow.Activate();
            }
        }

        private void BT_Shortcut_Click(object sender, RoutedEventArgs e)
        {
            ShortcutHelpWindow window = new ShortcutHelpWindow()
            {
                Owner=this
            };
            window.ShowDialog();
        }

        private void GD_Mail_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(MailWindow == null)
            {
                MailWindow = new MailWindow();
                MailWindow.Show();
                //mailWindow.Activate();
            }
            else
            {
                MailWindow.Show();
                MailWindow.Activate();
            }
        }

        private void GD_ProfileSettings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ProfileWindow == null)
            {
                ProfileWindow = new ProfileWindow();
                ProfileWindow.Show();
            }
            else
            {
                ProfileWindow.Show();
                ProfileWindow.Activate();
            }
        }

        private void TSW_DND_Click(object sender, RoutedEventArgs e)
        {
            IsDND = TSW_DND.IsChecked == true;
        }
    }
}
