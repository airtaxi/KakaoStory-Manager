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
    /// MailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MailWriteWindow : MetroWindow
    {
        private string reciver;

        private void AddTarget(string id, string name)
        {
            reciver = id;
            var fc = new UserNameWithCloseButton();
            fc.TB_Name.Text = name;
            fc.IC_Close.PreviewMouseLeftButtonDown += (s, e2) =>
            {
                SP_Friends.Children.Remove(fc);
                reciver = null;
            };
            SP_Friends.Children.Add(fc);
        }
        public MailWriteWindow(string id = null, string name = null)
        {
            InitializeComponent();
            if(id != null && name != null)
            {
                AddTarget(id, name);
            }
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Close();
                e.Handled = true;
            }
        }

        private void TBX_Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BT_Send_Click(BT_Send, null);
        }

        private async void BT_Send_Click(object sender, RoutedEventArgs e)
        {
            if(TBX_Main.Text.Length > 0 && reciver != null)
            {
                await KakaoRequestClass.SendMail(TBX_Main.Text, reciver, false);
            }
            else if(TBX_Main.Text.Length == 0)
            {
                MessageBox.Show("쪽지 내용을 입력해주세요.", "오류");
            }
            else if(reciver == null)
            {
                MessageBox.Show("쪽지를 받는 친구를 선택해주세요.", "오류");
            }
        }

        private void BT_Friends_Click(object sender, RoutedEventArgs e)
        {
            FriendSelectWindow fsw = new FriendSelectWindow((ids, names) =>
            {
                if (ids.Count > 0)
                {
                    reciver = ids.First();
                    AddTarget(ids.First(), names.First());
                }
            }, false)
            {
                Title = "쪽지 받는 친구 선택",
                Owner = this
            };
            fsw.ShowDialog();
        }
    }
}
