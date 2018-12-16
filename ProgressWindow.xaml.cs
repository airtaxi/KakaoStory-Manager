using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// ProgressWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgressWindow : MetroWindow
    {
        private Task deleteTask;
        private CancellationTokenSource cts;

        public ProgressWindow(List<FriendData.Profile> blinded)
        {
            InitializeComponent();
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            deleteTask = Task.Factory.StartNew(async () =>
            {
                try
                {
                    int count = 0;
                    foreach (var blindedFriend in blinded)
                    {
                        await Task.Delay(1000);
                        await KakaoRequestClass.DeleteFriend(blindedFriend.id);
                        TB_Content.Dispatcher.Invoke(() =>
                        {
                            count++;
                            TB_Content.Text = $"삭제중... {count.ToString()}/{blinded.Count}";
                            PB_Main.Value = (double) count / blinded.Count * 100.0;
                        });
                    }
                    await Dispatcher.Invoke(async() =>
                    {
                        await MainWindow.UpdateProfile();
                        MessageBox.Show("제한된 사용자의 삭제가 완료됐습니다.");
                    });
                    Close();
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("삭제 작업이 취소됐습니다.");
                }
            }, token);
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
