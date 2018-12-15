using MahApps.Metro.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Foundation;
using Windows.Storage;
using XamlAnimatedGif;
using static KSP_WPF.CommentData;

namespace KSP_WPF
{
    /// <summary>
    /// PostWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PostWindow : MetroWindow
    {
        public PostData data;
        private string feedID;
        private bool isAllRead = true;
        UploadedImageProp commentImage = null;
        private bool isVideo = false;
        private string lastVideo;
        private readonly Hashtable ht = new Hashtable();

        private void AddComment(string path, string name, string message, string uri, string commentID, string commentName, bool liked, string id, string imageUri, string timestamp, int likes, Comment commentProf)
        {
            CommentControl comment = new CommentControl();
            comment.TB_Name.Text = name;
            comment.TB_Content.Tag = message;
            foreach (var decorator in commentProf.decorators)
            {
                if (decorator.type.Equals("profile"))
                {
                    Bold content = new Bold(new Run(decorator.text));
                    MainWindow.SetClickObject(content);
                    content.MouseLeftButtonDown += (s, e) =>
                    {
                        TimeLineWindow tlw = new TimeLineWindow(decorator.id);
                        tlw.Show();
                        tlw.Focus();
                        e.Handled = true;
                    };
                    comment.TB_Content.Inlines.Add(content);
                }
                if (decorator.type.Equals("text") || decorator.type.Equals("emoticon"))
                {
                    comment.TB_Content.Inlines.Add(new Run(decorator.text.Replace("\\n", "\n")));
                }
            }

            comment.TB_Content.MouseRightButtonDown += (s, e) =>
            {
                Clipboard.SetDataObject((string)comment.TB_Content.Tag);
                MessageBox.Show("클립보드에 댓글 내용이 복사됐습니다.");
                e.Handled = true;
            };
            if (likes > 0)
                comment.TB_Like.Text = $"좋아요 {likes.ToString()}개";

            comment.TB_MetaData.Text = timestamp;
            string imgUri = commentProf.writer.profile_image_url ?? commentProf.writer.profile_thumbnail_url;
            //string imgUri = commentProf.writer.profile_video_url_square_small ?? commentProf.writer.profile_image_url ?? commentProf.writer.profile_thumbnail_url;
            GlobalHelper.AssignImage(comment.IMG_Profile, imgUri);

            MainWindow.SetClickObject(comment.IMG_Profile);
            comment.IMG_Profile.MouseLeftButtonDown += (s, e) =>
            {
                try
                {
                    TimeLineWindow tlw = new TimeLineWindow(commentID);
                    tlw.Show();
                    tlw.Focus();
                    e.Handled = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("접근이 불가능한 스토리입니다.");
                }
            };

            if (imageUri != null)
            {
                comment.IMG_Comment.Visibility = Visibility.Visible;
                GlobalHelper.AssignImage(comment.IMG_Comment, imageUri);
                comment.IMG_Comment.MouseRightButtonDown += MainWindow.CopyImageHandler;
                comment.IMG_Comment.MouseLeftButtonDown += MainWindow.SaveImageHandler;
            }
            else
                comment.IMG_Comment.Visibility = Visibility.Collapsed;

            if (liked)
                comment.IC_Like.Foreground = Brushes.Red;
            
            bool deletable = commentID.Equals(MainWindow.FriendData.profile.id) || data.actor.id.Equals(MainWindow.FriendData.profile.id);
            bool editable = commentID.Equals(MainWindow.FriendData.profile.id);

            if (!deletable)
                comment.BT_Delete.IsEnabled = false;

            comment.BT_Delete.Click += async (s, e) =>
            {
                await KakaoRequestClass.DeleteComment(id, data);
                await RenewComment();
            };

            if (editable)
            {
                comment.BT_Edit.Click += (s, e) =>
                {
                    CommentEditWindow cew = new CommentEditWindow
                    {
                        Owner = this
                    };
                    cew.TB_Comment.Text = GlobalHelper.GetStringFromQuoteData(commentProf.decorators, true);
                    async void OnButtonClick(object s2, RoutedEventArgs e2)
                    {
                        Button button = (Button)s2;
                        button.Content = "...";
                        button.IsEnabled = false;
                        await KakaoRequestClass.EditComment(commentProf, cew.TB_Comment.Text, data);
                        Refresh();
                        cew.Close();
                    }
                    RoutedEventHandler handler = new RoutedEventHandler(OnButtonClick);
                    cew.TB_Comment.PreviewKeyDown += (s2, e2) =>
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Control && e2.Key == Key.Enter)
                        {
                            handler(cew.BT_Submit, null);
                        }
                    };
                    cew.BT_Submit.Click += handler;
                    cew.BT_Quote.Click += (s2, e2) =>
                    {
                        FriendSelectWindow fsc = new FriendSelectWindow((ids, names) =>
                        {
                            for (int i = 0; i < ids.Count; i++)
                            {
                                string append = "{!{{" + "\"id\":\"" + ids[i] + "\", \"type\":\"profile\", \"text\":\"" + names[i] + "\"}}!} ";
                                if (i + 1 < ids.Count)
                                {
                                    append += ", ";
                                }
                                int lastPos = cew.TB_Comment.CaretIndex;
                                cew.TB_Comment.Text = cew.TB_Comment.Text.Insert(cew.TB_Comment.CaretIndex, append);
                                cew.TB_Comment.CaretIndex = lastPos + append.Length;
                                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => cew.TB_Comment.Focus()));
                            }
                        }, false)
                        {
                            Owner = cew
                        };
                        fsc.ShowDialog();
                    };
                    cew.ShowDialog();
                };
            }
            else
                comment.BT_Edit.IsEnabled = false;

            MainWindow.SetClickObject(comment.BT_Like);
            comment.BT_Like.PreviewMouseLeftButtonDown += async (s, e) =>
            {
                e.Handled = true;
                if (comment.BT_Like.IsEnabled)
                {
                    comment.IC_Like.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressClock;
                    comment.IC_Like.Foreground = Brushes.Gray;
                    comment.BT_Like.IsEnabled = false;
                    await KakaoRequestClass.LikeComment(feedID, id, liked);
                    await RenewComment();
                }
            };

            comment.Grid.MouseEnter += (s, e) =>
            {
                comment.Grid.Background = Brushes.LightSkyBlue;
            };
            comment.Grid.MouseLeave += (s, e) =>
            {
                comment.Grid.Background = Brushes.Transparent;
            };
            comment.Grid.MouseLeftButtonDown += (s, e) =>
            {
                string append = "{!{{" + "\"id\":\"" + commentProf.writer.id + "\", \"type\":\"profile\", \"text\":\"" + commentProf.writer.display_name + "\"}}!} ";
                int lastPos = TB_Comment.CaretIndex;
                TB_Comment.Text = TB_Comment.Text.Insert(TB_Comment.CaretIndex, append);
                TB_Comment.CaretIndex = lastPos + append.Length;
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => TB_Comment.Focus()));
            };
            SP_Comment.Children.Add(comment);
            Rectangle sep = new Rectangle
            {
                Stroke = Brushes.LightGray,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            SP_Comment.Children.Add(sep);
        }

        public static void ShowPostWindow(PostData data, string feedID)
        {
            if (MainWindow.posts.ContainsKey(feedID))
            {
                PostWindow postWindow = MainWindow.posts[feedID];
                postWindow.Refresh();
                postWindow.Show();
                postWindow.Activate();
                postWindow.SV_Comment.ScrollToEnd();
                postWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => postWindow.TB_Comment.Focus()));
            }
            else
            {
                PostWindow postWindow = new PostWindow(data, feedID);
                postWindow.Show();
                postWindow.Activate();
                postWindow.SV_Comment.ScrollToEnd();
                if(Properties.Settings.Default.PositionPostToTop)
                    postWindow.Top = 0;
                postWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => postWindow.TB_Comment.Focus()));
            }
        }

        public PostWindow(PostData data, string feedID)
        {
            InitializeComponent();
            Dispatcher.InvokeAsync(async () =>
            {
                if (!Properties.Settings.Default.ShowComment)
                    IC_CommentShow_PreviewMouseLeftButtonDown(null, null);
                if (!Properties.Settings.Default.HideScrollBar)
                {
                    SV_Content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    SV_Comment.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                MainWindow.SetClickObject(BT_Upload);
                MainWindow.SetClickObject(BT_Quote);
                MainWindow.SetClickObject(BT_SubmitComment);
                MainWindow.SetClickObject(IC_CommentShow);
                MainWindow.posts.Add(feedID, this);
                this.data = data;
                this.feedID = feedID;
                ht.Add("like", "좋아요");
                ht.Add("good", "멋져요");
                ht.Add("pleasure", "기뻐요");
                ht.Add("sad", "슬퍼요");
                ht.Add("cheerup", "힘내요");
                if (Properties.Settings.Default.FullScreen)
                {
                    WindowState = WindowState.Maximized;
                }
                if (data.liked)
                {
                    TB_LikeBTN.Text = "느낌 취소";
                    IC_LikeBTN.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cancel;
                }
                //TB_MustRead.Text = "";
                //TB_MustRead.Text += data.with_me ? "필독 친구로 설정됨" : "";
                TB_Name.Text = data.actor.display_name;
                GlobalHelper.RefreshContent(data.content_decorators, data.content, TB_Content);
                RefreshImage();
                TB_Content.MouseRightButtonDown += (s, e) =>
                {
                    Clipboard.SetDataObject((string)((object[])TB_Content.Tag)[1]);
                    MessageBox.Show("클립보드에 글 내용이 복사됐습니다.");
                    e.Handled = true;
                };
                await UpdateStats();

                if (data.content.Length == 0)
                {
                    TB_Content.Visibility = Visibility.Collapsed;
                }

                if (!data.actor.id.Equals(MainWindow.FriendData.profile.id))
                    BT_Delte.IsEnabled = false;
                if (!data.actor.id.Equals(MainWindow.FriendData.profile.id))
                    BT_Edit.IsEnabled = false;

                if (data.actor.profile_image_url != null) { }
                //string imgUri = data.actor.profile_video_url_square_small ?? data.actor.profile_image_url ?? data.actor.profile_thumbnail_url;
                string imgUri = data.actor.profile_thumbnail_url;
                GlobalHelper.AssignImage(IMG_Profile, imgUri);

                MainWindow.SetClickObject(IMG_Profile);
                IMG_Profile.MouseLeftButtonDown += (s, e) =>
                {
                    try
                    {
                        TimeLineWindow tlw = new TimeLineWindow(data.actor.id);
                        tlw.Show();
                        tlw.Focus();
                        e.Handled = true;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("접근이 불가능한 스토리입니다.");
                    }
                };
                RefreshComment(data.comments);

                if (data.verb != null && data.verb.Equals("share"))
                {
                    if (data.@object.deleted == true || data.@object.blinded == true)
                    {
                        GD_Share.Visibility = Visibility.Visible;
                        GD_ShareCount.Visibility = Visibility.Collapsed;
                        TB_NameShare.Text = "(삭제됨)";
                        TB_DateShare.Text = "(삭제됨)";
                        TB_ShareContent.Text = "삭제된 게시글입니다.";
                    }
                    else
                    {

                        GD_Share.Visibility = Visibility.Visible;
                        GD_ShareCount.Visibility = Visibility.Visible;
                        TB_GD_ShareCount.Text = data.@object.share_count.ToString();
                        MainWindow.SetClickObject(SP_Share);
                        SP_Share.MouseLeftButtonDown += async (s, e) =>
                        {
                            e.Handled = true;
                            try
                            {
                                PostData feed = await KSPNotificationActivator.GetPost(data.@object.id);
                                ShowPostWindow(feed, data.@object.id);
                                e.Handled = true;
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("접근할 수 없는 포스트입니다.");
                            }
                        };
                        string imgUri2 = data.@object.actor.profile_image_url;
                        //string imgUri2 = data.@object.actor.profile_video_url_square_small ?? data.@object.actor.profile_image_url;
                        GlobalHelper.AssignImage(IMG_ProfileShare, imgUri2);

                        MainWindow.SetClickObject(IMG_ProfileShare);

                        IMG_ProfileShare.MouseLeftButtonDown += (s, e) =>
                        {
                            try
                            {
                                TimeLineWindow tlw = new TimeLineWindow(data.@object.actor.id);
                                tlw.Show();
                                tlw.Focus();
                                e.Handled = true;
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("접근이 불가능한 스토리입니다.");
                            }
                        };

                        TB_NameShare.Text = data.@object.actor.display_name;
                        TB_DateShare.Text = GetTimeString(data.@object.created_at);
                        TB_ShareContent.Text = data.@object.content;
                        GlobalHelper.RefreshContent(data.@object.content_decorators, data.@object.content, TB_ShareContent);
                        TB_ShareContent.MouseRightButtonDown += (s, e) =>
                        {
                            Clipboard.SetDataObject(TB_ShareContent.Text);
                            MessageBox.Show("클립보드에 공유한 글 내용이 복사됐습니다.");
                            e.Handled = true;
                        };
                        
                        if (data.@object.media_type != null && data.@object.media_type.Equals("image"))
                        {
                            bool isFirst = true;
                            foreach (var media in data.@object.media)
                            {
                                string uri = media.origin_url;
                                if (uri != null)
                                {
                                    Image image = new Image();
                                    GlobalHelper.AssignImage(image, uri);
                                    image.Stretch = Stretch.UniformToFill;
                                    if (!isFirst)
                                        image.Margin = new Thickness(0, 10, 0, 10);
                                    isFirst = false;
                                    image.MouseRightButtonDown += MainWindow.CopyImageHandler;
                                    image.MouseLeftButtonDown += MainWindow.SaveImageHandler;
                                    SP_ShareContent.Children.Add(image);
                                }
                            }
                            SP_ShareContent.Visibility = Visibility.Visible;
                        }

                        if (data.@object.scrap != null)
                        {
                            GlobalHelper.RefreshScrap(data.@object.scrap, Scrap_Share);
                        }
                        else if (data.@object.media?.Count > 0 && data.@object.media?[0]?.url_hq != null)
                        {
                            TextBlock videoText = new TextBlock();
                            videoText.Inlines.Add(new Bold(new Run("(클릭하여 비디오 재생)")));
                            MainWindow.SetClickObject(videoText);
                            videoText.MouseLeftButtonDown += (s, e) =>
                            {
                                System.Diagnostics.Process.Start(data.@object.media?[0]?.url_hq);
                                e.Handled = true;
                            };
                            SP_ShareContent.Children.Add(videoText);
                            SP_ShareContent.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                    GD_Share.Visibility = Visibility.Collapsed;

                if (!data.sharable || (data.@object?.sharable == false))
                {
                    BT_Share.IsEnabled = false;
                    BT_UP.IsEnabled = false;
                }
                else if (data.sympathized)
                {
                    BT_UP.Content = "UP 취소";
                }
                if (!data.permission.Equals("A"))
                    isAllRead = false;

                if (data.scrap != null)
                {
                    GlobalHelper.RefreshScrap(data.scrap, Scrap_Main);
                }
                ShiftWindowOntoScreenHelper.ShiftWindowOntoScreen(this);
            });
        }

        private void RefreshImage()
        {
            SP_Content.Children.Clear();
            bool isFirst = true;
            if (data.media_type != null && data.media != null && data.media_type.Equals("image"))
            {
                foreach (var media in data.media)
                {
                    string uri = media.origin_url;
                    if (uri != null)
                    {
                        Image image = new Image();
                        GlobalHelper.AssignImage(image, uri);
                        image.Stretch = Stretch.UniformToFill;
                        if (!isFirst)
                            image.Margin = new Thickness(0, 10, 0, 10);
                        isFirst = false;
                        image.MouseRightButtonDown += MainWindow.CopyImageHandler;
                        image.MouseLeftButtonDown += MainWindow.SaveImageHandler;
                        SP_Content.Children.Add(image);
                        SP_Content.Visibility = Visibility.Visible;
                    }
                }
            }

            if (data.media?.Count > 0 && data.media?[0]?.url_hq != null)
            {
                TextBlock videoText = new TextBlock();
                videoText.Inlines.Add(new Bold(new Run("(클릭하여 비디오 재생)")));
                MainWindow.SetClickObject(videoText);
                videoText.MouseLeftButtonDown += (s, e) =>
                {
                    System.Diagnostics.Process.Start(data.media?[0]?.url_hq);
                    e.Handled = true;
                };
                SP_Content.Children.Add(videoText);
                SP_Content.Visibility = Visibility.Visible;
                isVideo = true;
            }
        }

        public void ValidateCommentGrid()
        {
            if (data.comment_count == 0)
                TB_CommentInfo.Visibility = Visibility.Visible;
            else
                TB_CommentInfo.Visibility = Visibility.Collapsed;
        }

        public async void Refresh()
        {
            data = await KSPNotificationActivator.GetPost(feedID);
            if (!data.permission.Equals("A"))
                isAllRead = false;
            GlobalHelper.RefreshContent(data.content_decorators, data.content, TB_Content);
            await RenewComment();
            RefreshImage();
            ShiftWindowOntoScreenHelper.ShiftWindowOntoScreen(this);
        }

        
        private async void OnEmotionClick(object sender, MouseButtonEventArgs e)
        {
            TextBlock source = (TextBlock)sender;
            source.Tag = true;
            data = await KSPNotificationActivator.GetPost(feedID);
            var shares = await KakaoRequestClass.GetShares(false, data);
            var ups = await KakaoRequestClass.GetShares(true, data);
            int index = 0;
            if (source.Equals(TB_LikeCount))
                index = 0;
            else if (source.Equals(TB_ShareCount))
                index = 1;
            if (source.Equals(TB_UpCount))
                index = 2;
            PostInfoWindow piw = new PostInfoWindow(data.likes, shares, ups, index);
            piw.Show();
            piw.Focus();
            e.Handled = true;
        }


        public static string GetTimeString(DateTime created_at)
        {
            int offset = DateTimeOffset.Now.Offset.Hours;
            string dateText = created_at.AddHours(offset).ToString();
            var diffTime = DateTime.Now.Subtract(created_at.AddHours(offset));
            if (diffTime.TotalSeconds < 60)
            {
                dateText = "방금 전";
            }
            else if (diffTime.TotalMinutes < 60)
            {
                dateText = ((int) diffTime.TotalMinutes).ToString() + "분 전";
            }
            else if (diffTime.TotalHours < 24)
            {
                dateText = ((int) diffTime.TotalHours).ToString() + "시간 전";
            }
            return dateText;
        }

        private async Task<bool> UpdateStats()
        {
            data = await KSPNotificationActivator.GetPost(feedID);

            TB_Date.Text = GetTimeString(data.created_at);
            if (data.content_updated_at != null && data.content_updated_at.Year > 1)
                TB_Date.Text += " (수정됨)";

            TB_CommentCount.Inlines.Clear();
            TB_LikeCount.Inlines.Clear();
            TB_ShareCount.Inlines.Clear();
            TB_UpCount.Inlines.Clear();
            ValidateCommentGrid();
            if (data.comment_count > 0)
            {
                TB_CommentCount.Visibility = Visibility.Visible;
                TB_CommentCount.Inlines.Add(new Run($"댓글"));
                TB_CommentCount.Inlines.Add(new Bold(new Run($" {data.comment_count.ToString()}  ")));
            }
            else
                TB_CommentCount.Visibility = Visibility.Collapsed;

            if (data.like_count > 0)
            {
                TB_LikeCount.Visibility = Visibility.Visible;
                TB_LikeCount.Inlines.Add(new Run($"느낌"));
                TB_LikeCount.Inlines.Add(new Bold(new Run($" {data.like_count.ToString()}  ")));
            }
            else
                TB_LikeCount.Visibility = Visibility.Collapsed;

            if (data.share_count - data.sympathy_count > 0)
            {
                TB_ShareCount.Visibility = Visibility.Visible;
                TB_ShareCount.Inlines.Add(new Run($"공유"));
                TB_ShareCount.Inlines.Add(new Bold(new Run($" {(data.share_count - data.sympathy_count).ToString()}  ")));
            }
            else
                TB_ShareCount.Visibility = Visibility.Collapsed;

            if (data.sympathy_count > 0)
            {
                TB_UpCount.Visibility = Visibility.Visible;
                TB_UpCount.Inlines.Add(new Run($"UP"));
                TB_UpCount.Inlines.Add(new Bold(new Run($" {data.sympathy_count.ToString()}  ")));
            }
            else
                TB_UpCount.Visibility = Visibility.Collapsed;


            if (data.like_count > 0 && (bool?)TB_LikeCount.Tag != true)
            {
                TB_LikeCount.Tag = true;
                MainWindow.SetClickObject(TB_LikeCount);
                TB_LikeCount.MouseLeftButtonDown += OnEmotionClick;
            }

            if (data.bookmarked)
            {
                IC_Favorite.Kind = MaterialDesignThemes.Wpf.PackIconKind.StarOff;
                TB_Favorite.Text = "관심글 취소";
            }
            else
            {
                IC_Favorite.Kind = MaterialDesignThemes.Wpf.PackIconKind.Star;
                TB_Favorite.Text = "관심글 추가";
            }

                if (!data.verb.Equals("share"))
            {
                if (data.share_count - data.sympathy_count > 0 && (bool?)TB_ShareCount.Tag != true)
                {
                    TB_ShareCount.Tag = true;
                    MainWindow.SetClickObject(TB_ShareCount);
                    TB_ShareCount.MouseLeftButtonDown += OnEmotionClick;
                }
                if (data.sympathy_count > 0 && (bool?)TB_UpCount.Tag != true)
                {
                    TB_UpCount.Tag = true;
                    MainWindow.SetClickObject(TB_UpCount);
                    TB_UpCount.MouseLeftButtonDown += OnEmotionClick;
                }
            }
            else if(data.verb.Equals("share"))
            {
                BT_Share.IsEnabled = false;
                BT_UP.IsEnabled = false;
            }
            return true;
        }

        private void RefreshComment(List<Comment> comments)
        {
            if (comments.Count == 0)
                TB_CommentInfo.Text = "댓글이 없습니다";

            Dispatcher.InvokeAsync(() =>
            {
                foreach (Comment comment in comments)
                {
                    string imageUri = null;
                    foreach (var decorator in comment.decorators)
                    {
                        if (decorator.type.Equals("image"))
                        {
                            imageUri = decorator.media?.origin_url;
                        }
                    }
                    AddComment("", comment.writer.display_name, comment.text, comment.writer.profile_thumbnail_url, comment.writer.id, comment.writer.display_name, comment.liked, comment.id, imageUri, GetTimeString(comment.created_at), comment.like_count, comment);
                }
            });
        }

        private async void BT_SubmitComent_Click(object sender, RoutedEventArgs e)
        {
            if (BT_Upload.IsEnabled && (TB_Comment.Text.Length > 0 || commentImage != null) && BT_SubmitComment.IsEnabled)
            {
                BT_SubmitComment.Foreground = Brushes.LightGray;
                BT_SubmitComment.IsEnabled = false;
                string text = TB_Comment.Text;
                TB_Comment.Text = "";
                await KakaoRequestClass.ReplyToFeed(feedID, text, null, null, commentImage);
                await RenewComment();
                commentImage = null;
                BT_Upload.Foreground = Brushes.Gray;
                BT_SubmitComment.Foreground = Brushes.Gray;
                BT_SubmitComment.IsEnabled = true;
                await UpdateStats();
                SV_Comment.ScrollToEnd();
            }
            else if (BT_Upload.IsEnabled == false)
            {
                MessageBox.Show("업로드중입니다.\n잠시 후 다시 시도해주세요.");
            }
            else if (TB_Comment.Text.Length == 0 && commentImage == null)
            {
                MessageBox.Show("댓글 내용을 입력해주세요.");
            }
        }

        private async Task<bool> RenewComment()
        {
            data = await KSPNotificationActivator.GetPost(feedID);
            SP_Comment.Children.Clear();
            RefreshComment(data.comments);
            await UpdateStats();
            return true;
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalHelper.HandleScroll(sender, e);
        }

        private async void BT_CommentRefresh_Click(object sender, RoutedEventArgs e)
        {
            TB_Refresh.Text = "갱신중..";
            IC_Refresh.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressClock;
            BT_CommentRefresh.IsEnabled = false;
            await RenewComment();
            TB_Refresh.Text = "새로고침";
            IC_Refresh.Kind = MaterialDesignThemes.Wpf.PackIconKind.Refresh;
            BT_CommentRefresh.IsEnabled = true;
        }

        private async void BT_Share_Click(object sender, RoutedEventArgs e)
        {
            string feedIDNow = feedID;
            if (data.verb.Equals("share"))
            {
                feedIDNow = data.@object.id;
            }
            StoryWriteWindow shareWindow = new StoryWriteWindow(feedID, isAllRead)
            {
                Owner = this
            };
            shareWindow.ShowDialog();
            await UpdateStats();
        }

        private async void BT_Like_Click(object sender, RoutedEventArgs e)
        {
            if (data.liked)
            {
                bool isSuccess = await KakaoRequestClass.LikeFeed(feedID, null);
                if (isSuccess)
                {
                    data.liked = false;
                    TB_LikeBTN.Text = "느낌 달기";
                    IC_LikeBTN.Kind = MaterialDesignThemes.Wpf.PackIconKind.Heart;
                }
            }
            else
            {
                IC_LikeBTN.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressClock;
                TB_LikeBTN.Text = "전송중..";
                BT_Like.IsEnabled = false;
                EmotionWindow window = new EmotionWindow(feedID, this)
                {
                    Owner = this,
                };
                window.ShowDialog();
            }
            await UpdateStats();
        }

        /// <summary>
        /// Writes multi part HTTP POST request. Author : Farhan Ghumra
        /// </summary>
        private void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, string fileName, string fileContentType, Stream fileStream)
        {
            /// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            /// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            /// the form data, properly formatted
            /// Content-Disposition: form-data; name="file_1"; filename="waifu.png"
            //Content - Type: image / png

            string formdataTemplate = "Content-Disposition; name=\"{0}\"\r\n\r\n{1}";
            /// the form-data file upload, properly formatted
            string fileheaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            /// Added to track if we need a CRLF or not.
            bool bNeedsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    /// if we need to drop a CRLF, do that.
                    if (bNeedsCRLF)
                        WriteToStream(s, "\r\n");

                    /// Write the boundary.
                    WriteToStream(s, boundarybytes);

                    /// Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCRLF = true;
                }
            }

            /// If we don't have keys, we don't need a crlf.
            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "file_1", fileName, fileContentType));
            // Write the file data to the stream.
            byte[] buffer = new byte[4096];
            while ((fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                WriteToStream(s, buffer);
            }
            fileStream.Dispose();
            WriteToStream(s, trailer);
        }

        /// <summary>
        /// Writes string to stream. Author : Farhan Ghumra
        /// </summary>
        private void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }
        private void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }

        public async Task<UploadedImageProp> UploadImage(string filepath)
        {
            var fileTask = StorageFile.GetFileFromPathAsync(filepath);
            while (fileTask.Status != AsyncStatus.Completed) { }
            StorageFile file = fileTask.GetResults();
            StreamReader fileStream = new StreamReader(filepath);

            string requestURI = "https://up-api-kage-4story.kakao.com/web/webstory-img/";

            string boundary = "----" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.CookieContainer = WebViewWindow.GetUriCookieContainer(new Uri("https://story.kakao.com"));

            request.Headers["X-Kakao-DeviceInfo"] = "web:d;-;-";
            request.Headers["X-Kakao-ApiLevel"] = "45";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            request.Headers["X-Kakao-VC"] = "1b242cf8fa50f1f96765";
            request.Headers["Cache-Control"] = "max-age=0";
            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.Headers["Accept-Language"] = "ko-KR,ko;q=0.8,en-US;q=0.6,en;q=0.4";

            request.Headers["DNT"] = "1";

            request.Headers["authority"] = "story.kakao.com";
            request.Referer = "https://story.kakao.com";
            request.KeepAlive = true;
            request.UseDefaultCredentials = true;
            request.Host = "story.kakao.com";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            request.Accept = "*/*";

            Stream writeStream = await request.GetRequestStreamAsync();

            WriteMultipartForm(writeStream, boundary, null, file.Name, file.ContentType, fileStream.BaseStream);

            var readStream = await request.GetResponseAsync();
            var respReader = readStream.GetResponseStream();

            string respResult = await (new StreamReader(respReader, Encoding.UTF8)).ReadToEndAsync();
            respReader.Close();

            UploadedImageProp result = JsonConvert.DeserializeObject<UploadedImageProp>(respResult);
            return result;
        }
        
        private async void BT_Upload_Click(object sender, RoutedEventArgs e)
        {
            if (commentImage == null)
            {
                BT_Upload.Foreground = Brushes.LightGray;
                BT_Upload.IsEnabled = false;
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "Image Files (*.jpg;*.jpeg; *.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|GIF Image File (*.gif)|*.gif",
                    DefaultExt = "jpg",
                    Multiselect = false
                };

                if (ofd.ShowDialog() == true)
                {
                    string path = ofd.FileName;
                    commentImage = await UploadImage(path);
                    BT_Upload.IsEnabled = true;
                    BT_Upload.Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete;
                    BT_Upload.Foreground = Brushes.Gray;
                }
                else
                {
                    BT_Upload.IsEnabled = true;
                    BT_Upload.Kind = MaterialDesignThemes.Wpf.PackIconKind.Attachment;
                    BT_Upload.Foreground = Brushes.Gray;
                }
            }
            else
            {
                commentImage = null;
                BT_Upload.Kind = MaterialDesignThemes.Wpf.PackIconKind.Attachment;
            }
        }

        private async void TB_Comment_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V && Clipboard.ContainsImage())
            {
                if (commentImage == null)
                {
                    BT_Upload.IsEnabled = false;
                    BT_Upload.Foreground = Brushes.LightGray;
                    await PasteImage();
                    BT_Upload.IsEnabled = true;
                    BT_Upload.Foreground = Brushes.Gray;
                    BT_Upload.Kind = MaterialDesignThemes.Wpf.PackIconKind.Delete;
                }
                else
                {
                    commentImage = null;
                    BT_Upload.Kind = MaterialDesignThemes.Wpf.PackIconKind.Attachment;
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                BT_SubmitComent_Click(BT_SubmitComment, null);
            }
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
            
        private async Task<bool> PasteImage()
        {
            if (Clipboard.ContainsImage())
            {
                var image = System.Windows.Forms.Clipboard.GetImage();
                string tempFile = System.IO.Path.GetTempFileName();
                image.Save(tempFile, ImageFormat.Jpeg);
                image.Dispose();
                commentImage = await UploadImage(tempFile);
            }
            return true;
        }

        private void BT_Web_Click(object sender, RoutedEventArgs e)
        {
            string uid = data.actor.id;
            string fid = data.id.Split(new string[] { "." }, StringSplitOptions.None)[1];
            string uri = "https://story.kakao.com/" + uid + "/" + fid;
            System.Diagnostics.Process.Start(uri);
        }

        private async void BT_Edit_Click(object sender, RoutedEventArgs e)
        {
            string content = GlobalHelper.GetStringFromQuoteData(data.content_decorators, true);
            
            StoryWriteWindow sww = new StoryWriteWindow(data.id, content, data.permission, data.media, data.@object != null, isVideo)
            {
                Owner = this
            };
            sww.ShowDialog();
            data = await KSPNotificationActivator.GetPost(feedID);
            GlobalHelper.RefreshContent(data.content_decorators, data.content, TB_Content);
            Refresh();
        }

        private void BT_Delte_Click(object sender, RoutedEventArgs e)
        {
            KakaoRequestClass.DeletePost(data.id);
            MessageBox.Show("포스트가 삭제됐습니다.");
            Close();
        }

        private async void BT_UP_Click(object sender, RoutedEventArgs e)
        {
            if (data.sympathized)
            {
                BT_UP.IsEnabled = false;
                BT_UP.Content = "...";
                await KakaoRequestClass.UPFeed(data.id, true);
                data.sympathized = false;
                BT_UP.IsEnabled = true;
                BT_UP.Content = "UP";
            }
            else
            {
                BT_UP.IsEnabled = false;
                BT_UP.Content = "...";
                await KakaoRequestClass.UPFeed(data.id, false);
                data.sympathized = true;
                BT_UP.IsEnabled = true;
                BT_UP.Content = "UP 취소";
            }
            await UpdateStats();
        }

        private void BT_QuoteFriend_Click(object sender, RoutedEventArgs e)
        {
            FriendSelectWindow fsc = new FriendSelectWindow((ids, names) =>
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    string append = "{!{{" + "\"id\":\"" + ids[i] + "\", \"type\":\"profile\", \"text\":\"" + names[i] + "\"}}!} ";
                    if (i + 1 < ids.Count)
                    {
                        append += ", ";
                    }
                    int lastPos = TB_Comment.CaretIndex;
                    TB_Comment.Text = TB_Comment.Text.Insert(TB_Comment.CaretIndex, append);
                    TB_Comment.CaretIndex += lastPos + append.Length;
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => TB_Comment.Focus()));
                }
            }, false)
            {
                Owner = this
            };
            fsc.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (MainWindow.posts != null && feedID != null)
                MainWindow.posts.Remove(feedID);
        }

        private void BT_Upload_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_Upload_Click(null, null);
        }

        private void BT_SubmitComment_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_SubmitComent_Click(null, null);
        }

        private void IC_CommentShow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IC_CommentShow.Kind == MaterialDesignThemes.Wpf.PackIconKind.ArrowCollapseHorizontal)
            {
                IC_CommentShow.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowExpandHorizontal;
                CD_Comment.Width = new GridLength(0, GridUnitType.Star);
                Grid.SetColumn(FL_Menu, 0);
            }
            else
            {
                IC_CommentShow.Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowCollapseHorizontal;
                CD_Comment.Width = new GridLength(0.7, GridUnitType.Star);
                Grid.SetColumn(FL_Menu, 1);
            }
        }

        private void BT_Quote_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BT_QuoteFriend_Click(null, null);
        }
        
        private void IC_Menu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FL_Menu.IsOpen = !FL_Menu.IsOpen;
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Close();
            }
            else if (e.Key == Key.F5 || (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control))
            {
                BT_CommentRefresh_Click(null, null);
            }
            else if (e.Key == Key.Escape)
            {
                FL_Menu.IsOpen = false;
            }
        }

        private void MetroWindow_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is System.Windows.Controls.TextBox))
                e.Handled = true;
        }

        private async void BT_AddFavorite_Click(object sender, RoutedEventArgs e)
        {
            BT_AddFavorite.IsEnabled = false;
            await KakaoRequestClass.PinPost(data.id, data.bookmarked);
            await RenewComment();
            BT_AddFavorite.IsEnabled = true;
        }
    }
}
