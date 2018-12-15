//using DesktopNotifications;
using DesktopNotifications;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            catch (Exception) { }
            if (!createdNew)
            {
                if (!KSP_WPF.Properties.Settings.Default.Disable_Message)
                    KSP_WPF.GlobalHelper.ShowNotification("안내", "프로그램이 이미 실행중이므로 자동 종료되었습니다.", null);
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
