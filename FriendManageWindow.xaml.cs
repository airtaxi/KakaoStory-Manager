﻿using MahApps.Metro.Controls;
using Newtonsoft.Json;
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
    /// FriendManageWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FriendManageWindow : MetroWindow
    {
        public FriendManageWindow()
        {
            InitializeComponent();

            if (Properties.Settings.Default.HideScrollBar)
                SV_Main.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
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

        private void BT_DeleteBlinded_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.IsLoggedIn && !MainWindow.isOffline)
            {
                List<FriendData.Profile> blinded = new List<FriendData.Profile>();
                foreach (var friend in MainWindow.userFriends.profiles)
                {
                    if (friend.blocked == true)
                    {
                        blinded.Add(friend);
                    }
                }
                if (blinded.Count > 0)
                {
                    if (MessageBox.Show($"{blinded.Count.ToString()}명의 제한된 사용자를 삭제하시겠습니까?\n이 작업은 돌이킬 수 없습니다.", "경고", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        ProgressWindow pw = new ProgressWindow(blinded)
                        {
                            Owner = this
                        };
                        pw.TB_Content.Text = "초기화중...";
                        pw.ShowDialog();
                    }
                }
                else
                    MessageBox.Show("친구 목록에 제한된 사용자가 없습니다.");
            }
            else
                GlobalHelper.ShowOfflineMessage();
        }

        private async void BT_BackupFriends_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            List<string> ids = new List<string>();
            var friends = JsonConvert.DeserializeObject<FriendData.Friends>(await KakaoRequestClass.GetFriendData());
            foreach(var friend in friends.profiles)
            {
                builder.Append(friend.display_name);
                builder.Append(" : ");
                builder.Append("https://story.kakao.com/");
                builder.Append(friend.id);
                builder.Append("\n");
                ids.Add(friend.id);
            }
            Clipboard.SetDataObject(builder.ToString());
            var sfd = new System.Windows.Forms.SaveFileDialog()
            {
                FileName = DateTime.Now.ToShortDateString() + ".kfd",
                Filter = "Kakao Friend Data|*.kfd",
                Title = "친구 목록 저장"
            };
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var str = JsonConvert.SerializeObject(ids);
                var writer = new System.IO.StreamWriter(sfd.FileName);
                writer.Write(str);
                writer.Close();
            }
            MessageBox.Show("클립보드에 친구 정보가 복사됐습니다.");
        }

        private async void BT_CompareFriends_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "Kakao Friend Data|*.kfd",
                Title = "친구 목록 불러오기"
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var reader = new System.IO.StreamReader(ofd.FileName);
                string str = reader.ReadToEnd();
                reader.Close();
                var loadedFriends = JsonConvert.DeserializeObject<List<string>>(str);
                var friends = JsonConvert.DeserializeObject<FriendData.Friends>(await KakaoRequestClass.GetFriendData());
                StringBuilder builder = new StringBuilder("삭제된 친구 목록 : \n");
                int count = 0;
                foreach (string id in loadedFriends)
                {
                    bool exists = false;
                    foreach (var friend in friends.profiles)
                    {
                        if (friend.id.Equals(id))
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        var profile = await KakaoRequestClass.GetProfileFeed(id, null);
                        builder.Append(profile.profile.display_name);
                        builder.Append(" : ");
                        builder.Append(profile.profile.permalink);
                        builder.Append("\n");
                        count++;
                    }
                }
                Clipboard.SetDataObject(builder.ToString());
                MessageBox.Show($"삭제된 친구 목록이 클립보드에 복사되었습니다.\n삭제된 친구 수 : {count.ToString()}", "안내");
            }
        }
    }
}