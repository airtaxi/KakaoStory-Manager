using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KSP_WPF
{
    /// <summary>
    /// FriendSelectWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FriendSelectWindow : MetroWindow
    {
        public List<FriendSelectControl> controls = new List<FriendSelectControl>();
        private readonly Action<List<string>, List<string>> listener;
        private readonly bool isFriendList = false;
        private static FriendSelectWindow instance;
        private struct ProfileData
        {
            public UserNameWithCloseButton control;
            public string id;
            public string name;
            public void Remove(object arg0, dynamic arg1)
            {
                instance.withProfiles.Remove(this);
                instance.SP_WithFriends.Children.Remove(control);
            }
        };
        private readonly List<ProfileData> withProfiles = new List<ProfileData>();

        private bool DoesUserExists(string id)
        {
            foreach(var withProfile in withProfiles)
            {
                if (withProfile.id.Equals(id))
                    return true;
            }
            return false;
        }

        public async void OnGridClick(object s, MouseButtonEventArgs e)
        {
            FriendSelectControl fsc = (FriendSelectControl)s;
            if (!isFriendList)
            {
                string id = (string)fsc.Grid.Tag;
                if(!DoesUserExists(id))
                {
                    var user = await KakaoRequestClass.GetProfile(id);
                    ProfileData profileData = new ProfileData() { id = id , name = user.profile.display_name };
                    UserNameWithCloseButton control = new UserNameWithCloseButton();
                    control.TB_Name.Text = user.profile.display_name;
                    profileData.control = control;
                    control.Margin = new Thickness(0, 0, 5, 0);
                    control.IC_Close.MouseLeftButtonDown += profileData.Remove;
                    withProfiles.Add(profileData);
                    SP_WithFriends.Children.Add(control);
                    SV_WithFriends.ScrollToRightEnd();
                }
            }
            else
            {
                string id = (string)fsc.Grid.Tag;
                TimeLineWindow tlw = new TimeLineWindow(id);
                tlw.Show();
                tlw.Focus();
            }
            e.Handled = true;
        }
        public FriendSelectWindow(Action<List<string>, List<string>> listener, bool isFriendList)
        {
            InitializeComponent();
            this.listener = listener;
            instance = this;

            if (Properties.Settings.Default.HideScrollBar)
                SV_Content.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            if (isFriendList)
            {
                Title = "친구 목록";
                RD_Submit.Height = new GridLength(0);
                RD_WithFriends.Height = new GridLength(0);
            }
            this.isFriendList = isFriendList;
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => TB_Search.Focus()));
        }

        private void ShowFriends(string filter)
        {
            SP_Content.Children.Clear();
            controls.Clear();

            foreach (var friend in MainWindow.userFriends.profiles)
            {
                if (friend.blocked != true && friend.display_name.ToLower().Contains(filter.ToLower()))
                {
                    if (CB_OnlyFavorite.IsChecked == true && !friend.is_favorite)
                        continue;

                    FriendSelectControl fsc = new FriendSelectControl
                    {
                        id = friend.id,
                        name = friend.display_name
                    };
                    MainWindow.SetClickObject(fsc.Grid);
                    string imgUri = friend.profile_thumbnail_url;
                    if (Properties.Settings.Default.GIFProfile && friend.profile_video_url_square_small != null)
                        imgUri = friend.profile_video_url_square_small;
                    GlobalHelper.AssignImage(fsc.IMG_Profile, imgUri);
                    fsc.TB_Name.Text = friend.display_name;
                    fsc.TB_Name.Tag = friend.is_favorite;
                    fsc.Grid.Tag = friend.id;
                    fsc.MouseLeftButtonDown += OnGridClick;
                    fsc.Tag = false;
                    fsc.Background = Brushes.Transparent;
                    controls.Add(fsc);
                    SP_Content.Children.Add(fsc);
                }
            }
            SV_Content.ScrollToTop();
            if(controls.Count == 0)
                TB_Empty.Visibility = Visibility.Visible;
            else
                TB_Empty.Visibility = Visibility.Collapsed;
        }

        private void BT_Submit_Click(object sender, RoutedEventArgs e)
        {
            List<string> ids = new List<string>();
            List<string> names = new List<string>();
            foreach (var withProfile in withProfiles)
            {
                ids.Add(withProfile.id);
                names.Add(withProfile.name);
            }
            listener.Invoke(ids, names);
            Close();
        }
        
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalHelper.HandleScroll(sender, e);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (var control in controls)
            {
                control.Tag = false;
                control.Background = Brushes.Transparent;
            }
            if (isFriendList)
                MainWindow.friendListWindow = null;
        }

        private void TB_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string targetStr = TB_Search.Text;
            if(targetStr.Length > 0)
                ShowFriends(targetStr);
            else
            {
                SP_Content.Children.Clear();
                TB_Empty.Visibility = Visibility.Visible;
            }
        }

        private void TB_Search_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (TB_Search.Text.Length == 0)
            {
                if (TB_Search.IsKeyboardFocused)
                {
                    SP_Content.Children.Clear();
                    TB_Empty.Visibility = Visibility.Visible;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SP_Content.Children.Clear();
        }

        private void CB_OnlyFavorite_Click(object sender, RoutedEventArgs e)
        {
            TB_Search_TextChanged(null, null);
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

        private void SV_WithFriends_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int threshold = 24;
            ScrollViewer scv = (ScrollViewer)sender;
            double target = scv.HorizontalOffset - Math.Min(Math.Max(e.Delta, -threshold), threshold);
            scv.ScrollToHorizontalOffset(target);
            e.Handled = true;
        }
    }
}
