using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// TimeLineWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TimeLineWindow : MetroWindow
    {
        private static readonly Dictionary<string, TimeLineWindow> profiles = new Dictionary<string, TimeLineWindow>();
        public static bool showBookmarkedGlobal = false;
        public bool showBookmarked = false;
        private bool isProfile = false;
        private double lastOffset = -1;
        private string nextRequest = null;
        private DispatcherTimer refreshTimer;
        private string profileID;

        public void InitRefreshTimer()
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

        public TimeLineWindow()
        {
            InitializeComponent();
            InitRefreshTimer();
            Dispatcher.Invoke(async() =>
            {
                if (Properties.Settings.Default.PositionTimelineToTop)
                    Top = 0;
                if (!Properties.Settings.Default.HideScrollBar)
                    SV_Content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                isProfile = false;
                MainWindow.SetClickObject(BT_Refresh);
                MainWindow.SetClickObject(BT_Write);
                MainWindow.SetClickObject(IC_Friend);
                await RefreshTimeline(null, true);
                Show();
                Activate();
            });
        }

        private void ShareContentMouseEvent(object s, MouseButtonEventArgs e)
        {
            e.Handled = true;
            MainContentMouseEvent(s, e);
        }
        private async void MainContentMouseEvent(object s, MouseButtonEventArgs e)
        {
            string id = (string)((FrameworkElement)s).Tag;
            try
            {
                CommentData.PostData data = await KSPNotificationActivator.GetPost(id);
                PostWindow.ShowPostWindow(data, id);
            }
            catch (Exception)
            {
                MessageBox.Show("접근할 수 없는 포스트입니다.");
            }
            e.Handled = true;
        }
        
        public TimeLineWindow(string id)
        {
            InitializeComponent();
            InitRefreshTimer();
            Dispatcher.Invoke(async() =>
            {
                if (showBookmarkedGlobal)
                {
                    showBookmarkedGlobal = false;
                    showBookmarked = true;
                }
                if (Properties.Settings.Default.PositionTimelineToTop)
                    Top = 0;
                if (!Properties.Settings.Default.HideScrollBar)
                    SV_Content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                isProfile = true;
                MainWindow.SetClickObject(BT_Refresh);
                MainWindow.SetClickObject(BT_Write);
                MainWindow.SetClickObject(IC_Friend);
                SP_Content.Margin = new Thickness(0, 10, 0, 0);
                if (id.Equals(MainWindow.userProfile.id) && showBookmarked != true)
                {
                    if (MainWindow.profileTimeLineWindow != null)
                    {
                        isClose = true;
                        MainWindow.profileTimeLineWindow.Show();
                        MainWindow.profileTimeLineWindow.Activate();
                        await MainWindow.profileTimeLineWindow.RefreshTimeline(null, true);
                        return;
                    }
                    else
                    {
                        fromMainMenu = true;
                        MainWindow.profileTimeLineWindow = this;
                    }
                }
                else if(showBookmarked != true)
                {
                    if (profiles.ContainsKey(id))
                    {
                        profiles[id].Show();
                        profiles[id].Activate();
                        await profiles[id].RefreshTimeline(null, true);
                        isClose = true;
                        Close();
                        return;
                    }
                    else
                        profiles.Add(id, this);
                }
                IC_Friend.Kind = MaterialDesignThemes.Wpf.PackIconKind.PersonAdd;
                profileID = id;
                Title = "프로필";
                await RefreshTimeline(null, true);
                CD_Profile.Visibility = Visibility.Visible;
            });
        }
        
        public bool fromMainMenu = false;
        private bool isClose = false;
        private ProfileRelationshipData.ProfileRelationship relationship;
        public bool isScrollOver = false;
        private bool scrollEnd = false;

        public async Task<bool> RefreshTimeline(string from, bool isClear)
        {
            BT_Refresh.IsEnabled = false;
            TB_RefreshBT.Text = "갱신중...";
            IC_Refresh.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressClock;

            List<CommentData.PostData> feeds;
            if (!isProfile)
            {
                TimeLineData.TimeLine feedData = await KakaoRequestClass.GetFeed(from);
                nextRequest = feedData.next_since;
                feeds = feedData.feeds;
            }
            else
            {
                string from2 = from;
                if (showBookmarked)
                    from2 = null;
                var profile = await KakaoRequestClass.GetProfileFeed(profileID, from2);
                relationship = await KakaoRequestClass.GetProfileRelationship(profileID);
                if (profile.profile.bg_image_url != null)
                {
                    string imgUri = profile.profile.profile_image_url2 ?? profile.profile.profile_thumbnail_url;
                    if (Properties.Settings.Default.GIFProfile && profile.profile.profile_video_url_square_small != null)
                        imgUri = profile.profile.profile_video_url_square_small;
                    GlobalHelper.AssignImage(IMG_Profile, imgUri);
                    GlobalHelper.AssignImage(IMG_ProfileBG, profile.profile.bg_image_url);
                    TB_Name.Text = profile.profile.display_name;

                    if (profile.profile.status_objects?.Count > 0)
                        TB_Desc.Text = profile.profile.status_objects?[0]?.message ?? "한줄 소개 없음";
                    else
                        TB_Desc.Text = "한줄 소개 없음";

                    TB_Desc2.Text = profile.mutual_friend?.message ?? "함께 아는 친구 없음";
                    TB_Desc2.Text += "/ " + profile.profile.activity_count.ToString() + "개의 스토리";
                }
                GD_Favorite.Visibility = Visibility.Collapsed;
                if (relationship.relationship.Equals("F"))
                {
                    IC_Friend.Kind = MaterialDesignThemes.Wpf.PackIconKind.AccountMinus;
                    IC_Friend.ToolTip = "친구 삭제";
                    GD_Favorite.Visibility = Visibility.Visible;
                    if (profile.profile.is_favorite)
                        IC_Favorite.Fill = Brushes.OrangeRed;
                    else
                        IC_Favorite.Fill = Brushes.Gray;
                    MainWindow.SetClickObject(IC_Favorite);
                    ICP_Favorite.MouseEnter += (s, e) =>
                    {
                        Mouse.OverrideCursor = Cursors.Hand;
                        e.Handled = true;
                    };
                    async void onMouseDown(object s, MouseButtonEventArgs e)
                    {
                        await KakaoRequestClass.FavoriteRequest(profile.profile.id, profile.profile.is_favorite);
                        await RefreshTimeline(null, true);
                        e.Handled = true;
                    }
                    IC_Favorite.MouseLeftButtonDown += onMouseDown;
                    ICP_Favorite.MouseLeftButtonDown += onMouseDown;
                }
                else if (relationship.relationship.Equals("R"))
                {
                    IC_Friend.Kind = MaterialDesignThemes.Wpf.PackIconKind.AccountRemove;
                    IC_Friend.ToolTip = "친구 신청 취소";
                }
                else if (relationship.relationship.Equals("C"))
                {
                    IC_Friend.Kind = MaterialDesignThemes.Wpf.PackIconKind.AccountQuestionMark;
                    IC_Friend.ToolTip = "친구 신청 처리";
                }
                else if (relationship.relationship.Equals("N"))
                {
                    IC_Friend.Kind = MaterialDesignThemes.Wpf.PackIconKind.AccountPlus;
                    IC_Friend.ToolTip = "친구 추가";
                }
                else
                    IC_Friend.Visibility = Visibility.Collapsed;

                BT_Write.Visibility = Visibility.Collapsed;
                if (profile.activities.Count > 15)
                {
                    nextRequest = profile.activities.Last().id;
                }

                if (MainWindow.userProfile.id.Equals(profileID) && showBookmarked != true)
                {
                    Title = "내 프로필";
                    TB_Desc2.Text = profile.profile.activity_count.ToString() + "개의 스토리";
                }
                else
                    Title = profile.profile.display_name + "님의 프로필";
                if (showBookmarked)
                {
                    Title = "관심글 조회";
                    var bookmarks = await KakaoRequestClass.GetBookmark(profileID, from);
                    var feedsNow = new List<CommentData.PostData>();
                    foreach(var bookmark in bookmarks.bookmarks)
                    {
                        feedsNow.Add(bookmark.activity);
                    }
                    feeds = feedsNow;
                    nextRequest = bookmarks.bookmarks.Last().id;
                }
                else
                    feeds = profile.activities;
            }
            
            if ((isProfile && feeds.Count != 18)) scrollEnd = true;

            if (isClear)
            {
                SP_Content.Children.Clear();
                SV_Content.ScrollToVerticalOffset(0);
            }
            foreach (var feed in feeds)
            {
                if (feed.verb.Equals("post") || feed.verb.Equals("share"))
                {
                    TimeLinePageControl tlp = new TimeLinePageControl();
                    if (Properties.Settings.Default.ScrollTimeline)
                        tlp.SV_Content.MaxHeight = 300;
                    try
                    {
                        RefreshTimeLineFeed(tlp, feed);
                    }
                    catch (Exception e)
                    {
                        tlp.SP_Content.Background = Brushes.DarkRed;
                        tlp.TB_Name.Text = "오류!";
                        tlp.TB_Content.Text = e.StackTrace;
                    }
                    
                    if (feed.scrap != null)
                    {
                        GlobalHelper.RefreshScrap(feed.scrap, tlp.Scrap_Main);
                    }
                    if (feed.media?.Count > 0 && feed.media?[0]?.url_hq != null)
                    {
                        TextBlock videoText = new TextBlock();
                        videoText.Inlines.Add(new Bold(new Run("(클릭하여 비디오 재생)")));
                        MainWindow.SetClickObject(videoText);
                        videoText.MouseLeftButtonDown += (s, e) =>
                        {
                            System.Diagnostics.Process.Start(feed.media?[0]?.url_hq);
                            e.Handled = true;
                        };
                        tlp.SP_Content.Children.Add(videoText);
                    }

                    SP_Content.Children.Add(tlp);

                    MainWindow.SetClickObject(tlp.Card);

                    tlp.Card.Tag = feed.id;
                    tlp.TB_Content.Tag = feed.id;
                    tlp.SP_Content.Tag = feed.id;

                    tlp.Card.MouseLeftButtonDown += MainContentMouseEvent;
                    tlp.TB_Content.MouseRightButtonDown += (s, e) =>
                    {
                        Clipboard.SetDataObject(feed.content);
                        MessageBox.Show("클립보드에 글 내용이 복사됐습니다.");
                        e.Handled = true;
                    };

                    //tlp.SV_Content.PreviewMouseWheel += HandleScroll;

                    if (feed.verb.Equals("share"))
                    {
                        tlp.GD_Share.Visibility = Visibility.Visible;
                        if (feed.@object.deleted == true || feed.@object.blinded == true)
                        {
                            tlp.GD_ShareCount.Visibility = Visibility.Collapsed;
                            tlp.TB_NameShare.Text = "(삭제됨)";
                            tlp.TB_DateShare.Text = "(삭제됨)";
                            tlp.TB_ShareContent.Text = "삭제된 게시글입니다.";
                        }
                        else
                        {
                            tlp.GD_ShareCount.Visibility = Visibility.Visible;
                            tlp.TB_GD_ShareCount.Text = feed.@object.share_count.ToString();
                            tlp.GD_Share.Tag = feed.@object.id;
                            tlp.GD_Share.MouseLeftButtonDown += ShareContentMouseEvent;

                            string imgUri = feed.@object.actor.profile_image_url2 ?? feed.@object.actor.profile_thumbnail_url;
                            if (Properties.Settings.Default.GIFProfile && feed.@object.actor.profile_video_url_square_small != null)
                                imgUri = feed.@object.actor.profile_video_url_square_small;
                            GlobalHelper.AssignImage(tlp.IMG_ProfileShare, imgUri);
                            MainWindow.SetClickObject(tlp.IMG_ProfileShare);

                            tlp.IMG_ProfileShare.Tag = feed.@object.actor.id;
                            tlp.IMG_ProfileShare.MouseLeftButtonDown += GlobalHelper.SubContentMouseEvent;

                            tlp.TB_NameShare.Text = feed.@object.actor.display_name;
                            tlp.TB_DateShare.Text = PostWindow.GetTimeString(feed.@object.created_at);
                            GlobalHelper.RefreshContent(feed.@object.content_decorators, feed.@object.content, tlp.TB_ShareContent);

                            tlp.TB_ShareContent.MouseRightButtonDown += (s, e) =>
                            {
                                Clipboard.SetDataObject(feed.@object.content);
                                MessageBox.Show("클립보드에 공유한 글 내용이 복사됐습니다.");
                                e.Handled = true;
                            };

                            if (feed.@object.media_type != null && feed.@object.media_type.Equals("image"))
                            {
                                Image lastImage = null;
                                foreach (var media in feed.@object.media)
                                {
                                    string uri = media.origin_url;
                                    if (uri.Contains(".gif") && !Properties.Settings.Default.UseGIF)
                                        uri = media.jpg_url;

                                    if (uri != null)
                                    {
                                        Image image = new Image();
                                        image.Tag = new Image[2] { lastImage, null };
                                        if (lastImage != null && lastImage.Tag is Image[])
                                        {
                                            ((Image[])lastImage.Tag)[1] = image;
                                        }
                                        GlobalHelper.AssignImage(image, uri);
                                        image.Stretch = Stretch.UniformToFill;
                                        image.Margin = new Thickness(0, 0, 0, 10);
                                        image.MouseRightButtonDown += GlobalHelper.CopyImageHandler;
                                        image.MouseLeftButtonDown += GlobalHelper.SaveImageHandler;
                                        lastImage = image;
                                        tlp.SP_ShareContent.Children.Add(image);
                                    }
                                }
                            }
                            if (feed.@object.scrap != null)
                            {
                                GlobalHelper.RefreshScrap(feed.@object.scrap, tlp.Scrap_Share);
                            }

                            if (feed.@object.media?.Count > 0 && feed.@object.media?[0]?.url_hq != null)
                            {
                                TextBlock videoText = new TextBlock();
                                videoText.Inlines.Add(new Bold(new Run("(클릭하여 비디오 재생)")));
                                MainWindow.SetClickObject(videoText);
                                videoText.MouseLeftButtonDown += (s, e) =>
                                {
                                    System.Diagnostics.Process.Start(feed.@object.media?[0]?.url_hq);
                                    e.Handled = true;
                                };
                                tlp.SP_ShareContent.Children.Add(videoText);
                            }
                        }
                    }
                    else
                    {
                        tlp.GD_Share.Visibility = Visibility.Collapsed;
                    }

                    //Rectangle rect = new Rectangle
                    //{
                    //    Stroke = Brushes.Black,
                    //    StrokeThickness = 1
                    //};
                    //SP_Content.Children.Add(rect);
                    SP_Content.Children.Add(new Rectangle()
                    {
                        Height = 10,
                        Fill = Brushes.Transparent
                    });
                }
            }
            BT_Refresh.IsEnabled = true;
            TB_RefreshBT.Text = "새로고침";
            IC_Refresh.Kind = MaterialDesignThemes.Wpf.PackIconKind.Refresh;

            TB_Loading.Visibility = Visibility.Collapsed;
            return true;
        }
        
        private void RefreshTimeLineFeed(TimeLinePageControl tlp, CommentData.PostData feed)
        {
            tlp.SP_Comments?.Children?.Clear();
            tlp.SP_Content?.Children?.Clear();
            string imgUri = feed.actor.profile_image_url2 ?? feed.actor.profile_thumbnail_url;
            if (Properties.Settings.Default.GIFProfile && feed.actor.profile_video_url_square_small != null)
                imgUri = feed.actor.profile_video_url_square_small;
            GlobalHelper.AssignImage(tlp.IMG_Profile, imgUri);

            MainWindow.SetClickObject(tlp.IMG_Profile);
            tlp.IMG_Profile.Tag = feed.actor.id;
            tlp.IMG_Profile.MouseLeftButtonDown += GlobalHelper.SubContentMouseEvent;

            tlp.TB_Name.Text = feed.actor.display_name;
            tlp.TB_Date.Text = PostWindow.GetTimeString(feed.created_at);
            GlobalHelper.RefreshContent(feed.content_decorators, feed.content, tlp.TB_Content);
            if (feed.media_type != null && feed.media_type.Equals("image"))
            {
                Image lastImage = null;
                foreach (var media in feed.media)
                {   
                    string uri = media.origin_url;
                    if (uri.Contains(".gif") && !Properties.Settings.Default.UseGIF)
                        uri = media.jpg_url;

                    Image img = new Image();
                    img.Tag = new Image[2] { lastImage, null };
                    if (lastImage != null && lastImage.Tag is Image[])
                    {
                        ((Image[])lastImage.Tag)[1] = img;
                    }
                    GlobalHelper.AssignImage(img, uri);
                    img.Stretch = Stretch.UniformToFill;
                    img.Margin = new Thickness(0, 0, 0, 10);
                    img.MouseRightButtonDown += GlobalHelper.CopyImageHandler;
                    img.MouseLeftButtonDown += GlobalHelper.SaveImageHandler;
                    lastImage = img;
                    tlp.SP_Content.Children.Add(img);
                }
            }

            bool willDisplayInfo = false;

            if (feed.comment_count > 0)
            {
                willDisplayInfo = true;

                tlp.TB_CommentCount.Visibility = Visibility.Visible;
                var txt = new Bold(new Run($" {feed.comment_count.ToString()}  "))
                {
                    FontSize = 12
                };
                tlp.TB_CommentCount.Inlines.Add(txt);
            }
            else
            {
                tlp.TB_CommentCount.Visibility = Visibility.Collapsed;
                tlp.SP_Comments.Margin = new Thickness(0, 0, 0, 0);
            }

            if (feed.like_count > 0)
            {
                willDisplayInfo = true;
                tlp.TB_LikeCount.Visibility = Visibility.Visible;
                var txt = new Bold(new Run($" {feed.like_count.ToString()}  "));
                txt.FontSize = 12;
                tlp.TB_LikeCount.Inlines.Add(txt);
            }
            else
                tlp.TB_LikeCount.Visibility = Visibility.Collapsed;

            int shares = feed.share_count - feed.sympathy_count;
            if (shares > 0)
            {
                willDisplayInfo = true;
                tlp.TB_ShareCount.Visibility = Visibility.Visible;
                var txt = new Bold(new Run($" {shares.ToString()}  "));
                txt.FontSize = 12;
                tlp.TB_ShareCount.Inlines.Add(txt);
            }
            else
                tlp.TB_ShareCount.Visibility = Visibility.Collapsed;

            if (feed.sympathy_count > 0)
            {
                willDisplayInfo = true;
                tlp.TB_UpCount.Visibility = Visibility.Visible;
                var txt = new Bold(new Run($" {feed.sympathy_count.ToString()}  "));
                txt.FontSize = 12;
                tlp.TB_UpCount.Inlines.Add(txt);
            }
            else
                tlp.TB_UpCount.Visibility = Visibility.Collapsed;

            if (!willDisplayInfo)
            {
                tlp.RD_CommentInfos.Height = new GridLength(0);
            }
        }

        public static RoutedEventHandlerInfo[] GetRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
        {
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty(
                "EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);

            var getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod(
                "GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(
                eventHandlersStore, new object[] { routedEvent });

            return routedEventHandlers;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (fromMainMenu)
            {
                if (!isProfile)
                    MainWindow.timeLineWindow = null;
                else
                    MainWindow.profileTimeLineWindow = null;
            }
            else if (isProfile)
            {
                if (profileID != null && !profileID.Equals(MainWindow.userProfile.id))
                {
                    profiles.Remove(profileID);
                }
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Topmost = CB_Topmost.IsChecked == true;
        }
        
        private async void SV_Content_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (!scrollEnd)
            {
                if (scrollViewer.VerticalOffset > Height && lastOffset != scrollViewer.ScrollableHeight && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                {
                    lastOffset = scrollViewer.ScrollableHeight;
                    await RefreshTimeline(nextRequest, Properties.Settings.Default.MemoryControl);
                }
            }
        }

        private void SV_Content_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalHelper.HandleScroll(sender, e);
            //if (!isScrollOver)
            //{
            //    int threshold = 48;
            //    ScrollViewer scrollViewer = (ScrollViewer)sender;
            //    double target = scrollViewer.VerticalOffset - Math.Min(Math.Max(e.Delta, -threshold), threshold);
            //    scrollViewer.ScrollToVerticalOffset(target);
            //}
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isClose)
            {
                Close();
            }
        }

        private async void IC_Friend_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (relationship.relationship.Equals("F"))
            {
                bool isDelete = MessageBox.Show("친구를 삭제하시겠습니까?", "안내", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                if (isDelete)
                {
                    await KakaoRequestClass.DeleteFriend(relationship.id);
                    await RefreshTimeline(null, true);
                }
            }
            else if (relationship.relationship.Equals("R"))
            {
                bool isRequest = MessageBox.Show("이 사용자에게 친구 신청을 보내시겠습니까?", "안내", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                if (isRequest)
                {
                    await KakaoRequestClass.FriendRequest(relationship.id, true);
                    await RefreshTimeline(null, true);
                }
            }
            else if (relationship.relationship.Equals("C"))
            {
                var question = MessageBox.Show("친구 신청을 수락하시겠습니까?", "안내", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(question != MessageBoxResult.Cancel && question != MessageBoxResult.None)
                {
                    bool isDelete = question == MessageBoxResult.No;
                    await KakaoRequestClass.FriendAccept(relationship.id, isDelete);
                    await RefreshTimeline(null, true);
                }
            }
            else if (relationship.relationship.Equals("N"))
            {
                await KakaoRequestClass.FriendRequest(relationship.id, false);
                await RefreshTimeline(null, true);
            }
        }

        private async void BT_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (BT_Refresh.IsEnabled)
                await RefreshTimeline(null, true);
        }

        private void BT_Write_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.storyWriteWindow == null)
            {
                MainWindow.storyWriteWindow = new StoryWriteWindow();
                MainWindow.storyWriteWindow.Show();
                MainWindow.storyWriteWindow.Activate();
            }
            else
            {
                MainWindow.storyWriteWindow.Show();
                MainWindow.storyWriteWindow.Activate();
            }
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
                BT_Refresh_Click(null, null);
            }
            else if (e.Key == Key.Escape)
            {
                FL_Menu.IsOpen = false;
            }
        }

        private void BT_Notification_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.instance.BT_Notifiations_Click(null, null);
        }

        private void BT_Menu_Click(object sender, RoutedEventArgs e)
        {
            FL_Menu.IsOpen = !FL_Menu.IsOpen;
        }
        
        private void TB_Refresh_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            double period;
            bool isSuccess = Double.TryParse(TB_Refresh.Text, out period);
            if (isSuccess)
                refreshTimer.Interval = TimeSpan.FromSeconds(Math.Max(period, 3));
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            refreshTimer.Stop();
            SP_Content.Children.Clear();
        }

        private void MetroWindow_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is System.Windows.Controls.TextBox))
                e.Handled = true;
        }
    }
}
