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

namespace KSP_WPF
{
    /// <summary>
    /// StoryModifyWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StoryModifyWindow : MetroWindow
    {
        private bool isClosed = false;
        public StoryModifyWindow()
        {
            InitializeComponent();
            CB_Task.SelectedIndex = 0;
            CB_Target.SelectedIndex = 0;
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Close();
            }
        }

        private void Grid_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is TextBox) && !(e.Source is ComboBox) && !(e.Source is ComboBoxItem))
                e.Handled = true;
        }


        private void CB_Task_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_Task.SelectedIndex == 0)
                SP_RangeSetting.Visibility = Visibility.Visible;
            else
                SP_RangeSetting.Visibility = Visibility.Collapsed;
        }

        private async void BT_Execute_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("작업을 실행하시겠습니까?\n이 작업은 되돌릴 수 없습니다!", "경고", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                SP_Progress.Visibility = Visibility.Visible;
                BT_Execute.IsEnabled = false;
                BT_Execute.Content = "실행중..";
                CB_Task.IsEnabled = false;
                CB_Target.IsEnabled = false;
                CB_Range.IsEnabled = false;
                CB_ExcludeFavorite.IsEnabled = false;

                TB_Progress.Text = "준비중...";
                SP_Progress.Visibility = Visibility.Visible;
                PR_Main.IsActive = true;

                List<CommentData.PostData> posts = new List<CommentData.PostData>();
                await Dispatcher.Invoke(async () =>
                {
                    if((CB_Task.SelectedIndex == 1 || CB_Task.SelectedIndex == 2) && CB_Target.SelectedIndex  == 5)
                    {
                        var bookmarkMain = await KakaoRequestClass.GetBookmark(MainWindow.userProfile.id, null);
                        while (!isClosed)
                        {
                            foreach (var bookmark in bookmarkMain.bookmarks)
                            {
                                posts.Add(bookmark.activity);
                            }
                            TB_Progress.Text = $"게시글 조회 ({posts.Count})";
                            bookmarkMain = await KakaoRequestClass.GetBookmark(MainWindow.userProfile.id, bookmarkMain.bookmarks.Last().id);
                            if (bookmarkMain.bookmarks.Count == 0)
                                break;
                        }
                    }
                    else
                    {
                        var feeds = await KakaoRequestClass.GetProfileFeed(MainWindow.userProfile.id, null);
                        while (!isClosed)
                        {
                            posts.AddRange(feeds.activities);
                            TB_Progress.Text = $"게시글 조회 ({posts.Count})";
                            feeds = await KakaoRequestClass.GetProfileFeed(MainWindow.userProfile.id, feeds.activities[feeds.activities.Count - 1].id);
                            if (feeds.activities.Count == 0)
                                break;
                        }
                    }
                    PR_Main.IsActive = false;
                    PR_Main.Visibility = Visibility.Collapsed;

                    PB_Main.Visibility = Visibility.Visible;
                    if (!isClosed)
                    {
                        FeedLoop(posts);
                    }
                });
            }
        }

        private async void FeedLoop(List<CommentData.PostData> posts)
        {
            StringBuilder extractedLink = new StringBuilder();
            TB_Progress.Text = "작업중...";
            await Dispatcher.Invoke(async () =>
            {
                for(int i = 0; i < posts.Count; i++)
                {
                    if (isClosed) break;
                    var post = posts[i];
                    if (CB_Target.SelectedIndex == 1 && !post.permission.Equals("A"))
                        continue;
                    else if (CB_Target.SelectedIndex == 2 && !post.permission.Equals("F"))
                        continue;
                    else if (CB_Target.SelectedIndex == 3 && !post.permission.Equals("P"))
                        continue;
                    else if (CB_Target.SelectedIndex == 4 && !post.permission.Equals("M"))
                        continue;
                    else if (CB_Target.SelectedIndex == 5 && !post.bookmarked)
                        continue;

                    if (CB_Task.SelectedIndex == 0)
                    {
                        await ChangePostRange(post);
                        await Task.Delay(100);
                    }
                    if (CB_Task.SelectedIndex == 1)
                    {
                        extractedLink.Append(post.permalink);
                        extractedLink.Append("\n");
                    }
                    if (CB_Task.SelectedIndex == 2)
                    {
                        if(CB_Target.SelectedIndex == 5)
                            await KakaoRequestClass.PinPost(post.id, true);
                        else
                            await KakaoRequestClass.PinPost(post.id, false);
                        await Task.Delay(100);
                    }
                    TB_Progress.Text = $"작업중... ({i}/{posts.Count})";
                    PB_Main.Value = (double)i / posts.Count * 100.0;
                }

                if (!isClosed)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (extractedLink.Length > 0)
                        {
                            Clipboard.SetDataObject(extractedLink.ToString());
                            MessageBox.Show("링크 추출 완료\n추출된 링크는 클립보드에 복사되었습니다. Ctrl+V를 사용하여 붙여넣기하세요.", "안내");
                        }
                        else
                        {
                            MessageBox.Show("작업 완료", "안내");
                        }
                        SP_Progress.Visibility = Visibility.Collapsed;
                        Close();
                    });
                }
            });
        }

        private async Task ChangePostRange(CommentData.PostData post)
        {
            string toPerm = null;
            if (CB_Range.SelectedIndex == 0)
                toPerm = "A";
            else if (CB_Range.SelectedIndex == 1)
                toPerm = "F";
            else if (CB_Range.SelectedIndex == 2)
                toPerm = "M";

            await KakaoRequestClass.SetActivityProfile(post.id, toPerm, post.sharable, post.comment_all_writable, post.is_must_read);
            return;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            isClosed = true;
            if (SP_Progress.Visibility == Visibility.Visible)
                MessageBox.Show("작업이 취소되었습니다.", "안내");
        }

        private void CB_Target_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_Target.SelectedIndex == 5)
            {
                CB_ExcludeFavorite.Visibility = Visibility.Collapsed;
                CBI_Bookmark.Content = "관심글에서 삭제";
            }
            else
            {
                CB_ExcludeFavorite.Visibility = Visibility.Visible;
                CBI_Bookmark.Content = "관심글에 추가";
            }
        }
    }
}
