//using DesktopNotifications;
using DesktopNotifications;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace KSP_WPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "KakaoStroy Notification";
            _mutex = new Mutex(true, appName, out bool createdNew);
            try
            {
                DesktopNotificationManagerCompat.RegisterAumidAndComServer<KSPNotificationActivator>(KSP_WPF.MainWindow.APP_ID);
                DesktopNotificationManagerCompat.RegisterActivator<KSPNotificationActivator>();
            }
            catch (Exception){}
            if(Environment.OSVersion.Version.Major != 10)
            {
                if (KSP_WPF.Properties.Settings.Default.W10Warn == false)
                {
                    KSP_WPF.Properties.Settings.Default.W10Warn = true;
                    KSP_WPF.Properties.Settings.Default.Save();
                    MessageBox.Show("현재 버전은 윈도우 7,8 호환모드로 작동하고있습니다.\n윈도우 10을 사용중이시라면 마이크로소프트 스토어에 KakaoStory Manager를 검색하여 스토어 버전을 받아주세요.", "경고");
                }
            }
            if (!createdNew)
            {
                if (!KSP_WPF.Properties.Settings.Default.Disable_Message)
                    GlobalHelper.ShowNotification("안내", "프로그램이 이미 실행중이므로 자동 종료되었습니다.", null);
                Current.Shutdown();
            }
            else
            {
                new MainWindow().Show();
            }
            base.OnStartup(e);
        }
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("내부오류가 발생했습니다.\n" + e.Exception.Message);
            e.Handled = true;
        }
    }
}
