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
        private readonly Hashtable ht = new Hashtable();


        private void AddComment(Comment commentProf, bool insert)
        {
            CommentControl comment = new CommentControl();
            comment.TB_Name.Text = commentProf.writer.display_name;
            //comment.TB_Content.Tag = commentProf.text;
            GlobalHelper.RefreshContent(commentProf.decorators, commentProf.text, comment.TB_Content);

            if (commentProf.updated_at.Year > 1)
            {
                comment.TB_Edit.Visibility = Visibility.Visible;
                comment.TB_Edit.Text += " - " + GetTimeString(commentProf.updated_at);
            }

            comment.TB_Content.MouseRightButtonDown += (s, e) =>
            {
                Clipboard.SetDataObject((string)((object[])comment.TB_Content.Tag)[1]);
                MessageBox.Show("클립보드에 댓글 내용이 복사됐습니다.");
                e.Handled = true;
            };
            if (commentProf.like_count > 0)
                comment.TB_Like.Text = $"좋아요 {commentProf.like_count.ToString()}개";

            comment.TB_MetaData.Text = GetTimeString(commentProf.created_at);
            string imgUri = commentProf.writer.profile_thumbnail_url;
            if (!Properties.Settings.Default.PostNoGIF && commentProf.writer.profile_video_url_square_small != null)
                imgUri = commentProf.writer.profile_video_url_square_micro_small;
            GlobalHelper.AssignImage(comment.IMG_Profile, imgUri);

            MainWindow.SetClickObject(comment.IMG_Profile);
            comment.IMG_Profile.MouseLeftButtonDown += (s, e) =>
            {
                try
                {
                    TimeLineWindow tlw = new TimeLineWindow(commentProf.writer.id);
                    tlw.Show();
                    tlw.Activate();
                    e.Handled = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("접근이 불가능한 스토리입니다.");
                }
            };

            string imageUri = null;
            bool overrideGif = false;
            foreach (var decorator in commentProf.decorators)
            {
                if (decorator.type.Equals("image"))
                {
                    imageUri = decorator.media?.origin_url;
                    if (imageUri.Contains(".gif") && Properties.Settings.Default.PostNoGIF)
                    {
                        overrideGif = true;
                    }
                }
            }

            if (imageUri != null)
            {
                comment.IMG_Comment.Visibility = Visibility.Visible;
                if (overrideGif)
                {
                    GlobalHelper.AssignImage(comment.IMG_Comment, "gif.png");
                    comment.IMG_Comment.Tag = imageUri;
                }
                else
                {
                    GlobalHelper.AssignImage(comment.IMG_Comment, imageUri);
                }
                comment.IMG_Comment.MouseRightButtonDown += GlobalHelper.CopyImageHandler;
                comment.IMG_Comment.MouseLeftButtonDown += GlobalHelper.SaveImageHandler;
            }
            else
                comment.IMG_Comment.Visibility = Visibility.Collapsed;

            if (commentProf.liked)
                comment.IC_Like.Foreground = Brushes.Red;

            bool deletable = commentProf.writer.id.Equals(MainWindow.UserProfile.id) || data.actor.id.Equals(MainWindow.UserProfile.id);
            bool editable = commentProf.writer.id.Equals(MainWindow.UserProfile.id);

            if (!deletable)
                comment.BT_Delete.IsEnabled = false;

            comment.BT_Delete.Click += async (s, e) =>
            {
                await KakaoRequestClass.DeleteComment(commentProf.id, data);
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
                    await KakaoRequestClass.LikeComment(feedID, commentProf.id, commentProf.liked);
                    await RenewComment();
                }
            };

            comment.Grid.MouseEnter += (s, e) =>
            {
                comment.Grid.Background = Brushes.Teal;
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
            Rectangle sep = new Rectangle
            {
                Stroke = Brushes.LightGray,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            if (!insert)
            {
                SP_Comment.Children.Add(comment);
                SP_Comment.Children.Add(sep);
            }
            else
            {
                SP_Comment.Children.Insert(0, sep);
                SP_Comment.Children.Insert(0, comment);
            }
        }

        public static void ShowPostWindow(PostData data, string feedID)
        {
            if (MainWindow.Posts.ContainsKey(feedID))
            {
                PostWindow postWindow = MainWindow.Posts[feedID];
                postWindow.Refresh();
                postWindow.Show();
                postWindow.Activate();
                postWindow.Focus();
                postWindow.SV_Comment.ScrollToEnd();
                postWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => postWindow.TB_Comment.Focus()));
            }
            else
            {
                MainWindow.Instance.Dispatcher.BeginInvoke(new Action(delegate
                {
                    PostWindow postWindow = new PostWindow(data, feedID);
                    postWindow.Show();
                    postWindow.Activate();
                    postWindow.Focus();
                    postWindow.SV_Comment.ScrollToEnd();
                    if (Properties.Settings.Default.PositionPostToTop)
                        postWindow.Top = 0;
                    postWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => postWindow.TB_Comment.Focus()));
                }), System.Windows.Threading.DispatcherPriority.ContextIdle);
            }
        }

        public PostWindow(PostData data, string feedID)
        {
            InitializeComponent();
            Dispatcher.InvokeAsync(async () =>
            {
                if (!Properties.Settings.Default.ShowComment)
                    IC_CommentShow_PreviewMouseLeftButtonDown(IC_CommentShow, null);
                if (!Properties.Settings.Default.HideScrollBar)
                {
                    SV_Content.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    SV_Comment.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                MainWindow.SetClickObject(BT_Upload);
                MainWindow.SetClickObject(BT_Quote);
                MainWindow.SetClickObject(BT_SubmitComment);
                MainWindow.SetClickObject(IC_CommentShow);
                MainWindow.Posts.Add(feedID, this);
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

                if (!data.actor.id.Equals(MainWindow.UserProfile.id))
                    BT_Delte.IsEnabled = false;
                if (!data.actor.id.Equals(MainWindow.UserProfile.id))
                    BT_Edit.IsEnabled = false;

                string imgUri = data.actor.profile_thumbnail_url;
                if (!Properties.Settings.Default.PostNoGIF && data.actor.profile_video_url_square_small != null)
                    imgUri = data.actor.profile_video_url_square_micro_small;
                GlobalHelper.AssignImage(IMG_Profile, imgUri);

                MainWindow.SetClickObject(IMG_Profile);
                IMG_Profile.MouseLeftButtonDown += (s, e) =>
                {
                    try
                    {
                        TimeLineWindow tlw = new TimeLineWindow(data.actor.id);
                        tlw.Show();
                        tlw.Activate();
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
                                PostData feed = await KakaoRequestClass.GetPost(data.@object.id);
                                ShowPostWindow(feed, data.@object.id);
                                e.Handled = true;
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("접근할 수 없는 포스트입니다.");
                            }
                        };

                        string imgUri2 = data.@object.actor.profile_thumbnail_url;
                        if (!Properties.Settings.Default.PostNoGIF && data.@object.actor.profile_video_url_square_small != null)
                            imgUri2 = data.@object.actor.profile_video_url_square_small;
                        GlobalHelper.AssignImage(IMG_ProfileShare, imgUri2);

                        MainWindow.SetClickObject(IMG_ProfileShare);

                        IMG_ProfileShare.MouseLeftButtonDown += (s, e) =>
                        {
                            try
                            {
                                TimeLineWindow tlw = new TimeLineWindow(data.@object.actor.id);
                                tlw.Show();
                                tlw.Activate();
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

                        if (data.@object.media_type != null && data.@object.media != null)
                        {
                            RefreshImageContent(data.@object.media, SP_ShareContent);
                            SP_ShareContent.Visibility = Visibility.Visible;
                        }

                        if (data.@object?.closest_with_tags != null && data.@object?.closest_with_tags.Count > 0)
                        {
                            Separator sep = new Separator();
                            SP_ShareContent.Children.Add(sep);
                            sep.Margin = new Thickness(0, 5, 0, 5);
                            var TB_Closest_With = GlobalHelper.GetWithFriendTB(data.@object);
                            SP_ShareContent.Children.Add(TB_Closest_With);
                            SP_ShareContent.Visibility = Visibility.Visible;
                        }

                        if (data.@object.scrap != null)
                        {
                            GlobalHelper.RefreshScrap(data.@object.scrap, Scrap_Share);
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

        private void RefreshImageContent(List<CommentData.Medium> medias, StackPanel panel)
        {
            bool isFirst = true;
            Image lastImage = null;
            foreach (var media in medias)
            {
                if (media.url_hq != null)
                {
                    Image videoImage = new Image();
                    GlobalHelper.AssignImage(videoImage, "img_vid.png");
                    MainWindow.SetClickObject(videoImage);
                    videoImage.MouseLeftButtonDown += (s, e) =>
                    {
                        System.Diagnostics.Process.Start(media.url_hq);
                        e.Handled = true;
                    };
                    panel.Children.Add(videoImage);
                }
                else
                {
                    string uri = media.origin_url;
                    bool overrideGif = false;
                    if (uri.Contains(".gif") && Properties.Settings.Default.PostNoGIF)
                    {
                        overrideGif = true;
                        uri = "gif.png";
                    }
                    if (uri != null)
                    {
                        Image image = new Image();
                        if (overrideGif != true)
                        {
                            image.Tag = new Image[2] { lastImage, null };
                            if (lastImage != null && lastImage.Tag is Image[])
                            {
                                ((Image[])lastImage.Tag)[1] = image;
                            }
                        }
                        else
                        {
                            image.Tag = media.origin_url;
                        }
                        GlobalHelper.AssignImage(image, uri);
                        image.Stretch = Stretch.UniformToFill;
                        if (!isFirst)
                            image.Margin = new Thickness(0, 10, 0, 10);
                        isFirst = false;
                        image.MouseRightButtonDown += GlobalHelper.CopyImageHandler;
                        image.MouseLeftButtonDown += GlobalHelper.SaveImageHandler;
                        lastImage = image;
                        panel.Children.Add(image);
                    }
                }
            }
        }

        private void RefreshImage()
        {
            SP_Content.Children.Clear();

            if (data.media_type != null && data.media != null)
            {
                RefreshImageContent(data.media, SP_Content);
                SP_Content.Visibility = Visibility.Visible;
            }

            if (data.closest_with_tags != null && data.closest_with_tags.Count > 0)
            {
                Separator sep = new Separator();
                SP_Content.Children.Add(sep);
                sep.Margin = new Thickness(0, 5, 0, 5);
                var TB_Closest_With = GlobalHelper.GetWithFriendTB(data);
                SP_Content.Children.Add(TB_Closest_With);
                SP_Content.Visibility = Visibility.Visible;
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
            data = await KakaoRequestClass.GetPost(feedID);
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
            data = await KakaoRequestClass.GetPost(feedID);
            var shares = await KakaoRequestClass.GetShares(false, data, null);
            var ups = await KakaoRequestClass.GetShares(true, data, null);
            var likes = await KakaoRequestClass.GetLikes(data, null);
            int index = 0;
            if (source.Equals(TB_LikeCount))
                index = 0;
            else if (source.Equals(TB_ShareCount))
                index = 1;
            if (source.Equals(TB_UpCount))
                index = 2;
            PostInfoWindow piw = new PostInfoWindow(likes, shares, ups, data, index);
            piw.Show();
            //piw.Activate();
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
                dateText = ((int)diffTime.TotalMinutes).ToString() + "분 전";
            }
            else if (diffTime.TotalHours < 24)
            {
                dateText = ((int)diffTime.TotalHours).ToString() + "시간 전";
            }
            return dateText;
        }

        private async Task<bool> UpdateStats()
        {
            data = await KakaoRequestClass.GetPost(feedID);

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
            else if (data.verb.Equals("share"))
            {
                BT_Share.IsEnabled = false;
                BT_UP.IsEnabled = false;
            }

            if (data.push_mute)
                BT_Mute.Content = "이 게시글 알림 받기";
            else
                BT_Mute.Content = "이 게시글 알림 음소거";
            return true;
        }

        private string lastComment;
        private void RefreshComment(List<Comment> comments)
        {
            SP_Comment.Children.Clear();
            requestingCommentID = null;

            if (comments.Count == 0)
                TB_CommentInfo.Text = "댓글이 없습니다";

            if (comments.Count == 30)
                lastComment = comments.First().id;
            else
                lastComment = null;
            Dispatcher.InvokeAsync(() =>
            {
                foreach (Comment comment in comments)
                {
                    AddComment(comment, false);
                }
                SV_Comment.ScrollToEnd();
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
            data = await KakaoRequestClass.GetPost(feedID);
            RefreshComment(data.comments);
            await UpdateStats();
            return true;
        }

        private string requestingCommentID = null;
        private async void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalHelper.HandleScroll(sender, e);
            if (SV_Comment.VerticalOffset == 0 && e.Delta > 0 && lastComment != null && (requestingCommentID != lastComment))
            {
                requestingCommentID = lastComment;
                double origHeight = SV_Comment.ScrollableHeight;
                var comments = await KakaoRequestClass.GetComment(feedID, lastComment);

                for (int i = comments.Count - 1; i >= 0; i--)
                {
                    var comment = comments[i];
                    AddComment(comment, true);
                }
                await SV_Comment.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
                {
                    if (comments.Count == 30)
                        lastComment = comments.First().id;
                    else
                        lastComment = null;

                    double diff = SV_Comment.ScrollableHeight - origHeight;
                    SV_Comment.ScrollToVerticalOffset(diff);
                }));
            }
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
                TB_LikeBTN.Text = "전송중..";
                IC_LikeBTN.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressClock;
                BT_Like.IsEnabled = true;
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
                FL_Emotion.IsOpen = !FL_Emotion.IsOpen;
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
            string filename = System.IO.Path.GetFileName(filepath);
            StreamReader fileStream = new StreamReader(filepath);

            string requestURI = "https://up-api-kage-4story.kakao.com/web/webstory-img/";

            HttpWebRequest request = WebRequest.CreateHttp(requestURI);
            request.Method = "POST";
            string boundary = "----" + DateTime.Now.Ticks.ToString("x");
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

            WriteMultipartForm(writeStream, boundary, null, filename, System.Web.MimeMapping.GetMimeMapping(filename), fileStream.BaseStream);

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

            StoryWriteWindow sww = new StoryWriteWindow(data.id, content, data.permission, data.media, data.@object != null)
            {
                Owner = this
            };
            if (data.allowed_profile_ids != null)
                sww.trust_ids = data.allowed_profile_ids;
            if (data.closest_with_tags != null)
            {
                List<string> with_ids = new List<string>();
                foreach (var friend in data.closest_with_tags)
                {
                    with_ids.Add(friend.id);
                }
                sww.with_ids = with_ids;
            }
            sww.ShowDialog();
            data = await KakaoRequestClass.GetPost(feedID);
            GlobalHelper.RefreshContent(data.content_decorators, data.content, TB_Content);
            Refresh();
        }

        private async void BT_Delte_Click(object sender, RoutedEventArgs e)
        {
            await KakaoRequestClass.DeletePost(data.id);
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
            if (MainWindow.Posts != null && feedID != null)
                MainWindow.Posts.Remove(feedID);
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
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Close();
            }
            else if (e.Key == Key.F5 || (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control))
            {
                e.Handled = true;
                if (BT_CommentRefresh.IsEnabled)
                    BT_CommentRefresh_Click(BT_CommentRefresh, null);
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                FL_Menu.IsOpen = false;
                FL_Emotion.IsOpen = false;
            }
            else if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (BT_Like.IsEnabled)
                    BT_Like_Click(BT_Like, null);
            }
            else if (e.Key == Key.T && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (IC_CommentShow.IsEnabled)
                    IC_CommentShow_PreviewMouseLeftButtonDown(IC_CommentShow, null);
            }
            else if (e.Key == Key.M && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (IC_Menu.IsEnabled)
                    IC_Menu_MouseLeftButtonDown(IC_Menu, null);
            }
            else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (BT_Share.IsEnabled)
                    BT_Share_Click(BT_Share, null);
            }
            else if (e.Key == Key.E && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (BT_Edit.IsEnabled)
                    BT_Edit_Click(BT_Share, null);
            }
            else if (e.Key == Key.U && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (BT_UP.IsEnabled)
                    BT_UP_Click(BT_UP, null);
            }
            else if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (BT_Mute.IsEnabled)
                    BT_Mute_Click(BT_Mute, null);
            }
            else if (e.Key == Key.B && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (BT_Web.IsEnabled)
                    BT_Web_Click(BT_Web, null);
            }
            else if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (BT_Upload.IsEnabled)
                    BT_Upload_Click(BT_Upload, null);
            }
            else if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (BT_AddFavorite.IsEnabled)
                    BT_AddFavorite_Click(BT_AddFavorite, null);
            }
            else if (e.Key == Key.L && FL_Emotion.IsOpen)
            {
                e.Handled = true;
                BT_E_Like_Click(BT_E_Like, null);
            }
            else if (e.Key == Key.G && FL_Emotion.IsOpen)
            {
                e.Handled = true;
                BT_E_Like_Click(BT_E_Good, null);
            }
            else if (e.Key == Key.P && FL_Emotion.IsOpen)
            {
                e.Handled = true;
                BT_E_Like_Click(BT_E_Pleasure, null);
            }
            else if (e.Key == Key.S && FL_Emotion.IsOpen)
            {
                e.Handled = true;
                BT_E_Like_Click(BT_E_Sad, null);
            }
            else if (e.Key == Key.C && FL_Emotion.IsOpen)
            {
                e.Handled = true;
                BT_E_Like_Click(BT_E_Cheerup, null);
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

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SP_Comment.Children.Clear();
            SP_Content.Children.Clear();
            SP_Share.Children.Clear();
            SP_ShareContent.Children.Clear();
        }

        private async void BT_Mute_Click(object sender, RoutedEventArgs e)
        {
            BT_Mute.IsEnabled = false;
            await KakaoRequestClass.MutePost(data.id, !data.push_mute);
            await RenewComment();
            BT_Mute.IsEnabled = true;
        }

        private async void BT_E_Like_Click(object sender, RoutedEventArgs e)
        {
            Button button = ((Button)sender);
            TB_LikeBTN.Text = "전송중..";
            IC_LikeBTN.Kind = MaterialDesignThemes.Wpf.PackIconKind.ProgressClock;
            BT_Like.IsEnabled = false;
            bool isSucces = await KakaoRequestClass.LikeFeed(feedID, (string)button.Tag);
            Dispatcher.Invoke(() =>
            {
                if (isSucces)
                {
                    TB_LikeBTN.Text = "느낌 취소";
                    IC_LikeBTN.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cancel;
                }
                else
                {
                    TB_LikeBTN.Text = "느낌 달기";
                    IC_LikeBTN.Kind = MaterialDesignThemes.Wpf.PackIconKind.Heart;
                }
                BT_Like.IsEnabled = true;
                data.liked = true;
                FL_Emotion.IsOpen = false;
            });
            await UpdateStats();
        }
    }
}
