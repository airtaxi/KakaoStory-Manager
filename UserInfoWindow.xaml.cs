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
using static KSP_WPF.CommentData;

namespace KSP_WPF
{
    /// <summary>
    /// UserInfoWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserInfoWindow : MetroWindow
    {
        public static void AssignProfile(FriendSelectControl fsc, ProfileData.Profile actor)
        {
            fsc.MouseEnter += (s, e) =>
            {
                fsc.Background = Brushes.LightGray;
            };
            fsc.MouseLeave += (s, e) =>
            {
                fsc.Background = Brushes.Transparent;
            };
            fsc.TB_Name.Text = actor.display_name;
            string imgUri = actor.profile_thumbnail_url;
            if (Properties.Settings.Default.GIFProfile && actor.profile_video_url_square_small != null)
                imgUri = actor.profile_video_url_square_small;
            GlobalHelper.AssignImage(fsc.IMG_Profile, imgUri);
            fsc.IMG_Profile.Tag = actor.id;
            fsc.IMG_Profile.MouseLeftButtonDown += GlobalHelper.SubContentMouseEvent;

            MainWindow.SetClickObject(fsc.Grid);
            if (actor.relationship.Equals("N"))
            {
                fsc.IC_Friend.Visibility = Visibility.Visible;
                MainWindow.SetClickObject(fsc.IC_Friend);
                fsc.IC_Friend.MouseLeftButtonDown += async (s, e) =>
                {
                    fsc.IC_Friend.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressClock;
                    await KakaoRequestClass.FriendRequest(actor.id, false);
                    fsc.IC_Friend.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressCheck;
                    fsc.IC_Friend.IsEnabled = false;
                    e.Handled = true;
                };
            }
            if (actor.relationship.Equals("R"))
            {
                fsc.IC_Friend.Visibility = Visibility.Visible;
                fsc.IC_Friend.Kind = MaterialDesignThemes.Wpf.PackIconKind.PersonAdd;
                fsc.IC_Friend.Foreground = Brushes.OrangeRed;
            }
        }

        public UserInfoWindow(List<string> ids)
        {
            InitializeComponent();

            if (!Properties.Settings.Default.HideScrollBar)
                SV_Main.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            Dispatcher.Invoke(async () =>
            {
                List<ProfileData.Profile> profiles = new List<ProfileData.Profile>();
                foreach (string id in ids)
                {
                    var user = await KakaoRequestClass.GetProfile(id);
                    profiles.Add(user.profile);
                }
                AddProfile(profiles);
            });
        }

        private void AddProfile(List<ProfileData.Profile> profiles)
        {
            foreach (var profile in profiles)
            {
                FriendSelectControl fsc = new FriendSelectControl();
                AssignProfile(fsc, profile);
                fsc.Grid.MouseLeftButtonDown += (s, e) =>
                {
                    TimeLineWindow tlw = new TimeLineWindow(profile.id);
                    tlw.Show();
                    tlw.Focus();
                    e.Handled = true;
                };
                SP_Main.Children.Add(fsc);
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
