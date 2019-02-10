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
    public partial class MailReadWindow : MetroWindow
    {
        public string id;
        public MailReadWindow(string id)
        {
            InitializeComponent();
            this.id = id;

            RA_Loading.Visibility = Visibility.Visible;
            PR_Loading.Visibility = Visibility.Visible;
            
            Dispatcher.InvokeAsync(async () =>
            {
                var mail = await KakaoRequestClass.GetMailDetail(id);

                string imgUri = mail.sender.profile_thumbnail_url;
                if (Properties.Settings.Default.GIFProfile && mail.sender.profile_video_url_square_small != null)
                    imgUri = mail.sender.profile_video_url_square_small;
                GlobalHelper.AssignImage(IMG_Profile, imgUri);
                MainWindow.SetClickObject(IMG_Profile);
                IMG_Profile.PreviewMouseLeftButtonDown += (s, e) =>
                {
                    e.Handled = true;
                    try
                    {
                        TimeLineWindow tlw = new TimeLineWindow(mail.sender.id);
                        tlw.Show();
                        tlw.Activate();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("접근이 불가능한 스토리입니다.");
                    }
                };

                if (mail.@object?.background != null && mail.@object.background.type.Equals("image"))
                {
                    GlobalHelper.AssignImage(IMG_Main, mail.@object.background.value);
                }

                TB_Main.Text = mail.content;
                TB_Name.Text = mail.sender.display_name + "님으로부터";

                RA_Loading.Visibility = Visibility.Collapsed;
                PR_Loading.IsActive = false;
            });
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Close();
                e.Handled = true;
            }
        }

        private void MetroWindow_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.Source is System.Windows.Controls.TextBox) && !(e.Source is System.Windows.Controls.PasswordBox))
                e.Handled = true;
        }

        private void BT_Send_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("현재 쪽지 보내기 기능은 준비중입니다", "안내");
        }

        private async void BT_Delete_Click(object sender, RoutedEventArgs e)
        {
            await KakaoRequestClass.DeleteMail(id);
            if(MainWindow.MailWindow != null)
                MainWindow.MailWindow.Refresh();

            MessageBox.Show("쪽지 삭제가 완료되었습니다.");
            Close();
        }
    }
}
