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
    /// PostInfoWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PostInfoWindow : MetroWindow
    {
        public static void AssignProfile(FriendSelectControl fsc, ShareData.Actor actor)
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
        public PostInfoWindow(List<ShareData.Share> likes, List<ShareData.Share> shares, List<ShareData.Share> ups, int index)
        {
            InitializeComponent();
            if (!Properties.Settings.Default.HideScrollBar)
            {
                SV_Emotions.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                SV_Shares.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                SV_UP.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }

            TC_Main.SelectedIndex = index;
            foreach (var like in likes)
            {
                FriendSelectControl fsc = new FriendSelectControl();
                fsc.GD_Emotion.Visibility = Visibility.Visible;
                if (like.emotion.Equals("like"))
                    fsc.EM_Like.Visibility = Visibility.Visible;
                else if (like.emotion.Equals("good"))
                    fsc.EM_Cool.Visibility = Visibility.Visible;
                else if (like.emotion.Equals("pleasure"))
                    fsc.EM_Pleasure.Visibility = Visibility.Visible;
                else if (like.emotion.Equals("sad"))
                    fsc.EM_Sad.Visibility = Visibility.Visible;
                else if (like.emotion.Equals("cheerup"))
                    fsc.EM_Cheer.Visibility = Visibility.Visible;
                AssignProfile(fsc, like.actor);
                MainWindow.SetClickObject(fsc.Grid);
                fsc.Grid.MouseLeftButtonDown += (s, e) =>
                {
                    TimeLineWindow tlw = new TimeLineWindow(like.actor.id);
                    tlw.Show();
                    tlw.Focus();
                    e.Handled = true;
                };
                SP_Emotions.Children.Add(fsc);
            }
            foreach (var up in ups)
            {
                FriendSelectControl fsc = new FriendSelectControl();
                AssignProfile(fsc, up.actor);
                fsc.Grid.MouseLeftButtonDown += (s, e) =>
                {
                    TimeLineWindow tlw = new TimeLineWindow(up.actor.id);
                    tlw.Show();
                    tlw.Focus();
                    e.Handled = true;
                };
                SP_UP.Children.Add(fsc);
            }
            foreach (var share in shares)
            {
                FriendSelectControl fsc = new FriendSelectControl();
                AssignProfile(fsc, share.actor);
                fsc.Grid.MouseLeftButtonDown += async (s, e) =>
                {
                    try
                    {
                        PostData pd = await KSPNotificationActivator.GetPost(share.activity_id);
                        PostWindow.ShowPostWindow(pd, share.activity_id);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("접근할 수 없는 스토리입니다.");
                    }
                    e.Handled = true;
                };
                SP_Shares.Children.Add(fsc);
            }
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
    }
}
