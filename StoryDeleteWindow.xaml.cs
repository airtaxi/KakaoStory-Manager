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
    /// StoryDeleteWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StoryDeleteWindow : MetroWindow
    {
        private bool activate = true;
        public StoryDeleteWindow()
        {
            InitializeComponent();
        }

        private enum StroyFilterCategory { All, AllShare, FriendShare, ComfShare, MeOnly };

        private void CB_Include_Click(object sender, RoutedEventArgs e)
        {
            if (CB_Include.IsChecked == true)
            {
                CB_Exclude.IsChecked = false;
                TB_Filter.Visibility = Visibility.Visible;
            }
            else if(CB_Exclude.IsChecked != true)
                TB_Filter.Visibility = Visibility.Collapsed;
        }

        private void CB_Exclude_Click(object sender, RoutedEventArgs e)
        {
            if (CB_Exclude.IsChecked == true)
            {
                CB_Include.IsChecked = false;
                TB_Filter.Visibility = Visibility.Visible;
            }
            else if (CB_Include.IsChecked != true)
                TB_Filter.Visibility = Visibility.Collapsed;
        }

        private async void BT_Confirm_Click(object sender, RoutedEventArgs e)
        {
            BT_Confirm.IsEnabled = false;
            if (MainWindow.IsLoggedIn && !MainWindow.isOffline)
            {
                int deleted = 0;
                int counted = 0;
                var feed = await KakaoRequestClass.GetProfileFeed(MainWindow.FriendData.profile.id, null);
                if(feed.activities.Count == 0)
                {
                    MessageBox.Show("삭제할 게시글이 존재하지 않습니다.");
                    BT_Confirm.IsEnabled = true;
                    return;
                }
                else
                {
                    if(MessageBox.Show("조건에 맞는 게시글을 전부 삭제하시겠습니까?\n이 작업은 되돌릴 수 없습니다!", "경고", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        CB_Category.IsEnabled = false;
                        CB_Exclude.IsEnabled = false;
                        CB_Include.IsEnabled = false;
                        TB_Filter.IsEnabled = false;
                        SP_Progress.Visibility = Visibility.Visible;
                        TB_Progress.Text = "삭제 준비중...";
                        try
                        {
                            MainWindow.IsLoggedIn = false;
                            async void Delete()
                            {
                                foreach (var activity in feed.activities)
                                {
                                    bool willDelete = true;
                                    string[] shareIndex = { null, "A", "F", "P", "M" };
                                    if (CB_Category.SelectedIndex == 5)
                                    {
                                        if (!activity.blinded)
                                            willDelete = false;
                                    }
                                    else if (CB_Category.SelectedIndex > 0 && !activity.permission.Equals(shareIndex[CB_Category.SelectedIndex]))
                                        willDelete = false;
                                    if (CB_Include.IsChecked == true && !activity.content.Contains(TB_Filter.Text))
                                        willDelete = false;
                                    if (CB_Exclude.IsChecked == true && activity.content.Contains(TB_Filter.Text))
                                        willDelete = false;
                                    if (willDelete)
                                    {
                                        await Task.Delay(100);
                                        KakaoRequestClass.DeletePost(activity.id);
                                        deleted++;
                                    }
                                    counted++;
                                    TB_Progress.Text = $"삭제된 게시글/전체 게시글 : {deleted}/{counted}";
                                    if (!activate)
                                        break;
                                }
                                if (!activate)
                                {
                                    MessageBox.Show("게시글 삭제가 취소됐습니다.", "안내");
                                    MainWindow.IsLoggedIn = true;
                                    MainWindow.FriendData.profile.activity_count = counted - deleted;
                                    return;
                                }
                                feed = await KakaoRequestClass.GetProfileFeed(MainWindow.FriendData.profile.id, feed.activities[feed.activities.Count - 1].id);
                                if (feed != null && (feed.activities?.Count ?? 0) > 0)
                                    Delete();
                                else
                                {
                                    MessageBox.Show("삭제가 모두 완료됐습니다.", "안내");
                                    MainWindow.IsLoggedIn = true;
                                    MainWindow.FriendData.profile.activity_count = counted - deleted;
                                    Close();
                                }
                            }
                            Delete();
                        } catch(Exception e2) { MessageBox.Show("작업 도중 알 수 없는 오류가 발생했습니다.\n" + e2.Message); }
                    }
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            activate = false;
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
    }
}
