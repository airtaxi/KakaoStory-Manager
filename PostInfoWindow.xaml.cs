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
        public static void AssignProfile(FriendSelectControl fsc, ShareData.Actor actor, PostInfoWindow instance, string likeID = null)
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
            if(likeID != null && instance.data.actor.id.Equals(MainWindow.UserProfile.id))
            {
                fsc.IC_Delete.Visibility = Visibility.Visible;
                fsc.IC_Delete.PreviewMouseLeftButtonDown += async (s, e) =>
                {
                    e.Handled = true;
                    await KakaoRequestClass.DeleteLike(instance.data.id, likeID);
                    instance.SP_Emotions.Children.Remove(fsc);
                };
            }
        }

        PostData data;
        private string lastShareID;
        private string lastLikeID;
        private string lastUPID;

        public PostInfoWindow(List<ShareData.Share> likes, List<ShareData.Share> shares, List<ShareData.Share> ups, PostData data, int index)
        {
            InitializeComponent();

            if (!Properties.Settings.Default.HideScrollBar)
            {
                SV_Emotions.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                SV_Shares.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                SV_UP.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }

            this.data = data;

            TC_Main.SelectedIndex = index;
            AddLikes(likes);
            AddUps(ups);
            AddShares(shares);

            if(likes.Count > 0)
                lastLikeID = likes.Last().id;
            if(shares.Count > 0)
                lastShareID = shares.Last().id;
            if(ups.Count > 0)
                lastUPID = ups.Last().id;
        }

        private void AddShares(List<ShareData.Share> shares)
        {
            foreach (var share in shares)
            {
                FriendSelectControl fsc = new FriendSelectControl();
                AssignProfile(fsc, share.actor, this);
                fsc.Grid.MouseLeftButtonDown += async (s, e) =>
                {
                    try
                    {
                        PostData pd = await KakaoRequestClass.GetPost(share.activity_id);
                        PostWindow.ShowPostWindow(pd, share.activity_id);
                    }
                    catch (Exception) {}
                    e.Handled = true;
                };
                SP_Shares.Children.Add(fsc);
            }
        }

        private void AddLikes(List<ShareData.Share> likes)
        {
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
                AssignProfile(fsc, like.actor, this, like.id);
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
        }

        private void AddUps(List<ShareData.Share> ups)
        {
            foreach (var up in ups)
            {
                FriendSelectControl fsc = new FriendSelectControl();
                AssignProfile(fsc, up.actor, this);
                fsc.Grid.MouseLeftButtonDown += (s, e) =>
                {
                    TimeLineWindow tlw = new TimeLineWindow(up.actor.id);
                    tlw.Show();
                    tlw.Focus();
                    e.Handled = true;
                };
                SP_UP.Children.Add(fsc);
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

        private bool isUPWorking = false;
        private async void SV_UP_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalHelper.HandleScroll(sender, e);
            if (SV_UP.VerticalOffset == SV_UP.ScrollableHeight && e.Delta < 0 && !isUPWorking)
            {
                isUPWorking = true;
                var ups = await KakaoRequestClass.GetShares(true, data, lastUPID);
                if (ups.Count > 0)
                {
                    lastUPID = ups.Last().id;
                    AddUps(ups);
                }
                isUPWorking = false;
            }
        }

        private bool isShareWorking = false;
        private async void SV_Shares_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalHelper.HandleScroll(sender, e);
            if (SV_Shares.VerticalOffset == SV_Shares.ScrollableHeight && e.Delta < 0 && lastShareID != null && !isShareWorking)
            {
                isShareWorking = true;
                var shares = await KakaoRequestClass.GetShares(false, data, lastShareID);
                if (shares.Count > 0)
                {
                    AddShares(shares);
                    lastShareID = shares.Last().id;
                }
                isShareWorking = false;
            }
        }

        private bool isEmotionWorking = false;
        private async void SV_Emotions_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalHelper.HandleScroll(sender, e);
            if (SV_Emotions.VerticalOffset == SV_Emotions.ScrollableHeight && e.Delta < 0 && !isEmotionWorking)
            {
                isEmotionWorking = true;
                var likes = await KakaoRequestClass.GetLikes(data, lastLikeID);
                if (likes.Count > 0)
                {
                    lastLikeID = likes.Last().id;
                    AddLikes(likes);
                }
                isEmotionWorking = false;
            }
        }
    }
}
